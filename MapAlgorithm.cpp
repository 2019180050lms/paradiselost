#include<iostream>
#include<cmath>
#include<array>
#include<cstdlib>
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
	/*if (depth == divide_num)
	{
		for (int i = reaf->GetX() + 1; i < reaf->GetX() + reaf->GetWidth() - 1; i++)
		{
			for (int j = reaf->GetY() + 1; j < reaf->GetY() + reaf->GetHeight() - 1; j++)
			{
				intmap[i][j] = 4;
			}
		}
	}*/


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




	/*if (reaf->left_node != NULL)
	{
		CreateDunGeon(reaf->left_node);
	}*/

	
	if (reaf->left_node == NULL || reaf->right_node == NULL)
	{
		std::bernoulli_distribution Create_Room_Per(0.5);
		if (Create_Room_Per(engine))
		{
			cout << "makeD" << endl;
			for (int i = reaf->GetX() + 1; i < reaf->GetX() + reaf->GetWidth() - 1; i++)
			{
				for (int j = reaf->GetY() + 1; j < reaf->GetY() + reaf->GetHeight() - 1; j++)
				{
					intmap[i][j] = 4;
				}
			}
		}
		return;
	}

	

	/*if (reaf->right_node != NULL)
	{
		CreateDunGeon(reaf->right_node);
	}*/
	CreateDunGeon(reaf->left_node);
	CreateDunGeon(reaf->right_node);




}

void CreateRoad(TreeNode* reaf)
{
	if (reaf->left_node == NULL && reaf->right_node == NULL)
	{
		return;
	}

	//일단은 기존 알고리즘대로
	int x1 = reaf->left_node->GetX() + reaf->left_node->GetWidth() / 2;
	int x2 = reaf->right_node->GetX() + reaf->right_node->GetWidth() / 2;
	int y1 = reaf->left_node->GetY() + reaf->left_node->GetHeight() / 2;
	int y2 = reaf->right_node->GetY() + reaf->right_node->GetHeight() / 2;

	for (int i = std::min(x1, x2); i <= std::max(x1, x2); i++)
	{
		intmap[i][(y1 + y2) / 2] = 5;
	}

	for (int i = std::min(y1, y2); i <= std::max(y1, y2); i++)
	{
		intmap[(x1 + x2)/2][i] = 5;
	}

	CreateRoad(reaf->left_node);
	CreateRoad(reaf->right_node);
}

void PrintMap()
{
	for (int i = 0; i < 50; i++)
	{
		for (int j = 0; j < 50; j++)
		{
			if (intmap[i][j] == 4 || intmap[i][j] == 5)
			{
				cout << intmap[i][j];
			}
			else
			{
				cout << " ";
			}
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
	CreateRoad(root);
	PrintMap();
	


	return 0;
}