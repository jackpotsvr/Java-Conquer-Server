#include "StdAfx.h"
#include "Client.h"
#include "Entity.h"
#include "Packets.h"
#include "ServerSocket.h"
#include "Cryptography.h"
#include "UserScreen.h"
#include "FlexibleArray.h"
#include "DatabaseRoot.h"
#include "ConquerCallbacks.h"

//
// ------ CGameClient Class Implementation ------
//
// AddInventory Function
bool CGameClient::AddInventory(CItem* pItem)
{
	
	return false;
}
//
// TeleportTo Function
void CGameClient::TeleportTo(MAP_ID Map, WORD X, WORD Y)
{
	bool respawn = (Distance(X, Y, Entity->X, Entity->Y) > MAX_SCREEN_WIDTH);
	
	DataPacket Teleport;
	Teleport.UID = this->ID;
	if (respawn)
	{
		Teleport.ID = DATA_ID_REMOVE_ENTITY;
		SendRangePacket(&Teleport, Entity, false, RemoveEntityCallback);
	}
	Entity->X = X;
	Entity->Y = Y;
	if (respawn)
	{
		SendRangePacket(Entity->Data, Entity, false, AddEntityCallback);
	}
	Teleport.ID = DATA_ID_TELEPORT;
	Teleport.dwParam = Map.ID;
	Teleport.wParam1 = X;
	Teleport.wParam2 = Y;
	this->Send(&Teleport);
}
//
// LoadInventory Function
void CGameClient::LoadInventory()
{
	CDatabaseRoot::Core->LoadItems(this, true);

	Inventory->ObtainSyncHandle();
	for (int i = 0; i < Inventory->Count; i++)
	{
		Send((ItemPacket*)Inventory->Elements[i]);
	}
	Inventory->FreeSyncHandle();
}
//
// SendStatMsg Function
void CGameClient::SendStatMsg()
{
	char szStatMsg[255];

	CMsgPacket* Msg = new CMsgPacket();
	Msg->ChatType = CHAT_TYPE_CENTER;
	Msg->Color = 0xFFFFFF;
	Msg->From = "SYSTEM";
	Msg->To = "ALL";
	sprintf(szStatMsg, 
		"Attack: %i~%i MagicAttack: %i Defence: %i MDefence: %i Dodge: %i",
		Entity->MinAttack, Entity->MaxAttack, 
		Entity->MagicAttack, 
		Entity->Defence, 
		Entity->MDefence + Entity->PlusMDefence, 
		Entity->Dodge
	);
	Msg->Message = szStatMsg;
	Send(Msg);
}
//
// CalculateAttack Function
void CGameClient::CalculateAttack()
{
	Entity->MaxAttack = (int)((Stats.Strength + BaseMaxAttack) * (1 + (Gems[DRAGON_GEM] * 0.01)));
	Entity->MinAttack = (int)((Stats.Strength + BaseMinAttack) * (1 + (Gems[DRAGON_GEM] * 0.01)));
	Entity->MagicAttack = (int)(BaseMagicAttack * (1 + (Gems[PHOENIX_GEM] * 0.01)));
}
//
// CalculateHPAndMP Function
void CGameClient::CalculateHPAndMP()
{
	// Mana
	char ManaBoost = 5;
    char JobID = Job / 10;
    if (JobID == 13 || JobID == 14)
	{
		char chk_sum = Job % 10;
		if (chk_sum >= 3 && chk_sum <= 5)
		{
			ManaBoost += 5 * (Job - (JobID * 10));
		}
	}
	MaxMana = (Stats.Spirit * ManaBoost) + ItemMP;
	Mana = __min(Mana, MaxMana);
	// Hitpoints
	const char HitpointBoost = 24;
    WORD StatHP = (Stats.Strength * 3) + (Stats.Agility * 3) + 
		(Stats.Spirit * 3) + (Stats.Vitality * HitpointBoost) + 1;
	switch (Job)
	{
		case 11: Entity->MaxHitpoints = (int)(StatHP * 1.05F); break;
		case 12: Entity->MaxHitpoints = (int)(StatHP * 1.08F); break;
		case 13: Entity->MaxHitpoints = (int)(StatHP * 1.10F); break;
		case 14: Entity->MaxHitpoints = (int)(StatHP * 1.12F); break;
		case 15: Entity->MaxHitpoints = (int)(StatHP * 1.15F); break;
		default: Entity->MaxHitpoints = (int)StatHP; break;
	}
	Entity->MaxHitpoints += ItemHP;
    Entity->Hitpoints = __min(Entity->Hitpoints, Entity->MaxHitpoints);
}
//
//
// Send Function
void CGameClient::Send(void* Ptr, bool Delete)
{
	unsigned short Size = *((unsigned short*)Ptr);
	unsigned char* encBuffer = new unsigned char[Size];
	Crypto->Encrypt(Ptr, encBuffer, Size);
	Socket->Send(encBuffer, Size);
	delete[] encBuffer;
	if (Delete)
	{
		delete[] Ptr;
	}
}
void CGameClient::Send(IClassPacket* cPtr, bool Delete)
{
	int Size = cPtr->Size();
	unsigned char* encBuffer = new unsigned	char[Size];
	cPtr->Serialize(encBuffer);

	Crypto->Encrypt(encBuffer, encBuffer, Size);
	Socket->Send(encBuffer, Size);
	delete[] encBuffer;
	if (Delete)
	{
		delete cPtr;
	}
}
//
// Ctor and Dtor
CGameClient::CGameClient(void)
{
	memset(Gems, 0, sizeof(WORD) * 8);
	BaseMaxAttack = 0;
	BaseMinAttack = 0;
	BaseMagicAttack = 0;
	ItemHP = 0;
	ItemMP = 0;

	Phase = CLIENT_PHASE_GAME;
	ID = NULL;
	
	Crypto = new CAuthCryptography();
	Entity = new CEntity(this, ENTITY_PLAYER);
	Screen = new CUserScreen(this);
	Inventory = new CFlexibleArray<CItem*>();
	Equipment = new CFlexibleArray<CItem*>();
}

CGameClient::~CGameClient(void)
{
	delete Crypto;
	delete Entity;
	delete Screen;
	Inventory->ObtainSyncHandle();
	for (int i = 0; i < Inventory->Count; i++)
	{
		delete Inventory->Elements[i];
	}
	Inventory->FreeSyncHandle();
	delete Inventory;
	Equipment->ObtainSyncHandle();
	for (int i = 0; i < Equipment->Count; i++)
	{
		delete Equipment->Elements[i];
	}
	Equipment->FreeSyncHandle();
	delete Equipment;
}
//
//
// ------ CAuthClient Class Implementation ------
//
// Send Function
void CAuthClient::Send(void* Ptr)
{
	unsigned short Size = *((unsigned short*)Ptr);
	unsigned char* encBuffer = new unsigned char[Size];
	Crypto->Encrypt(Ptr, encBuffer, Size);
	Socket->Send(encBuffer, Size);
	delete[] encBuffer;
}
//
// Ctor and Dtor
CAuthClient::CAuthClient(void)
{
	Phase = CLIENT_PHASE_AUTH;
	Crypto = new CAuthCryptography();
	ID = NULL;
}

CAuthClient::~CAuthClient(void)
{
	delete Crypto;
}
//
