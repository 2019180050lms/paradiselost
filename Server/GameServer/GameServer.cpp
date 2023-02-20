#include "pch.h"
#include "ThreadManager.h"
#include "Service.h"
#include "Session.h"
#include "GameSession.h"
#include "GameSessionManager.h"
#include "BufferWriter.h"
#include "ServerPacketHandler.h"
#include <tchar.h>
#include "Player.h"

int main()
{
	ServerServiceRef service = MakeShared<ServerService>(
		NetAddress(L"192.168.0.7", 7777),
		MakeShared<IocpCore>(),
		MakeShared<GameSession>, // TODO : SessionManager 등
		100);

	ASSERT_CRASH(service->Start());

	// 몬스터 생성
	PlayerList l_player;

	for (int i = 0; i < 5; i++)
	{
		l_player.isSelf = false;
		l_player.playerId = 500 + i;
		l_player.type = (int32)PlayerType::MONSTER;
		l_player.hp = 100;
		l_player.posX = 1.f * (float)(i + 20);
		l_player.posY = 0.f;
		l_player.posZ = 1.f * (float)(i + 20);

		GRoom._monsters.push_back(l_player);
	}
	//
	for (int32 i = 0; i < 5; i++)
	{
		GThreadManager->Launch([=]()
			{
				while (true)
				{
					service->GetIocpCore()->Dispatch();
				}				
			});
	}

	GThreadManager->Join();
}