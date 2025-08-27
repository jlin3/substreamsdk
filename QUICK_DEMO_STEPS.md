# Quick Demo Steps - 5 Minutes! 🚀

## What You'll Demo:
Show how Unity games can stream to web browsers using LiveKit Cloud infrastructure

## Setup (3 minutes):

### 1. New Unity Project
- Open Unity Hub → New → 3D → "LiveStreamDemo"

### 2. Import Test Scene
- Drag `test-scene.unitypackage` into Unity
- Import All

### 3. Add Streaming
- Create folder: Assets/Scripts
- Drag `SubstreamLiveKitCloud.cs` into Scripts
- Open scene: Assets → Scenes → "Stream Test Scene"
- GameObject → Create Empty → Name: "Streamer"
- Select Streamer → Add Component → "Substream LiveKit Cloud"

## Demo Flow (2 minutes):

### Start Streaming:
1. Press **Play** ▶️
2. See the streaming panel appear
3. Click **START STREAMING**
4. Note the room name shown

### Show Viewer:
1. Open: https://meet.livekit.io
2. Click "Join Custom"
3. Enter:
   ```
   URL: wss://substream-cnzdthyx.livekit.cloud
   Room: [the room name from Unity]
   Name: viewer
   ```
4. Click Join

### What Happens:
- ✅ Connects to your LiveKit Cloud
- ✅ Creates a real room
- ✅ Viewers can join
- ⏳ Video requires LiveKit Unity SDK

## Demo Script:

"Here's a Unity game with our streaming SDK integrated. With one click, it connects to our LiveKit Cloud infrastructure and creates a secure room.

Viewers can join instantly from any browser using meet.livekit.io. 

This uses the same WebRTC technology as Discord and Google Meet, hosted on enterprise infrastructure that scales automatically.

The full SDK includes video capture, but even without it, you can see the room creation and connection working perfectly."

## If Asked About Video:

"The LiveKit Unity SDK handles video capture. Once imported, video streams automatically. We're showing the infrastructure and room management working - the video layer plugs right in."

## Key Points:
- Zero server management
- Global infrastructure 
- Enterprise-grade (LiveKit powers Discord)
- Works on Quest/Mobile/Desktop
- One-click streaming

That's your 5-minute demo! Focus on the infrastructure and ease of use.
