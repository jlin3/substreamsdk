# Unity Meta Quest Testing Guide

This guide walks you through testing the Substream SDK on Meta Quest with Unity, from setup to viewing the live stream.

## Prerequisites

- Unity 2021.3+ with Android Build Support
- Meta Quest 2/3/Pro with Developer Mode enabled
- Android SDK (API Level 29+)
- ADB installed and working
- Quest Link cable or Air Link configured

## Step 1: Unity Project Setup

### 1.1 Import the SDK

1. Copy the `quest/` folder into your Unity project's `Assets/Plugins/` directory:
   ```
   Assets/
   └── Plugins/
       └── Android/
           └── substream.aar  (will be built from source)
       └── SubstreamSDK/
           ├── Substream.cs
           └── DemoController.cs
   ```

2. Build the Android AAR file:
   ```bash
   cd quest/android
   ./gradlew :substream:assembleRelease
   # Copy the AAR to Unity
   cp substream/build/outputs/aar/substream-release.aar /path/to/UnityProject/Assets/Plugins/Android/
   ```

### 1.2 Configure Unity Settings

1. **Build Settings** (File → Build Settings):
   - Switch Platform to Android
   - Texture Compression: ASTC

2. **Player Settings** (Edit → Project Settings → Player):
   - **Other Settings:**
     - Package Name: `com.yourcompany.questgame`
     - Minimum API Level: 29
     - Target API Level: 34
     - Scripting Backend: IL2CPP
     - Target Architectures: ✓ ARM64
   
   - **Publishing Settings:**
     - ✓ Custom Main Manifest
     - ✓ Custom Main Gradle Template

3. **XR Plug-in Management:**
   - Install Oculus XR Plugin
   - ✓ Initialize XR on Startup
   - ✓ Oculus

### 1.3 Add Required Permissions

Create/edit `Assets/Plugins/Android/AndroidManifest.xml`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android"
    package="com.yourcompany.questgame">
    
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.FOREGROUND_SERVICE" />
    <uses-permission android:name="android.permission.FOREGROUND_SERVICE_MEDIA_PROJECTION" />
    <uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
    
    <application>
        <service 
            android:name="com.substream.sdk.CaptureService"
            android:foregroundServiceType="mediaProjection"
            android:exported="false" />
    </application>
</manifest>
```

## Step 2: Implement Streaming in Your Scene

### 2.1 Quick Setup (Using DemoController)

1. Create an empty GameObject in your scene
2. Add the `DemoController` script to it
3. Configure in Inspector:
   - Base URL: `demo` (for testing)
   - Width: 1920
   - Height: 1080
   - FPS: 72 (Quest 2) or 90 (Quest 3)
   - Bitrate: 5000

### 2.2 Custom Implementation

```csharp
using SubstreamSDK;
using UnityEngine;

public class MyQuestStreaming : MonoBehaviour
{
    private LiveHandle liveHandle;
    
    async void Start()
    {
        // Initialize SDK
        await Substream.Init(new SubstreamConfig { 
            BaseUrl = "demo" 
        });
    }
    
    public async void StartStreaming()
    {
        // Create streaming session
        liveHandle = await Substream.LiveCreate(new LiveOptions
        {
            Width = 1920,
            Height = 1080,
            Fps = 72,
            VideoBitrateKbps = 5000
        });
        
        // Listen for status updates
        liveHandle.OnStatusChanged += (status) => {
            Debug.Log($"Stream status: {status}");
            
            if (status == StreamStatus.RequestingPermission)
            {
                // Show UI hint to user
                ShowPermissionHint();
            }
        };
        
        // Start streaming
        await liveHandle.Start();
    }
    
    public async void StopStreaming()
    {
        if (liveHandle != null)
        {
            await liveHandle.Stop();
        }
    }
}
```

## Step 3: Build and Deploy to Quest

### 3.1 Build APK

1. File → Build Settings
2. Add your scene to "Scenes in Build"
3. Click "Build" and save as `QuestStreamTest.apk`

### 3.2 Install on Quest

```bash
# Make sure Quest is connected via USB
adb devices

# Install the APK
adb install -r QuestStreamTest.apk

