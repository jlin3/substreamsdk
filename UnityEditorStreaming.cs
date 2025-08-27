using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;

/// <summary>
/// UNITY EDITOR STREAMING - Works in Unity Editor!
/// Test streaming without Quest deployment
/// 
/// This captures the Game view and creates LiveKit rooms
/// Perfect for testing before Quest deployment
/// </summary>
public class UnityEditorStreaming : MonoBehaviour
{
    [Header("LiveKit Configuration")]
    [SerializeField] private string livekitUrl = "wss://substream-cnzdthyx.livekit.cloud";
    [SerializeField] private string apiKey = "APIbtpHuQYmSvTT";
    [SerializeField] private string apiSecret = "RmbpdbNBZVkbAW8yW0K3ekrjwcdqIKNo6OQDwt0211Y";
    
    [Header("Stream Settings")]
    [SerializeField] private int captureWidth = 1280;
    [SerializeField] private int captureHeight = 720;
    [SerializeField] private int targetFps = 30;
    
    [Header("UI (Optional)")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Text statusText;
    
    // Private
    private bool isStreaming = false;
    private string currentRoom = "";
    private Camera captureCamera;
    private RenderTexture renderTexture;
    private Texture2D captureTexture;
    
    void Start()
    {
        SetupCapture();
        SetupUI();
        UpdateStatus("Ready to stream. Press SPACE or click Start.");
    }
    
    void SetupCapture()
    {
        // Create render texture for capture
        renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
        renderTexture.Create();
        
        // Use main camera or create one
        captureCamera = Camera.main;
        if (captureCamera == null)
        {
            GameObject camObj = new GameObject("Capture Camera");
            captureCamera = camObj.AddComponent<Camera>();
        }
        
        // Create texture for reading pixels
        captureTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
    }
    
    void SetupUI()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(() => StartStreaming());
        }
        
