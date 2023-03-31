using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class StageBoss : BossEnemy
{
    //public GameObject missile;
    //public Transform missilePortA;
    //public Transform missilePortB;
    //public bool isLook;

    //Vector3 lookVec;
    //Vector3 tauntVec;
    

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();  // Material¿∫ MesgRenderer∏¶ ≈Î«ÿ ∞°¡Æø»
        //nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        //nav.isStopped = true;

        //StartCoroutine(Think());
    }
    void Update()
    {
       
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.3f);

        int ranAction = Random.Range(0, 3);
        switch(ranAction)
        {
            case 0:
            case 1:
                // ≥ª∑¡¬Ô±‚
                StartCoroutine(Attack1());
                break;
            case 2:
            case 3:
                // ¬Ó∏£±‚
                StartCoroutine(Attack2());
                break;
        }
    }

    IEnumerator Attack1()
    {
        anim.SetTrigger("Attack1");
        yield return new WaitForSeconds(2f);

        StartCoroutine(Think());
    }

    IEnumerator Attack2()
    {
        anim.SetTrigger("Attack2");
        yield return new WaitForSeconds(5f);

        StartCoroutine(Think());
    }

    

}
