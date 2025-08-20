# Deployment Guide

Deploy the Substream SDK demo with one click or customize for production.

## 🚀 One-Click Demo Deployment

### Deploy to Vercel (Recommended for Demo)
[![Deploy with Vercel](https://vercel.com/button)](https://vercel.com/new/clone?repository-url=https://github.com/yourusername/substreamsdk)

- ✅ Free tier available
- ✅ Automatic HTTPS
- ✅ Global CDN
- ✅ Zero configuration

### Deploy to Netlify
[![Deploy to Netlify](https://www.netlify.com/img/deploy/button.svg)](https://app.netlify.com/start/deploy?repository=https://github.com/yourusername/substreamsdk)

- ✅ Free tier available
- ✅ Automatic HTTPS
- ✅ Instant rollbacks
- ✅ Branch previews

### Deploy to Render (Full Stack)
[![Deploy to Render](https://render.com/images/deploy-to-render-button.svg)](https://render.com/deploy?repo=https://github.com/yourusername/substreamsdk)

- ✅ Includes LiveKit server
- ✅ WHIP ingress endpoint
- ✅ Complete streaming infrastructure
- 💰 Requires paid plan for persistent services

## 🏠 Local Development

```bash
# Quick start
npm install
npm run demo

# Manual setup
docker compose up -d    # Start LiveKit
npm run dev            # Start dev server
```

## 🏢 Production Deployment

### 1. LiveKit Cloud (Recommended)

```bash
# Sign up at livekit.cloud
# Get your API keys
export LIVEKIT_API_KEY=your-key
export LIVEKIT_API_SECRET=your-secret

# Create WHIP ingress
npm run create:ingress

# Use the provided WHIP URL in your app
```

### 2. Self-Hosted LiveKit

```yaml
# docker-compose.prod.yml
version: '3.8'
services:
  livekit:
    image: livekit/livekit-server:latest
    command: --config /etc/livekit.yaml
    volumes:
      - ./livekit.yaml:/etc/livekit.yaml
    ports:
      - "7880:7880"
      - "7881:7881"
      - "50000-60000:50000-60000/udp"
    environment:
      - LIVEKIT_KEYS=${LIVEKIT_API_KEY}:${LIVEKIT_API_SECRET}
      
  ingress:
    image: livekit/ingress:latest
    depends_on:
      - livekit
    environment:
      - LIVEKIT_HOST=livekit:7880
      - LIVEKIT_API_KEY=${LIVEKIT_API_KEY}
      - LIVEKIT_API_SECRET=${LIVEKIT_API_SECRET}
    ports:
      - "80:8080"
```

### 3. Kubernetes Deployment

```yaml
# substream-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: substream-demo
spec:
  replicas: 3
  selector:
    matchLabels:
      app: substream-demo
  template:
    metadata:
      labels:
        app: substream-demo
    spec:
      containers:
      - name: web
        image: your-registry/substream-demo:latest
        ports:
        - containerPort: 3000
        env:
        - name: VITE_WHIP_PUBLISH_URL
          value: "https://whip.yourdomain.com"
---
apiVersion: v1
kind: Service
metadata:
  name: substream-demo
spec:
  selector:
    app: substream-demo
  ports:
  - port: 80
    targetPort: 3000
  type: LoadBalancer
```

## 🔧 Environment Variables

### Required for Production

```env
# LiveKit Configuration
LIVEKIT_URL=wss://your-livekit-server
LIVEKIT_API_KEY=your-api-key
LIVEKIT_API_SECRET=your-api-secret

# WHIP Endpoint
VITE_WHIP_PUBLISH_URL=https://your-whip-endpoint/whip

# Authentication (if not using demo mode)
VITE_PARENTCONNECT_BASE_URL=https://api.yourdomain.com
VITE_STREAM_ISSUER=https://auth.yourdomain.com
```

### Optional Configuration

```env
# Performance tuning
VITE_DEFAULT_BITRATE=3500
VITE_DEFAULT_FPS=30

# Analytics
VITE_ANALYTICS_ID=UA-XXXXXXXX

# Feature flags
VITE_ENABLE_VOD=true
VITE_ENABLE_CHAT=false
```

## 📊 Scaling Considerations

### CDN for Static Assets
```nginx
# nginx.conf
location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg)$ {
    expires 1y;
    add_header Cache-Control "public, immutable";
}
```

### Load Balancing WebRTC
- Use geo-distributed TURN servers
- Implement server-side load balancing
- Monitor bandwidth usage per region

### Database for Metadata
```sql
-- streams table
CREATE TABLE streams (
    id UUID PRIMARY KEY,
    room_name VARCHAR(255),
    user_id VARCHAR(255),
    started_at TIMESTAMP,
    ended_at TIMESTAMP,
    metadata JSONB
);

-- viewers table  
CREATE TABLE viewers (
    stream_id UUID REFERENCES streams(id),
    viewer_id VARCHAR(255),
    joined_at TIMESTAMP,
    left_at TIMESTAMP
);
```

## 🔒 Security Best Practices

### 1. Authentication
```javascript
// Implement proper token validation
const tokenProvider = async () => {
  const token = await fetch('/api/stream-token', {
    headers: {
      'Authorization': `Bearer ${userAuthToken}`
    }
  })
  return token.json()
}
```

### 2. CORS Configuration
```javascript
// Express middleware
app.use(cors({
  origin: process.env.ALLOWED_ORIGINS?.split(','),
  credentials: true
}))
```

### 3. Rate Limiting
```javascript
// Limit stream creation
const rateLimiter = rateLimit({
  windowMs: 15 * 60 * 1000, // 15 minutes
  max: 5 // 5 streams per window
})

app.post('/api/streams', rateLimiter, createStream)
```

## 📈 Monitoring

### Metrics to Track
- Active streams count
- Viewer count per stream
- Bandwidth usage
- Stream quality (bitrate, FPS)
- Error rates

### Recommended Tools
- **Prometheus** + **Grafana** for metrics
- **Sentry** for error tracking
- **CloudWatch** / **Datadog** for infrastructure

### Example Monitoring Setup
```yaml
# prometheus.yml
scrape_configs:
  - job_name: 'livekit'
    static_configs:
      - targets: ['livekit:6789']
        
  - job_name: 'app'
    static_configs:
      - targets: ['app:3000']
```

## 🆘 Troubleshooting Deployment

### WHIP Connection Failed
1. Check firewall rules for ports 80/443
2. Verify SSL certificates
3. Test with `curl`:
   ```bash
   curl -X POST https://your-whip/endpoint \
     -H "Content-Type: application/sdp" \
     -d "test"
   ```

### No Video/Audio
1. Check UDP ports 50000-60000 are open
2. Verify TURN server configuration
3. Test with LiveKit CLI:
   ```bash
   livekit-cli test-connection
   ```

### High Latency
1. Deploy LiveKit closer to users
2. Use regional TURN servers
3. Optimize encoding settings

## 💰 Cost Optimization

### Bandwidth Costs
- Use adaptive bitrate
- Set maximum quality limits
- Implement viewer limits

### Compute Costs
- Use auto-scaling groups
- Schedule downtime for demos
- Optimize container sizes

### Example Auto-scaling
```yaml
# AWS Auto Scaling
Resources:
  AutoScalingGroup:
    Type: AWS::AutoScaling::AutoScalingGroup
    Properties:
      MinSize: 1
      MaxSize: 10
      TargetGroupARNs:
        - !Ref TargetGroup
      HealthCheckType: ELB
      HealthCheckGracePeriod: 300
```

## 📚 Additional Resources

- [LiveKit Deployment Guide](https://docs.livekit.io/deploy)
- [WebRTC Infrastructure Planning](https://webrtc.org/getting-started/infrastructure)
- [Streaming Cost Calculator](https://calculator.aws/#/addService/MediaLive)

---

**Need help deploying?** Join our [Discord](https://discord.gg/substream) or email deploy@substream.dev
