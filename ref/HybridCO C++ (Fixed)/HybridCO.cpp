// HybridCO.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <iostream>
#include <string.h>
#include "ServerSocket.h"
#include "FlexibleArray.h"
#include "Client.h"
#include "Cryptography.h"
#include "Entity.h"
#include "UserScreen.h"
#include "DatabaseRoot.h"
#include "Packets.h"
#include "Processor.h"
#include "ConquerCallbacks.h"

static char* host_ip;

// Auth Functions
void AuthConnect(CCustomWinsockClient* client, CServerSocket* server)
{
	CAuthClient* pWrapper = new CAuthClient();
	pWrapper->Socket = client;
	client->PublicWrapper = pWrapper;
}
void AuthDisconnect(CCustomWinsockClient* client, CServerSocket* server)
{
	CAuthClient* pWrapper = (CAuthClient*)client->PublicWrapper;
	if (pWrapper->Phase != CLIENT_PHASE_AUTH_SUCCESS)
	{
		delete client->PublicWrapper;
	}
}

void AuthReceive(CCustomWinsockClient* client, CServerSocket* server, unsigned char* buf, int buflen)
{
	CAuthClient* pWrapper = (CAuthClient*)client->PublicWrapper;	
	pWrapper->Crypto->Decrypt(buf, buf, buflen);
	PACKET_HEAD* header = (PACKET_HEAD*)buf;
	if (header->Size == buflen)
	{
		if (header->Type == AUTHLOGINPACKET_ID)
		{
			AuthLoginPacket* Packet = (AuthLoginPacket*)buf;
			memcpy(pWrapper->szUsername, Packet->szUsername, 16);
			CPasswordCryptography::Decrypt(Packet->Password);
			memcpy(pWrapper->szPassword, Packet->Password, 16);//My crypter worked :o

			printf("LOGGING IN...\r\nUsername: %s\r\nPassword: %s\r\n", pWrapper->szUsername, pWrapper->szPassword);

			if (CDatabaseRoot::Core->ValidAccount(pWrapper) &&
				pWrapper->ID != NULL)
			{
				CDatabaseRoot::Core->AuthClients->ObtainSyncHandle();
				for (int i = 0; i < CDatabaseRoot::Core->AuthClients->Count; i++)
				{
					if (CDatabaseRoot::Core->AuthClients->Elements[i]->ID == pWrapper->ID)
					{
						CDatabaseRoot::Core->AuthClients->RemoveAt(i, false);
						break;
					}
				}
				CDatabaseRoot::Core->AuthClients->Add(pWrapper, false);
				CDatabaseRoot::Core->AuthClients->FreeSyncHandle();

				
				
				Reply;
				Reply.Key1 = pWrapper->ID;
				Reply.Key2 = pWrapper->ID + rand();
				Reply.Port = 5816;
				strcpy(Reply.szIPAddress, host_ip);

				pWrapper->Phase = CLIENT_PHASE_AUTH_SUCCESS;
				pWrapper->Send(&Reply);
			}
			else
			{
				pWrapper->Socket->Disconnect();
			}
		}
	}
}
//
// Game Functions
void GameConnect(CCustomWinsockClient* client, CServerSocket* server)
{
	CGameClient* pWrapper = new CGameClient();
	pWrapper->Socket = client;
	client->PublicWrapper = pWrapper;
}
void GameDisconnect(CCustomWinsockClient* client, CServerSocket* server)
{
	CGameClient* pWrapper = (CGameClient*)client->PublicWrapper;

	DataPacket Remove;
	Remove.ID = DATA_ID_REMOVE_ENTITY;
	Remove.UID = pWrapper->ID;
	Remove.dwParam = pWrapper->ID;
	SendRangePacket(&Remove, pWrapper->Entity, false, RemoveEntityCallback);

	CDatabaseRoot::Core->SaveAccount(pWrapper);
	CDatabaseRoot::Core->SaveItems(pWrapper, true); // Inventory
	CDatabaseRoot::Core->SaveItems(pWrapper, false); // Equipment

	CDatabaseRoot::Core->Clients->ObtainSyncHandle();
	for (int i = 0; i < CDatabaseRoot::Core->Clients->Count; i++)
	{
		if (CDatabaseRoot::Core->Clients->Elements[i]->ID == pWrapper->ID)
		{
			CDatabaseRoot::Core->Clients->RemoveAt(i, false);
			break;
		}
	}
	CDatabaseRoot::Core->Clients->FreeSyncHandle();
	
	delete pWrapper;
}
void GameReceive(CCustomWinsockClient* client, CServerSocket* server, unsigned char* buf, int buflen)
{
	CGameClient* pWrapper = (CGameClient*)client->PublicWrapper;	
	pWrapper->Crypto->Decrypt(buf, buf, buflen);
	
	for (int Counter = 0; Counter < buflen; /*Counter += Size*/)
	{
		PACKET_HEAD* header = (PACKET_HEAD*)(buf + Counter);
		if (header->Size > buflen)
		{
			pWrapper->Socket->Disconnect(); // INVALID_LEN
		}
		else
		{
			//PrintHexStr(buf + Counter, header->Size);
			CPacketProcessor::Process(pWrapper, header);
		}
		Counter += header->Size;
	}//
}
//


