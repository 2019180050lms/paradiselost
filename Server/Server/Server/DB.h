#pragma once
#include <windows.h>
#include <iostream>
#include <format>
#include <string>

#include <sqlext.h>

#define NAME_LEN 20

class DB
{
private:
	SQLHENV henv;
	SQLHDBC hdbc;
	SQLHSTMT hstmt = 0;
	SQLRETURN retcode;

public:
	DB() {}
	~DB() {}

	void show_error(SQLHANDLE hHandle, SQLSMALLINT hType, RETCODE RetCode);
	void DBConnect();
	void DBDisconnect();

	bool check_user_id(wchar_t* id);
	void get_user_data(wchar_t* id, wchar_t* name, float* x, float* y, float* z, int* exp, int* level, int* weapon, int* type, int* stage);
	void add_user_data(wchar_t* id, wchar_t* name, float* x, float* y, float* z, int* exp, int* level, int* weapon, int* type, int* stage);
	void update_user_data(wchar_t* id, wchar_t* name, float* x, float* y, float* z, int* exp, int* level, int* weapon, int* stage);
};

