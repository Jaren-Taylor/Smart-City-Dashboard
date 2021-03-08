using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSensorMenu : MonoBehaviour, IFocusableWindow
{
    [SerializeField]
    private ScrollablePopupMenu menu;

    public bool IsFullyVisible()
    {
        return gameObject.activeSelf;
    }

    public void OnNumberKeyPress(int value)
    {
        return;
    }

    public void ToggleMenuHandler()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }
}
