#!/bin/bash

# WHIP Ingress Setup Script for LiveKit Cloud
# Project: substream-cnzdthyx

echo "🔴 LiveKit WHIP Ingress Setup"
echo "=============================="
echo ""

# Set credentials
export LIVEKIT_URL=wss://substream-cnzdthyx.livekit.cloud
export LIVEKIT_API_KEY=APIbtpHuQYmSvTT
export LIVEKIT_API_SECRET=RmbpdbNBZVkbAW8yW0K3ekrjwcdqIKNo6OQDwt0211Y

# Check if livekit-cli is installed
if ! command -v livekit-cli &> /dev/null; then
    echo "❌ LiveKit CLI not found!"
    echo ""
    echo "Please install it first:"
    echo "  Mac: brew install livekit-cli"
    echo "  Or download from: https://github.com/livekit/livekit-cli/releases"
    echo ""
    exit 1
fi

echo "✅ LiveKit CLI found"
echo ""
echo "Creating WHIP ingress..."
echo ""

# Create WHIP ingress
livekit-cli create-ingress \
  --request '{
    "input_type": 3,
    "name": "Unity Game Stream",
    "room_name": "",
    "participant_identity": "unity-streamer",
    "participant_name": "Unity Stream",
    "bypass_transcoding": false
  }'

echo ""
echo "📝 Next Steps:"
echo "1. Copy the WHIP URL from above"
echo "2. Open SimpleDemoScript.cs in Unity"
echo "3. Replace line 102 with your actual WHIP URL:"
echo "   whipUrl = \"YOUR-WHIP-URL-HERE\";"
echo ""
echo "Done! 🎮"
