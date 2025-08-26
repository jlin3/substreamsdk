# 🔴 WHIP Ingress Setup for LiveKit Cloud

Since WHIP ingress creation requires API permissions, here are your options:

## Option 1: Use LiveKit CLI (Recommended)

### 1. Install LiveKit CLI
```bash
# Mac
brew install livekit-cli

# Or download from:
# https://github.com/livekit/livekit-cli/releases
```

### 2. Set Your Credentials
```bash
export LIVEKIT_URL=wss://substream-cnzdthyx.livekit.cloud
export LIVEKIT_API_KEY=APIbtpHuQYmSvTT
export LIVEKIT_API_SECRET=RmbpdbNBZVkbAW8yW0K3ekrjwcdqIKNo6OQDwt0211Y
```

### 3. Create WHIP Ingress
```bash
livekit-cli create-ingress \
  --request '{
    "input_type": 3,
    "name": "Unity Game Stream",
    "room_name": "",
    "participant_identity": "unity-streamer",
    "participant_name": "Unity Stream"
  }'
```

### 4. Copy the WHIP URL
The output will show:
```
URL: https://url-xxxxxxxxx.whip.livekit.cloud/w
```

## Option 2: Use RTMP Instead (Alternative)

If WHIP isn't available, you can use RTMP ingress:

1. In LiveKit dashboard, you might be able to create RTMP ingress
2. Use OBS or similar to convert Unity output to RTMP
3. Stream to the RTMP URL

## Option 3: Direct WebRTC (No Ingress)

Instead of using WHIP ingress, connect directly:

1. Generate a token for publishing
2. Use LiveKit Unity SDK to connect directly
3. No ingress needed!

### Modified Unity Approach:
```csharp
// Instead of WHIP URL, use direct connection
var token = GenerateToken(); // Server-side token generation
await room.Connect(
    "wss://substream-cnzdthyx.livekit.cloud",
    token
);
```

## Option 4: Contact LiveKit Support

Your project might need WHIP ingress enabled:
- Email: support@livekit.io
- Ask to enable WHIP ingress for project: substream-cnzdthyx

## Current Status

✅ You have:
- LiveKit Cloud project (substream-cnzdthyx)
- API credentials
- SIP ingress (for phone calls)

❌ You need:
- WHIP ingress endpoint
- Or use alternative approach

## Quick Test with OBS

While waiting for WHIP:
1. Download OBS Studio
2. Settings → Stream
3. Service: Custom
4. Server: (RTMP URL if you have one)
5. Stream Key: (from LiveKit)

---

**Need the WHIP URL for Unity?** Try the CLI method above or contact LiveKit support to enable WHIP for your project!
