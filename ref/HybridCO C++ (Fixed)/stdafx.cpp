// stdafx.cpp : source file that includes just the standard includes
// HybridCO.pch will be the pre-compiled header
// stdafx.obj will contain the pre-compiled type information

#include "stdafx.h"
#include <windows.h>
#include "DatabaseRoot.h"
#include "Client.h"
#include "Entity.h"
#include "Packets.h"
#include "FlexibleArray.h"
#include "UserScreen.h" // IMapObject

CSplitString::CSplitString(char* String, char Delimeter)
{
	int last_pos = 0;
	int tmp_counter = 0;
	int str_len = strlen(String);
	int str_size;
	m_Count = 1;
	m_Elements = NULL;

	// Obtain the count
	for (int i = 0; i < str_len; i++)
	{
		if (String[i] == Delimeter)
			m_Count++;
	}
	// Split the string
	m_Elements = new char*[m_Count];
	for (int i = 0; i < str_len; i++)
	{
		if (String[i] == Delimeter)
		{
			str_size = i - last_pos;
			m_Elements[tmp_counter] = new char[str_size + 1];
			m_Elements[tmp_counter][str_size] = NULL;
			memcpy(m_Elements[tmp_counter], String + last_pos, str_size);
			tmp_counter++;
			last_pos = i + 1;
		}
	}
	str_size = str_len - last_pos;
	m_Elements[tmp_counter] = new char[str_size + 1];
	m_Elements[tmp_counter][str_size] = NULL;
	memcpy(m_Elements[tmp_counter], String + last_pos, str_size);
}
CSplitString::~CSplitString()
{
	for (int i = 0; i < m_Count; i++)
	{
		delete[] m_Elements[i];
	}
	delete[] m_Elements;
}

void SendRangePacket(IClassPacket* Packet, IMapObject* mapObj, bool SendSelf, StdConquerCallback Callback, bool Delete)
{
	int Size = Packet->Size();
	unsigned char* ptrBuffer = new unsigned	char[Size];
	Packet->Serialize(ptrBuffer);

	SendRangePacket(ptrBuffer, mapObj, SendSelf, Callback, false);
	delete[] ptrBuffer;

	if (Delete)
	{
		delete Packet;
	}
}
void SendRangePacket(void* Packet, IMapObject* mapObj, bool SendSelf, StdConquerCallback Callback, bool Delete)
{
	CGameClient* Client;
	CDatabaseRoot::Core->Clients->ObtainSyncHandle();
	for (int i = 0; i < CDatabaseRoot::Core->Clients->Count; i++)
	{
		Client = CDatabaseRoot::Core->Clients->Elements[i];
		if (Client->Entity->Map == mapObj->Map)
		{
			if (Distance(Client->Entity->X, Client->Entity->Y, mapObj->X, mapObj->Y) <= MAX_VIEW_DISTANCE)
			{
				if (Client->ID == mapObj->UID)
				{
					if (SendSelf)
					{
						Client->Send(Packet);
						if (Callback != NULL)
							Callback(mapObj, Client->Entity, NULL);
					}
				}
				else
				{
					Client->Send(Packet);
					if (Callback != NULL)
						Callback(mapObj, Client->Entity, NULL);
				}
			}
		}
	}
	CDatabaseRoot::Core->Clients->FreeSyncHandle();
	if (Delete)
	{
		delete[] Packet;
	}
}

unsigned short Distance(unsigned short x1, unsigned short y1, 
						unsigned short x2, unsigned short y2)
{
	WORD x = abs(x1-x2);
	WORD y = abs(y1-y2);
	return __max(x, y);
}

void PrintHexStr(unsigned char* Ptr, int Size)
{
	const int h_size = (16 * 3) + 1;
	const int s_size = 16 + 1;
	char HexCharacters[h_size];
	memset(HexCharacters, 0, h_size);

	char StrCharacters[s_size];
	char HexChars[] = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
		'A', 'B', 'C', 'D', 'E', 'F' };
	int i = 0;
	while (i < Size)
	{
		memset(HexCharacters, ' ', h_size - 1);
		memset(StrCharacters, 0, s_size);
		for (int i2 = 0; i2 < 16; i2++)
		{
			if (i >= Size)
			{
				break;
			}
			HexCharacters[(i2 * 3) + 1] = HexChars[Ptr[i] & 0x0F];
			HexCharacters[(i2 * 3) + 0] = HexChars[(Ptr[i] >> 4) & 0x0F];
			HexCharacters[(i2 * 3) + 2] = ' ';
			StrCharacters[i2] = (Ptr[i] >= 32 && Ptr[i] <= 126) ? Ptr[i] : '.';
			i++;
		}
		printf("%s\t%s\r\n", HexCharacters, StrCharacters);
	}
	printf("\r\n");
}

// TODO: reference any additional headers you need in STDAFX.H
// and not in this file
