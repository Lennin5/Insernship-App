using UnityEngine;
using UnityEngine.UI;

public class ScrollReset : MonoBehaviour
{
    public ScrollRect scrollRect;

    private void ResetScrollPosition()
    {
        // Restablece la posición del Scroll Rect al principio
        scrollRect.normalizedPosition = new Vector2(0f, 1f);
    }
}
