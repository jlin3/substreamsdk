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
                CallAndroid("startLive", _width, _height, _fps, _bitrate, _metadata, _withAudio);
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

        internal LiveHandle(int width, int height, int fps, int bitrate, string metadata, bool withAudio)
        {
            _width = width; 
            _height = height; 
            _fps = fps; 
            _bitrate = bitrate; 
            _metadata = metadata;
            _withAudio = withAudio;
#if UNITY_ANDROID && !UNITY_EDITOR
            using var jc = new AndroidJavaClass("com.substream.sdk.SubstreamNative");
            jc.CallStatic("setStatusCallback", new StatusCallbackProxy(this));
            jc.CallStatic("setErrorCallback", new ErrorCallbackProxy(this));
#endif
        }

        private int _width; 
        private int _height; 
        private int _fps; 
        private int _bitrate; 
        private readonly string _metadata;
        private readonly bool _withAudio;

        public void UpdateQuality(int width, int height, int fps, int bitrateKbps)
        {
            _width = width;
            _height = height;
            _fps = fps;
            _bitrate = bitrateKbps;
#if UNITY_ANDROID && !UNITY_EDITOR
            CallAndroid("adjustQuality", width, height, fps, bitrateKbps);
#else
            Debug.Log($"[Substream] Adjust quality to {width}x{height}@{fps} {bitrateKbps}kbps");
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private static void CallAndroid(string method, params object[] args)
        {
            using var jc = new AndroidJavaClass("com.substream.sdk.SubstreamNative");
            jc.CallStatic(method, args);
        }

        private class StatusCallbackProxy : AndroidJavaProxy
        {
            private readonly LiveHandle _owner;
            public StatusCallbackProxy(LiveHandle owner) : base("kotlin.jvm.functions.Function1") { _owner = owner; }
            public AndroidJavaObject invoke(AndroidJavaObject statusObj)
            {
                string status = statusObj?.Call<string>("toString");
                switch (status)
                {
                    case "requesting_permission": _owner.Status = StreamStatus.RequestingPermission; break;
                    case "permission_granted": _owner.Status = StreamStatus.PermissionGranted; break;
                    case "starting": _owner.Status = StreamStatus.Starting; break;
                    case "streaming": _owner.Status = StreamStatus.Streaming; break;
                    case "stopping": _owner.Status = StreamStatus.Stopping; break;
                    case "stopped": _owner.Status = StreamStatus.Stopped; break;
                }
                return null;
            }
        }

        private class ErrorCallbackProxy : AndroidJavaProxy
        {
            private readonly LiveHandle _owner;
            public ErrorCallbackProxy(LiveHandle owner) : base("kotlin.jvm.functions.Function1") { _owner = owner; }
            public AndroidJavaObject invoke(AndroidJavaObject messageObj)
            {
                string message = messageObj?.Call<string>("toString");
                _owner.Status = StreamStatus.Error;
                _owner.OnError?.Invoke(message);
                return null;
            }
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
                options.MetadataJson,
                options.WithAudio
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
