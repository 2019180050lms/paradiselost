using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int enemyId;
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public bool isChase;
    public bool isAttack;


    public Vector3 moveVec2;

    public BoxCollider meleeArea;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;

    NavMeshAgent nav;

    public Animator anim;

    NetworkManager _network;

    public bool walking;

    private void Start()
    {
        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        walking = true;
    }

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;  // Material�� MesgRenderer�� ���� ������
        //nav = GetComponent<NavMeshAgent>();
        
        

        //Invoke("Walk", 2);
        
       // anim.SetBool("isWalk", true);
    }

    void Walk()
    {
        //isChase = true;
        //anim.SetBool("isWalk", true);
    }
     void Update()
    {
        /*
        if (nav.enabled)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase; 
        }
        */
        //Debug.Log(walking);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            //Debug.Log(" �浹 " );
           Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
           // Debug.Log("Melee : " + curHealth);
            StartCoroutine(OnDamage(reactVec));
            C_AttackedMonster attackedPacket = new C_AttackedMonster();
            attackedPacket.id = enemyId;
            attackedPacket.hp = (ushort)curHealth;
            _network.Send(attackedPacket.Write());
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            Debug.Log("Bullet : " + curHealth);
            StartCoroutine(OnDamage(reactVec));
        }
    }


    IEnumerator OnDamage(Vector3 reactVec)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 7;
            isChase = false;
            nav.enabled = false;  // ��� ��� �����ϱ� ���� ��Ȱ��ȭ
            anim.SetTrigger("doDie");

            // ����� �˹�
            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);

            //Destroy(gameObject, 2);
        }
    }

    void FreezVelocity() // ȸ�� ���� �ذ�
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void Targeting()
    {
        float targetRadius = 1.5f;
        float targetRange = 3f;

        RaycastHit[] rayHits =
            Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
        if(rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        yield return new WaitForSeconds(0.2f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(1f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);

    }

    void FixedUpdate()
    {
        Targeting();
        FreezVelocity();
    }

}
