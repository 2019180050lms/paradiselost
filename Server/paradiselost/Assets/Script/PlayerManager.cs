using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    public MyPlayer _myplayer;
    Player player = null;
    Enemy enemy = null;
    public Joint_Robot joint;
    public BossEnemy _boss = null;
    Dictionary<int, Player> _players = new Dictionary<int, Player>();
    public Dictionary<int, Joint_Robot> _playerParts = new Dictionary<int, Joint_Robot>();
    Dictionary<int, Enemy> _enemys = new Dictionary<int, Enemy>();
    Joint_Robot c_p_parts = null;
    public Vector3 moveVec;
    public static PlayerManager Instance { get; } = new PlayerManager();

    void SetCharacter(int playerId, ushort hp, int playerType, bool isSelf, Vector3 pos)
    {
        
        
        if (isSelf)
        {
            Object obj = Resources.Load("Player_t1");
            GameObject go = Object.Instantiate(obj) as GameObject;
            Object obj2 = Resources.Load("PlayerPtr");
            GameObject PlayerPtr = Object.Instantiate(obj2) as GameObject;
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
                        joint.body.transform.position = joint.leg.transform.position + joint.leg.transform.Find("Joint_Leg").transform.localPosition - joint.body.transform.Find("Joint_Leg").transform.localPosition;

                        joint.head = Object.Instantiate(joint.po_list[0], myPlayer.transform);
                        joint.head.transform.position = joint.body.transform.position + joint.body.transform.Find("Joint_Head").transform.localPosition - joint.head.transform.Find("Joint_Head").transform.localPosition;

                        myPlayer.PlayerId = playerId;
                        myPlayer.hp = hp;
                        //myPlayer.name = name;
                        //Debug.Log(p.hp);

                        myPlayer.transform.position = new Vector3(pos.x, pos.y, pos.z);
                        _myplayer = myPlayer;
                        PlayerPtr.transform.SetParent(go.transform, false);

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
                        joint.head.transform.position = joint.body.transform.position + joint.body.transform.Find("Joint_Head").transform.localPosition - joint.head.transform.Find("Joint_Head").transform.localPosition;

                        myPlayer.PlayerId = playerId;
                        //myPlayer.name = p.name;
                        myPlayer.hp = hp;
                        //Debug.Log(p.hp);
                        myPlayer.transform.position = new Vector3(pos.x, pos.y, pos.z);
                        _myplayer = myPlayer;
                        PlayerPtr.transform.SetParent(go.transform, false);

                        Debug.Log(myPlayer.name);
                        break;
                    }
                case 3:
                    {
                        MyPlayer myPlayer = go.AddComponent<MyPlayer>();
                        Object head = Resources.Load("Sp_Head_Parts");
                        Object body = Resources.Load("Sp_Body_Parts");
                        Object leg = Resources.Load("Sp_Leg_Parts");

                        joint = go.AddComponent<Joint_Robot>();
                        joint.po_list = new GameObject[3];
                        joint.sh_list = new GameObject[3];
                        joint.sp_list = new GameObject[3];

                        joint.sp_list[0] = head as GameObject;
                        joint.sp_list[1] = body as GameObject;
                        joint.sp_list[2] = leg as GameObject;

                        joint.leg = Object.Instantiate(joint.sp_list[2], myPlayer.transform);
                        //leg.transform.position = new Vector3(-5, 5, 3);

                        joint.body = Object.Instantiate(joint.sp_list[1], myPlayer.transform);
                        joint.body.transform.position = joint.leg.transform.position + joint.leg.transform.Find("Joint_Leg").transform.localPosition - joint.body.transform.Find("Joint_Leg").transform.localPosition;

                        joint.head = Object.Instantiate(joint.sp_list[0], myPlayer.transform);
                        joint.head.transform.position = joint.body.transform.position + joint.body.transform.Find("Joint_Head").transform.localPosition - joint.head.transform.Find("Joint_Head").transform.localPosition;

                        myPlayer.PlayerId = playerId;
                        //myPlayer.name = p.name;
                        myPlayer.hp = hp;
                        //Debug.Log(p.hp);
                        myPlayer.transform.position = new Vector3(pos.x, pos.y, pos.z);
                        _myplayer = myPlayer;
                        PlayerPtr.transform.SetParent(go.transform, false);

                        Debug.Log(myPlayer.name);
                        break;
                    }
                default:
                    Debug.Log("오류");
                    break;
            }
        }
        else
        {
            Object obj = Resources.Load("Player_t1");
            GameObject go = Object.Instantiate(obj) as GameObject;
            switch (playerType)
            {
                case 1:
                    {
                        Player player = go.AddComponent<Player>();
                        Object head = Resources.Load("Po_Head_Parts");
                        Object body = Resources.Load("Po_Body_Parts");
                        Object leg = Resources.Load("Po_Leg_Parts");

                        player.PlayerId = playerId;
                        //player.name = p.name;
                        //player.transform.position = new Vector3(p.posX, p.posY, p.posZ);
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
                        jointP.head.transform.position = jointP.body.transform.position + jointP.body.transform.Find("Joint_Head").transform.localPosition - jointP.head.transform.Find("Joint_Head").transform.localPosition;

                        player.transform.position = new Vector3(pos.x, pos.y, pos.z);

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
                        //player.name = p.name;
                        //player.transform.position = new Vector3(p.posX, p.posY, p.posZ);
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
                        jointP.head.transform.position = jointP.body.transform.position + jointP.body.transform.Find("Joint_Head").transform.localPosition - jointP.head.transform.Find("Joint_Head").transform.localPosition;

                        player.transform.position = new Vector3(pos.x, pos.y, pos.z);

                        _playerParts.Add(playerId, jointP);
                        _players.Add(playerId, player);

                        Debug.Log(player.name);
                        break;
                    }
                case 3:
                    {
                        Player player = go.AddComponent<Player>();
                        Object head = Resources.Load("Sp_Head_Parts");
                        Object body = Resources.Load("Sp_Body_Parts");
                        Object leg = Resources.Load("Sp_Leg_Parts");

                        player.PlayerId = playerId;
                        //player.name = p.name;
                        //player.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                        Joint_Robot jointP = go.AddComponent<Joint_Robot>();

                        jointP.po_list = new GameObject[3];
                        jointP.sh_list = new GameObject[3];
                        jointP.sp_list = new GameObject[3];

                        jointP.sp_list[0] = head as GameObject;
                        jointP.sp_list[1] = body as GameObject;
                        jointP.sp_list[2] = leg as GameObject;

                        jointP.leg = Object.Instantiate(jointP.sp_list[2], player.transform);
                        //leg.transform.position = new Vector3(-5, 5, 3);

                        jointP.body = Object.Instantiate(jointP.sp_list[1], player.transform);
                        jointP.body.transform.position = jointP.leg.transform.position + jointP.leg.transform.Find("Joint_Leg").transform.localPosition - jointP.body.transform.Find("Joint_Leg").transform.localPosition;

                        jointP.head = Object.Instantiate(jointP.sp_list[0], player.transform);
                        jointP.head.transform.position = jointP.body.transform.position + jointP.body.transform.Find("Joint_Head").transform.localPosition - jointP.head.transform.Find("Joint_Head").transform.localPosition;

                        player.transform.position = new Vector3(pos.x, pos.y, pos.z);

                        _playerParts.Add(playerId, jointP);
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
            Vector3 playerPos = new Vector3(p.posX, p.posY, p.posZ);
            SetCharacter(p.playerId, p.hp, p.type, p.isSelf, playerPos);
            if (p.type == 5)
            {
                Object obj = Resources.Load("StageBoss");
                GameObject go = Object.Instantiate(obj) as GameObject;

                BossEnemy boss = go.AddComponent<BossEnemy>();
                boss.enemyId = p.playerId;
                boss.maxHealth = p.hp;
                boss.curHealth = p.hp;
                boss.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                _boss = boss;
                Debug.Log("보스 생성");

            }
            else if (p.type == 4)
            {
                Object obj = Resources.Load("Enemy");
                GameObject go = Object.Instantiate(obj) as GameObject;

                Enemy enemy = go.AddComponent<Enemy>();
                enemy.enemyId = p.playerId;
                enemy.maxHealth = p.hp;
                enemy.curHealth = p.hp;
                enemy.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                enemy.prevVec = new Vector3(p.posX, p.posY, p.posZ);
                _enemys.Add(p.playerId, enemy);

                //Debug.Log("Monster 생성");
                //Debug.Log(enemy.enemyId);
                //Debug.Log(enemy.maxHealth);
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
            Debug.Log(packet.isJump);
            _myplayer.testJump = packet.isJump;
            //_myplayer.transform.position = movePos;

            //Debug.Log(packet.hp);

            _myplayer.anim.SetBool("isRun", _myplayer.moveVec != Vector3.zero);
            if (packet.wDown)
            {
                _myplayer.StopCoroutine("Swing");
                //_myplayer.anim.SetTrigger("doSwing");
                _myplayer.StartCoroutine("Swing");
            }
            //_myplayer.transform.LookAt(_myplayer.transform.position + _myplayer.moveVec2);
        }
        else if (packet.playerId < 500)
        {
            if (_players.TryGetValue(packet.playerId, out player))
            {
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
                if (packet.wDown)
                    player.anim.SetTrigger("doSwing");
                player.transform.LookAt(player.transform.position + player.moveVec2);
            }
        }
        else
        {
            if (player != null)
                //player.anim.SetBool("isRun", false);
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

                enemy.moveVec2 = new Vector3(packet.posX, packet.posY, packet.posZ);

                //enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, new Vector3(packet.posX, packet.posY, packet.posZ), 1f);
                //Debug.Log(enemy.transform.position);
                //enemy.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
                enemy.posVec = new Vector3(packet.posX, packet.posY, packet.posZ);
                enemy.anim.SetBool("isWalk", enemy.isAttack != false);
                if (packet.wDown)
                {
                    //Debug.Log("attack !");
                    enemy.anim.SetTrigger("doAttack");
                }
                //enemy.transform.LookAt(enemy.transform.position + enemy.moveVec2);
                enemy.transform.LookAt(enemy.posVec);
            }
            // 보스 처리
            else if(_boss.enemyId == packet.playerId)
            {
                _boss.isAttack = packet.wDown;
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

                //enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, new Vector3(packet.posX, packet.posY, packet.posZ), 1f);
                //Debug.Log(enemy.transform.position);
                //enemy.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
                _boss.posVec = new Vector3(packet.posX, packet.posY, packet.posZ);
                //_boss.anim.SetBool("isWalk", _boss.isAttack != false);
                if (packet.wDown)
                {
                    _boss.anim.SetTrigger("doAttack");

                    //_boss.transform.LookAt();
                }
                //enemy.transform.LookAt(enemy.transform.position + enemy.moveVec2);
                _boss.transform.LookAt(_boss.posVec);
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

            Object head = Resources.Load("Po_Head_Parts");
            Object body = Resources.Load("Po_Body_Parts");
            Object leg = Resources.Load("Po_Leg_Parts");


            Player player = go.AddComponent<Player>();
            player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);

            player.PlayerId = packet.playerId;

            Joint_Robot jointP = go.AddComponent<Joint_Robot>();

            jointP.po_list = new GameObject[3];
            jointP.po_list[0] = head as GameObject;
            jointP.po_list[1] = body as GameObject;
            jointP.po_list[2] = leg as GameObject;

            jointP.leg = Object.Instantiate(jointP.po_list[2], player.transform);
            //leg.transform.position = new Vector3(-5, 5, 3);

            jointP.body = Object.Instantiate(jointP.po_list[1], player.transform);
            jointP.body.transform.position = jointP.leg.transform.position + jointP.leg.transform.Find("Joint_Leg").transform.localPosition - jointP.body.transform.Find("Joint_Leg").transform.localPosition;

            jointP.head = Object.Instantiate(jointP.po_list[0], player.transform);
            jointP.head.transform.position = jointP.body.transform.position + jointP.body.transform.Find("Joint_Head").transform.localPosition - jointP.head.transform.Find("Joint_Head").transform.localPosition;

            _playerParts.Add(packet.playerId, jointP);
            _players.Add(packet.playerId, player);

            Debug.Log(player.name);
        }
        else if (packet.type == 2)
        {
            Object obj = Resources.Load("Player2");
            GameObject go = Object.Instantiate(obj) as GameObject;

            Player player = go.AddComponent<Player>();
            player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
            _players.Add(packet.playerId, player);
        }
        else if (packet.type == 3)
        {
            Object obj = Resources.Load("Player3");
            GameObject go = Object.Instantiate(obj) as GameObject;

            Player player = go.AddComponent<Player>();
            player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
            _players.Add(packet.playerId, player);
        }
    }

    public void LeaveGame(S_BroadcastLeaveGame packet)
    {
        if(_myplayer.PlayerId == packet.playerId)
        {
            GameObject.Destroy(_myplayer.gameObject);
            _myplayer = null;
            joint = null;
        }
        else
        {
            if(packet.playerId < 500)
            {
                Player player = null;
                if (_players.TryGetValue(packet.playerId, out player))
                {
                    GameObject.Destroy(player.gameObject);
                    _players.Remove(packet.playerId);
                    _playerParts.Remove(packet.playerId);
                }
            }
            else
            {
                Enemy enemy = null;
                if(_enemys.TryGetValue(packet.playerId, out enemy))
                {
                    GameObject.Destroy(enemy.gameObject);
                    _enemys.Remove(packet.playerId);
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
                _myplayer.head = packet.itemType;
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
                _myplayer.body = packet.itemType;
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
                _myplayer.leg = packet.itemType;
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
                    player.head = packet.itemType;
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
                    player.body = packet.itemType;
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
                    player.leg = packet.itemType;
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
}
