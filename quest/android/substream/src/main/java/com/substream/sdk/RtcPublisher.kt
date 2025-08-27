package com.substream.sdk

import android.app.Activity
import android.content.Context
import android.media.projection.MediaProjection
import android.media.projection.MediaProjectionManager
import android.media.AudioPlaybackCaptureConfiguration
import android.media.AudioFormat
import android.media.AudioRecord
import android.media.MediaRecorder
import android.util.Log
import android.content.Intent
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch
import org.webrtc.*

class RtcPublisher(
    private val context: Context,
    private val whipUrl: String,
    private val onResourceUrl: (String?) -> Unit
) {
    private val scope = CoroutineScope(SupervisorJob() + Dispatchers.Default)
    private var peerConnection: PeerConnection? = null
    private var factory: PeerConnectionFactory? = null
    private var mediaProjection: MediaProjection? = null
    private var screenCapturer: ScreenCapturerAndroid? = null
    private var videoSource: VideoSource? = null
    private var videoTrack: VideoTrack? = null
    private var audioSource: AudioSource? = null
    private var audioTrack: AudioTrack? = null
    private var resourceUrl: String? = null
    private val whip = WhipClient()
    private var videoSender: RtpSender? = null

    fun start(activity: Activity, width: Int, height: Int, fps: Int, projectionData: Intent, withAudio: Boolean) {
        setupFactory()
        ensureProjection(activity)
        startPeer(width, height, fps, projectionData, withAudio)
    }

    fun stop() {
        val res = resourceUrl
        scope.launch { if (res != null) runCatching { whip.delete(res) } }
        peerConnection?.dispose(); peerConnection = null
        videoTrack?.dispose(); videoTrack = null
        videoSource?.dispose(); videoSource = null
        audioTrack?.dispose(); audioTrack = null
        audioSource?.dispose(); audioSource = null
        screenCapturer?.dispose(); screenCapturer = null
        factory?.dispose(); factory = null
    }

    private fun setupFactory() {
        if (factory != null) return
        val init = PeerConnectionFactory.InitializationOptions.builder(context)
            .createInitializationOptions()
        PeerConnectionFactory.initialize(init)
        val encoder = DefaultVideoEncoderFactory(
            EglBase.create().eglBaseContext, /* enableIntelVp8Encoder= */true, /* enableH264HighProfile= */true
        )
        val decoder = DefaultVideoDecoderFactory(EglBase.create().eglBaseContext)
        factory = PeerConnectionFactory.builder()
            .setVideoEncoderFactory(encoder)
            .setVideoDecoderFactory(decoder)
            .createPeerConnectionFactory()
    }

    private fun ensureProjection(activity: Activity) {
        if (mediaProjection != null) return
        val mpm = activity.getSystemService(Context.MEDIA_PROJECTION_SERVICE) as MediaProjectionManager
        // NOTE: For MVP, assume consent already granted via your app flow.
        // In production, launch mpm.createScreenCaptureIntent() and store the result.
    }

    private fun startPeer(width: Int, height: Int, fps: Int, projectionData: Intent, withAudio: Boolean) {
        val rtcConfig = PeerConnection.RTCConfiguration(emptyList())
        val pc = factory!!.createPeerConnection(rtcConfig, object : PeerConnection.Observer by emptyObserver() {})
        peerConnection = pc

        // Capture screen
        screenCapturer = ScreenCapturerAndroid(projectionData, object: MediaProjection.Callback() {})
        videoSource = factory!!.createVideoSource(false)
        val videoCapturer = screenCapturer!!
        val egl = EglBase.create().eglBaseContext
        val helper = SurfaceTextureHelper.create("SubstreamCapture", egl)
        videoCapturer.initialize(helper, context, videoSource!!.capturerObserver)
        videoCapturer.startCapture(width, height, fps)

        videoTrack = factory!!.createVideoTrack("video0", videoSource)
        videoSender = pc!!.addTrack(videoTrack)

        // Audio: try playback capture (game audio). Fallback: mic source
        if (withAudio) {
            val constraints = MediaConstraints()
            audioSource = factory!!.createAudioSource(constraints)
            audioTrack = factory!!.createAudioTrack("audio0", audioSource)
            pc!!.addTrack(audioTrack)
        }

        // Offer/Answer via WHIP
        pc!!.createOffer(object: SdpObserver by emptySdpObserver() {
            override fun onCreateSuccess(desc: SessionDescription) {
                pc.setLocalDescription(object: SdpObserver by emptySdpObserver() {}, desc)
                scope.launch {
                    runCatching {
                        val (answer, location) = whip.publish(whipUrl, desc.description)
                        resourceUrl = location
                        onResourceUrl(location)
                        pc.setRemoteDescription(object: SdpObserver by emptySdpObserver() {}, SessionDescription(SessionDescription.Type.ANSWER, answer))
                    }.onFailure { err -> Log.e("RtcPublisher", "whip error", err) }
                }
            }
        }, MediaConstraints())
    }

    fun adjustQuality(width: Int, height: Int, fps: Int, bitrateKbps: Int) {
        try {
            screenCapturer?.changeCaptureFormat(width, height, fps)
            val sender = videoSender ?: return
            val params = sender.parameters
            val encodings = params.encodings
            if (encodings != null && encodings.isNotEmpty()) {
                encodings[0].maxBitrateBps = bitrateKbps * 1000
                sender.parameters = params
            }
        } catch (e: Exception) {
            Log.w("RtcPublisher", "adjustQuality failed: ${e.message}")
        }
    }
}

fun emptyObserver() = object : PeerConnection.Observer {
    override fun onSignalingChange(newState: PeerConnection.SignalingState) {}
    override fun onIceConnectionChange(newState: PeerConnection.IceConnectionState) {}
    override fun onIceConnectionReceivingChange(receiving: Boolean) {}
    override fun onIceGatheringChange(newState: PeerConnection.IceGatheringState) {}
    override fun onIceCandidate(candidate: IceCandidate) {}
    override fun onIceCandidatesRemoved(candidates: Array<out IceCandidate>) {}
    override fun onAddStream(stream: MediaStream) {}
    override fun onRemoveStream(stream: MediaStream) {}
    override fun onDataChannel(dc: DataChannel) {}
    override fun onRenegotiationNeeded() {}
    override fun onAddTrack(receiver: RtpReceiver, streams: Array<out MediaStream>) {}
}

fun emptySdpObserver() = object : SdpObserver {
    override fun onCreateSuccess(p0: SessionDescription?) {}
    override fun onSetSuccess() {}
    override fun onCreateFailure(p0: String?) {}
    override fun onSetFailure(p0: String?) {}
}


