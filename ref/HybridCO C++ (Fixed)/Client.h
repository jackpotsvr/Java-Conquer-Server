#pragma once
#ifndef _CLIENT_H_
#define _CLIENT_H_

enum CLIENT_PHASE
{
	CLIENT_PHASE_LOGGEDOUT = 0,
	CLIENT_PHASE_AUTH = 1,
	CLIENT_PHASE_AUTH_SUCCESS = 2,
	CLIENT_PHASE_GAME = 3,
	CLIENT_PHASE_GAME_SUCCESS = 4,
	CLIENT_PHASE_COMPLETE = 5
};

enum ADMIN_FLAG
{
	ADMIN_NONE = 0,
	ADMIN_GM = 1,
	ADMIN_PM = 2
};

class CCustomWinsockClient;
class CAuthCryptography;
class CUserScreen;
class CEntity;
class IClassPacket;

class CAuthClient
{
public:
	CCustomWinsockClient* Socket;
	CAuthCryptography* Crypto;

	char szUsername[16];
	char szPassword[16];
	OBJID ID;
	CLIENT_PHASE Phase;

	void Send(void* Ptr);

	CAuthClient(void);
	~CAuthClient(void);
};

struct UserStats
{
	WORD Strength;
	WORD Agility;
	WORD Vitality;
	WORD Spirit;
	WORD Points;
};

template <typename T>
class CFlexibleArray;
class CItem;

class CGameClient
{
public:
	CCustomWinsockClient* Socket;
	CAuthCryptography* Crypto; // will be changed to blowfish l8er
	CEntity* Entity;
	CUserScreen* Screen;

	CFlexibleArray<CItem*>* Inventory;
	CFlexibleArray<CItem*>* Equipment;

	char szUsername[16];
	char szPassword[16];
	char szSpouse[16];

	int BaseMaxAttack;
	int BaseMinAttack;
	int BaseMagicAttack;
	WORD ItemHP;
	WORD ItemMP;
	WORD Gems[8];

	OBJID ID;
	CLIENT_PHASE Phase;
	ADMIN_FLAG Admin;
	int Money;
	int ConquerPoints;
	BYTE Job;
	
	UserStats Stats;
	WORD MaxMana, Mana;
	BYTE Stamina;

	void Send(void* Ptr, bool Delete=false);
	void Send(IClassPacket* cPtr, bool Delete=true);

	void CalculateHPAndMP();
	void CalculateAttack();
	void SendStatMsg();
	void TeleportTo(MAP_ID Map, WORD X, WORD Y);

	void LoadInventory();
	void LoadEquipment();
	
	bool AddInventory(CItem* pItem);

	CGameClient(void);
	~CGameClient(void);
};

#endif