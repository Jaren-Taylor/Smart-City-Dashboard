using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorInfoMenu : MonoBehaviour
{
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

}
