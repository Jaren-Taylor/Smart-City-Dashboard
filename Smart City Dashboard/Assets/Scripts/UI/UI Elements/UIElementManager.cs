using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIElementManager : MonoBehaviour
{
    public string Header = "Header";
    public TextMeshProUGUI headerObject = new TextMeshProUGUI();
    public static float Margin = 8;
    private static float halfMargin = Margin / 2;

    public List<UIElement> elements;

    private void Start()
    {
        headerObject.text = Header;
        headerObject.transform.position = new Vector3(Margin, 0, 0);
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
