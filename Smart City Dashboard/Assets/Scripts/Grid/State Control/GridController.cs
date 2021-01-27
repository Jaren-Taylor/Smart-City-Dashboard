using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController
{
    private IGridControlState activeState;

    public GridController(IGridControlState initState) => activeState = initState;

    public void SetState(IGridControlState newState, Vector2Int? mousePosition)
    {
        activeState?.OnPop(mousePosition);
        activeState = newState;
        activeState?.OnPush(mousePosition);
    }

    public void MoveCursor(Vector2Int? oldPosition, Vector2Int? newPosition)
    {
        if (activeState == null) throw new System.Exception("State not set");
        activeState.OnMouseExitTile(oldPosition);
        activeState.OnMouseEnterTile(newPosition);
    }

    public void OnMouseDown(Vector2Int? mousePosition) => activeState?.OnMouseDown(mousePosition);
}
