using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class SimpleCard : UIElement
{
    public UnityEvent<UIElement> OnRemoveClicked;

    public static readonly string prefabAddress = "Prefabs/UI/UI Elements/SimpleCard";

    //public Button CloseButton;

    [SerializeField]
    private Header header;
    [SerializeField]
    private GameObject closeButton;

    private void RemoveClicked()
    {
        OnRemoveClicked?.Invoke(this);
    }

    public void SetText(string text) => header.TMPText.text = text;

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
        SimpleCard simpleCard = Spawn(parent, prefabAddress).GetComponent<SimpleCard>();
        simpleCard.SetText(text);
        simpleCard.GetComponent<Image>().color = backgroundColor;
       
        return simpleCard;
    }

    private void Start()
    {
        closeButton.GetComponent<Button>().onClick.AddListener(RemoveClicked);
    }

}
