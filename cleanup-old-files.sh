#!/bin/bash

# Script to archive old/redundant Unity scripts
echo "🧹 Cleaning up redundant files..."

# Create archive directory
mkdir -p archived-scripts

# Move old/redundant scripts
echo "📦 Archiving old scripts..."
mv SubstreamAutoInstaller.cs archived-scripts/ 2>/dev/null
mv SubstreamBundled.cs archived-scripts/ 2>/dev/null
mv SubstreamComplete.cs archived-scripts/ 2>/dev/null
mv SubstreamDemoComplete.cs archived-scripts/ 2>/dev/null
mv SubstreamLiveKitCloud.cs archived-scripts/ 2>/dev/null
mv SubstreamPackager.cs archived-scripts/ 2>/dev/null
mv SubstreamQuestDemo.cs archived-scripts/ 2>/dev/null
mv SubstreamTestScene.cs archived-scripts/ 2>/dev/null
mv UnityTestBothPlatforms.cs archived-scripts/ 2>/dev/null
mv UnityEditorStreaming.cs archived-scripts/ 2>/dev/null
mv WorkingStreamSolution.cs archived-scripts/ 2>/dev/null
mv QuestStreamingEasySetup.cs archived-scripts/ 2>/dev/null

echo "✅ Archived old scripts to archived-scripts/"

echo ""
echo "🎯 Keeping only essential scripts:"
echo "  - SubstreamSDK.cs (main SDK)"
echo "  - SubstreamOneClick.cs (simplest demo)"
echo "  - SubstreamUnityDemo.cs (interactive demo)"
echo "  - TestQuestAAR.cs (AAR verification)"
echo ""
echo "Run 'git add -A && git commit -m \"Archive redundant scripts\"' to commit"
