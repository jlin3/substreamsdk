/*
 * Substream SDK - Unity Streaming Script
 * 
 * SETUP INSTRUCTIONS:
 * 
 * 1. START LIVEKIT SERVER:
 *    Option A: Local LiveKit (easiest for testing)
 *    - Open terminal in substreamsdk folder
 *    - Run: docker-compose up -d
 *    - Your WHIP URL will be: http://localhost:8080/rtc
 *    
 *    Option B: LiveKit Cloud (for production)
 *    - Sign up at: https://livekit.io
 *    - Create a project
 *    - Your WHIP URL will be: https://your-project.livekit.cloud/rtc
 * 
 * 2. CONFIGURE THIS SCRIPT:
 *    - Edit line 81 below to set your LiveKit URL
 *    - OR enter the URL in the Unity Inspector (whipUrlInput field)
 * 
 * 3. TEST:
 *    - Press Play in Unity
 *    - Click Start Streaming
 *    - Open the viewer URL shown in the UI
 *    
 * For help, check STREAMING_SETUP.md in the repo
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SubstreamSDK;
using System.Threading.Tasks;

public class SimpleDemoScript : MonoBehaviour
{
    // UI References - connect these in the Inspector
    [Header("UI Elements")]
    public Button startButton;
    public Button stopButton;
    public Text statusText;
    public Text viewerLinkText;
    public InputField whipUrlInput; // Optional - for custom WHIP URL
    
    // Stream Settings
    [Header("Stream Settings")]
    public int streamWidth = 1920;
    public int streamHeight = 1080;
    public int streamFPS = 30;
    public int bitrateKbps = 3500;
    public bool includeAudio = true;
    
    // Internal state
    private LiveHandle currentStream;
    private bool isStreaming = false;
    private string currentRoomId = "";
    
    void Start()
    {
        // Initialize SDK in demo mode on start
        InitializeSDK();
        
        // Connect button listeners
        if (startButton != null)
            startButton.onClick.AddListener(StartStreaming);
            
        if (stopButton != null)
            stopButton.onClick.AddListener(StopStreaming);
            
        // Initial UI state
        UpdateUI("Ready to stream", false);
    }
    
    async void InitializeSDK()
    {
        try
        {
            var config = new SubstreamConfig();
            
            // Check for environment variables or hardcoded values
            string whipUrl = "";
            
            // Option 1: Use input field if provided
            if (whipUrlInput != null && !string.IsNullOrEmpty(whipUrlInput.text))
            {
                whipUrl = whipUrlInput.text;
            }
            // Option 2: Use your LiveKit Cloud URL
            else
            {
                // IMPORTANT: Replace with your actual LiveKit Cloud WHIP URL!
                // Get this from your LiveKit Cloud dashboard:
                // 1. Go to https://cloud.livekit.io
                // 2. Select your project
                // 3. Go to "Ingress" section
                // 4. Copy the WHIP URL (looks like: https://url-xxxxxxxxx.whip.livekit.cloud/w)
                
                // Your LiveKit Cloud WHIP URL:
                whipUrl = "https://substream-cnzdthyx.whip.livekit.cloud/w"; // ✅ READY TO STREAM!
                
                // For testing with local LiveKit, use:
                // whipUrl = "http://localhost:8080/rtc";
            }
            
            if (string.IsNullOrEmpty(whipUrl) || whipUrl == "http://localhost:8080/rtc")
            {
                UpdateStatus("⚠️ Using local LiveKit URL - make sure Docker is running!");
                Debug.LogWarning("[SimpleDemoScript] Using localhost - ensure LiveKit is running locally");
                Debug.LogWarning("[SimpleDemoScript] Or use the configured LiveKit Cloud URL");
            }
            
            config.BaseUrl = "https://api.substream.io"; // Your API endpoint
            config.WhipPublishUrl = whipUrl;
            
            await Substream.Init(config);
            UpdateStatus($"SDK Initialized - WHIP: {whipUrl}");
            Debug.Log($"[SimpleDemoScript] Initialized with WHIP URL: {whipUrl}");
        }
        catch (System.Exception e)
        {
            UpdateStatus($"Init Error: {e.Message}");
            Debug.LogError($"[SimpleDemoScript] Failed to initialize: {e}");
        }
    }
    
    public async void StartStreaming()
    {
        if (isStreaming)
        {
            UpdateStatus("Already streaming!");
            return;
        }
        
        try
        {
            UpdateStatus("Starting stream...");
            
            // Generate room ID first so we can include it in metadata
            currentRoomId = "unity-stream-" + System.Guid.NewGuid().ToString().Substring(0, 8);
            
            // Create stream options
            var options = new LiveOptions
            {
                Width = streamWidth,
                Height = streamHeight,
                Fps = streamFPS,
                VideoBitrateKbps = bitrateKbps,
                WithAudio = includeAudio,
                MetadataJson = JsonUtility.ToJson(new {
                    game = Application.productName,
                    platform = Application.platform.ToString(),
                    version = Application.version,
                    timestamp = System.DateTime.Now.ToString(),
                    room = currentRoomId  // Include room ID for LiveKit
                })
            };
            
            // Create live stream
            currentStream = await Substream.LiveCreate(options);
            
            // Subscribe to status changes
            currentStream.OnStatusChanged += OnStreamStatusChanged;
            currentStream.OnError += OnStreamError;
            
            // Start the stream
            await currentStream.Start();
            
        }
        catch (System.Exception e)
        {
            UpdateStatus($"Start Error: {e.Message}");
            Debug.LogError($"[SimpleDemoScript] Failed to start stream: {e}");
        }
    }
    
    public async void StopStreaming()
    {
        if (!isStreaming || currentStream == null)
        {
            UpdateStatus("Not streaming");
            return;
        }
        
        try
        {
            UpdateStatus("Stopping stream...");
            await currentStream.Stop();
        }
        catch (System.Exception e)
        {
            UpdateStatus($"Stop Error: {e.Message}");
            Debug.LogError($"[SimpleDemoScript] Failed to stop stream: {e}");
        }
    }
    
    void OnStreamStatusChanged(StreamStatus status)
    {
        Debug.Log($"[SimpleDemoScript] Stream status: {status}");
        
        switch (status)
        {
            case StreamStatus.RequestingPermission:
                UpdateStatus("Requesting permission...");
                break;
                
            case StreamStatus.PermissionGranted:
                UpdateStatus("Permission granted!");
                break;
                
            case StreamStatus.Starting:
                UpdateStatus("Starting stream...");
                break;
                
            case StreamStatus.Streaming:
                isStreaming = true;
                UpdateStatus("🔴 LIVE - Streaming!");
                UpdateUI("🔴 LIVE - Streaming!", true);
                ShowViewerLink();
                break;
                
            case StreamStatus.Stopping:
                UpdateStatus("Stopping stream...");
                break;
                
            case StreamStatus.Stopped:
                isStreaming = false;
                UpdateStatus("Stream stopped");
                UpdateUI("Ready to stream", false);
                HideViewerLink();
                break;
                
            case StreamStatus.Error:
                isStreaming = false;
                UpdateStatus("Stream error!");
                UpdateUI("Error - Check logs", false);
                break;
        }
    }
    
    void OnStreamError(string error)
    {
        UpdateStatus($"Error: {error}");
        Debug.LogError($"[SimpleDemoScript] Stream error: {error}");
    }
    
    void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
            
        Debug.Log($"[SimpleDemoScript] {message}");
    }
    
    void UpdateUI(string status, bool streaming)
    {
        UpdateStatus(status);
        
        // Update button states
        if (startButton != null)
            startButton.interactable = !streaming;
            
        if (stopButton != null)
            stopButton.interactable = streaming;
    }
    
    void ShowViewerLink()
    {
        if (viewerLinkText != null)
        {
            string viewerUrl = "";
            
            // Use the room ID that was already generated in StartStreaming
            if (string.IsNullOrEmpty(currentRoomId))
            {
                Debug.LogError("[SimpleDemoScript] Room ID not set!");
                return;
            }
            
            // Determine the viewer URL based on the WHIP URL
            string whipUrl = whipUrlInput != null && !string.IsNullOrEmpty(whipUrlInput.text) 
                ? whipUrlInput.text 
                : "https://substream-cnzdthyx.whip.livekit.cloud/w"; // Your LiveKit Cloud URL
            
            if (whipUrl.Contains("localhost"))
            {
                // Local LiveKit viewer
                viewerUrl = $"http://localhost:5173/viewer.html?room={currentRoomId}";
            }
            else if (whipUrl.Contains("whip.livekit.cloud"))
            {
                // LiveKit Cloud - extract the project identifier from WHIP URL
                // WHIP URL format: https://url-xxxxxxxxx.whip.livekit.cloud/w
                // Viewer URL format: https://meet.livekit.io/app/your-api-key/room-name
                
                // For LiveKit Cloud, users can view the stream at:
                // 1. LiveKit Cloud dashboard preview
                // 2. LiveKit Meet (with API key)
                // 3. Custom viewer app
                
                viewerUrl = $"https://meet.livekit.io/\n" +
                           $"  Room: {currentRoomId}\n" +
                           $"  Or use LiveKit Cloud dashboard";
                
                Debug.Log("[SimpleDemoScript] For LiveKit Cloud viewing options:");
                Debug.Log($"[SimpleDemoScript] 1. Go to https://cloud.livekit.io");
                Debug.Log($"[SimpleDemoScript] 2. Select your project → Rooms");
                Debug.Log($"[SimpleDemoScript] 3. Find room: {currentRoomId}");
                Debug.Log($"[SimpleDemoScript] 4. Click 'Join' to preview");
            }
            else
            {
                // Custom viewer URL
                viewerUrl = $"Stream Room: {currentRoomId}";
            }
            
            viewerLinkText.text = $"Viewer: {viewerUrl}";
            viewerLinkText.gameObject.SetActive(true);
            
            // Important: Pass the room name to the streaming metadata
            if (currentStream != null)
            {
                Debug.Log($"[SimpleDemoScript] Streaming to room: {currentRoomId}");
            }
        }
    }
    
    void HideViewerLink()
    {
        if (viewerLinkText != null)
            viewerLinkText.gameObject.SetActive(false);
    }
    
    // Quality adjustment methods (optional)
    public void SetQualityLow()
    {
        if (currentStream != null && isStreaming)
        {
            currentStream.UpdateQuality(1280, 720, 30, 2000);
            UpdateStatus("Quality: Low (720p 30fps 2Mbps)");
        }
    }
    
    public void SetQualityMedium()
    {
        if (currentStream != null && isStreaming)
        {
            currentStream.UpdateQuality(1920, 1080, 30, 3500);
            UpdateStatus("Quality: Medium (1080p 30fps 3.5Mbps)");
        }
    }
    
    public void SetQualityHigh()
    {
        if (currentStream != null && isStreaming)
        {
            currentStream.UpdateQuality(1920, 1080, 60, 5000);
            UpdateStatus("Quality: High (1080p 60fps 5Mbps)");
        }
    }
    
    // Cleanup
    void OnDestroy()
    {
        if (isStreaming && currentStream != null)
        {
            StopStreaming();
        }
    }
    
    // Helper method for testing in Unity Editor
    void Update()
    {
        // Press S to start streaming (for testing)
        if (Input.GetKeyDown(KeyCode.S) && !isStreaming)
        {
            StartStreaming();
        }
        
        // Press X to stop streaming (for testing)
        if (Input.GetKeyDown(KeyCode.X) && isStreaming)
        {
            StopStreaming();
        }
    }
}
