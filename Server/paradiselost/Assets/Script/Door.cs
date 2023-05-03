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
        Debug.Log(findEnemy);
        Debug.Log(findEnemy2);
        anim.SetBool("Open", findEnemy == null && findEnemy2 == null);
        anim.SetBool("Close", findEnemy != null && findEnemy2 != null);

        if (findEnemy == null && findEnemy2 == null)
            boxCollider.enabled = false;
        else
            boxCollider.enabled = true;
       
    }
}
