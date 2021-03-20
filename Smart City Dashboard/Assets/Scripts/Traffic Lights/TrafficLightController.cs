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
    private SortedDictionary<NodeCollectionController.Direction, float> TrafficLightDownTime = new SortedDictionary<NodeCollectionController.Direction, float>(); 
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
                        isTransitioning = false;
                        totalTime = 0;
                        TurnedGreen?.Invoke();
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
                        TurnedGreen?.Invoke();
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
        var sortedDirections = (from kvp in TrafficLightDownTime orderby kvp.Value ascending select kvp.Key).ToList();



        if ((TrafficLightDownTime[sortedDirections[0]] > 0.1f) ||
            (isEastWest && (sortedDirections[0] == NodeCollectionController.Direction.EastBound || sortedDirections[0] == NodeCollectionController.Direction.WestBound)) ||
            (!isEastWest && (sortedDirections[0] == NodeCollectionController.Direction.NorthBound || sortedDirections[0] == NodeCollectionController.Direction.SouthBound))) return;

        switch (sortedDirections[0])
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

    private void IncrementTrafficTimers(float timeDelta)
    {
        foreach (var key in TrafficLightDownTime.Keys.ToList()) TrafficLightDownTime[key] += timeDelta;
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
