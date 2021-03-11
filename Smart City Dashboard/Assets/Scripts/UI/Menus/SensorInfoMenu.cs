using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorInfoMenu : MonoBehaviour,IFocusableWindow
{
    //TODO: Programatically show traffic light info on the UI, allow user to change flow via red buttons. Find correct orientation and disable the appropriate button for 3 way roads. Get light countdown time and display.

    [SerializeField]
    private GameObject sensorInfoMenu;
    public Action<bool> DisableCameraControls;
    private bool isShowing = false;
    private void Start()
    {
        sensorInfoMenu.SetActive(isShowing);
    }

    public void SetVisible(Vector3 tileTransform)
    {

        //rotates the SensorInfoMenu to match the camera's rotation

        if(GridManager.Instance.Grid[tileTransform.ToGridInt()] is RoadTile road)
        {
            switch(road.Type)
            {
                case RoadTile.TileType.Road3Way:

                    break;

                case RoadTile.TileType.Road4Way:
                    ToggleMenuHandler();
                    DisableCameraControls?.Invoke(IsFullyVisible());
                    break;
                default:

                    break;
            }
        }
        if(IsFullyVisible())
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + GameObject.Find("Camera Rig").transform.rotation.eulerAngles.y);
        CameraManager.Instance.OnReachedTarget += SetVisible;
    }

    public bool IsFullyVisible()
    {
        return sensorInfoMenu.activeSelf;
    }

    public void OnNumberKeyPress(int value)
    {
        return;
    }

    public void ToggleMenuHandler()
    {
        isShowing = !isShowing;
        sensorInfoMenu.SetActive(isShowing);
    }
    public void ChangeTrafficFlow()
    {
        Debug.Log("Should be changing the flow of traffic");

    }
}
