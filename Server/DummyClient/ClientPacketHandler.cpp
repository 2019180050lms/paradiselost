#include "pch.h"
#include "ClientPacketHandler.h"
#include "BufferReader.h"
#include <BufferWriter.h>

void ClientPacketHandler::HandlePacket(PacketSessionRef& session, BYTE* buffer, int32 len)
{
	BufferReader br(buffer, len);

	PacketHeader header;
	br >> header;

	switch (header.id)
	{
	case S_Chat:
		Handle_S_Chat(session, buffer, len);
		break;
	default:
		break;
	}
}

void ClientPacketHandler::Handle_S_Chat(PacketSessionRef& session, BYTE* buffer, int32 len)
{
	BufferReader br(buffer, len);

	PacketHeader header;
	br >> header;

	int32 id;
	br >> id;

	wstring name;
	uint16 nameLen;
	br >> nameLen;
	name.resize(nameLen);

	br.Read((void*)name.data(), nameLen * sizeof(WCHAR));

	wcout.imbue(std::locale("kor"));
	wcout << name << endl;

	auto sendBuffer = ClientPacketHandler::Make_C_Chat(L"Hi");
	session->Send(sendBuffer);
}

SendBufferRef ClientPacketHandler::Make_C_Chat(wstring chat)
{
	SendBufferRef sendBuffer = GSendBufferManager->Open(4096);

	BufferWriter bw(sendBuffer->Buffer(), sendBuffer->AllocSize());

	PacketHeader* header = bw.Reserve<PacketHeader>();

	bw << (uint16)chat.size();
	bw.Write((void*)chat.data(), chat.size() * sizeof(WCHAR));

	header->size = bw.WriteSize();
	header->id = C_Chat; // 1 : Test Msg

	sendBuffer->Close(bw.WriteSize());

	return sendBuffer;
}
