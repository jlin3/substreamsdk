#!/bin/bash

# Alternative WHIP Creation Methods

echo "🔴 Alternative WHIP Setup Methods"
echo "================================="
echo ""

# Your credentials
export LIVEKIT_URL=wss://substream-cnzdthyx.livekit.cloud
export LIVEKIT_API_KEY=APIbtpHuQYmSvTT
export LIVEKIT_API_SECRET=RmbpdbNBZVkbAW8yW0K3ekrjwcdqIKNo6OQDwt0211Y

echo "Since the CLI isn't working, here are alternatives:"
echo ""

echo "Option 1: Use cURL to create WHIP ingress"
echo "========================================="
echo ""

# Generate JWT token using Python
JWT_TOKEN=$(python3 -c "
import jwt
import time

payload = {
    'iss': 'APIbtpHuQYmSvTT',
    'sub': 'APIbtpHuQYmSvTT', 
    'aud': 'substream-cnzdthyx',
    'exp': int(time.time()) + 600,
    'nbf': int(time.time()),
    'video': {'roomCreate': True, 'roomList': True, 'roomRecord': True, 'roomAdmin': True, 'roomJoin': True, 'room': '*', 'canPublish': True, 'canSubscribe': True, 'canPublishData': True, 'canPublishSources': ['camera', 'microphone', 'screen_share', 'screen_share_audio'], 'canUpdateOwnMetadata': True, 'ingressAdmin': True}
}

token = jwt.encode(payload, 'RmbpdbNBZVkbAW8yW0K3ekrjwcdqIKNo6OQDwt0211Y', algorithm='HS256')
print(token)
" 2>/dev/null)

if [ -z "$JWT_TOKEN" ]; then
    echo "❌ Failed to generate JWT token"
    echo "Make sure Python and PyJWT are installed:"
    echo "  pip3 install pyjwt"
    echo ""
else
    echo "✅ JWT token generated"
    echo ""
    echo "Running cURL request..."
    echo ""
    
    curl -X POST https://substream-cnzdthyx.livekit.cloud/twirp/livekit.Ingress/CreateIngress \
      -H "Authorization: Bearer $JWT_TOKEN" \
      -H "Content-Type: application/json" \
      -d '{
        "input_type": 3,
        "name": "Unity Game Stream",
        "room_name": "",
        "participant_identity": "unity-streamer",
        "participant_name": "Unity Stream"
      }'
    
    echo ""
    echo ""
fi

echo "Option 2: Use the old CLI syntax"
echo "================================"
echo ""
echo "Try these variations:"
echo ""
echo "# Variation 1:"
echo "livekit-cli ingress create --name 'Unity Game Stream' --type whip"
echo ""
echo "# Variation 2:"
echo "livekit-cli create-ingress --name 'Unity Game Stream' --input-type whip"
echo ""
echo "# Variation 3 (with stdin):"
echo "echo '{\"input_type\":3,\"name\":\"Unity Game Stream\"}' | livekit-cli create-ingress --request -"
echo ""

echo "Option 3: Manual API Request"
echo "==========================="
echo ""
echo "Use Postman or Insomnia with:"
echo "URL: POST https://substream-cnzdthyx.livekit.cloud/twirp/livekit.Ingress/CreateIngress"
echo "Headers:"
echo "  Authorization: Bearer [Generate token at https://docs.livekit.io/realtime/guides/access-tokens/]"
echo "  Content-Type: application/json"
echo "Body:"
echo '{'
echo '  "input_type": 3,'
echo '  "name": "Unity Game Stream",'
echo '  "room_name": "",'
echo '  "participant_identity": "unity-streamer",'
echo '  "participant_name": "Unity Stream"'
echo '}'
echo ""

echo "Option 4: Contact Support"
echo "========================"
echo ""
echo "Email support@livekit.io with:"
echo "- Project ID: substream-cnzdthyx"
echo "- Request: 'Please create a WHIP ingress endpoint for Unity streaming'"
echo "- They can create it for you!"
echo ""
