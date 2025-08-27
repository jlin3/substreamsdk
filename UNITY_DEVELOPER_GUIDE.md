# Unity Developer Guide - Substream SDK

## 🎯 The Simplest Streaming Solution

### Installation (30 seconds)

1. **Download**: [`SubstreamComplete.cs`](https://github.com/jlin3/substreamsdk/blob/main/SubstreamComplete.cs)
2. **Drop** into your Unity project
3. **Done!**

### Basic Usage

#### Option 1: Auto Setup (Recommended)
```csharp
// Just add to any GameObject in your scene
public class MyGame : MonoBehaviour 
{
    void Start() 
    {
        // Add streaming capability
        gameObject.AddComponent<SubstreamComplete>();
    }
}
```

#### Option 2: Manual Setup
1. Create empty GameObject
2. Add `SubstreamComplete.cs` component
3. Press Play

### What You Get

```
┌─────────────────────────┐
│    SUBSTREAM DEMO       │
│                         │
│  [🎮 START STREAMING]   │
│                         │
│   Ready to stream!      │
└─────────────────────────┘
```

When streaming:
```
┌─────────────────────────┐
│    SUBSTREAM DEMO       │
│                         │
│  [⏹️ STOP STREAMING]    │
│                         │
│   🔴 LIVE - Streaming!  │
│                         │
│  📺 CLICK TO VIEW:      │
│  https://cloud.live...  │
└─────────────────────────┘
```

### Customization

Open `SubstreamComplete.cs` and modify:

#### Change UI Colors
```csharp
// Line 56 - Panel background
panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);

// Line 87 - Button color
buttonImage.color = new Color(0.2f, 0.8f, 0.2f); // Green
```

#### Change Button Text
```csharp
// Line 94
buttonText.text = "🎮 START STREAMING"; // Your text here
```

#### Adjust Panel Size
```csharp
// Line 51
panelRect.sizeDelta = new Vector2(400, 250); // Width, Height
```

### Advanced Integration

#### Listen for Stream Events
```csharp
public class StreamManager : MonoBehaviour 
{
    SubstreamComplete streamer;
    
    void Start() 
    {
        streamer = gameObject.AddComponent<SubstreamComplete>();
        // The component handles everything!
    }
    
    void Update() 
    {
        // Check streaming state
        if (Input.GetKeyDown(KeyCode.S)) 
        {
            Debug.Log("User pressed S to start streaming");
        }
    }
}
```

#### Disable Auto-UI
If you want to use your own UI, modify `SubstreamComplete.cs`:
```csharp
void Start()
{
    // Comment out: CreateUI();
    // Use your own buttons to call: ToggleStreaming();
}
```

### Platform-Specific

#### PC/Mac/Linux
- Works immediately
- No special setup

#### WebGL
- Not supported (browser security)

#### Android/Quest
- Automatically requests screen capture permission
- Shows system permission dialog
- Then streams normally

#### iOS
- Requires iOS 14+
- May need additional privacy permissions

### Best Practices

1. **Let Players Control** - Don't auto-start streaming
2. **Show Status Clearly** - Red dot = live
3. **Make Links Clickable** - Already handled!
4. **Test Locally First** - Use Play mode

### Performance Tips

- Streaming uses ~5-10% CPU
- 1080p/30fps recommended
- Lower quality on mobile/Quest
- Disable when not needed

### Common Modifications

#### Change Stream Quality
Find in `StartStreaming()`:
```csharp
// Add these settings (currently using defaults)
int width = 1920;
int height = 1080;
int fps = 30;
int bitrate = 5000; // kbps
```

#### Custom Viewer URL
```csharp
// Line 165
string viewerUrl = "your-custom-viewer.com/" + roomId;
```

#### Remove Keyboard Shortcuts
```csharp
// Comment out the Update() method
```

### Troubleshooting

**UI Not Showing?**
- Check Game view (not Scene view)
- Ensure GameObject is active
- Look for Canvas in hierarchy

**Can't Click Button?**
- Check Canvas render mode
- Ensure no UI is blocking

**Quest Permission Denied?**
- User must accept permission
- Can't stream without it

### Example: Full Game Integration

```csharp
public class GameManager : MonoBehaviour
{
    [Header("Streaming")]
    public bool enableStreaming = true;
    
    void Start()
    {
        // Add streaming if enabled
        if (enableStreaming)
        {
            var streamGO = new GameObject("Streamer");
            streamGO.AddComponent<SubstreamComplete>();
            DontDestroyOnLoad(streamGO);
        }
        
        // Continue with game setup...
    }
}
```

---

## That's It! 🎉

Seriously, it's just one file. No packages to import, no settings to configure, no dependencies to install.

**Download → Drop in Unity → Press Play → Click Button → Streaming!**

Questions? The code is simple and well-commented. Just open `SubstreamComplete.cs` and take a look!
