using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPoint : MonoBehaviour
{ 
    /*
    public TileManager parent;
    public GameObject activeTile;
    public Vector2Int coords;

    private void Start()
    {
        activeTile = null;
    }

    void OnMouseEnter()
    {
        if (activeTile == null) {
            //Debug.Log("Changing tile " + coords.x + " , " + coords.y);
            ChangeTile(coords.x, coords.y, TileManager.TileOrientation.center, false);
        }
    }
    public void ChangeTile(int x, int y, TileManager.TileOrientation orientation, bool onlyNeighbors) {
        RoadTile roadTileScript;
        bool wasPermanent = false;
        // delete an already existing tile
        if (activeTile != null)
        {
            roadTileScript = activeTile.GetComponent<RoadTile>();
            if (roadTileScript.isPermanent) wasPermanent = true;
            roadTileScript.Destroy2();
        }
        // instantiate to what road tile we should be
        activeTile = parent.WhatRoadTileAmI(coords.x, coords.y, orientation, onlyNeighbors);
        if (activeTile != null) { 
            roadTileScript = activeTile.GetComponent<RoadTile>();
            roadTileScript.parent = this;
            roadTileScript.isPermanent = wasPermanent;
        }
    } */
}
