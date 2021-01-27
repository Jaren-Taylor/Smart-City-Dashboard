using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridControlState
{
    public void OnPush(Vector2Int? location); //Setup state to be run
    public void OnPop(Vector2Int? location); //Teardown state 
    public void OnMouseExitTile(Vector2Int? location); //When the cursor is exiting a tile location (may be null)
    public void OnMouseEnterTile(Vector2Int? location); //When a cursor is entering a tile location (may be null)
    public void OnMouseDown(Vector2Int? location); //When a mouse down event has occured at this location (may be null)
}
