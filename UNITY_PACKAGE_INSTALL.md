# Substream SDK Unity Package Installation Guide

This guide explains how to install and use the Substream SDK Unity Package.

## 📦 Package Contents

The Unity Package includes:
- **Scripts/** - Core SDK scripts (Substream.cs)
- **Demos/** - Example scene and demo controller
- **Editor/** - Unity Editor tools and menu items
- **Plugins/Android/** - Android native library and manifest
- **Documentation/** - README and API reference

## 🚀 Installation Methods

### Method 1: Import .unitypackage (Recommended)

1. Download `SubstreamSDK.unitypackage`
2. Open your Unity project
3. Go to **Assets → Import Package → Custom Package...**
4. Select the downloaded package
5. Click "Import" to import all files
6. Done! The SDK is ready to use

### Method 2: Unity Package Manager (Git URL)

1. Open **Window → Package Manager**
2. Click **+ → Add package from git URL...**
3. Enter: `https://github.com/yourusername/substreamsdk.git?path=unity-package/Assets/SubstreamSDK`
4. Click "Add"

### Method 3: Manual Installation

1. Download the SDK files
2. Copy the `SubstreamSDK` folder to your project's `Assets` folder
3. Unity will automatically import the files

## 🏃‍♂️ Quick Start

### 1. Create Demo Scene

After installation, create a demo scene:

1. Go to menu: **Substream → Create Demo Scene**
2. A new scene will be created with:
   - UI controls for streaming
   - Configured Substream Controller
   - Example rotating cube

### 2. Test in Editor

1. Press Play in Unity Editor
2. Click the "🔴 Go Live" button
3. Status will show "Demo Mode - Streaming"
4. Click "⬛ Stop Streaming" to stop

### 3. Build for Quest

1. **Switch Platform:**
   - File → Build Settings → Android
   - Click "Switch Platform"

2. **Configure Settings:**
   - Player Settings → Other Settings:
     - Package Name: `com.yourcompany.yourgame`
     - Minimum API Level: 29
     - Target API Level: 34
     - Scripting Backend: IL2CPP
     - Target Architectures: ARM64

3. **Build and Run:**
   - Connect Quest via USB
   - File → Build and Run

## 📝 Code Examples

### Basic Usage

```csharp
using SubstreamSDK;
using UnityEngine;

public class MyStreaming : MonoBehaviour
{
    async void Start()
    {
        // Initialize SDK
        await Substream.Init(new SubstreamConfig { 
            BaseUrl = "demo" 
        });
        
        // Quick demo mode
        var live = await Substream.QuickDemo();
        await live.Start();
    }
}
```

### With UI Feedback

```csharp
public class StreamingUI : MonoBehaviour
{
    public Text statusText;
    private LiveHandle liveHandle;
    
    async void Start()
    {
        await Substream.Init(new SubstreamConfig { 
            BaseUrl = "demo" 
        });
    }
    
    public async void OnGoLiveClick()
    {
        liveHandle = await Substream.LiveCreate(new LiveOptions
        {
            Width = 1920,
            Height = 1080,
            Fps = 72
        });
        
        liveHandle.OnStatusChanged += (status) => {
            statusText.text = status.ToString();
        };
        
        await liveHandle.Start();
    }
    
    public async void OnStopClick()
    {
        if (liveHandle != null)
        {
            await liveHandle.Stop();
        }
    }
}
```

## 🎮 Platform-Specific Setup

### Meta Quest

1. **Enable Developer Mode** on your Quest
2. **Android Manifest** is automatically included
3. **Permissions** will be requested at runtime
4. **USB Debugging** must be enabled

### WebGL

No special setup required! The SDK automatically uses WebRTC in browsers.

### Standalone (Windows/Mac/Linux)

Works in Unity Editor for testing. For builds, ensure you have:
- .NET Framework 4.x or .NET Standard 2.1
- IL2CPP for best performance

## 🔧 Configuration Options

### Stream Quality Presets

```csharp
// Quest 2 Optimized
new LiveOptions {
    Width = 1440,
    Height = 1440,
    Fps = 72,
    VideoBitrateKbps = 4000
}

// Quest 3/Pro High Quality
new LiveOptions {
    Width = 2048,
    Height = 2048,
    Fps = 90,
    VideoBitrateKbps = 8000
}

// Battery Saver Mode
new LiveOptions {
    Width = 1280,
    Height = 720,
    Fps = 30,
    VideoBitrateKbps = 2000
}
```

## 🐛 Troubleshooting

### "DllNotFoundException" on Android

Ensure the `substream-release.aar` file is in:
`Assets/SubstreamSDK/Plugins/Android/`

If missing, build it:
```bash
cd quest/android
gradle :substream:assembleRelease
```

### "Permission Dialog Not Showing"

1. Check AndroidManifest.xml includes all permissions
2. Verify Quest OS is version 50+
3. Try clearing app data: `adb shell pm clear com.yourcompany.game`

### "Can't Find Substream Menu"

1. Ensure all scripts are imported
2. Check for compilation errors in Console
3. Right-click in Project → Reimport All

## 📊 Performance Tips

1. **Lower resolution for better performance:**
   ```csharp
   Width = 1280, Height = 720  // Instead of 1920x1080
   ```

2. **Reduce framerate on older devices:**
   ```csharp
   Fps = 30  // Instead of 60 or 72
   ```

3. **Adjust bitrate based on network:**
   ```csharp
   VideoBitrateKbps = 2500  // Lower for WiFi issues
   ```

## 🆘 Support

- **Documentation:** See `Assets/SubstreamSDK/Documentation/`
- **Demo Scene:** Use Substream → Create Demo Scene
- **Debug Logs:** `adb logcat -s Unity,SubstreamSDK`
- **GitHub:** https://github.com/yourusername/substreamsdk

## 📄 License

This SDK is provided under the MIT License. See LICENSE.md for details.
