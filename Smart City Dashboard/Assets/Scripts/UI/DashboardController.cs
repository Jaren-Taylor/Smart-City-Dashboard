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
        CreateHeatMap(100, 100);
    }

    public void CreateHeatMap(int width, int height)
    {
        heatmap = new HeatMap(width, height);
        dashboardMenu.UpdateSpriteFromTexture(heatmap.CreatePNG());
    }
}
