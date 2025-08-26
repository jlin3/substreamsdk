# 📺 Viewer Setup for Your LiveKit Project

## Your LiveKit Details:
- **Project**: substream-cnzdthyx
- **WebSocket URL**: wss://substream-cnzdthyx.livekit.cloud
- **API Key**: APIbtpHuQYmSvTT

## Viewing Options:

### 1. LiveKit Cloud Dashboard (Easiest)
1. Go to: https://cloud.livekit.io/projects/substream-cnzdthyx/rooms
2. Find your room (e.g., `unity-stream-abc123`)
3. Click "Join" to preview

### 2. LiveKit Meet
1. Go to: https://meet.livekit.io
2. Enter:
   - **LiveKit URL**: `wss://substream-cnzdthyx.livekit.cloud`
   - **Token**: Generate one in your dashboard
   - **Room Name**: From Unity console (e.g., `unity-stream-abc123`)

### 3. Custom Web Viewer

Create an HTML file with:

```html
<!DOCTYPE html>
<html>
<head>
    <title>Unity Stream Viewer</title>
    <script src="https://unpkg.com/livekit-client/dist/livekit-client.umd.min.js"></script>
</head>
<body>
    <h1>Unity Stream Viewer</h1>
    <div id="video-container"></div>
    <script>
        const url = 'wss://substream-cnzdthyx.livekit.cloud';
        const token = 'YOUR_VIEWER_TOKEN'; // Generate from dashboard
        const roomName = 'unity-stream-abc123'; // From Unity console
        
        async function connectToRoom() {
            const room = new LivekitClient.Room();
            
            room.on('trackSubscribed', (track, publication, participant) => {
                if (track.kind === 'video') {
                    const element = track.attach();
                    document.getElementById('video-container').appendChild(element);
                }
            });
            
            await room.connect(url, token);
            console.log('Connected to room:', roomName);
        }
        
        connectToRoom();
    </script>
</body>
</html>
```

## Generate Viewer Tokens

Use the LiveKit dashboard:
1. Go to: https://cloud.livekit.io/projects/substream-cnzdthyx/api-keys
2. Click "Generate Token"
3. Set:
   - Room: Your room name from Unity
   - Can Subscribe: ✓
   - Can Publish: ✗ (viewer only)

## Test Flow:

1. Start streaming from Unity
2. Note the room name in console
3. Use one of the viewing options above
4. You should see your Unity game!

---

**Still need**: Your WHIP ingress URL to complete the Unity setup!
