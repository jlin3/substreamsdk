# Substream SDK Quick Start Guide

Stream your game with one line of code! This guide will get you streaming in under 5 minutes.

## 🚀 Demo Mode (Fastest Way to Start)

### Web Games

```bash
# 1. Install the SDK
npm install @substream/sdk

# 2. Start streaming with just 3 lines!
```

```javascript
import { Substream } from '@substream/sdk'

// Initialize in demo mode
await Substream.init('demo')

// Start streaming your canvas
const live = await Substream.startLive({
  source: { type: 'canvas', canvas: gameCanvas }
})

// That's it! Share the viewer link shown in console
```

### Unity/Quest Games

1. Import the SDK package into Unity
2. Add this script to any GameObject:

```csharp
using SubstreamSDK;

// In your Start method:
await Substream.Init(new SubstreamConfig { BaseUrl = "demo" });

// To start streaming:
var live = await Substream.QuickDemo();
await live.Start();
```

## 📺 Local Demo Setup (Optional)

Want to run everything locally? Here's a one-command setup:

```bash
# Clone and start the demo
git clone https://github.com/yourusername/substreamsdk
cd substreamsdk
npm install
./scripts/start-demo.sh

# Open http://localhost:5173/demo.html
```

## 🎮 Integration Examples

### Web Game with React

```jsx
import { useEffect, useState } from 'react'
import { Substream } from '@substream/sdk'

function StreamButton({ canvasRef }) {
  const [isStreaming, setIsStreaming] = useState(false)
  const [liveSession, setLiveSession] = useState(null)

  useEffect(() => {
    Substream.init('demo')
  }, [])

  const toggleStream = async () => {
    if (!isStreaming) {
      const session = await Substream.startLive({
        source: { 
          type: 'canvas', 
          canvas: canvasRef.current,
          fps: 60,
          withAudio: true 
        }
      })
      setLiveSession(session)
      setIsStreaming(true)
      console.log('Viewer link:', Substream.getViewerLink())
    } else {
      await liveSession?.stop()
      setIsStreaming(false)
    }
  }

  return (
    <button onClick={toggleStream}>
      {isStreaming ? '⏹ Stop Stream' : '🔴 Go Live'}
    </button>
  )
}
```

### Unity Quest VR Game

```csharp
using UnityEngine;
using SubstreamSDK;

public class VRStreamer : MonoBehaviour
{
    private LiveHandle stream;
    
    async void Start()
    {
        // Demo mode - no server required
        await Substream.Init(new SubstreamConfig { 
            BaseUrl = "demo" 
        });
    }
    
    public async void OnStreamButtonPressed()
    {
        if (stream == null)
        {
            // Use Quest-optimized settings
            stream = await Substream.LiveCreate(new LiveOptions {
                Width = 1440,
                Height = 1440,
                Fps = 72,
                VideoBitrateKbps = 4000
            });
            await stream.Start();
        }
        else
        {
            await stream.Stop();
            stream = null;
        }
    }
}
```

### Phaser.js Game

```javascript
// In your game scene
async create() {
  // Your game setup...
  
  // Add streaming
  await Substream.init('demo')
  
  // Stream the Phaser canvas
  this.input.keyboard.on('keydown-L', async () => {
    const live = await Substream.startLive({
      source: { 
        type: 'canvas', 
        canvas: this.game.canvas 
      }
    })
    console.log('Now streaming! Viewers:', Substream.getViewerLink())
  })
}
```

## 🔧 Configuration Options

### Stream Quality Presets

```javascript
// High Quality (1080p60)
{
  source: { type: 'canvas', canvas: gameCanvas, fps: 60 },
  metadata: { quality: 'high' }
}

// Balanced (720p30)
{
  source: { type: 'canvas', canvas: gameCanvas, fps: 30 },
  metadata: { quality: 'balanced' }
}

// Mobile Optimized
{
  source: { type: 'canvas', canvas: gameCanvas, fps: 30 },
  metadata: { quality: 'mobile' }
}
```

### Audio Options

```javascript
// Include microphone audio
source: { 
  type: 'canvas', 
  canvas: gameCanvas, 
  withAudio: true 
}

// Display capture with system audio
source: { 
  type: 'display',
  withAudio: true 
}
```

## 🔐 Production Setup

When you're ready to go beyond demo mode:

```javascript
// Use your own authentication
await Substream.init({
  baseUrl: 'https://api.yourgame.com',
  tokenProvider: async () => {
    const response = await fetch('/api/stream-token')
    const { token, expiresAt } = await response.json()
    return { accessToken: token, expiresAt }
  }
})
```

## 📱 Platform-Specific Notes

### Web
- Works in all modern browsers (Chrome, Firefox, Safari, Edge)
- Canvas capture requires no permissions
- Display capture will prompt for screen sharing

### Quest/Android
- Minimum Android API 29 (Android 10)
- First stream will request screen capture permission
- Streams continue in background with notification

### Unity
- Supports Unity 2020.3+
- Works with URP and Built-in pipelines
- VR optimized with hardware encoding

## 🎯 Common Use Cases

### Tournament Streaming
```javascript
const metadata = {
  event: 'Summer Tournament 2024',
  player: currentPlayer.name,
  round: matchRound
}

const live = await Substream.startLive({
  source: { type: 'canvas', canvas: gameCanvas },
  metadata
})
```

### Tutorial Recording
```javascript
// Record gameplay for tutorials
const recording = await Substream.startVod({
  source: { type: 'canvas', canvas: gameCanvas },
  chunkMs: 10000, // 10 second chunks
  metadata: { type: 'tutorial', chapter: 1 }
})

// Stop and upload when done
await recording.stopAndUpload()
```

## 🚨 Troubleshooting

### "SDK not initialized"
Make sure to call `Substream.init()` before any other methods:
```javascript
await Substream.init('demo') // or your config
```

### No video in viewer
1. Check that the stream started successfully
2. Ensure LiveKit services are running (for local setup)
3. Check browser console for errors

### Quest permission denied
The user must approve screen capture. Check for status callbacks:
```csharp
live.OnStatusChanged += (status) => {
  if (status == StreamStatus.RequestingPermission) {
    // Show UI hint to approve permission
  }
}
```

## 📚 Next Steps

- [API Reference](./API.md) - Full SDK documentation
- [Examples](./examples/) - Complete example projects
- [Server Setup](./SERVER.md) - Deploy your own infrastructure

## 💬 Need Help?

- Discord: [discord.gg/substream](https://discord.gg/substream)
- GitHub Issues: [github.com/substream/sdk](https://github.com/substream/sdk)
- Email: support@substream.dev

---

**Ready to stream?** Try the [live demo](http://localhost:5173/demo.html) right now!
