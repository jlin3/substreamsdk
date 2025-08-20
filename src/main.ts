import { Substream } from './sdk'
import { startGame } from './game'

const env = {
  baseUrl: import.meta.env.VITE_PARENTCONNECT_BASE_URL as string,
  issuer: import.meta.env.VITE_STREAM_ISSUER as string,
  whipPublishUrl: import.meta.env.VITE_WHIP_PUBLISH_URL as string | undefined,
  livekitUrl: import.meta.env.VITE_LIVEKIT_URL as string | undefined,
  livekitApiKey: import.meta.env.VITE_LIVEKIT_API_KEY as string | undefined,
  livekitApiSecret: import.meta.env.VITE_LIVEKIT_API_SECRET as string | undefined
}

function requireEnv(name: keyof typeof env) {
  const v = env[name]
  if (!v) throw new Error(`Missing ${name}. Set it in .env.local`)
  return v
}

function drawDemo(canvas: HTMLCanvasElement) {
  const ctx = canvas.getContext('2d')!
  let t = 0
  function frame() {
    t += 1
    const w = canvas.width
    const h = canvas.height
    ctx.fillStyle = '#0b0f17'
    ctx.fillRect(0, 0, w, h)
    const x = Math.round((Math.sin(t / 30) * 0.4 + 0.5) * (w - 120))
    const y = Math.round((Math.cos(t / 40) * 0.4 + 0.5) * (h - 120))
    ctx.fillStyle = '#2563eb'
    ctx.fillRect(x, y, 120, 120)
    ctx.fillStyle = '#e6edf3'
    ctx.font = '16px ui-monospace, SFMono-Regular, Menlo, Monaco'
    ctx.fillText('Substream Demo', 16, 28)
    requestAnimationFrame(frame)
  }
  frame()
}

async function tokenProvider() {
  // Demo: fetch a short-lived token from the mock server
  try {
    const res = await fetch('/pc/token')
    if (res.ok) {
      return await res.json()
    }
  } catch {}
  const raw = localStorage.getItem('SUBSTREAM_BEARER') || ''
  return { accessToken: raw, expiresAt: Date.now() + 10 * 60 * 1000 }
}

function setStatus(s: string) {
  const el = document.getElementById('status')!
  el.textContent = s
}

async function main() {
  const baseUrl = requireEnv('baseUrl')
  await Substream.init({ baseUrl, tokenProvider, streamIssuer: env.issuer })

  const canvas = document.getElementById('game') as HTMLCanvasElement
  startGame(canvas)

  const btnLive = document.getElementById('btnLive') as HTMLButtonElement
  const btnStopLive = document.getElementById('btnStopLive') as HTMLButtonElement
  const btnVod = document.getElementById('btnVod') as HTMLButtonElement
  const btnStopVod = document.getElementById('btnStopVod') as HTMLButtonElement

  let live: Awaited<ReturnType<typeof Substream.startLive>> | null = null
  let vod: Awaited<ReturnType<typeof Substream.startVod>> | null = null

  btnLive.onclick = async () => {
    try {
      setStatus('Connecting live...')
      btnLive.disabled = true
      live = await Substream.startLive({
        source: { type: 'canvas', canvas, fps: 30, withAudio: true },
        metadata: { demo: true },
        whipPublishUrl: env.whipPublishUrl
      })
      btnStopLive.disabled = false
      setStatus('LIVE started')
    } catch (e: any) {
      setStatus(`Live error: ${e?.message ?? e}`)
      btnLive.disabled = false
    }
  }

  btnStopLive.onclick = async () => {
    try {
      setStatus('Stopping live...')
      btnStopLive.disabled = true
      await live?.stop()
      setStatus('Live stopped')
    } catch (e: any) {
      setStatus(`Stop error: ${e?.message ?? e}`)
    } finally {
      btnLive.disabled = false
    }
  }

  btnVod.onclick = async () => {
    try {
      setStatus('Recording VOD...')
      btnVod.disabled = true
      vod = await Substream.startVod({
        source: { type: 'canvas', canvas, fps: 60, withAudio: true },
        chunkMs: 6000,
        metadata: { demo: true }
      })
      btnStopVod.disabled = false
    } catch (e: any) {
      setStatus(`VOD error: ${e?.message ?? e}`)
      btnVod.disabled = false
    }
  }

  btnStopVod.onclick = async () => {
    try {
      setStatus('Finalizing upload...')
      btnStopVod.disabled = true
      await vod?.stopAndUpload()
      setStatus('VOD uploaded & finalized')
    } catch (e: any) {
      setStatus(`Finalize error: ${e?.message ?? e}`)
    } finally {
      btnVod.disabled = false
    }
  }

  const envInfo = document.getElementById('envInfo')!
  envInfo.textContent = `BASE=${baseUrl}`
}

main().catch(err => {
  console.error(err)
  setStatus(String(err?.message ?? err))
})


