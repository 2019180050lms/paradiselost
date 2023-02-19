using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    MyPlayer _myplayer;
    Player player = null;
    Dictionary<int, Player> _players = new Dictionary<int, Player>();

    public static PlayerManager Instance { get; } = new PlayerManager();

    public void Add(S_PlayerList packet)
    {
        Object obj = Resources.Load("Player");

        foreach (S_PlayerList.Player p in packet.players)
        {
            GameObject go = Object.Instantiate(obj) as GameObject;

            if(p.isSelf)
            {
                MyPlayer myPlayer = go.AddComponent<MyPlayer>();
                myPlayer.PlayerId = p.playerId;
                myPlayer.hp = p.hp;
                Debug.Log(p.hp);
                myPlayer.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                _myplayer = myPlayer;
            }
            else
            {
                Player player = go.AddComponent<Player>();
                player.PlayerId = p.playerId;
                //player.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                _players.Add(p.playerId, player);
            }
        }
    }

    public void Move(S_BroadcastMove packet)
    {
        if(_myplayer.PlayerId == packet.playerId)
        {

            if (packet.playerDir == 0)
                _myplayer.moveVec2 = new Vector3(0, 0, 0);
            else if (packet.playerDir == 1)
                _myplayer.moveVec2 = new Vector3(1, 0, 0);
            else if (packet.playerDir == 2)
                _myplayer.moveVec2 = new Vector3(-1, 0, 0);
            else if (packet.playerDir == 3)
                _myplayer.moveVec2 = new Vector3(0, 0, 1);
            else if (packet.playerDir == 4)
                _myplayer.moveVec2 = new Vector3(0, 0, -1);
            else if (packet.playerDir == 5)
                _myplayer.moveVec2 = new Vector3(Mathf.Sqrt(0.5f), 0, Mathf.Sqrt(0.5f));
            else if (packet.playerDir == 6)
                _myplayer.moveVec2 = new Vector3(Mathf.Sqrt(0.5f), 0, -(Mathf.Sqrt(0.5f)));
            else if (packet.playerDir == 7)
                _myplayer.moveVec2 = new Vector3(-(Mathf.Sqrt(0.5f)), 0, Mathf.Sqrt(0.5f));
            else if (packet.playerDir == 8)
                _myplayer.moveVec2 = new Vector3(-(Mathf.Sqrt(0.5f)), 0, -(Mathf.Sqrt(0.5f)));

            Vector3 movePos = new Vector3(packet.posX, packet.posY, packet.posZ);
            _myplayer.moveVec = new Vector3(_myplayer.hAxis, 0, _myplayer.vAxis).normalized;
            
            /*
            if(packet.isJump && _myplayer.moveVec2 == Vector3.zero && !_myplayer.isJump && !_myplayer.isDodge && !_myplayer.isSwap)
            {
                _myplayer.rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
                _myplayer.anim.SetBool("isJump", true);
                _myplayer.anim.SetTrigger("doJump");
                _myplayer.isJump = true;
            }    
            */

            _myplayer.wDown = packet.wDown;
            _myplayer.transform.position = movePos;

            Debug.Log(packet.hp);

            _myplayer.anim.SetBool("isRun", _myplayer.moveVec2 != Vector3.zero);
            if (packet.wDown)
                _myplayer.anim.SetTrigger("doSwing");
            _myplayer.transform.LookAt(_myplayer.transform.position + _myplayer.moveVec2);
        }
        else
        {
            if(_players.TryGetValue(packet.playerId, out player))
            {
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

                Vector3 movePos = new Vector3(packet.posX, packet.posY, packet.posZ);
                player.moveVec = movePos;

                //player.moveVec = new Vector3(_myplayer.hAxis, 0, _myplayer.vAxis).normalized;
                
                player.wDown = packet.wDown;
                player.transform.position = movePos;

                Debug.Log(packet.hp);

                player.anim.SetBool("isRun", player.moveVec2 != Vector3.zero);
                if (packet.wDown)
                    player.anim.SetTrigger("doSwing");
                player.transform.LookAt(player.transform.position + player.moveVec2);
            }
        }
    }



    public void EnterGame(S_BroadcastEnterGame packet)
    {
        if (packet.playerId == _myplayer.PlayerId)
            return;

        Object obj = Resources.Load("Player");
        GameObject go = Object.Instantiate(obj) as GameObject;

        Player player = go.AddComponent<Player>();
        player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
        _players.Add(packet.playerId, player);
    }

    public void LeaveGame(S_BroadcastLeaveGame packet)
    {
        if(_myplayer.PlayerId == packet.playerId)
        {
            GameObject.Destroy(_myplayer.gameObject);
            _myplayer = null;
        }
        else
        {
            Player player = null;
            if(_players.TryGetValue(packet.playerId, out player))
            {
                GameObject.Destroy(player.gameObject);
                _players.Remove(packet.playerId);
            }
        }
    }
}
