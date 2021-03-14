using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public InputManager inputManager;
    public CameraManager cameraManager;
    public GridManager gridManager;
    public UIManager uiManager;
    public SensorInfoMenu sensorInfoMenu;

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

        inputManager.OnNumberPressed += uiManager.OnNumberKeyPress;

        inputManager.OnCPressed += gridManager.ToggleCursor;

        // UI events

        inputManager.OnF1Pressed += uiManager.ReceiveMenuKey;

        inputManager.OnTildePressed += uiManager.ReceiveMenuKey;

        inputManager.OnTabPressed += uiManager.SwitchTabs;

        uiManager.OnUIToggle += inputManager.DisableCameraPan;

        sensorInfoMenu.DisableCameraControls += inputManager.DisableCameraPan;

        sensorInfoMenu.DisableCameraControls += inputManager.SetTopDownMode;

        sensorInfoMenu.DisableCameraControls += inputManager.DisableCameraRotation;

        sensorInfoMenu.DisableCameraControls += inputManager.DisableCameraZoom;

        sensorInfoMenu.ToggleCursor += gridManager.ToggleCursor;
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
