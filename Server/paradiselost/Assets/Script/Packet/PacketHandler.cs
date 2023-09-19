﻿using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

class PacketHandler
{
	public static void S_EnterGame(PacketSession session, IPacket packet)
	{
		S_ENTER_GAME s_enterPacket = packet as S_ENTER_GAME;
		ServerSession serverSession = session as ServerSession;

		if (s_enterPacket.success == true)
		{
			//SceneManager.LoadScene("InGame");
		}
		else
		{
			SceneManager.LoadScene("CharacterSelect");
		}			
	}

	public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
	{
		S_BroadcastEnterGame s_enterPacket = packet as S_BroadcastEnterGame;
		ServerSession serverSession = session as ServerSession;


		PlayerManager.Instance.EnterGame(s_enterPacket);
	}

	public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
	{
		S_BroadcastLeaveGame pkt = packet as S_BroadcastLeaveGame;
		ServerSession serverSession = session as ServerSession;

		PlayerManager.Instance.LeaveGame(pkt);
	}

	public static void S_Move(PacketSession session, IPacket packet)
	{
		S_Move movePacket = packet as S_Move;
		ServerSession serverSession = session as ServerSession;

		PlayerManager.Instance.CollisionMove(movePacket);

		Debug.Log("col");
	}

	public static void S_PlayerListHandler(PacketSession session, IPacket packet)
	{
		S_ENTER_PLAYER pkt = packet as S_ENTER_PLAYER;
		ServerSession serverSession = session as ServerSession;

		Debug.Log("들어옴");
		switch(pkt.stage)
        {
			case 0:
				{
					SceneManager.LoadScene("InGame");
					break;
                }
			case 1:
				{
					SceneManager.LoadScene("Stage1");
					break;
                }
			case 2:
				{
					SceneManager.LoadScene("Stage2");
					break;
                }
			case 3:
				{
					SceneManager.LoadScene("Stage3");
					break;
                }
            default:
                {
					Debug.Log("오류");
					break;
                }
        }

		PlayerManager.Instance.Add(pkt);
	}

    public static void S_EnemyListHandler(PacketSession session, IPacket packet)
    {
        S_EnemyList pkt = packet as S_EnemyList;
        ServerSession serverSession = session as ServerSession;

        //Debug.Log("");

        PlayerManager.Instance.EnemyAdd(pkt);
    }

    public static void S_BroadcastMoveHandler(PacketSession session, IPacket packet)
	{
		S_BroadcastMove pkt = packet as S_BroadcastMove;
		ServerSession serverSession = session as ServerSession;
		//Debug.Log("monsterId: " + pkt.playerDir + "pos: " + pkt.posX + " " + pkt.posZ);
		PlayerManager.Instance.Move(pkt);
	}

    public static void S_AttackedMonsterHandler(PacketSession session, IPacket packet)
    {
        S_AttackedMonster pkt = packet as S_AttackedMonster;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.AttackedMonster(pkt);
    }

    public static void S_BossAttackedHandler(PacketSession session, IPacket packet)
    {
        S_BOSS_Attack pkt = packet as S_BOSS_Attack;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.BossAttack(pkt);
    }

    public static void S_AttackedPlayerHandler(PacketSession session, IPacket packet)
	{
		S_AttackedPlayer pkt = packet as S_AttackedPlayer;
		ServerSession serverSession = session as ServerSession;

		PlayerManager.Instance.AttackedPlayer(pkt);
	}

	public static void S_NpcHandler(PacketSession session, IPacket packet)
	{
		S_Npc pkt = packet as S_Npc;
		ServerSession serverSession = session as ServerSession;

		//Debug.Log("");

		PlayerManager.Instance.NPCManager(pkt);
	}

	public static void S_ChatHandler(PacketSession session, IPacket packet)
	{
		S_Chat chatPacket = packet as S_Chat;
		ServerSession serverSession = session as ServerSession;

		//if (chatPacket.playerId == 1)
		{
            //Debug.Log(chatPacket.chat);

			//GameObject go = GameObject.Find("Player");
			
			//if(go == null)
            //    Debug.Log("Player not found");
			//else
            //    Debug.Log("Player found");
        }
		//if (chatPacket.playerId == 1)
			//Console.WriteLine(chatPacket.chat);
	}

	public static void S_BroadCastItem(PacketSession session, IPacket packet)
	{
		S_Broadcast_Item pkt = packet as S_Broadcast_Item;
		ServerSession serverSession = session as ServerSession;

		PlayerManager.Instance.ItemManager(pkt);

		Debug.Log("아이템 먹은 플레이어 " + pkt.playerId + " 아이템 타입: " + pkt.charactorType + " 아이템 값: " + pkt.itemType);
	}

	public static void S_StageClearHandler(PacketSession session, IPacket packet)
	{
		S_StageClear pkt = packet as S_StageClear;
		ServerSession serverSession = session as ServerSession;

		//Debug.Log("");

		PlayerManager.Instance.StageClearManager(pkt);
	}

    public static void S_SetActiveObjectHandler(PacketSession session, IPacket packet)
    {
        S_SETACTIVE_OBJECT pkt = packet as S_SETACTIVE_OBJECT;
        ServerSession serverSession = session as ServerSession;

        //Debug.Log("");

        PlayerManager.Instance.SetActiveObjectManager(pkt);
    }
}
