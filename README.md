# Substream SDK - Easy Unity Streaming Demo

Stream your Unity games with minimal setup!

## 🎮 Two Ways to Use

### Option 1: Simple One-File (60 seconds)
Best for: Quick testing, minimal setup, adding to existing games

### Option 2: Test Scene with Game Content (Better Demo!)
Best for: Showing streaming capabilities, testing with actual content, demos

---

## 🚀 Option 1: Quick Start (60 seconds)

### Step 1: Download
Download [`SubstreamComplete.cs`](https://github.com/jlin3/substreamsdk/blob/main/SubstreamComplete.cs)

### Step 2: Add to Unity
1. Open your Unity project
2. Drag `SubstreamComplete.cs` into your Project window (Assets folder)
3. In the Hierarchy window, right-click → Create Empty
4. Select the new GameObject
5. In the Inspector, click "Add Component"
6. Search for "Substream Complete" and add it

### Step 3: Stream!
1. Press Play ▶️
2. Click the green **START STREAMING** button
3. Click the blue viewer link that appears
4. You're live! 🎉

---

## 🎬 Option 2: Test Scene Demo (Shows Actual Game Content!)

This option includes animated content that viewers can see changing in real-time!

### Step 1: Download Both Files
1. Download [`test-scene.unitypackage`](https://github.com/jlin3/substreamsdk/blob/main/test-scene.unitypackage)
2. Download [`SubstreamComplete.cs`](https://github.com/jlin3/substreamsdk/blob/main/SubstreamComplete.cs)

### Step 2: Import Test Scene
1. Open Unity
2. Assets → Import Package → Custom Package
3. Select `test-scene.unitypackage`
4. Import All

### Step 3: Add Streaming Script
1. Drag `SubstreamComplete.cs` into your Assets folder (not SubstreamTestScene.cs)
2. Open the imported test scene
3. Select any GameObject (or create an empty one)
4. Add Component → Substream Complete

### Step 4: See It In Action!
1. Press Play ▶️
2. Watch the cube rotate and particles emit
3. Click **START STREAMING**
4. The score will start increasing (viewers see this!)
5. Click the viewer link to watch the live stream

### What Viewers Will See:
- 🎲 Rotating 3D cube
- ✨ Particle effects when streaming
- 📊 Live score updates
- 🎮 Actual game content!

## 🎮 Features

- **Zero Configuration** - Works immediately  
- **Auto UI Creation** - Creates streaming interface automatically
- **One-Click Interface** - Simple streaming controls
- **Demo Flow** - Shows how streaming would work
- **Quest Compatible** - Works on Meta Quest
- **Keyboard Shortcuts** - S to start, X to stop

## ⚠️ Current Status

This is a **UI/UX demo** showing the streaming interface. For actual video streaming, you'll need:
- Unity Render Streaming package (free)
- A signaling server (self-hosted or cloud)
- Or integration with streaming services (Twitch, YouTube)

## 📱 Quest/Android Support

Same steps! The script handles Quest permissions automatically:
1. Build for Android
2. Deploy to Quest
3. Run and click START STREAMING
4. Grant screen capture permission
5. Stream your VR gameplay!

## 🛠️ How It Works

`SubstreamComplete.cs` contains everything:
- WebRTC streaming setup
- UI creation
- LiveKit integration
- Viewer link generation
- Permission handling

No dependencies, no setup, no configuration files!

## 📺 Viewing Streams

When you start streaming:
1. A viewer link appears in Unity
2. Click it to open LiveKit dashboard
3. Click "Join" to watch
4. Share the link with others!

## 🎯 For Developers

### Integration (literally 2 lines):
```csharp
// Add to any GameObject:
gameObject.AddComponent<SubstreamComplete>();
```

### Customization:
Edit these in `SubstreamComplete.cs`:
- UI colors and layout
- Streaming quality (1080p/30fps default)
- Button text and styling

### API Usage:
```csharp
// The script handles everything internally
// Just let your players click the button!
```

## 📋 Requirements

- Unity 2019.4 or newer
- Internet connection
- That's it!

## 🆘 Troubleshooting

**Button not clicking?**
- The script now auto-creates an EventSystem
- If still not working: GameObject → UI → Event System
- Make sure you're clicking in Game view, not Scene view

**Can't see the button?**
- Make sure you're in Game view
- Check that the script is on an active GameObject

**Stream not working?**
- Check internet connection
- Try refreshing the viewer page

**Which option should I use?**
- Option 1: For quick integration into your existing game
- Option 2: To see streaming with actual game content

## 📄 License

MIT License - Use freely in your games!

---

**One file. Zero setup. Instant streaming.** 🚀

[Download SubstreamComplete.cs](https://github.com/jlin3/substreamsdk/blob/main/SubstreamComplete.cs) and start streaming in 60 seconds!