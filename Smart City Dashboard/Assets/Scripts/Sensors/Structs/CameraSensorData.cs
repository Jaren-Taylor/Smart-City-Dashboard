using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CameraSensorData
{
    public readonly CameraSensor Sender;
    public GameObject DetectedObject { get; }
    public Vector3 Position { get; }
    public Vector3 Velocity { get; }
    
    public CameraSensorData(CameraSensor sender, GameObject detectedObject, Vector3 position, Vector3 velocity)
    {
        Sender = sender;
        DetectedObject = detectedObject;
        Position = position;
        Velocity = velocity;
    }
}
