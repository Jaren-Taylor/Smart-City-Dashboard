using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public float totalTime { private set; get; } = 0f;
    public float switchDelay { private set; get; } = 10f;
    private Dictionary<NodeCollectionController.Direction, LightAnimationController> TrafficLights = new Dictionary<NodeCollectionController.Direction, LightAnimationController>();
    private Dictionary<NodeCollectionController.Direction, float> TrafficLightDownTime = new Dictionary<NodeCollectionController.Direction, float>();
    private List<NodeCollectionController.Direction> TrafficLightDirections = new List<NodeCollectionController.Direction>();

    public bool isEastWest { private set; get; } = false;
    private bool isTransitioning = false;
    private bool isInitialized = false;
    public Action TurnedGreen;
    public Action TurnedRed;


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
        spanWire.TrafficLightDownTime.Add(direction, 0f);
        spanWire.TrafficLightDirections.Add(direction);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized)
            InitializeLights();

        float timeDelta = Time.deltaTime;
        totalTime += timeDelta;
        CheckForSmartTrafficOverride();
        IncrementTrafficTimers(timeDelta);
        if (totalTime >= switchDelay)
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
                    if (TryIsRed(NodeCollectionController.Direction.EastBound) || TryIsRed(NodeCollectionController.Direction.WestBound))
                    {
                        isEastWest = false;
                        TryTurnGreen(NodeCollectionController.Direction.NorthBound);
                        TryTurnGreen(NodeCollectionController.Direction.SouthBound);
                        TurnedGreen?.Invoke();
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
                        TurnedGreen?.Invoke();
                        isTransitioning = false;
                        totalTime = 0;
                        
                    }
                }

            }
        }
    }

    private void CheckForSmartTrafficOverride()
    {
        if (isTransitioning ||
            totalTime < 4f) return;

        //Sort the kvp by the smallest value and return the sorted keys in order
        var sortedDirection = GetShortestDirection();
            //(from kvp in TrafficLightDownTime orderby kvp.Value ascending select kvp.Key).ToList();



        if ((TrafficLightDownTime[sortedDirection] > 0.1f) ||
            (isEastWest && (sortedDirection == NodeCollectionController.Direction.EastBound || sortedDirection == NodeCollectionController.Direction.WestBound)) ||
            (!isEastWest && (sortedDirection == NodeCollectionController.Direction.NorthBound || sortedDirection == NodeCollectionController.Direction.SouthBound))) return;

        switch (sortedDirection)
        {
            case NodeCollectionController.Direction.EastBound: case NodeCollectionController.Direction.WestBound:
                if ((TrafficLightDownTime.TryGetValue(NodeCollectionController.Direction.NorthBound, out float northTime) && northTime < 0.1f) ||
                    (TrafficLightDownTime.TryGetValue(NodeCollectionController.Direction.SouthBound, out float southTime) && southTime < 0.1f)) return;
                
                //Debug.Log($"Switch early for: {sortedDirections[0]} | Early by {switchDelay - totalTime} seconds.");
                totalTime = switchDelay; //Switch light directions
                break;
            default:
                if ((TrafficLightDownTime.TryGetValue(NodeCollectionController.Direction.WestBound, out float westBound) && westBound < 0.1f) ||
                    (TrafficLightDownTime.TryGetValue(NodeCollectionController.Direction.EastBound, out float eastBound) && eastBound < 0.1f)) return;

                //Debug.Log($"Switch early for: {sortedDirections[0]} | Early by {switchDelay - totalTime} seconds.");
                totalTime = switchDelay; //Switch light directions
                break;
        } 
    }

    private NodeCollectionController.Direction GetShortestDirection()
    {
        NodeCollectionController.Direction shortestDir = TrafficLightDirections[0];
        float shortestTime = TrafficLightDownTime[shortestDir];

        foreach(var kvp in TrafficLightDownTime)
        {
            if(kvp.Value < shortestTime)
            {
                shortestTime = kvp.Value;
                shortestDir = kvp.Key;
            }
        }

        return shortestDir;
    }

    private void IncrementTrafficTimers(float timeDelta)
    {
        foreach (var direction in TrafficLightDirections) TrafficLightDownTime[direction] += timeDelta;
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
        TurnedGreen?.Invoke();
    }

    public void VehicleFoundInDirection(NodeCollectionController.Direction direction)
    {
        if (TrafficLightDownTime.ContainsKey(direction))
        {
            TrafficLightDownTime[direction] = 0f;
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
            TurnedRed?.Invoke();
        }
    }
    public void ChangeTrafficDirection()
    {
        totalTime += switchDelay;
    }
}
