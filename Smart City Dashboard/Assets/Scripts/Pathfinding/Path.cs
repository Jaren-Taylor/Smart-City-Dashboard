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

    public Path(IEnumerable<Vector2Int> tilePoints, Vector3 startingPosition, NodeController endingPoint, NodeCollectionController.TargetUser userType)
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
}
