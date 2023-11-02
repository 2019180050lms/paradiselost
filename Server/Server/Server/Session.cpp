#include "Session.h"
#include "protocol.h"
#include "DB.h"
#include "Timer.h"
#include <iostream>
using namespace std;

array<Session, MAX_USER + MAX_NPC> clients;

bool can_see(int from, int to)
{
	if (clients[from]._stage != clients[to]._stage) return false;
	if (abs(clients[from].x - clients[to].x) > VIEW_RANGE) return false;
	return abs(clients[from].z - clients[to].z) <= VIEW_RANGE;
}

bool is_pc(int o_id)
{
	return o_id < MAX_USER;
}

bool wakeup_npc(int oid)
{
	add_timer(oid, chrono::system_clock::now() + 1s, EV_RANDOM_MOVE);
	return true;
}

int get_new_client_id()
{
	for (int i = 0; i < MAX_USER; ++i) {
		lock_guard <mutex> ll{ clients[i]._s_lock };
		if (clients[i]._state == ST_FREE)
			return i;
	}
	return -1;
}

void disconnect(int c_id)
{
	clients[c_id]._vl.lock();
	unordered_set <int> vl = clients[c_id]._view_list;
	clients[c_id]._vl.unlock();
	for (auto& p_id : vl) {
		if (is_pc(p_id)) {
			auto& pl = clients[p_id];
			{
				lock_guard<mutex> ll(pl._s_lock);
				if (ST_INGAME != pl._state) continue;
			}
			if (pl._id == c_id) continue;
			pl.send_remove_player_packet(c_id);
		}
	}
	if (is_pc(c_id)) {
		closesocket(clients[c_id]._socket);
		_db_l.lock();
		db.update_user_data(clients[c_id]._name, clients[c_id]._name, &clients[c_id].x, &clients[c_id].y, &clients[c_id].z, &clients[c_id].exp, &clients[c_id].level, &clients[c_id].weapon_item, &clients[c_id]._stage);
		_db_l.unlock();
		clients[c_id]._stage = 0;
		clients[c_id]._quest_stage = -1;
		clients[c_id]._vl.lock();
		clients[c_id]._view_list.clear();
		clients[c_id]._vl.unlock();
	}
	if (c_id == MAX_USER + MAX_NPC - 1) {
		add_timer(c_id, chrono::system_clock::now() + 10s, EV_STAGE_CLEAR);
	}
	lock_guard<mutex> ll(clients[c_id]._s_lock);
	clients[c_id]._state = ST_FREE;
}

Session::Session() : _id(-1), _socket(0), x(1.f), y(1.f), z(1.f), head_item(-1), weapon_item(-1),
leg_item(-1), _state(ST_FREE), _prev_remain(0), targetId(-1), bossAttack(-1), my_max_x(0.f),
my_max_z(0.f), my_min_x(0.f), my_min_z(0.f), _stage(0), _quest_stage(-1) 
{
	_name[0] = 0;
}

void Session::do_recv()
{
	DWORD recv_flag = 0;
	memset(&_recv_over._over, 0, sizeof(_recv_over._over));
	_recv_over._wsabuf.len = BUF_SIZE - _prev_remain;
	_recv_over._wsabuf.buf = _recv_over._send_buf + _prev_remain;
	WSARecv(_socket, &_recv_over._wsabuf, 1, 0, &recv_flag,
		&_recv_over._over, 0);
}

void Session::do_send(void* packet)
{
	OVER_EXP* sdata = new OVER_EXP{ reinterpret_cast<char*>(packet) };
	WSASend(_socket, &sdata->_wsabuf, 1, 0, 0, &sdata->_over, 0);
}

void Session::send_login_info_packet(bool check)
{
	SC_CHAR_SELECT_PACKET p;
	p.size = sizeof(p);
	p.type = SC_LOGIN_INFO;
	p.success = check;
	p.c_type = 0;
	do_send(&p);
	//cout << "login info send: " << endl;
}

void Session::send_enter_game_packet()
{
	SC_LOGIN_INFO_PACKET p;
	p.size = sizeof(SC_LOGIN_INFO_PACKET);
	p.type = SC_ADD_PLAYER;
	p.id = _id;
	wcscpy_s(p.name, _name);
	p.c_type = _type;
	p.weapon = weapon_item;
	p.hp = 100;
	p.x = x;
	p.y = y;
	p.z = z;
	p.stage = _stage;
	do_send(&p);
}

void Session::send_item_info(int c_id, int itemType, int itemValue)
{
	SC_ITEM_INFO_PACKET p;
	p.size = sizeof(p);
	p.type = SC_ITEM_INFO;
	p.id = c_id;
	p.itemType = itemType;
	p.itemValue = itemValue;
	do_send(&p);
	cout << "item p_id: " << p.id << ", " << p.itemType << ", " << p.itemValue << endl;
}

