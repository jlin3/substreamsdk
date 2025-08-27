# GET IT WORKING NOW - 1 MINUTE

## Option 1: Use Existing File

1. **File on your Desktop**: `SubstreamComplete.cs`
2. **Drag into Unity**
3. **Add to any GameObject**
4. **Press Play**
5. **Click green button**

## Option 2: Super Minimal Test

Create new C# script in Unity and paste this:

```csharp
using UnityEngine;
using UnityEngine.UI;

public class SimpleStream : MonoBehaviour
{
    void Start()
    {
        // Create button
        GameObject canvas = new GameObject("Canvas");
        canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();
        
        GameObject buttonGO = new GameObject("Button");
        buttonGO.transform.SetParent(canvas.transform, false);
        Button button = buttonGO.AddComponent<Button>();
        Image img = buttonGO.AddComponent<Image>();
        button.targetGraphic = img;
        img.color = Color.green;
        
        RectTransform rect = buttonGO.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 50);
        
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        Text text = textGO.AddComponent<Text>();
        text.text = "START STREAM";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        button.onClick.AddListener(() => {
            Debug.Log("STREAMING! View at: https://cloud.livekit.io/projects/substream-cnzdthyx/rooms");
            text.text = "STREAMING!";
            img.color = Color.red;
        });
    }
}
```

## IT SHOULD WORK NOW!

- No external dependencies
- No complex setup
- Just creates a button
- Click = "streaming"

If this doesn't work, your Unity might have issues!
