using System;
using UnityEngine;

public class UIFactory : MonoBehaviour
{
    protected static GameObject Spawn(Transform parent, string prefabAddress)
    {
        var model = Resources.Load<GameObject>(prefabAddress);
        var uiElementGO = Instantiate(model, parent.position, Quaternion.identity);
        uiElementGO.transform.SetParent(parent);
        return uiElementGO;
    }
}
