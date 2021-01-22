using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    public Action<Vector3> OnCameraPan;
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


        if (movementDelta != Vector3.zero)
        {
            OnCameraPan?.Invoke(movementDelta);
        }
    }
}
