using UnityEngine;

public class AnchorTopLeft : MonoBehaviour
{
    public RectTransform healthPanel; // Assign the HealthPanel RectTransform in the Inspector
    public Vector2 offset = new Vector2(10, -10); // Offset from the top-left corner

    void Update()
    {
        // Get screen size
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        // Set position relative to the top-left
        healthPanel.position = new Vector2(offset.x, screenSize.y + offset.y);
    }
}
