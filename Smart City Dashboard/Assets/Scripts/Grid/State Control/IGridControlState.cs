using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Interface for creating states used to control the building mode
/// </summary>
public interface IGridControlState
{
    /// <summary>
    /// Called when the state is entered. Setup for functioning 
    /// </summary>
    /// <param name="location">Location where cursor is upon entering the state</param>
    public void OnPush(DigitalCursor location); //Setup state to be run

    /// <summary>
    /// Called when the state is removed. Teardown for functioning
    /// </summary>
    /// <param name="location"></param>
    public void OnPop(DigitalCursor location); //Teardown state 

    /// <summary>
    /// Called when the mouse is leaving a tile.  
    /// </summary>
    /// <param name="location">The tile that the cursor is leaving from</param>
    public void OnMouseExitTile(DigitalCursor location); //When the cursor is exiting a tile location (may be null)

    /// <summary>
    /// Called when the mouse is entering a tile.
    /// </summary>
    /// <param name="location">The tile that the cursor is leaving from</param>
    public void OnMouseEnterTile(DigitalCursor location); //When a cursor is entering a tile location (may be null)


    /// <summary>
    /// Called when a mousedown event has occured while mouse is over tile
    /// </summary>
    /// <param name="location">Tile that the cursor was hovering over</param>
    public void OnMouseDown(DigitalCursor location); //When a mouse down event has occured at this location (may be null)
}
