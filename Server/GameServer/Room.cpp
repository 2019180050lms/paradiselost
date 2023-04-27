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
	l_player.posX = -660.f;
	l_player.posY = -2.f;
	l_player.posZ = 118.f;
	GRoom._monsters.push_back(l_player);

	return l_player;
}

void Room::MoveMonster()
{
	float maxXpos[4] = {1.f, -235.f, 0.f, -660.f};
	float maxZpos[4] = {1.f, 27.f, 225.f, 118.f};
	int bossAttack = 0;
	if (_monsters.size() == 0)
	{
		if (stage == 0) {
			for (auto& p : _players)
			{
				if (p.second->xPos <= -120.f + 10.f && p.second->zPos <= 27.f + 7.5f
					&& p.second->xPos >= -120.f - 10.f && p.second->zPos >= 27.f - 7.5f)
				{
					CreateMonster(-235.f, 2.5f, 27.f);
					auto monster = ServerPacketHandler::Make_S_PlayerList(_monsters);
					BroadCast(monster);
					stage += 1;
					break;
				}
				else if (p.second->xPos <= 0.f + 10.f && p.second->zPos <= 125.f + 7.5f
					&& p.second->xPos >= 0.f - 10.f && p.second->zPos >= 125.f - 7.5f)
				{
					CreateMonster(0.f, 2.5f, 225.f);
					auto monster = ServerPacketHandler::Make_S_PlayerList(_monsters);
					BroadCast(monster);
					stage += 1;
					break;
				}
			}
		}
		else if (stage == 1)
		{
			for (auto& p : _players)
			{
				if (p.second->xPos <= -120.f + 10.f && p.second->zPos <= 225.f + 7.5f
					&& p.second->xPos >= -120.f - 10.f && p.second->zPos >= 225.f - 7.5f)
				{
					CreateMonster(-235.f, 2.5f, 225.f);
					auto monster = ServerPacketHandler::Make_S_PlayerList(_monsters);
					BroadCast(monster);
					stage += 1;
					break;
				}
				if (p.second->xPos <= -235.f + 10.f && p.second->zPos <= 120.f + 7.5f
					&& p.second->xPos >= -235.f - 10.f && p.second->zPos >= 120.f - 7.5f)
				{
					CreateMonster(-235.f, 2.5f, 225.f);
					auto monster = ServerPacketHandler::Make_S_PlayerList(_monsters);
					BroadCast(monster);
					stage += 1;
					break;
				}
			}
		}
		else if (stage == 2)
		{
			for (auto& p : _players)
			{
				if (p.second->xPos <= -435.f + 10.f && p.second->zPos <= 118.f + 7.5f
					&& p.second->xPos >= -435.f - 10.f && p.second->zPos >= 118.f - 7.5f)
				{
					List<PlayerList> l_boss;
					PlayerList boss = CreateBossMonster();
					l_boss.emplace_back(boss);
					auto bossSend = ServerPacketHandler::Make_S_PlayerList(l_boss);
					BroadCast(bossSend);
					cout << "send boss " << stage << endl;
					stage += 1;
					break;
				}
			}
		}
		else if (stage == 3)
			stage = 0;
	}
	else
	{
		for (auto& m : _monsters)
		{
			for (auto& p : _players)
			{
				if (p.second->playerId < 500)
				{
					if (m.type == (int)BOSS1)
					{
						if (p.second->xPos <= m.posX + 10 && p.second->zPos <= m.posZ + 10
							&& p.second->xPos >= m.posX - 10 && p.second->zPos >= m.posZ - 10)
						{
							if (p.second->xPos <= m.posX)
								m.posX -= m_speed;
							if (p.second->xPos >= m.posX)
								m.posX += m_speed;
							if (p.second->zPos <= m.posZ)
								m.posZ -= m_speed;
							if (p.second->zPos >= m.posZ)
								m.posZ += m_speed;
							m.wDown = true;
							bossAttack = 1;
							cout << "boss attack: " << bossAttack << endl;
						}
						else if (p.second->xPos <= m.posX + 20 && p.second->zPos <= m.posZ + 20
							&& p.second->xPos >= m.posX - 20 && p.second->zPos >= m.posZ - 20)
						{
							m.wDown = true;
							bossAttack = 2;
							cout << "boss attack: " << bossAttack << endl;
						}
						else {
							m.wDown = false;
							bossAttack = 0;
						}
					}
					else
					{
						if (p.second->xPos <= m.posX + 10 && p.second->zPos <= m.posZ + 10
							&& p.second->xPos >= m.posX - 10 && p.second->zPos >= m.posZ - 10)
						{
							if (m.type == 4) {
								continue;
							}
							if (p.second->xPos <= m.posX)
								m.posX -= m_speed;
							if (p.second->xPos >= m.posX)
								m.posX += m_speed;
							if (p.second->zPos <= m.posZ)
								m.posZ -= m_speed;
							if (p.second->zPos >= m.posZ)
								m.posZ += m_speed;
							m.wDown = true;
							//cout << "p id: " << p.second->playerId << " m id: " << m.playerId << " x: " << m.posX << " z: " << m.posZ << " attack: " << m.wDown << endl;
						}
						else
							m.wDown = false;
					}
				}
			}
			uint16 randDir = rand() % 9;
			if (!m.wDown && m.type != 4)
			{
				if (randDir == 0)
				{
					continue;
				}
				else if (randDir == 1)
				{
					if (m.posX > maxXpos[stage] + 20)
						continue;
					m.Dir = randDir;
					m.posX = m.posX + m_speed;
				}
				else if (randDir == 2)
				{
					if (m.posX < maxXpos[stage] - 20)
						continue;
					m.Dir = randDir;
					m.posX = m.posX - m_speed;
				}
				else if (randDir == 3)
				{
					if (m.posZ > maxZpos[stage] + 20)
						continue;
					m.Dir = randDir;
					m.posZ = m.posZ + m_speed;
				}
				else if (randDir == 4)
				{
					if (m.posZ < maxZpos[stage] - 20)
						continue;
					m.Dir = randDir;
					m.posZ = m.posZ - m_speed;
				}
				else if (randDir == 5)
				{
					if (m.posZ > maxZpos[stage] + 20)
						continue;
					m.Dir = randDir;
					m.posX = m.posX + (m_speed / 2);
					m.posZ = m.posZ + (m_speed / 2);
				}
				else if (randDir == 6)
				{
					if (m.posZ > maxZpos[stage] + 20)
						continue;
					else if (m.posZ < maxZpos[stage] - 20)
						continue;
					m.Dir = randDir;
					m.posX = m.posX + (m_speed / 2);
					m.posZ = m.posZ - (m_speed / 2);
				}
				else if (randDir == 7)
				{
					if (m.posZ < maxZpos[stage] - 20)
						continue;
					else if (m.posZ > maxZpos[stage] + 20)
						continue;
					m.posX = m.posX - (m_speed / 2);
					m.posZ = m.posZ + (m_speed / 2);
				}
				else if (randDir == 8)
				{
					if (m.posZ < maxZpos[stage] - 20)
						continue;
					else if (m.posZ < maxZpos[stage] - 20)
						continue;
					m.Dir = randDir;
					m.posX = m.posX - (m_speed / 2);
					m.posZ = m.posZ - (m_speed / 2);
				}
				//cout << "m id: " << m.playerId << " m x: " << m.posX << " m z: " << m.posZ << endl;
			}

			auto sendBufferM = ServerPacketHandler::Make_S_BroadcastMove(m.playerId,
				m.Dir,
				m.hp, m.posX,
				m.posY, m.posZ,
				m.wDown, false, bossAttack);

			BroadCast(sendBufferM);
		}
	}
}
