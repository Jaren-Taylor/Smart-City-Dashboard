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

    public NodeController GetInboundNodeFrom(Direction inboundDirection, int position) => inboundDirection switch
    {
        Direction.NorthBound => GetNode(0, position),
        Direction.EastBound => GetNode(position, 0),
        Direction.WestBound => GetNode(position, 3),
        _ => GetNode(3, position),
    };

    public NodeController GetNode(int row, int col)
    {
        if(col < 4 & row < 4) return this.NodeCollection[col + row * 4];
        else { throw new IndexOutOfRangeException(); }
    }

    /// <summary>
    /// Gets column or run number of node depending on the exiting direction
    /// </summary>
    internal int GetPositionFrom(Direction currentExitDirection, NodeController currentNode) => currentExitDirection switch
    {
        Direction.NorthBound => Array.IndexOf(NodeCollection, currentNode) % 4, //Returns column number
        Direction.EastBound => Array.IndexOf(NodeCollection, currentNode) / 4, //Returns row number
        Direction.WestBound => Array.IndexOf(NodeCollection, currentNode) / 4, //Returns row number
        _ => Array.IndexOf(NodeCollection, currentNode) % 4 //Returns column number
    };

    internal NodeController GetVehicleSpawnNode(Tile tile)
    {
        if(tile is BuildingTile building)
        {
            switch (building.currentFacing)
            {
                case Tile.Facing.Left:
                    return GetNode(2, 1);
                case Tile.Facing.Right:
                    return GetNode(1, 2);
                case Tile.Facing.Top:
                    return GetNode(2, 2);
                default :
                    return GetNode(1, 1);

            
            }
        }
        else
        {
            return GetNode(2, 2);
        }

    }

    internal bool CanEnterFromPosition(Direction enterDirection, int position)
    {
        return true; //TODO: Add something for traffic lights here
    }

    internal NodeController GetPedestrianSpawnNode(Tile tile)
    {
        if (tile is BuildingTile building)
        {
            switch (building.currentFacing)
            {
                case Tile.Facing.Left:
                    return GetNode(1, 1);
                case Tile.Facing.Right:
                    return GetNode(2, 2);
                case Tile.Facing.Top:
                    return GetNode(2, 1);
                default:
                    return GetNode(1, 2);


            }
        }
        else
        {
            return GetNode(3, 0);
        }

    }
}
