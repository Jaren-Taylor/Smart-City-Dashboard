using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPoint : MonoBehaviour
{
    public TileManager parent;
    public GameObject activeTile;
    public GameObject[] adjacencyList = new GameObject[4];

    private void Start()
    {
        activeTile = null;
    }

    void OnMouseEnter()
    {
        if (activeTile == null)
        {
            /*activeTile = Instantiate(tile, transform.position, transform.rotation);
            RoadTile script = activeTile.GetComponent<RoadTile>();
            script.parent = this;*/
        }
    }
}