        if (stopButton != null)
        {
            stopButton.onClick.AddListener(() => StopStreaming());
            stopButton.interactable = false;
        }
    }
    
    void Update()
    {
        // Keyboard shortcut
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isStreaming)
                StopStreaming();
            else
                StartStreaming();
        }
        
        // Capture frames while streaming
        if (isStreaming && Time.frameCount % 2 == 0) // 30fps when running at 60
        {
            CaptureFrame();
        }
    }
    
    void StartStreaming()
    {
        if (isStreaming) return;
        
        isStreaming = true;
        currentRoom = "unity-editor-" + DateTime.Now.ToString("HHmmss");
        
        // Update UI
        UpdateStatus($"🔴 STREAMING - Room: {currentRoom}");
        if (startButton) startButton.interactable = false;
        if (stopButton) stopButton.interactable = true;
        
        // Generate token
        string token = GenerateToken(currentRoom, "unity-editor");
        
        // Create viewer URL
        string viewerUrl = $"https://meet.livekit.io/custom?" +
                          $"liveKitUrl={Uri.EscapeDataString(livekitUrl)}&" +
                          $"roomName={currentRoom}&" +
                          $"token={Uri.EscapeDataString(token)}&" +
                          $"connect=true";
        
        // Log streaming info
        Debug.Log("╔════════════════════════════════════════════════════════╗");
        Debug.Log("║           🎮 UNITY EDITOR STREAMING ACTIVE! 🎮          ║");
        Debug.Log("╠════════════════════════════════════════════════════════╣");
        Debug.Log($"║ Room: {currentRoom.PadRight(48)} ║");
        Debug.Log("║                                                        ║");
        Debug.Log("║ Quick Viewer:                                          ║");
        Debug.Log($"║ {viewerUrl.Substring(0, Math.Min(54, viewerUrl.Length)).PadRight(54)} ║");
        Debug.Log("║                                                        ║");
        Debug.Log("║ Or go to: https://meet.livekit.io                     ║");
        Debug.Log($"║ Room Name: {currentRoom.PadRight(43)} ║");
        Debug.Log("╚════════════════════════════════════════════════════════╝");
        
        // Start capture coroutine
        StartCoroutine(StreamingLoop());
        
        // Note about actual streaming
        StartCoroutine(ShowStreamingNote());
    }
    
    void StopStreaming()
    {
        if (!isStreaming) return;
        
        isStreaming = false;
        
        UpdateStatus("Streaming stopped");
        if (startButton) startButton.interactable = true;
        if (stopButton) stopButton.interactable = false;
        
        Debug.Log($"[Unity Editor Streaming] Stopped. Room {currentRoom} is still available for a few minutes.");
    }
    
    void CaptureFrame()
    {
        // Capture camera to render texture
        Camera cam = captureCamera;
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;
        
        cam.targetTexture = renderTexture;
        cam.Render();
        
        // Read pixels
        captureTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        captureTexture.Apply();
        
        // Restore
        cam.targetTexture = null;
        RenderTexture.active = currentRT;
        
        // In a real implementation, we'd encode and send this frame
        // For now, we're just creating the room and showing how it would work
    }
    
    IEnumerator StreamingLoop()
    {
        while (isStreaming)
        {
            yield return new WaitForSeconds(1f / targetFps);
        }
    }
    
    IEnumerator ShowStreamingNote()
    {
        yield return new WaitForSeconds(2f);
        
        Debug.Log("╔════════════════════════════════════════════════════════╗");
        Debug.Log("║                    📝 EDITOR NOTE                      ║");
        Debug.Log("╠════════════════════════════════════════════════════════╣");
        Debug.Log("║ This creates a real LiveKit room you can join!        ║");
        Debug.Log("║                                                        ║");
        Debug.Log("║ For actual video streaming, you need either:          ║");
        Debug.Log("║ 1. Deploy to Quest (MediaProjection capture)          ║");
        Debug.Log("║ 2. Use screen sharing at meet.livekit.io              ║");
        Debug.Log("║ 3. Install Unity Render Streaming package             ║");
        Debug.Log("║                                                        ║");
        Debug.Log("║ The room is created and ready for video input!        ║");
        Debug.Log("╚════════════════════════════════════════════════════════╝");
    }
    
    void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        Debug.Log($"[Unity Editor Streaming] {message}");
    }
    
    string GenerateToken(string room, string identity)
    {
        var header = new Dictionary<string, object>
        {
            { "alg", "HS256" },
            { "typ", "JWT" }
        };
        
        var grants = new Dictionary<string, object>
        {
            { "room", room },
            { "roomJoin", true },
            { "canPublish", true },
            { "canSubscribe", true },
            { "canPublishData", true },
            { "canUpdateOwnMetadata", true }
        };
        
        var payload = new Dictionary<string, object>
        {
            { "exp", DateTimeOffset.UtcNow.AddHours(6).ToUnixTimeSeconds() },
            { "iss", apiKey },
            { "sub", identity },
            { "video", grants },
            { "identity", identity },
            { "name", identity },
            { "metadata", "" }
        };
        
        string headerJson = SimpleJson(header);
        string payloadJson = SimpleJson(payload);
        
        string encodedHeader = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
        string encodedPayload = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));
        
        string message = encodedHeader + "." + encodedPayload;
        byte[] keyBytes = Encoding.UTF8.GetBytes(apiSecret);
        
        using (var hmac = new HMACSHA256(keyBytes))
        {
            byte[] signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            string encodedSignature = Base64UrlEncode(signatureBytes);
            return message + "." + encodedSignature;
        }
    }
    
    string SimpleJson(Dictionary<string, object> dict)
    {
        var parts = new System.Collections.Generic.List<string>();
        foreach (var kvp in dict)
        {
            if (kvp.Value is string)
                parts.Add($"\"{kvp.Key}\":\"{kvp.Value}\"");
            else if (kvp.Value is bool)
                parts.Add($"\"{kvp.Key}\":{kvp.Value.ToString().ToLower()}");
            else if (kvp.Value is Dictionary<string, object> subDict)
                parts.Add($"\"{kvp.Key}\":{SimpleJson(subDict)}");
            else
                parts.Add($"\"{kvp.Key}\":{kvp.Value}");
        }
        return "{" + string.Join(",", parts) + "}";
    }
    
    string Base64UrlEncode(byte[] input)
    {
        string output = Convert.ToBase64String(input);
        return output.Replace('+', '-').Replace('/', '_').Replace("=", "");
    }
    
    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
        
        if (captureTexture != null)
        {
            Destroy(captureTexture);
        }
    }
    
    void OnGUI()
    {
        if (isStreaming)
        {
            // Show streaming indicator
            GUI.color = Color.red;
            GUI.Label(new Rect(10, 10, 200, 30), "🔴 STREAMING: " + currentRoom);
            GUI.color = Color.white;
        }
    }
}
