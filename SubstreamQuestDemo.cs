using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SubstreamSDK;
using System;

/// <summary>
/// SUBSTREAM QUEST DEMO - Complete VR streaming solution
/// Drop this into any Quest Unity project!
/// </summary>
public class SubstreamQuestDemo : MonoBehaviour
{
    // Configuration
    [Header("LiveKit Settings")]
    public string LiveKitUrl = "wss://substream-cnzdthyx.livekit.cloud";
    public string LiveKitApiKey = "APIbtpHuQYmSvTT";
    public string LiveKitApiSecret = "RmbpdbNBZVkbAW8yW0K3ekrjwcdqIKNo6OQDwt0211Y";
    
    [Header("Stream Quality")]
    public int Width = 1440;
    public int Height = 1440;
    public int Fps = 72;
    public int BitrateKbps = 4000;
    
    // UI Elements
    private Canvas canvas;
    private Text statusText;
    private Button streamButton;
    private GameObject vrPointer;
    
    // Streaming
    private LiveHandle liveHandle;
    private bool isStreaming = false;
    private string roomName = "";
    
    async void Start()
    {
        CreateVRUI();
        
        try
        {
            // Initialize SDK with LiveKit
            await Substream.Init(new SubstreamConfig
            {
                BaseUrl = LiveKitUrl.Replace("wss://", "https://").Replace(".livekit.cloud", ".api.livekit.cloud"),
                WhipPublishUrl = LiveKitUrl.Replace("wss://", "https://").Replace(".livekit.cloud", ".whip.livekit.cloud/w")
            });
            
            UpdateStatus("✅ Ready to stream!");
            streamButton.interactable = true;
        }
        catch (Exception e)
        {
            UpdateStatus($"❌ Init failed: {e.Message}");
            Debug.LogError($"[Substream Quest] Init failed: {e}");
        }
    }
    
    void CreateVRUI()
    {
        // Create world-space canvas for VR
        GameObject canvasGO = new GameObject("VR Stream Canvas");
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Position in front of player
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.localPosition = new Vector3(0, 1.5f, 2f);
        canvasRect.localScale = Vector3.one * 0.003f;
        canvasRect.sizeDelta = new Vector2(800, 600);
        
        // Background panel
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(canvas.transform, false);
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        
        // Title
        CreateText("🎮 SUBSTREAM VR STREAMING", new Vector2(0, 200), 48, FontStyle.Bold);
        
        // Status
        var statusGO = CreateText("Initializing...", new Vector2(0, 100), 32);
        statusText = statusGO.GetComponent<Text>();
        
        // Stream button
        streamButton = CreateButton("START STREAMING", new Vector2(0, 0), new Vector2(400, 100));
        streamButton.onClick.AddListener(ToggleStreaming);
        streamButton.interactable = false;
        
        // Instructions
        CreateText("Point controller and pull trigger to click", new Vector2(0, -150), 24);
        
        // Create EventSystem for UI
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
        }
    }
    
    void CreateVRPointer()
    {
        // VR pointer setup - implement based on your VR SDK
        // For Oculus: Use OVRRaycaster
        // For OpenXR: Use XR Interaction Toolkit
    }
    
    async void ToggleStreaming()
    {
        if (!isStreaming)
        {
            await StartStreaming();
        }
        else
        {
            await StopStreaming();
        }
    }
    
    async System.Threading.Tasks.Task StartStreaming()
    {
        try
        {
            UpdateStatus("📱 Starting stream...");
            streamButton.interactable = false;
            
            // Generate room name
            roomName = "quest-" + DateTime.Now.ToString("HHmmss");
            
            // Create live stream
            liveHandle = await Substream.LiveCreate(new LiveOptions
            {
                Width = Width,
                Height = Height,
                Fps = Fps,
                VideoBitrateKbps = BitrateKbps,
                WithAudio = true,
                MetadataJson = JsonUtility.ToJson(new
                {
                    game = Application.productName,
                    platform = "Quest",
                    room = roomName
                })
            });
            
            // Set up callbacks
            liveHandle.OnStatusChanged += OnStreamStatusChanged;
            liveHandle.OnError += OnStreamError;
            
            // Start streaming
            await liveHandle.Start();
            
            // Success!
            isStreaming = true;
            streamButton.GetComponentInChildren<Text>().text = "STOP STREAMING";
            streamButton.GetComponent<Image>().color = Color.red;
            streamButton.interactable = true;
            
            UpdateStatus($"🔴 LIVE - Room: {roomName}");
            
            // Show viewer URL
            string viewerUrl = $"https://meet.livekit.io/custom?" +
                              $"liveKitUrl={Uri.EscapeDataString(LiveKitUrl)}&" +
                              $"roomName={roomName}&connect=true";
            
            Debug.Log($"[Substream Quest] Viewer URL: {viewerUrl}");
            
            // Haptic feedback on success (implement for your VR SDK)
            // For Oculus: OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.RTouch);
            // For OpenXR: Use XR Haptics
        }
        catch (Exception e)
        {
            UpdateStatus($"❌ Failed: {e.Message}");
            streamButton.interactable = true;
            Debug.LogError($"[Substream Quest] Stream failed: {e}");
        }
    }
    
    async System.Threading.Tasks.Task StopStreaming()
    {
        try
        {
            UpdateStatus("Stopping stream...");
            streamButton.interactable = false;
            
            if (liveHandle != null)
            {
                await liveHandle.Stop();
                liveHandle.OnStatusChanged -= OnStreamStatusChanged;
                liveHandle.OnError -= OnStreamError;
                liveHandle = null;
            }
            
            isStreaming = false;
            streamButton.GetComponentInChildren<Text>().text = "START STREAMING";
            streamButton.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f);
            streamButton.interactable = true;
            
            UpdateStatus("Stream stopped");
        }
        catch (Exception e)
        {
            UpdateStatus($"❌ Stop failed: {e.Message}");
            Debug.LogError($"[Substream Quest] Stop failed: {e}");
        }
    }
    
    void OnStreamStatusChanged(StreamStatus status)
    {
        switch (status)
        {
            case StreamStatus.RequestingPermission:
                UpdateStatus("📱 Approve screen capture in headset...");
                break;
            case StreamStatus.PermissionGranted:
                UpdateStatus("✅ Permission granted!");
                break;
            case StreamStatus.Streaming:
                UpdateStatus($"🔴 LIVE - {Width}x{Height}@{Fps}fps");
                break;
        }
    }
    
    void OnStreamError(string error)
    {
        UpdateStatus($"❌ Error: {error}");
    }
    
    void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        Debug.Log($"[Substream Quest] {message}");
    }
    
    GameObject CreateText(string content, Vector2 position, int fontSize, FontStyle style = FontStyle.Normal)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(canvas.transform, false);
        
        Text text = go.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(700, 100);
        rect.anchoredPosition = position;
        
        return go;
    }
    
    Button CreateButton(string label, Vector2 position, Vector2 size)
    {
        GameObject go = new GameObject("Button");
        go.transform.SetParent(canvas.transform, false);
        
        Button btn = go.AddComponent<Button>();
        Image img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.8f, 0.2f);
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
        text.fontSize = 36;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        
        return btn;
    }
    
    void Update()
    {
        // Keyboard shortcut for desktop testing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleStreaming();
        }
        
        // VR controller input - implement for your SDK
        // For Oculus: OVRInput.GetDown(OVRInput.Button.One)
        // For OpenXR: Use Input System
    }
}
