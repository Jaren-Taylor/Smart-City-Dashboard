using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CameraSensorData
{
    public GameObject DetectedObject { get; }
    public Vector3 Position { get; }
    public Vector3 Velocity { get; }
    public Vector3 SelfPostion { get; }
    public Vector3 SelfVelocity { get; }
    
    public CameraSensorData(GameObject detectedObject, Vector3 position, Vector3 velocity, Vector3 selfPostion, Vector3 selfVelocity)
    {
        DetectedObject = detectedObject;
        Position = position;
        Velocity = velocity;
        SelfPostion = selfPostion;
        SelfVelocity = selfVelocity;
    }
}