void Session::send_move_packet(int c_id)
{
	SC_MOVE_PLAYER_PACKET p;
	p.id = c_id;
	p.size = sizeof(SC_MOVE_PLAYER_PACKET);
	p.dir = clients[c_id]._dir;
	p.type = SC_MOVE_PLAYER;
	p.hp = clients[c_id]._hp;
	p.x = clients[c_id].x;
	p.y = clients[c_id].y;
	p.z = clients[c_id].z;
	p.isAttack = clients[c_id].isAttack;
	p.isJump = clients[c_id].isJump;
	//p.move_time = clients[c_id].last_move_time;
	do_send(&p);
}

void Session::send_player_attacked_packet(int c_id) {
	SC_PLAYER_ATTACKED_PACKET p;
	p.size = sizeof(p);
	p.type = SC_PLAYER_ATTACKED;
	p.playerid = c_id;
	p.hp = clients[c_id]._hp;
	do_send(&p);
}

void Session::send_remove_player_packet(int c_id)
{
	_vl.lock();
	if (_view_list.count(c_id))
		_view_list.erase(c_id);
	else {
		_vl.unlock();
		return;
	}
	_vl.unlock();
	SC_REMOVE_PLAYER_PACKET p;
	p.id = c_id;
	p.size = sizeof(p);
	if (clients[c_id]._type > 3)
		p.itemNum = rand() % 2;
	else
		p.itemNum = 0;
	p.type = SC_REMOVE_PLAYER;
	do_send(&p);
}

void Session::send_add_player_packet(int c_id)
{
	SC_ADD_PLAYER_PACKET add_packet;
	add_packet.id = c_id;
	add_packet.c_type = clients[c_id]._type;
	add_packet.hp = clients[c_id]._hp;
	wcscpy_s(add_packet.name, clients[c_id]._name);
	add_packet.size = sizeof(clients[c_id]._name);
	add_packet.size = sizeof(add_packet);
	add_packet.type = SC_ENTER_PLAYER;
	add_packet.x = clients[c_id].x;
	add_packet.y = clients[c_id].y;
	add_packet.z = clients[c_id].z;
	add_packet.head_item = clients[c_id].head_item;
	add_packet.weapon_item = clients[c_id].weapon_item;
	add_packet.leg_item = clients[c_id].leg_item;
	add_packet.stage = clients[c_id]._stage;
	_vl.lock();
	_view_list.insert(c_id);
	_vl.unlock();
	do_send(&add_packet);
}

void Session::send_attacked_monster(int c_id)
{
	SC_ATTACKED_MONSTER_PACKET p;
	p.size = sizeof(p);
	p.type = SC_ATTACKED_MONSTER;
	p.id = c_id;
	p.hp = clients[c_id]._hp;
	do_send(&p);
}

void Session::send_boss_attack(int c_id)
{
	SC_BOSS_ATTACK_PACKET p;
	p.size = sizeof(p);
	p.type = SC_BOSS_ATTACK;
	p.playerid = clients[c_id].targetId;
	p.bossAttack = clients[c_id].bossAttack;
	do_send(&p);
}

void Session::run_astar(int n_id, int c_id)
{
	if (clients[n_id].openList.size() == 0) {
		SetAStarMap(clients[n_id]._stage, n_id);
		SetAStarStart((int)clients[n_id].x, (int)clients[n_id].z, n_id);
		SetAStarGoal((int)clients[c_id].x, (int)clients[c_id].z, n_id);
		int startRowInd = (int)clients[n_id].z;
		int startColInd = (int)clients[n_id].x;
		ASNode* startNode = new ASNode(startRowInd, startColInd, 'S', 'S');
		clients[n_id]._a_lock.lock();
		clients[n_id].openList.push_back(startNode);
		clients[n_id]._a_lock.unlock();
		FindPath(clients[n_id].openList, clients[n_id].closeList, n_id);
	}
	else {
		cout << "openList full: " << clients[n_id].openList.size() << endl;
	}
}

void Session::send_astar_move(int n_id)
{
	if (clients[n_id].openList.size() > 0 && clients[n_id].openList.empty() == false) {
		auto anode = clients[n_id].openList.front();
		SC_MOVE_PLAYER_PACKET p;
		p.id = n_id;
		p.size = sizeof(SC_MOVE_PLAYER_PACKET);
		p.dir = clients[n_id]._dir;
		p.type = SC_MOVE_PLAYER;
		p.hp = clients[n_id]._hp;
		p.x = anode->col;
		p.y = clients[n_id].y;
		p.z = anode->row;
		p.isAttack = clients[n_id].isAttack;
		p.isJump = clients[n_id].isJump;
		do_send(&p);
		clients[n_id].x = anode->col;
		clients[n_id].z = anode->row;
		clients[n_id]._a_lock.lock();
		if (clients[n_id].openList.empty() == false) {
			clients[n_id].openList.pop_front();
		}
		if (clients[n_id].closeList.empty() == false)
			clients[n_id].closeList.pop_front();
		clients[n_id]._a_lock.unlock();
		cout << n_id << " astar move pos: " << p.x << ", " << p.z << endl;
	}
}

void Session::send_stage_clear(int stage, int item)
{
	SC_STAGE_CLEAR_PACKET p;
	p.size = sizeof(p);
	p.type = SC_STAGE_CLEAR;
	p.stage = stage;
	p.item = item;
	do_send(&p);
	cout << _id << ": " << stage << " stage clear!" << endl;
}

