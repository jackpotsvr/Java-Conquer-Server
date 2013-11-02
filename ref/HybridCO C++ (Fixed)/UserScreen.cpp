#include "StdAfx.h"
#include "UserScreen.h"
#include "FlexibleArray.h"
#include "Entity.h"
#include "Client.h"
#include "Packets.h"

//
// ------ CUserScreen Class Implementation ------
//
// Add Function
bool CUserScreen::Add(IMapObject *mapObj)
{
	bool added = false;
	Objects->ObtainSyncHandle();
	if (!Objects->Contains(mapObj, false))
	{
		Objects->Add(mapObj, false);
		added = true;
	}
	Objects->FreeSyncHandle();
	return added;
}
//
// Remove Function
//
void CUserScreen::Remove(OBJID mapObjID)
{
	Objects->ObtainSyncHandle();
	for (int i = 0; i < Objects->Count; i++)
	{
		if (Objects->Elements[i]->UID == mapObjID)
		{
			Objects->RemoveAt(i, false);
			break;
		}
	}
	Objects->FreeSyncHandle();
}
//
// FindObject Function
IMapObject* CUserScreen::FindObject(OBJID mapObjID)
{
	IMapObject* find = NULL;
	Objects->ObtainSyncHandle();
	for (int i = 0; i < Objects->Count; i++)
	{
		if (Objects->Elements[i]->UID == mapObjID)
		{
			find = Objects->Elements[i];
			break;
		}
	}
	Objects->FreeSyncHandle();
	return find;
}
//
// Clean Function
void CUserScreen::Cleanup()
{
	bool remove;
	IMapObject *pObject;

	Objects->ObtainSyncHandle();
	for (int i = 0; i < Objects->Count; i++)
	{
		remove = false;
		pObject = Objects->Elements[i];

		if (pObject->MapObjType == MAP_OBJ_MONSTER ||
			pObject->MapObjType == MAP_OBJ_SOB ||
			pObject->MapObjType == MAP_OBJ_PET)
		{
			remove = ((CEntity*)pObject)->Dead ||
				(Distance(m_Owner->Entity->X, m_Owner->Entity->Y, pObject->X, pObject->Y) > MAX_VIEW_DISTANCE);
		}
		else if (pObject->MapObjType == MAP_OBJ_PLAYER)
		{
			remove = (Distance(m_Owner->Entity->X, m_Owner->Entity->Y, pObject->X, pObject->Y) > MAX_VIEW_DISTANCE);
		}
		
		if (remove)
		{
			Objects->RemoveAt(i, false);
			i--;
		}
	}
	Objects->FreeSyncHandle();
}
//
// Wipe Function
void CUserScreen::Wipe()
{
	Objects->Clear();
}
//
// Ctor and Dtor
CUserScreen::CUserScreen(CGameClient* Owner)
{
	m_Owner = Owner;
	Objects = new CFlexibleArray<IMapObject*>();
}
CUserScreen::~CUserScreen(void)
{
	delete Objects;
}
//
