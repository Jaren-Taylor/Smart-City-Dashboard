using UnityEngine;
using UnityEngine.UI;

public class PanelFactory : UIFactory
{
    public static readonly string prefabAddress = "Prefabs/UI/UI Elements/Panel";

    /// <summary>
    /// Spawns a Panel prefab with the given color to display and transform as the parent
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static GameObject Spawn(Transform parent, Color color)
    {
        GameObject panel = Spawn(parent, prefabAddress);
        Image img = panel.GetComponent<Image>();
        img.color = color;
        return panel;
    }
}
