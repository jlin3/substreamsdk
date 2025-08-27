using UnityEngine;

public class QuickTest : MonoBehaviour
{
    void Start()
    {
        gameObject.AddComponent<SubstreamComplete>();
        Debug.Log("Streaming UI created! Look at Game view and press the button!");
    }
}
