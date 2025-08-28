using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// This script creates a Unity Package with everything needed for the demo
/// Just run this in Unity Editor to package everything up!
/// </summary>
public class PrepareUnityPackage : MonoBehaviour
{
    [MenuItem("Substream/Prepare Demo Package")]
    public static void PreparePackage()
    {
        Debug.Log("=== Preparing Substream Demo Package ===");
        
        // Create folders if they don't exist
        string pluginsPath = "Assets/Plugins";
        string androidPath = "Assets/Plugins/Android";
        string sdkPath = "Assets/SubstreamSDK";
        string demosPath = "Assets/SubstreamSDK/Demos";
        
        if (!AssetDatabase.IsValidFolder(pluginsPath))
            AssetDatabase.CreateFolder("Assets", "Plugins");
            
        if (!AssetDatabase.IsValidFolder(androidPath))
            AssetDatabase.CreateFolder("Assets/Plugins", "Android");
            
        if (!AssetDatabase.IsValidFolder(sdkPath))
            AssetDatabase.CreateFolder("Assets", "SubstreamSDK");
            
        if (!AssetDatabase.IsValidFolder(demosPath))
            AssetDatabase.CreateFolder("Assets/SubstreamSDK", "Demos");
        
        Debug.Log("✓ Folders created");
        
        // List of files to include
        string[] filesToCopy = new string[]
        {
            "SubstreamSDK.cs",
            "SubstreamUnityDemo.cs",
            "SubstreamComplete.cs",
            "QuestStreamingEasySetup.cs"
        };
        
        // Copy scripts to the package
        foreach (string file in filesToCopy)
        {
            string sourcePath = Path.Combine(Application.dataPath, "..", file);
            string destPath = Path.Combine(Application.dataPath, "SubstreamSDK", "Demos", file);
            
            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, destPath, true);
                Debug.Log($"✓ Copied {file}");
            }
        }
        
        // Copy AAR if it exists
        string aarSource = Path.Combine(Application.dataPath, "..", "substream-release-quest.aar");
        string aarDest = Path.Combine(androidPath, "substream-release.aar");
        
        if (File.Exists(aarSource))
        {
            File.Copy(aarSource, aarDest, true);
            Debug.Log("✓ Copied Quest AAR");
        }
        
        // Create a README
        string readmeContent = @"# Substream SDK for Unity

## Quick Start (30 seconds)

1. Add any of these scripts to a GameObject:
   - `SubstreamSDK` - Automatic UI, one-line integration
   - `SubstreamUnityDemo` - Full demo with instructions
   - `QuestStreamingEasySetup` - Quest-specific features

2. Press Play

3. Click 'Go Live' or press SPACE

4. Share your Unity Game window when browser opens

That's it! You're streaming!

## For Meta Quest

The AAR is already included. Just:
1. Switch to Android platform
2. Build and deploy
3. Same scripts work on device!

## Demo Scripts Included

- **SubstreamSDK.cs** - Production-ready SDK with auto UI
- **SubstreamUnityDemo.cs** - Interactive demo with platform detection  
- **SubstreamComplete.cs** - Simple all-in-one example
- **QuestStreamingEasySetup.cs** - Quest-optimized version

Pick any script based on your needs!";
        
        File.WriteAllText(Path.Combine(sdkPath, "README.txt"), readmeContent);
        Debug.Log("✓ Created README");
        
        // Refresh AssetDatabase
        AssetDatabase.Refresh();
        
        // Now export as package
        string[] assets = new string[]
        {
            "Assets/SubstreamSDK",
            "Assets/Plugins/Android/substream-release.aar"
        };
        
        // Filter out non-existent assets
        var existingAssets = new System.Collections.Generic.List<string>();
        foreach (var asset in assets)
        {
            if (AssetDatabase.IsValidFolder(asset) || 
                File.Exists(Path.Combine(Application.dataPath, "..", asset)))
            {
                existingAssets.Add(asset);
            }
        }
        
        string packagePath = "SubstreamSDK_Demo.unitypackage";
        AssetDatabase.ExportPackage(existingAssets.ToArray(), packagePath, 
            ExportPackageOptions.Recurse | ExportPackageOptions.Interactive);
        
        Debug.Log($"=== Package Ready: {packagePath} ===");
        Debug.Log("Share this .unitypackage file for instant setup!");
    }
    
    [MenuItem("Substream/Open Demo Instructions")]
    public static void OpenInstructions()
    {
        EditorUtility.DisplayDialog(
            "Substream Demo Instructions",
            "1. Add SubstreamSDK to any GameObject\n" +
            "2. Press Play\n" +
            "3. Click 'Go Live'\n" +
            "4. Share Unity Game window in browser\n\n" +
            "That's it! You're streaming!",
            "Got it!"
        );
    }
}
