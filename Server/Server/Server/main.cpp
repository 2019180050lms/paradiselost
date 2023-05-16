#include <iostream>
#include <array>
#include <WS2tcpip.h>
#include <MSWSock.h>
#include <thread>
#include <vector>
#include <mutex>
#include <unordered_set>
#include <chrono>
#include <queue>
#include "protocol.h"

#pragma comment(lib, "WS2_32.lib")
#pragma comment(lib, "MSWSock.lib")
using namespace std;

constexpr int VIEW_RANGE = 100;
constexpr int MAX_NPC = 3;

enum COMP_TYPE { OP_CONNECT, OP_DISCONNECT, OP_ACCEPT, OP_RECV, OP_SEND, OP_NPC_AI };
class OVER_EXP {
public:
	WSAOVERLAPPED _over;
	WSABUF _wsabuf;
	char _send_buf[BUF_SIZE];
	COMP_TYPE _comp_type;
	OVER_EXP()
	{
		_wsabuf.len = BUF_SIZE;
		_wsabuf.buf = _send_buf;
		_comp_type = OP_RECV;
		ZeroMemory(&_over, sizeof(_over));
	}
	OVER_EXP(char* packet)
	{
		_wsabuf.len = packet[0];
		_wsabuf.buf = _send_buf;
		ZeroMemory(&_over, sizeof(_over));
		_comp_type = OP_SEND;
		memcpy(_send_buf, packet, packet[0]);
	}
};

enum S_STATE { ST_FREE, ST_ALLOC, ST_INGAME };

class SESSION {
	OVER_EXP _recv_over;

public:
	mutex _s_lock;
	S_STATE _state;
	bool	_is_active;				// NPC만 유효
	bool _delay_attack;				// NPC만 유효
	int _id;
	int _type;
	int _dir;
	SOCKET _socket;
	short head_item, weapon_item, leg_item;
	short _hp;
	float	x, y, z;
	bool isAttack, isJump;
	int targetId;
	char	_name[NAME_SIZE];
	int		_prev_remain;
	unordered_set <int> _view_list;
	mutex	_vl;
	int		last_move_time;
public:
	SESSION()
	{
		_id = -1;
		_socket = 0;
		x = y = z = 1.f;
		head_item = weapon_item = leg_item = -1;
		_name[0] = 0;
		_state = ST_FREE;
		_prev_remain = 0;
		targetId = -1;
	}

	~SESSION() {}

	void do_recv()
	{
		DWORD recv_flag = 0;
		memset(&_recv_over._over, 0, sizeof(_recv_over._over));
		_recv_over._wsabuf.len = BUF_SIZE - _prev_remain;
		_recv_over._wsabuf.buf = _recv_over._send_buf + _prev_remain;
		WSARecv(_socket, &_recv_over._wsabuf, 1, 0, &recv_flag,
			&_recv_over._over, 0);
	}

	void do_send(void* packet)
	{
		OVER_EXP* sdata = new OVER_EXP{ reinterpret_cast<char*>(packet) };
		WSASend(_socket, &sdata->_wsabuf, 1, 0, 0, &sdata->_over, 0);
	}
	void send_login_info_packet()
	{
		SC_CHAR_SELECT_PACKET p;
		p.size = sizeof(p);
		p.type = SC_LOGIN_INFO;
		p.success = true;
		p.c_type = 0;
		do_send(&p);
		//cout << "login info send: " << endl;
	}
	void send_enter_game_packet()
	{
		SC_LOGIN_INFO_PACKET p;
		p.size = sizeof(SC_LOGIN_INFO_PACKET);
		p.type = SC_ADD_PLAYER;
		p.id = _id;
		p.c_type = _type;
		p.hp = 100;
		p.x = x;
		p.y = y;
		p.z = z;
		do_send(&p);
		//cout << "send enter game: " << p.id << ", " << p.c_type << ", ";
	}
	void send_move_packet(int c_id);
	void send_add_player_packet(int c_id);
	void send_remove_player_packet(int c_id)
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
		p.itemNum = 0;
		p.type = SC_REMOVE_PLAYER;
		do_send(&p);
	}
	void send_item_info(int c_id, int itemType, int itemValue)
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
	void send_attacked_monster(int c_id);
};

array<SESSION, MAX_USER + MAX_NPC> clients;

SOCKET g_s_socket, g_c_socket;
OVER_EXP g_a_over;

enum EVENT_TYPE { EV_RANDOM_MOVE, EV_HEAL, EV_ATTACK };