void Session::send_setactive_object()
{
	SC_SETACTIVE_OBJECT_PACKET p;
	p.size = sizeof(p);
	p.type = SC_SETACTIVE_OBJECT;
	p._stage = _stage;
	do_send(&p);
}

void add_monster()
{
	cout << "NPC intialize begin.\n";
	for (int i = 0; i < MAX_NPC; ++i) {
		if (i < 12) {
			if (i < 4) {
				clients[MAX_USER + i]._hp = 100;
				clients[MAX_USER + i].x = 78.f;
				clients[MAX_USER + i].y = -1.2f;
				clients[MAX_USER + i].z = 39.f + i;
				clients[MAX_USER + i].my_max_x = 90.f;
				clients[MAX_USER + i].my_max_z = 65.f;
				clients[MAX_USER + i].my_min_x = 67.f;
				clients[MAX_USER + i].my_min_z = 22.f;
			}
			else if (i >= 4 && i < 8) {
				clients[MAX_USER + i]._hp = 100;
				clients[MAX_USER + i].x = 18.f + i;
				clients[MAX_USER + i].y = -1.2f;
				clients[MAX_USER + i].z = 56.f;
				clients[MAX_USER + i].my_max_x = 40.f;
				clients[MAX_USER + i].my_max_z = 67.f;
				clients[MAX_USER + i].my_min_x = 7.f;
				clients[MAX_USER + i].my_min_z = 44.f;
			}
			else if (i >= 8 && i < 12) {
				clients[MAX_USER + i]._hp = 100;
				clients[MAX_USER + i].x = 24.f + i;
				clients[MAX_USER + i].y = -1.2f;
				clients[MAX_USER + i].z = 93.f;
				clients[MAX_USER + i].my_max_x = 40.f;
				clients[MAX_USER + i].my_max_z = 102.f;
				clients[MAX_USER + i].my_min_x = 7.f;
				clients[MAX_USER + i].my_min_z = 80.f;
			}
			clients[MAX_USER + i]._type = HUMAN_ROBOT;
			clients[MAX_USER + i]._stage = 1;
			clients[MAX_USER + i]._id = MAX_USER + i;
			clients[MAX_USER + i]._socket = NULL;
			clients[MAX_USER + i].targetId = -1;
			clients[MAX_USER + i]._state = ST_INGAME;
		}
		else if (i >= 12 && i < 15) {
			if (i < 15) {
				clients[MAX_USER + i]._hp = 100;
				clients[MAX_USER + i].x = 35.f + i - 9;
				//clients[MAX_USER + i].y = 0.f;
				clients[MAX_USER + i].y = 0.2f;
				clients[MAX_USER + i].z = 39.f;
				clients[MAX_USER + i].my_max_x = 50.f;
				clients[MAX_USER + i].my_max_z = 51.f;
				clients[MAX_USER + i].my_min_x = 21.f;
				clients[MAX_USER + i].my_min_z = 32.f;
			}
			else if (i >= 16 && i < 20) {
				clients[MAX_USER + i]._hp = 100;
				clients[MAX_USER + i].x = 56.f;
				clients[MAX_USER + i].y = 100.f;
				clients[MAX_USER + i].z = 78.f;
				clients[MAX_USER + i].my_max_x = 63.f;
				clients[MAX_USER + i].my_max_z = 87.f;
				clients[MAX_USER + i].my_min_x = 46.f;
				clients[MAX_USER + i].my_min_z = 59.f;
			}
			else if (i >= 20 && i < 24) {
				clients[MAX_USER + i]._hp = 100;
				clients[MAX_USER + i].x = 43.f;
				clients[MAX_USER + i].y = 100.f; //19.03f;
				clients[MAX_USER + i].z = 73.f;
				clients[MAX_USER + i].my_max_x = 60.f;
				clients[MAX_USER + i].my_max_z = 90.f;
				clients[MAX_USER + i].my_min_x = 35.f;
				clients[MAX_USER + i].my_min_z = 57.f;
			}
			clients[MAX_USER + i]._type = GUN_ROBOT;
			clients[MAX_USER + i]._stage = 2;
			clients[MAX_USER + i]._id = MAX_USER + i;
			clients[MAX_USER + i]._socket = NULL;
			clients[MAX_USER + i].targetId = -1;
			clients[MAX_USER + i]._state = ST_INGAME;
		}
		else if (i >= 24 && i < 36) {
			if (i < 30) {
				clients[MAX_USER + i]._hp = 100;
				clients[MAX_USER + i].x = 74.f;
				clients[MAX_USER + i].y = 0.4f;
				clients[MAX_USER + i].z = 100.f;
				clients[MAX_USER + i].my_max_x = 89.f;
				clients[MAX_USER + i].my_max_z = 115.f;
				clients[MAX_USER + i].my_min_x = 59.f;
				clients[MAX_USER + i].my_min_z = 84.f;
			}
			else if (i >= 30 && i < 36) {
				clients[MAX_USER + i]._hp = 100;
				clients[MAX_USER + i].x = 128.f;
				clients[MAX_USER + i].y = -8.9f; //-8.f;
				clients[MAX_USER + i].z = 20.f;
				clients[MAX_USER + i].my_max_x = 165.f;
				clients[MAX_USER + i].my_max_z = 30.f;
				clients[MAX_USER + i].my_min_x = 95.f;
				clients[MAX_USER + i].my_min_z = 8.f;
			}
			if (i % 2 == 0)
				clients[MAX_USER + i]._type = HUMAN_ROBOT;
			else
				clients[MAX_USER + i]._type = GUN_ROBOT;
			clients[MAX_USER + i]._stage = 3;
			clients[MAX_USER + i]._id = MAX_USER + i;
			clients[MAX_USER + i]._socket = NULL;
			clients[MAX_USER + i].targetId = -1;
			clients[MAX_USER + i]._state = ST_INGAME;
		}
		else {
			clients[MAX_USER + i]._state = ST_FREE;
			clients[MAX_USER + i]._id = MAX_USER + i;
		}
		clients[MAX_USER + MAX_NPC - 1]._state = ST_FREE;
		wprintf_s(clients[MAX_USER + i]._name, "N%d", MAX_USER + i);
		lua_State* L = clients[MAX_USER + i]._L = luaL_newstate();
		luaL_openlibs(L);
		luaL_loadfile(L, "npc.lua");
		lua_pcall(L, 0, 0, 0);
		lua_getglobal(L, "set_uid");
		lua_pushnumber(L, MAX_USER);
		lua_pcall(L, 1, 0, 0);
		lua_pop(L, 1);
		lua_register(L, "API_SendMessage", API_SendMessage);
		lua_register(L, "API_get_x", API_get_x);
		lua_register(L, "API_get_y", API_get_y);
		lua_register(L, "API_add_timer", API_add_timer);
	}
	cout << "NPC initialize end.\n";
}

