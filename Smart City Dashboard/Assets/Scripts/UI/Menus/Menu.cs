using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    [HideInInspector]
    public List<Tab> tabs = new List<Tab>();
    protected RectTransform menuBounds;
    public EUIPosition uiPosition = EUIPosition.Bottom;
    protected float glideAmount;
    protected int glideSpeed = 25;
    [HideInInspector]
    public int ActiveTab = 0;
    [HideInInspector]
    public bool isOnScreen;

    protected void Start()
    {
        // Assumed to be used in child classes for use in movement calculations
        menuBounds = gameObject.GetComponent<RectTransform>();
        InitializeTabsList();
        DeactivateTabs();
        tabs[0].Activate();
        InitializeGlideAmount();
        JumpToDestination();
        isOnScreen = false;
    }

    /// <summary>
    /// If needed, glide the menu into/out of place
    /// </summary>
    private void Update()
    {
        // dont try to move if we're at our target position
        switch (uiPosition)
        {
            case EUIPosition.Top:
            case EUIPosition.Bottom:
                if (transform.position.y != glideAmount) GlideTowardsDestination();
                break;
            case EUIPosition.Left:
            case EUIPosition.Right:
                if (transform.position.x != glideAmount) GlideTowardsDestination();
                break;
        }
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
            case EUIPosition.Top:
                newPosition.y += (glideAmount - newPosition.y) / glideSpeed;
                break;
            case EUIPosition.Bottom:
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
    /// Quickly move the menu in and out of view
    /// </summary>
    private void JumpToDestination()
    {
        // Move towards destination portions at a time
        Vector3 newPosition = transform.position;
        switch (uiPosition)
        {
            case EUIPosition.Top:
                newPosition.y += glideAmount - newPosition.y;
                break;
            case EUIPosition.Bottom:
                newPosition.y += glideAmount - newPosition.y;
                break;
            case EUIPosition.Left:
                newPosition.x += glideAmount - newPosition.x;
                break;
            case EUIPosition.Right:
                newPosition.x += glideAmount - newPosition.x;
                break;
        }
        transform.position = newPosition;
    }

    /// <summary>
    /// Set the Menu's glide amount based on its EUIPosition
    /// </summary>
    private void InitializeGlideAmount()
    {
        switch (uiPosition)
        {
            case EUIPosition.Top:
                glideAmount = +menuBounds.rect.height;
                break;
            case EUIPosition.Bottom:
                glideAmount = -menuBounds.rect.height;
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
    }

    /// <summary>
    /// Switch to the ith tab
    /// </summary>
    /// <param name="index"></param>
    public void SwitchTab(int index)
    {
        if (index >= 0 && index < tabs.Count)
        {
            tabs[ActiveTab].DeActivate();
            ActiveTab = index;
            tabs[index].Activate();
        } else
        {
            throw new System.Exception("Tab index out of bounds "+index);
        }
    }

    /// <summary>
    /// Switch to a given Tab instance
    /// </summary>
    /// <param name="index"></param>
    public void SwitchTab(Tab tab)
    {
        tabs[ActiveTab].DeActivate();
        ActiveTab = tabs.IndexOf(tab);
        tabs[ActiveTab].Activate();
    }

    /// <summary>
    /// Communicates to a child Tab that a number key was pressed
    /// </summary>
    /// <param name="index"></param>
    public void OnNumberKeyPress(int index)
    {
        tabs[ActiveTab].ButtonClick(index);
    }

    /// <summary>
    /// Opens and closes the menu
    /// </summary>
    public virtual void ToggleMenuHandler()
    {
        float movementDelta = 0;
        // move with respect to the menu's RectTransform width or height
        switch (uiPosition)
        {
            case EUIPosition.Top:
            case EUIPosition.Bottom:
                movementDelta = isOnScreen ? -menuBounds.rect.height : menuBounds.rect.height;
                break;
            case EUIPosition.Left:
            case EUIPosition.Right:
                movementDelta = isOnScreen ? -menuBounds.rect.width : menuBounds.rect.width;
                break;

        }
        isOnScreen = !isOnScreen;
        // move the menu offscreen
        glideAmount += movementDelta;
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

    /// <summary>
    /// Fetch the Tab component from the ith transform child
    /// </summary>
    /// <param name="i"></param>
    private void TryFetchTab(int i)
    {
        if (transform.GetChild(i).TryGetComponent(out Tab tab))
        {
            tabs.Add(tab);
        }
    }
}