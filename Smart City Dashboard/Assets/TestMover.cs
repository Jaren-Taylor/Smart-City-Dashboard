using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestMover : MonoBehaviour
{
    public List<NavPoint> points;
    public List<Vector3> pointPositions;


    private float t = 0;

    private void Start()
    {
        pointPositions = points.Select((x) => x.transform.position).ToList();
        MoveTo(pointPositions[0]);
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime * 0.5f;

        Vector3 target = Bezier.PointAlongCurve(pointPositions, t);
        MoveTo(target);
    }

    void MoveTo(Vector3 target)
    {
        transform.LookAt(target);
        transform.position = target;
    }
}
