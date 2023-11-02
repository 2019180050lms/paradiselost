#include "Timer.h"
#include "Network.h"
#include "Session.h"
#include <chrono>
#include <concurrent_priority_queue.h>
#include <mutex>
using namespace std;

concurrency::concurrent_priority_queue <EVENT> g_timer_queue;
mutex g_tl;

void add_timer(int o_id, chrono::system_clock::time_point exec_t, EVENT_TYPE et)
{
	EVENT n_ev{ o_id, exec_t, et };
	g_tl.lock();
	g_timer_queue.push(n_ev);
	g_tl.unlock();
}

void do_timer(HANDLE h_iocp)
{
	while (true) {
		EVENT ev;
		auto current_time = chrono::system_clock::now();
		if (true == g_timer_queue.try_pop(ev)) {
			if (ev._exec_time > current_time) {
				g_timer_queue.push(ev);
				this_thread::sleep_for(1ms);
				continue;
			}
			switch (ev._type) {
			case EV_RANDOM_MOVE: {
				OVER_EXP* exover = new OVER_EXP;
				exover->_comp_type = OP_NPC_AI;
				PostQueuedCompletionStatus(h_iocp, 1, ev._oid, &exover->_over);
				break;
			}
			case EV_ATTACK: {
				break;
			}
			case EV_HEAL: {
				break;
			}
			case EV_STAGE_CLEAR: {
				OVER_EXP* exover = new OVER_EXP;
				exover->_comp_type = OP_STAGE_CLEAR;
				PostQueuedCompletionStatus(h_iocp, 1, ev._oid, &exover->_over);
				break;
			}
			case EV_RESPAWN: {
				break;
			}
			case EV_SKILL_DELAY: {
				int p_id = ev._oid;
				clients[p_id]._sl.lock();
				if (clients[p_id]._skill_list.count(1))
					clients[p_id]._skill_list.erase(1);
				clients[p_id]._sl.unlock();
				if (clients[p_id].isAttack)
					clients[p_id].isAttack = false;
				break;
			}
			default:
				break;
			}

		}
	}
}
