# 🚀 Unity Quick Integration - 5 Minutes

## Step 1: Copy Files
```
SimpleDemoScript.cs → Your Unity project
```

## Step 2: In Unity Scene
1. GameObject → Create Empty → Name it "StreamManager"
2. Add Component → SimpleDemoScript
3. Drag your UI elements to the script slots:
   - Start Button → startButton
   - Stop Button → stopButton  
   - Status Text → statusText

## Step 3: Test It
1. Press Play in Unity
2. Click Start button
3. See "🔴 LIVE - Streaming!" status
4. Click Stop button

## That's it! 🎉

### What's Happening:
- **In Editor**: Simulates streaming (demo mode)
- **On Quest**: Requests permission → Captures screen → Streams

### Keyboard Shortcuts (Editor only):
- **S** = Start streaming
- **X** = Stop streaming

### The Code You Need:
```csharp
// Your button clicks will call these:
public void StartStreaming() // Already in script
public void StopStreaming()  // Already in script
```

### Status Messages:
- "Ready to stream" → Initialized
- "🔴 LIVE - Streaming!" → Active
- "Stream stopped" → Inactive

### Need Real Streaming?
Add your WHIP URL in the Inspector or code:
```csharp
config.BaseUrl = "https://your-api.com";
config.WhipPublishUrl = "https://your-whip.com/endpoint";
```

---
**Demo Viewer**: https://substreamapp.surge.sh/demo-viewer.html
