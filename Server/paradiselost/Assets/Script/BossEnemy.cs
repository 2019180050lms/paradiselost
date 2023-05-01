using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossEnemy : MonoBehaviour
{
    public enum Type {  A, D};
    public Type enemyType;
    public int enemyId;
    public int maxHealth;
    public int curHealth;
    //public Transform target;
    public BoxCollider meleeArea;
    public HitBox hitBox;
    public GameObject bullet;
    public bool isChase;
    public bool isAttack;
    public bool isDead;
    //test
    public Vector3 moveVec2;
    public Vector3 posVec;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;

    public NavMeshAgent nav;

    public Animator anim;

    NetworkManager _network;

    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;
    public Transform target2;

    public Player target;
    public Vector3 targetPos;
    void Start()
    {
        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        hitBox = GetComponent<HitBox>();
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();  // Material은 MesgRenderer를 통해 가져옴
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        missilePortA = transform.GetChild(2);
        missilePortB = transform.GetChild(3);

        //target2 = GameObject.Find("Player_t1(Clone)").GetComponent<Transform>();

        target = GameObject.Find("Player_t1(Clone)").GetComponent<Player>();
        if(target.PlayerId == 1)
        {
            Debug.Log("find PlayerID  1");
            targetPos = target.transform.position;
        }
    }

    void ChaseStart()
    {
    }
     void Update()
    {
        transform.position = Vector3.Lerp(transform.position, posVec, 0.005f);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            //Debug.Log(" 충돌 " );
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            //Debug.Log("Melee : " + curHealth);
            StartCoroutine(OnDamage(reactVec));

            C_AttackedMonster attackedPacket = new C_AttackedMonster();
            attackedPacket.id = enemyId;
            attackedPacket.hp = (short)curHealth;
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

            C_AttackedMonster attackedPacket = new C_AttackedMonster();
            attackedPacket.id = enemyId;
            attackedPacket.hp = (short)curHealth;
            _network.Send(attackedPacket.Write());
        }
    }
    

    IEnumerator OnDamage(Vector3 reactVec)
    {
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
        }
        else
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            gameObject.layer = 7;
            isDead = true;
            isChase = false;
            //nav.enabled = false;  // 사망 모션 구현하기 위해 비활성화
            //anim.SetTrigger("doDie");

            // 사망시 넉백
            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);

            //if(enemyType != Type.D)
                //Destroy(gameObject, 2);

        }
    }

    void FreezVelocity() // 회전 버그 해결
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void Targeting()
    {
        if(!isDead && enemyType != Type.D)
        {
            float targetRadius = 1.5f;
            float targetRange = 3f;

            RaycastHit[] rayHits =
                Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
            if (rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
        
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        yield return new WaitForSeconds(0.2f);
        hitBox.enabled = true;

        yield return new WaitForSeconds(1f);
        hitBox.enabled = false;

        yield return new WaitForSeconds(1f);

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);

    }

    IEnumerator MissileShot()
    {
        //anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(Resources.Load("Boss Missile", typeof(GameObject)), missilePortA.position, missilePortA.rotation) as GameObject;
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.targetPos = targetPos;

        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(Resources.Load("Boss Missile", typeof(GameObject)), missilePortB.position, missilePortB.rotation) as GameObject;
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.targetPos = targetPos;


        yield return new WaitForSeconds(2f);

    }

    void FixedUpdate()
    {
        Targeting();
        FreezVelocity();
    }

}
