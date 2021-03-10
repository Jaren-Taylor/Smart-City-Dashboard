using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sensor<T> : ISensor
{
    public Action<List<T>> DataCollected;
    public Action<ISensor> StatusUpdated { get; set; }

    protected string lastStatus = "";
    protected SensorStatus statusEnum = SensorStatus.Fine;


    public void CollectDataFrom(HashSet<GameObject> sensedObjects)
    {
        List<T> collectedData = new List<T>();
        foreach(GameObject sensedObject in sensedObjects) collectedData.Add(CollectData(sensedObject));
        DataCollected?.Invoke(collectedData);
        var (msg, status) = GetStatus(collectedData);
        if(msg != lastStatus)
        {
            lastStatus = msg;
            statusEnum = status;
            StatusUpdated?.Invoke(this);
        }
    }

    protected abstract (string msg, SensorStatus status) GetStatus(List<T> collectedData);
    protected abstract T CollectData(GameObject sensedObject);

    public (string msg, SensorStatus status) Status() => ($"{ToString()}: {lastStatus}", statusEnum);
}
