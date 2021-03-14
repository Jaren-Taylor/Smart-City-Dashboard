using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    private Dictionary<Key, MenuType> keyToMenuDict = new Dictionary<Key, MenuType>();
    private Dictionary<MenuType, Menu> enumToMenu = new Dictionary<MenuType, Menu>();
    private List<Menu> menus = new List<Menu>();

    public Action<bool> OnUIToggle;
    public Action OnEnteringUI;
    public Action OnExitingUI;
    public Menu F1Menu;
    public Menu F2Menu;
    public Menu TildeMenu;
    public TileSensorMenu TileSensorPreview; 
    [HideInInspector]
    public Menu ActiveMenu;

    public static readonly Dictionary<UIBackgroundSprite, Sprite> BackgroundSprites = new Dictionary<UIBackgroundSprite, Sprite>();
    public static UIManager Instance;

    private void Start()
    {
        Instance = this;

        if (BackgroundSprites.Count == 0)
        {
            BackgroundSprites.Add(UIBackgroundSprite.Red,    Resources.Load<Sprite>("UI/UI Elements/Buttons/red button"));
            BackgroundSprites.Add(UIBackgroundSprite.Green,  Resources.Load<Sprite>("UI/UI Elements/Buttons/green button"));
            BackgroundSprites.Add(UIBackgroundSprite.Blue,   Resources.Load<Sprite>("UI/UI Elements/Buttons/blue button"));
            BackgroundSprites.Add(UIBackgroundSprite.Yellow, Resources.Load<Sprite>("UI/UI Elements/Buttons/yellow button"));
            BackgroundSprites.Add(UIBackgroundSprite.Orange, Resources.Load<Sprite>("UI/UI Elements/Buttons/Orange background"));
        }
        
        enumToMenu.Add(MenuType.Dashboard, TildeMenu);
        enumToMenu.Add(MenuType.GridState, F1Menu);
        enumToMenu.Add(MenuType.TileSensorPopup, TileSensorPreview);
        enumToMenu.Add(MenuType.SaveLoadMenu, F2Menu);
        //enumToMenu.Add(MenuType.SensorInfo, SensorInfoMenuInstance);

        keyToMenuDict.Add(Key.F1, MenuType.GridState);
        keyToMenuDict.Add(Key.F2, MenuType.SaveLoadMenu);
        keyToMenuDict.Add(Key.Backquote, MenuType.Dashboard);
    }

    public void ReceiveMenuKey(Key key)
    {
        if (keyToMenuDict.ContainsKey(key)) {
            ToggleMenu(keyToMenuDict[key]);
        } else {
            throw new Exception("That key is not bound to a menu! Key: " + key.ToString());
        }
    }

    public void ToggleSensorPopup() => ToggleMenu(MenuType.TileSensorPopup);

    public void ToggleMenu(MenuType menuType)
    {
        Menu menu = enumToMenu[menuType];
        // check if we'll be turning the menu off or on
        if (menu.IsOpen())
        {
            TurnOffMenu(menu);
        }
        else
        {
            TurnOnMenu(menu);
        }
        menu.Toggle();
        OnUIToggle?.Invoke(IsUIActive());
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

    /// <summary>
    /// Returns true if any Menu is open
    /// </summary>
    /// <returns></returns>
    public bool IsUIActive()
    {
        foreach (var menu in menus)
        {
            if (menu.IsOpen()) return true;
        }
        return false;
    }

    public void OnNumberKeyPress(int value)
    {
        if (ActiveMenu != null && ActiveMenu.TabGroup != null)
        {
            ActiveMenu.TabGroup.OnNumberKeyPress(value);
        }
        else
        {
            F1Menu.TabGroup.OnNumberKeyPress(value);
        }
    }

    /// <summary>
    /// Switches menu tabs only if a menu is open
    /// </summary>
    public void NextTab()
    {
        if (ActiveMenu != null && ActiveMenu.TabGroup != null)
        {
            ActiveMenu.TabGroup.NextTab();
        }
        else
        {
            F1Menu.TabGroup.NextTab();
        }
    }

    internal void InspectTile(Vector2Int position)
    {
        TileSensorPreview.FocusTile(position);
        if (!menus.Contains(TileSensorPreview)) ToggleSensorPopup();
    }

    /// <summary>
    /// Called whenever the pointer hovers over a Menu
    /// </summary>
    public void OnPointerEnter() => OnEnteringUI?.Invoke();

    /// <summary>
    /// Called whenever the pointer hovers off of a Menu
    /// </summary>
    public void OnPointerExit() => OnExitingUI?.Invoke();
}

public enum MenuType
{
    Dashboard,
    GridState,
    TileSensorPopup,
    SaveLoadMenu
}

public enum UIBackgroundSprite
{
    Red,
    Green,
    Blue,
    Yellow,
    Orange,
}