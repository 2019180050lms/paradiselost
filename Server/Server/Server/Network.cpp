#include "Network.h"
#include "protocol.h"
#include "Session.h"
#include "Timer.h"
#include "DB.h"
#include <iostream>
using namespace std;

SOCKET g_s_socket, g_c_socket;
OVER_EXP g_a_over;

OVER_EXP::OVER_EXP()
{
	_wsabuf.len = BUF_SIZE;
	_wsabuf.buf = _send_buf;
	_comp_type = OP_RECV;
	ZeroMemory(&_over, sizeof(_over));
}

OVER_EXP::OVER_EXP(char* packet)
{
	_wsabuf.len = packet[0];
	_wsabuf.buf = _send_buf;
	ZeroMemory(&_over, sizeof(_over));
	_comp_type = OP_SEND;
	memcpy(_send_buf, packet, packet[0]);
}

void worker_thread(HANDLE h_iocp)
{
	while (true) {
		DWORD num_bytes;
		ULONG_PTR key;
		WSAOVERLAPPED* over = nullptr;
		BOOL ret = GetQueuedCompletionStatus(h_iocp, &num_bytes, &key, &over, INFINITE);
		OVER_EXP* ex_over = reinterpret_cast<OVER_EXP*>(over);
		if (FALSE == ret) {
			if (ex_over->_comp_type == OP_ACCEPT) cout << "Accept Error";
			else {
				cout << "GQCS Error on client[" << key << "]\n";
				disconnect(static_cast<int>(key));
				if (ex_over->_comp_type == OP_SEND) delete ex_over;
				continue;
			}
		}

		if ((0 == num_bytes) && ((ex_over->_comp_type == OP_RECV) || (ex_over->_comp_type == OP_SEND))) {
			disconnect(static_cast<int>(key));
			if (ex_over->_comp_type == OP_SEND) delete ex_over;
			continue;
		}

		switch (ex_over->_comp_type) {
		case OP_ACCEPT: {
			int client_id = get_new_client_id();
			if (client_id != -1) {
				{
					lock_guard<mutex> ll(clients[client_id]._s_lock);
					clients[client_id]._state = ST_ALLOC;
				}
				clients[client_id]._hp = 100;
				clients[client_id].x = 0;
				clients[client_id].y = 5.f;
				clients[client_id].z = 0;
				clients[client_id]._id = client_id;
				clients[client_id]._name[0] = 0;
				clients[client_id]._prev_remain = 0;
				clients[client_id]._socket = g_c_socket;
				CreateIoCompletionPort(reinterpret_cast<HANDLE>(g_c_socket),
					h_iocp, client_id, 0);
				clients[client_id].do_recv();
				g_c_socket = WSASocket(AF_INET, SOCK_STREAM, 0, NULL, 0, WSA_FLAG_OVERLAPPED);
			}
			else {
				cout << "Max user exceeded.\n";
			}
			ZeroMemory(&g_a_over._over, sizeof(g_a_over._over));
			int addr_size = sizeof(SOCKADDR_IN);
			AcceptEx(g_s_socket, g_c_socket, g_a_over._send_buf, 0, addr_size + 16, addr_size + 16, 0, &g_a_over._over);
			cout << "Accept Send" << endl;
			break;
		}
		case OP_RECV: {
			int remain_data = num_bytes + clients[key]._prev_remain;
			char* p = ex_over->_send_buf;
			while (remain_data > 0) {
				int packet_size = p[0];
				if (packet_size <= remain_data) {
					process_packet(static_cast<int>(key), p);
					p = p + packet_size;
					remain_data = remain_data - packet_size;
				}
				else break;
			}
			clients[key]._prev_remain = remain_data;
			if (remain_data > 0) {
				memcpy(ex_over->_send_buf, p, remain_data);
			}
			clients[key].do_recv();
			break;
		}
		case OP_SEND:
			delete ex_over;
			break;
		case OP_NPC_AI:
		{
			bool deactivate = true;
			for (int i = 0; i < MAX_USER; ++i) {
				if (clients[i]._state != ST_INGAME) continue;
				if (can_see(i, key)) {
					deactivate = true;
					break;
				}
			}

			if (true == deactivate) {
				if (!clients[key].isAttack && clients[key].targetId < 0 && clients[key].bossAttack < 0) {
					do_random_move(static_cast<int>(key));
					add_timer(key, chrono::system_clock::now() + 1s, EV_RANDOM_MOVE);
				}
				else if (clients[key]._stage == 2 || (!clients[key].isAttack && clients[key].targetId >= 0 && clients[key].bossAttack < 0)) {
					do_player_attack(static_cast<int>(key), clients[key].targetId);
					add_timer(key, chrono::system_clock::now() + 1s, EV_RANDOM_MOVE);
				}
				else if (clients[key].isAttack && clients[key].bossAttack < 0) {
					do_delay_disable(static_cast<int>(key), clients[key].targetId);
					add_timer(key, chrono::system_clock::now() + 1s, EV_RANDOM_MOVE);
				}
				else if (clients[key].bossAttack >= 0) {
					do_player_attack(static_cast<int>(key), clients[key].targetId);
					add_timer(key, chrono::system_clock::now() + 1s, EV_RANDOM_MOVE);
				}
			}
			else
				cout << "deactivate = false" << endl;
			break;
		}
		case OP_BOSS_AI:
		{

			break;
		}
		case OP_STAGE_CLEAR:
		{
			short stage = clients[key]._stage;
			for (auto& client : clients) {
				if (client._id >= MAX_USER) break;
				if (client._state == ST_FREE || client._state == ST_ALLOC || client._state == ST_ML_AGENT) continue;
				if (client._stage == stage) {
					client.send_stage_clear(stage, 1);
					client._s_lock.lock();
					client.x = 30.f;
					client.y = 5.f;
					client.z = 40.f;
					client._stage = 0;
					client._s_lock.unlock();
					client.send_move_packet(client._id);
				}
			}
			for (int i = MAX_USER; i < MAX_USER + MAX_NPC - 1; ++i) {
				if (clients[i]._state == ST_FREE) {
					lock_guard<mutex> ll(clients[i]._s_lock);
					clients[i]._state = ST_INGAME;
					clients[i].targetId = -1;
					clients[i].isAttack = false;
					clients[i]._hp = 100;
				}
			}
			break;
		}
		default:
			break;
		}
	}
}

