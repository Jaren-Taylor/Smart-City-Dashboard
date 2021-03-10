using System.Collections.Generic;
using UnityEngine;

public class TabbedMenu : Menu
{
    public List<Tab> Tabs;
    [HideInInspector]
    public int ActiveTab = 0;

    private new void Start()
    {
        base.Start();
        DeactivateTabs();
        if (Tabs.Count > 0) Tabs[0].Activate();
    }

    /// <summary>
    /// Deactivates all tabs
    /// </summary>
    private void DeactivateTabs()
    {
        for (int i = 0; i < Tabs.Count; i++)
        {
            Tabs[i].DeActivate();
        }
    }

    /// <summary>
    /// Switches to the next tab. If on the last tab, warps back to the 1st tab
    /// </summary>
    public void SwitchTabs()
    {
        // deactivate current tab
        Tabs[ActiveTab].DeActivate();
        // increment or reset counter
        ActiveTab = ActiveTab == Tabs.Count - 1 ? 0 : ActiveTab + 1;
        // activate new tab
        Tabs[ActiveTab].Activate();
    }

    /// <summary>
    /// Switch to the ith tab
    /// </summary>
    /// <param name="index"></param>
    public void SwitchTab(int index)
    {
        if (index >= 0 && index < Tabs.Count)
        {
            Tabs[ActiveTab].DeActivate();
            ActiveTab = index;
            Tabs[index].Activate();
        }
        else
        {
            throw new System.Exception("Tab index out of bounds " + index);
        }
    }

    /// <summary>
    /// Switch to a given Tab instance
    /// </summary>
    /// <param name="index"></param>
    public void SwitchTab(Tab tab)
    {
        if (Tabs.Contains(tab)) {
            Tabs[ActiveTab].DeActivate();
            ActiveTab = Tabs.IndexOf(tab);
            Tabs[ActiveTab].Activate();
        }
    }

    /// <summary>
    /// Communicates to a child Tab that a number key was pressed
    /// </summary>
    /// <param name="index"></param>
    public void OnNumberKeyPress(int index)
    {
        Tabs[ActiveTab].ButtonClick(index);
    }
}
