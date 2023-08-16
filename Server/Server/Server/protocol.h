constexpr int PORT_NUM = 7777;
constexpr int BUF_SIZE = 8192;
constexpr int NAME_SIZE = 20;

constexpr int MAX_USER = 500;
constexpr int MAX_NPC = 37;

constexpr int W_WIDTH = 500;
constexpr int W_HEIGHT = 500;

constexpr int TEXT_WIDTH = 200;
constexpr int TEXT_HEIGHT = 200;

constexpr char WALL = '1';

// Packet ID
constexpr char CS_LOGIN = 0;
constexpr char CS_MOVE = 6;
constexpr char CS_ENTER_GAME = 4;
constexpr char CS_MONSTER_ATTACKED = 12;
constexpr char CS_EQUIP_ITEM = 16;
constexpr char CS_NPC = 17;
constexpr char CS_PORTAL = 23;
constexpr char CS_PLAYER_ATTACKED = 24;
constexpr char CS_MONSTER_AI = 25;			// ml_agent

constexpr char SC_LOGIN_INFO = 3;
constexpr char SC_ENTER_PLAYER = 8;
constexpr char SC_ADD_PLAYER = 7;
constexpr char SC_REMOVE_PLAYER = 9;
constexpr char SC_MOVE_PLAYER = 10;
constexpr char SC_ATTACKED_MONSTER = 14;
constexpr char SC_ITEM_INFO = 15;
constexpr char SC_BOSS_ATTACK = 20;
constexpr char SC_NPC = 21;
constexpr char SC_STAGE_CLEAR = 22;
constexpr char SC_PLAYER_ATTACKED = 18;
//constexpr char SC_ITEM_INFO = 

enum MONSTER { STAGE1_BOSS = 4, GUN_ROBOT = 5, HUMAN_ROBOT = 6, BOSS_TEST = 7 };

#pragma pack (push, 1)
struct CS_LOGIN_PACKET {
	unsigned short size;
	unsigned short type;
	//char	name[NAME_SIZE];
	//std::wstring name;
	WCHAR name[NAME_SIZE] = { NULL };
};

struct SC_CHAR_SELECT_PACKET {
	unsigned short size;
	unsigned short type;
	bool success;
	int c_type;
};

struct CS_CREATE_PLAYER_PACKET {
	unsigned short size;
	unsigned short type;
	int c_type;
	WCHAR name[NAME_SIZE] = { NULL };
};

struct CS_MOVE_PACKET {
	unsigned short size;
	unsigned short type;
	int id;
	int	direction;  // 0 : UP, 1 : DOWN, 2 : LEFT, 3 : RIGHT
	short hp;
	float x, y, z;
	bool isAttack;
	bool isJump;
	//unsigned	move_time;
};

struct CS_MONSTER_AI_PACKET {
	unsigned short size;
	unsigned short type;
	int id;
	int	direction;  // 0 : UP, 1 : DOWN, 2 : LEFT, 3 : RIGHT
	short hp;
	float x, y, z;
	bool isAttack;
	bool isJump;
	//unsigned	move_time;
};

struct CS_MONSTER_ATTACKED_PACKET {
	unsigned short size;
	unsigned short type;
	int id;
	short hp;
	int playerId;
	bool hitEnemy;
};

struct CS_EQUIP_ITEM_PACKET {
	unsigned short size;
	unsigned short type;
	int id;
	unsigned short itemType;
	unsigned short itemValue;
};

struct CS_NPC_PACKET {
	unsigned short size;
	unsigned short type;
	bool active;
	short _quest_stage;
};

struct CS_PORTAL_PACKET {
	unsigned short size;
	unsigned short type;
	int stage;
};

struct CS_PLAYER_ATTACKED_PACKET {
	unsigned short size;
	unsigned short type;
	int playerid;
	int monsterid;
};

struct SC_LOGIN_INFO_PACKET {
	unsigned short size;
	unsigned short type;
	short id;
	int c_type;
	int weapon;
	short hp;
	float	x, y, z;
	int stage;
	WCHAR	name[NAME_SIZE + 2];
};

struct SC_ADD_PLAYER_PACKET {
	unsigned short size;
	unsigned short type;
	int	id;
	int	c_type;
	short hp;
	float x, y, z;
	short head_item, weapon_item, leg_item;
	WCHAR	name[NAME_SIZE + 2];
};

struct SC_REMOVE_PLAYER_PACKET {
	unsigned short size;
	unsigned short type;
	int	id;
	int itemNum;
};

struct SC_MOVE_PLAYER_PACKET {
	unsigned short size;
	unsigned short type;
	int	id;
	int dir;
	short hp;
	float	x, y, z;
	bool isAttack, isJump;
	short bossAttack;
	//unsigned int move_time;
};

struct SC_ATTACKED_MONSTER_PACKET {
	unsigned short size;
	unsigned short type;
	int id;
	short hp;
};

struct SC_NPC_PACKET {
	unsigned short size;
	unsigned short type;
	short stage;
	bool active;
	short monster_count;
};

struct SC_BOSS_ATTACK_PACKET {
	unsigned short size;
	unsigned short type;
	int playerid;
	short bossAttack;
};

struct SC_ITEM_INFO_PACKET {
	unsigned short size;
	unsigned short type;
	int id;
	unsigned short itemType;
	unsigned short itemValue;
};

struct SC_STAGE_CLEAR_PACKET {
	unsigned short size;
	unsigned short type;
	int stage;
	int item;
};

struct SC_PLAYER_ATTACKED_PACKET {
	unsigned short size;
	unsigned short type;
	int playerid;
	short hp;
};

#pragma pack (pop)