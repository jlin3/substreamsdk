#!/bin/bash
# Start Substream Demo - One-click setup

echo "🚀 Starting Substream SDK Demo..."

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker Desktop first."
    exit 1
fi

# Check if ports are available
for port in 7880 7881 7882 8080 5173; do
    if lsof -Pi :$port -sTCP:LISTEN -t >/dev/null 2>&1; then
        echo "⚠️  Port $port is already in use. Please free it up first."
        exit 1
    fi
done

# Start LiveKit services
echo "📡 Starting LiveKit server and WHIP ingress..."
if docker compose up -d 2>/dev/null; then
    echo "⏳ Waiting for services to start..."
    sleep 8
    
    # Check if LiveKit is healthy
    if curl -f http://localhost:7880/ > /dev/null 2>&1; then
        echo "✅ LiveKit server is running"
        
        # Check if ingress is healthy (optional)
        if curl -f http://localhost:8089/health > /dev/null 2>&1; then
            echo "✅ WHIP ingress is running"
        else
            echo "⚠️  WHIP ingress not available, using direct LiveKit connection"
        fi
    else
        echo "❌ LiveKit server failed to start. Check docker logs with: docker compose logs"
        exit 1
    fi
else
    echo "⚠️  Full setup failed, trying simplified version..."
    docker compose -f docker-compose.simple.yml up -d
    
    echo "⏳ Waiting for LiveKit to start..."
    for i in {1..10}; do
        if curl -f http://localhost:7880/ > /dev/null 2>&1; then
            echo "✅ LiveKit server running (simplified mode)"
            break
        fi
        echo -n "."
        sleep 2
    done
    
    if ! curl -f http://localhost:7880/ > /dev/null 2>&1; then
        echo ""
        echo "❌ Could not start LiveKit. Checking logs..."
        docker compose -f docker-compose.simple.yml logs --tail 20
        exit 1
    fi
fi

# Install dependencies if needed
if [ ! -d "node_modules" ]; then
    echo "📦 Installing dependencies..."
    npm install
fi

# Create .env.local with demo settings
echo "⚙️  Configuring demo environment..."
cat > .env.local << EOF
VITE_PARENTCONNECT_BASE_URL=http://localhost:8787
VITE_WHIP_PUBLISH_URL=http://localhost:8080/whip
EOF

# Start the demo server
echo "🎮 Starting demo server..."
npm run dev &

# Wait a moment for the server to start
sleep 2

echo ""
echo "✨ Substream SDK Demo is ready!"
echo ""
echo "📺 Demo UI:     http://localhost:5173/demo.html"
echo "🎮 Game Demo:   http://localhost:5173/"
echo "👀 Viewer:      http://localhost:5173/viewer.html"
echo ""
echo "To stop: Press Ctrl+C and run 'docker compose down'"
echo ""

# Keep script running
wait
