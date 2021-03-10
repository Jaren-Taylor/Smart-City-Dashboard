using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFocusableWindow 
{
    public bool IsFullyVisible();
    public void ToggleMenuHandler();
    void OnNumberKeyPress(int value);
}
