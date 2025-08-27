# Unity Render Streaming + LiveKit Setup

## Step 1: Install Unity Render Streaming (2 minutes)

### In Unity:
1. **Window → Package Manager**
2. Click **"+"** → **"Add package from git URL"**
3. Paste: `com.unity.renderstreaming@3.1.0-exp.7`
4. Click **Add**
5. Wait for installation (it will also install WebRTC package)

## Step 2: Install Input System (Required)
1. Still in Package Manager
2. Click **"+"** → **"Add package by name"**
3. Name: `com.unity.inputsystem`
4. Version: `1.7.0`
5. Click **Add**
6. When prompted about backend, choose **"Yes"** (Unity will restart)

## Step 3: Create Streaming Setup

### Add These Components:
1. **Create Empty GameObject** → Name it "RenderStreaming"
2. **Add Component** → "Render Streaming"
3. **Create Another Empty** → Name it "Broadcast"  
4. **Add Component** → "Broadcast"
5. **Add Component** → "Camera Streamer"

## Step 4: Configure Components

### On Render Streaming:
- **URL Signaling**: `wss://substream-cnzdthyx.livekit.cloud`
- **Ice Servers**: Leave default
- **Run On Awake**: ✓ Checked

### On Camera Streamer:
- **Streaming Size**: 1280 x 720
- **Render Texture**: Create new (Assets → Create → Render Texture)
- **Camera**: Drag your Main Camera

## Step 5: The Magic Script

```csharp
using UnityEngine;
using Unity.RenderStreaming;

public class LiveKitWebRTCStreamer : MonoBehaviour
{
    private RenderStreaming renderStreaming;
    
    void Start()
    {
        renderStreaming = GetComponent<RenderStreaming>();
        if (renderStreaming != null)
        {
            renderStreaming.Run();
            Debug.Log("Unity Render Streaming started!");
        }
    }
}
```

## Step 6: View Your Stream
1. **In Browser**: Go to the WebApp sample
2. **URL**: Check Unity package samples for viewer
3. **Or use**: Custom WebRTC viewer with LiveKit

## Common Issues:

**"No video codec available"**
- Go to Project Settings → Player → WebGL → Publishing Settings
- Enable "WebRTC"

**"Connection failed"**
- Check firewall settings
- Try http://localhost instead of wss://

**"Black screen"**
- Make sure Camera is rendering to Render Texture
- Check Render Texture settings match streaming size
