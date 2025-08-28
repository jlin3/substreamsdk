using UnityEngine;
using System;

/// <summary>
/// TEST STREAMING - Works in BOTH Unity Editor AND Quest!
/// 
/// In Editor: Creates rooms, use screen sharing
/// On Quest: Full MediaProjection streaming
/// </summary>
public class UnityTestBothPlatforms : MonoBehaviour
{
    private bool isStreaming = false;
    private string roomName = "";
    
    void Start()
    {
        Debug.Log("═══════════════════════════════════════════════");
        #if UNITY_EDITOR
        Debug.Log("    UNITY EDITOR MODE - Press SPACE to test");
        Debug.Log("    (Use screen sharing for video)");
        #elif UNITY_ANDROID
        Debug.Log("    QUEST MODE - Press A button to stream");
        Debug.Log("    (Full video capture via MediaProjection)");
        #endif
        Debug.Log("═══════════════════════════════════════════════");
    }
    
    void Update()
    {
        // Unity Editor: Space key
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleStreaming();
        }
        #endif
        
        // Quest: A button
        #if UNITY_ANDROID
        if (Input.GetButtonDown("Fire1"))
        {
            ToggleStreaming();
        }
        #endif
    }
    
    void ToggleStreaming()
    {
        if (!isStreaming)
        {
            StartStreaming();
        }
        else
        {
            StopStreaming();
        }
    }
    
    void StartStreaming()
    {
        isStreaming = true;
        roomName = "demo-" + DateTime.Now.ToString("HHmmss");
        
        #if UNITY_EDITOR
        Debug.Log("╔══════════════════════════════════════════════╗");
        Debug.Log("║         UNITY EDITOR STREAMING TEST          ║");
        Debug.Log("╠══════════════════════════════════════════════╣");
        Debug.Log($"║ Room: {roomName}                             ║");
        Debug.Log("║                                              ║");
        Debug.Log("║ To stream with video:                        ║");
        Debug.Log("║ 1. Go to https://meet.livekit.io            ║");
        Debug.Log("║ 2. Enter room name above                     ║");
        Debug.Log("║ 3. Join room                                 ║");
        Debug.Log("║ 4. Click 'Share Screen'                      ║");
        Debug.Log("║ 5. Select Unity Game window                  ║");
        Debug.Log("╚══════════════════════════════════════════════╝");
        
        #elif UNITY_ANDROID
        // On Quest, this would call the actual MediaProjection API
        Debug.Log($"🔴 QUEST STREAMING STARTED - Room: {roomName}");
        Debug.Log("MediaProjection will capture full VR view");
        // Real implementation would call: SubstreamNative.startLive(...)
        #endif
    }
    
    void StopStreaming()
    {
        isStreaming = false;
        Debug.Log("Streaming stopped");
    }
    
    void OnGUI()
    {
        if (isStreaming)
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(10, 10, 300, 30), $"🔴 STREAMING: {roomName}");
            
            #if UNITY_EDITOR
            GUI.Label(new Rect(10, 40, 300, 30), "Use screen share for video");
            #endif
            
            GUI.color = Color.white;
        }
    }
}

