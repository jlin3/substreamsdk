# 🛠️ Complete Guide: Building Quest AAR in Android Studio

## Prerequisites Check

Before starting, ensure you have:
- ✅ Android Studio installed (Arctic Fox or newer)
- ✅ Java JDK (comes with Android Studio)
- ✅ About 10GB free disk space

## 📋 Step-by-Step Instructions

### Step 1: Open the Project in Android Studio

1. **Launch Android Studio**
2. Click **"Open"** (not "New Project")
3. Navigate to: `/Users/jesselinson/substreamsdk/quest/android`
4. Select the `android` folder and click **"Open"**
5. Android Studio will start loading the project

### Step 2: Wait for Initial Sync

Android Studio will show a progress bar at the bottom:
- "Scanning files to index..." 
- "Gradle sync started..."

**⏱️ This may take 3-5 minutes on first open**

If you see any popups:
- **"Trust project?"** → Click **Trust Project**
- **"Update Gradle Plugin?"** → Click **Don't remind me again**

### Step 3: Fix SDK Location

1. Look at the bottom of Android Studio for any red errors
2. If you see "SDK location not found":
   - Click **File** → **Project Structure** (⌘+; on Mac)
   - Under **SDK Location**, click **"Download"** next to Android SDK
   - Or set it to: `/Users/jesselinson/Library/Android/sdk`
   - Click **OK**

### Step 4: Install Required SDK Components

1. Click **Tools** → **SDK Manager**
2. In the **SDK Platforms** tab, ensure checked:
   - ✅ Android 14.0 (API 34)
   - ✅ Android 10.0 (API 29)
3. In the **SDK Tools** tab, ensure checked:
   - ✅ Android SDK Build-Tools 34
   - ✅ Android SDK Platform-Tools
   - ✅ NDK (Side by side) - version 25.2.9519653
4. Click **OK** and let it download

### Step 5: Sync Gradle

1. Click the **"Sync Project with Gradle Files"** button (🔄 icon in toolbar)
   - Or: **File** → **Sync Project with Gradle Files**
2. Wait for sync to complete (bottom progress bar)
3. The **Build** tab should show "Gradle sync finished"

### Step 6: Resolve Any Dependency Issues

If you see errors about missing dependencies:

1. Open `quest/android/substream/build.gradle.kts`
2. Android Studio might show yellow highlights on dependencies
3. Click the yellow bulb 💡 and select **"Change to..."** for any updates
4. Click **Sync Now** (appears as blue text at top of editor)

### Step 7: Build the AAR

**Method A: Using the Build Menu**
1. Click **Build** → **Clean Project** (clears any old builds)
2. Click **Build** → **Make Module 'substream'**
3. Wait for build to complete (see progress in bottom bar)

**Method B: Using Gradle Panel**
1. Click **Gradle** tab on right side of Android Studio
2. Navigate to: `substream-sdk → substream → Tasks → build`
3. Double-click **`assembleRelease`**
4. Watch the **Build** output tab at bottom

**Method C: Using Terminal (in Android Studio)**
1. Click **Terminal** tab at bottom
2. Run:
   ```bash
   ./gradlew :substream:assembleRelease
   ```

### Step 8: Find Your AAR File

✅ **Your AAR is located at:**
```
/Users/jesselinson/substreamsdk/quest/android/substream/build/outputs/aar/substream-release.aar
```

To verify:
1. In Android Studio, switch to **Project** view (dropdown above file tree)
2. Navigate to: `substream → build → outputs → aar`
3. You should see **`substream-release.aar`** (about 2-5 MB)

### Step 9: Copy AAR to Unity

1. Copy the AAR file
2. Paste it to your Unity project:
   ```
   YourUnityProject/Assets/Plugins/Android/substream-release.aar
   ```

## 🚨 Common Issues & Solutions

### Issue: "Gradle sync failed"
**Solution:**
- File → Invalidate Caches → Invalidate and Restart
- Delete `.gradle` folder and try again

### Issue: "SDK not found"
**Solution:**
1. Android Studio → Preferences → System Settings → Android SDK
2. Download Android SDK 34 and 29
3. Note the SDK path and update `local.properties`

### Issue: "Could not resolve dependencies"
**Solution:**
1. Check internet connection (needs to download libraries)
2. File → Settings → Build → Gradle
3. Uncheck "Offline work"

### Issue: "Build failed with NDK error"
**Solution:**
1. Tools → SDK Manager → SDK Tools
2. Install NDK version 25.2.9519653 specifically
3. Sync project again

### Issue: "Kotlin version conflict"
**Solution:**
- Let Android Studio update Kotlin plugin
- Tools → Kotlin → Configure Kotlin Plugin Updates

## ✅ Success Checklist

You know it worked when:
- [ ] Build output shows "BUILD SUCCESSFUL"
- [ ] No red errors in Build tab
- [ ] `substream-release.aar` exists in build/outputs/aar/
- [ ] File size is 2-5 MB

## 🎯 Quick Terminal Alternative

If Android Studio is giving you trouble, try the terminal:

```bash
cd /Users/jesselinson/substreamsdk/quest/android

# Make gradlew executable
chmod +x gradlew

# Build the AAR
./gradlew :substream:assembleRelease

# Find your AAR
ls -la substream/build/outputs/aar/
```

## 📱 Next Steps

Once you have the AAR:
1. Copy to Unity: `Assets/Plugins/Android/`
2. Set Unity to Android platform
3. Build and deploy to Quest
4. Test MediaProjection streaming!

## 💡 Pro Tips

- **First build is slow** (downloads dependencies) - get coffee ☕
- **Keep Android Studio updated** for best compatibility
- **Use Release build** for Unity (not debug)
- **Save the AAR** in multiple places as backup

---

🎉 **That's it!** You should now have `substream-release.aar` ready for Unity!
