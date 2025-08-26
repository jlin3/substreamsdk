using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using LiveKit;

/// <summary>
/// LiveKit Unity Streaming Demo - Ready to use!
/// 
/// Instructions:
/// 1. Import LiveKit Unity SDK: https://github.com/livekit/client-sdk-unity.git
/// 2. Add this script to a GameObject
/// 3. Connect UI elements in Inspector
/// 4. Press Play and click "Start Streaming"
/// 5. View at: https://cloud.livekit.io/projects/substream-cnzdthyx/rooms
/// </summary>
public class LiveKitStreamingDemo : MonoBehaviour
{
    [Header("UI Elements")]
    public Button startButton;
    public Button stopButton;
    public Text statusText;
    public Text roomInfoText;
    
    [Header("LiveKit Settings")]
    [SerializeField] private string livekitUrl = "wss://substream-cnzdthyx.livekit.cloud";
    [SerializeField] private string roomName = "unity-demo-room";
    
    // Pre-generated token (valid for 24 hours from generation)
    private string accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3NTYzMzEyOTksImlzcyI6IkFQSWJ0cEh1UVltU3ZUVCIsInN1YiI6InVuaXR5LXN0cmVhbWVyIiwidmlkZW8iOnsicm9vbUpvaW4iOnRydWUsInJvb20iOiJ1bml0eS1kZW1vLXJvb20iLCJjYW5QdWJsaXNoIjp0cnVlLCJjYW5QdWJsaXNoRGF0YSI6dHJ1ZSwiY2FuU3Vic2NyaWJlIjp0cnVlLCJjYW5QdWJsaXNoU291cmNlcyI6WyJjYW1lcmEiLCJtaWNyb3Bob25lIiwic2NyZWVuX3NoYXJlIiwic2NyZWVuX3NoYXJlX2F1ZGlvIl19LCJtZXRhZGF0YSI6IntcInBsYXRmb3JtXCI6IFwidW5pdHlcIiwgXCJkZXZpY2VcIjogXCJxdWVzdFwifSJ9.8zsDZKPPmmtktn1eMMuf1vH8sMdM5QT1x7ePG9FNWyQ";
    
    [Header("Stream Settings")]
    [SerializeField] private bool streamVideo = true;
    [SerializeField] private bool streamAudio = true;
    [SerializeField] private VideoCodec videoCodec = VideoCodec.H264;
    [SerializeField] private int videoBitrate = 3500000; // 3.5 Mbps
    [SerializeField] private int videoFramerate = 30;
    
    private Room room;
    private LocalVideoTrack videoTrack;
    private LocalAudioTrack audioTrack;
    private bool isStreaming = false;
    private TextureVideoSource videoSource;
    private MicrophoneSource audioSource;
    
    void Start()
    {
        // Connect UI
        if (startButton) startButton.onClick.AddListener(StartStreaming);
        if (stopButton) stopButton.onClick.AddListener(StopStreaming);
        
        UpdateUI("Ready to stream", false);
        
        // Show connection info
        if (roomInfoText)
        {
            roomInfoText.text = $"Room: {roomName}\nServer: {livekitUrl}";
        }
    }
    
    public void StartStreaming()
    {
        if (!isStreaming)
        {
            StartCoroutine(ConnectAndStream());
        }
    }
    
    public void StopStreaming()
    {
        if (isStreaming)
        {
            StartCoroutine(Disconnect());
        }
    }
    
    IEnumerator ConnectAndStream()
    {
        UpdateUI("Connecting to LiveKit...", false);
        
        // Create room
        room = new Room();
        
        // Set up event handlers
        room.ParticipantConnected += OnParticipantConnected;
        room.ParticipantDisconnected += OnParticipantDisconnected;
        room.TrackPublished += OnTrackPublished;
        
        // Connect to room
        var connect = room.Connect(livekitUrl, accessToken);
        yield return connect;
        
        if (connect.IsError)
        {
            UpdateUI($"Connection failed: {connect.Error}", false);
            Debug.LogError($"[LiveKit] Connection failed: {connect.Error}");
            yield break;
        }
        
        Debug.Log($"[LiveKit] Connected to room: {room.Name}");
        UpdateUI("Connected! Starting streams...", false);
        
        // Publish video if enabled
        if (streamVideo)
        {
            yield return PublishVideo();
        }
        
        // Publish audio if enabled
        if (streamAudio)
        {
            yield return PublishAudio();
        }
        
        isStreaming = true;
        UpdateUI("🔴 LIVE - Streaming!", true);
        
        // Log viewing instructions
        Debug.Log("=====================================");
        Debug.Log("🎮 STREAM IS LIVE!");
        Debug.Log($"Room: {roomName}");
        Debug.Log("");
        Debug.Log("👀 VIEW YOUR STREAM HERE:");
        Debug.Log("https://cloud.livekit.io/projects/substream-cnzdthyx/rooms");
        Debug.Log($"→ Click on room '{roomName}'");
        Debug.Log("→ Click 'Join' to watch");
        Debug.Log("=====================================");
    }
    
