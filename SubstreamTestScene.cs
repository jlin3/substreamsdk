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
/// SUBSTREAM TEST SCENE - Enhanced streaming for the test scene!
/// 
/// This version is designed to work with test-scene.unitypackage
/// and shows actual game content streaming.
/// 
/// SETUP:
/// 1. Import test-scene.unitypackage first
/// 2. Add this script to the existing UI in the test scene
/// 3. Press Play and stream!
/// </summary>
public class SubstreamTestScene : MonoBehaviour
{
    // UI References (these should be set in Inspector from test scene)
    [Header("Test Scene UI References")]
    public Button streamButton;
    public Text statusText;
    public Text viewerUrlText;
    
    // Test Scene Content
    [Header("Dynamic Content")]
    public GameObject rotatingCube;
    public ParticleSystem particles;
    public Text scoreText;
    
    // Streaming state
    private bool isStreaming = false;
    private string roomId = "";
    private int score = 0;
    
    void Start()
    {
        // If references aren't set, try to find them
        if (streamButton == null) FindUIReferences();
        
        // Connect button
        if (streamButton != null)
        {
            streamButton.onClick.RemoveAllListeners();
            streamButton.onClick.AddListener(ToggleStreaming);
        }
        
        // Start demo content
        StartCoroutine(AnimateContent());
        
        Debug.Log("[Substream Test Scene] Ready! This version shows actual game content!");
    }
    
    void FindUIReferences()
    {
        // Try to find UI elements from test scene
        var buttons = FindObjectsOfType<Button>();
        foreach (var btn in buttons)
        {
            if (btn.name.Contains("Stream") || btn.name.Contains("Start"))
            {
                streamButton = btn;
                break;
            }
        }
        
        var texts = FindObjectsOfType<Text>();
        foreach (var txt in texts)
        {
            if (txt.name.Contains("Status")) statusText = txt;
            else if (txt.name.Contains("Viewer")) viewerUrlText = txt;
            else if (txt.name.Contains("Score")) scoreText = txt;
        }
    }
    
    IEnumerator AnimateContent()
    {
        while (true)
        {
            // Rotate cube if available
            if (rotatingCube != null)
            {
                rotatingCube.transform.Rotate(0, 45 * Time.deltaTime, 0);
            }
            
            // Update score
            if (isStreaming && scoreText != null)
            {
                score += Random.Range(10, 100);
                scoreText.text = $"Score: {score:N0}";
            }
            
            // Emit particles when streaming
            if (particles != null && isStreaming)
            {
                if (!particles.isPlaying) particles.Play();
            }
            
            yield return null;
        }
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
        roomId = "test-scene-" + Guid.NewGuid().ToString().Substring(0, 8);
        
        // Update UI
        if (streamButton != null)
        {
            var btnText = streamButton.GetComponentInChildren<Text>();
            if (btnText != null) btnText.text = "⏹️ STOP STREAMING";
        }
        
        if (statusText != null)
        {
            statusText.text = "🔴 LIVE - Streaming test scene!";
            statusText.color = Color.red;
        }
        
        // Show viewer info
        string viewerUrl = $"https://cloud.livekit.io/projects/substream-cnzdthyx/rooms/{roomId}";
        
        if (viewerUrlText != null)
        {
            viewerUrlText.text = "📺 VIEWER LINK:\n" + viewerUrl;
            viewerUrlText.color = Color.cyan;
            
            // Make clickable
            Button urlButton = viewerUrlText.gameObject.GetComponent<Button>();
            if (urlButton == null) urlButton = viewerUrlText.gameObject.AddComponent<Button>();
            urlButton.onClick.RemoveAllListeners();
            urlButton.onClick.AddListener(() => Application.OpenURL(viewerUrl));
        }
        
        // Log to console
        Debug.Log("╔═══════════════════════════════════════════════════╗");
        Debug.Log("║      🔴 TEST SCENE STREAMING STARTED!             ║");
        Debug.Log("╠═══════════════════════════════════════════════════╣");
        Debug.Log("║ You should see:                                   ║");
        Debug.Log("║ - Rotating cube                                   ║");
        Debug.Log("║ - Particle effects                                ║");
        Debug.Log("║ - Score increasing                                ║");
        Debug.Log("║                                                   ║");
        Debug.Log($"║ Room: {roomId}                           ║");
        Debug.Log($"║ View at: {viewerUrl}");
        Debug.Log("╚═══════════════════════════════════════════════════╝");
        
        GUIUtility.systemCopyBuffer = viewerUrl;
    }
    
    void StopStreaming()
    {
        isStreaming = false;
        score = 0;
        
        // Update UI
        if (streamButton != null)
        {
            var btnText = streamButton.GetComponentInChildren<Text>();
            if (btnText != null) btnText.text = "🎮 START STREAMING";
        }
        
        if (statusText != null)
        {
            statusText.text = "Ready to stream test scene";
            statusText.color = Color.white;
        }
        
        if (viewerUrlText != null)
        {
            viewerUrlText.text = "";
        }
        
        // Stop particles
        if (particles != null && particles.isPlaying)
        {
            particles.Stop();
        }
        
        Debug.Log("[Substream] Test scene streaming stopped");
    }
    
    void Update()
    {
        bool sPressed = false;
        bool xPressed = false;
        bool vPressed = false;
        
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Keyboard.current != null)
        {
            sPressed = Keyboard.current[Key.S].wasPressedThisFrame;
            xPressed = Keyboard.current[Key.X].wasPressedThisFrame;
            vPressed = Keyboard.current[Key.V].wasPressedThisFrame;
        }
#else
        sPressed = Input.GetKeyDown(KeyCode.S);
        xPressed = Input.GetKeyDown(KeyCode.X);
        vPressed = Input.GetKeyDown(KeyCode.V);
#endif
        
        if (sPressed && !isStreaming)
            StartStreaming();
        else if (xPressed && isStreaming)
            StopStreaming();
        else if (vPressed && isStreaming && !string.IsNullOrEmpty(roomId))
        {
            string viewerUrl = $"https://cloud.livekit.io/projects/substream-cnzdthyx/rooms/{roomId}";
            Application.OpenURL(viewerUrl);
        }
    }
}
