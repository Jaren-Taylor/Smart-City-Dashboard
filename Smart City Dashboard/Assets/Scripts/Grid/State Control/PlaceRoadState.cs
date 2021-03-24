using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Places roads upon clicking 
/// </summary>
public class PlaceRoadState : IGridControlState
{

    public void OnPop(DigitalCursor location)
    {
        OnMouseExitTile(location); //Removes tile that was currently being previewed
    }

    public void OnPush(DigitalCursor location)
    {
        OnMouseEnterTile(location); //Creates a temp tile for the preview
    }

    /// <summary>
    /// Sets tile to permanent
    /// </summary>
    /// <param name="location"></param>
    public void OnMouseDown(DigitalCursor location)
    {
        if (location.IsValid() && GridManager.GetTile(location.Position) is RoadTile road && !road.IsPermanent)
        {
            GridManager.Instance.MakePermanent(location.Position);
            GridManager.Instance.Grid.RoadAdded(location.Position);
        }
    }


    /// <summary>
    /// Creates a temp tile at location
    /// </summary>
    /// <param name="location"></param>
    public void OnMouseEnterTile(DigitalCursor location)
    {
        if (location.IsValid()) GridManager.Instance.CreateTemporaryTile<RoadTile>(location.Position);
    }

    /// <summary>
    /// Deletes temp tile from loctation
    /// </summary>
    /// <param name="location"></param>
    public void OnMouseExitTile(DigitalCursor location)
    {
        if (location.IsValid()) GridManager.Instance.RemoveTileIfTemporary(location.Position);
    }
}
