#pragma once

enum PlayerType
{
	NONE = 0,
	POWER = 1,
	SPEED = 2,
	AMOR = 3
};

class Player
{
public:

	int32			playerId = 0;
	PlayerType		type;
	wstring			name;
	float			xPos;
	float			yPos;
	float			zPos;
	GameSessionRef	ownerSession; // 메모리 누수 문제 해결해야함
};

