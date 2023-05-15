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
	for (auto iter = _monsters.begin(); iter != _monsters.end(); iter++)
	{
		cout << "create monster: " << iter->posX << " " << iter->posY << " " << iter->posZ << endl;
	}

	auto sendBufferM = ServerPacketHandler::Make_S_EnemyList(_monsters);
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
		l_player.posX = iter->second->xPos;
		l_player.posY = iter->second->yPos;
		l_player.posZ = iter->second->zPos;
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

void Room::PlayerDead(PlayerRef player)
{
	auto sendBuffer = ServerPacketHandler::Make_S_BroadcastLeave_Game(player->playerId);
	cout << "Dead Player ID: " << player->playerId << endl;
	player->dead = true;
	BroadCast(sendBuffer);
}

void Room::DeadMonster(int32 monsterId)
{
	for (auto iter = _monsters.begin(); iter != _monsters.end(); iter++)
	{
		if (iter->enmyId == monsterId) {
			WRITE_LOCK;
			_monsters.erase(iter);

			// 모두에게 알린다.
			auto sendBuffer = ServerPacketHandler::Make_S_BroadcastLeave_Game(monsterId);
			cout << "Dead Monster ID: " << monsterId << endl;
			BroadCast(sendBuffer);
			break;
		}
	}
}

