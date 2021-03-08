using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITester : MonoBehaviour
{
    public UIElementManager manager;
    // Start is called before the first frame update
    void Start()
    {
        string test = "This is really long text that should cause wrapping on each of the cards I am meant to test with and at least this text is a little bit more appropriate and does not contain any swears not that the last one actually contain swears no not at all.";
        for (int i = 0; i < 5; i++)
        {
            //test += "\n";
            SimpleCard.Spawn(gameObject.transform, Color.blue, test + i);//.transform.SetParent(transform);
        }
    }
}
