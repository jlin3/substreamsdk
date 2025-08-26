# 🚀 LiveKit Quick Setup for Unity Streaming

## Option 1: Local LiveKit (5 minutes)

### 1. Start LiveKit Server
```bash
cd substreamsdk
docker-compose up -d
```

### 2. Check it's running
```bash
docker ps
# Should show livekit and livekit-ingress containers
```

### 3. Your URLs:
- **WHIP URL**: `http://localhost:8080/rtc`
- **Viewer**: `http://localhost:5173/viewer.html`

### 4. In Unity:
- Set WHIP URL to: `http://localhost:8080/rtc`
- Or edit SimpleDemoScript.cs line 81

---

## Option 2: LiveKit Cloud (10 minutes)

### 1. Sign up at https://livekit.io
- Create free account
- Create new project

### 2. Get your URLs:
- **WHIP URL**: `https://your-project.livekit.cloud/rtc`
- **API Key**: In project settings
- **API Secret**: In project settings

### 3. In Unity:
- Set WHIP URL to your LiveKit Cloud URL
- Authentication handled by SDK

---

## ✅ Testing Your Setup

### In Unity Editor:
1. Press Play
2. Click "Start Streaming"
3. Look for: "SDK Initialized - WHIP: [your-url]"
4. Status should show: "🔴 LIVE - Streaming!"
5. Copy the viewer URL from UI

### Check the Stream:
1. Open viewer URL in browser
2. You should see your Unity game streaming!

---

## 🔧 Troubleshooting

### "LiveKit URL not configured!"
- You're still using the default localhost URL
- Either start local LiveKit OR use LiveKit Cloud

### "Connection refused"
- LiveKit server not running
- Run: `docker-compose up -d`

### "Stream not visible"
- Check viewer URL is correct
- Make sure LiveKit server is running
- Check Unity Console for errors

### Docker not installed?
```bash
# Mac
brew install docker

# Windows/Linux
# Download from https://docker.com
```

---

## 📺 Viewer URLs

### Local LiveKit:
```
http://localhost:5173/viewer.html?room=[auto-generated-id]
```

### LiveKit Cloud:
```
https://your-project.livekit.cloud/preview?room=[auto-generated-id]
```

The room ID is automatically generated and shown in the Unity UI!

---

## 🎮 That's it!

Your Unity game is now streaming to LiveKit! 

Next steps:
- Build for Quest and test on device
- Customize streaming quality
- Add authentication
