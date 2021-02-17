using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    public void DeActivate()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
            // make transparent
            SetTransparency(0.5f);
        }
    }

    public void Activate()
    {
        for (int i = 1; i < transform.childCount; i++)
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
