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
	case C_Chat:
		Handle_C_Login(session, buffer, len);
		break;
	case C_ENTER_GAME:
		Handle_C_ENTER_GAME(session, buffer, len);
		break;
	case C_MOVE:
		Handle_C_MOVE(session, buffer, len);
		break;
	case C_Login:
		Handle_C_Chat(session ,buffer, len);
		break;
	case C_MONSTERATTACK:
		Handle_C_MonsterAttack(session, buffer, len);
		break;
	case C_MONSTERDEAD:
		Handle_C_MonsterDead(session, buffer, len);
		break;
	case C_ENTER_ITEM:
		Handle_C_Item(session, buffer, len);
		break;
	case C_PLAYERATTACK:
		Handle_C_PlayerAttacked(session, buffer, len);
		break;
	default:
		break;
	}
}

bool ServerPacketHandler::Handle_C_Login(PacketSessionRef& session, BYTE* buffer, int32 len)
{
	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);

	static Atomic<int32> idGenerator = 1;
	BufferReader br(buffer, len);

	PacketHeader header;
	br >> header;

	short strLen;
	br >> strLen;

	char name[256];

	for (int i = 0; i < strLen; i++)
	{
		br >> name[i];
	}
	
	char name_print[256];

	int t = 0;
	for (int i = 0; i < strLen; i++)
	{
		if (name[i] == '\0') {
		}
		else {
			name_print[t] = name[i];
			t++;
			name_print[t] = '\0';
		}
	}
	wstring wname(name_print, &name_print[sizeof(name_print)]);
	wcout << wname << endl;

	{
		PlayerRef playerRef = MakeShared<Player>();
		playerRef->playerId = idGenerator++;
		playerRef->hp = 100;
		playerRef->name = wname;
		playerRef->playerDir = 0;
		playerRef->type = PlayerType::NONE;
		playerRef->xPos = 19.0f;
		playerRef->yPos = 2.0f;
		playerRef->zPos = 19.0f;
		playerRef->ownerSession = gameSession;

		gameSession->_players.push_back(playerRef);

		//int num = 1;
		//auto it = find(gameSession->_players.begin(), gameSession->_players.end(), num);
		auto sendBuffer = Make_S_ENTER_GAME(true, (int32)playerRef->type);
		session->Send(sendBuffer);
	}
	
	return true;
}

// 로비로 왔을때 받는 패킷
bool ServerPacketHandler::Handle_C_ENTER_GAME(PacketSessionRef& session, BYTE* buffer, int32 len)
{
	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);
	
	BufferReader br(buffer, len);

	PacketHeader header;
	br >> header;

	int32 playerIndex, playerType;
	br >> playerIndex >> playerType;

	PlayerRef player = gameSession->_players[playerIndex];
	player->type = (PlayerType)playerType;

	if (playerType == PlayerType::AMOR)
	{
		gameSession->_players[0]->head = 2;
		gameSession->_players[0]->body = 2;
		gameSession->_players[0]->leg = 2;
	}
	else if (playerType == PlayerType::POWER)
	{
		gameSession->_players[0]->head = 1;
		gameSession->_players[0]->body = 1;
		gameSession->_players[0]->leg = 1;
	}
	else if (playerType == PlayerType::SPEED)
	{
		gameSession->_players[0]->head = 3;
		gameSession->_players[0]->body = 3;
		gameSession->_players[0]->leg = 3;
	}

	GRoom.Enter(player);

	cout << "ENTER GAME ID: " << gameSession->_players[playerIndex]->playerId << " "
		<< gameSession->_players[playerIndex]->head << " " << gameSession->_players[playerIndex]->body <<
		" " << gameSession->_players[playerIndex]->leg << endl;

	return true;
}

