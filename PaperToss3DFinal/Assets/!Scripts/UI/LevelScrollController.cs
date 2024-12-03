using UnityEngine;
using UnityEngine.UI;

public class LevelScrollController : MonoBehaviour
{
    public RectTransform content; // The RectTransform of the LevelProgression
    public ScrollRect scrollRect;

    public float upperLimit = 0f; // The maximum Y position (topmost)
    public float lowerLimit = -1000f; // The minimum Y position (bottommost)

    void Update()
    {
        // Get the current anchored position of the content
        Vector2 contentPos = content.anchoredPosition;

        // Clamp the Y position to the limits
        contentPos.y = Mathf.Clamp(contentPos.y, lowerLimit, upperLimit);

        // Apply the clamped position back to the content
        content.anchoredPosition = contentPos;
    }
}
