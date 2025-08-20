using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SubstreamSDK
{
    [Serializable]
    public class SubstreamConfig
    {
        public string BaseUrl = "demo"; // Use "demo" for quick testing
        public string WhipPublishUrl = "";
    }

    [Serializable]
    public class LiveOptions
    {
        public int Width = 1280;
        public int Height = 720;
        public int Fps = 30;
        public int VideoBitrateKbps = 3500;
        public string MetadataJson = "{}";
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
        public StreamStatus Status 
        { 
            get => _status;
            private set
            {
                _status = value;
                OnStatusChanged?.Invoke(value);
            }
        }

        public async Task Start()
        {
            try
            {
                Status = StreamStatus.Starting;
#if UNITY_ANDROID && !UNITY_EDITOR
                CallAndroid("startLive", _width, _height, _fps, _bitrate, _metadata);
#else
                Debug.Log($"[Substream] Start live {_width}x{_height}@{_fps}fps {_bitrate}kbps");
                await Task.Delay(1000); // Simulate startup
                Status = StreamStatus.Streaming;
#endif
            }
            catch (Exception e)
            {
                Status = StreamStatus.Error;
                OnError?.Invoke(e.Message);
                throw;
            }
        }
        
        public async Task Stop()
        {
            try
            {
                Status = StreamStatus.Stopping;
#if UNITY_ANDROID && !UNITY_EDITOR
                CallAndroid("stopLive");
#else
                Debug.Log("[Substream] Stop live");
                await Task.Delay(500); // Simulate shutdown
#endif
                Status = StreamStatus.Stopped;
            }
            catch (Exception e)
            {
                Status = StreamStatus.Error;
                OnError?.Invoke(e.Message);
                throw;
            }
        }

        internal LiveHandle(int width, int height, int fps, int bitrate, string metadata)
        {
            _width = width; 
            _height = height; 
            _fps = fps; 
            _bitrate = bitrate; 
            _metadata = metadata;
            
#if UNITY_ANDROID && !UNITY_EDITOR
            // Set up callbacks from native
            using var jc = new AndroidJavaClass("com.substream.sdk.SubstreamNative");
            jc.CallStatic("setStatusCallback", new AndroidJavaProxy("kotlin.jvm.functions.Function1") {
                // Implementation would handle status updates
            });
#endif
        }

        private readonly int _width; 
        private readonly int _height; 
        private readonly int _fps; 
        private readonly int _bitrate; 
        private readonly string _metadata;

#if UNITY_ANDROID && !UNITY_EDITOR
        private static void CallAndroid(string method, params object[] args)
        {
            using var jc = new AndroidJavaClass("com.substream.sdk.SubstreamNative");
            jc.CallStatic(method, args);
        }
#endif
    }

    public static class Substream
    {
        private static SubstreamConfig _config;
        private static bool _isInitialized = false;

        public static bool IsInitialized => _isInitialized;

        public static async Task Init(SubstreamConfig config)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("[Substream] Already initialized");
                return;
            }

            _config = config;
            
            // Handle demo mode
            if (config.BaseUrl == "demo")
            {
                Debug.Log("[Substream] Initializing in DEMO mode");
#if UNITY_ANDROID && !UNITY_EDITOR
                using var jc = new AndroidJavaClass("com.substream.sdk.SubstreamNative");
                jc.CallStatic("initDemo");
#else
                config.BaseUrl = "http://localhost:8787";
                config.WhipPublishUrl = "http://localhost:8080/whip";
#endif
            }
            else
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                using var jc = new AndroidJavaClass("com.substream.sdk.SubstreamNative");
                jc.CallStatic("init", activity, config.BaseUrl, config.WhipPublishUrl);
#endif
            }
            
            Debug.Log($"[Substream] Initialized - BaseUrl: {config.BaseUrl}");
            _isInitialized = true;
            await Task.CompletedTask;
        }

        public static Task<LiveHandle> LiveCreate(LiveOptions options)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("Substream not initialized. Call Init() first.");
            }
            
            var handle = new LiveHandle(
                options.Width, 
                options.Height, 
                options.Fps, 
                options.VideoBitrateKbps, 
                options.MetadataJson
            );
            
            return Task.FromResult(handle);
        }
        
        // Helper method for quick demo setup
        public static async Task<LiveHandle> QuickDemo()
        {
            if (!_isInitialized)
            {
                await Init(new SubstreamConfig { BaseUrl = "demo" });
            }
            
            return await LiveCreate(new LiveOptions
            {
                Width = Screen.width,
                Height = Screen.height,
                Fps = 60,
                VideoBitrateKbps = 5000,
                MetadataJson = JsonUtility.ToJson(new {
                    game = Application.productName,
                    platform = "Quest",
                    version = Application.version
                })
            });
        }
    }
}
