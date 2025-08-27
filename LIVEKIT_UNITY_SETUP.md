# LiveKit Unity SDK Setup for Real Streaming

## Step 1: Import LiveKit Unity SDK
1. Download the latest release: https://github.com/livekit/client-sdk-unity/releases
2. Download `LiveKit-Unity-SDK-*.unitypackage`
3. Import into Unity: Assets → Import Package → Custom Package

## Step 2: Use This Simple Script

```csharp
using UnityEngine;
using LiveKit;

public class SimpleLiveKitStream : MonoBehaviour
{
    Room room;
    
    async void Start()
    {
        // Your LiveKit Cloud details
        string url = "wss://substream-cnzdthyx.livekit.cloud";
        string token = "YOUR_TOKEN_HERE"; // Generate from server
        
        room = new Room();
        await room.Connect(url, token);
        
        // Publish camera
        var videoTrack = await room.LocalParticipant.SetCameraEnabled(true);
        
        Debug.Log("Streaming to room: " + room.Name);
    }
}
```

## Step 3: Generate Access Token

Use this Python script to generate tokens:

```python
import jwt
import time

API_KEY = "APIbtpHuQYmSvTT"
API_SECRET = "RmbpdbNBZVkbAW8yW0K3ekrjwcdqIKNo6OQDwt0211Y"

def create_token(room_name, participant_name):
    payload = {
        "exp": int(time.time()) + 3600,  # 1 hour
        "iss": API_KEY,
        "sub": participant_name,
        "video": {
            "room": room_name,
            "roomJoin": True,
            "canPublish": True,
            "canSubscribe": True
        }
    }
    
    token = jwt.encode(payload, API_SECRET, algorithm='HS256')
    return token

# Example
room = "unity-test"
participant = "streamer"
token = create_token(room, participant)
print(f"Token: {token}")
print(f"Room: {room}")
```

## Step 4: View Your Stream
1. Go to https://meet.livekit.io
2. Enter your LiveKit URL: wss://substream-cnzdthyx.livekit.cloud
3. Enter the room name from your script
4. Enter any participant name
5. Click Join

## Current Issue:
The LiveKit Unity SDK examples show the connection works, but actual video capture from Unity requires additional setup with Unity's camera rendering pipeline.

## Alternative: Use OBS + WHIP
1. Stream Unity with OBS
2. Use OBS WHIP output to LiveKit
3. This works TODAY without code changes
