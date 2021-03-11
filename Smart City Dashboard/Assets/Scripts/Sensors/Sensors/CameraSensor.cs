using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSensor : Sensor<CameraSensorData>
{
    public CameraSensor(Vector2Int position) : base(position) { }

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
        return ("Test", SensorStatus.Fine);
    }
}
