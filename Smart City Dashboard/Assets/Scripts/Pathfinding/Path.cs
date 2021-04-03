using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Path
{
    private NodeStream stream;
    private List<Vector3> currentlyTraversing = new List<Vector3>();
    public Vector3 lastTarget { private set; get; }
    public List<Vector3> CurrentlyTraversing { get => currentlyTraversing; set => currentlyTraversing = value; }

    private float tStep = 0f;

    public Path(LinkedList<Vector2Int> tilePoints, Vector3 startingPosition, NodeController endingPoint, NodeCollectionController.TargetUser userType)
    {
        Initialize(tilePoints, startingPosition, endingPoint, userType);
    }

    private void Initialize(LinkedList<Vector2Int> tilePoints, Vector3 startingPosition, NodeController endingPoint, NodeCollectionController.TargetUser userType)
    {
        stream = new NodeStream(GridManager.Instance.Grid, tilePoints, startingPosition, endingPoint, userType);
        lastTarget = startingPosition;
        CurrentlyTraversing.Add(lastTarget);
        CurrentlyTraversing.Add(lastTarget);
    }

    public bool IsValid() => !stream.IsCorrupted() && !stream.IsEndOfStream();

    public bool ReachedDestination() => stream.IsEndOfStream() && CurrentlyTraversing.Count == 0;

    public Vector3 GetNextTarget(Vector3 currentPosition, float timeDelta)
    {
        if (stream.IsCorrupted()) return currentPosition;
        float excessDelta = 0f;
        if (currentPosition.IsBasicallyEqualTo(lastTarget))
        {
            if (tStep < 1)
            {
                tStep += timeDelta;
                lastTarget = CalculateTarget(CurrentlyTraversing, tStep);
                if (tStep <= 1)
                {
                    return lastTarget;
                }
                else
                {
                    excessDelta = tStep - 1;
                    tStep = 0f;
                    CurrentlyTraversing.Clear();
                }
            }
            else
            {
                excessDelta = timeDelta;
                CurrentlyTraversing.Clear();
            }
        }
        else return lastTarget;

        if (CurrentlyTraversing.Count == 0)
        {
            if (TryPopulateTraversingList())
            {
                tStep = excessDelta;
                lastTarget = CalculateTarget(CurrentlyTraversing, tStep);
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
        if (CurrentlyTraversing.Count > 0) return false; //Already populated
       
        CurrentlyTraversing.Add(stream.GetCurrent());

        while (TryIncreaseTraversalList());

        if (CurrentlyTraversing.Count == 1)
        {
            CurrentlyTraversing.Clear();
            return false;
        }

        return true;
    }

    private bool TryIncreaseTraversalList()
    {
        if(stream.PeekNext(out Vector3 position).IsSuccessful())
        {
            CurrentlyTraversing.Add(position);
            if (TraverseListValid())
            {
                stream.MoveNext();
                return true;
            }
            CurrentlyTraversing.RemoveAt(CurrentlyTraversing.Count - 1);
        }
        return false;
    }

    /// <summary>
    /// Traversal lists are considered valid if no 3 points are collinear
    /// </summary>
    /// <returns></returns>
    private bool TraverseListValid()
    {
        int count = CurrentlyTraversing.Count;
        if (count < 2) return false;
        else if (count == 2) return true; //If there are not 3 points present, then they certainly can't be collinear
        else
        {
            //Only checks the last 3 values because it is assumed that when creating a traversal list, the only new point is the last one
            return !CurrentlyTraversing[count - 1].IsCollinearWith(CurrentlyTraversing[count - 2], CurrentlyTraversing[count - 3]);
        }
    }
}
