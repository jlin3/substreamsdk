using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// LIVEKIT QUICK TEST - Hardcoded token for immediate testing!
/// 
/// This will create a real LiveKit room you can view at:
/// https://meet.livekit.io
/// 
/// SETUP:
/// 1. Make sure LiveKit Unity SDK is imported
/// 2. Add this script to any GameObject
/// 3. Press Play
/// 4. Check console for connection status
/// </summary>
public class LiveKitQuickTest : MonoBehaviour
{
    // Hardcoded for immediate testing
    private string url = "wss://substream-cnzdthyx.livekit.cloud";
    private string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3NTYzNTIwMTMsImlzcyI6IkFQSWJ0cEh1UVltU3ZUVCIsInN1YiI6InVuaXR5LXN0cmVhbWVyIiwidmlkZW8iOnsicm9vbSI6InVuaXR5LXRlc3Qtcm9vbSIsInJvb21Kb2luIjp0cnVlLCJjYW5QdWJsaXNoIjp0cnVlLCJjYW5QdWJsaXNoRGF0YSI6dHJ1ZSwiY2FuU3Vic2NyaWJlIjp0cnVlfSwicm9vbUNyZWF0ZSI6dHJ1ZSwicm9vbUxpc3QiOnRydWUsInJvb21SZWNvcmQiOnRydWUsImlkZW50aXR5IjoidW5pdHktc3RyZWFtZXIiLCJuYW1lIjoidW5pdHktc3RyZWFtZXIifQ.DZdFEnTtm60evMDZXuh8AMq52zbW75-t0zQnj-bhN8E";
    private string roomName = "unity-test-room";
    
    // Simple UI
    private bool isConnected = false;
    private string statusMessage = "Press SPACE to connect to LiveKit";
    
    void Start()
    {
        Debug.Log("╔═══════════════════════════════════════════════════╗");
        Debug.Log("║          LIVEKIT QUICK TEST READY!                ║");
        Debug.Log("╠═══════════════════════════════════════════════════╣");
        Debug.Log("║ Press SPACE to connect to LiveKit                 ║");
        Debug.Log("║                                                   ║");
        Debug.Log("║ TO VIEW YOUR STREAM:                              ║");
        Debug.Log("║ 1. Go to: https://meet.livekit.io                 ║");
        Debug.Log("║ 2. Custom Server                                  ║");
        Debug.Log("║ 3. LiveKit URL: wss://substream-cnzdthyx.livekit.cloud");
        Debug.Log($"║ 4. Room: {roomName}                         ║");
        Debug.Log("║ 5. Your name: viewer                              ║");
        Debug.Log("║ 6. Click Join                                     ║");
        Debug.Log("╚═══════════════════════════════════════════════════╝");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isConnected)
        {
            ConnectToLiveKit();
        }
    }
    
    async void ConnectToLiveKit()
    {
        statusMessage = "Connecting to LiveKit...";
        Debug.Log($"[LiveKit] Connecting to {url}...");
        
        try
        {
            // Try to use LiveKit SDK if available
            var roomType = System.Type.GetType("LiveKit.Room, LiveKit");
            if (roomType != null)
            {
                dynamic room = System.Activator.CreateInstance(roomType);
                await room.Connect(url, token);
                
                isConnected = true;
                statusMessage = $"Connected to room: {roomName}";
                Debug.Log($"[LiveKit] ✅ Connected to room: {roomName}");
                
                // Try to enable camera
                try
                {
                    await room.LocalParticipant.SetCameraEnabled(true);
                    Debug.Log("[LiveKit] ✅ Camera enabled - you should be streaming!");
                }
                catch (System.Exception camEx)
                {
                    Debug.Log($"[LiveKit] ⚠️  Camera not enabled: {camEx.Message}");
                    Debug.Log("[LiveKit] The room is created - viewers can join, but video needs setup");
                }
            }
            else
            {
                Debug.LogError("[LiveKit] ❌ LiveKit SDK not found! Import the LiveKit Unity package first.");
                statusMessage = "LiveKit SDK not found!";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[LiveKit] ❌ Connection failed: {e.Message}");
            statusMessage = $"Failed: {e.Message}";
            isConnected = false;
        }
    }
    
    void OnGUI()
    {
        // Simple status display
        GUI.Box(new Rect(10, 10, 400, 30), statusMessage);
        
        if (isConnected)
        {
            GUI.Box(new Rect(10, 50, 400, 60), 
                $"Room: {roomName}\n" +
                "Go to https://meet.livekit.io to view\n" +
                "Use Custom Server with the URL above");
        }
    }
}
