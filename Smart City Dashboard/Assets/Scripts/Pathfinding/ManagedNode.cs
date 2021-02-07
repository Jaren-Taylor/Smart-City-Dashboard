using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagedNode : MonoBehaviour
{
    private string ManagedNodeCollection = "Prefabs/NodeCollection";
    private NodeCollectionController NodeCollection;
    // Start is called before the first frame update
    void Start()
    {
        var prefab = Resources.Load<GameObject>(ManagedNodeCollection);
        NodeCollection = Instantiate(prefab, transform).GetComponent<NodeCollectionController>();

    }

   
}
