using UnityEngine;
using UnityEngine.UI;
public class SimpleCard : UIElement // lEsS lInEs Of CoDe = GoOd
{
    public static readonly string prefabAddress = "Prefabs/UI/UI Elements/UIElement"; 

    public static GameObject Spawn(Transform parent, Color backgroundColor, string text)
    {
        GameObject gObject = Spawn(parent, prefabAddress);
        gObject.AddComponent<ContentSizeFitter>();
        InitializePanel(gObject, backgroundColor);
        float textHeight = InitializeHeader(gObject, text);
        Debug.Log(textHeight);
        InitializeCloseButton(gObject);
        SizeDelta(gObject.RectTransform(), parent.RectTransform().rect.width- (UIElementManager.Margin*2), textHeight + (UIElementManager.Margin * 2));
        return gObject;
    }

    public static void SizeDelta(RectTransform transform, float width, float height) => transform.sizeDelta = new Vector2(width, height);

    private static void InitializePanel(GameObject parent, Color backgroundColor)
    {
        GameObject panel = Panel.Spawn(parent.transform, backgroundColor);
        panel.GetComponent<Image>().color = backgroundColor;
        ((RectTransform)panel.transform).anchoredPosition = new Vector2(0, 0);
    }

    private static float InitializeHeader(GameObject parent, string text)
    {
        GameObject header = Header.Spawn(parent.transform, text);
        RectTransform rectTransform = header.RectTransform();
        rectTransform.anchorMin = new Vector2(0, 0.5f);
        rectTransform.anchorMax = new Vector2(0, 0.5f);
        rectTransform.pivot = new Vector2(0, 0.5f);
        rectTransform.anchoredPosition = new Vector2(UIElementManager.Margin, 0);
        return rectTransform.rect.height;
    }

    private static void InitializeCloseButton(GameObject parent)
    {
        GameObject butt = CloseButton.Spawn(parent.transform);
        butt.GetComponent<Button>().onClick.AddListener(() => Destroy(parent));
        RectTransform rect = (RectTransform)butt.transform;
        rect.anchoredPosition = new Vector2(-UIElementManager.Margin, 0);
    }
}
