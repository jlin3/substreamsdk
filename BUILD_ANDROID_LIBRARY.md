# 🔧 Building the Android Library for Unity

## The Missing Piece: `substream-release.aar`

The Unity streaming isn't working because the Android native library is missing. This library handles the actual WebRTC connection.

## Option 1: Build the AAR (Requires Android Studio)

### Prerequisites:
- Android Studio
- Java JDK 11+
- Android SDK

### Build Steps:
```bash
cd quest/android/

# If on Mac/Linux:
./gradlew :substream:assembleRelease

# If on Windows:
gradlew.bat :substream:assembleRelease
```

### Output Location:
```
quest/android/substream/build/outputs/aar/substream-release.aar
```

### Copy to Unity:
```bash
# Create directory if needed
mkdir -p YourUnityProject/Assets/SubstreamSDK/Plugins/Android/

# Copy the AAR
cp quest/android/substream/build/outputs/aar/substream-release.aar \
   YourUnityProject/Assets/SubstreamSDK/Plugins/Android/
```

## Option 2: Pre-built AAR (Quick Fix)

I'll build it for you. Run this:
```bash
cd quest/android/
./gradlew :substream:assembleRelease
```

Then I'll commit the AAR to the repo.

## Option 3: Bypass Native Plugin (Unity Editor Only)

For testing in Unity Editor only, modify `Substream.cs`:
```csharp
public static async Task Init(SubstreamConfig config)
{
    // ... existing code ...
    
    // For Editor testing without AAR
    #if UNITY_EDITOR
    Debug.Log("[Substream] Editor mode - simulating init");
    _isInitialized = true;
    return;
    #endif
    
    // ... rest of code ...
}
```

## 🚨 Why It's Not Working:

The Unity code is trying to call:
```csharp
#if UNITY_ANDROID && !UNITY_EDITOR
CallAndroid("startLive", _width, _height, _fps, _bitrate, _metadata, _withAudio);
#endif
```

But without the AAR file, `CallAndroid` fails silently. The C# code thinks it's streaming, but no native code is running.

## 📋 Quick Test After Adding AAR:

1. Copy AAR to Unity project
2. Unity will import it automatically
3. In Unity Console, you should see:
   - "SDK Initialized - WHIP: [url]"
   - "Streaming to room: [room-id]"
   - And room WILL appear in LiveKit dashboard!

## Alternative: Use Web-Based Approach

Since Unity WebGL/Editor doesn't need native plugins, you could:
1. Use Unity's WebRTC package directly
2. Implement WHIP in pure C#
3. Skip the Android native layer

Example: https://github.com/Unity-Technologies/com.unity.webrtc

---

**The Fix:** Build and include `substream-release.aar` in the Unity project, and streaming will work!
