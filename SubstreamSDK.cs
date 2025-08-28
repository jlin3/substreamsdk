using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// SUBSTREAM SDK - One-line streaming for Unity games
/// 
/// Add this component to any GameObject and streaming UI appears automatically.
/// Works on Meta Quest, PC, and mobile platforms.
/// 
/// Usage:
///   gameObject.AddComponent<SubstreamSDK>();
/// 
/// Or with configuration:
///   var sdk = gameObject.AddComponent<SubstreamSDK>();
///   sdk.autoStart = true;
///   sdk.quality = SubstreamSDK.Quality.HD;
/// </summary>
[AddComponentMenu("Substream/Substream SDK")]
[HelpURL("https://github.com/substreamsdk/unity")]
public class SubstreamSDK : MonoBehaviour
{
    #region Public Configuration
    
    [Header("Quick Setup")]
    [Tooltip("Start streaming automatically when scene loads")]
    public bool autoStart = false;
    
    [Tooltip("Show floating UI panel for manual control")]
    public bool showUI = true;
    
    [Tooltip("Streaming quality preset")]
    public Quality quality = Quality.HD;
    
    [Header("Advanced Settings")]
    [Tooltip("Custom LiveKit URL (leave empty for default)")]
    public string customLiveKitUrl = "";
    
    [Tooltip("UI position on screen")]
    public UIPosition uiPosition = UIPosition.TopRight;
    
    [Tooltip("UI scale factor")]
    [Range(0.5f, 2f)]
    public float uiScale = 1f;
    
    #endregion
    
    #region Enums
    
    public enum Quality
    {
        Low,      // 720p, 30fps, 2Mbps
        HD,       // 1080p, 60fps, 5Mbps  
        Ultra,    // 4K, 60fps, 10Mbps
        Custom    // Use custom settings
    }
    
