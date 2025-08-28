# Quest Streaming - From Zero to Streaming in 10 Minutes

## Step 1: Create New Unity Project (2 min)

1. **Open Unity Hub**
2. Click **"New Project"**
3. Select **"3D (Built-in)"**
4. Name: **"QuestStreamTest"**
5. Click **"Create Project"**
6. Wait for Unity to open

## Step 2: Switch to Android (1 min)

1. **File → Build Settings**
2. Select **"Android"**
3. Click **"Switch Platform"**
4. Wait for reimport (coffee break ☕)

## Step 3: Copy SDK Files (1 min)

**Option A: Use Our Script**
```bash
cd /Users/jesselinson/substreamsdk
./copy-to-unity.sh ~/Unity/QuestStreamTest
```

**Option B: Manual Copy**
1. In Finder, go to `/Users/jesselinson/substreamsdk`
2. Create these folders in your Unity project:
   - `QuestStreamTest/Assets/Plugins/Android/`
   - `QuestStreamTest/Assets/Scripts/`
3. Copy these 3 files:
   - `substream-release.aar` → `Assets/Plugins/Android/`
   - `quest/unity/SubstreamSDK/Substream.cs` → `Assets/Scripts/`
   - `quest/unity/SubstreamSDK/DemoController.cs` → `Assets/Scripts/`

## Step 4: Quick Android Settings (1 min)

1. **File → Build Settings → Player Settings**
2. Change only these:
   - **Company Name**: anything
   - **Product Name**: QuestStreamTest
   - **Other Settings → Minimum API Level**: Android 10.0 (API 29)
   - **Other Settings → Target API Level**: Automatic (highest)

That's it for settings!

## Step 5: Create the Streaming GameObject (1 min)

1. In Hierarchy: **Right-click → Create Empty**
2. Name it: **"Streamer"**
3. With Streamer selected: **Inspector → Add Component**
4. Type: **"Demo"** → Select **"Demo Controller"**
5. In Demo Controller settings:
   - **Base URL**: `demo` (just type "demo")
   - Leave everything else default

## Step 6: Create Basic UI (Optional - 1 min)

The Demo Controller works without UI, but to see status:

1. **GameObject → UI → Text - TextMeshPro**
2. Click "Import TMP Essentials" if prompted
3. Position the text in view
4. Drag this Text to Demo Controller → **Status Text** slot

## Step 7: Build & Deploy (3 min)

1. **Connect Quest to computer via USB**
2. **File → Build Settings**
3. **Run Device**: Select your Quest
4. Click **"Build and Run"**
5. Save as: **"quest.apk"**
6. Wait for build...

## Step 8: Stream! (1 min)

1. **Put on Quest headset**
2. App starts automatically
3. **Press spacebar** on controller (or click UI button)
4. **Approve screen recording** when prompted
5. **YOU'RE STREAMING!** 🎉

## View Your Stream

On your computer:
1. Note the room name in Unity console (e.g., "unity-abc123")
2. Go to: https://meet.livekit.io
3. Enter the room name
4. Click Join
5. See your Quest game streaming live!

---

## Super Quick Test (No UI)

Want even faster? Here's the absolute minimum:

```csharp
// Create new script: QuickStream.cs
using UnityEngine;
using SubstreamSDK;

public class QuickStream : MonoBehaviour
{
    async void Start()
    {
        await Substream.Init(new SubstreamConfig { BaseUrl = "demo" });
        var stream = await Substream.QuickDemo();
        await stream.Start();
        Debug.Log($"Streaming to room: {stream.RoomName}");
    }
}
```

1. Add this script to any GameObject
2. Build & Run
3. Approve permission
4. Check console for room name
5. Done!

---

## Troubleshooting

**"DemoController not found"**
- Make sure you copied all 3 files
- Wait for Unity to compile scripts (spinner in bottom right)

**"Build failed"**
- Check Android Build Support is installed in Unity Hub
- Verify Minimum API is 29

**"No video in stream"**
- Make sure you approved screen recording on Quest
- Check room name matches exactly

---

## That's literally it! 
No complex setup, no configuration files, just:
1. New project
2. Copy 3 files  
3. Add component
4. Build
5. Stream!

The whole process takes less than 10 minutes! 🚀

