using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NavPointConnection 
{
    public enum ConnectionType
    {
        Directed,
        Bidirectional
    }


    [SerializeField]
    private NavPoint target;
    [SerializeField]
    private ConnectionType type;

    public NavPointConnection(NavPoint target, ConnectionType type)
    {
        this.target = target;
        this.type = type;
    }

    public NavPoint Target { get => target; }
    public ConnectionType Type { get => type; }


}
