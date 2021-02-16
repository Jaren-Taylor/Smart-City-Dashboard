using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    Vector2Int tileDestination;
    Vector2Int tilePosition;
    float maxSpeed = .5f;

    float destinationTolerance = .5f;

    GameObject Node;
    GameObject nodeDestination;
    List<Vector3> NodeLevelPath = new List<Vector3>();
    List<Vector2Int> TileLevelPath = new List<Vector2Int>();


   // void Start()
  //  {
        // we will need more of the entity management system built out to facilitate assigning spawn and despawn locations to entities before we can programmatically set it here.
        //tileDestination = PathFindingManager.getDestination();
        //nodeDestination = PathFindingManager.CalculatePath(tilePosition, tileDestination);

    //}

    //All of this will be broken until the path finding system is built.
    //Abstraction required to make modular and fit both cars and pedestrians

    /// <summary>
    /// this will be encapsulated in a TileLevelPath check in the final build.
    /// </summary>
    void Update()
    {
        if (NodeLevelPath.Any())
        {
            var i = 0;
            var nextNode = NodeLevelPath[i];
            // Destination tolerance does what exactly? - Jaren
            if (Vector3.Distance(transform.position, nextNode) > destinationTolerance)
            {
                transform.LookAt(nextNode);
                MoveToNextNode(nextNode);
            }
        }
    }

    public void MoveToNextNode(Vector3 node)
    {
        transform.position = Vector3.MoveTowards(this.transform.position, node, maxSpeed);
    }

    public void InitiateTraversal(List<Vector3> directions)
    {
        NodeLevelPath = directions;
    }
   

}

