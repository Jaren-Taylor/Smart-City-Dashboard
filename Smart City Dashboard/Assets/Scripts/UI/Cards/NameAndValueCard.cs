using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NameAndValueCard : HeaderCard
{
    public static new readonly string prefabAddress = "Prefabs/UI/Cards/NameAndValueCard";
    private static new GameObject staticPrefab = null;

    [SerializeField]
    private NameValuePair nameValuePair;

    public string Name 
    {
        get { return nameValuePair.Key.text; }
        set { nameValuePair.Key.text = value; }
    }

    public string Value
    {
        get { return nameValuePair.Value.text; }
        set { nameValuePair.Value.text = value; }
    }

    /// <summary>
    /// Spawns in a DetailCard prefab with given background sprite and 1 NameValuePair with given key and value
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="backgroundSprite"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public new static NameAndValueCard Spawn(Transform parent, UIBackgroundSprite backgroundSprite, string header, string name, string value)
    {
        NameAndValueCard nameValueCard = CopyPrefabToParent(parent);
        nameValueCard.Header = header;
        nameValueCard.Name = name;
        nameValueCard.Value = value;
        nameValueCard.BackgroundSprite = backgroundSprite;
        return nameValueCard;
    }

    private static NameAndValueCard CopyPrefabToParent(Transform parent)
    {
        if (staticPrefab == null) staticPrefab = Resources.Load<GameObject>(prefabAddress);
        return Instantiate(staticPrefab, parent.position, Quaternion.identity, parent).GetComponent<NameAndValueCard>();
    }
}
