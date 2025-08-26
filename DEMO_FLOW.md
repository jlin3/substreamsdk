# 🎮 Substream Unity Demo Flow

## What the Developer Will See:

### 1. Import Package
- Import `test-scene.unitypackage` (your scene)
- Import `SubstreamSDK_WithProminentViewer.zip` (SDK files)

### 2. Create Demo Scene
**GameObject → Substream → Create Demo Scene with UI**

This creates:
```
📦 Scene Hierarchy
├── 🎥 Main Camera
├── 💡 Directional Light
├── 📺 Canvas
│   ├── Stream Control Panel
│   │   ├── SUBSTREAM LIVE DEMO (title)
│   │   ├── 🎮 START STREAMING (button)
│   │   ├── ⏹️ STOP STREAMING (button)
│   │   ├── Status Text
│   │   └── Viewer Link Panel
│   │       ├── 🔗 Click to View Stream
│   │       └── 📺 OPEN VIEWER (button)
│   └── Instructions Panel
└── 🎮 Substream Demo Controller
```

### 3. Press Play & Start Streaming

**Before Streaming:**
- Green "START STREAMING" button visible
- Status: "Ready to stream"
- Viewer panel hidden

**Click START STREAMING:**
- Status: "🔄 Connecting to stream server..."
- (On Quest: "📱 Please grant screen capture permission...")
- Status: "✅ Permission granted!"
- Status: "🔴 LIVE - You're streaming!"

**Viewer Panel Appears:**
- Light blue background panel slides in
- Big cyan text: "🔗 Click to View Stream"
- Bright cyan button: "📺 OPEN VIEWER"
- URL auto-copied to clipboard

### 4. Click OPEN VIEWER

**Browser Opens:**
```
https://cloud.livekit.io/projects/substream-cnzdthyx/rooms/unity-stream-abc123
```

**In LiveKit Dashboard:**
1. See your room listed
2. Click "Join" button
3. Watch your Unity game streaming live!

### 5. Console Output

```
[Substream] SDK Ready - Click Start to begin streaming!
[Substream] Stream status: Starting
[Substream] 🔴 LIVE - You're streaming!

╔═══════════════════════════════════════════════════════════╗
║                  🎮 STREAM IS LIVE!                       ║
╠═══════════════════════════════════════════════════════════╣
║ Room ID: unity-stream-abc123                              ║
║                                                           ║
║ 👀 VIEW YOUR STREAM:                                      ║
║                                                           ║
║ 1. Click the blue 'View Stream' button in Unity          ║
║    OR                                                     ║
║ 2. Open this link in your browser:                       ║
║    https://cloud.livekit.io/.../unity-stream-abc123      ║
║                                                           ║
║ 3. Click 'Join' to watch the stream                      ║
╚═══════════════════════════════════════════════════════════╝

📋 Viewer link copied to clipboard!
```

## Visual Design:

- **Stream Control Panel**: Dark gray (90% opacity)
- **Start Button**: Green (#00FF00)
- **Stop Button**: Red (#FF4C4C)
- **Viewer Panel**: Light blue glow (#0080FF, 30% opacity)
- **Viewer Button**: Bright cyan (#00CCFF)
- **Status Text**: White, changes color based on state
- **Instructions**: Top-right corner with shortcuts

## The Result:

✅ **Clear, clickable viewer link**
✅ **Prominent visual design**
✅ **Auto-opens browser**
✅ **Works with test-scene.unitypackage**
✅ **Professional UI layout**

The viewer link is now impossible to miss! 🎯
