using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TrafficLightSensorData
{
    public readonly NodeCollectionController.Direction EstimatedDirection;
    public readonly float StopTime;
    public readonly NodeCollectionController.TargetUser Entity;

    public TrafficLightSensorData(NodeCollectionController.Direction estimatedDirection, float stopTime, NodeCollectionController.TargetUser entity)
    {
        EstimatedDirection = estimatedDirection;
        StopTime = stopTime;
        Entity = entity;
    }
}
