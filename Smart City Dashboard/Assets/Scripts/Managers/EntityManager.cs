
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    Vector2Int tileDestination;
    Vector2Int tilePosition;

    float destinationTolerance = .5f;
    
    //TileNode nodeDestination;

    void Start()
    {
        //functions here have placeholder names until the path finder is built
        //tileDestination = PathFindingManager.getDestination();
        //nodeDestination = PathFindingManager.CalculatePath(tilePosition, tileDestination);

    }

    //All of this will be broken until the path finding system is built.
    //Abstraction required to make modular and fit both cars and pedestrians

    /*
    void Update()
    {
        if(Vector3.Distance(transform.position, nodeDestination) > destinationTolerance)
        {
            MoveToNextNode();
        }
        else if(nodeDestination.next != null)
        {
            SetNextDestinationNode();
            transform.LookAt(nodeDestination.transform.position);
        }
        else if(nodeDestination == null)
        {
            Destroy(this);
        }
    }

    void MoveToNextNode()
    {
        transform.position = Vector3.MoveTowards(transform.position, nodeDestination.transform.position, .01f);
    }
    void SetNextDestinationNode()
    {
        nodeDestination = nodeDestination.Next();
    }
    */
}
    
