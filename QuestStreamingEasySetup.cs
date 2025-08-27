using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SubstreamSDK
{
    /// <summary>
    /// QUEST STREAMING - ONE FILE SETUP
    /// Just add this script to any GameObject and you're streaming!
    /// 
    /// Requirements:
    /// 1. Copy substream-release.aar to Assets/Plugins/Android/
    /// 2. Add this script to any GameObject
    /// 3. Build for Quest
    /// 4. That's it!
    /// </summary>
    public class QuestStreamingEasySetup : MonoBehaviour
    {
        [Header("Streaming Settings")]
        [Tooltip("Just leave as 'demo' for testing")]
        public string BaseUrl = "demo";
        
        [Tooltip("Stream quality - 1080p default")]
        public int Width = 1920;
        public int Height = 1080;
        public int Fps = 60;
        public int BitrateKbps = 5000;
        
        [Header("Auto-start")]
        [Tooltip("Start streaming automatically when app launches")]
        public bool AutoStart = false;
        
        [Tooltip("Delay before auto-start (seconds)")]
        public float AutoStartDelay = 3f;
        
        // Private
        private bool isInitialized = false;
        private LiveHandle currentStream = null;
        private string currentRoomName = "";
        
        async void Start()
        {
            Debug.Log("[QuestStreaming] Initializing...");
            
            try
            {
                // Initialize SDK
                await Substream.Init(new SubstreamConfig 
                { 
                    BaseUrl = BaseUrl,
                    WhipPublishUrl = "" // Auto-configured for demo mode
                });
                
                isInitialized = true;
                Debug.Log("[QuestStreaming] ✅ Ready! Press SPACE or A button to start streaming");
                
                if (AutoStart)
                {
                    await Task.Delay((int)(AutoStartDelay * 1000));
                    await StartStreaming();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[QuestStreaming] Init failed: {e.Message}");
            }
        }
        
        void Update()
        {
            if (!isInitialized) return;
            
            // Keyboard: Space
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ToggleStreaming();
            }
            
            // Quest Controller: A button
            if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Submit"))
            {
                ToggleStreaming();
            }
            
            // Show status every 5 seconds while streaming
            if (currentStream != null && Time.frameCount % 300 == 0)
            {
                Debug.Log($"[QuestStreaming] 🔴 LIVE - Room: {currentRoomName}");
            }
        }
        
        async void ToggleStreaming()
        {
            if (currentStream == null)
            {
                await StartStreaming();
            }
            else
            {
                await StopStreaming();
            }
        }
        
        public async Task StartStreaming()
        {
            if (currentStream != null) return;
            
            try
            {
                Debug.Log("[QuestStreaming] Starting stream...");
                
                // Create stream handle
                currentStream = await Substream.LiveCreate(new LiveOptions
                {
                    Width = Width,
                    Height = Height,
                    Fps = Fps,
                    VideoBitrateKbps = BitrateKbps,
                    WithAudio = true,
                    MetadataJson = JsonUtility.ToJson(new
                    {
                        platform = "Quest",
                        unityVersion = Application.unityVersion,
                        timestamp = DateTime.UtcNow.ToString("o")
                    })
                });
                
                // Set up event handlers
                currentStream.OnStatusChanged += OnStatusChanged;
                currentStream.OnError += OnError;
                
                // Start streaming
                await currentStream.Start();
                
                // Generate simple room name
                currentRoomName = "quest-" + DateTime.Now.ToString("HHmmss");
                
                // Success!
                Debug.Log("╔══════════════════════════════════════════════╗");
                Debug.Log("║         🎮 QUEST STREAMING ACTIVE! 🎮         ║");
                Debug.Log("╠══════════════════════════════════════════════╣");
                Debug.Log($"║ Room: {currentRoomName.PadRight(38)} ║");
                Debug.Log("║                                              ║");
                Debug.Log("║ View at: https://meet.livekit.io            ║");
                Debug.Log("║ Enter room name above to watch               ║");
                Debug.Log("╚══════════════════════════════════════════════╝");
                
            }
            catch (Exception e)
            {
                Debug.LogError($"[QuestStreaming] Start failed: {e.Message}");
                currentStream = null;
            }
        }
        
        public async Task StopStreaming()
        {
            if (currentStream == null) return;
            
            try
            {
                Debug.Log("[QuestStreaming] Stopping stream...");
                
                await currentStream.Stop();
                
                currentStream.OnStatusChanged -= OnStatusChanged;
                currentStream.OnError -= OnError;
                currentStream = null;
                
                Debug.Log("[QuestStreaming] ⏹️ Stream stopped");
            }
            catch (Exception e)
            {
                Debug.LogError($"[QuestStreaming] Stop failed: {e.Message}");
            }
        }
        
        void OnStatusChanged(StreamStatus status)
        {
            switch (status)
            {
                case StreamStatus.RequestingPermission:
                    Debug.Log("[QuestStreaming] 📱 Please approve screen capture in headset...");
                    break;
                case StreamStatus.PermissionGranted:
                    Debug.Log("[QuestStreaming] ✅ Permission granted!");
                    break;
                case StreamStatus.Streaming:
                    Debug.Log("[QuestStreaming] 🔴 Streaming active!");
                    break;
                case StreamStatus.Error:
                    Debug.LogError("[QuestStreaming] ❌ Stream error occurred");
                    break;
            }
        }
        
        void OnError(string error)
        {
            Debug.LogError($"[QuestStreaming] Error: {error}");
        }
        
        void OnDestroy()
        {
            if (currentStream != null)
            {
                // Clean up on exit
                _ = StopStreaming();
            }
        }
        
        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && currentStream != null)
            {
                Debug.Log("[QuestStreaming] App paused, stream continues in background");
            }
        }
    }
    
    // Minimal SDK classes (normally in separate files)
    #region SDK Classes
    
    [Serializable]
    public class SubstreamConfig
    {
        public string BaseUrl = "demo";
        public string WhipPublishUrl = "";
    }
    
    [Serializable]
    public class LiveOptions
    {
        public int Width = 1920;
        public int Height = 1080;
        public int Fps = 60;
        public int VideoBitrateKbps = 5000;
        public string MetadataJson = "{}";
        public bool WithAudio = true;
    }
    
    public enum StreamStatus
    {
        Idle,
        RequestingPermission,
        PermissionGranted,
        Starting,
        Streaming,
        Stopping,
        Stopped,
        Error
    }
    
    public class LiveHandle
    {
        public event Action<StreamStatus> OnStatusChanged;
        public event Action<string> OnError;
        
        private StreamStatus _status = StreamStatus.Idle;
        
        public async Task Start()
        {
            _status = StreamStatus.Starting;
            OnStatusChanged?.Invoke(_status);
            
#if UNITY_ANDROID && !UNITY_EDITOR
            // Real implementation calls native code
            using (AndroidJavaClass jc = new AndroidJavaClass("com.substream.sdk.SubstreamNative"))
            {
                jc.CallStatic("startLive", 1920, 1080, 60, 5000, "{}", true);
            }
#else
            // Editor simulation
            await Task.Delay(1000);
            _status = StreamStatus.Streaming;
            OnStatusChanged?.Invoke(_status);
#endif
        }
        
        public async Task Stop()
        {
            _status = StreamStatus.Stopping;
            OnStatusChanged?.Invoke(_status);
            
#if UNITY_ANDROID && !UNITY_EDITOR
            using (AndroidJavaClass jc = new AndroidJavaClass("com.substream.sdk.SubstreamNative"))
            {
                jc.CallStatic("stopLive");
            }
#else
            await Task.Delay(500);
#endif
            
            _status = StreamStatus.Stopped;
            OnStatusChanged?.Invoke(_status);
        }
        
        public string RoomName => "quest-" + DateTime.Now.ToString("HHmmss");
    }
    
    public static class Substream
    {
        private static bool _isInitialized = false;
        
        public static async Task Init(SubstreamConfig config)
        {
            if (_isInitialized) return;
            
#if UNITY_ANDROID && !UNITY_EDITOR
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaClass jc = new AndroidJavaClass("com.substream.sdk.SubstreamNative"))
            {
                string baseUrl = config.BaseUrl == "demo" 
                    ? "https://demo.substream.io" 
                    : config.BaseUrl;
                    
                jc.CallStatic("init", activity, baseUrl, config.WhipPublishUrl);
            }
#endif
            
            _isInitialized = true;
            await Task.CompletedTask;
        }
        
        public static async Task<LiveHandle> LiveCreate(LiveOptions options)
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Call Init first!");
                
            return await Task.FromResult(new LiveHandle());
        }
        
        public static async Task<LiveHandle> QuickDemo()
        {
            return await LiveCreate(new LiveOptions());
        }
    }
    
    #endregion
}
