using System.Collections.Generic;
using UnityEngine;

public interface ISensor
{
    public void RegisterToManager(SensorManager manager);
    public void DeregisterFromManager(SensorManager manager);
    public void CollectDataFrom(HashSet<GameObject> sensedObjects);
    public string GetSimpleName();
}