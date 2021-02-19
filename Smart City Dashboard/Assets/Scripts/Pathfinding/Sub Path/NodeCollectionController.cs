using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Schema;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodeCollectionController : MonoBehaviour
{
    [SerializeField]
    private NodeController[] NodeCollection;

    public NodeController[] NodeCollectionReference => NodeCollection;

    /// <summary>
    /// The type of user that can traverse the node
    /// </summary>
    public enum TargetUser
    {
        Vehicles,
        Pedestrians,
        Both // for areas which will occasionally need to be traversed by both entity types
    }
    /// <summary>
    /// The direction that the entity will exit the tile
    /// </summary>
    public enum Direction
    {
        WestBound = 0,
        EastBound = 1,
        NorthBound = 2,
        SouthBound = 3
    }

    public static Direction GetDirectionFromDelta(Vector2Int tile1, Vector2Int tile2)
    {
        Vector2Int delta = tile1 - tile2;
        //Get direction from movement delta
        if (delta.x == 1 && delta.y == 0) return Direction.EastBound;
        else if (delta.x == -1 && delta.y == 0) return Direction.WestBound;
        else if (delta.x == 0 && delta.y == 1) return Direction.NorthBound;
        else if (delta.x == 0 && delta.y == -1) return Direction.SouthBound;
        else throw new Exception("Path invalid"); //Delta is wonky
    }

    public NodeController GetNode(int row, int col)
    {
        if(col < 4 & row < 4) return this.NodeCollection[col + row * 4];
        else { throw new IndexOutOfRangeException(); }
    }

    public NodeController GetSpawnNode(Direction direction, TargetUser user)
    {
        return GetInboundNode(direction, user); //Temp fix
    }

    public NodeController GetInboundNode(Direction direction, TargetUser user)
    {
        switch (direction)
        {
            case Direction.NorthBound:
                return GetNode(0, 2);
            case Direction.EastBound:
                return GetNode(1, 0);
            case Direction.WestBound:
                return GetNode(2, 3);
            default:
                return GetNode(3, 1);
        }
    }
}
