using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SensorSocket : MonoBehaviour
{
    //Timing variables
    private float totalTime = 0f; 
    private readonly float callDelay = .25f;

    //Used to sense objects in collider
    private HashSet<GameObject> objectsInCollider;
    private Rigidbody sensorRigidBody;
    private Collider sensorShape;

    //Called whenever a sensor inside this socket is removed
    public Action<ISensor> SensorDetached;

    [SerializeField]
    private SensorType sensorType;

    //Sensor Data
    private ISensor sensor;

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        if (!TryGetComponent(out sensorShape)) Debug.LogError("No collider attached to sensor socket");
        if (!TryGetComponent(out sensorRigidBody)) Debug.LogError("No rigidbody attached to sensor socket");
        sensorShape.isTrigger = true;
        objectsInCollider = new HashSet<GameObject>();

        AttachSensor(sensorType);
    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;
        if (totalTime >= callDelay) //Executes code after callDelay seconds have passed (used to force to a slower speed)
        {
            ScanForDeadReferences(); //Clean up any GameObjects that may have despawned before exiting collider
            
            //Checks if anything is actually in the collider to collect data from
            if(AreEntitiesInCollider())
                sensor.CollectDataFrom(objectsInCollider);

            totalTime -= callDelay;
        }
    }

    private void OnTriggerEnter(Collider other) // Objects MUST have a rigidbody component and have "Entity" tag
    {
        if (other.CompareTag("Entity") && !objectsInCollider.Contains(other.gameObject)) objectsInCollider.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other) // Objects MUST have a rigidbody component and have "Entity" tag
    {
        if (other.CompareTag("Entity") && objectsInCollider.Contains(other.gameObject)) objectsInCollider.Remove(other.gameObject);
    }

    private void OnDestroy()
    {
        sensor.DeregisterFromManager(SensorManager.Instance);
        SensorDetached?.Invoke(sensor);
    }

    #endregion

    #region Private Methods
    private bool AreEntitiesInCollider() => objectsInCollider.Count > 0;

    private void AttachSensor(SensorType value)
    {
        switch (value)
        {
            case SensorType.Camera:
                AttachSensor(new CameraSensor());
                break;
            default:
                throw new Exception("Sensor type not yet implemented");
        }
    }

    private void AttachSensor(ISensor sensor)
    {
        if (!(sensor is null))
        {
            sensor.RegisterToManager(SensorManager.Instance);
            this.sensor = sensor;
        }
        else
        {
            throw new Exception("Sensor must not be null");
        }
    }

    private void ScanForDeadReferences()
    {
        HashSet<GameObject> deadRef = new HashSet<GameObject>();
        foreach(var obj in objectsInCollider)
        {
            if (obj == null) deadRef.Add(obj);
        }

        if (deadRef.Count > 0) objectsInCollider.ExceptWith(deadRef);
    }

    #endregion
}
