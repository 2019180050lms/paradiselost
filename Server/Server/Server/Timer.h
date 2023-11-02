#pragma once
#include <concurrent_priority_queue.h>
#include <mutex>
using namespace std;

enum EVENT_TYPE { EV_RANDOM_MOVE, EV_HEAL, EV_ATTACK, EV_BOSS, EV_RESPAWN, EV_BOSS_MOVE, EV_STAGE_CLEAR, EV_SKILL_DELAY };

struct EVENT {
	int _oid;
	chrono::system_clock::time_point _exec_time;
	EVENT_TYPE _type;
	int target_id;
	constexpr bool operator < (const EVENT& _Left) const
	{
		return (_exec_time > _Left._exec_time);
	}
};

extern concurrency::concurrent_priority_queue <EVENT> g_timer_queue;
extern mutex g_tl;

void do_timer(HANDLE h_iocp);
void add_timer(int o_id, chrono::system_clock::time_point exec_t, EVENT_TYPE et);