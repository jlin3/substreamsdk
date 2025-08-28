#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// SUBSTREAM PACKAGER
/// Creates a complete .unitypackage with all dependencies
/// </summary>
public class SubstreamPackager : EditorWindow
{
    [MenuItem("Substream/Package SDK")]
    static void ShowPackager()
    {
        var window = GetWindow<SubstreamPackager>("Substream Packager");
        window.minSize = new Vector2(400, 300);
        window.Show();
    }
    
    bool includeLiveKit = false;
    bool includeTestScene = true;
    bool includeAutoInstaller = true;
    
    void OnGUI()
    {
        GUILayout.Label("Substream SDK Packager", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label("This tool creates a complete .unitypackage for distribution");
        GUILayout.Space(20);
        
        // Check LiveKit installation
        var liveKitInstalled = System.Type.GetType("LiveKit.Room, LiveKit") != null;
        
        if (liveKitInstalled)
        {
            EditorGUILayout.HelpBox("✅ LiveKit SDK detected - can include in package", MessageType.Info);
            includeLiveKit = EditorGUILayout.Toggle("Include LiveKit SDK", includeLiveKit);
        }
        else
        {
            EditorGUILayout.HelpBox("❌ LiveKit SDK not installed - will include auto-installer instead", MessageType.Warning);
            if (GUILayout.Button("Install LiveKit SDK Now"))
            {
                SubstreamAutoInstaller.InstallLiveKitSDK();
            }
        }
        
        GUILayout.Space(10);
        includeTestScene = EditorGUILayout.Toggle("Include Test Scene", includeTestScene);
        includeAutoInstaller = EditorGUILayout.Toggle("Include Auto-Installer", includeAutoInstaller);
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("Create Package", GUILayout.Height(40)))
        {
            CreatePackage();
        }
    }
    
    void CreatePackage()
    {
        List<string> assets = new List<string>();
        
        // Core scripts
        AddIfExists(assets, "Assets/Scripts/SubstreamBundled.cs");
        AddIfExists(assets, "Assets/Scripts/SubstreamComplete.cs");
        AddIfExists(assets, "Assets/Scripts/SubstreamLiveKitCloud.cs");
        AddIfExists(assets, "Assets/Scripts/SubstreamTestScene.cs");
        
        // Auto-installer
        if (includeAutoInstaller)
        {
            AddIfExists(assets, "Assets/Scripts/SubstreamAutoInstaller.cs");
            AddIfExists(assets, "Assets/Scripts/SubstreamPackager.cs");
        }
        
        // Test scene
        if (includeTestScene)
        {
            AddIfExists(assets, "Assets/Scenes/Stream Test Scene.unity");
            // Add scene assets
            string[] sceneAssets = AssetDatabase.FindAssets("", new[] { "Assets/Scenes" });
            foreach (var guid in sceneAssets)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!assets.Contains(path))
                    assets.Add(path);
            }
        }
        
        // LiveKit SDK if requested and available
        if (includeLiveKit && System.Type.GetType("LiveKit.Room, LiveKit") != null)
        {
            // Add LiveKit folders
            AddFolderRecursive(assets, "Assets/LiveKit");
            AddFolderRecursive(assets, "Assets/Plugins/LiveKit");
            AddFolderRecursive(assets, "Assets/Plugins/WebRTC");
        }
        
        // Native plugins
        AddIfExists(assets, "Assets/Plugins/Android/substream-release.aar");
        
        if (assets.Count == 0)
        {
            EditorUtility.DisplayDialog("No Assets", "No assets found to package!", "OK");
            return;
        }
        
        // Create package
        string packagePath = EditorUtility.SaveFilePanel(
            "Save Substream SDK Package",
            "",
            includeLiveKit ? "SubstreamSDK-Complete.unitypackage" : "SubstreamSDK.unitypackage",
            "unitypackage"
        );
        
        if (!string.IsNullOrEmpty(packagePath))
        {
            AssetDatabase.ExportPackage(assets.ToArray(), packagePath, 
                ExportPackageOptions.Interactive);
            
            EditorUtility.DisplayDialog("Success!", 
                $"Package created with {assets.Count} assets!\n\n" +
                (includeLiveKit ? "✅ Includes LiveKit SDK" : "⚠️ Auto-installer included"), 
                "Great!");
        }
    }
    
    void AddIfExists(List<string> list, string path)
    {
        if (File.Exists(path) && !list.Contains(path))
            list.Add(path);
    }
    
    void AddFolderRecursive(List<string> list, string folder)
    {
        if (Directory.Exists(folder))
        {
            string[] assets = AssetDatabase.FindAssets("", new[] { folder });
            foreach (var guid in assets)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!list.Contains(path))
                    list.Add(path);
            }
        }
    }
}
#endif
