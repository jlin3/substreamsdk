#!/usr/bin/env python3
"""
Manual Unity Package Builder for Substream SDK
This creates the directory structure needed for Unity packaging
when Unity CLI is not available.
"""

import os
import shutil
import zipfile
import json
from datetime import datetime

def create_meta_file(file_path, guid=None):
    """Create a .meta file for Unity assets"""
    import uuid
    
    if guid is None:
        # Generate a deterministic GUID based on file path
        guid = str(uuid.uuid5(uuid.NAMESPACE_DNS, file_path)).replace('-', '')
    
    ext = os.path.splitext(file_path)[1].lower()
    
    # Determine file type
    if os.path.isdir(file_path):
        meta_content = f"""fileFormatVersion: 2
guid: {guid}
folderAsset: yes
DefaultImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
    elif ext in ['.cs']:
        meta_content = f"""fileFormatVersion: 2
guid: {guid}
MonoImporter:
  externalObjects: {{}}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {{instanceID: 0}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
    elif ext in ['.asmdef']:
        meta_content = f"""fileFormatVersion: 2
guid: {guid}
AssemblyDefinitionImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
    elif ext in ['.json', '.md', '.txt', '.xml']:
        meta_content = f"""fileFormatVersion: 2
guid: {guid}
TextScriptImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
    else:
        meta_content = f"""fileFormatVersion: 2
guid: {guid}
DefaultImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
    
    with open(f"{file_path}.meta", 'w') as f:
        f.write(meta_content)

def create_meta_files_recursive(directory):
    """Recursively create .meta files for all assets"""
    # First, create meta for the directory itself
    create_meta_file(directory)
    
    # Then walk through all files and subdirectories
    for root, dirs, files in os.walk(directory):
        # Create meta files for directories
        for dir_name in dirs:
            dir_path = os.path.join(root, dir_name)
            if not os.path.exists(f"{dir_path}.meta"):
                create_meta_file(dir_path)
        
        # Create meta files for files
        for file_name in files:
            if not file_name.endswith('.meta'):
                file_path = os.path.join(root, file_name)
                if not os.path.exists(f"{file_path}.meta"):
                    create_meta_file(file_path)

def main():
    print("🚀 Preparing Substream SDK Unity Package...")
    
    # Check if unity-package directory exists
    if not os.path.exists("unity-package/Assets/SubstreamSDK"):
        print("❌ Error: unity-package/Assets/SubstreamSDK directory not found!")
        print("Please run this script from the substreamsdk root directory.")
        return 1
    
    # Create a clean export directory
    export_dir = "SubstreamSDK_Export"
    if os.path.exists(export_dir):
        shutil.rmtree(export_dir)
    
    print("📁 Creating export directory...")
    os.makedirs(export_dir)
    
    # Copy the SDK files
    print("📋 Copying SDK files...")
    shutil.copytree("unity-package/Assets/SubstreamSDK", f"{export_dir}/SubstreamSDK")
    
    # Generate .meta files
    print("🔧 Generating Unity meta files...")
    create_meta_files_recursive(f"{export_dir}/SubstreamSDK")
    
    # Create a package info file
    package_info = {
        "name": "Substream SDK",
        "version": "1.0.0",
        "unity_version": "2021.3",
        "description": "Stream your Unity game in one line of code!",
        "export_date": datetime.now().isoformat(),
        "contents": [
            "Scripts/Substream.cs - Main SDK interface",
            "Demos/Scripts/DemoController.cs - Example implementation",
            "Editor/DemoSceneCreator.cs - Demo scene creation tool",
            "Plugins/Android/AndroidManifest.xml - Required permissions",
            "Plugins/Android/substream-release.aar - Android native library (build separately)",
            "Documentation/README.md - Getting started guide"
        ]
    }
    
    with open(f"{export_dir}/package_info.json", 'w') as f:
        json.dump(package_info, f, indent=2)
    
    print("📦 Package structure ready!")
    print("")
    print("✅ Success! The package structure is ready in: " + export_dir)
    print("")
    print("📝 Next steps:")
    print("1. Build the Android AAR:")
    print("   cd quest/android")
    print("   gradle :substream:assembleRelease")
    print("   cp substream/build/outputs/aar/substream-release.aar \\")
    print(f"      ../{export_dir}/SubstreamSDK/Plugins/Android/")
    print("")
    print("2. Import into Unity:")
    print("   - Open Unity")
    print("   - Drag the SubstreamSDK folder into your Assets")
    print("   - Or use Assets → Import Package → Custom Package")
    print("")
    print("3. To share with others:")
    print("   - In Unity, right-click the SubstreamSDK folder")
    print("   - Select 'Export Package...'")
    print("   - Include all dependencies")
    print("   - Save as SubstreamSDK.unitypackage")
    print("")
    print("Alternative: Create a ZIP file of the export directory for manual installation")
    
    # Optionally create a zip file
    create_zip = input("\n📦 Create a ZIP file for distribution? (y/n): ").lower().strip()
    if create_zip == 'y':
        zip_name = "SubstreamSDK_Unity_Package.zip"
        print(f"\n🗜️  Creating {zip_name}...")
        
        with zipfile.ZipFile(zip_name, 'w', zipfile.ZIP_DEFLATED) as zipf:
            for root, dirs, files in os.walk(export_dir):
                for file in files:
                    file_path = os.path.join(root, file)
                    arcname = os.path.relpath(file_path, os.path.dirname(export_dir))
                    zipf.write(file_path, arcname)
        
        print(f"✅ Created {zip_name} ({os.path.getsize(zip_name) // 1024} KB)")
        print("   Users can extract this into their Unity Assets folder")

if __name__ == "__main__":
    main()
