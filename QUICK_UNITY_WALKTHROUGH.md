# 🚀 Quick Unity Setup - 5 Minutes

## 1️⃣ Unity Hub - Create Project

**In Unity Hub:**
```
┌─────────────────────────────────────┐
│ Unity Hub                           │
├─────────────────────────────────────┤
│ Projects  Installs  Learn  Community│
│                                     │
│  [New project ▼]                    │
│                                     │
│  ○ 3D                              │
│  ○ 3D (URP)  ← Choose this        │
│  ○ 2D                             │
│                                     │
│  Project name: SubstreamDemo        │
│  Location: [Choose folder]          │
│                                     │
│  [Create project]                   │
└─────────────────────────────────────┘
```

**Wait for Unity to open...**

## 2️⃣ Import Files (2 ways)

### Option A: Drag & Drop (Easiest)
1. Download from GitHub:
   - `SubstreamSDK_WithProminentViewer.zip`
   - `test-scene.unitypackage`
2. Unzip the SDK file
3. **Drag folders directly into Unity's Project window**:
   ```
   Project Window
   ├── 📁 Assets
   │   └── [Drag these here]
   │       ├── 📁 Scripts
   │       ├── 📁 Editor
   │       └── 📁 Plugins
   ```

### Option B: Import Package
1. **Assets → Import Package → Custom Package**
2. Select `test-scene.unitypackage`
3. Click Import

## 3️⃣ Create Streaming Scene

**In Unity menu bar:**
```
GameObject → Substream → Create Demo Scene with UI
           ↑
    This appears after importing!
```

**You'll see this created:**
```
Hierarchy
├── 🎥 Main Camera
├── 💡 Directional Light
├── 📺 Canvas
│   ├── Stream Control Panel
│   └── Instructions Panel
└── 🎮 Substream Demo Controller ← Selected
```

## 4️⃣ Press Play & Test

```
Unity Editor Top Bar:
[▶️ Play] [⏸️] [⏹️]   ← Click Play!
```

**In Game View:**
```
┌────────────────────────────────┐
│      SUBSTREAM LIVE DEMO       │
│                                │
│  [🎮 START STREAMING]          │
│                                │
│    Ready to stream             │
└────────────────────────────────┘
```

**Click START STREAMING:**
```
┌────────────────────────────────┐
│      SUBSTREAM LIVE DEMO       │
│                                │
│  [⏹️ STOP STREAMING]           │
│                                │
│    🔴 LIVE - Streaming!        │
│ ┌────────────────────────────┐ │
│ │  🔗 Click to View Stream   │ │
│ │    [📺 OPEN VIEWER]        │ │
│ └────────────────────────────┘ │
└────────────────────────────────┘
```

## 5️⃣ View Your Stream

1. **Click the big blue [📺 OPEN VIEWER] button**
2. Browser opens: `https://cloud.livekit.io/projects/...`
3. Find your room (e.g., `unity-stream-12345`)
4. Click **"Join"**
5. You're watching your Unity game live! 🎉

## 📝 Quick Checklist

✅ Unity Hub → New 3D Project  
✅ Import SDK files to Assets folder  
✅ GameObject menu → Create Demo Scene  
✅ Play → Start Streaming  
✅ Click Open Viewer button  
✅ Join room in browser  

## 🎯 That's It!

Total time: ~5 minutes
- 2 min: Create project
- 1 min: Import files  
- 1 min: Create scene
- 1 min: Test streaming

## ⌨️ Pro Tips

While Unity is playing:
- **S** = Start streaming
- **X** = Stop streaming  
- **V** = Open viewer

## 🔧 If Something's Wrong

**No Substream menu?**
→ Make sure Editor folder is in Assets

**Can't see stream?**
→ Check Console window (Window → General → Console)

**Need the files?**
→ https://github.com/jlin3/substreamsdk
