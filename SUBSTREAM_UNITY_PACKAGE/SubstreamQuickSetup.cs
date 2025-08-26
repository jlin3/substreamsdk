using UnityEngine;
using UnityEngine.UI;
using Substream;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Quick setup helper for Substream SDK
/// Creates a complete streaming UI in one click!
/// </summary>
public class SubstreamQuickSetup : MonoBehaviour
{
    #if UNITY_EDITOR
    [MenuItem("GameObject/Substream/Create Streaming UI", false, 10)]
    static void CreateStreamingUI()
    {
        // Create canvas if needed
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // Create panel
        GameObject panel = new GameObject("Streaming Panel");
        panel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);
        panelRect.pivot = new Vector2(0, 1);
        panelRect.anchoredPosition = new Vector2(20, -20);
        panelRect.sizeDelta = new Vector2(300, 150);
        
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);

        // Create start button
        GameObject startBtn = new GameObject("Start Button");
        startBtn.transform.SetParent(panel.transform, false);
        RectTransform startRect = startBtn.AddComponent<RectTransform>();
        startRect.anchorMin = new Vector2(0.5f, 0.5f);
        startRect.anchorMax = new Vector2(0.5f, 0.5f);
        startRect.anchoredPosition = new Vector2(-60, 20);
        startRect.sizeDelta = new Vector2(100, 40);
        
        Button startButton = startBtn.AddComponent<Button>();
        Image startImage = startBtn.AddComponent<Image>();
        startButton.targetGraphic = startImage;
        
        GameObject startText = new GameObject("Text");
        startText.transform.SetParent(startBtn.transform, false);
        Text startTxt = startText.AddComponent<Text>();
        startTxt.text = "Start Stream";
        startTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        startTxt.alignment = TextAnchor.MiddleCenter;
        startTxt.color = Color.white;
        RectTransform startTxtRect = startText.GetComponent<RectTransform>();
        startTxtRect.anchorMin = Vector2.zero;
        startTxtRect.anchorMax = Vector2.one;
        startTxtRect.sizeDelta = Vector2.zero;
        startTxtRect.offsetMin = Vector2.zero;
        startTxtRect.offsetMax = Vector2.zero;

        // Create stop button
        GameObject stopBtn = new GameObject("Stop Button");
        stopBtn.transform.SetParent(panel.transform, false);
        RectTransform stopRect = stopBtn.AddComponent<RectTransform>();
        stopRect.anchorMin = new Vector2(0.5f, 0.5f);
        stopRect.anchorMax = new Vector2(0.5f, 0.5f);
        stopRect.anchoredPosition = new Vector2(60, 20);
        stopRect.sizeDelta = new Vector2(100, 40);
        
        Button stopButton = stopBtn.AddComponent<Button>();
        Image stopImage = stopBtn.AddComponent<Image>();
        stopButton.targetGraphic = stopImage;
        stopImage.color = new Color(1f, 0.3f, 0.3f);
        
        GameObject stopText = new GameObject("Text");
        stopText.transform.SetParent(stopBtn.transform, false);
        Text stopTxt = stopText.AddComponent<Text>();
        stopTxt.text = "Stop Stream";
        stopTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        stopTxt.alignment = TextAnchor.MiddleCenter;
        stopTxt.color = Color.white;
        RectTransform stopTxtRect = stopText.GetComponent<RectTransform>();
        stopTxtRect.anchorMin = Vector2.zero;
        stopTxtRect.anchorMax = Vector2.one;
        stopTxtRect.sizeDelta = Vector2.zero;
        stopTxtRect.offsetMin = Vector2.zero;
        stopTxtRect.offsetMax = Vector2.zero;
        
        stopBtn.SetActive(false);

        // Create status text
        GameObject statusGO = new GameObject("Status Text");
        statusGO.transform.SetParent(panel.transform, false);
        RectTransform statusRect = statusGO.AddComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0.5f, 0.5f);
        statusRect.anchorMax = new Vector2(0.5f, 0.5f);
        statusRect.anchoredPosition = new Vector2(0, -20);
        statusRect.sizeDelta = new Vector2(280, 30);
        
        Text statusText = statusGO.AddComponent<Text>();
        statusText.text = "Ready to stream";
        statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        statusText.alignment = TextAnchor.MiddleCenter;
        statusText.color = Color.white;
        statusText.fontSize = 14;

        // Create viewer link text
        GameObject linkGO = new GameObject("Viewer Link");
        linkGO.transform.SetParent(panel.transform, false);
        RectTransform linkRect = linkGO.AddComponent<RectTransform>();
        linkRect.anchorMin = new Vector2(0.5f, 0);
        linkRect.anchorMax = new Vector2(0.5f, 0);
        linkRect.anchoredPosition = new Vector2(0, 10);
        linkRect.sizeDelta = new Vector2(280, 20);
        
        Text linkText = linkGO.AddComponent<Text>();
        linkText.text = "";
        linkText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        linkText.alignment = TextAnchor.MiddleCenter;
        linkText.color = Color.cyan;
        linkText.fontSize = 12;
        linkGO.SetActive(false);

        // Add SubstreamUI component
        SubstreamUI ui = panel.AddComponent<SubstreamUI>();
        ui.startButton = startButton;
        ui.stopButton = stopButton;
        ui.statusText = statusText;
        ui.viewerLinkText = linkText;

        Debug.Log("[Substream] Streaming UI created! Press Play to test.");
        Selection.activeGameObject = panel;
    }

    [MenuItem("Substream/Open Viewer Dashboard")]
    static void OpenViewerDashboard()
    {
        Application.OpenURL("https://cloud.livekit.io/projects/substream-cnzdthyx/rooms");
    }

    [MenuItem("Substream/Documentation")]
    static void OpenDocumentation()
    {
        Application.OpenURL("https://github.com/jlin3/substreamsdk");
    }
    #endif
}
