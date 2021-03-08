using System;
using UnityEngine;
using UnityEngine.Events;

public class UIElement : MonoBehaviour
{
    public Action OnClick;
    public Action<UIElement> OnDestroyElement;

    private void OnMouseDown()
    {
        OnClick?.Invoke();
    }

    private void OnDestroy()
    {
        OnDestroyElement?.Invoke(this);
    }

    /// <summary>
    /// Spawns a prefab with the transform as the parent
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="prefabAddress"></param>
    /// <returns></returns>
    public static GameObject Spawn(Transform parent, string prefabAddress)
    {
        var model = Resources.Load<GameObject>(prefabAddress);
        var uiElementGO = UnityEngine.Object.Instantiate(model, parent.position, Quaternion.identity, parent);
        return uiElementGO;
    }
}
