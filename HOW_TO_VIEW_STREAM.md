# 👀 How to View Your Unity Stream

## 🎯 Quick Answer: LiveKit Dashboard

### Step 1: Start Streaming in Unity
1. Press Play in Unity
2. Click "Start Streaming"
3. **Look in Unity Console for room name:**
   ```
   [SimpleDemoScript] Streaming to room: unity-stream-abc12345
   ```
   📋 Copy this room name!

### Step 2: Open LiveKit Dashboard
1. Go to: https://cloud.livekit.io/projects/substream-cnzdthyx/rooms
2. You'll see your room listed:
   ```
   Room Name: unity-stream-abc12345
   Participants: 1
   Status: Active
   ```

### Step 3: Join as Viewer
1. Click on your room name
2. Click **"Join"** button
3. ✅ **You're now watching your Unity game stream!**

## 📺 What You'll See

- **In Unity Editor**: Your Game view camera output
- **From Quest**: The VR player's view (both eyes combined)
- **Quality**: 1920x1080 @ 30fps
- **Delay**: ~1-2 seconds

## 🔍 Alternative Viewing Methods

### Option 1: Direct Room Link
```
https://cloud.livekit.io/projects/substream-cnzdthyx/rooms/unity-stream-abc12345
```

### Option 2: LiveKit Meet (Public Viewer)
1. Go to: https://meet.livekit.io
2. Click "Custom"
3. Enter:
   - **LiveKit URL**: `wss://substream-cnzdthyx.livekit.cloud`
   - **Token**: Generate viewer token in dashboard
   - **Room**: `unity-stream-abc12345`

### Option 3: Quick Viewer HTML
Create `viewer.html`:
```html
<!DOCTYPE html>
<html>
<head>
    <title>Unity Stream Viewer</title>
    <script src="https://unpkg.com/livekit-client/dist/livekit-client.umd.min.js"></script>
    <style>
        body { margin: 0; background: #000; }
        video { width: 100%; height: 100vh; object-fit: contain; }
    </style>
</head>
<body>
    <div id="video-container"></div>
    <script>
        // Your room from Unity console
        const ROOM_NAME = 'unity-stream-abc12345';
        
        // Generate token at: https://cloud.livekit.io/projects/substream-cnzdthyx/settings/keys
        const TOKEN = 'YOUR_VIEWER_TOKEN';
        
        async function connect() {
            const room = new LivekitClient.Room();
            
            room.on('trackSubscribed', (track, publication, participant) => {
                if (track.kind === 'video') {
                    const element = track.attach();
                    document.getElementById('video-container').appendChild(element);
                }
            });
            
            await room.connect('wss://substream-cnzdthyx.livekit.cloud', TOKEN);
            console.log('Connected to room:', ROOM_NAME);
        }
        
        connect();
    </script>
</body>
</html>
```

## 🎮 Developer Testing Flow

```
1. Unity: Start Streaming
   ↓
2. Console: "Streaming to room: unity-stream-xyz123"
   ↓
3. Browser: https://cloud.livekit.io/projects/substream-cnzdthyx/rooms
   ↓
4. Click your room → Join
   ↓
5. ✅ Watching your game live!
```

## 🔴 Visual Indicators

**In Unity:**
- Status Text: "🔴 LIVE - Streaming!"
- Console: Room name and streaming confirmation

**In LiveKit Dashboard:**
- Room appears in list
- Shows "1 participant" (your stream)
- Status: Active

**In Viewer:**
- Video appears after ~2 seconds
- Full game view
- Real-time updates

## 💡 Pro Tips

1. **Multiple Viewers**: Share room name with teammates
2. **Mobile Testing**: Dashboard works on phones/tablets
3. **Recording**: Enable in room settings to save streams
4. **Quality**: Adjust in SimpleDemoScript (streamWidth, streamHeight, bitrateKbps)

## 🚨 Troubleshooting

**Can't see room in dashboard?**
- Make sure streaming started (check Unity console)
- Refresh dashboard page
- Check correct project: substream-cnzdthyx

**Black screen in viewer?**
- Wait 2-3 seconds for stream to start
- Check Unity Game view is visible
- Ensure camera is rendering

**No audio?**
- Set `includeAudio = true` in SimpleDemoScript
- Check Unity audio is not muted

---

**Quick Link for Developers:**
Bookmark this: https://cloud.livekit.io/projects/substream-cnzdthyx/rooms

That's your streaming dashboard! 🎉
