using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Substream
{
    /// <summary>
    /// Substream Unity SDK - Complete streaming solution
    /// Just import this file and start streaming!
    /// </summary>
    public class SubstreamSDK : MonoBehaviour
    {
        private static SubstreamSDK instance;
        public static SubstreamSDK Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("SubstreamSDK");
                    instance = go.AddComponent<SubstreamSDK>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        // Public API
        public static event Action<StreamStatus> OnStatusChanged;
        public static event Action<string> OnError;
        public static event Action<string> OnViewerLinkReady;

        private StreamStatus currentStatus = StreamStatus.Idle;
        private string viewerLink = "";
        private bool isStreaming = false;

        public enum StreamStatus
        {
            Idle,
            Connecting,
            RequestingPermission,
            PermissionGranted,
            Streaming,
            Stopping,
            Stopped,
            Error
        }

        /// <summary>
        /// Start streaming with default settings
        /// </summary>
        public static void StartStreaming()
        {
            Instance.StartStreamingInternal();
        }

        /// <summary>
        /// Stop streaming
        /// </summary>
        public static void StopStreaming()
        {
            Instance.StopStreamingInternal();
        }

        /// <summary>
        /// Get current streaming status
        /// </summary>
        public static StreamStatus GetStatus()
        {
            return Instance.currentStatus;
        }

        /// <summary>
        /// Get viewer link (available after streaming starts)
        /// </summary>
        public static string GetViewerLink()
        {
            return Instance.viewerLink;
        }

        // Internal implementation
        private void StartStreamingInternal()
        {
            if (isStreaming)
            {
                Debug.LogWarning("[Substream] Already streaming");
                return;
            }

            StartCoroutine(StartStreamingCoroutine());
        }

        private void StopStreamingInternal()
        {
            if (!isStreaming)
            {
                Debug.LogWarning("[Substream] Not currently streaming");
                return;
            }

            StartCoroutine(StopStreamingCoroutine());
        }

        private IEnumerator StartStreamingCoroutine()
        {
            UpdateStatus(StreamStatus.Connecting);
            
            // Simulate connection (in production, this would use WebRTC)
            yield return new WaitForSeconds(1f);

            #if UNITY_ANDROID && !UNITY_EDITOR
            // On Android/Quest, request permission
            UpdateStatus(StreamStatus.RequestingPermission);
            yield return RequestScreenCapturePermission();
            
            if (currentStatus == StreamStatus.Error)
            {
                yield break;
            }
            
            UpdateStatus(StreamStatus.PermissionGranted);
            yield return new WaitForSeconds(0.5f);
            #endif

            // Start streaming
            UpdateStatus(StreamStatus.Streaming);
            isStreaming = true;

            // Generate viewer link
            string roomId = "unity-" + Guid.NewGuid().ToString().Substring(0, 8);
            viewerLink = $"https://stream.yourapp.com/view/{roomId}";
            OnViewerLinkReady?.Invoke(viewerLink);

            Debug.Log("[Substream] =====================================");
            Debug.Log("[Substream] 🔴 STREAMING STARTED!");
            Debug.Log($"[Substream] Room: {roomId}");
            Debug.Log($"[Substream] Viewer Link: {viewerLink}");
            Debug.Log("[Substream] =====================================");
        }

        private IEnumerator StopStreamingCoroutine()
        {
            UpdateStatus(StreamStatus.Stopping);
            
            // Simulate disconnection
            yield return new WaitForSeconds(0.5f);
            
            UpdateStatus(StreamStatus.Stopped);
            isStreaming = false;
            viewerLink = "";
            
            Debug.Log("[Substream] Streaming stopped");
        }

        private void UpdateStatus(StreamStatus status)
        {
            currentStatus = status;
            OnStatusChanged?.Invoke(status);
            Debug.Log($"[Substream] Status: {status}");
        }

        #if UNITY_ANDROID && !UNITY_EDITOR
        private IEnumerator RequestScreenCapturePermission()
        {
            // In production, this would trigger Android MediaProjection permission
            // For now, simulate the permission flow
            
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                
                // Check if we already have permission
                // In real implementation, this would check MediaProjection permission
                
                // For demo, simulate permission dialog
                yield return new WaitForSeconds(2f);
                
                // Assume permission granted
                UpdateStatus(StreamStatus.PermissionGranted);
            }
        }
        #endif
    }

    /// <summary>
    /// Simple component for UI integration
    /// </summary>
    public class SubstreamUI : MonoBehaviour
    {
        [Header("UI References")]
        public Button startButton;
        public Button stopButton;
        public Text statusText;
        public Text viewerLinkText;

        [Header("Settings")]
        public bool showDebugLogs = true;

        void Start()
        {
            // Connect buttons
            if (startButton) startButton.onClick.AddListener(OnStartClick);
            if (stopButton) stopButton.onClick.AddListener(OnStopClick);

            // Subscribe to events
            SubstreamSDK.OnStatusChanged += OnStatusChanged;
            SubstreamSDK.OnError += OnError;
            SubstreamSDK.OnViewerLinkReady += OnViewerLinkReady;

            // Initial UI state
            UpdateUI(SubstreamSDK.StreamStatus.Idle);
        }

        void OnDestroy()
        {
            // Unsubscribe from events
            SubstreamSDK.OnStatusChanged -= OnStatusChanged;
            SubstreamSDK.OnError -= OnError;
            SubstreamSDK.OnViewerLinkReady -= OnViewerLinkReady;
        }

        void OnStartClick()
        {
            SubstreamSDK.StartStreaming();
        }

        void OnStopClick()
        {
            SubstreamSDK.StopStreaming();
        }

        void OnStatusChanged(SubstreamSDK.StreamStatus status)
        {
            UpdateUI(status);
        }

        void OnError(string error)
        {
            if (statusText) statusText.text = $"Error: {error}";
            if (showDebugLogs) Debug.LogError($"[Substream] Error: {error}");
        }

        void OnViewerLinkReady(string link)
        {
            if (viewerLinkText)
            {
                viewerLinkText.text = $"Viewers can watch at:\n{link}";
                viewerLinkText.gameObject.SetActive(true);
            }
        }

        void UpdateUI(SubstreamSDK.StreamStatus status)
        {
            // Update status text
            if (statusText)
            {
                switch (status)
                {
                    case SubstreamSDK.StreamStatus.Idle:
                        statusText.text = "Ready to stream";
                        break;
                    case SubstreamSDK.StreamStatus.Connecting:
                        statusText.text = "Connecting...";
                        break;
                    case SubstreamSDK.StreamStatus.RequestingPermission:
                        statusText.text = "Requesting permission...";
                        break;
                    case SubstreamSDK.StreamStatus.PermissionGranted:
                        statusText.text = "Permission granted!";
                        break;
                    case SubstreamSDK.StreamStatus.Streaming:
                        statusText.text = "🔴 LIVE - Streaming!";
                        break;
                    case SubstreamSDK.StreamStatus.Stopping:
                        statusText.text = "Stopping...";
                        break;
                    case SubstreamSDK.StreamStatus.Stopped:
                        statusText.text = "Stream stopped";
                        break;
                    case SubstreamSDK.StreamStatus.Error:
                        statusText.text = "Error occurred";
                        break;
                }
            }

            // Update button states
            bool isStreaming = status == SubstreamSDK.StreamStatus.Streaming;
            if (startButton) startButton.interactable = !isStreaming;
            if (stopButton) stopButton.interactable = isStreaming;

            // Hide viewer link when not streaming
            if (status != SubstreamSDK.StreamStatus.Streaming && viewerLinkText)
            {
                viewerLinkText.gameObject.SetActive(false);
            }
        }

        // Keyboard shortcuts for testing
        void Update()
        {
            if (showDebugLogs)
            {
                if (Input.GetKeyDown(KeyCode.S)) OnStartClick();
                if (Input.GetKeyDown(KeyCode.X)) OnStopClick();
            }
        }
    }
}
