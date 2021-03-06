using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    private float totalTime = 0f;
    private float switchDelay = 5f;
    private Dictionary<NodeCollectionController.Direction, LightAnimationController> TrafficLights = new Dictionary<NodeCollectionController.Direction, LightAnimationController>();
    private bool isEastWest = false;
    private bool isTransitioning = false;
    private bool isInitialized = false;


    /// <summary>
    /// From a given set of directions, instantiates the appropriate traffic light and returns the attached script.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static TrafficLightController CreateLight( List<NodeCollectionController.Direction> directions, Transform parent = null)
    {

        var spanWireModel = Resources.Load<GameObject>("Prefabs/Traffic_Lights/Span_Wire");
        var lightModel = Resources.Load<GameObject>("Prefabs/Traffic_Lights/Traffic_Light");

        var spanWire = InstantiateWithOptionalParent(parent, spanWireModel);

        foreach (var direction in directions)
        {
            AttachToSpanWire(spanWire, direction, lightModel);
        }
        return spanWire;

    }

    private static TrafficLightController InstantiateWithOptionalParent(Transform parent, GameObject spanWireModel)
    {
        return (parent is null) ? Instantiate(spanWireModel).GetComponent<TrafficLightController>() : Instantiate(spanWireModel, parent).GetComponent<TrafficLightController>();
    }

    private static void AttachToSpanWire(TrafficLightController spanWire, NodeCollectionController.Direction direction, GameObject lightPrefab)
    {
        if (spanWire.TrafficLights.ContainsKey(direction))
            throw new Exception(" Traffic light dictionary already contains the passed in direction");
        var rotation = direction switch 
        {
            NodeCollectionController.Direction.NorthBound => Quaternion.Euler(0,-90,0),
            NodeCollectionController.Direction.WestBound => Quaternion.Euler(0,-180,0),
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
        if(! isInitialized)
            InitializeLights();
        
        totalTime += Time.deltaTime;
        if (totalTime > switchDelay)
        {
            if (!isTransitioning)
            {
                isTransitioning = true;
                if (isEastWest)
                {                    
                    TryTurnRed(NodeCollectionController.Direction.EastBound);
                    TryTurnRed(NodeCollectionController.Direction.WestBound);
                }
                else
                {
                    TryTurnRed(NodeCollectionController.Direction.NorthBound);
                    TryTurnRed(NodeCollectionController.Direction.SouthBound);

                }
            }
            else
            {
                if (isEastWest)
                {
                    if (TryIsRed(NodeCollectionController.Direction.EastBound) || TryIsRed(NodeCollectionController.Direction.WestBound) )
                    {
                        isEastWest = false;
                        TryTurnGreen(NodeCollectionController.Direction.NorthBound);
                        TryTurnGreen(NodeCollectionController.Direction.SouthBound);
                        isTransitioning = false;
                        totalTime = 0;
                    }

                }
                else
                {
                    if (TryIsRed(NodeCollectionController.Direction.NorthBound) || TryIsRed(NodeCollectionController.Direction.SouthBound))
                    {
                        isEastWest = true;
                        TryTurnGreen(NodeCollectionController.Direction.EastBound);
                        TryTurnGreen(NodeCollectionController.Direction.WestBound);
                        isTransitioning = false;
                        totalTime = 0;
                    }                    
                }

            }
        }
    }

    private void InitializeLights()
    {
        isInitialized = true;
        if (isEastWest)
        {
            TryTurnGreen(NodeCollectionController.Direction.EastBound);
            TryTurnGreen(NodeCollectionController.Direction.WestBound);
        }
        else
        {
            TryTurnGreen(NodeCollectionController.Direction.NorthBound);
            TryTurnGreen(NodeCollectionController.Direction.SouthBound);
        }
    }

    private bool TryIsRed(NodeCollectionController.Direction direction)
    {
        if (TrafficLights.TryGetValue(direction, out var value))
        {
            return value.State == LightAnimationController.LightColor.Red;
        }
        return false;
    }

    public bool HasLight(NodeCollectionController.Direction direction, out LightAnimationController.LightColor state)
    {
        if (TrafficLights.TryGetValue(direction, out var light))
        {
            state = light.State;
            return true;
        }
        state = LightAnimationController.LightColor.Green;
        return false;
    }
    private bool TryIsGreen(NodeCollectionController.Direction direction)
    {
        if (TrafficLights.TryGetValue(direction, out var value))
        {
            return value.State == LightAnimationController.LightColor.Green;
        }
        return false;
    }

    private void TryTurnGreen(NodeCollectionController.Direction direction)
    {
        if (TrafficLights.TryGetValue(direction, out var value))
        {
            value.TurnGreen();
        }
    }

    private void TryTurnRed(NodeCollectionController.Direction direction)
    {
        if (TrafficLights.TryGetValue(direction, out var value))
        {
            value.TurnRed();
        }
    }
}
