package com.substream.sdk

import android.app.Activity
import android.content.Intent
import android.media.projection.MediaProjectionManager
import android.os.Bundle

class ProjectionActivity : Activity() {
    private val requestCode = 9001

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        val mpm = getSystemService(MEDIA_PROJECTION_SERVICE) as MediaProjectionManager
        startActivityForResult(mpm.createScreenCaptureIntent(), requestCode)
    }

    override fun onActivityResult(reqCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(reqCode, resultCode, data)
        if (reqCode == requestCode && resultCode == RESULT_OK && data != null) {
            SubstreamNative.setProjectionData(data)
        }
        finish()
    }
}


