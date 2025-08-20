// Mock WHIP implementation for demo purposes
// This provides a simple WHIP-like endpoint without requiring the full ingress service

export class MockWhipEndpoint {
  private static resourceCounter = 0;
  private static activeStreams = new Map<string, any>();

  /**
   * Mock WHIP publish endpoint
   * Returns a fake SDP answer and resource URL
   */
  static async publish(sdpOffer: string): Promise<{ answer: string; location: string }> {
    const resourceId = `demo-stream-${++this.resourceCounter}`;
    
    // Store the stream info
    this.activeStreams.set(resourceId, {
      offer: sdpOffer,
      startTime: Date.now(),
      roomName: `demo-room-${resourceId}`
    });

    // Generate a mock SDP answer
    const answer = this.generateMockAnswer(sdpOffer);
    
    // Return the response
    return {
      answer,
      location: `/whip/resource/${resourceId}`
    };
  }

  /**
   * Mock resource deletion
   */
  static async deleteResource(resourceId: string): Promise<void> {
    this.activeStreams.delete(resourceId);
  }

  /**
   * Generate a mock SDP answer based on the offer
   * In a real implementation, this would negotiate codecs, ICE candidates, etc.
   */
  private static generateMockAnswer(offer: string): string {
    // Extract some info from the offer for a more realistic answer
    const hasVideo = offer.includes('m=video');
    const hasAudio = offer.includes('m=audio');
    
    const audioSection = hasAudio ? `
m=audio 9 UDP/TLS/RTP/SAVPF 111
c=IN IP4 0.0.0.0
a=rtcp:9 IN IP4 0.0.0.0
a=ice-ufrag:demo
a=ice-pwd:demopassword123456789012
a=fingerprint:sha-256 00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00
a=setup:active
a=mid:0
a=recvonly
a=rtcp-mux
a=rtpmap:111 opus/48000/2` : '';

    const videoSection = hasVideo ? `
m=video 9 UDP/TLS/RTP/SAVPF 96
c=IN IP4 0.0.0.0
a=rtcp:9 IN IP4 0.0.0.0
a=ice-ufrag:demo
a=ice-pwd:demopassword123456789012
a=fingerprint:sha-256 00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00:00
a=setup:active
a=mid:1
a=recvonly
a=rtcp-mux
a=rtpmap:96 VP8/90000` : '';

    const answer = `v=0
o=- ${Date.now()} 2 IN IP4 127.0.0.1
s=-
t=0 0
a=group:BUNDLE 0 1
a=msid-semantic: WMS${audioSection}${videoSection}`;

    return answer.trim();
  }

  /**
   * Get active streams (for demo dashboard)
   */
  static getActiveStreams(): Array<{ id: string; roomName: string; duration: number }> {
    return Array.from(this.activeStreams.entries()).map(([id, info]) => ({
      id,
      roomName: info.roomName,
      duration: Date.now() - info.startTime
    }));
  }
}

// For demo mode, we can use this mock endpoint instead of a real WHIP server
export function setupMockWhipHandler() {
  if (typeof window !== 'undefined') {
    // Override fetch for WHIP endpoints in demo mode
    const originalFetch = window.fetch;
    window.fetch = async function(...args) {
      const [url, options] = args;
      
      // Intercept WHIP publish requests
      if (typeof url === 'string' && url.includes('/whip') && options?.method === 'POST') {
        console.log('[Mock WHIP] Intercepting publish request');
        const offer = await (options.body as string);
        const { answer, location } = await MockWhipEndpoint.publish(offer);
        
        return new Response(answer, {
          status: 200,
          headers: {
            'Content-Type': 'application/sdp',
            'Location': location
          }
        });
      }
      
      // Intercept WHIP delete requests
      if (typeof url === 'string' && url.includes('/whip/resource/') && options?.method === 'DELETE') {
        console.log('[Mock WHIP] Intercepting delete request');
        const resourceId = url.split('/').pop()!;
        await MockWhipEndpoint.deleteResource(resourceId);
        
        return new Response('', { status: 204 });
      }
      
      // Pass through other requests
      return originalFetch.apply(this, args as any);
    };
  }
}
