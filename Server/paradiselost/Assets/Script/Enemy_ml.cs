using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_ml : MonoBehaviour
{
    float speed = 5.0f;
    public int enemyId;
    public int enemyType;
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public bool isChase;
    public bool isAttack;
    public bool hitEnemy = false;

    public Vector3 prevVec;
    public Vector3 moveVec2;
    public Vector3 posVec;
    public Vector3 rotVec;


    public BoxCollider meleeArea;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;

    float timer;
    int waitingtime;
    NavMeshAgent nav;
    public HitBox hitBox;
    public Animator anim;
    public Transform bulletPos;
    public Transform bulletPos2;
    public GameObject bullet;
    public GameObject hitEffect;
    public int dir;
    

    public GameObject gunParticleObj;
    public ParticleSystem gunParticle;
    NetworkManager _network;

    public ParticleSystem ps;
    public bool walking;


    int count = 0;

    public SoundManager soundManager;
    public AudioSource audioSource;

    private void Start()
    {
        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        hitBox = GetComponent<HitBox>();
        ps = GetComponentInChildren<ParticleSystem>();
        bulletPos = transform.GetChild(2);
        bulletPos2 = transform.GetChild(3);
        // 사운드
        audioSource = gameObject.AddComponent<AudioSource>();
        soundManager = GetComponent<SoundManager>();

        gunParticleObj = transform.GetChild(2).gameObject;
        gunParticle = gunParticleObj.GetComponent<ParticleSystem>();

        hitEffect = GameObject.Find("hitEffect");
        hitEffect.SetActive(false);
        walking = true;
        
        timer = 0.0f;
        waitingtime = 1;
        //DontDestroyOnLoad(this);
        DontDestroyOnLoad(this);
    }

    void Awake()
    {
        //anim = GetComponentInChildren<Animator>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        //mat = GetComponentInChildren<MeshRenderer>().material;  // Material�� MesgRenderer�� ���� ������
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
        Vector3 velo = Vector3.zero;
        //transform.position = Vector3.Lerp(transform.position, pos, 0.001f);
        transform.position = Vector3.SmoothDamp(transform.position, posVec, ref velo, 0.1f);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rotVec), Time.deltaTime * 2f);

        //MoveControl(posVec);

        //m_MoveControl();


        if (transform.tag == "EnemyTurret" && isAttack && count == 0)
        {
            Debug.Log("몬스터 총");
            StartCoroutine("Shoot");
            count++;
        }
        if (transform.tag == "Enemy" && isAttack && count == 0)
        {
            StartCoroutine("Attack");
            count++;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("doAttack"))
            hitEnemy = true;
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("doAttack"))
            hitEnemy = false;

        moveVec2 = Vector3.zero;
    }

    void MoveControl(Vector3 target)
    {
        var dir = (target - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target);
        if (distance <= 0.01f)
        {
            return;
        }
        else
        {
            transform.position += dir * speed * Time.deltaTime;
            if (!isAttack)
                transform.LookAt(target);
        }
    }

    void m_MoveControl()
    {
        switch (dir)
        {
            case 0:
                moveVec2 = new Vector3(0, 0, 0);
                break;
            case 1:
                moveVec2 = new Vector3(1, 0, 0);
                break;
            case 2:
                moveVec2 = new Vector3(-1, 0, 0);
                break;
            case 3:
                moveVec2 = new Vector3(0, 0, 1);
                break;
            case 4:
                moveVec2 = new Vector3(0, 0, -1);
                break;
            case 5:
                moveVec2 = new Vector3(Mathf.Sqrt(0.5f), 0, Mathf.Sqrt(0.5f));
                break;
            case 6:
                moveVec2 = new Vector3(Mathf.Sqrt(0.5f), 0, -(Mathf.Sqrt(0.5f)));
                break;
            case 7:
                moveVec2 = new Vector3(-(Mathf.Sqrt(0.5f)), 0, Mathf.Sqrt(0.5f));
                break;
            case 8:
                moveVec2 = new Vector3(-(Mathf.Sqrt(0.5f)), 0, -(Mathf.Sqrt(0.5f)));
                break;
            default:
                break;
        }

        transform.position += moveVec2 * speed * Time.deltaTime;
        transform.LookAt(transform.position + moveVec2);
    }

    void OnTriggerEnter(Collider other)
    {
        if (curHealth >= 20)
        {
            if (other.tag == "Melee")
            {
                //Debug.Log(" �浹 " );
                Weapon weapon = other.GetComponent<Weapon>();
                //curHealth -= weapon.damage;
                Vector3 reactVec = transform.position - other.transform.position;
                // Debug.Log("Melee : " + curHealth);
                StartCoroutine(OnDamage(reactVec));
                C_AttackedMonster attackedPacket = new C_AttackedMonster();
                attackedPacket.id = enemyId;
                attackedPacket.hp = (short)curHealth;
                attackedPacket.playerId = weapon.ParentId;
                attackedPacket.hitEnemy = hitEnemy;
                _network.Send(attackedPacket.Write());
            }
            else if (other.tag == "Bullet")
            {
                Bullet bullet = other.GetComponent<Bullet>();
                //curHealth -= bullet.damage;

                Vector3 reactVec = transform.position - other.transform.position;
                Destroy(other.gameObject);

                if (curHealth < 1)
                    curHealth = 0;
                C_AttackedMonster attackedPacket = new C_AttackedMonster();
                attackedPacket.id = enemyId;
                attackedPacket.hp = (short)curHealth;
                attackedPacket.playerId = bullet.ParentID;
                attackedPacket.hitEnemy = hitEnemy;
                _network.Send(attackedPacket.Write());
                StartCoroutine(OnDamage(reactVec));
            }
            else if (other.tag == "EnemyBullet")
            {
                Destroy(other);
            }
        }
    }


    IEnumerator OnDamage(Vector3 reactVec)
    {
        yield return new WaitForSeconds(0.1f);
        hitEffect.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        hitEffect.SetActive(false);
    }

    void FreezVelocity() 
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

        //Debug.Log("몬스터 공격");
        isChase = false;
        isAttack = true;
        anim.SetTrigger("doAttack");

        yield return new WaitForSeconds(0.2f);
        hitBox.meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        hitBox.meleeArea.enabled = false;

        yield return new WaitForSeconds(3f); // 몬스터 공격속도

        isChase = true;
        isAttack = false;
        //anim.SetBool("isAttack", false);

        count--;
    }

    IEnumerator Shoot()
    {
        audioSource.clip = soundManager.shootSfx;
        
        Debug.Log("Shoot 코루틴 0");
        isAttack = true;

        anim.SetTrigger("doAttack");
        yield return new WaitForSeconds(0.5f);
        GameObject intantBullet = Instantiate(Resources.Load("EnemyBullet", typeof(GameObject)), bulletPos.position, bulletPos.rotation) as GameObject;
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        //BossMissile test = intantBullet.AddComponent<BossMissile>();

        GameObject intantBullet2 = Instantiate(Resources.Load("EnemyBullet", typeof(GameObject)), bulletPos2.position, bulletPos2.rotation) as GameObject;
        Rigidbody bulletRigid2 = intantBullet2.GetComponent<Rigidbody>();
        //BossMissile test2 = intantBullet2.AddComponent<BossMissile>();

        //test.enemyId = enemyId;
        Debug.Log("Shoot 코루틴 ");
        audioSource.Play();
        gunParticle.Play();
        bulletRigid.velocity = bulletPos.forward * 60;
        bulletRigid2.velocity = bulletPos.forward * 60;
        Destroy(intantBullet, 3f);
        Destroy(intantBullet2, 3f);
        //yield return new WaitForSeconds(1f);
        isAttack = false;


        count--;
    }

    void FixedUpdate()
    {
        Targeting();
        FreezVelocity();
    }

}
