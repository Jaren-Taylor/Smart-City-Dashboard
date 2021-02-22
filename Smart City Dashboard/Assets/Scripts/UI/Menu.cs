using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Menu : MonoBehaviour
{
    public Action<EGridControlState> OnUIButtonClick;
    public List<ButtonMapping> buttonMappings;
    protected RectTransform menuBounds;
    [HideInInspector]
    public int ActiveTab;
    [HideInInspector]
    public bool isOnScreen;

    protected void Start()
    {
        // Assumed to be used in child classes for use in movement calculations
        menuBounds = gameObject.GetComponent<RectTransform>();
        // Deactivate all but the first child tab
        for (int i = 2; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.GetComponent<Tab>().DeActivate();
        }
        //
        ActiveTab = 1;
        isOnScreen = false;
    }

    public void SwitchTabs()
    {
        // deactivate current tab
        transform.GetChild(ActiveTab).GetComponent<Tab>().DeActivate();
        // increment or reset counter
        ActiveTab = ActiveTab == transform.childCount - 1 ? 1 : ActiveTab + 1;
        // activate new tab
        transform.GetChild(ActiveTab).GetComponent<Tab>().Activate();
    }

    public abstract void ToggleMenuHandler();

    /*  public void MapFunctionToButton(ButtonMapping buttonMapping, Button button)
      {
          switch (buttonMapping.ControlState)
          {
              case EGridControlState.PlaceRoads:
                  buttonMapping.button.onClick.AddListener(PlaceRoadsHandler);
                  break;
              case EGridControlState.PlaceBuildings:
                  buttonMapping.button.onClick.AddListener(PlaceBuildingsHandler);
                  break;
              case EGridControlState.DeleteMode:
                  buttonMapping.button.onClick.AddListener(DeleteModeHandler);
                  break;
          }

      }*/


    public EGridControlState controlState;

    //private void PlaceRoadsHandler() => OnUIButtonClick?.Invoke(EGridControlState.PlaceRoads);
    //private void PlaceBuildingsHandler() => OnUIButtonClick?.Invoke(EGridControlState.PlaceBuildings);
    //private void DeleteModeHandler() => OnUIButtonClick?.Invoke(EGridControlState.DeleteMode);
}