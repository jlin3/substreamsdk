#!/usr/bin/env python3
import jwt
import time
import sys

# Your LiveKit credentials
API_KEY = "APIbtpHuQYmSvTT"
API_SECRET = "RmbpdbNBZVkbAW8yW0K3ekrjwcdqIKNo6OQDwt0211Y"

def create_token(room_name, participant_name):
    """Generate a LiveKit access token"""
    payload = {
        "exp": int(time.time()) + 86400,  # 24 hours
        "iss": API_KEY,
        "sub": participant_name,
        "video": {
            "room": room_name,
            "roomJoin": True,
            "canPublish": True,
            "canPublishData": True,
            "canSubscribe": True
        },
        "roomCreate": True,
        "roomList": True,
        "roomRecord": True,
        "identity": participant_name,
        "name": participant_name
    }
    
    token = jwt.encode(payload, API_SECRET, algorithm='HS256')
    return token

if __name__ == "__main__":
    # Default values
    room = "unity-demo"
    participant = "unity-streamer"
    
    # Override with command line args if provided
    if len(sys.argv) > 1:
        room = sys.argv[1]
    if len(sys.argv) > 2:
        participant = sys.argv[2]
    
    token = create_token(room, participant)
    
    print("\n=== LiveKit Access Token Generated ===")
    print(f"Room: {room}")
    print(f"Participant: {participant}")
    print(f"\nToken:\n{token}")
    print(f"\nViewer URL: https://meet.livekit.io")
    print(f"Server URL: wss://substream-cnzdthyx.livekit.cloud")
    print("\n=====================================")
