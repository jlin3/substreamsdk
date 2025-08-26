/*
 * Substream SDK - Unity Streaming Script
 * 
 * This script is configured and ready to stream!
 * Just press Play and click "Start Streaming"
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
    public Button viewerLinkButton; // NEW: Clickable link button
    public GameObject viewerPanel;  // NEW: Panel to show link prominently
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
    private string fullViewerUrl = "";
    
    void Start()
    {
        // Initialize SDK on start
        InitializeSDK();
        
        // Connect button listeners
        if (startButton != null)
            startButton.onClick.AddListener(StartStreaming);
            
        if (stopButton != null)
            stopButton.onClick.AddListener(StopStreaming);
            
        if (viewerLinkButton != null)
            viewerLinkButton.onClick.AddListener(OpenViewerLink);
            
        // Initial UI state
        UpdateUI("Ready to stream", false);
        
        // Hide viewer panel initially
        if (viewerPanel != null)
            viewerPanel.SetActive(false);
    }
    
    async void InitializeSDK()
    {
        try
        {
            var config = new SubstreamConfig();
            
            // Your LiveKit Cloud WHIP URL (already configured!)
            string whipUrl = "https://substream-cnzdthyx.whip.livekit.cloud/w";
            
            // Allow override from input field
            if (whipUrlInput != null && !string.IsNullOrEmpty(whipUrlInput.text))
            {
                whipUrl = whipUrlInput.text;
            }
            
            config.BaseUrl = "https://api.substream.io";
            config.WhipPublishUrl = whipUrl;
            
            await Substream.Init(config);
            UpdateStatus("SDK Ready - Click Start to begin streaming!");
            Debug.Log($"[Substream] Initialized with WHIP URL: {whipUrl}");
        }
        catch (System.Exception e)
        {
            UpdateStatus($"Init Error: {e.Message}");
            Debug.LogError($"[Substream] Failed to initialize: {e}");
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
            
            // Generate room ID
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
                    room = currentRoomId
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
            Debug.LogError($"[Substream] Failed to start stream: {e}");
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
            Debug.LogError($"[Substream] Failed to stop stream: {e}");
        }
    }
    
    void OnStreamStatusChanged(StreamStatus status)
    {
        Debug.Log($"[Substream] Stream status: {status}");
        
        switch (status)
        {
            case StreamStatus.RequestingPermission:
                UpdateStatus("📱 Please grant screen capture permission...");
                break;
                
            case StreamStatus.PermissionGranted:
                UpdateStatus("✅ Permission granted!");
                break;
                
            case StreamStatus.Starting:
                UpdateStatus("🔄 Connecting to stream server...");
                break;
                
            case StreamStatus.Streaming:
                isStreaming = true;
                UpdateStatus("🔴 LIVE - You're streaming!");
                UpdateUI("🔴 LIVE - Streaming!", true);
                ShowViewerLink();
                break;
                
            case StreamStatus.Stopping:
                UpdateStatus("⏹️ Stopping stream...");
                break;
                
            case StreamStatus.Stopped:
                isStreaming = false;
                UpdateStatus("Ready to stream");
                UpdateUI("Ready to stream", false);
                HideViewerLink();
                break;
                
            case StreamStatus.Error:
                isStreaming = false;
                UpdateStatus("❌ Stream error!");
                UpdateUI("Error - Check logs", false);
                break;
        }
    }
    
    void OnStreamError(string error)
    {
        UpdateStatus($"Error: {error}");
        Debug.LogError($"[Substream] Stream error: {error}");
    }
    
    void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
            
        Debug.Log($"[Substream] {message}");
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
        // Build the full viewer URL
        fullViewerUrl = $"https://cloud.livekit.io/projects/substream-cnzdthyx/rooms/{currentRoomId}";
        
        // Update viewer link text
        if (viewerLinkText != null)
        {
            viewerLinkText.text = "🔗 Click to View Stream";
            viewerLinkText.color = Color.cyan;
            viewerLinkText.fontSize = 20;
            viewerLinkText.fontStyle = FontStyle.Bold;
        }
        
        // Show viewer panel with prominent display
        if (viewerPanel != null)
        {
            viewerPanel.SetActive(true);
            
            // Add background color if it has an Image component
            var panelImage = viewerPanel.GetComponent<Image>();
            if (panelImage != null)
            {
                panelImage.color = new Color(0, 0.5f, 1f, 0.2f); // Light blue background
            }
        }
        
        // Make button prominent
        if (viewerLinkButton != null)
        {
            viewerLinkButton.gameObject.SetActive(true);
            var buttonImage = viewerLinkButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = new Color(0, 0.8f, 1f, 1f); // Bright cyan
            }
        }
        
        // Log clear instructions
        Debug.Log("╔═══════════════════════════════════════════════════════════╗");
        Debug.Log("║                  🎮 STREAM IS LIVE!                       ║");
        Debug.Log("╠═══════════════════════════════════════════════════════════╣");
        Debug.Log($"║ Room ID: {currentRoomId}                                ║");
        Debug.Log("║                                                           ║");
        Debug.Log("║ 👀 VIEW YOUR STREAM:                                      ║");
        Debug.Log("║                                                           ║");
        Debug.Log("║ 1. Click the blue 'View Stream' button in Unity          ║");
        Debug.Log("║    OR                                                     ║");
        Debug.Log("║ 2. Open this link in your browser:                       ║");
        Debug.Log($"║    {fullViewerUrl}");
        Debug.Log("║                                                           ║");
        Debug.Log("║ 3. Click 'Join' to watch the stream                      ║");
        Debug.Log("╚═══════════════════════════════════════════════════════════╝");
        
        // Also copy to clipboard automatically
        GUIUtility.systemCopyBuffer = fullViewerUrl;
        Debug.Log("📋 Viewer link copied to clipboard!");
    }
    
    void HideViewerLink()
    {
        if (viewerPanel != null)
            viewerPanel.SetActive(false);
            
        if (viewerLinkButton != null)
            viewerLinkButton.gameObject.SetActive(false);
    }
    
    // Open viewer link in browser
    public void OpenViewerLink()
    {
        if (!string.IsNullOrEmpty(fullViewerUrl))
        {
            Application.OpenURL(fullViewerUrl);
            Debug.Log($"[Substream] Opening viewer: {fullViewerUrl}");
        }
    }
    
    // Quality adjustment methods (optional)
    public void SetQualityLow()
    {
        if (currentStream != null && isStreaming)
        {
            currentStream.UpdateQuality(1280, 720, 30, 2000);
            UpdateStatus("Quality: Low (720p)");
        }
    }
    
    public void SetQualityMedium()
    {
        if (currentStream != null && isStreaming)
        {
            currentStream.UpdateQuality(1920, 1080, 30, 3500);
            UpdateStatus("Quality: Medium (1080p)");
        }
    }
    
    public void SetQualityHigh()
    {
        if (currentStream != null && isStreaming)
        {
            currentStream.UpdateQuality(1920, 1080, 60, 5000);
            UpdateStatus("Quality: High (1080p 60fps)");
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
        // Press S to start streaming
        if (Input.GetKeyDown(KeyCode.S) && !isStreaming)
        {
            StartStreaming();
        }
        
        // Press X to stop streaming
        if (Input.GetKeyDown(KeyCode.X) && isStreaming)
        {
            StopStreaming();
        }
        
        // Press V to open viewer
        if (Input.GetKeyDown(KeyCode.V) && isStreaming)
        {
            OpenViewerLink();
        }
    }
}