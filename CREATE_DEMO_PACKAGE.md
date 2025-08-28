# 📦 Creating the Demo Unity Package

## To Create SubstreamSDK_Demo.unitypackage:

1. **Open Unity**
2. **Create these folders:**
   ```
   Assets/
   ├── SubstreamSDK/
   │   ├── Scripts/
   │   └── Documentation/
   └── Plugins/
       └── Android/
   ```

3. **Import files:**
   - Copy `SubstreamSDK.cs` → `Assets/SubstreamSDK/Scripts/`
   - Copy `SubstreamUnityDemo.cs` → `Assets/SubstreamSDK/Scripts/`
   - Copy `SubstreamOneClick.cs` → `Assets/SubstreamSDK/Scripts/`
   - Copy `TestQuestAAR.cs` → `Assets/SubstreamSDK/Scripts/`
   - Copy `substream-release-quest.aar` → `Assets/Plugins/Android/`

4. **Add PrepareUnityPackage.cs to Unity**

5. **Run from menu: Substream → Prepare Demo Package**

6. **Package created!** Share `SubstreamSDK_Demo.unitypackage`

## What Developers Get:

When they import this package:
```
SubstreamSDK/
├── Scripts/
│   ├── SubstreamSDK.cs         (main SDK)
│   ├── SubstreamUnityDemo.cs   (interactive demo)
│   ├── SubstreamOneClick.cs    (simplest demo)
│   └── TestQuestAAR.cs         (verify Quest setup)
├── Documentation/
│   └── README.txt
└── Plugins/Android/
    └── substream-release.aar   (Quest support)
```

## Developer Instructions (included in package):

1. Import SubstreamSDK_Demo.unitypackage
2. Add any script to a GameObject:
   - `SubstreamSDK` for production use
   - `SubstreamOneClick` for quick demo
3. Press Play
4. Click "Go Live"
5. You're streaming!

## The Viewer Experience:

When streaming starts:
1. **Unity Editor**: Opens `meet.livekit.io` → Share screen
2. **Quest**: Asks permission → Streams VR view
3. **Viewers**: Get link like `https://meet.livekit.io/custom?roomName=unity-demo-123456`
4. **Anyone** can watch in browser - no install needed!

---

Or just tell developers: **"Download one file, add to GameObject, done!"** 🚀
