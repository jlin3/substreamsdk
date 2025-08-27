# Substream SDK - One-File Unity Streaming

Stream your Unity games to the web with just ONE file!

## 🚀 Quick Start (60 seconds)

### Step 1: Download
Download [`SubstreamComplete.cs`](https://github.com/jlin3/substreamsdk/blob/main/SubstreamComplete.cs)

### Step 2: Add to Unity
1. Open your Unity project
2. Drag `SubstreamComplete.cs` into your Project window
3. Add it to any GameObject (or create an empty one)

### Step 3: Stream!
1. Press Play ▶️
2. Click the green **START STREAMING** button
3. Click the blue viewer link that appears
4. You're live! 🎉

## 🎮 Features

- **Zero Configuration** - Works immediately
- **Auto UI Creation** - Creates streaming interface automatically
- **One-Click Streaming** - Just press the button
- **Clickable Viewer Links** - Opens stream in browser
- **Quest Compatible** - Works on Meta Quest
- **Keyboard Shortcuts** - S to start, X to stop

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

**Can't see the button?**
- Make sure you're in Game view
- Check that the script is on an active GameObject

**Stream not working?**
- Check internet connection
- Try refreshing the viewer page

## 📄 License

MIT License - Use freely in your games!

---

**One file. Zero setup. Instant streaming.** 🚀

[Download SubstreamComplete.cs](https://github.com/jlin3/substreamsdk/blob/main/SubstreamComplete.cs) and start streaming in 60 seconds!