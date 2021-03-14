using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSensorMenu : Menu
{
    public UIBackgroundSprite CardColor;
    [SerializeField]
    private ScrollablePopupMenu menu;

    private Tile focusedTile = null;

    // Start is called before the first frame update
    void Start()
    {
        Close();
    }

    internal void FocusTile(Vector2Int position)
    {
        Tile tile = GridManager.GetTile(position);
        focusedTile = null;
        PopulateFromTile(tile);

        if (focusedTile is Tile) TrySetMenuTo(position);
    }

    private void TrySetMenuTo(Vector2Int tilePosition)
    {
       transform.position = Camera.main.WorldToScreenPoint(new Vector3(tilePosition.x, 0f, tilePosition.y));
    }

    private void PopulateFromTile(Tile tile)
    {
        if (tile is null) throw new Exception("Cannot inspect tile that does not exist");
        menu.Clear();
        focusedTile = tile;
        foreach(SensorType sensorType in tile.Sensors)
        {
            AddSensorToMenu(sensorType);
        }
    }

    private void AddSensorToMenu(SensorType sensor)
    {
        var card = menu.AddNewItem(CardColor, sensor.ToString());
        card.OnRemoveClicked.AddListener((_) => focusedTile.RemoveSensor(sensor));
    }
}
