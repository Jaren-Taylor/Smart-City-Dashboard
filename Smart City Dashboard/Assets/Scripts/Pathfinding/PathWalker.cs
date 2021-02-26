using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathWalker : MonoBehaviour
{
    private enum MovementStatus
    {
        MoveToSingle,
        MoveCurve
    }

    [SerializeField]
    protected GameObject raycastSource;

    public NodeController SpawnPosition;

    //private int layerMask = 1 << 7;
    private float maxSpeed = .5f;
    private Path path;

    public Vector2Int TilePosition => Vector2Int.RoundToInt(new Vector2(transform.position.x, transform.position.z));

    public Action<float> OnReachedDestination;

    public bool TrySetDestination(Vector2Int tileLocation, NodeCollectionController.TargetUser targetUser)
    {
        var pathList = Pathfinding.GetListOfPositionsFromTo(TilePosition, tileLocation);
        if (pathList is null)
            return false;
        path = new Path(pathList, SpawnPosition, null, targetUser);
        return !(path is null);
    }

    private void Update()
    {
        if (HasPath())
        {
            TryMoveAlongPath();
        }
    }

    private void TryMoveAlongPath()
    {
        if (path.GetNextTarget(transform.position, Time.deltaTime) is Vector3 position)
        {
            transform.LookAt(position);
            TryMoveTowardsPosition(position);
        }

        if (path.ReachedDestination()) DestroyPath(2f);
    }

    private bool HasArrivedAtNode(Vector3 position) => Vector3.Distance(transform.position, position) < .0005;
    private void TryMoveTowardsPosition(Vector3 position)
    {
        var speed = maxSpeed;
        /* TODO: Collision Check
        if (Physics.Raycast(raycastSource.transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, maxSpeed * .5f))
        {
            Debug.Log("Draw hit distance: " + hit.distance);
            Debug.DrawRay(raycastSource.transform.position, transform.TransformDirection(Vector3.forward) * maxSpeed, Color.white, 1f, false);
            speed = 0;
        }
        */
        transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
    }
    private void DestroyPath(float delay)
    {
        path = null;
        OnReachedDestination?.Invoke(delay);
    }
    private bool HasPath() => !(path is null);
}
