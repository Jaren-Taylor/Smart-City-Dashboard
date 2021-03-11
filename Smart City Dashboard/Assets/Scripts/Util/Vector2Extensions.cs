using UnityEngine;

public static class Vector2Extensions
{
    public static NodeCollectionController.Direction? ToDirection(this Vector2 delta)
    {
        Vector2 flatNorm = delta;
        flatNorm.Normalize();
        if (flatNorm.y > 0.707f) return NodeCollectionController.Direction.NorthBound;
        else if (flatNorm.y < -0.707f) return NodeCollectionController.Direction.SouthBound;
        else if (flatNorm.x > 0.707f) return NodeCollectionController.Direction.EastBound;
        else if (flatNorm.x < -0.707f) return NodeCollectionController.Direction.WestBound;
        else return null;
    }

    public static Vector3 ToGridVector3(this Vector2 position) => new Vector3(position.x, 0f, position.y);
    public static Vector3 ToGridVector3(this Vector2Int position) => new Vector3(position.x, 0f, position.y);
}

