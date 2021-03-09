using System.Collections.Generic;
using UnityEngine;

public interface ISensor
{
    public void CollectDataFrom(HashSet<GameObject> sensedObjects);
}