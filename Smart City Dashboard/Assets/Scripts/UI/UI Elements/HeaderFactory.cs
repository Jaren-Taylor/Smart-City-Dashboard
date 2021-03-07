using TMPro;
using UnityEngine;

public class HeaderFactory : UIFactory
{
    public static readonly string prefabAddress = "Prefabs/UI/UI Elements/Header";
    public static readonly int height = 20;
    public static readonly int charWidth = 12;

    /// <summary>
    /// Spawns a Header prefab with the given text to display and transform as the parent
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public new static GameObject Spawn(Transform parent, string text) 
    {
        GameObject header = UIFactory.Spawn(parent, prefabAddress);
        TextMeshProUGUI TMProText = header.GetComponent<TextMeshProUGUI>();
        TMProText.text = text;
        return header;
    }
}
