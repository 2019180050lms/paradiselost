#define _CRT_SECURE_NO_WARNINGS

#include "DB.h"
#include <string.h>


void DB::show_error(SQLHANDLE hHandle, SQLSMALLINT hType, RETCODE RetCode)
{
    /************************************************************************
/* HandleDiagnosticRecord : display error/warning information
/*
/* Parameters:
/*      hHandle     ODBC handle
/*      hType       Type of handle (SQL_HANDLE_STMT, SQL_HANDLE_ENV, SQL_HANDLE_DBC)
/*      RetCode     Return code of failing command
/************************************************************************/
    {
        SQLSMALLINT iRec = 0;
        SQLINTEGER  iError;
        WCHAR       wszMessage[1000];
        WCHAR       wszState[SQL_SQLSTATE_SIZE + 1];

        if (RetCode == SQL_INVALID_HANDLE) {
            std::wcout << L"Invalid handle!\n";
            return;
        }
        while (SQLGetDiagRec(hType, hHandle, ++iRec, wszState, &iError, wszMessage,
            (SQLSMALLINT)(sizeof(wszMessage) / sizeof(WCHAR)), (SQLSMALLINT*)NULL) == SQL_SUCCESS) {
            // Hide data truncated..
            if (wcsncmp(wszState, L"01004", 5)) {
                std::wcout << L"[" << wszState << L"] " << wszMessage << L"  " << iError << std::endl;
            }
        }
    }
}

void DB::DBConnect()
{
    std::cout << "DB Connect ..." << std::endl;
    retcode = SQLAllocHandle(SQL_HANDLE_ENV, SQL_NULL_HANDLE, &henv);

    if (retcode == SQL_SUCCESS || retcode == SQL_SUCCESS_WITH_INFO) {
        retcode = SQLSetEnvAttr(henv, SQL_ATTR_ODBC_VERSION, (SQLPOINTER*)SQL_OV_ODBC3, 0);

        if (retcode == SQL_SUCCESS || retcode == SQL_SUCCESS_WITH_INFO) {
            SQLAllocHandle(SQL_HANDLE_DBC, henv, &hdbc);

            if (retcode == SQL_SUCCESS || retcode == SQL_SUCCESS_WITH_INFO) {
                SQLSetConnectAttr(hdbc, SQL_LOGIN_TIMEOUT, (SQLPOINTER)5, 0);

                retcode = SQLConnect(hdbc, (SQLWCHAR*)L"paradiselost", SQL_NTS, (SQLWCHAR*)NULL, 0, NULL, 0);

                if (retcode == SQL_ERROR) {
                    show_error(hstmt, SQL_HANDLE_STMT, retcode);
                    return;
                }
            }
        }
    }

    std::cout << "DB Connect Complete!" << std::endl;
}

void DB::DBDisconnect()
{
    SQLCancel(hstmt);
    SQLFreeHandle(SQL_HANDLE_STMT, hstmt);
    SQLDisconnect(hdbc);
    SQLFreeHandle(SQL_HANDLE_DBC, hdbc);
    SQLFreeHandle(SQL_HANDLE_ENV, henv);
}

bool DB::check_user_id(wchar_t* id)
{
    SQLCHAR user_name[10] = { NULL };
    SQLLEN cbName = 0;

    retcode = SQLAllocHandle(SQL_HANDLE_STMT, hdbc, &hstmt);
    wchar_t get_id[100] = L"";
    if (id[0] == '\0')
        return false;
    swprintf_s(get_id, L"EXEC get_user_name %s", id);

    retcode = SQLExecDirect(hstmt, (SQLWCHAR*)get_id, SQL_NTS);

    if (retcode == SQL_SUCCESS || retcode == SQL_SUCCESS_WITH_INFO) {
        retcode = SQLBindCol(hstmt, 1, SQL_C_CHAR, user_name, 10, &cbName);

        for (int i = 0; ; i++) {
            retcode = SQLFetch(hstmt);
            if (retcode == SQL_ERROR)
                show_error(hstmt, SQL_HANDLE_STMT, retcode);
            else if (retcode == SQL_SUCCESS || retcode == SQL_SUCCESS_WITH_INFO)
            {
                std::cout << "user_name: " << reinterpret_cast<char*>(user_name) << std::endl;
            }
            else
                break;
        }
    }
    else {
        show_error(hstmt, SQL_HANDLE_STMT, retcode);
        SQLCancel(hstmt);
        SQLFreeHandle(SQL_HANDLE_STMT, hstmt);
        return false;
    }
    char* retName = reinterpret_cast<char*>(user_name);
    if (retName[0] == '\0')
        return false;
    else
        return true;
}