void do_add_boss(int c_id)
{
	int stage1 = 0, stage2 = 0, stage3 = 0;
	for (int i = MAX_USER; i < MAX_USER + MAX_NPC; ++i) {
		switch (clients[i]._stage) {
		case 1:
			if (clients[i]._state == ST_FREE) stage1++;
			break;
		case 2:
			if (clients[i]._state == ST_FREE) stage2++;
			break;
		case 3:
			if (clients[i]._state == ST_FREE) stage3++;
			break;
		}
	}

	if (stage1 == 12) {
		add_boss(1);
	}
	else if (stage2 == 12) {
		add_timer(c_id, chrono::system_clock::now() + 10s, EV_STAGE_CLEAR);
	}
	else if (stage3 == 12) {
		add_boss(3);
	}
}

void add_boss(short stage)
{
	switch (stage) {
	case 1: {
		clients[MAX_USER + MAX_NPC - 1]._id = MAX_USER + MAX_NPC - 1;
		clients[MAX_USER + MAX_NPC - 1]._type = STAGE1_BOSS;
		clients[MAX_USER + MAX_NPC - 1]._hp = 1000;
		clients[MAX_USER + MAX_NPC - 1].x = 169.f;
		clients[MAX_USER + MAX_NPC - 1].y = -1.f;
		clients[MAX_USER + MAX_NPC - 1].z = 98.f;
		clients[MAX_USER + MAX_NPC - 1].my_max_x = 181.f;
		clients[MAX_USER + MAX_NPC - 1].my_max_z = 115.f;
		clients[MAX_USER + MAX_NPC - 1].my_min_x = 152.f;
		clients[MAX_USER + MAX_NPC - 1].my_min_z = 80.f;
		clients[MAX_USER + MAX_NPC - 1]._dir = 4;
		clients[MAX_USER + MAX_NPC - 1]._stage = 1;
		clients[MAX_USER + MAX_NPC - 1]._socket = NULL;
		clients[MAX_USER + MAX_NPC - 1].targetId = -1;
		clients[MAX_USER + MAX_NPC - 1].bossAttack = -1;
		wprintf_s(clients[MAX_USER + MAX_NPC - 1]._name, "N%d", MAX_USER + MAX_NPC - 1);
		clients[MAX_USER + MAX_NPC - 1]._state = ST_INGAME;
		break;
	}
	case 3: {
		clients[MAX_USER + MAX_NPC - 1]._id = MAX_USER + MAX_NPC - 1;
		clients[MAX_USER + MAX_NPC - 1]._type = BOSS_TEST;
		clients[MAX_USER + MAX_NPC - 1]._hp = 1000;
		clients[MAX_USER + MAX_NPC - 1].x = 156.f;
		clients[MAX_USER + MAX_NPC - 1].y = 0.f;
		clients[MAX_USER + MAX_NPC - 1].z = 102.f;
		clients[MAX_USER + MAX_NPC - 1]._dir = 4;
		clients[MAX_USER + MAX_NPC - 1]._stage = 3;
		clients[MAX_USER + MAX_NPC - 1]._socket = NULL;
		clients[MAX_USER + MAX_NPC - 1].targetId = -1;
		clients[MAX_USER + MAX_NPC - 1].bossAttack = -1;
		wprintf_s(clients[MAX_USER + MAX_NPC - 1]._name, "N%d", MAX_USER + MAX_NPC - 1);
		clients[MAX_USER + MAX_NPC - 1]._state = ST_INGAME;
		break;
	}
	default:
		break;
	}
}

