using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorkingStreamSolution : MonoBehaviour
{
    private string roomName;
    private Canvas canvas;
    
    void Start()
    {
        roomName = "unity-demo-" + Random.Range(1000, 9999);
        CreateUI();
        ShowInstructions();
    }
    
    void CreateUI()
    {
        // Canvas setup
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            
            GameObject eventGO = new GameObject("EventSystem");
            eventGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        // Main panel
        GameObject panel = new GameObject("Stream Panel");
        panel.transform.SetParent(canvas.transform, false);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(500, 350);
        
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.05f, 0.05f, 0.95f);
        
        // Title
        CreateText(panel, "WORKING UNITY → WEB STREAM", new Vector2(0, 140), 24, FontStyle.Bold);
        
        // Method 1
        CreateText(panel, "METHOD 1: Screen Share (Works Now!)", new Vector2(0, 80), 18, FontStyle.Bold);
        CreateText(panel, $"1. Go to: https://meet.livekit.io\n2. Join room: {roomName}\n3. Click 'Share Screen'\n4. Select Unity window", new Vector2(0, 30), 14);
        
        // Button to open viewer
        GameObject btn1 = CreateButton(panel, "Open LiveKit Meet", new Vector2(-120, -30), new Vector2(200, 40));
        btn1.GetComponent<Button>().onClick.AddListener(() => {
            Application.OpenURL($"https://meet.livekit.io/custom?liveKitUrl=wss://substream-cnzdthyx.livekit.cloud&roomName={roomName}&participantName=streamer&connect=true");
        });
        
        // Copy room name button
        GameObject btn2 = CreateButton(panel, "Copy Room Name", new Vector2(120, -30), new Vector2(200, 40));
        btn2.GetComponent<Button>().onClick.AddListener(() => {
            GUIUtility.systemCopyBuffer = roomName;
            btn2.GetComponentInChildren<Text>().text = "Copied!";
            StartCoroutine(ResetText(btn2.GetComponentInChildren<Text>(), "Copy Room Name"));
        });
        
        // Method 2
        CreateText(panel, "METHOD 2: OBS + Browser", new Vector2(0, -90), 18, FontStyle.Bold);
        CreateText(panel, "1. OBS: Add 'Game Capture' → Unity\n2. OBS: Start Virtual Camera\n3. In browser: Use OBS Virtual Camera as input", new Vector2(0, -130), 14);
    }
    
    void ShowInstructions()
    {
        Debug.Log("=== WORKING STREAMING SOLUTION ===");
        Debug.Log($"Room: {roomName}");
        Debug.Log("Option 1: Use screen share in browser");
        Debug.Log("Option 2: Use OBS Virtual Camera");
        Debug.Log("Both work with your LiveKit Cloud TODAY!");
    }
    
    GameObject CreateText(GameObject parent, string content, Vector2 pos, int fontSize, FontStyle style = FontStyle.Normal)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent.transform, false);
        
        Text text = go.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(450, 50);
        rect.anchoredPosition = pos;
        
        return go;
    }
    
    GameObject CreateButton(GameObject parent, string label, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject("Button");
        go.transform.SetParent(parent.transform, false);
        
        Button btn = go.AddComponent<Button>();
        Image img = go.AddComponent<Image>();
        btn.targetGraphic = img;
        img.color = new Color(0.2f, 0.5f, 0.8f);
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = pos;
        
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        Text text = textGO.AddComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 16;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        
        return go;
    }
    
    IEnumerator ResetText(Text text, string original)
    {
        yield return new WaitForSeconds(2);
        text.text = original;
    }
}
