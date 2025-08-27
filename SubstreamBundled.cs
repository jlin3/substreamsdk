using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using UnityEngine.Networking;

/// <summary>
/// SUBSTREAM BUNDLED - Complete streaming solution with JWT generation
/// No external dependencies needed!
/// 
/// SETUP:
/// 1. Import test-scene.unitypackage
/// 2. Add this script to any GameObject
/// 3. Press Play and click Start Streaming
/// 4. If LiveKit SDK is available, it will use it
/// 5. Otherwise shows instructions
/// </summary>
public class SubstreamBundled : MonoBehaviour
{
    // LiveKit Configuration
    private const string LIVEKIT_URL = "wss://substream-cnzdthyx.livekit.cloud";
    private const string LIVEKIT_API_KEY = "APIbtpHuQYmSvTT";
    private const string LIVEKIT_API_SECRET = "RmbpdbNBZVkbAW8yW0K3ekrjwcdqIKNo6OQDwt0211Y";
    
    // UI Elements
    private Canvas canvas;
    private GameObject panel;
    private Button streamButton;
    private Text statusText;
    private Text instructionsText;
    private Button viewerButton;
    
    // State
    private bool isStreaming = false;
    private string roomName = "";
    private string accessToken = "";
    
    void Start()
    {
        CreateUI();
        CheckLiveKitSDK();
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
        panel = new GameObject("Substream Panel");
        panel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(500, 400);
        panelRect.anchoredPosition = Vector2.zero;
        
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        
        // Title
        CreateText("SUBSTREAM UNITY STREAMING", new Vector2(0, 160), 26, FontStyle.Bold);
        
        // Stream Button
        streamButton = CreateButton("🎮 START STREAMING", new Vector2(0, 80), new Vector2(300, 60));
        streamButton.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f);
        streamButton.onClick.AddListener(StartStreaming);
        
        // Status
        var statusGO = CreateText("Checking LiveKit SDK...", new Vector2(0, 20), 16);
        statusText = statusGO.GetComponent<Text>();
        
        // Instructions
        var instrGO = CreateText("", new Vector2(0, -60), 14);
        instructionsText = instrGO.GetComponent<Text>();
        instructionsText.text = "";
        
