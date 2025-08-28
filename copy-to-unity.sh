#!/bin/bash

# Copy Quest SDK files to Unity project
# Usage: ./copy-to-unity.sh /path/to/your/unity/project

UNITY_PROJECT=$1

if [ -z "$UNITY_PROJECT" ]; then
    echo "Usage: ./copy-to-unity.sh /path/to/your/unity/project"
    echo "Example: ./copy-to-unity.sh ~/Unity/QuestStreamDemo"
    exit 1
fi

echo "=== Copying Quest SDK to Unity ==="
echo "Target: $UNITY_PROJECT"

# Create directories
mkdir -p "$UNITY_PROJECT/Assets/Plugins/Android"
mkdir -p "$UNITY_PROJECT/Assets/Scripts/SubstreamSDK"

# Copy AAR
echo "Copying AAR..."
cp substream-release.aar "$UNITY_PROJECT/Assets/Plugins/Android/"

# Copy Unity scripts
echo "Copying Unity scripts..."
cp quest/unity/SubstreamSDK/Substream.cs "$UNITY_PROJECT/Assets/Scripts/SubstreamSDK/"
cp quest/unity/SubstreamSDK/DemoController.cs "$UNITY_PROJECT/Assets/Scripts/SubstreamSDK/"

echo ""
echo "✅ Files copied successfully!"
echo ""
echo "Next steps in Unity:"
echo "1. Open your project"
echo "2. Add DemoController to a GameObject"
echo "3. Configure LiveKit URLs"
echo "4. Build for Quest!"

