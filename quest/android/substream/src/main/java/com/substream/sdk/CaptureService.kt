package com.substream.sdk

import android.app.Notification
import android.app.NotificationChannel
import android.app.NotificationManager
import android.app.PendingIntent
import android.app.Service
import android.content.Context
import android.content.Intent
import android.os.Build
import android.os.IBinder
import android.util.Log

class CaptureService : Service() {
    companion object {
        private const val CHANNEL_ID = "substream_capture"
        private const val NOTIFICATION_ID = 42
        private const val TAG = "CaptureService"
    }

    override fun onBind(intent: Intent?): IBinder? = null

    override fun onCreate() {
        super.onCreate()
        Log.i(TAG, "Service created")
        createNotificationChannel()
    }

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
        Log.i(TAG, "Starting foreground service")
        
        val width = intent?.getIntExtra("width", 1280) ?: 1280
        val height = intent?.getIntExtra("height", 720) ?: 720
        val fps = intent?.getIntExtra("fps", 30) ?: 30
        
        startForeground(NOTIFICATION_ID, buildNotification(width, height, fps))
        
        // Handle stop action
        if (intent?.action == "STOP_CAPTURE") {
            Log.i(TAG, "Stopping service via notification action")
            SubstreamNative.stopLive()
            stopSelf()
        }
        
        return START_STICKY
    }

    override fun onDestroy() {
        Log.i(TAG, "Service destroyed")
        super.onDestroy()
    }

    private fun createNotificationChannel() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            val channel = NotificationChannel(
                CHANNEL_ID,
                "Screen Capture",
                NotificationManager.IMPORTANCE_LOW
            ).apply {
                description = "Shows when streaming your screen"
                setShowBadge(false)
                enableLights(false)
                enableVibration(false)
            }
            
            val notificationManager = getSystemService(Context.NOTIFICATION_SERVICE) as NotificationManager
            notificationManager.createNotificationChannel(channel)
        }
    }

    private fun buildNotification(width: Int, height: Int, fps: Int): Notification {
        // Create stop action
        val stopIntent = Intent(this, CaptureService::class.java).apply {
            action = "STOP_CAPTURE"
        }
        
        val stopPendingIntent = if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            PendingIntent.getService(
                this, 0, stopIntent,
                PendingIntent.FLAG_UPDATE_CURRENT or PendingIntent.FLAG_IMMUTABLE
            )
        } else {
            PendingIntent.getService(
                this, 0, stopIntent,
                PendingIntent.FLAG_UPDATE_CURRENT
            )
        }

        val builder = if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            Notification.Builder(this, CHANNEL_ID)
        } else {
            Notification.Builder(this)
        }
        
        return builder
            .setContentTitle("🔴 Streaming Active")
            .setContentText("${width}x${height} @ ${fps}fps")
            .setSmallIcon(android.R.drawable.ic_media_play)
            .setOngoing(true)
            .also { b ->
                // Add stop action for API 23+
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
                    b.addAction(
                        Notification.Action.Builder(
                            android.R.drawable.ic_media_pause,
                            "Stop Stream",
                            stopPendingIntent
                        ).build()
                    )
                }
            }
            .build()
    }
}


