using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet
{
    public Player target;
    NavMeshAgent nav;

    public Vector3 targetPos;

    void Start()
    {
        
    }
    void Awake()
    {
        //nav = GetComponent<NavMeshAgent>();
        //nav.enabled = false;
        target = GameObject.Find("Player_t1(Clone)").GetComponent<Player>();
        if(target.PlayerId == 1)
        {
            targetPos = target.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {

        //nav.SetDestination(target.position);
        //nav.enabled = true;
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.005f);
        transform.LookAt(targetPos);
    }
}
