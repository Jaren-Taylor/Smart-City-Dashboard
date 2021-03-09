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
        if (sensedObject.TryGetComponent<PathWalker>(out var walker))
        {
            NodeCollectionController.Direction? estimatedDirection = walker.LastMoveDelta.ToDirection();
            if (estimatedDirection is null)
            {
                return new TrafficLightSensorData(GuessDirection(sensedObject.transform.position.ToGrid()), walker.CurrentStopTime, walker.User);
            }
            else return new TrafficLightSensorData(estimatedDirection.Value, walker.CurrentStopTime, walker.User);
        }
        else
        {
            Debug.LogWarning("No Pathwalker, we just guessing what it is.");
            return new TrafficLightSensorData(GuessDirection(sensedObject.transform.position.ToGrid()), 0f, NodeCollectionController.TargetUser.Both);
        }
        
    }

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
