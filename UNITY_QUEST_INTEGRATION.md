# Unity Quest Streaming - Complete Integration Guide

## Prerequisites
- Unity 2020.3+ with Android Build Support
- Meta Quest 2/3 connected via USB
- `substream-release.aar` (already built ✅)
- `test-scene.unitypackage` (you have this ✅)

## Step 1: Prepare Unity Project (2 minutes)

1. **Open Unity Hub**
   - Create new 3D project OR use existing "SubstreamDemo"
   - Name: "QuestStreamingDemo"

2. **Switch to Android Platform**
   - File → Build Settings
   - Select "Android"
   - Click "Switch Platform" (wait for reimport)

3. **Import Test Scene**
   - Assets → Import Package → Custom Package
   - Select `test-scene.unitypackage`
   - Import All

## Step 2: Add Quest Streaming SDK (3 minutes)

1. **Copy AAR to Unity**
   - In Finder: Copy `substream-release.aar`
   - In Unity: Create folder `Assets/Plugins/Android/`
   - Paste `substream-release.aar` there

2. **Copy Unity Scripts**
   - In Finder: Navigate to `/Users/jesselinson/substreamsdk/quest/unity/SubstreamSDK/`
   - Copy these files to Unity `Assets/Scripts/`:
     - `Substream.cs`
     - `DemoController.cs`

## Step 3: Configure Android Settings (2 minutes)

1. **Open Player Settings**
   - File → Build Settings → Player Settings

2. **Company & Product**
   - Company Name: "YourCompany"
   - Product Name: "QuestStreamDemo"

3. **Other Settings**
   - Package Name: `com.yourcompany.queststreamDemo`
   - Minimum API Level: **29** (Android 10)
   - Target API Level: **34**
   - Configuration: **IL2CPP**
   - Target Architectures: ✅ **ARM64**

4. **Publishing Settings**
   - ✅ Custom Main Manifest (Unity will create one)

## Step 4: Set Up the Scene (3 minutes)

1. **Open Test Scene**
   - Project → Assets → Scenes → "Stream Test Scene"
   - Double-click to open

2. **Add Streaming Controller**
   ```
   - GameObject → Create Empty
   - Name it: "QuestStreaming"
   - Inspector → Add Component → "Demo Controller"
   ```

3. **Configure Demo Controller**
   In Inspector:
   - Base URL: `https://substream-cnzdthyx.api.livekit.cloud`
   - WHIP Publish URL: `https://substream-cnzdthyx.whip.livekit.cloud/w`
   - Width: **1920**
   - Height: **1080**
   - FPS: **60** (or 72 for Quest 2)
   - Bitrate: **5000** kbps

4. **Create UI (Optional)**
   If Demo Controller UI references are empty:
   ```
   - Create UI → Button → "Go Live Button"
   - Create UI → Button → "Stop Button"
   - Create UI → Text → "Status Text"
   ```
   Drag these to the Demo Controller slots

## Step 5: Build for Quest (5 minutes)

1. **Connect Quest**
   - Connect Quest via USB
   - Put Quest in Developer Mode
   - Allow USB debugging when prompted

2. **Build Settings**
   - File → Build Settings
   - Scenes in Build: ✅ "Stream Test Scene"
   - Run Device: Select your Quest

3. **Build and Run**
   - Click "Build and Run"
   - Name: "QuestStream.apk"
   - Wait for build (~3-5 minutes first time)

## Step 6: Test on Quest! (2 minutes)

1. **Put on Headset**
   - App launches automatically
   - You'll see the test scene with cube/particles

2. **Start Streaming**
   - Look at UI panel
   - Point controller at "Go Live" button
   - Pull trigger to click
   
3. **Approve Permission**
   - System dialog appears: "Allow screen recording?"
   - Select "Start now"
   - Streaming begins!

4. **View Stream**
   - On computer, go to:
   ```
   https://meet.livekit.io
   ```
   - Enter:
     - URL: `wss://substream-cnzdthyx.livekit.cloud`
     - Room: Check Unity console for room name (e.g., "unity-stream-abc123")
   - Join as viewer

## Troubleshooting

### "Go Live" button not working
- Check Demo Controller is attached to GameObject
- Ensure scene has EventSystem (GameObject → UI → Event System)

### Permission dialog doesn't appear
- Check AndroidManifest has RECORD_AUDIO permission
- Ensure minSdk is 29 or higher

### No video in viewer
- Check Unity Console for errors
- Verify room name matches
- Ensure Quest has internet connection

### Black screen in stream
- Normal for first few seconds
- Check Quest isn't in sleep mode
- Verify MediaProjection permission was granted

## Code Integration (Optional)

If you want custom integration instead of Demo Controller:

```csharp
using SubstreamSDK;
using UnityEngine;

public class MyQuestGame : MonoBehaviour
{
    private LiveHandle liveStream;
    
    async void Start()
    {
        // Initialize SDK
        await Substream.Init(new SubstreamConfig
        {
            BaseUrl = "https://substream-cnzdthyx.api.livekit.cloud",
            WhipPublishUrl = "https://substream-cnzdthyx.whip.livekit.cloud/w"
        });
    }
    
    public async void StartStreaming()
    {
        // Create stream
        liveStream = await Substream.LiveCreate(new LiveOptions
        {
            Width = 1920,
            Height = 1080,
            Fps = 72,
            VideoBitrateKbps = 5000,
            WithAudio = true
        });
        
        // Start streaming
        await liveStream.Start();
        Debug.Log("Streaming to room: " + liveStream.RoomName);
    }
    
    public async void StopStreaming()
    {
        if (liveStream != null)
        {
            await liveStream.Stop();
        }
    }
}
```

## Success Checklist

✅ AAR copied to Assets/Plugins/Android/
✅ Unity scripts in Assets/Scripts/
✅ Android settings configured (API 29+)
✅ Demo Controller added to scene
✅ Built and deployed to Quest
✅ MediaProjection permission granted
✅ Streaming live to LiveKit!

## What You're Streaming

The Quest SDK captures:
- Full stereoscopic view (both eyes)
- System UI overlays
- 1920x1080 @ 60fps
- Hardware-accelerated H.264
- Low latency via WebRTC

## Next Steps

1. **Custom UI**: Create VR-friendly streaming controls
2. **Viewer Page**: Build custom viewer instead of meet.livekit.io
3. **Analytics**: Add stream quality monitoring
4. **Recording**: Enable cloud recording on LiveKit

Congratulations! You now have Quest VR streaming working! 🎮

