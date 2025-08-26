#!/bin/bash

# Create LiveKit Unity Package for easy distribution
echo "📦 Creating LiveKit Unity Package..."

# Create directory structure
mkdir -p LiveKitStreamingPackage/Scripts
mkdir -p LiveKitStreamingPackage/Documentation
mkdir -p LiveKitStreamingPackage/Prefabs

# Copy the main script
cp LiveKitStreamingDemo.cs LiveKitStreamingPackage/Scripts/

# Create a simple README
cat > LiveKitStreamingPackage/README.txt << 'EOF'
LiveKit Streaming for Unity - Quick Start

1. INSTALL LIVEKIT SDK:
   - Window → Package Manager
   - Click "+" → Add package from git URL
   - Enter: https://github.com/livekit/client-sdk-unity.git
   - Click Add

2. USE THE SCRIPT:
   - Find LiveKitStreamingDemo.cs in this package
   - Add to any GameObject
   - Connect UI: Start Button, Stop Button, Status Text

3. TEST:
   - Press Play in Unity
   - Click "Start Streaming"
   - View at: https://cloud.livekit.io/projects/substream-cnzdthyx/rooms
   - Look for room: unity-demo-room

That's it! The token is pre-configured and ready to use.

For Quest: Build normally - permissions handled automatically.
EOF

# Create a setup guide
cat > LiveKitStreamingPackage/Documentation/SETUP_GUIDE.txt << 'EOF'
Complete Setup Guide for LiveKit Unity Streaming

WHAT'S INCLUDED:
- LiveKitStreamingDemo.cs: Complete streaming script with token
- Pre-configured for LiveKit Cloud
- Works in Unity Editor and Quest

REQUIREMENTS:
- Unity 2021.3 or newer
- LiveKit Unity SDK (install via Package Manager)

STEP BY STEP:
1. Import this package
2. Install LiveKit SDK from Package Manager
3. Create empty GameObject
4. Add LiveKitStreamingDemo script
5. Create UI with Start/Stop buttons and status text
6. Connect UI elements in Inspector
7. Press Play and test!

VIEWING STREAMS:
- Go to: https://cloud.livekit.io/projects/substream-cnzdthyx/rooms
- Find your room (unity-demo-room)
- Click Join to watch

QUEST/ANDROID:
- Build normally for Android
- Permission dialog appears automatically
- User grants permission and streaming starts

TROUBLESHOOTING:
- Red errors? Make sure LiveKit SDK is installed
- Can't see stream? Check internet connection
- Token expired? Contact for new token

This is a complete, working solution!
EOF

# Create the package info
cat > LiveKitStreamingPackage/package.json << 'EOF'
{
  "name": "LiveKit Streaming Demo",
  "version": "1.0.0",
  "description": "Complete LiveKit streaming solution for Unity"
}
EOF

# Create a ZIP file
zip -r LiveKitStreamingPackage.zip LiveKitStreamingPackage/

echo "✅ Package created: LiveKitStreamingPackage.zip"
echo ""
echo "📤 Share this ZIP file with the developer!"
echo "   They just need to:"
echo "   1. Import the ZIP into Unity"
echo "   2. Install LiveKit SDK from Package Manager"
echo "   3. Use the included script"
echo ""
echo "🎯 Total setup time: ~5 minutes"
