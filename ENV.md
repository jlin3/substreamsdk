# Environment Configuration

Create a `.env.local` file in the project root with:

```
VITE_PARENTCONNECT_BASE_URL=https://api.parentconnect.example
VITE_STREAM_ISSUER=https://auth.parentconnect.example
```

If you do not have an auth server yet, set a short-lived token in your browser for the demo:

```
localStorage.setItem('SUBSTREAM_BEARER', '<jwt>')
```

The demo SDK will send this token as a Bearer to the ingest endpoints.