struct EVENT {
	int _oid;
	chrono::system_clock::time_point _exec_time;
	EVENT_TYPE _type;
	constexpr bool operator < (const EVENT& _Left) const
	{
		return (_exec_time > _Left._exec_time);
	}

};

priority_queue <EVENT> g_timer_queue;
mutex g_tl;

void do_random_move(int o_id);

bool can_see(int from, int to)
{
	if (abs(clients[from].x - clients[to].x) > VIEW_RANGE) return false;
	return abs(clients[from].z - clients[to].z) <= VIEW_RANGE;
}

bool is_pc(int o_id)
{
	return o_id < MAX_USER;
}

void add_timer(int o_id, chrono::system_clock::time_point exec_t, EVENT_TYPE et)
{
	EVENT n_ev{ o_id, exec_t, et };
	g_tl.lock();
	g_timer_queue.push(n_ev);
	g_tl.unlock();
}

bool wakeup_npc(int oid)
{
	add_timer(oid, chrono::system_clock::now() + 1s, EV_RANDOM_MOVE);
	return true;
}

void SESSION::send_move_packet(int c_id)
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
	//cout << "send move: " << p.id << ", " << p.dir << endl;
}

void SESSION::send_add_player_packet(int c_id)
{
	SC_ADD_PLAYER_PACKET add_packet;
	add_packet.id = c_id;
	add_packet.c_type = clients[c_id]._type;
	add_packet.hp = clients[c_id]._hp;
	//strcpy_s(add_packet.name, clients[c_id]._name);
	add_packet.size = sizeof(add_packet);
	add_packet.type = SC_ENTER_PLAYER;
	add_packet.x = clients[c_id].x;
	add_packet.y = clients[c_id].y;
	add_packet.z = clients[c_id].z;
	add_packet.head_item = clients[c_id].head_item;
	add_packet.weapon_item = clients[c_id].weapon_item;
	add_packet.leg_item = clients[c_id].leg_item;
	_vl.lock();
	_view_list.insert(c_id);
	_vl.unlock();
	do_send(&add_packet);
	cout << "add player send: " << add_packet.id << ", " << add_packet.c_type << ", " << add_packet.weapon_item << endl;
}

void SESSION::send_attacked_monster(int c_id)
{
	SC_ATTACKED_MONSTER_PACKET p;
	p.size = sizeof(p);
	p.type = SC_ATTACKED_MONSTER;
	p.id = c_id;
	p.hp = clients[c_id]._hp;
	do_send(&p);
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

void disconnect(int c_id);
void do_player_attack(int n_id, int c_id);

void process_packet(int c_id, char* packet)
{
	switch (packet[2]) {
	case CS_LOGIN: {
		CS_LOGIN_PACKET* p = reinterpret_cast<CS_LOGIN_PACKET*>(packet);
		strcpy_s(clients[c_id]._name, p->name);
		clients[c_id].send_login_info_packet();
		break;
	}
	case CS_ENTER_GAME: {
		CS_ENTER_GAME_PACKET* p = reinterpret_cast<CS_ENTER_GAME_PACKET*>(packet);
		clients[c_id]._type = p->c_type;
		//cout << "client send: " << p->size << ", " << p->c_type << ", " << p->playerindex << endl;
		{
			lock_guard<mutex> ll{ clients[c_id]._s_lock };
			clients[c_id].x = 1.f;
			clients[c_id].y = 1.f;
			clients[c_id].z = 1.f;
			clients[c_id]._state = ST_INGAME;
		}
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
			if (is_pc(pl._id)) pl.send_add_player_packet(c_id);
			else wakeup_npc(pl._id);
			clients[c_id].send_add_player_packet(pl._id);
		}
		break;
	}
	case CS_MOVE: {
		CS_MOVE_PACKET* p = reinterpret_cast<CS_MOVE_PACKET*>(packet);
		//clients[c_id].last_move_time = p->move_time;
		clients[c_id]._hp = p->hp;
		clients[c_id]._dir = p->direction;
		clients[c_id].x = p->x;
		clients[c_id].y = p->y;
		clients[c_id].z = p->z;
		clients[c_id].isAttack = p->isAttack;
		clients[c_id].isJump = p->isJump;

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

		clients[c_id].send_move_packet(c_id);

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
				if (false == is_pc(pl))
					wakeup_npc(pl);
			}
		}

		for (auto& pl : old_vlist)
			if (0 == near_list.count(pl)) {
				clients[c_id].send_remove_player_packet(pl);
				if (is_pc(pl))
					clients[pl].send_remove_player_packet(c_id);
			}
		break;
	}
	case CS_MONSTER_ATTACKED: {
		CS_MONSTER_ATTACKED_PACKET* p = reinterpret_cast<CS_MONSTER_ATTACKED_PACKET*>(packet);
		clients[p->id]._hp = p->hp;
		clients[p->id].targetId = p->playerId;
		clients[p->id].isAttack = true;
		if (clients[p->id]._hp < 1)
			disconnect(p->id);
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
				}
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
		default:
			cout << "Unknown packet (error)" << endl;
			disconnect(c_id);
			break;
		}
		clients[c_id].send_item_info(c_id, p->itemType, clients[c_id].weapon_item);
		cout << "c_id: " << c_id << ", " << clients[c_id].weapon_item << endl;
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
					cout << "pl id: " << pl << endl;
				}
				else {
					cpl._vl.unlock();
				}
			}
		}
		break;
	}
	default:
		break;
	}
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
	closesocket(clients[c_id]._socket);

	lock_guard<mutex> ll(clients[c_id]._s_lock);
	clients[c_id]._state = ST_FREE;
}

