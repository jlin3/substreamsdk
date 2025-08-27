# Android Studio - Step by Step AAR Build

## Step 1: Open the Project

1. **Launch Android Studio**
2. On the welcome screen, click **"Open"**
   - Or if you have a project open: File → Open
3. Navigate to: `/Users/jesselinson/substreamsdk/quest/android`
4. Click **"Open"**

## Step 2: Wait for Sync (First Time Only)

Android Studio will show:
- **"Loading..."** in bottom right
- **"Syncing..."** progress bar
- This downloads Gradle, Kotlin, Android SDK components
- **First time: 5-15 minutes** depending on internet speed

You'll see messages like:
```
Downloading gradle-8.2-bin.zip
Downloading kotlin-compiler-1.9.23.zip
```

## Step 3: Check for Errors

After sync completes, look at bottom panel:
- **"Build" tab** - Should show "Sync finished"
- If you see **red errors**, click them for details
- Common fixes:
  - Accept Android SDK licenses
  - Install missing SDK versions

## Step 4: Open Terminal in Android Studio

1. Bottom of Android Studio → Click **"Terminal"** tab
2. You should see: `android %` or similar
3. You're now in the `/quest/android` directory

## Step 5: Build the AAR

In the Terminal, type:
```bash
./gradlew :substream:assembleRelease
```

What you'll see:
```
> Task :substream:preBuild
> Task :substream:preReleaseBuild
> Task :substream:compileReleaseKotlin
> Task :substream:bundleReleaseAar
BUILD SUCCESSFUL in 47s
```

## Step 6: Find Your AAR

The AAR is located at:
```
quest/android/substream/build/outputs/aar/substream-release.aar
```

To open in Finder:
```bash
open substream/build/outputs/aar/
```

## Step 7: Copy to Unity

1. Copy `substream-release.aar`
2. In Unity: Create folder `Assets/Plugins/Android/`
3. Paste the AAR there
4. Unity will automatically import it

## Troubleshooting

### If gradlew permission denied:
```bash
chmod +x gradlew
./gradlew :substream:assembleRelease
```

### If "SDK not found":
1. Android Studio → Preferences → System Settings → Android SDK
2. Note the SDK path
3. Create file `quest/android/local.properties`:
```
sdk.dir=/Users/jesselinson/Library/Android/sdk
```

### If "NDK not found":
1. Android Studio → Tools → SDK Manager
2. SDK Tools tab → Check "NDK (Side by side)"
3. Apply

## Quick Check - Is it Working?

In Android Studio Terminal:
```bash
ls -la substream/build/outputs/aar/
```

You should see:
```
substream-release.aar
```

That's your file! 🎉
