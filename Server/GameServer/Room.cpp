#include "pch.h"
#include "Room.h"
#include "Player.h"
#include "GameSession.h"
#include "ServerPacketHandler.h"

Room GRoom;

void Room::Enter(PlayerRef player)
{
	// 플레이어 추가
	WRITE_LOCK;
	_players[player->playerId] = player;

	List<PlayerList> players;
	PlayerList l_player;

	cout << "player ID: " << player->playerId << endl;

	// 몬스터 5명 생성 보내기
	int i = 0;
	for (auto iter = _monsters.begin(); iter != _monsters.end(); iter++)
	{
		l_player.isSelf = false;
		l_player.playerId = 500 + i;
		l_player.type = (int32)iter->type;
		l_player.hp = iter->hp;
		l_player.posX = iter->posX;
		l_player.posY = iter->posY;
		l_player.posZ = iter->posZ;

		players.push_back(l_player);
		i++;
	}
	i = 0;

	auto sendBufferM = ServerPacketHandler::Make_S_PlayerList(players);
	_players[player->playerId]->ownerSession->Send(sendBufferM);

	players.clear();
	for (auto iter = _players.begin(); iter != _players.end(); iter++)
	{
		if (iter->second->playerId == _players[player->playerId]->playerId)
			l_player.isSelf = true;
		else
			l_player.isSelf = false;
		
		l_player.playerId = iter->first;
		l_player.hp = iter->second->hp;
		l_player.type = (int32)iter->second->type;
		l_player.posX = 0.f;
		l_player.posY = 3.f;
		l_player.posZ = 0.f;

		players.push_back(l_player);
	}

	auto sendBufferP = ServerPacketHandler::Make_S_PlayerList(players);
	_players[player->playerId]->ownerSession->Send(sendBufferP);

	// 신입생 입장을 모두에게 알린다.
	auto sendBuffers = ServerPacketHandler::Make_S_BroadcastEnter_Game(player->playerId, (int32)player->type, player->xPos, player->yPos, player->zPos);
	BroadCast(sendBuffers);
	players.clear();
}

void Room::Leave(PlayerRef player)
{
	// 플레이어 제거
	WRITE_LOCK;
	_players.erase(player->playerId);

	// 모두에게 알린다.
	auto sendBuffer = ServerPacketHandler::Make_S_BroadcastLeave_Game(player->playerId);
	cout << "Leave ID: " << player->playerId << endl;
	BroadCast(sendBuffer);
}

void Room::DeadMonster(int32 monsterId)
{
	for (auto iter = _monsters.begin(); iter != _monsters.end(); iter++)
	{
		if (iter->playerId == monsterId)
			_monsters.erase(iter);
	}
	// 모두에게 알린다.
	auto sendBuffer = ServerPacketHandler::Make_S_BroadcastLeave_Game(monsterId);
	cout << "Dead Monster ID: " << monsterId << endl;
	BroadCast(sendBuffer);
}

void Room::AttackedMonster(int32 monsterId, uint16 hp)
{
	for (auto iter = _monsters.begin(); iter != _monsters.end(); iter++)
	{
		if (iter->playerId == monsterId)
		{
			iter->hp = hp;
			auto sendBuffer = ServerPacketHandler::Make_S_AttackedMonster(iter->playerId, iter->hp);
			cout << "Attacked Monster ID: " << iter->playerId << " hp: " << iter->hp << endl;
			BroadCast(sendBuffer);
		}
	}
}

void Room::BroadCast(SendBufferRef sendBuffer)
{
	WRITE_LOCK;
	for (auto& p : _players)
	{
		if (_players.empty())
			continue;
		p.second->ownerSession->Send(sendBuffer);
	}
}

void Room::CreateMonster()
{
	PlayerList l_player;

	for (int i = 0; i < MAX_MONSTER; i++)
	{
		l_player.isSelf = false;
		l_player.playerId = 500 + i;
		l_player.type = (int32)PlayerType::MONSTER;
		l_player.hp = 100;
		l_player.posX = 1.f;
		l_player.posY = 5.f;
		l_player.posZ = 1.f;

		GRoom._monsters.push_back(l_player);
	}
}
