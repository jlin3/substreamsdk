# 🎮 How Substream SDK Works

## For Unity Developers

### In Unity Editor (Screen Share)
```
Your Game → SubstreamSDK.cs → Opens Browser → Share Screen → LiveKit Cloud → Viewers
```

### On Meta Quest (Native Capture)
```
Your Game → SubstreamSDK.cs → AAR Library → MediaProjection → Hardware Encoder → LiveKit Cloud → Viewers
```

## The AAR Explained

The AAR (Android Archive) is a compiled Android library that:
- Accesses Quest's MediaProjection API
- Captures the VR display (both eyes)
- Encodes video using hardware acceleration
- Streams via WebRTC

Think of it like a Unity plugin - you don't see it, but it provides native functionality.

## What Happens When You Stream

### 1. Unity Editor Mode
- You add `SubstreamSDK` to any GameObject
- Click "Go Live"
- Browser opens for screen sharing
- You share your Unity Game window
- Viewers watch via LiveKit

### 2. Quest Mode (with AAR)
- You add `SubstreamSDK` to any GameObject (same script!)
- Player clicks "Go Live" in VR
- Quest shows: "Allow YourGame to record screen?"
- Player approves (one time)
- VR view streams automatically
- Viewers see the full VR experience

## File Relationships

```
Unity Project/
├── Assets/
│   ├── Plugins/
│   │   └── Android/
│   │       └── substream-release.aar  ← Native Quest functionality
│   └── Scripts/
│       └── SubstreamSDK.cs           ← Your interface to streaming
│
└── Your Game Scenes                  ← Works with ANY scene!
```

## No Special Setup Required

- ✅ Works with any Unity scene
- ✅ No special cameras needed  
- ✅ No configuration files
- ✅ Automatic platform detection
- ✅ Same script for all platforms

## The Magic

The SDK automatically detects:
- **In Editor?** → Use screen share
- **On Quest with AAR?** → Use native capture
- **On Quest without AAR?** → Fall back to screen share

You write one line of code. Players get streaming. That's it! 🚀