bool ServerPacketHandler::Handle_C_MOVE(PacketSessionRef& session, BYTE* buffer, int32 len)
{
	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);

	BufferReader br(buffer, len);

	PacketHeader header;
	br >> header;

	int32 id, dir;
	uint16 hp;
	float x, y, z;
	bool wDown, isJump;
	br >> id >> dir >> hp >> x >> y >> z >> wDown >> isJump;

	//cout << "ID: " << gameSession->_players[0]->playerId << " HP: " << gameSession->_players[0]->hp << endl;
	//cout << "Dir: " << dir << endl;

	PlayerRef player = gameSession->_players[0];

	gameSession->_players[0]->playerDir = dir;

	if (gameSession->_players[0]->dead == false) {
		if (isJump)
		{
			if (gameSession->_players[0]->yPos > 5)
				gameSession->_players[0]->isJump = false;
		}

		//cout << gameSession->_players[0]->isJump << endl;

		if (x > 20)
		{
			//gameSession->_players[0]->xPos = 19.f;
			gameSession->_players[0]->xPos = x;
			gameSession->_players[0]->yPos = y;
			gameSession->_players[0]->zPos = z;
			gameSession->_players[0]->wDown = wDown;
			gameSession->_players[0]->isJump = isJump;

			auto collisionMove = Make_S_MOVE(id, gameSession->_players[0]->xPos, gameSession->_players[0]->yPos, gameSession->_players[0]->zPos);

			//session->Send(collisionMove);
		}
		else if (x < -20)
		{
			//gameSession->_players[0]->xPos = -19.f;
			gameSession->_players[0]->xPos = x;
			gameSession->_players[0]->yPos = y;
			gameSession->_players[0]->zPos = z;
			gameSession->_players[0]->wDown = wDown;
			gameSession->_players[0]->isJump = isJump;

			auto collisionMove = Make_S_MOVE(id, gameSession->_players[0]->xPos, gameSession->_players[0]->yPos, gameSession->_players[0]->zPos);

			//session->Send(collisionMove);
		}
		else if (z > 20)
		{
			//gameSession->_players[0]->zPos = 19.f;
			gameSession->_players[0]->xPos = x;
			gameSession->_players[0]->yPos = y;
			gameSession->_players[0]->zPos = z;
			gameSession->_players[0]->wDown = wDown;
			gameSession->_players[0]->isJump = isJump;

			auto collisionMove = Make_S_MOVE(id, gameSession->_players[0]->xPos, gameSession->_players[0]->yPos, gameSession->_players[0]->zPos);

			//session->Send(collisionMove);
		}
		else if (z < -20)
		{
			//gameSession->_players[0]->zPos = -19.f;
			gameSession->_players[0]->xPos = x;
			gameSession->_players[0]->yPos = y;
			gameSession->_players[0]->zPos = z;
			gameSession->_players[0]->wDown = wDown;
			gameSession->_players[0]->isJump = isJump;

			auto collisionMove = Make_S_MOVE(id, gameSession->_players[0]->xPos, gameSession->_players[0]->yPos, gameSession->_players[0]->zPos);

			//session->Send(collisionMove);
		}
		if (y < -13.f)
		{
			gameSession->_players[0]->xPos = 19.f;
			//gameSession->_players[0]->yPos = 1.5f;
			gameSession->_players[0]->yPos = 2.f;
			gameSession->_players[0]->zPos = 19.f;

			auto collisionMove = Make_S_MOVE(id, gameSession->_players[0]->xPos, gameSession->_players[0]->yPos, gameSession->_players[0]->zPos);
			//cout << "ID: " << id << " POS: " << gameSession->_players[0]->xPos << " " << gameSession->_players[0]->yPos << " " << gameSession->_players[0]->zPos << " " << endl;
			session->Send(collisionMove);
		}
		else
		{
			gameSession->_players[0]->xPos = x;
			gameSession->_players[0]->yPos = y;
			gameSession->_players[0]->zPos = z;
			gameSession->_players[0]->wDown = wDown;
			gameSession->_players[0]->isJump = isJump;
		}
	}
	//cout << "ID: " << id << " POS: " << gameSession->_players[0]->xPos << " " << gameSession->_players[0]->yPos << " " << gameSession->_players[0]->zPos << " " << endl;

	auto sendBuffer = Make_S_BroadcastMove(gameSession->_players[0]->playerId,
		gameSession->_players[0]->playerDir,
		gameSession->_players[0]->hp,
		gameSession->_players[0]->xPos, gameSession->_players[0]->yPos,
		gameSession->_players[0]->zPos, gameSession->_players[0]->wDown,
		gameSession->_players[0]->isJump, 0);

	GRoom.BroadCast(sendBuffer);

	return true;
}

