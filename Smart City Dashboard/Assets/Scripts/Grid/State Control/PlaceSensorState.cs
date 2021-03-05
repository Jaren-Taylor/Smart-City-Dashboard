using UnityEngine;

class PlaceSensorState : DrawsWithCursorState
{
    public readonly SensorType sensor;
    public PlaceSensorState(SensorType sensorType) => sensor = sensorType;

    public override string GetIconPrefabAddress() => "Prefabs/UI/Camera_Icon";

    public override void OnMouseDown(DigitalCursor location)
    {
        if (location.OnGrid && SensorManager.TryCreateSensorAt(location.Position, sensor))
        {
            ClickCursor();
        }
    }
}
