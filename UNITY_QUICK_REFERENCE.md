# Unity Quest Streaming - Quick Reference

## 🚀 Fastest Setup (5 minutes)

### 1. Copy Files
```bash
# Build the Android library
cd quest/android
./gradlew :substream:assembleRelease

# Copy to Unity
cp substream/build/outputs/aar/substream-release.aar /YourUnityProject/Assets/Plugins/Android/
cp -r ../unity/SubstreamSDK /YourUnityProject/Assets/Scripts/
```

### 2. Add to Scene
1. GameObject → Create Empty → Name it "Streamer"
2. Add Component → DemoController
3. Set Base URL to: `demo`

### 3. Build & Run
1. File → Build Settings → Android → Switch Platform
2. Add Open Scenes
3. Build and Run

## 📱 Player Flow

```
Start Game → Press Stream Button → Android Permission → Approve → Streaming!
                                       ↓
                              "Start recording?"
                              [Cancel] [Start now]
```

## 💻 Code Examples

### Minimal Implementation
```csharp
// Initialize once
await Substream.Init(new SubstreamConfig { BaseUrl = "demo" });

// Start streaming
var live = await Substream.QuickDemo();
await live.Start();

// Stop streaming
await live.Stop();
```

### With Status Handling
```csharp
var live = await Substream.LiveCreate(options);

live.OnStatusChanged += (status) => {
    switch(status) {
        case StreamStatus.RequestingPermission:
            ShowHint("Please approve screen capture");
            break;
        case StreamStatus.Streaming:
            ShowHint("🔴 LIVE");
            break;
    }
};

await live.Start();
```

### Custom Settings
```csharp
var options = new LiveOptions {
    Width = 1920,
    Height = 1080,
    Fps = 72,              // Quest 2
    VideoBitrateKbps = 5000
};
```

## 🔧 AndroidManifest.xml Required

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.FOREGROUND_SERVICE" />
<uses-permission android:name="android.permission.FOREGROUND_SERVICE_MEDIA_PROJECTION" />

<application>
    <service 
        android:name="com.substream.sdk.CaptureService"
        android:foregroundServiceType="mediaProjection"
        android:exported="false" />
</application>
```

## 🐛 Debug Commands

```bash
# Install APK
adb install -r YourGame.apk

# View logs
adb logcat -s Unity,SubstreamSDK

# Clear app data
adb shell pm clear com.yourcompany.game
```

## 🌐 Viewer Links

**Demo Mode:**
```
http://localhost:5173/demo-viewer.html?room=demo-[timestamp]
```

**Production:**
```
https://yourdomain.com/viewer?room=[room-id]
```

## ⚡ Performance Tips

| Quality | Resolution | FPS | Bitrate | For |
|---------|------------|-----|---------|-----|
| Low | 1280x720 | 30 | 2 Mbps | Quest 2, WiFi |
| Medium | 1440x1440 | 60 | 3.5 Mbps | Quest 2, Good WiFi |
| High | 1920x1080 | 72 | 5 Mbps | Quest 3/Pro |
| Ultra | 2048x2048 | 90 | 8 Mbps | Quest Pro, Ethernet |

## 🎮 Unity Settings Checklist

- [ ] Platform: Android
- [ ] Minimum API: 29
- [ ] Target API: 34
- [ ] IL2CPP Backend
- [ ] ARM64 Only
- [ ] XR Plugin: Oculus
- [ ] Package Name Set

## 📞 Support

- Logs: Check `adb logcat`
- Docs: See [UNITY_QUEST_TESTING.md](./UNITY_QUEST_TESTING.md)
- Demo: https://substream-demo.surge.sh/quest-demo.html
