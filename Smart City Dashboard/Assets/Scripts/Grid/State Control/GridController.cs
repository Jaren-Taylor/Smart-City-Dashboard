using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController
{
    private IGridControlState activeState;

    public GridController(IGridControlState initState) => activeState = initState;

    public void SetState(IGridControlState newState, DigitalCursor mousePosition)
    {
        if (IsValid(mousePosition))
        {
            activeState?.OnPop(mousePosition);
            activeState = newState;
            activeState?.OnPush(mousePosition);
        }
        else activeState = newState;
    }

    public void MoveCursor(DigitalCursor oldPosition, DigitalCursor newPosition)
    {
        if (activeState == null) throw new System.Exception("State not set");
        if(IsValid(oldPosition)) activeState.OnMouseExitTile(oldPosition);
        if(IsValid(newPosition)) activeState.OnMouseEnterTile(newPosition);
    }

    public void OnMouseDown(DigitalCursor mousePosition)
    {
        if(IsValid(mousePosition)) activeState?.OnMouseDown(mousePosition);
    }

    private bool IsValid(DigitalCursor cursor) => cursor?.OnGrid ?? false;
}
