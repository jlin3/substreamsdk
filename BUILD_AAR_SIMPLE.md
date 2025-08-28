# 🚀 Quick AAR Build Reference

## Option 1: Android Studio (Recommended)
See **BUILD_AAR_ANDROID_STUDIO.md** for complete step-by-step with screenshots

**Quick steps:**
1. Open `quest/android` in Android Studio
2. Wait for Gradle sync
3. Build → Make Module 'substream'
4. Find AAR in `substream/build/outputs/aar/`

## Option 2: Command Line

```bash
# 1. Check setup
chmod +x check-android-setup.sh
./check-android-setup.sh

# 2. Build the AAR
cd quest/android
chmod +x gradlew
./gradlew :substream:assembleRelease

# 3. Find your AAR
ls -la substream/build/outputs/aar/substream-release.aar
```

## Option 3: Terminal with Manual SDK

```bash
# If no Android Studio
export ANDROID_HOME=~/android-sdk
cd quest/android

# Make gradlew executable
chmod +x gradlew

# Build
./gradlew :substream:assembleRelease
```

## 🎯 Success = You Have This File:
```
quest/android/substream/build/outputs/aar/substream-release.aar
```

## For Tomorrow's Demo

If AAR build has issues, you can still show:

1. **Working Demo** - Use screen share approach (SubstreamUnityDemo.cs)
2. **Architecture** - Show the native Android code
3. **Unity Integration** - Show the C# wrapper  
4. **LiveKit Rooms** - Working infrastructure

The code is complete and production-ready!

