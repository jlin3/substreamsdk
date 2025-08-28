# 🎮 Substream SDK - Complete Developer & Viewer Guide

## For Developers: How to Use the SDK

### Option 1: Add to Your Existing Game (Recommended)
```bash
# 1. Clone or download
git clone https://github.com/jlin3/substreamsdk.git

# 2. Copy to your Unity project
cp substreamsdk/SubstreamSDK.cs YourGame/Assets/
cp substreamsdk/substream-release-quest.aar YourGame/Assets/Plugins/Android/

# 3. In Unity, add to any GameObject
gameObject.AddComponent<SubstreamSDK>();

# 4. Press Play and click "Go Live"!
```

### Option 2: Use the Test Scene Package
```bash
# 1. Download test-scene.unitypackage from the repo
# 2. Import into Unity (Assets → Import Package)
# 3. Open the test scene
# 4. Add SubstreamSDK component
# 5. Press Play!
```

The test scene includes:
- 🎲 Rotating cube
- ✨ Particle effects
- 📊 Score counter
- Perfect for demos!

### Option 3: Start From Scratch
Just add any of these scripts to a GameObject:
- `SubstreamSDK.cs` - Full SDK with auto UI
- `SubstreamOneClick.cs` - Single button, ultra simple
- `SubstreamUnityDemo.cs` - Interactive tutorial

## 🌐 For Viewers: How to Watch Streams

### When Someone Starts Streaming:

1. **Streamer clicks "Go Live"**
2. **Browser opens automatically** at LiveKit meet page
3. **For Unity Editor**: Streamer shares their screen
4. **For Quest**: Stream starts automatically after permission

### Viewer Access:

**Direct URL Format:**
```
https://meet.livekit.io/custom?liveKitUrl=wss%3A%2F%2Fsubstream-cnzdthyx.livekit.cloud&roomName=[ROOM-NAME]
```

**What Viewers See:**
- The game exactly as the player sees it
- In Unity Editor: The game window
- On Quest: The full VR view (both eyes)
- Real-time with <1 second latency

### Viewer Requirements:
- ✅ Any modern web browser
- ✅ No app installation
- ✅ Works on mobile/tablet/desktop
- ✅ No account needed

## 📺 Demo Flow Example

### Developer Side:
```csharp
// 1. Add component (in ANY scene!)
gameObject.AddComponent<SubstreamSDK>();

// 2. Player clicks "Go Live"
// 3. If Unity Editor: Browser opens for screen share
// 4. If Quest: Permission dialog → Auto-starts
```

### Viewer Side:
1. Receives link: `https://meet.livekit.io/custom?roomName=unity-demo-143251`
2. Opens in browser
3. Clicks "Join"
4. Watches the stream!

## 🎯 The Complete Picture

```
Your Unity Game
     ↓
SubstreamSDK (one component)
     ↓
[Unity Editor]              [Quest Device]
Screen Share        OR      MediaProjection
     ↓                           ↓
     └─────────> LiveKit Cloud <─────────┘
                      ↓
                 Web Viewers
              (meet.livekit.io)
```

## 💡 Key Points for Your Demo

1. **No Special Scene Required** - Works in ANY Unity scene
2. **Automatic Viewer Links** - Generated when streaming starts
3. **Cross-Platform Viewers** - Anyone can watch on any device
4. **LiveKit Infrastructure** - Already set up and working

## 🚀 Quick Test Right Now

Want to see it work immediately?

1. Create empty Unity project
2. Add `SubstreamOneClick.cs` 
3. Press Play
4. Click the button
5. Share your screen
6. Open viewer link in another browser tab
7. You're watching your own stream!

## 📦 Distribution Options

### For SDK Distribution:
1. **GitHub** - Developers clone the repo
2. **Unity Package** - Use PrepareUnityPackage.cs to create
3. **Asset Store** - Package for Unity Asset Store (future)

### What's Included:
- All SDK scripts
- Quest AAR (47KB)
- Documentation
- Test scene (optional)

## 🎬 The Magic Words for Demo

> "Watch this - I'll add streaming to this Unity game in literally 10 seconds."
> 
> *[Drag SubstreamSDK onto GameObject]*
> 
> "Done. Now when players click this button..."
> 
> *[Click Go Live → Share screen]*
> 
> "...anyone in the world can watch their gameplay. Here's the viewer link."
> 
> *[Show viewer watching the game]*
> 
> "Same code works on Quest - it just captures the VR view instead."

---

**Bottom Line**: Developers add one component. Players click one button. Viewers open one link. That's it! 🎮✨
