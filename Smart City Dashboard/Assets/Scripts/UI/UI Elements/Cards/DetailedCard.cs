using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DetailedCard : SimpleCard
{
    public static new readonly string prefabAddress = "Prefabs/UI/UI Elements/Cards/DetailedCard";
    [HideInInspector]
    public Button button;

    private static GameObject staticPrefab = null;
    private Dictionary<string, NameValuePair> items = new Dictionary<string, NameValuePair>();
    [SerializeField]
    private GameObject textArea;
    [SerializeField]
    private GameObject NameValuePairPrefab;

    private new void Start()
    {
        base.Start();
        button.onClick.AddListener(Click);
    }

    public void Click()
    {
        OnClick?.Invoke(this);
    }

    public string GetHeader() => header.text;

    public void AddItem(string key, string value)
    {
        
        if (NameValuePairPrefab is null) throw new System.Exception("NameValuePair prefab not set on DetailCard script");
        GameObject nameValueObj = Instantiate(NameValuePairPrefab, textArea.transform);
        if (nameValueObj.TryGetComponent(out NameValuePair pair))
        {
            pair.Key.text = key;
            pair.Value.text = value;
            items.Add(key, pair);
        }
        else
        {
            throw new System.Exception("Name value pair object has no NameValuePair script");
        }
    }

    public void DeleteItem(string key)
    {
        Destroy(items[key].gameObject);
    }

    /// <summary>
    /// Gets access to the key-value pair with the given key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string this[string key]
    {
        get
        {
            if (items.ContainsKey(key))
            {
                return items[key].Value.text;
            }
            else
            {
                return "";
            }
        }
        set
        {
            if (items.ContainsKey(key))
            {
                items[key].Value.text = value;
            }
        }
    }

    /// <summary>
    /// Spawns in a DetailCard prefab with given background sprite and 1 NameValuePair with given key and value
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="backgroundSprite"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public new static DetailedCard Spawn(Transform parent, UIBackgroundSprite backgroundSprite, string header)
    {
        DetailedCard detailCard = CopyPrefabToParent(parent);
        detailCard.SetText(header);
        detailCard.SetBackgroundSprite(backgroundSprite);
        detailCard.button = detailCard.GetComponent<Button>();
        return detailCard;
    }

    private static DetailedCard CopyPrefabToParent(Transform parent)
    {
        if (staticPrefab == null) staticPrefab = Resources.Load<GameObject>(prefabAddress);
        return Instantiate(staticPrefab, parent.position, Quaternion.identity, parent).GetComponent<DetailedCard>();
    }
}