    IEnumerator PublishVideo()
    {
        Debug.Log("[LiveKit] Publishing video...");
        
        // Create render texture for camera capture
        var renderTexture = new RenderTexture(1920, 1080, 24, RenderTextureFormat.ARGB32);
        renderTexture.Create();
        
        // Assign to main camera
        if (Camera.main != null)
        {
            Camera.main.targetTexture = renderTexture;
            videoSource = new TextureVideoSource(renderTexture);
        }
        else
        {
            // Fallback to screen capture
            Debug.LogWarning("[LiveKit] No main camera found, using screen capture");
            videoSource = new ScreenVideoSource();
        }
        
        // Create video track
        videoTrack = LocalVideoTrack.CreateVideoTrack("unity-video", videoSource, room);
        
        // Set up publish options
        var options = new TrackPublishOptions
        {
            VideoCodec = videoCodec,
            Source = TrackSource.SourceCamera,
            VideoEncoding = new VideoEncoding
            {
                MaxBitrate = (ulong)videoBitrate,
                MaxFramerate = (ulong)videoFramerate
            }
        };
        
        // Publish track
        var publish = room.LocalParticipant.PublishTrack(videoTrack, options);
        yield return publish;
        
        if (publish.IsError)
        {
            Debug.LogError($"[LiveKit] Video publish failed: {publish.Error}");
        }
        else
        {
            videoSource.Start();
            StartCoroutine(videoSource.Update());
            Debug.Log("[LiveKit] Video published successfully!");
        }
    }
    
    IEnumerator PublishAudio()
    {
        Debug.Log("[LiveKit] Publishing audio...");
        
        // Check for microphone
        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("[LiveKit] No microphone found");
            yield break;
        }
        
        // Create audio source
        var audioObject = new GameObject("LiveKit_Microphone");
        audioSource = new MicrophoneSource(Microphone.devices[0], audioObject);
        audioTrack = LocalAudioTrack.CreateAudioTrack("unity-audio", audioSource, room);
        
        // Set up publish options
        var options = new TrackPublishOptions
        {
            Source = TrackSource.SourceMicrophone,
            AudioEncoding = new AudioEncoding
            {
                MaxBitrate = 128000 // 128 kbps
            }
        };
        
        // Publish track
        var publish = room.LocalParticipant.PublishTrack(audioTrack, options);
        yield return publish;
        
        if (publish.IsError)
        {
            Debug.LogError($"[LiveKit] Audio publish failed: {publish.Error}");
        }
        else
        {
            audioSource.Start();
            Debug.Log("[LiveKit] Audio published successfully!");
        }
    }
    
    IEnumerator Disconnect()
    {
        UpdateUI("Disconnecting...", false);
        
        // Stop sources
        if (videoSource != null)
        {
            videoSource.Stop();
            videoSource = null;
        }
        
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource = null;
        }
        
        // Disconnect from room
        if (room != null)
        {
            room.Disconnect();
            room = null;
        }
        
        // Reset camera
        if (Camera.main != null)
        {
            Camera.main.targetTexture = null;
        }
        
        isStreaming = false;
        UpdateUI("Disconnected", false);
        
        yield return null;
    }
    
    void UpdateUI(string status, bool streaming)
    {
        if (statusText) statusText.text = status;
        if (startButton) startButton.interactable = !streaming;
        if (stopButton) stopButton.interactable = streaming;
    }
    
    // Event handlers
    void OnParticipantConnected(RemoteParticipant participant)
    {
        Debug.Log($"[LiveKit] Participant connected: {participant.Identity}");
    }
    
    void OnParticipantDisconnected(RemoteParticipant participant)
    {
        Debug.Log($"[LiveKit] Participant disconnected: {participant.Identity}");
    }
    
    void OnTrackPublished(RemoteTrackPublication publication, RemoteParticipant participant)
    {
        Debug.Log($"[LiveKit] Track published: {publication.Sid} by {participant.Identity}");
    }
    
    void OnDestroy()
    {
        if (isStreaming)
        {
            StopStreaming();
        }
    }
    
    // Helper methods for testing
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && !isStreaming)
        {
            StartStreaming();
        }
        
        if (Input.GetKeyDown(KeyCode.X) && isStreaming)
        {
            StopStreaming();
        }
    }
}
