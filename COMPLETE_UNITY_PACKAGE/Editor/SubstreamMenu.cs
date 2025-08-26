using UnityEngine;
using UnityEditor;

public class SubstreamMenu : MonoBehaviour
{
    [MenuItem("GameObject/Substream/Create Demo Scene with UI", false, 10)]
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
        GameObject demoController = new GameObject("Substream Demo Controller");
        SimpleDemoScript demo = demoController.AddComponent<SimpleDemoScript>();
        
        // Create main UI Panel
        GameObject panel = CreateUIPanel(canvas.transform);
        
        // Create title
        var title = CreateText(panel.transform, "SUBSTREAM LIVE DEMO", new Vector2(0, 100), 24, true);
        title.GetComponent<UnityEngine.UI.Text>().color = Color.white;
        
        // Create buttons
        var startButton = CreateButton(panel.transform, "🎮 START STREAMING", new Vector2(-120, 40), Color.green);
        var stopButton = CreateButton(panel.transform, "⏹️ STOP STREAMING", new Vector2(120, 40), new Color(1f, 0.3f, 0.3f));
        
        // Create status text
        var statusText = CreateText(panel.transform, "Ready to stream", new Vector2(0, 0), 18);
        
        // Create viewer panel (prominent display)
        GameObject viewerPanel = CreateViewerPanel(panel.transform);
        var viewerText = CreateText(viewerPanel.transform, "🔗 Click to View Stream", new Vector2(0, 10), 20, true);
        viewerText.GetComponent<UnityEngine.UI.Text>().color = Color.cyan;
        
        // Create clickable viewer button
        var viewerButton = CreateButton(viewerPanel.transform, "📺 OPEN VIEWER", new Vector2(0, -30), Color.cyan);
        viewerButton.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 50);
        
        // Connect to demo script
        demo.startButton = startButton.GetComponent<UnityEngine.UI.Button>();
        demo.stopButton = stopButton.GetComponent<UnityEngine.UI.Button>();
        demo.statusText = statusText.GetComponent<UnityEngine.UI.Text>();
        demo.viewerLinkText = viewerText.GetComponent<UnityEngine.UI.Text>();
        demo.viewerLinkButton = viewerButton.GetComponent<UnityEngine.UI.Button>();
        demo.viewerPanel = viewerPanel;
        
        // Configure default streaming settings
        demo.streamWidth = 1920;
        demo.streamHeight = 1080;
        demo.streamFPS = 30;
        demo.bitrateKbps = 3500;
        demo.includeAudio = true;
        
        // Hide viewer panel initially
        viewerPanel.SetActive(false);
        
        // Add instructions panel
        CreateInstructionsPanel(canvas.transform);
        
        Debug.Log("[Substream] ✅ Demo scene created!");
        Debug.Log("[Substream] Press Play → Click 'START STREAMING' → Click 'OPEN VIEWER' to watch");
        Selection.activeGameObject = demoController;
    }
    
    static GameObject CreateUIPanel(Transform parent)
    {
        GameObject panel = new GameObject("Stream Control Panel");
        panel.transform.SetParent(parent, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(500, 300);
        rect.anchoredPosition = Vector2.zero;
        
        UnityEngine.UI.Image image = panel.AddComponent<UnityEngine.UI.Image>();
        image.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        
        return panel;
    }
    
    static GameObject CreateViewerPanel(Transform parent)
    {
        GameObject panel = new GameObject("Viewer Link Panel");
        panel.transform.SetParent(parent, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0);
        rect.anchorMax = new Vector2(0.5f, 0);
        rect.sizeDelta = new Vector2(400, 100);
        rect.anchoredPosition = new Vector2(0, -80);
        
        UnityEngine.UI.Image image = panel.AddComponent<UnityEngine.UI.Image>();
        image.color = new Color(0, 0.5f, 1f, 0.3f); // Light blue background
        
        return panel;
    }
    
    static GameObject CreateInstructionsPanel(Transform parent)
    {
        GameObject panel = new GameObject("Instructions Panel");
        panel.transform.SetParent(parent, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.sizeDelta = new Vector2(300, 150);
        rect.anchoredPosition = new Vector2(-160, -80);
        rect.pivot = new Vector2(1, 1);
        
        UnityEngine.UI.Image image = panel.AddComponent<UnityEngine.UI.Image>();
        image.color = new Color(0, 0, 0, 0.8f);
        
        var instructions = CreateText(panel.transform, 
            "KEYBOARD SHORTCUTS:\n" +
            "S - Start Streaming\n" +
            "X - Stop Streaming\n" +
            "V - Open Viewer\n\n" +
            "Stream appears in LiveKit dashboard",
            new Vector2(0, 0), 14);
        
        instructions.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 130);
        instructions.GetComponent<UnityEngine.UI.Text>().alignment = TextAnchor.MiddleCenter;
        
        return panel;
    }
    
    static GameObject CreateButton(Transform parent, string text, Vector2 position, Color color = default)
    {
        GameObject button = new GameObject(text + " Button");
        button.transform.SetParent(parent, false);
        
        RectTransform rect = button.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(180, 50);
        rect.anchoredPosition = position;
        
        UnityEngine.UI.Button btn = button.AddComponent<UnityEngine.UI.Button>();
        UnityEngine.UI.Image image = button.AddComponent<UnityEngine.UI.Image>();
        btn.targetGraphic = image;
        
        if (color != default)
            image.color = color;
        
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
        txt.fontSize = 16;
        txt.fontStyle = FontStyle.Bold;
        
        return button;
    }
    
    static GameObject CreateText(Transform parent, string text, Vector2 position, int fontSize = 16, bool bold = false)
    {
        GameObject textObj = new GameObject(text.Split('\n')[0] + " Text");
        textObj.transform.SetParent(parent, false);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(480, 40);
        rect.anchoredPosition = position;
        
        UnityEngine.UI.Text txt = textObj.AddComponent<UnityEngine.UI.Text>();
        txt.text = text;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.fontSize = fontSize;
        if (bold) txt.fontStyle = FontStyle.Bold;
        
        return textObj;
    }
    
    [MenuItem("Substream/Import Test Scene Package")]
    static void ImportTestScene()
    {
        string packagePath = "Assets/../test-scene.unitypackage";
        if (System.IO.File.Exists(packagePath))
        {
            AssetDatabase.ImportPackage(packagePath, true);
            Debug.Log("[Substream] Importing test-scene.unitypackage...");
        }
        else
        {
            Debug.LogError("[Substream] test-scene.unitypackage not found in project root!");
        }
    }
    
    [MenuItem("Substream/Open LiveKit Viewer Dashboard")]
    static void OpenStreamsDashboard()
    {
        Application.OpenURL("https://cloud.livekit.io/projects/substream-cnzdthyx/rooms");
        Debug.Log("[Substream] Opening LiveKit dashboard - look for your room and click 'Join' to view");
    }
    
    [MenuItem("Substream/Documentation")]
    static void OpenDocumentation()
    {
        Application.OpenURL("https://github.com/jlin3/substreamsdk");
    }
}