void do_delay_disable(int n_id);

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
				clients[client_id].y = 0;
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
				if (clients[i]._state != ST_INGAME)  continue;
				if (false == can_see(i, key)) continue;
				deactivate = false;
				if (!clients[key].isAttack)
					do_random_move(static_cast<int>(key));
				else if (clients[key].isAttack && !clients[key]._delay_attack)
					do_player_attack(static_cast<int>(key), clients[key].targetId);
				else if (clients[key].isAttack && clients[key]._delay_attack)
					do_delay_disable(static_cast<int>(key));

				break;
			}

			if (false == deactivate && !clients[key].isAttack) {
				add_timer(key, chrono::system_clock::now() + 500ms, EV_RANDOM_MOVE);
			}
			else if (false == deactivate && clients[key].isAttack && !clients[key]._delay_attack) {
				add_timer(key, chrono::system_clock::now() + 1s, EV_RANDOM_MOVE);
			}
			else if (false == deactivate && clients[key].isAttack && clients[key]._delay_attack) {
				add_timer(key, chrono::system_clock::now() + 300ms, EV_RANDOM_MOVE);
			}
			break;
		}
		}
	}
}

void InitializeNPC()
{
	float z = 0.f;
	for (int i = MAX_USER; i < MAX_USER + MAX_NPC; ++i) {
		clients[i]._hp = 500;
		clients[i].x = 1.f;
		clients[i].y = 1.f;
		clients[i].z = 1.f + z;
		clients[i]._id = i;
		clients[i]._type = 5;
		clients[i]._socket = NULL;
		clients[i].targetId = -1;
		sprintf_s(clients[i]._name, "N%d", i);
		clients[i]._state = ST_INGAME;
		z += 10.f;
	}
	z = 0;
	//cout << "complete" << endl;
}

void do_random_move(int c_id)
{
	unordered_set<int> view_list;
	for (auto& cl : clients) {
		if (cl._id >= MAX_USER) break;
		if (cl._state != ST_INGAME) continue;
		if (cl._id == c_id) continue;
		if (can_see(c_id, cl._id))
			view_list.insert(cl._id);
	}

	float x = clients[c_id].x;
	float y = clients[c_id].y;
	float z = clients[c_id].z;
	int dir = rand() % 9;
	switch (dir) {
	case 0: if (z > 0) z -= 3.f; break;
	case 1: if (z < W_HEIGHT - 1) z += 3.f; break;
	case 2: if (x > 0) x -= 3.f; break;
	case 3: if (x < W_WIDTH - 1) x += 3.f; break;
	case 4: if (x < W_WIDTH && z < W_HEIGHT) { x += 1.5; z += 1.5; } break;
	case 5: if (x > 0 && z < W_HEIGHT) { x -= 1.5; z += 1.5; } break;
	case 6: if (x < W_WIDTH && z > 0) { x += 1.5; z -= 1.5; } break;
	case 7: if (x > 0 && z > 0) { x -= 1.5; z -= 1.5; } break;
	case 8: break;
	}
	clients[c_id].x = x;
	clients[c_id].y = y;
	clients[c_id].z = z;
	clients[c_id]._dir = dir;

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
		{
			cpl._vl.lock();
			if (clients[pl]._view_list.count(c_id) && clients[pl]._state == ST_INGAME) {
				cpl._vl.unlock();
				clients[pl].send_move_packet(c_id);
			}
			else {
				cpl._vl.unlock();
				if (clients[c_id]._state != ST_FREE)
					clients[pl].send_add_player_packet(c_id);
			}
		}
	}

	for (auto& pl : view_list)
		if (0 == near_list.count(pl)) {
			clients[pl].send_remove_player_packet(c_id);
		}
}

