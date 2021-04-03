using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReducedTileMap 
{
    private static int height;
    private static int width;
    private static TileGrid grid;

    private static Vector2Int Target;
    private Vector2Int? FromTilePos = null;
    private Vector2Int? FirstRoadTilePos = null;

    //These are all of the roads that either have 1, 3, or 4 neighbors that are roads OR is a corner
    private Dictionary<Vector2Int, TileIntersectionNode> interestsToNodes = new Dictionary<Vector2Int, TileIntersectionNode>();
    private HashSet<Vector2Int> pointsOfInterests = new HashSet<Vector2Int>();
    private HashSet<TileIntersectionNode> activeNodes = new HashSet<TileIntersectionNode>();
    private LinkedList<TileIntersectionNode> costDistSortedActiveNodes = new LinkedList<TileIntersectionNode>();

    private readonly Queue<TileIntersectionNode> pool = new Queue<TileIntersectionNode>();

    private readonly Dictionary<Tile.Facing, Func<Vector2Int, bool>> facingBoundCheck = new Dictionary<Tile.Facing, Func<Vector2Int, bool>>()
    { 
        {Tile.Facing.Top, (x) => x.y < height },
        {Tile.Facing.Bottom, (x) => x.y >= 0 },
        {Tile.Facing.Left, (x) => x.x >= 0 },
        {Tile.Facing.Right, (x) => x.x < width }
    };


    public ReducedTileMap(TileGrid grid)
    {
        ReducedTileMap.grid = grid;
        ReducedTileMap.height = grid.Height;
        ReducedTileMap.width = grid.Width;
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

    public void AddRoad(Vector2Int position)
    {
        var neigh = grid.GetNeighbors(position);

        Vector2Int topCandidate = position + Tile.Facing.Top.ToVector2();
        Vector2Int bottomCandidate = position + Tile.Facing.Bottom.ToVector2();
        Vector2Int leftCandidate = position + Tile.Facing.Left.ToVector2();
        Vector2Int rightCandidate = position + Tile.Facing.Right.ToVector2();
        bool topFound = interestsToNodes.TryGetValue(topCandidate, out var topNode);
        bool bottomFound = interestsToNodes.TryGetValue(bottomCandidate, out var bottomNode);
        bool leftFound = interestsToNodes.TryGetValue(leftCandidate, out var leftNode);
        bool rightFound = interestsToNodes.TryGetValue(rightCandidate, out var rightNode);

        TileIntersectionNode newIntersection = null;

        if (topFound)
        {
            if (newIntersection is null) newIntersection = new TileIntersectionNode() { Position = position };
            newIntersection.Connections.Add(Tile.Facing.Top, topNode);
            newIntersection.ConnectionCost.Add(Tile.Facing.Top, 1);
            topNode.Connections.Add(Tile.Facing.Bottom, newIntersection);
            topNode.ConnectionCost.Add(Tile.Facing.Bottom, 1);
        }
        else
        {
            if (neigh.top is RoadTile && TryFindHorizontalConnectionCandidate(topCandidate, out Vector2Int connectedPosition, out Tile.Facing direction))
            {
                if (newIntersection is null) newIntersection = new TileIntersectionNode() { Position = position };
                TileIntersectionNode connectedNode = interestsToNodes[connectedPosition];
                topNode = InsertNodeAlongConnection(connectedNode, direction, topCandidate);
                ConnectNodes(newIntersection, topNode, Tile.Facing.Top);
                interestsToNodes.Add(topCandidate, topNode);
                pointsOfInterests.Add(topCandidate);
            }
        }

        if (bottomFound)
        {
            if (newIntersection is null) newIntersection = new TileIntersectionNode() { Position = position };
            newIntersection.Connections.Add(Tile.Facing.Bottom, bottomNode);
            newIntersection.ConnectionCost.Add(Tile.Facing.Bottom, 1);
            bottomNode.Connections.Add(Tile.Facing.Top, newIntersection);
            bottomNode.ConnectionCost.Add(Tile.Facing.Top, 1);
        }
        else
        {
            if (neigh.bottom is RoadTile && TryFindHorizontalConnectionCandidate(bottomCandidate, out Vector2Int connectedPosition, out Tile.Facing direction))
            {
                if (newIntersection is null) newIntersection = new TileIntersectionNode() { Position = position };
                TileIntersectionNode connectedNode = interestsToNodes[connectedPosition];
                bottomNode = InsertNodeAlongConnection(connectedNode, direction, bottomCandidate);
                ConnectNodes(newIntersection, bottomNode, Tile.Facing.Bottom);
                interestsToNodes.Add(bottomCandidate, bottomNode);
                pointsOfInterests.Add(bottomCandidate);
            }
        }

        if (leftFound)
        {
            if (newIntersection is null) newIntersection = new TileIntersectionNode() { Position = position };
            newIntersection.Connections.Add(Tile.Facing.Left, leftNode);
            newIntersection.ConnectionCost.Add(Tile.Facing.Left, 1);
            leftNode.Connections.Add(Tile.Facing.Right, newIntersection);
            leftNode.ConnectionCost.Add(Tile.Facing.Right, 1);
        }
        else
        {
            if (neigh.left is RoadTile && TryFindVerticalConnectionCandidate(leftCandidate, out Vector2Int connectedPosition, out Tile.Facing direction))
            {
                if (newIntersection is null) newIntersection = new TileIntersectionNode() { Position = position };
                TileIntersectionNode connectedNode = interestsToNodes[connectedPosition];
                leftNode = InsertNodeAlongConnection(connectedNode, direction, leftCandidate);
                ConnectNodes(newIntersection, leftNode, Tile.Facing.Left);
                interestsToNodes.Add(leftCandidate, leftNode);
                pointsOfInterests.Add(leftCandidate);
            }
        }

        if (rightFound)
        {
            if (newIntersection is null) newIntersection = new TileIntersectionNode() { Position = position };
            newIntersection.Connections.Add(Tile.Facing.Right, rightNode);
            newIntersection.ConnectionCost.Add(Tile.Facing.Right, 1);
            rightNode.Connections.Add(Tile.Facing.Left, newIntersection);
            rightNode.ConnectionCost.Add(Tile.Facing.Left, 1);
        }
        else
        {
            if (neigh.right is RoadTile && TryFindVerticalConnectionCandidate(rightCandidate, out Vector2Int connectedPosition, out Tile.Facing direction))
            {
                if (newIntersection is null) newIntersection = new TileIntersectionNode() { Position = position };
                TileIntersectionNode connectedNode = interestsToNodes[connectedPosition];
                rightNode = InsertNodeAlongConnection(connectedNode, direction, rightCandidate);
                ConnectNodes(newIntersection, rightNode, Tile.Facing.Right);
                interestsToNodes.Add(rightCandidate, rightNode);
                pointsOfInterests.Add(rightCandidate);
            }
        }

        if (newIntersection is null) newIntersection = new TileIntersectionNode() { Position = position };
        interestsToNodes.Add(position, newIntersection);
        pointsOfInterests.Add(position);

        if (topNode != null) TryDissolveNode(topNode);
        if (bottomNode != null) TryDissolveNode(bottomNode);
        if (leftNode != null) TryDissolveNode(leftNode);
        if (rightNode != null) TryDissolveNode(rightNode);
        TryDissolveNode(newIntersection);
    }


    public void RemoveRoad(Vector2Int position)
    {
        if(interestsToNodes.TryGetValue(position, out TileIntersectionNode node))
        {
            Tile.Facing checkFacing = Tile.Facing.Top;
            for(int i = 0; i < 4; i++)
            {
                if (node.Connections.TryGetValue(checkFacing, out var farNode)) 
                {
                    var oppDir = checkFacing.Oppisite();
                    if(node.ConnectionCost[checkFacing] == 1) //The nodes are adjacent
                    {
                        farNode.Connections.Remove(oppDir);
                        farNode.ConnectionCost.Remove(oppDir);
                        TryDissolveNode(farNode);
                    }
                    else
                    {
                        //int cost = node.ConnectionCost[checkFacing] - 1;
                        Vector2Int newNodePos = node.Position + checkFacing.ToVector2();
                        var newNode = new TileIntersectionNode() { Position = newNodePos };

                        ConnectNodes(newNode, farNode, checkFacing);

                        pointsOfInterests.Add(newNodePos);
                        interestsToNodes.Add(newNodePos, newNode);
                    }
                }
                checkFacing = checkFacing.TurnRight();
            }

            pointsOfInterests.Remove(position);
            interestsToNodes.Remove(position);
        }
        else if(TryFindConnectionCandidate(position, out Vector2Int forwardPosition, out Tile.Facing backwardDirection))
        {
            var forwardDirection = backwardDirection.Oppisite();
            var forwardNode = interestsToNodes[forwardPosition];
            var backwardNode = forwardNode.Connections[backwardDirection];

            var (forwardDistance, backwardDistance) = (backwardDirection.IsHorizontal()) 
                ? (Mathf.Abs(forwardNode.Position.x - position.x), Mathf.Abs(backwardNode.Position.x - position.x)) 
                : (Mathf.Abs(forwardNode.Position.y - position.y), Mathf.Abs(backwardNode.Position.y - position.y));

            if(forwardDistance == 1)
            {
                forwardNode.Connections.Remove(backwardDirection);
                forwardNode.ConnectionCost.Remove(backwardDirection);
                TryDissolveNode(forwardNode);
            }
            else
            {
                var newPosNode = new TileIntersectionNode() { Position = position + forwardDirection.ToVector2() };

                ConnectNodes(forwardNode, newPosNode, backwardDirection);

                pointsOfInterests.Add(newPosNode.Position);
                interestsToNodes.Add(newPosNode.Position, newPosNode);
            }

            if (backwardDistance == 1)
            {
                backwardNode.Connections.Remove(forwardDirection);
                backwardNode.ConnectionCost.Remove(forwardDirection);
                TryDissolveNode(backwardNode);
            }
            else
            {
                var newNegNode = new TileIntersectionNode() { Position = position + backwardDirection.ToVector2() };

                ConnectNodes(backwardNode, newNegNode, forwardDirection);
                pointsOfInterests.Add(newNegNode.Position);
                interestsToNodes.Add(newNegNode.Position, newNegNode);
            }

            pointsOfInterests.Remove(position);
            interestsToNodes.Remove(position);
        }
        else
        {
            throw new Exception("This should have been connected");
        }
    }

    private bool TryDissolveNode(TileIntersectionNode node)
    {
        if (node.Connections.Count == 2 && !(node.Connections.ContainsKey(Tile.Facing.Top) ^ node.Connections.ContainsKey(Tile.Facing.Bottom)))
        {
            Tile.Facing posDir = (node.Connections.ContainsKey(Tile.Facing.Top)) ? Tile.Facing.Top : Tile.Facing.Right;
            Tile.Facing negDir = posDir.Oppisite();

            var posNode = node.Connections[posDir];
            var negNode = node.Connections[negDir];

            int cost = node.ConnectionCost[posDir] + node.ConnectionCost[negDir];

            posNode.Connections[negDir] = negNode;
            negNode.Connections[posDir] = posNode;
            posNode.ConnectionCost[negDir] = cost;
            negNode.ConnectionCost[posDir] = cost;

            interestsToNodes.Remove(node.Position);
            pointsOfInterests.Remove(node.Position);
            return true;
        }
        return false;
    }

    private void ConnectNodes(TileIntersectionNode fromNode, TileIntersectionNode toNode, Tile.Facing direction)
    {
        var oppDir = direction.Oppisite();
        var cost = (direction.IsHorizontal()) ? Mathf.Abs(fromNode.Position.x - toNode.Position.x) : Mathf.Abs(fromNode.Position.y - toNode.Position.y);

        if (fromNode.Connections.ContainsKey(direction))
        {
            fromNode.Connections[direction] = toNode;
            fromNode.ConnectionCost[direction] = cost;
        }
        else
        {
            fromNode.Connections.Add(direction, toNode);
            fromNode.ConnectionCost.Add(direction, cost);
        }

        if (toNode.Connections.ContainsKey(oppDir))
        {
            toNode.Connections[oppDir] = fromNode;
            toNode.ConnectionCost[oppDir] = cost;
        }
        else
        {
            toNode.Connections.Add(oppDir, fromNode);
            toNode.ConnectionCost.Add(oppDir, cost);
        }
    }

    private TileIntersectionNode InsertNodeAlongConnection(TileIntersectionNode node, Tile.Facing direction, Vector2Int position)
    {
        var oppDir = direction.Oppisite();
        TileIntersectionNode newNode = new TileIntersectionNode() { Position = position };
        TileIntersectionNode farNode = node.Connections[direction];

        ConnectNodes(node, newNode, direction);
        ConnectNodes(newNode, farNode, direction);

        return newNode;
    }

    private bool IsAdjacentToPoI(Vector2Int position, out Tile.Facing direction)
    {
        direction = Tile.Facing.Top;
        for(int i = 0; i < 4; i++)
        {
            Vector2Int checkPos = position + direction.ToVector2();
            if (pointsOfInterests.Contains(checkPos)) return true;
            direction.TurnRight();
        }
        return false;
    }

    private bool TryFindHorizontalConnectionCandidate(Vector2Int position, out Vector2Int connectedPosition, out Tile.Facing direction)
    {
        var (xSign, xFlippedFacing) = (position.x < width / 2) ? (-1, Tile.Facing.Right) : (1, Tile.Facing.Left);
        Vector2Int xSearch = position;
        
        while (xSign != 0)
        {

            xSearch.x += xSign;
            if (xSearch.x >= 0 && xSearch.x < width)
            {
                if (interestsToNodes.TryGetValue(xSearch, out TileIntersectionNode node))
                {
                    if (node.Connections.ContainsKey(xFlippedFacing))
                    {
                        //Found nearest connecition
                        connectedPosition = xSearch;
                        direction = xFlippedFacing;
                        return true;
                    }
                    else xSign = 0; //Reached Edge of searching in this direction
                }
            }
            else xSign = 0; //Reached Edge of searching
        }

        connectedPosition = Vector2Int.zero;
        direction = Tile.Facing.Top;
        return false;
    }

    private bool TryFindVerticalConnectionCandidate(Vector2Int position, out Vector2Int connectedPosition, out Tile.Facing direction)
    {        
        var (ySign, yFlippedFacing) = (position.y < height / 2) ? (-1, Tile.Facing.Top) : (1, Tile.Facing.Bottom);
        Vector2Int ySearch = position;

        while (ySign != 0)
        {
            ySearch.y += ySign;
            if (ySearch.y >= 0 && ySearch.y < height)
            {
                if (interestsToNodes.TryGetValue(ySearch, out TileIntersectionNode node))
                {
                    if (node.Connections.ContainsKey(yFlippedFacing))
                    {
                        //Found nearest connecition
                        connectedPosition = ySearch;
                        direction = yFlippedFacing;
                        return true;
                    }
                    else ySign = 0; //Reached Edge of searching in this direction
                }
            }
            else ySign = 0; //Reached Edge of searching
        }

        connectedPosition = Vector2Int.zero;
        direction = Tile.Facing.Top;
        return false;
    }

    private bool TryFindConnectionCandidate(Vector2Int position, out Vector2Int connectedPosition, out Tile.Facing direction)
    {
        var (xSign, xFlippedFacing) = (position.x < width / 2) ? (-1, Tile.Facing.Right) : (1, Tile.Facing.Left);
        var (ySign, yFlippedFacing) = (position.y < height / 2) ? (-1, Tile.Facing.Top) : (1, Tile.Facing.Bottom);

        Vector2Int xSearch = position;
        Vector2Int ySearch = position;

        while (xSign != 0 || ySign != 0)
        {
            if(xSign != 0)
            {
                xSearch.x += xSign; 
                if(xSearch.x >= 0 && xSearch.x < width)
                {
                    if (interestsToNodes.TryGetValue(xSearch, out TileIntersectionNode node))
                    {
                        if (node.Connections.ContainsKey(xFlippedFacing))
                        {
                            //Found nearest connecition
                            connectedPosition = xSearch;
                            direction = xFlippedFacing;
                            return true;
                        }
                        else xSign = 0; //Reached Edge of searching in this direction
                    }
                }
                else xSign = 0; //Reached Edge of searching
            }

            if(ySign != 0)
            {
                ySearch.y += ySign;
                if (ySearch.y >= 0 && ySearch.y < height)
                {
                    if (interestsToNodes.TryGetValue(ySearch, out TileIntersectionNode node))
                    {
                        if (node.Connections.ContainsKey(yFlippedFacing))
                        {
                            //Found nearest connecition
                            connectedPosition = ySearch;
                            direction = yFlippedFacing;
                            return true;
                        }
                        else ySign = 0; //Reached Edge of searching in this direction
                    }
                }
                else ySign = 0; //Reached Edge of searching
            }
        }

        connectedPosition = Vector2Int.zero;
        direction = Tile.Facing.Top;
        return false;
    }

    public LinkedList<Vector2Int> GetListOfPositionsFromToReduced(Vector2Int fromTile, Vector2Int toTile)
    {

        LinkedList<Vector2Int> output = new LinkedList<Vector2Int>();

        if (!(grid[toTile] is BuildingTile targetBuilding)) return output;
        foreach(var kvp in interestsToNodes)
        {
            kvp.Value.visited = false;
            kvp.Value.ParentNode = null;
            kvp.Value.ToParentDelta = Vector2Int.zero;
        }

        activeNodes.Clear();
        costDistSortedActiveNodes.Clear();

        output.AddLast(toTile);



        var frontOfToTile = toTile + targetBuilding.currentFacing.ToVector2();

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
                        if(rightSearch == Target)
                        {
                            //Found target early!
                            return ReturnPathList(output, null, rightDir);
                        }
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
                        if (leftSearch == Target)
                        {
                            //Found target early!
                            return ReturnPathList(output, null, leftDir);
                        }
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
        if(lastWaypoint != null)
        {
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
        }
        else
        {
            while(tilePos != FirstRoadTilePos)
            {
                outputList.AddFirst(tilePos);
                tilePos += initalDelta;
            }
            outputList.AddFirst(FirstRoadTilePos.Value);
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
