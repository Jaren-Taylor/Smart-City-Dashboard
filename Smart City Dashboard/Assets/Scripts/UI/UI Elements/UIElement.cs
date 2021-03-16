using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIElement : MonoBehaviour
{
    public UnityEvent<UIElement> OnClick;
    private Button button;

    protected virtual void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Click);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void Click()
    {
        OnClick?.Invoke(this);
    }

    /// <summary>
    /// Spawns a prefab with the transform as the parent
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="prefabAddress"></param>
    /// <returns></returns>
    public static UIElement Spawn(Transform parent, string prefabAddress)
    {
        var model = Resources.Load<GameObject>(prefabAddress);
        return Instantiate(model, parent.position, Quaternion.identity, parent).GetComponent<UIElement>();
    }

    public static UIElement Spawn(Transform parent, UIElement original)
    {
        return Instantiate(original.gameObject, parent.position, Quaternion.identity, parent).GetComponent<UIElement>();
    }
}
