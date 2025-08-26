# 🧪 Developer Testing Instructions for Substream SDK

## Overview
Please test the Substream SDK streaming functionality across Unity Editor, Meta Quest device, and web viewer.

## 📋 Prerequisites
- Unity 2021.3 or newer
- Meta Quest 2/3/Pro with developer mode enabled
- Android Build Support in Unity
- Git

## 🚀 Quick Test Steps

### 1. Clone and Setup (5 minutes)
```bash
git clone https://github.com/jlin3/substreamsdk.git
cd substreamsdk
```

### 2. Unity Editor Test (10 minutes)

1. **Create New Unity Project**
   - Open Unity Hub → New Project → 3D → Name: "SubstreamTest"

2. **Import Demo Scene**
   - Double-click `StreamTestScene.unitypackage` 
   - Import All → Open the imported scene

3. **Add Streaming Script**
   - Copy `SimpleDemoScript.cs` to Assets folder
   - Select the GameObject with UI in the scene
   - Add Component → SimpleDemoScript
   - Connect UI elements in Inspector:
     - Start Button → startButton slot
     - Stop Button → stopButton slot
     - Status Text → statusText slot

4. **Test in Editor**
   - Press Play
   - Click "Start Streaming" button
   - Verify status shows: "🔴 LIVE - Streaming!"
   - Click "Stop Streaming" button
   - Verify status shows: "Stream stopped"

✅ **Expected**: Simulated streaming works, status updates properly

### 3. Build for Meta Quest (15 minutes)

1. **Configure Build Settings**
   - File → Build Settings → Android
   - Switch Platform
   - Player Settings:
     - Company Name: YourCompany
     - Product Name: SubstreamTest
     - Package Name: com.yourcompany.substreamtest
     - Minimum API Level: 29

2. **Build Android AAR** (if not included)
   ```bash
   cd quest/android
   ./gradlew :substream:assembleRelease
   # Copy AAR to Unity project
   cp substream/build/outputs/aar/substream-release.aar \
      ~/SubstreamTest/Assets/SubstreamSDK/Plugins/Android/
   ```

3. **Build APK**
   - Build Settings → Build
   - Save as: SubstreamTest.apk

### 4. Test on Meta Quest (10 minutes)

1. **Install on Quest**
   ```bash
   adb install SubstreamTest.apk
   ```

2. **Run Test**
   - Launch app on Quest
   - Click "Start Streaming"
   - **IMPORTANT**: Accept screen capture permission dialog
   - Verify "🔴 LIVE - Streaming!" appears

3. **Check Web Viewer**
   - On computer, open: https://substreamapp.surge.sh/demo-viewer.html
   - You should see "Demo Mode Active" message
   - In real deployment, this would show the actual stream

✅ **Expected**: Permission dialog appears, streaming status updates work

### 5. Test Real Streaming (Optional - 20 minutes)

1. **Setup WHIP Endpoint**
   - Option A: Use LiveKit Cloud (free tier)
   - Option B: Run local LiveKit server:
     ```bash
     cd substreamsdk
     npm run docker:up
     ```

2. **Configure Credentials**
   - In Unity SimpleDemoScript, modify:
     ```csharp
     config.BaseUrl = "https://your-api-url";
     config.WhipPublishUrl = "https://your-whip-url/rtc";
     ```

3. **Test Full Flow**
   - Build and deploy to Quest
   - Start streaming
   - View actual stream in web viewer

## 📊 Test Checklist

### Unity Editor
- [ ] Scene imports correctly
- [ ] SimpleDemoScript connects to UI
- [ ] Start button begins streaming
- [ ] Status text updates properly
- [ ] Stop button ends streaming
- [ ] No console errors

### Meta Quest Device
- [ ] App launches without crashes
- [ ] Permission dialog appears
- [ ] Permission can be granted
- [ ] Streaming starts after permission
- [ ] Status updates visible in VR
- [ ] Stop button works

### Web Viewer
- [ ] Demo viewer page loads
- [ ] Shows demo mode message (or real stream)
- [ ] No JavaScript errors

## 🐛 What to Report

Please document:
1. **Unity Version**: (e.g., 2022.3.10f1)
2. **Quest Model**: (e.g., Quest 3)
3. **Any Errors**: Console logs, crash reports
4. **Screenshots**: Of any issues
5. **Status Messages**: What text appeared
6. **Permission Flow**: Did dialog appear correctly?

## 📹 Expected Flow Video
Developer walkthrough: https://drive.google.com/drive/folders/1iA_geJ2jmnzQTM3pzisGLOzLIlDLuuqn

## ⚠️ Common Issues

1. **"Missing Android library"**
   - Build the AAR from quest/android/
   - Or proceed without it for Editor testing

2. **No permission dialog on Quest**
   - Ensure AndroidManifest.xml is included
   - Check developer mode is enabled

3. **Buttons not working**
   - Verify SimpleDemoScript is attached
   - Check UI elements are connected in Inspector

## 💬 Quick Test Commands

```bash
# Quick Editor test (no Quest needed)
echo "1. Import StreamTestScene.unitypackage"
echo "2. Add SimpleDemoScript.cs" 
echo "3. Press Play, click Start"

# Full Quest test
echo "1. Build for Android"
echo "2. adb install SubstreamTest.apk"
echo "3. Run on Quest, grant permission"
echo "4. Check https://substreamapp.surge.sh/demo-viewer.html"
```

## 📞 Contact
Report results to: [your email]
Include: Logs, screenshots, and this checklist

---

**Total Test Time**: ~30-40 minutes
**Critical Path**: Unity Editor → Quest Permission → Streaming Status
