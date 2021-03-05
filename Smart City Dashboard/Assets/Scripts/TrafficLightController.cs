using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    private Dictionary<NodeCollectionController.Direction, LightAnimationController> TrafficLights = new Dictionary<NodeCollectionController.Direction, LightAnimationController>();
    
    /// <summary>
    /// From a given set of directions, instantiates the appropriate traffic light and returns the attached script.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static TrafficLightController CreateLight(Transform parent, List<NodeCollectionController.Direction> directions)
    {
       
        var spanWireModel = Resources.Load<GameObject>("Prefabs/Traffic_Lights/Span_Wire");
        var lightModel = Resources.Load<GameObject>("Prefabs/Traffic_Lights/Traffic_Light");

        var spanWire = Instantiate(spanWireModel, parent).GetComponent<TrafficLightController>();

        foreach(var direction in directions)
        {
            AttachToSpanWire(spanWire, direction, lightModel);

        }
        return spanWire;
        
    }

    private static void AttachToSpanWire(TrafficLightController spanWire, NodeCollectionController.Direction direction, GameObject lightPrefab)
    {
        if (spanWire.TrafficLights.ContainsKey(direction))
            throw new Exception(" Traffic light dictionary already contains the passed in direction");
        var rotation = direction switch 
        {
            NodeCollectionController.Direction.NorthBound => Quaternion.Euler(0,-90,0),
            NodeCollectionController.Direction.EastBound => Quaternion.Euler(0,-180,0),
            NodeCollectionController.Direction.SouthBound => Quaternion.Euler(0, 90, 0),
            _ => Quaternion.identity
        };
        var lightController = Instantiate(lightPrefab, Vector3.zero, rotation, spanWire.transform).GetComponent<LightAnimationController>();
        lightController.transform.localPosition = Vector3.zero;
        spanWire.TrafficLights.Add(direction, lightController);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
