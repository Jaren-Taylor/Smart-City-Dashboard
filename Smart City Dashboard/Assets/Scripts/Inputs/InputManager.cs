using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Controls;
using static UnityEngine.InputSystem.InputAction;

/// <summary>
/// All input for the project should pass through here
/// </summary>
public class InputManager : MonoBehaviour   
{
    [SerializeField]
    public Action<Vector3> OnCameraPan;
    public Action<float> OnCameraRotation;
    public Action<float> OnCameraZoom;

    // Keyboard Actions
    public Action<int> OnNumberPressed;
    public Action<KeyCode> OnF1Pressed;
    public Action<KeyCode> OnTildePressed;
    public Action OnCPressed;
    public Action OnTabPressed;

    public Action OnPlaceTile;
    public Action OnEndPlaceTile;

    private bool isMoving = false;
    private bool isPanDisabled;
    private bool isRotationDisabled;
    private bool isZoomDisabled;
    private bool isInTopDown;
    Vector3 moveBy;

    private void Update()
    {
        if(isMoving == true)
        {
            if (!isPanDisabled && !isInTopDown) OnCameraPan.Invoke(moveBy);
        }

    }

    public void OnPlace(CallbackContext context)
    {
        if (context.started) OnPlaceTile?.Invoke();
        else if (context.performed) OnEndPlaceTile?.Invoke();
    }

    public void OnMouseMovement(CallbackContext context)
    {
        Vector2 mousePosition = context.ReadValue<Vector2>();
        float xUpperBound = Screen.width - Config.boundaryFraction * Screen.width;
        float xLowerBound = Config.boundaryFraction * Screen.width;
        float yUpperBound = Screen.height - Config.boundaryFraction * Screen.height;
        float yLowerBound = Config.boundaryFraction * Screen.height;
        Vector3 movementDelta = Vector3.zero;
        if (mousePosition.x > xUpperBound)
        {
            movementDelta.x =  (mousePosition.x - xUpperBound) / xLowerBound;
        }
        else if (mousePosition.x < xLowerBound)
        {
            movementDelta.x = -(xLowerBound - mousePosition.x) / xLowerBound;
       }

        if (mousePosition.y > yUpperBound)
        {
            movementDelta.z = (mousePosition.y - yUpperBound) / yLowerBound;
        }
        else if (mousePosition.y < yLowerBound)
        {
            movementDelta.z = -(yLowerBound - mousePosition.y) / yLowerBound;

        }
        if(movementDelta != Vector3.zero)
        {
            isMoving = true;
            if(movementDelta.x != 0 && movementDelta.z != 0)
            {
                moveBy = new Vector3(Mathf.Clamp(movementDelta.x, -.71f, .71f), 0, Mathf.Clamp(movementDelta.z, -.71f, .71f));
            }
            else
            {
                moveBy = movementDelta;
            }
        }
        else
        {
            isMoving = false;
        }

    }
    public void OnRotation(CallbackContext context)
    {
        if (!isRotationDisabled)
        {
        float direciton = context.ReadValue<float>();
        OnCameraRotation?.Invoke(direciton);
        }
        
    }
    public void OnZoom(CallbackContext context)
    {
        if (!isZoomDisabled)
        {
            Vector2 zoom = context.ReadValue<Vector2>();
            OnCameraZoom?.Invoke(zoom.y / 3);
        }
    }

    public void OnNumberKeyPressed(CallbackContext context)
    {
        if (context.started)
        {
            OnNumberPressed?.Invoke(((KeyControl)context.control).keyCode - UnityEngine.InputSystem.Key.Digit1);
        }
    }

    public void OnF1KeyPressed(CallbackContext context)
    {
        if (context.started)
        {
            OnF1Pressed?.Invoke(KeyCode.F1);
        }
    }

    public void OnTildeKeyPressed(CallbackContext context)
    {
        if (context.started)
        {
            OnTildePressed?.Invoke(KeyCode.BackQuote);
        }
    }

    public void OnCKeyPressed(CallbackContext context)
    {
        if (context.started)
        {
            OnCPressed?.Invoke();
        }
    }
    public void OnTabKeyPressed(CallbackContext context)
    {
        if (context.started)
        {
            OnTabPressed?.Invoke();
        }
    }

    // Used as an event handler in Game manager. This way UI manager can talk to this manager
    public void DisableCameraPan(bool active) {
        isPanDisabled = active; 
    }
    public void SetTopDownMode(bool active)
    {
        isInTopDown = active;
    }
    public void DisableCameraRotation(bool active)
    {
        isRotationDisabled = active;
    }
    public void DisableCameraZoom(bool active)
    {
        isZoomDisabled = active;
    }
}
