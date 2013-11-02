#pragma once
#ifndef _DATABASE_ROOT_H_
#define _DATABASE_ROOT_H_

class CServerSocket;
class CAuthClient;
class CGameClient;
template <typename T>
class CFlexibleArray;
class CDatabaseRoot;
class IBaseEntity;

class CDatabaseRoot
{
public:
	static CDatabaseRoot* Core;
	static CServerSocket* AuthServer;
	static CServerSocket* GameServer;
	static void Create();
	static void Destroy();

	CFlexibleArray<CAuthClient*>* AuthClients;
	bool ValidAccount(CAuthClient* Client);

	CFlexibleArray<CGameClient*>* Clients;
	bool LoadAccount(CGameClient* Client);
	void LoadItems(CGameClient* Client, bool Inventory);
	void SaveItems(CGameClient* Client, bool Inventory);
	void SaveAccount(CGameClient* Client);
	void NewAccount(char* szAccount, char* szPassword);

	IBaseEntity* FindEntity(OBJID UID, MAP_ID MapID);
	CGameClient* FindClient(char* szName);

	CDatabaseRoot(void);
	~CDatabaseRoot(void);
};
#endif
