#!/bin/bash

# Android Build Setup Checker for Substream SDK
echo "🔍 Checking Android build environment..."
echo "========================================"

# Check Java
echo -n "✓ Java: "
if command -v java &> /dev/null; then
    java -version 2>&1 | head -n 1
else
    echo "❌ NOT FOUND - Install via Android Studio"
    exit 1
fi

# Check ANDROID_HOME
echo -n "✓ ANDROID_HOME: "
if [ -z "$ANDROID_HOME" ]; then
    # Try common locations
    if [ -d "$HOME/Library/Android/sdk" ]; then
        export ANDROID_HOME="$HOME/Library/Android/sdk"
        echo "$ANDROID_HOME (auto-detected)"
    elif [ -d "$HOME/android-sdk" ]; then
        export ANDROID_HOME="$HOME/android-sdk"
        echo "$ANDROID_HOME (auto-detected)"
    else
        echo "❌ NOT SET - Set in Android Studio SDK Manager"
        exit 1
    fi
else
    echo "$ANDROID_HOME"
fi

# Check Android SDK
echo -n "✓ Android SDK: "
if [ -d "$ANDROID_HOME/platforms/android-34" ]; then
    echo "API 34 ✓"
else
    echo "❌ API 34 missing - Install in SDK Manager"
fi

# Check gradlew
echo -n "✓ Gradle Wrapper: "
if [ -f "quest/android/gradlew" ]; then
    echo "Found"
    chmod +x quest/android/gradlew
else
    echo "❌ NOT FOUND - Run in quest/android directory"
fi

# Check local.properties
echo -n "✓ local.properties: "
if [ -f "quest/android/local.properties" ]; then
    echo "Exists"
    cat quest/android/local.properties
else
    echo "Creating..."
    echo "sdk.dir=$ANDROID_HOME" > quest/android/local.properties
    echo "Created with sdk.dir=$ANDROID_HOME"
fi

echo ""
echo "========================================"
echo "🚀 Quick Build Commands:"
echo ""
echo "1. Using Android Studio:"
echo "   - Open quest/android folder"
echo "   - Build → Make Module 'substream'"
echo ""
echo "2. Using Terminal:"
echo "   cd quest/android"
echo "   ./gradlew :substream:assembleRelease"
echo ""
echo "AAR location: quest/android/substream/build/outputs/aar/substream-release.aar"
echo "========================================"
