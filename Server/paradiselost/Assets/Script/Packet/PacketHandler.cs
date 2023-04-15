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
			if (s_enterPacket.type == 0)
            {
				SceneManager.LoadScene("CharacterSelect");
			}
            else
            {
				SceneManager.LoadScene("Lobby");
				C_ENTER_GAME c_enterPacket = new C_ENTER_GAME();
				c_enterPacket.playerIndex = 0;
				ArraySegment<byte> segment = c_enterPacket.Write();
				serverSession.Send(segment);
			}
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
		S_PlayerList pkt = packet as S_PlayerList;
		ServerSession serverSession = session as ServerSession;

		//Debug.Log("");

		PlayerManager.Instance.Add(pkt);
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

		Debug.Log("아이템 먹은 플레이어" + pkt.playerId);
		Debug.Log("캐릭터 타입: " + pkt.charactorType);
		Debug.Log("아이템 타입: " + pkt.itemType);
	}
}
