using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class perroScript : MonoBehaviour
{
    [HideInInspector] public Pathfinding.Patrol patrol;
    [HideInInspector] public Pathfinding.AIDestinationSetter chase;
    [HideInInspector] public Pathfinding.FollowerEntity entity;
    [HideInInspector] public RigidbodyFollower rbFollow;

    public float chaseSpeed;
    public float patrolSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        patrol = gameObject.GetComponent<Pathfinding.Patrol>();
        chase = gameObject.GetComponent<Pathfinding.AIDestinationSetter>();
        entity = gameObject.GetComponent<Pathfinding.FollowerEntity>();
        rbFollow = gameObject.GetComponent <RigidbodyFollower>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(gameObject.GetComponent<Pathfinding.FollowerEntity>().remainingDistance);
        if(entity.reachedEndOfPath == true)
        {
            //Debug.Log("caca");
            patrol.enabled = true;
            chase.enabled = false;
            this.rbFollow.moveSpeed = patrolSpeed;
        }
    }
}