void DB::get_user_data(wchar_t* id, wchar_t* name, float* x, float* y, float* z, int* exp, int* level, int* weapon, int* type)
{
    SQLCHAR user_name[NAME_LEN];
    SQLINTEGER user_exp = 0, user_level = 0, user_weapon = 0, user_type = -1;
    SQLFLOAT user_x = 0, user_y = 0, user_z = 0;
    SQLLEN cbX = 0, cbY = 0, cbZ = 0, cbName = 0, cbExp = 0, cbLevel = 0, cbWeapon = 0, cbType = 0;

    retcode = SQLAllocHandle(SQL_HANDLE_STMT, hdbc, &hstmt);
    wchar_t user_id[100] = L"";
    swprintf_s(user_id, L"EXEC get_user_data %s", id);

    retcode = SQLExecDirect(hstmt, (SQLWCHAR*)user_id, SQL_NTS);

    if (retcode == SQL_SUCCESS || retcode == SQL_SUCCESS_WITH_INFO) {
        retcode = SQLBindCol(hstmt, 1, SQL_C_CHAR, &user_name, 10, &cbName);
        retcode = SQLBindCol(hstmt, 2, SQL_DOUBLE, &user_x, 12, &cbX);
        retcode = SQLBindCol(hstmt, 3, SQL_DOUBLE, &user_y, 12, &cbY);
        retcode = SQLBindCol(hstmt, 4, SQL_DOUBLE, &user_z, 12, &cbZ);
        retcode = SQLBindCol(hstmt, 5, SQL_INTEGER, &user_exp, 12, &cbExp);
        retcode = SQLBindCol(hstmt, 6, SQL_INTEGER, &user_level, 12, &cbLevel);
        retcode = SQLBindCol(hstmt, 7, SQL_INTEGER, &user_weapon, 12, &cbWeapon);
        retcode = SQLBindCol(hstmt, 8, SQL_INTEGER, &user_type, 12, &cbType);

        for (int i = 0; ; i++) {
            retcode = SQLFetch(hstmt);
            if (retcode == SQL_ERROR)
                show_error(hstmt, SQL_HANDLE_STMT, retcode);
            else if (retcode == SQL_SUCCESS || retcode == SQL_SUCCESS_WITH_INFO) {
                std::cout << user_name << ", " << user_x << ", " << user_y << ", "
                    << user_z << ", " << user_exp << ", " << user_level << ", "
                    << user_weapon << ", " << user_type << std::endl;
            }
            else
                break;
        }
    }
    else
        show_error(hstmt, SQL_HANDLE_STMT, retcode);
    char* i_name = reinterpret_cast<char*>(user_name);
    wchar_t* pStr;
    int size = MultiByteToWideChar(CP_ACP, 0, i_name, -1, NULL, NULL);
    pStr = new WCHAR[size];
    MultiByteToWideChar(CP_ACP, 0, i_name, strlen(i_name) + 1, pStr, size);
    if (i_name[0] != '\0')
        wcscpy(name, pStr);
    *x = user_x;
    *y = user_y;
    *z = user_z;
    *exp = user_exp;
    *level = user_level;
    *weapon = user_weapon;
    *type = user_type;
}

void DB::add_user_data(wchar_t* id, wchar_t* name, float* x, float* y, float* z, int* exp, int* level, int* weapon, int* type)
{
    retcode = SQLAllocHandle(SQL_HANDLE_STMT, hdbc, &hstmt);
    wchar_t add_user[200] = L"";
    if (id[0] == '\0')
        return;
    swprintf_s(add_user, L"EXEC add_user_data %s, %s, %f, %f, %f, %d, %d, %d, %d", id, name, *x, *y, *z, *exp, *level, *weapon, *type);

    retcode = SQLExecDirect(hstmt, (SQLWCHAR*)add_user, SQL_NTS);

    if (retcode == SQL_SUCCESS || retcode == SQL_SUCCESS_WITH_INFO) {
        SQLCancel(hstmt);
        SQLFreeHandle(SQL_HANDLE_STMT, hstmt);

        std::cout << "add user_data complete!" << std::endl;
    }
    else {
        show_error(hstmt, SQL_HANDLE_STMT, retcode);
        SQLCancel(hstmt);
        SQLFreeHandle(SQL_HANDLE_STMT, hstmt);
    }
}

void DB::update_user_data(wchar_t* id, wchar_t* name, float* x, float* y, float* z, int* exp, int* level, int* weapon)
{
    retcode = SQLAllocHandle(SQL_HANDLE_STMT, hdbc, &hstmt);
    wchar_t update[200] = L"";
    if (id[0] == '\0')
        return;
    swprintf_s(update, L"EXEC update_user %s, %s, %f, %f, %f, %d, %d, %d", id, name, *x, *y, *z, *exp, *level, *weapon);
    
    retcode = SQLExecDirect(hstmt, (SQLWCHAR*)update, SQL_NTS);

    if (retcode == SQL_SUCCESS || retcode == SQL_SUCCESS_WITH_INFO) {
        SQLCancel(hstmt);
        SQLFreeHandle(SQL_HANDLE_STMT, hstmt);

        std::cout << "update user_data complete !" << std::endl;
    }
    else {
        show_error(hstmt, SQL_HANDLE_STMT, retcode);
        SQLCancel(hstmt);
        SQLFreeHandle(SQL_HANDLE_STMT, hstmt);
    }
}