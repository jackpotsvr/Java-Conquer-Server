#include "StdAfx.h"
#include "DatabaseRoot.h"
#include "FlexibleArray.h"
#include "Client.h"
#include "Entity.h"
#include "Packets.h"
#include "DataMap.h"
#include "ServerSocket.h"
#include <assert.h>

//
// ------ CDatabaseRoot Class Implementation ------
//
// Static Variables
CDatabaseRoot* CDatabaseRoot::Core;
CServerSocket* CDatabaseRoot::AuthServer;
CServerSocket* CDatabaseRoot::GameServer;
void CDatabaseRoot::Create()
{
	CDatabaseRoot::Core = new CDatabaseRoot();
	CDatabaseRoot::AuthServer = new CServerSocket();
	CDatabaseRoot::GameServer = new CServerSocket();
}
void CDatabaseRoot::Destroy()
{
	delete CDatabaseRoot::AuthServer;
	delete CDatabaseRoot::GameServer;
	delete CDatabaseRoot::Core;
}
//
// NewAccount Function
void CDatabaseRoot::NewAccount(char* szAccount, char* szPassword)
{
	char szPath[MAX_PATH];
	memset(szPath, 0, MAX_PATH);
	strcat(szPath, "C:\\ConquerServerDatabase\\Accounts\\");
	strcat(szPath, szAccount);
	strcat(szPath, ".dat");

	if (GetFileAttributesA(szPath) != INVALID_FILE_ATTRIBUTES)
	{
		memset(szPath, 0, MAX_PATH);
		sprintf(szPath, "An account with the name `%s` already exists.", szAccount);
		throw szPath;
	}

	char Account[16];
	char Password[16];
	memset(Account, 0, 16);
	memset(Password, 0, 16);
	strcpy(Account, szAccount);
	strcpy(Password, szPassword);
	OBJID UID = 1000000;

	char* str_find = "C:\\ConquerServerDatabase\\Accounts\\*";
	WIN32_FIND_DATAA find;
	HANDLE hFind = FindFirstFileA(str_find, &find);
	if (hFind != INVALID_HANDLE_VALUE)
	{
		do
		{
			UID++;
		}
		while (FindNextFileA(hFind, &find));
		FindClose(hFind);
	}

	FILE* pAccount = fopen(szPath, "w");
	fwrite(Account, sizeof(char), 16, pAccount);
	fwrite(Password, sizeof(char), 16, pAccount);
	fwrite(&UID, sizeof(OBJID), 1, pAccount);
	fclose(pAccount);
}
//
// FindClient Function
CGameClient* CDatabaseRoot::FindClient(char* szName)
{
	CGameClient* result = NULL;
	CGameClient* Client;
	CDatabaseRoot::Core->Clients->ObtainSyncHandle();
	for (int i = 0; i < CDatabaseRoot::Core->Clients->Count; i++)
	{
		Client = CDatabaseRoot::Core->Clients->Elements[i];
		if (strcmp(Client->Entity->szName, szName) == 0)
		{
			result = Client;
			break;
		}
	}
	CDatabaseRoot::Core->Clients->FreeSyncHandle();
	return result;
}
//
// FindEntity Function
IBaseEntity* CDatabaseRoot::FindEntity(OBJID UID, MAP_ID MapID)
{
	IBaseEntity* result = NULL;
	if (UID > 1000000)
	{
		CGameClient* Client;
		CDatabaseRoot::Core->Clients->ObtainSyncHandle();
		for (int i = 0; i < CDatabaseRoot::Core->Clients->Count; i++)
		{
			Client = CDatabaseRoot::Core->Clients->Elements[i];
			if (Client->Entity->Map == MapID)
			{
				if (Client->ID == UID)
				{
					result = Client->Entity;
					break;
				}
			}
		}
		CDatabaseRoot::Core->Clients->FreeSyncHandle();
	}
	return result;
}
//
// SaveAccountFunction
void CDatabaseRoot::SaveAccount(CGameClient* Client)
{
	char szPath[MAX_PATH];
	memset(szPath, 0, MAX_PATH);
	strcat(szPath, "C:\\ConquerServerDatabase\\Accounts\\");
	strcat(szPath, Client->szUsername);
	strcat(szPath, ".dat");

	FILE* pAccount = fopen(szPath, "w");
	assert(pAccount);
	
	fwrite(Client->szUsername, sizeof(char), 16, pAccount);
	fwrite(Client->szPassword, sizeof(char), 16, pAccount);
	fwrite(&Client->ID, sizeof(OBJID), 1, pAccount); 
	fwrite(Client->Entity->szName, sizeof(char), 16, pAccount);
	fwrite(Client->szSpouse, sizeof(char), 16, pAccount);
	fwrite(&Client->Money, sizeof(int), 1, pAccount);
	fwrite(&Client->ConquerPoints, sizeof(int), 1, pAccount);
	fwrite(&Client->Entity->Data->Model, sizeof(DWORD), 1, pAccount);
	fwrite(&Client->Entity->Data->HairStyle, sizeof(WORD), 1, pAccount);
	fwrite(&Client->Job, sizeof(BYTE), 1, pAccount);
	fwrite(&Client->Entity->Data->Reborn, sizeof(WORD), 1, pAccount);
	fwrite(&Client->Entity->Data->Level, sizeof(WORD), 1, pAccount);
	fwrite(&Client->Stats, sizeof(UserStats), 1, pAccount);
	fwriteval<int>(Client->Entity->Hitpoints, pAccount);
	fwrite(&Client->Mana, sizeof(WORD), 1, pAccount);
	fwriteval<MAP_ID>(MAP_ID(Client->Entity->Map.ID), pAccount);
	fwrite(&Client->Entity->Data->X, sizeof(WORD), 1, pAccount);
	fwrite(&Client->Entity->Data->Y, sizeof(WORD), 1, pAccount);
	fwrite(&Client->Admin, sizeof(ADMIN_FLAG), 1, pAccount);

	fclose(pAccount);
}
//
// SaveInventory Function
void CDatabaseRoot::SaveItems(CGameClient* Client, bool Inventory)
{
	CFlexibleArray<CItem*>* Items;
	if (Inventory)
		Items = Client->Inventory;
	else
		Items = Client->Equipment;

	char szPath[MAX_PATH];
	memset(szPath, 0, MAX_PATH);
	if (Inventory)
		strcat(szPath, "C:\\ConquerServerDatabase\\Inventory\\");
	else
		strcat(szPath, "C:\\ConquerServerDatabase\\Equipment\\");
	strcat(szPath, Client->szUsername);
	strcat(szPath, ".dat");

	FILE* pItems = fopen(szPath, "w");
	assert(pItems);
	fwriteval<int>(Items->Count, pItems);
	
	Items->ObtainSyncHandle();
	for (int i = 0; i < Items->Count; i++)
	{
		fwrite((ItemPacket*)Items->Elements[i], sizeof(ItemPacket), 1, pItems);
	}
	Items->FreeSyncHandle();
	fclose(pItems);
}
//
// LoadInventory Function
void CDatabaseRoot::LoadItems(CGameClient* Client, bool Inventory)
{
	CFlexibleArray<CItem*>* Items;
	if (Inventory)
		Items = Client->Inventory;
	else
		Items = Client->Equipment;

	char szPath[MAX_PATH];
	memset(szPath, 0, MAX_PATH);
	if (Inventory)
		strcat(szPath, "C:\\ConquerServerDatabase\\Inventory\\");
	else
		strcat(szPath, "C:\\ConquerServerDatabase\\Equipment\\");
	strcat(szPath, Client->szUsername);
	strcat(szPath, ".dat");

	FILE* pItems = fopen(szPath, "r");
	if (pItems != NULL)
	{
		Items->ObtainSyncHandle();
		int count;
		fread<int>(&count, 0, pItems);
		if (count > 0)
		{
			for (int i = 0; i < count; i++)
			{
				CItem* pTmpItem = new ItemPacket();
				if (fread(pTmpItem, sizeof(ItemPacket), 1, pItems) == sizeof(ItemPacket))
				{
					Items->Add(pTmpItem, false);
				}
				else
				{
					delete pTmpItem;
					break;
				}
			}
		}
		fclose(pItems);
		Items->FreeSyncHandle();
	}
}
//
// LoadAccount Function
bool CDatabaseRoot::LoadAccount(CGameClient* Client)
{
	bool result = false;
	AuthClients->ObtainSyncHandle();
	for (int i = 0; i < AuthClients->Count; i++)
	{
		if (AuthClients->Elements[i]->ID == Client->ID)
		{
			memcpy(Client->szUsername, AuthClients->Elements[i]->szUsername, 16);
			memcpy(Client->szPassword, AuthClients->Elements[i]->szPassword, 16);
			result = true;
			AuthClients->RemoveAt(i, false);
			break;
		}
	}
	AuthClients->FreeSyncHandle();
	if (result)
	{
		char szPath[MAX_PATH];
		memset(szPath, 0, MAX_PATH);
		strcat(szPath, "C:\\ConquerServerDatabase\\Accounts\\");
		strcat(szPath, Client->szUsername);
		strcat(szPath, ".dat");

		FILE* pAccount = fopen(szPath, "r");
		if (pAccount != NULL)
		{
			fpos_t file_pos;
			fgetpos(pAccount, &file_pos);
			file_pos += sizeof(char) * 16;		// username
			file_pos += sizeof(char) * 16;		// password
			file_pos += sizeof(OBJID);			// uid
			fsetpos(pAccount, &file_pos);
	
			char szName[16];
			memset(szName, 0, 16);
			Client->Entity->UID = Client->ID;
			fread<char>(szName, 16, INVALID_NAME_STR, pAccount);
			Client->Entity->szName = szName;
			fread<char>(Client->szSpouse, 16, NONE_STR, pAccount);
			fread<int>(&Client->Money, 0, pAccount);
			fread<int>(&Client->ConquerPoints, 0, pAccount);
			fread<int>(&Client->Entity->Data->Model, 1002, pAccount);
			fread<WORD>(&Client->Entity->Data->HairStyle, 420, pAccount);
			fread<char>(&Client->Job, 10, pAccount);
			fread<WORD>(&Client->Entity->Data->Reborn, 0, pAccount);
			fread<WORD>(&Client->Entity->Data->Level, 1, pAccount);
			
			if (Client->Entity->Data->Reborn)
			{
				fread(&Client->Stats, sizeof(UserStats), 1, pAccount);
			}
			else
			{
				fgetpos(pAccount, &file_pos);
				file_pos += sizeof(UserStats);
				fsetpos(pAccount, &file_pos);

				// <todo = LOAD STATS FROM INI HERE>
				Client->Stats.Strength = 1;
				Client->Stats.Agility = 1;
				Client->Stats.Spirit = 1;
				Client->Stats.Vitality = 1;
				Client->Stats.Points = 0;
				// </todo>
			}
			Client->Entity->Hitpoints = freadval<int>(0, 1, pAccount);
			fread<WORD>(&Client->Mana, 0, pAccount);
			fread<WORD>(&Client->Entity->Data->Hitpoints, 0, pAccount);
			Client->Entity->Map = MAP_ID(freadval<WORD>(1002, 1, pAccount));
			fread<WORD>(&Client->Entity->Data->X, 400, pAccount);
			fread<WORD>(&Client->Entity->Data->Y, 400, pAccount);
			fread<ADMIN_FLAG>(&Client->Admin, ADMIN_NONE, pAccount);

			fclose(pAccount);
		}
	}
	return result;
}
//
// ValidAccount Function
bool CDatabaseRoot::ValidAccount(CAuthClient* Client)
{
	bool result = false;
	char szPath[MAX_PATH];
	memset(szPath, 0, MAX_PATH);
	strcat(szPath, "C:\\ConquerServerDatabase\\Accounts\\");
	strcat(szPath, Client->szUsername);
	strcat(szPath, ".dat");

	FILE* pAccount = fopen(szPath, "r");
	if (pAccount != NULL)
	{
		char chk_user[16];
		char chk_pass[16];
		fread(chk_user, sizeof(char), 16, pAccount);
		fread(chk_pass, sizeof(char), 16, pAccount);

		if (strcmp(chk_user, Client->szUsername) == 0 &&
			strcmp(chk_pass, Client->szPassword) == 0)
		{
			fread(&Client->ID, sizeof(OBJID), 1, pAccount);
			result = true;
		}
		fclose(pAccount);
	}
	return result;
}
//
// Ctor and Dtor
CDatabaseRoot::CDatabaseRoot(void)
{
	AuthClients = new CFlexibleArray<CAuthClient*>();
	Clients = new CFlexibleArray<CGameClient*>();
}

CDatabaseRoot::~CDatabaseRoot(void)
{
	delete AuthClients;
	delete Clients;
}
//