bool ServerPacketHandler::Handle_C_MonsterAttack(PacketSessionRef& session, BYTE* buffer, int32 len)
{
	BufferReader br(buffer, len);

	PacketHeader header;
	br >> header;

	int32 id;
	int16 hp;
	br >> id >> hp;
	if (hp > 1)
		GRoom.AttackedMonster(id, hp);
	else if (hp < 1)
		GRoom.DeadMonster(id);

	return true;
}

bool ServerPacketHandler::Handle_C_PlayerAttacked(PacketSessionRef& session, BYTE* buffer, int32 len)
{
	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);

	BufferReader br(buffer, len);

	PacketHeader header;
	br >> header;

	int32 p_id, m_id;
	br >> p_id >> m_id;
	
	PlayerRef player = gameSession->_players[0];

	cout << "player id: " << p_id << " monster id: " << m_id << endl;

	for (auto& m : GRoom._monsters)
	{
		if (m.playerId == m_id)
		{
			if (m.type == MONSTER1)
			{
				if (player->xPos <= m.posX + 13 && player->zPos <= m.posZ + 13
					&& player->xPos >= m.posX - 13 && player->zPos >= m.posZ - 13)
				{
					player->hp -= 10;
					cout << "player id: " << p_id << " monster id: " << m_id << " type: " << m.type << endl;
				}
			}
			else
			{
				if (player->xPos <= m.posX + 5 && player->zPos <= m.posZ + 5
					&& player->xPos >= m.posX - 5 && player->zPos >= m.posZ - 5)
				{
					player->hp -= 10;
					cout << "player id: " << p_id << " monster id: " << m_id << " type: " << m.type << endl;
				}
			}
		}
	}

	if (player->hp > 1)
	{
		auto sendBuffer = Make_S_AttackedPlayer(p_id, player->hp);
		GRoom.BroadCast(sendBuffer);
	}
	else if (player->hp < 1)
	{
		GRoom.PlayerDead(player);
	}

	return true;
}

bool ServerPacketHandler::Handle_C_MonsterDead(PacketSessionRef& session, BYTE* buffer, int32 len)
{
	BufferReader br(buffer, len);

	PacketHeader header;
	br >> header;

	int32 id;
	br >> id;
	GRoom.DeadMonster(id);

	return true;
}

