#pragma once
#include "ServerPacketHandler.h"

#define MAX_MONSTER 5

class Room
{
public:
	void Enter(PlayerRef player);
	void Leave(PlayerRef player);
	void BroadCast(SendBufferRef sendBuffer);

private:
	USE_LOCK;
	map<uint64, PlayerRef> _players;

public:
	List<PlayerList> _monsters;
};

extern Room GRoom;