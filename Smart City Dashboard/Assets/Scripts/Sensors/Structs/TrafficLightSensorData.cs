using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TrafficLightSensorData
{
    public readonly Vector2Int TilePosition;
    public readonly NodeCollectionController.Direction EstimatedDirection;
    public readonly float StopTime;
    public readonly NodeCollectionController.TargetUser Entity;

    public TrafficLightSensorData(Vector2Int tilePosition, NodeCollectionController.Direction estimatedDirection, float stopTime, NodeCollectionController.TargetUser entity)
    {
        TilePosition = tilePosition;
        EstimatedDirection = estimatedDirection;
        StopTime = stopTime;
        Entity = entity;
    }
}
