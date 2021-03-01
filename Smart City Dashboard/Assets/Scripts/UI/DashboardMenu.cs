using UnityEngine;
using UnityEngine.UI;

public class DashboardMenu : Menu
{
    [SerializeField]
    private Image heatmapImage;

    public void UpdateSpriteFromTexture(Texture2D texture)
    {
        heatmapImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 1));
    }
}
