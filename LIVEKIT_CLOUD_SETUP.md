# 🔴 LiveKit Cloud Setup for Unity Streaming

This guide will help you set up ACTUAL streaming from Unity to LiveKit Cloud.

## Step 1: Get Your LiveKit Cloud Account (5 minutes)

1. **Sign up at**: https://cloud.livekit.io
2. **Create a new project** (e.g., "unity-streaming")
3. **Select a region** closest to you

## Step 2: Get Your WHIP URL (2 minutes)

1. In LiveKit Cloud dashboard, go to **"Ingress"** section
2. Click **"Create Ingress Endpoint"**
3. Configure:
   - **Name**: Unity Stream
   - **Type**: WHIP
   - **Room Name**: Leave empty (we'll set it per stream)
4. Click **"Create"**
5. **Copy the WHIP URL** - it looks like:
   ```
   https://url-xxxxxxxxx.whip.livekit.cloud/w
   ```

## Step 3: Configure Unity Script (1 minute)

### Option A: In Unity Inspector (Easiest)
1. Select your GameObject with SimpleDemoScript
2. In the Inspector, find **"Whip Url Input"** field
3. Paste your WHIP URL

### Option B: In Code
1. Open `SimpleDemoScript.cs`
2. Find line 100:
   ```csharp
   whipUrl = "https://url-xxxxxxxxx.whip.livekit.cloud/w"; // ← REPLACE WITH YOUR URL!
   ```
3. Replace with your actual WHIP URL

## Step 4: Test Streaming (2 minutes)

1. **In Unity Editor**:
   - Press Play
   - Click "Start Streaming"
   - Look for: "SDK Initialized - WHIP: [your-url]"
   - Status shows: "🔴 LIVE - Streaming!"

2. **Check Console**:
   - You'll see the room name: `unity-stream-xxxxxxxx`
   - Note this room name!

## Step 5: View Your Stream (2 minutes)

### Option 1: LiveKit Cloud Dashboard
1. Go to https://cloud.livekit.io
2. Select your project → **"Rooms"**
3. Find your room (e.g., `unity-stream-12345678`)
4. Click **"Join"** to preview

### Option 2: LiveKit Meet
1. Go to https://meet.livekit.io
2. Enter:
   - **LiveKit URL**: Your project URL (wss://your-project.livekit.cloud)
   - **Token**: Generate one from dashboard
   - **Room**: Your room name from Unity console

### Option 3: Custom Viewer
Use our web viewer at:
```
https://substreamapp.surge.sh/viewer.html
```

## 🎮 Testing on Meta Quest

1. **Build for Android**
2. **Install on Quest**
3. **Run and grant permission**
4. **Stream appears in LiveKit Cloud!**

## ✅ Verification Checklist

- [ ] WHIP URL is set (not the placeholder)
- [ ] Unity console shows "SDK Initialized"
- [ ] Status shows "🔴 LIVE - Streaming!"
- [ ] Room name appears in console
- [ ] Stream visible in LiveKit dashboard

## 🚨 Common Issues

### "LiveKit URL not configured!"
You haven't replaced the placeholder URL. Check line 100 in SimpleDemoScript.cs

### "Connection failed"
- Check your WHIP URL is correct
- Ensure you have internet connection
- Verify the ingress endpoint is active in LiveKit Cloud

### "Stream not visible"
- Make sure you're looking in the correct room
- Check LiveKit Cloud dashboard → Rooms
- The room name is shown in Unity console

### "Permission denied"
Your LiveKit project might have security settings. Check:
- Dashboard → Settings → Security
- Ensure WHIP ingress is enabled

## 📊 LiveKit Cloud Pricing

- **Free tier**: 50GB egress/month
- **Starter**: $0.02/participant minute
- **Growth**: Volume discounts

See https://livekit.io/pricing

## 🔧 Advanced Configuration

### Custom Room Names
Instead of auto-generated rooms, you can set specific names:
```csharp
currentRoomId = "my-game-room-1";
```

### Authentication
For production, add authentication:
1. Generate API Key/Secret in LiveKit Cloud
2. Create tokens server-side
3. Pass tokens to Unity client

### Quality Settings
Adjust in SimpleDemoScript Inspector:
- **Low**: 720p 30fps 2Mbps
- **Medium**: 1080p 30fps 3.5Mbps  
- **High**: 1080p 60fps 5Mbps

## 🎯 Next Steps

1. **Test locally first** in Unity Editor
2. **Build and test on Quest**
3. **Monitor streams** in LiveKit dashboard
4. **Set up authentication** for production
5. **Create custom viewer** app

---

**Need help?** 
- Check Unity Console for detailed logs
- LiveKit docs: https://docs.livekit.io
- Support: support@livekit.io
