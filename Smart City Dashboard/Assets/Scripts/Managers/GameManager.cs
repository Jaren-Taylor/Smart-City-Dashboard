using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public InputManager inputManager;
    public CameraManager cameraManager;
    public GridManager gridManager;
    public Menu menu;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        // Please be mindful. Order of handler assignment could matters

        // Camera Manager events

        inputManager.OnCameraPan += cameraManager.PanHandler;

        inputManager.OnCameraRotation += cameraManager.RotationHandler;

        inputManager.OnCameraZoom += cameraManager.ZoomHandler;

        // Grid Manager events

        inputManager.OnPlaceTile += gridManager.PlaceHandler;

        inputManager.OnNumberPressed += gridManager.StateNumberChangeHandler;

        inputManager.OnCPressed += gridManager.ToggleCursor;

        // Menu events

        inputManager.OnEscapePressed += menu.ToggleMenuHandler;

        inputManager.OnNumberPressed += menu.ActivateTab;
    }

    public void HandleLog(int numer)
    {
        Debug.Log(numer);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(inputManager.Cursor);
    }

}
