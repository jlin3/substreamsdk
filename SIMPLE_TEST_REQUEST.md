# Hey! Can you test the Substream SDK for me? 🎮

## What I need tested:
A Unity SDK that lets Quest games livestream to the web. It should take about 30 minutes total.

## Quick Steps:

### 1. Get the code (2 min)
```bash
git clone https://github.com/jlin3/substreamsdk.git
```

### 2. Test in Unity Editor (10 min)
- Create new Unity 3D project
- Import `StreamTestScene.unitypackage` (it's in the repo)
- Copy in `SimpleDemoScript.cs` 
- Attach script to the UI GameObject and connect the buttons
- Hit Play → Click Start → Should see "🔴 LIVE - Streaming!"

### 3. Test on Quest (15 min)
- Switch to Android platform
- Build APK
- Install on Quest: `adb install YourApp.apk`
- Run it → Click Start → **Grant screen capture permission**
- Should see streaming status

### 4. Check the viewer (2 min)
Open this in a browser: https://substreamapp.surge.sh/demo-viewer.html
(In demo mode it just shows a message, but in production it would show the stream)

## What I need to know:
- ✅ Did it work in Unity Editor?
- ✅ Did the permission dialog show up on Quest?
- ✅ Any errors or crashes?
- ✅ Your Unity version & Quest model

## If something breaks:
- Screenshot any errors
- Copy console logs
- Let me know what step failed

That's it! The whole point is to verify that:
1. The demo scene works
2. Quest asks for permission properly
3. The streaming status updates correctly

Thanks! 🙏

---
*Note: This is testing in "demo mode" - it simulates streaming without needing a real server*
