# Complete Substream Unity Package

This folder contains everything needed to create the final `.unitypackage` file.

## Contents:

1. **Scripts/**
   - `SimpleDemoScript.cs` - The demo script provided by the developer
   - `Substream.cs` - Main SDK wrapper
   - `DemoController.cs` - Additional demo controller

2. **Plugins/Android/**
   - `substream-release.aar` - The Android native library
   - `AndroidManifest.xml` - Android permissions

3. **StreamTestScene/**
   - The demo scene from StreamTestScene.unitypackage

4. **Documentation/**
   - Setup instructions
   - API reference

## Creating the .unitypackage:

Since .unitypackage files must be created from within Unity Editor:

1. **In Unity:**
   - Create new Unity project (or use existing)
   - Import all files from this folder
   - Assets → Export Package
   - Select all Substream files
   - Export as `SubstreamSDK.unitypackage`

2. **What to include:**
   - ✅ All Scripts
   - ✅ Plugins folder with AAR
   - ✅ Demo scene
   - ✅ Documentation

## For the Developer:

The package includes:
- Working demo scene with UI
- SimpleDemoScript.cs (already configured)
- Android native library for Quest
- LiveKit Cloud integration (pre-configured)
