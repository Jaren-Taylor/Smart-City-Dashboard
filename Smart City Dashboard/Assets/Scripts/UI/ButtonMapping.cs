using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ButtonMapping
{
    [SerializeField]
    public Button button;
    [SerializeField]
    public EGridControlState ControlState;
}