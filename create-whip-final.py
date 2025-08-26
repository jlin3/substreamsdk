#!/usr/bin/env python3

import jwt
import time
import requests
import json

# LiveKit credentials
API_KEY = 'APIbtpHuQYmSvTT'
API_SECRET = 'RmbpdbNBZVkbAW8yW0K3ekrjwcdqIKNo6OQDwt0211Y'
LIVEKIT_HOST = 'https://substream-cnzdthyx.livekit.cloud'

# Generate JWT token
payload = {
    'iss': API_KEY,
    'sub': API_KEY,
    'aud': 'substream-cnzdthyx',
    'exp': int(time.time()) + 600,
    'nbf': int(time.time()),
    'video': {
        'roomCreate': True,
        'roomList': True,
        'roomRecord': True,
        'roomAdmin': True,
        'roomJoin': True,
        'room': '*',
        'canPublish': True,
        'canSubscribe': True,
        'canPublishData': True,
        'canPublishSources': ['camera', 'microphone', 'screen_share', 'screen_share_audio'],
        'canUpdateOwnMetadata': True,
        'ingressAdmin': True
    }
}

token = jwt.encode(payload, API_SECRET, algorithm='HS256')

print("🎮 Creating WHIP Ingress for Unity Game Streaming\n")

# Create WHIP ingress (type 1 works!)
response = requests.post(
    f"{LIVEKIT_HOST}/twirp/livekit.Ingress/CreateIngress",
    headers={
        'Authorization': f'Bearer {token}',
        'Content-Type': 'application/json'
    },
    json={
        'input_type': 1,  # This is WHIP on LiveKit Cloud!
        'name': 'Unity Game Stream',
        'room_name': '',  # Empty for dynamic rooms
        'participant_identity': 'unity-streamer',
        'participant_name': 'Unity Stream'
    }
)

if response.status_code == 200:
    data = response.json()
    whip_url = data.get('url', 'N/A')
    stream_key = data.get('stream_key', 'N/A')
    ingress_id = data.get('ingress_id', 'N/A')
    
    print("✅ WHIP Ingress created successfully!\n")
    print("=" * 60)
    print(f"🔗 WHIP URL: {whip_url}")
    print(f"🔑 Stream Key: {stream_key}")
    print(f"🆔 Ingress ID: {ingress_id}")
    print("=" * 60)
    print("\n📝 Next Steps:\n")
    print("1. Open SimpleDemoScript.cs in Unity")
    print("2. Find line 102:")
    print('   whipUrl = "https://url-xxxxxxxxx.whip.livekit.cloud/w";')
    print("\n3. Replace it with:")
    print(f'   whipUrl = "{whip_url}";')
    print("\n4. Save and test streaming!")
    print("\n🎯 Full URL for streaming:")
    print(f"   {whip_url}/{stream_key}")
    
else:
    error = response.json()
    print(f"❌ Failed to create ingress: {error.get('msg', 'Unknown error')}")
    print(f"Response: {error}")
