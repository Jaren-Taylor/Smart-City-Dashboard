using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sensor<T> : MonoBehaviour
{
    private float totalTime = 0f;
    private float callDelay;
    protected Rigidbody ownerRigidBody = null;
    [SerializeField]
    public int TargetCallsPerSecond
    {
        get => (int)(1 / callDelay);
        set => callDelay = (value > 0) ? (1f / value) : 1f;
    }

    public Collider ViewShape { get; private set; }
    public Action<List<T>> DataCollected;

    private HashSet<GameObject> objectsInCollider;

    // Start is called before the first frame update
    void Start()
    {
        RegisterToManager(SensorManager.Instance);
        TryGetComponent(out ownerRigidBody);

        if(TryGetComponent(out Collider viewShape))
        {
            ViewShape = viewShape;
        }
        else
        {
            gameObject.AddComponent<SphereCollider>();
            SphereCollider sc = gameObject.GetComponent<SphereCollider>();
            sc.radius = 1f;
            ViewShape = sc;
        }

        ViewShape.isTrigger = true;
        objectsInCollider = new HashSet<GameObject>();
        TargetCallsPerSecond = 4; 
    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;
        if(totalTime > callDelay)
        {
            CollectAndSendData();
            totalTime -= callDelay; // Consider setting to zero if we find that this contributes to lag
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
        DeregisterFromManager(SensorManager.Instance);
    }

    private void CollectAndSendData()
    {
        if(AreEntitiesInCollider())
            DataCollected?.Invoke(DetectEnvironment());
    }

    private bool AreEntitiesInCollider() => objectsInCollider.Count > 0;

    private List<T> DetectEnvironment()
    {
        List<T> collectedData = new List<T>();
        HashSet<GameObject> deadReferences = new HashSet<GameObject>();
        foreach (GameObject sensedObject in objectsInCollider)
        {
            if (sensedObject == null) deadReferences.Add(sensedObject); // Safeguard to ensure that the sensed object was not deleted before it could exit the trigger
            else collectedData.Add(CollectData(sensedObject));
        }
        foreach (GameObject deadRef in deadReferences) objectsInCollider.Remove(deadRef);
        return collectedData;
    }

    protected abstract T CollectData(GameObject sensedObject);
    public abstract void RegisterToManager(SensorManager sensor);
    public abstract void DeregisterFromManager(SensorManager sensor);

    public override string ToString()
    {
        return "Generic Sensor";
    }
}
