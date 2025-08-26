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
            // Initialize in demo mode for easy testing
            var config = new SubstreamConfig
            {
                BaseUrl = "demo", // Use "demo" for testing without server
                WhipPublishUrl = "" // Leave empty for demo mode
            };
            
            // Check if user provided a custom WHIP URL
            if (whipUrlInput != null && !string.IsNullOrEmpty(whipUrlInput.text))
            {
                config.BaseUrl = "https://api.substream.io"; // Replace with your API URL
                config.WhipPublishUrl = whipUrlInput.text;
            }
            
            await Substream.Init(config);
            UpdateStatus("SDK Initialized");
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
                    timestamp = System.DateTime.Now.ToString()
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
            // In demo mode, show demo viewer link
            string viewerUrl = "https://substreamapp.surge.sh/demo-viewer.html";
            
            // If using real WHIP URL, construct proper viewer link
            if (whipUrlInput != null && !string.IsNullOrEmpty(whipUrlInput.text))
            {
                // You would construct your actual viewer URL here
                viewerUrl = "https://yourviewer.com/watch?stream=xxx";
            }
            
            viewerLinkText.text = $"Viewer: {viewerUrl}";
            viewerLinkText.gameObject.SetActive(true);
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
