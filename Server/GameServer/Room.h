#pragma once
#include "ServerPacketHandler.h"

#define MAX_MONSTER 6

class Room
{
public:
	void Enter(PlayerRef player);
	void Leave(PlayerRef player);
	void PlayerDead(PlayerRef player);
	void DeadMonster(int32 monsterId);
	void AttackedMonster(int32 monsterId, uint16 hp, int32 targetId);
	void BroadCast(SendBufferRef sendBuffer);
	void CreateMonster(float x, float y, float z);
	EnemyObject CreateBossMonster();
	void MoveMonster();

private:
	USE_LOCK;
	map<uint64, PlayerRef> _players;

public:
	List<EnemyObject> _monsters;
	short stage = 0;

	float maxXpos[4] = { 1.f, -235.f, 0.f, -660.f };
	float maxZpos[4] = { 1.f, 27.f, 225.f, 118.f };
};

extern Room GRoom;