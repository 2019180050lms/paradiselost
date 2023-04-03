using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int PlayerId { get; set; }

    public float speed = 15.0f;
    public GameObject[] weapons;
    public bool[] hasWeapons;

    //public string name;

    public ushort hp;
    public int dir = 0;
    public float hAxis;
    public float vAxis;
    public bool wDown;
    public bool iDown;
    public bool sDown1;
    public bool sDown2;
    public bool fDown;

    public bool isDodge;
    public bool isJump;
    public bool isSwap;
    public bool isFireReady = true;
    public bool other_jump;

    public Vector3 moveVec;
    public Vector3 moveVec2;
    public Vector3 posVec;
    public Vector3 dodgeVec;

    public Rigidbody rigid;
    public Animator anim;

    public GameObject nearObject;
    public Weapon equipWeapon;
    public HitBox hitBox;
    public int equipWeaponIndex = -1;
    public float fireDelay;
    bool isBorder;

    int jumpCount = 0;

    void Start()
    {
        
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, posVec, 0.005f);
        Jump(other_jump);
        other_jump = false;
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
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

    public void Move()
    {
        // 방향값을 무조건 1로 고정
        moveVec2 = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec2 = dodgeVec;

        if (isSwap || !isFireReady)
            moveVec2 = Vector3.zero;

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

        anim.SetBool("isRun", moveVec2 != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    public void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    public void Jump(bool isJump)
    {
        if (isJump && jumpCount == 0 && moveVec != Vector3.zero)
        {
            jumpCount += 1;
            rigid.AddForce(Vector3.up * speed, ForceMode.Impulse);
            //.SetBool("isJump", true);
            //anim.SetTrigger("doJump");
            jumpCount -= 1;
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

    public void Swap()
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
            nearObject = null;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }
}
