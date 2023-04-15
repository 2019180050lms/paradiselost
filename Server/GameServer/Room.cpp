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
		l_player.head = 0;
		l_player.body = 0;
		l_player.leg = 0;

		players.push_back(l_player);
		i++;
	}
	i = 0;

	auto sendBufferM = ServerPacketHandler::Make_S_PlayerList(_monsters);
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
		l_player.posX = 10.f;
		l_player.posY = 3.f;
		l_player.posZ = 10.f;
		l_player.head = iter->second->head;
		l_player.body = iter->second->body;
		l_player.leg = iter->second->leg;
		//l_player.name = iter->second->name;

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
		if (iter->playerId == monsterId) {
			WRITE_LOCK;
			_monsters.erase(iter);
			break;
		}
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

void Room::CreateMonster(float x, float y, float z)
{
	PlayerList l_player;

	for (int i = 0; i < MAX_MONSTER; i++)
	{
		l_player.isSelf = false;
		l_player.playerId = 500 + i;
		if (i < 2)
			l_player.type = (int32)MonsterType::MONSTER1;
		else if(i == 3)
			l_player.type = (int32)MonsterType::MONSTER2;
		else if(i == 4)
			l_player.type = (int32)MonsterType::MONSTER3;
		l_player.hp = 100;
		l_player.posX = x;
		if (l_player.type == MONSTER2)
		{
			l_player.posY = y + 2;
		}
		else
			l_player.posY = y;

		l_player.posZ = z;
		GRoom._monsters.push_back(l_player);
	}
}

PlayerList Room::CreateBossMonster()
{
	PlayerList l_player;

	l_player.isSelf = false;
	l_player.playerId = 1000;
	l_player.Dir = 0;
	l_player.type = (int32)BossType::BOSS1;
	l_player.hp = 1000;   
	l_player.posX = 246.757f;
	l_player.posY = -7.6f;
	l_player.posZ = 1.53712f;
	GRoom._monsters.push_back(l_player);

	return l_player;
}

void Room::MoveMonster()
{
	if (_monsters.size() == 0)
	{
		if (stage == 0) {
			CreateMonster(106.f, 1.5f, 1.5f);
			auto monster = ServerPacketHandler::Make_S_PlayerList(_monsters);
			BroadCast(monster);
			stage += 1;
		}
		else if (stage == 1)
		{
			List<PlayerList> l_boss;
			PlayerList boss = CreateBossMonster();
			l_boss.emplace_back(boss);
			auto bossSend = ServerPacketHandler::Make_S_PlayerList(l_boss);
			BroadCast(bossSend);
			cout << "send boss" << endl;
			stage = 0;
		}
	}

	for (auto& m : _monsters)
	{
		for (auto& p : _players)
		{
			if (p.second->playerId < 500)
			{
				if (p.second->xPos <= m.posX + 5 && p.second->zPos <= m.posZ + 5
					&& p.second->xPos >= m.posX - 5 && p.second->zPos >= m.posZ - 5)
				{
					//cout << "p id: " << p.second->playerId << " m id: " << m.playerId << " attack: " << m.wDown << endl;
					m.wDown = true;
					break;
				}
				else
					m.wDown = false;
			}
		}
		uint16 randDir = rand() % 9;
		if (!m.wDown)
		{
			if (randDir == 0)
			{
				continue;
			}
			else if (randDir == 1)
			{
				if (m.posX > 20 && stage == 0)
					continue;
				m.Dir = randDir;
				m.posX = m.posX + m_speed;
			}
			else if (randDir == 2)
			{
				if (m.posX < -20 && stage == 0)
					continue;
				m.Dir = randDir;
				m.posX = m.posX - m_speed;
			}
			else if (randDir == 3)
			{
				if (m.posZ > 20 && stage == 0)
					continue;
				m.Dir = randDir;
				m.posZ = m.posZ + m_speed;
			}
			else if (randDir == 4)
			{
				if (m.posZ < -20 && stage == 0)
					continue;
				m.Dir = randDir;
				m.posZ = m.posZ - m_speed;
			}
			else if (randDir == 5)
			{
				if (m.posZ > 20 && stage == 0)
					continue;
				else if (m.posZ > 20 && stage == 0)
					continue;
				m.Dir = randDir;
				m.posX = m.posX + (m_speed / 2);
				m.posZ = m.posZ + (m_speed / 2);
			}
			else if (randDir == 6)
			{
				if (m.posZ > 20 && stage == 0)
					continue;
				else if (m.posZ < -20 && stage == 0)
					continue;
				m.Dir = randDir;
				m.posX = m.posX + (m_speed / 2);
				m.posZ = m.posZ - (m_speed / 2);
			}
			else if (randDir == 7)
			{
				if (m.posZ < -20 && stage == 0)
					continue;
				else if (m.posZ > 20 && stage == 0)
					continue;
				m.posX = m.posX - (m_speed / 2);
				m.posZ = m.posZ + (m_speed / 2);
			}
			else if (randDir == 8)
			{
				if (m.posZ < -20 && stage == 0)
					continue;
				else if (m.posZ < -20 && stage == 0)
					continue;
				m.Dir = randDir;
				m.posX = m.posX - (m_speed / 2);
				m.posZ = m.posZ - (m_speed / 2);
			}
			cout << "m id: " << m.playerId << " m x: " << m.posX << " m z: " << m.posZ << endl;
		}
		
		auto sendBufferM = ServerPacketHandler::Make_S_BroadcastMove(m.playerId,
			m.Dir,
			m.hp, m.posX,
			m.posY, m.posZ,
			m.wDown, false);

		BroadCast(sendBufferM);
	}
}