void do_random_move(int c_id)
{
	if (c_id < 0)
		return;
	/*
	if (clients[c_id]._stage == 2) {
		for (auto& client : clients) {
			if (client._state != ST_INGAME) continue;
			if(can_see(client._id, c_id)) continue;
			if (client._id > MAX_USER) break;
			if (clients[c_id].x + 40.f < client.x && client.x - 40.f > client.x) {
				if (clients[c_id].z + 40.f < clients[c_id].z && client.z - 40.f > client.z) {
					do_player_attack(c_id, client._id);
					break;
				}
			}
		}
		return;
	}
	*/
	unordered_set<int> view_list;
	for (auto& cl : clients) {
		if (cl._id >= MAX_USER) break;
		if (cl._state != ST_INGAME) continue;
		if (cl._id == c_id) continue;
		if (can_see(c_id, cl._id))
			view_list.insert(cl._id);
	}

	if (clients[c_id]._type != 7) {
		float x = clients[c_id].x;
		float y = clients[c_id].y;
		float z = clients[c_id].z;
		int dir = rand() % 8;
		switch (dir) {
		case 0: {
			if (z > clients[c_id].my_min_z) {
				z -= 1.5f;
				clients[c_id]._dir = dir;
			}
			break;
		}
		case 1: {
			if (z < clients[c_id].my_max_z) {
				z += 1.5f;
				clients[c_id]._dir = dir;
			}
			break;
		}
		case 2: {
			if (x > clients[c_id].my_min_x) {
				x -= 1.5f;
				clients[c_id]._dir = dir;
			}
			break;
		}
		case 3: {
			if (x < clients[c_id].my_max_x) {
				x += 1.5f;
				clients[c_id]._dir = dir;
			}
			break;
		}
		case 4: {
			if (x < clients[c_id].my_max_x && z < clients[c_id].my_max_z) {
				x += 1.f;
				z += 1.f;
				clients[c_id]._dir = dir;
			}
			break;
		}
		case 5: {
			if (x > clients[c_id].my_min_x && z < clients[c_id].my_max_z) {
				x -= 1.f;
				z += 1.f;
				clients[c_id]._dir = dir;
			}
			break;
		}
		case 6: {
			if (x < clients[c_id].my_max_x && z > clients[c_id].my_min_z) {
				x += 1.f;
				z -= 1.f;
				clients[c_id]._dir = dir;
			}
			break;
		}
		case 7: {
			if (x > clients[c_id].my_min_x && z > clients[c_id].my_min_z) {
				x -= 1.f;
				z -= 1.f;
				clients[c_id]._dir = dir;
			}
			break;
		}
		default:
			clients[c_id]._dir = 0;
			break;
		}
		clients[c_id].x = x;
		clients[c_id].y = y;
		clients[c_id].z = z;
	}

	unordered_set<int> near_list;

	for (auto& cl : clients) {
		if (cl._id >= MAX_USER) break;
		if (cl._state != ST_INGAME) continue;
		if (cl._id == c_id) continue;
		if (can_see(c_id, cl._id))
			near_list.insert(cl._id);
	}

	for (auto& pl : near_list) {
		auto& cpl = clients[pl];
		cpl._vl.lock();
		if (clients[pl]._view_list.count(c_id) && clients[pl]._state == ST_INGAME) {
			cpl._vl.unlock();
			if (clients[c_id]._type != 7)
				clients[pl].send_move_packet(c_id);
			else {
				clients[c_id].bossAttack = rand() % 2;
				clients[c_id].targetId = 0;
				clients[pl].send_boss_attack(c_id);
			}
		}
		else {
			cpl._vl.unlock();
			if (clients[c_id]._state != ST_FREE) {
				clients[pl].send_add_player_packet(c_id);
			}
		}
	}

	for (auto& pl : view_list)
		if (0 == near_list.count(pl)) {
			clients[pl].send_remove_player_packet(c_id);
		}
	//cout << "monster pos(random move): " << clients[c_id].x << ", " << clients[c_id].y << ", " << clients[c_id].z << endl;
}

