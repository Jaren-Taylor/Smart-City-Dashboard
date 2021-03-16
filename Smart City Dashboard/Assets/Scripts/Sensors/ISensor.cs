using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISensor
{
    public Action<ISensor> StatusUpdated { get; set; }
    public Vector2Int GetTilePosition();
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
    public static UIBackgroundSprite GetColor(this SensorStatus status) => status switch
    {
        SensorStatus.Meh => UIBackgroundSprite.Yellow,
        SensorStatus.Bad => UIBackgroundSprite.Red,
        _ => UIBackgroundSprite.Green,
    };
}