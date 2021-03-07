using UnityEngine;
using UnityEngine.UI;
public class SimpleCard : UIFactory // lEsS lInEs Of CoDe = GoOd
{
    public static readonly string prefabAddress = "Prefabs/UI/UI Elements/UIElement";

    /// <summary>
    /// Spawn a SimpleCard with a UI element manager as the parent
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="backgroundColor"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static GameObject Spawn(UIElementManager manager, Color backgroundColor, string text)
    {
        GameObject gObject = Spawn(manager.transform, prefabAddress);
        float textHeight = InitializeStaticComponents(gObject, backgroundColor, text);
        InitializeCloseButton(gObject, manager);
        SizeDelta(gObject.RectTransform(), manager.gameObject.RectTransform().rect.width - (UIElementManager.Margin * 2), textHeight + (UIElementManager.Margin * 2));
        manager.Add(gObject);
        return gObject;
    }

    /// <summary>
    /// Spawn a SimpleCard with a generic GameObject as the parent
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="backgroundColor"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static GameObject Spawn(Transform parent, Color backgroundColor, string text)
    {
        GameObject gObject = Spawn(parent, prefabAddress);
        float textHeight = InitializeStaticComponents(gObject, backgroundColor, text);
        InitializeCloseButton(gObject);
        SizeDelta(gObject.RectTransform(), parent.RectTransform().rect.width - (UIElementManager.Margin * 2), textHeight + (UIElementManager.Margin * 2));
        return gObject;
    }

    /// <summary>
    /// Initalizes the ContentSizeFitter, Panel, and Header. The closing button is static, but can be initialized in more than 1 way. Refer to InitializeCloseButton().
    /// </summary>
    /// <param name="gObject"></param>
    /// <param name="backgroundColor"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    private static float InitializeStaticComponents(GameObject gObject, Color backgroundColor, string text)
    {
        //gObject.AddComponent<ContentSizeFitter>();
        InitializePanel(gObject, backgroundColor);
        float textHeight = InitializeHeader(gObject, text);
        return textHeight;
    }

    /// <summary>
    /// Changes the size of the parent object that all UI elements inherit from
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    private static void SizeDelta(RectTransform transform, float width, float height) => transform.sizeDelta = new Vector2(width, height);

    /// <summary>
    /// Initializes the background panel's position
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="backgroundColor"></param>
    private static void InitializePanel(GameObject parent, Color backgroundColor)
    {
        GameObject panel = PanelFactory.Spawn(parent.transform, backgroundColor);
        panel.GetComponent<Image>().color = backgroundColor;
        ((RectTransform)panel.transform).anchoredPosition = new Vector2(0, 0);
    }

    /// <summary>
    /// Initializes the header position
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    private static float InitializeHeader(GameObject parent, string text)
    {
        GameObject header = HeaderFactory.Spawn(parent.transform, text);
        RectTransform rectTransform = header.RectTransform();
        rectTransform.anchorMin = new Vector2(0, 0.5f);
        rectTransform.anchorMax = new Vector2(0, 0.5f);
        rectTransform.pivot = new Vector2(0, 0.5f);
        rectTransform.anchoredPosition = new Vector2(UIElementManager.Margin, 0);
        return rectTransform.rect.height;
    }

    /// <summary>
    /// Initializes the closing buttons position and function with a generic GameObject
    /// </summary>
    /// <param name="parent"></param>
    private static void InitializeCloseButton(GameObject parent)
    {
        GameObject butt = CloseButtonFactory.Spawn(parent.transform);
        PositionCloseButton(butt.RectTransform());
    }

    /// <summary>
    /// Initializes the closing buttons position and function with a UI element manager in mind
    /// </summary>
    /// <param name="manager"></param>
    private static void InitializeCloseButton(GameObject parent, UIElementManager manager)
    {
        GameObject butt = CloseButtonFactory.Spawn(parent.transform, manager);
        PositionCloseButton(butt.RectTransform());
    }

    /// <summary>
    /// Simply a helper for InitializeCloseButton. It can be overloaded, but this functionality is intended to stay consistent
    /// </summary>
    private static void PositionCloseButton(RectTransform rectTransform)
    {
        rectTransform.anchoredPosition = new Vector2(-UIElementManager.Margin, 0);
    }
}