void do_player_attack(int n_id, int c_id)
{
	if (c_id < 0 || n_id < 0 || clients[n_id]._state == ST_FREE)
		return;

	unordered_set<int> view_list;
	for (auto& cl : clients) {
		if (cl._id >= MAX_USER) break;
		if (cl._state != ST_INGAME) continue;
		if (cl._id == n_id) continue;
		if (can_see(n_id, cl._id))
			view_list.insert(cl._id);
	}
	switch (clients[n_id]._type)
	{
	case MONSTER::GUN_ROBOT:
	{
		if (clients[n_id].x + 10.f >= clients[c_id].x && clients[n_id].x - 10.f <= clients[c_id].x) {
			if (clients[n_id].z + 10.f >= clients[c_id].z && clients[n_id].z - 10.f <= clients[c_id].z) {
				clients[n_id].isAttack = true;
				if (clients[c_id].x > clients[n_id].x) {
					clients[n_id].x += 1.5f;
					clients[n_id]._dir = 1;
				}
				else if (clients[c_id].z > clients[n_id].z) {
					clients[n_id].z += 1.5f;
					clients[n_id]._dir = 3;
				}
				else if (clients[c_id].x < clients[n_id].x) {
					clients[n_id].x -= 1.5f;
					clients[n_id]._dir = 2;
				}
				else if (clients[c_id].z < clients[n_id].z) {
					clients[n_id].z -= 1.5f;
					clients[n_id]._dir = 4;
				}
				//cout << "n_id: " << n_id << ", " << clients[n_id].isAttack << endl;
			}
		}
		else if (clients[n_id].x + 40.f < clients[c_id].x && clients[n_id].x - 40.f > clients[c_id].x) {
			if (clients[n_id].z + 40.f < clients[c_id].z && clients[n_id].z - 40.f > clients[c_id].z) {
				clients[n_id].isAttack = false;
				clients[n_id].targetId = -1;
			}
		}
		else {
			clients[n_id].isAttack = false;
		}

		if (!clients[n_id].isAttack) {
			if (clients[n_id].x < clients[c_id].x) {
				clients[n_id].x += 1.5f;
				clients[n_id]._dir = 1;
			}
			else if (clients[n_id].x > clients[c_id].x) {
				clients[n_id].x -= 1.5f;
				clients[n_id]._dir = 2;
			}

			if (clients[n_id].z < clients[c_id].z) {
				clients[n_id].z += 1.5f;
				clients[n_id]._dir = 3;
			}
			else if (clients[n_id].z > clients[c_id].z) {
				clients[n_id].z -= 1.5f;
				clients[n_id]._dir = 4;
			}
			if (clients[n_id].x < clients[c_id].x
				&& clients[n_id].z < clients[c_id].z) {
				clients[n_id].x += 1.5f;
				clients[n_id].z += 1.5f;
				clients[n_id]._dir = 5;
			}
			else if (clients[n_id].x < clients[c_id].x
				&& clients[n_id].z > clients[c_id].z) {
				clients[n_id].x += 1.5f;
				clients[n_id].z -= 1.5f;
				clients[n_id]._dir = 6;
			}
			else if (clients[n_id].x > clients[c_id].x
				&& clients[n_id].z < clients[c_id].z) {
				clients[n_id].x -= 1.5f;
				clients[n_id].z += 1.5f;
				clients[n_id]._dir = 7;
			}
			else if (clients[n_id].x > clients[c_id].x
				&& clients[n_id].z > clients[c_id].z) {
				clients[n_id].x -= 1.5f;
				clients[n_id].z -= 1.5f;
				clients[n_id]._dir = 8;
			}
		}
		break;
	}
	case MONSTER::HUMAN_ROBOT:
	{
		if (clients[n_id].x + 1.5f >= clients[c_id].x && clients[n_id].x - 1.5f <= clients[c_id].x) {
			if (clients[n_id].z + 1.5f >= clients[c_id].z && clients[n_id].z - 1.5f <= clients[c_id].z) {
				clients[n_id].isAttack = true;
				//cout << "n_id: " << n_id << ", " << clients[n_id].isAttack << endl;
			}
			else {
				clients[n_id].isAttack = false;
			}
		}
		else if (clients[n_id].x + 40.f < clients[c_id].x && clients[n_id].x - 40.f > clients[c_id].x) {
			if (clients[n_id].z + 40.f < clients[c_id].z && clients[n_id].z - 40.f > clients[c_id].z) {
				clients[n_id].isAttack = false;
				clients[n_id].targetId = -1;
			}
		}
		else {
			clients[n_id].isAttack = false;
		}

		if (!clients[n_id].isAttack) {
			if (clients[n_id].x < clients[c_id].x) {
				if (clients[n_id].z > clients[c_id].z) {
					clients[n_id].x += 1.5f;
					clients[n_id].z -= 1.5f;
					clients[n_id]._dir = 6;
				}
				else if (clients[n_id].z < clients[c_id].z) {
					clients[n_id].x += 1.5f;
					clients[n_id].z += 1.5f;
					clients[n_id]._dir = 5;
				}
				else {
					clients[n_id].x += 1.5f;
					clients[n_id]._dir = 1;
				}
			}
			else if (clients[n_id].x > clients[c_id].x) {
				if (clients[n_id].z < clients[c_id].z) {
					clients[n_id].x -= 1.5f;
					clients[n_id].z += 1.5f;
					clients[n_id]._dir = 7;
				}
				else if (clients[n_id].z > clients[c_id].z) {
					clients[n_id].x -= 1.5f;
					clients[n_id].z -= 1.5f;
					clients[n_id]._dir = 8;
				}
				else {
					clients[n_id].x -= 1.5f;
					clients[n_id]._dir = 2;
				}
			}
		}
		break;
	}
	case MONSTER::BOSS_TEST:
	{
		if (clients[n_id].x + 5.f >= clients[c_id].x && clients[n_id].x - 5.f <= clients[c_id].x) {
			if (clients[n_id].z + 5.f >= clients[c_id].z && clients[n_id].z - 5.f <= clients[c_id].z) {
				clients[n_id].isAttack = true;
				clients[n_id].bossAttack = 1;
				//cout << "n_id: " << n_id << ", " << clients[n_id].isAttack << endl;
			}
		}
		else if (clients[n_id].x + 25.f >= clients[c_id].x && clients[n_id].x - 25.f <= clients[c_id].x) {
			if (clients[n_id].z + 25.f >= clients[c_id].z && clients[n_id].z - 25.f <= clients[c_id].z) {
				clients[n_id].isAttack = true;
				clients[n_id].bossAttack = 2;
				//cout << "n_id: " << n_id << ", " << clients[n_id].isAttack << endl;
			}
		}
		else if (clients[n_id].x + 30.f < clients[c_id].x && clients[n_id].x - 30.f > clients[c_id].x) {
			if (clients[n_id].z + 30.f < clients[c_id].z && clients[n_id].z - 30.f > clients[c_id].z) {
				clients[n_id].isAttack = false;
				clients[n_id].targetId = -1;
			}
		}
		else {
			clients[n_id].isAttack = false;
		}
		break;
	}
	case MONSTER::STAGE1_BOSS:
	{
		if (clients[n_id].x + 3.f >= clients[c_id].x && clients[n_id].x - 3.f <= clients[c_id].x) {
			if (clients[n_id].z + 3.f >= clients[c_id].z && clients[n_id].z - 3.f <= clients[c_id].z) {
				clients[n_id].isAttack = true;
				clients[n_id].bossAttack = 1;
				//cout << "n_id: " << n_id << ", " << clients[n_id].isAttack << endl;
			}
		}
		else if (clients[n_id].x + 10.f >= clients[c_id].x && clients[n_id].x - 10.f <= clients[c_id].x) {
			if (clients[n_id].z + 10.f >= clients[c_id].z && clients[n_id].z - 10.f <= clients[c_id].z) {
				clients[n_id].isAttack = true;
				clients[n_id].bossAttack = 2;
				//cout << "n_id: " << n_id << ", " << clients[n_id].isAttack << endl;
			}
		}
		else if (clients[n_id].x + 20.f < clients[c_id].x && clients[n_id].x - 20.f > clients[c_id].x) {
			if (clients[n_id].z + 20.f < clients[c_id].z && clients[n_id].z - 20.f > clients[c_id].z) {
				clients[n_id].isAttack = false;
				clients[n_id].targetId = -1;
			}
		}
		else {
			clients[n_id].isAttack = false;
		}

		if (!clients[n_id].isAttack) {
			if (clients[n_id].x < clients[c_id].x) {
				clients[n_id].x += 3.f;
				clients[n_id]._dir = 1;
			}
			else if (clients[n_id].x > clients[c_id].x) {
				clients[n_id].x -= 3.f;
				clients[n_id]._dir = 2;
			}

			if (clients[n_id].z < clients[c_id].z) {
				clients[n_id].z += 3.f;
				clients[n_id]._dir = 3;
			}
			else if (clients[n_id].z > clients[c_id].z) {
				clients[n_id].z -= 3.f;
				clients[n_id]._dir = 4;
			}
			if (clients[n_id].x < clients[c_id].x
				&& clients[n_id].z < clients[c_id].z) {
				clients[n_id].x += 1.5f;
				clients[n_id].z += 1.5f;
				clients[n_id]._dir = 5;
			}
			else if (clients[n_id].x < clients[c_id].x
				&& clients[n_id].z > clients[c_id].z) {
				clients[n_id].x += 1.5f;
				clients[n_id].z -= 1.5f;
				clients[n_id]._dir = 6;
			}
			else if (clients[n_id].x > clients[c_id].x
				&& clients[n_id].z < clients[c_id].z) {
				clients[n_id].x -= 1.5f;
				clients[n_id].z += 1.5f;
				clients[n_id]._dir = 7;
			}
			else if (clients[n_id].x > clients[c_id].x
				&& clients[n_id].z > clients[c_id].z) {
				clients[n_id].x -= 1.5f;
				clients[n_id].z -= 1.5f;
				clients[n_id]._dir = 8;
			}
		}
		break;
	}
	default:
		break;
	}


	/*
	switch (clients[n_id]._type)
	{
	case HUMAN_ROBOT: {
		if (clients[n_id].x + 1.f >= clients[c_id].x && clients[n_id].x - 1.f <= clients[c_id].x) {
			if (clients[n_id].z + 1.f >= clients[c_id].z && clients[n_id].z - 1.f <= clients[c_id].z) {
				clients[n_id].isAttack = true;
			}
		}
		else {
			clients[n_id].run_astar(n_id, c_id);
		}
		break;
	}
	case GUN_ROBOT: {
		if (clients[n_id].x + 15.f >= clients[c_id].x && clients[n_id].x - 15.f <= clients[c_id].x) {
			if (clients[n_id].z + 15.f >= clients[c_id].z && clients[n_id].z - 15.f <= clients[c_id].z) {
				clients[n_id].isAttack = true;
			}
		}
		else {
			clients[n_id].run_astar(n_id, c_id);
		}
		break;
	}
	default:
		break;
	}
	*/

	unordered_set<int> near_list;

	for (auto& cl : clients) {
		if (cl._id >= MAX_USER) break;
		if (cl._state != ST_INGAME) continue;
		if (cl._id == n_id) continue;
		if (can_see(n_id, cl._id))
			near_list.insert(cl._id);
	}

	for (auto& pl : near_list) {
		auto& cpl = clients[pl];
		cpl._vl.lock();
		if (clients[pl]._view_list.count(n_id) && clients[pl]._state == ST_INGAME) {
			cpl._vl.unlock();
			if (clients[n_id]._type != 7 && clients[n_id]._type != 4)
				clients[pl].send_move_packet(n_id);
			else {
				if (clients[n_id].isAttack) {
					clients[n_id].targetId = c_id;
					clients[pl].send_boss_attack(n_id);
				}
				else {
					clients[pl].send_move_packet(n_id);
				}
			}
		}
		else
			cpl._vl.unlock();
	}
	for (auto& pl : view_list)
		if (0 == near_list.count(pl)) {
			clients[pl].send_remove_player_packet(n_id);
		}
}

