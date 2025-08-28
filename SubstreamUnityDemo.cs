using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// SUBSTREAM UNITY DEMO - Complete streaming solution for Unity developers
/// 
/// This script demonstrates how Unity game developers can integrate streaming
/// for Meta Quest VR games. It provides multiple approaches based on platform.
/// 
/// QUICK START:
/// 1. Add this script to any GameObject
/// 2. Press Play
/// 3. Click "Start Demo" or press SPACE
/// 4. Follow on-screen instructions
/// 
/// For Meta Quest developers: Once the AAR is built, this will capture
/// and stream VR gameplay directly from the headset!
/// </summary>
public class SubstreamUnityDemo : MonoBehaviour
{
    [Header("Demo Configuration")]
    [Tooltip("Your LiveKit Cloud URL (already configured!)")]
    public string LiveKitUrl = "wss://substream-cnzdthyx.livekit.cloud";
    
    [Tooltip("Choose demo mode based on your platform")]
    public DemoMode demoMode = DemoMode.AutoDetect;
    
    [Header("Stream Quality")]
    public int streamWidth = 1920;
    public int streamHeight = 1080;
    public int streamFps = 60;
    public int bitrateKbps = 5000;
    
    public enum DemoMode
    {
        AutoDetect,      // Automatically choose best option
        ScreenShare,     // Use browser screen sharing (works today!)
        LiveKitNative,   // Use LiveKit Unity SDK (requires SDK)
        QuestNative      // Use Quest MediaProjection (requires AAR)
    }
    
    // UI Elements
    private Canvas canvas;
    private GameObject mainPanel;
    private Text titleText;
    private Text statusText;
    private Button startButton;
    private GameObject instructionsPanel;
    private Text instructionsText;
    private Button actionButton;
    
    // State
    private bool isStreaming = false;
    private string roomName = "";
    private string viewerUrl = "";
    private DemoMode actualMode;
    
    void Start()
    {
        CreateDemoUI();
        DetectBestMode();
        UpdateUI();
    }
    
    void DetectBestMode()
    {
        if (demoMode != DemoMode.AutoDetect)
        {
            actualMode = demoMode;
            return;
        }
        
        // Auto-detect best streaming method
#if UNITY_ANDROID && !UNITY_EDITOR
        // On Quest/Android
        if (HasQuestAAR())
        {
            actualMode = DemoMode.QuestNative;
            Debug.Log("[Substream] Quest AAR detected - using native streaming!");
        }
        else
        {
            actualMode = DemoMode.ScreenShare;
            Debug.Log("[Substream] Quest AAR not found - using screen share demo");
        }
#else
        // In Unity Editor or other platforms
        if (HasLiveKitSDK())
        {
            actualMode = DemoMode.LiveKitNative;
            Debug.Log("[Substream] LiveKit SDK detected - using native capture!");
        }
        else
        {
            actualMode = DemoMode.ScreenShare;
            Debug.Log("[Substream] Using screen share demo (works immediately!)");
        }
#endif
    }
    
    bool HasQuestAAR()
    {
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
    }
    
    bool HasLiveKitSDK()
    {
        return Type.GetType("LiveKit.Room, LiveKit") != null;
    }
    
    void CreateDemoUI()
    {
        // Find or create Canvas
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        // Create EventSystem if needed
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            eventSystemGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            #else
            eventSystemGO.AddComponent<StandaloneInputModule>();
            #endif
        }
        
        // Main Panel
        mainPanel = CreatePanel("Main Panel", new Vector2(500, 350));
        
        // Title
        titleText = CreateText(mainPanel, "SUBSTREAM UNITY SDK", 28, new Vector2(0, 120), TextAnchor.MiddleCenter);
        titleText.fontStyle = FontStyle.Bold;
        
        // Status
        statusText = CreateText(mainPanel, "Initializing...", 18, new Vector2(0, 60), TextAnchor.MiddleCenter);
        
        // Start Button
        startButton = CreateButton(mainPanel, "🎮 START DEMO", new Vector2(0, 0), new Vector2(300, 60));
        startButton.onClick.AddListener(OnStartDemo);
        
        // Instructions Panel (hidden initially)
        instructionsPanel = CreatePanel("Instructions", new Vector2(600, 400));
        instructionsPanel.SetActive(false);
        
