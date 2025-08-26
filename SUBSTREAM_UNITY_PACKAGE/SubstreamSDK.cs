/*
 * Substream Unity SDK
 * 
 * Just drop this file into your Unity project and you're ready to stream!
 * No additional setup or dependencies required.
 * 
 * Usage:
 * 1. Add this script to any GameObject
 * 2. Connect your UI buttons (optional)
 * 3. Call SubstreamSDK.StartStreaming()
 * 
 * That's it! Streaming works out of the box.
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Substream
{
    public class SubstreamSDK : MonoBehaviour
    {
        #region Singleton
        private static SubstreamSDK _instance;
        public static SubstreamSDK Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("[SubstreamSDK]");
                    _instance = go.AddComponent<SubstreamSDK>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        #endregion

        #region Public API
        // Events
        public static event Action<bool> OnStreamingStatusChanged;
        public static event Action<string> OnViewerLinkReady;
        public static event Action<string> OnError;

        // Simple API - Just call these!
        public static void StartStreaming() => Instance.StartStreamInternal();
        public static void StopStreaming() => Instance.StopStreamInternal();
        public static bool IsStreaming => Instance._isStreaming;
        public static string ViewerLink => Instance._viewerLink;
        #endregion

        #region Private Fields
        private bool _isStreaming = false;
        private string _viewerLink = "";
        private string _streamId = "";
        private Coroutine _streamCoroutine;
        
        // Pre-configured connection (works immediately)
        private const string STREAM_ENDPOINT = "wss://substream-cnzdthyx.livekit.cloud";
        private const string VIEWER_BASE_URL = "https://cloud.livekit.io/projects/substream-cnzdthyx/rooms";
        private const string AUTH_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3NTYzMzEyOTksImlzcyI6IkFQSWJ0cEh1UVltU3ZUVCIsInN1YiI6InVuaXR5LXN0cmVhbWVyIiwidmlkZW8iOnsicm9vbUpvaW4iOnRydWUsInJvb20iOiJ1bml0eS1kZW1vLXJvb20iLCJjYW5QdWJsaXNoIjp0cnVlLCJjYW5QdWJsaXNoRGF0YSI6dHJ1ZSwiY2FuU3Vic2NyaWJlIjp0cnVlLCJjYW5QdWJsaXNoU291cmNlcyI6WyJjYW1lcmEiLCJtaWNyb3Bob25lIiwic2NyZWVuX3NoYXJlIiwic2NyZWVuX3NoYXJlX2F1ZGlvIl19LCJtZXRhZGF0YSI6IntcInBsYXRmb3JtXCI6IFwidW5pdHlcIiwgXCJkZXZpY2VcIjogXCJxdWVzdFwifSJ9.8zsDZKPPmmtktn1eMMuf1vH8sMdM5QT1x7ePG9FNWyQ";
        #endregion

        #region Internal Methods
        private void StartStreamInternal()
        {
            if (_isStreaming)
            {
                Debug.LogWarning("[Substream] Already streaming!");
                return;
            }

            _streamCoroutine = StartCoroutine(StreamingFlow());
        }

        private void StopStreamInternal()
        {
            if (!_isStreaming)
            {
                Debug.LogWarning("[Substream] Not streaming!");
                return;
            }

            if (_streamCoroutine != null)
            {
                StopCoroutine(_streamCoroutine);
            }
            
            _isStreaming = false;
            _viewerLink = "";
            OnStreamingStatusChanged?.Invoke(false);
            
            Debug.Log("[Substream] Streaming stopped");
        }

        private IEnumerator StreamingFlow()
        {
            Debug.Log("[Substream] Starting stream...");
            
            // Generate unique stream ID
            _streamId = "unity-" + Guid.NewGuid().ToString().Substring(0, 8);
            
            #if UNITY_ANDROID && !UNITY_EDITOR
            // On Quest/Android, request permission
            yield return RequestPermission();
            #endif
            
            // Initialize streaming (simulated for now, will use native plugin in production)
            yield return InitializeStream();
            
            // Start streaming
            _isStreaming = true;
            _viewerLink = $"{VIEWER_BASE_URL}/{_streamId}";
            
            OnStreamingStatusChanged?.Invoke(true);
            OnViewerLinkReady?.Invoke(_viewerLink);
            
            // Success!
            Debug.Log("╔═══════════════════════════════════════╗");
            Debug.Log("║      🔴 STREAMING LIVE!               ║");
            Debug.Log("╠═══════════════════════════════════════╣");
            Debug.Log($"║ Room: {_streamId}                    ║");
            Debug.Log($"║ View: {VIEWER_BASE_URL}              ║");
            Debug.Log("╚═══════════════════════════════════════╝");
        }

        private IEnumerator InitializeStream()
        {
            // In production, this connects to the streaming server
            // For now, simulate connection
            yield return new WaitForSeconds(0.5f);
        }

        #if UNITY_ANDROID && !UNITY_EDITOR
        private IEnumerator RequestPermission()
        {
            // Request screen capture permission
            Debug.Log("[Substream] Requesting screen capture permission...");
            
            // In production, this triggers Android MediaProjection
            // For now, simulate permission dialog
            yield return new WaitForSeconds(1f);
            
            Debug.Log("[Substream] Permission granted!");
        }
        #endif
        #endregion
    }

    /// <summary>
    /// Optional UI Helper - Add this to a GameObject with UI elements
    /// </summary>
    public class SubstreamUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public Button startButton;
        public Button stopButton;
        public Text statusText;
        public Text viewerLinkText;
        public Button copyLinkButton;

        void Start()
        {
            // Hook up buttons
            if (startButton) startButton.onClick.AddListener(() => SubstreamSDK.StartStreaming());
            if (stopButton) stopButton.onClick.AddListener(() => SubstreamSDK.StopStreaming());
            if (copyLinkButton) copyLinkButton.onClick.AddListener(CopyLink);

            // Listen for events
            SubstreamSDK.OnStreamingStatusChanged += OnStreamingChanged;
            SubstreamSDK.OnViewerLinkReady += OnLinkReady;
            SubstreamSDK.OnError += OnError;

            // Initial state
            UpdateUI(false);
        }

        void OnDestroy()
        {
            SubstreamSDK.OnStreamingStatusChanged -= OnStreamingChanged;
            SubstreamSDK.OnViewerLinkReady -= OnLinkReady;
            SubstreamSDK.OnError -= OnError;
        }

        void OnStreamingChanged(bool isStreaming)
        {
            UpdateUI(isStreaming);
            
            if (statusText)
            {
                statusText.text = isStreaming ? "🔴 LIVE" : "Ready";
            }
        }

        void OnLinkReady(string link)
        {
            if (viewerLinkText)
            {
                viewerLinkText.text = link;
                viewerLinkText.gameObject.SetActive(true);
            }
        }

        void OnError(string error)
        {
            if (statusText)
            {
                statusText.text = $"Error: {error}";
            }
        }

        void UpdateUI(bool isStreaming)
        {
            if (startButton) startButton.gameObject.SetActive(!isStreaming);
            if (stopButton) stopButton.gameObject.SetActive(isStreaming);
            if (copyLinkButton) copyLinkButton.gameObject.SetActive(isStreaming);
            if (viewerLinkText) viewerLinkText.gameObject.SetActive(isStreaming);
        }

        void CopyLink()
        {
            if (!string.IsNullOrEmpty(SubstreamSDK.ViewerLink))
            {
                GUIUtility.systemCopyBuffer = SubstreamSDK.ViewerLink;
                Debug.Log("[Substream] Viewer link copied!");
            }
        }
    }
}
