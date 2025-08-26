#!/bin/bash

# LiveKit Unity SDK Quick Installer
echo "🚀 LiveKit Unity SDK Quick Setup"
echo "================================"
echo ""

# Check if we're in a Unity project
if [ ! -d "Assets" ]; then
    echo "❌ Error: This doesn't look like a Unity project directory."
    echo "   Please run this script from your Unity project root (where Assets folder is)."
    exit 1
fi

echo "📦 Step 1: Cloning LiveKit Unity SDK..."
cd Packages
git clone https://github.com/livekit/client-sdk-unity.git
cd ..

echo ""
echo "📝 Step 2: Creating streaming script..."

# Create the streaming script
mkdir -p Assets/Scripts
cp ../substreamsdk/LiveKitStreamingDemo.cs Assets/Scripts/

echo ""
echo "✅ Installation Complete!"
echo ""
echo "📋 Next Steps:"
echo "1. Open Unity"
echo "2. Find LiveKitStreamingDemo.cs in Assets/Scripts"
echo "3. Add it to a GameObject"
echo "4. Connect your UI buttons"
echo "5. Press Play and click 'Start Streaming'"
echo ""
echo "👀 View stream at:"
echo "https://cloud.livekit.io/projects/substream-cnzdthyx/rooms"
echo ""
echo "🎯 That's it! Streaming should work immediately!"
