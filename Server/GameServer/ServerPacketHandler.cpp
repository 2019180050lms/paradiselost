#include "pch.h"
#include "ServerPacketHandler.h"
#include "BufferReader.h"
#include "BufferWriter.h"
#include "Player.h"
#include "Room.h"
#include "GameSession.h"

void ServerPacketHandler::HandlePacket(PacketSessionRef& session, BYTE* buffer, int32 len)
{
	BufferReader br(buffer, len);

	PacketHeader header;
	br.Peek(&header);

	switch (header.id)
	{
	case C_Login:
		Handle_C_Login(session, buffer, len);
		break;
	case C_ENTER_GAME:
		Handle_C_ENTER_GAME(session, buffer, len);
		break;
	case C_MOVE:
		Handle_C_MOVE(session, buffer, len);
		break;
	case C_Chat:
		Handle_C_Chat(session ,buffer, len);
		break;
	default:
		break;
	}
}

bool ServerPacketHandler::Handle_C_Login(PacketSessionRef& session, BYTE* buffer, int32 len)
{
	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);

	static Atomic<int32> idGenerator = 1;

	{
		PlayerRef playerRef = MakeShared<Player>();
		playerRef->playerId = idGenerator++;
		playerRef->name = L"Test";
		playerRef->type = PlayerType::SPEED;
		playerRef->xPos = 1.0f;
		playerRef->yPos = 2.0f;
		playerRef->zPos = 3.0f;
		playerRef->ownerSession = gameSession;

		gameSession->_players.push_back(playerRef);

		//int num = 1;
		//auto it = find(gameSession->_players.begin(), gameSession->_players.end(), num);

	}

	auto sendBuffer = Make_S_ENTER_GAME(true);
	session->Send(sendBuffer);

	return true;
}

bool ServerPacketHandler::Handle_C_ENTER_GAME(PacketSessionRef& session, BYTE* buffer, int32 len)
{
	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);
	
	BufferReader br(buffer, len);

	PacketHeader header;
	br >> header;

	int32 playerIndex;
	br >> playerIndex;

	PlayerRef player = gameSession->_players[playerIndex];
	GRoom.Enter(player);

	cout << "ENTER GAME ID: " << gameSession->_players[playerIndex]->playerId << endl;

	return true;
}

bool ServerPacketHandler::Handle_C_MOVE(PacketSessionRef& session, BYTE* buffer, int32 len)
{
	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);

	BufferReader br(buffer, len);

	PacketHeader header;
	br >> header;

	int32 id;
	float x, y, z;
	br >> id >> x >> y >> z;


	PlayerRef player = gameSession->_players[0];

	gameSession->_players[0]->xPos = x;
	gameSession->_players[0]->yPos = y;
	gameSession->_players[0]->zPos = z;

	cout << "ID: " << id << "POS: " << x << " " << y << " " << z << endl;
	auto sendBuffer = Make_S_BroadcastMove(gameSession->_players[0]->playerId, gameSession->_players[0]->xPos, gameSession->_players[0]->yPos, gameSession->_players[0]->zPos);
	GRoom.BroadCast(sendBuffer);
	return true;
}

bool ServerPacketHandler::Handle_C_Chat(PacketSessionRef& session, BYTE* buffer, int32 len)
{
	BufferReader br(buffer, len);

	PacketHeader header;
	br >> header;

	
	wstring name;
	uint16 nameLen;
	br >> nameLen;
	name.resize(nameLen);

	br.Read((void*)name.data(), nameLen * sizeof(WCHAR));

	wcout.imbue(std::locale("kor"));
	wcout << (wstring)name << endl;

	return true;
}

SendBufferRef ServerPacketHandler::Make_S_Chat(int32 id, wstring chat)
{
	SendBufferRef sendBuffer = GSendBufferManager->Open(4096);

	BufferWriter bw(sendBuffer->Buffer(), sendBuffer->AllocSize());

	PacketHeader* header = bw.Reserve<PacketHeader>();

	bw << id;

	bw << (uint16)chat.size();
	bw.Write((void*)chat.data(), chat.size() * sizeof(WCHAR));

	header->size = bw.WriteSize();
	header->id = S_Chat; // 1 : Test Msg

	sendBuffer->Close(bw.WriteSize());

	return sendBuffer;
}

