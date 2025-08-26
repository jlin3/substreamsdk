# Unity Integration Guide for SimpleDemoScript

## 🎯 Quick Setup

### 1. Copy the Script
Copy `SimpleDemoScript.cs` to your Unity project's Scripts folder.

### 2. Attach to GameObject
1. In your demo scene, create an empty GameObject (or use existing UI manager)
2. Add the `SimpleDemoScript` component to it

### 3. Connect UI Elements in Inspector

Connect these UI elements by dragging them from the Hierarchy to the script component:

- **Start Button** → Your start streaming button
- **Stop Button** → Your stop streaming button  
- **Status Text** → Text element to show current status
- **Viewer Link Text** → Text element to show viewer URL (optional)
- **Whip URL Input** → InputField for custom WHIP URL (optional)

### 4. Configure Stream Settings

In the Inspector, you can adjust:
- **Stream Width**: 1920 (default)
- **Stream Height**: 1080 (default)
- **Stream FPS**: 30 (default)
- **Bitrate Kbps**: 3500 (default)
- **Include Audio**: ✓ (checked by default)

## 🔴 How It Works

### Demo Mode (Default)
The script starts in "demo mode" which:
- Simulates streaming without needing a real server
- Shows status updates in Unity Editor
- Perfect for testing your UI

### Real Streaming Mode
To use real streaming:
1. Enter a WHIP URL in the input field
2. Or modify the `InitializeSDK()` method with your credentials

## 📱 Testing on Meta Quest

### In Unity Editor
- Press Play
- Click Start button (or press 'S' key)
- Watch status updates
- Click Stop button (or press 'X' key)

### On Quest Device
1. Build for Android
2. Install APK on Quest
3. App will request screen capture permission
4. Once granted, streaming starts automatically

## 🎨 UI Status Flow

The status text will show:
1. "Ready to stream"
2. "Requesting permission..." (Quest only)
3. "Permission granted!" (Quest only)
4. "Starting stream..."
5. "🔴 LIVE - Streaming!"
6. "Stopping stream..."
7. "Stream stopped"

## ⚠️ Common Issues

### "SDK not initialized"
- Make sure `InitializeSDK()` runs before streaming
- Check console for initialization errors

### Buttons not working
- Ensure buttons are connected in Inspector
- Check that button OnClick events are set

### No permission dialog on Quest
- Verify AndroidManifest.xml is included
- Check that you're running on actual Quest device

## 🔧 Advanced Features

### Quality Adjustment
The script includes methods for changing quality on the fly:
```csharp
// Add buttons that call these methods
SetQualityLow();    // 720p 30fps 2Mbps
SetQualityMedium(); // 1080p 30fps 3.5Mbps  
SetQualityHigh();   // 1080p 60fps 5Mbps
```

### Custom Metadata
Modify the metadata in `StartStreaming()`:
```csharp
MetadataJson = JsonUtility.ToJson(new {
    game = "Your Game Name",
    player = "Player123",
    level = 5,
    score = 1000
})
```

### Error Handling
The script logs all errors to Unity Console.
Check for:
- `[SimpleDemoScript]` prefixed messages
- `[Substream]` SDK messages

## 📺 Viewer Link

In demo mode:
- Shows link to demo viewer
- Anyone can view at: https://substreamapp.surge.sh/demo-viewer.html

With real streaming:
- Modify `ShowViewerLink()` to generate your actual viewer URLs
- Can include stream ID, authentication tokens, etc.

## 🚀 Next Steps

1. Test in Unity Editor first
2. Build and test on Quest device
3. Customize UI and branding
4. Add your own game-specific features
5. Set up real WHIP endpoint for production

## Need Help?

- Check Unity Console for detailed logs
- Errors are prefixed with `[SimpleDemoScript]`
- SDK logs are prefixed with `[Substream]`
- Status text shows current state
