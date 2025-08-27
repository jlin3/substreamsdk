# Quest Streaming Quick Start (10 minutes)

## Files You Need
1. ✅ `/Users/jesselinson/substreamsdk/substream-release.aar`
2. ✅ `/Users/jesselinson/substreamsdk/quest/unity/SubstreamSDK/Substream.cs`
3. ✅ `/Users/jesselinson/substreamsdk/quest/unity/SubstreamSDK/DemoController.cs`
4. ✅ Your `test-scene.unitypackage`

## Quick Steps

### 1. Unity Setup (2 min)
```
- Open Unity project
- File → Build Settings → Android → Switch Platform
- Import test-scene.unitypackage
```

### 2. Add SDK (1 min)
```
- Create: Assets/Plugins/Android/
- Copy: substream-release.aar → Assets/Plugins/Android/
- Copy: Substream.cs & DemoController.cs → Assets/Scripts/
```

### 3. Configure (2 min)
```
Player Settings:
- Minimum API: 29
- Target API: 34
- Package: com.yourcompany.queststream
```

### 4. Scene Setup (1 min)
```
- Open "Stream Test Scene"
- GameObject → Create Empty → "Streamer"
- Add Component → Demo Controller
- Set Base URL: https://substream-cnzdthyx.api.livekit.cloud
```

### 5. Build & Run (4 min)
```
- Connect Quest via USB
- Build Settings → Build and Run
- Put on headset
- Click "Go Live"
- Approve screen capture
- STREAMING! 🎉
```

## View Your Stream
```
1. Go to: https://meet.livekit.io
2. Server: wss://substream-cnzdthyx.livekit.cloud  
3. Room: Check Unity console for room name
4. Join as viewer
```

## That's It!
Your Quest game is now streaming to the web!
