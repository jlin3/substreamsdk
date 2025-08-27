#!/bin/bash
# Build script for Substream Quest AAR

echo "=== Building Substream Quest AAR ==="
echo ""
echo "Prerequisites:"
echo "- Android Studio installed"
echo "- Android SDK with API 29+ installed"
echo "- JAVA_HOME set to JDK 11+"
echo ""

cd quest/android

# Check if gradlew exists
if [ ! -f "./gradlew" ]; then
    echo "Error: gradlew not found in quest/android/"
    echo "Please ensure you're in the correct directory"
    exit 1
fi

# Make gradlew executable
chmod +x gradlew

# Clean previous builds
echo "Cleaning previous builds..."
./gradlew clean

# Build release AAR
echo "Building release AAR..."
./gradlew :substream:assembleRelease

# Check if build succeeded
if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Build successful!"
    echo "AAR location: quest/android/substream/build/outputs/aar/substream-release.aar"
    echo ""
    echo "Next steps:"
    echo "1. Copy substream-release.aar to Unity project: Assets/Plugins/Android/"
    echo "2. Import quest/unity/SubstreamSDK/ to Assets/"
    echo "3. Add DemoController to scene"
else
    echo ""
    echo "❌ Build failed!"
    echo "Please check the error messages above"
    exit 1
fi
