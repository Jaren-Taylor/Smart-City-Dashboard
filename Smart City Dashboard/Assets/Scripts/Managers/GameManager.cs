using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public InputManager inputManager;
    public CameraManager cameraManager;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        inputManager.OnCameraPan += cameraManager.PanHandler;
        inputManager.OnCameraPan += Test;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Test(Vector3 test)
    {
        Debug.Log(test);
    }
}
