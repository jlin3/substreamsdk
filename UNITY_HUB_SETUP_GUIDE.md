# 🎮 Unity Hub Setup Guide - Step by Step

## Step 1: Open Unity Hub

1. **Launch Unity Hub** on your computer
2. Make sure you're signed in (top-right corner)
3. Check you have a Unity version installed (2019.4 or newer)

## Step 2: Create New Project

1. Click **"New project"** button (top-right)
2. Select Unity version (e.g., 2021.3 LTS recommended)
3. Choose template: **"3D"** (or "3D (URP)" for better graphics)
4. Project settings:
   - **Project name**: `SubstreamDemo`
   - **Location**: Choose where to save
5. Click **"Create project"**
6. Wait for Unity to open (may take 1-2 minutes)

## Step 3: Import Substream SDK

1. **Download files from GitHub**:
   - Go to: https://github.com/jlin3/substreamsdk
   - Download: `SubstreamSDK_WithProminentViewer.zip`
   - Download: `test-scene.unitypackage`

2. **In Unity**, go to menu: **Assets → Import Package → Custom Package**
3. Navigate to downloads, select `test-scene.unitypackage`
4. Click "Import" in the dialog (import all files)

5. **Extract SDK files**:
   - Unzip `SubstreamSDK_WithProminentViewer.zip` on your desktop
   - In Unity Project window, right-click on `Assets`
   - Select "Show in Explorer" (Windows) or "Reveal in Finder" (Mac)
   - Copy the extracted folders into the Assets folder:
     - `Scripts` folder
     - `Editor` folder  
     - `Plugins` folder

6. **Go back to Unity** - it will automatically import the files

## Step 4: Create Demo Scene

1. In Unity menu: **GameObject → Substream → Create Demo Scene with UI**
2. You'll see a new scene created with:
   - Canvas with streaming UI
   - Substream Demo Controller

## Step 5: Configure Build Settings

1. Go to **File → Build Settings**
2. Platform settings:
   - **For PC**: Keep "PC, Mac & Linux Standalone"
   - **For Quest**: Switch to "Android" (click Switch Platform)
3. Click **"Add Open Scenes"** to add current scene
4. Close Build Settings

## Step 6: Test Streaming

1. **Press Play** button (▶️) at top of Unity
2. In Game view, you'll see the streaming UI:
   - Title: "SUBSTREAM LIVE DEMO"
   - Green button: "🎮 START STREAMING"

3. **Click "START STREAMING"**
   - Status changes to "🔴 LIVE - You're streaming!"
   - Blue panel appears with viewer link
   - Big cyan button: "📺 OPEN VIEWER"

4. **Click "OPEN VIEWER"**
   - Browser opens to LiveKit dashboard
   - Find your room (e.g., `unity-stream-abc123`)
   - Click "Join" to watch your stream!

## Step 7: Check Console Output

Open Console window (**Window → General → Console**) to see:
```
[Substream] SDK Ready - Click Start to begin streaming!
[Substream] 🔴 LIVE - You're streaming!
╔═══════════════════════════════════════════════════════════╗
║                  🎮 STREAM IS LIVE!                       ║
╚═══════════════════════════════════════════════════════════╝
📋 Viewer link copied to clipboard!
```

## Keyboard Shortcuts (While Playing)

- **S** - Start Streaming
- **X** - Stop Streaming
- **V** - Open Viewer

## Troubleshooting

**"Substream menu not showing"**
- Make sure Editor folder is in Assets
- Right-click in Project → Reimport All

**"Scripts not working"**
- Check Console for errors (Window → General → Console)
- Make sure all scripts are in Assets/Scripts

**"Can't see stream in viewer"**
- Check internet connection
- Try refreshing the LiveKit dashboard
- Look for your specific room ID

## Next Steps

### For Quest Development:
1. Install Android Build Support in Unity Hub
2. Switch platform to Android
3. Set up Quest in Developer Mode
4. Build and deploy to Quest

### For Production:
1. Customize the UI appearance
2. Integrate into your game
3. Add your own branding

## File Structure After Setup

```
Assets/
├── Scripts/
│   ├── SimpleDemoScript.cs
│   ├── Substream.cs
│   └── DemoController.cs
├── Editor/
│   └── SubstreamMenu.cs
├── Plugins/
│   └── Android/
│       └── AndroidManifest.xml
└── [Your test scene files]
```

---

**Need Help?**
- Console shows detailed logs
- Viewer URL is auto-copied to clipboard
- Big blue button opens viewer directly

You're ready to stream! 🚀
