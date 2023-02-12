#pragma once

enum
{
	C_Login = 0,
	C_Chat = 1,
	S_Chat = 2,
	S_ENTER_GAME = 3,
	C_ENTER_GAME = 4,
	S_MOVE = 5,
	C_MOVE = 6,
	S_PLAYERLIST = 7,
	S_BROADCASTENTER_GAME = 8,
	S_BROADCASTLEAVE_GAME = 9,
	S_BROADCAST_MOVE = 10
};

#pragma pack(push, 1)

struct PlayerList
{
	bool isSelf;
	int32 playerId;
	float posX;
	float posY;
	float posZ;
};
#pragma pack(pop)

class ServerPacketHandler
{
public:
	static void HandlePacket(PacketSessionRef& session, BYTE* buffer, int32 len);
	
	static bool Handle_C_Login(PacketSessionRef& session, BYTE* buffer, int32 len);
	static bool Handle_C_ENTER_GAME(PacketSessionRef& session, BYTE* buffer, int32 len);
	static bool Handle_C_MOVE(PacketSessionRef& session, BYTE* buffer, int32 len);

	static bool Handle_C_Chat(PacketSessionRef& session, BYTE* buffer, int32 len);
	
	static SendBufferRef Make_S_Chat(int32 id, wstring chat);
	static SendBufferRef Make_S_ENTER_GAME(bool success);
	static SendBufferRef Make_S_MOVE(int32 playerIndex, float x, float y, float z);
	static SendBufferRef Make_S_PlayerList(List<PlayerList> players);
	static SendBufferRef Make_S_BroadcastEnter_Game(int32 playerId, float posX, float posY, float posZ);
	static SendBufferRef Make_S_BroadcastLeave_Game(int32 playerId);
	static SendBufferRef Make_S_BroadcastMove(int32 playerId, float posX, float posY, float posZ);
};

