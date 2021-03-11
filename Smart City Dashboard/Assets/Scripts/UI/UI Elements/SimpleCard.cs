using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class SimpleCard : UIElement
{
    public UnityEvent<UIElement> OnRemoveClicked;

    private static GameObject simpleCardPrefab = null;

    public static readonly string prefabAddress = "Prefabs/UI/UI Elements/SimpleCard";

    [SerializeField]
    private Image bkgImage;
    [SerializeField]
    private Header header;
    [SerializeField]
    private GameObject closeButton;

    private void RemoveClicked()
    {
        OnRemoveClicked?.Invoke(this);
    }

    public void SetColor(Color color)
    {
        if(bkgImage.color != color) bkgImage.color = color;
    }

    public void SetMaterial(Material material) => bkgImage.material = material;

    public void SetText(string text)
    {
        if(header.TMPText.text != text) header.TMPText.text = text;
    }

    /// <summary>
    /// Spawn a SimpleCard with a generic GameObject as the parent
    /// <summary>
    /// Changes the size of the parent object that all UI elements inherit from
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    private void SizeDelta(RectTransform transform, float width, float height) => transform.sizeDelta = new Vector2(width, height);

    /// </summary>
    /// <param name="parent"></param>
    /// <param name="backgroundColor"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static SimpleCard Spawn(Transform parent, Color backgroundColor, string text)
    {
        SimpleCard simpleCard = CopyPrefabToParent(parent);
        simpleCard.SetText(text);
        simpleCard.SetColor(backgroundColor);
       
        return simpleCard;
    }

    public static SimpleCard Spawn(Transform parent, Material backgroundMaterial, string text)
    {
        SimpleCard simpleCard = CopyPrefabToParent(parent);
        simpleCard.SetText(text);
        simpleCard.SetMaterial(backgroundMaterial);

        return simpleCard;
    }

    private static SimpleCard CopyPrefabToParent(Transform parent)
    {
        if (simpleCardPrefab == null) simpleCardPrefab = Resources.Load<GameObject>(prefabAddress);
        return Instantiate(simpleCardPrefab, parent.position, Quaternion.identity, parent).GetComponent<SimpleCard>();
    }

    private void Start()
    {
        closeButton.GetComponent<Button>().onClick.AddListener(RemoveClicked);
    }

}
