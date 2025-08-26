# Unity Package Creation TODO

This document is for a Unity developer to create the actual .unitypackage file.

## Files Provided

All Unity files are in: `unity-package/Assets/SubstreamSDK/`

## Steps to Create Package

### 1. Setup Unity Project
- Create new Unity project (2021.3 or newer)
- Copy entire `SubstreamSDK` folder to your `Assets/` folder

### 2. Build Android Library
The Android native code needs to be compiled:

```bash
cd quest/android

# If you have gradle installed:
gradle :substream:assembleRelease

# If not, you'll need to install gradle first
# The output will be in: substream/build/outputs/aar/substream-release.aar
```

Copy the AAR to: `Assets/SubstreamSDK/Plugins/Android/substream-release.aar`

### 3. Verify Everything Works
- Check Console for any errors
- Create test scene using: Substream → Create Demo Scene
- Make sure DemoController script works

### 4. Configure Android Settings
- Switch to Android platform
- Set minimum API to 29
- Ensure the AndroidManifest.xml is being included

### 5. Export Unity Package
- Right-click on `Assets/SubstreamSDK` folder
- Select "Export Package..."
- Make sure these are checked:
  - ✓ Include dependencies
  - ✓ Scripts/
  - ✓ Editor/
  - ✓ Plugins/Android/ (with AAR file)
  - ✓ Demos/
  - ✓ Documentation/
- Save as: `SubstreamSDK.unitypackage`

### 6. Test the Package
- Create a fresh Unity project
- Import the .unitypackage
- Try creating demo scene
- Build for Android/Quest

## What Should Be in Final Package

- `Scripts/Substream.cs` - Main SDK
- `Editor/DemoSceneCreator.cs` - Menu tools
- `Demos/Scripts/DemoController.cs` - Example
- `Plugins/Android/substream-release.aar` - Native library
- `Plugins/Android/AndroidManifest.xml` - Permissions
- All .meta files
- All .asmdef files

## Deliverable

Final `SubstreamSDK.unitypackage` file that developers can import with one click.

## Support Files Locations

- C# Scripts: `quest/unity/SubstreamSDK/`
- Android Source: `quest/android/substream/src/`
- Documentation: `UNITY_QUEST_TESTING.md`

