# 🎮 Substream SDK - Stream Unity Games with One Line of Code

Add livestreaming to any Unity game in literally 10 seconds!

## 🚀 Quick Start (60 seconds)

### 1. Clone or Download
```bash
git clone https://github.com/jlin3/substreamsdk.git
```

### 2. Choose Your Integration

#### Option A: Simplest (One Button)
```csharp
// Add SubstreamOneClick.cs to any GameObject
// A "GO LIVE" button appears - click it!
```

#### Option B: Professional (Auto UI)
```csharp
// Add SubstreamSDK.cs to any GameObject  
// Floating UI with minimize, status, etc.
```

### 3. Press Play & Stream!
- Click "Go Live"
- Browser opens → Share your Unity window
- Anyone can watch at the generated link!

## 🥽 Meta Quest Support

Same scripts work on Quest! Just:
1. Copy `substream-release-quest.aar` to `Assets/Plugins/Android/`
2. Build & deploy to Quest
3. Player approves screen capture
4. VR view streams automatically!

## 📦 What's Included

### Essential Scripts (Use ONE of these):
- **SubstreamSDK.cs** - Full-featured SDK with automatic UI
- **SubstreamOneClick.cs** - Simplest possible - just one button
- **SubstreamUnityDemo.cs** - Interactive demo with instructions

### For Quest:
- **substream-release-quest.aar** - Native Quest streaming support
- **TestQuestAAR.cs** - Verify Quest setup

### Optional:
- **test-scene.unitypackage** - Demo scene with animated content ⚠️ **See [TEST_SCENE_INSTRUCTIONS.md](TEST_SCENE_INSTRUCTIONS.md) first!**

## 🎯 For Developers

**Integration is literally one line:**
```csharp
gameObject.AddComponent<SubstreamSDK>();
```

That's it. No configuration. No setup. It just works.

## 🌐 How Viewers Watch

1. Streamer clicks "Go Live"
2. Gets link like: `https://meet.livekit.io/custom?roomName=unity-demo-123456`
3. Anyone opens link in browser
4. They see the game live!

No apps. No downloads. Any device.

## 📺 Try It Right Now

1. Create empty Unity project
2. Add `SubstreamOneClick.cs` to any GameObject
3. Press Play
4. Click the button
5. You're streaming!

## 🛠️ Advanced Options

```csharp
// Customize the SDK
var sdk = gameObject.AddComponent<SubstreamSDK>();
sdk.quality = SubstreamSDK.Quality.Ultra;     // 4K/60fps
sdk.uiPosition = SubstreamSDK.UIPosition.TopRight;
sdk.autoStart = true;                          // Start on load
sdk.showUI = false;                            // Headless mode
```

## 📚 Documentation

- [Integration Guide](UNITY_INTEGRATION_GUIDE.md)
- [Developer & Viewer Guide](DEVELOPER_VIEWER_GUIDE.md)  
- [How It Works](HOW_IT_WORKS.md)
- [Build AAR Instructions](BUILD_AAR_ANDROID_STUDIO.md)

## 💡 Support

Issues? Questions? Create an issue on GitHub!

---

**Built with ❤️ for Unity developers who just want streaming to work.**
