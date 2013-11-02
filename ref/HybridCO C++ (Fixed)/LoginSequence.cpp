#include "StdAfx.h"
#include "LoginSequence.h"

#include "Client.h"
#include "Entity.h"
#include "Packets.h"
#include "DatabaseRoot.h"

#include "FlexibleArray.h"
#include "ServerSocket.h"
#include "Cryptography.h"

void StartLogin(CGameClient *Client, PACKET_HEAD *pHead)
{
	struct Packet_Data
	{
		PACKET_HEAD Header;
		DWORD Key2;
		DWORD Key1;
	};
	Packet_Data* Packet = (Packet_Data*)pHead;

	Client->ID = Packet->Key1;
	CMsgPacket* Reply = new CMsgPacket();
	Reply->From = "SYSTEM";
	Reply->To = "ALLUSERS";
	Reply->Color = 0x00FFFFFF;
	Reply->Message = "Failed to load your character, please report this to an admin with your username!";
	Reply->ChatType = CHAT_TYPE_DIALOG;

	if (CDatabaseRoot::Core->LoadAccount(Client))
	{
		Reply->Message = "ANSWER_OK";
	
		CharacterInfoPacket* cInfo = (CharacterInfoPacket*)CharacterInfoPacket::Create(Client->Entity->szName, Client->szSpouse);
		cInfo->UID = Client->ID;
		cInfo->Model = Client->Entity->Data->Model;
		cInfo->HairStyle = Client->Entity->Data->HairStyle;
		cInfo->Money = Client->Money;
		cInfo->ConquerPoints = Client->ConquerPoints;
		memcpy(cInfo->Stats, &Client->Stats, sizeof(UserStats));
		cInfo->Hitpoints = Client->Entity->Hitpoints;
		cInfo->Mana = Client->Mana;
		cInfo->Level = (BYTE)Client->Entity->Data->Level;
		cInfo->Job = Client->Job;
		cInfo->Reborn = (BYTE)Client->Entity->Data->Reborn;

		CDatabaseRoot::Core->Clients->Add(Client);
		Client->Crypto->SetKeys(&Packet->Key1, &Packet->Key2);

		Client->Send(cInfo, true);
		Client->Send(Reply,true);
	}
	else
	{
		Client->Send(Reply);
		Client->Socket->Disconnect();
	}
}

void SetLocation(CGameClient *Client, DataPacket *Ptr)
{
	Ptr->UID = Client->ID;
	Ptr->dwParam = Client->Entity->Map.ID;
	Ptr->wParam1 = Client->Entity->X;
	Ptr->wParam2 = Client->Entity->Y;
	Client->Send(Ptr);
}

void SendHotkeys(CGameClient *Client, DataPacket *Ptr)
{
	if (Client->Phase == CLIENT_PHASE_GAME)
	{
		Client->Phase = CLIENT_PHASE_GAME_SUCCESS;
		Client->LoadInventory();
		Client->Send(Ptr);
	}
}

void CompleteLogin(CGameClient *Client)
{
	Client->CalculateHPAndMP();

	Client->Stamina = 100;
	Client->Entity->Hitpoints = Client->Entity->MaxHitpoints;

	CSyncPacket* sync = new CSyncPacket(2);
	sync->UID = Client->ID;
	sync->Elements[0] = CSyncPacket::Data(SYNC_ID_STAMINA, Client->Stamina);
	sync->Elements[1] = CSyncPacket::Data(SYNC_ID_HP, Client->Entity->Hitpoints);
	Client->Send(sync);

	Client->SendStatMsg();

	Client->Phase = CLIENT_PHASE_COMPLETE;
}