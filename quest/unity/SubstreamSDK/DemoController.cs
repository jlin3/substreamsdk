using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SubstreamSDK
{
    /// <summary>
    /// Demo controller for Substream SDK - drag onto any GameObject in your scene
    /// </summary>
    public class DemoController : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Use 'demo' for quick testing without server setup")]
        public string BaseUrl = "demo";
        public string WhipPublishUrl = "";
        
        [Header("Stream Settings")]
        public int Width = 1920;
        public int Height = 1080;
        public int Fps = 60;
        public int BitrateKbps = 5000;
        public bool WithAudio = true;
        
        [Header("UI References (Optional)")]
        public Button GoLiveButton;
        public Button StopButton;
        public Text StatusText;
        public GameObject StreamingIndicator;

        private LiveHandle _live;
        private VodHandle _vod;
        private bool _isStreaming = false;

        async void Start()
        {
            // Initialize SDK
            try
            {
                UpdateStatus("Initializing Substream SDK...");
                
                await Substream.Init(new SubstreamConfig
                {
                    BaseUrl = BaseUrl,
                    WhipPublishUrl = WhipPublishUrl
                });
                
                UpdateStatus("Ready to stream!");
                Debug.Log($"[Substream Demo] Initialized with BaseUrl: {BaseUrl}");
                
                // Set up UI if references are assigned
                if (GoLiveButton != null)
                {
                    GoLiveButton.onClick.AddListener(() => _ = GoLive());
                    GoLiveButton.interactable = true;
                }
                
                if (StopButton != null)
                {
                    StopButton.onClick.AddListener(() => _ = StopLive());
                    StopButton.interactable = false;
                }
                
                if (StreamingIndicator != null)
                {
                    StreamingIndicator.SetActive(false);
                }
            }
            catch (System.Exception e)
            {
                UpdateStatus($"Init failed: {e.Message}", true);
                Debug.LogError($"[Substream Demo] Failed to initialize: {e}");
            }
        }
        
        void Update()
        {
            // Keyboard shortcuts
            if (Input.GetKeyDown(KeyCode.Space) && !_isStreaming)
            {
                _ = GoLive();
            }
            else if (Input.GetKeyDown(KeyCode.S) && _isStreaming)
            {
                _ = StopLive();
            }
            
            // Animate streaming indicator
            if (_isStreaming && StreamingIndicator != null)
            {
                StreamingIndicator.transform.Rotate(0, 0, -2f);
            }
        }

        public async Task GoLive()
        {
            if (_live != null || _isStreaming) return;
            
            try
            {
                UpdateStatus("Starting stream...");
                SetButtonStates(false, false);
                
                // Create streaming session
                _live = await Substream.LiveCreate(new LiveOptions
                {
                    Width = Width,
                    Height = Height,
                    Fps = Fps,
                    VideoBitrateKbps = BitrateKbps,
                    WithAudio = WithAudio,
                    MetadataJson = JsonUtility.ToJson(new
                    {
                        game = Application.productName,
                        scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
                        platform = Application.platform.ToString(),
                        timestamp = System.DateTime.UtcNow.ToString("o")
                    })
                });
                
                // Set up event handlers
                _live.OnStatusChanged += OnStatusChanged;
                _live.OnError += OnError;
                
                // Start streaming
                await _live.Start();
                
                _isStreaming = true;
                UpdateStatus($"🔴 LIVE - {Width}x{Height}@{Fps}fps");
                SetButtonStates(false, true);
                
                if (StreamingIndicator != null)
                {
                    StreamingIndicator.SetActive(true);
                }
                
                Debug.Log($"[Substream Demo] Streaming started successfully");
            }
            catch (System.Exception e)
            {
                UpdateStatus($"Failed: {e.Message}", true);
                Debug.LogError($"[Substream Demo] Failed to start stream: {e}");
                SetButtonStates(true, false);
                _live = null;
            }
        }

        public async Task StopLive()
        {
            if (_live == null || !_isStreaming) return;
            
            try
            {
                UpdateStatus("Stopping stream...");
                SetButtonStates(false, false);
                
                await _live.Stop();
                
                _isStreaming = false;
                UpdateStatus("Stream stopped");
                SetButtonStates(true, false);
                
                if (StreamingIndicator != null)
                {
                    StreamingIndicator.SetActive(false);
                }
                
                // Clean up
                if (_live != null)
                {
                    _live.OnStatusChanged -= OnStatusChanged;
                    _live.OnError -= OnError;
                    _live = null;
                }
                
                Debug.Log("[Substream Demo] Stream stopped successfully");
            }
            catch (System.Exception e)
            {
                UpdateStatus($"Stop failed: {e.Message}", true);
                Debug.LogError($"[Substream Demo] Failed to stop stream: {e}");
            }
        }
        
        // Example VOD usage (call from your UI)
        public async Task StartRecording()
        {
            if (_vod != null) return;
            _vod = await Substream.VodCreate(new VodOptions
            {
                Width = Width,
                Height = Height,
                Fps = Fps,
                VideoBitrateKbps = BitrateKbps,
                WithAudio = WithAudio,
                OutputHint = Application.productName
            });
            _vod.OnSaved += path => UpdateStatus($"Saved recording: {path}");
            _vod.OnError += err => UpdateStatus($"VOD error: {err}", true);
            await _vod.Start();
            UpdateStatus("⏺ Recording...");
        }

        public async Task StopRecording()
        {
            if (_vod == null) return;
            var path = await _vod.Stop();
            UpdateStatus($"Saved recording: {path}");
            _vod = null;
        }
        
        private void OnStatusChanged(StreamStatus status)
        {
            Debug.Log($"[Substream Demo] Status: {status}");
            
            switch (status)
            {
                case StreamStatus.RequestingPermission:
                    UpdateStatus("📱 Please approve screen capture...");
                    break;
                case StreamStatus.Streaming:
                    UpdateStatus($"🔴 LIVE - Streaming");
                    break;
                case StreamStatus.Error:
                    UpdateStatus("❌ Stream error", true);
                    break;
            }
        }
        
        private void OnError(string error)
        {
            UpdateStatus($"Error: {error}", true);
            Debug.LogError($"[Substream Demo] Error: {error}");
        }
        
        private void UpdateStatus(string message, bool isError = false)
        {
            if (StatusText != null)
            {
                StatusText.text = message;
                StatusText.color = isError ? Color.red : Color.white;
            }
        }
        
        private void SetButtonStates(bool canGoLive, bool canStop)
        {
            if (GoLiveButton != null)
                GoLiveButton.interactable = canGoLive;
            if (StopButton != null)
                StopButton.interactable = canStop;
        }
        
        void OnDestroy()
        {
            // Clean up when destroyed
            if (_isStreaming && _live != null)
            {
                _ = StopLive();
            }
        }
        
        // Helper methods for common presets
        [ContextMenu("Use 720p30 Preset")]
        public void Use720p30()
        {
            Width = 1280;
            Height = 720;
            Fps = 30;
            BitrateKbps = 2500;
        }
        
        [ContextMenu("Use 1080p60 Preset")]
        public void Use1080p60()
        {
            Width = 1920;
            Height = 1080;
            Fps = 60;
            BitrateKbps = 5000;
        }
        
        [ContextMenu("Use Quest 2 Optimized")]
        public void UseQuest2Optimized()
        {
            Width = 1440;
            Height = 1440;
            Fps = 72;
            BitrateKbps = 4000;
        }

        [ContextMenu("Disable Audio")]
        public void DisableAudio()
        {
            WithAudio = false;
        }

        [ContextMenu("Enable Audio")]
        public void EnableAudio()
        {
            WithAudio = true;
        }
    }
}
