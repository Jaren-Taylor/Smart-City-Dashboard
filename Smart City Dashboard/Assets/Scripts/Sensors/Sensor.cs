using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sensor<T> : ISensor
{
    protected readonly static Dictionary<SensorStatus, string> StatusStringMapping = new Dictionary<SensorStatus, string>()
    {
        { SensorStatus.Fine, "No Congestion" },
        { SensorStatus.Meh, "Light Congestion" },
        { SensorStatus.Bad, "Heavy Congestion" }
    };

    private readonly Vector2Int tilePosition;

    public Action<List<T>> DataCollected;
    public Action<ISensor> StatusUpdated { get; set; }

    protected SensorStatus statusEnum = SensorStatus.Fine;
    protected string lastStatus = StatusStringMapping[SensorStatus.Fine];

    public Sensor(Vector2Int tilePosition)
    {
        this.tilePosition = tilePosition;
    }

    public void CollectDataFrom(HashSet<GameObject> sensedObjects)
    {
        List<T> collectedData = new List<T>();
        foreach(GameObject sensedObject in sensedObjects) collectedData.Add(CollectData(sensedObject));
        DataCollected?.Invoke(collectedData);
        var (msg, status) = GetStatus(collectedData);
        if(status != statusEnum)
        {
            lastStatus = msg;
            statusEnum = status;
            StatusUpdated?.Invoke(this);
        }
    }

    protected abstract (string msg, SensorStatus status) GetStatus(List<T> collectedData);
    protected abstract T CollectData(GameObject sensedObject);

    public (string msg, SensorStatus status) Status() => ($"{ToString()}: {lastStatus}", statusEnum);

    public Vector2Int GetTilePosition() => tilePosition;
}
