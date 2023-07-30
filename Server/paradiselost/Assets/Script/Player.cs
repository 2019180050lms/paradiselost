﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int PlayerId { get; set; }

    public float speed = 10.5f;
    public List<GameObject> weapons = new List<GameObject>() { };
    public bool[] hasWeapons = new bool[4];
    public Transform bulletPos;
    public Transform bulletPos2;
    public Transform bulletPos3;
    public GameObject bullet;
    public GameObject bullet2;
    public GameObject bullet3;
    public Object intantBullet;

    //public string name;

    public short hp;
    public int dir = 0;
    public float hAxis;
    public float vAxis;
    public bool wDown;
    public bool iDown;
    public bool cDown1;
    public bool cDown2;
    public bool fDown;

    public bool isDodge;
    public bool isJump;
    public bool isSwap;
    public bool isFireReady = true;
    public bool other_jump;

    // 아이템
    public ushort head;
    public ushort body;
    public ushort leg;

    public Vector3 moveVec;
    public Vector3 moveVec2;
    public Vector3 posVec;
    public Vector3 dodgeVec;

    public Rigidbody rigid;
    public Animator anim;

    public Animator anim_Head;
    public Animator anim_Body;
    public Animator anim_Leg;

    public GameObject nearObject;
    public Weapon equipWeapon;
    public HitBox hitBox;
    public ParticleSystem ps;
    public ParticleSystem gunParticle;
    public TrailRenderer trailEffect;
    public TrailRenderer twoHandTrailEffect;

    public int equipWeaponIndex = -1;
    public float fireDelay;
    public bool falling = false;

    public bool isJumping;
    public bool isShot;
    public bool isShotGun;
    public int bulletCount = 0;

    public int atkCombo = 0;
    public float currentTime;
    public float maxTime;

    public int setActiveWeapon = 0;
    // 사운드
    public SoundManager soundManager;
    public AudioSource audioSource;
    public AudioSource audioSourceRun;
    public AudioSource audioSourceBgm;

    // 퀘스트
    public int questInt = 0;

    [HideInInspector]
    public float delay_body;
    public float delay_leg;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //anim = GetComponentInChildren<Animator>();
        anim = GetComponent<Animator>();
        ps = GetComponent<ParticleSystem>();
        gunParticle = GetComponentInChildren<ParticleSystem>();
        bulletPos = transform.GetChild(0);
        bulletPos2 = transform.GetChild(1);
        bulletPos3 = transform.GetChild(2);
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSourceRun = gameObject.AddComponent<AudioSource>();
        audioSourceBgm = gameObject.AddComponent<AudioSource>();
        //ps.Play();
        //ps.Emit(100);
        //ps.Stop();
    }
    void Start()
    {
        delay_body = 0f;
        delay_leg = 0f;
        isJumping = false;
        hp = 100;
        weapons.Add(GameObject.Find("Weapon Hammer"));
        weapons.Add(GameObject.Find("Weapon Rifle"));
        weapons.Add(GameObject.Find("Weapon 2H Sword"));
        weapons.Add(GameObject.Find("Weapon ShotGun"));
        weapons[0].SetActive(false);
        weapons[1].SetActive(false);
        weapons[2].SetActive(false);
        weapons[3].SetActive(false);
        equipWeaponIndex = 4;
        DontDestroyOnLoad(this);
    }


    //void Delay_Update()
    //{
    //    delay_body -= Time.deltaTime;
    //    delay_leg -= Time.deltaTime;

    //    anim_Body.SetFloat("Delay", delay_body);
    //    anim_Leg.SetFloat("Delay", delay_leg);
    //}

    void Update()
    {
        //Delay_Update();
        Vector3 velo = Vector3.zero;
        if (!falling)
            transform.position = Vector3.SmoothDamp(transform.position, posVec, ref velo, 0.03f);
        Jump(other_jump);
        if (transform.position == posVec)
        {
            anim.SetBool("isRun", false);
            //anim_Leg.SetBool("isRun", false);
        }

        if (isShot && bulletCount == 0)
        {
            Debug.Log("총알 발사 생성");
            bullet = Object.Instantiate(intantBullet) as GameObject;
            gunParticle.Play();
            bullet.transform.position = new Vector3(bulletPos.transform.position.x, bulletPos.transform.position.y + 2, bulletPos.transform.position.z);
            bullet.transform.rotation = bulletPos.rotation;
            Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = bulletPos.forward * 120;
            Bullet BulletId = bullet.GetComponent<Bullet>();
            BulletId.ParentID = PlayerId;
            bulletCount--;
            Destroy(bullet, 3f);
        }

        if (isShotGun && bulletCount == 0)
        {
            bullet = Object.Instantiate(intantBullet) as GameObject;
            bullet.transform.position = bulletPos.transform.position;
            gunParticle.Play();
            //audioSource.clip = soundManager.shootSfx;
            //audioSource.Play();
            bullet.transform.rotation = bulletPos.rotation;
            Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = bulletPos.forward * 120;
            Bullet BulletId = bullet.GetComponent<Bullet>();
            BulletId.ParentID = PlayerId;

            bullet2 = Object.Instantiate(intantBullet) as GameObject;
            bullet2.transform.position = bulletPos2.transform.position;
            bullet2.transform.rotation = bulletPos2.rotation;
            Rigidbody bulletRigid2 = bullet2.GetComponent<Rigidbody>();
            bulletRigid2.velocity = bulletPos2.forward * 120;
            Bullet BulletId2 = bullet2.GetComponent<Bullet>();
            BulletId2.ParentID = PlayerId;

            bullet3 = Object.Instantiate(intantBullet) as GameObject;
            bullet3.transform.position = bulletPos3.transform.position;
            bullet3.transform.rotation = bulletPos3.rotation;
            Rigidbody bulletRigid3 = bullet3.GetComponent<Rigidbody>();
            bulletRigid3.velocity = bulletPos3.forward * 120;
            Bullet BulletId3 = bullet3.GetComponent<Bullet>();
            BulletId3.ParentID = PlayerId;

            bulletCount--;
            Destroy(bullet, 0.2f);
            Destroy(bullet2, 0.2f);
            Destroy(bullet3, 0.2f);
        }

        moveVec2 = Vector3.zero;

        if (setActiveWeapon == 0)
        {
            weapons[0].SetActive(false);
            weapons[1].SetActive(false);
            weapons[2].SetActive(false);
            weapons[3].SetActive(false);
        }

        if (setActiveWeapon == 1)
        {
            weapons[0].SetActive(true);
            weapons[1].SetActive(false);
            weapons[2].SetActive(false);
            weapons[3].SetActive(false);
        }

        if (setActiveWeapon == 2)
        {
            weapons[0].SetActive(false);
            weapons[1].SetActive(true);
            weapons[2].SetActive(false);
            weapons[3].SetActive(false);
        }

        if (setActiveWeapon == 3)
        {
            weapons[0].SetActive(false);
            weapons[1].SetActive(false);
            weapons[2].SetActive(true);
            weapons[3].SetActive(false);
        }

        if (setActiveWeapon == 4)
        {
            weapons[0].SetActive(false);
            weapons[1].SetActive(false);
            weapons[2].SetActive(false);
            weapons[3].SetActive(true);
        }



    }



    void FreezeRotation() // 회전 버그 해결
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        
        FreezeRotation();
    }

    


    public void Move()
    {
        // 방향값을 무조건 1로 고정
        moveVec2 = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec2 = dodgeVec;

        if (isSwap || !isFireReady)
            moveVec2 = Vector3.zero;

        if(delay_body <= 0 && delay_leg <= 0)
        {
            if (wDown)
            {
                transform.position += moveVec2 * speed * 0.1f * Time.deltaTime;
                moveVec = transform.position;
            }
            else
            {
                transform.position += moveVec2 * speed * Time.deltaTime;
                moveVec = transform.position;
            }
        }
       

        //anim.SetBool("isRun", moveVec2 != Vector3.zero);
        //anim.SetBool("isWalk", wDown);
    }

    public void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    public void Jump(bool Jumping)
    {
        if (Jumping)
        {
            if (!isJumping)
            {
                isJumping = true;
                rigid.AddForce(Vector3.up * speed * 0.7f, ForceMode.Impulse);
            }
            else
                return;
            //.SetBool("isJump", true);
            anim.SetTrigger("doJump");
        }
    }

    public void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            anim.SetTrigger("doSwing");
            fireDelay = 0;
        }
    }
    public void Dodge()
    {
    }

    public IEnumerator timer()
    {
        currentTime = 0;
        maxTime = 5;
        while (currentTime < maxTime)
        {
            currentTime = currentTime + Time.deltaTime;
            //Debug.Log(currentTime);
            yield return null;
        }

        currentTime = 0;
        //anim.SetBool("isCombo2", false);
    }

    public IEnumerator Punch()
    {

        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        //meleeArea.enabled = true;
        //hitBox.meleeArea.enabled = true;

        yield return new WaitForSeconds(0.3f);

        yield return new WaitForSeconds(0.4f);
        //meleeArea.enabled = false;
        //hitBox.meleeArea.enabled = false;
        StopCoroutine("timer");
        currentTime = 0;
    }

    public IEnumerator Swing()
    {
        audioSource.clip = soundManager.slashSfx;


        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        //meleeArea.enabled = true;
        hitBox.meleeArea.enabled = true;
        //trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        audioSource.Play();

        yield return new WaitForSeconds(0.4f);
        //meleeArea.enabled = false;
        hitBox.meleeArea.enabled = false;
        //trailEffect.enabled = false;
        StopCoroutine("timer");
        currentTime = 0;
    }

    public IEnumerator TwoHandSwing()
    {
        audioSource.clip = soundManager.twoHandSlashSfx;


        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        //meleeArea.enabled = true;
        StartCoroutine("timer");
        hitBox.meleeArea.enabled = true;
        //twoHandTrailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        audioSource.Play();

        yield return new WaitForSeconds(1f);
        //meleeArea.enabled = false;
        hitBox.meleeArea.enabled = false;
        //twoHandTrailEffect.enabled = false;
        StopCoroutine("timer");
        Debug.Log("stopTImer");
        currentTime = 0;
    }

    IEnumerator Shot()
    {
        Debug.Log("Shot 코루틴");
        // 총알 발사
        isShot = true;
        yield return new WaitForSeconds(0.15f);
        isShot = false;
        //yield return new WaitForSeconds(1f);
        bulletCount++;

        if (bulletCount > 1)
            bulletCount = 0;
        //bullet = Object.Instantiate(intantBullet) as GameObject;
        //bullet.transform.position = bulletPos.transform.position;
        //bullet.transform.rotation = bulletPos.rotation;
        //Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
        //bulletRigid.velocity = bulletPos.forward * 75;
        
        //Destroy(bullet, 3f);
        //yield return new WaitForSeconds(2f);
    }

    IEnumerator ShotGun()
    {
        Debug.Log("ShotGun 코루틴");
        // 총알 발사
        isShotGun = true;
        yield return new WaitForSeconds(0.15f);
        isShotGun = false;
        //yield return new WaitForSeconds(1f);
        bulletCount++;

        if (bulletCount > 1)
            bulletCount = 0;
        //bullet = Object.Instantiate(intantBullet) as GameObject;
        //bullet.transform.position = bulletPos.transform.position;
        //bullet.transform.rotation = bulletPos.rotation;
        //Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
        //bulletRigid.velocity = bulletPos.forward * 75;

        //Destroy(bullet, 3f);
        //yield return new WaitForSeconds(2f);
    }

    public void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    public void SwapOut()
    {
        isSwap = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
        // Debug.Log(nearObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            //Destroy(other.gameObject);
            //other.gameObject.SetActive(false);
            //Debug.Log("destroy item");
        }

        else if (other.tag == "EnemyBullet")
        {
            // 피격 처리
            BossMissile monsterInfo = other.GetComponent<BossMissile>(); // 공격한 몬스터 객체 불러오기
            //Debug.Log(monsterInfo.enemyId);  // 공격한 몬스터 객체의 ID 출력
            //hp -= 20;
            Debug.Log(hp);
            Destroy(other.gameObject);
            //cs_send_playerdamage(monsterInfo.enemyId);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            //anim.SetBool("isJump", false);
            isJumping = false;
        }

    }
}
