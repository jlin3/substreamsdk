# Meta Quest Unity → LiveKit Streaming Guide

## How It Actually Works

Your Quest SDK captures the entire VR view and streams it to LiveKit:

1. **Unity Game** → Renders to Quest display
2. **MediaProjection** → Captures Quest screen (both eyes)
3. **MediaCodec** → Hardware encodes to H.264
4. **WebRTC** → Packages video for streaming
5. **WHIP Protocol** → Sends to LiveKit Cloud
6. **Viewers** → Watch on any browser

## Quick Setup (15 minutes)

### 1. Build the Android Library

**Option A: Manual Build**
```bash
cd quest/android
./gradlew :substream:assembleRelease
# Output: quest/android/substream/build/outputs/aar/substream-release.aar
```

**Option B: Use Android Studio**
- Open `quest/android` in Android Studio
- Sync project
- Build → Make Module 'substream'

### 2. Unity Setup

1. **Copy to Unity:**
   - `substream-release.aar` → `Assets/Plugins/Android/`
   - `quest/unity/SubstreamSDK/` → `Assets/SubstreamSDK/`
   - `quest/unity/AndroidManifest.xml` → `Assets/Plugins/Android/` (IMPORTANT!)

2. **Configure Unity:**
   - File → Build Settings → Android
   - Player Settings:
     - Minimum API Level: 29
     - Target API Level: 34
     - Internet Access: Required
     - Package Name: com.yourcompany.yourgame

3. **Add to Scene:**
   ```
   GameObject → Create Empty → "StreamManager"
   Add Component → Substream SDK → Demo Controller
   ```

### 3. Configure LiveKit

In Demo Controller inspector:
- Base URL: `demo` (for testing)
- OR for production:
  - Base URL: `https://api.yourserver.com`
  - WHIP URL: `https://substream-cnzdthyx.whip.livekit.cloud/w`

### 4. Test on Quest

1. Build and deploy to Quest
2. Put on headset
3. Press Space or click "Go Live"
4. Approve screen capture permission
5. Stream starts automatically!

## What Gets Captured

The SDK captures the **full Quest display** including:
- Both eye views (stereoscopic)
- Your game's UI
- System overlays (if any)
- Full resolution up to 1920x1080

## Demo Scene Setup

```csharp
using UnityEngine;
using SubstreamSDK;

public class QuestStreamingDemo : MonoBehaviour
{
    async void Start()
    {
        // Quick setup for demos
        var live = await Substream.QuickDemo();
        
        // Handle events
        live.OnStatusChanged += (status) => 
        {
            Debug.Log($"Stream status: {status}");
        };
        
        // Start streaming
        await live.Start();
    }
}
```

## Viewer Experience

Viewers see your Quest game at:
```
https://meet.livekit.io
Room: [auto-generated]
```

They'll see the full VR view as you play!

## Performance Tips

**Quest 2 Optimal Settings:**
- Resolution: 1440x1440
- FPS: 72
- Bitrate: 4000 kbps

**Quest 3 High Quality:**
- Resolution: 1920x1080
- FPS: 90
- Bitrate: 6000 kbps

## Current Implementation Status

✅ **Working:**
- MediaProjection screen capture
- Hardware H.264 encoding
- WebRTC/WHIP streaming
- Unity async API
- Permission handling

⚠️ **Needs Testing:**
- LiveKit WHIP endpoint connection
- Token authentication
- Audio capture

❌ **Not Implemented Yet:**
- Custom viewer page
- Stream analytics
- Adaptive bitrate

## Troubleshooting

### Permission Dialog Not Showing?

1. **Check Logs:**
   ```bash
   adb logcat -s Unity:* SubstreamNative:*
   ```

2. **Verify AndroidManifest.xml:**
   - MUST be in `Assets/Plugins/Android/`
   - Use the provided template

3. **Test Permission:**
   - In Inspector: Right-click DemoController → Test Permission Request
   - Check headset immediately

4. **Common Fixes:**
   - Clear app data: Settings → Apps → YourApp → Clear Data
   - Reinstall APK fresh
   - Check QUEST2_DEBUG_GUIDE.txt for detailed steps

### Build Errors?

- Delete `Library/` folder
- Reimport all assets
- Restart Unity

The AAR is built and included in the repo!
