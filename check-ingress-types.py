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

print("🔍 Checking available ingress types...\n")

# Ingress types to test
ingress_types = [
    (0, "URL_INPUT (RTMP)"),
    (1, "UNKNOWN_1"),
    (2, "UNKNOWN_2"), 
    (3, "WHIP_INPUT"),
    (4, "SIP_INPUT")
]

print("Testing different input types:")
print("=" * 50)

for input_type, name in ingress_types:
    try:
        response = requests.post(
            f"{LIVEKIT_HOST}/twirp/livekit.Ingress/CreateIngress",
            headers={
                'Authorization': f'Bearer {token}',
                'Content-Type': 'application/json'
            },
            json={
                'input_type': input_type,
                'name': f'Test {name}',
                'room_name': '',
                'participant_identity': 'test-streamer',
                'participant_name': 'Test Stream'
            }
        )
        
        if response.status_code == 200:
            data = response.json()
            print(f"✅ Type {input_type} ({name}): SUCCESS")
            print(f"   URL: {data.get('url', 'N/A')}")
            print(f"   ID: {data.get('ingress_id', 'N/A')}")
            # Delete the test ingress
            requests.post(
                f"{LIVEKIT_HOST}/twirp/livekit.Ingress/DeleteIngress",
                headers={
                    'Authorization': f'Bearer {token}',
                    'Content-Type': 'application/json'
                },
                json={'ingress_id': data.get('ingress_id')}
            )
        else:
            error = response.json()
            print(f"❌ Type {input_type} ({name}): {error.get('msg', 'Failed')}")
            
    except Exception as e:
        print(f"❌ Type {input_type} ({name}): Error - {str(e)}")

print("\n" + "=" * 50)
print("\n📋 Summary:")
print("If WHIP (type 3) failed, it means:")
print("1. WHIP ingress is not enabled for your project")
print("2. You need to contact LiveKit support to enable it")
print("3. Or use an alternative method (RTMP, direct SDK, etc.)")
print("\nYour SIP ingress works because type 4 is enabled.")
