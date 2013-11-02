#include "StdAfx.h"
#include "Packets.h"

// ------ CSyncPacket Class Implementation ------
// Count Property
void CSyncPacket::set_Count(int value)
{
	DWORD copy_uid = 0;
	if (Buffer != NULL)
	{
		copy_uid = *((DWORD*)(Buffer + 4));
		delete[] Buffer;
	}
	WORD size = 12 + (value * sizeof(CSyncPacket::Data));
	Buffer = new unsigned char[size];
	PACKET_HEAD* ptr = (PACKET_HEAD*)Buffer;
	ptr->Size = size;
	ptr->Type = SYNCPACKET_ID;
	*((DWORD*)(Buffer + 4)) = copy_uid;
	*((int*)(Buffer + 8)) = value;
}
//

// ------ CharacterInfoPacket Struct Implementation ------
// Create Function
unsigned char* CharacterInfoPacket::Create(char *szName, char *szSpouse)
{
	int Name_Len = strlen(szName);
	int Spouse_Len = strlen(szSpouse);

	unsigned char* Ptr = new unsigned char[70 + Name_Len + Spouse_Len];
	memset(Ptr, 0, 70 + Name_Len + Spouse_Len);

	CharacterInfoPacket* lpInfo = (CharacterInfoPacket*)Ptr;
	lpInfo->Head.Size = 70 + Name_Len + Spouse_Len;
	lpInfo->Head.Type = CHARINFOPACKET_ID;
	lpInfo->_0x05 = 0x05;
	lpInfo->_0x01 = 0x01;

	unsigned char* offsetPtr = Ptr + sizeof(CharacterInfoPacket);
	offsetPtr[-1] = 0x02;
	offsetPtr[0] = Name_Len;
	strncpy((char*)offsetPtr + 1, szName, Name_Len);
	offsetPtr[1 + Name_Len] = Spouse_Len;
	strncpy((char*)offsetPtr + Name_Len + 2, szSpouse, Spouse_Len);

	return Ptr;
}
//

// ------ CMsgPacket Class Implementation ------
// Struct Declaration
struct MsgData
{
	DWORD Color;
	CHAT_TYPE ChatType;
	DWORD SenderUID;
};
//
// Properties
//
char* CMsgPacket::get_Message() { return m_Msg; }
void CMsgPacket::set_Message(char* value) 
{ 
	if (new_Msg)
	{
		delete[] m_Msg;
		new_Msg = false;
	}
	m_Msg = value; 
	Msg_Len = strlen(value); 
}
//
char* CMsgPacket::get_To() { return m_To; }
void CMsgPacket::set_To(char* value) 
{ 
	if (new_To)
	{
		delete[] m_To;
		new_To = false;
	}
	m_To = value; 
	To_Len = strlen(m_To); 
}
//
char* CMsgPacket::get_From() { return m_From; }
void CMsgPacket::set_From(char* value) 
{ 
	if (new_From)
	{
		delete[] m_From;
		new_From = false;
	}
	m_From = value; 
	From_Len = strlen(m_From); 
}
//
//
// Size Function
WORD CMsgPacket::Size()
{
	return 32 + From_Len + To_Len + Msg_Len;
}
//
// Serialize Function
void CMsgPacket::Serialize(unsigned char* Ptr)
{ 
	memset(Ptr, 0, Size());

	PACKET_HEAD* pHeader = (PACKET_HEAD*)Ptr;
	pHeader->Size = Size();
	pHeader->Type = MSGPACKET_ID;

	MsgData* pMsgData = (MsgData*)(Ptr + sizeof(PACKET_HEAD));
	pMsgData->Color = Color;
	pMsgData->ChatType = ChatType;
	pMsgData->SenderUID = SenderUID;

	int offset = sizeof(PACKET_HEAD) + sizeof(MsgData) + 9;
	Ptr[offset - 1] = 4;
	Ptr[offset] = From_Len;
	strcpy((char*)Ptr + offset + 1, m_From);
	Ptr[offset + From_Len + 1] = To_Len;
	strcpy((char*)Ptr + offset + From_Len + 2, m_To);
	Ptr[offset + From_Len + To_Len + 3] = Msg_Len;
	strcpy((char*)Ptr + offset + From_Len + To_Len + 4, m_Msg);
}
//
// Deserialize Function
void CMsgPacket::Deserialize(unsigned char* Ptr)
{
	MsgData* pMsgData = (MsgData*)(Ptr + sizeof(PACKET_HEAD));
	Color = pMsgData->Color;
	ChatType = pMsgData->ChatType;
	SenderUID = pMsgData->SenderUID;

	int offset = sizeof(PACKET_HEAD) + sizeof(MsgData) + 9;
	new_From = new_To = new_Msg = true;

	From_Len = Ptr[offset];
	m_From = new char[From_Len + 1];
	m_From[From_Len] = NULL;
	strncpy(m_From, (char*)Ptr + offset + 1, From_Len);
	To_Len = Ptr[offset + From_Len + 1];
	m_To = new char[To_Len + 1];
	m_To[To_Len] = NULL;
	strncpy(m_To, (char*)Ptr + offset + From_Len + 2, To_Len);
	Msg_Len = Ptr[offset + From_Len + To_Len + 3];
	m_Msg = new char[Msg_Len + 1];
	m_Msg[Msg_Len] = NULL;
	strncpy(m_Msg, (char*)Ptr + offset + From_Len + To_Len + 4, Msg_Len);
}
//
// Ctor and Dtor
//
CMsgPacket::CMsgPacket()
{
	m_To = m_From = m_Msg = NULL;
	To_Len = From_Len = Msg_Len = 0;
	new_From = new_To = new_Msg = false;
	Color = SenderUID = 0;
	ChatType = CHAT_TYPE_NONE;
}
CMsgPacket::~CMsgPacket()
{
	if (new_From)
		delete[] m_From;
	if (new_To)
		delete[] m_To;
	if (new_Msg)
		delete[] m_Msg;
}
//