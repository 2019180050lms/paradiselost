using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyPlayer : Player
{
    NetworkManager _network;

    public Inventory inven;
    public int playerType;

    public float xmove = 0;  // X축 누적 이동량
    public float ymove = 0;  // Y축 누적 이동량
    public float distance = 8;
    public static int health = 80;
    public static int maxHealth = 100;
    public bool jDown;
    public bool testJump;
    public bool A_dontMove = false;
    public bool donShoot = false;
    public int _stage;

    bool frontDown;
    bool leftDown;
    bool rightDown;
    bool backDown;
    bool isBorder;
    public bool hasHeadItem = false;

    bool camera1 = true;
    bool camera2 = false;

    
    // 카메라
    [SerializeField] [Range(0.01f, 0.1f)] float shakeRange = 0.05f;
    [SerializeField] [Range(0.1f, 1f)] float duration = 0.5f;
    Vector3 cameraPos;
    bool isDamaging;
    public void Start()
    {
        delay_body = 0.0f;
        delay_leg = 0.0f;
        StartCoroutine("CoSendPacket");
        hitBox = GetComponent<HitBox>();
        soundManager = GetComponent<SoundManager>();
        inven = GetComponent<Inventory>();
        // 사운드
        audioSourceRun.clip = soundManager.runningSfx;
        audioSourceRun.loop = true;
        audioSourceRun.Play();

        audioSourceBgm.clip = soundManager.Bgm;
        audioSourceBgm.loop = true;
        audioSourceBgm.Play();

        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        bulletPos = transform.GetChild(0);
        
        //transform.tag = "MyPlayer";
        weapons.Add(GameObject.Find("Weapon Hammer"));
        weapons[0].name = "MyPlayerSword";
        weapons.Add(GameObject.Find("Weapon Rifle"));
        weapons[1].name = "MyPlayerRifle";
        weapons.Add(GameObject.Find("Weapon 2H Sword"));
        weapons[2].name = "MyPlayer2HSword";
        weapons.Add(GameObject.Find("Weapon ShotGun"));
        weapons[3].name = "MyPlayerShotGun";
        weapons[0].SetActive(false);
        weapons[1].SetActive(false);
        weapons[2].SetActive(false);
        weapons[3].SetActive(false);
        equipWeaponIndex = 4;

        //trailEffect = GameObject.Find("trailEffect").GetComponent<TrailRenderer>();
        //twoHandTrailEffect = GameObject.Find("twoHandTrailEffect").GetComponent<TrailRenderer>();

        DontDestroyOnLoad(this);

        
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

        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    Debug.Log("P");
        //    SceneManager.LoadScene("InGame2");
        //}
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    SceneManager.LoadScene("InGame3");
        //}

        GetInput();

        audioSourceRun.mute = false;

        frontDown = Input.GetKey(KeyCode.W);
        leftDown = Input.GetKey(KeyCode.A);
        rightDown = Input.GetKey(KeyCode.D);
        backDown = Input.GetKey(KeyCode.S);

        anim.SetBool("isRun", true);

        if (moveVec == Vector3.zero)
        {
            //anim_Body.SetBool("isRun", false);
            //anim_Leg.SetBool("isRun", false);
            anim.SetBool("isRun", false);
            audioSourceRun.mute = true;
        }

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


        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Swing"))
            A_dontMove = true;
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Swing"))
            A_dontMove = false;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("swing2"))
            A_dontMove = true;
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("swing2"))
            A_dontMove = false;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("swing3"))
            A_dontMove = true;
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("swing3"))
            A_dontMove = false;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Run_Aim"))
            donShoot = true;
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Run_Aim"))
            donShoot = false;

        if (!donShoot)
            MoveControl();
        Jump(testJump);

        if (delay_body <= 0 && delay_leg <= 0)
        {
            //MoveControl();
            //Jump(testJump);
        }

        if(isJumping || A_dontMove)
            audioSourceRun.mute = true;

        if (isBorder)
        {
            moveVec = Vector3.zero;
        }

        
        if (isShot && bulletCount == 0 && !donShoot)
        {
            bullet = Object.Instantiate(intantBullet) as GameObject;
            bullet.transform.position = bulletPos.transform.position;
            gunParticle.Play();
            audioSource.clip = soundManager.shootSfx;
            audioSource.Play();
            bullet.transform.rotation = bulletPos.rotation;
            Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = bulletPos.forward * 120;
            Bullet BulletId = bullet.GetComponent<Bullet>();
            BulletId.ParentID = PlayerId;
            bulletCount--;
            Destroy(bullet, 3f);
        }

        if (isShotGun && bulletCount == 0 && !donShoot)
        {
            bullet = Object.Instantiate(intantBullet) as GameObject;
            bullet.transform.position = bulletPos.transform.position;
            gunParticle.Play();
            audioSource.clip = soundManager.shootSfx;
            audioSource.Play();
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

        Vector3 reverseDistance = new Vector3(0.0f, 0.0f, distance); // 이동량에 따른 Z 축방향의 벡터를 구합니다.
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z) - Camera.main.transform.rotation * reverseDistance; // 플레이어의 위치에서 카메라가 바라보는 방향에 벡터값을 적용한 상대 좌표를 차감합니다.

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
            cs_move_packet();
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
        if (isDamaging)
            StartShake();
        //Debug.Log(isDamaging);
        isDamaging = false;
        if (Input.GetMouseButton(1))
        {
            xmove += Input.GetAxis("Mouse X"); // 마우스의 좌우 이동량을 xmove 에 누적합니다.
            ymove -= Input.GetAxis("Mouse Y"); // 마우스의 상하 이동량을 ymove 에 누적합니다.

            if (xmove > 360)
                xmove = 0;
            else if (xmove < -360)
                xmove = 0;
        }

        

    }

    void StartShake()
    {
        Debug.Log("startShake");
        float cameraPosX = Random.value * shakeRange * 2 - shakeRange;
        float cameraPosY = Random.value * shakeRange * 2 - shakeRange;
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.x += cameraPosX;
        cameraPos.y += cameraPosY;
        Camera.main.transform.position = cameraPos;

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



    


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
        // Debug.Log(nearObject.name);
        else if (other.tag == "EnemyMelee" || other.tag == "EnemyBullet" || other.tag == "BossMelee")
        {
            isDamaging = true;
        }
        else if(other.tag == "Portal1")
        {
            C_Portal packet = new C_Portal();
            packet.stage = 1;
            _network.Send(packet.Write());
            SceneManager.LoadScene("Stage1");
            _stage = 1;
            inven.Invoke("FindInven", 1f);
        }
        else if (other.tag == "Portal2")
        {
            C_Portal packet = new C_Portal();
            packet.stage = 2;
            _network.Send(packet.Write());
            SceneManager.LoadScene("Stage2");
            _stage = 2;
            inven.Invoke("FindInven", 1f);
        }
        else if (other.tag == "Portal3")
        {
            C_Portal packet = new C_Portal();
            packet.stage = 3;
            _network.Send(packet.Write());
            SceneManager.LoadScene("Stage3");
            _stage = 3;
            inven.Invoke("FindInven", 1f);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        isDamaging = true;
        anim.SetTrigger("doDamaged");
        Debug.Log("파티클 ");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyMelee")
        {
            // 피격 처리
            Enemy monsterInfo = other.GetComponentInParent<Enemy>(); // 공격한 몬스터 객체 불러오기
            //Debug.Log(monsterInfo.enemyId);  // 공격한 몬스터 객체의 ID 출력
            //hp -= 20;
            cameraPos = Camera.main.transform.position;
            anim.SetTrigger("doDamaged");
            cs_send_playerdamage(monsterInfo.enemyId);
        }

        else if(other.tag == "BossMelee")
        {
            BossEnemy monsterInfo2 = other.GetComponentInParent<BossEnemy>(); // 공격한 몬스터 객체 불러오기
            //Debug.Log(monsterInfo.enemyId);  // 공격한 몬스터 객체의 ID 출력
            //hp -= 20;
            anim.SetTrigger("doDamaged");
            Debug.Log("Test boss id: " + monsterInfo2.enemyId);
            cs_send_playerdamage(monsterInfo2.enemyId);
        }

        else if (other.tag == "Boss1Melee")
        {
            Enemy1StageBoss monsterInfo2 = other.GetComponentInParent<Enemy1StageBoss>(); // 공격한 몬스터 객체 불러오기
            //Debug.Log(monsterInfo.enemyId);  // 공격한 몬스터 객체의 ID 출력
            //hp -= 20;
            anim.SetTrigger("doDamaged");
            Debug.Log("Test boss id: " + monsterInfo2.enemyId);
            cs_send_playerdamage(monsterInfo2.enemyId);
        }

        else if (other.tag == "EnemyBullet")
        {
            // 피격 처리
            BossMissile monsterInfo = other.GetComponent<BossMissile>();
            //Debug.Log(monsterInfo.enemyId);  // 공격한 몬스터 객체의 ID 출력
            //Debug.Log(hp);
            Debug.Log("원거리공격맞음");
            cs_send_playerdamage(monsterInfo.enemyId);
            Destroy(other.gameObject);
        }

        else if (other.tag == "key")
        {
            Object stageClear = Resources.Load("Items/StageClear");
            stageClearLogo = Object.Instantiate(stageClear) as GameObject;
            stageClearLogo.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z);
            Destroy(other.gameObject);
            C_Clear_Fail packet = new C_Clear_Fail();
            packet.type = 1;
            _network.Send(packet.Write());
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
            //cs_move_packet();
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