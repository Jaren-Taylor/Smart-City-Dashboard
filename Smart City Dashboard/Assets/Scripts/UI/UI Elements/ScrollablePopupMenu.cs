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

    public HeaderCard AddNewItem(UIBackgroundSprite spriteColor, string text)
    {
        return HeaderCard.Spawn(menuContent.transform, spriteColor, text);
    }

    public HeaderCard AddNewItem(Material material, string text)
    {
        return HeaderCard.Spawn(menuContent.transform, material, text);
    }

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
