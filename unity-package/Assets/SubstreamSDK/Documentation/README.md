# Substream SDK for Unity

Stream your Unity game in one line of code! This SDK provides ultra-low latency WebRTC streaming for Unity games on Web, Android, and Meta Quest platforms.

## 🚀 Quick Start

### 1. Installation

Simply import the `SubstreamSDK.unitypackage` into your Unity project. All necessary files and dependencies will be automatically set up.

### 2. Basic Usage

```csharp
using SubstreamSDK;

// Initialize the SDK
await Substream.Init(new SubstreamConfig { BaseUrl = "demo" });

// Start streaming
var live = await Substream.QuickDemo();
await live.Start();
```

### 3. Try the Demo

1. Go to menu: **Substream → Create Demo Scene**
2. Press Play in Unity Editor
3. Click the "🔴 Go Live" button
4. Your game is now streaming!

## 📱 Platform Support

- ✅ Unity Editor (Windows/Mac/Linux)
- ✅ WebGL builds
- ✅ Android (API 29+)
- ✅ Meta Quest 2/3/Pro

## 🎮 Meta Quest Setup

### Build Settings

1. Switch to Android platform
2. Set Minimum API Level to 29
3. Enable IL2CPP backend
4. Target ARM64 architecture

### Permissions

The SDK will automatically request screen recording permission on Quest. Users will see a system dialog asking to approve screen capture.

## 🔧 Advanced Configuration

```csharp
var options = new LiveOptions
{
    Width = 1920,
    Height = 1080,
    Fps = 72,  // Quest 2 native framerate
    VideoBitrateKbps = 5000,
    MetadataJson = JsonUtility.ToJson(new {
        game = "My VR Game",
        level = "Tutorial"
    })
};

var live = await Substream.LiveCreate(options);
```

## 📊 Performance Guidelines

| Platform | Recommended Settings |
|----------|---------------------|
| Quest 2 | 1440x1440 @ 72fps, 4Mbps |
| Quest 3 | 1920x1080 @ 90fps, 6Mbps |
| PC VR | 2048x2048 @ 90fps, 8Mbps |

## 🔍 Debugging

Enable debug logs:
```csharp
Substream.EnableDebugLogs = true;
```

View native logs on Quest:
```bash
adb logcat -s Unity,SubstreamSDK
```

## 📝 API Reference

### Substream.Init()
Initializes the SDK with configuration options.

### Substream.LiveCreate()
Creates a new live streaming session with specified options.

### LiveHandle.Start()
Begins streaming. On Android/Quest, this will trigger the permission dialog.

### LiveHandle.Stop()
Stops the active stream.

### Events
- `OnStatusChanged` - Fired when stream status changes
- `OnError` - Fired when an error occurs

## 🆘 Troubleshooting

### "Permission dialog not showing"
- Ensure your AndroidManifest.xml includes all required permissions
- Check that Quest OS is version 50 or higher

### "Stream not starting"
- Verify network connectivity
- Check that the streaming server is accessible
- Review debug logs for specific errors

## 📞 Support

- Documentation: https://github.com/yourusername/substreamsdk
- Issues: https://github.com/yourusername/substreamsdk/issues
- Discord: https://discord.gg/substream

## 📄 License

This SDK is provided under the MIT License. See LICENSE file for details.
