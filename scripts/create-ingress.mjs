#!/usr/bin/env node
// Create a LiveKit WHIP ingress and print the publish URL

const {
  LIVEKIT_URL,
  LIVEKIT_API_KEY,
  LIVEKIT_API_SECRET,
  ROOM_NAME = 'parentconnect-demo',
  PARTICIPANT_IDENTITY = 'publisher-web',
  INGRESS_NAME = 'substream-web-demo'
} = process.env

if (!LIVEKIT_URL || !LIVEKIT_API_KEY || !LIVEKIT_API_SECRET) {
  console.error('Missing LIVEKIT_URL, LIVEKIT_API_KEY, or LIVEKIT_API_SECRET env vars')
  process.exit(1)
}

const { IngressClient } = await import('livekit-server-sdk')
const { IngressInput } = await import('@livekit/protocol')

const host = LIVEKIT_URL.startsWith('wss:')
  ? LIVEKIT_URL.replace(/^wss:/, 'https:')
  : LIVEKIT_URL.startsWith('ws:')
    ? LIVEKIT_URL.replace(/^ws:/, 'http:')
    : LIVEKIT_URL

const client = new IngressClient(host, LIVEKIT_API_KEY, LIVEKIT_API_SECRET)

// If an ingress with same name exists, delete it to avoid conflicts
const all = await client.listIngress()
for (const ig of all) {
  if (ig.name === INGRESS_NAME) {
    try { await client.deleteIngress(ig.ingressId) } catch {}
  }
}

const opts = {
  name: INGRESS_NAME,
  roomName: ROOM_NAME,
  participantIdentity: PARTICIPANT_IDENTITY
}

const info = await client.createIngress(IngressInput.WHIP_INPUT, opts)
console.log('Created WHIP ingress:')
console.log(JSON.stringify(info, null, 2))
if (!info.url) {
  console.error('No WHIP URL returned; check LiveKit config')
  process.exit(2)
}
console.log('\nWHIP publish URL:')
console.log(info.url)


