#!/bin/bash

# Build script for Substream Unity Package
# This creates a .unitypackage file from the unity-package directory

echo "🚀 Building Substream Unity Package..."

# Check if Unity is installed and get path
if [[ "$OSTYPE" == "darwin"* ]]; then
    # macOS
    UNITY_PATH="/Applications/Unity/Hub/Editor/2021.3.*/Unity.app/Contents/MacOS/Unity"
    UNITY_EXECUTABLE=$(ls $UNITY_PATH 2>/dev/null | head -n 1)
elif [[ "$OSTYPE" == "msys" || "$OSTYPE" == "win32" ]]; then
    # Windows
    UNITY_EXECUTABLE="C:/Program Files/Unity/Hub/Editor/2021.3.*/Editor/Unity.exe"
    UNITY_EXECUTABLE=$(ls $UNITY_EXECUTABLE 2>/dev/null | head -n 1)
else
    # Linux
    UNITY_EXECUTABLE="$HOME/Unity/Hub/Editor/2021.3.*/Editor/Unity"
    UNITY_EXECUTABLE=$(ls $UNITY_EXECUTABLE 2>/dev/null | head -n 1)
fi

if [ -z "$UNITY_EXECUTABLE" ]; then
    echo "❌ Unity 2021.3 not found!"
    echo "Please install Unity 2021.3 or later, or set UNITY_EXECUTABLE environment variable"
    echo ""
    echo "Manual packaging instructions:"
    echo "1. Open Unity"
    echo "2. Create a new project"
    echo "3. Copy unity-package/Assets/SubstreamSDK to your Assets folder"
    echo "4. Right-click on SubstreamSDK folder"
    echo "5. Select 'Export Package...'"
    echo "6. Include all files and dependencies"
    echo "7. Save as SubstreamSDK.unitypackage"
    exit 1
fi

echo "✅ Found Unity at: $UNITY_EXECUTABLE"

# Create a temporary Unity project for packaging
TEMP_PROJECT="temp_unity_package_project"
OUTPUT_PACKAGE="SubstreamSDK.unitypackage"

echo "📁 Creating temporary Unity project..."
mkdir -p $TEMP_PROJECT/Assets

# Copy our package files
echo "📋 Copying SDK files..."
cp -r unity-package/Assets/SubstreamSDK $TEMP_PROJECT/Assets/

# Create a simple Unity packaging script
cat > $TEMP_PROJECT/Assets/ExportPackage.cs << 'EOL'
using UnityEngine;
using UnityEditor;

public class ExportPackage
{
    static void Export()
    {
        string[] assetPaths = new string[] { "Assets/SubstreamSDK" };
        AssetDatabase.ExportPackage(
            assetPaths, 
            "SubstreamSDK.unitypackage",
            ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies
        );
        EditorApplication.Exit(0);
    }
}
EOL

# Run Unity in batch mode to create the package
echo "📦 Building Unity package..."
"$UNITY_EXECUTABLE" \
    -batchmode \
    -quit \
    -nographics \
    -silent-crashes \
    -projectPath "$TEMP_PROJECT" \
    -executeMethod ExportPackage.Export \
    -logFile build.log

# Check if package was created
if [ -f "$TEMP_PROJECT/$OUTPUT_PACKAGE" ]; then
    mv "$TEMP_PROJECT/$OUTPUT_PACKAGE" .
    echo "✅ Successfully created $OUTPUT_PACKAGE"
    echo "📏 Package size: $(du -h $OUTPUT_PACKAGE | cut -f1)"
    
    # Clean up
    rm -rf $TEMP_PROJECT
    rm -f build.log
    
    echo ""
    echo "🎉 Unity package build complete!"
    echo ""
    echo "Installation instructions:"
    echo "1. Open your Unity project"
    echo "2. Assets → Import Package → Custom Package..."
    echo "3. Select $OUTPUT_PACKAGE"
    echo "4. Import all files"
    echo "5. Go to Substream → Create Demo Scene to get started!"
else
    echo "❌ Failed to create Unity package"
    echo "Check build.log for errors"
    exit 1
fi
