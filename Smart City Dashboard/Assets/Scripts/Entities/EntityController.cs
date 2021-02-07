using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    public bool isMoving;
    public Vector3 nodeDestination;
    public List<StepNode> mapping = new List<StepNode>();
    public Dictionary<string, Dictionary<string,Vector3>> StepNodeLocations = new Dictionary<string, Dictionary<string,Vector3>>();

   

    private void Awake()
    {
        nodeDestination = new Vector3(transform.position.x, transform.position.y, this.transform.position.z - .4f);
    }
    private void Update()
    {
        if (Vector3.Distance(transform.position, nodeDestination) > .00001)
        {
            Movement();
        }
    }

    private void initDict()
    {
        StepNodeLocations.Add("TWO_WAY", new Dictionary<string, Vector3>());
        StepNodeLocations.Add("THREE_WAY", new Dictionary<string, Vector3>());
        StepNodeLocations.Add("FOUR_WAY", new Dictionary<string, Vector3>());
        StepNodeLocations.Add("CAR_LEFT", new Dictionary<string, Vector3>());

        StepNodeLocations["TWO_WAY"].Add("CAR_RIGHT", new Vector3(.18f, .012f, .35f));
        StepNodeLocations["TWO_WAY"].Add("CAR_LEFT", new Vector3(-.18f, .012f, -.35f));

        StepNodeLocations["THREE_WAY"].Add("CAR_TOP_RIGHT", new Vector3(.2f, .012f, .185f));
        StepNodeLocations["THREE_WAY"].Add("CAR_TOP_LEFT", new Vector3(-.2f, .012f, .185f));
        StepNodeLocations["THREE_WAY"].Add("CAR_BOTTOM_LEFT", new Vector3(-.185f, .012f, -.2f));
        StepNodeLocations["THREE_WAY"].Add("CAR_BOTTOM_RIGHT", new Vector3(.185f, .012f, -.2f));

        StepNodeLocations["FOUR_WAY"].Add("CAR_TOP_RIGHT", new Vector3(.2f, .012f, .2f));
        StepNodeLocations["FOUR_WAY"].Add("CAR_TOP_LEFT", new Vector3(-.2f, .012f, .2f));
        StepNodeLocations["FOUR_WAY"].Add("CAR_BOTTOM_LEFT", new Vector3(-.2f, .012f, -.2f));
        StepNodeLocations["FOUR_WAY"].Add("CAR_BOTTOM_RIGHT", new Vector3(.2f, .012f, -.2f));

        StepNodeLocations["CORNER"].Add("CAR_BOTTOM_RIGHT", new Vector3(.2f, .012f, -.2f));
        StepNodeLocations["CORNER"].Add("CAR_TOP_LEFT", new Vector3(-.2f, .012f, .2f));

        //StepNodeLocations["FOUR_WAY"][CAR_TOP_LEFT] will return a vector 3.
    }

    private void Movement()
    {
        if (mapping.Count != 0 || mapping != null)
        {
            foreach (StepNode node in mapping)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, node.pos, .0001f);
            }
        };
    }
}

