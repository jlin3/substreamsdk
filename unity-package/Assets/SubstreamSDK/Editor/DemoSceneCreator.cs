using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using SubstreamSDK;

namespace SubstreamSDK.Editor
{
    public class DemoSceneCreator : EditorWindow
    {
        [MenuItem("Substream/Create Demo Scene")]
        public static void CreateDemoScene()
        {
            // Create new scene
            UnityEditor.SceneManagement.EditorSceneManager.NewScene(
                UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects, 
                UnityEditor.SceneManagement.NewSceneMode.Single
            );
            
            // Set up camera for VR
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.2f);
                mainCamera.fieldOfView = 90f;
            }
            
            // Create UI Canvas
            GameObject canvasGO = new GameObject("UI Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // Create UI Panel
            GameObject panelGO = new GameObject("Control Panel");
            panelGO.transform.SetParent(canvasGO.transform, false);
            RectTransform panelRect = panelGO.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(0, 1);
            panelRect.pivot = new Vector2(0, 1);
            panelRect.anchoredPosition = new Vector2(20, -20);
            panelRect.sizeDelta = new Vector2(400, 300);
            
            Image panelImage = panelGO.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.8f);
            
            // Add vertical layout
            VerticalLayoutGroup layout = panelGO.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(20, 20, 20, 20);
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            
            // Add Title
            GameObject titleGO = new GameObject("Title");
            titleGO.transform.SetParent(panelGO.transform, false);
            Text titleText = titleGO.AddComponent<Text>();
            titleText.text = "Substream SDK Demo";
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 24;
            titleText.fontStyle = FontStyle.Bold;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform titleRect = titleGO.GetComponent<RectTransform>();
            titleRect.preferredHeight = 40;
            
            // Add Status Text
            GameObject statusGO = new GameObject("Status Text");
            statusGO.transform.SetParent(panelGO.transform, false);
            Text statusText = statusGO.AddComponent<Text>();
            statusText.text = "Ready to stream";
            statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            statusText.fontSize = 18;
            statusText.color = Color.green;
            statusText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform statusRect = statusGO.GetComponent<RectTransform>();
            statusRect.preferredHeight = 30;
            
            // Add Go Live Button
            GameObject goLiveButtonGO = new GameObject("Go Live Button");
            goLiveButtonGO.transform.SetParent(panelGO.transform, false);
            Button goLiveButton = goLiveButtonGO.AddComponent<Button>();
            Image goLiveImage = goLiveButtonGO.AddComponent<Image>();
            goLiveButton.targetGraphic = goLiveImage;
            
            RectTransform goLiveRect = goLiveButtonGO.GetComponent<RectTransform>();
            goLiveRect.preferredHeight = 50;
            
            GameObject goLiveTextGO = new GameObject("Text");
            goLiveTextGO.transform.SetParent(goLiveButtonGO.transform, false);
            Text goLiveText = goLiveTextGO.AddComponent<Text>();
            goLiveText.text = "🔴 Go Live";
            goLiveText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            goLiveText.fontSize = 20;
            goLiveText.color = Color.white;
            goLiveText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform goLiveTextRect = goLiveTextGO.GetComponent<RectTransform>();
            goLiveTextRect.anchorMin = Vector2.zero;
            goLiveTextRect.anchorMax = Vector2.one;
            goLiveTextRect.sizeDelta = Vector2.zero;
            goLiveTextRect.anchoredPosition = Vector2.zero;
            
            // Add Stop Button
            GameObject stopButtonGO = new GameObject("Stop Button");
            stopButtonGO.transform.SetParent(panelGO.transform, false);
            Button stopButton = stopButtonGO.AddComponent<Button>();
            Image stopImage = stopButtonGO.AddComponent<Image>();
            stopButton.targetGraphic = stopImage;
            stopButton.interactable = false;
            
            RectTransform stopRect = stopButtonGO.GetComponent<RectTransform>();
            stopRect.preferredHeight = 50;
            
            GameObject stopTextGO = new GameObject("Text");
            stopTextGO.transform.SetParent(stopButtonGO.transform, false);
            Text stopText = stopTextGO.AddComponent<Text>();
            stopText.text = "⬛ Stop Streaming";
            stopText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            stopText.fontSize = 20;
            stopText.color = Color.white;
            stopText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform stopTextRect = stopTextGO.GetComponent<RectTransform>();
            stopTextRect.anchorMin = Vector2.zero;
            stopTextRect.anchorMax = Vector2.one;
            stopTextRect.sizeDelta = Vector2.zero;
            stopTextRect.anchoredPosition = Vector2.zero;
            
            // Add Streaming Indicator
            GameObject indicatorGO = new GameObject("Streaming Indicator");
            indicatorGO.transform.SetParent(canvasGO.transform, false);
            RectTransform indicatorRect = indicatorGO.AddComponent<RectTransform>();
            indicatorRect.anchorMin = new Vector2(1, 1);
            indicatorRect.anchorMax = new Vector2(1, 1);
            indicatorRect.pivot = new Vector2(1, 1);
            indicatorRect.anchoredPosition = new Vector2(-20, -20);
            indicatorRect.sizeDelta = new Vector2(200, 50);
            
            Text indicatorText = indicatorGO.AddComponent<Text>();
            indicatorText.text = "🔴 LIVE";
            indicatorText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            indicatorText.fontSize = 32;
            indicatorText.fontStyle = FontStyle.Bold;
            indicatorText.color = Color.red;
            indicatorText.alignment = TextAnchor.MiddleCenter;
            indicatorGO.SetActive(false);
            
            // Create Demo Controller GameObject
            GameObject controllerGO = new GameObject("Substream Controller");
            DemoController controller = controllerGO.AddComponent<DemoController>();
            
            // Wire up the UI
            controller.GoLiveButton = goLiveButton;
            controller.StopButton = stopButton;
            controller.StatusText = statusText;
            controller.StreamingIndicator = indicatorGO;
            
            // Set default values
            controller.BaseUrl = "demo";
            controller.Width = 1920;
            controller.Height = 1080;
            controller.Fps = Application.platform == RuntimePlatform.Android ? 72 : 60;
            controller.BitrateKbps = 5000;
            
            // Create a simple 3D object to demonstrate streaming
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Demo Cube";
            cube.transform.position = new Vector3(0, 0, 3);
            
            // Add rotation script to cube
            RotateObject rotator = cube.AddComponent<RotateObject>();
            
            // Select the controller in the hierarchy
            Selection.activeGameObject = controllerGO;
            
            Debug.Log("✅ Substream demo scene created successfully!");
            Debug.Log("ℹ️ Press Play and click 'Go Live' to start streaming");
            Debug.Log("ℹ️ For Quest: Build and deploy to device");
        }
        
        [MenuItem("Substream/Documentation")]
        public static void OpenDocumentation()
        {
            Application.OpenURL("https://github.com/yourusername/substreamsdk");
        }
        
        [MenuItem("Substream/About")]
        public static void ShowAbout()
        {
            EditorUtility.DisplayDialog(
                "Substream SDK", 
                "Version 1.0.0\n\n" +
                "Stream your Unity game in one line of code!\n\n" +
                "© 2024 Substream", 
                "OK"
            );
        }
    }
    
    // Simple rotation script for demo object
    public class RotateObject : MonoBehaviour
    {
        public float speed = 50f;
        
        void Update()
        {
            transform.Rotate(Vector3.up * speed * Time.deltaTime);
            transform.Rotate(Vector3.right * speed * 0.3f * Time.deltaTime);
        }
    }
}