SendBufferRef ServerPacketHandler::Make_S_ENTER_GAME(bool success)
{
	SendBufferRef sendBuffer = GSendBufferManager->Open(4096);

	BufferWriter bw(sendBuffer->Buffer(), sendBuffer->AllocSize());

	PacketHeader* header = bw.Reserve<PacketHeader>();

	bw << success;

	header->size = bw.WriteSize();
	header->id = S_ENTER_GAME;

	sendBuffer->Close(bw.WriteSize());

	return sendBuffer;
}

SendBufferRef ServerPacketHandler::Make_S_MOVE(int32 playerIndex, float x, float y, float z)
{
	SendBufferRef sendBuffer = GSendBufferManager->Open(4096);

	BufferWriter bw(sendBuffer->Buffer(), sendBuffer->AllocSize());

	PacketHeader* header = bw.Reserve<PacketHeader>();

	bw << playerIndex << x << y << z;

	header->size = bw.WriteSize();
	header->id = S_MOVE;

	sendBuffer->Close(bw.WriteSize());

	return sendBuffer;
}

SendBufferRef ServerPacketHandler::Make_S_PlayerList(List<PlayerList> players)
{
	SendBufferRef sendBuffer = GSendBufferManager->Open(4096);

	BufferWriter bw(sendBuffer->Buffer(), sendBuffer->AllocSize());

	PacketHeader* header = bw.Reserve<PacketHeader>();

	//players = bw.Reserve<list<PlayerList>>();
	
	//uint16 size = players.size();
	//bw << size;

	 //PlayerList* player = bw.Reserve<PlayerList>();
	/*
	for (int32 i = 0; i < players.size(); i++)
	{
		bw << players. << players.playerId << players.posX << players.posY << players.posZ;
	}
	*/
	
	bw << (uint16)(players.size());

	for (PlayerList p : players)
	{
		bw << (bool)p.isSelf << (int32)p.playerId << (float)p.posX << (float)p.posY << (float)p.posZ;
	}

	cout << sizeof(PlayerList) << endl;

	header->size = bw.WriteSize();
	header->id = S_PLAYERLIST;

	sendBuffer->Close(bw.WriteSize());

	return sendBuffer;
}

SendBufferRef ServerPacketHandler::Make_S_BroadcastEnter_Game(int32 playerId, float posX, float posY, float posZ)
{
	SendBufferRef sendBuffer = GSendBufferManager->Open(4096);

	BufferWriter bw(sendBuffer->Buffer(), sendBuffer->AllocSize());

	PacketHeader* header = bw.Reserve<PacketHeader>();

	bw <<  playerId << posX << posY << posZ;

	header->size = bw.WriteSize();
	header->id = S_BROADCASTENTER_GAME;

	sendBuffer->Close(bw.WriteSize());

	return sendBuffer;
}

SendBufferRef ServerPacketHandler::Make_S_BroadcastLeave_Game(int32 playerId)
{
	SendBufferRef sendBuffer = GSendBufferManager->Open(4096);

	BufferWriter bw(sendBuffer->Buffer(), sendBuffer->AllocSize());

	PacketHeader* header = bw.Reserve<PacketHeader>();

	bw << playerId;

	header->size = bw.WriteSize();
	header->id = S_BROADCASTENTER_GAME;

	sendBuffer->Close(bw.WriteSize());

	return sendBuffer;
}

SendBufferRef ServerPacketHandler::Make_S_BroadcastMove(int32 playerId, float posX, float posY, float posZ)
{
	SendBufferRef sendBuffer = GSendBufferManager->Open(4096);

	BufferWriter bw(sendBuffer->Buffer(), sendBuffer->AllocSize());

	PacketHeader* header = bw.Reserve<PacketHeader>();

	bw << playerId << posX << posY << posZ;

	header->size = bw.WriteSize();
	header->id = S_BROADCAST_MOVE;

	sendBuffer->Close(bw.WriteSize());

	return sendBuffer;
}
