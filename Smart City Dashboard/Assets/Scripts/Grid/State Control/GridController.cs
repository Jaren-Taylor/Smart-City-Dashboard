using System.Collections.Generic;
/// <summary>
/// Accepts control states to control the grid using
/// </summary>
public class GridController
{
    /// <summary>
    /// Current state that is operating
    /// </summary>
    private IGridControlState activeState;

    private enum IGridControlGroups {
        Tiles,
        Sensors,
        Entities,
        Objects
    }

    private Dictionary<IGridControlGroups, IGridControlState[]> controlGroups = new Dictionary<IGridControlGroups, IGridControlState[]>();
    //private IGridControlState[] controlStates = new IGridControlState[4];

    /// <summary>
    /// Creates grid controller with the inital state
    /// </summary>
    /// <param name="initState"></param>
    public GridController(IGridControlState initState) {
        controlGroups.Add(IGridControlGroups.Tiles, new IGridControlState[4]);
        controlGroups[IGridControlGroups.Tiles][0] = new PlaceRoadState();
        controlGroups[IGridControlGroups.Tiles][0] = new PlaceStructureState(BuildingTile.StructureType.House);
        controlGroups[IGridControlGroups.Tiles][0] = new RemoveTileState();
        // TODO
        // controlGroups.Add(IGridControlGroups.Sensors, new IGridControlState[3]);
        // controlGroups.Add(IGridControlGroups.Entities, new IGridControlState[3]);
        // controlGroups.Add(IGridControlGroups.Objects, new IGridControlState[3]);
        activeState = initState;
    }

    /// <summary>
    /// Replaces active state with the new state
    /// </summary>
    /// <param name="newState">New state to enable</param>
    /// <param name="mousePosition"</param>
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

    /// <summary>
    /// Handles moving a cursor from one tile to another
    /// </summary>
    /// <param name="oldPosition"></param>
    /// <param name="newPosition"></param>
    public void MoveCursor(DigitalCursor oldPosition, DigitalCursor newPosition)
    {
        if (activeState == null) throw new System.Exception("State not set");
        if (IsValid(oldPosition)) activeState.OnMouseExitTile(oldPosition);
        if (IsValid(newPosition)) activeState.OnMouseEnterTile(newPosition);
    }

    /// <summary>
    /// Calls the mouse down logic for the current state
    /// </summary>
    /// <param name="location">Tile that the cursor was hovering over</param>
    public void OnMouseDown(DigitalCursor mousePosition)
    {
        if (IsValid(mousePosition)) activeState?.OnMouseDown(mousePosition);
    }

    /// <summary>
    /// Checks if the cursor is valid to use in current state
    /// </summary>
    /// <param name="cursor"></param>
    /// <returns></returns>
    private bool IsValid(DigitalCursor cursor) => cursor?.OnGrid ?? false;

    public void SuspendState(DigitalCursor mousePosition)
    {
        if (IsValid(mousePosition))
        {
            activeState?.OnPop(mousePosition);
        }
    }

    public void ResumeState(DigitalCursor mousePosition)
    {
        if (IsValid(mousePosition))
        {
            activeState?.OnPush(mousePosition);
        }
    }
}
