using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject test = SimpleCard.Spawn(gameObject.transform, Color.blue, "Hello World");
        RectTransform rect = (RectTransform)test.transform;
        rect.anchoredPosition = new Vector2(UIElementManager.Margin, -UIElementManager.Margin);
        //rect.position = new Vector3(rect.parent.position.x+UIElementManager.Margin, rect.parent.position.y + UIElementManager.Margin, 0);
    }
}
