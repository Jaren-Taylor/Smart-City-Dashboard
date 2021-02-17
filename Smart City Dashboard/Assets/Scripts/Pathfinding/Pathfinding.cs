using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinding 
{
    private class PathNode
    {
        public int X;
        public int Y;
        public Vector2Int Position { get => new Vector2Int(X, Y); set { X = value.x; Y = value.y; } }
        public Vector2Int Target { set => SetDistance(value); }

        public int Cost; //How many tiles have been traversed to get here
        public int Distance { get; private set; } //Manhattan distance to target
 
        public int CostDistance => Cost + Distance;

        public PathNode Parent; //What tile we came from

        public void SetDistance(Vector2Int target)
        {
            this.Distance = Mathf.Abs(target.x - X) + Mathf.Abs(target.y - Y);
        }
    }

    public static List<Vector2Int> PathFromTo(TileGrid grid, Vector2Int fromTile, Vector2Int toTile) 
    {
        var start = new PathNode() { Position = fromTile };
        var finish = new PathNode() { Position = toTile };

        start.SetDistance(toTile); //Calculates the distance the start tile is away from the end tile

        var activeNodes = new List<PathNode>();
        activeNodes.Add(start);
        var visitedNodes = new List<PathNode>();

        while (activeNodes.Any())
        {
            var checkNode = activeNodes.OrderBy(x => x.CostDistance).First();

            if(checkNode.X == finish.X && checkNode.Y == finish.Y)
            {
                return PathNodeToVectorList(checkNode);
            }

            visitedNodes.Add(checkNode);
            activeNodes.Remove(checkNode);

            var walkableNodes = GetWalkableNodes(grid, checkNode, finish);

            foreach(var walkableNode in walkableNodes)
            {
                //If this node has already been visited. 
                if (visitedNodes.Any(node => node.X == walkableNode.X && node.Y == walkableNode.Y)) 
                    continue; //Then skip it. Been there, done that.

                //If this node is in the active list
                if (activeNodes.Any(node => node.X == walkableNode.X && node.Y == walkableNode.Y))
                {
                    //Get the node that it's in the list
                    var existingNode = activeNodes.First(node => node.X == walkableNode.X && node.Y == walkableNode.Y);

                    //If the current cost for getting to this tile is better than a previously found tile
                    if(existingNode.CostDistance > walkableNode.CostDistance) //TODO : Check if this should be checkTile instead
                    { //Switch it out!
                        activeNodes.Remove(existingNode);
                        activeNodes.Add(walkableNode);
                    }
                }
                else
                {
                    //If I haven't see it, it's new to me!
                    activeNodes.Add(walkableNode);
                }
            }
        }

        throw new System.Exception("No tile-level path found!");

    }

    private static List<Vector2Int> PathNodeToVectorList(PathNode endNode)
    {
        if (endNode == null) return new List<Vector2Int>();
        Stack<Vector2Int> output = new Stack<Vector2Int>();
        PathNode node = endNode;
        do
        {
            output.Push(node.Position);
            node = node.Parent;
        } while (node != null);

        return output.ToList();
    }

    private static List<PathNode> GetWalkableNodes(TileGrid grid, PathNode current, PathNode target) 
    {
        var possibleTiles = new List<PathNode>();

        for(int i = 0; i < 4; i++) //For each direction (Tile.Facing enums can be cast from ints)
        {
            if (IsWalkableInDirection(grid, current.Position, (Tile.Facing)i)) //If walkable in said direction
                possibleTiles.Add(new PathNode() { Position = current.Position + Tile.Directions[i], Target = target.Position, Cost = current.Cost + 1, Parent = current}); //Add a node for that direction
        }

        return possibleTiles; //Returns nodes for walkable tiles
    }

    private static bool IsWalkableInDirection(TileGrid grid, Vector2Int current, Tile.Facing direction)
    {
        Tile currentTile = grid[current];
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
        return false;
    }
}


