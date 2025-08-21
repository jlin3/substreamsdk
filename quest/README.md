# Substream SDK for Unity (Meta Quest)

This guide walks Unity developers through integrating live streaming and recording in Meta Quest (Quest 2/3/Pro) games using Substream SDK. It covers setup, permissions, APIs, performance tuning, and troubleshooting.

## Prerequisites
- Unity 2020.3+ (URP or Built-in pipeline supported)
- Meta Quest 2/3/Pro device on recent OS (50+ recommended)
- Android SDK/NDK installed via Unity Hub
- Minimum Android API level 29 (Android 10) in Player Settings
- Internet access for WHIP ingress (LiveKit Cloud or your own)

## What you get
- Live streaming using Android MediaProjection + hardware encoder via WebRTC/WHIP
- Optional VOD recording to MP4 using MediaRecorder
- Foreground service for background-friendly capture (system notification)
- Unity-friendly async/await C# API with status/error callbacks
- Runtime adaptive quality (resolution, fps, bitrate)

## Install the SDK into your Unity project
1) Copy the Unity wrapper scripts into your project:
- Copy the directory `quest/unity/SubstreamSDK/` into your Unity project's `Assets/` folder.

2) Import the Android plugin (AAR):
- Build the Android library (instructions below) to generate `substream-release.aar`, then place it into `Assets/Plugins/Android/`.
- Alternatively, set up a CI job to build and drop the AAR into your project automatically.

3) Player Settings (Android):
- Minimum API Level: 29+
- Target API Level: 34+
- Internet Access: Require (for WHIP)

4) Permissions/Manifest
The included Android module declares required permissions:
- INTERNET
- FOREGROUND_SERVICE
- RECORD_AUDIO (optional if you want mic audio; playback capture will be added later)
- Application tag has `allowAudioPlaybackCapture="true"` for playback-capture readiness.

The module also declares:
- `ProjectionActivity` (trampoline for MediaProjection consent)
- `CaptureService` (foreground service with notification)

No additional manifest merges should be necessary if you use the provided AAR.

## Building the Android AAR
In `quest/android/`:
- Ensure Gradle 8.x and Kotlin 1.9.x are available (or add a Gradle wrapper in your environment/CI)
- Commands:
```
# From repository root or quest/android
# Using your local Gradle installation
gradle :substream:assembleRelease

# Output:
quest/android/substream/build/outputs/aar/substream-release.aar
```
Place the produced AAR into Unity at `Assets/Plugins/Android/substream-release.aar`.

Tip: Create a GitHub Actions workflow to build the AAR and attach it as an artifact on every push to main.

## Quick Start in Unity (Live Streaming)
1) Add a C# script to any GameObject:
```csharp
using UnityEngine;
using SubstreamSDK;
using System.Threading.Tasks;

public class VRStreamer : MonoBehaviour
{
    private LiveHandle live;

    async void Start()
    {
        await Substream.Init(new SubstreamConfig {
            BaseUrl = "demo", // or your API base URL
            WhipPublishUrl = "" // optional; defaults to BaseUrl/whip/publish
        });
    }

    public async void ToggleLive()
    {
        if (live == null)
        {
            live = await Substream.LiveCreate(new LiveOptions {
                Width = 1280,
                Height = 720,
                Fps = 30,
                VideoBitrateKbps = 2500,
                WithAudio = true
            });

            live.OnStatusChanged += s => Debug.Log($"Live status: {s}");
            live.OnError += e => Debug.LogError($"Live error: {e}");

            await live.Start(); // Triggers MediaProjection system prompt
        }
        else
        {
            await live.Stop();
            live = null;
        }
    }
}
```
2) Build to Android and run on Quest. When starting, the system will show a permission dialog for screen capture. Accept to begin streaming.

