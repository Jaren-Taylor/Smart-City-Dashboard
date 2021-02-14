using UnityEngine;
using UnityEngine.UI;

public class DashboardMenu : Menu
{
    private float targetXPos; // { get; private set; }
    [SerializeField]
    private Image heatmapImage;
    private HeatMap testMap;
    private new void Start()
    {
        base.Start();
        targetXPos = -menuBounds.rect.width;//-gameObject.transform.position.x;
    }

    private void Update()
    {
        // dont try to move if we're at our target position
        if (transform.position.x != targetXPos)
        {
            // Move towards destination portions at a time
            Vector3 newPosition = transform.position;
            newPosition.x += (targetXPos - newPosition.x) / 25;
            transform.position = newPosition;
        }
    }

    // Toggle => showing/hiding the menu
    public override void ToggleMenuHandler()
    {
        // move with respect to the menu's RectTransform width or height
        float xMove = isOnScreen ? -menuBounds.rect.width : menuBounds.rect.width;
        isOnScreen = !isOnScreen;
        // move the menu offscreen
        targetXPos += xMove;
    }

    public void UpdateSpriteFromTexture(Texture2D texture)
    {

        heatmapImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 1));
    }
}
