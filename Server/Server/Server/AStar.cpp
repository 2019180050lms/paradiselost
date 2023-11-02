#include "AStar.h"
#include <iostream>
#include <list>
#include <array>
#include <algorithm>
#include <tuple>
#include "Session.h"

static char uniqueName = 'a';

ASNode::ASNode(int _x, int _y, char _v, int _i) : row(_x), col(_y), value(_v), nodeName(_i), g{}, h{}, f{}
{
}

std::tuple<int, int> GetGoalIndex(int n_id)
{

	int maxMapSizeRow = sizeof(clients[n_id].astar_map) / sizeof(clients[n_id].astar_map[0]);
	int maxMapSizeCol = sizeof(clients[n_id].astar_map[0]);
	for (int i = 0; i < maxMapSizeRow; ++i) {
		for (int j = 0; j < maxMapSizeCol; ++j) {
			if (clients[n_id].astar_map[i][j] == 'G') {
				return std::make_tuple(i, j);
			}
		}
	}
}

void DebugPrintList(std::list<ASNode*>& nodelist, std::string name)
{
	std::cout << name.c_str() << ":" << std::endl;
	//cout << "list length:" << nodelist.size() << endl;
	for (auto& ele : nodelist)
	{
		std::cout << "(" << ele->row << "," << ele->col << ")";
	}
	std::cout << std::endl;
}

void DebugPrintElement(ASNode* curNode, ASNode* parent)
{
	std::cout << "set parent:" << "(" << (curNode)->row << "," << (curNode)->col << ")->(" << (parent)->row << "," << (parent)->col << ")" << std::endl;
}

void FindPath(std::list<ASNode*>& openList, std::list<ASNode*>& closeList, int n_id)
{//DebugPrintList(openList, "Open");
	//DebugPrintList(closeList, "Close");
	int maxMapSizeRow = sizeof(clients[n_id].astar_map) / sizeof(clients[n_id].astar_map[0]);
	int maxMapSizeCol = sizeof(clients[n_id].astar_map[0]);
	//int maxMapSizeRow = W_HEIGHT;
	//int maxMapSizeCol = W_WIDTH;
	//cout << "maxMapSizeRow:" << maxMapSizeRow << ", maxMapSizeCol:" << maxMapSizeCol <<endl;
	if (openList.size() == 0 || openList.size() > 100)
	{
		//end of finding.  no route to goal
		std::cout << "no path exists." << std::endl;
		return;
	}
	ASNode* openNode = nullptr;
	int smallest_f = 10000;
	for (auto& op : openList)
	{
		if (op->f < smallest_f)
		{
			smallest_f = op->f;
			openNode = op;
		}
	}
	if (openNode != nullptr)
	{
		if (openNode->nodeName == 'G') //arrive at Goal
		{
			std::cout << "< Optimal Path (row, column)>" << std::endl;
			while (openNode != nullptr)
			{
				std::cout << "(" << openNode->row << "," << openNode->col << ")";
				int rowind = openNode->row;
				int colind = openNode->col;
				//clients[n_id].astar_map[rowind][colind] = '*';
				openNode = openNode->conn;
				if (openNode != nullptr)
					std::cout << "<==";
			}
			std::cout << std::endl;
			for (int i = 0; i < 500; ++i) {
				for (int j = 0; j < 500; ++j) {
					if (clients[n_id].astar_map[i][j] == 'S' || clients[n_id].astar_map[i][j] == 'G') {
						clients[n_id].astar_map[i][j] = '0';
					}
				}
			}
			//ShowMap();
		}
		else
		{
			//Get children nodes from the current node
			//check 4-way, up,down,left,right
			int rowInd = openNode->row;
			int colInd = openNode->col;
			//check up
			ASNode* childNode;
			if (openNode->row - 1 >= 0)
			{
				int childIndRow = openNode->row - 1;
				int childIndCol = openNode->col;
				childNode = GetChildNodes(childIndRow, childIndCol, openNode, openList, closeList, n_id);
			}
			if (openNode->row + 1 < maxMapSizeRow)
			{
				int childIndRow = openNode->row + 1;
				int childIndCol = openNode->col;
				childNode = GetChildNodes(childIndRow, childIndCol, openNode, openList, closeList, n_id);
			}
			if (openNode->col + 1 < maxMapSizeCol)
			{
				int childIndRow = openNode->row;
				int childIndCol = openNode->col + 1;
				childNode = GetChildNodes(childIndRow, childIndCol, openNode, openList, closeList, n_id);
			}
			if (openNode->col - 1 >= 0)
			{
				int childIndRow = openNode->row;
				int childIndCol = openNode->col - 1;
				childNode = GetChildNodes(childIndRow, childIndCol, openNode, openList, closeList, n_id);
			}
			//cout << "[Remove from openlist] (" << rowInd << "," << colInd << ")" << endl;
			clients[n_id]._a_lock.lock();
			openList.remove_if([&](ASNode* node)
				{
					if (node->row == rowInd && node->col == colInd)
					{
						return true;
					}
					else
					{
						return false;
					}
				});
			//cout << "[push] to closeList:" << rowInd << "," << openNode->col << endl;
			closeList.push_back(openNode);
			clients[n_id]._a_lock.unlock();
			FindPath(openList, closeList, n_id);
		}
	}
}

