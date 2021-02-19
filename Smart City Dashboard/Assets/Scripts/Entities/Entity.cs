using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public NodeController SpawnPosition { get; protected set; }
    private float maxSpeed = .5f;
    private Path path;
    public Vector2Int TilePosition => Vector2Int.RoundToInt(new Vector2(transform.position.x, transform.position.z));
    public bool TrySetDestination(Vector2Int tileLocation)
    {
        path = Pathfinding.GetPathFromTo(GridManager.Instance.Grid, TilePosition, tileLocation);
        return !(path is null);
    }

    private void Update()
    {
        if (HasPath())
        {
            MoveAlongPath();
        }
    }
    //Comment
    private void MoveAlongPath()
    {
        if (path.GetCurrentNode() is NodeController nodeController)
        {
            MoveTowardsPosition(nodeController.transform.position);
            if (HasArrivedAtNode(nodeController.transform.position))
            {
                if (!path.AdvanceNextNode())
                {
                    DestroyPath();
                }
            }
        }
        else DestroyPath();
    }

    private bool HasArrivedAtNode(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) < .0005;
    }

    private void MoveTowardsPosition(Vector3 position)
    {
        transform.position = Vector3.MoveTowards(transform.position, position, maxSpeed * Time.deltaTime);
    }

    private void DestroyPath() => path = null;

    private bool HasPath() => !(path is null);
}