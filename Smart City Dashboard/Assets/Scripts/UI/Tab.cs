using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    public void DeActivate()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject element = transform.GetChild(i).gameObject;
            if (!element.TryGetComponent<TextMeshProUGUI>(out _)) element.SetActive(false);
            // make transparent
            SetTransparency(0.5f);
        }
    }

    public void Activate()
    {
        Menu manager = transform.parent.gameObject.GetComponent<Menu>();
        if (this != manager.activeTab)
        {
            manager.activeTab.DeActivate();
            manager.activeTab = this;

        }
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
            // make fully visible
            SetTransparency(1);
        }
    }

    private void SetTransparency(float a)
    {
        Image image = GetComponent<Image>();
        Color color = image.color;
        color.a = a;
        image.color = color;
    }
}
