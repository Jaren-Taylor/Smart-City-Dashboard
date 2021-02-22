using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object that has fields that allows it's data to be easily managed. Used by Tiles to create a physical model in the scene.
/// </summary>
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

    /// <summary>
    /// Apply whatever material is current in cache to child model
    /// </summary>
    private void ApplyCachedMaterial()
    {
        MeshRenderer renderer = childModel.transform.GetComponentInChildren<MeshRenderer>();
        if (renderer != null && cachedMaterial != null) renderer.material = cachedMaterial;
    }


    /// <summary>
    /// Add component to the object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T AddComponent<T>() where T : Component => gameObject.AddComponent<T>();

    /// <summary>
    /// Trys to remove component from object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool TryRemoveComponent<T>() where T : Component
    {
        Component comp = GetComponent<T>();
        if (comp == null) return false;
        else
        {
            Destroy(comp);
            return true;
        }
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

    internal void AddComponentToManaged<T>() where T : Component
    {
        if (ModelExists) childModel.AddComponent<T>();
    }


    /// <summary>
    /// Deletes current model and replaced with passed prefab.
    /// </summary>
    /// <param name="modelPrefab">Prefab to be instanced (will be copied if already instantiated)</param>
    /// <param name="rotation">Rotation for instanced model</param>
    public void SwapModel(GameObject modelPrefab, Quaternion? rotation = null) => InstantiateModel(modelPrefab, rotation);
}
