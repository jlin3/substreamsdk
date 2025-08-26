#!/usr/bin/env python3

import jwt
import time
import json

# LiveKit credentials
API_KEY = 'APIbtpHuQYmSvTT'
API_SECRET = 'RmbpdbNBZVkbAW8yW0K3ekrjwcdqIKNo6OQDwt0211Y'

# Token settings
ROOM_NAME = 'unity-demo-room'
PARTICIPANT_IDENTITY = 'unity-streamer'

# Generate token for Unity streaming
def generate_token():
    # Token grants
    video_grant = {
        'roomJoin': True,
        'room': ROOM_NAME,
        'canPublish': True,
        'canPublishData': True,
        'canSubscribe': True,
        'canPublishSources': ['camera', 'microphone', 'screen_share', 'screen_share_audio']
    }
    
    # JWT claims
    claims = {
        'exp': int(time.time()) + 86400,  # 24 hours
        'iss': API_KEY,
        'sub': PARTICIPANT_IDENTITY,
        'video': video_grant,
        'metadata': json.dumps({
            'platform': 'unity',
            'device': 'quest'
        })
    }
    
    # Generate token
    token = jwt.encode(claims, API_SECRET, algorithm='HS256')
    
    return token

if __name__ == '__main__':
    token = generate_token()
    
    print("🎮 LiveKit Unity Streaming Token\n")
    print("=" * 80)
    print(f"Token: {token}")
    print("=" * 80)
    print("\n📋 Token Details:")
    print(f"- Room: {ROOM_NAME}")
    print(f"- Identity: {PARTICIPANT_IDENTITY}")
    print("- Can Publish: ✓")
    print("- Can Subscribe: ✓")
    print("- Valid for: 24 hours")
    
    print("\n🚀 How to Use:")
    print("1. Copy the token above")
    print("2. In Unity script, replace 'YOUR_TOKEN_HERE' with this token")
    print("3. Press Play and click 'Start Streaming'")
    
    print("\n👀 View Stream:")
    print(f"https://cloud.livekit.io/projects/substream-cnzdthyx/rooms")
    print(f"Look for room: {ROOM_NAME}")
    
    print("\n✅ This token allows Unity to:")
    print("- Stream video (camera/screen)")
    print("- Stream audio (microphone)")
    print("- Connect to LiveKit Cloud")
    print("- Publish to viewers")
