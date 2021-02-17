using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    protected RectTransform menuBounds;
    private int activeTab;
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
        activeTab = 1;
        isOnScreen = false;
    }

    public void SwitchTabs()
    {
        // deactivate current tab
        transform.GetChild(activeTab).GetComponent<Tab>().DeActivate();
        // increment or reset counter
        activeTab = activeTab == transform.childCount - 1 ? 1 : activeTab+1;
        // activate new tab
        transform.GetChild(activeTab).GetComponent<Tab>().Activate();
    }

    public abstract void ToggleMenuHandler();
}
