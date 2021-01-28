using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;

    [Range(0, 3)]
    public int defaultRotation;

    public Vector2 defaultPosition;


    private Vector3 panVelocity = Vector3.zero;
    private float sizeVelocity = 0;




    private float size;
    public float Size { get => size; set => size = Mathf.Clamp(value, Config.minSize, Config.maxSize); }

    private int rotation;
    public int Rotation { get => rotation; set => rotation = value; }

    private Vector3 trackedPosition;
    public Vector3 Position { get => transform.localPosition; set => trackedPosition = new Vector3(value.x, 0f, value.z); }

    void ResetSize() => Size = Config.defaultSize;
    void ResetPosition() => Position = defaultPosition;
    void ResetRotation() => Rotation = defaultRotation;

    void ResetCamera()
    {
        ResetSize();
        ResetRotation();
        ResetPosition();
    }

    // Start is called before the first frame update
    void Start()
    {
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
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, trackedPosition, ref panVelocity, Config.smoothTime);;
        mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, size, ref sizeVelocity, Config.smoothTime);
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
    
}
