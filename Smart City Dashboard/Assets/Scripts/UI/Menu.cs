using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    protected RectTransform MenuBounds;
    private int activeTab;
    [HideInInspector]
    public bool isOnScreen;

    protected void Start()
    {
        // Assumed to be used in child classes for use in movement calculations
        MenuBounds = gameObject.GetComponent<RectTransform>();
        // Deactivate all but the first child tab
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.GetComponent<Tab>().DeActivate();
        }
        //
        isOnScreen = false;
    }

    public void SwitchTabs()
    {
        // deactivate current tab
        transform.GetChild(activeTab).GetComponent<Tab>().DeActivate();
        // increment or reset counter
        activeTab = activeTab == transform.childCount - 1 ? 0 : activeTab+1;
        // activate new tab
        transform.GetChild(activeTab).GetComponent<Tab>().Activate();
    }

    public abstract void ToggleMenuHandler();
}
