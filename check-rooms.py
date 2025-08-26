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

print("🔍 Checking LiveKit Rooms\n")

# List rooms
response = requests.post(
    f"{LIVEKIT_HOST}/twirp/livekit.RoomService/ListRooms",
    headers={
        'Authorization': f'Bearer {token}',
        'Content-Type': 'application/json'
    },
    json={}
)

if response.status_code == 200:
    data = response.json()
    rooms = data.get('rooms', [])
    
    if not rooms:
        print("❌ No active rooms found")
        print("\nThis means:")
        print("1. The Unity stream is NOT reaching LiveKit")
        print("2. There's likely an authentication or connection issue")
    else:
        print(f"✅ Found {len(rooms)} active room(s):\n")
        for room in rooms:
            print(f"Room: {room.get('name')}")
            print(f"  SID: {room.get('sid')}")
            print(f"  Participants: {room.get('num_participants', 0)}")
            print(f"  Created: {room.get('creation_time')}")
            print(f"  Max Participants: {room.get('max_participants', 0)}")
            print()
    
    print("\n" + "="*50)
    print("\n📊 Summary:")
    if not rooms:
        print("The Unity developer's stream is NOT working.")
        print("The console shows it's 'streaming' but data isn't reaching LiveKit.")
        print("\nLikely issues:")
        print("1. WebRTC connection not establishing")
        print("2. WHIP authentication failing")
        print("3. Network/firewall blocking")
    else:
        print("Streaming IS working! Rooms are active.")
        
else:
    error = response.json()
    print(f"❌ Failed to list rooms: {error.get('msg', 'Unknown error')}")

# Also check ingress status
print("\n🔍 Checking Ingress Status\n")

response = requests.post(
    f"{LIVEKIT_HOST}/twirp/livekit.Ingress/ListIngress",
    headers={
        'Authorization': f'Bearer {token}',
        'Content-Type': 'application/json'
    },
    json={}
)

if response.status_code == 200:
    data = response.json()
    items = data.get('items', [])
    
    for ingress in items:
        print(f"Ingress: {ingress.get('name')}")
        print(f"  State: {ingress.get('state', {}).get('status', 'UNKNOWN')}")
        print(f"  URL: {ingress.get('url')}")
        print(f"  Stream Key: {ingress.get('stream_key')}")
        print(f"  Room: {ingress.get('room_name', 'Dynamic')}")
        print()
