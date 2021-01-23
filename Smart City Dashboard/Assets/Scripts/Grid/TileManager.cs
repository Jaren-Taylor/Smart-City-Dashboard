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
                //tempTile.GetComponent<GridPoint>;
                // update next position
                posVector.z -= unitVectorZ;
            }
            posVector.z = 0;
            posVector.x -= unitVectorX;
        }
        // set adjacency lists for each gridpoint
        /*GridPoint gridPointScript;
        for (var i = 0; i < gridSize; i++) {
            for (var j = 0; j < gridSize; j++) {
                for (var k = 0; k < 8; k++) {
                    gridPointScript = tileGrid[i, j].GetComponent<GridPoint>();
                    // check for edge cases
                    if (i > 0 && i < gridSize) {
                        gridPointScript.adjacencyList[k] = tileGrid[i, j];
                    }
                }
            }
        }*/
    }

    // Update is called once per frame
    void Update() {
        
    }
}