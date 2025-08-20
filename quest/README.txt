Substream Quest SDK (MVP scaffold)

Includes
- Android native plugin stubs (MediaProjection + MediaCodec + WHIP via libwebrtc)
- Unity C# wrapper with a demo controller (Go Live / Stop, Record / Finalize)
- Build and integration instructions for Unity

Targets
- Live: MediaProjection consent -> H.264 via MediaCodec -> WebRTC publish to LiveKit WHIP ingress
- VOD: MediaMuxer MP4 recording -> chunked upload -> finalize

Directory
- android/: Android library (AAR) stubs
- unity/SubstreamSDK/: Unity wrapper scripts (drop into your Unity project Assets/)

Android build (AAR)
- Requires: Android Studio Flamingo+, NDK r26+, AGP 8+, Kotlin 1.9+, minSdk 29
- Build:
  cd quest/android
  ./gradlew :substream:assembleRelease
- Output: quest/android/substream/build/outputs/aar/substream-release.aar

Unity integration
- Copy quest/unity/SubstreamSDK into your Unity project's Assets/
- Import substream-release.aar into Assets/Plugins/Android/
- Player Settings (Android): min API 29, target 34+, Internet permission
- AndroidManifest additions: MediaProjection + foreground service permissions

Usage (C#)
  await Substream.Init(new SubstreamConfig {
    BaseUrl = "https://api.parentconnect.example",
    TokenProvider = MyTokenProvider,
    WhipPublishUrl = "https://<your>.whip.livekit.cloud/w"
  });
  var live = await Substream.LiveCreate(new LiveOptions {
    Width = 1280, Height = 720, Fps = 30, VideoBitrateKbps = 3500,
    MetadataJson = "{\"gameId\":\"beat-saber\"}"
  });
  await live.Start();
  await live.Stop();

Next steps
- Implement native capture/publish internals in SubstreamNative.kt
- Add background uploader (WorkManager) for VOD chunks
- Token refresh hook and telemetry callbacks
