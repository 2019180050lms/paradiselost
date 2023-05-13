using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int PlayerId { get; set; }

    public float speed = 15.0f;
    public List<GameObject> weapons = new List<GameObject>() { };
    public bool[] hasWeapons = new bool[1];
    public Transform bulletPos;
    public GameObject bullet;
    public Object intantBullet;

    //public string name;

    public ushort hp;
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

    public int equipWeaponIndex = -1;
    public float fireDelay;
    public bool falling = false;

    public bool isJumping;
    public bool isShot;
    public int bulletCount = 0;


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
        moveVec2 = Vector3.zero;
    }



    void Interaction()
    {
        Debug.Log("idown");
        if (iDown && nearObject != null && !isJump && !isDodge)
        {
            if (nearObject.tag == "Weapon")
            {
                ItemParts item = nearObject.GetComponent<ItemParts>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                //Destroy(nearObject);
            }
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
                transform.position += moveVec2 * speed * 0.3f * Time.deltaTime;
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
                rigid.AddForce(Vector3.up * speed, ForceMode.Impulse);
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
        /*
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
        */
    }

    //public void Swap()
    //{
    //    if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
    //        return;
    //    if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
    //        return;

    //    int weaponIndex = -1;
    //    if (sDown1) weaponIndex = 0;
    //    if (sDown2) weaponIndex = 1;

    //    if ((sDown1 || sDown2) && !isJump && !isDodge)
    //    {
    //        if (equipWeapon != null)
    //            equipWeapon.gameObject.SetActive(false);

    //        equipWeaponIndex = weaponIndex;
    //        equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
    //        equipWeapon.gameObject.SetActive(true);

    //        anim.SetTrigger("doSwap");

    //        isSwap = true;

    //        Invoke("SwapOut", 0.5f);
    //    }
    //}

    IEnumerator Shot()
    {
        delay_body = 0.6f;
        delay_leg = 0.6f;
        // 총알 발사
        isShot = true;

        yield return new WaitForSeconds(1f);
        isShot = false;

        //bullet = Object.Instantiate(intantBullet) as GameObject;
        //bullet.transform.position = bulletPos.transform.position;
        //bullet.transform.rotation = bulletPos.rotation;
        //Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
        //bulletRigid.velocity = bulletPos.forward * 75;
        bulletCount++;
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
