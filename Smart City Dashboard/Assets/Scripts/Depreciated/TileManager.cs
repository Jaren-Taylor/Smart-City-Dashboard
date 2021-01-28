using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileManager : MonoBehaviour
{
    /*
    public GameObject gridPoint;
    public GameObject road0Way;
    public GameObject road2Way;
    public GameObject road3Way;
    public GameObject road4Way;
    public GameObject roadEndCap;
    public GameObject roadCorner;


    [Range(minGridSize, maxGridSize)]
    public int gridSize;
    private const int minGridSize = 1;
    private const int maxGridSize = 100;

    private const int unitVectorX = 2;
    private const int unitVectorZ = 2;
    private GameObject[,] tileGrid; // strictly contains GridPoint GameObjects

    // Start is called before the first frame update
    void Start()
    {
        // create c# jagged array
        tileGrid = new GameObject[gridSize, gridSize];
        // create vector used to position the grid points
        Vector3 posVector = new Vector3(0, 0, 0);

        // fill grid with points. this will always produce a square grid
        for (int i = 0; i < gridSize; i++) {
            for (int j = 0; j < gridSize; j++) {
                tileGrid[i, j] = Instantiate(gridPoint, posVector, Quaternion.Euler(-90, 0, 0));
                GridPoint gridPointScript = tileGrid[i, j].GetComponent<GridPoint>();
                gridPointScript.parent = this;
                gridPointScript.coords = new Vector2Int(i, j);
                // increment column
                posVector.z -= unitVectorZ;
            }
            // reset column
            posVector.z = 0;
            // increment row
            posVector.x -= unitVectorX;
        }
    }

    public enum TileOrientation {
        center,
        left,
        right,
        top,
        bottom,
        nothing
    }
    public GameObject WhatRoadTileAmI(int x, int y, TileOrientation orientation, bool onlyNeighbors) {
        int count = 0; // how many adjacent tiles are there
        bool left = false;
        bool right = false;
        bool top = false;
        bool bottom = false;

        // figures out what tiles are around the tile in question
        // left
        if (x - 1 >= 0) {
            if ((orientation == TileOrientation.right && !onlyNeighbors) || HasActiveTile(x - 1, y)) {
                count++;
                left = true;
                // recurses on adjacent tile
                if (orientation == TileOrientation.center) tileGrid[x - 1, y].GetComponent<GridPoint>().ChangeTile(x - 1, y, TileOrientation.left, onlyNeighbors);
            }
        }
        // right
        if (x + 1 < gridSize) {
            if ((orientation == TileOrientation.left && !onlyNeighbors) || HasActiveTile(x + 1, y)) {
                count++;
                right = true;
                // recurses on adjacent tile
                if (orientation == TileOrientation.center) tileGrid[x + 1, y].GetComponent<GridPoint>().ChangeTile(x + 1, y, TileOrientation.right, onlyNeighbors);
            }
        }
        // top
        if (y - 1 >= 0) {
            if ((orientation == TileOrientation.bottom && !onlyNeighbors) || HasActiveTile(x, y - 1)) {
                count++;
                top = true;
                // recurses on adjacent tile
                if (orientation == TileOrientation.center) tileGrid[x, y - 1].GetComponent<GridPoint>().ChangeTile(x, y - 1, TileOrientation.top, onlyNeighbors);
            }
        }
        // bottom
        if (y + 1 < gridSize) {
            if ((orientation == TileOrientation.top && !onlyNeighbors) || HasActiveTile(x, y + 1)) {
                count++;
                bottom = true;
                // recurses on adjacent tile
                if (orientation == TileOrientation.center) tileGrid[x, y + 1].GetComponent<GridPoint>().ChangeTile(x, y + 1, TileOrientation.bottom, onlyNeighbors);
            }
        }

        //Debug.Log(left+" "+right + " " +top + " " +bottom);
        // all this switch statement does it handle which way the tile should be rotated
        GameObject prefab = null;
        Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        switch (count) {
            // 0 way
            case 0:
                prefab = road0Way;
                break;
            // end cap
            case 1:
                prefab = roadEndCap;
                if (bottom) {
                    rotation = Quaternion.Euler(-90, 180, 0);
                }
                else if (right) {
                    rotation = Quaternion.Euler(-90, -90, 0);
                }
                else if (left) {
                    rotation = Quaternion.Euler(-90, 90, 0);
                }
                break;
            // 2 way or corner
            case 2:
                if (right && top || right && bottom || left && top || left && bottom) {
                    prefab = roadCorner;
                    if (left && bottom) {
                        rotation = Quaternion.Euler(-90, 180, 0);
                    } else if (right && bottom) {
                        rotation = Quaternion.Euler(-90, -90, 0);
                    } else if (left && top) {
                        rotation = Quaternion.Euler(-90, 90, 0);
                    }
                }
                else { 
                    prefab = road2Way;
                    if (right) rotation = Quaternion.Euler(-90, 90, 0);
                }
                break;
            // 3 way
            case 3:
                prefab = road3Way;
                if (!top) {
                    rotation = Quaternion.Euler(-90, 180, 0);
                } else if (!right) {
                    rotation = Quaternion.Euler(-90, 90, 0);
                } else if (!left) {
                    rotation = Quaternion.Euler(-90, -90, 0);
                }
                break;
            // 4 way
            case 4:
                prefab = road4Way;
                break;
        }

        if (onlyNeighbors && orientation == TileOrientation.center) {
            return null;
        } else {
            return Instantiate(prefab, tileGrid[x, y].transform.position, rotation);
        }
    }

    private bool HasActiveTile(int x, int y) {
        gridPoint = tileGrid[x, y];
        return gridPoint.GetComponent<GridPoint>().activeTile != null && gridPoint.GetComponent<GridPoint>().activeTile.GetComponent<RoadTile>().isPermanent;
    }*/
}