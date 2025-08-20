// Minimal mock server for VOD + WHIP to demo end-to-end
// Not a full WHIP/WebRTC stack — WHIP simply echoes back the offer for demo wiring
import express from 'express'
import cors from 'cors'
import multer from 'multer'
import { AccessToken } from 'livekit-server-sdk'
import jwt from 'jsonwebtoken'

const app = express()
app.use(cors())
app.use(express.json())
app.use(express.text({ type: ['application/sdp', 'text/plain'] }))

const upload = multer({ storage: multer.memoryStorage() })

// Simple in-memory store
const sessions = new Map()

// WHIP publish mock: returns same SDP and a resource Location
app.post('/whip/publish', (req, res) => {
  const id = Math.random().toString(36).slice(2)
  sessions.set(`whip:${id}`, { createdAt: Date.now() })
  const sdp = req.body || ''
  res.setHeader('Location', `/whip/publish/${id}`)
  return res.status(200).type('application/sdp').send(sdp)
})

app.delete('/whip/publish/:id', (req, res) => {
  sessions.delete(`whip:${req.params.id}`)
  return res.status(204).end()
})

// VOD sessions
app.post('/vod/sessions', (req, res) => {
  const id = Math.random().toString(36).slice(2)
  sessions.set(`vod:${id}`, { chunks: [], finalized: false })
  const uploadUrl = `/vod/upload/${id}`
  return res.json({ sessionId: id, uploadUrl })
})

app.post('/vod/upload/:id', upload.single('file'), (req, res) => {
  const key = `vod:${req.params.id}`
  const ses = sessions.get(key)
  if (!ses) return res.status(404).json({ error: 'no session' })
  ses.chunks.push({ size: req.file?.size || 0, at: Date.now() })
  return res.json({ ok: true })
})

app.post('/vod/sessions/:id/finalize', (req, res) => {
  const key = `vod:${req.params.id}`
  const ses = sessions.get(key)
  if (!ses) return res.status(404).json({ error: 'no session' })
  ses.finalized = true
  return res.json({ ok: true, chunks: ses.chunks.length })
})

// Create a WHIP ingress via LiveKit API (for demos)
app.post('/ingress/whip', async (req, res) => {
  try {
    const { IngressClient } = await import('livekit-server-sdk')
    const { IngressInput } = await import('@livekit/protocol')
    const url = process.env.LIVEKIT_URL?.replace(/^wss:/, 'https:').replace(/^ws:/, 'http:')
    if (!url) return res.status(500).json({ error: 'Missing LIVEKIT_URL' })
    const client = new IngressClient(url, process.env.LIVEKIT_API_KEY, process.env.LIVEKIT_API_SECRET)
    const info = await client.createIngress(IngressInput.WHIP_INPUT, {
      roomName: (req.body?.roomName || 'parentconnect-demo'),
      participantIdentity: (req.body?.identity || 'publisher-web')
    })
    return res.json({ url: info.url, ingressId: info.ingressId })
  } catch (e) {
    console.error(e)
    return res.status(500).json({ error: String(e) })
  }
})

const port = Number(process.env.MOCK_PORT || 8787)

// Issue a LiveKit room join token for viewer demo
app.get('/token', (req, res) => {
  const roomName = (req.query.room || 'parentconnect-demo') + ''
  const identity = req.query.identity || `viewer-${Math.random().toString(36).slice(2)}`
  const url = process.env.LIVEKIT_URL
  const key = process.env.LIVEKIT_API_KEY
  const secret = process.env.LIVEKIT_API_SECRET
  if (!url || !key || !secret) return res.status(500).json({ error: 'Missing LIVEKIT_URL/API_KEY/API_SECRET' })
  const at = new AccessToken(key, secret, { identity: String(identity) })
  at.addGrant({ room: roomName, roomJoin: true, canSubscribe: true, canPublish: false })
  const token = at.toJwt()
  return res.json({ token, url })
})

// Issue a Parent Connect-style short-lived JWT for SDK calls (demo only)
app.get('/pc/token', (req, res) => {
  const secret = process.env.PARENTCONNECT_DEMO_SECRET || 'demo-secret'
  const payload = {
    sub: 'demo-sdk',
    scopes: ['live:publish', 'vod:upload', 'telemetry:write'],
    exp: Math.floor(Date.now() / 1000) + 15 * 60
  }
  const token = jwt.sign(payload, secret)
  res.json({ accessToken: token, expiresAt: Date.now() + 15 * 60 * 1000 })
})

app.listen(port, () => {
  console.log(`Mock server running on http://localhost:${port}`)
})


