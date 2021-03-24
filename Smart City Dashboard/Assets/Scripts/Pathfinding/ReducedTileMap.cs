using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReducedTileMap 
{
    private readonly int height;
    private readonly int width;
    private readonly TileGrid grid;

    private static Vector2Int Target;
    private Vector2Int? FromTilePos = null;
    private Vector2Int? FirstRoadTilePos = null;

    //These are all of the roads that either have 1, 3, or 4 neighbors that are roads OR is a corner
    private Dictionary<Vector2Int, TileIntersectionNode> interestsToNodes = new Dictionary<Vector2Int, TileIntersectionNode>();
    private HashSet<Vector2Int> pointsOfInterests = new HashSet<Vector2Int>();
    private HashSet<TileIntersectionNode> activeNodes = new HashSet<TileIntersectionNode>();
    private LinkedList<TileIntersectionNode> costDistSortedActiveNodes = new LinkedList<TileIntersectionNode>();


    public ReducedTileMap(TileGrid grid)
    {
        this.grid = grid;
        this.height = grid.Height;
        this.width = grid.Width;
        CalculateGraph();
    }

    public void CalculateGraph()
    {
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++) 
            {
                Vector2Int position = new Vector2Int(x, y);

                Tile currentTile = grid[position];

                if (currentTile is RoadTile roadTile && roadTile.IsPermanent is true)
                {
                    var neighbors = grid.GetNeighbors(position);
                    if(neighbors.GetRoadCount() == 2)
                    {
                        //If one of them is a road and one isn't, then this is a corner
                        if(neighbors.left is RoadTile ^ neighbors.right is RoadTile)
                        {
                            pointsOfInterests.Add(position);
                            interestsToNodes.Add(position, new TileIntersectionNode() { Position = position });
                        }
                    }
                    else
                    {
                        pointsOfInterests.Add(position);
                        interestsToNodes.Add(position, new TileIntersectionNode() { Position = position });
                    }
                }
            }
        }

        //Points of interests is now full of all corners and intersections

        foreach (Vector2Int point in pointsOfInterests)
        {
            Vector2Int initPoint = point;
            var initNode = interestsToNodes[initPoint];
            for (int i = 0; i < 4; i++)
            {
                Tile.Facing facing = Tile.Facings[i];
                if (initNode.Connections.ContainsKey(facing)) continue; //Checks to see if the connection was already made
                
                Vector2Int directionDelta = Tile.Directions[i];
                Vector2Int endPoint = initPoint + directionDelta;
                if(grid[endPoint] is RoadTile)
                {
                    //Move until reached next interest point (they are all in straight lines from the initial nodes
                    while (!pointsOfInterests.Contains(endPoint))
                    {
                        endPoint += directionDelta;
                    }

                    var endNode = interestsToNodes[endPoint];
                    int cost = Mathf.Max(Mathf.Abs(endNode.Position.x - initNode.Position.x), Mathf.Abs(endNode.Position.y - initNode.Position.y));

                    //Add a two way connection to the nodes
                    initNode.Connections.Add(facing, endNode);
                    initNode.ConnectionCost.Add(facing, cost);
                    endNode.Connections.Add(facing.Oppisite(), initNode);
                    endNode.ConnectionCost.Add(facing.Oppisite(), cost);
                }
            }
        }
    }

    public LinkedList<Vector2Int> GetListOfPositionsFromToReduced(Vector2Int fromTile, Vector2Int toTile)
    {
        LinkedList<Vector2Int> output = new LinkedList<Vector2Int>();

        foreach(var kvp in interestsToNodes)
        {
            kvp.Value.visited = false;
            kvp.Value.ParentNode = null;
            kvp.Value.ToParentDelta = Vector2Int.zero;
        }

        activeNodes.Clear();
        costDistSortedActiveNodes.Clear();

        output.AddLast(toTile);

        var frontOfToTile = toTile + ((BuildingTile)grid[toTile]).currentFacing.ToVector2();

        Target = frontOfToTile;

        FromTilePos = fromTile;
        FirstRoadTilePos = null; 

        if (grid[fromTile] is BuildingTile buildingTile)
        {
            var direction = buildingTile.currentFacing;
            var firstRoadPos = fromTile + direction.ToVector2();
            if (!pointsOfInterests.Contains(firstRoadPos))
            {
                FirstRoadTilePos = fromTile + direction.ToVector2();
                var rightDir = direction.TurnRight();
                var rightDelta = rightDir.ToVector2();
                Vector2Int rightSearch = firstRoadPos + rightDelta;
                if (grid[rightSearch] is RoadTile)
                {
                    int cost = 2;
                    while (!pointsOfInterests.Contains(rightSearch))
                    {
                        rightSearch += rightDelta;
                        cost++;
                    }

                    var rightNode = interestsToNodes[rightSearch];
                    rightNode.Cost = cost;
                    rightNode.SetDistance();
                    rightNode.ToParentDelta = rightDelta * -1;
                    activeNodes.Add(rightNode);
                    costDistSortedActiveNodes.AddFirst(rightNode);
                }

                var leftDir = direction.TurnLeft();
                var leftDelta = leftDir.ToVector2();
                Vector2Int leftSearch = firstRoadPos + leftDelta;
                if (grid[leftSearch] is RoadTile)
                {
                    int cost = 2;
                    while (!pointsOfInterests.Contains(leftSearch))
                    {
                        leftSearch += leftDelta;
                        cost++;
                    }

                    var leftNode = interestsToNodes[leftSearch];
                    leftNode.Cost = cost;
                    leftNode.SetDistance();
                    leftNode.ToParentDelta = leftDelta * -1;
                    activeNodes.Add(leftNode);
                    if (costDistSortedActiveNodes.Count == 0 || costDistSortedActiveNodes.First.Value.CostDistance > leftNode.CostDistance) costDistSortedActiveNodes.AddFirst(leftNode);
                    else costDistSortedActiveNodes.AddLast(leftNode);
                }
            }
            else
            {
                var node = interestsToNodes[firstRoadPos];
                node.Cost = 1;
                node.SetDistance();
                activeNodes.Add(node);
                costDistSortedActiveNodes.AddFirst(node);
            }

            while(costDistSortedActiveNodes.Count > 0)
            {
                var checkNode = costDistSortedActiveNodes.First.Value;
                var checkPos = checkNode.Position;

                //Check to see if node is along a connection

                checkNode.visited = true;
                costDistSortedActiveNodes.RemoveFirst();
                activeNodes.Remove(checkNode);

                foreach(var curFacing in checkNode.ConnectionCost.Keys)
                {
                    if (!checkNode.Connections[curFacing].visited)
                    {
                        if(!IsTargetBetweenIntersections(checkNode, curFacing))
                        {
                            var nextNode = checkNode.Connections[curFacing];
                            if (activeNodes.Contains(nextNode))
                            {
                                //Try updating cost
                                int cost = checkNode.Cost + checkNode.ConnectionCost[curFacing];
                                int costDistance = nextNode.Distance + cost;

                                if(costDistance < nextNode.CostDistance)
                                {
                                    nextNode.Cost = cost;
                                    nextNode.ParentNode = checkNode;
                                    nextNode.ToParentDelta = curFacing.ToVector2() * -1;

                                    //Resort this node's standing
                                    LinkedListNode<TileIntersectionNode> targetSwapNode;
                                    LinkedListNode<TileIntersectionNode> currSwapNode = costDistSortedActiveNodes.First;
                                    while (currSwapNode.Value.CostDistance < costDistance)
                                    {
                                        currSwapNode = currSwapNode.Next;
                                    }
                                    targetSwapNode = currSwapNode;
                                    while (currSwapNode.Value != nextNode)
                                    {
                                        currSwapNode = currSwapNode.Next;
                                    }
                                    if (currSwapNode != targetSwapNode)
                                    {
                                        costDistSortedActiveNodes.Remove(currSwapNode);
                                        costDistSortedActiveNodes.AddBefore(targetSwapNode, currSwapNode);
                                    }
                                }
                            }
                            else
                            {
                                //This node is new, so add it.
                                nextNode.Cost = checkNode.Cost + checkNode.ConnectionCost[curFacing];
                                nextNode.SetDistance();
                                nextNode.ParentNode = checkNode;
                                nextNode.ToParentDelta = curFacing.ToVector2() * -1;
                                activeNodes.Add(nextNode);

                                int costDistance = nextNode.CostDistance;

                                var curSortingNode = costDistSortedActiveNodes.First;
                                while (curSortingNode != null && curSortingNode.Value.CostDistance < costDistance)
                                {
                                    curSortingNode = curSortingNode.Next;
                                }
                                if (curSortingNode == null) costDistSortedActiveNodes.AddLast(nextNode);
                                else costDistSortedActiveNodes.AddBefore(curSortingNode, nextNode);
                            }
                        }
                        else //Found target
                        {
                            return ReturnPathList(output, checkNode, curFacing);
                        }
                    }
                }
            }
        }
        return null;
    }

    private LinkedList<Vector2Int> ReturnPathList(LinkedList<Vector2Int> outputList, TileIntersectionNode lastWaypoint, Tile.Facing lastDir)
    {
        Vector2Int tilePos = Target;
        Vector2Int initalDelta = lastDir.ToVector2() * -1;
        while(tilePos != lastWaypoint.Position)
        {
            outputList.AddFirst(tilePos);
            tilePos += initalDelta;
        }

        TileIntersectionNode waypoint = lastWaypoint;
        while(waypoint.ParentNode != null)
        {
            LineToParent(outputList, waypoint);
            waypoint = waypoint.ParentNode;
        }

        if(FirstRoadTilePos != null)
        {
            tilePos = waypoint.Position;
            Vector2Int parentPos = FirstRoadTilePos.Value;
            while (tilePos != parentPos)
            {
                outputList.AddFirst(tilePos);
                tilePos += waypoint.ToParentDelta;
            }

            outputList.AddFirst(parentPos);
        }
        else
        {
            outputList.AddFirst(waypoint.Position);
        }
        
        outputList.AddFirst(FromTilePos.Value);
        return outputList;
    }

    private void LineToParent(LinkedList<Vector2Int> outputList, TileIntersectionNode waypoint)
    {
        Vector2Int tilePos = waypoint.Position;
        Vector2Int parentPos = waypoint.ParentNode.Position;
        while(tilePos != parentPos)
        {
            outputList.AddFirst(tilePos);
            tilePos += waypoint.ToParentDelta;
        }
    }

    private bool IsTargetBetweenIntersections(TileIntersectionNode intersection, Tile.Facing direction)
    {
        Vector2Int fromPos = intersection.Position;
        Vector2Int toPos = intersection.Connections[direction].Position;
        switch (direction)
        {
            case Tile.Facing.Bottom:
                return (fromPos.x == Target.x) && (fromPos.y >= Target.y) && (Target.y >= toPos.y);
            case Tile.Facing.Top:
                return (fromPos.x == Target.x) && (fromPos.y <= Target.y) && (Target.y <= toPos.y);
            case Tile.Facing.Left:
                return (fromPos.y == Target.y) && (fromPos.x >= Target.x) && (Target.x >= toPos.x);
            case Tile.Facing.Right:
                return (fromPos.y == Target.y) && (fromPos.x <= Target.x) && (Target.x <= toPos.x);
        }

        return false;
    }

    private bool IsBetweenIntersections(TileIntersectionNode intersection, Tile.Facing direction, Vector2Int pos)
    {
        Vector2Int fromPos = intersection.Position;
        Vector2Int toPos = intersection.Connections[direction].Position;
        switch (direction)
        {
            case Tile.Facing.Bottom:
                return (fromPos.x == pos.x) && (fromPos.y >= pos.y) && (pos.y >= toPos.y);            
            case Tile.Facing.Top:
                return (fromPos.x == pos.x) && (fromPos.y <= pos.y) && (pos.y <= toPos.y);
            case Tile.Facing.Left:
                return (fromPos.y == pos.y) && (fromPos.x >= pos.x) && (pos.x >= toPos.x);
            case Tile.Facing.Right:
                return (fromPos.y == pos.y) && (fromPos.x <= pos.x) && (pos.x <= toPos.x);
        }

        return false;
    }

    private class TileIntersectionNode
    {
        public int CostDistance => Cost + Distance;
        public int Cost = -1;
        public int Distance { get; private set; } = -1;
        public bool visited = false;
        public Dictionary<Tile.Facing, TileIntersectionNode> Connections = new Dictionary<Tile.Facing, TileIntersectionNode>();
        public Dictionary<Tile.Facing, int> ConnectionCost = new Dictionary<Tile.Facing, int>();
        public Vector2Int Position;
        public TileIntersectionNode ParentNode = null;
        public Vector2Int ToParentDelta = Vector2Int.zero;

        public void SetDistance() => Distance = Mathf.Abs(Target.x - Position.x) + Mathf.Abs(Target.y - Position.y);
    }

    private enum TileData
    {
        Road,
        House
    }
}
