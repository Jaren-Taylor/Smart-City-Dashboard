using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
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
    private GameObject[,] tileGrid;

    // Start is called before the first frame update
    void Start()
    {
        // create c# jagged array
        tileGrid = new GameObject[gridSize, gridSize];

        Vector3 posVector = new Vector3(0, 0, 0);
        GameObject tempTile;
        // fill grid with points. this will always produce a square grid
        for (int i = 0; i < gridSize; i++) {
            for (int j = 0; j < gridSize; j++) {
                tempTile = Instantiate(gridPoint, posVector, Quaternion.Euler(-90, 0, 0));
                tileGrid[i, j] = tempTile;
                GridPoint gridPointScript = tempTile.GetComponent<GridPoint>();
                gridPointScript.parent = this;
                gridPointScript.coords = new Vector2Int(i, j);
                // update next position
                posVector.z -= unitVectorZ;
            }
            posVector.z = 0;
            posVector.x -= unitVectorX;
        }
    }

    public GameObject WhatRoadTileAmI(Vector2Int coords) {
        GameObject road;
        Vector3 position = tileGrid[coords.x, coords.y].transform.position;
        int count = 0;
        bool left = false;
        bool right = false;
        bool top = false;
        bool bottom = false;

        if (coords.x-1 >= 0) {
            gridPoint = tileGrid[coords.x-1, coords.y];
            if (gridPoint.GetComponent<GridPoint>().activeTile != null) {
                Debug.Log("w");
                count++;
                left = true;
            }
        }
        if (coords.x+1 < gridSize) {
            gridPoint = tileGrid[coords.x+1, coords.y];
            if (gridPoint.GetComponent<GridPoint>().activeTile != null) {
                Debug.Log("w");
                count++;
                right = true;
            }
        }
        if (coords.y-1 >= 0) {
            gridPoint = tileGrid[coords.x, coords.y-1];
            if (gridPoint.GetComponent<GridPoint>().activeTile != null) {
                Debug.Log("w");
                count++;
                top = true;
            }
        }
        if (coords.y+1 < gridSize) {
            gridPoint = tileGrid[coords.x, coords.y+1];
            if (gridPoint.GetComponent<GridPoint>().activeTile != null) {
                Debug.Log("w");
                count++;
                bottom = true;
            }
        }

        GameObject prefab = null;
        Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        switch (count) {
            case 0:
                prefab = road0Way;
                break;
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
            case 4:
                prefab = road4Way;
                break;
        }
        return Instantiate(prefab, position, rotation);
    }
}