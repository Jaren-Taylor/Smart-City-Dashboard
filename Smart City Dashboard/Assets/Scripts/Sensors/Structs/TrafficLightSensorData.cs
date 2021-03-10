using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TrafficLightSensorData
{
    public readonly TrafficLightSensor Sender;
    public readonly Vector2Int TilePosition;
    public readonly NodeCollectionController.Direction EstimatedDirection;
    public readonly bool IsInbound;
    public readonly float StopTime;
    public readonly NodeCollectionController.TargetUser Entity;

    public TrafficLightSensorData(TrafficLightSensor sender, Vector2Int tilePosition, NodeCollectionController.Direction estimatedDirection, bool isInbound, float stopTime, NodeCollectionController.TargetUser entity)
    {
        Sender = sender;
        TilePosition = tilePosition;
        EstimatedDirection = estimatedDirection;
        IsInbound = isInbound;
        StopTime = stopTime;
        Entity = entity;
    }

    public override string ToString()
    { 
        if(IsInbound) return $"({Entity}): Inbound in {EstimatedDirection} stopped for {StopTime}  Tile: [{TilePosition.x}, {TilePosition.y}]";
        else return $"({Entity}): Outgoing in {EstimatedDirection} stopped for {StopTime} Tile: [{TilePosition.x}, {TilePosition.y}]";
    }
}
