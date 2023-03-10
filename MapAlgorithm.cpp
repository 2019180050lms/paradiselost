#include<iostream>
#include<cmath>
#include<array>
#include<algorithm>
#include <random>
using std::endl;
using std::cout;

std::mt19937 engine((unsigned int)time(NULL));
int divide_num;
int intmap[50][50];

class TreeNode
{
private:
	int x;
	int y;
	int width;
	int height;
public:
	TreeNode* left_node;
	TreeNode* right_node;

	TreeNode()
	{
		x = 0;
		y = 0;
		width = 0;
		height = 0;
		left_node = NULL;
		right_node = NULL;
	}

	TreeNode(int _x, int _y, int _width, int _height)
	{
		x = _x;
		y = _y;
		width = _width;
		height = _height;
		left_node = NULL;
		right_node = NULL;
	}

	int GetX()
	{
		return x;
	}

	int GetY()
	{
		return y;
	}

	int GetWidth()
	{
		return width;
	}

	int GetHeight()
	{
		return height;
	}

	void SetX(int _x)
	{
		x = _x;
	}

	void SetY(int _y)
	{
		y = _y;
	}

	void SetWidth(int _width)
	{
		width = _width;
	}

	void SetHeight(int _height)
	{
		height = _height;
	}
};


void Divide_Dungeon(int depth, TreeNode* reaf)
{
	if (depth == divide_num)
	{
		for (int i = reaf->GetX() + 1; i < reaf->GetX() + reaf->GetWidth() - 1; i++)
		{
			for (int j = reaf->GetY() + 1; j < reaf->GetY() + reaf->GetHeight() - 1; j++)
			{
				intmap[i][j] = 4;
			}
		}
	}


	if (depth < divide_num)
	{
		int length = reaf->GetWidth() >= reaf->GetHeight() ? reaf->GetWidth() : reaf->GetHeight();
		std::uniform_int_distribution<int> split(length * 0.4, length * 0.6);
		int divide = split(engine);
		cout << "divide : " << divide << endl;
		if (reaf->GetWidth() >= reaf->GetHeight())
		{
			
			reaf->left_node = new TreeNode(reaf->GetX(), reaf->GetY(), divide, reaf->GetHeight());
			reaf->right_node = new TreeNode(reaf->GetX() + divide, reaf->GetY(), reaf->GetWidth() - divide, reaf->GetHeight());
			cout << "x :" << reaf->right_node->GetX() << ", " << "y : " << reaf->right_node->GetY() << endl;
			cout << "witdth : " << reaf->right_node->GetWidth() << ", height :" << reaf->right_node->GetHeight() << endl;
		}
		else
		{
			reaf->left_node = new TreeNode(reaf->GetX(), reaf->GetY(), reaf->GetWidth(), divide);
			reaf->right_node = new TreeNode(reaf->GetX(), reaf->GetY() + divide, reaf->GetWidth(), reaf->GetHeight() - divide);
			cout << "x :" << reaf->right_node->GetX() << ", " << "y : " << reaf->right_node->GetY() << endl;
			cout << "witdth : " << reaf->right_node->GetWidth() << ", height :" <<  reaf->right_node->GetHeight() << endl;
		}

		Divide_Dungeon(depth + 1, reaf->left_node);
		Divide_Dungeon(depth + 1, reaf->right_node);
	}

}

void CreateDunGeon(TreeNode* reaf)
{
	if (reaf->left_node == NULL || reaf->right_node == NULL)
	{
		cout << "makeD" << endl;
		intmap[reaf->GetX()][reaf->GetY()] = 4;
	}



	if (reaf->left_node != NULL)
	{
		CreateDunGeon(reaf->left_node);
	}
	else if (reaf->right_node != NULL)
	{
		CreateDunGeon(reaf->right_node);
	}

}

void PrintMap()
{
	for (int i = 0; i < 50; i++)
	{
		for (int j = 0; j < 50; j++)
		{
			cout << intmap[j][i];
		}
		cout << endl;
	}
}
int main()
{
	//1. 2차 배열을 만든다.
	divide_num = 4;
	TreeNode *root = new TreeNode(0, 0, 50, 50);
	memset(intmap, 0, sizeof(intmap));
	Divide_Dungeon(0, root);
	CreateDunGeon(root);
	PrintMap();
	


	return 0;
}