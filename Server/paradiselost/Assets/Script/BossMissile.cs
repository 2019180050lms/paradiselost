using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet
{
    public Transform target;
    NavMeshAgent nav;

    void Start()
    {
        
    }
    void Awake()
    {
        //nav = GetComponent<NavMeshAgent>();
        //nav.enabled = false;
        target = GameObject.Find("Player_t1(Clone)").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {

        //nav.SetDestination(target.position);
        //nav.enabled = true;
        transform.position = Vector3.Lerp(transform.position, target.position, 0.005f);
        transform.LookAt(target.position);
    }
}
