using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ServerCore;
using UnityEngine;

public enum PacketID
{
	C_Chat = 0,
	C_Login = 1,
    S_Chat = 2,
    S_ENTER_GAME = 3,
    C_ENTER_GAME = 4,
    S_MOVE = 5,
    C_MOVE = 6,
    S_PLAYERLIST = 7,
    S_BROADCASTENTER_GAME = 8,
    S_BROADCASTLEAVE_GAME = 9,
    S_BROADCAST_MOVE = 10,
    C_MAKEROOM = 11,
    C_MONSTERATTACK = 12,
    C_MONSTERDEAD = 13,
    S_ATTACKEDMONSTER = 14,
	S_BROADCAST_ITEM = 15,
	C_ENTER_ITEM = 16,
	C_PLAYERATTACK = 24,
	S_PLAYERATTACK = 18,
	S_ENEMYLIST = 19,
	S_BOSSATTACK = 20,
	S_NPC = 21,
	C_NPC = 17,
	C_PORTAL = 23,
	S_STAGE_CLEAR = 22,
	C_MONSTER_AI = 25,
    C_CLEAR_AND_FAIL = 26,
	S_SETACTIVE_OBJECT = 27
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}

class C_Login : IPacket
{
	public ushort Protocol { get { return (ushort)PacketID.C_Chat; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Login), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

class C_Chat : IPacket
{
    public bool ml_client;
    public string chat;

	public ushort Protocol { get { return (ushort)PacketID.C_Chat; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort chatLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, chatLen);
		count += chatLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Chat), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort); 
		Array.Copy(BitConverter.GetBytes(this.ml_client), 0, segment.Array, segment.Offset + count, sizeof(bool));
        count += sizeof(bool);
        ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(chatLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += chatLen;

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

class S_Chat : IPacket
{
	public int playerId;
	public string chat;

	public ushort Protocol { get { return (ushort)PacketID.S_Chat; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		ushort chatLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, chatLen);
		count += chatLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_Chat), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(chatLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += chatLen;

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_ENTER_GAME : IPacket
{
	public bool success;
	public int type; 

	public ushort Protocol { get { return (ushort)PacketID.S_ENTER_GAME; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.success = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		this.type = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_Chat), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.success), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		Array.Copy(BitConverter.GetBytes(this.type), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_SETACTIVE_OBJECT : IPacket
{
    public int stage;

    public ushort Protocol { get { return (ushort)PacketID.S_SETACTIVE_OBJECT; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.stage = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_Chat), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.stage), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_Clear_Fail : IPacket
{
    public int type;	// 0 : MONSTER, 1 : KEY

    public ushort Protocol { get { return (ushort)PacketID.C_CLEAR_AND_FAIL; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.type = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_CLEAR_AND_FAIL), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.type), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_ADD_PLAYER : IPacket
{
	public short id;
	public int type;
	public float x, y, z;
	public string name;

	public ushort Protocol { get { return (ushort)PacketID.S_ENTER_GAME; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.id = BitConverter.ToInt16(segment.Array, segment.Offset + count);
		count += sizeof(short);
		this.type = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.x = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.y = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.z = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, 20);
		count += 20;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_Chat), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.type), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_ENTER_PLAYER : IPacket
{
	public short id;
	public int type;
	public short hp;
	public float x, y, z;
	public int weapon;
	public int stage;
	public string name;

	public ushort Protocol { get { return (ushort)PacketID.S_PLAYERLIST; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.id = BitConverter.ToInt16(segment.Array, segment.Offset + count);
		count += sizeof(short);
		this.type = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.weapon = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.hp = BitConverter.ToInt16(segment.Array, segment.Offset + count);
		count += sizeof(short);
		this.x = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.y = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.z = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.stage = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		//ushort chatLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		//count += sizeof(ushort);
		this.name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, 10);
		count += 10;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_Chat), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.type), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

class C_CREATE_PLAYER : IPacket
{
	public int type;
	public string nickname;

	public ushort Protocol { get { return (ushort)PacketID.C_ENTER_GAME; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_ENTER_GAME), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);;
        Array.Copy(BitConverter.GetBytes(this.type), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);
		if (nickname != null)
		{
			ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.nickname, 0, this.nickname.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(chatLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += chatLen;
		}
		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_Move : IPacket
{
	public int playerIndex;
	public int playerDir;
	public short hp;
	public float posX;
	public float posY;
	public float posZ;
	public bool wDown;
	public bool isJump;

	public ushort Protocol { get { return (ushort)PacketID.C_MOVE; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.playerIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.playerDir), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.hp), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.wDown = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		this.isJump = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_MOVE), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort); 
		Array.Copy(BitConverter.GetBytes(this.playerIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.playerDir), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.hp), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.wDown), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		Array.Copy(BitConverter.GetBytes(this.isJump), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_Move_AI : IPacket
{
	public int playerIndex;
	public int playerDir;
	public short hp;
	public float posX;
	public float posY;
	public float posZ;
	public bool wDown;
	public bool isJump;

	public ushort Protocol { get { return (ushort)PacketID.C_MONSTER_AI; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.playerIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.playerDir), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.hp), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.wDown = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		this.isJump = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_MONSTER_AI), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.playerIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.playerDir), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.hp), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.wDown), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		Array.Copy(BitConverter.GetBytes(this.isJump), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_BroadcastMove : IPacket
{
	public int playerId;
	public int playerDir;
	public ushort hp;
	public float posX;
	public float posY;
	public float posZ;
	public bool wDown;
	public bool isJump;
	public short bossAttack;

	public ushort Protocol { get { return (ushort)PacketID.S_BROADCAST_MOVE; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.playerDir = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.hp = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.wDown = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		this.isJump = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		this.bossAttack = BitConverter.ToInt16(segment.Array, segment.Offset + count);
		count += sizeof(short);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BROADCAST_MOVE), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.playerDir), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.hp), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.wDown), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		Array.Copy(BitConverter.GetBytes(this.isJump), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		Array.Copy(BitConverter.GetBytes(this.bossAttack), 0, segment.Array, segment.Offset + count, sizeof(short));
		count += sizeof(short);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_BroadcastEnterGame : IPacket
{
	public int playerId;
	public int type;
	public short hp;
	public float posX;
	public float posY;
	public float posZ;
	public short head_item;
	public short weapon_item;
	public short leg_item;
	public int stage;
	public string name;

	public ushort Protocol { get { return (ushort)PacketID.S_BROADCASTENTER_GAME; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
        this.type = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);
		this.hp = BitConverter.ToInt16(segment.Array, segment.Offset + count);
		count += sizeof(short);
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.head_item = BitConverter.ToInt16(segment.Array, segment.Offset + count);
		count += sizeof(short);
		this.weapon_item = BitConverter.ToInt16(segment.Array, segment.Offset + count);
		count += sizeof(short);
		this.leg_item = BitConverter.ToInt16(segment.Array, segment.Offset + count);
		count += sizeof(short);
        this.stage = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);
        this.name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, 10);
		count += 10;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BROADCASTENTER_GAME), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        Array.Copy(BitConverter.GetBytes(this.type), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);
        Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}


public class S_Move : IPacket
{
	public int playerIndex;
	public float posX;
	public float posY;
	public float posZ;

	public ushort Protocol { get { return (ushort)PacketID.S_MOVE; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.playerIndex = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_MOVE), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.playerIndex), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_BroadcastLeaveGame : IPacket
{
	public int playerId;
	public int itemNum;

	public ushort Protocol { get { return (ushort)PacketID.S_BROADCASTLEAVE_GAME; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.itemNum = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BROADCASTLEAVE_GAME), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.itemNum), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_PlayerList : IPacket
{
	public class Player
	{
		public bool isSelf;
		public int playerId;
		public int type;
		public ushort hp;
		public float posX;
		public float posY;
		public float posZ;
		public ushort head;
		public ushort body;
		public ushort leg;
		public string name;

		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			this.isSelf = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.type = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.hp = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.head = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.body = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.leg = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			//ushort chatLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			//count += sizeof(ushort);
			//this.name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, chatLen);
			//count += chatLen;
		}

		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			Array.Copy(BitConverter.GetBytes(this.isSelf), 0, segment.Array, segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			Array.Copy(BitConverter.GetBytes(this.type), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			Array.Copy(BitConverter.GetBytes(this.hp), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			return success;
		}
	}
	public List<Player> players = new List<Player>();

	public ushort Protocol { get { return (ushort)PacketID.S_PLAYERLIST; } }

	// [packetheader][struct struct struct]
	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.players.Clear();
		ushort playerLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < playerLen; i++)
		{
			Player player = new Player();
			player.Read(segment, ref count);
			//Debug.Log("check");
			players.Add(player);
		}
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_PLAYERLIST), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)this.players.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (Player player in this.players)
			player.Write(segment, ref count);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}


public class S_EnemyList : IPacket
{
    public class Enemy
    {
        public int enemyId;
        public int type;
        public ushort hp;
        public float posX;
        public float posY;
        public float posZ;
		public bool isAttack;

        public void Read(ArraySegment<byte> segment, ref ushort count)
        {
            this.enemyId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);
            this.type = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);
            this.hp = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
            this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);
			this.isAttack = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
            //ushort chatLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            //count += sizeof(ushort);
            //this.name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, chatLen);
            //count += chatLen;
        }

        public bool Write(ArraySegment<byte> segment, ref ushort count)
        {
            bool success = true;
            Array.Copy(BitConverter.GetBytes(this.enemyId), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);
            Array.Copy(BitConverter.GetBytes(this.type), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);
            Array.Copy(BitConverter.GetBytes(this.hp), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);
            return success;
        }
    }
    public List<Enemy> enemys = new List<Enemy>();

    public ushort Protocol { get { return (ushort)PacketID.S_ENEMYLIST; } }

    // [packetheader][struct struct struct]
    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.enemys.Clear();
        ushort enemyLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        for (int i = 0; i < enemyLen; i++)
        {
            Enemy enemy = new Enemy();
            enemy.Read(segment, ref count);
            //Debug.Log("check");
            enemys.Add(enemy);
        }
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_ENEMYLIST), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)this.enemys.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        foreach (Enemy enemy in this.enemys)
            enemy.Write(segment, ref count);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_AttackedMonster : IPacket
{
    public int id;
    public short hp;

    public ushort Protocol { get { return (ushort)PacketID.S_ATTACKEDMONSTER; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.id = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);
        this.hp = BitConverter.ToInt16(segment.Array, segment.Offset + count);
        count += sizeof(short);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_ATTACKEDMONSTER), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.id), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);
        Array.Copy(BitConverter.GetBytes(this.hp), 0, segment.Array, segment.Offset + count, sizeof(short));
        count += sizeof(short);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class S_BOSS_Attack : IPacket
{
    public int targetid;
	public short bossAttack;

    public ushort Protocol { get { return (ushort)PacketID.S_BOSSATTACK; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.targetid = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);
        this.bossAttack = BitConverter.ToInt16(segment.Array, segment.Offset + count);
        count += sizeof(short);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BOSSATTACK), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.targetid), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);
        Array.Copy(BitConverter.GetBytes(this.bossAttack), 0, segment.Array, segment.Offset + count, sizeof(short));
        count += sizeof(short);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_Item : IPacket
{
	public int playerId;
	public ushort charactorType;
	public ushort itemType;

	public ushort Protocol { get { return (ushort)PacketID.C_ENTER_ITEM; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.charactorType = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.itemType = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_ENTER_ITEM), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.charactorType), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.itemType), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_Broadcast_Item : IPacket
{
	public int playerId;
	public ushort charactorType;
	public ushort itemType;

	public ushort Protocol { get { return (ushort)PacketID.S_BROADCAST_ITEM; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.charactorType = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.itemType = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BROADCAST_ITEM), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.itemType), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_AttackedMonster : IPacket
{
    public int id;
    public short hp;
	public int playerId;
	public bool hitEnemy;

    public ushort Protocol { get { return (ushort)PacketID.C_MONSTERATTACK; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.id = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);
        this.hp = BitConverter.ToInt16(segment.Array, segment.Offset + count);
        count += sizeof(short);
        this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);
		this.hitEnemy = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
	}

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_MONSTERATTACK), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.id), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);
        Array.Copy(BitConverter.GetBytes(this.hp), 0, segment.Array, segment.Offset + count, sizeof(short));
        count += sizeof(short);
        Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.hitEnemy), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class C_AttackedPlayer : IPacket
{
	public int p_id;
	public int m_id;

	public ushort Protocol { get { return (ushort)PacketID.C_PLAYERATTACK; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.p_id = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.m_id = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_PLAYERATTACK), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.p_id), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.m_id), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_AttackedPlayer : IPacket
{
	public int p_id;
	public short hp;

	public ushort Protocol { get { return (ushort)PacketID.S_PLAYERATTACK; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.p_id = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.hp = BitConverter.ToInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_PLAYERATTACK), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.p_id), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.hp), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_Npc : IPacket
{
	public short stage;
	public bool active;
	public short monster_count;

	public ushort Protocol { get { return (ushort)PacketID.S_NPC; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.stage = BitConverter.ToInt16(segment.Array, segment.Offset + count);
		count += sizeof(short);
		this.active = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		this.monster_count = BitConverter.ToInt16(segment.Array, segment.Offset + count);
		count += sizeof(short);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_Chat), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.stage), 0, segment.Array, segment.Offset + count, sizeof(short));
		count += sizeof(short);
		Array.Copy(BitConverter.GetBytes(this.active), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		Array.Copy(BitConverter.GetBytes(this.monster_count), 0, segment.Array, segment.Offset + count, sizeof(short));
		count += sizeof(short);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_Npc : IPacket
{
	public bool active;
	public short _quest_stage;

	public ushort Protocol { get { return (ushort)PacketID.C_NPC; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.active = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
		count += sizeof(bool);
		this._quest_stage = BitConverter.ToInt16(segment.Array, segment.Offset + count);
		count += sizeof(short);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_NPC), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.active), 0, segment.Array, segment.Offset + count, sizeof(bool));
		count += sizeof(bool);
		Array.Copy(BitConverter.GetBytes(this._quest_stage), 0, segment.Array, segment.Offset + count, sizeof(short));
		count += sizeof(short);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_Portal : IPacket
{
	public int stage;

	public ushort Protocol { get { return (ushort)PacketID.C_PORTAL; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.stage = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_PORTAL), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.stage), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_StageClear : IPacket
{
	public int stage;
	public int item;

	public ushort Protocol { get { return (ushort)PacketID.S_STAGE_CLEAR; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.stage = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.item = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_STAGE_CLEAR), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.stage), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.item), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}