## Recording (VOD) to MP4
```csharp
private VodHandle vod;

public async void StartRecording()
{
    vod = await Substream.VodCreate(new VodOptions {
        Width = 1280,
        Height = 720,
        Fps = 30,
        VideoBitrateKbps = 2500,
        WithAudio = true,
        OutputHint = Application.productName
    });
    vod.OnSaved += path => Debug.Log($"Saved recording: {path}");
    vod.OnError += err => Debug.LogError($"VOD error: {err}");
    await vod.Start(); // Uses same permission flow if not already granted
}

public async void StopRecording()
{
    var path = await vod.Stop();
    Debug.Log($"Saved: {path}");
    vod = null;
}
```
Saved files are written to `Android/data/<your.app>/files/Movies/` by default (SAF/export flow can be added as a next step).

## Adaptive Quality at Runtime
To reduce performance impact or handle network constraints, you can adjust quality on the fly:
```csharp
live.UpdateQuality(width: 960, height: 540, fps: 30, bitrateKbps: 1500);
```
On Quest this updates the capture format and encoder bitrate through the native plugin.

## Recommended Defaults for Quest
- Start with 1280x720 @ 30fps, 2.5–3.5 Mbps
- Avoid matching the headset max framerate for complex scenes; VR must stay performant
- If you detect frame drops or high GPU time, lower resolution first, then bitrate, then fps

## Permission UX and Foreground Service
- Every capture session requires user consent via system dialog (cannot be bypassed)
- While streaming/recording, a notification is shown to the user through a foreground service
- You should consider adding an in-game HUD label ("LIVE" or "REC") to be transparent with the user

## Networking & WHIP
- LiveKit WHIP is used to publish the WebRTC stream
- Configure `SubstreamConfig.WhipPublishUrl` or rely on `BaseUrl/whip/publish`
- For production, add token-based auth: server endpoint issues a time-limited WHIP token

## Audio
- Current MVP uses microphone audio path for simplicity
- Playback capture (game audio) is being integrated via Android’s AudioPlaybackCaptureConfiguration; ensure `allowAudioPlaybackCapture` is enabled and test on Quest OS that supports it

## Unity API Summary
- `await Substream.Init(SubstreamConfig)`
- `await Substream.LiveCreate(LiveOptions)` -> `LiveHandle`
  - `await LiveHandle.Start()` / `await LiveHandle.Stop()`
  - `LiveHandle.OnStatusChanged`, `LiveHandle.OnError`
  - `LiveHandle.UpdateQuality(width,height,fps,bitrateKbps)`
- `await Substream.VodCreate(VodOptions)` -> `VodHandle`
  - `await VodHandle.Start()` / `await VodHandle.Stop()` (returns path)
  - `VodHandle.OnSaved`, `VodHandle.OnError`

## Performance Tips
- Use Unity’s XR stats to track GPU/CPU times; if frame time spikes, reduce stream resolution
- Prefer power-efficient presets for long sessions (720p30 at ~2.5–3 Mbps)
- Consider a cap on session duration to avoid thermal throttling
- Use hardware encoder (default) to minimize CPU usage

## Troubleshooting
- No permission prompt: Ensure Player Settings min API 29+ and `ProjectionActivity` exists in the AAR manifest
- Stream starts but viewers see nothing: Confirm WHIP URL and that LiveKit ingress is reachable; check logs
- Audio missing: If using mic, confirm RECORD_AUDIO permission; if playback capture, confirm OS support and allowlist
- Crashes on start: Look for MediaProjection or MediaRecorder errors; reduce resolution/fps as a test

## Building in CI (recommended)
- Create a GitHub Actions workflow that:
  - Sets up JDK 17, Gradle 8.x, Android SDK
  - Runs `gradle :substream:assembleRelease`
  - Uploads `substream-release.aar` as an artifact
- Optionally, publish AAR to an internal package registry for easy consumption

## Roadmap / Advanced Features
- Spectator camera capture (Unity `Camera`-based) as an alternative to MediaProjection
- Full playback audio capture path with capture config and filters
- Token-provider callback and auto-refresh for WHIP auth
- Chunked VOD upload with WorkManager and background resiliency
- HEVC and bitrate mode selection (CBR/VBR)
- Thermal feedback and auto quality throttling

---

Need help? Join our Discord or open an issue with device model, OS version, logs, and reproduction steps.