using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class UICardManager : MonoBehaviour
{
    public HeaderCard AddHeaderCard(UIBackgroundSprite spriteColor, string header)
    {
        return HeaderCard.Spawn(transform, spriteColor, header);
    }
    public NameAndValueCard AddNameValueCard(UIBackgroundSprite spriteColor, string header, string name, string value)
    {
        return NameAndValueCard.Spawn(transform, spriteColor, header, name, value);
    }

    public DictionaryCard AddDictionaryCard(UIBackgroundSprite spriteColor, string header)
    {
        return DictionaryCard.Spawn(transform, spriteColor, header);
    }

    public void Clear()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
