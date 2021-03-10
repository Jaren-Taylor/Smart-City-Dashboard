using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightSensor : Sensor<TrafficLightSensorData>
{
    private readonly Vector2Int tilePosition;

    public TrafficLightSensor(Vector2Int tilePosition)
    {
        this.tilePosition = tilePosition;
    }

    protected override TrafficLightSensorData CollectData(GameObject sensedObject)
    {
        var (direction, isInbound) = EstimateDirection(sensedObject.transform.position.ToGrid());



        if (sensedObject.TryGetComponent<PathWalker>(out var walker))
        {
            return new TrafficLightSensorData(this, tilePosition, direction, isInbound, walker.CurrentStopTime, walker.User);
        }
        else
        {
            Debug.LogWarning("No Pathwalker, we're just guessing what it is.");
            return new TrafficLightSensorData(this, tilePosition, direction, isInbound, 0f, NodeCollectionController.TargetUser.Both);
        }
        
    }

    private (NodeCollectionController.Direction direction, bool inbound) EstimateDirection(Vector2 position)
    {
        Vector2 delta = position - tilePosition;
        var directionFromCenter = delta.ToDirection().Value;
        bool inbound = GetInboundFromRelativeDirection(directionFromCenter, delta);
        if (inbound) return (directionFromCenter.Oppisite(), inbound);
        else return (directionFromCenter, inbound);
    }

    private bool GetInboundFromRelativeDirection(NodeCollectionController.Direction direction, Vector2 delta) => direction switch
    {
        NodeCollectionController.Direction.EastBound => delta.y >= 0,
        NodeCollectionController.Direction.WestBound => delta.y <= 0,
        NodeCollectionController.Direction.NorthBound => delta.x <= 0,
        NodeCollectionController.Direction.SouthBound => delta.x >= 0,
        _ => false
    };

    private NodeCollectionController.Direction GuessDirection(Vector2 position)
    {
        Vector2 delta = (position - tilePosition) * -1f;
        var direction = delta.ToDirection();
        if (direction is null) throw new System.Exception("I just can't figure it out.");
        return direction.Value;
    }

    public override string ToString()
    {
        return $"Smart Traffic Light [{tilePosition.x},{tilePosition.y}]";
    }
}
