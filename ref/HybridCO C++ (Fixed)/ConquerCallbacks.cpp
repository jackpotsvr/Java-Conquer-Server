#include "StdAfx.h"
#include "ConquerCallbacks.h"
#include "UserScreen.h"
#include "Client.h"

int RemoveEntityCallback(IMapObject* Sender, IMapObject* Param, void* Arg)
{
	((CGameClient*)Param->Owner)->Screen->Remove(Sender->UID);
	return 0;
}
int AddEntityCallback(IMapObject* Sender, IMapObject* Param, void* Arg)
{
	((CGameClient*)Param->Owner)->Screen->Add(Sender);
	return 0;
}