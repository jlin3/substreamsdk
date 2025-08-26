# Simple Unity Package Creation Steps

## 1. Download and Open Unity Editor
- Download Unity Hub from: https://unity.com/download
- Install Unity 2021.3 or newer
- Open Unity Hub and create a new 3D project

## 2. Copy Files to Your Unity Project
Find the `SubstreamSDK` folder at:
`substreamsdk/unity-package/Assets/SubstreamSDK/`

Copy this entire folder to:
`YourUnityProject/Assets/`

You can do this by:
- Opening Finder/Explorer
- Navigating to the SubstreamSDK folder
- Drag and drop it into Unity's Project window

## 3. Check It Imported Correctly
In Unity, you should see:
- Assets/SubstreamSDK folder in the Project window
- A "Substream" menu in the top menu bar
- No red errors in the Console

## 4. Test the Demo Scene Creator
- Click: Substream → Create Demo Scene
- A new scene should be created with UI elements

## 5. Add the Android Library
The Android library (AAR file) is missing. You have two options:

**Option A: Skip It (For Testing)**
- The package will work in Unity Editor
- Just won't work on actual Android/Quest devices

**Option B: Build It**
- Need Java and Gradle installed
- Navigate to `quest/android/` in terminal
- Run: `gradle :substream:assembleRelease`
- Copy the AAR file to: `Assets/SubstreamSDK/Plugins/Android/`

## 6. Export the Package
1. Right-click on `Assets/SubstreamSDK` folder
2. Select "Export Package..."
3. Make sure everything is checked
4. Click "Export"
5. Save as: `SubstreamSDK.unitypackage`

## 7. Test Your Package
1. Create a new Unity project
2. Double-click your `.unitypackage` file
3. Import all
4. Try: Substream → Create Demo Scene

## If You Get Stuck
The key folders you need are:
- `unity-package/Assets/SubstreamSDK/` - All the Unity files
- `quest/android/` - Android source code (optional)

## Common Issues
- **"Can't find Substream menu"** - Scripts didn't import correctly
- **"Missing Android library"** - Expected, still works in Editor
- **"Compilation errors"** - Check Unity version is 2021.3+

