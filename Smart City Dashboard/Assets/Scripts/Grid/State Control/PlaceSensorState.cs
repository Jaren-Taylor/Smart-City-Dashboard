using UnityEngine;

class PlaceSensorState : DrawsWithCursorState
{
    public readonly SensorType sensor;
    public PlaceSensorState(SensorType sensorType) => sensor = sensorType;

    public override string GetIconPrefabAddress() => sensor switch
    {
        SensorType.Camera => "Prefabs/UI/Camera_Icon",
        SensorType.TrafficLight => "Prefabs/Traffic_Lights/Traffic_Light",
        _ => "",
    };

    public override void OnMouseDown(DigitalCursor location)
    {
        if (location.IsValid() && SensorManager.TryCreateSensorAt(location.Position, sensor))
        {
            ClickCursor();
        }
    }
}
