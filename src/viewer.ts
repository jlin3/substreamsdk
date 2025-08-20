import { Room, RoomEvent, RemoteParticipant, RemoteTrackPublication, RemoteTrack } from 'livekit-client'

async function fetchToken() {
  const res = await fetch('/token')
  if (!res.ok) throw new Error('Token endpoint error')
  return res.json() as Promise<{ token: string, url: string }>
}

function log(msg: string) {
  const el = document.getElementById('viewerStatus')!
  el.textContent = msg
}

async function main() {
  const btn = document.getElementById('btnJoin') as HTMLButtonElement
  const video = document.getElementById('video') as HTMLVideoElement

  let room: Room | null = null

  btn.onclick = async () => {
    btn.disabled = true
    try {
      const { token, url } = await fetchToken()
      room = new Room()

      room.on(RoomEvent.TrackSubscribed, (_track: RemoteTrack, _pub: RemoteTrackPublication, participant: RemoteParticipant) => {
        const tracks = Array.from(participant.videoTracks.values())
        if (tracks[0]?.track) {
          const mediaStream = new MediaStream([tracks[0].track.mediaStreamTrack])
          video.srcObject = mediaStream
          video.play().catch(() => {})
        }
      })

      await room.connect(url.replace(/^wss:/, 'wss:'), token)
      log('Viewer joined, waiting for video...')
    } catch (e: any) {
      log(`Join error: ${e?.message ?? e}`)
      btn.disabled = false
    }
  }
}

main()


