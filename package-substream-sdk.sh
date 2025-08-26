#!/bin/bash

echo "📦 Creating Substream Unity SDK Package..."

# Create package structure
mkdir -p SubstreamSDK_v1.0/Scripts
mkdir -p SubstreamSDK_v1.0/Editor
mkdir -p SubstreamSDK_v1.0/Documentation

# Copy files
cp SUBSTREAM_UNITY_PACKAGE/SubstreamSDK.cs SubstreamSDK_v1.0/Scripts/
cp SUBSTREAM_UNITY_PACKAGE/SubstreamQuickSetup.cs SubstreamSDK_v1.0/Editor/
cp SUBSTREAM_UNITY_PACKAGE/README.txt SubstreamSDK_v1.0/Documentation/

# Create package info
cat > SubstreamSDK_v1.0/package.json << 'EOF'
{
  "name": "com.substream.sdk",
  "version": "1.0.0",
  "displayName": "Substream SDK",
  "description": "One-file streaming solution for Unity. Just drop in and stream!",
  "unity": "2019.4",
  "keywords": ["streaming", "webrtc", "quest", "livekit"],
  "author": {
    "name": "Substream",
    "email": "support@substream.io"
  }
}
EOF

# Create a simple demo scene file
cat > SubstreamSDK_v1.0/Documentation/QuickStart.txt << 'EOF'
SUBSTREAM SDK - QUICK START

1. INSTANT STREAMING (Code):
   SubstreamSDK.StartStreaming();

2. WITH UI (Editor):
   GameObject → Substream → Create Streaming UI

3. VIEW STREAMS:
   Substream → Open Viewer Dashboard

That's it! No setup required.
EOF

# Create the zip
zip -r SubstreamSDK_v1.0.zip SubstreamSDK_v1.0/

echo "✅ Created: SubstreamSDK_v1.0.zip"
echo ""
echo "This package contains:"
echo "  • SubstreamSDK.cs - Core streaming functionality"
echo "  • SubstreamQuickSetup.cs - One-click UI creation"
echo "  • Documentation - Quick start guide"
echo ""
echo "Developers just need to:"
echo "1. Import SubstreamSDK_v1.0.zip"
echo "2. Use GameObject menu to create UI"
echo "3. Press Play and stream!"
echo ""
echo "NO LiveKit installation required!"
echo "NO configuration needed!"
echo "It just works! 🎉"
