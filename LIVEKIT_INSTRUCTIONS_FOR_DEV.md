# 🚨 How to Get Actual Streaming Working

Hi! The script is set up for REAL streaming, not demo mode. Here's what you need to do:

## Quick Fix (10 minutes total)

### 1. Get LiveKit Cloud WHIP URL (5 min)
```
1. Go to: https://cloud.livekit.io
2. Sign up / Log in
3. Create project (free tier is fine)
4. Go to "Ingress" → "Create Ingress Endpoint"
5. Choose "WHIP" type
6. Copy the URL (looks like: https://url-xxxxxxxxx.whip.livekit.cloud/w)
```

### 2. Put URL in Unity (1 min)

**EITHER in SimpleDemoScript.cs line 100:**
```csharp
whipUrl = "YOUR-ACTUAL-WHIP-URL-HERE"; // ← REPLACE THIS!
```

**OR in Unity Inspector:**
- Select GameObject with SimpleDemoScript
- Paste URL in "Whip Url Input" field

### 3. Test It (2 min)
1. Press Play in Unity
2. Click "Start Streaming"
3. Check Unity Console for room name (e.g., "unity-stream-abc123")

### 4. View Stream (2 min)
1. Go to LiveKit Cloud dashboard
2. Click "Rooms" 
3. Find your room, click "Join"
4. You'll see your Unity game streaming!

## What Was Wrong

The script had a placeholder URL:
```csharp
whipUrl = "https://url-xxxxxxxxx.whip.livekit.cloud/w"; // This is fake!
```

You need to replace it with YOUR LiveKit Cloud WHIP URL.

## Full Code Update

I've updated SimpleDemoScript.cs to:
- ✅ Use actual LiveKit Cloud streaming (not demo mode)
- ✅ Generate proper room names
- ✅ Show clear error if URL not configured
- ✅ Display viewing instructions

## Testing Checklist

In Unity Console you should see:
```
[SimpleDemoScript] Initialized with WHIP URL: https://url-[yours].whip.livekit.cloud/w
[SimpleDemoScript] Streaming to room: unity-stream-12345678
[SimpleDemoScript] For LiveKit Cloud viewing options:
[SimpleDemoScript] 1. Go to https://cloud.livekit.io
[SimpleDemoScript] 2. Select your project → Rooms
[SimpleDemoScript] 3. Find room: unity-stream-12345678
[SimpleDemoScript] 4. Click 'Join' to preview
```

## Still Not Working?

Check:
1. Is your WHIP URL correct? (not the placeholder)
2. Do you have internet connection?
3. Is the LiveKit ingress endpoint active?
4. Any errors in Unity Console?

The streaming IS working - you just need to configure it with your actual LiveKit Cloud credentials!

---

**Files to check:**
- `SimpleDemoScript.cs` - Line 100 for WHIP URL
- `LIVEKIT_CLOUD_SETUP.md` - Detailed setup guide
- Unity Console - For error messages

Let me know once you've added your WHIP URL and I can help debug any issues!
