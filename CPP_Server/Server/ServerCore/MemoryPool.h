#pragma once

enum
{
	SLIST_ALIGNMENT = 16
};

// �޸� ����ȭ ����
// ���� �޸� �Ҵ��� �� ó�� ���༭ ���� ���ص� ������
// �˰������� ������

// [32][64][ ][ ][ ][ ][ ][ ]

// [ 32 32 32 ...           ]
// ������ ũ���� �����͸� ����ش�.

/*-----------------
	MemoryHeader
-------------------*/

DECLSPEC_ALIGN(SLIST_ALIGNMENT)
struct MemoryHeader : public SLIST_ENTRY
{
	// [MemoryHeader][Data]
	MemoryHeader(int32 size) : allocSize(size) { }

	// ó�� �޸� �Ҵ��� ������ ���x ù��° �ּ�([MemoryHeader])�� �Ѱ��� ����
	static void* AttachHeader(MemoryHeader* header, int32 size)
	{
		new(header)MemoryHeader(size); // placement new
		return reinterpret_cast<void*>(++header);
	}

	// [Data]�� ���� ��ġ�� ��ȯ
	static MemoryHeader* DetachHeader(void* ptr)
	{
		MemoryHeader* header = reinterpret_cast<MemoryHeader*>(ptr) - 1;
		return header;
	}

	int32 allocSize;
	// TODO : �ʿ��� �߰� ����
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
	SLIST_HEADER	_header; // ù��° �����͸� ����Ŵ
	int32			_allocSize = 0;
	atomic<int32>	_useCount = 0;
	atomic<int32>	_reserveCount = 0;
};

