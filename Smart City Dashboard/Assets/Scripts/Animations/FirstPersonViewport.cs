using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonViewport : MonoBehaviour
{
    [SerializeField]
    private Renderer modelRenderer;
    [SerializeField]
    private Animator animator;

    private VehicleEntity mappingTarget;
    private PathWalker walker;
    [HideInInspector]
    public bool CurrentlyTracking { get; private set; } = false;
    private float currentAngle = 0f;
    private float turnVelocity = 0f;

    // Update is called once per frame
    void Update()
    {
        if(mappingTarget is VehicleEntity && walker is PathWalker) //It still exists
        {
            currentAngle = Mathf.SmoothDamp(currentAngle, walker.TurnDelta * 3f, ref turnVelocity, 0.3f);//(walker.TurnDelta + currentAngle) / 1.5f;
            animator.SetFloat("WheelAngle", currentAngle.ClampTo(-1, 1));
        }
        else
        {
            StopTracking();
        }
    }

    public void SetTrackTo(VehicleEntity entity)
    {
        if(entity is VehicleEntity)
        {
            mappingTarget = entity;
            walker = entity.GetComponent<PathWalker>();
            if(walker is PathWalker)
            {
                CurrentlyTracking = true;
                currentAngle = 0f;
            }
            SetChildMaterial(entity.ChildMaterial);
            
        }
    }

    private void SetChildMaterial(Material mat)
    {
        var matCopy = modelRenderer.materials;
        matCopy[1] = mat;
        modelRenderer.materials = matCopy;
    }

    private void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
    }

    public void StopTracking()
    {
        mappingTarget = null;
        walker = null;
        CurrentlyTracking = false;
    }
}
