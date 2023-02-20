#pragma once

enum PlayerType
{
	NONE = 0,
	POWER = 1,
	SPEED = 2,
	AMOR = 3,
	MONSTER = 4,
};

class Player
{
public:

	int32			playerId = 0;
	int32			playerDir = 0;
	uint16			hp = 0;
	PlayerType		type;
	wstring			name;
	float			xPos;
	float			yPos;
	float			zPos;

	// �ִϸ��̼�
	bool wDown = false;
	bool isJump = false;

	GameSessionRef	ownerSession; // �޸� ���� ���� �ذ��ؾ���
};

