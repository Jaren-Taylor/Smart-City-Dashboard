using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    [HideInInspector]
    public List<TabButton> TabButtons;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public List<GameObject> ObjectsToSwap;

    [HideInInspector]
    public TabButton ActiveTab;

    public void Subscribe(TabButton button)
    {
        if(TabButtons == null)
        {
            TabButtons = new List<TabButton>();
        }
        TabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if (ActiveTab == null || button != ActiveTab) 
        { 
            button.background.sprite = tabHover;
        }
    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
    {
        ActiveTab = button;
        ResetTabs();
        button.background.sprite = tabActive;
        SwitchToTab(button.transform.GetSiblingIndex());
    }

    public void ResetTabs()
    {
        foreach (TabButton button in TabButtons)
        {
            if (ActiveTab != null && button == ActiveTab) continue;
            button.background.sprite = tabIdle;
        }
    }

    /*private void Start()
    {
        DeactivateTabs();
        if (TabButtons.Count > 0) TabButtons[0].Activate();
    }*/

    /// <summary>
    /// Deactivates all tabs
    /// </summary>
    /*private void DeactivateTabs()
    {
        for (int i = 0; i < TabButtons.Count; i++)
        {
            TabButtons[i].DeActivate();
        }
    }
*/
    /// <summary>
    /// Switches to the next TabButton. If on the last tab, warps back to the 1st tab
    /// </summary>
    public void NextTab()
    {
        int index = ActiveTab.transform.GetSiblingIndex();
        if (index == ObjectsToSwap.Count - 1)
        {
            OnTabSelected(TabButtons[0]);
        }
        else
        {
            OnTabSelected(TabButtons[index + 1]);
        }
    }

    /// <summary>
    /// Switch to the ith TabButton
    /// </summary>
    /// <param name="index"></param>
    private void SwitchToTab(int index)
    {
        if (index >= 0 && index < TabButtons.Count)
        {
            for (int i = 0; i < ObjectsToSwap.Count; i++)
            {
                if (i == index)
                {
                    ObjectsToSwap[i].SetActive(true);
                }
                else
                {
                    ObjectsToSwap[i].SetActive(false);
                }
            }
            //TabButtons[ActiveTab].DeActivate();
            //ActiveTab = index;
            //TabButtons[index].Activate();
        }
        else
        {
            throw new System.Exception("Tab index out of bounds " + index);
        }
    }

    /// <summary>
    /// Communicates to a TabButton that a number key was pressed
    /// </summary>
    /// <param name="index"></param>
    public void OnNumberKeyPress(int index)
    {
        TabButtons[ActiveTab.transform.GetSiblingIndex()].ButtonClick(index);
    }
}
