plugins {
    id("com.android.library")
    kotlin("android")
}

android {
    namespace = "com.substream.sdk"
    compileSdk = 34

    defaultConfig {
        minSdk = 29
        targetSdk = 34
        consumerProguardFiles("consumer-rules.pro")
    }

    buildTypes {
        release {
            isMinifyEnabled = false
        }
        debug {
            isMinifyEnabled = false
        }
    }
}

dependencies {
    implementation("androidx.core:core-ktx:1.13.1")
    implementation("org.webrtc:google-webrtc:1.0.32006")
    implementation("com.squareup.okhttp3:okhttp:4.12.0")
    implementation("org.jetbrains.kotlinx:kotlinx-coroutines-android:1.8.1")
}


