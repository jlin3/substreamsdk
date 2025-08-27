# Unity Render Streaming - Quick Start Guide 🚀

## Step 1: Install Packages (3 minutes)

In Unity, go to **Window → Package Manager**

### Add these packages in order:

1. **WebRTC** (required first)
   - Click **"+"** → **"Add package by name"**
   - Name: `com.unity.webrtc`
   - Version: `3.0.0-pre.7`
   - Click Add

2. **Unity Render Streaming**
   - Click **"+"** → **"Add package from git URL"**
   - URL: `com.unity.renderstreaming@3.1.0-exp.7`
   - Click Add

3. **Input System** (if prompted)
   - Let Unity install it
   - Choose "Yes" when asked about backend
   - Unity will restart

## Step 2: Run the Signaling Server

Unity Render Streaming needs a signaling server. Here's the easiest way:

### Option A: Use the Built-in Server (Easiest)
1. In Unity: **Window → Render Streaming → Download Web App**
2. Extract the downloaded file
3. Run `webserver.exe` (Windows) or `webserver` (Mac/Linux)
4. Keep it running in the background

### Option B: Use Node.js Server
```bash
# In terminal
cd /path/to/unity/project
npm install -g @unity/renderstreaming-signaling-server
unity-renderstreaming-signaling-server
```

## Step 3: Add the Script

1. Create new GameObject: **"StreamingManager"**
2. Add script: **SubstreamRenderStreaming.cs**
3. In Inspector:
   - Streaming Camera: (drag Main Camera)
   - Leave other settings default

## Step 4: Test It!

1. Make sure signaling server is running
2. Press Play in Unity
3. Click **START STREAMING**
4. Note the **Connection ID** (like "unity-5678")

## Step 5: View Your Stream

### Using Unity's Web App:
1. Open browser: `http://localhost:80`
2. Enter the Connection ID from Unity
3. Click Connect
4. You'll see your Unity game streaming!

### Alternative Viewers:
- Unity package includes sample viewers
- Check: `Packages/Render Streaming/Samples`

## Troubleshooting:

**"Connection failed"**
- Is signaling server running?
- Check firewall isn't blocking port 80
- Try `http://localhost` instead of `ws://localhost:80`

**"No video"**
- Make sure Main Camera is assigned
- Check camera isn't disabled
- Try lower resolution (720p)

**"Black screen"**
- Camera might be rendering to wrong target
- Check Render Texture is created

## How It Works:

1. **Unity** captures camera → WebRTC video
2. **Signaling Server** connects Unity ↔ Browser
3. **Browser** receives and displays video
4. **P2P Connection** = Low latency!

## Next Steps:

Want to stream to LiveKit instead of local viewer?
- Use LiveKit's WHIP endpoint
- Or bridge WebRTC → LiveKit
- Or use OBS with Unity + LiveKit WHIP

## The Bottom Line:

Unity Render Streaming gives you real WebRTC video from Unity. It's production-ready and used by major studios. This is your best path for streaming Unity content! 🎮
