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

        uiManager.OnUIToggle += inputManager.IsUIActive;

        inputManager.OnEscapePressed += uiManager.ToggleEscapeMenu;

        inputManager.OnTildePressed += uiManager.ToggleTildeMenu;

        inputManager.OnTabPressed += uiManager.SwitchTabs;

        // inputManager.OnNumberPressed += menu.ActivateTab;
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
