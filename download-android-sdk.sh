#!/bin/bash

# Download Android SDK Command Line Tools
echo "=== Downloading Android SDK Command Line Tools ==="

# Create SDK directory
mkdir -p ~/android-sdk
cd ~/android-sdk

# Download command line tools (macOS)
echo "Downloading SDK tools..."
curl -o commandlinetools.zip https://dl.google.com/android/repository/commandlinetools-mac-10406996_latest.zip

# Extract
echo "Extracting..."
unzip -q commandlinetools.zip
rm commandlinetools.zip

# Create the expected directory structure
mkdir -p cmdline-tools/latest
mv cmdline-tools/* cmdline-tools/latest/ 2>/dev/null || true

# Set up environment
export ANDROID_HOME=~/android-sdk
export PATH=$PATH:$ANDROID_HOME/cmdline-tools/latest/bin:$ANDROID_HOME/platform-tools

# Accept licenses
echo "Accepting licenses..."
yes | sdkmanager --licenses

# Install required packages
echo "Installing Android SDK components..."
sdkmanager "platform-tools" "platforms;android-33" "build-tools;33.0.2"

echo ""
echo "✅ Android SDK installed at: ~/android-sdk"
echo ""
echo "Add this to your ~/.zshrc or ~/.bash_profile:"
echo "export ANDROID_HOME=~/android-sdk"
echo "export PATH=\$PATH:\$ANDROID_HOME/cmdline-tools/latest/bin:\$ANDROID_HOME/platform-tools"

