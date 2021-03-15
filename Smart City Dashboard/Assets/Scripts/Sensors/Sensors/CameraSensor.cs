using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSensor : Sensor<CameraSensorData>
{
    public CameraSensor(Vector2Int position) : base(position) { }

    private static Dictionary<SensorStatus, string> StatusStringMapping = new Dictionary<SensorStatus, string>()
    {
        { SensorStatus.Fine, "No Congestion" },
        { SensorStatus.Meh, "Light Congestion" },
        { SensorStatus.Bad, "Heavy Congestion" }
    };

    protected override CameraSensorData CollectData(GameObject sensedObject)
    {
        return new CameraSensorData(this,
            sensedObject,
            sensedObject.transform.position,
            sensedObject.GetComponent<Rigidbody>().velocity);
    }

    public override string ToString()
    {
        return "Camera Sensor";
    }

    protected override (string msg, SensorStatus status) GetStatus(List<CameraSensorData> collectedData)
    {
        var status = CountToStatus(collectedData.Count);
        return (StatusStringMapping[status], status);
    }

    private SensorStatus CountToStatus(int count)
    {
        if (count < 3) return SensorStatus.Fine;
        else if (count < 6) return SensorStatus.Meh;
        else return SensorStatus.Bad;
    }
}
