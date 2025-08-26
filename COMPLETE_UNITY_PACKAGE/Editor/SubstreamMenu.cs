using UnityEngine;
using UnityEditor;

public class SubstreamMenu : MonoBehaviour
{
    [MenuItem("GameObject/Substream/Create Demo Scene", false, 10)]
    static void CreateDemoScene()
    {
        // Create a new scene with all demo elements
        UnityEditor.SceneManagement.EditorSceneManager.NewScene(
            UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects,
            UnityEditor.SceneManagement.NewSceneMode.Single
        );
        
        // Create UI Canvas
        GameObject canvas = new GameObject("Canvas");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        // Create Demo Controller
        GameObject demoController = new GameObject("Demo Controller");
        SimpleDemoScript demo = demoController.AddComponent<SimpleDemoScript>();
        
        // Create UI Panel
        GameObject panel = CreateUIPanel(canvas.transform);
        
        // Create buttons and text
        var startButton = CreateButton(panel.transform, "Start Stream", new Vector2(-100, 0));
        var stopButton = CreateButton(panel.transform, "Stop Stream", new Vector2(100, 0));
        var statusText = CreateText(panel.transform, "Ready to stream", new Vector2(0, 50));
        var viewerText = CreateText(panel.transform, "", new Vector2(0, -50));
        
        // Connect to demo script
        demo.startButton = startButton.GetComponent<UnityEngine.UI.Button>();
        demo.stopButton = stopButton.GetComponent<UnityEngine.UI.Button>();
        demo.statusText = statusText.GetComponent<UnityEngine.UI.Text>();
        demo.viewerLinkText = viewerText.GetComponent<UnityEngine.UI.Text>();
        
        // Configure default streaming settings
        demo.streamWidth = 1920;
        demo.streamHeight = 1080;
        demo.streamFPS = 30;
        demo.bitrateKbps = 3500;
        demo.includeAudio = true;
        
        viewerText.SetActive(false);
        
        Debug.Log("[Substream] Demo scene created! Press Play to test streaming.");
        Selection.activeGameObject = demoController;
    }
    
    static GameObject CreateUIPanel(Transform parent)
    {
        GameObject panel = new GameObject("Stream Panel");
        panel.transform.SetParent(parent, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(400, 200);
        rect.anchoredPosition = Vector2.zero;
        
        UnityEngine.UI.Image image = panel.AddComponent<UnityEngine.UI.Image>();
        image.color = new Color(0, 0, 0, 0.9f);
        
        return panel;
    }
    
    static GameObject CreateButton(Transform parent, string text, Vector2 position)
    {
        GameObject button = new GameObject(text + " Button");
        button.transform.SetParent(parent, false);
        
        RectTransform rect = button.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(150, 40);
        rect.anchoredPosition = position;
        
        UnityEngine.UI.Button btn = button.AddComponent<UnityEngine.UI.Button>();
        UnityEngine.UI.Image image = button.AddComponent<UnityEngine.UI.Image>();
        btn.targetGraphic = image;
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(button.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        UnityEngine.UI.Text txt = textObj.AddComponent<UnityEngine.UI.Text>();
        txt.text = text;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        
        return button;
    }
    
    static GameObject CreateText(Transform parent, string text, Vector2 position)
    {
        GameObject textObj = new GameObject(text + " Text");
        textObj.transform.SetParent(parent, false);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(380, 30);
        rect.anchoredPosition = position;
        
        UnityEngine.UI.Text txt = textObj.AddComponent<UnityEngine.UI.Text>();
        txt.text = text;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.fontSize = 16;
        
        return textObj;
    }
    
    [MenuItem("Substream/Open Documentation")]
    static void OpenDocumentation()
    {
        Application.OpenURL("https://github.com/jlin3/substreamsdk");
    }
    
    [MenuItem("Substream/View Streams Dashboard")]
    static void OpenStreamsDashboard()
    {
        Application.OpenURL("https://cloud.livekit.io/projects/substream-cnzdthyx/rooms");
    }
}
