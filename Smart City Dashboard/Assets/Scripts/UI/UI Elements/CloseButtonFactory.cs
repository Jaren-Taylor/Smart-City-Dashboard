using UnityEngine;
using UnityEngine.UI;

public class CloseButtonFactory : UIFactory
{
    public static readonly string prefabAddress = "Prefabs/UI/UI Elements/CloseButton";

    /// <summary>
    /// Spawns a CloseButton prefab with the given transform as the parent. The button will destroy its parent on click, and remove itself from the UIElementManager
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="manager"></param>
    /// <returns></returns>
    public static GameObject Spawn(Transform parent, UIElementManager manager)
    {
        GameObject butt = Spawn(parent, prefabAddress);
        butt.GetComponent<Button>().onClick.AddListener(() => { manager.Remove(parent.gameObject); Destroy(parent.gameObject); });
        return butt;
    }

    /// <summary>
    /// Spawns a CloseButton prefab with the given transform as the parent. The button will destroy its parent on click
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject Spawn(Transform parent)
    {
        GameObject butt = Spawn(parent, prefabAddress);
        butt.GetComponent<Button>().onClick.AddListener(() => Destroy(parent.gameObject));
        return butt;
    }
}
