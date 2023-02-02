#include "pch.h"
#include "Allocator.h"
#include "Memory.h"

/*--------------------
	BaseAllocator
--------------------*/

void* BaseAllocator::Alloc(int32 size)
{
	return ::malloc(size);
}

void BaseAllocator::Release(void* ptr)
{
	::free(ptr);
}

/*--------------------
	StompAllocator
--------------------*/

void* StompAllocator::Alloc(int32 size)
{
	// 댕글링 포인터(메모리 오염)를 방지하기 위해 메모리 할당
	// 운영체제에서 직접 하도록 한다.
	// 오버플로우 방지를 위해 [               [  ]]
	// 메모리를 뒤에 할당한다.
	const int64 pageCount = (size + PAGE_SIZE - 1) / PAGE_SIZE;
	const int64 dataOffset = pageCount * PAGE_SIZE - size;
	void* baseAddress = ::VirtualAlloc(NULL, pageCount * PAGE_SIZE, MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
	return static_cast<void*>(static_cast<int8*>(baseAddress) + dataOffset);
}

void StompAllocator::Release(void* ptr)
{
	const int64 adress = reinterpret_cast<int64>(ptr);
	const int64 baseAdress = adress - (adress % PAGE_SIZE);
	::VirtualFree(reinterpret_cast<void*>(baseAdress), 0, MEM_RELEASE);
}

/*--------------------
	PoolAllocator
--------------------*/

void* PoolAllocator::Alloc(int32 size)
{
	return GMemory->Allocate(size);
}

void PoolAllocator::Release(void* ptr)
{
	GMemory->Release(ptr);
}