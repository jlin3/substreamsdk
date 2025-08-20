export type StreamToken = {
  accessToken: string
  expiresAt: number // epoch ms
}

export type StreamTokenProvider = () => Promise<StreamToken>

export type CaptureSourceCanvas = {
  type: 'canvas'
  canvas: HTMLCanvasElement
  fps?: number
  withAudio?: boolean
}

export type CaptureSourceDisplay = {
  type: 'display'
  fps?: number
  withAudio?: boolean
}

export type CaptureSource = CaptureSourceCanvas | CaptureSourceDisplay

export type LiveStartOptions = {
  source: CaptureSource
  metadata?: Record<string, string | number | boolean>
  onStatus?: (s: LiveStatus) => void
  onError?: (e: Error) => void
}

export type VodStartOptions = {
  source: CaptureSource
  chunkMs?: number
  metadata?: Record<string, string | number | boolean>
  onStatus?: (s: VodStatus) => void
  onError?: (e: Error) => void
}

export type VodSessionInfo = {
  sessionId: string
  uploadUrl: string
}

export type SubstreamInitOptions = {
  tokenProvider: StreamTokenProvider
  baseUrl: string
  streamIssuer?: string
  whipPath?: string // e.g. '/whip/publish'
  whipPublishUrl?: string // absolute WHIP endpoint; overrides whipPath/baseUrl
}

export type LiveStatus =
  | { type: 'connecting' }
  | { type: 'connected' }
  | { type: 'disconnected' }
  | { type: 'ice', state: RTCPeerConnectionIceConnectionState }

export type VodStatus =
  | { type: 'recording-started' }
  | { type: 'chunk-uploaded'; size: number }
  | { type: 'finalized' }


