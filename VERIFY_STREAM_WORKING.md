# 🔍 How to Verify if Stream is Actually Working

## Developer's Test Results:
- ✅ Console shows: "streaming to room: unity-stream-55d613d6"
- ❓ Not sure if actually streaming

## Quick Verification Steps:

### 1. Check LiveKit Dashboard
Go to: https://cloud.livekit.io/projects/substream-cnzdthyx/rooms

**What to look for:**
- Is room "unity-stream-55d613d6" listed?
- Does it show "1 participant"?
- Is status "Active"?

### 2. Common Issues & Fixes:

#### Issue: Room not appearing in dashboard
**Possible causes:**
1. **Authentication failing** - The SDK might not be authenticating properly
2. **Network blocked** - Unity might be blocked from connecting
3. **WHIP URL issue** - Connection not establishing

#### Debug Steps:

1. **Check Unity Console for errors:**
   Look for any red errors after "streaming to room"
   
2. **Add debug logging to SimpleDemoScript.cs:**
   ```csharp
   void OnStreamError(string error)
   {
       UpdateStatus($"Error: {error}");
       Debug.LogError($"[SimpleDemoScript] Stream error: {error}");
       // Add this line:
       Debug.LogError($"[SimpleDemoScript] Full error details: {error}");
   }
   ```

3. **Test with simpler WHIP client:**
   ```bash
   # Test if WHIP endpoint is accessible
   curl -X POST https://substream-cnzdthyx.whip.livekit.cloud/w/j3SPJ9KtW8Js \
     -H "Content-Type: application/sdp" \
     -d "test"
   ```

### 3. What "Streaming" Actually Means:

**If console shows "streaming" but no room in dashboard:**
- Unity SDK thinks it's connected
- But data isn't reaching LiveKit
- Usually an auth or network issue

### 4. Quick Test with OBS:

To verify the WHIP endpoint works:
1. Download OBS Studio
2. Settings → Stream
3. Service: WHIP
4. Server: `https://substream-cnzdthyx.whip.livekit.cloud/w/j3SPJ9KtW8Js`
5. Start Streaming
6. Check if it appears in LiveKit dashboard

## 🔧 Immediate Fixes to Try:

### Fix 1: Add Authentication Headers
The SDK might need auth headers. In `src/sdk/live.ts`, check if auth is being sent:
```typescript
headers: {
  'Authorization': `Bearer ${token}`,
  'Content-Type': 'application/sdp'
}
```

### Fix 2: Check Network in Unity
In Unity → Edit → Project Settings → Player:
- Configuration → Internet Access: Require
- Configuration → API Compatibility: .NET Standard 2.1

### Fix 3: Test Direct Connection
Try bypassing the SDK wrapper:
```csharp
// Add this test method to SimpleDemoScript
public async void TestDirectConnection()
{
    using (var client = new HttpClient())
    {
        var response = await client.GetAsync("https://substream-cnzdthyx.whip.livekit.cloud/w");
        Debug.Log($"WHIP endpoint response: {response.StatusCode}");
    }
}
```

## 📊 What Success Looks Like:

1. **Unity Console:**
   ```
   [SimpleDemoScript] Streaming to room: unity-stream-xxxxx
   [SimpleDemoScript] Stream status: Streaming
   ```

2. **LiveKit Dashboard:**
   - Room appears in list
   - Shows 1 participant
   - Can click "Join" to view

3. **No errors** in Unity console

## 🚨 If Nothing Works:

The issue might be:
1. **Android native plugin missing** - Check if substream-release.aar is included
2. **WebRTC not initializing** - Unity might need additional setup
3. **WHIP authentication** - May need token-based auth

## Alternative Approach:
Instead of WHIP, use direct LiveKit Unity SDK:
- https://github.com/livekit/client-sdk-unity
- More reliable for Unity → LiveKit streaming
- Better error messages

---

**Let me know:**
1. Do you see the room in LiveKit dashboard?
2. Any errors in Unity console?
3. Can you test with OBS to verify WHIP works?
