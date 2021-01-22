using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePlacement : MonoBehaviour
{
    public GameObject gridPoint;
    public GameObject tile;

    [Range(minGridSize, maxGridSize)]
    public int gridSize;
    private const int minGridSize = 1;
    private const int maxGridSize = 100;

    private const int unitVectorX = 2;
    private const int unitVectorZ = 2;

    // Start is called before the first frame update
    void Start()
    {
        // create c# jagged array
        GameObject[,] tileGrid = new GameObject[gridSize, gridSize];

        Vector3 posVector = new Vector3(0, 0, 0);
        GameObject tempTile;
        // fill grid with points. this will always produce a square grid
        for (var i = 0; i < gridSize; i++) {
            for (var j = 0; j < gridSize; j++) {
                tempTile = Instantiate(gridPoint, posVector, Quaternion.Euler(-90, 0, 0));
                tileGrid[i, j] = tempTile;
                // update next position
                posVector.z -= unitVectorZ;
            }
            posVector.z = 0;
            posVector.x -= unitVectorX;
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}