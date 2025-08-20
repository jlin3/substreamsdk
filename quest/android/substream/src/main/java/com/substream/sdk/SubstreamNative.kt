package com.substream.sdk

import android.app.Activity
import android.content.Context
import android.content.Intent
import android.media.projection.MediaProjection
import android.media.projection.MediaProjectionManager
import android.util.Log
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch

object SubstreamNative {
    private const val TAG = "SubstreamNative"
    private const val REQUEST_CODE_PROJECTION = 1001

    private var activity: Activity? = null
    private var baseUrl: String = ""
    private var whipUrl: String = ""
    private var publisher: RtcPublisher? = null
    private var projectionData: Intent? = null
    private var streamConfig: StreamConfig? = null
    private val scope = CoroutineScope(SupervisorJob() + Dispatchers.Main)
    
    // Callbacks for Unity
    private var onStatusCallback: ((String) -> Unit)? = null
    private var onErrorCallback: ((String) -> Unit)? = null

    data class StreamConfig(
        val width: Int,
        val height: Int,
        val fps: Int,
        val videoBitrateKbps: Int,
        val metadataJson: String
    )

    @JvmStatic
    fun init(context: Context, baseUrl: String, whipPublishUrl: String) {
        Log.i(TAG, "init baseUrl=$baseUrl whip=$whipPublishUrl")
        this.activity = context as? Activity
        this.baseUrl = baseUrl
        this.whipUrl = whipPublishUrl.ifEmpty { 
            "$baseUrl/whip/publish" 
        }
    }

    @JvmStatic
    fun startLive(width: Int, height: Int, fps: Int, videoBitrateKbps: Int, metadataJson: String) {
        Log.i(TAG, "startLive ${width}x${height}@${fps} ${videoBitrateKbps}kbps meta=$metadataJson")
        
        streamConfig = StreamConfig(width, height, fps, videoBitrateKbps, metadataJson)
        
        val act = activity ?: run {
            onErrorCallback?.invoke("Activity not initialized")
            return
        }
        
        // Request MediaProjection permission if not already granted
        if (projectionData == null) {
            requestProjectionPermission(act)
            return
        }
        
        // Start streaming with existing permission
        startStreamingInternal(act)
    }

    private fun requestProjectionPermission(activity: Activity) {
        try {
            val projectionManager = activity.getSystemService(Context.MEDIA_PROJECTION_SERVICE) as MediaProjectionManager
            val intent = projectionManager.createScreenCaptureIntent()
            
            // For Quest/Android, we need to handle this differently
            // In Unity, this would trigger the permission dialog
            activity.startActivityForResult(intent, REQUEST_CODE_PROJECTION)
            onStatusCallback?.invoke("requesting_permission")
        } catch (e: Exception) {
            Log.e(TAG, "Failed to request projection", e)
            onErrorCallback?.invoke("Failed to request screen capture permission: ${e.message}")
        }
    }

    private fun startStreamingInternal(activity: Activity) {
        scope.launch {
            try {
                onStatusCallback?.invoke("starting")
                
                // Start foreground service for background capture
                val serviceIntent = Intent(activity, CaptureService::class.java).apply {
                    putExtra("width", streamConfig?.width ?: 1280)
                    putExtra("height", streamConfig?.height ?: 720)
                    putExtra("fps", streamConfig?.fps ?: 30)
                }
                activity.startForegroundService(serviceIntent)
                
                // Initialize WebRTC publisher
                publisher = RtcPublisher(activity, whipUrl) { resourceUrl ->
                    Log.i(TAG, "Stream resource: $resourceUrl")
                }
                
                streamConfig?.let { config ->
                    publisher?.start(activity, config.width, config.height, config.fps, projectionData!!)
                    onStatusCallback?.invoke("streaming")
                }
                
            } catch (e: Exception) {
                Log.e(TAG, "Failed to start streaming", e)
                onErrorCallback?.invoke("Streaming failed: ${e.message}")
            }
        }
    }

    @JvmStatic
    fun stopLive() {
        Log.i(TAG, "stopLive")
        scope.launch {
            try {
                onStatusCallback?.invoke("stopping")
                publisher?.stop()
                publisher = null
                activity?.stopService(Intent(activity, CaptureService::class.java))
                onStatusCallback?.invoke("stopped")
            } catch (e: Exception) {
                Log.e(TAG, "Error stopping stream", e)
                onErrorCallback?.invoke("Failed to stop: ${e.message}")
            }
        }
    }

    @JvmStatic
    fun handleActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        if (requestCode == REQUEST_CODE_PROJECTION) {
            if (resultCode == Activity.RESULT_OK && data != null) {
                projectionData = data
                onStatusCallback?.invoke("permission_granted")
                
                // Automatically start streaming after permission granted
                activity?.let { startStreamingInternal(it) }
            } else {
                onErrorCallback?.invoke("Screen capture permission denied")
            }
        }
    }

    @JvmStatic
    fun setStatusCallback(callback: (String) -> Unit) {
        onStatusCallback = callback
    }

    @JvmStatic
    fun setErrorCallback(callback: (String) -> Unit) {
        onErrorCallback = callback
    }

    // Demo mode support
    @JvmStatic
    fun initDemo() {
        init(
            activity ?: return,
            "http://10.0.2.2:8787", // Android emulator localhost
            "http://10.0.2.2:8080/whip"
        )
    }
}


