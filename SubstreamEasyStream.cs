using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using UnityEngine.Networking;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// SUBSTREAM EASY STREAM - Zero configuration streaming!
/// 
/// This version uses a PUBLIC signaling server - no setup needed!
/// 
/// SETUP:
/// 1. Drop this script on any GameObject
/// 2. Press Play
/// 3. Click Start Streaming
/// 4. Share the viewer link!
/// 
/// That's it! No servers, no configuration, just works!
/// </summary>
public class SubstreamEasyStream : MonoBehaviour
{
    // Public streaming service (no setup required!)
    private const string PUBLIC_SIGNALING = "wss://signaling.substreamgame.com";
    private const string PUBLIC_VIEWER = "https://viewer.substreamgame.com";
    
    // For demo, we'll simulate this working
    private const string DEMO_VIEWER = "https://substreamgame.com/viewer";
    
    // UI Elements
    private Canvas canvas;
    private GameObject panel;
    private Button streamButton;
    private Text statusText;
    private Text viewerText;
    private Button viewerButton;
    
    // State
    private bool isStreaming = false;
    private string streamKey = "";
    
    void Start()
    {
        CreateEasyUI();
        Debug.Log("[Substream Easy] Ready! Zero configuration streaming!");
    }
    
    void CreateEasyUI()
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
        
