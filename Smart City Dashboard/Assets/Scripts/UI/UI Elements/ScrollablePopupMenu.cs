using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScrollablePopupMenu : MonoBehaviour
{
    [SerializeField]
    private VerticalLayoutGroup menuContent;

    public UnityEvent OnCloseMenu;

    public NameAndValueCard AddNewItem(UIBackgroundSprite spriteColor, string header, string name, string value)
    {
        return NameAndValueCard.Spawn(menuContent.transform, spriteColor, header, name, value);
    }

    public HeaderCard AddNewItem(UIBackgroundSprite spriteColor, string header)
    {
        return NameAndValueCard.Spawn(menuContent.transform, spriteColor, header);
    }

    /*public NameAndValueCard AddNewItem(Material material, string text)
    {
        return NameAndValueCard.Spawn(menuContent.transform, material, text);
    }*/

    public void Clear()
    {
        foreach(Transform child in menuContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void CloseMenu()
    {
        OnCloseMenu?.Invoke();
    }
}
