using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
	Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
		
	public void Register()
	{
		_makeFunc.Add((ushort)PacketID.S_MOVE, MakePacket<S_Move>);
		_handler.Add((ushort)PacketID.S_MOVE, PacketHandler.S_Move);

		_makeFunc.Add((ushort)PacketID.S_ENTER_GAME, MakePacket<S_ENTER_GAME>);
		_handler.Add((ushort)PacketID.S_ENTER_GAME, PacketHandler.S_EnterGame);

		_makeFunc.Add((ushort)PacketID.S_PLAYERLIST, MakePacket<S_ENTER_PLAYER>);
		_handler.Add((ushort)PacketID.S_PLAYERLIST, PacketHandler.S_PlayerListHandler);

        _makeFunc.Add((ushort)PacketID.S_ENEMYLIST, MakePacket<S_EnemyList>);
        _handler.Add((ushort)PacketID.S_ENEMYLIST, PacketHandler.S_EnemyListHandler);

        _makeFunc.Add((ushort)PacketID.S_Chat, MakePacket<S_Chat>);
		_handler.Add((ushort)PacketID.S_Chat, PacketHandler.S_ChatHandler);

		_makeFunc.Add((ushort)PacketID.S_BROADCASTENTER_GAME, MakePacket<S_BroadcastEnterGame>);
		_handler.Add((ushort)PacketID.S_BROADCASTENTER_GAME, PacketHandler.S_BroadcastEnterGameHandler);

		_makeFunc.Add((ushort)PacketID.S_BROADCASTLEAVE_GAME, MakePacket<S_BroadcastLeaveGame>);
		_handler.Add((ushort)PacketID.S_BROADCASTLEAVE_GAME, PacketHandler.S_BroadcastLeaveGameHandler);

		_makeFunc.Add((ushort)PacketID.S_BROADCAST_MOVE, MakePacket<S_BroadcastMove>);
		_handler.Add((ushort)PacketID.S_BROADCAST_MOVE, PacketHandler.S_BroadcastMoveHandler);

        _makeFunc.Add((ushort)PacketID.S_ATTACKEDMONSTER, MakePacket<S_AttackedMonster>);
        _handler.Add((ushort)PacketID.S_ATTACKEDMONSTER, PacketHandler.S_AttackedMonsterHandler);

		_makeFunc.Add((ushort)PacketID.S_PLAYERATTACK, MakePacket<S_AttackedPlayer>);
		_handler.Add((ushort)PacketID.S_PLAYERATTACK, PacketHandler.S_AttackedPlayerHandler);

		_makeFunc.Add((ushort)PacketID.S_BROADCAST_ITEM, MakePacket<S_Broadcast_Item>);
		_handler.Add((ushort)PacketID.S_BROADCAST_ITEM, PacketHandler.S_BroadCastItem);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
		if (_makeFunc.TryGetValue(id, out func))
		{
            IPacket packet = func.Invoke(session, buffer);
			if (onRecvCallback != null)
				onRecvCallback.Invoke(session, packet);
			else
				HandlePacket(session, packet);
        }
	}

	T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
	{
		T pkt = new T();
		pkt.Read(buffer);
		return pkt;
	}

	public void HandlePacket(PacketSession session, IPacket packet)
	{
        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(packet.Protocol, out action))
            action.Invoke(session, packet);
    }
}