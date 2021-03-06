using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIElementManager : MonoBehaviour
{
    public static float Margin = 8;
    private static float halfMargin = Margin / 2;

     
    public List<UIElement> elements = new List<UIElement>();

    public void AddElement(UIElement element)
    {

    }

    public void UpdateElementPositions()
    {
        float nextYPosition = Margin;
        foreach(UIElement element in elements)
        {
            element.transform.position = new Vector3(Margin, nextYPosition, 0);
            nextYPosition += element.Height + Margin;
        }
    } 
}
