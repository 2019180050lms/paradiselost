using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet
{
    public int enemyId;

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
        target = GameObject.Find("test(Clone)").GetComponent<Player>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (target.PlayerId == 1)
        //{
        //    targetPos = target.transform.position;
        //}

        targetPos = target.transform.position;
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.005f);
        transform.LookAt(targetPos);
    }
}