void do_delay_disable(int n_id, int c_id)
{
	if (c_id < 0 || n_id < 0)
		return;
	unordered_set<int> view_list;
	for (auto& cl : clients) {
		if (cl._id >= MAX_USER) break;
		if (cl._state != ST_INGAME) continue;
		if (cl._id == n_id) continue;
		if (can_see(n_id, cl._id))
			view_list.insert(cl._id);
	}
	if (clients[n_id]._type != 7) {
		if (clients[n_id].x + 1.5f >= clients[c_id].x && clients[n_id].x - 1.5f <= clients[c_id].x) {
			if (clients[n_id].z + 1.5f >= clients[c_id].z && clients[n_id].z - 1.5f <= clients[c_id].z) {
				clients[n_id].isAttack = true;
				//cout << "n_id: " << n_id << ", " << clients[n_id].isAttack << endl;
			}
			else {
				clients[n_id].isAttack = false;
			}
		}
		else if (clients[n_id].x + 15.f < clients[c_id].x || clients[n_id].x - 15.f > clients[c_id].x) {
			if (clients[n_id].z + 15.f < clients[c_id].z || clients[n_id].z - 15.f > clients[c_id].z) {
				clients[n_id].isAttack = false;
				clients[n_id].targetId = -1;
			}
		}
		else {
			clients[n_id].isAttack = false;
		}
	}


	unordered_set<int> near_list;

	//cout << "player id: " << c_id << ", pos(" << clients[c_id].x << ", " << clients[c_id].y << ", " << clients[c_id].z << endl;
	//cout << "monster id: " << n_id << ", pos(" << clients[n_id].x << ", " << clients[n_id].y << ", " << clients[n_id].z << endl;

	for (auto& cl : clients) {
		if (cl._id >= MAX_USER) break;
		if (cl._state != ST_INGAME) continue;
		if (cl._id == n_id) continue;
		if (can_see(n_id, cl._id))
			near_list.insert(cl._id);
	}

	for (auto& pl : near_list) {
		auto& cpl = clients[pl];
		cpl._vl.lock();
		if (clients[pl]._view_list.count(n_id) && clients[pl]._state == ST_INGAME) {
			cpl._vl.unlock();
			if (clients[n_id]._type != 7)
				clients[pl].send_move_packet(n_id);
			else {
				clients[n_id].bossAttack = rand() % 2;
				clients[n_id].targetId = c_id;
				clients[pl].send_boss_attack(n_id);
			}
		}
		else
			cpl._vl.unlock();
	}

	for (auto& pl : view_list)
		if (0 == near_list.count(pl)) {
			clients[pl].send_remove_player_packet(n_id);
		}
}

int API_get_x(lua_State* L)
{
	int user_id =
		(int)lua_tointeger(L, -1);
	lua_pop(L, 2);
	int x = clients[user_id].x;
	lua_pushnumber(L, x);
	return 1;
}

int API_add_timer(lua_State* L)
{
	int user_id = (int)lua_tointeger(L, -1);
	lua_pop(L, 2);
	//add_timer(user_id, chrono::system_clock::now() + 3s, EV_BYE);
	return 1;
}

int API_get_y(lua_State* L)
{
	int user_id =
		(int)lua_tointeger(L, -1);
	lua_pop(L, 2);
	int y = clients[user_id].y;
	lua_pushnumber(L, y);
	return 1;
}

int API_SendMessage(lua_State* L)
{
	int my_id = (int)lua_tointeger(L, -3);
	int user_id = (int)lua_tointeger(L, -2);
	char* mess = (char*)lua_tostring(L, -1);

	lua_pop(L, 4);

	//clients[user_id].send_chat_packet(my_id, mess);
	return 0;
}