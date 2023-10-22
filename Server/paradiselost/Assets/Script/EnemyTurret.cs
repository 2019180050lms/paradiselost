using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret  : MonoBehaviour
{
    

    public static EnemyTurret Instance { get; } = new EnemyTurret();

    public Vector3 targetPos;
    private void Start()
    {
    }


    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(targetPos);
        if (other.tag == "Player" || other.tag == "MyPlayer")
        {
            targetPos = other.transform.position;
        }
    }
}
