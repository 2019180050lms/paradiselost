using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy1StageBoss : MonoBehaviour
{
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
    public Transform bulletPos3;
    public Transform bulletPos4;
    public Transform bulletPos5;
    public Transform bulletPos6;

    public GameObject bullet;
    public GameObject hitEffect;
    
    NetworkManager _network;

    public ParticleSystem ps;
    public bool walking;


    int count = 0;

    public SoundManager soundManager;
    public AudioSource audioSource;

    public ParticleSystem ShootEffect;

    private void Start()
    {
        ShootEffect = GetComponentInChildren<ParticleSystem>();
        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        hitBox = GetComponent<HitBox>();
        ps = GetComponentInChildren<ParticleSystem>();
        bulletPos = transform.GetChild(1);
        bulletPos2 = transform.GetChild(2);
        bulletPos3 = transform.GetChild(3);
        bulletPos4 = transform.GetChild(4);
        bulletPos5 = transform.GetChild(5);
        bulletPos6 = transform.GetChild(6);
        // 사운드
        audioSource = gameObject.AddComponent<AudioSource>();
        soundManager = GetComponent<SoundManager>();

        hitEffect = GameObject.Find("hitEffect");
        hitEffect.SetActive(false);
        walking = true;
        
        timer = 0.0f;
        waitingtime = 1;

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

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
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

        Debug.Log("몬스터 공격");
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

    IEnumerator Fire()
    {

        isChase = false;
        isAttack = true;
        anim.SetTrigger("doFire");

        yield return new WaitForSeconds(0.2f);
        //ShootEffect.Emit(100);
        ShootEffect.Play();
        yield return new WaitForSeconds(3f);
        ShootEffect.Stop();

        yield return new WaitForSeconds(3f); // 몬스터 공격속도

        isChase = true;
        isAttack = false;

        count--;
    }

    IEnumerator Shoot()
    {
        audioSource.clip = soundManager.shootSfx;
        audioSource.Play();
        Debug.Log("Shoot 코루틴 0");
        isAttack = true;
        anim.SetTrigger("doRangeAttack");
        yield return new WaitForSeconds(0.5f);
        GameObject intantBullet = Instantiate(Resources.Load("EnemyBullet2", typeof(GameObject)), bulletPos.position, bulletPos.rotation) as GameObject;
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        //BossMissile test = intantBullet.AddComponent<BossMissile>();

        GameObject intantBullet2 = Instantiate(Resources.Load("EnemyBullet2", typeof(GameObject)), bulletPos2.position, bulletPos2.rotation) as GameObject;
        Rigidbody bulletRigid2 = intantBullet2.GetComponent<Rigidbody>();
        //BossMissile test2 = intantBullet2.AddComponent<BossMissile>();

        GameObject intantBullet3 = Instantiate(Resources.Load("EnemyBullet2", typeof(GameObject)), bulletPos3.position, bulletPos3.rotation) as GameObject;
        Rigidbody bulletRigid3 = intantBullet3.GetComponent<Rigidbody>();
        //BossMissile test3 = intantBullet3.AddComponent<BossMissile>();

        GameObject intantBullet4 = Instantiate(Resources.Load("EnemyBullet2", typeof(GameObject)), bulletPos4.position, bulletPos4.rotation) as GameObject;
        Rigidbody bulletRigid4 = intantBullet4.GetComponent<Rigidbody>();
        //BossMissile test4 = intantBullet4.AddComponent<BossMissile>();

        GameObject intantBullet5 = Instantiate(Resources.Load("EnemyBullet2", typeof(GameObject)), bulletPos5.position, bulletPos5.rotation) as GameObject;
        Rigidbody bulletRigid5 = intantBullet5.GetComponent<Rigidbody>();
        //BossMissile test5 = intantBullet5.AddComponent<BossMissile>();

        GameObject intantBullet6 = Instantiate(Resources.Load("EnemyBullet2", typeof(GameObject)), bulletPos6.position, bulletPos6.rotation) as GameObject;
        Rigidbody bulletRigid6 = intantBullet6.GetComponent<Rigidbody>();
        //BossMissile test6 = intantBullet6.AddComponent<BossMissile>();

        //test.enemyId = enemyId;
        Debug.Log("Shoot 코루틴 ");

        bulletRigid.velocity = bulletPos.forward * 10;
        bulletRigid2.velocity = bulletPos.forward * 15;
        bulletRigid3.velocity = bulletPos.forward * 7;
        bulletRigid4.velocity = bulletPos.forward * 20;
        bulletRigid5.velocity = bulletPos.forward * 25;
        bulletRigid6.velocity = bulletPos.forward * 13;

        Destroy(intantBullet, 3f);
        Destroy(intantBullet2, 3f);
        Destroy(intantBullet3, 3f);
        Destroy(intantBullet4, 3f);
        Destroy(intantBullet5, 3f);
        Destroy(intantBullet6, 3f);
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
