using UnityEngine;
using UnityEngine.UI;

// A still menu looks dead. The texture coordinate moves, not the object.
public class BackgroundScroller : MonoBehaviour
{
    public RawImage targetImage;
    public float screensPerSecond = 0.03f;

    private void Update()
    {
        Rect uvRect = targetImage.uvRect;

        // Wrapped, so the value does not grow until float precision breaks.
        uvRect.y = Mathf.Repeat(uvRect.y + screensPerSecond * Time.deltaTime, 1f);

        targetImage.uvRect = uvRect;
    }
        
}

