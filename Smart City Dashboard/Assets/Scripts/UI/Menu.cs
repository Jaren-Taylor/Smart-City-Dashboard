using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    protected RectTransform menuBounds;
    public int ActiveTab;
    [HideInInspector]
    public bool isOnScreen;

    protected void Start()
    {
        // Assumed to be used in child classes for use in movement calculations
        menuBounds = gameObject.GetComponent<RectTransform>();
        // Deactivate all but the first child tab
        for (int i = 2; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.GetComponent<Tab>().DeActivate();
        }
        //
        ActiveTab = 1;
        isOnScreen = false;
    }

    public void SwitchTabs()
    {
        // deactivate current tab
        transform.GetChild(ActiveTab).GetComponent<Tab>().DeActivate();
        // increment or reset counter
        ActiveTab = ActiveTab == transform.childCount - 1 ? 1 : ActiveTab+1;
        // activate new tab
        transform.GetChild(ActiveTab).GetComponent<Tab>().Activate();
    }

    public abstract void ToggleMenuHandler();
}
