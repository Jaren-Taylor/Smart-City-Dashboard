using System;
using UnityEngine;

[Serializable]
public enum SensorType
{
    Camera
}


public static class SensorTypeExtensions
{
    public static GameObject GetPrefab(this SensorType sensor)
    {
        return Resources.Load<GameObject>(sensor.PrefabAddress());
    }

    public static string PrefabAddress(this SensorType sensor) =>
        sensor switch
        {
            SensorType.Camera => "Prefabs/Sensors/CameraSensor",
            _ => throw new System.Exception("Sensor type not implemented yet")
        };
}