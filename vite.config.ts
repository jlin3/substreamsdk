import { defineConfig } from 'vite'

export default defineConfig({
  server: {
    port: 5173,
    proxy: {
      '/token': {
        target: 'http://localhost:8787',
        changeOrigin: true
      },
      '/pc/token': {
        target: 'http://localhost:8787',
        changeOrigin: true
      }
    }
  },
  build: {
    rollupOptions: {
      input: {
        main: 'index.html',
        viewer: 'viewer.html',
        demo: 'demo.html',
        demoViewer: 'demo-viewer.html',
        questDemo: 'quest-demo.html'
      }
    }
  }
})


