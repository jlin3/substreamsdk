using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// SUBSTREAM AUTO-INSTALLER
/// One-click LiveKit SDK installation!
/// </summary>
public class SubstreamAutoInstaller : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Substream/Install LiveKit SDK")]
    static void InstallLiveKitSDK()
    {
        if (EditorUtility.DisplayDialog("Install LiveKit Unity SDK?", 
            "This will download and import the LiveKit Unity SDK (v1.5.2).\n\n" +
            "The SDK is ~15MB and includes WebRTC libraries.\n\n" +
            "Continue?", "Install", "Cancel"))
        {
            DownloadAndInstallLiveKit();
        }
    }
    
    [MenuItem("Substream/Check Installation")]
    static void CheckInstallation()
    {
        var liveKitType = System.Type.GetType("LiveKit.Room, LiveKit");
        
        if (liveKitType != null)
        {
            EditorUtility.DisplayDialog("LiveKit SDK Status", 
                "✅ LiveKit SDK is installed!\n\n" +
                "You can now use video streaming.", "OK");
        }
        else
        {
            if (EditorUtility.DisplayDialog("LiveKit SDK Status", 
                "❌ LiveKit SDK not found.\n\n" +
                "Would you like to install it now?", "Install", "Later"))
            {
                InstallLiveKitSDK();
            }
        }
    }
    
    static async void DownloadAndInstallLiveKit()
    {
        string url = "https://github.com/livekit/client-sdk-unity/releases/download/v1.5.2/io.livekit.sdk-1.5.2.unitypackage";
        string tempPath = Path.Combine(Application.temporaryCachePath, "livekit-sdk.unitypackage");
        
        EditorUtility.DisplayProgressBar("Installing LiveKit SDK", "Downloading...", 0.3f);
        
        using (var request = UnityWebRequest.Get(url))
        {
            var operation = request.SendWebRequest();
            
            while (!operation.isDone)
            {
                EditorUtility.DisplayProgressBar("Installing LiveKit SDK", 
                    $"Downloading... {(int)(operation.progress * 100)}%", 
                    operation.progress * 0.8f);
                await System.Threading.Tasks.Task.Delay(100);
            }
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(tempPath, request.downloadHandler.data);
                
                EditorUtility.DisplayProgressBar("Installing LiveKit SDK", "Importing package...", 0.9f);
                
                // Import the package
                AssetDatabase.ImportPackage(tempPath, false);
                
                EditorUtility.ClearProgressBar();
                
                // Clean up
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
                
                EditorUtility.DisplayDialog("Success!", 
                    "LiveKit SDK installed successfully!\n\n" +
                    "You can now use video streaming.", "Awesome!");
            }
            else
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Download Failed", 
                    $"Failed to download LiveKit SDK:\n{request.error}\n\n" +
                    "You can manually download from:\n" +
                    "github.com/livekit/client-sdk-unity/releases", "OK");
            }
        }
    }
#endif
}

/// <summary>
/// Runtime installer component
/// </summary>
public class SubstreamInstaller : MonoBehaviour
{
    public static bool IsLiveKitInstalled()
    {
        return System.Type.GetType("LiveKit.Room, LiveKit") != null;
    }
    
    public static string GetInstallInstructions()
    {
        return "To enable video streaming:\n\n" +
               "1. In Unity: Substream → Install LiveKit SDK\n" +
               "2. Or download manually:\n" +
               "   github.com/livekit/client-sdk-unity/releases\n" +
               "3. Import the .unitypackage file";
    }
}
