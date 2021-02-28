 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;
    public Camera entityCamera;

    public FirstPersonViewport vehicleViewport;

    [Range(0, 3)]
    public int defaultRotation;

    public Vector2 defaultPosition;
    public bool isFollowingEntity { get; private set; } = false;
    private Entity trackedEntity;
    private Vector3 panVelocity = Vector3.zero;
    private float sizeVelocity = 0;

    private float size;
    public float Size { get => size; set => size = Mathf.Clamp(value, Config.minSize, Config.maxSize); }

    private int rotation;
    public int Rotation { get => rotation; set => rotation = value; }

    private Vector3 trackedPosition;
    public Vector3 Position { get => transform.localPosition; set => trackedPosition = new Vector3(value.x, 0f, value.z); }

    private float targetTrackHeight = 0f;

    public static CameraManager Instance { get; private set; }

    void ResetSize() => Size = Config.defaultSize;
    void ResetPosition() => Position = defaultPosition;
    void ResetRotation() => Rotation = defaultRotation;

    void ResetCamera()
    {
        ResetSize();
        ResetRotation();
        ResetPosition();
    }

    private void Awake()
    {
        if (Instance != null) Destroy(this);
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        entityCamera.gameObject.SetActive(false);
        ResetCamera();
    }

    void RotateLeft()
    {
        transform.Rotate(new Vector3(0, 45, 0), Space.Self);
    }

    void RotateRight()
    {
        transform.Rotate(new Vector3(0, -45, 0), Space.Self);
    }

    // Update is called once per frame
    void Update()
    {

        if (isFollowingEntity is false)
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, trackedPosition, ref panVelocity, Config.smoothTime); ;
            mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, size, ref sizeVelocity, Config.smoothTime);
        }
        if(isFollowingEntity is true)
        {
            FollowEntity(trackedEntity);
        }
    }
    internal void PanHandler(Vector3 panDelta)
    {
        Vector3 scaledDelta = panDelta * Time.deltaTime * Config.panSpeed;
        trackedPosition += Quaternion.Euler(0, transform.rotation.eulerAngles.y + 45f, 0) * scaledDelta * (size / Config.minSize) * Config.panZoomSenstivity;

    }
    internal void RotationHandler(float direction)
    {
        if (direction < 0)
        {
            RotateLeft();
        }
        if (direction > 0)
        {
            RotateRight();
        }
    }
    internal void ZoomHandler(float zoom)
    {
        Size += -zoom * Config.zoomScale;
    }
    public void StartFollowEntity(Entity entity)
    {
        GridManager.Instance.CursorEnabled = false;
        isFollowingEntity = true;
        GridManager.Instance.GridSM.SuspendState(new DigitalCursor());
        mainCamera.gameObject.SetActive(false);
        entityCamera.gameObject.SetActive(true);
        trackedEntity = entity;
        trackedEntity.OnBeingDestroy += TrackedEntityDestroyed;
        var collider = entity.GetComponent<BoxCollider>();//.bounds.size.y;
        if (entity is VehicleEntity vehicle)
        {
            vehicleViewport.gameObject.SetActive(true);
            vehicleViewport.SetTrackTo(vehicle);
            targetTrackHeight = collider.bounds.size.y * .75f;
            vehicle.SetModelVisibility(false);
        }
        else
        {
            vehicleViewport.gameObject.SetActive(false);
            targetTrackHeight = collider.bounds.size.y;
        }
    }

    private void TrackedEntityDestroyed()
    {
        StopFollowEntity();
    }

    public void StopFollowEntity()
    {
        GridManager.Instance.CursorEnabled = true;
        entityCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        GridManager.Instance.GridSM.ResumeState(new DigitalCursor());
        ResetCamera();
        isFollowingEntity = false;
        if(trackedEntity is Entity entity)
            entity.OnBeingDestroy -= TrackedEntityDestroyed;
        trackedEntity = null;
        if (vehicleViewport.gameObject.activeSelf)
        {
            vehicleViewport.StopTracking();
            vehicleViewport.gameObject.SetActive(false);
            if(trackedEntity is VehicleEntity vehicleEntity)
            {
                vehicleEntity.SetModelVisibility(true);
            }
        }
        
    }
    public void FollowEntity(Entity entity)
    {
        transform.position = entity.transform.position + Vector3.up * targetTrackHeight;
        transform.rotation = entity.transform.rotation;
    }
    
}
