#!/usr/bin/env node

/**
 * Simple WHIP Ingress Creation using LiveKit API
 */

import fetch from 'node-fetch';
import jwt from 'jsonwebtoken';

// Your LiveKit credentials
const LIVEKIT_HOST = 'https://substream-cnzdthyx.livekit.cloud';
const LIVEKIT_API_KEY = 'APIbtpHuQYmSvTT';
const LIVEKIT_API_SECRET = 'RmbpdbNBZVkbAW8yW0K3ekrjwcdqIKNo6OQDwt0211Y';

// Generate JWT token for API access
function generateToken() {
  const payload = {
    iss: LIVEKIT_API_KEY,
    exp: Math.floor(Date.now() / 1000) + 600, // 10 minutes
    nbf: Math.floor(Date.now() / 1000),
    sub: LIVEKIT_API_KEY,
  };
  
  return jwt.sign(payload, LIVEKIT_API_SECRET, { algorithm: 'HS256' });
}

async function createWhipIngress() {
  try {
    const token = generateToken();
    
    const response = await fetch(`${LIVEKIT_HOST}/twirp/livekit.Ingress/CreateIngress`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        input_type: 3, // WHIP_INPUT = 3
        name: 'Unity Game Stream',
        room_name: '', // Dynamic room names
        participant_identity: 'unity-streamer',
        participant_name: 'Unity Stream',
        bypass_transcoding: false,
      })
    });

    if (!response.ok) {
      const error = await response.text();
      throw new Error(`HTTP ${response.status}: ${error}`);
    }

    const ingress = await response.json();
    
    console.log('✅ WHIP Ingress created successfully!\n');
    console.log('🔗 WHIP URL:', ingress.url);
    console.log('🆔 Ingress ID:', ingress.ingress_id);
    console.log('🏷️  Name:', ingress.name);
    console.log('🔑 Stream Key:', ingress.stream_key);
    console.log('\n📝 Add this to your Unity SimpleDemoScript.cs (line 102):');
    console.log(`whipUrl = "${ingress.url}";`);
    
    return ingress;
  } catch (error) {
    console.error('❌ Failed to create WHIP ingress:', error.message);
    console.error('\nTrying alternative approach...\n');
    
    // Alternative: Show manual instructions
    console.log('📋 Manual Setup Instructions:\n');
    console.log('1. Install LiveKit CLI:');
    console.log('   brew install livekit-cli');
    console.log('\n2. Set credentials:');
    console.log('   export LIVEKIT_URL=' + LIVEKIT_HOST.replace('https://', 'wss://'));
    console.log('   export LIVEKIT_API_KEY=' + LIVEKIT_API_KEY);
    console.log('   export LIVEKIT_API_SECRET=' + LIVEKIT_API_SECRET);
    console.log('\n3. Create WHIP ingress:');
    console.log('   livekit-cli create-ingress \\');
    console.log('     --input-type whip \\');
    console.log('     --name "Unity Game Stream" \\');
    console.log('     --participant-identity unity-streamer \\');
    console.log('     --participant-name "Unity Stream"');
    console.log('\n4. Copy the URL from the output and paste in Unity!');
  }
}

// Run the script
createWhipIngress();
