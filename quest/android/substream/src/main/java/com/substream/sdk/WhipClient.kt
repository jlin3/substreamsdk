package com.substream.sdk

import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.RequestBody.Companion.toRequestBody

class WhipClient(private val baseOkHttp: OkHttpClient = OkHttpClient()) {
    suspend fun publish(whipUrl: String, sdpOffer: String): Pair<String, String?> = withContext(Dispatchers.IO) {
        val req = Request.Builder()
            .url(whipUrl)
            .post(sdpOffer.toRequestBody("application/sdp".toMediaType()))
            .build()
        baseOkHttp.newCall(req).execute().use { resp ->
            if (!resp.isSuccessful) error("WHIP publish failed ${resp.code}")
            val answer = resp.body?.string() ?: ""
            val location = resp.header("Location")
            return@use Pair(answer, location)
        }
    }

    suspend fun delete(resourceUrl: String) = withContext(Dispatchers.IO) {
        val req = Request.Builder().url(resourceUrl).delete().build()
        baseOkHttp.newCall(req).execute().use { }
    }
}


