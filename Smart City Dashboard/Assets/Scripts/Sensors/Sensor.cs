using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sensor<T> : ISensor
{
    public Action<List<T>> DataCollected;
    public void CollectDataFrom(HashSet<GameObject> sensedObjects)
    {
        List<T> collectedData = new List<T>();
        foreach(GameObject sensedObject in sensedObjects) collectedData.Add(CollectData(sensedObject));
        DataCollected?.Invoke(collectedData);
    }


    protected abstract T CollectData(GameObject sensedObject);
    public abstract void RegisterToManager(SensorManager sensor);
    public abstract void DeregisterFromManager(SensorManager sensor);
    public abstract string GetSimpleName();
}
