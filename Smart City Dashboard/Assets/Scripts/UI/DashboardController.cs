using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashboardController : MonoBehaviour
{
    private HeatMap heatmap;
    [SerializeField]
    private DashboardMenu dashboardMenu;

    private void Start()
    {
        SensorManager.Instance.OnHeatMapUpdated += UpdateGraphic;

        //CreateHeatMap(100, 100);
    }

    private void UpdateGraphic(HeatMap heatMap)
    {
        dashboardMenu.UpdateSpriteFromTexture(heatMap.CreatePNG());
    }

    public void CreateHeatMap(int width, int height)
    {
        heatmap = new HeatMap(width, height);
        dashboardMenu.UpdateSpriteFromTexture(heatmap.CreatePNG());
    }
}
