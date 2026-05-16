using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class perroScript : MonoBehaviour
{
    public Pathfinding.Patrol patrol;
    public Pathfinding.AIDestinationSetter chase;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        patrol = gameObject.GetComponent<Pathfinding.Patrol>();
        chase = gameObject.GetComponent<Pathfinding.AIDestinationSetter>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(gameObject.GetComponent<Pathfinding.FollowerEntity>().remainingDistance);
        if(gameObject.GetComponent<Pathfinding.FollowerEntity>().reachedEndOfPath == true)
        {
            //Debug.Log("caca");
            patrol.enabled = true;
            chase.enabled = false;
        }
    }
}
