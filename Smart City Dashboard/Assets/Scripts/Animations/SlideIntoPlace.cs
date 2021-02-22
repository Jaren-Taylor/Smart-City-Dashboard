using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideIntoPlace : MonoBehaviour
{
    private Vector3 target;
    private readonly float slideInSpeed = 2f;

    void Start()
    {
        target = transform.position;

        if(TryGetMesh(out Renderer renderer))
        {
            float height = renderer.bounds.size.y;
            if(height > 0)
            {
                transform.position -= new Vector3(0, height, 0);
            }
        }
        //If it can't find the renderer, it will destroy itself on first update call

    }

    void Update()
    {
        // Once it reaches position it will remove the script.
        // Until then it slides on to it's target...
        if (!HasReachedPosition()) 
        {
            MoveTowardTarget();
        }
        else RemoveScript();
    }

    /// <summary>
    /// Moves game object in direction of target moving by at most the slide in speed specified.
    /// </summary>
    private void MoveTowardTarget() => transform.position = Vector3.MoveTowards(transform.position, target, slideInSpeed * Time.deltaTime);
    
    /// <summary>
    /// Detects when the transform is essentially at the target position (accounts for float rounding).
    /// </summary>
    private bool HasReachedPosition() => Vector3.Distance(transform.position, target) < .0005f;
    
    /// <summary>
    /// Sets the transform the the target and then removes script.
    /// </summary>
    private void RemoveScript()
    {
        transform.position = target;
        Destroy(this);
    }

    /// <summary>
    /// Attempts to get a renderer from the object to detect height of the model. 
    /// </summary>
    /// <param name="renderer"></param>
    /// <returns></returns>
    private bool TryGetMesh(out Renderer renderer)
    {
        //First trys to get it from the child of the attached object
        var meshRenderer = GetComponentInChildren<MeshRenderer>(); 

        //If it can't, it then falls back to attempting to get a model from the attached object
        if (meshRenderer is null) meshRenderer = GetComponent<MeshRenderer>();

        renderer = meshRenderer;

        return !(renderer is null); //True if something was found
    }
}