void process_packet(int c_id, char* packet)
{
	switch (packet[2]) {
	case CS_LOGIN: {
		CS_LOGIN_PACKET* p = reinterpret_cast<CS_LOGIN_PACKET*>(packet);
		if (p->ml_client) {
			{
				lock_guard<mutex> ll{ clients[c_id]._s_lock };
				clients[c_id]._state = ST_ML_AGENT;
				clients[c_id]._stage = 2;
				clients[c_id]._type = 0;
			}
			clients[c_id].send_login_info_packet(true);
			clients[c_id].send_enter_game_packet();
			for (auto& pl : clients) {
				{
					lock_guard<mutex> ll(pl._s_lock);
					if (ST_INGAME != pl._state) continue;
				}
				if (pl._id == c_id) continue;
				if (false == can_see(c_id, pl._id))
					continue;
				if (is_pc(pl._id) && clients[c_id]._stage == pl._stage) pl.send_add_player_packet(c_id);
				else if (pl._stage == clients[c_id]._stage) {
					wakeup_npc(pl._id);
				}
				if (pl._stage == clients[c_id]._stage) {
					clients[c_id].send_add_player_packet(pl._id);
					cout << pl._id << " add player" << endl;

				}
			}
		}
		else if (!p->ml_client)
		{
			if (p->name[0] == L'\x2')
				break;
			wcscpy_s(clients[c_id]._name, p->name);
			_db_l.lock();
			bool check = db.check_user_id(clients[c_id]._name);
			_db_l.unlock();
			{
				lock_guard<mutex> ll{ clients[c_id]._s_lock };
				clients[c_id]._state = ST_INGAME;
				clients[c_id]._stage = 0;
			}
			//cout << clients[c_id].x << ", " << clients[c_id].y << ", " << clients[c_id].z << endl;
			clients[c_id].send_login_info_packet(check);
			if (check == true) {
				_db_l.lock();
				db.get_user_data(clients[c_id]._name, clients[c_id]._name, &clients[c_id].x,
					&clients[c_id].y, &clients[c_id].z, &clients[c_id].exp, &clients[c_id].level,
					&clients[c_id].weapon_item, &clients[c_id]._type, &clients[c_id]._stage);
				_db_l.unlock();
				clients[c_id].send_enter_game_packet();
				for (auto& pl : clients) {
					{
						lock_guard<mutex> ll(pl._s_lock);
						if (ST_INGAME != pl._state) continue;
					}
					if (pl._id == c_id) continue;
					if (false == can_see(c_id, pl._id))
						continue;
					if (is_pc(pl._id)) pl.send_add_player_packet(c_id);
					else {
						wakeup_npc(pl._id);
					}
					if (pl._stage == clients[c_id]._stage) {
						clients[c_id].send_add_player_packet(pl._id);
						cout << pl._id << " add player" << endl;
					}
				}
			}
		}
		break;
	}
	case CS_ENTER_GAME: {
		CS_CREATE_PLAYER_PACKET* p = reinterpret_cast<CS_CREATE_PLAYER_PACKET*>(packet);
		clients[c_id]._type = p->c_type;
		if (p->name[0] == L'\x2')
			break;
		//cout << "client send: " << p->size << ", " << p->c_type << ", " << p->playerindex << endl;
		{
			lock_guard<mutex> ll{ clients[c_id]._s_lock };
			clients[c_id].x = 30.f;
			clients[c_id].y = 3.f;
			clients[c_id].z = 40.f;
			clients[c_id]._state = ST_INGAME;
			clients[c_id]._stage = 0;
		}
		clients[c_id].exp = 0;
		clients[c_id].level = 1;
		switch (clients[c_id]._type)
		{
		case 2:
			clients[c_id].weapon_item = 1;
			break;
		case 3:
			clients[c_id].weapon_item = 0;
			break;
		default:
			clients[c_id].weapon_item = -1;
			break;
		}
		_db_l.lock();
		db.add_user_data(clients[c_id]._name, clients[c_id]._name, &clients[c_id].x,
			&clients[c_id].y, &clients[c_id].z, &clients[c_id].exp, &clients[c_id].level,
			&clients[c_id].weapon_item, &clients[c_id]._type, &clients[c_id]._stage);
		_db_l.unlock();
		clients[c_id].send_enter_game_packet();
		//cout << "enter game send: " << endl;
		for (auto& pl : clients) {
			{
				lock_guard<mutex> ll(pl._s_lock);
				if (ST_INGAME != pl._state) continue;
			}
			if (pl._id == c_id) continue;
			if (false == can_see(c_id, pl._id))
				continue;
			if (is_pc(pl._id)) {
				pl.send_add_player_packet(c_id);
			}
			else {
				wakeup_npc(pl._id);
			}
			clients[c_id].send_add_player_packet(pl._id);
			cout << "first: " << pl._id << ": add " << c_id << endl;
		}
		break;
	}
	case CS_MOVE: {
		CS_MOVE_PACKET* p = reinterpret_cast<CS_MOVE_PACKET*>(packet);
		//clients[c_id].last_move_time = p->move_time;
		clients[c_id]._hp = p->hp;
		clients[c_id]._dir = p->direction;

		float c_x = clients[c_id].x;
		float c_y = clients[c_id].y;
		float c_z = clients[c_id].z;

		int z = (int)p->z;
		int x = (int)p->x;
		//if(p->x < -25 || p->x > 38)
		if (clients[c_id]._stage == 0) {
			if (hometown[z][x] == WALL)
			{
				clients[c_id].send_move_packet(c_id);
			}
			else if (clients[c_id].y < 0) {
				clients[c_id].y = 3.f;
				clients[c_id].send_move_packet(c_id);
			}
			else
			{
				clients[c_id].x = p->x;
				clients[c_id].y = p->y;
				clients[c_id].z = p->z;
			}
		}
		else if (clients[c_id]._stage == 1) {
			if (stage1[z][x] == WALL)
			{
				clients[c_id].send_move_packet(c_id);
			}
			else if (clients[c_id].y < -2) {
				clients[c_id].y = 3.f;
				clients[c_id].send_move_packet(c_id);
			}
			else
			{
				clients[c_id].x = p->x;
				clients[c_id].y = p->y;
				clients[c_id].z = p->z;
			}
		}
		else if (clients[c_id]._stage == 2) {
			if (stage2[z][x] == WALL)
			{
				clients[c_id].send_move_packet(c_id);
			}
			else if (clients[c_id].y < -50) {
				clients[c_id].y = 3.f;
				clients[c_id].send_move_packet(c_id);
			}
			else
			{
				clients[c_id].x = p->x;
				clients[c_id].y = p->y;
				clients[c_id].z = p->z;
			}
		}
		else if (clients[c_id]._stage == 3) {
			/*
			if (stage3[z][x] == WALL)
			{
				clients[c_id].send_move_packet(c_id);
			}
			else
			{
			}
			*/
			if (clients[c_id].y < -20) {
				clients[c_id].y = 3.f;
				clients[c_id].send_move_packet(c_id);
			}
			clients[c_id].x = p->x;
			clients[c_id].y = p->y;
			clients[c_id].z = p->z;
		}
		clients[c_id].isJump = p->isJump;

		if (p->isAttack && !clients[c_id]._skill_list.count(1) && clients[c_id]._stage != 2) {
			clients[c_id].isAttack = p->isAttack;
			clients[c_id]._sl.lock();
			clients[c_id]._skill_list.insert(1);
			clients[c_id]._sl.unlock();
			add_timer(c_id, std::chrono::system_clock::now() + 600ms, EV_SKILL_DELAY);
			clients[c_id].send_move_packet(c_id);
		}
		else {
			clients[c_id].isAttack = false;
		}

		unordered_set<int> near_list;
		clients[c_id]._vl.lock();
		unordered_set<int> old_vlist = clients[c_id]._view_list;
		clients[c_id]._vl.unlock();
		for (auto& cl : clients) {
			if (cl._state != ST_INGAME && cl._state != ST_ML_AGENT) continue;
			if (cl._id == c_id) continue;
			if (can_see(c_id, cl._id))
				near_list.insert(cl._id);
		}


		for (auto& pl : near_list) {
			auto& cpl = clients[pl];
			if (is_pc(pl)) {
				cpl._vl.lock();
				if (clients[pl]._view_list.count(c_id)) {
					cpl._vl.unlock();
					clients[pl].send_move_packet(c_id);
				}
				else {
					cpl._vl.unlock();
					clients[pl].send_add_player_packet(c_id);
				}
			}

			if (old_vlist.count(pl) == 0) {
				clients[c_id].send_add_player_packet(pl);
				//cout << "pl: " << pl << endl;
				if (false == is_pc(pl))
					wakeup_npc(pl);
			}
		}

		for (auto& pl : old_vlist) {
			if (0 == near_list.count(pl)) {
				clients[c_id].send_remove_player_packet(pl);
				if (is_pc(pl))
					clients[pl].send_remove_player_packet(c_id);
			}
		}
		break;
	}
	case CS_MONSTER_ATTACKED: {
		CS_MONSTER_ATTACKED_PACKET* p = reinterpret_cast<CS_MONSTER_ATTACKED_PACKET*>(packet);
		clients[p->id]._hp -= 20;
		clients[p->id].targetId = p->playerId;
		if (clients[p->id]._hp < 1) {
			disconnect(p->id);
			do_add_boss(c_id);
		}
		else {
			//wakeup_npc(p->id);
			clients[c_id].send_attacked_monster(p->id);

			unordered_set<int> near_list;
			clients[c_id]._vl.lock();
			unordered_set<int> old_vlist = clients[c_id]._view_list;
			clients[c_id]._vl.unlock();
			for (auto& cl : clients) {
				if (cl._state != ST_INGAME) continue;
				if (cl._id == c_id) continue;
				if (can_see(c_id, cl._id))
					near_list.insert(cl._id);
			}

			for (auto& pl : near_list) {
				auto& cpl = clients[pl];
				if (is_pc(pl)) {
					cpl._vl.lock();
					if (clients[pl]._view_list.count(c_id)) {
						cpl._vl.unlock();
						clients[pl].send_attacked_monster(p->id);
					}
					else
						cpl._vl.unlock();
				}
			}
		}
		break;
	}
	case CS_PLAYER_ATTACKED: {
		CS_PLAYER_ATTACKED_PACKET* p = reinterpret_cast<CS_PLAYER_ATTACKED_PACKET*>(packet);
		clients[p->playerid]._hp -= 2;
		//wakeup_npc(p->id);
		clients[c_id].send_player_attacked_packet(p->playerid);

		unordered_set<int> near_list;
		clients[c_id]._vl.lock();
		unordered_set<int> old_vlist = clients[c_id]._view_list;
		clients[c_id]._vl.unlock();
		for (auto& cl : clients) {
			if (cl._state != ST_INGAME) continue;
			if (cl._id == c_id) continue;
			if (can_see(c_id, cl._id))
				near_list.insert(cl._id);
		}

		for (auto& pl : near_list) {
			auto& cpl = clients[pl];
			if (is_pc(pl)) {
				cpl._vl.lock();
				if (clients[pl]._view_list.count(c_id) && (cpl._stage == clients[c_id]._stage)) {
					cpl._vl.unlock();
					clients[pl].send_player_attacked_packet(p->playerid);
				}
				else
					cpl._vl.unlock();
			}
		}
		break;
	}
	case CS_EQUIP_ITEM: {
		CS_EQUIP_ITEM_PACKET* p = reinterpret_cast<CS_EQUIP_ITEM_PACKET*>(packet);
		switch (p->itemType) {
		case 1:
			clients[c_id].head_item = p->itemValue;
			break;
		case 2:
			clients[c_id].weapon_item = p->itemValue;
			break;
		case 3:
			clients[c_id].leg_item = p->itemValue;
			break;
		case 10:
			clients[c_id]._hp += 10;
			break;
		default:
			cout << "Unknown packet (error)" << endl;
			disconnect(c_id);
			break;
		}
		clients[c_id].send_item_info(c_id, p->itemType, clients[c_id].weapon_item);
		//cout << "c_id: " << c_id << ", " << clients[c_id].weapon_item << endl;
		unordered_set<int> near_list;
		clients[c_id]._vl.lock();
		unordered_set<int> old_vlist = clients[c_id]._view_list;
		clients[c_id]._vl.unlock();
		for (auto& cl : clients) {
			if (cl._state != ST_INGAME) continue;
			if (cl._id == c_id) continue;
			if (can_see(c_id, cl._id))
				near_list.insert(cl._id);
		}

		for (auto& pl : near_list) {
			auto& cpl = clients[pl];
			if (is_pc(pl)) {
				cpl._vl.lock();
				if (clients[pl]._view_list.count(c_id)) {
					cpl._vl.unlock();
					clients[pl].send_item_info(c_id, p->itemType, p->itemValue);
					//cout << "pl id: " << pl << endl;
				}
				else {
					cpl._vl.unlock();
				}
			}
		}
		break;
	}
	case CS_NPC: {
		CS_NPC_PACKET* p = reinterpret_cast<CS_NPC_PACKET*>(packet);
		if (p->active = true) {
			clients[c_id]._quest_stage = p->_quest_stage;
		}
		break;
	}
	case CS_PORTAL: {
		CS_PORTAL_PACKET* p = reinterpret_cast<CS_PORTAL_PACKET*>(packet);
		clients[c_id]._stage_lock.lock();
		clients[c_id]._stage = p->stage;
		clients[c_id]._stage_lock.unlock();
		/*
		clients[c_id]._vl.lock();
		clients[c_id]._view_list.clear();
		clients[c_id]._vl.unlock();
		*/
		// TODO 스테이지별 위치 조정 추가
		switch (p->stage) {
		case 1: {
			clients[c_id]._s_lock.lock();
			clients[c_id].x = 48.f;
			clients[c_id].z = 48.f;
			clients[c_id].send_move_packet(c_id);
			clients[c_id]._s_lock.unlock();
			/*
			for (int i = MAX_USER; i < MAX_USER + MAX_NPC; ++i) {
				if (clients[i]._stage == 1) {
					clients[i]._s_lock.lock();
					clients[i].y = -1.4f;
					clients[i]._s_lock.unlock();
				}
			}*/
			break;
		}
		case 2: {
			clients[c_id]._s_lock.lock();
			//clients[c_id].x = 34.f;
			//clients[c_id].z = 10.f;
			clients[c_id].x = 53.f;
			clients[c_id].z = 24.f;
			clients[c_id]._s_lock.unlock();
			clients[c_id].send_move_packet(c_id);
			/*
			for (int i = MAX_USER; i < MAX_USER + MAX_NPC; ++i) {
				if (clients[i]._stage == 2) {
					clients[i]._s_lock.lock();
					clients[i].y = 0.2f;
					clients[i]._s_lock.unlock();
				}
			}*/
			break;
		}
		case 3: {
			/*
			for (int i = MAX_USER; i < MAX_USER + MAX_NPC; ++i) {
				if (clients[i]._stage == 3) {
					if (i >= MAX_USER + 30 && i <= MAX_USER + 36) {
						clients[i]._s_lock.lock();
						clients[i].y = -8.9f;
						clients[i]._s_lock.unlock();
					}
					else {
						clients[i]._s_lock.lock();
						clients[i].y = 0.4f;
						clients[i]._s_lock.unlock();
					}
				}
			}*/
			break;
		}
		default:
			break;
		}
		clients[c_id].send_setactive_object();
		break;
	}
	case CS_MONSTER_AI: {
		CS_MONSTER_AI_PACKET* p = reinterpret_cast<CS_MONSTER_AI_PACKET*>(packet);

		unordered_set<int> view_list;
		for (auto& cl : clients) {
			if (cl._id >= MAX_USER) break;
			if (cl._state != ST_INGAME) continue;
			if (cl._id == p->id) continue;
			if (can_see(p->id, cl._id))
				view_list.insert(cl._id);
		}

		clients[p->id].x = p->x;
		//clients[p->id].y = p->y;
		clients[p->id].z = p->z;

		unordered_set<int> near_list;

		for (auto& cl : clients) {
			if (cl._id >= MAX_USER) break;
			if (cl._state != ST_INGAME) continue;
			if (can_see(p->id, cl._id))
				near_list.insert(cl._id);
		}

		for (auto& pl : near_list) {
			auto& cpl = clients[pl];
			cpl._vl.lock();
			if (clients[pl]._view_list.count(p->id) && clients[pl]._state == ST_INGAME) {
				cpl._vl.unlock();
				if (clients[p->id]._type != 7)
					clients[pl].send_move_packet(p->id);
				else {
					clients[p->id].bossAttack = rand() % 2;
					clients[p->id].targetId = 0;
					clients[pl].send_boss_attack(p->id);
				}
			}
			else {
				cpl._vl.unlock();
				if (clients[p->id]._state != ST_FREE) {
					clients[pl].send_add_player_packet(p->id);
				}
			}
		}

		for (auto& pl : view_list)
			if (0 == near_list.count(pl)) {
				clients[pl].send_remove_player_packet(p->id);
			}

		break;
	}
	case CS_CLEAR_AND_FAIL: {
		CS_CLEAR_AND_FAIL_PACKET* p = reinterpret_cast<CS_CLEAR_AND_FAIL_PACKET*>(packet);
		switch (p->clear_type)
		{
		case 0:
			for (auto& client : clients) {
				if (client._id >= MAX_USER) break;
				if (client._state == ST_FREE || client._state == ST_ALLOC || client._state == ST_ML_AGENT) continue;
				if (client._stage == 2) {
					client._s_lock.lock();
					client.x = 34.f;
					client.y = 5.f;
					client.z = 10.f;
					client._s_lock.unlock();
					client.send_move_packet(client._id);
				}
			}
			break;
		case 1: {
			for (int i = MAX_USER; i < MAX_USER + MAX_NPC; ++i) {
				if (clients[i]._stage == 2) {
					disconnect(i);
				}
			}
			add_timer(c_id, chrono::system_clock::now() + 10s, EV_STAGE_CLEAR);
		}
			  break;
		default:
			break;
		}
		break;
	}
	default:
		break;
	}
}