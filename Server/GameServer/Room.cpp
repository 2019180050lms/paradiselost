#include "pch.h"
#include "Room.h"
#include "Player.h"
#include "GameSession.h"
#include "ServerPacketHandler.h"

Room GRoom;

void Room::Enter(PlayerRef player)
{
	// �÷��̾� �߰�
	WRITE_LOCK;
	_players[player->playerId] = player;

	List<PlayerList> players;
	PlayerList l_player;

	cout << "player ID: " << player->playerId << endl;

	for (int i = 0; i < _players.size(); i++)
	{
		if (_players[i + 1] != nullptr)
		{
			if (_players[i + 1]->playerId == _players[player->playerId]->playerId)
			{
				l_player.isSelf = true;
			}
			else
				l_player.isSelf = false;
			l_player.playerId = _players[i + 1]->playerId;
			l_player.posX = 0.f;
			l_player.posY = 0.f;
			l_player.posZ = 0.f;

			players.push_back(l_player);

		}
	}

	auto sendBuffer = ServerPacketHandler::Make_S_PlayerList(players);
	_players[player->playerId]->ownerSession->Send(sendBuffer);

	// ���Ի� ������ ��ο��� �˸���.
	auto sendBuffers = ServerPacketHandler::Make_S_BroadcastEnter_Game(player->playerId, 0.f, 0.f, 0.f);
	BroadCast(sendBuffers);
}

void Room::Leave(PlayerRef player)
{
	// �÷��̾� ����
	WRITE_LOCK;
	_players.erase(player->playerId);

	// ��ο��� �˸���.
	auto sendBuffer = ServerPacketHandler::Make_S_BroadcastLeave_Game(player->playerId);
	BroadCast(sendBuffer);
}

void Room::BroadCast(SendBufferRef sendBuffer)
{
	WRITE_LOCK;
	for (auto& p : _players)
	{
		p.second->ownerSession->Send(sendBuffer);
	}
}
