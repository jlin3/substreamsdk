# 🎮 Substream SDK - Stream Your Game in One Line of Code

[![Demo](https://img.shields.io/badge/Live%20Demo-Try%20Now-blue)](http://localhost:5173/demo.html)
[![Deploy to Vercel](https://vercel.com/button)](https://vercel.com/new/clone?repository-url=https://github.com/yourusername/substreamsdk)
[![Discord](https://img.shields.io/discord/1234567890?label=Discord&logo=discord)](https://discord.gg/substream)

Add live streaming to your game in minutes! Substream SDK handles all the complexity of WebRTC, encoding, and infrastructure so you can focus on your game.

## ✨ Features

- **🚀 One-line integration** - Start streaming with just `await Substream.init('demo')`
- **🌐 Multi-platform** - Web, Unity, Quest/Android native support
- **⚡ Ultra-low latency** - Sub-second delay with WebRTC/WHIP
- **🎮 Game-optimized** - Hardware encoding, background capture, Quest VR support
- **📺 Instant viewing** - Share a link, viewers join immediately
- **🔒 Production-ready** - Authentication, metadata, analytics hooks

## 🎯 Quick Demo (30 seconds)

### Web Game
```javascript
import { Substream } from '@substream/sdk'

// That's it! Your canvas is now streamable
await Substream.init('demo')
const live = await Substream.startLive({
  source: { type: 'canvas', canvas: gameCanvas }
})
```

### Unity/Quest Game
```csharp
using SubstreamSDK;

// Add to any GameObject
await Substream.Init(new SubstreamConfig { BaseUrl = "demo" });
var live = await Substream.QuickDemo();
await live.Start();
```

## 🏃‍♂️ Try It Now

```bash
# Clone and run the demo
git clone https://github.com/yourusername/substreamsdk
cd substreamsdk
npm install
npm run demo

# Open http://localhost:5173/demo.html
```

Or try the [live demo](https://substream-demo.vercel.app) (no installation needed!)

## 📖 Documentation

- **[Quick Start Guide](./QUICKSTART.md)** - Get streaming in 5 minutes
- **[Unity Package Install](./UNITY_PACKAGE_INSTALL.md)** - Unity .unitypackage installation
- **[Unity Quest Testing](./UNITY_QUEST_TESTING.md)** - Complete Quest/VR testing guide
- **[Unity on Meta Quest Guide](./quest/README.md)** - Full setup and integration for Quest
- **[API Reference](./API.md)** - Full SDK documentation
- **[Deployment Guide](./DEPLOY.md)** - Production deployment options
- **[Examples](./examples/)** - Sample integrations for popular game engines

## 🏗️ Architecture

```
┌─────────────┐     WebRTC/WHIP      ┌─────────────┐
│   Your Game │ ──────────────────> │  LiveKit    │
│   (Canvas)  │                      │   Server    │
└─────────────┘                      └──────┬──────┘
                                           │
┌─────────────┐                            │
│   Viewers   │ <──────────────────────────┘
│  (Browser)  │         WebRTC
└─────────────┘
```

## 🛠️ SDK Components

### Web SDK (`src/sdk/`)
- **Pure TypeScript** - No framework dependencies
- **Modular design** - Tree-shakeable, ~15KB gzipped
- **Demo mode** - No server required for testing

### Android/Quest Native (`quest/android/`)
- **MediaProjection API** - System-level screen capture
- **Hardware encoding** - H.264 via MediaCodec
- **Background streaming** - Continues with notification

### Unity Integration (`quest/unity/`)
- **Async/await API** - Modern C# patterns
- **Editor testing** - Works in Unity Editor
- **VR optimized** - Quest 2/3/Pro specific settings

## 🎮 Supported Platforms

| Platform | Capture Method | Min Version | Status |
|----------|---------------|-------------|---------|
| Chrome/Edge | Canvas/Display | 90+ | ✅ Stable |
| Firefox | Canvas/Display | 88+ | ✅ Stable |
| Safari | Canvas only | 15+ | ✅ Stable |
| Quest Browser | MediaProjection | Quest OS 50+ | ✅ Stable |
| Unity | Platform-specific | 2020.3+ | ✅ Stable |
| Android | MediaProjection | API 29+ | ✅ Stable |
| iOS | - | - | 🚧 Coming Soon |

## 🔧 Configuration Options

### Stream Quality Presets
```javascript
// Competitive (1080p60, 5Mbps)
await Substream.startLive({
  source: { type: 'canvas', canvas: gameCanvas, fps: 60 },
  quality: 'competitive'
})

// Balanced (720p30, 2.5Mbps)  
await Substream.startLive({
  source: { type: 'canvas', canvas: gameCanvas, fps: 30 },
  quality: 'balanced'
})

// Mobile (480p30, 1Mbps)
await Substream.startLive({
  source: { type: 'canvas', canvas: gameCanvas, fps: 30 },
  quality: 'mobile'
})
```

### Advanced Options
```javascript
const live = await Substream.startLive({
  source: { 
    type: 'canvas', 
    canvas: gameCanvas,
    fps: 60,
    withAudio: true  // Include microphone
  },
  metadata: {
    gameId: 'my-awesome-game',
    userId: currentUser.id,
    level: currentLevel,
    score: playerScore
  },
  onStatus: (status) => {
    console.log('Stream status:', status)
  },
  onError: (error) => {
    console.error('Stream error:', error)
  }
})
```

## 🚀 Production Deployment

### Option 1: Managed Infrastructure
Use [LiveKit Cloud](https://livekit.cloud) for a fully managed solution:
- Global edge network
- Automatic scaling
- 99.95% uptime SLA

### Option 2: Self-Hosted
Deploy your own infrastructure:
```bash
# Using our Docker Compose setup
docker compose -f docker-compose.prod.yml up -d

# Or deploy to Kubernetes
kubectl apply -f deploy/k8s/
```

### Option 3: Hybrid
Use managed WHIP ingress with your own application servers for maximum flexibility.

## 📊 Performance

| Metric | Web | Quest Native |
|--------|-----|--------------|
| Latency | <500ms | <300ms |
| CPU Usage | 5-10% | 3-5% |
| Memory | ~50MB | ~30MB |
| Bandwidth | Adaptive | Adaptive |

## 🤝 Contributing

We welcome contributions! See [CONTRIBUTING.md](./CONTRIBUTING.md) for guidelines.

### Development Setup
```bash
# Install dependencies
npm install

# Run tests
npm test

# Build all packages
npm run build

# Start dev server
npm run dev
```

## 📄 License

MIT License - see [LICENSE](./LICENSE)

## 🙏 Acknowledgments

Built with:
- [LiveKit](https://livekit.io) - WebRTC SFU
- [WebRTC](https://webrtc.org) - Real-time communication
- [Vite](https://vitejs.dev) - Build tooling

---

**Ready to add streaming to your game?** Check out the [Quick Start Guide](./QUICKSTART.md), the [Unity on Quest Guide](./quest/README.md), or join our [Discord](https://discord.gg/substream) for help!
