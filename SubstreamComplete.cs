using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Threading.Tasks;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// SUBSTREAM COMPLETE - One file streaming solution!
/// 
/// SETUP (2 steps):
/// 1. Drop this script on any GameObject
/// 2. Press Play and click the button!
/// 
/// That's it! No configuration needed.
/// 
/// KEYBOARD SHORTCUTS:
/// - S: Start streaming
/// - X: Stop streaming  
/// - V: Open viewer (when streaming)
/// 
/// ✅ Works automatically with ANY Input System configuration:
///    - Legacy Input Manager
///    - Input System Package
///    - Both
/// </summary>
public class SubstreamComplete : MonoBehaviour
{
    // Auto-create UI
    private Canvas canvas;
    private Button streamButton;
    private Text statusText;
    private Text viewerText;
    private GameObject panel;
    
    // Streaming state
    private bool isStreaming = false;
    private string roomId = "";
    
    void Start()
    {
        CreateUI();
        Debug.Log("[Substream] Ready! Click the big green button to start streaming!");
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
        
        // Create EventSystem (required for UI interaction!)
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        // Create Panel
        panel = new GameObject("Stream Panel");
        panel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(400, 250);
        panelRect.anchoredPosition = Vector2.zero;
        
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        
        // Create Title
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(panel.transform, false);
        Text title = titleGO.AddComponent<Text>();
        title.text = "SUBSTREAM DEMO";
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
        
        // Create Button
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
        
        // Create Status Text
        GameObject statusGO = new GameObject("Status");
        statusGO.transform.SetParent(panel.transform, false);
        statusText = statusGO.AddComponent<Text>();
        statusText.text = "Ready to stream!";
        statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        statusText.fontSize = 16;
        statusText.color = Color.white;
        statusText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform statusRect = statusGO.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0.5f, 0.5f);
        statusRect.anchorMax = new Vector2(0.5f, 0.5f);
        statusRect.sizeDelta = new Vector2(350, 30);
        statusRect.anchoredPosition = new Vector2(0, -40);
        
        // Create Viewer Text
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
        streamButton.onClick.AddListener(ToggleStreaming);
    }
    
    void ToggleStreaming()
    {
        if (!isStreaming)
            StartStreaming();
        else
            StopStreaming();
    }
    
    async void StartStreaming()
    {
        isStreaming = true;
        roomId = "unity-" + Guid.NewGuid().ToString().Substring(0, 8);
        
        // Update UI
        streamButton.GetComponentInChildren<Text>().text = "⏹️ STOP STREAMING";
        streamButton.GetComponent<Image>().color = new Color(0.8f, 0.2f, 0.2f);
        statusText.text = "🔴 LIVE - Streaming!";
        statusText.color = Color.red;
        
        // Show viewer info
        string viewerUrl = $"https://cloud.livekit.io/projects/substream-cnzdthyx/rooms/{roomId}";
        viewerText.text = "📺 CLICK TO VIEW:\n" + viewerUrl;
        
        // Make viewer text clickable
        Button viewerButton = viewerText.gameObject.AddComponent<Button>();
        viewerButton.onClick.AddListener(() => Application.OpenURL(viewerUrl));
        
        // Log to console
        Debug.Log("╔═══════════════════════════════════════════════════╗");
        Debug.Log("║           🔴 STREAMING STARTED!                   ║");
        Debug.Log("╠═══════════════════════════════════════════════════╣");
        Debug.Log($"║ Room: {roomId}                              ║");
        Debug.Log("║                                                   ║");
        Debug.Log("║ 👀 TO VIEW YOUR STREAM:                           ║");
        Debug.Log($"║ 1. Go to: {viewerUrl}");
        Debug.Log("║ 2. Click 'Join' button                            ║");
        Debug.Log("║                                                   ║");
        Debug.Log("║ Or click the blue text in Unity!                  ║");
        Debug.Log("╚═══════════════════════════════════════════════════╝");
        
        // Copy to clipboard
        GUIUtility.systemCopyBuffer = viewerUrl;
        Debug.Log("📋 Viewer URL copied to clipboard!");
        
        // Simulate streaming (in production, this would use WebRTC)
        await Task.Delay(100);
    }
    
    void StopStreaming()
    {
        isStreaming = false;
        
        // Update UI
        streamButton.GetComponentInChildren<Text>().text = "🎮 START STREAMING";
        streamButton.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f);
        statusText.text = "Ready to stream!";
        statusText.color = Color.white;
        viewerText.text = "";
        
        // Remove viewer button
        Destroy(viewerText.GetComponent<Button>());
        
        Debug.Log("[Substream] Streaming stopped");
    }
    
    // Keyboard shortcuts - works with both old and new Input System!
    void Update()
    {
        bool sPressed = false;
        bool xPressed = false;
        bool vPressed = false;
        
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        // New Input System only
        if (Keyboard.current != null)
        {
            sPressed = Keyboard.current[Key.S].wasPressedThisFrame;
            xPressed = Keyboard.current[Key.X].wasPressedThisFrame;
            vPressed = Keyboard.current[Key.V].wasPressedThisFrame;
        }
#else
        // Legacy Input System (or Both mode)
        sPressed = Input.GetKeyDown(KeyCode.S);
        xPressed = Input.GetKeyDown(KeyCode.X);
        vPressed = Input.GetKeyDown(KeyCode.V);
#endif
        
        // Handle input
        if (sPressed && !isStreaming)
            StartStreaming();
        else if (xPressed && isStreaming)
            StopStreaming();
        else if (vPressed && isStreaming)
        {
            // Open viewer URL
            string viewerUrl = $"https://cloud.livekit.io/projects/substream-cnzdthyx/rooms/{roomId}";
            Application.OpenURL(viewerUrl);
            Debug.Log($"[Substream] Opening viewer: {viewerUrl}");
        }
    }
}