        instructionsText = CreateText(instructionsPanel, "", 16, new Vector2(0, 50), TextAnchor.MiddleCenter);
        actionButton = CreateButton(instructionsPanel, "Got it!", new Vector2(0, -120), new Vector2(200, 50));
        actionButton.onClick.AddListener(() => instructionsPanel.SetActive(false));
    }
    
    GameObject CreatePanel(string name, Vector2 size)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(canvas.transform, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;
        
        Image img = panel.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        
        return panel;
    }
    
    Text CreateText(GameObject parent, string text, int fontSize, Vector2 position, TextAnchor alignment)
    {
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(parent.transform, false);
        
        Text textComp = textGO.AddComponent<Text>();
        textComp.text = text;
        textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComp.fontSize = fontSize;
        textComp.color = Color.white;
        textComp.alignment = alignment;
        
        RectTransform rect = textGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(550, 100);
        rect.anchoredPosition = position;
        
        return textComp;
    }
    
    Button CreateButton(GameObject parent, string text, Vector2 position, Vector2 size)
    {
        GameObject buttonGO = new GameObject("Button");
        buttonGO.transform.SetParent(parent.transform, false);
        
        Button button = buttonGO.AddComponent<Button>();
        Image img = buttonGO.AddComponent<Image>();
        button.targetGraphic = img;
        img.color = new Color(0.2f, 0.7f, 0.3f);
        
        RectTransform rect = buttonGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        
        // Button text
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        Text buttonText = textGO.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 20;
        buttonText.fontStyle = FontStyle.Bold;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        return button;
    }
    
    void UpdateUI()
    {
        string modeText = actualMode switch
        {
            DemoMode.QuestNative => "Quest Native Streaming",
            DemoMode.LiveKitNative => "LiveKit SDK Streaming",
            DemoMode.ScreenShare => "Screen Share Demo",
            _ => "Unknown"
        };
        
        statusText.text = $"Ready! Mode: {modeText}";
        
        if (actualMode == DemoMode.ScreenShare)
        {
            statusText.text += "\n<size=14>Perfect for tomorrow's demo!</size>";
        }
    }
    
    void OnStartDemo()
    {
        switch (actualMode)
        {
            case DemoMode.ScreenShare:
                StartScreenShareDemo();
                break;
            case DemoMode.LiveKitNative:
                StartLiveKitDemo();
                break;
            case DemoMode.QuestNative:
                StartQuestDemo();
                break;
        }
    }
    
    void StartScreenShareDemo()
    {
        StartCoroutine(ScreenShareFlow());
    }
    
    IEnumerator ScreenShareFlow()
    {
        // Step 1: Create LiveKit room
        statusText.text = "Creating streaming room...";
        roomName = "unity-demo-" + DateTime.Now.ToString("HHmmss");
        
        yield return new WaitForSeconds(0.5f); // Simulate API call
        
        // Step 2: Generate viewer URL
        string encodedUrl = System.Uri.EscapeDataString(LiveKitUrl);
        viewerUrl = $"https://meet.livekit.io/custom?liveKitUrl={encodedUrl}&roomName={roomName}";
        
        // Step 3: Show instructions
        ShowScreenShareInstructions();
        
        statusText.text = "🔴 Room Created! Follow instructions...";
        startButton.GetComponentInChildren<Text>().text = "📋 SHOW INSTRUCTIONS";
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(() => ShowScreenShareInstructions());
        
        Debug.Log("═══════════════════════════════════════════════════════");
        Debug.Log("🎮 UNITY STREAMING DEMO - SCREEN SHARE MODE");
        Debug.Log("═══════════════════════════════════════════════════════");
        Debug.Log($"Room: {roomName}");
        Debug.Log($"Viewer URL: {viewerUrl}");
        Debug.Log("═══════════════════════════════════════════════════════");
    }
    
    void ShowScreenShareInstructions()
    {
        instructionsPanel.SetActive(true);
        instructionsText.text = @"<b>🎮 SCREEN SHARE DEMO INSTRUCTIONS</b>

1. Click the button below to open LiveKit
2. Click 'Join' to enter the room
3. Click 'Share Screen' 🖥️
4. Select your Unity Game window
5. Viewers will see your game!

<b>Perfect for tomorrow's demo!</b>
Shows real gameplay streaming immediately.";
        
        actionButton.GetComponentInChildren<Text>().text = "🌐 OPEN LIVEKIT";
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => {
            Application.OpenURL(viewerUrl);
            instructionsPanel.SetActive(false);
        });
    }
    
    void StartLiveKitDemo()
    {
        instructionsPanel.SetActive(true);
        instructionsText.text = @"<b>🎥 LIVEKIT SDK STREAMING</b>

This mode requires the LiveKit Unity SDK.

<b>To enable:</b>
1. Import LiveKit Unity SDK
2. Add LIVEKIT_SDK_AVAILABLE to Scripting Defines
3. Restart this demo

<b>Benefits:</b>
• Direct camera capture
• No browser needed
• Automatic streaming

For now, try Screen Share mode!";
        
        actionButton.GetComponentInChildren<Text>().text = "Use Screen Share Instead";
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => {
            actualMode = DemoMode.ScreenShare;
            instructionsPanel.SetActive(false);
            UpdateUI();
        });
    }
    
    void StartQuestDemo()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(QuestStreamingFlow());
        #else
        instructionsPanel.SetActive(true);
        instructionsText.text = @"<b>🥽 QUEST NATIVE STREAMING</b>

This mode only works on Meta Quest devices.

<b>Requirements:</b>
• Build and deploy to Quest
• substream-release.aar installed
• MediaProjection permission

<b>What it captures:</b>
• Full VR view (both eyes)
• System audio
• 1920x1080 @ 60fps

Switch to Editor mode to test in Unity!";
        #endif
    }
    
    #if UNITY_ANDROID && !UNITY_EDITOR
    IEnumerator QuestStreamingFlow()
    {
        statusText.text = "Requesting screen capture permission...";
        
        // Call native code to start streaming
        using (var jc = new AndroidJavaClass("com.substream.sdk.SubstreamNative"))
        {
            jc.CallStatic("startLive", streamWidth, streamHeight, streamFps, bitrateKbps, "{}", true);
        }
        
        yield return new WaitForSeconds(2f);
        
        statusText.text = "🔴 STREAMING LIVE FROM QUEST!";
        instructionsText.text = $@"<b>🥽 YOU'RE LIVE ON QUEST!</b>

Viewers can watch at:
{viewerUrl}

Your VR gameplay is being streamed!";
        
        instructionsPanel.SetActive(true);
    }
    #endif
    
    void Update()
    {
        // Keyboard shortcuts
        bool spacePressed = false;
        
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Keyboard.current != null)
        {
            spacePressed = Keyboard.current[Key.Space].wasPressedThisFrame;
        }
#else
        spacePressed = Input.GetKeyDown(KeyCode.Space);
#endif
        
        if (spacePressed && !isStreaming)
        {
            OnStartDemo();
        }
    }
    
    void OnDestroy()
    {
        // Cleanup if needed
        if (isStreaming)
        {
            Debug.Log("[Substream] Cleaning up streaming session");
        }
    }
}
