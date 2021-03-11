using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIElement : MonoBehaviour, IPointerDownHandler
{
    public Action<UIElement> OnClick;
    public void DestroyUIElement()
    {
        if(gameObject != null) Destroy(gameObject);
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

    public void OnPointerDown(PointerEventData eventData)
    {
        OnClick?.Invoke(this);
    }
}
