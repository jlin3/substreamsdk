# The SIMPLEST Quest Streaming Setup (5 minutes)

## You Only Need 2 Files:

1. **substream-release.aar** (the Android plugin)
2. **QuestStreamingEasySetup.cs** (everything else in one file!)

## Steps:

### 1. New Unity Project
- Unity Hub → New → 3D → "MyQuestGame"
- File → Build Settings → Android → Switch Platform

### 2. Copy Files
```
MyQuestGame/
├── Assets/
│   ├── Plugins/
│   │   └── Android/
│   │       └── substream-release.aar  ← Copy here
│   └── Scripts/
│       └── QuestStreamingEasySetup.cs ← Copy here
```

### 3. Add to Scene
- GameObject → Create Empty → "Streamer"
- Add Component → Quest Streaming Easy Setup
- ✅ That's it!

### 4. Build & Run
- Connect Quest
- Build Settings → Build and Run
- Save as: game.apk

### 5. In Quest
- Press A button (or Space in editor)
- Approve screen recording
- Check Unity console for room name
- You're streaming!

## Even Simpler: Auto-Start

In the component settings:
- ✅ **Auto Start**: Check this
- **Auto Start Delay**: 3 seconds

Now it streams automatically when you launch!

## View Stream
1. See room name in console (e.g., "quest-143052")
2. Go to: https://meet.livekit.io
3. Enter room name
4. Watch your Quest game!

---

That's literally the entire setup! One script, one AAR, done! 🚀

