using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridControlState
{
    public void OnPush(DigitalCursor location); //Setup state to be run
    public void OnPop(DigitalCursor location); //Teardown state 
    public void OnMouseExitTile(DigitalCursor location); //When the cursor is exiting a tile location (may be null)
    public void OnMouseEnterTile(DigitalCursor location); //When a cursor is entering a tile location (may be null)
    public void OnMouseDown(DigitalCursor location); //When a mouse down event has occured at this location (may be null)
}
