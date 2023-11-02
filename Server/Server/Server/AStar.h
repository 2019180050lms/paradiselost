#pragma once
#include <list>

class ASNode {
public:
	ASNode* conn;
	int row, col;
	int g, h, f;
	char nodeName;
	char value;

public:
	ASNode(int _x, int _y, char _v, int _i);
};

ASNode* GetChildNodes(int childIndRow, int childIndCol, ASNode* parentNode, std::list<ASNode*>& openList, std::list<ASNode*>& closeList, int n_id);
ASNode* CreateNodeByIndex(int rowIndex, int colIndex, ASNode* parentNode, int n_id);

void FindPath(std::list<ASNode*>& openList, std::list<ASNode*>& closeList, int n_id);

void SetAStarMap(int _stage, int n_id);
void SetAStarStart(int _x, int _z, int n_id);
void SetAStarGoal(int _x, int _z, int n_id);