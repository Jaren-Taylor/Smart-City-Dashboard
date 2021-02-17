using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private Dictionary<KeyCode, Menu> keyToMenuDict = new Dictionary<KeyCode, Menu>();
    private List<Menu> menus = new List<Menu>();
    public Action<bool> OnUIToggle;
    public Menu EscapeMenu;
    public Menu TildeMenu;
    // contains a dupe reference to the currently active menu
    [HideInInspector]
    public Menu ActiveMenu;

    private void Start()
    {
        keyToMenuDict.Add(KeyCode.Escape, EscapeMenu);
        keyToMenuDict.Add(KeyCode.BackQuote, TildeMenu);
    }
    public void OnUIToggleHandler() {
        Debug.Log(IsUIActive());
        OnUIToggle?.Invoke(IsUIActive());
    }

    public bool IsUIActive()
    {
        foreach (var menu in menus)
        {
            if (menu.isOnScreen) return true;
        }
        return false;
    }

    public void SwitchTabs()
    {
        if (ActiveMenu != null) ActiveMenu.SwitchTabs();
    }

    public void ToggleMenu(KeyCode key)
    {
        if (keyToMenuDict.ContainsKey(key)) {
            Menu menu = keyToMenuDict[key];
            // check if we'll be turning the menu off or on
            if (menu.isOnScreen)
            {
                TurnOffMenu(menu);
            } else
            {
                TurnOnMenu(menu);
            }
            menu.ToggleMenuHandler();
        } else {
            throw new Exception("That key is not bound to a menu!");
        }
    }

    private void TurnOffMenu(Menu menu)
    {
        menus.Remove(menu);
        // only re-set ActiveMenu if the menu being turned off was the ActiveMenu
        if (menus.Count > 0) { 
            if (menu == ActiveMenu) ActiveMenu = menus[menus.Count-1];
        } else
        {
            ActiveMenu = null;
        }
    }

    private void TurnOnMenu(Menu menu)
    {
        menus.Add(menu);
        ActiveMenu = menu;
    }
}
