#pragma once
#include <WS2tcpip.h>
#include <MSWSock.h>

constexpr int BUF_SIZE{ 8192 };
constexpr int NAME_SIZE = 20;

enum COMP_TYPE {
	OP_CONNECT, 
	OP_DISCONNECT, 
	OP_ACCEPT, 
	OP_RECV, 
	OP_SEND, 
	OP_NPC_AI, 
	OP_BOSS_AI, 
	OP_STAGE_CLEAR,
};

class OVER_EXP {
public:
	WSAOVERLAPPED _over;
	WSABUF _wsabuf;
	char _send_buf[BUF_SIZE];
	COMP_TYPE _comp_type;
	OVER_EXP();
	OVER_EXP(char* packet);
};

extern SOCKET g_s_socket, g_c_socket;
extern OVER_EXP g_a_over;

void worker_thread(HANDLE h_iocp);
void process_packet(int c_id, char* packet);