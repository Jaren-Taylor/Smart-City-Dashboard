using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWindow 
{
    public bool IsOpen();
    public void Open();
    public void Close();
    public void Toggle();
}
