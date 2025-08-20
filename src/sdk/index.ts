import { ApiClient } from './http'
import { LiveSession } from './live'
import { VodRecording } from './vod'
import { SubstreamInitOptions, LiveStartOptions, VodStartOptions } from './types'
import { DemoAuth, isDemoMode } from './demo-auth'

export interface DemoConfig {
  enabled?: boolean
  baseUrl?: string
}

class SubstreamSdk {
  private api: ApiClient | null = null
  private demoMode: boolean = false

  /**
   * Initialize the SDK with authentication
   * @param options - Configuration options or 'demo' for quick demo mode
   */
  async init(options: SubstreamInitOptions | 'demo'): Promise<void> {
    if (options === 'demo') {
      // Super simple demo mode
      this.demoMode = true
      const demoBaseUrl = 'http://localhost:8787' // Default demo server
      this.api = new ApiClient(demoBaseUrl, DemoAuth.createDemoTokenProvider())
      
      console.log('🎮 Substream SDK initialized in demo mode')
      console.log('📺 Viewer link:', DemoAuth.getViewerLink())
      console.log('💡 Note: Demo mode simulates streaming without a real server')
      return
    }
    
    this.demoMode = false
    this.api = new ApiClient(options.baseUrl, options.tokenProvider)
  }

  /**
   * Quick demo initialization for testing
   */
  async initDemo(config?: DemoConfig): Promise<void> {
    this.demoMode = true
    const baseUrl = config?.baseUrl || 'http://localhost:8787'
    this.api = new ApiClient(baseUrl, DemoAuth.createDemoTokenProvider())
    
    // Generate and display demo info
    const demoInfo = await DemoAuth.generateDemoToken()
    console.log('🎮 Substream Demo Mode Active')
    console.log('📺 Room:', demoInfo.roomName)
    console.log('🔗 Viewer:', DemoAuth.getViewerLink())
  }

  async startLive(options: LiveStartOptions & { whipPublishUrl?: string; whipPath?: string }): Promise<LiveSession> {
    if (!this.api) throw new Error('SDK not initialized. Call Substream.init() first')
    
    // Add demo mode defaults
    if (this.demoMode && !options.whipPublishUrl) {
      options.whipPublishUrl = 'http://localhost:8080/whip'
      console.log('[Demo Mode] Streaming will be simulated locally')
    }
    
    const live = new LiveSession(this.api)
    await live.start(options)
    return live
  }

  async startVod(options: VodStartOptions): Promise<VodRecording> {
    if (!this.api) throw new Error('SDK not initialized. Call Substream.init() first')
    const vod = new VodRecording(this.api)
    await vod.start(options)
    return vod
  }

  /**
   * Get viewer link for sharing (demo mode)
   */
  getViewerLink(): string {
    if (!this.demoMode) {
      console.warn('Viewer links are only available in demo mode')
      return ''
    }
    return DemoAuth.getViewerLink()
  }

  /**
   * Check if SDK is initialized
   */
  isInitialized(): boolean {
    return this.api !== null
  }
}

export const Substream = new SubstreamSdk()
export * from './types'
export { DemoAuth }


