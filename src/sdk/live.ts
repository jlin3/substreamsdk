import { ApiClient } from './http'
import { CaptureSource, LiveStartOptions } from './types'

export class LiveSession {
  private readonly api: ApiClient
  private readonly pc: RTCPeerConnection
  private resourcePath: string | null = null

  constructor(api: ApiClient) {
    this.api = api
    this.pc = new RTCPeerConnection()
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

  async start(options: LiveStartOptions & { whipPublishUrl?: string; whipPath?: string }): Promise<void> {
    const stream = await this.getStreamFromSource(options.source)
    stream.getTracks().forEach(track => this.pc.addTrack(track, stream))

    this.pc.oniceconnectionstatechange = () => {
      // Skip ICE state changes in demo mode
      const isDemoMode = (options.whipPublishUrl ?? '').includes('localhost')
      if (isDemoMode) return
      
      options.onStatus?.({ type: 'ice', state: this.pc.iceConnectionState })
      if (this.pc.iceConnectionState === 'connected') options.onStatus?.({ type: 'connected' })
      if (this.pc.iceConnectionState === 'disconnected' || this.pc.iceConnectionState === 'failed') options.onStatus?.({ type: 'disconnected' })
    }

    const offer = await this.pc.createOffer()
    await this.pc.setLocalDescription(offer)

    const whipTarget = options.whipPublishUrl ?? options.whipPath ?? '/whip/publish'
    options.onStatus?.({ type: 'connecting' })
    
    // Check if we're in demo mode with mock WHIP
    const isDemoMode = whipTarget.includes('localhost') || whipTarget.includes('127.0.0.1')
    
    if (isDemoMode) {
      console.log('[Demo Mode] Simulating WHIP connection...')
      // For demo, just simulate a successful connection
      this.resourcePath = `/demo/stream/${Date.now()}`
      
      // Simulate connection after a short delay
      setTimeout(() => {
        options.onStatus?.({ type: 'connected' })
      }, 1000)
      
      return
    }
    
    const res = await this.api.postSdp(whipTarget, offer.sdp ?? '')
    if (!res.ok) throw new Error(`WHIP publish failed: ${res.status}`)

    const answerSdp = await res.text()
    const location = res.headers.get('Location')
    if (!location) throw new Error('Missing Location header from WHIP')

    await this.pc.setRemoteDescription({ type: 'answer', sdp: answerSdp })

    // Accept absolute or relative Resource URL. If absolute, convert to path for ApiClient usage
    try {
      const url = new URL(location)
      this.resourcePath = url.pathname + url.search
    } catch {
      this.resourcePath = location
    }
  }

  async stop(): Promise<void> {
    try {
      if (this.resourcePath) {
        await this.api.delete(this.resourcePath)
      }
    } finally {
      this.pc.getSenders().forEach(s => s.track?.stop())
      this.pc.close()
    }
  }
}


