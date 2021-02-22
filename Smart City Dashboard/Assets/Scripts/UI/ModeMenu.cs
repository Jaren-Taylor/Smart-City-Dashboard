using UnityEngine;
using UnityEngine.UI;

public class ModeMenu : Menu
{
    private float targetYPos; // { get; private set; }

    private new void Start()
    {
        base.Start();
        targetYPos = -menuBounds.rect.height; //-gameObject.transform.position.y;
    }

    private void Update()
    {
        // dont try to move if we're at our target position
        if (transform.position.y != targetYPos)
        {
            // Move towards destination portions at a time
            Vector3 newPosition = transform.position;
            newPosition.y += (targetYPos - newPosition.y) / 25;
            transform.position = newPosition;
        }
    }

    // Toggle => showing/hiding the menu
    public override void ToggleMenuHandler()
    {
        // move with respect to the menu's RectTransform width or height
        float yMove = isOnScreen ? -menuBounds.rect.height : menuBounds.rect.height;
        isOnScreen = !isOnScreen;
        // move the menu offscreen
        targetYPos += yMove;
    }
}
