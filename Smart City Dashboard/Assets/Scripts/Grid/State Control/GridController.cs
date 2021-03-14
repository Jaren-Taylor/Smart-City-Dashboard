using System.Collections.Generic;
/// <summary>
/// Accepts control states to control the grid using
/// </summary>
public class GridController
{
    /// <summary>
    /// Current state that is operating
    /// </summary>
    private IGridControlState activeControlState;
    public EGridControlState state;

    /// <summary>
    /// Dictionary of available control states
    /// </summary>
    public Dictionary<EGridControlState, IGridControlState> controlStates = new Dictionary<EGridControlState, IGridControlState>();

    /// <summary>
    /// Creates grid controller with the inital state
    /// </summary>
    /// <param name="initState"></param>
    public GridController(EGridControlState initState) {
        controlStates.Add(EGridControlState.PlaceRoads,     new PlaceRoadState());
        controlStates.Add(EGridControlState.PlaceHouse,     new PlaceStructureState(BuildingTile.StructureType.House));
        controlStates.Add(EGridControlState.PlaceOffice,    new PlaceStructureState(BuildingTile.StructureType.Office));
        controlStates.Add(EGridControlState.PlaceCamera,    new PlaceSensorState(SensorType.Camera));
        controlStates.Add(EGridControlState.PlaceSmartTrafficLight, new PlaceSensorState(SensorType.TrafficLight));
        controlStates.Add(EGridControlState.ExamineTile,    new ExamineTileSensorsState());
        controlStates.Add(EGridControlState.DeleteTile,     new RemoveTileState());
        controlStates.Add(EGridControlState.SelectEntity,   new SelectEntityState());
        SetState(initState, DigitalCursor.Empty);
    }

    /// <summary>
    /// Replaces active state with the new state
    /// </summary>
    /// <param name="newState">New state to enable</param>
    /// <param name="mousePosition"</param>
    public void SetState(EGridControlState newState, DigitalCursor mousePosition)
    {
        activeControlState?.OnPop(mousePosition);
        activeControlState = controlStates[newState];
        activeControlState?.OnPush(mousePosition);
        state = newState;
    }

    //private void PlaceRoadsHandler() => OnUIButtonClick?.Invoke(EGridControlState.PlaceRoads);
    //private void PlaceBuildingsHandler() => OnUIButtonClick?.Invoke(EGridControlState.PlaceBuildings);
    //private void DeleteModeHandler() => OnUIButtonClick?.Invoke(EGridControlState.DeleteMode);

    /// <summary>
    /// Handles moving a cursor from one tile to another
    /// </summary>
    /// <param name="oldPosition"></param>
    /// <param name="newPosition"></param>
    public void MoveCursor(DigitalCursor oldPosition, DigitalCursor newPosition)
    {
        if (activeControlState == null) throw new System.Exception("State not set");
        activeControlState.OnMouseExitTile(oldPosition);
        activeControlState.OnMouseEnterTile(newPosition);
    }

    /// <summary>
    /// Calls the mouse down logic for the current state
    /// </summary>
    /// <param name="location">Tile that the cursor was hovering over</param>
    public void OnMouseDown(DigitalCursor mousePosition)
    {
        activeControlState?.OnMouseDown(mousePosition);
    }

    public void SuspendState(DigitalCursor mousePosition)
    {
        activeControlState?.OnPop(mousePosition);
    }

    public void ResumeState(DigitalCursor mousePosition)
    {
        activeControlState?.OnPush(mousePosition);
    }
}
