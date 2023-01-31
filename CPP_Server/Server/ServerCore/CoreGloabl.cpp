#include "pch.h"
#include "CoreGloabl.h"
#include "ThreadManager.h"
#include "DeadLockProfiler.h"

ThreadManager* GThreadManager = nullptr;
DeadLockProfiler* GDeadLockProfiler = nullptr;

class CoreGloabl
{
public:
	CoreGloabl()
	{
		GThreadManager = new ThreadManager();
		GDeadLockProfiler = new DeadLockProfiler();
	}

	~CoreGloabl()
	{
		delete GThreadManager;
		delete GDeadLockProfiler;
	}
} GCoreGlobal;