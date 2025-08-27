using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveKit;
using Unity.WebRTC;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// SUBSTREAM LIVEKIT REAL - Actually streams with LiveKit Unity SDK!
/// 
/// REQUIREMENTS:
/// 1. Import LiveKit Unity SDK package
/// 2. Add this script to any GameObject
/// 3. Assign a Camera in the Inspector (or it will use Camera.main)
/// 4. Press Play and click Start Streaming!
/// </summary>
public class SubstreamLiveKitReal : MonoBehaviour
{
    // LiveKit Configuration
    [Header("LiveKit Settings")]
    [SerializeField] private string livekitUrl = "wss://substream-cnzdthyx.livekit.cloud";
    [SerializeField] private string apiKey = "APIbtpHuQYmSvTT";
    [SerializeField] private string apiSecret = "RmbpdbNBZVkbAW8yW0K3ekrjwcdqIKNo6OQDwt0211Y";
    
    [Header("Streaming Settings")]
    [SerializeField] private Camera streamingCamera;
    [SerializeField] private int videoWidth = 1280;
    [SerializeField] private int videoHeight = 720;
    [SerializeField] private int frameRate = 30;
    
    // LiveKit Components
    private Room room;
    private LocalVideoTrack videoTrack;
    private RenderTexture renderTexture;
    
    // UI Elements
    private Canvas canvas;
    private GameObject panel;
    private Button streamButton;
    private Text statusText;
    private Text viewerText;
    private Button viewerButton;
    
    // State
    private bool isStreaming = false;
    private string roomName = "";
    private string participantName = "unity-streamer";
    
    void Start()
    {
        // Use main camera if none assigned
        if (streamingCamera == null)
            streamingCamera = Camera.main;
            
        // Create render texture for capturing
        renderTexture = new RenderTexture(videoWidth, videoHeight, 24);
        
        // Initialize WebRTC
        StartCoroutine(InitializeWebRTC());
        
        CreateStreamingUI();
    }
    
    IEnumerator InitializeWebRTC()
    {
        // Initialize WebRTC
        WebRTC.Initialize();
        yield return null;
        
        Debug.Log("[LiveKit] WebRTC initialized");
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
        panel = new GameObject("LiveKit Panel");
        panel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(400, 250);
        panelRect.anchoredPosition = Vector2.zero;
        
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        
        // Title
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(panel.transform, false);
        Text title = titleGO.AddComponent<Text>();
        title.text = "LIVEKIT STREAMING";
        title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        title.fontSize = 28;
        title.fontStyle = FontStyle.Bold;
        title.color = Color.white;
        title.alignment = TextAnchor.MiddleCenter;
        
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1);
        titleRect.anchorMax = new Vector2(0.5f, 1);
        titleRect.sizeDelta = new Vector2(300, 50);
        titleRect.anchoredPosition = new Vector2(0, -40);
        
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
        buttonRect.sizeDelta = new Vector2(250, 60);
        buttonRect.anchoredPosition = new Vector2(0, 10);
        