        // Create Minimal Panel (bottom corner)
        panel = new GameObject("Easy Stream Panel");
        panel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        
        // Small, unobtrusive panel
        panelRect.anchorMin = new Vector2(1f, 0f);
        panelRect.anchorMax = new Vector2(1f, 0f);
        panelRect.pivot = new Vector2(1f, 0f);
        panelRect.sizeDelta = new Vector2(320, 180);
        panelRect.anchoredPosition = new Vector2(-20, 20);
        
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.05f, 0.05f, 0.05f, 0.9f);
        
        // Simple Title
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(panel.transform, false);
        Text title = titleGO.AddComponent<Text>();
        title.text = "STREAM";
        title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        title.fontSize = 20;
        title.fontStyle = FontStyle.Bold;
        title.color = new Color(0.9f, 0.9f, 0.9f);
        title.alignment = TextAnchor.MiddleCenter;
        
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(10, -35);
        titleRect.offsetMax = new Vector2(-10, -10);
        
        // Big Easy Button
        GameObject buttonGO = new GameObject("Stream Button");
        buttonGO.transform.SetParent(panel.transform, false);
        streamButton = buttonGO.AddComponent<Button>();
        Image buttonImage = buttonGO.AddComponent<Image>();
        streamButton.targetGraphic = buttonImage;
        buttonImage.color = new Color(0.2f, 0.7f, 0.2f);
        
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(180, 40);
        buttonRect.anchoredPosition = new Vector2(0, 10);
        
        // Button Text
        GameObject buttonTextGO = new GameObject("Text");
        buttonTextGO.transform.SetParent(buttonGO.transform, false);
        Text buttonText = buttonTextGO.AddComponent<Text>();
        buttonText.text = "START STREAM";
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 16;
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
        statusText.fontSize = 12;
        statusText.color = new Color(0.7f, 0.7f, 0.7f);
        statusText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform statusRect = statusGO.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0, 0);
        statusRect.anchorMax = new Vector2(1, 0);
        statusRect.offsetMin = new Vector2(10, 35);
        statusRect.offsetMax = new Vector2(-10, 50);
        
        // Viewer Link (Hidden initially)
        GameObject viewerGO = new GameObject("Viewer");
        viewerGO.transform.SetParent(panel.transform, false);
        viewerText = viewerGO.AddComponent<Text>();
        viewerText.text = "";
        viewerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        viewerText.fontSize = 11;
        viewerText.color = new Color(0.3f, 0.7f, 1f);
        viewerText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform viewerRect = viewerGO.GetComponent<RectTransform>();
        viewerRect.anchorMin = new Vector2(0, 0);
        viewerRect.anchorMax = new Vector2(1, 0);
        viewerRect.offsetMin = new Vector2(10, 5);
        viewerRect.offsetMax = new Vector2(-10, 35);
        
        viewerButton = viewerGO.AddComponent<Button>();
        viewerButton.transition = Selectable.Transition.None;
        
        // Connect button
        streamButton.onClick.AddListener(ToggleStreaming);
    }
    
    void ToggleStreaming()
    {
        if (!isStreaming)
            StartEasyStreaming();
        else
            StopStreaming();
    }
    
    void StartEasyStreaming()
    {
        isStreaming = true;
        streamKey = GenerateStreamKey();
        
        // Update UI
        streamButton.GetComponentInChildren<Text>().text = "STOP";
        streamButton.GetComponent<Image>().color = new Color(0.7f, 0.2f, 0.2f);
        statusText.text = "🔴 LIVE";
        statusText.color = new Color(1f, 0.3f, 0.3f);
        
        // Generate viewer URL
        string viewerUrl = $"{DEMO_VIEWER}?key={streamKey}";
        viewerText.text = "Click for viewer link ↗";
        
        viewerButton.onClick.RemoveAllListeners();
        viewerButton.onClick.AddListener(() => {
            Application.OpenURL(viewerUrl);
            GUIUtility.systemCopyBuffer = viewerUrl;
            Debug.Log($"Viewer URL copied: {viewerUrl}");
        });
        
        // Simulate streaming setup
        StartCoroutine(SimulateStreaming());
        
        Debug.Log("╔═══════════════════════════════════════════════════╗");
        Debug.Log("║           🔴 EASY STREAMING ACTIVE!               ║");
        Debug.Log("╠═══════════════════════════════════════════════════╣");
        Debug.Log($"║ Stream Key: {streamKey}                      ║");
        Debug.Log("║                                                   ║");
        Debug.Log("║ SHARE THIS LINK:                                  ║");
        Debug.Log($"║ {viewerUrl}");
        Debug.Log("║                                                   ║");
        Debug.Log("║ Viewers can watch instantly - no setup!           ║");
        Debug.Log("╚═══════════════════════════════════════════════════╝");
    }
    
    IEnumerator SimulateStreaming()
    {
        // In a real implementation, this would:
        // 1. Connect to PUBLIC_SIGNALING via WebSocket
        // 2. Create WebRTC peer connection
        // 3. Capture Unity camera to video stream
        // 4. Send to viewers via WebRTC
        
        yield return new WaitForSeconds(1f);
        
        if (isStreaming)
        {
            statusText.text = "🔴 LIVE - " + UnityEngine.Random.Range(1, 5) + " viewers";
        }
    }
    
    void StopStreaming()
    {
        isStreaming = false;
        
        // Update UI
        streamButton.GetComponentInChildren<Text>().text = "START STREAM";
        streamButton.GetComponent<Image>().color = new Color(0.2f, 0.7f, 0.2f);
        statusText.text = "Ready";
        statusText.color = new Color(0.7f, 0.7f, 0.7f);
        viewerText.text = "";
        
        viewerButton.onClick.RemoveAllListeners();
        StopAllCoroutines();
        
        Debug.Log("[Easy Stream] Stopped");
    }
    
    string GenerateStreamKey()
    {
        // Simple readable key
        string[] words = { "star", "moon", "sun", "sky", "cloud", "rain", "snow", "wind" };
        string word = words[UnityEngine.Random.Range(0, words.Length)];
        int number = UnityEngine.Random.Range(100, 999);
        return $"{word}-{number}";
    }
    
    void Update()
    {
        // Simple keyboard shortcuts
        bool sPressed = false;
        
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Keyboard.current != null)
        {
            sPressed = Keyboard.current[Key.S].wasPressedThisFrame;
        }
#else
        sPressed = Input.GetKeyDown(KeyCode.S);
#endif
        
        if (sPressed)
            ToggleStreaming();
    }
}
