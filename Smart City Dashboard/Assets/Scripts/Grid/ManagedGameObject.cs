using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagedGameObject : MonoBehaviour
{
    public GameObject childModel = null;

    private Material cachedMaterial = null;
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
    /// Sets the model's material to whatever is passed. If no model is currenly held, will cache for later
    /// </summary>
    /// <param name="material"></param>
    public void SetModelMaterial(Material material)
    {
        if (ModelExists)
        {
            if(cachedMaterial == null || cachedMaterial != material)
            {
                cachedMaterial = material;
                ApplyCachedMaterial();
            }
        }
        else cachedMaterial = material;
    }

    private void ApplyCachedMaterial()
    {
        MeshRenderer renderer = childModel.transform.GetComponentInChildren<MeshRenderer>();
        Debug.Log(renderer);
        Debug.Log(cachedMaterial);
        if (renderer != null && cachedMaterial != null) renderer.material = cachedMaterial;
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
        ApplyCachedMaterial();
        
    }


    /// <summary>
    /// Deletes current model and replaced with passed prefab.
    /// </summary>
    /// <param name="modelPrefab">Prefab to be instanced (will be copied if already instantiated)</param>
    /// <param name="rotation">Rotation for instanced model</param>
    public void SwapModel(GameObject modelPrefab, Quaternion? rotation = null) => InstantiateModel(modelPrefab, rotation);
}
