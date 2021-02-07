using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    bool isMoving;
    Vector3 destination;

    private void Awake()
    {
        destination = new Vector3(transform.position.x, transform.position.y, this.transform.position.z - .4f);
    }
    private void Update()
    {
        if (Vector3.Distance(transform.position, destination) > .00001)
        {
            Movement();
        }
    }

    private void Movement()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, destination, .0001f);
    }
}
