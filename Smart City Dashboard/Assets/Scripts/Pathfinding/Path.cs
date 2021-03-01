using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Path
{
    private NodeStream stream;
    private List<Vector3> currentlyTraversing = new List<Vector3>();
    private Vector3 lastTarget;
    private float tStep = 0f;

    public Path(List<Vector2Int> tilePoints, Vector3 startingPosition, NodeController endingPoint, NodeCollectionController.TargetUser userType)
    {
        Initialize(tilePoints, startingPosition, endingPoint, userType);
    }

    private void Initialize(IEnumerable<Vector2Int> tilePoints, Vector3 startingPosition, NodeController endingPoint, NodeCollectionController.TargetUser userType)
    {
        stream = new NodeStream(GridManager.Instance.Grid, tilePoints, startingPosition, endingPoint, userType);
        lastTarget = startingPosition;
        currentlyTraversing.Add(lastTarget);
        currentlyTraversing.Add(lastTarget);
    }

    public bool IsValid() => !stream.IsEndOfStream() && !stream.IsCorrupted();

    public bool ReachedDestination() => stream.IsEndOfStream() && currentlyTraversing.Count == 0;

    public Vector3 GetNextTarget(Vector3 currentPosition, float timeDelta)
    {
        if (stream.IsCorrupted()) return currentPosition;
        float excessDelta = 0f;
        if (currentPosition.IsBasicallyEqualTo(lastTarget))
        {
            if (tStep < 1)
            {
                tStep += timeDelta;
                lastTarget = CalculateTarget(currentlyTraversing, tStep);
                if (tStep <= 1)
                {
                    return lastTarget;
                }
                else
                {
                    excessDelta = tStep - 1;
                    tStep = 0f;
                    currentlyTraversing.Clear();
                }
            }
            else
            {
                excessDelta = timeDelta;
                currentlyTraversing.Clear();
            }
        }
        else return lastTarget;

        if (currentlyTraversing.Count == 0)
        {
            if (TryPopulateTraversingList())
            {
                tStep = excessDelta;
                lastTarget = CalculateTarget(currentlyTraversing, tStep);
                return lastTarget;
            }
            else tStep = 1f;
        }
        return lastTarget;
    }

    private Vector3 CalculateTarget(List<Vector3> currentlyTraversing, float tStep)
    {
        if (currentlyTraversing.Count <= 2)
        {
            this.tStep = 1f;
            return currentlyTraversing[1];
        }
        else return Bezier.PointAlongCurve(currentlyTraversing, tStep);
    }

    private bool TryPopulateTraversingList()
    {
        if (currentlyTraversing.Count > 0) return false; //Already populated
       
        currentlyTraversing.Add(stream.GetCurrent());

        while (TryIncreaseTraversalList());

        if (currentlyTraversing.Count == 1)
        {
            currentlyTraversing.Clear();
            return false;
        }

        return true;
    }

    private bool TryIncreaseTraversalList()
    {
        if(stream.PeekNext(out Vector3 position).IsSuccessful())
        {
            currentlyTraversing.Add(position);
            if (TraverseListValid())
            {
                stream.MoveNext();
                return true;
            }
            currentlyTraversing.RemoveAt(currentlyTraversing.Count - 1);
        }
        return false;
    }

    /// <summary>
    /// Traversal lists are considered valid if no 3 points are collinear
    /// </summary>
    /// <returns></returns>
    private bool TraverseListValid()
    {
        int count = currentlyTraversing.Count;
        if (count < 2) return false;
        else if (count == 2) return true; //If there are not 3 points present, then they certainly can't be collinear
        else
        {
            //Only checks the last 3 values because it is assumed that when creating a traversal list, the only new point is the last one
            return !currentlyTraversing[count - 1].IsCollinearWith(currentlyTraversing[count - 2], currentlyTraversing[count - 3]);
        }
    }
    /*
    private List<Vector2Int> TilePoints;
    private List<NodeController> Nodes;
    private List<Vector3> currentlyTraversing = new List<Vector3>();
    private NodeController endingNode;
    private int currentTileIndex;
    private NodeCollectionController.Direction? currentExitDirection;
    private NodeCollectionController.TargetUser userType;

    private Vector3? lastTarget = null;
    private float tStep = 0f;

    private bool reachedDestination = false;

    public Path(List<Vector2Int> tilePoints, NodeController startingPoint, NodeController endingPoint,  NodeCollectionController.TargetUser userType)
    {
        InitalizeClass(tilePoints, startingPoint, endingPoint, userType);
    }

    private void InitalizeClass(List<Vector2Int> tilePoints, NodeController startingPoint, NodeController endingPoint, NodeCollectionController.TargetUser userType)
    {
        TilePoints = tilePoints;
        this.userType = userType;
        endingNode = endingPoint;
        if (TilePoints.Count < 1) throw new System.Exception("Destination already reached");
        TryUpdateExitingDirection();
        Nodes = GetRegularPathWithController(startingPoint, currentExitDirection.Value);
        currentTileIndex = 0;
        RegisterToPath(tilePoints);
    }

    internal Vector3? GetNextTarget(Vector3 position, float timeDelta)
    {
        float excessDelta = 0f;
        if (lastTarget.HasValue)
        {
            if (Vector3.Distance(lastTarget.Value, position) < 0.0005f)
            {
                if (tStep < 1)
                {
                    tStep += timeDelta;
                    lastTarget = CalculateTarget(currentlyTraversing, tStep);
                    if (tStep <= 1)
                    {
                        return lastTarget;
                    }
                    else
                    {
                        excessDelta = tStep - 1;
                        tStep = 0f;
                        currentlyTraversing.Clear();
                    }
                }
                else
                {
                    currentlyTraversing.Clear();
                }
            }
            else return lastTarget;
        }
        else currentlyTraversing.Clear();

        if(currentlyTraversing.Count == 0)
        {
            if (PopulateTraversingList())
            {
                tStep = excessDelta;
                lastTarget = CalculateTarget(currentlyTraversing, tStep);
                return lastTarget;
            }
        }
        return lastTarget;
    }

    private Vector3 CalculateTarget(List<Vector3> currentlyTraversing, float tStep)
    {
        if(currentlyTraversing.Count <= 2)
        {
            this.tStep = 1f;
            return currentlyTraversing[1];
        }
        else return Bezier.PointAlongCurve(currentlyTraversing, tStep);
    }

    

    public bool ReachedDestination() => reachedDestination;

    private bool PopulateTraversingList()
    {
        if (currentlyTraversing.Count > 0) return false; //Already populated

        if (Nodes.Count < 2)
        {
            if (!TryGetNextTilePath())
            {
                if (ReachedDestinationTile()) 
                {
                    reachedDestination = true;
                    return false; //Reached Destination!
                } 
                else return false; //Only one node sitting in pool, but can't get more. Must wait.
            }
            //At least one node has been added.
        }

        currentlyTraversing.Add(Nodes[0].Position);

        while (CanIncreaseTraversalList())
        {
            Nodes.RemoveAt(0);
            currentlyTraversing.Add(Nodes[0].Position);
        }
        return true;
    }

    private bool CanIncreaseTraversalList()
    {
        if(Nodes.Count < 2)
        {
            if (!TryGetNextTilePath())
            {
                return false;
            }
        }

        currentlyTraversing.Add(Nodes[1].Position);
        bool ableTo = TraverseListValid();
        currentlyTraversing.RemoveAt(currentlyTraversing.Count - 1);

        return ableTo;
    }

    private bool TryGetNextTilePath()
    {
        if (ReachedDestinationTile()) return false;
        var nextCollection = GridManager.GetCollectionAtTileLocation(TilePoints[currentTileIndex + 1]);
        var position = GridManager.GetCollectionAtTileLocation(TilePoints[currentTileIndex]).GetPositionFrom(currentExitDirection.Value, Nodes.Last());
        if(nextCollection.CanEnterFromPosition(currentExitDirection.Value, position))
        {
            DepartCurrentTile();
            var inboundNode = nextCollection.GetInboundNodeFrom(currentExitDirection.Value, position);
            List<NodeController> newPath = GetPathNodes(inboundNode, nextCollection);
            Nodes.AddRange(newPath);
            return true;
        }
        return false;
    }

    private bool TraverseListValid()
    {
        int pathIndex = 0;
        if (currentlyTraversing.Count < 2) return false;
        else if (currentlyTraversing.Count == 2) return true;
        else
        {
            while(pathIndex + 2 < currentlyTraversing.Count)
            {
                if(IsCollinear(
                    currentlyTraversing[pathIndex    ],
                    currentlyTraversing[pathIndex + 1],
                    currentlyTraversing[pathIndex + 2]))
                {
                    return false;
                }
                pathIndex++;
            }
            return true;
        }
    }

    private bool IsCollinear(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        List<float> Distances = new List<float>()
        {
            Vector3.Distance(p1, p2),
            Vector3.Distance(p2, p3),
            Vector3.Distance(p3, p1)
        };

        Distances.Sort();

        return Math.Abs(Distances[2] - (Distances[1] + Distances[0])) < .0005f;
    }

    ~Path()
    {
        for(int i = currentTileIndex; i < TilePoints.Count; i++)
        {
            DeregisterFromTileAt(TilePoints[currentTileIndex]);
        }
    }

    private void TileDestroyedOnPath(Tile tile)
    {
        DeregisterFromAll();
        var newPath = Pathfinding.GetListOfPositionsFromTo(TilePoints[currentTileIndex], TilePoints[TilePoints.Count - 1]);
        if (newPath is null)
        {
            // Can't set destination, so destroy self
            endingNode = null;
        }
        else
        {
            throw new Exception("Path modified");
            // New path valid, assign path object
            //InitalizeClass(newPath, currentNode, endingNode, userType);
        }

    }

    public void DeregisterFromAll()
    {
        foreach (Vector2Int tilePos in TilePoints) DeregisterFromTileAt(tilePos);
    }

    public void DeregisterFromTileAt(Vector2Int position)
    {
        if (GridManager.Instance.Grid[position] is Tile t) t.OnTileDestroyed -= TileDestroyedOnPath;
    }

    public void RegisterToPath(List<Vector2Int> tilePoints)
    {
        foreach(Vector2Int tilePos in tilePoints)
        {
            GridManager.GetTile(tilePos).OnTileDestroyed += TileDestroyedOnPath;
        }
    }

    private NodeController GetNodeInDirection(NodeCollectionController.Direction direction, NodeController controller) =>
        (userType == NodeCollectionController.TargetUser.Vehicles) ?
            controller.GetNodeForVehicleByDirection(direction) :
            controller.GetNodeForPedestrianByDirection(direction);

    private List<NodeController> GetPathNodes(NodeController inboundNode, NodeCollectionController nextCollection)
    {
        if(currentTileIndex == TilePoints.Count - 1)
        {
            //On last tile can't use exit direction
            return new List<NodeController>() { inboundNode };
        }

        if (!TryUpdateExitingDirection()) throw new Exception("Can't update direction");

        if (currentTileIndex == TilePoints.Count - 2)
        {
            //On second to last. Must use special logic to pull into driveway
            return GetRegularPathWithController(inboundNode, currentExitDirection.Value);
        } 
        else
        {
            //On normal tile, use normal pathing.
            return GetRegularPathWithController(inboundNode, currentExitDirection.Value);
        }
    }

    private List<NodeController> GetRegularPathWithController(NodeController inital, NodeCollectionController.Direction direction)
    {
        List<NodeController> output = new List<NodeController>();
        output.Add(inital);
        while (GetNodeInDirection(direction, output[output.Count - 1]) is NodeController nextNode)
        {
            output.Add(nextNode);
        }
        return output;
    }

    private void DepartCurrentTile()
    {
        DeregisterFromTileAt(TilePoints[currentTileIndex]);
        currentTileIndex++;
    }

    private bool TryUpdateExitingDirection()
    {
        if (currentTileIndex + 1 < TilePoints.Count) //Is not on the last tile
        {
            //How to handle the OOB Exception?
            currentExitDirection = NodeCollectionController.GetDirectionFromDelta(TilePoints[currentTileIndex + 1], TilePoints[currentTileIndex]);
            return true;
        }
        currentExitDirection = null;
        return false;
    }

    private bool ReachedDestinationTile() => currentTileIndex >= TilePoints.Count - 1;
    */
}