ASNode* GetChildNodes(int childIndRow, int childIndCol, ASNode* parentNode, std::list<ASNode*>& openList, std::list<ASNode*>& closeList, int n_id)
{
	auto it_open = find_if(openList.begin(), openList.end(), [&](ASNode* node)
		{
			if (node->row == childIndRow && node->col == childIndCol)
			{
				return true;
			}
			else
			{
				return false;
			}
		});
	auto it_close = find_if(closeList.begin(), closeList.end(), [&](ASNode* node)
		{
			if (node->row == childIndRow && node->col == childIndCol)
			{
				return true;
			}
			else
			{
				return false;
			}
		});
	if (it_open != openList.end())
	{
		//exist
		if ((*it_open)->g < parentNode->g + 1)
		{
			(*it_open)->g = parentNode->g + 1;
			parentNode->conn = (*it_open);
			(*it_open)->f = (*it_open)->g + (*it_open)->h;
			//cout << "[parenting openlist]";
			//DebugPrintElement(*it_open, parentNode);
		}
		return *it_open;
	}
	else if (it_close != closeList.end())
	{
		//exist
		if ((*it_close)->g < parentNode->g + 1)
		{
			(*it_close)->g = parentNode->g + 1;
			parentNode->conn = (*it_close);
			(*it_close)->f = (*it_close)->g + (*it_close)->h;
			/*	cout << "[parenting closelist]";
						DebugPrintElement(*it_close, parentNode);*/
		}
		return *it_close;
	}
	else
	{
		ASNode* newNode = CreateNodeByIndex(childIndRow, childIndCol, parentNode, n_id);
		if (newNode != nullptr)
		{
			//cout << "[push] to openlist:" << newNode->row << "," << newNode->col << endl;
			clients[n_id]._a_lock.lock();
			openList.push_back(newNode);
			clients[n_id]._a_lock.unlock();
		}
		return newNode;
	}
	return nullptr;
}

ASNode* CreateNodeByIndex(int rowIndex, int colIndex, ASNode* parentNode, int n_id)
{
	char val = clients[n_id].astar_map[rowIndex][colIndex];
	if (val == '1')
		return nullptr;
	ASNode* node = nullptr;
	if (val == 'G')
	{
		node = new ASNode(rowIndex, colIndex, 'G', 'G');
		node->g = parentNode->g + 1;
		node->h = 0;
		node->f = node->g;
		node->conn = parentNode;
	}
	else
	{
		node = new ASNode(rowIndex, colIndex, val, uniqueName++);
		node->g = parentNode->g + 1;
		auto inds = GetGoalIndex(n_id);
		int goalRowInd = std::get<0>(inds);
		int goalColInd = std::get<1>(inds);
		int h = abs(goalRowInd - rowIndex) + abs(goalColInd - colIndex);
		node->h = h;
		node->f = node->g + h;
		node->conn = parentNode;
	}
	return node;
}


void SetAStarMap(int _stage, int n_id)
{
	switch (_stage) {
	case 1: {
		for (int i = 0; i < 500; ++i) {
			for (int j = 0; j < 500; ++j) {
				clients[n_id].astar_map[i][j] = stage1[i][j];
			}
		}
		break;
	}
	case 2: {
		for (int i = 0; i < 500; ++i) {
			for (int j = 0; j < 500; ++j) {
				clients[n_id].astar_map[i][j] = stage1[i][j];
			}
		}
		break;
	}
	case 3: {
		for (int i = 0; i < 500; ++i) {
			for (int j = 0; j < 500; ++j) {
				clients[n_id].astar_map[i][j] = stage1[i][j];
			}
		}
		break;
	}
	default:
		break;
	}
}

void SetAStarStart(int _x, int _z, int n_id)
{
	clients[n_id].astar_map[_z][_x] = 'S';
}

void SetAStarGoal(int _x, int _z, int n_id)
{
	clients[n_id].astar_map[_z][_x] = 'G';
}