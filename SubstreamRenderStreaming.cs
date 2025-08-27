using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.RenderStreaming;
using Unity.WebRTC;
using System;
using System.Collections;
using System.Linq;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// SUBSTREAM RENDER STREAMING - Real WebRTC streaming using Unity's official solution!
/// 
/// SETUP:
/// 1. Install Unity Render Streaming package (see instructions)
/// 2. Add this script to any GameObject
/// 3. Assign your Main Camera in Inspector
/// 4. Press Play and click Start Streaming!
/// 
/// This creates a peer-to-peer WebRTC stream you can view in a browser.
/// </summary>
public class SubstreamRenderStreaming : MonoBehaviour
{
    [Header("Streaming Configuration")]
    [SerializeField] private Camera streamingCamera;
    [SerializeField] private int streamWidth = 1280;
    [SerializeField] private int streamHeight = 720;
    [SerializeField] private int targetFramerate = 30;
    
    [Header("Signaling Server")]
    [SerializeField] private string signalingUrl = "ws://localhost:80";
    [SerializeField] private bool useHttps = false;
    
    // Render Streaming Components
    private RenderStreaming renderStreaming;
    private Broadcast broadcast;
    private CameraStreamer cameraStreamer;
    private RenderTexture renderTexture;
    
    // UI Elements
    private Canvas canvas;
    private GameObject panel;
    private Button streamButton;
    private Text statusText;
    private Text viewerText;
    private InputField connectionIdField;
    
    // State
    private bool isStreaming = false;
    private string connectionId = "";
    
    void Start()
    {
        // Use main camera if none assigned
        if (streamingCamera == null)
            streamingCamera = Camera.main;
        
        // Create render texture
        renderTexture = new RenderTexture(streamWidth, streamHeight, 24);
        renderTexture.Create();
        
        // Setup Render Streaming components
        SetupRenderStreaming();
        
        // Create UI
        CreateStreamingUI();
        
        Debug.Log("[Render Streaming] Ready! Using Unity's official WebRTC solution.");
    }
    
    void SetupRenderStreaming()
    {
        // Add RenderStreaming component
        renderStreaming = gameObject.AddComponent<RenderStreaming>();
        
        // Configure URL
        renderStreaming.urlSignaling = signalingUrl;
        renderStreaming.runOnAwake = false;
        
        // Add Broadcast component
        var broadcastGO = new GameObject("Broadcast");
        broadcastGO.transform.SetParent(transform);
        broadcast = broadcastGO.AddComponent<Broadcast>();
        broadcast.streamName = "camera-stream";
        
        // Add CameraStreamer
        cameraStreamer = broadcastGO.AddComponent<CameraStreamer>();
        cameraStreamer.targetCamera = streamingCamera;
        cameraStreamer.targetTexture = renderTexture;
        cameraStreamer.streamingSize = new Vector2Int(streamWidth, streamHeight);
        
        // Set camera target texture
        streamingCamera.targetTexture = renderTexture;
        
        // Configure video stream
        var videoStreamSender = cameraStreamer.gameObject.AddComponent<VideoStreamSender>();
        videoStreamSender.source = renderTexture;
        videoStreamSender.width = (uint)streamWidth;
        videoStreamSender.height = (uint)streamHeight;
        
        // Add to broadcast
        broadcast.handlers = new SignalingHandlerBase[] { cameraStreamer };
    }
    
    void CreateStreamingUI()
    {
        // Create Canvas
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        // Create EventSystem
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
        }
        
