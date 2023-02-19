using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public int ammo;
    public Camera followCamera;

    public int health;

    public int maxAmmo;
    public int maxHealth;

    float hAxis;
    float vAxis;
    bool wDown;
    bool iDown;
    bool jDown;
    bool sDown1;
    bool sDown2;
    bool fDown;
    bool rDown;

    bool isDodge;
    bool isJump;
    bool isSwap;
    bool isReload;
    bool isFireReady = true;

    bool isDamage;

    bool isBorder;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    MeshRenderer[] meshs;

    GameObject nearObject;
    Weapon equipWeapon;
    int equipWeaponIndex = -1;
    float fireDelay;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        Dodge();
        Swap();
        Interaction();
        Reload();

     
        

    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
        iDown = Input.GetButtonDown("Interaction");
        rDown = Input.GetButtonDown("Reload");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");

    }

    void Move()
    {
        // 방향값을 무조건 1로 고정
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        
        if (isDodge)
            moveVec = dodgeVec;

        if (isSwap || !isFireReady)
            moveVec = Vector3.zero;

        if (!isBorder)
        {
            //if (wDown)
            //    transform.position += moveVec * speed * 0.3f * Time.deltaTime;
            //else
            //    transform.position += moveVec * speed * Time.deltaTime;
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        }

        anim.SetBool("isRun", moveVec != Vector3.zero);
        //anim.SetBool("isWalk", wDown);
        if(wDown)
            anim.SetTrigger("doSwing");
    }

    void Turn()
    {
        // 키보드에 의한 회전
        transform.LookAt(transform.position + moveVec);

        // 마우스에 의한 회전
        if (fDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        if(jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        //if (fDown && isFireReady && !isDodge && !isSwap)

        if (fDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");  // 삼항연산자 : 장착하고 있는 무기가 근접이면 Swing 아니면 Shot
            fireDelay = 0;
            Debug.Log("공격");
        }
    }

    void Reload()
    {
        if (equipWeapon == null)
            return;

        if (equipWeapon.type == Weapon.Type.Melee)
            return;

        if (ammo == 0)
            return;

        if(rDown && !isJump && !isDodge && !isSwap && isFireReady)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 0.5f);
        }
    }
    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo; // 소지하고 있는 탄수가 maxammo 보다 적으면 플레이어에게 남아있는 개수 아니면 최대개수
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        

        isReload = false;
    }
    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;

        if ((sDown1 || sDown2) && !isJump && !isDodge)
        {
            if (equipWeapon != null)
                 equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.5f);
        }
    }

    void Interaction()
    {
        if(iDown && nearObject != null && !isJump && !isDodge)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                //Destroy(nearObject);
            }
        }
    }
    
    void StopToWall() // 관통 버그 해결
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));       // ray가 벽을 감지하면 Border가 true
    }
    void FreezeRotation() // 회전 버그 해결
    {
        rigid.angularVelocity = Vector3.zero;
    }
     void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
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
            nearObject = null;
    }

     void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            Destroy(other.gameObject);
        }

        else if (other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;
                StartCoroutine(OnDamage());
            }
        }
    }

    IEnumerator OnDamage() 
    {
        // 피격시 무적시간
        isDamage = true;
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }
        yield return new WaitForSeconds(1f); 

        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
    }
}
