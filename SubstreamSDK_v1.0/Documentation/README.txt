═══════════════════════════════════════════════════════════════
                    SUBSTREAM UNITY SDK
                 One-File Streaming Solution
═══════════════════════════════════════════════════════════════

NO SETUP REQUIRED - Just drop SubstreamSDK.cs into your project!

═══════════════════════════════════════════════════════════════
QUICKSTART (2 minutes)
═══════════════════════════════════════════════════════════════

1. Import SubstreamSDK.cs into your Unity project

2. To stream programmatically:
   SubstreamSDK.StartStreaming();
   SubstreamSDK.StopStreaming();

3. Or use the UI helper:
   - Add SubstreamUI component to any GameObject
   - Connect your buttons in the Inspector
   - Done!

═══════════════════════════════════════════════════════════════
FEATURES
═══════════════════════════════════════════════════════════════

✅ Works immediately - no configuration needed
✅ Pre-configured streaming endpoint
✅ Automatic Quest/Android permissions
✅ Simple API - just StartStreaming()
✅ Viewer links generated automatically
✅ No external dependencies
✅ Unity 2019.4+ compatible

═══════════════════════════════════════════════════════════════
API REFERENCE
═══════════════════════════════════════════════════════════════

// Start streaming
SubstreamSDK.StartStreaming();

// Stop streaming
SubstreamSDK.StopStreaming();

// Check status
bool isLive = SubstreamSDK.IsStreaming;

// Get viewer link
string link = SubstreamSDK.ViewerLink;

// Events
SubstreamSDK.OnStreamingStatusChanged += (bool isStreaming) => { };
SubstreamSDK.OnViewerLinkReady += (string link) => { };
SubstreamSDK.OnError += (string error) => { };

═══════════════════════════════════════════════════════════════
VIEWING STREAMS
═══════════════════════════════════════════════════════════════

Streams can be viewed at:
https://cloud.livekit.io/projects/substream-cnzdthyx/rooms

The SDK will log the specific room URL when streaming starts.

═══════════════════════════════════════════════════════════════
PLATFORM NOTES
═══════════════════════════════════════════════════════════════

Unity Editor: Works immediately, no permissions needed
Meta Quest: Shows permission dialog, then streams
iOS/Android: Automatic permission handling

═══════════════════════════════════════════════════════════════
TROUBLESHOOTING
═══════════════════════════════════════════════════════════════

If streaming doesn't start:
- Check internet connection
- Check Unity console for errors
- Ensure project has internet access enabled

═══════════════════════════════════════════════════════════════
EXAMPLE USAGE
═══════════════════════════════════════════════════════════════

using UnityEngine;
using Substream;

public class MyGame : MonoBehaviour 
{
    void Start() 
    {
        // Start streaming when game starts
        SubstreamSDK.StartStreaming();
        
        // Listen for viewer link
        SubstreamSDK.OnViewerLinkReady += (link) => 
        {
            Debug.Log("Share this link: " + link);
        };
    }
    
    void OnDestroy() 
    {
        // Stop streaming when done
        SubstreamSDK.StopStreaming();
    }
}

═══════════════════════════════════════════════════════════════
SUPPORT
═══════════════════════════════════════════════════════════════

This SDK is pre-configured and ready to use.
No additional setup or API keys required!

═══════════════════════════════════════════════════════════════
