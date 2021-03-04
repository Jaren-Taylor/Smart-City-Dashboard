using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAnimationController : MonoBehaviour
{
    [SerializeField]
    private LightColor State;
    [SerializeField]
    private Material[] LightAddresses;
    [SerializeField]
    private MeshRenderer Mesh;
    public enum LightColor
    {
        Red,
        Yellow,
        Green,
    }
    private IEnumerator WaitAndChange(float delta)
    {
        var state = 0;
        while (true) 
        {
            ChangeLightState((LightColor)state);
            state++;
            state %= 3;
            yield return new WaitForSeconds(delta);
        }
        
        
    }

    private void Start()
    {
        ChangeLightState(LightColor.Red);
    }

    private void ChangeLightState(LightColor nextState)
    {
        this.State = nextState;
        var mats = Mesh.materials;
        switch (this.State)
        {
            case LightColor.Red:
                mats[1] = LightAddresses[0];
                mats[2] = LightAddresses[5];
                mats[3] = LightAddresses[4];
                break;

            case LightColor.Green:
                mats[1] = LightAddresses[3];
                mats[2] = LightAddresses[2];
                mats[3] = LightAddresses[4];
                break;

            case LightColor.Yellow:
                mats[1] = LightAddresses[3];
                mats[2] = LightAddresses[5];
                mats[3] = LightAddresses[1];
                break;
        }
        Mesh.materials = mats;
   
    }

   
}
