using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoadTile : MonoBehaviour
{
    public enum TileType { 
        Road0Way = 0,
        RoadEndCap = 1,
        Road2Way = 2,
        Road3Way = 3,
        Road4Way = 4,
        RoadCorner = 5
    }
    public TileType tileType;
    public bool isPermanent;
    public GridPoint parent;


    void Start()
    {
        isPermanent = false;
        parent = null;
    }

    // Update is called once per frame
    void OnMouseExit()
    {
        if (!isPermanent)
        {
            Destroy2();
        }
    }

    void OnMouseDown()
    {
        Debug.Log(isPermanent);
        if (isPermanent) {
            Destroy2();
        } else {
            isPermanent = true;
        }
    }

    private void Destroy2() {
        if (parent != null) parent.activeTile = null;
        Destroy(gameObject);
    }
}
