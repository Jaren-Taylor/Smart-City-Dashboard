using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFocusableWindow 
{
    public bool IsFullyVisible();
    public void OnNumberKeyPress(int value);
    public void ToggleMenuHandler();
}
