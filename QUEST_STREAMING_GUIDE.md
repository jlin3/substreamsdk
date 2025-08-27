# Quest Streaming Guide - Substream SDK

## 🥽 Stream Your Quest Games in 3 Steps

### Step 1: Add to Unity Project
1. Download [`SubstreamComplete.cs`](https://github.com/jlin3/substreamsdk/blob/main/SubstreamComplete.cs)
2. Add to your Quest Unity project
3. Attach to any GameObject

### Step 2: Build for Quest
1. **File → Build Settings**
2. Switch to **Android**
3. **Player Settings**:
   - Company Name: YourCompany
   - Package Name: com.yourcompany.yourgame
   - Minimum API Level: 29 (Quest 2)
4. **Build APK**

### Step 3: Deploy & Stream
1. Connect Quest via USB
2. Install APK: `adb install YourGame.apk`
3. Launch on Quest
4. Click **START STREAMING**
5. Accept permission dialog
6. You're streaming VR! 🎉

## 📺 Viewing Quest Streams

When streaming starts:
1. The viewer URL appears in VR
2. On your computer, go to the URL
3. Click "Join" in LiveKit dashboard
4. Watch your VR gameplay!

## 🎮 Quest-Specific Features

### Automatic Permission Handling
The SDK automatically:
- Requests screen capture permission
- Shows Quest system dialog
- Starts streaming after approval

### VR UI Considerations
The streaming panel appears as:
- World-space UI in VR
- Centered in view
- Large buttons for VR controllers

### Performance Optimized
- Default: 1080p/30fps
- Recommended for Quest 2: 720p/30fps
- Minimal performance impact

## 🛠️ Customization for VR

### Optimize for Quest Performance
Edit `SubstreamComplete.cs`:
```csharp
// In StartStreaming() method, add:
int width = 1280;  // Lower resolution
int height = 720;  // for better performance
int fps = 30;
int bitrate = 3000; // kbps
```

### Position UI in VR
```csharp
// Make UI world-space for VR
canvas.renderMode = RenderMode.WorldSpace;
canvas.transform.position = new Vector3(0, 1.5f, 2); // 2m in front
canvas.transform.localScale = Vector3.one * 0.005f;
```

### Controller Support
```csharp
// Add to Update() method
if (OVRInput.GetDown(OVRInput.Button.One)) // A button
{
    ToggleStreaming();
}
```

## 📱 Quest Development Setup

### Prerequisites
- Unity 2019.4 or newer
- Android Build Support
- Quest in Developer Mode

### Enable Developer Mode
1. Oculus app on phone
2. Settings → Device → Developer Mode → ON
3. Connect Quest via USB
4. Trust computer when prompted

### Build Settings
```
Platform: Android
Texture Compression: ASTC
Target Architecture: ARM64
```

## 🚀 Quick Test Script

```csharp
using UnityEngine;
using UnityEngine.XR;

public class QuestStreamTest : MonoBehaviour
{
    void Start()
    {
        // Add streaming
        gameObject.AddComponent<SubstreamComplete>();
        
        // Ensure VR is running
        if (XRSettings.isDeviceActive)
        {
            Debug.Log("VR is active - streaming ready!");
        }
    }
}
```

## 🔧 Troubleshooting

### "Permission Denied"
- User must accept screen capture
- Cannot stream without permission
- Re-launch app to try again

### UI Not Visible in VR
- Check Canvas is world-space
- Adjust position/scale
- Ensure it's in camera view

### Low Frame Rate
- Reduce stream resolution
- Lower bitrate
- Check Quest performance

### APK Won't Install
```bash
# Check device is connected
adb devices

# Uninstall old version
adb uninstall com.yourcompany.yourgame

# Install with verbose output
adb install -r YourGame.apk
```

## 📊 Recommended Settings

| Quest Model | Resolution | FPS | Bitrate |
|-------------|------------|-----|---------|
| Quest 2     | 1280x720   | 30  | 3 Mbps  |
| Quest Pro   | 1920x1080  | 30  | 5 Mbps  |
| Quest 3     | 1920x1080  | 60  | 8 Mbps  |

## 🎯 Best Practices

1. **Test in Link First** - Use Quest Link for faster iteration
2. **Monitor Performance** - Use Oculus Metrics Tool
3. **Optimize UI Size** - Make buttons large for VR
4. **Clear Status** - Show streaming state clearly
5. **Handle Permissions** - Gracefully handle denials

## 💡 Pro Tips

### Stream Just Gameplay
Hide UI while streaming:
```csharp
if (isStreaming)
{
    hudCanvas.enabled = false; // Hide HUD
}
```

### Add Voice Chat
The stream includes Quest microphone audio automatically!

### Multiple Cameras
Stream from spectator camera instead of VR view:
```csharp
// Create separate camera for streaming
GameObject streamCam = new GameObject("StreamCamera");
streamCam.AddComponent<Camera>();
streamCam.transform.position = new Vector3(0, 2, -3);
```

---

## 🎮 Example: Complete Quest Setup

```csharp
public class QuestGameManager : MonoBehaviour
{
    void Start()
    {
        // Setup VR
        UnityEngine.XR.XRSettings.LoadDeviceByName("Oculus");
        
        // Add streaming
        GameObject streamObj = new GameObject("Streaming");
        streamObj.AddComponent<SubstreamComplete>();
        
        // Position for VR
        streamObj.transform.position = new Vector3(0, 1.5f, 2);
        
        Debug.Log("Quest streaming ready!");
    }
}
```

That's it! Your Quest game now has streaming built-in! 🚀
