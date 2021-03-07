using UnityEngine;
using UnityEngine.UI;

public class PanelFactory : UIFactory
{
    public static readonly string prefabAddress = "Prefabs/UI/UI Elements/Panel";

    public static GameObject Spawn(Transform parent, Color color)
    {
        GameObject panel = Spawn(parent, prefabAddress);
        Image img = panel.GetComponent<Image>();
        img.color = color;
        return panel;
    }
}
