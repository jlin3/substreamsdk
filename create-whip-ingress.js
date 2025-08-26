#!/usr/bin/env node

/**
 * Create WHIP Ingress for LiveKit Cloud
 * 
 * This script creates a WHIP ingress endpoint for Unity streaming
 */

import { IngressClient, IngressInput } from 'livekit-server-sdk';

// Your LiveKit credentials
const LIVEKIT_HOST = 'https://substream-cnzdthyx.livekit.cloud';
const LIVEKIT_API_KEY = 'APIbtpHuQYmSvTT';
const LIVEKIT_API_SECRET = 'RmbpdbNBZVkbAW8yW0K3ekrjwcdqIKNo6OQDwt0211Y';

async function createWhipIngress() {
  try {
    // Create ingress client
    const ingressClient = new IngressClient(
      LIVEKIT_HOST,
      LIVEKIT_API_KEY,
      LIVEKIT_API_SECRET
    );

    // Create WHIP ingress
    const ingressInfo = {
      inputType: IngressInput.WHIP_INPUT,
      name: 'Unity Game Stream',
      roomName: '', // Empty = dynamic room names
      participantIdentity: 'unity-streamer',
      participantName: 'Unity Stream',
      bypassTranscoding: false,
      video: {
        source: 0,
        preset: 0, // Use preset instead of encodingOptions
      },
      audio: {
        source: 0,
        preset: 0,
      }
    };
    
    const ingress = await ingressClient.createIngress(ingressInfo);

    console.log('✅ WHIP Ingress created successfully!\n');
    console.log('🔗 WHIP URL:', ingress.url);
    console.log('🆔 Ingress ID:', ingress.ingressId);
    console.log('🏷️  Name:', ingress.name);
    console.log('📹 Stream Key:', ingress.streamKey);
    console.log('\n📝 Add this to your Unity SimpleDemoScript.cs:');
    console.log(`whipUrl = "${ingress.url}";`);
    
    return ingress;
  } catch (error) {
    console.error('❌ Failed to create WHIP ingress:', error.message);
    process.exit(1);
  }
}

// Run the script
createWhipIngress();