    public enum UIPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        BottomLeft,
        BottomCenter,
        BottomRight,
        Center
    }
    
    public enum StreamingMethod
    {
        Auto,          // Automatically detect best method
        QuestNative,   // Meta Quest MediaProjection
        ScreenShare,   // Browser screen sharing
        LiveKitSDK,    // LiveKit Unity SDK
        Disabled       // No streaming
    }
    
    #endregion
    
    #region Private Fields
    
    // Configuration
    private const string DEFAULT_LIVEKIT_URL = "wss://substream-cnzdthyx.livekit.cloud";
    private const string DEFAULT_API_KEY = "APIbtpHuQYmSvTT";
    private const string DEFAULT_API_SECRET = "RmbpdbNBZVkbAW8yW0K3ekrjwcdqIKNo6OQDwt0211Y";
    
    // State
    private bool isStreaming = false;
    private StreamingMethod currentMethod = StreamingMethod.Auto;
    private string roomName = "";
    private string viewerUrl = "";
    
    // UI
    private GameObject uiContainer;
    private Button streamButton;
    private Text statusText;
    private bool uiMinimized = false;
    
    // Quality settings
    private int streamWidth;
    private int streamHeight;
    private int streamFps;
    private int bitrateKbps;
    
    #endregion
    
    #region Unity Lifecycle
    
    void Awake()
    {
        ApplyQualitySettings();
        DetectStreamingMethod();
    }
    
    void Start()
    {
        if (showUI)
        {
            CreateStreamingUI();
        }
        
        if (autoStart)
        {
            StartCoroutine(AutoStartStreaming());
        }
        
        LogInitialization();
    }
    
    void OnDestroy()
    {
        if (isStreaming)
        {
            StopStreaming();
        }
    }
    
    #endregion
    
    #region Public API
    
    /// <summary>
    /// Start streaming with current settings
    /// </summary>
    public void StartStreaming()
    {
        if (isStreaming)
        {
            Debug.LogWarning("[Substream] Already streaming");
            return;
        }
        
        StartCoroutine(StartStreamingCoroutine());
    }
    
    /// <summary>
    /// Stop current streaming session
    /// </summary>
    public void StopStreaming()
    {
        if (!isStreaming)
        {
            Debug.LogWarning("[Substream] Not currently streaming");
            return;
        }
        
        StartCoroutine(StopStreamingCoroutine());
    }
    
    /// <summary>
    /// Get the current viewer URL (empty if not streaming)
    /// </summary>
    public string GetViewerUrl()
    {
        return viewerUrl;
    }
    
    /// <summary>
    /// Check if currently streaming
    /// </summary>
    public bool IsStreaming()
    {
        return isStreaming;
    }
    
    /// <summary>
    /// Set custom quality parameters
    /// </summary>
    public void SetCustomQuality(int width, int height, int fps, int bitrate)
    {
        quality = Quality.Custom;
        streamWidth = width;
        streamHeight = height;
        streamFps = fps;
        bitrateKbps = bitrate;
    }
    
    #endregion
    
    #region Initialization
    
    void ApplyQualitySettings()
    {
        switch (quality)
        {
            case Quality.Low:
                streamWidth = 1280;
                streamHeight = 720;
                streamFps = 30;
                bitrateKbps = 2000;
                break;
                
            case Quality.HD:
                streamWidth = 1920;
                streamHeight = 1080;
                streamFps = 60;
                bitrateKbps = 5000;
                break;
                
            case Quality.Ultra:
                streamWidth = 3840;
                streamHeight = 2160;
                streamFps = 60;
                bitrateKbps = 10000;
                break;
                
            case Quality.Custom:
                // Keep existing custom values
                break;
        }
    }
    
    void DetectStreamingMethod()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        // On Quest/Android
        if (HasNativeQuestSupport())
        {
            currentMethod = StreamingMethod.QuestNative;
        }
        else
        {
            currentMethod = StreamingMethod.ScreenShare;
        }
        #else
        // Editor or other platforms
        currentMethod = StreamingMethod.ScreenShare;
        #endif
    }
    
    bool HasNativeQuestSupport()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var jc = new AndroidJavaClass("com.substream.sdk.SubstreamNative"))
            {
                return jc != null;
            }
        }
        catch
        {
            return false;
        }
        #else
        return false;
        #endif
    }
    
    void LogInitialization()
    {
        Debug.Log($"[Substream SDK] Initialized - Method: {currentMethod}, Quality: {quality} ({streamWidth}x{streamHeight}@{streamFps}fps)");
    }
    
    #endregion
    
    #region UI Creation
    
    void CreateStreamingUI()
    {
        // Create container
        uiContainer = new GameObject("Substream UI");
        uiContainer.transform.SetParent(transform, false);
        
        // Add canvas
        Canvas canvas = uiContainer.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        
        // Create panel
        GameObject panel = CreateUIPanel();
        
        // Create minimize button
        CreateMinimizeButton(panel);
        
        // Create status text
        statusText = CreateStatusText(panel);
        
        // Create stream button  
        streamButton = CreateStreamButton(panel);
        
        UpdateUIPosition();
    }
    
    GameObject CreateUIPanel()
    {
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(uiContainer.transform, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(250 * uiScale, 100 * uiScale);
        
        Image img = panel.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.8f);
        
        // Add outline
        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(0.2f, 0.7f, 1f, 0.5f);
        outline.effectDistance = new Vector2(2, 2);
        
        return panel;
    }
    
    void CreateMinimizeButton(GameObject panel)
    {
        GameObject minBtn = new GameObject("Minimize");
        minBtn.transform.SetParent(panel.transform, false);
        
        Button btn = minBtn.AddComponent<Button>();
        Image img = minBtn.AddComponent<Image>();
        img.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        
        RectTransform rect = minBtn.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.sizeDelta = new Vector2(30 * uiScale, 30 * uiScale);
        rect.anchoredPosition = new Vector2(-5, -5);
        
        // Add minimize text
        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(minBtn.transform, false);
        Text txt = txtObj.AddComponent<Text>();
        txt.text = "−";
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = Mathf.RoundToInt(20 * uiScale);
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        
        RectTransform txtRect = txtObj.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.sizeDelta = Vector2.zero;
        
        btn.onClick.AddListener(ToggleMinimize);
    }
    
    Text CreateStatusText(GameObject panel)
    {
        GameObject txtObj = new GameObject("Status");
        txtObj.transform.SetParent(panel.transform, false);
        
        Text txt = txtObj.AddComponent<Text>();
        txt.text = "Ready to stream";
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = Mathf.RoundToInt(14 * uiScale);
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        
        RectTransform rect = txtObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.5f);
        rect.anchorMax = new Vector2(1, 1);
        rect.offsetMin = new Vector2(10, 0);
        rect.offsetMax = new Vector2(-10, -35 * uiScale);
        
        return txt;
    }
    
    Button CreateStreamButton(GameObject panel)
    {
        GameObject btnObj = new GameObject("Stream Button");
        btnObj.transform.SetParent(panel.transform, false);
        
        Button btn = btnObj.AddComponent<Button>();
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.7f, 0.3f);
        
        RectTransform rect = btnObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 0.5f);
        rect.offsetMin = new Vector2(10, 10);
        rect.offsetMax = new Vector2(-10, -5);
        
        // Button text
        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(btnObj.transform, false);
        Text txt = txtObj.AddComponent<Text>();
        txt.text = "🔴 Go Live";
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = Mathf.RoundToInt(16 * uiScale);
        txt.fontStyle = FontStyle.Bold;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        
        RectTransform txtRect = txtObj.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.sizeDelta = Vector2.zero;
        
        btn.onClick.AddListener(() => {
            if (isStreaming) StopStreaming();
            else StartStreaming();
        });
        
        return btn;
    }
    
    void UpdateUIPosition()
    {
        if (uiContainer == null) return;
        
        RectTransform rect = uiContainer.GetComponent<RectTransform>();
        if (rect == null) return;
        
        switch (uiPosition)
        {
            case UIPosition.TopLeft:
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
                rect.anchoredPosition = new Vector2(20, -20);
                break;
                
            case UIPosition.TopCenter:
                rect.anchorMin = new Vector2(0.5f, 1);
                rect.anchorMax = new Vector2(0.5f, 1);
                rect.pivot = new Vector2(0.5f, 1);
                rect.anchoredPosition = new Vector2(0, -20);
                break;
                
            case UIPosition.TopRight:
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1, 1);
                rect.anchoredPosition = new Vector2(-20, -20);
                break;
                
            case UIPosition.BottomLeft:
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 0);
                rect.pivot = new Vector2(0, 0);
                rect.anchoredPosition = new Vector2(20, 20);
                break;
                
            case UIPosition.BottomCenter:
                rect.anchorMin = new Vector2(0.5f, 0);
                rect.anchorMax = new Vector2(0.5f, 0);
                rect.pivot = new Vector2(0.5f, 0);
                rect.anchoredPosition = new Vector2(0, 20);
                break;
                
            case UIPosition.BottomRight:
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(1, 0);
                rect.anchoredPosition = new Vector2(-20, 20);
                break;
                
            case UIPosition.Center:
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
                break;
        }
    }
    
    void ToggleMinimize()
    {
        uiMinimized = !uiMinimized;
        
        Transform panel = uiContainer.transform.GetChild(0);
        RectTransform rect = panel.GetComponent<RectTransform>();
        
        if (uiMinimized)
        {
            rect.sizeDelta = new Vector2(40 * uiScale, 40 * uiScale);
            statusText.gameObject.SetActive(false);
            streamButton.gameObject.SetActive(false);
        }
        else
        {
            rect.sizeDelta = new Vector2(250 * uiScale, 100 * uiScale);
            statusText.gameObject.SetActive(true);
            streamButton.gameObject.SetActive(true);
        }
    }
    
    #endregion
    
    #region Streaming Logic
    
    IEnumerator AutoStartStreaming()
    {
        yield return new WaitForSeconds(1f); // Wait for scene to settle
        StartStreaming();
    }
    
    IEnumerator StartStreamingCoroutine()
    {
        isStreaming = true;
        UpdateStatus("Starting stream...");
        
        // Generate room name
        roomName = $"unity-{SystemInfo.deviceName}-{DateTime.Now:HHmmss}".ToLower().Replace(" ", "-");
        
        // Create viewer URL
        string liveKitUrl = string.IsNullOrEmpty(customLiveKitUrl) ? DEFAULT_LIVEKIT_URL : customLiveKitUrl;
        string encodedUrl = System.Uri.EscapeDataString(liveKitUrl);
        viewerUrl = $"https://meet.livekit.io/custom?liveKitUrl={encodedUrl}&roomName={roomName}";
        
        yield return new WaitForSeconds(0.5f); // Simulate setup
        
        switch (currentMethod)
        {
            case StreamingMethod.QuestNative:
                yield return StartQuestNativeStreaming();
                break;
                
            case StreamingMethod.ScreenShare:
                yield return StartScreenShareStreaming();
                break;
                
            case StreamingMethod.LiveKitSDK:
                UpdateStatus("LiveKit SDK not available");
                yield break;
        }
        
        UpdateStreamButton("⏹️ Stop", new Color(0.8f, 0.2f, 0.2f));
        
        Debug.Log($"[Substream] Stream started - Room: {roomName}");
        Debug.Log($"[Substream] Viewer URL: {viewerUrl}");
        
        // Copy URL to clipboard if possible
        GUIUtility.systemCopyBuffer = viewerUrl;
    }
    
    IEnumerator StartQuestNativeStreaming()
    {
        UpdateStatus("Requesting permission...");
        
        #if UNITY_ANDROID && !UNITY_EDITOR
        using (var jc = new AndroidJavaClass("com.substream.sdk.SubstreamNative"))
        {
            jc.CallStatic("startLive", streamWidth, streamHeight, streamFps, bitrateKbps, "{}", true);
        }
        #endif
        
        yield return new WaitForSeconds(2f);
        UpdateStatus($"🔴 LIVE ({currentMethod})");
    }
    
    IEnumerator StartScreenShareStreaming()
    {
        UpdateStatus("🔴 Room ready - Share screen");
        
        // Open viewer URL
        Application.OpenURL(viewerUrl);
        
        yield return null;
    }
    
    IEnumerator StopStreamingCoroutine()
    {
        UpdateStatus("Stopping stream...");
        
        #if UNITY_ANDROID && !UNITY_EDITOR
        if (currentMethod == StreamingMethod.QuestNative)
        {
            using (var jc = new AndroidJavaClass("com.substream.sdk.SubstreamNative"))
            {
                jc.CallStatic("stopLive");
            }
        }
        #endif
        
        yield return new WaitForSeconds(0.5f);
        
        isStreaming = false;
        roomName = "";
        viewerUrl = "";
        
        UpdateStatus("Ready to stream");
        UpdateStreamButton("🔴 Go Live", new Color(0.2f, 0.7f, 0.3f));
        
        Debug.Log("[Substream] Stream stopped");
    }
    
    void UpdateStatus(string status)
    {
        if (statusText != null)
        {
            statusText.text = status;
        }
    }
    
    void UpdateStreamButton(string text, Color color)
    {
        if (streamButton != null)
        {
            streamButton.GetComponentInChildren<Text>().text = text;
            streamButton.GetComponent<Image>().color = color;
        }
    }
    
    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(SubstreamSDK))]
public class SubstreamSDKEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Substream SDK is ready! Just press Play to test streaming.", MessageType.Info);
        
        SubstreamSDK sdk = (SubstreamSDK)target;
        
        if (Application.isPlaying)
        {
            EditorGUILayout.Space();
            
            if (sdk.IsStreaming())
            {
                EditorGUILayout.HelpBox($"Currently streaming!\nViewer URL: {sdk.GetViewerUrl()}", MessageType.None);
                
                if (GUILayout.Button("Copy Viewer URL"))
                {
                    GUIUtility.systemCopyBuffer = sdk.GetViewerUrl();
                }
                
                if (GUILayout.Button("Stop Streaming"))
                {
                    sdk.StopStreaming();
                }
            }
            else
            {
                if (GUILayout.Button("Start Streaming"))
                {
                    sdk.StartStreaming();
                }
            }
        }
    }
}
#endif
