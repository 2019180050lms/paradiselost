#pragma once

/*---------------
	RecvBuffer
-----------------*/

class RecvBuffer
{
	// 복사 비용을 낮추기 위해 버퍼 크기를 크게 잡아
	// 최대한 read, writePos가 겹치는 경우의 수를 늘려
	// 복사를 덜하게 해준다. ( 버퍼 크기를 10배로 잡아준다. )
	enum { BUFFER_COUNT = 10 };

public:
	RecvBuffer(int32 bufferSize);
	~RecvBuffer();

	void			Clean();
	bool			OnRead(int32 numOfBytes);
	bool			OnWrite(int32 numOfBytes);

	BYTE*			ReadPos() { return &_buffer[_readPos]; }
	BYTE*			WritePos() { return &_buffer[_writePos]; }
	int32			DataSize() { return _writePos - _readPos; }
	int32			FreeSize() { return _capacity - _writePos; }

private:
	int32			_capacity = 0;
	int32			_bufferSize = 0;
	int32			_readPos = 0;
	int32			_writePos = 0;
	Vector<BYTE>	_buffer;
};

