#pragma once

enum
{
	C_Chat = 1,
	S_Chat = 2
};

class ClientPacketHandler
{
public:
	static void HandlePacket(PacketSessionRef& session, BYTE* buffer, int32 len);

	static void Handle_S_Chat(PacketSessionRef& session, BYTE* buffer, int32 len);

	static SendBufferRef Make_C_Chat(wstring chat);
};

