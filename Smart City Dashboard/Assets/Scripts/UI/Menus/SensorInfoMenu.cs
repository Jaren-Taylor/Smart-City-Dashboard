using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensorInfoMenu : MonoBehaviour,IFocusableWindow
{
    //TODO: Programatically show traffic light info on the UI, allow user to change flow via red buttons. Get light countdown time and display.

    [SerializeField]
    private GameObject sensorInfoMenu;
    [SerializeField]
    private GameObject NorthLight;
    [SerializeField]
    private GameObject EastLight;
    [SerializeField]
    private GameObject WestLight;
    [SerializeField]
    private GameObject SouthLight;
    [SerializeField]
    private GameObject ArrowTop;
    [SerializeField]
    private GameObject ArrowBottom;
    [SerializeField]
    private GameObject ButtonAndTextCanvas;
    [SerializeField]
    TextMeshProUGUI DelayText;
    [SerializeField]
    private GameObject ResetViewButton;
    public Action<bool> DisableCameraControls;
    public Action ToggleCursor;
    private RoadTile currentRoad;
    private TrafficLightController currentTrafficLight;
    private bool isCurrentlyEastWest  = false;
    private bool isShowing = false;
    private bool hasRun = false;
    private void Start()
    {
        sensorInfoMenu.SetActive(isShowing);
        ButtonAndTextCanvas.SetActive(isShowing);
        ResetViewButton.SetActive(isShowing);
    }
    void Update()
    {

        if (IsFullyVisible())
        {
            currentTrafficLight.TurnedGreen += UpdateView;
            DelayText.SetText((currentTrafficLight.switchDelay - currentTrafficLight.totalTime + 1).ToString("N1"));
        }
        
    }

    private void UpdateView()
    {
        if (!hasRun)
        {
            if (currentTrafficLight.isEastWest)
            {
                SwapLights();
            }
        }
        if (hasRun && currentTrafficLight.isEastWest != isCurrentlyEastWest)
        {
            SwapLights();
        }
        hasRun = true;
        ResetLights();
        if (currentRoad.Type == RoadTile.TileType.Road3Way)
        {
            DetermineRotation(currentRoad);
        }
    }

    private void SwapLights()
    {
        Sprite temp = NorthLight.GetComponent<Image>().sprite;
        NorthLight.GetComponent<Image>().sprite = EastLight.GetComponent<Image>().sprite;
        EastLight.GetComponent<Image>().sprite = temp;
        temp = SouthLight.GetComponent<Image>().sprite;
        SouthLight.GetComponent<Image>().sprite = WestLight.GetComponent<Image>().sprite;
        WestLight.GetComponent<Image>().sprite = temp;
        RotateArrows();
        isCurrentlyEastWest = !isCurrentlyEastWest;
    }

    private void RotateArrows()
    {
        ArrowTop.transform.localPosition = new Vector3(-ArrowTop.transform.localPosition.x, ArrowTop.transform.localPosition.y, ArrowTop.transform.localPosition.z);
        ArrowBottom.transform.localPosition = new Vector3(-ArrowBottom.transform.localPosition.x, ArrowBottom.transform.localPosition.y, ArrowBottom.transform.localPosition.z);
        ArrowTop.transform.localRotation = Quaternion.Euler(0, 0, -ArrowTop.transform.localRotation.eulerAngles.z);
        ArrowBottom.transform.localRotation = Quaternion.Euler(0, 0, -ArrowBottom.transform.localRotation.eulerAngles.z);
    }

    public void SetVisible(Vector3 tileTransform)
    {
        DisableMenu();
        hasRun = false;
        ResetLights();
        var tilePosition = tileTransform.ToGridInt();
        ResetViewButton.SetActive(true);
        ToggleCursor?.Invoke();
        if (GridManager.Instance.Grid[tilePosition] is RoadTile road && !(road.TrafficLight is null))
        {
            currentRoad = road;
            currentTrafficLight = road.TrafficLight;
            isCurrentlyEastWest = road.TrafficLight.isEastWest;
            switch (road.Type)
            {
                case RoadTile.TileType.Road3Way:
                    EnableMenu();
                    DetermineRotation(road);
                    break;

                case RoadTile.TileType.Road4Way:
                    EnableMenu();
                    break;
                default:

                    break;
            }
        }
        if (IsFullyVisible())
        {
            //rotates the SensorInfoMenu to match the camera's rotation
            CameraManager.Instance.DisableUserInput();
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + GameObject.Find("Camera Rig").transform.rotation.eulerAngles.y);
            ButtonAndTextCanvas.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - GameObject.Find("Camera Rig").transform.rotation.eulerAngles.y);
        }
        CameraManager.Instance.OnReachedTarget += SetVisible;
    }

    public void DisableUserInput()
    {
        DisableCameraControls?.Invoke(true);
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
        ButtonAndTextCanvas.SetActive(isShowing);
    }
    public void EnableMenu()
    {
        isShowing = true;
        sensorInfoMenu.SetActive(isShowing);
        ButtonAndTextCanvas.SetActive(isShowing);
        ResetViewButton.SetActive(isShowing);
    }
    public void DisableMenu()
    {
        isShowing = false;
        sensorInfoMenu.SetActive(isShowing);
        ButtonAndTextCanvas.SetActive(isShowing);
        ResetViewButton.SetActive(isShowing);
    }
    public void ChangeTrafficFlow()
    {
        currentTrafficLight.ChangeTrafficDirection();
    }
    public void DetermineRotation(RoadTile tile)
    {

        switch (tile.rotation)
        {
            case (Tile.Facing.Top):
                SouthLight.SetActive(false);
                break;
            case (Tile.Facing.Left):
                EastLight.SetActive(false);
                break;
            case (Tile.Facing.Bottom):
                NorthLight.SetActive(false);
                break;
            default:
                WestLight.SetActive(false);
                break;
        };
    }
    public void ResetLights()
    {
        NorthLight.SetActive(true);
        EastLight.SetActive(true);
        WestLight.SetActive(true);
        SouthLight.SetActive(true);
    }
    public void OnResetClicked()
    {

        DisableMenu();
        DisableCameraControls?.Invoke(false);
        ToggleCursor?.Invoke();
        CameraManager.Instance.StopTrackObject();
    }
}
