using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;

    [Range(minSize, maxSize)]
    public float defaultSize;

    [Range(0, 3)]
    public int defaultRotation;

    public Vector2 defaultPosition;


    public float boundaryFraction = .2f;
    public int defaultPanSpeed = 20;
    public float panZoomSenstivity = 4f;
    public float zoomScale = .1f;


    private Vector3 panVelocity = Vector3.zero;
    private float sizeVelocity = 0;
    private const float smoothTime = 0.3F;

    private const float minSize = 5f;
    private const float maxSize = 25f;



    private float size;
    public float Size { get => size; set => size = Mathf.Clamp(value, minSize, maxSize); }

    private int rotation;
    public int Rotation { get => rotation; set => rotation = value ; }

    private Vector3 trackedPosition;
    public Vector3 Position { get => transform.localPosition; set => trackedPosition = new Vector3(value.x, 0f, value.z); }

    void ResetSize() => Size = defaultSize;
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
        /*if (Input.mouseScrollDelta.y != 0)
        {
            Size += -Input.mouseScrollDelta.y * zoomScale;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) RotateLeft();
        else if (Input.GetKeyDown(KeyCode.RightArrow)) RotateRight();
        */
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, trackedPosition, ref panVelocity, smoothTime);
        mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, size, ref sizeVelocity, smoothTime);
    }
    internal void PanHandler(Vector3 panDelta)
    {
        Vector3 scaledDelta = panDelta * Time.deltaTime * Config.panSpeed;
        trackedPosition += Quaternion.Euler(0, transform.rotation.eulerAngles.y + 45f, 0) * scaledDelta * (size / minSize) * panZoomSenstivity;

    }
}
