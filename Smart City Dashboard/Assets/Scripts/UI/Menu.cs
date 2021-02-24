using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    protected RectTransform menuBounds;
    [HideInInspector]
    public int ActiveTab = 0;
    [HideInInspector]
    public bool isOnScreen;
    public List<Tab> Tabs;

    protected void Start()
    {
        // Assumed to be used in child classes for use in movement calculations
        menuBounds = gameObject.GetComponent<RectTransform>();
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
    }

    public abstract void ToggleMenuHandler();
}
