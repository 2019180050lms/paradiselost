using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    GameObject findEnemy;

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
        anim.SetBool("Open", findEnemy == null);
        anim.SetBool("Close", findEnemy != null);

        if (findEnemy == null)
            boxCollider.enabled = false;
        else
            boxCollider.enabled = true;
       
    }
}
