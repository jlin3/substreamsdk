#!/bin/bash

# Commit script for Substream Unity SDK improvements

echo "📦 Adding new demo files..."

# Add the key new files for the demo
git add SubstreamSDK.cs
git add SubstreamUnityDemo.cs  
git add SubstreamOneClick.cs
git add TestQuestAAR.cs

# Add documentation
git add DEMO_TOMORROW.md
git add DEMO_READY.md
git add DEMO_SIMPLE_STEPS.txt
git add HOW_IT_WORKS.md
git add UNITY_INTEGRATION_GUIDE.md
git add BUILD_AAR_ANDROID_STUDIO.md

# Add utility scripts
git add check-android-setup.sh
git add PrepareUnityPackage.cs

# Add the built AAR
git add substream-release-quest.aar

echo "✅ Files staged for commit"
echo ""
echo "📝 Creating commit..."
git commit -m "Add improved Unity SDK with one-line integration

- Created SubstreamSDK.cs with automatic UI and platform detection
- Added SubstreamUnityDemo.cs for interactive demos
- Added SubstreamOneClick.cs for simplest possible integration
- Built Quest AAR successfully (47KB)
- Added comprehensive documentation for demos
- Created build and setup verification scripts
- Simplified integration to literally one line of code
- Screen share demo works immediately in Unity Editor
- Quest native capture ready with built AAR"

echo ""
echo "🚀 Ready to push!"
echo "Run: git push origin main"
