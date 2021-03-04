using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTrafficLights : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<NodeCollectionController.Direction> list = new List<NodeCollectionController.Direction>()
        {
            NodeCollectionController.Direction.EastBound,
            NodeCollectionController.Direction.NorthBound,
            NodeCollectionController.Direction.WestBound
        };
        TrafficLightController.CreateLight(this.transform, list);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
