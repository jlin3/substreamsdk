type Vec2 = { x: number; y: number }

export function startGame(canvas: HTMLCanvasElement) {
  const ctx = canvas.getContext('2d')!
  const size = { w: canvas.width, h: canvas.height }

  const keys = new Set<string>()
  window.addEventListener('keydown', (e) => keys.add(e.key))
  window.addEventListener('keyup', (e) => keys.delete(e.key))

  const player = { pos: { x: 60, y: 60 } as Vec2, vel: { x: 0, y: 0 } as Vec2, speed: 2.4 }
  const enemies = Array.from({ length: 5 }).map((_, i) => ({
    pos: { x: 120 + i * 80, y: 100 + (i % 2) * 80 } as Vec2,
    vel: { x: (Math.random() * 2 - 1) * 1.6, y: (Math.random() * 2 - 1) * 1.6 } as Vec2,
    r: 18
  }))
  let score = 0
  let lastHitAt = 0

  const audioCtx = new (window.AudioContext || (window as any).webkitAudioContext)()
  function beep(frequency = 660, durationMs = 120) {
    const osc = audioCtx.createOscillator()
    const gain = audioCtx.createGain()
    osc.type = 'square'
    osc.frequency.value = frequency
    gain.gain.value = 0.06
    osc.connect(gain)
    gain.connect(audioCtx.destination)
    osc.start()
    setTimeout(() => { osc.stop(); osc.disconnect(); gain.disconnect() }, durationMs)
  }

  function clamp(v: number, min: number, max: number) { return Math.max(min, Math.min(max, v)) }

  function step() {
    // Input
    player.vel.x = 0; player.vel.y = 0
    if (keys.has('ArrowLeft') || keys.has('a')) player.vel.x -= player.speed
    if (keys.has('ArrowRight') || keys.has('d')) player.vel.x += player.speed
    if (keys.has('ArrowUp') || keys.has('w')) player.vel.y -= player.speed
    if (keys.has('ArrowDown') || keys.has('s')) player.vel.y += player.speed
    player.pos.x = clamp(player.pos.x + player.vel.x, 10, size.w - 10 - 24)
    player.pos.y = clamp(player.pos.y + player.vel.y, 10, size.h - 10 - 24)

    // Enemies move and bounce
    for (const e of enemies) {
      e.pos.x += e.vel.x
      e.pos.y += e.vel.y
      if (e.pos.x < e.r || e.pos.x > size.w - e.r) e.vel.x *= -1
      if (e.pos.y < e.r || e.pos.y > size.h - e.r) e.vel.y *= -1
    }

    // Collisions
    let hit = false
    for (const e of enemies) {
      const dx = (player.pos.x + 12) - e.pos.x
      const dy = (player.pos.y + 12) - e.pos.y
      if (dx * dx + dy * dy < (e.r + 12) * (e.r + 12)) { hit = true; break }
    }
    if (hit) {
      if (performance.now() - lastHitAt > 800) { beep(280, 150); lastHitAt = performance.now() }
      score = Math.max(0, score - 1)
    } else {
      score += 1
    }

    // Render
    ctx.fillStyle = '#0b0f17'
    ctx.fillRect(0, 0, size.w, size.h)
    ctx.strokeStyle = '#1e2a3a'
    ctx.lineWidth = 2
    ctx.strokeRect(10, 10, size.w - 20, size.h - 20)

    // Player
    ctx.fillStyle = '#10b981'
    ctx.fillRect(player.pos.x, player.pos.y, 24, 24)

    // Enemies
    ctx.fillStyle = '#ef4444'
    for (const e of enemies) {
      ctx.beginPath(); ctx.arc(e.pos.x, e.pos.y, e.r, 0, Math.PI * 2); ctx.fill()
    }

    // HUD
    ctx.fillStyle = '#e6edf3'
    ctx.font = '14px ui-monospace, SFMono-Regular, Menlo, Monaco'
    ctx.fillText(`Score: ${score}`, 20, 28)
    ctx.fillText('Move with WASD/Arrows', 20, 48)

    requestAnimationFrame(step)
  }

  requestAnimationFrame(step)
}