int GetConsoleWidth(HANDLE hConsole)
{
	CONSOLE_SCREEN_BUFFER_INFO bufferInfo;
	GetConsoleScreenBufferInfo(hConsole, &bufferInfo);
    return ((bufferInfo.srWindow.Right - bufferInfo.srWindow.Left) + 1);
}
COORD GetConsoleCursorPosition(HANDLE hConsole)
{
	CONSOLE_SCREEN_BUFFER_INFO bufferInfo;
	GetConsoleScreenBufferInfo(hConsole, &bufferInfo);
	return bufferInfo.dwCursorPosition;
}

int _tmain(int argc, _TCHAR* argv[])
{
#define FOREGROUND_WHITE FOREGROUND_BLUE | FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_INTENSITY
#define FOREGROUND_CYAN FOREGROUND_BLUE | FOREGROUND_GREEN | FOREGROUND_INTENSITY
#define FOREGROUND_LIME FOREGROUND_GREEN | FOREGROUND_INTENSITY

	//host_ip = "68.10.158.209";e
	host_ip = "127.0.0.1";	

	SetConsoleTitleA("HybridCO Server");
	HANDLE hStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
	
	SetConsoleTextAttribute(hStdOut, FOREGROUND_WHITE);
	WriteLn("HybridCO");

	SetConsoleTextAttribute(hStdOut, FOREGROUND_CYAN);
	printf("Creating Database... ");
	CDatabaseRoot::Create();
	SetConsoleTextAttribute(hStdOut, FOREGROUND_LIME);
	WriteLn("Completed.");
	
	SetConsoleTextAttribute(hStdOut, FOREGROUND_CYAN);
	printf("Creating Servers... ");
	CDatabaseRoot::AuthServer->Port = 9958;
	CDatabaseRoot::AuthServer->OnClientConnect = AuthConnect;
	CDatabaseRoot::AuthServer->OnClientDisconnect = AuthDisconnect;
	CDatabaseRoot::AuthServer->OnClientReceive = AuthReceive;
	CDatabaseRoot::AuthServer->Enabled = true;
	CDatabaseRoot::GameServer->Port = 5816;
	CDatabaseRoot::GameServer->OnClientConnect = GameConnect;
	CDatabaseRoot::GameServer->OnClientDisconnect = GameDisconnect;
	CDatabaseRoot::GameServer->OnClientReceive = GameReceive;
	CDatabaseRoot::GameServer->Enabled = true;
	SetConsoleTextAttribute(hStdOut, FOREGROUND_LIME);
	WriteLn("Completed.");
	SetConsoleTextAttribute(hStdOut, FOREGROUND_WHITE);
	printf("\t- Auth Server : 9958\r\n");
	printf("\t- Game Server : 5816\r\n");

	const int max_write_space = 12;
	SetConsoleTextAttribute(hStdOut, FOREGROUND_RED | FOREGROUND_INTENSITY);
	printf("\r\n\r\n");
	COORD LogXY = GetConsoleCursorPosition(hStdOut);
	COORD InputXY;
	InputXY.Y = LogXY.Y + max_write_space + 1;
	InputXY.X = 0;
	SetConsoleCursorPosition(hStdOut, InputXY);
	WriteLn("Type `/help` for a list of commands, and `/quit` to exit the application.");
	InputXY.Y++;

	int width = GetConsoleWidth(hStdOut);
	char* zero_fill = new char[width + 1];
	memset(zero_fill, ' ', width);
	zero_fill[width] = NULL;

	char szBuffer[256];
	do
	{
		SetConsoleCursorPosition(hStdOut, InputXY);
		printf(zero_fill);
		SetConsoleCursorPosition(hStdOut, InputXY);

		memset(szBuffer, 0, 256);
		gets(szBuffer);

		SetConsoleCursorPosition(hStdOut, LogXY);
		for (int i = 0; i < max_write_space; i++)
		{
			printf(zero_fill);
		}
		SetConsoleCursorPosition(hStdOut, LogXY);

		CSplitString *Splitter = new CSplitString(szBuffer, ' ');
		int str_len = strlen(Splitter->Elements[0]);
		for (int i = 0; i < str_len; i++)
			Splitter->Elements[0][i] = tolower(Splitter->Elements[0][i]);

		try
		{
			if (strcmp(Splitter->Elements[0], "/quit") == 0)
			{
				delete Splitter;
				break;
			}
			else if (strcmp(Splitter->Elements[0], "/help") == 0)
			{
				WriteLn("`/quit` - Closes this application safely.");
				WriteLn("`/createacc [acc_name] [pass]` - Creates a new account");
			}
			else if (strcmp(Splitter->Elements[0], "/createacc") == 0)
			{
				if (Splitter->Count >= 3)
				{
					CDatabaseRoot::Core->NewAccount(Splitter->Elements[1], Splitter->Elements[2]);
					WriteLn("Account created!");
				}
				else
				{
					WriteLn("Not enough arguments.");
				}
			}
		}
		catch (char* szError)
		{
			WriteLn(szError);
		}
		
		delete Splitter;
	}
	while (true);
	

	CDatabaseRoot::Destroy();
	delete[] zero_fill;

	return 0;
}