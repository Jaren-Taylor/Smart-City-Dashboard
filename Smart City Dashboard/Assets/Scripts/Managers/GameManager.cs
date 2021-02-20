using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public InputManager inputManager;
    public CameraManager cameraManager;
    public GridManager gridManager;
    public UIManager uiManager;

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

        // UI events

        inputManager.OnEscapePressed += uiManager.ToggleMenu;

        inputManager.OnTildePressed += uiManager.ToggleMenu;

        inputManager.OnTabPressed += uiManager.SwitchTabs;

        uiManager.OnUIToggle += inputManager.IsUIActive;

        uiManager.OnTabSwitch += gridManager.StateNumberChangeHandler;
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
