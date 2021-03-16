using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HeaderCard : UIElement
{
    public UnityEvent<UIElement> OnRemoveClicked;

    private static GameObject staticPrefab = null;
    public static readonly string prefabAddress = "Prefabs/UI/Cards/HeaderCard";

    [SerializeField]
    protected Image bkgImage;
    [SerializeField]
    protected TextMeshProUGUI header;
    [SerializeField]
    protected Button closeButton;

    protected void Start()
    {
        closeButton.onClick.AddListener(RemoveClicked);
        closeButton.onClick.AddListener(DestroyUIElement);
    }

    private void RemoveClicked()
    {
        OnRemoveClicked?.Invoke(this);
    }

    #region Component methods

        public void SetBackgroundSprite(UIBackgroundSprite backgroundSprite)                                               
        {
            if(bkgImage.sprite != UIManager.BackgroundSprites[backgroundSprite]) bkgImage.sprite = UIManager.BackgroundSprites[backgroundSprite];
        }

        public void SetMaterial(Material material)
        {
            bkgImage.material = material;
        }

        public void SetText(string text)
        {
            if(header.text != text) header.text = text;
        }

    #endregion

    /// </summary>
    /// <param name="parent"></param>
    /// <param name="backgroundColor"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static HeaderCard Spawn(Transform parent, UIBackgroundSprite backgroundSprite, string text)
    {
        HeaderCard simpleCard = CopyPrefabToParent(parent);
        simpleCard.SetText(text);
        simpleCard.SetBackgroundSprite(backgroundSprite);
       
        return simpleCard;
    }

    public static HeaderCard Spawn(Transform parent, Material backgroundMaterial, string text)
    {
        HeaderCard simpleCard = CopyPrefabToParent(parent);
        simpleCard.SetText(text);
        simpleCard.SetMaterial(backgroundMaterial);

        return simpleCard;
    }

    private static HeaderCard CopyPrefabToParent(Transform parent)
    {
        if (staticPrefab == null) staticPrefab = Resources.Load<GameObject>(prefabAddress);
        return Instantiate(staticPrefab, parent.position, Quaternion.identity, parent).GetComponent<HeaderCard>();
    }
}
