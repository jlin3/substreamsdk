# How to Build the Substream Quest AAR

You have 3 options to build the Android AAR:

## Option 1: Use Android Studio (Easiest)

1. **Download Android Studio** (if not installed)
   - https://developer.android.com/studio

2. **Open the project**
   - Open Android Studio
   - File → Open → Navigate to `quest/android`
   - Let it sync (may take a few minutes)

3. **Build the AAR**
   - In Android Studio: Build → Make Module 'substream'
   - Or in Terminal tab: `./gradlew :substream:assembleRelease`
   - Output: `quest/android/substream/build/outputs/aar/substream-release.aar`

## Option 2: Install Gradle Globally

1. **Install via Homebrew** (macOS)
   ```bash
   brew install gradle
   ```

2. **Build the AAR**
   ```bash
   cd quest/android
   gradle :substream:assembleRelease
   ```

## Option 3: Generate Gradle Wrapper

1. **If you have gradle installed:**
   ```bash
   cd quest/android
   gradle wrapper --gradle-version=8.2
   ./gradlew :substream:assembleRelease
   ```

## Quick Android Studio Setup

Since you don't have gradlew, **Android Studio is your fastest path**:

1. Open Android Studio
2. "Open an existing Android Studio project"
3. Select `/Users/jesselinson/substreamsdk/quest/android`
4. Wait for sync
5. Terminal → `./gradlew :substream:assembleRelease`

## What You Need Installed

- **JDK 11 or higher** (check with `java -version`)
- **Android SDK API 29+** (Android Studio installs this)
- **Android NDK** (for WebRTC native code)

## Expected Output

When successful, you'll see:
```
BUILD SUCCESSFUL in XXs
```

And find your AAR at:
`quest/android/substream/build/outputs/aar/substream-release.aar`

## If Build Fails

Common issues:
- Missing Android SDK → Install via Android Studio
- Missing NDK → Android Studio → SDK Manager → SDK Tools → NDK
- Kotlin version mismatch → Update in build.gradle.kts

## Alternative: Pre-built AAR

If you need the AAR immediately for testing, I can help you find a pre-built version or create a mock AAR for UI testing while you set up the build environment.
