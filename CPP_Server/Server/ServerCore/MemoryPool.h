#pragma once

enum
{
	SLIST_ALIGNMENT = 16
};

// 메모리 파편화 문제
// 요즘 메모리 할당기는 잘 처리 해줘서 굳이 안해도 되지만
// 알고있으면 좋으니

// [32][64][ ][ ][ ][ ][ ][ ]

// [ 32 32 32 ...           ]
// 동일한 크기의 데이터만 모아준다.

/*-----------------
	MemoryHeader
-------------------*/

DECLSPEC_ALIGN(SLIST_ALIGNMENT)
struct MemoryHeader : public SLIST_ENTRY
{
	// [MemoryHeader][Data]
	MemoryHeader(int32 size) : allocSize(size) { }

	// 처음 메모리 할당후 데이터 사용x 첫번째 주소([MemoryHeader])를 넘겨준 상태
	static void* AttachHeader(MemoryHeader* header, int32 size)
	{
		new(header)MemoryHeader(size); // placement new
		return reinterpret_cast<void*>(++header);
	}

	// [Data]의 시작 위치를 반환
	static MemoryHeader* DetachHeader(void* ptr)
	{
		MemoryHeader* header = reinterpret_cast<MemoryHeader*>(ptr) - 1;
		return header;
	}

	int32 allocSize;
	// TODO : 필요한 추가 정보
};

/*-----------------
	MemoryPool
-------------------*/

class MemoryPool
{
public:
	MemoryPool(int32 allocSize);
	~MemoryPool();

	void			Push(MemoryHeader* ptr);
	MemoryHeader*	Pop();

private:
	SLIST_HEADER	_header; // 첫번째 데이터를 가리킴
	int32			_allocSize = 0;
	atomic<int32>	_useCount = 0;
	atomic<int32>	_reserveCount = 0;
};

