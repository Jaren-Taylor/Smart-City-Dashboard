using System;
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
            var card = popup.AddNewItem(Color.blue, "Test Entry: " + i);
            card.OnClick += Clicked;
        }
    }

    private void Clicked(UIElement obj)
    {
        Debug.Log("It's been clicked!");
    }
}
