using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScrollMenu : MonoBehaviour
{
    public ScrollablePopupMenu popup;

    int i = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            popup.gameObject.SetActive(!popup.isActiveAndEnabled);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            i++;
            popup.AddNewItem(Color.blue, "Test Entry: " + i);
        }
    }
}
