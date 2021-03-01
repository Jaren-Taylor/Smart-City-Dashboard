<<<<<<< HEAD
=======
using NUnit.Framework;
>>>>>>> b53a54f119af99d11537d00654eeb13e89ac73d3
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    protected List<Tab> tabs = new List<Tab>();
    protected RectTransform menuBounds;
    public EUIPosition uiPosition = EUIPosition.Bottom;
    protected float glideAmount;
    protected int glideSpeed = 25;
    [HideInInspector]
    public int ActiveTab = 0;
    [HideInInspector]
    public bool isOnScreen;
    public List<Tab> Tabs;

    protected void Start()
    {
        // Assumed to be used in child classes for use in movement calculations
        menuBounds = gameObject.GetComponent<RectTransform>();
<<<<<<< HEAD
        InitializeTabsList();
        DeactivateTabs();
        tabs[0].Activate();
        InitializeGlideAmount();
        isOnScreen = false;
    }

    /// <summary>
    /// Searches transform children for objects with Tab components, and adds them to the List tabs
    /// </summary>
    private void InitializeTabsList()
    {
        tabs.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            TryFetchTab(i);
        }
    }

    private void TryFetchTab(int i)
    {
        if (transform.GetChild(i).TryGetComponent(out Tab tab))
        {
            tabs.Add(tab);
        }
    }

    /// <summary>
    /// Deactivates all tabs
    /// </summary>
    private void DeactivateTabs()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            tabs[i].DeActivate();
        }
    }

    /// <summary>
    /// Set the Menu's glide amount based on its EUIPosition
    /// </summary>
    private void InitializeGlideAmount()
    {
        switch (uiPosition)
        {
            case EUIPosition.Bottom:
                glideAmount = -menuBounds.rect.height;
                break;
            case EUIPosition.Top:
                glideAmount = +menuBounds.rect.height;
                break;
            case EUIPosition.Left:
                glideAmount = -menuBounds.rect.width;
                break;
            case EUIPosition.Right:
                glideAmount = +menuBounds.rect.width;
                break;
        }
    }

    /// <summary>
    /// If needed, glide the menu into/out of place
    /// </summary>
    private void Update()
    {
        // dont try to move if we're at our target position
        if (transform.position.y != glideAmount) GlideTowardsDestination();
    }

    /// <summary>
    /// Opens and closes the menu
    /// </summary>
    public virtual void ToggleMenuHandler()
    {
        // move with respect to the menu's RectTransform width or height
        float yMove = isOnScreen ? -menuBounds.rect.height : menuBounds.rect.height;
        isOnScreen = !isOnScreen;
        // move the menu offscreen
        glideAmount += yMove;
    }

    /// <summary>
    /// Glide the menu in and out of view
    /// </summary>
    protected virtual void GlideTowardsDestination()
    {
        // Move towards destination portions at a time
        Vector3 newPosition = transform.position;
        switch (uiPosition)
        {
            case EUIPosition.Bottom:
                newPosition.y += (glideAmount - newPosition.y) / glideSpeed;
                break;
            case EUIPosition.Top:
                newPosition.y += (glideAmount - newPosition.y) / glideSpeed;
                break;
            case EUIPosition.Left:
                newPosition.x += (glideAmount - newPosition.x) / glideSpeed;
                break;
            case EUIPosition.Right:
                newPosition.x += (glideAmount - newPosition.x) / glideSpeed;
                break;
        }
        transform.position = newPosition;
    }

    /// <summary>
    /// Switches to the next tab. If on the last tab, warps back to the 1st tab
    /// </summary>
    public void SwitchTabs()
    {
        // deactivate current tab
        tabs[ActiveTab].DeActivate();
        // increment or reset counter
        ActiveTab = ActiveTab == tabs.Count-1 ? 0 : ActiveTab+1;
        // activate new tab
        tabs[ActiveTab].Activate();
=======
        FetchTabs();
        DeactivateTabs();
        isOnScreen = false;
    }

    private void FetchTabs()
    {
        // Deactivate all but the first child tab
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var tab = child.GetComponent<Tab>();
            if (tab != null)
            {
                Tabs.Add(tab);
            }
        }
        //
    }
    private void DeactivateTabs()
    {
        // Deactivate all but the first child tab
        for (int i = 1; i < Tabs.Count; i++)
        {
            Tabs[i].DeActivate();
        }
        //
    }
    public void SwitchTabs()
    {
        // deactivate current tab
        Tabs[ActiveTab].DeActivate();
        // increment or reset counter
        ActiveTab = ActiveTab == Tabs.Count - 1 ? 0 : ActiveTab+1;
        // activate new tab
        Tabs[ActiveTab].Activate();
>>>>>>> b53a54f119af99d11537d00654eeb13e89ac73d3
    }

    /// <summary>
    /// Switch to a specific tab
    /// </summary>
    /// <param name="index"></param>
    public void SwitchTab(int index)
    {
        if (index >= 0 && index < tabs.Count)
        {
            ActiveTab = index;
            tabs[index].Activate();
        } else
        {
            throw new System.Exception("Tab index out of bounds "+index);
        }
    }

    /// <summary>
    /// Communicates to a child Tab that a number key was pressed
    /// </summary>
    /// <param name="index"></param>
    public void OnNumberKeyPress(int index)
    {
        tabs[ActiveTab].ButtonClick(index);
    }
}