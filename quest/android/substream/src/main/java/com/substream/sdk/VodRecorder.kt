package com.substream.sdk

import android.app.Activity
import android.content.Intent
import android.media.MediaCodecInfo
import android.media.MediaRecorder
import android.media.projection.MediaProjection
import android.media.projection.MediaProjectionManager
import android.os.Environment
import android.util.DisplayMetrics
import android.view.Surface
import java.io.File
import java.text.SimpleDateFormat
import java.util.Date
import java.util.Locale

class VodRecorder(private val activity: Activity) {
    private var mediaProjection: MediaProjection? = null
    private var mediaRecorder: MediaRecorder? = null
    private var virtualDisplay: android.hardware.display.VirtualDisplay? = null
    private var outputPath: String? = null

    fun start(
        projectionData: Intent,
        width: Int,
        height: Int,
        fps: Int,
        videoBitrateKbps: Int,
        withAudio: Boolean,
        outputHint: String,
        onSaved: (String) -> Unit
    ) {
        val mpm = activity.getSystemService(Activity.MEDIA_PROJECTION_SERVICE) as MediaProjectionManager
        mediaProjection = mpm.getMediaProjection(Activity.RESULT_OK, projectionData)

        val metrics = DisplayMetrics()
        activity.windowManager.defaultDisplay.getRealMetrics(metrics)

        val fileName = (if (outputHint.isNotEmpty()) outputHint + "-" else "") +
            SimpleDateFormat("yyyyMMdd-HHmmss", Locale.US).format(Date()) + ".mp4"
        val dir = activity.getExternalFilesDir(Environment.DIRECTORY_MOVIES) ?: activity.filesDir
        outputPath = File(dir, fileName).absolutePath

        val recorder = MediaRecorder()
        mediaRecorder = recorder
        recorder.setVideoSource(MediaRecorder.VideoSource.SURFACE)
        if (withAudio) {
            recorder.setAudioSource(MediaRecorder.AudioSource.MIC)
        }
        recorder.setOutputFormat(MediaRecorder.OutputFormat.MPEG_4)
        recorder.setOutputFile(outputPath)
        recorder.setVideoEncoder(MediaRecorder.VideoEncoder.H264)
        recorder.setVideoEncodingBitRate(videoBitrateKbps * 1000)
        recorder.setVideoFrameRate(fps)
        recorder.setVideoSize(width, height)
        if (withAudio) {
            recorder.setAudioEncoder(MediaRecorder.AudioEncoder.AAC)
            recorder.setAudioEncodingBitRate(128000)
            recorder.setAudioChannels(2)
            recorder.setAudioSamplingRate(44100)
        }
        recorder.prepare()

        val surface: Surface = recorder.surface
        virtualDisplay = mediaProjection!!.createVirtualDisplay(
            "SubstreamVOD",
            width,
            height,
            metrics.densityDpi,
            0,
            surface,
            null,
            null
        )

        recorder.start()
    }

    fun stop(): String {
        val path = outputPath ?: ""
        try {
            mediaRecorder?.stop()
        } finally {
            mediaRecorder?.reset()
            mediaRecorder?.release()
            mediaRecorder = null
            virtualDisplay?.release(); virtualDisplay = null
            mediaProjection?.stop(); mediaProjection = null
        }
        return path
    }
}