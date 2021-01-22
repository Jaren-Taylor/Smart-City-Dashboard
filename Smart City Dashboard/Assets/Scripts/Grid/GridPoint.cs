using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPoint : MonoBehaviour
{
    public GameObject tile;
    public GameObject activeTile;

    private void Start()
    {
        activeTile = null;
    }

    void OnMouseEnter()
    {
        if (activeTile == null)
        {
            activeTile = Instantiate(tile, transform.position, transform.rotation);
            RoadTile script = activeTile.GetComponent<RoadTile>();
            script.parent = this;
        }
    }

    /*    void OnMouseExit()
        {
            Debug.Log(activeTile.transform.parent != this);
            if (activeTile != null && activeTile.transform.parent == this) Destroy(activeTile);
            activeTile = null;
        }*/
}
