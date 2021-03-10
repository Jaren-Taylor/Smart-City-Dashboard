using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISensor
{
    public Action<ISensor> StatusUpdated { get; set; }

    public void CollectDataFrom(HashSet<GameObject> sensedObjects);
    public (string msg, SensorStatus status) Status();
}

public enum SensorStatus
{
    Fine,
    Meh,
    Bad
}

public static class SensorStatusExtensions
{
    public static Color GetColor(this SensorStatus status) => status switch
    {
        SensorStatus.Meh => Color.yellow,
        SensorStatus.Bad => Color.red,
        _ => Color.green,
    };
}