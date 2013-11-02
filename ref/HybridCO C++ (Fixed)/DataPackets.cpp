#include "StdAfx.h"
#include "DataPackets.h"

#include "Client.h"
#include "Entity.h"
#include "Packets.h"
#include "DatabaseRoot.h"

#include "FlexibleArray.h"
#include "ServerSocket.h"

void RequestEntity(CGameClient *Client, DataPacket *Ptr)
{
	IBaseEntity* search = CDatabaseRoot::Core->FindEntity(Ptr->dwParam, Client->Entity->Map);
	if (search != NULL)
	{
		if (Distance(Client->Entity->X, Client->Entity->Y, search->X, search->Y) <= MAX_VIEW_DISTANCE)
		{
			Client->Send(((CEntity*)search)->Data);
		}
	}
}

void PlayerJump(CGameClient *Client, DataPacket *Ptr)
{
	WORD X = LOWORD(Ptr->dwParam);
	WORD Y = HIWORD(Ptr->dwParam);

	if (Distance(X, Y, Client->Entity->X, Client->Entity->Y) > MAX_SCREEN_WIDTH)
	{
		Client->Socket->Disconnect();
		return;
	}

	Client->Entity->X = X;
	Client->Entity->Y = Y;

	SendRangePacket(Ptr, Client->Entity, true);

	LoadScreen(Client, NULL);
}

void LoadScreen(CGameClient *Client, DataPacket *Ptr)
{
	if (Ptr != NULL) // LoadScreen() Standerd
	{
		Client->Screen->Wipe();

		Ptr->ID = DATA_ID_SET_MAP_COLOR;
		Ptr->dwParam = 0xFFFFFFFF;
		Client->Send(Ptr);
		// <todo = weather>
		// </todo>
	}
	else
	{
		Client->Screen->Cleanup();
	}

	// <Clients>
	CDatabaseRoot::Core->Clients->ObtainSyncHandle();
	for (int i = 0; i < CDatabaseRoot::Core->Clients->Count; i++)
	{
		CGameClient* iClient = CDatabaseRoot::Core->Clients->Elements[i];
		if (iClient->Entity->Map == Client->Entity->Map &&
			iClient->ID != Client->ID)
		{
			if (Distance(Client->Entity->X, Client->Entity->Y,
					iClient->Entity->X, iClient->Entity->Y) <= MAX_SCREEN_WIDTH)
			{
				if (Client->Screen->Add(iClient->Entity))
				{
					Client->Send(iClient->Entity->Data);
					if (Ptr != NULL) // LoadScreen() Standerd
					{
						iClient->Screen->Add(Client->Entity);
						iClient->Send(Client->Entity->Data);
					}
				}
			}
		}
	}
	CDatabaseRoot::Core->Clients->FreeSyncHandle();
	// </Clients>
}