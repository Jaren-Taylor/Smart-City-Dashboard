using TMPro;
using UnityEngine;

public class Header : UIElement
{
    public static readonly string prefabAddress = "Prefabs/UI/UI Elements/Header";
    public static readonly int height = 20;
    public static readonly int charWidth = 12;

    public new static GameObject Spawn(Transform parent, string text) 
    {
        GameObject header = UIElement.Spawn(parent, prefabAddress);
        TextMeshProUGUI TMProText = header.GetComponent<TextMeshProUGUI>();
        TMProText.text = text;
        return header;
    }
}
