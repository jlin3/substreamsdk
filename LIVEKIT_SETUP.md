# Setting Up Cloud Streaming Infrastructure

Follow these steps to enable real streaming instead of demo mode.

## 1. Get Streaming Credentials

Contact Parent Connect support to receive:
   - **API Key** (e.g., `APIxxxxxxxx`)
   - **API Secret** (e.g., `xxxxxxxxxxxxxxxxxxxxxx`)
   - **WebSocket URL** (e.g., `wss://your-project.streaming.cloud`)

## 2. Configure Environment

Create a `.env.local` file in your project root:

```bash
# LiveKit Cloud Configuration
VITE_LIVEKIT_URL=wss://your-project.livekit.cloud
VITE_LIVEKIT_API_KEY=APIxxxxxxxx
VITE_LIVEKIT_API_SECRET=your-secret-here

# Leave empty for now - we'll generate this
VITE_WHIP_PUBLISH_URL=
```

## 3. Create WHIP Ingress

Run this command to create a WHIP ingress endpoint:

```bash
npm run create:ingress
```

This will output something like:
```
WHIP publish URL: https://url-xxxxxxxxx.whip.livekit.cloud/w
```

Add this URL to your `.env.local`:
```bash
VITE_WHIP_PUBLISH_URL=https://url-xxxxxxxxx.whip.livekit.cloud/w
```

## 4. Update Your Code

Replace demo mode with real configuration:

```javascript
// Instead of:
await Substream.init('demo')

// Use:
await Substream.init({
  baseUrl: import.meta.env.VITE_PARENTCONNECT_BASE_URL,
  tokenProvider: async () => {
    // In production, get a token from your backend
    // For testing, you can use the LiveKit JWT generator
    const token = await generateToken()
    return {
      accessToken: token,
      expiresAt: Date.now() + 3600000
    }
  },
  whipPublishUrl: import.meta.env.VITE_WHIP_PUBLISH_URL
})
```

## 5. Generate Access Tokens

For testing, you can generate tokens using the LiveKit CLI or their online JWT generator:

### Option A: Online Generator
1. Go to https://docs.livekit.io/realtime/guides/access-tokens/
2. Use the JWT generator with your API key/secret

### Option B: Backend Implementation
```javascript
// Example Node.js backend
import { AccessToken } from 'livekit-server-sdk';

function generateToken(roomName, participantName) {
  const token = new AccessToken(
    process.env.LIVEKIT_API_KEY,
    process.env.LIVEKIT_API_SECRET,
    {
      identity: participantName,
      ttl: '10m',
    }
  );
  
  token.addGrant({ 
    roomJoin: true, 
    room: roomName,
    canPublish: true,
    canSubscribe: true 
  });
  
  return token.toJwt();
}
```

## 6. View Your Stream

Once streaming with real LiveKit:

1. **LiveKit Cloud Dashboard**: 
   - Go to https://cloud.livekit.io
   - Click on your project → Rooms
   - You'll see active rooms and participants

2. **Custom Viewer**:
   - Update `viewer.html` to use LiveKit Client SDK
   - Connect with a viewer token to the same room

3. **LiveKit Meet**:
   - Quick test: https://meet.livekit.io
   - Enter your WebSocket URL and a viewer token

## 7. Restart Demo

After setting up `.env.local`:

```bash
# Restart the dev server to load new env vars
npm run dev
```

Your stream will now use real WebRTC through LiveKit Cloud!

## Troubleshooting

- **"Invalid token"**: Make sure your API key/secret match
- **"Connection failed"**: Check WHIP URL is correct
- **No video in viewer**: Ensure viewer has subscribe permissions
- **High latency**: Choose LiveKit region closest to you

## Costs

LiveKit Cloud pricing:
- **Free tier**: 50 participant minutes/month
- **Pay as you go**: ~$0.006 per participant minute
- **WHIP ingress**: Counts as one participant

See https://livekit.io/pricing for details.
