using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITester : MonoBehaviour
{
    public UIElementManager manager;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            SimpleCard.Spawn(gameObject.transform, Color.blue, "Hello\n World " + i).transform.SetParent(transform);
        }
    }
}
