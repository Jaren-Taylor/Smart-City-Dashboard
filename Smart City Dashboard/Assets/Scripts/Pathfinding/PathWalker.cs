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

    private Vector2Int currentDestination;
    private NodeCollectionController.TargetUser userType;

    private Vector3 MyPosition => transform.position;

    public Action<float> OnReachedDestination;

    public bool TrySetDestination(Vector2Int tileLocation, NodeCollectionController.TargetUser targetUser)
    {
        currentDestination = tileLocation;
        userType = targetUser;
        var pathList = Pathfinding.GetListOfPositionsFromTo(MyPosition.ToGridInt(), tileLocation);
        if (pathList is null)
            return false; 
        path = new Path(pathList, MyPosition, null, targetUser);
        if (path.IsValid()) return true;
        else
        {
            path = null;
            return false;
        }
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
        Vector3 target = path.GetNextTarget(transform.position, Time.deltaTime);

        if (MyPosition.IsBasicallyEqualTo(target))
        {
            if (path.ReachedDestination()) DestroyPath(2f);
            else if (!path.IsValid() && !TrySetDestination(currentDestination, userType)) DestroyPath(0f); 
            else
            {
                //Just wait ¯\_(-.-)_/¯
            }
        }
        else
        {
            transform.LookAt(target);
            TryMoveTowardsPosition(target);
        }
    }

    private void TryMoveTowardsPosition(Vector3 position)
    {
        var speed = maxSpeed;
        /*
        //TODO: Collision Check
        var timeDelta = Time.deltaTime;
        if (Physics.Raycast(raycastSource.transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, maxSpeed * timeDelta) ||
            Physics.Raycast(raycastSource.transform.position + transform.TransformDirection(Vector3.left) * .05f, transform.TransformDirection(Vector3.forward), out RaycastHit hit2, maxSpeed * timeDelta) ||
            Physics.Raycast(raycastSource.transform.position + transform.TransformDirection(Vector3.right) * .05f, transform.TransformDirection(Vector3.forward), out RaycastHit hit3, maxSpeed * timeDelta))
        {
            //Debug.Log("Draw hit distance: " + hit.distance);
            Debug.DrawRay(raycastSource.transform.position, transform.TransformDirection(Vector3.forward) * maxSpeed, Color.white, 1f, false);
            Debug.DrawRay(raycastSource.transform.position + transform.TransformDirection(Vector3.left) * .05f, transform.TransformDirection(Vector3.forward) * maxSpeed * timeDelta, Color.white, 1f, false);
            Debug.DrawRay(raycastSource.transform.position + transform.TransformDirection(Vector3.right) * .05f, transform.TransformDirection(Vector3.forward) * maxSpeed * timeDelta, Color.white, 1f, false);
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
