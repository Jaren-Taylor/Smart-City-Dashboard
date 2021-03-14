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

    public SimpleCard AddNewItem(UIBackgroundSprite sprite, string text)
    {
        return SimpleCard.Spawn(menuContent.transform, sprite, text);
    }

    public SimpleCard AddNewItem(Material material, string text)
    {
        return SimpleCard.Spawn(menuContent.transform, material, text);
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
