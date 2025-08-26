# 🔴 Create WHIP Ingress for Unity Streaming

You currently have a SIP ingress (for phone calls), but you need a **WHIP ingress** for Unity game streaming.

## Step-by-Step Instructions:

### 1. Go to Your Ingress Page
https://cloud.livekit.io/projects/substream-cnzdthyx/ingress

### 2. Click "Create Ingress Endpoint"

### 3. IMPORTANT: Select the Correct Type
- **Input Type**: Select **"WHIP"** (not SIP!)
  - WHIP = WebRTC streaming (what we need)
  - SIP = Phone/teleconference (what you have)

### 4. Configure WHIP Settings:
- **Name**: Unity Game Stream
- **Room Name Prefix**: (leave empty)
- **Participant Identity**: (leave as default)
- **Participant Name**: Unity Stream

### 5. Click "Create"

### 6. Copy Your New WHIP URL
It will look like:
```
https://url-xxxxxxxxx.whip.livekit.cloud/w
```

## Visual Guide:

```
Ingress Endpoints:

1. Phone Ingress (SIP) ❌
   sip:1megt1da62e.sip.livekit.cloud
   
2. Unity Stream (WHIP) ✅ ← You need to create this!
   https://url-xxxxxxxxx.whip.livekit.cloud/w
```

## Why WHIP?

- **WHIP**: WebRTC-HTTP Ingestion Protocol - for streaming video/audio from apps
- **SIP**: Session Initiation Protocol - for phone/VoIP calls

Unity needs WHIP to stream gameplay!

---

**After creating the WHIP ingress, paste the URL here and we'll be ready to stream!**
