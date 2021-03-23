using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding 
{
    private static Vector2Int Target;

    private static readonly Vector2Int NullParent = new Vector2Int(-1, -1);

    private static Dictionary<Vector2Int, PathNode> activeNodes = new Dictionary<Vector2Int, PathNode>();
    private static LinkedList<Vector2Int> costDistSortedActiveNodes = new LinkedList<Vector2Int>();
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
        costDistSortedActiveNodes.Clear();

        activeNodes.Add(fromTile, start);
        costDistSortedActiveNodes.AddFirst(fromTile);

        while (activeNodes.Count > 0)
        {
            var checkPos = costDistSortedActiveNodes.First.Value;
            var checkNode = activeNodes[checkPos];
                //FindSmallestCostDistance(activeNodes);
                //activeNodes.OrderBy(x => x.Value.CostDistance).First().Value;

            if (checkNode.X == Target.x && checkNode.Y == Target.y)
            {
                return PathNodeToVectorList(checkNode);
            }

            visitedNodes.Add(checkPos, checkNode);
            activeNodes.Remove(checkPos);
            costDistSortedActiveNodes.RemoveFirst();

            int nextCost = checkNode.Cost + 1;

            Tile currentTile = grid[checkPos];

            if (currentTile is null || currentTile.IsPermanent is false) continue;

            for(int i = 0; i < 4; i++)
            {
                if(IsWalkableInDirectionReduced(grid, currentTile, checkNode.Position, (Tile.Facing)i))
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

                            //Resort this node's standing
                            LinkedListNode<Vector2Int> targetSwapNode;
                            LinkedListNode<Vector2Int> currSwapNode = costDistSortedActiveNodes.First;
                            while(activeNodes[currSwapNode.Value].CostDistance < costDistance)
                            {
                                currSwapNode = currSwapNode.Next;
                            }
                            targetSwapNode = currSwapNode;
                            while(currSwapNode.Value != walkedPosition)
                            {
                                currSwapNode = currSwapNode.Next;
                            }
                            if(currSwapNode != targetSwapNode)
                            {
                                costDistSortedActiveNodes.Remove(currSwapNode);
                                costDistSortedActiveNodes.AddBefore(targetSwapNode, currSwapNode);
                            }
                            activeNodes[walkedPosition] = existingNode;
                        }
                    }
                    else
                    {
                        //If I haven't see it, it's new to me!
                        var newNode = new PathNode(walkedPosition, checkPos, nextCost); //Allocate new data

                        activeNodes.Add(walkedPosition, newNode); 
                        int costDistance = newNode.CostDistance;

                        //Sort this node's standing
                        LinkedListNode<Vector2Int> currPos = costDistSortedActiveNodes.First;
                        while (currPos != null && activeNodes[currPos.Value].CostDistance < costDistance)
                        {
                            currPos = currPos.Next;
                        }
                        if(currPos == null) costDistSortedActiveNodes.AddLast(walkedPosition);
                        else costDistSortedActiveNodes.AddBefore(currPos, walkedPosition);
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
            if (currentTile is BuildingTile buildingTile)
            {
                if (buildingTile.currentFacing == direction && grid[current + Tile.Directions[(int)direction]] is RoadTile) return true;
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

    private static bool IsWalkableInDirectionReduced(TileGrid grid, Tile currentTile, Vector2Int current, Tile.Facing direction)
    {
        if (currentTile is BuildingTile buildingTile)
        {
            return buildingTile.currentFacing == direction;
        }
        else if (currentTile is RoadTile)
        {
            var tilePos = current + Tile.Directions[(int)direction];
            Tile tileInDirection = grid[tilePos];
            if (tileInDirection is RoadTile) return true;
            else if (tileInDirection is BuildingTile && ((BuildingTile)tileInDirection).currentFacing == Tile.OppositeDirection(direction)) return true;
        }
        return false;
    }
}


