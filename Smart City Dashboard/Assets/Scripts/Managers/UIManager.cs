using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private List<Menu> menus = new List<Menu>();
    public Action<bool> OnUIToggle;
    public Menu EscapeMenu;
    public Menu TildeMenu;
    // contains a dupe reference to the currently active menu
    [HideInInspector]
    public Menu ActiveMenu;

    private void Start()
    {
        if (EscapeMenu != null) menus.Add(EscapeMenu);
        if (TildeMenu != null) menus.Add(TildeMenu);
    }

    public bool IsUIActive()
    {
        foreach (var menu in menus)
        {
            if (menu.isActive) return true;
        }
        return false;
    }

    public void SwitchTabs()
    {
        ActiveMenu.SwitchTabs();
    }

    public void ToggleEscapeMenu() {
        EscapeMenu.ToggleMenuHandler();
        ActiveMenu = EscapeMenu;
    }

    public void ToggleTildeMenu()
    {
        TildeMenu.ToggleMenuHandler();
        ActiveMenu = TildeMenu;
    }

    public void OnUIToggleHandler() { 
        OnUIToggle?.Invoke(IsUIActive());
    }
}
