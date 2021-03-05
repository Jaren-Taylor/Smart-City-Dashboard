using UnityEngine;

public class UIElement : MonoBehaviour
{
    private float width;
    private float height;
    private RectTransform rectTransform;

    public float Width { get => rectTransform.rect.width; set => rectTransform.sizeDelta = new Vector2(value, height); }
    public float Height { get => rectTransform.rect.height; set => rectTransform.sizeDelta = new Vector2(width, value); }
    public void SizeDelta(float width, float height) => rectTransform.sizeDelta = new Vector2(width, height);

    protected void Start()
    {
        rectTransform = TryGetRectTransform();
    }

    private RectTransform TryGetRectTransform()
    {
        if (gameObject.TryGetComponent(out RectTransform rectTransform))
        {
            return rectTransform;
        }
        else
        {
            throw new System.Exception("A UIElement is expected to have a RectTransform!");
        }
    }

    protected static T Spawn<T>(Transform parent, string prefabAddress) where T : UIElement
    {
        var model = Resources.Load<GameObject>(prefabAddress);
        var uiElementGO = Instantiate(model, parent.position, Quaternion.identity);
        uiElementGO.transform.parent = parent;
        var uiElement = uiElementGO.GetComponent<T>();
        return uiElement;
    }

    protected static GameObject Spawn(Transform parent, string prefabAddress)
    {
        var model = Resources.Load<GameObject>(prefabAddress);
        var uiElementGO = Instantiate(model, parent.position, Quaternion.identity);
        uiElementGO.transform.parent = parent;
        return uiElementGO;
    }
}
