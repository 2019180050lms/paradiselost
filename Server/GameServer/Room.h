#pragma once
#include "ServerPacketHandler.h"

#define MAX_MONSTER 5

class Room
{
public:
	void Enter(PlayerRef player);
	void Leave(PlayerRef player);
	void DeadMonster(int32 monsterId);
	void AttackedMonster(int32 monsterId, uint16 hp);
	void BroadCast(SendBufferRef sendBuffer);
	void CreateMonster();

private:
	USE_LOCK;
	map<uint64, PlayerRef> _players;

public:
	list<PlayerList> _monsters;
};

extern Room GRoom;