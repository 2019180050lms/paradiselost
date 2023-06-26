using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    public MyPlayer _myplayer;
    public Player player = null;
    Enemy enemy = null;
    public Joint_Robot joint;
    public BossEnemy _boss = null;
    Dictionary<int, Player> _players = new Dictionary<int, Player>();
    public Dictionary<int, Joint_Robot> _playerParts = new Dictionary<int, Joint_Robot>();
    Dictionary<int, Enemy> _enemys = new Dictionary<int, Enemy>();
    Joint_Robot c_p_parts = null;
    public GameObject item;
    public Vector3 moveVec;
    public static PlayerManager Instance { get; } = new PlayerManager();


    void SetCharacter(int playerId, short hp, int playerType, Vector3 pos)
    {
        Object obj = Resources.Load("test/test");
        GameObject go = Object.Instantiate(obj) as GameObject;
        switch (playerType)
        {
            case 1:
                {
                    MyPlayer myPlayer = go.AddComponent<MyPlayer>();
                    Object head = Resources.Load("Po_Head_Parts");
                    Object body = Resources.Load("Po_Body_Parts");
                    Object leg = Resources.Load("Po_Leg_Parts");

                    joint = go.AddComponent<Joint_Robot>();

                    joint.po_list = new GameObject[3];
                    joint.sh_list = new GameObject[3];
                    joint.sp_list = new GameObject[3];

                    joint.po_list[0] = head as GameObject;
                    joint.po_list[1] = body as GameObject;
                    joint.po_list[2] = leg as GameObject;

                    joint.leg = Object.Instantiate(joint.po_list[2], myPlayer.transform);
                    //leg.transform.position = new Vector3(-5, 5, 3);

                    joint.body = Object.Instantiate(joint.po_list[1], myPlayer.transform);
                    joint.body.transform.localPosition = joint.leg.transform.localPosition + joint.leg.transform.Find("Joint_Leg").transform.localPosition - joint.body.transform.Find("Joint_Leg").transform.localPosition;

                    joint.head = Object.Instantiate(joint.po_list[0], myPlayer.transform);
                    joint.head.transform.position = joint.body.transform.position + joint.body.transform.Find("Joint_Head").transform.position;

                    myPlayer.PlayerId = playerId;
                    myPlayer.hp = hp;
                    myPlayer.body = 1;
                    //myPlayer.name = name;
                    //Debug.Log(p.hp);

                    myPlayer.transform.tag = "MyPlayer";
                    myPlayer.transform.position = new Vector3(pos.x, pos.y, pos.z);
                    _myplayer = myPlayer;
                    myPlayer.anim_Head = joint.head.gameObject.transform.GetChild(0).GetComponent<Animator>();
                    myPlayer.anim_Body = joint.body.gameObject.transform.GetChild(0).GetComponent<Animator>();
                    myPlayer.anim_Leg = joint.leg.gameObject.transform.GetChild(0).GetComponent<Animator>();

                    Debug.Log(myPlayer.name);
                    break;
                }
            case 2:
                {
                    MyPlayer myPlayer = go.AddComponent<MyPlayer>();
                    Object head = Resources.Load("Sh_Head_Parts");
                    Object body = Resources.Load("Sh_Body_Parts");
                    Object leg = Resources.Load("Sh_Leg_Parts");

                    joint = go.AddComponent<Joint_Robot>();
                    joint.po_list = new GameObject[3];
                    joint.sh_list = new GameObject[3];
                    joint.sp_list = new GameObject[3];

                    joint.sh_list[0] = head as GameObject;
                    joint.sh_list[1] = body as GameObject;
                    joint.sh_list[2] = leg as GameObject;

                    joint.leg = Object.Instantiate(joint.sh_list[2], myPlayer.transform);
                    //leg.transform.position = new Vector3(-5, 5, 3);

                    joint.body = Object.Instantiate(joint.sh_list[1], myPlayer.transform);
                    joint.body.transform.position = joint.leg.transform.position + joint.leg.transform.Find("Joint_Leg").transform.localPosition - joint.body.transform.Find("Joint_Leg").transform.localPosition;

                    joint.head = Object.Instantiate(joint.sh_list[0], myPlayer.transform);
                    joint.head.transform.position = joint.body.transform.position + joint.body.transform.Find("Joint_Head").transform.position;

                    myPlayer.PlayerId = playerId;
                    //myPlayer.name = p.name;
                    myPlayer.transform.tag = "MyPlayer";
                    myPlayer.hp = hp;
                    myPlayer.body = 2;
                    //Debug.Log(p.hp);
                    myPlayer.transform.position = new Vector3(pos.x, pos.y, pos.z);
                    Debug.Log("pos in: " + pos.x + " " + pos.y + " " + pos.z);
                    _myplayer = myPlayer;
                    myPlayer.anim_Head = joint.head.gameObject.transform.GetChild(0).GetComponent<Animator>();
                    myPlayer.anim_Body = joint.body.gameObject.transform.GetChild(0).GetComponent<Animator>();
                    myPlayer.anim_Leg = joint.leg.gameObject.transform.GetChild(0).GetComponent<Animator>();


                    Debug.Log(myPlayer.name);
                    break;
                }
            case 3:
                {
                    MyPlayer myPlayer = go.AddComponent<MyPlayer>();
                    myPlayer.PlayerId = playerId;
                    //myPlayer.name = p.name;
                    myPlayer.transform.tag = "MyPlayer";
                    myPlayer.hp = hp;
                    myPlayer.body = 3;
                    //Debug.Log(p.hp);
                    myPlayer.intantBullet = Resources.Load("Bullet");
                    myPlayer.transform.position = new Vector3(pos.x, pos.y, pos.z);
                    Debug.Log("pos in: " + pos.x + " " + pos.y + " " + pos.z);
                    _myplayer = myPlayer;
                    Debug.Log("test p_id: " + _myplayer.PlayerId);
                    break;
                }
            default:
                Debug.Log("오류");
                break;
        }

    }

    public void Add(S_ENTER_PLAYER packet)
    {
        if (packet.type <= 3)
        {
            Vector3 playerPos = new Vector3(packet.x, packet.y, packet.z);
            SetCharacter(packet.id, packet.hp, packet.type, playerPos);
            Debug.Log("test pos: " + packet.x + packet.y + packet.z);
            Debug.Log("test name: " + packet.name);
            GameObject.Find("Game Manager").GetComponent<GameUIManager>().FindPlayerUI();
            /*
            switch (packet.weapon)
            {
                case 0:
                    _myplayer.equipWeapon = _myplayer.weapons[1].GetComponent<Weapon>();
                    _myplayer.equipWeapon.gameObject.SetActive(false);

                    _myplayer.equipWeapon = _myplayer.weapons[0].GetComponent<Weapon>();
                    _myplayer.equipWeapon.gameObject.SetActive(true);
                    break;
                case 1:
                    _myplayer.equipWeapon = _myplayer.weapons[0].GetComponent<Weapon>();
                    _myplayer.equipWeapon.gameObject.SetActive(false);

                    _myplayer.equipWeapon = _myplayer.weapons[1].GetComponent<Weapon>();
                    _myplayer.equipWeapon.gameObject.SetActive(true);
                    break;
            }
            */
        }
    }

    public void EnemyAdd(S_EnemyList packet)
    {
        foreach (S_EnemyList.Enemy p in packet.enemys)
        {
            //Object obj = Resources.Load("Player");
            //GameObject go = Object.Instantiate(obj) as GameObject;

        }
    }

    public void Move(S_BroadcastMove packet)
    {
        if (_myplayer.PlayerId == packet.playerId)
        {
            Vector3 movePos = new Vector3(packet.posX, packet.posY, packet.posZ);
            _myplayer.dir = packet.playerDir;
            _myplayer.wDown = packet.wDown;
            //Debug.Log(packet.isJump);
            _myplayer.testJump = packet.isJump;
            //_myplayer.transform.position = movePos;

            //Debug.Log(packet.hp);

            if (_myplayer.delay_body <= 0f && _myplayer.delay_leg <= 0f)
            {
                //_myplayer.anim_Head.SetBool("isRun", _myplayer.moveVec != Vector3.zero);
                //_myplayer.anim_Body.SetBool("isRun", _myplayer.moveVec != Vector3.zero);
                //_myplayer.anim_Leg.SetBool("isRun", _myplayer.moveVec != Vector3.zero);
                _myplayer.anim.SetBool("isRun", _myplayer.moveVec != Vector3.zero);
            }


            if (packet.wDown)
            {
                if ((int)_myplayer.body == 3)        // 플레이어 좌클릭
                {
                    _myplayer.StopCoroutine("timer");
                    if (_myplayer.hasWeapons[1] && !_myplayer.anim.GetCurrentAnimatorStateInfo(0).IsName("Run_Aim"))
                    {
                        _myplayer.StartCoroutine("Shot");
                        _myplayer.anim.SetTrigger("doAim");
                    }

                    if (!_myplayer.hasWeapons[0] && !_myplayer.hasWeapons[1] && _myplayer.currentTime == 0)
                    {
                        _myplayer.StopCoroutine("Punch");
                        _myplayer.StartCoroutine("Punch");

                        _myplayer.StopCoroutine("timer");
                        _myplayer.StartCoroutine("timer");

                        _myplayer.anim.SetTrigger("doPunch");
                    }

                    else if (!_myplayer.hasWeapons[0] && !_myplayer.hasWeapons[1] && _myplayer.currentTime > 0f && _myplayer.currentTime < 1.0f)
                    {
                        _myplayer.anim.SetTrigger("doPunch2");
                        _myplayer.StopCoroutine("Punch");
                        _myplayer.StartCoroutine("Punch");
                        _myplayer.currentTime = 0;

                    }

                    if (_myplayer.currentTime == 0)
                    {
                        if (!_myplayer.anim.GetCurrentAnimatorStateInfo(0).IsName("Swing") && _myplayer.hasWeapons[0]) // !_myplayer.anim.GetCurrentAnimatorStateInfo(0).IsName("Swing") &&
                        {
                            _myplayer.StopCoroutine("Swing");
                            _myplayer.StartCoroutine("Swing");

                            _myplayer.StopCoroutine("timer");
                            _myplayer.StartCoroutine("timer");

                            _myplayer.anim.SetTrigger("doSwing");
                        }
                    }
                    else if (!_myplayer.anim.GetCurrentAnimatorStateInfo(0).IsName("swing2") && _myplayer.hasWeapons[0] && _myplayer.currentTime > 0f && _myplayer.currentTime < 1.0f && _myplayer.atkCombo == 0)
                    {
                        _myplayer.anim.SetTrigger("doSwing2");
                        _myplayer.StopCoroutine("Swing");
                        _myplayer.StartCoroutine("Swing");
                        _myplayer.StartCoroutine("timer");
                        _myplayer.atkCombo = 1;
                        _myplayer.currentTime = 0;
                    }

                    else if (!_myplayer.anim.GetCurrentAnimatorStateInfo(0).IsName("swing3") && _myplayer.hasWeapons[0] && _myplayer.currentTime > 0f && _myplayer.currentTime < 1.0f && _myplayer.atkCombo == 1)
                    {
                        _myplayer.anim.SetTrigger("doSwing3");
                        _myplayer.StopCoroutine("Swing");
                        _myplayer.StartCoroutine("Swing");
                        _myplayer.atkCombo = 0;
                        _myplayer.currentTime = 0;
                    }

                    else if (_myplayer.hasWeapons[0] && _myplayer.currentTime > 2)
                    {
                        _myplayer.StopCoroutine("timer");
                        _myplayer.currentTime = 0;
                    }



                }
                else if ((int)_myplayer.body == 2)
                {
                    _myplayer.StopCoroutine("Swing");
                    _myplayer.StartCoroutine("Swing");
                    if (!_myplayer.anim_Body.GetCurrentAnimatorStateInfo(0).IsName("Swing"))
                    {
                        _myplayer.anim_Body.SetTrigger("doSwing");
                        _myplayer.delay_body = 0.3f;
                    }
                    _myplayer.anim_Head.SetTrigger("doSwing");

                    if (!_myplayer.anim_Leg.GetCurrentAnimatorStateInfo(0).IsName("Swing"))
                    {
                        _myplayer.anim_Leg.SetTrigger("doSwing");
                        _myplayer.delay_leg = 1.2f;
                    }
                }
                else
                {
                    _myplayer.StopCoroutine("Swing");
                    _myplayer.anim_Head.SetTrigger("doSwing");
                    _myplayer.anim_Body.SetTrigger("doSwing");
                    _myplayer.anim_Leg.SetTrigger("doSwing");
                    _myplayer.StartCoroutine("Swing");
                }
            }
            //_myplayer.transform.LookAt(_myplayer.transform.position + _myplayer.moveVec2);
        }
        else if (packet.playerId < 500)
        {
            if (_players.TryGetValue(packet.playerId, out player))
            {
                if (packet.posX == 19.0f && packet.posY == 2.0f && packet.posZ == 19.0f)
                {
                    player.falling = true;
                    player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
                    player.falling = false;
                }
                player.posVec = new Vector3(packet.posX, packet.posY, packet.posZ);
                if (packet.playerDir == 0)
                    player.moveVec2 = new Vector3(0, 0, 0);
                else if (packet.playerDir == 1)
                    player.moveVec2 = new Vector3(1, 0, 0);
                else if (packet.playerDir == 2)
                    player.moveVec2 = new Vector3(-1, 0, 0);
                else if (packet.playerDir == 3)
                    player.moveVec2 = new Vector3(0, 0, 1);
                else if (packet.playerDir == 4)
                    player.moveVec2 = new Vector3(0, 0, -1);
                else if (packet.playerDir == 5)
                    player.moveVec2 = new Vector3(Mathf.Sqrt(0.5f), 0, Mathf.Sqrt(0.5f));
                else if (packet.playerDir == 6)
                    player.moveVec2 = new Vector3(Mathf.Sqrt(0.5f), 0, -(Mathf.Sqrt(0.5f)));
                else if (packet.playerDir == 7)
                    player.moveVec2 = new Vector3(-(Mathf.Sqrt(0.5f)), 0, Mathf.Sqrt(0.5f));
                else if (packet.playerDir == 8)
                    player.moveVec2 = new Vector3(-(Mathf.Sqrt(0.5f)), 0, -(Mathf.Sqrt(0.5f)));

                //player.moveVec = new Vector3(_myplayer.hAxis, 0, _myplayer.vAxis).normalized;

                player.wDown = packet.wDown;
                player.other_jump = packet.isJump;
                //player.transform.position = movePos;

                //Debug.Log("Pmovevec:");
                //Debug.Log(player.moveVec);

                //player.anim.SetBool("isRun", player.moveVec2 != Vector3.zero);
                if (player.delay_body <= 0f && player.delay_leg <= 0f)
                {
                    player.anim.SetBool("isRun", player.moveVec2 != Vector3.zero);
                    //player.anim_Head.SetBool("isRun", player.moveVec2 != Vector3.zero);
                    //player.anim_Body.SetBool("isRun", player.moveVec2 != Vector3.zero);
                    //player.anim_Leg.SetBool("isRun", player.moveVec2 != Vector3.zero);
                }



                if (packet.wDown)
                {
                    Debug.Log("다른 player wDown ");
                    Debug.Log(player.currentTime);
                    //player.StopCoroutine("timer");
                    if (player.hasWeapons[1] && !player.anim.GetCurrentAnimatorStateInfo(0).IsName("Run_Aim"))
                    {
                        player.StartCoroutine("Shot");
                        player.anim.SetTrigger("doAim");
                    }

                    else if (!player.hasWeapons[0] && !player.hasWeapons[1] && player.currentTime == 0)
                    {
                        player.StopCoroutine("Punch");
                        player.StartCoroutine("Punch");

                        player.StopCoroutine("timer");
                        player.StartCoroutine("timer");

                        player.anim.SetTrigger("doPunch");
                    }

                    else if (!player.hasWeapons[0] && !player.hasWeapons[1] && player.currentTime > 0f && player.currentTime < 1.0f)
                    {
                        player.anim.SetTrigger("doPunch2");
                        player.StopCoroutine("Punch");
                        player.StartCoroutine("Punch");
                        player.currentTime = 0;

                    }

                    else if (player.currentTime == 0.0f)
                    {
                        Debug.Log("currenttime 0 ");
                        Debug.Log("swing1 ");
                        player.StopCoroutine("Swing");
                        player.StartCoroutine("Swing");

                        player.StopCoroutine("timer");
                        player.StartCoroutine("timer");

                        player.anim.SetTrigger("doSwing");
                    }

                    //else if (player.currentTime == 0)
                    //{
                    //    Debug.Log("currenttime 0 ");
                    //    if ( player.hasWeapons[0]) // !_myplayer.anim.GetCurrentAnimatorStateInfo(0).IsName("Swing") &&
                    //    {
                    //        Debug.Log("swing1 ");
                    //        player.StopCoroutine("Swing");
                    //        player.StartCoroutine("Swing");

                    //        player.StopCoroutine("timer");
                    //        player.StartCoroutine("timer");

                    //        player.anim.SetTrigger("doSwing");
                    //    }
                    //}

                    else if (player.hasWeapons[0] && player.currentTime > 0f && player.currentTime < 1.0f && player.atkCombo == 0)
                    {
                        Debug.Log("swing2");
                        player.anim.SetTrigger("doSwing2");
                        player.StopCoroutine("Swing");
                        player.StartCoroutine("Swing");
                        player.StartCoroutine("timer");
                        player.atkCombo = 1;
                        player.currentTime = 0;
                    }

                    else if (player.hasWeapons[0] && player.currentTime > 0f && player.currentTime < 1.0f && player.atkCombo == 1)
                    {
                        Debug.Log("swing3");
                        player.anim.SetTrigger("doSwing3");
                        player.StopCoroutine("Swing");
                        player.StartCoroutine("Swing");
                        player.atkCombo = 0;
                        player.currentTime = 0;
                    }

                    else if (player.hasWeapons[0] && player.currentTime > 2)
                    {
                        player.StopCoroutine("timer");
                        player.currentTime = 0;
                    }


                    else if ((int)player.body == 2)
                    {
                        player.StopCoroutine("Swing");
                        player.StartCoroutine("Swing");
                        if (!player.anim_Body.GetCurrentAnimatorStateInfo(0).IsName("Swing"))
                        {
                            player.anim_Body.SetTrigger("doSwing");
                            player.delay_body = 0.3f;
                        }
                        player.anim_Head.SetTrigger("doSwing");

                        if (!player.anim_Leg.GetCurrentAnimatorStateInfo(0).IsName("Swing"))
                        {
                            player.anim_Leg.SetTrigger("doSwing");
                            player.delay_leg = 1.2f;
                        }
                    }
                    else
                    {
                        player.StopCoroutine("Swing");
                        player.anim_Head.SetTrigger("doSwing");
                        player.anim_Body.SetTrigger("doSwing");
                        player.anim_Leg.SetTrigger("doSwing");
                        player.StartCoroutine("Swing");
                    }
                }
                player.transform.LookAt(player.transform.position + player.moveVec2);
            }
        }
        else
        {
            // 몬스터 움직임
            if (_enemys.TryGetValue(packet.playerId, out enemy))
            {
                //moveVec = new Vector3(packet.posX, packet.posY, packet.posZ).normalized;
                //enemy.transform.position += moveVec * 1f * 0.3f * Time.deltaTime;
                //moveVec = enemy.transform.position;

                enemy.isAttack = packet.wDown;
                if (packet.playerDir == 0)
                    enemy.moveVec2 = new Vector3(0, 0, 0);
                else if (packet.playerDir == 1)
                    enemy.moveVec2 = new Vector3(1, 0, 0);
                else if (packet.playerDir == 2)
                    enemy.moveVec2 = new Vector3(-1, 0, 0);
                else if (packet.playerDir == 3)
                    enemy.moveVec2 = new Vector3(0, 0, 1);
                else if (packet.playerDir == 4)
                    enemy.moveVec2 = new Vector3(0, 0, -1);
                else if (packet.playerDir == 5)
                    enemy.moveVec2 = new Vector3(Mathf.Sqrt(0.5f), 0, Mathf.Sqrt(0.5f));
                else if (packet.playerDir == 6)
                    enemy.moveVec2 = new Vector3(Mathf.Sqrt(0.5f), 0, -(Mathf.Sqrt(0.5f)));
                else if (packet.playerDir == 7)
                    enemy.moveVec2 = new Vector3(-(Mathf.Sqrt(0.5f)), 0, Mathf.Sqrt(0.5f));
                else if (packet.playerDir == 8)
                    enemy.moveVec2 = new Vector3(-(Mathf.Sqrt(0.5f)), 0, -(Mathf.Sqrt(0.5f)));

                if (enemy.enemyType != 4)
                {
                    enemy.moveVec2 = new Vector3(packet.posX, packet.posY, packet.posZ);
                }

                //enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, new Vector3(packet.posX, packet.posY, packet.posZ), 1f);
                //Debug.Log(enemy.transform.position);
                //enemy.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);

                enemy.posVec = new Vector3(packet.posX, packet.posY, packet.posZ);
                //enemy.anim.SetBool("isWalk", enemy.isAttack != false);
                if (packet.wDown)
                {
                    //enemy.StartCoroutine("Shoot");
                    //enemy.anim.SetTrigger("doAttack");
                    

                }
                //enemy.transform.LookAt(enemy.transform.position + enemy.moveVec2);
                enemy.rotVec = enemy.posVec - enemy.transform.position;
                //enemy.transform.LookAt(enemy.posVec);

                if (enemy.tag == "EnemyTurret")
                {
                    EnemyTurret enemyTurret = GameObject.Find("TargetArea").GetComponent<EnemyTurret>();
                    //enemy.transform.LookAt(enemyTurret.targetPos);
                    //Debug.Log(enemyTurret.targetPos);
                }

            }
            // 보스 처리
            else if (_boss.enemyId == packet.playerId)
            {
                _boss.isAttack = packet.wDown;
                _boss.bossAttack = packet.bossAttack;
                if (packet.playerDir == 0)
                    _boss.moveVec2 = new Vector3(0, 0, 0);
                else if (packet.playerDir == 1)
                    _boss.moveVec2 = new Vector3(1, 0, 0);
                else if (packet.playerDir == 2)
                    _boss.moveVec2 = new Vector3(-1, 0, 0);
                else if (packet.playerDir == 3)
                    _boss.moveVec2 = new Vector3(0, 0, 1);
                else if (packet.playerDir == 4)
                    _boss.moveVec2 = new Vector3(0, 0, -1);
                else if (packet.playerDir == 5)
                    _boss.moveVec2 = new Vector3(Mathf.Sqrt(0.5f), 0, Mathf.Sqrt(0.5f));
                else if (packet.playerDir == 6)
                    _boss.moveVec2 = new Vector3(Mathf.Sqrt(0.5f), 0, -(Mathf.Sqrt(0.5f)));
                else if (packet.playerDir == 7)
                    _boss.moveVec2 = new Vector3(-(Mathf.Sqrt(0.5f)), 0, Mathf.Sqrt(0.5f));
                else if (packet.playerDir == 8)
                    _boss.moveVec2 = new Vector3(-(Mathf.Sqrt(0.5f)), 0, -(Mathf.Sqrt(0.5f)));

                _boss.moveVec2 = new Vector3(packet.posX, packet.posY, packet.posZ);
                _boss.posVec = new Vector3(packet.posX, packet.posY, packet.posZ);
                //_boss.anim.SetBool("isWalk", _boss.isAttack != false);

                if (packet.bossAttack == 1)
                {
                    //_boss.StopCoroutine("Attack");
                    //_boss.anim.SetTrigger("doAttack");
                    //_boss.StartCoroutine("Attack");
                    //Debug.Log("Boss Attack1 " + packet.bossAttack);
                }
                else if (packet.bossAttack == 2)
                {
                    //_boss.StopCoroutine("MissileShot");
                    //_boss.StartCoroutine("MissileShot");
                    //Debug.Log("Boss Attack2 " + packet.bossAttack);
                    //_boss.transform.LookAt();
                }
                //enemy.transform.LookAt(enemy.transform.position + enemy.moveVec2);
                Debug.Log("StageBoss Move");
                _boss.transform.LookAt(_boss.posVec);
                if (enemy.tag == "StageBoss")
                {
                    //EnemyTurret enemyTurret = GameObject.Find("TargetArea").GetComponent<EnemyTurret>();

                    Debug.Log("StageBoss LookAt");
                }
            }
        }
    } 

    public void CollisionMove(S_Move packet)
    {
        if(_myplayer.PlayerId == packet.playerIndex)
        {
            _myplayer.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);

            Debug.Log("collision");
        }
    }


    public void EnterGame(S_BroadcastEnterGame packet)
    {
        if (packet.playerId == _myplayer.PlayerId)
            return;

        if (packet.type == 0)
            Debug.Log("캐릭터 생성");
        else if(packet.type == 1)
        {
            Object obj = Resources.Load("Player_t1");
            GameObject go = Object.Instantiate(obj) as GameObject;
            Object obj3 = Resources.Load("PlayerText");
            GameObject PlayerText = Object.Instantiate(obj3) as GameObject;

            Object head = Resources.Load("Po_Head_Parts");
            Object body = Resources.Load("Po_Body_Parts");
            Object leg = Resources.Load("Po_Leg_Parts");


            Player player = go.AddComponent<Player>();
            player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);

            player.PlayerId = packet.playerId;

            Joint_Robot jointP = go.AddComponent<Joint_Robot>();

            jointP.po_list = new GameObject[3];
            jointP.sh_list = new GameObject[3];
            jointP.sp_list = new GameObject[3];

            jointP.po_list[0] = head as GameObject;
            jointP.po_list[1] = body as GameObject;
            jointP.po_list[2] = leg as GameObject;

            jointP.leg = Object.Instantiate(jointP.po_list[2], player.transform);
            //leg.transform.position = new Vector3(-5, 5, 3);

            jointP.body = Object.Instantiate(jointP.po_list[1], player.transform);
            jointP.body.transform.position = jointP.leg.transform.position + jointP.leg.transform.Find("Joint_Leg").transform.localPosition - jointP.body.transform.Find("Joint_Leg").transform.localPosition;

            jointP.head = Object.Instantiate(jointP.po_list[0], player.transform);
            jointP.head.transform.position = jointP.body.transform.position + jointP.body.transform.Find("Joint_Head").transform.position;

            PlayerText.transform.SetParent(go.transform, false);
            PlayerText playerText = PlayerText.GetComponent<PlayerText>();

            playerText.playerText.text = packet.name;
            /*
            switch (packet.playerId % 4)
            {
                case 1:
                    playerText.playerText.text = "Player 1";
                    break;
                case 2:
                    playerText.playerText.text = "Player 2";
                    break;
                case 3:
                    playerText.playerText.text = "Player 3";
                    break;
                case 0:
                    playerText.playerText.text = "Player 4";
                    break;
            }
            */

            player.anim_Head = jointP.head.gameObject.transform.GetChild(0).GetComponent<Animator>();
            player.anim_Body = jointP.body.gameObject.transform.GetChild(0).GetComponent<Animator>();
            player.anim_Leg = jointP.leg.gameObject.transform.GetChild(0).GetComponent<Animator>();
            _playerParts.Add(packet.playerId, jointP);
            _players.Add(packet.playerId, player);

            GameObject.Find("Game Manager").GetComponent<GameUIManager>().FindPlayerUI();

            Debug.Log(player.name);
        }
        else if (packet.type == 2)
        {
            Object obj = Resources.Load("Player_t1");
            GameObject go = Object.Instantiate(obj) as GameObject;
            Object obj3 = Resources.Load("PlayerText");
            GameObject PlayerText = Object.Instantiate(obj3) as GameObject;

            Object head = Resources.Load("Sh_Head_Parts");
            Object body = Resources.Load("Sh_Body_Parts");
            Object leg = Resources.Load("Sh_Leg_Parts");


            Player player = go.AddComponent<Player>();
            player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);

            player.PlayerId = packet.playerId;

            Joint_Robot jointP = go.AddComponent<Joint_Robot>();

            jointP.po_list = new GameObject[3];
            jointP.sh_list = new GameObject[3];
            jointP.sp_list = new GameObject[3];

            jointP.sh_list[0] = head as GameObject;
            jointP.sh_list[1] = body as GameObject;
            jointP.sh_list[2] = leg as GameObject;

            jointP.leg = Object.Instantiate(jointP.sh_list[2], player.transform);
            //leg.transform.position = new Vector3(-5, 5, 3);

            jointP.body = Object.Instantiate(jointP.sh_list[1], player.transform);
            jointP.body.transform.position = jointP.leg.transform.position + jointP.leg.transform.Find("Joint_Leg").transform.localPosition - jointP.body.transform.Find("Joint_Leg").transform.localPosition;

            jointP.head = Object.Instantiate(jointP.sh_list[0], player.transform);
            jointP.head.transform.position = jointP.body.transform.position + jointP.body.transform.Find("Joint_Head").transform.position;

            PlayerText.transform.SetParent(go.transform, false);
            PlayerText playerText = PlayerText.GetComponent<PlayerText>();

            switch (packet.playerId % 4)
            {
                case 1:
                    playerText.playerText.text = "Player 1";
                    break;
                case 2:
                    playerText.playerText.text = "Player 2";
                    break;
                case 3:
                    playerText.playerText.text = "Player 3";
                    break;
                case 0:
                    playerText.playerText.text = "Player 4";
                    break;
            }

            player.anim_Head = jointP.head.gameObject.transform.GetChild(0).GetComponent<Animator>();
            player.anim_Body = jointP.body.gameObject.transform.GetChild(0).GetComponent<Animator>();
            player.anim_Leg = jointP.leg.gameObject.transform.GetChild(0).GetComponent<Animator>();

            _playerParts.Add(packet.playerId, jointP);
            _players.Add(packet.playerId, player);
            GameObject.Find("Game Manager").GetComponent<GameUIManager>().FindPlayerUI();
            Debug.Log(player.name);
        }
        else if (packet.type == 3)
        {
            Object obj = Resources.Load("test/test");
            GameObject go = Object.Instantiate(obj) as GameObject;
            Object obj3 = Resources.Load("PlayerText");
            GameObject PlayerText = Object.Instantiate(obj3) as GameObject;



            Player player = go.AddComponent<Player>();
            player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);

            player.PlayerId = packet.playerId;
            player.body = 3;
            player.anim = player.GetComponent<Animator>();
            player.equipWeaponIndex = 2;
            

            PlayerText.transform.SetParent(go.transform, false);
            PlayerText playerText = PlayerText.GetComponent<PlayerText>();
            playerText.playerText.text = packet.name;
            /*
            switch (packet.playerId % 4)
            {
                case 1:
                    playerText.playerText.text = "Player 1";
                    break;
                case 2:
                    playerText.playerText.text = "Player 2";
                    break;
                case 3:
                    playerText.playerText.text = "Player 3";
                    break;
                case 0:
                    playerText.playerText.text = "Player 4";
                    break;
            }
            */
            player.intantBullet = Resources.Load("Bullet") as Object;

            //_playerParts.Add(packet.playerId, jointP);
            Debug.Log("test o_id: " + player.PlayerId);
            _players.Add(packet.playerId, player);
            GameObject.Find("Game Manager").GetComponent<GameUIManager>().FindPlayerUI();
            //Debug.Log(player.name);
        }
        if (packet.type == 4)
        {
            Object obj = Resources.Load("Monster_turret");
            GameObject go = Object.Instantiate(obj) as GameObject;
            Enemy enemy = go.AddComponent<Enemy>();
            enemy.enabled = true;
            enemy.enemyId = packet.playerId;
            enemy.enemyType = packet.type;
            enemy.maxHealth = packet.hp;
            enemy.curHealth = packet.hp;
            enemy.ps = go.GetComponentInChildren<ParticleSystem>();

            enemy.posVec = new Vector3(packet.posX, packet.posY, packet.posZ);
            _enemys.Add(packet.playerId, enemy);

        }
        else if (packet.type == 5)
        {
            Object obj = Resources.Load("Monster/Robot2");
            GameObject go = Object.Instantiate(obj) as GameObject;
            Enemy enemy = go.AddComponent<Enemy>();
            enemy.enabled = true;
            enemy.enemyId = packet.playerId;
            enemy.enemyType = packet.type;
            enemy.maxHealth = packet.hp;
            enemy.curHealth = packet.hp;
            enemy.ps = go.GetComponentInChildren<ParticleSystem>();
            enemy.posVec = new Vector3(packet.posX, packet.posY, packet.posZ);
            _enemys.Add(packet.playerId, enemy);

            Debug.Log("Monster 생성");
        }
        else if (packet.type == 6)
        {
            Object obj = Resources.Load("Monster_Spider");
            GameObject go = Object.Instantiate(obj) as GameObject;
            Enemy enemy = go.AddComponent<Enemy>();
            enemy.enabled = true;
            enemy.enemyId = packet.playerId;
            enemy.enemyType = packet.type;
            enemy.maxHealth = packet.hp;
            enemy.curHealth = packet.hp;
            enemy.ps = go.GetComponentInChildren<ParticleSystem>();
            enemy.posVec = new Vector3(packet.posX, packet.posY, packet.posZ);
            _enemys.Add(packet.playerId, enemy);

        }
        else if (packet.type == 7)
        {
            Object obj = Resources.Load("Monster/BossTest");
            GameObject go = Object.Instantiate(obj) as GameObject;

            BossEnemy boss = go.AddComponent<BossEnemy>();
            boss.enabled = true;
            boss.enemyId = packet.playerId;
            boss.maxHealth = packet.hp;
            boss.curHealth = packet.hp;
            boss.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
            _boss = boss;
            Debug.Log("보스 생성");

        }
    }

    public void LeaveGame(S_BroadcastLeaveGame packet)
    {
        if(_myplayer.PlayerId == packet.playerId)
        {
            _myplayer.hp = 0;
            Debug.Log("사망 처리 테스트");
        }
        else
        {
            if(packet.playerId < 500)
            {
                Player player = null;
                if (_players.TryGetValue(packet.playerId, out player))
                {
                    player.hp = 0;
                    GameObject.Destroy(player.gameObject, 1f);
                    _players.Remove(packet.playerId);
                }
            }
            else
            {
                Enemy enemy = null;
                Object items = null;
                switch (packet.itemNum)
                {
                    case 0:
                        items = Resources.Load("Items/Weapon Rifle Item");
                        break;
                    case 1:
                        items = Resources.Load("Items/Weapon Sword Item");
                        break;
                    default:
                        items = null;
                        break;
                }
                Debug.Log("itemNum: " + packet.itemNum);
                if(_enemys.TryGetValue(packet.playerId, out enemy))
                {
                    SoundManager soundManager = enemy.GetComponent<SoundManager>();
                    enemy.audioSource.clip = soundManager.monsterDieSfx;
                    enemy.audioSource.Play();
                    enemy.ps.Play();
                    _myplayer.questInt++;
                    GameObject.Destroy(enemy.gameObject, 0.6f);
                    Debug.Log("dead monster test");
                    _enemys.Remove(packet.playerId);
                    if (items != null)
                    {
                        item = Object.Instantiate(items) as GameObject;
                        Rigidbody rigidbody = item.GetComponent<Rigidbody>();
                        item.transform.position = new Vector3(enemy.transform.position.x, 5f, enemy.transform.position.z);
                        rigidbody.AddForce(new Vector3(7, 10, 0), ForceMode.Impulse);
                    }
                }
                else if(packet.playerId == _boss.enemyId)
                {
                    _boss.anim.SetTrigger("doDie");
                    GameObject.Destroy(_boss.gameObject, 2);

                    Debug.Log("dead boss monster test");
                    _boss = null;
                }
            }
        }
    }

    public void ItemManager(S_Broadcast_Item packet)
    {
        if(packet.playerId == _myplayer.PlayerId)
        {
            Debug.Log("test item packet: " + packet.playerId + ", " + packet.charactorType + ", " + packet.itemType);
            if (packet.charactorType == 1)
            {
                switch (packet.itemType)
                {
                    case 1:
                        joint.po_list[0] = Resources.Load("Weapon Hammer") as GameObject;
                        joint.SwitchParts(packet.itemType);
                        break;
                    case 2:
                        joint.sh_list[0] = Resources.Load("Weapon Hammer") as GameObject;
                        joint.SwitchParts(packet.itemType);
                        break;
                    case 3:
                        joint.sp_list[0] = Resources.Load("Weapon Hammer") as GameObject;
                        joint.SwitchParts(packet.itemType);
                        break;
                }
            }
            else if (packet.charactorType == 2)  // 무기
            {
                switch (packet.itemType)
                {

                    case 0:
                        _myplayer.equipWeapon = _myplayer.weapons[1].GetComponent<Weapon>();
                        _myplayer.equipWeapon.gameObject.SetActive(false);

                        _myplayer.equipWeapon = _myplayer.weapons[0].GetComponent<Weapon>();
                        _myplayer.equipWeapon.gameObject.SetActive(true);
                        break;
                    case 1:
                        _myplayer.equipWeapon = _myplayer.weapons[0].GetComponent<Weapon>();
                        _myplayer.equipWeapon.gameObject.SetActive(false);

                        _myplayer.equipWeapon = _myplayer.weapons[1].GetComponent<Weapon>();
                        _myplayer.equipWeapon.gameObject.SetActive(true);
                        break;
                    case 3:
                        joint.sp_list[1] = Resources.Load("Weapon Hammer") as GameObject;
                        joint.SwitchParts(packet.itemType);
                        break;
                }
            }
            else if (packet.charactorType == 3)
            {
                switch (packet.itemType)
                {
                    case 1:
                        joint.po_list[2] = Resources.Load("Weapon Hammer") as GameObject;
                        joint.SwitchParts(packet.itemType);
                        break;
                    case 2:
                        joint.sh_list[2] = Resources.Load("Weapon Hammer") as GameObject;
                        joint.SwitchParts(packet.itemType);
                        break;
                    case 3:
                        joint.sp_list[2] = Resources.Load("Weapon Hammer") as GameObject;
                        joint.SwitchParts(packet.itemType);
                        break;
                }
            }
        }
        else if (_players.TryGetValue(packet.playerId, out player))
        {
            Debug.Log("test item packet: " + packet.playerId + ", " + packet.charactorType + ", " + packet.itemType);
            if (packet.charactorType == 1)
            {
                switch (packet.itemType)
                {
                    case 0:
                        player.equipWeapon = player.weapons[0].GetComponent<Weapon>();
                        player.equipWeapon.gameObject.SetActive(true);
                        break;
                    case 1:
                        player.equipWeapon = player.weapons[1].GetComponent<Weapon>();
                        player.equipWeapon.gameObject.SetActive(true);
                        break;
                    case 3:
                        c_p_parts.sp_list[0] = Resources.Load("Sp_Head_Parts") as GameObject;
                        c_p_parts.SwitchParts(packet.itemType);
                        break;
                }
                player.anim_Head = c_p_parts.head.gameObject.transform.GetChild(0).GetComponent<Animator>();
                player.head = packet.itemType;
                player.ps.Emit(100);
            }
            else if (packet.charactorType == 2) // 무기
            {
                switch (packet.itemType)
                {
                    case 0:
                        player.equipWeapon = player.weapons[1].GetComponent<Weapon>();
                        player.equipWeapon.gameObject.SetActive(false);
                        player.hasWeapons[1] = false;


                        player.hasWeapons[packet.itemType] = true;
                        player.equipWeapon = player.weapons[0].GetComponent<Weapon>();
                        player.equipWeapon.gameObject.SetActive(true);
                        player.body = 3;
                        break;
                    case 1:
                        player.equipWeapon = player.weapons[0].GetComponent<Weapon>();
                        player.equipWeapon.gameObject.SetActive(false);
                        player.hasWeapons[0] = false;

                        player.hasWeapons[packet.itemType] = true;
                        player.equipWeapon = player.weapons[1].GetComponent<Weapon>();
                        player.equipWeapon.gameObject.SetActive(true);
                        break;
                    case 3:
                        c_p_parts.sp_list[1] = Resources.Load("Sp_Body_Parts") as GameObject;
                        c_p_parts.SwitchParts(packet.itemType);
                        break;
                }
                //player.anim_Body = c_p_parts.body.gameObject.transform.GetChild(0).GetComponent<Animator>();
                player.body = packet.itemType;
                //player.ps.Emit(100);
            }
            else if (packet.charactorType == 3)
            {
                switch (packet.itemType)
                {
                    case 1:
                        c_p_parts.po_list[2] = Resources.Load("Po_Leg_Parts") as GameObject;
                        c_p_parts.SwitchParts(packet.itemType);
                        break;
                    case 2:
                        c_p_parts.sh_list[2] = Resources.Load("Sh_Leg_Parts") as GameObject;
                        c_p_parts.SwitchParts(packet.itemType);
                        break;
                    case 3:
                        c_p_parts.sp_list[2] = Resources.Load("Sp_Leg_Parts") as GameObject;
                        c_p_parts.SwitchParts(packet.itemType);
                        break;
                }
                player.anim_Leg = c_p_parts.leg.gameObject.transform.GetChild(0).GetComponent<Animator>();
                player.leg = packet.itemType;
                player.ps.Emit(100);
            }
        }
    }

    public void AttackedMonster(S_AttackedMonster packet)
    {
        Enemy enemy = null;

        
        if (_enemys.TryGetValue(packet.id, out enemy))
        {
            enemy.curHealth = packet.hp;
            enemy.anim.SetTrigger("doDamaged");
        }
        else if(packet.id >= 503)
        {
            _boss.curHealth = packet.hp;
            Debug.Log("보스 피격" + _boss.curHealth);
        }
    }

    public void BossAttack(S_BOSS_Attack packet)
    {

        Debug.Log("타겟 아이디: " + packet.targetid + " 보스 공격" + packet.bossAttack);
        if( packet.bossAttack == 1)
        {
            _boss.StartCoroutine("Attack");
            _boss.anim.SetTrigger("doAttack");
        }
        else if (packet.bossAttack == 0)
        {
            _boss.StartCoroutine("Shot");
            _boss.anim.SetTrigger("doRangeAttack");
        }

    }

    public void AttackedPlayer(S_AttackedPlayer packet)
    {
        

        if(_myplayer.PlayerId == packet.p_id)
        {
            _myplayer.hp = packet.hp;
        }
        else if(_players.TryGetValue(packet.p_id, out player))
        {
            player.anim.SetTrigger("doDamaged");
            player.hp = packet.hp;
        }

        Debug.Log("player hp: " + packet.hp);
    }
}
