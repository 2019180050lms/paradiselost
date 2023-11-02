#pragma once
#include "Network.h"
#include "AStar.h"
#include "Map.h"
#include <list>
#include <unordered_set>
#include <array>
#include <mutex>

#include "include/lua.hpp"
#pragma comment(lib, "WS2_32.lib")
#pragma comment(lib, "MSWSock.lib")
#pragma comment(lib, "lua54.lib")
using namespace std;

enum S_STATE { ST_FREE, ST_ALLOC, ST_INGAME, ST_ML_AGENT };

constexpr int VIEW_RANGE = 1000;
constexpr int MAX_USER = 500;
constexpr int MAX_NPC = 37;

class Session {
	OVER_EXP _recv_over;

public:
	mutex _s_lock;
	S_STATE _state;
	mutex _stage_lock;
	bool	_is_active;				// NPC만 유효
	bool _delay_attack;				// NPC만 유효
	int _id;
	int _type;
	int _dir;
	int _stage;					// 포탈 이동
	short _quest_stage;				// 몇 번째 퀘스트 인지
	mutex _a_lock;
	std::list<ASNode*> openList;	// astar 사용
	std::list<ASNode*> closeList;	// astar 사용
	char astar_map[W_HEIGHT][W_WIDTH];	// astar 사용
	//array<array<char, W_HEIGHT>, W_WIDTH> astar_map;
	SOCKET _socket;
	int head_item, weapon_item, leg_item;
	int exp, level;
	short _hp;
	float	x, y, z;
	float	my_max_x, my_max_z, my_min_x, my_min_z;
	bool isAttack, isJump;
	short bossAttack;
	int targetId;
	WCHAR	_name[NAME_SIZE];
	int		_prev_remain;
	unordered_set <int> _view_list;
	unordered_set <int> _skill_list;
	mutex	_vl;
	mutex	_sl;
	int		last_move_time;
	lua_State* _L;
	mutex _ll;
public:
	Session();

	~Session() {}
	
	//
	void do_recv();
	void do_send(void* packet);

	//
	void send_login_info_packet(bool check);
	void send_enter_game_packet();
	void send_move_packet(int c_id);
	void send_add_player_packet(int c_id);
	void send_remove_player_packet(int c_id);
	void send_player_attacked_packet(int c_id);
	void send_item_info(int c_id, int itemType, int itemValue);
	void send_attacked_monster(int c_id);
	void send_boss_attack(int c_id);
	void send_stage_clear(int stage, int item);
	void run_astar(int n_id, int c_id);
	void send_astar_move(int n_id);
	void send_setactive_object();
};

extern array<Session, MAX_USER + MAX_NPC> clients;

bool can_see(int, int);
bool is_pc(int);
bool wakeup_npc(int);

void do_player_attack(int, int);
void do_delay_disable(int, int);

int API_get_x(lua_State* L);
int API_get_y(lua_State* L);
int API_add_timer(lua_State* L);
int API_SendMessage(lua_State* L);

int get_new_client_id();

void disconnect(int c_id);
void do_random_move(int);
void add_boss(short);

void add_monster();

void do_add_boss(int);