using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Text;
using System.Security.Cryptography;

/// <summary>
/// Unity WebRTC Streaming - Direct approach for Quest/Desktop
/// Uses Unity Render Streaming for actual video capture
/// </summary>
public class UnityWebRTCStreaming : MonoBehaviour
{
    [Header("LiveKit Configuration")]
    public string LiveKitUrl = "wss://substream-cnzdthyx.livekit.cloud";
    public string ApiKey = "APIbtpHuQYmSvTT";
    public string ApiSecret = "RmbpdbNBZVkbAW8yW0K3ekrjwcdqIKNo6OQDwt0211Y";
    
    [Header("Stream Settings")]
    public int Width = 1280;
    public int Height = 720;
    public int TargetFramerate = 30;
    
    // UI
    private Text statusText;
    private Button streamButton;
    private bool isStreaming = false;
    private string roomName;
    private RenderTexture streamTexture;
    
    void Start()
    {
        CreateSimpleUI();
        SetupRenderTexture();
        
        statusText.text = "Ready to stream";
        
        // For Quest: Request microphone permission
        #if UNITY_ANDROID && !UNITY_EDITOR
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Microphone))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Microphone);
        }
        #endif
    }
    
    void CreateSimpleUI()
    {
        // Canvas
        GameObject canvasGO = new GameObject("StreamCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // EventSystem
        if (!FindObjectOfType<UnityEngine.EventSystems.EventSystem>())
        {
            GameObject eventGO = new GameObject("EventSystem");
            eventGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        // Panel
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = new Vector2(20, 20);
        panelRect.offsetMax = new Vector2(-20, -20);
        
        // Status Text
        GameObject statusGO = new GameObject("Status");
        statusGO.transform.SetParent(panel.transform, false);
        statusText = statusGO.AddComponent<Text>();
        statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        statusText.fontSize = 24;
        statusText.alignment = TextAnchor.MiddleCenter;
        statusText.color = Color.white;
        
        RectTransform statusRect = statusGO.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0.5f, 0.7f);
        statusRect.anchorMax = new Vector2(0.5f, 0.7f);
        statusRect.sizeDelta = new Vector2(600, 50);
        
        // Stream Button
        GameObject buttonGO = new GameObject("StreamButton");
        buttonGO.transform.SetParent(panel.transform, false);
        streamButton = buttonGO.AddComponent<Button>();
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.8f, 0.2f);
        
        GameObject buttonTextGO = new GameObject("Text");
        buttonTextGO.transform.SetParent(buttonGO.transform, false);
        Text buttonText = buttonTextGO.AddComponent<Text>();
        buttonText.text = "START STREAMING";
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 20;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(300, 80);
        
        RectTransform buttonTextRect = buttonTextGO.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.sizeDelta = Vector2.zero;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;
        
        streamButton.onClick.AddListener(ToggleStreaming);
    }
    
    void SetupRenderTexture()
    {
        streamTexture = new RenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32);
        streamTexture.Create();
        
        // For Quest/VR: Use the main camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            // Create a duplicate camera for streaming
            GameObject streamCamGO = new GameObject("StreamCamera");
            Camera streamCam = streamCamGO.AddComponent<Camera>();
            streamCam.CopyFrom(mainCam);
            streamCam.targetTexture = streamTexture;
            streamCam.enabled = false; // We'll enable when streaming
            
            // Store reference
            streamCamGO.transform.SetParent(mainCam.transform, false);
            streamCamGO.tag = "StreamCamera";
        }
    }
    
    void ToggleStreaming()
    {
        if (!isStreaming)
            StartStreaming();
        else
            StopStreaming();
    }
    
    void StartStreaming()
    {
        isStreaming = true;
        roomName = "unity-" + DateTime.Now.ToString("HHmmss");
        
        // Update UI
        streamButton.GetComponentInChildren<Text>().text = "STOP STREAMING";
        streamButton.GetComponent<Image>().color = Color.red;
        statusText.text = "🔴 STREAMING - Room: " + roomName;
        
        // Enable stream camera
        GameObject streamCam = GameObject.FindWithTag("StreamCamera");
        if (streamCam != null)
        {
            streamCam.GetComponent<Camera>().enabled = true;
        }
        
        // Generate JWT token
        string token = GenerateToken(roomName, "unity-publisher");
        
        // Log connection info
        string viewerUrl = $"https://meet.livekit.io/custom?liveKitUrl={Uri.EscapeDataString(LiveKitUrl)}&roomName={roomName}&connect=true";
        
        Debug.Log("==== STREAMING STARTED ====");
        Debug.Log($"Room: {roomName}");
        Debug.Log($"Token: {token}");
        Debug.Log($"Viewer URL: {viewerUrl}");
        Debug.Log("==========================");
        
        // Show instruction
        StartCoroutine(ShowInstruction(
            "To complete streaming setup:\n" +
            "1. Install Unity Render Streaming package\n" +
            "2. Or use screen sharing at meet.livekit.io"
        ));
    }
    
    void StopStreaming()
    {
        isStreaming = false;
        
        // Update UI
        streamButton.GetComponentInChildren<Text>().text = "START STREAMING";
        streamButton.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f);
        statusText.text = "Streaming stopped";
        
        // Disable stream camera
        GameObject streamCam = GameObject.FindWithTag("StreamCamera");
        if (streamCam != null)
        {
            streamCam.GetComponent<Camera>().enabled = false;
        }
    }
    
    IEnumerator ShowInstruction(string message)
    {
        yield return new WaitForSeconds(2f);
        
        GameObject instructionGO = new GameObject("Instruction");
        instructionGO.transform.SetParent(statusText.transform.parent, false);
        
        Text instruction = instructionGO.AddComponent<Text>();
        instruction.text = message;
        instruction.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        instruction.fontSize = 18;
        instruction.alignment = TextAnchor.MiddleCenter;
        instruction.color = Color.yellow;
        
        RectTransform rect = instructionGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.3f);
        rect.anchorMax = new Vector2(0.5f, 0.3f);
        rect.sizeDelta = new Vector2(600, 100);
        
        yield return new WaitForSeconds(10f);
        Destroy(instructionGO);
    }
    
    string GenerateToken(string room, string identity)
    {
        var header = new { alg = "HS256", typ = "JWT" };
        var payload = new
        {
            exp = DateTimeOffset.UtcNow.AddHours(6).ToUnixTimeSeconds(),
            iss = ApiKey,
            sub = identity,
            video = new
            {
                room = room,
                roomJoin = true,
                canPublish = true,
                canSubscribe = true
            },
            identity = identity
        };
        
        string headerJson = JsonUtility.ToJson(header);
        string payloadJson = JsonUtility.ToJson(payload);
        
        string encodedHeader = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
        string encodedPayload = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));
        
        string message = encodedHeader + "." + encodedPayload;
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(ApiSecret)))
        {
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            string signature = Base64UrlEncode(hash);
            return message + "." + signature;
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
        if (streamTexture != null)
        {
            streamTexture.Release();
            Destroy(streamTexture);
        }
    }
}
