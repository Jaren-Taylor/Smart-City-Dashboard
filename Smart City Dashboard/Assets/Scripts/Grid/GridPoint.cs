using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPoint : MonoBehaviour
{
    public TileManager parent;
    public GameObject activeTile;
    public Vector2Int coords;

    private void Start()
    {
        activeTile = null;
    }

    void OnMouseEnter()
    {
        if (activeTile == null)
        {
            activeTile = parent.WhatRoadTileAmI(coords);
            RoadTile script = activeTile.GetComponent<RoadTile>();
            script.parent = this;
        }
    }
}
