using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingTileInterface : MonoBehaviour
{
    private string ManagedNodeCollection = "Prefabs/NodeCollection";
    public NodeCollectionController NodeCollection;

    void Start()
    {
        var prefab = Resources.Load<GameObject>(ManagedNodeCollection);
        NodeCollection = Instantiate(prefab, transform).GetComponent<NodeCollectionController>();
    }
}