bool ServerPacketHandler::Handle_C_Item(PacketSessionRef& session, BYTE* buffer, int32 len)
{
	GameSessionRef gameSession = static_pointer_cast<GameSession>(session);

	BufferReader br(buffer, len);

	PacketHeader header;
	br >> header;

	PlayerRef player = gameSession->_players[0];

	int32 id;
	uint16 charactorType, itemType;
	br >> id >> charactorType >> itemType;

	if (charactorType == 1)
	{
		player->head = itemType;
		
	}
	else if (charactorType == 2)
	{
		player->body = itemType;
	}
	else if (charactorType == 3)
	{
		player->leg = itemType;
	}

	cout << "player id: " << id << " charactorType: " << charactorType << " itemType: " << itemType << endl;
		
	auto sendBuffer = Make_S_BroadcastItem(id, charactorType, itemType);
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

SendBufferRef ServerPacketHandler::Make_S_ENTER_GAME(bool success, int32 type)
{
	SendBufferRef sendBuffer = GSendBufferManager->Open(4096);

	BufferWriter bw(sendBuffer->Buffer(), sendBuffer->AllocSize());

	PacketHeader* header = bw.Reserve<PacketHeader>();

	bw << success << type;

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
	
	bw << (uint16)(players.size());

	short strLen;
	for (PlayerList& p : players)
	{
		bw << (bool)p.isSelf << (int32)p.playerId << (int32)p.type << (uint16)p.hp << (float)p.posX << (float)p.posY << (float)p.posZ << p.head << p.body << p.leg;
		
		//bw.Write((void*)p.name.data(), p.name.size() * sizeof(WCHAR));

		cout << "name size: " << p.name.size() * sizeof(WCHAR) << endl;
	}

	//cout << sizeof(PlayerList) << endl;

	header->size = bw.WriteSize();
	header->id = S_PLAYERLIST;

	sendBuffer->Close(bw.WriteSize());

	//cout << "size: " << header->size << endl;

	return sendBuffer;
}

SendBufferRef ServerPacketHandler::Make_S_BroadcastEnter_Game(int32 playerId, int32 type ,float posX, float posY, float posZ)
{
	SendBufferRef sendBuffer = GSendBufferManager->Open(4096);

	BufferWriter bw(sendBuffer->Buffer(), sendBuffer->AllocSize());

	PacketHeader* header = bw.Reserve<PacketHeader>();

	bw <<  playerId << type << posX << posY << posZ;

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
	
	int item[10] = { 1,2,3,4,5,6,7,8,9,10 };

	if (playerId > 500)
	{
		int itemNum = rand() % 10;
		bw << item[itemNum];
	}

	header->size = bw.WriteSize();
	header->id = S_BROADCASTLEAVE_GAME;

	sendBuffer->Close(bw.WriteSize());

	return sendBuffer;
}

SendBufferRef ServerPacketHandler::Make_S_BroadcastMove(int32 playerId, int32 playerDir, uint16 hp, float posX, float posY, float posZ, bool wDown, bool isJump, short bossAttack)
{
	SendBufferRef sendBuffer = GSendBufferManager->Open(4096);

	BufferWriter bw(sendBuffer->Buffer(), sendBuffer->AllocSize());

	PacketHeader* header = bw.Reserve<PacketHeader>();

	bw << playerId << playerDir << hp << posX << posY << posZ << wDown << isJump << bossAttack;

	header->size = bw.WriteSize();
	header->id = S_BROADCAST_MOVE;

	sendBuffer->Close(bw.WriteSize());

	return sendBuffer;
}

SendBufferRef ServerPacketHandler::Make_S_BroadcastItem(int32 playerId, uint16 charactorType, uint16 itemType)
{
	SendBufferRef sendBuffer = GSendBufferManager->Open(4096);

	BufferWriter bw(sendBuffer->Buffer(), sendBuffer->AllocSize());

	PacketHeader* header = bw.Reserve<PacketHeader>();

	bw << playerId << charactorType << itemType;

	header->size = bw.WriteSize();
	header->id = S_BROADCAST_ITEM;

	sendBuffer->Close(bw.WriteSize());

	return sendBuffer;
}

SendBufferRef ServerPacketHandler::Make_S_AttackedMonster(int32 playerId, uint16 hp)
{
	SendBufferRef sendBuffer = GSendBufferManager->Open(4096);

	BufferWriter bw(sendBuffer->Buffer(), sendBuffer->AllocSize());

	PacketHeader* header = bw.Reserve<PacketHeader>();

	bw << playerId << hp;

	header->size = bw.WriteSize();
	header->id = S_ATTACKEDMONSTER;

	sendBuffer->Close(bw.WriteSize());

	return sendBuffer;
}

SendBufferRef ServerPacketHandler::Make_S_AttackedPlayer(int32 playerId, uint16 hp)
{
	SendBufferRef sendBuffer = GSendBufferManager->Open(4096);

	BufferWriter bw(sendBuffer->Buffer(), sendBuffer->AllocSize());

	PacketHeader* header = bw.Reserve<PacketHeader>();

	bw << playerId << hp;

	header->size = bw.WriteSize();
	header->id = S_PLAYERATTACK;

	sendBuffer->Close(bw.WriteSize());

	return sendBuffer;
}