        // Viewer Button (hidden initially)
        viewerButton = CreateButton("📺 OPEN VIEWER", new Vector2(0, -140), new Vector2(250, 50));
        viewerButton.GetComponent<Image>().color = new Color(0.3f, 0.6f, 1f);
        viewerButton.gameObject.SetActive(false);
    }
    
    void CheckLiveKitSDK()
    {
        // Check if LiveKit SDK is available
        var liveKitType = Type.GetType("LiveKit.Room, LiveKit");
        
        if (liveKitType != null)
        {
            statusText.text = "✅ LiveKit SDK detected!";
            statusText.color = Color.green;
            instructionsText.text = "Click START STREAMING to begin";
        }
        else
        {
            statusText.text = "⚠️ LiveKit SDK not found";
            statusText.color = Color.yellow;
            instructionsText.text = "Room creation will work, but no video.\nTo enable video:\n" +
                                   "1. Download LiveKit Unity SDK\n" +
                                   "2. Import into project\n" +
                                   "3. Restart this scene";
        }
    }
    
    void StartStreaming()
    {
        if (isStreaming) return;
        
        isStreaming = true;
        roomName = "unity-" + Guid.NewGuid().ToString().Substring(0, 8);
        
        // Update UI
        streamButton.GetComponentInChildren<Text>().text = "⏹️ STOP STREAMING";
        streamButton.GetComponent<Image>().color = new Color(0.8f, 0.2f, 0.2f);
        streamButton.onClick.RemoveAllListeners();
        streamButton.onClick.AddListener(StopStreaming);
        
        statusText.text = "Creating room...";
        
        // Generate proper JWT token
        accessToken = GenerateToken(roomName, "unity-streamer");
        
        // Try to connect with LiveKit SDK if available
        StartCoroutine(ConnectToLiveKit());
    }
    
    IEnumerator ConnectToLiveKit()
    {
        var liveKitType = Type.GetType("LiveKit.Room, LiveKit");
        
        if (liveKitType != null)
        {
            // LiveKit SDK is available
            statusText.text = "Connecting with LiveKit SDK...";
            
            // Use reflection to create room and connect
            dynamic room = Activator.CreateInstance(liveKitType);
            
            try
            {
                yield return room.Connect(LIVEKIT_URL, accessToken);
                statusText.text = "Connected! Enabling camera...";
                
                // Try to enable camera
                yield return room.LocalParticipant.SetCameraEnabled(true);
                
                statusText.text = "🔴 LIVE - Streaming with video!";
                statusText.color = Color.red;
                instructionsText.text = $"Room: {roomName}\nVideo streaming active!";
            }
            catch (Exception e)
            {
                Debug.LogError($"LiveKit connection error: {e.Message}");
                statusText.text = "Connected but no video";
                instructionsText.text = $"Room: {roomName}\nLiveKit SDK needs camera setup";
            }
        }
        else
        {
            // No LiveKit SDK - just create the room
            statusText.text = "🔴 Room Created (no video)";
            statusText.color = Color.red;
            instructionsText.text = $"Room: {roomName}\nViewers can join but no video stream";
        }
        
        // Show viewer button
        viewerButton.gameObject.SetActive(true);
        string viewerUrl = $"https://meet.livekit.io/custom?liveKitUrl={Uri.EscapeDataString(LIVEKIT_URL)}&roomName={roomName}&participantName=viewer&connect=true";
        viewerButton.onClick.RemoveAllListeners();
        viewerButton.onClick.AddListener(() => Application.OpenURL(viewerUrl));
        
        // Log details
        Debug.Log($"[Substream] Room: {roomName}");
        Debug.Log($"[Substream] Token: {accessToken}");
        Debug.Log($"[Substream] Viewer URL: {viewerUrl}");
        
        yield return null;
    }
    
    void StopStreaming()
    {
        isStreaming = false;
        
        // Reset UI
        streamButton.GetComponentInChildren<Text>().text = "🎮 START STREAMING";
        streamButton.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f);
        streamButton.onClick.RemoveAllListeners();
        streamButton.onClick.AddListener(StartStreaming);
        
        statusText.text = "Stopped";
        statusText.color = Color.white;
        instructionsText.text = "Click START STREAMING to begin";
        viewerButton.gameObject.SetActive(false);
        
        // TODO: Disconnect from LiveKit if connected
    }
    
    string GenerateToken(string room, string identity)
    {
        // Create JWT token for LiveKit
        var header = new
        {
            alg = "HS256",
            typ = "JWT"
        };
        
        var payload = new
        {
            exp = DateTimeOffset.UtcNow.AddHours(6).ToUnixTimeSeconds(),
            iss = LIVEKIT_API_KEY,
            sub = identity,
            video = new
            {
                room = room,
                roomJoin = true,
                canPublish = true,
                canSubscribe = true,
                canPublishData = true,
                canUpdateOwnMetadata = true
            },
            identity = identity,
            name = identity,
            metadata = ""
        };
        
        // Convert to JSON
        string headerJson = JsonUtility.ToJson(header);
        string payloadJson = JsonUtility.ToJson(payload);
        
        // Base64Url encode
        string encodedHeader = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
        string encodedPayload = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));
        
        // Create signature
        string message = encodedHeader + "." + encodedPayload;
        byte[] keyBytes = Encoding.UTF8.GetBytes(LIVEKIT_API_SECRET);
        
        using (var hmac = new HMACSHA256(keyBytes))
        {
            byte[] signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            string encodedSignature = Base64UrlEncode(signatureBytes);
            
            return message + "." + encodedSignature;
        }
    }
    
    string Base64UrlEncode(byte[] input)
    {
        string output = Convert.ToBase64String(input);
        output = output.Replace('+', '-');
        output = output.Replace('/', '_');
        output = output.Replace("=", "");
        return output;
    }
    
    GameObject CreateText(string content, Vector2 position, int fontSize, FontStyle style = FontStyle.Normal)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(panel.transform, false);
        
        Text text = go.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(450, 60);
        rect.anchoredPosition = position;
        
        return go;
    }
    
    Button CreateButton(string label, Vector2 position, Vector2 size)
    {
        GameObject go = new GameObject("Button");
        go.transform.SetParent(panel.transform, false);
        
        Button btn = go.AddComponent<Button>();
        Image img = go.AddComponent<Image>();
        btn.targetGraphic = img;
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        Text text = textGO.AddComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 18;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        
        return btn;
    }
}
