# Quick Setup for Real Streaming

## 1. Add Your Credentials

Create a file called `.env.local` in the project root with your streaming credentials:

```bash
# Your streaming infrastructure credentials
VITE_LIVEKIT_URL=wss://YOUR_URL_HERE
VITE_LIVEKIT_API_KEY=YOUR_API_KEY_HERE
VITE_LIVEKIT_API_SECRET=YOUR_SECRET_HERE

# Your WHIP endpoint (if you have one already)
VITE_WHIP_PUBLISH_URL=YOUR_WHIP_URL_HERE
```

## 2. Restart the Demo

After adding your credentials:

```bash
# Stop the current server (Ctrl+C)
# Then restart:
npm run dev
```

## 3. Test Real Streaming

1. Open http://localhost:5173/demo.html
2. Click "Go Live"
3. You should see "🔴 LIVE - Streaming Active"
4. Your stream is now broadcasting!

## Need Credentials?

Contact Parent Connect support to get your streaming infrastructure access.

---

**Note**: The demo works without credentials too - it will show a local preview of the game.
