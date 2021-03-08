using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private Dictionary<KeyCode, MenuType> keyToMenuDict = new Dictionary<KeyCode, MenuType>();
    private Dictionary<MenuType, IFocusableWindow> enumToMenu = new Dictionary<MenuType, IFocusableWindow>();
    private List<IFocusableWindow> menus = new List<IFocusableWindow>();

    public Action<bool> OnUIToggle;
    public Action<int> OnTabSwitch;
    public Menu F1Menu;
    public Menu TildeMenu;
    public TileSensorMenu TileSensorPreview; 
    // contains a dupe reference to the currently active menu
    [HideInInspector]
    public IFocusableWindow ActiveMenu;

    public static UIManager Instance;

    private void Start()
    {
        Instance = this;

        enumToMenu.Add(MenuType.Dashboard, TildeMenu);
        enumToMenu.Add(MenuType.GridState, F1Menu);
        enumToMenu.Add(MenuType.TileSensorPopup, TileSensorPreview);

        keyToMenuDict.Add(KeyCode.F1, MenuType.GridState);
        keyToMenuDict.Add(KeyCode.BackQuote, MenuType.Dashboard);
    }

    public void SwitchTabs()
    {
        if (ActiveMenu is Menu tabbedMenu)
        {
            tabbedMenu.SwitchTabs();
            if(tabbedMenu == F1Menu) // TODO this only works if ModeMenu is externally set as the escape menu
            {
                OnTabSwitch?.Invoke(tabbedMenu.ActiveTab);
            }
        }
    }

    public void ReceiveMenuKey(KeyCode key)
    {
        if (keyToMenuDict.ContainsKey(key)) {
            ToggleMenu(keyToMenuDict[key]);
        } else {
            throw new Exception("That key is not bound to a menu!");
        }
    }

    public void ToggleSensorPopup() => ToggleMenu(MenuType.TileSensorPopup);

    public void ToggleMenu(MenuType menuType)
    {
        IFocusableWindow menu = enumToMenu[menuType];
        // check if we'll be turning the menu off or on
        if (menu.IsFullyVisible())
        {
            TurnOffMenu(menu);
        }
        else
        {
            TurnOnMenu(menu);
        }
        menu.ToggleMenuHandler();
        OnUIToggle?.Invoke(IsUIActive());
    }

    private void TurnOffMenu(IFocusableWindow menu)
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

    private void TurnOnMenu(IFocusableWindow menu)
    {
        menus.Add(menu);
        ActiveMenu = menu;
    }

    public bool IsUIActive()
    {
        foreach (var menu in menus)
        {
            if (menu.IsFullyVisible()) return true;
        }
        return false;
    }

    public void OnNumberKeyPress(int value)
    {
        if (ActiveMenu != null)
        {
            ActiveMenu.OnNumberKeyPress(value);
        }else
        {
            F1Menu.OnNumberKeyPress(value);
        }
    }

    internal void InspectTile(Vector2Int position)
    {
        TileSensorPreview.FocusTile(position);
        if (!menus.Contains(TileSensorPreview)) ToggleSensorPopup();
    }
}

public enum MenuType
{
    Dashboard,
    GridState,
    TileSensorPopup
}