        // Button Text
        GameObject buttonTextGO = new GameObject("Text");
        buttonTextGO.transform.SetParent(buttonGO.transform, false);
        Text buttonText = buttonTextGO.AddComponent<Text>();
        buttonText.text = "🎮 START STREAMING";
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 20;
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
        statusText.text = "Ready to stream with LiveKit";
        statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        statusText.fontSize = 16;
        statusText.color = Color.white;
        statusText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform statusRect = statusGO.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0.5f, 0.5f);
        statusRect.anchorMax = new Vector2(0.5f, 0.5f);
        statusRect.sizeDelta = new Vector2(350, 30);
        statusRect.anchoredPosition = new Vector2(0, -40);
        
        // Viewer Text
        GameObject viewerGO = new GameObject("Viewer");
        viewerGO.transform.SetParent(panel.transform, false);
        viewerText = viewerGO.AddComponent<Text>();
        viewerText.text = "";
        viewerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        viewerText.fontSize = 14;
        viewerText.color = Color.cyan;
        viewerText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform viewerRect = viewerGO.GetComponent<RectTransform>();
        viewerRect.anchorMin = new Vector2(0.5f, 0);
        viewerRect.anchorMax = new Vector2(0.5f, 0);
        viewerRect.sizeDelta = new Vector2(380, 60);
        viewerRect.anchoredPosition = new Vector2(0, 40);
        
        // Connect button
        streamButton.onClick.AddListener(() => {
            if (!isStreaming)
                StartCoroutine(StartStreaming());
            else
                StartCoroutine(StopStreaming());
        });
    }
    
    IEnumerator StartStreaming()
    {
        isStreaming = true;
        roomName = "unity-" + Guid.NewGuid().ToString().Substring(0, 8);
        
        // Update UI
        streamButton.GetComponentInChildren<Text>().text = "⏹️ STOP STREAMING";
        streamButton.GetComponent<Image>().color = new Color(0.8f, 0.2f, 0.2f);
        statusText.text = "Connecting to LiveKit...";
        
        // Create access token
        string token = CreateToken(roomName, participantName);
        
        // Create and connect to room
        room = new Room();
        
        var connectOptions = new RoomOptions
        {
            adaptiveStream = true,
            dynacast = true
        };
        
        try
        {
            // Connect to LiveKit
            yield return room.Connect(livekitUrl, token, connectOptions);
            
            statusText.text = "🔴 LIVE - Creating video track...";
            
            // Create video source from camera
            var videoSource = new CameraVideoSource(streamingCamera, renderTexture, videoWidth, videoHeight, frameRate);
            
            // Create video track
            var videoTrackOptions = new LocalVideoTrackOptions
            {
                source = videoSource
            };
            
            videoTrack = LocalVideoTrack.CreateTrack("camera", videoTrackOptions);
            
            // Publish video track
            var publishOptions = new TrackPublishOptions
            {
                videoCodec = VideoCodec.H264,
                videoEncoding = new VideoEncoding
                {
                    maxBitrate = 2000000, // 2 Mbps
                    maxFramerate = frameRate
                }
            };
            
            yield return room.LocalParticipant.PublishTrack(videoTrack, publishOptions);
            
            statusText.text = "🔴 LIVE - Streaming!";
            statusText.color = Color.red;
            
            // Show viewer instructions
            string viewerUrl = "https://meet.livekit.io";
            viewerText.text = $"TO VIEW:\n1. Go to {viewerUrl}\n2. Server: {livekitUrl}\n3. Room: {roomName}";
            
            // Make clickable
            var viewerButton = viewerText.gameObject.AddComponent<Button>();
            viewerButton.onClick.AddListener(() => Application.OpenURL(viewerUrl));
            
            Debug.Log($"[LiveKit] Streaming started! Room: {roomName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[LiveKit] Failed to start streaming: {e.Message}");
            statusText.text = "Failed to connect!";
            statusText.color = Color.red;
            isStreaming = false;
            
            // Reset button
            streamButton.GetComponentInChildren<Text>().text = "🎮 START STREAMING";
            streamButton.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f);
        }
    }
    
    IEnumerator StopStreaming()
    {
        isStreaming = false;
        
        // Update UI
        streamButton.GetComponentInChildren<Text>().text = "🎮 START STREAMING";
        streamButton.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f);
        statusText.text = "Stopping stream...";
        statusText.color = Color.white;
        viewerText.text = "";
        
        // Unpublish and dispose track
        if (videoTrack != null)
        {
            yield return room.LocalParticipant.UnpublishTrack(videoTrack.Sid);
            videoTrack.Dispose();
            videoTrack = null;
        }
        
        // Disconnect from room
        if (room != null)
        {
            yield return room.Disconnect();
            room = null;
        }
        
        statusText.text = "Ready to stream with LiveKit";
        
        Debug.Log("[LiveKit] Streaming stopped");
    }
    
    string CreateToken(string room, string identity)
    {
        // For demo purposes - in production this should be on your server!
        var claims = new Dictionary<string, object>
        {
            { "video", new Dictionary<string, object>
                {
                    { "room", room },
                    { "roomJoin", true },
                    { "canPublish", true },
                    { "canSubscribe", true }
                }
            },
            { "identity", identity }
        };
        
        // This is a simplified token generation - use your server in production!
        return JWT.Encode(claims, apiSecret, JWT.JwsAlgorithm.HS256);
    }
    
    void OnDestroy()
    {
        if (isStreaming)
        {
            StartCoroutine(StopStreaming());
        }
        
        if (renderTexture != null)
        {
            renderTexture.Release();
        }
        
        WebRTC.Dispose();
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
            StartCoroutine(StartStreaming());
        else if (xPressed && isStreaming)
            StartCoroutine(StopStreaming());
    }
}
