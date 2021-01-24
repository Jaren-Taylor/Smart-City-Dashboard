using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public InputManager inputManager;
    public CameraManager cameraManager;
    public GridManager gridManager;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        
        inputManager.OnCameraPan += cameraManager.PanHandler;

        inputManager.OnCameraRotation += cameraManager.RotationHandler;

        inputManager.OnCameraZoom += cameraManager.ZoomHandler;

        inputManager.OnPlaceTile += gridManager.PlaceHandler;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(inputManager.Cursor);
    }

}
