using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    GameObject findEnemy;
    GameObject findEnemy2;

    Animator anim;
    BoxCollider boxCollider;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        findEnemy = GameObject.FindWithTag("Enemy");
        findEnemy2 = GameObject.FindWithTag("EnemyTurret");

        if (findEnemy == null && findEnemy2 == null)
        {
            Destroy(gameObject);
        }
        else
            boxCollider.enabled = true;
       
    }
}
