using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyPlayer : Player
{
    NetworkManager _network;


    public float xmove = 0;  // X축 누적 이동량
    public float ymove = 0;  // Y축 누적 이동량
    public float distance = 8;
    public static int health = 80;
    public static int maxHealth = 100;
    public bool jDown;
    public bool testJump;
    public bool A_dontMove = false;
    public bool donShoot = false;

    bool frontDown;
    bool leftDown;
    bool rightDown;
    bool backDown;
    bool isBorder;
    public bool hasHeadItem = false;

    [HideInInspector]
    public float delay_body;
    public float delay_leg;

    bool camera1 = true;
    bool camera2 = false;

    void Start()
    {
        delay_body = 0.0f;
        delay_leg = 0.0f;
        StartCoroutine("CoSendPacket");
        hitBox = GetComponent<HitBox>();
        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        bulletPos = transform.GetChild(0);
        //transform.tag = "MyPlayer";
        weapons.Add(GameObject.Find("Weapon Hammer"));
        weapons.Add(GameObject.Find("Weapon Rifle"));
        weapons[0].SetActive(false);
        weapons[1].SetActive(false);
        equipWeaponIndex = 2;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Camera1"))
        {
            if (camera1 == true)
            {
                camera1 = false;
                camera2 = false;
            }
            else
            {
                camera1 = true;
                camera2 = false;
            }
        }
        if (Input.GetButtonDown("Camera2"))
        {
            Debug.Log("press2");
            if (camera2 == true)
            {
                camera1 = false;
                camera2 = false;
            }
            else
            {
                camera2 = true;
                camera1 = false;
            }
            Debug.Log(camera2);
        }

        /*
        if (delay_body > 0f)
        {
            delay_body -= Time.deltaTime;
           
        }

        if(delay_leg > 0f)
        {
            delay_leg -= Time.deltaTime;
        }

        anim_Body.SetFloat("Delay", delay_body);
        anim_Leg.SetFloat("Delay", delay_leg);
        */
        GetInput();
        

        frontDown = Input.GetKey(KeyCode.W);
        leftDown = Input.GetKey(KeyCode.A);
        rightDown = Input.GetKey(KeyCode.D);
        backDown = Input.GetKey(KeyCode.S);

        if (moveVec == Vector3.zero)
        {
            //anim_Body.SetBool("isRun", false);
            //anim_Leg.SetBool("isRun", false);
            anim.SetBool("isRun", false);
            anim.SetBool("isRun", false);
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Swing"))
            A_dontMove = true;
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Swing"))
            A_dontMove = false;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Run_Aim"))
            donShoot = true;
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Run_Aim"))
            donShoot = false;

        MoveControl();
        Jump(testJump);

        if (delay_body <= 0 && delay_leg <= 0)
        {
            //MoveControl();
            //Jump(testJump);
        }

        
        if (isBorder)
        {
            moveVec = Vector3.zero;
        }

        
        if (isShot && bulletCount == 0 && !donShoot)
        {
            bullet = Object.Instantiate(intantBullet) as GameObject;
            bullet.transform.position = bulletPos.transform.position;
            gunParticle.Play();
            bullet.transform.rotation = bulletPos.rotation;
            Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = bulletPos.forward * 120;
            Bullet BulletId = bullet.GetComponent<Bullet>();
            BulletId.ParentID = PlayerId;
            bulletCount--;
            Destroy(bullet, 3f);
        }

        Debug.Log(bulletCount);
        testJump = false;
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButton("Jump");
        fDown = Input.GetButtonDown("Fire1");
        iDown = Input.GetButtonDown("Interaction");


    }

    void MoveControl()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        CameraMove();

        if (A_dontMove == false)
        {
            if (dir == 0)
                moveVec2 = new Vector3(0, 0, 0);
            else if (dir == 1)
                moveVec2 = new Vector3(1, 0, 0);
            else if (dir == 2)
                moveVec2 = new Vector3(-1, 0, 0);
            else if (dir == 3)
                moveVec2 = new Vector3(0, 0, 1);
            else if (dir == 4)
                moveVec2 = new Vector3(0, 0, -1);
            else if (dir == 5)
                moveVec2 = new Vector3(Mathf.Sqrt(0.5f), 0, Mathf.Sqrt(0.5f));
            else if (dir == 6)
                moveVec2 = new Vector3(Mathf.Sqrt(0.5f), 0, -(Mathf.Sqrt(0.5f)));
            else if (dir == 7)
                moveVec2 = new Vector3(-(Mathf.Sqrt(0.5f)), 0, Mathf.Sqrt(0.5f));
            else if (dir == 8)
                moveVec2 = new Vector3(-(Mathf.Sqrt(0.5f)), 0, -(Mathf.Sqrt(0.5f)));

            transform.position += moveVec2 * speed * Time.deltaTime;
            transform.LookAt(transform.position + moveVec2);
        }
    }

    void LateUpdate()
    {
        Camera.main.transform.rotation = Quaternion.Euler(ymove, xmove, 0); // 이동량에 따라 카메라의 바라보는 방향을 조정합니다.
        Vector3 reverseDistance = new Vector3(0.0f, 0.0f, distance); // 이동량에 따른 Z 축방향의 벡터를 구합니다.

        if (camera1)
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z) - Camera.main.transform.rotation * reverseDistance; // 플레이어의 위치에서 카메라가 바라보는 방향에 벡터값을 적용한 상대 좌표를 차감합니다.
        else if (camera2)
        {
            Player[] playerOb = GameObject.FindGameObjectWithTag("Player").GetComponents<Player>();
            Debug.Log(playerOb);
            Camera.main.transform.position = new Vector3(playerOb[0].transform.position.x, playerOb[0].transform.position.y + 5, playerOb[0].transform.position.z) - Camera.main.transform.rotation * reverseDistance;
        }
        //Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y + 7, transform.position.z - 5);
        //Debug.Log(xmove);
        if (Input.GetMouseButton(1))
        {
            xmove += Input.GetAxis("Mouse X"); // 마우스의 좌우 이동량을 xmove 에 누적합니다.
            ymove -= Input.GetAxis("Mouse Y"); // 마우스의 상하 이동량을 ymove 에 누적합니다.

            if (xmove > 360)
                xmove = 0;
            else if (xmove < -360)
                xmove = 0;
        }

        /*
        float delay_body = anim_Body.GetFloat("Delay");
        
        if(delay_body > 0)
        {
            anim_Body.SetFloat("Delay", delay_body - Time.deltaTime);
         }
        

        //float delay_leg = anim_Leg.GetFloat("Delay");
        if(delay_leg > 0)
        {
            anim_Leg.SetFloat("Delay", delay_leg - Time.deltaTime);
        }
        */
        

    }


    void StopToWall() // 관통 버그 해결
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));       // ray가 벽을 감지하면 Border가 true
    }

    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    void FreezeRotation() // 회전 버그 해결
    {
        rigid.angularVelocity = Vector3.zero;
    }


    void cs_move_packet()
    {
        if(moveVec != Vector3.zero || wDown || jDown)
        {
            C_Move movePacket = new C_Move();
            movePacket.playerIndex = PlayerId;
            movePacket.playerDir = dir;
            movePacket.hp = hp;
            movePacket.posX = transform.position.x;
            movePacket.posY = transform.position.y;
            movePacket.posZ = transform.position.z;
            movePacket.wDown = wDown;
            movePacket.isJump = jDown;
            _network.Send(movePacket.Write());
            //Debug.Log(PlayerId);
        }
        else
        {
            //Debug.Log("movevec = 0");
        }
    }

    void cs_send_playerdamage(int m_id)
    {
        C_AttackedPlayer packet = new C_AttackedPlayer();
        packet.p_id = PlayerId;
        packet.m_id = m_id;
        _network.Send(packet.Write());
        Debug.Log("player id: " + PlayerId + " monsterid: " + m_id);
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        //meleeArea.enabled = true;
        hitBox.meleeArea.enabled = true;
        //trailEffect.enabled = true;

        yield return new WaitForSeconds(0.7f);
        //meleeArea.enabled = false;
        hitBox.meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        //trailEffect.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
        // Debug.Log(nearObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyMelee")
        {
            // 피격 처리
            Enemy monsterInfo = other.GetComponentInParent<Enemy>(); // 공격한 몬스터 객체 불러오기
            //Debug.Log(monsterInfo.enemyId);  // 공격한 몬스터 객체의 ID 출력
            //hp -= 20;
            anim.SetTrigger("doDamaged");
            cs_send_playerdamage(monsterInfo.enemyId);
        }

        else if(other.tag == "BossMelee")
        {
            BossEnemy monsterInfo2 = other.GetComponentInParent<BossEnemy>(); // 공격한 몬스터 객체 불러오기
            //Debug.Log(monsterInfo.enemyId);  // 공격한 몬스터 객체의 ID 출력
            //hp -= 20;
            Debug.Log("Test boss id: " + monsterInfo2.enemyId);
            cs_send_playerdamage(monsterInfo2.enemyId);
        }

        else if (other.tag == "EnemyBullet")
        {
            // 피격 처리
            BossMissile monsterInfo = other.GetComponent<BossMissile>();
            //Debug.Log(monsterInfo.enemyId);  // 공격한 몬스터 객체의 ID 출력
            //Debug.Log(hp);
            Debug.Log("monster id" + monsterInfo.enemyId);
            cs_send_playerdamage(monsterInfo.enemyId);
            Destroy(other.gameObject);
        }
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (wDown)
                Debug.Log("wDown True");

            //CameraMove();
            cs_move_packet();
        }
    }

    void CameraMove()
    {
        // ----카메라 오른쪽 회전----
        if (xmove > 337.5 && xmove < 260 || xmove > 0 && xmove < 22.5 || xmove < 0 && xmove > -22.5 || xmove < -337.5 && xmove > -360 || xmove == 0) // 정면
        {
            if (frontDown && !leftDown && !rightDown && !backDown)
                dir = 3;
            else if (leftDown && !frontDown && !rightDown && !backDown)
                dir = 2;
            else if (rightDown && !frontDown && !leftDown && !backDown)
                dir = 1;
            else if (backDown && !frontDown && !leftDown && !rightDown)
                dir = 4;
            else if (frontDown && leftDown)
                dir = 7;
            else if (frontDown && rightDown)
                dir = 5;
            else if (backDown && leftDown)
                dir = 8;
            else if (backDown && rightDown)
                dir = 6;
            else
                dir = 0;
        }
        else if ((xmove > 22.5 && xmove < 67.5) || (xmove < -292.5 && xmove > -337.5)) // 오른쪽으로 45도 회전
        {
            if (frontDown && !leftDown && !rightDown && !backDown)
                dir = 5;
            else if (leftDown && !frontDown && !rightDown && !backDown)
                dir = 7;
            else if (rightDown && !frontDown && !leftDown && !backDown)
                dir = 6;
            else if (backDown && !frontDown && !leftDown && !rightDown)
                dir = 8;
            else if (frontDown && leftDown)
                dir = 3;
            else if (frontDown && rightDown)
                dir = 1;
            else if (backDown && leftDown)
                dir = 2;
            else if (backDown && rightDown)
                dir = 4;
            else
                dir = 0;
        }
        else if ((xmove > 67.5 && xmove < 112.5) || (xmove < -247.5 && xmove > -292.5)) // 오른쪽으로 90도 회전
        {
            if (frontDown && !leftDown && !rightDown && !backDown)
                dir = 1; // 오른쪽
            else if (leftDown && !frontDown && !rightDown && !backDown)
                dir = 3;
            else if (rightDown && !frontDown && !leftDown && !backDown)
                dir = 4;
            else if (backDown && !frontDown && !leftDown && !rightDown)
                dir = 2;
            else if (frontDown && leftDown)
                dir = 5;
            else if (frontDown && rightDown)
                dir = 6;
            else if (backDown && leftDown)
                dir = 7;
            else if (backDown && rightDown)
                dir = 8;
            else
                dir = 0;
        }
        else if ((xmove > 112.5 && xmove < 157.5) || (xmove < -202.5 && xmove > -247.5)) // 오른쪽으로 135도 회전
        {
            if (frontDown && !leftDown && !rightDown && !backDown)
                dir = 6;
            else if (leftDown && !frontDown && !rightDown && !backDown)
                dir = 5;
            else if (rightDown && !frontDown && !leftDown && !backDown)
                dir = 8;
            else if (backDown && !frontDown && !leftDown && !rightDown)
                dir = 7;
            else if (frontDown && leftDown)
                dir = 1;
            else if (frontDown && rightDown)
                dir = 4;
            else if (backDown && leftDown)
                dir = 3;
            else if (backDown && rightDown)
                dir = 2;
            else
                dir = 0;
        }
        else if ((xmove > 157.5 && xmove < 202.5) || (xmove < -157.5 && xmove > -202.5)) // 오른쪽으로 180도 회전
        {
            if (frontDown && !leftDown && !rightDown && !backDown)
                dir = 4;
            else if (leftDown && !frontDown && !rightDown && !backDown)
                dir = 1;
            else if (rightDown && !frontDown && !leftDown && !backDown)
                dir = 2;
            else if (backDown && !frontDown && !leftDown && !rightDown)
                dir = 3;
            else if (frontDown && leftDown)
                dir = 6;
            else if (frontDown && rightDown)
                dir = 8;
            else if (backDown && leftDown)
                dir = 5;
            else if (backDown && rightDown)
                dir = 7;
            else
                dir = 0;
        }
        else if ((xmove > 202.5 && xmove < 247.5) || (xmove < -112.5 && xmove > -157.5)) // 오른쪽으로 225도 회전
        {
            if (frontDown && !leftDown && !rightDown && !backDown)
                dir = 8;
            else if (leftDown && !frontDown && !rightDown && !backDown)
                dir = 6;
            else if (rightDown && !frontDown && !leftDown && !backDown)
                dir = 7;
            else if (backDown && !frontDown && !leftDown && !rightDown)
                dir = 5;
            else if (frontDown && leftDown)
                dir = 4;
            else if (frontDown && rightDown)
                dir = 2;
            else if (backDown && leftDown)
                dir = 1;
            else if (backDown && rightDown)
                dir = 3;
            else
                dir = 0;
        }
        else if ((xmove > 247.5 && xmove < 292.5) || (xmove < -67.5 && xmove > -112.5)) // 오른쪽으로 270도 회전
        {
            if (frontDown && !leftDown && !rightDown && !backDown)
                dir = 2;
            else if (leftDown && !frontDown && !rightDown && !backDown)
                dir = 4;
            else if (rightDown && !frontDown && !leftDown && !backDown)
                dir = 3;
            else if (backDown && !frontDown && !leftDown && !rightDown)
                dir = 1;
            else if (frontDown && leftDown)
                dir = 8;
            else if (frontDown && rightDown)
                dir = 7;
            else if (backDown && leftDown)
                dir = 6;
            else if (backDown && rightDown)
                dir = 5;
            else
                dir = 0;
        }
        else if ((xmove > 292.5 && xmove < 337.5) || (xmove < -22.5 && xmove > -67.5)) // 오른쪽으로 315도 회전
        {
            if (frontDown && !leftDown && !rightDown && !backDown)
                dir = 7;
            else if (leftDown && !frontDown && !rightDown && !backDown)
                dir = 8;
            else if (rightDown && !frontDown && !leftDown && !backDown)
                dir = 5;
            else if (backDown && !frontDown && !leftDown && !rightDown)
                dir = 6;
            else if (frontDown && leftDown)
                dir = 2;
            else if (frontDown && rightDown)
                dir = 3;
            else if (backDown && leftDown)
                dir = 4;
            else if (backDown && rightDown)
                dir = 1;
            else
                dir = 0;
        }
    }
}