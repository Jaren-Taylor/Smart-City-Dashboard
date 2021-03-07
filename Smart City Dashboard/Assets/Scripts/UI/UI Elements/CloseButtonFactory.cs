using UnityEngine;
using UnityEngine.UI;

public class CloseButtonFactory : UIFactory
{
    public static readonly string prefabAddress = "Prefabs/UI/UI Elements/CloseButton";

    public static GameObject Spawn(Transform parent, UIElementManager manager)
    {
        GameObject butt = Spawn(parent, prefabAddress);
        butt.GetComponent<Button>().onClick.AddListener(() => { manager.Remove(parent.gameObject); Destroy(parent.gameObject); });
        return butt;
    }

    public static GameObject Spawn(Transform parent)
    {
        GameObject butt = Spawn(parent, prefabAddress);
        butt.GetComponent<Button>().onClick.AddListener(() => Destroy(parent.gameObject));
        return butt;
    }
}
