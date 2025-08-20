import { StreamTokenProvider } from './types'

// Demo authentication for easy testing
export class DemoAuth {
  private static readonly DEMO_TOKEN_KEY = 'substream_demo_token'
  private static readonly DEMO_ROOM_KEY = 'substream_demo_room'
  
  /**
   * Generate a demo token for testing
   * In production, this would be replaced with your actual auth endpoint
   */
  static async generateDemoToken(): Promise<{ token: string; roomName: string; expiresAt: number }> {
    // Check for existing valid token
    const stored = localStorage.getItem(this.DEMO_TOKEN_KEY)
    if (stored) {
      try {
        const parsed = JSON.parse(stored)
        if (parsed.expiresAt > Date.now()) {
          return parsed
        }
      } catch {}
    }

    // Generate new demo credentials
    const roomName = `demo-${Math.random().toString(36).substring(2, 8)}`
    const userId = `user-${Math.random().toString(36).substring(2, 8)}`
    
    // In a real app, this would call your backend
    // For demo, we'll use a simple JWT-like structure
    const payload = {
      sub: userId,
      room: roomName,
      iat: Math.floor(Date.now() / 1000),
      exp: Math.floor(Date.now() / 1000) + 3600, // 1 hour
      permissions: {
        publish: true,
        subscribe: true
      }
    }
    
    // Simple base64 encoding for demo (NOT secure for production!)
    const token = btoa(JSON.stringify(payload))
    const expiresAt = Date.now() + 3600000
    
    const result = { token, roomName, expiresAt }
    localStorage.setItem(this.DEMO_TOKEN_KEY, JSON.stringify(result))
    localStorage.setItem(this.DEMO_ROOM_KEY, roomName)
    
    return result
  }

  /**
   * Create a demo token provider for the SDK
   */
  static createDemoTokenProvider(): StreamTokenProvider {
    return async () => {
      const demo = await this.generateDemoToken()
      return {
        accessToken: demo.token,
        expiresAt: demo.expiresAt
      }
    }
  }

  /**
   * Get the current demo room name for generating viewer links
   */
  static getCurrentRoom(): string | null {
    return localStorage.getItem(this.DEMO_ROOM_KEY)
  }

  /**
   * Clear demo credentials
   */
  static clearDemoCredentials(): void {
    localStorage.removeItem(this.DEMO_TOKEN_KEY)
    localStorage.removeItem(this.DEMO_ROOM_KEY)
  }

  /**
   * Generate a shareable viewer link
   */
  static getViewerLink(baseUrl?: string): string {
    const room = this.getCurrentRoom()
    if (!room) return ''
    
    const url = baseUrl || window.location.origin
    return `${url}/demo-viewer.html?room=${room}&autoplay=true`
  }
}

// Helper to check if running in demo mode
export function isDemoMode(): boolean {
  return !import.meta.env.VITE_PARENTCONNECT_BASE_URL || 
         import.meta.env.VITE_PARENTCONNECT_BASE_URL.includes('localhost')
}