void do_player_attack(int n_id, int c_id)
{
	if (clients[n_id]._delay_attack)
		return;

	unordered_set<int> view_list;
	for (auto& cl : clients) {
		if (cl._id >= MAX_USER) break;
		if (cl._state != ST_INGAME) continue;
		if (cl._id == n_id) continue;
		if (can_see(n_id, cl._id))
			view_list.insert(cl._id);
	}

	if (clients[n_id].x + 4.f < clients[c_id].x && clients[n_id].x - 4.f > clients[c_id].x)
		if (clients[n_id].z + 4.f < clients[c_id].z && clients[n_id].z - 4.f > clients[c_id].z) {
			clients[n_id].isAttack = true;
			clients[n_id]._delay_attack = true;
		}
	if (clients[n_id].x > clients[c_id].x + 7.f || clients[n_id].x < clients[c_id].x - 7.f)
		if (clients[n_id].z > clients[c_id].z + 7.f || clients[n_id].z < clients[c_id].z - 7.f)
			clients[n_id].isAttack = false;

	if (!clients[n_id]._delay_attack) {
		if (clients[n_id].x < clients[c_id].x) {
			clients[n_id].x += 2.f;
		}
		else if (clients[n_id].x > clients[c_id].x) {
			clients[n_id].x -= 2.f;
		}

		if (clients[n_id].z < clients[c_id].z) {
			clients[n_id].z += 2.f;
		}
		else if (clients[n_id].z > clients[c_id].z) {
			clients[n_id].z -= 2.f;
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
		{
			cpl._vl.lock();
			if (clients[pl]._view_list.count(n_id) && clients[pl]._state == ST_INGAME) {
				cpl._vl.unlock();
				clients[pl].send_move_packet(n_id);
			}
			else
				cpl._vl.unlock();
		}
	}
}

void do_delay_disable(int n_id)
{
	clients[n_id]._delay_attack = false;
}

void do_timer(HANDLE h_iocp)
{
	while (true) {
		g_tl.lock();
		if (g_timer_queue.empty() == true) {
			g_tl.unlock();
			//this_thread::sleep_for(10ms);
			continue;
		}
		auto ev = g_timer_queue.top();
		if (ev._exec_time > chrono::system_clock::now()) {
			g_tl.unlock();
			//this_thread::sleep_for(10ms);
			continue;
		}
		g_timer_queue.pop();
		g_tl.unlock();
		OVER_EXP* exover = new OVER_EXP;
		exover->_comp_type = OP_NPC_AI;
		PostQueuedCompletionStatus(h_iocp, 1, ev._oid, &exover->_over);
	}
}

int main()
{
	HANDLE h_iocp;

	InitializeNPC();

	WSADATA WSAData;
	WSAStartup(MAKEWORD(2, 2), &WSAData);
	g_s_socket = WSASocket(AF_INET, SOCK_STREAM, 0, NULL, 0, WSA_FLAG_OVERLAPPED);
	SOCKADDR_IN server_addr;
	memset(&server_addr, 0, sizeof(server_addr));
	server_addr.sin_family = AF_INET;
	server_addr.sin_port = htons(PORT_NUM);
	server_addr.sin_addr.S_un.S_addr = INADDR_ANY;
	bind(g_s_socket, reinterpret_cast<sockaddr*>(&server_addr), sizeof(server_addr));
	listen(g_s_socket, SOMAXCONN);
	SOCKADDR_IN cl_addr;
	int addr_size = sizeof(cl_addr);

	h_iocp = CreateIoCompletionPort(INVALID_HANDLE_VALUE, 0, 0, 0);
	CreateIoCompletionPort(reinterpret_cast<HANDLE>(g_s_socket), h_iocp, 9999, 0);
	g_c_socket = WSASocket(AF_INET, SOCK_STREAM, 0, NULL, 0, WSA_FLAG_OVERLAPPED);
	g_a_over._comp_type = OP_ACCEPT;
	AcceptEx(g_s_socket, g_c_socket, g_a_over._send_buf, 0, addr_size + 16, addr_size + 16, 0, &g_a_over._over);

	// thread ai_thread{ do_ai };
	thread timer_thread{ do_timer, h_iocp };
	vector <thread> worker_threads;
	int num_threads = std::thread::hardware_concurrency();
	for (int i = 0; i < num_threads; ++i)
		worker_threads.emplace_back(worker_thread, h_iocp);
	for (auto& th : worker_threads)
		th.join();
	// ai_thread.join();
	timer_thread.join();
	closesocket(g_s_socket);
	WSACleanup();
}
