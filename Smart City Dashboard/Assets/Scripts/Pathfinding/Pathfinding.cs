using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinding 
{
    private static Vector2Int Target;

    private static readonly Vector2Int NullParent = new Vector2Int(-1, -1);

    private static Dictionary<Vector2Int, PathNode> activeNodes = new Dictionary<Vector2Int, PathNode>();
    private static Dictionary<Vector2Int, PathNode> visitedNodes = new Dictionary<Vector2Int, PathNode>();

    private struct PathNode
    {
        public int X => Position.x;
        public int Y => Position.y;
        public Vector2Int Position { get; set; }

        public int Cost; //How many tiles have been traversed to get here
        public int Distance { get; private set; } //Manhattan distance to target
        public int CostDistance => Cost + Distance;

        public Vector2Int Parent; //What tile we came from

        public PathNode(Vector2Int position, Vector2Int parent, int cost)
        {
            Position = position;
            Parent = parent;
            Cost = cost;
            Distance = Mathf.Abs(Target.x - position.x) + Mathf.Abs(Target.y - position.y);
        }

        public static int CalculateDistanceToTarget(Vector2Int position)
        {
            return Mathf.Abs(Target.x - position.x) + Mathf.Abs(Target.y - position.y);
        }
    }

    private static PathNode FindSmallestCostDistance(Dictionary<Vector2Int, PathNode> pathNodes)
    {
        int smallest = -1;
        Vector2Int position = Vector2Int.zero;
        foreach(var kvp in pathNodes)
        {
            if(smallest == -1)
            {
                smallest = kvp.Value.CostDistance;
                position = kvp.Key;
            }
            else
            {
                int cd = kvp.Value.CostDistance;
                if (cd < smallest)
                {
                    smallest = cd;
                    position = kvp.Key;
                }
            }
        }

        return pathNodes[position];
    }


    public static LinkedList<Vector2Int> GetListOfPositionsFromToReduced(Vector2Int fromTile, Vector2Int toTile)
    {
        var grid = GridManager.Instance.Grid;

        Target = toTile;

        var start = new PathNode(fromTile, new Vector2Int(-1, -1), 0);
        //var finish = new PathNode() { Position = toTile };
        activeNodes.Clear();
        visitedNodes.Clear();

        activeNodes.Add(fromTile, start);

        while (activeNodes.Count > 0)
        {
            var checkNode = FindSmallestCostDistance(activeNodes);
                //activeNodes.OrderBy(x => x.Value.CostDistance).First().Value;

            if (checkNode.X == Target.x && checkNode.Y == Target.y)
            {
                return PathNodeToVectorList(checkNode);
            }

            visitedNodes.Add(checkNode.Position,checkNode);
            activeNodes.Remove(checkNode.Position);

            int nextCost = checkNode.Cost + 1;

            for(int i = 0; i < 4; i++)
            {
                if(IsWalkableInDirection(grid, checkNode.Position, (Tile.Facing)i))
                {
                    var walkedPosition = checkNode.Position + Tile.Directions[i];

                    //If this node has already been visited. 
                    if (visitedNodes.ContainsKey(walkedPosition))
                        continue; //Then skip it. Been there, done that.

                    if(activeNodes.TryGetValue(walkedPosition, out PathNode existingNode))
                    {
                        //Get the node that it's in the list
                        //var existingNode = activeNodes.First(node => node.X == walkedPosition.x && node.Y == walkedPosition.y);

                        int distance = PathNode.CalculateDistanceToTarget(walkedPosition);
                        int costDistance = distance + nextCost;

                        //If the current cost for getting to this tile is better than a previously found tile
                        if (existingNode.CostDistance > costDistance)
                        {
                            //Swap out parent and cost
                            existingNode.Cost = nextCost;
                            existingNode.Parent = checkNode.Position;
                        }
                    }
                    else
                    {
                        //If I haven't see it, it's new to me!
                        activeNodes.Add(walkedPosition,
                            new PathNode() { 
                                Position = walkedPosition,
                                Parent = checkNode.Position,
                                Cost = nextCost
                            }); //Allocate new data
                    }
                }
            }
        }

        return null;

    }

    private static LinkedList<Vector2Int> PathNodeToVectorList(PathNode endNode)
    {
        if (endNode.Parent == NullParent) return null;
        LinkedList<Vector2Int> output = new LinkedList<Vector2Int>();
        output.AddFirst(endNode.Position);
        Vector2Int position = endNode.Parent;
        do
        {
            output.AddFirst(position);
            position = visitedNodes[position].Parent;
        } while (position != NullParent);

        return output;
    }

    private static bool IsWalkableInDirection(TileGrid grid, Vector2Int current, Tile.Facing direction)
    {
        if (grid[current] is Tile currentTile)
        {
            if (!currentTile.IsPermanent)
                return false;
            if (currentTile is BuildingTile)
            {
                if (((BuildingTile)currentTile).currentFacing == direction && grid[current + Tile.Directions[(int)direction]] is RoadTile) return true;
            }
            else if (currentTile is RoadTile)
            {
                var tilePos = current + Tile.Directions[(int)direction];
                Tile tileInDirection = grid[tilePos];
                if (tileInDirection is RoadTile) return true;
                else if (tileInDirection is BuildingTile && ((BuildingTile)tileInDirection).currentFacing == Tile.OppositeDirection(direction)) return true;
            }
        }
        return false;
    }
}


