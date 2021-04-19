using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public Action<bool> OnUIToggle;
    public Action OnEnteringUI;
    public Action OnExitingUI;
    [SerializeField]
    private Menu defaultMenu;
    [SerializeField]
    private TileSensorMenu TileSensorPreview;
    [SerializeField]
    private Menu dashboardMenu;
    [SerializeField]
    private Menu pauseMenu;
    [HideInInspector]
    public Menu ActiveMenu;
    public static bool DashboardMode = false;

    private Dictionary<Key, Menu> keyToMenuDict = new Dictionary<Key, Menu>();
    private static List<Menu> activeMenus = new List<Menu>();

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
    }

    public void Subscribe(Menu menu)
    {
        if (menu.Key!=Key.None)keyToMenuDict.Add(menu.Key, menu);
        if (DashboardMode && menu == dashboardMenu) return;
        menu.Close();
        menu.OnOpen += AddMenu;
        menu.OnClose += RemoveMenu;
        menu.OnEnter += OnPointerEnter;
        menu.OnExit += OnPointerExit;
    }

    public void ReceiveMenuKey(Key key)
    {
        if (keyToMenuDict.ContainsKey(key)) 
        {
            ToggleMenu(keyToMenuDict[key]);
        } 
        else 
        {
            //Debug.Log("That key is not bound to any menu! Key: " + key.ToString());
        }
    }

    public void ToggleMenu(Menu menu)
    {
        if (!DashboardMode || menu == pauseMenu)
        {
            menu.Toggle();
        }
    }

    public void LoadMainMenuScene()
    {
        if(CameraManager.Instance.isFollowingEntity)
            CameraManager.Instance.StopFollowEntity();


        ObjectPoolerManager.ClearPools();
        GameSceneManager.LoadScene(SceneIndexes.TITLE, "Loading Main Menu");
    }

    private void RemoveMenu(Menu menu)
    {
        activeMenus.Remove(menu);
        // only re-set ActiveMenu if the menu being turned off was the ActiveMenu
        if (activeMenus.Count > 0)
        {
            if (menu == ActiveMenu) ActiveMenu = activeMenus[activeMenus.Count - 1];
        }
        else
        {
            ActiveMenu = null;
        }
        OnUIToggle?.Invoke(IsUIActive());
    }

    private void AddMenu(Menu menu)
    {
        activeMenus.Add(menu);
        ActiveMenu = menu;
        OnUIToggle?.Invoke(true);
    }

    /// <summary>
    /// Returns true if any Menu is open
    /// </summary>
    /// <returns></returns>
    public bool IsUIActive()
    {
        foreach (Menu menu in activeMenus)
        {
            if (menu.IsOpen()) return true;
        }
        return false;
    }

    public static void CloseAll()
    {
        for (int i = 0; i < activeMenus.Count; i++)
        {
            activeMenus[i].Close();
        }
    }

    #region Tab group methods

    public void OnNumberKeyPress(int value)
    {
        if (ActiveMenu != null)
        {
            ActiveMenu.OnNumberKeyPress(value);
        }
        else
        {
            defaultMenu.OnNumberKeyPress(value);
        }
    }

    /// <summary>
    /// Switches menu tabs
    /// </summary>
    public void NextTab()
    {
        if (ActiveMenu != null)
        {
            ActiveMenu.NextTab();
        }
        else
        {
            defaultMenu.NextTab();
        }
    }

    #endregion

    #region Sensor popup methods

    public void ToggleSensorPopup() => ToggleMenu(TileSensorPreview);

    internal void InspectTile(Vector2Int position)
    {
        TileSensorPreview.FocusTile(position);
        if (!activeMenus.Contains(TileSensorPreview)) ToggleSensorPopup();
    }

    #endregion

    /// <summary>
    /// Called whenever the pointer hovers over a Menu
    /// </summary>
    public void OnPointerEnter()
    {
        OnEnteringUI?.Invoke();
    }

    /// <summary>
    /// Called whenever the pointer hovers off of a Menu
    /// </summary>
    public void OnPointerExit()
    {
        OnExitingUI?.Invoke();
    }
}

public enum UIBackgroundSprite
{
    Red,
    Green,
    Blue,
    Yellow,
    Orange,
    OrangeButton,
    Purple
}

public static class UIBackgroundSpriteExtensions
{
    public static string GetAddress(this UIBackgroundSprite sprite) => sprite switch
    {
        UIBackgroundSprite.Red => "UI/UI Elements/Buttons/red button",
        UIBackgroundSprite.Green => "UI/UI Elements/Buttons/green button",
        UIBackgroundSprite.Blue => "UI/UI Elements/Buttons/blue button",
        UIBackgroundSprite.Yellow => "UI/UI Elements/Buttons/yellow button",
        UIBackgroundSprite.Orange => "UI/UI Elements/Buttons/Orange button",
        UIBackgroundSprite.OrangeButton => "UI/UI Elements/Buttons/Orange_button",
        UIBackgroundSprite.Purple => "UI/ UI Elements/Buttons/purple_button_inverted",
        _ => ""
    };
}