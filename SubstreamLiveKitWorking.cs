using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LiveKit;
using Unity.WebRTC;

public class SubstreamLiveKitWorking : MonoBehaviour
{
    // LiveKit Settings
    private const string LIVEKIT_URL = "wss://substream-cnzdthyx.livekit.cloud";
    private const string LIVEKIT_API_KEY = "APIbtpHuQYmSvTT";
    private const string LIVEKIT_API_SECRET = "RmbpdbNBZVkbAW8yW0K3ekrjwcdqIKNo6OQDwt0211Y";
    
    // Streaming Settings
    [SerializeField] private Camera streamingCamera;
    [SerializeField] private int videoWidth = 1280;
    [SerializeField] private int videoHeight = 720;
    
    // LiveKit Objects
    private Room room;
    private RenderTexture renderTexture;
    private Texture2D frameTexture;
    
    // UI
    private Canvas canvas;
    private GameObject panel;
    private Button streamButton;
    private Text statusText;
    private Text viewerText;
    
    // State
    private bool isStreaming = false;
    private string roomName = "";
    
    void Start()
    {
        // Use main camera if not set
        if (streamingCamera == null)
            streamingCamera = Camera.main;
            
        // Create render texture for camera capture
        renderTexture = new RenderTexture(videoWidth, videoHeight, 24);
        frameTexture = new Texture2D(videoWidth, videoHeight, TextureFormat.RGBA32, false);
        
        // Assign render texture to camera
        streamingCamera.targetTexture = renderTexture;
        
        CreateUI();
        Debug.Log("[LiveKit] Ready to stream with real video!");
    }
    
    void CreateUI()
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
        title.fontSize = 24;
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
        
        GameObject buttonTextGO = new GameObject("Text");
        buttonTextGO.transform.SetParent(buttonGO.transform, false);
        Text buttonText = buttonTextGO.AddComponent<Text>();
        buttonText.text = "START STREAMING";
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
        statusText.text = "Ready";
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
        streamButton.onClick.AddListener(() => StartCoroutine(ToggleStreaming()));
    }
    
    IEnumerator ToggleStreaming()
    {
        if (!isStreaming)
            yield return StartCoroutine(StartStreaming());
        else
            yield return StartCoroutine(StopStreaming());
    }
    
    IEnumerator StartStreaming()
    {
        isStreaming = true;
        roomName = "unity-" + Guid.NewGuid().ToString().Substring(0, 8);
        
        // Update UI
        streamButton.GetComponentInChildren<Text>().text = "STOP STREAMING";
        streamButton.GetComponent<Image>().color = new Color(0.8f, 0.2f, 0.2f);
        statusText.text = "Connecting...";
        
        // Create room
        room = new Room();
        
        // Generate token
        string token = GenerateToken(roomName, "unity-streamer");
        
        // Connect options
        var connectOptions = new RoomOptions
        {
            adaptiveStream = true,
            dynacast = true
        };
        
        // Connect to room
        yield return room.Connect(LIVEKIT_URL, token, connectOptions);
        
        statusText.text = "Connected! Starting video...";
        
        // Create video source from render texture
        var videoSource = new TextureVideoSource(renderTexture);
        
        // Create local video track
        var trackOptions = new LocalVideoTrackOptions
        {
            source = videoSource
        };
        
        var videoTrack = LocalVideoTrack.CreateTrack("camera", trackOptions);
        
        // Publish video
        var publishOptions = new TrackPublishOptions
        {
            videoCodec = VideoCodec.H264,
            videoEncoding = new VideoEncoding
            {
                maxBitrate = 2_000_000,
                maxFramerate = 30
            }
        };
        
        yield return room.LocalParticipant.PublishTrack(videoTrack, publishOptions);
        
        statusText.text = "🔴 LIVE - Streaming video!";
        statusText.color = Color.red;
        
        // Create viewer link
        string viewerUrl = $"https://meet.livekit.io/custom?liveKitUrl={Uri.EscapeDataString(LIVEKIT_URL)}&roomName={roomName}&participantName=viewer&connect=true";
        viewerText.text = "📺 CLICK TO VIEW STREAM";
        
        var viewerButton = viewerText.gameObject.AddComponent<Button>();
        viewerButton.onClick.AddListener(() => Application.OpenURL(viewerUrl));
        
        Debug.Log($"[LiveKit] Streaming video to room: {roomName}");
    }
    
    IEnumerator StopStreaming()
    {
        isStreaming = false;
        
        // Update UI
        streamButton.GetComponentInChildren<Text>().text = "START STREAMING";
        streamButton.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f);
        statusText.text = "Stopping...";
        statusText.color = Color.white;
        viewerText.text = "";
        
        // Disconnect
        if (room != null)
        {
            yield return room.Disconnect();
            room = null;
        }
        
        statusText.text = "Ready";
        
        var viewerButton = viewerText.GetComponent<Button>();
        if (viewerButton != null)
            Destroy(viewerButton);
    }
    
    string GenerateToken(string room, string identity)
    {
        var header = new Dictionary<string, object>
        {
            { "alg", "HS256" },
            { "typ", "JWT" }
        };
        
        var payload = new Dictionary<string, object>
        {
            { "exp", DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds() },
            { "iss", LIVEKIT_API_KEY },
            { "sub", identity },
            { "video", new Dictionary<string, object>
                {
                    { "room", room },
                    { "roomJoin", true },
                    { "canPublish", true },
                    { "canSubscribe", true }
                }
            },
            { "identity", identity },
            { "name", identity }
        };
        
        // Use LiveKit's JWT implementation or a proper library
        // This is simplified - in production use proper JWT signing
        string headerJson = JsonUtility.ToJson(header);
        string payloadJson = JsonUtility.ToJson(payload);
        
        string encodedHeader = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
        string encodedPayload = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));
        
        string message = encodedHeader + "." + encodedPayload;
        var key = Encoding.UTF8.GetBytes(LIVEKIT_API_SECRET);
        
        using (var hmac = new System.Security.Cryptography.HMACSHA256(key))
        {
            var signature = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            string encodedSignature = Base64UrlEncode(signature);
            return message + "." + encodedSignature;
        }
    }
    
    string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");
    }
    
    void OnDestroy()
    {
        if (room != null)
        {
            StartCoroutine(StopStreaming());
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
}

// Custom video source for render texture
public class TextureVideoSource : VideoStreamSource
{
    private RenderTexture source;
    private Texture2D temp;
    
    public TextureVideoSource(RenderTexture renderTexture) : base()
    {
        source = renderTexture;
        temp = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
    }
    
    protected override void Update()
    {
        if (source == null) return;
        
        // Capture render texture
        RenderTexture.active = source;
        temp.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
        temp.Apply();
        RenderTexture.active = null;
        
        // Send frame
        var nativeArray = temp.GetRawTextureData<byte>();
        using (var buffer = new NativeArray<byte>(nativeArray, Allocator.Temp))
        {
            var frame = new VideoFrame
            {
                width = (uint)source.width,
                height = (uint)source.height,
                format = VideoFormat.RGBA,
                data = buffer
            };
            
            SendFrame(frame);
        }
    }
}
