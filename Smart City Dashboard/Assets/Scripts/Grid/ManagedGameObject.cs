using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagedGameObject : MonoBehaviour
{
    public GameObject childModel = null;

    public bool ModelExists { get => childModel != null; }

    /// <summary>
    /// Destroys self as well as ALL attached children
    /// </summary>
    public void DestroyTree() 
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        Destroy(gameObject);
    }


    /// <summary>
    /// Destroys instance of Model if it exists
    /// </summary>
    public void DestroyModel()
    {
        if (childModel != null) Destroy(childModel);
    }

    /// <summary>
    /// Instantiates prefab as child of this object. Saves this object as it's Model. Will DESTROY current model if it exists.
    /// </summary>
    /// <param name="modelPrefab">Prefab to be instanced (will be copied if already instantiated)</param>
    /// <param name="rotation">Rotation for instanced model</param>
    public void InstantiateModel(GameObject modelPrefab, Quaternion? rotation = null)
    {
        DestroyModel();
        childModel = Instantiate(modelPrefab, transform, false);
        childModel.transform.rotation = rotation ?? Quaternion.identity;
        childModel.transform.localPosition = Vector3.zero;
        childModel.name = childModel.name.Replace("(Clone)", "");
    }


    /// <summary>
    /// Deletes current model and replaced with passed prefab.
    /// </summary>
    /// <param name="modelPrefab">Prefab to be instanced (will be copied if already instantiated)</param>
    /// <param name="rotation">Rotation for instanced model</param>
    public void SwapModel(GameObject modelPrefab, Quaternion? rotation = null) => InstantiateModel(modelPrefab, rotation);
}