        // Create Panel
        panel = new GameObject("Render Streaming Panel");
        panel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(450, 300);
        panelRect.anchoredPosition = Vector2.zero;
        
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        
        // Title
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(panel.transform, false);
        Text title = titleGO.AddComponent<Text>();
        title.text = "UNITY RENDER STREAMING";
        title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        title.fontSize = 26;
        title.fontStyle = FontStyle.Bold;
        title.color = Color.white;
        title.alignment = TextAnchor.MiddleCenter;
        
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1);
        titleRect.anchorMax = new Vector2(0.5f, 1);
        titleRect.sizeDelta = new Vector2(400, 50);
        titleRect.anchoredPosition = new Vector2(0, -40);
        
        // Connection ID Field
        GameObject idFieldGO = new GameObject("Connection ID Field");
        idFieldGO.transform.SetParent(panel.transform, false);
        Image fieldBG = idFieldGO.AddComponent<Image>();
        fieldBG.color = new Color(0.2f, 0.2f, 0.2f);
        
        connectionIdField = idFieldGO.AddComponent<InputField>();
        
        // Field text
        GameObject fieldTextGO = new GameObject("Text");
        fieldTextGO.transform.SetParent(idFieldGO.transform, false);
        Text fieldText = fieldTextGO.AddComponent<Text>();
        fieldText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        fieldText.fontSize = 18;
        fieldText.color = Color.white;
        fieldText.alignment = TextAnchor.MiddleCenter;
        fieldText.supportRichText = false;
        
        connectionIdField.textComponent = fieldText;
        connectionIdField.text = GenerateConnectionId();
        
        RectTransform fieldRect = idFieldGO.GetComponent<RectTransform>();
        fieldRect.anchorMin = new Vector2(0.5f, 0.5f);
        fieldRect.anchorMax = new Vector2(0.5f, 0.5f);
        fieldRect.sizeDelta = new Vector2(300, 40);
        fieldRect.anchoredPosition = new Vector2(0, 30);
        
        RectTransform fieldTextRect = fieldTextGO.GetComponent<RectTransform>();
        fieldTextRect.anchorMin = Vector2.zero;
        fieldTextRect.anchorMax = Vector2.one;
        fieldTextRect.offsetMin = new Vector2(10, 0);
        fieldTextRect.offsetMax = new Vector2(-10, 0);
        
        // Stream Button
        GameObject buttonGO = new GameObject("Stream Button");
        buttonGO.transform.SetParent(panel.transform, false);
        streamButton = buttonGO.AddComponent<Button>();
        Image buttonImage = buttonGO.AddComponent<Image>();
        streamButton.targetGraphic = buttonImage;
        buttonImage.color = new Color(0.2f, 0.8f, 0.2f);
        
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(250, 50);
        buttonRect.anchoredPosition = new Vector2(0, -20);
        
        // Button Text
        GameObject buttonTextGO = new GameObject("Text");
        buttonTextGO.transform.SetParent(buttonGO.transform, false);
        Text buttonText = buttonTextGO.AddComponent<Text>();
        buttonText.text = "🎮 START STREAMING";
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 18;
        buttonText.fontStyle = FontStyle.Bold;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform buttonTextRect = buttonTextGO.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.sizeDelta = Vector2.zero;
        
        // Status Text
        GameObject statusGO = new GameObject("Status");
        statusGO.transform.SetParent(panel.transform, false);
        statusText = statusGO.AddComponent<Text>();
        statusText.text = "Ready to stream";
        statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        statusText.fontSize = 14;
        statusText.color = Color.white;
        statusText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform statusRect = statusGO.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0.5f, 0.5f);
        statusRect.anchorMax = new Vector2(0.5f, 0.5f);
        statusRect.sizeDelta = new Vector2(400, 30);
        statusRect.anchoredPosition = new Vector2(0, -70);
        
        // Viewer Instructions
        GameObject viewerGO = new GameObject("Viewer");
        viewerGO.transform.SetParent(panel.transform, false);
        viewerText = viewerGO.AddComponent<Text>();
        viewerText.text = "Connection ID above is your room code";
        viewerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        viewerText.fontSize = 12;
        viewerText.color = new Color(0.7f, 0.7f, 0.7f);
        viewerText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform viewerRect = viewerGO.GetComponent<RectTransform>();
        viewerRect.anchorMin = new Vector2(0.5f, 0);
        viewerRect.anchorMax = new Vector2(0.5f, 0);
        viewerRect.sizeDelta = new Vector2(400, 60);
        viewerRect.anchoredPosition = new Vector2(0, 40);
        
        // Connect button
        streamButton.onClick.AddListener(() =>
        {
            if (!isStreaming)
                StartStreaming();
            else
                StopStreaming();
        });
    }
    
    string GenerateConnectionId()
    {
        return "unity-" + UnityEngine.Random.Range(1000, 9999);
    }
    
    void StartStreaming()
    {
        isStreaming = true;
        connectionId = connectionIdField.text;
        
        // Update UI
        streamButton.GetComponentInChildren<Text>().text = "⏹️ STOP STREAMING";
        streamButton.GetComponent<Image>().color = new Color(0.8f, 0.2f, 0.2f);
        statusText.text = "🔴 LIVE - Streaming via WebRTC";
        statusText.color = Color.red;
        
        // Set connection ID and start
        broadcast.connectionId = connectionId;
        renderStreaming.Run(handlers: broadcast.handlers);
        
        // Show viewer instructions
        viewerText.text = $"TO VIEW: Open viewer.html with Connection ID: {connectionId}\n" +
                         $"Or use: {signalingUrl}/viewer?id={connectionId}";
        
        Debug.Log("╔═══════════════════════════════════════════════════╗");
        Debug.Log("║        🔴 RENDER STREAMING STARTED!               ║");
        Debug.Log("╠═══════════════════════════════════════════════════╣");
        Debug.Log($"║ Connection ID: {connectionId}                     ║");
        Debug.Log("║                                                   ║");
        Debug.Log("║ TO VIEW:                                          ║");
        Debug.Log("║ 1. Open the WebApp sample from package            ║");
        Debug.Log($"║ 2. Enter Connection ID: {connectionId}            ║");
        Debug.Log("║ 3. Click Connect                                  ║");
        Debug.Log("║                                                   ║");
        Debug.Log("║ Stream Settings:                                  ║");
        Debug.Log($"║ - Resolution: {streamWidth}x{streamHeight}        ║");
        Debug.Log($"║ - Framerate: {targetFramerate} FPS                ║");
        Debug.Log($"║ - Signaling: {signalingUrl}                       ║");
        Debug.Log("╚═══════════════════════════════════════════════════╝");
    }
    
    void StopStreaming()
    {
        isStreaming = false;
        
        // Update UI
        streamButton.GetComponentInChildren<Text>().text = "🎮 START STREAMING";
        streamButton.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f);
        statusText.text = "Ready to stream";
        statusText.color = Color.white;
        viewerText.text = "Connection ID above is your room code";
        
        // Stop streaming
        renderStreaming.Stop();
        
        Debug.Log("[Render Streaming] Stopped");
    }
    
    void OnDestroy()
    {
        if (isStreaming)
        {
            StopStreaming();
        }
        
        if (renderTexture != null)
        {
            renderTexture.Release();
        }
        
        // Reset camera
        if (streamingCamera != null)
        {
            streamingCamera.targetTexture = null;
        }
    }
    
    void Update()
    {
        // Keyboard shortcuts
        bool sPressed = false;
        bool xPressed = false;
        
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Keyboard.current != null)
        {
            sPressed = Keyboard.current[Key.S].wasPressedThisFrame;
            xPressed = Keyboard.current[Key.X].wasPressedThisFrame;
        }
#else
        sPressed = Input.GetKeyDown(KeyCode.S);
        xPressed = Input.GetKeyDown(KeyCode.X);
#endif
        
        if (sPressed && !isStreaming)
            StartStreaming();
        else if (xPressed && isStreaming)
            StopStreaming();
    }
}
