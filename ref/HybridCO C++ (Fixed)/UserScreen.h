#pragma once
#ifndef _USER_SCREEN_H_
#define _USER_SCREEN_H_

enum MAP_OBJECT_TYPE
{
	MAP_OBJ_INVALID = 0,
	MAP_OBJ_PLAYER = 1,
	MAP_OBJ_MONSTER = 2,
	MAP_OBJ_PET = 3,
	MAP_OBJ_ITEM = 4,
	MAP_OBJ_NPC = 5,
	MAP_OBJ_SOB = 6
};

class IMapObject
{
public:
	VIRTUALPROPERTY_GET(unsigned short, X, get_X)
	VIRTUALPROPERTY_GET(unsigned short, Y, get_Y)
	VIRTUALPROPERTY_GET(MAP_ID, Map, get_Map)
	VIRTUALPROPERTY_GET(unsigned int, UID, get_UID)
	VIRTUALPROPERTY_GET(void*, Owner, get_Owner)
	VIRTUALPROPERTY_GET(MAP_OBJECT_TYPE, MapObjType, get_MapObjType)
};

class IBaseEntity;
template <typename T>
class CFlexibleArray;

class CGameClient;
class IClassPacket;

class CUserScreen
{
private:
	CGameClient* m_Owner;
public:
	CFlexibleArray<IMapObject*>* Objects;

	bool Add(IMapObject *mapObj);
	void Remove(OBJID mapObjID);
	void Wipe();
	void Cleanup();
	IMapObject* FindObject(OBJID mapObjID);

	CUserScreen(CGameClient* Owner);
	~CUserScreen(void);
};
#endif
