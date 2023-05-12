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


    void SetCharacter(int playerId, ushort hp, int playerType, bool isSelf, Vector3 pos)
    {
        if (isSelf)
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
                        joint.body.transform.position = joint.leg.transform.position + joint.leg.transform.Find("Joint_Leg").transform.localPosition  - joint.body.transform.Find("Joint_Leg").transform.localPosition;

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
                        myPlayer.transform.position = new Vector3(pos.x, pos.y, pos.z);
                        Debug.Log("pos in: " + pos.x + " " + pos.y + " " + pos.z);
                        _myplayer = myPlayer;

                        /*
                        MyPlayer myPlayer = go.AddComponent<MyPlayer>();
                        Object head = Resources.Load("Sp_Head_Parts");
                        Object body = Resources.Load("Sp_Body_Parts");
                        Object leg = Resources.Load("Sp_Leg_Parts");

                        myPlayer.body = 3;

                        joint = go.AddComponent<Joint_Robot>();
                        joint.po_list = new GameObject[3];
                        joint.sh_list = new GameObject[3];
                        joint.sp_list = new GameObject[3];

                        joint.sp_list[0] = head as GameObject;
                        joint.sp_list[1] = body as GameObject;
                        joint.sp_list[2] = leg as GameObject;

                        joint.leg = Object.Instantiate(joint.sp_list[2], myPlayer.transform);
                        joint.leg.transform.position = new Vector3(myPlayer.transform.position.x, myPlayer.transform.position.y, myPlayer.transform.position.z);

                        joint.body = Object.Instantiate(joint.sp_list[1], myPlayer.transform);
                        joint.body.transform.position = joint.leg.transform.position + joint.leg.transform.Find("Joint_Leg").transform.localPosition - joint.body.transform.Find("Joint_Leg").transform.localPosition;

                        joint.head = Object.Instantiate(joint.sp_list[0], myPlayer.transform);
                        joint.head.transform.position = joint.body.transform.position + joint.body.transform.Find("Joint_Head").transform.position;

                        myPlayer.PlayerId = playerId;
                        //myPlayer.name = p.name;
                        myPlayer.transform.tag = "MyPlayer";
                        myPlayer.hp = hp;
                        //Debug.Log(p.hp);
                        myPlayer.transform.position = new Vector3(pos.x, pos.y, pos.z);
                        Debug.Log("pos in: " + pos.x + " " + pos.y + " " + pos.z);
                        _myplayer = myPlayer;
                        myPlayer.anim_Head = joint.head.gameObject.transform.GetChild(0).GetComponent<Animator>();
                        myPlayer.anim_Body = joint.body.gameObject.transform.GetChild(0).GetComponent<Animator>();
                        myPlayer.anim_Leg = joint.leg.gameObject.transform.GetChild(0).GetComponent<Animator>();
                        myPlayer.intantBullet =Resources.Load("Bullet");
                        myPlayer.bulletPos = myPlayer.transform.GetChild(1);
                        Debug.Log(myPlayer.name);
                        */
                        break;
                    }
                default:
                    Debug.Log("오류");
                    break;
            }
        }
        else
        {
            //Object obj = Resources.Load("Player_t1");
            Object obj2 = Resources.Load("test/test");
            GameObject go = Object.Instantiate(obj2) as GameObject;
            Object obj3 = Resources.Load("PlayerText");
            GameObject PlayerText = Object.Instantiate(obj3) as GameObject;
            switch (playerType)
            {
                case 1:
                    {
                        Player player = go.AddComponent<Player>();
                        Object head = Resources.Load("Po_Head_Parts");
                        Object body = Resources.Load("Po_Body_Parts");
                        Object leg = Resources.Load("Po_Leg_Parts");

                        player.PlayerId = playerId;
                        player.hp = hp;
                        player.body = 1;
                        //player.name = p.name;
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
                        player.transform.position = new Vector3(pos.x, pos.y, pos.z);
                        player.posVec = new Vector3(pos.x, pos.y, pos.z);
                        PlayerText.transform.SetParent(go.transform, false);
                        PlayerText playerText = PlayerText.GetComponent<PlayerText>();

                        switch (playerId % 4)
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
                        player.intantBullet = Resources.Load("Bullet");
                        player.bulletPos = player.transform.GetChild(1);
                        _playerParts.Add(playerId, jointP);
                        _players.Add(playerId, player);

                        Debug.Log(player.name);
                        break;
                    }
                case 2:
                    {
                        Player player = go.AddComponent<Player>();
                        Object head = Resources.Load("Sh_Head_Parts");
                        Object body = Resources.Load("Sh_Body_Parts");
                        Object leg = Resources.Load("Sh_Leg_Parts");

                        player.PlayerId = playerId;
                        player.hp = hp;
                        player.body = 2;
                        //player.name = p.name;
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

                        player.transform.position = new Vector3(pos.x, pos.y, pos.z);
                        player.posVec = new Vector3(pos.x, pos.y, pos.z);

                        PlayerText.transform.SetParent(go.transform, false);
                        PlayerText playerText = PlayerText.GetComponent<PlayerText>();

                        switch (playerId % 4)
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

                        _playerParts.Add(playerId, jointP);
                        _players.Add(playerId, player);

                        Debug.Log(player.name);
                        break;
                    }
                case 3:
                    {
                        Player player = go.AddComponent<Player>();
                        //Object head = Resources.Load("Sp_Head_Parts");
                        //Object body = Resources.Load("Sp_Body_Parts");
                        //Object leg = Resources.Load("Sp_Leg_Parts");

                        player.PlayerId = playerId;
                        player.hp = hp;
                        player.body = 3;
                        //player.name = p.name;
                        /*
                        Joint_Robot jointP = go.AddComponent<Joint_Robot>();

                        jointP.po_list = new GameObject[3];
                        jointP.sh_list = new GameObject[3];
                        jointP.sp_list = new GameObject[3];

                        jointP.sp_list[0] = head as GameObject;
                        jointP.sp_list[1] = body as GameObject;
                        jointP.sp_list[2] = leg as GameObject;

                        jointP.leg = Object.Instantiate(jointP.sp_list[2], player.transform);
                        jointP.leg.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);

                        jointP.body = Object.Instantiate(jointP.sp_list[1], player.transform);
                        jointP.body.transform.position = jointP.leg.transform.position + jointP.leg.transform.Find("Joint_Leg").transform.localPosition - jointP.body.transform.Find("Joint_Leg").transform.localPosition;

                        jointP.head = Object.Instantiate(jointP.sp_list[0], player.transform);
                        jointP.head.transform.position = jointP.body.transform.position + jointP.body.transform.Find("Joint_Head").transform.position;
                        */
                        player.transform.position = new Vector3(pos.x, pos.y, pos.z);
                        player.posVec = new Vector3(pos.x, pos.y, pos.z);

                        PlayerText.transform.SetParent(go.transform, false);
                        PlayerText playerText = PlayerText.GetComponent<PlayerText>();

                        switch (playerId % 4)
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

                        player.anim = player.GetComponent<Animator>();
                        /*
                        player.anim_Head = jointP.head.gameObject.transform.GetChild(0).GetComponent<Animator>();
                        player.anim_Body = jointP.body.gameObject.transform.GetChild(0).GetComponent<Animator>();
                        player.anim_Leg = jointP.leg.gameObject.transform.GetChild(0).GetComponent<Animator>();
                        player.intantBullet = Resources.Load("Bullet");
                        player.bulletPos = player.transform.GetChild(1);
                        */
                        //_playerParts.Add(playerId, jointP);
                        _players.Add(playerId, player);

                        Debug.Log(player.name);
                        break;
                    }
                default:
                    Debug.Log("오류");
                    break;
            }


        }

    }

    public void Add(S_PlayerList packet)
    {
        foreach (S_PlayerList.Player p in packet.players)
        {
            //Object obj = Resources.Load("Player");
            //GameObject go = Object.Instantiate(obj) as GameObject;
            if(p.type <= 3)
            {
                Vector3 playerPos = new Vector3(p.posX, p.posY, p.posZ);
                SetCharacter(p.playerId, p.hp, p.type, p.isSelf, playerPos);
                Debug.Log("test pos: " + p.posX + p.posY + p.posZ);
                GameObject.Find("Game Manager").GetComponent<GameUIManager>().FindPlayerUI();
            }
        }
    }

    public void EnemyAdd(S_EnemyList packet)
    {
        foreach (S_EnemyList.Enemy p in packet.enemys)
        {
            //Object obj = Resources.Load("Player");
            //GameObject go = Object.Instantiate(obj) as GameObject;
            if (p.type == 4)
            {
                Object obj = Resources.Load("Monster_turret");
                GameObject go = Object.Instantiate(obj) as GameObject;
                Enemy enemy = go.AddComponent<Enemy>();
                enemy.enabled = true;
                enemy.enemyId = p.enemyId;
                enemy.enemyType = p.type;
                enemy.maxHealth = p.hp;
                enemy.curHealth = p.hp;
                enemy.ps = go.GetComponentInChildren<ParticleSystem>();

                //enemy.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                enemy.posVec = new Vector3(p.posX, p.posY, p.posZ);
                //Debug.Log("test enemy pos: " + new Vector3(p.posX, p.posY, p.posZ));
                _enemys.Add(p.enemyId, enemy);

                //Debug.Log("Monster 생성");
                //Debug.Log(enemy.enemyId);
                //Debug.Log(enemy.maxHealth);
            }
            else if (p.type == 5)
            {
                Object obj = Resources.Load("Monster_Dog");
                GameObject go = Object.Instantiate(obj) as GameObject;
                Enemy enemy = go.AddComponent<Enemy>();
                enemy.enabled = true;
                enemy.enemyId = p.enemyId;
                enemy.enemyType = p.type;
                enemy.maxHealth = p.hp;
                enemy.curHealth = p.hp;
                enemy.ps = go.GetComponentInChildren<ParticleSystem>();
                enemy.posVec = new Vector3(p.posX, p.posY, p.posZ);
                _enemys.Add(p.enemyId, enemy);

                //Debug.Log("Monster 생성");
                //Debug.Log(enemy.enemyId);
                //Debug.Log(enemy.maxHealth);
            }
            else if (p.type == 6)
            {
                Object obj = Resources.Load("Monster_Spider");
                GameObject go = Object.Instantiate(obj) as GameObject;
                Enemy enemy = go.AddComponent<Enemy>();
                enemy.enabled = true;
                enemy.enemyId = p.enemyId;
                enemy.enemyType = p.type;
                enemy.maxHealth = p.hp;
                enemy.curHealth = p.hp;
                enemy.ps = go.GetComponentInChildren<ParticleSystem>();
                enemy.posVec = new Vector3(p.posX, p.posY, p.posZ);
                _enemys.Add(p.enemyId, enemy);

                //Debug.Log("Monster 생성");
                //Debug.Log(enemy.enemyId);
                //Debug.Log(enemy.maxHealth);
            }
            else if (p.type == 7)
            {
                Object obj = Resources.Load("StageBoss");
                GameObject go = Object.Instantiate(obj) as GameObject;

                BossEnemy boss = go.AddComponent<BossEnemy>();
                boss.enabled = true;
                boss.enemyId = p.enemyId;
                boss.maxHealth = p.hp;
                boss.curHealth = p.hp;
                boss.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                _boss = boss;
                Debug.Log("보스 생성");

            }
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
            
            if(_myplayer.delay_body <=0f && _myplayer.delay_leg <= 0f)
            {
                //_myplayer.anim_Head.SetBool("isRun", _myplayer.moveVec != Vector3.zero);
                //_myplayer.anim_Body.SetBool("isRun", _myplayer.moveVec != Vector3.zero);
                //_myplayer.anim_Leg.SetBool("isRun", _myplayer.moveVec != Vector3.zero);
                _myplayer.anim.SetBool("isRun", _myplayer.moveVec != Vector3.zero);
            }
            

            if (packet.wDown)
            {
                if((int)_myplayer.body == 3)
                {
                    //_myplayer.StopCoroutine("Shot");
                    //_myplayer.anim.SetTrigger("doSwing");
                    //_myplayer.delay_body = 0.6f;


                    //_myplayer.anim.SetTrigger("doSwing");
                    //_myplayer.anim.SetTrigger("doSwing");
                    //_myplayer.delay_leg = 0.6f;
                    if (!_myplayer.anim.GetCurrentAnimatorStateInfo(0).IsName("Swing"))
                    {
                        _myplayer.anim.SetTrigger("doSwing");
                    }


                    //_myplayer.StartCoroutine("Shot");
                }
                else if((int)_myplayer.body == 2)
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
                if(player.delay_body <=0f && player.delay_leg <= 0f)
                {
                    player.anim.SetBool("isRun", player.moveVec2 != Vector3.zero);
                    //player.anim_Head.SetBool("isRun", player.moveVec2 != Vector3.zero);
                    //player.anim_Body.SetBool("isRun", player.moveVec2 != Vector3.zero);
                    //player.anim_Leg.SetBool("isRun", player.moveVec2 != Vector3.zero);
                }

                

                if (packet.wDown)
                {
                    if ((int)player.body == 3)
                    {
                        if (!player.anim.GetCurrentAnimatorStateInfo(0).IsName("Swing"))
                        {
                            player.anim.SetTrigger("doSwing");
                        }
                    }
                    else if((int)player.body == 2)
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

                if(enemy.enemyType != 4)
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
                enemy.transform.LookAt(enemy.posVec);
                if(enemy.tag == "EnemyTurret" )
                {
                    EnemyTurret enemyTurret = GameObject.Find("TargetArea").GetComponent<EnemyTurret>();
                    enemy.transform.LookAt(enemyTurret.targetPos);
                    //Debug.Log(enemyTurret.targetPos);
                }
            }
            // 보스 처리
            else if(_boss.enemyId == packet.playerId)
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

                if(packet.bossAttack == 1)
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

            //Object head = Resources.Load("Sp_Head_Parts");
            //Object body = Resources.Load("Sp_Body_Parts");
            //Object leg = Resources.Load("Sp_Leg_Parts");


            Player player = go.AddComponent<Player>();
            player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);

            player.PlayerId = packet.playerId;
            player.body = 3;
            player.anim = player.GetComponent<Animator>();
            //Joint_Robot jointP = go.AddComponent<Joint_Robot>();

            //jointP.po_list = new GameObject[3];
            //jointP.sh_list = new GameObject[3];
            //jointP.sp_list = new GameObject[3];

            //jointP.sp_list[0] = head as GameObject;
            //jointP.sp_list[1] = body as GameObject;
            //jointP.sp_list[2] = leg as GameObject;

            //jointP.leg = Object.Instantiate(jointP.sp_list[2], player.transform);
            //jointP.leg.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);

            //jointP.body = Object.Instantiate(jointP.sp_list[1], player.transform);
            //jointP.body.transform.position = jointP.leg.transform.position + jointP.leg.transform.Find("Joint_Leg").transform.localPosition - jointP.body.transform.Find("Joint_Leg").transform.localPosition;

            //jointP.head = Object.Instantiate(jointP.sp_list[0], player.transform);
            //jointP.head.transform.position = jointP.body.transform.position + jointP.body.transform.Find("Joint_Head").transform.position;

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

            //player.anim_Head = jointP.head.gameObject.transform.GetChild(0).GetComponent<Animator>();
            //player.anim_Body = jointP.body.gameObject.transform.GetChild(0).GetComponent<Animator>();
            //player.anim_Leg = jointP.leg.gameObject.transform.GetChild(0).GetComponent<Animator>();
            //player.intantBullet = Resources.Load("Bullet");
            //player.bulletPos = player.transform.GetChild(1);

            //_playerParts.Add(packet.playerId, jointP);
            _players.Add(packet.playerId, player);
            GameObject.Find("Game Manager").GetComponent<GameUIManager>().FindPlayerUI();
            Debug.Log(player.name);
        }
    }

    public void LeaveGame(S_BroadcastLeaveGame packet)
    {
        if(_myplayer.PlayerId == packet.playerId)
        {
            //GameObject.Destroy(_myplayer.gameObject);
            /*
            GameObject.Destroy(joint.head);
            GameObject.Destroy(joint.body);
            GameObject.Destroy(joint.leg);
            */
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
                    _playerParts.Remove(packet.playerId);
                }
            }
            else
            {
                Enemy enemy = null;
                Object items = null;
                switch (packet.itemNum)
                {
                    case 1:
                        items = Resources.Load("Items/Po_Body_Item");
                        break;
                    case 2:
                        items = Resources.Load("Items/Po_Head_Item");
                        break;
                    case 3:
                        items = Resources.Load("Items/Po_Leg_Item");
                        break;
                    case 4:
                        items = Resources.Load("Items/Sh_Body_Item");
                        break;
                    case 5:
                        items = Resources.Load("Items/Sh_Head_Item");
                        break;
                    case 6:
                        items = Resources.Load("Items/Sh_Leg_Item");
                        break;
                    case 7:
                        items = Resources.Load("Items/Sp_Body_Item");
                        break;
                    case 8:
                        items = Resources.Load("Items/Sp_Head_Item");
                        break;
                    case 9:
                        items = Resources.Load("Items/Sp_Leg_Item");
                        break;
                    default:
                        items = null;
                        break;
                }
                Debug.Log("itemNum: " + packet.itemNum);
                if(_enemys.TryGetValue(packet.playerId, out enemy))
                {
                    enemy.ps.Play();
                    GameObject.Destroy(enemy.gameObject, 0.6f);
                    _enemys.Remove(packet.playerId);
                    if (items != null)
                    {
                        item = Object.Instantiate(items) as GameObject;
                        Rigidbody rigidbody = item.GetComponent<Rigidbody>();
                        item.transform.position = new Vector3(enemy.transform.position.x, 5f, enemy.transform.position.z);
                        rigidbody.AddForce(new Vector3(7, 10, 0), ForceMode.Impulse);
                    }
                }
                else if(packet.playerId >= 1000)
                {
                    GameObject.Destroy(_boss.gameObject, 2);
                    _boss = null;
                }
            }
        }
    }

    public void ItemManager(S_Broadcast_Item packet)
    {
        if(packet.playerId == _myplayer.PlayerId)
        {
            joint.change_parts = packet.charactorType;
            if (packet.charactorType == 1)
            {
                switch(packet.itemType)
                {
                    case 1:
                        joint.po_list[0] = Resources.Load("Po_Head_Parts") as GameObject;
                        joint.SwitchParts(packet.itemType);
                        break;
                    case 2:
                        joint.sh_list[0] = Resources.Load("Sh_Head_Parts") as GameObject;
                        joint.SwitchParts(packet.itemType);
                        break;
                    case 3:
                        joint.sp_list[0] = Resources.Load("Sp_Head_Parts") as GameObject;
                        joint.SwitchParts(packet.itemType);
                        break;
                }
                _myplayer.anim_Head = joint.head.gameObject.transform.GetChild(0).GetComponent<Animator>();
                _myplayer.head = packet.itemType;
                //_myplayer.ps.Play();
                _myplayer.ps.Emit(100);
            }
            else if (packet.charactorType == 2)
            {
                switch (packet.itemType)
                {
                    case 1:
                        joint.po_list[1] = Resources.Load("Po_Body_Parts") as GameObject;
                        joint.SwitchParts(packet.itemType);
                        break;
                    case 2:
                        joint.sh_list[1] = Resources.Load("Sh_Body_Parts") as GameObject;
                        joint.SwitchParts(packet.itemType);
                        break;
                    case 3:
                        joint.sp_list[1] = Resources.Load("Sp_Body_Parts") as GameObject;
                        joint.SwitchParts(packet.itemType);
                        break;
                }
                _myplayer.anim_Body = joint.body.gameObject.transform.GetChild(0).GetComponent<Animator>();
                _myplayer.body = packet.itemType;
                //_myplayer.ps.Play();
                _myplayer.ps.Emit(100);
            }
            else if (packet.charactorType == 3)
            {
                switch (packet.itemType)
                {
                    case 1:
                        joint.po_list[2] = Resources.Load("Po_Leg_Parts") as GameObject;
                        joint.SwitchParts(packet.itemType);
                        break;
                    case 2:
                        joint.sh_list[2] = Resources.Load("Sh_Leg_Parts") as GameObject;
                        joint.SwitchParts(packet.itemType);
                        break;
                    case 3:
                        joint.sp_list[2] = Resources.Load("Sp_Leg_Parts") as GameObject;
                        joint.SwitchParts(packet.itemType);
                        break;
                }
                _myplayer.anim_Leg = joint.leg.gameObject.transform.GetChild(0).GetComponent<Animator>();
                _myplayer.leg = packet.itemType;
                //_myplayer.ps.Play();
                _myplayer.ps.Emit(100);
            }
        }
        else if (_players.TryGetValue(packet.playerId, out player))
        {
            if (_playerParts.TryGetValue(packet.playerId, out c_p_parts))
            {
                c_p_parts.change_parts = packet.charactorType;
                if (packet.charactorType == 1)
                {
                    switch (packet.itemType)
                    {
                        case 1:
                            c_p_parts.po_list[0] = Resources.Load("Po_Head_Parts") as GameObject;
                            c_p_parts.SwitchParts(packet.itemType);
                            break;
                        case 2:
                            c_p_parts.sh_list[0] = Resources.Load("Sh_Head_Parts") as GameObject;
                            c_p_parts.SwitchParts(packet.itemType);
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
                else if (packet.charactorType == 2)
                {
                    switch (packet.itemType)
                    {
                        case 1:
                            c_p_parts.po_list[1] = Resources.Load("Po_Body_Parts") as GameObject;
                            c_p_parts.SwitchParts(packet.itemType);
                            break;
                        case 2:
                            c_p_parts.sh_list[1] = Resources.Load("Sh_Body_Parts") as GameObject;
                            c_p_parts.SwitchParts(packet.itemType);
                            break;
                        case 3:
                            c_p_parts.sp_list[1] = Resources.Load("Sp_Body_Parts") as GameObject;
                            c_p_parts.SwitchParts(packet.itemType);
                            break;
                    }
                    player.anim_Body = c_p_parts.body.gameObject.transform.GetChild(0).GetComponent<Animator>();
                    player.body = packet.itemType;
                    player.ps.Emit(100);
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
    }

    public void AttackedMonster(S_AttackedMonster packet)
    {
        Enemy enemy = null;
        if (_enemys.TryGetValue(packet.id, out enemy))
        {
            enemy.curHealth = packet.hp;
        }
        else if(packet.id >= 1000)
        {
            _boss.curHealth = packet.hp;
            Debug.Log("보스 피격" + _boss.curHealth);
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
            player.hp = packet.hp;
        }

        Debug.Log("player hp: " + packet.hp);
    }
}
