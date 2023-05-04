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

void Monster_AI();

int main()
{
	ServerServiceRef service = MakeShared<ServerService>(
<<<<<<< Updated upstream
		NetAddress(L"127.0.0.1", 7777),
=======
		NetAddress(L"127.0.0.1", 7778),
>>>>>>> Stashed changes
		MakeShared<IocpCore>(),
		MakeShared<GameSession>, // TODO : SessionManager 등
		100);

	ASSERT_CRASH(service->Start());

	// 몬스터 생성
	//GRoom.CreateBossMonster();
	//cout << "send boss " << stage << endl;
	GRoom.CreateMonster(1.f, 2.1f, 31.f);

	for (int32 i = 0; i < 5; i++)
	{
		if (i == 4)
		{
			GThreadManager->Launch([=]()
				{
					Monster_AI();
				});
		}
		else
		{
			GThreadManager->Launch([=]()
				{
					while (true)
					{
						service->GetIocpCore()->Dispatch();
					}
				});
		}
	}
	GThreadManager->Join();
}

void Monster_AI()
{
	while (true)
	{
		this_thread::sleep_for(200ms);
		GRoom.MoveMonster();
	}
}