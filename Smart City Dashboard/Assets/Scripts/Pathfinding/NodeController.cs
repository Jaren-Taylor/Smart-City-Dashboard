using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    private Queue<Entity> capacity = new Queue<Entity>();
    private List<NodeController> neighbors = new List<NodeController>();
    private Tile target { get;  set; }
    private Tile origin { get; set; }
    private Entity currOccupant { get; set; }

    private void ChangeEntity(Entity entity)
    {
        this.currOccupant = entity;
    }
}
