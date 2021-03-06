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
    private const string MODEL_LOCATION = "";

    public NodeController[] NodeCollectionReference => NodeCollection;

    private float time = 0f;
    private bool isLocked = true;
    private Tile tile;

    public void Start()
    {
        time = UnityEngine.Random.Range(0f, 4f);
        isLocked = UnityEngine.Random.Range(0, 2) == 0;
        tile = GridManager.GetTile(transform.position.ToGridInt());
    }

    public void Update()
    {
        time += Time.deltaTime;
        if(time > 4f)
        {
            isLocked = !isLocked;
            time -= 4f;
        }
    }

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

    public static Direction GetDirectionFromDelta(Vector2Int To, Vector2Int From)
    {
        Vector2Int delta = To - From;
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

    private int ToIndex(int col, int row) => col + row * 4;

    public NodeController GetNode(int row, int col)
    {

        if (col < 4 & row < 4) return this.NodeCollection[ToIndex(col, row)];
        else { throw new IndexOutOfRangeException(); }
    }

    public bool TryGetPosition(NodeController controller, out int row, out int col)
    {
        for (int c = 0; c < 4; c++)
            for (int r = 0; r < 4; r++) 
                if (NodeCollection[ToIndex(c, r)] == controller)
                {
                    row = r;
                    col = c;
                    return true;
                }
        row = -1;
        col = -1;
        return false;
    }

    internal NodeController GetClosestNodeTo(Vector3 startingPosition) => NodeCollection.OrderBy(x => Vector3.Distance(x.Position, startingPosition)).First();

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
        //return !isLocked; //For debug only
        if (tile is RoadTile road && road.TrafficLight is TrafficLightController && road.TrafficLight.HasLight(enterDirection, out var light))
        {
            return (light == LightAnimationController.LightColor.Green);
        }
             
        
        return true; //TODO: Add something for traffic lights here
    }




    internal List<NodeController> GetEnterDrivewayPath(Direction enteringDirection, int position, Direction exitingDirection, TargetUser userType)
    {
        if (userType == TargetUser.Pedestrians) return GetPedestrianEnterDriveway(enteringDirection, position, exitingDirection);
        else return GetVehicleEnterDriveway(enteringDirection, position, exitingDirection);
    }

    private List<NodeController> GetVehicleEnterDriveway(Direction enteringDirection, int position, Direction exitingDirection)
    {
        List<NodeController> output = new List<NodeController>();
        NodeControllerCrawler crawler = new NodeControllerCrawler(GetInboundNodeFrom(enteringDirection, position), this);

        int intendedInboundPosition = (enteringDirection == Direction.NorthBound || enteringDirection == Direction.WestBound) ? 2 : 1;

        while (crawler.DirectionNeededToAlignPositionInDirection(enteringDirection, intendedInboundPosition) is Direction direction)
        {
            output.Add(crawler.Node);
            crawler.MoveIn(direction);
        }

        if (!crawler.CanMove(exitingDirection)) throw new Exception("Cannot enter and exit from same side");

        int intendedOutboundPosition = (exitingDirection == Direction.NorthBound || exitingDirection == Direction.WestBound) ? 1 : 2;

        if (enteringDirection == exitingDirection)
        {
            for(int i = 0; i < 2; i++)
            {
                output.Add(crawler.Node);
                crawler.MoveIn(enteringDirection);
            }

            output.Add(crawler.Node);
            crawler.MoveIn(crawler.DirectionNeededToAlignPositionInDirection(exitingDirection, intendedOutboundPosition).Value);

            output.Add(crawler.Node);
            crawler.MoveIn(exitingDirection);

            output.Add(crawler.Node);
        }
        else
        {
            while(crawler.DirectionNeededToAlignPositionInDirection(exitingDirection, intendedOutboundPosition) is Direction allignmentDirection)
            {
                output.Add(crawler.Node);
                crawler.MoveIn(allignmentDirection);
            }

            while (crawler.CanMove(exitingDirection))
            {
                output.Add(crawler.Node);
                crawler.MoveIn(exitingDirection);
            }

            output.Add(crawler.Node);
        }

        return output;
    }

    public List<NodeController> GetExitDrivewayPath(NodeController initialPosition, Direction exitingDirection)
    {
        List<NodeController> output = new List<NodeController>();
        NodeControllerCrawler crawler = new NodeControllerCrawler(initialPosition, this);
        if (crawler.Node is null) throw new IndexOutOfRangeException();

        output.Add(crawler.Node);

        while(crawler.CanMove(exitingDirection)) output.Add(crawler.MoveIn(exitingDirection));

        return output;

    }

    private List<NodeController> GetPedestrianEnterDriveway(Direction enteringDirection, int position, Direction exitingDirection)
    {
        List<NodeController> output = new List<NodeController>();
        NodeControllerCrawler crawler = new NodeControllerCrawler(GetInboundNodeFrom(enteringDirection, position), this);
        if (crawler.Node is null) throw new IndexOutOfRangeException();

        int exitingPosition = (exitingDirection == Direction.NorthBound || exitingDirection == Direction.WestBound) ? 2 : 1;

        if(crawler.DirectionNeededToMisallignPositionInDirection(exitingDirection, exitingPosition) is Direction misallignDirection)
        {
            while (crawler.CanMove(misallignDirection))
            {
                output.Add(crawler.Node);
                crawler.MoveIn(misallignDirection);
            }
        }

        while (crawler.CanMove(exitingDirection))
        {
            output.Add(crawler.Node);
            crawler.MoveIn(exitingDirection);
        }


        while(crawler.DirectionNeededToAlignPositionInDirection(exitingDirection, exitingPosition) is Direction direction)
        {
            output.Add(crawler.Node);
            crawler.MoveIn(direction);
        }

        output.Add(crawler.Node);

        return output;
    }

    internal List<NodeController> GetFinalPath(NodeCollectionController.Direction enteringDirection, int position, TargetUser userType)
    {
        List<NodeController> output = new List<NodeController>();
        NodeControllerCrawler crawler = new NodeControllerCrawler(GetInboundNodeFrom(enteringDirection, position), this);
        if (crawler.Node is null) throw new IndexOutOfRangeException();
        output.Add(crawler.Node);

        for(int i = 0; i < 2; i++) output.Add(crawler.MoveIn(enteringDirection));

        return output;
        
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

    private class NodeControllerCrawler
    {
        private NodeController node;
        private NodeCollectionController controller;
        private int row;
        private int col;

        public NodeController Node => node;
        public int Row => row;
        public int Col => col;

        public NodeControllerCrawler(NodeController node, NodeCollectionController controller)
        {
            this.controller = controller;
            this.node = node;
            if (!controller.TryGetPosition(node, out row, out col)) throw new IndexOutOfRangeException("Cannot find node");
        }

        public NodeControllerCrawler(int row, int col, NodeCollectionController controller)
        {
            this.controller = controller;
            this.node = controller.GetNode(row, col);
            this.row = row;
            this.col = col;
        }

        public bool CanMove(Direction direction)
        {
            return direction switch
            {
                Direction.EastBound => col < 3,
                Direction.WestBound => col > 0,
                Direction.NorthBound => row < 3,
                Direction.SouthBound => row > 0,
                _ => false,
            };
        }
    
        public NodeController MoveIn(Direction direction)
        {
            if (CanMove(direction))
            {
                return direction switch
                {
                    Direction.EastBound => SetNodeToValue(row, col + 1),
                    Direction.WestBound => SetNodeToValue(row, col - 1),
                    Direction.NorthBound => SetNodeToValue(row + 1, col),
                    Direction.SouthBound => SetNodeToValue(row - 1, col),
                    _ => throw new Exception("Unknown direction"),
                };
            }
            else return null;
        }



        public Direction? DirectionNeededToMisallignPositionInDirection(Direction direction, int targetPosition)
        {
            if (node is null) return null;
            else
            {
                if (direction == Direction.NorthBound || direction == Direction.SouthBound) //Position is in terms of columns
                {
                    int currentPosition = col;
                    if (currentPosition < targetPosition) return Direction.WestBound;
                    else if (currentPosition > targetPosition) return Direction.EastBound;
                    else
                    {
                        switch (currentPosition)
                        {
                            case 0: case 2:
                                return Direction.EastBound;
                            case 1: case 3:
                                return Direction.WestBound;
                            default:
                                throw new IndexOutOfRangeException();
                        }
                    }
                }
                else //Position is in terms of rows
                {
                    int currentPosition = row;
                    if (currentPosition < targetPosition) return Direction.SouthBound;
                    else if (currentPosition > targetPosition) return Direction.NorthBound;
                    else
                    {
                        switch (currentPosition)
                        {
                            case 0:
                            case 2:
                                return Direction.NorthBound;
                            case 1:
                            case 3:
                                return Direction.SouthBound;
                            default:
                                throw new IndexOutOfRangeException();
                        }
                    }
                }
            }
        }

        public Direction? DirectionNeededToAlignPositionInDirection(Direction direction, int targetPosition)
        {
            if (node is null) return null;
            else
            {
                if(direction == Direction.NorthBound || direction == Direction.SouthBound) //Position is in terms of columns
                {
                    int currentPosition = col;
                    if (currentPosition < targetPosition) return Direction.EastBound;
                    else if (currentPosition > targetPosition) return Direction.WestBound;
                    else return null;
                }
                else //Position is in terms of rows
                {
                    int currentPosition = row;
                    if (currentPosition < targetPosition) return Direction.NorthBound;
                    else if (currentPosition > targetPosition) return Direction.SouthBound;
                    else return null;
                }
            }
        }

        private NodeController SetNodeToValue(int row, int col)
        {
            this.row = row;
            this.col = col;
            if (row.IsBetween(0, 3) && col.IsBetween(0, 3)) this.node = controller.GetNode(row, col);
            else this.node = null;
            return node;
        }
    }
}