void Room::AttackedMonster(int32 monsterId, uint16 hp, int32 targetId, bool hitEnemy)
{
	for (auto iter = _monsters.begin(); iter != _monsters.end(); iter++)
	{
		if (iter->enmyId == monsterId)
		{
			iter->hp = hp;
			iter->hitEnemy = hitEnemy;
			iter->agro = true;
			iter->targetId = targetId;
			auto sendBuffer = ServerPacketHandler::Make_S_AttackedMonster(iter->enmyId, iter->hp);
			cout << "Attacked Monster ID: " << iter->enmyId << " hp: " << iter->hp << " hitEnemy: " << hitEnemy << endl;
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

void Room::CreateMonster(float x, float y, float z, int stage)
{
	EnemyObject l_enemy;

	switch (stage)
	{
	case 0: 
	{
		for (int i = 0; i < 5; i++)
		{
			l_enemy.enmyId = 500 + i;
			l_enemy.hp = 150;
			l_enemy.type = MONSTER2;
			l_enemy.posX = x;
			l_enemy.posY = y + 1;
			l_enemy.posZ = z;
			l_enemy.agro = false;
			l_enemy.targetId = 0;
			l_enemy.isAttack = false;
			l_enemy.dir = 0;
			GRoom._monsters.push_back(l_enemy);
		}
		break;
	}
	case 1: {
		for (int i = 0; i < 2; i++)
		{
			l_enemy.enmyId = 500 + i;
			l_enemy.hp = 500;
			l_enemy.type = MONSTER1;
			if (i == 0)
				l_enemy.posX = x + 10;
			else if (i == 1)
				l_enemy.posX = x - 10;
			l_enemy.posY = y;
			l_enemy.posZ = z;
			l_enemy.agro = false;
			l_enemy.targetId = 0;
			l_enemy.isAttack = false;
			l_enemy.dir = 0;
			GRoom._monsters.push_back(l_enemy);
		}
		break;
	}
	case 2: {
		for (int i = 0; i < 6; i++)
		{
			l_enemy.enmyId = 500 + i;
			l_enemy.hp = 200;
			if (i % 2 == 0) {
				l_enemy.type = MONSTER2;
				l_enemy.posX = x;
				l_enemy.posY = y;
				l_enemy.posZ = z;
			}
			else {
				l_enemy.type = MONSTER3;
				l_enemy.posX = x;
				l_enemy.posY = y - 1.7f;
				l_enemy.posZ = z;
			}
			l_enemy.agro = false;
			l_enemy.targetId = 0;
			l_enemy.isAttack = false;
			l_enemy.dir = 0;
			GRoom._monsters.push_back(l_enemy);
		}
		break;
	}
	default:
		break;
	}


		  /*
		  l_player.posX = x + rand() % 10;
		  if (l_player.type == MONSTER2)
		  {
			  l_player.posY = y + 2;
		  }
		  else
			  l_player.posY = y;

		  l_player.posZ = z + rand() % 10;
		  */
}

EnemyObject Room::CreateBossMonster()
{
	EnemyObject l_player;

	l_player.enmyId = 1000;
	l_player.dir = 0;
	l_player.type = (int32)BossType::BOSS1;
	l_player.hp = 1000;   
	l_player.posX = -660.f;
	//l_player.posX = 1.f;
	l_player.posY = -6.f;
	//l_player.posY = 2.1f;
	l_player.posZ = 118.f;
	//l_player.posZ = 31.f;
	GRoom._monsters.push_back(l_player);

	return l_player;
}

void Room::MoveMonster()
{
	int bossAttack = 0;
	if (_monsters.size() == 0)
	{
		if (stage == 0) {
			for (auto& p : _players)
			{
				if (p.second->xPos <= -120.f + 10.f && p.second->zPos <= 27.f + 7.5f
					&& p.second->xPos >= -120.f - 10.f && p.second->zPos >= 27.f - 7.5f)
				{
					stage += 1;
					CreateMonster(-235.f, 2.5f, 27.f, stage);
					maxXpos[1] = -235.f;
					maxZpos[1] = 27.f;
					auto monster = ServerPacketHandler::Make_S_EnemyList(_monsters);
					BroadCast(monster);
					break;
				}
				else if (p.second->xPos <= 0.f + 10.f && p.second->zPos <= 125.f + 7.5f
					&& p.second->xPos >= 0.f - 10.f && p.second->zPos >= 125.f - 7.5f)
				{
					stage += 1;
					CreateMonster(0.f, 2.5f, 225.f, stage);
					maxXpos[1] = 0.f;
					maxZpos[1] = 225.f;
					auto monster = ServerPacketHandler::Make_S_EnemyList(_monsters);
					BroadCast(monster);
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
					stage += 1;
					CreateMonster(-235.f, 2.5f, 225.f, stage);
					maxXpos[2] = -235.f;
					maxZpos[2] = 225.f;
					auto monster = ServerPacketHandler::Make_S_EnemyList(_monsters);
					BroadCast(monster);
					break;
				}
				else if (p.second->xPos <= -235.f + 10.f && p.second->zPos <= 120.f + 7.5f
					&& p.second->xPos >= -235.f - 10.f && p.second->zPos >= 120.f - 7.5f)
				{
					stage += 1;
					CreateMonster(-235.f, 2.5f, 225.f, stage);
					maxXpos[2] = -235.f;
					maxZpos[2] = 225.f;
					auto monster = ServerPacketHandler::Make_S_EnemyList(_monsters);
					BroadCast(monster);
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
					stage += 1;
					List<EnemyObject> l_boss;
					EnemyObject boss = CreateBossMonster();
					l_boss.emplace_back(boss);
					maxXpos[3] = -660.f;
					maxZpos[3] = 118.f;
					auto bossSend = ServerPacketHandler::Make_S_EnemyList(l_boss);
					BroadCast(bossSend);
					cout << "send boss " << stage << endl;
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
							&& p.second->xPos >= m.posX - 10 && p.second->zPos >= m.posZ - 10 && !p.second->dead && !m.agro && !m.hitEnemy)
						{
							if (p.second->xPos <= m.posX)
								m.posX -= m_speed;
							if (p.second->xPos >= m.posX)
								m.posX += m_speed;
							if (p.second->zPos <= m.posZ)
								m.posZ -= m_speed;
							if (p.second->zPos >= m.posZ)
								m.posZ += m_speed;
							m.isAttack = true;
							bossAttack = 1;
							//cout << "boss attack: " << bossAttack << endl;
							auto sendBufferM = ServerPacketHandler::Make_S_BroadcastMove(m.enmyId,
								m.dir,
								m.hp, m.posX,
								m.posY, m.posZ,
								m.isAttack, false, bossAttack);

							BroadCast(sendBufferM);
						}
						else if (p.second->xPos <= m.posX + 50 && p.second->zPos <= m.posZ + 50
							&& p.second->xPos >= m.posX - 50 && p.second->zPos >= m.posZ - 50 && !p.second->dead && !m.agro && !m.hitEnemy)
						{
							m.isAttack = true;
							bossAttack = 2;
							//cout << "boss attack: " << bossAttack << endl;
							auto sendBufferM = ServerPacketHandler::Make_S_BroadcastMove(m.enmyId,
								m.dir,
								m.hp, m.posX,
								m.posY, m.posZ,
								m.isAttack, false, bossAttack);

							BroadCast(sendBufferM);
						}
						else {
							m.isAttack = false;
							bossAttack = 0;
						}
					}
					else if(m.type == 4)
					{
						if (p.second->xPos <= m.posX + 30 && p.second->zPos <= m.posZ + 30
							&& p.second->xPos >= m.posX - 30 && p.second->zPos >= m.posZ - 30 && !p.second->dead && m.type == 4)
						{
							m.isAttack = true;
							cout << " m id: " << m.enmyId << " m type: " << m.type << " attack: " << m.isAttack << endl;
							auto sendBufferM = ServerPacketHandler::Make_S_BroadcastMove(m.enmyId,
								m.dir,
								m.hp, m.posX,
								m.posY, m.posZ,
								m.isAttack, false, bossAttack);

							BroadCast(sendBufferM);
						}
						else
							m.isAttack = false;
					}
					else
					{
						if (p.second->xPos <= m.posX + 10 && p.second->zPos <= m.posZ + 10
							&& p.second->xPos >= m.posX - 10 && p.second->zPos >= m.posZ - 10 && !p.second->dead && m.type != 4 && !m.agro && !m.hitEnemy)
						{
							if (p.second->xPos - 3 <= m.posX)
								m.posX -= m_speed;
							if (p.second->xPos + 3 >= m.posX)
								m.posX += m_speed;
							if (p.second->zPos - 3 <= m.posZ)
								m.posZ -= m_speed;
							if (p.second->zPos + 3 >= m.posZ)
								m.posZ += m_speed;
							m.isAttack = true;
							//cout << " m id: " << m.enmyId << " m type: " << m.type << " attack: " << m.isAttack << endl;
							auto sendBufferM = ServerPacketHandler::Make_S_BroadcastMove(m.enmyId,
								m.dir,
								m.hp, m.posX,
								m.posY, m.posZ,
								m.isAttack, false, bossAttack);

							BroadCast(sendBufferM);
						}
						else
							m.isAttack = false;
					}
				}
			}
			uint16 randDir = rand() % 9;
			if (!m.isAttack && m.type != 4 && !m.agro && !m.hitEnemy)
			{
				if (randDir == 0)
				{
					continue;
				}
				else if (randDir == 1)
				{
					if (m.posX > maxXpos[stage] + 20)
						continue;
					m.dir = randDir;
					m.posX = m.posX + m_speed;
				}
				else if (randDir == 2)
				{
					if (m.posX < maxXpos[stage] - 20)
						continue;
					m.dir = randDir;
					m.posX = m.posX - m_speed;
				}
				else if (randDir == 3)
				{
					if (m.posZ > maxZpos[stage] + 20)
						continue;
					m.dir = randDir;
					m.posZ = m.posZ + m_speed;
				}
				else if (randDir == 4)
				{
					if (m.posZ < maxZpos[stage] - 20)
						continue;
					m.dir = randDir;
					m.posZ = m.posZ - m_speed;
				}
				else if (randDir == 5)
				{
					if (m.posZ > maxZpos[stage] + 20)
						continue;
					m.dir = randDir;
					m.posX = m.posX + (m_speed / 2);
					m.posZ = m.posZ + (m_speed / 2);
				}
				else if (randDir == 6)
				{
					if (m.posZ > maxZpos[stage] + 20)
						continue;
					else if (m.posZ < maxZpos[stage] - 20)
						continue;
					m.dir = randDir;
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
					m.dir = randDir;
					m.posX = m.posX - (m_speed / 2);
					m.posZ = m.posZ - (m_speed / 2);
				}
				//cout << "m id: " << m.playerId << " m x: " << m.posX << " m z: " << m.posZ << endl;
				auto sendBufferM = ServerPacketHandler::Make_S_BroadcastMove(m.enmyId,
					m.dir,
					m.hp, m.posX,
					m.posY, m.posZ,
					m.isAttack, false, bossAttack);

				BroadCast(sendBufferM);
			}
			else if (m.agro && !m.hitEnemy)
			{
				if (m.type == MONSTER1)
					continue;
				for (auto& p : _players)
				{
					if (m.targetId == p.second->playerId)
					{
						if (p.second->xPos - 3 <= m.posX)
							m.posX -= m_speed;
						if (p.second->xPos + 3 >= m.posX)
							m.posX += m_speed;
						if (p.second->zPos - 3 <= m.posZ)
							m.posZ -= m_speed;
						if (p.second->zPos + 3 >= m.posZ)
							m.posZ += m_speed;
						m.isAttack = true;
						if (p.second->xPos <= m.posX + 50 && p.second->zPos <= m.posZ + 50
							&& p.second->xPos >= m.posX - 50 && p.second->zPos >= m.posZ - 50 && !p.second->dead && !m.agro)
						{
							bossAttack = 2;
						}
						else
							bossAttack = 1;
						cout << " m id: " << m.enmyId << " m type: " << m.type << " target id: " << m.targetId << endl;
						auto sendBufferM = ServerPacketHandler::Make_S_BroadcastMove(m.enmyId,
							m.dir,
							m.hp, m.posX,
							m.posY, m.posZ,
							m.isAttack, false, bossAttack);

						BroadCast(sendBufferM);
					}
				}
			}
		}
	}
}
