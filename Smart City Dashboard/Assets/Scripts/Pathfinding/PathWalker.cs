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

    private readonly int layerMask = 1 << 7;
    private float maxSpeed = .5f;
    private Path path;

    private Vector2Int currentDestination;
    private NodeCollectionController.TargetUser userType;

    private float stopTime = 0f;
    private float trafficTolerance = 0f;

    private Vector3 MyPosition => transform.position;

    public float TurnDelta { get; private set; } = 0f;

    public Action<float> OnReachedDestination;

    public NodeCollectionController.TargetUser User => userType;
    public float CurrentStopTime => stopTime;
    public Vector3 LastMoveDelta { get; private set; } = Vector3.zero;
    public Path Path { get => path; set => path = value; }

    public bool TrySetDestination(Vector2Int tileLocation, NodeCollectionController.TargetUser targetUser)
    {
        currentDestination = tileLocation;
        userType = targetUser;
        if (targetUser == NodeCollectionController.TargetUser.Pedestrians)
        {
            maxSpeed = .25f;
            trafficTolerance = UnityEngine.Random.Range(10f, 15f);
        }
        else
        {
            trafficTolerance = UnityEngine.Random.Range(15f, 20f);
        }
        var pathList = GridManager.Instance.Grid.GetListOfPositionsFromTo(MyPosition.ToGridInt(), tileLocation);
            //Pathfinding.GetListOfPositionsFromToReduced(MyPosition.ToGridInt(), tileLocation);
        if (pathList is null)
            return false; 
        Path = new Path(pathList, MyPosition, null, targetUser);
        if (Path.IsValid()) return true;
        else
        {
            Path = null;
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
        Vector3 target = Path.GetNextTarget(transform.position, Time.deltaTime);

        if (MyPosition.IsBasicallyEqualTo(target))
        {
            if (Path.ReachedDestination()) DestroyPath(2f);
            else if (!Path.IsValid() && !TrySetDestination(currentDestination, userType)) DestroyPath(0f); 
            else
            {
                //Just wait ¯\_(-.-)_/¯
                stopTime += Time.deltaTime;
            }
        }
        else
        {
            TurnDelta = Vector3.SignedAngle(transform.TransformDirection(Vector3.forward), target - transform.position, Vector3.up);
            transform.LookAt(target);


            TryMoveTowardsPosition(target);
        }
    }

    private RaycastHit[] hitBuffer = new RaycastHit[20];

    private Vector3 halfExtents = new Vector3(.05f, .05f, .01f);

    private void TryMoveTowardsPosition(Vector3 position)
    {
        var speed = maxSpeed;

        if (true)
        {
            //TODO: Collision Check
            var timeDelta = Time.deltaTime;
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            if (Physics.BoxCastNonAlloc(raycastSource.transform.position + forward * .01f, halfExtents, forward, hitBuffer, transform.localRotation, maxSpeed * .2f, layerMask) != 0)
            {
                var hitVector = hitBuffer[0].collider.transform.TransformDirection(Vector3.forward);
                var dot = Vector3.Dot(forward, hitVector);
                if(dot > -.995)
                {
                    speed = 0;
                    stopTime += timeDelta;
                } //Completely oppisite, just keep going
            }
            
            /*
            if (Physics.Raycast(raycastSource.transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, maxSpeed * .25f, layerMask) ||
                Physics.Raycast(raycastSource.transform.position + transform.TransformDirection(Vector3.left) * .05f, transform.TransformDirection(Vector3.forward), out RaycastHit hit2, maxSpeed * .25f, layerMask) ||
                Physics.Raycast(raycastSource.transform.position + transform.TransformDirection(Vector3.right) * .05f, transform.TransformDirection(Vector3.forward), out RaycastHit hit3, maxSpeed * .25f, layerMask) ||
                Physics.Raycast(raycastSource.transform.position + transform.TransformDirection(Vector3.left) * .05f, Quaternion.AngleAxis(30, Vector3.up) * transform.TransformDirection(Vector3.forward), out RaycastHit hit4, maxSpeed * .25f, layerMask) ||
                Physics.Raycast(raycastSource.transform.position + transform.TransformDirection(Vector3.right) * .05f, Quaternion.AngleAxis(30, Vector3.up) * transform.TransformDirection(Vector3.forward), out RaycastHit hit5, maxSpeed * .25f, layerMask))
            {

                //Debug.Log("Draw hit distance: " + hit.distance);
                //Debug.DrawRay(raycastSource.transform.position, transform.TransformDirection(Vector3.forward) * maxSpeed, Color.white, 1f, false);
                //Debug.DrawRay(raycastSource.transform.position + transform.TransformDirection(Vector3.left) * .05f, transform.TransformDirection(Vector3.forward) * maxSpeed * timeDelta, Color.white, 1f, false);
                //Debug.DrawRay(raycastSource.transform.position + transform.TransformDirection(Vector3.right) * .05f, transform.TransformDirection(Vector3.forward) * maxSpeed * timeDelta, Color.white, 1f, false);
                speed = 0;
                stopTime += timeDelta;
            }*/

            if (speed != 0) stopTime = 0f;

            if (stopTime > trafficTolerance) DestroyPath(0f);
        }
        LastMoveDelta = Vector3.Normalize(position - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
    }
    private void DestroyPath(float delay)
    {
        Path = null;
        OnReachedDestination?.Invoke(delay);
    }
    private bool HasPath() => !(Path is null);
}
