using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoadTile : MonoBehaviour
{
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
