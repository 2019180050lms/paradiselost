constexpr int PORT_NUM = 7777;
constexpr int BUF_SIZE = 8192;
constexpr int NAME_SIZE = 20;

constexpr int MAX_USER = 500;

constexpr int W_WIDTH = 50;
constexpr int W_HEIGHT = 50;

// Packet ID
constexpr char CS_LOGIN = 0;
constexpr char CS_MOVE = 6;
constexpr char CS_ENTER_GAME = 4;
constexpr char CS_MONSTER_ATTACKED = 12;
constexpr char CS_EQUIP_ITEM = 16;

constexpr char SC_LOGIN_INFO = 3;
constexpr char SC_ENTER_PLAYER = 8;
constexpr char SC_ADD_PLAYER = 7;
constexpr char SC_REMOVE_PLAYER = 9;
constexpr char SC_MOVE_PLAYER = 10;
constexpr char SC_ATTACKED_MONSTER = 14;
constexpr char SC_ITEM_INFO = 15;
constexpr char SC_BOSS_ATTACK = 20;
//constexpr char SC_ITEM_INFO = 

#pragma pack (push, 1)
struct CS_LOGIN_PACKET {
	unsigned short size;
	unsigned short type;
	char	name[NAME_SIZE];
};

struct SC_CHAR_SELECT_PACKET {
	unsigned short size;
	unsigned short type;
	bool success;
	int c_type;
};

struct CS_ENTER_GAME_PACKET {
	unsigned short size;
	unsigned short type;
	int playerindex;
	int c_type;
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

struct SC_LOGIN_INFO_PACKET {
	unsigned short size;
	unsigned short type;
	short id;
	int c_type;
	short hp;
	float	x, y, z;
};

struct SC_ADD_PLAYER_PACKET {
	unsigned short size;
	unsigned short type;
	int	id;
	int	c_type;
	short hp;
	float x, y, z;
	short head_item, weapon_item, leg_item;
	//char	name[NAME_SIZE];
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

#pragma pack (pop)