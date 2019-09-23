using System;
using MLAgents;
using UnityEngine;

public class RollerAgent : Agent
{
    public GameObject target;
    [SerializeField]
    private Rigidbody rb;

    public Transform arena;

    [SerializeField]
    private float minRelocation;
    
    [SerializeField]
    private float maxRelocation;
    
    public float speed;

    private Vector3 lastPos;

    private float t = 0;
    
    public float runtime;
    public float lastDist;
    private int n = 0;

    private float timer = 0;

    private void Start() {
        //transform.position = new Vector3(UnityEngine.Random.Range(arena.position.x + minRelocation, arena.position.x + maxRelocation), 1.5f, UnityEngine.Random.Range(arena.position.z + minRelocation, arena.position.z + maxRelocation));
        transform.position = new Vector3(arena.position.x, 1.5f, arena.position.z);
        lastPos = transform.position;
        lastDist = Vector3.Distance(transform.position, target.transform.position);
    } 

    private void FixedUpdate() {
        RaycastHit[] hits = Physics.RaycastAll(lastPos, transform.position - lastPos, Vector3.Distance(lastPos, transform.position));
        // Loop hits backwards, last index is first collider hit by raycast.
        for(int i = hits.Length - 1; i >= 0; i--)
        {
            // Ignore this collider as well as owner's collider
            if(hits[i].transform.gameObject == gameObject)
                continue;

            if(hits[i].transform.gameObject == target)
            {
                //Debug.Log("Hit Target");
                GiveReward();
            }

            return;
        }
        lastPos = transform.position;
    }

    private void GiveReward()
    {
        n++;
        // Add reward and move target
        AddReward(n);
        target.transform.position = new Vector3(UnityEngine.Random.Range(arena.position.x + minRelocation, arena.position.x + maxRelocation), 2.5f, UnityEngine.Random.Range(arena.position.z + minRelocation, arena.position.z + maxRelocation));
        //lastDist = Vector3.Distance(transform.position, target.transform.position);
        //Done();
        timer = 0;
    }

    public override void AgentReset()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        //transform.position = new Vector3(UnityEngine.Random.Range(arena.position.x + minRelocation, arena.position.x + maxRelocation), 1.5f, UnityEngine.Random.Range(arena.position.z + minRelocation, arena.position.z + maxRelocation));
        target.transform.position = new Vector3(UnityEngine.Random.Range(arena.position.x + minRelocation, arena.position.x + maxRelocation), 2.5f, UnityEngine.Random.Range(arena.position.z + minRelocation, arena.position.z + maxRelocation));
        transform.position = new Vector3(arena.position.x, 1.5f, arena.position.z);
        lastPos = transform.position;
        lastDist = Vector3.Distance(transform.position, target.transform.position);
        t = 0;
        n = 0;
        timer = 0;
    }

    

    public override void CollectObservations()
    {
        // Positions of agent and target
        //AddVectorObs(target.transform.position);
        AddVectorObs(transform.position);
        // Direction to the cube
        AddVectorObs(target.transform.position - transform.position);
        // Direction of travel
        //AddVectorObs(lastPos - transform.position);
        // AddVectorObs(lastDist);
        AddVectorObs(Vector3.Distance(transform.position, target.transform.position));
        // Agent Velocity
        AddVectorObs(rb.velocity.x);
        AddVectorObs(rb.velocity.z);
        // AddVectorObs(n);
        // AddVectorObs(timer);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        Vector3 controlForceSignal = Vector3.zero;
        controlForceSignal.x = vectorAction[0];
        controlForceSignal.z = vectorAction[1];
        rb.AddForce(controlForceSignal * speed);

        if(transform.position.y < 0)
        {
            Fell();
            return;
        }

        if(t >= runtime)
        {
            //Debug.Log("Finished cycle.");
            //GiveReward();
            Done();
        }
        else
        {
            float currentDist = Vector3.Distance(transform.position, target.transform.position);
            if(currentDist < lastDist)
                AddReward(.1f * rb.velocity.magnitude);
            else
                AddReward(-.1f * rb.velocity.magnitude);
            lastDist = currentDist;

            AddReward(-.00001f * timer);

            t += Time.deltaTime;
            timer += Time.deltaTime;
        }
    }

    public void Fell()
    {
        // Punish the agent
        AddReward(-1);
        // float currentReward = GetCumulativeReward();
        // if(currentReward > 0)
        //     SetReward(-currentReward);
            //AddReward(-currentReward);
        Done();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject == target)
        {
            //Debug.Log("Hit Target");
            GiveReward();
        }
    }
}