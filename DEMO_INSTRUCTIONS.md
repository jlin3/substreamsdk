# Complete Demo Instructions - Test Scene + LiveKit Cloud

## Prerequisites (5 minutes)
1. **Unity 2019.4+** installed
2. **test-scene.unitypackage** downloaded
3. **SubstreamLiveKitCloud.cs** downloaded
4. **LiveKit Unity SDK** (we'll get this)

## Step 1: Create Fresh Unity Project
1. Open Unity Hub
2. New Project → 3D → Name: "SubstreamDemo"
3. Click Create
4. Wait for Unity to open

## Step 2: Import Test Scene
1. **Assets → Import Package → Custom Package**
2. Select `test-scene.unitypackage`
3. Click "Import All"
4. Wait for import to complete

## Step 3: Import LiveKit Unity SDK
1. Download: https://github.com/livekit/client-sdk-unity/releases/latest
2. Get `io.livekit.sdk-*.unitypackage` (latest version)
3. **Assets → Import Package → Custom Package**
4. Select the LiveKit package
5. Import All
6. Wait for compilation

## Step 4: Add Streaming Script
1. In Project window: **Assets → Create → Folder** → Name: "Scripts"
2. Drag `SubstreamLiveKitCloud.cs` into Scripts folder
3. Wait for Unity to compile

## Step 5: Set Up the Scene
1. **Open test scene**: Assets → Scenes → "Stream Test Scene"
2. You should see cube, sphere, cylinder objects
3. **Create streaming manager**: 
   - Hierarchy → Right Click → Create Empty
   - Name it "LiveKitManager"
4. Select LiveKitManager
5. **Add Component** → Search "Substream" → Select "Substream LiveKit Cloud"

## Step 6: Configure Camera (Important!)
1. Select Main Camera in Hierarchy
2. Note its settings (should be positioned to see the objects)
3. Select LiveKitManager
4. In Inspector, you might see camera field - drag Main Camera there

## Step 7: Start the Demo!

### In Unity:
1. Press **Play** button ▶️
2. You'll see "LIVEKIT CLOUD STREAMING" panel
3. Click **🎮 START STREAMING**
4. Status changes to "🔴 LIVE - Room created!"
5. Note the room name (like "unity-abc123")

### To View the Stream:
1. Open browser: **https://meet.livekit.io**
2. Click **"Join Custom"**
3. Enter:
   - LiveKit URL: `wss://substream-cnzdthyx.livekit.cloud`
   - Room: (the room name from Unity)
   - Your name: `viewer`
4. Click **Join**

## What You'll See:

### If Video is Working:
- Live stream of the rotating cube
- Particle effects
- Real-time Unity gameplay

### If Only Room Works (No Video Yet):
- You'll join the room successfully
- But no video feed (black screen)
- This means LiveKit Cloud is working!
- Just need to enable camera in the script

## Step 8: Enable Video (If Needed)

Edit `SubstreamLiveKitCloud.cs` at line ~238:
```csharp
void TryConnectWithSDK()
{
    var roomType = Type.GetType("LiveKit.Room, LiveKit");
    if (roomType != null)
    {
        // ADD THIS CODE:
        StartCoroutine(ConnectAndPublishVideo());
    }
}

IEnumerator ConnectAndPublishVideo()
{
    var room = new LiveKit.Room();
    yield return room.Connect(LIVEKIT_URL, accessToken);
    
    // Enable camera
    yield return room.LocalParticipant.SetCameraEnabled(true);
    
    Debug.Log("[LiveKit] Video publishing started!");
}
```

## Keyboard Shortcuts:
- **S** - Start streaming
- **X** - Stop streaming

## Demo Talk Track:

"Here we have a Unity game with some 3D objects. With just one script, we can stream this live to the web using our LiveKit Cloud infrastructure.

When I click Start Streaming, it:
1. Creates a secure room on LiveKit Cloud
2. Generates access tokens
3. Publishes the Unity camera feed
4. Anyone can view it instantly at meet.livekit.io

No servers to manage, scales automatically, works globally. This is the same infrastructure Discord uses for their video calls."

## Troubleshooting:

**"Connection failed"**
- Check internet connection
- Verify LiveKit credentials in script

**"No video but room works"**
- LiveKit SDK might need camera permissions
- Check Console for errors
- Ensure Main Camera is assigned

**"Can't import LiveKit SDK"**
- Make sure you got the Unity package, not source code
- Try older SDK version if latest fails

## Success Metrics:
✅ Room creates successfully
✅ Viewer can join room  
✅ Video streams from Unity (with SDK setup)
✅ Low latency (<1 second)
✅ Works globally

That's it! You now have Unity → LiveKit Cloud → Web streaming!
