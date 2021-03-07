using System;
using UnityEngine;

public class UIFactory : MonoBehaviour
{
    /// <summary>
    /// Spawns a prefab with the transform as the parent
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="prefabAddress"></param>
    /// <returns></returns>
    protected static GameObject Spawn(Transform parent, string prefabAddress)
    {
        var model = Resources.Load<GameObject>(prefabAddress);
        var uiElementGO = Instantiate(model, parent.position, Quaternion.identity);
        uiElementGO.transform.SetParent(parent);
        return uiElementGO;
    }
}
