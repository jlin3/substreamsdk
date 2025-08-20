import { ApiClient } from './http'
import { CaptureSource, VodSessionInfo, VodStartOptions } from './types'

export class VodRecording {
  private readonly api: ApiClient
  private recorder: MediaRecorder | null = null
  private session: VodSessionInfo | null = null
  private chunkMs: number

  constructor(api: ApiClient) {
    this.api = api
    this.chunkMs = 6000
  }

  private async getStreamFromSource(source: CaptureSource): Promise<MediaStream> {
    if (source.type === 'canvas') {
      const stream = source.canvas.captureStream(source.fps ?? 30)
      if (source.withAudio) {
        const audio = await navigator.mediaDevices.getUserMedia({ audio: true, video: false })
        audio.getAudioTracks().forEach(t => stream.addTrack(t))
      }
      return stream
    }
    const display = await (navigator.mediaDevices as any).getDisplayMedia({
      video: { frameRate: source.fps ?? 30 },
      audio: source.withAudio ?? false
    })
    return display as MediaStream
  }

  async start(options: VodStartOptions): Promise<void> {
    this.chunkMs = options.chunkMs ?? 6000
    const stream = await this.getStreamFromSource(options.source)

    this.session = await this.api.postJson<{
      metadata?: Record<string, unknown>
    }, VodSessionInfo>('/vod/sessions', { metadata: options.metadata ?? {} })

    const mimeCandidates = [
      'video/mp4;codecs=h264,aac',
      'video/webm;codecs=vp9,opus',
      'video/webm;codecs=vp8,opus'
    ]
    const mimeType = mimeCandidates.find(t => MediaRecorder.isTypeSupported(t))
    this.recorder = new MediaRecorder(stream, mimeType ? { mimeType } : undefined)

    this.recorder.ondataavailable = async (ev: BlobEvent) => {
      if (!ev.data || ev.data.size === 0) return
      if (!this.session) return
      const fd = new FormData()
      const filename = `chunk-${Date.now()}`
      fd.append('file', ev.data, filename)
      await fetch(this.session.uploadUrl, { method: 'POST', body: fd })
      options.onStatus?.({ type: 'chunk-uploaded', size: ev.data.size })
    }

    this.recorder.start(this.chunkMs)
    options.onStatus?.({ type: 'recording-started' })
  }

  async stopAndUpload(): Promise<void> {
    if (!this.recorder) return
    const done = new Promise<void>((resolve) => {
      if (!this.recorder) return resolve()
      this.recorder.onstop = () => resolve()
    })
    this.recorder.stop()
    await done

    if (this.session) {
      await this.api.postJson(`/vod/sessions/${this.session.sessionId}/finalize`, {})
    }
    // Best-effort notify
    try { (this as any)._lastOptions?.onStatus?.({ type: 'finalized' }) } catch {}
  }
}


