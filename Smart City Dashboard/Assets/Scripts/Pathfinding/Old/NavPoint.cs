using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavPoint : MonoBehaviour
{
    public List<NavPointConnection> Connections = null;

    private float DEBUG_radius = .1f;

    private void OnDrawGizmos()
    {
        Color stashedColor = Gizmos.color;
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, DEBUG_radius);

        Gizmos.color = stashedColor;
    }

    private void OnDrawGizmosSelected()
    {
        Color stashedColor = Gizmos.color;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, DEBUG_radius);

        Gizmos.color = stashedColor;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