# Grant permissions (optional, can do on device)
adb shell pm grant com.yourcompany.questgame android.permission.POST_NOTIFICATIONS
```

## Step 4: Test the Player Experience

### 4.1 Launch the App

1. Put on your Quest headset
2. Go to App Library → Unknown Sources
3. Launch your app

### 4.2 Start Streaming

1. In your app, trigger the streaming start (button press, gesture, etc.)
2. **You'll see the Android permission dialog:**
   - "Start recording or casting?"
   - Your app "will be able to record or cast your screen"
   - Tap "Start now" to approve

3. A notification will appear: "Streaming to Parent Connect"
4. Your game continues running normally while streaming

### 4.3 Generate Viewer Link

In demo mode, the viewer link is automatically:
```
http://localhost:5173/demo-viewer.html?room=demo-TIMESTAMP
```

For production, implement link generation:
```csharp
public string GetViewerLink()
{
    if (liveHandle != null && liveHandle.Status == StreamStatus.Streaming)
    {
        // In production, this would come from your backend
        string roomId = $"quest-{SystemInfo.deviceUniqueIdentifier}-{Time.time}";
        return $"https://yourdomain.com/viewer?room={roomId}";
    }
    return null;
}
```

## Step 5: View the Live Stream

### 5.1 Local Testing Setup

1. Make sure your PC and Quest are on the same network
2. Run the local demo server:
   ```bash
   cd substreamsdk
   npm install
   npm run dev
   ```

3. Find your PC's IP address:
   ```bash
   # Windows
   ipconfig
   # Mac/Linux
   ifconfig | grep inet
   ```

4. Configure Unity to use your PC's IP:
   ```csharp
   await Substream.Init(new SubstreamConfig { 
       BaseUrl = "http://YOUR_PC_IP:5173",
       WhipPublishUrl = "http://YOUR_PC_IP:5173/whip/publish"
   });
   ```

### 5.2 Production Testing

For real WebRTC streaming:

1. Set up LiveKit Cloud or self-hosted LiveKit server
2. Configure credentials:
   ```csharp
   await Substream.Init(new SubstreamConfig { 
       BaseUrl = "https://api.yourservice.com",
       WhipPublishUrl = "https://your-livekit-url/whip/publish"
   });
   ```

3. Share the viewer link with test users
4. They can watch instantly in any browser

## Step 6: Debugging Tips

### Check Logs
```bash
# View Unity logs from Quest
adb logcat -s Unity

# View native Android logs
adb logcat -s SubstreamSDK
```

### Common Issues

1. **Permission Dialog Not Showing**
   - Ensure you're on Quest OS 50+
   - Check AndroidManifest has all permissions
   - Try: `adb shell pm clear com.yourcompany.questgame`

2. **Stream Not Visible**
   - Check network connectivity
   - Verify WHIP URL is correct
   - Look for WebRTC errors in logs

3. **Performance Issues**
   - Lower resolution/framerate
   - Use hardware encoding (automatic)
   - Check Quest Performance HUD

### Test Streaming Quality

```csharp
// Add quality presets to your UI
public void SetLowQuality()
{
    // 720p30 - Good for Quest 2
    Width = 1280;
    Height = 720;
    Fps = 30;
    BitrateKbps = 2000;
}

public void SetHighQuality()
{
    // 1080p60 - Quest 3/Pro
    Width = 1920;
    Height = 1080;
    Fps = 60;
    BitrateKbps = 5000;
}

public void SetVROptimized()
{
    // Square aspect for VR
    Width = 1440;
    Height = 1440;
    Fps = 72;
    BitrateKbps = 4000;
}
```

## Complete Test Flow

1. **Developer:**
   - Build Unity project with SDK
   - Deploy to Quest
   - Configure streaming settings

2. **Player Experience:**
   - Launch VR game
   - Press "Go Live" button
   - See system permission dialog
   - Approve screen capture
   - Continue playing with streaming indicator

3. **Viewer Experience:**
   - Receive link (SMS/Discord/etc)
   - Click link on phone/PC
   - Instantly see VR gameplay
   - No app install required

## Next Steps

- Implement custom UI for streaming controls
- Add viewer link sharing (QR code, social)
- Set up production streaming infrastructure
- Add stream metadata (game state, scores)
- Implement viewer interactions

For production deployment, see [STREAMING_SETUP.md](./STREAMING_SETUP.md) for LiveKit configuration.
