using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSensor : Sensor<CameraSensorData>
{
    public override void DeregisterFromManager(SensorManager sensor)
    {
        if (sensor.CameraSet.Contains(this))
        {
            sensor.CameraSet.Remove(this);
            this.DataCollected -= sensor.OnReceiveCameraData;
        }
    }

    public override void RegisterToManager(SensorManager sensor)
    {
        if (!sensor.CameraSet.Contains(this))
        {
            sensor.CameraSet.Add(this);
            this.DataCollected += sensor.OnReceiveCameraData;
        }
    }

    protected override CameraSensorData CollectData(GameObject sensedObject)
    {
        return new CameraSensorData(sensedObject,
            sensedObject.transform.position,
            sensedObject.GetComponent<Rigidbody>().velocity);
    }

    public override string GetSimpleName()
    {
        throw new System.NotImplementedException();
    }

    public override string ToString()
    {
        return "Camera Sensor";
    }
}
