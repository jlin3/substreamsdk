using UnityEngine;

/// <summary>
/// THE SIMPLEST POSSIBLE STREAMING DEMO
/// 
/// 1. Add this to ANY GameObject
/// 2. A button appears
/// 3. Click it
/// 4. You're streaming!
/// 
/// That's literally it.
/// </summary>
public class SubstreamOneClick : MonoBehaviour
{
    private bool showButton = true;
    private bool isStreaming = false;
    private string status = "Ready to stream!";
    private float buttonWidth = 200;
    private float buttonHeight = 60;
    
    void OnGUI()
    {
        if (!showButton) return;
        
        // Center the button
        float x = (Screen.width - buttonWidth) / 2;
        float y = (Screen.height - buttonHeight) / 2;
        
        // Status text
        GUI.Label(new Rect(x, y - 30, buttonWidth, 30), status, new GUIStyle(GUI.skin.label) 
        { 
            alignment = TextAnchor.MiddleCenter,
            fontSize = 16
        });
        
        // Big button
        if (GUI.Button(new Rect(x, y, buttonWidth, buttonHeight), 
            isStreaming ? "⏹ STOP STREAMING" : "🔴 GO LIVE"))
        {
            if (!isStreaming)
                StartStreaming();
            else
                StopStreaming();
        }
        
        // Instructions
        if (!isStreaming)
        {
            GUI.Label(new Rect(x, y + 70, buttonWidth, 60), 
                "Click to start streaming\n(Opens browser for screen share)", 
                new GUIStyle(GUI.skin.label) 
                { 
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 12
                });
        }
        else
        {
            GUI.Label(new Rect(x, y + 70, buttonWidth, 60), 
                "Share your Unity Game window\nin the browser that opened!", 
                new GUIStyle(GUI.skin.label) 
                { 
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 12,
                    fontStyle = FontStyle.Bold
                });
        }
    }
    
    void StartStreaming()
    {
        isStreaming = true;
        status = "🔴 STREAMING LIVE!";
        
        // Generate room
        string room = "demo-" + System.DateTime.Now.ToString("HHmmss");
        
        // Open viewer
        string url = $"https://meet.livekit.io/custom?liveKitUrl=wss%3A%2F%2Fsubstream-cnzdthyx.livekit.cloud&roomName={room}";
        Application.OpenURL(url);
        
        Debug.Log($"[Substream] Streaming started! Room: {room}");
        Debug.Log($"[Substream] Share your Unity Game window in the browser!");
        Debug.Log($"[Substream] Viewer URL: {url}");
    }
    
    void StopStreaming()
    {
        isStreaming = false;
        status = "Stream ended. Ready to go again!";
        Debug.Log("[Substream] Streaming stopped");
    }
    
    void Start()
    {
        Debug.Log("[Substream] One-click streaming ready! Just click the button!");
    }
}
