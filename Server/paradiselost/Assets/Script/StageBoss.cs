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
    public HitBox hitBox;
    //Vector3 lookVec;
    //Vector3 tauntVec;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();  // Material은 MesgRenderer를 통해 가져옴
        //nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        hitBox = GetComponent<HitBox>();
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
                // 내려찍기
                StartCoroutine(Attack1());
                break;
            case 2:
            case 3:
                // 찌르기
                StartCoroutine(Attack2());
                break;
        }
    }

    IEnumerator Attack1()
    {
        anim.SetTrigger("Attack1");
        //yield return new WaitForSeconds(2f);

        yield return new WaitForSeconds(0.2f);
        hitBox.meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        hitBox.meleeArea.enabled = false;

        yield return new WaitForSeconds(3f); // 몬스터 공격속도

        StartCoroutine(Think());
    }

    IEnumerator Attack2()
    {
        anim.SetTrigger("Attack2");
        yield return new WaitForSeconds(5f);

        StartCoroutine(Think());
    }

    

}
