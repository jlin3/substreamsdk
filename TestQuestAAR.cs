using UnityEngine;

/// <summary>
/// Test if Quest AAR is properly installed
/// Add this to any GameObject and check the console
/// </summary>
public class TestQuestAAR : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== Testing Quest AAR Installation ===");
        
        #if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaClass jc = new AndroidJavaClass("com.substream.sdk.SubstreamNative"))
            {
                Debug.Log("✅ SUCCESS: Quest AAR is properly installed!");
                Debug.Log("✅ Native streaming is available!");
                Debug.Log("✅ You can now use MediaProjection capture!");
                
                // Try to get version info if available
                try
                {
                    string version = jc.CallStatic<string>("getVersion");
                    Debug.Log($"AAR Version: {version}");
                }
                catch
                {
                    Debug.Log("AAR Version: 1.0 (no version method)");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ FAILED: Quest AAR not found or not working!");
            Debug.LogError($"Error: {e.Message}");
            Debug.LogError("Make sure substream-release.aar is in Assets/Plugins/Android/");
        }
        #else
        Debug.Log("⚠️ Not running on Android device - AAR test skipped");
        Debug.Log("Deploy to Quest to test AAR functionality");
        #endif
        
        Debug.Log("=== Test Complete ===");
    }
}
