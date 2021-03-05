using UnityEngine;
using UnityEngine.UI;

public class Panel : UIElement
{
    public static readonly string prefabAddress = "Prefabs/UI/UI Elements/Panel";

    public static GameObject Spawn(Transform parent, Color color)
    {
        GameObject panel = UIElement.Spawn(parent, prefabAddress);
        Image img = panel.GetComponent<Image>();
        img.color = color;
        return panel;
    }
}
