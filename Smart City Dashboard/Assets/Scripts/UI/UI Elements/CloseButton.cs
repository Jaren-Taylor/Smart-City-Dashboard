using UnityEngine;
using UnityEngine.UI;

public class CloseButton : UIElement
{
    public static readonly string prefabAddress = "Prefabs/UI/UI Elements/CloseButton";

    public static GameObject Spawn(Transform parent)
    {
        return Spawn(parent, prefabAddress);
    }
}
