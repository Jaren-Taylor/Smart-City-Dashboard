using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HeaderCard : UIElement
{
    public static readonly string prefabAddress = "Prefabs/UI/Cards/HeaderCard";
    protected static GameObject staticPrefab = null;

    public UnityEvent<UIElement> OnRemoveClicked;

    [SerializeField]
    protected Image bkgImage;
    [SerializeField]
    protected TextMeshProUGUI header;
    [SerializeField]
    protected Button closeButton;

    private UIBackgroundSprite spriteEnum;

    public string Header
    {
        get { return header.text; }
        set { header.text = value; }
    }

    public UIBackgroundSprite BackgroundSprite
    {
        get { return spriteEnum; }
        set {
            if (bkgImage.sprite != UIManager.BackgroundSprites[value]) 
            {
                spriteEnum = value;
                bkgImage.sprite = UIManager.BackgroundSprites[value];
            }
        }
    }

    public Material Material
    {
        get { return bkgImage.material; }
        set { bkgImage.material = value; }
    }

    protected override void Start()
    {
        closeButton.onClick.AddListener(RemoveClicked);
        closeButton.onClick.AddListener(Destroy);
        base.Start();
    }

    private void RemoveClicked()
    {
        OnRemoveClicked?.Invoke(this);
    }

    /// </summary>
    /// <param name="parent"></param>
    /// <param name="backgroundColor"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static HeaderCard Spawn(Transform parent, UIBackgroundSprite backgroundSprite, string text)
    {
        HeaderCard simpleCard = CopyPrefabToParent(parent);
        simpleCard.Header = text;
        simpleCard.BackgroundSprite = backgroundSprite;
        return simpleCard;
    }

    public static HeaderCard Spawn(Transform parent, Material backgroundMaterial, string text)
    {
        HeaderCard simpleCard = CopyPrefabToParent(parent);
        simpleCard.Header = text;
        simpleCard.Material = backgroundMaterial;
        return simpleCard;
    }

    private static HeaderCard CopyPrefabToParent(Transform parent)
    {
        if (staticPrefab == null) staticPrefab = Resources.Load<GameObject>(prefabAddress);
        return Instantiate(staticPrefab, parent.position, Quaternion.identity, parent).GetComponent<HeaderCard>();
    }
}
