#pragma once
#ifndef _ENTITY_H_
#define _ENTITY_H_
#include "UserScreen.h"

enum ENTITY_FLAG
{
	ENTITY_PLAYER = 1,
	ENTITY_MONSTER = 2,
	ENTITY_SOB = 3,
	ENTITY_PET = 4
};

class IBaseEntity
{
public:
	VIRTUALPROPERTY_BOTH(bool, Dead, set_Dead, get_Dead)
	VIRTUALPROPERTY_BOTH(int, Hitpoints, set_Hitpoints, get_Hitpoints)
	VIRTUALPROPERTY_GET(int, MaxHitpoints, get_MaxHitpoints)
	VIRTUALPROPERTY_GET(void*, Owner, get_Owner)
	VIRTUALPROPERTY_GET(int, MaxAttack, get_MaxAttack)
	VIRTUALPROPERTY_GET(int, MinAttack, get_MinAttack)
	VIRTUALPROPERTY_GET(int, MagicAttack, get_MagicAttack)
	VIRTUALPROPERTY_GET(unsigned short, Defence, get_Defence)
	VIRTUALPROPERTY_GET(unsigned short, MDefence, get_MDefence)
	VIRTUALPROPERTY_GET(unsigned short, PlusMDefence, get_PlusMDefence)
	VIRTUALPROPERTY_GET(char, Dodge, get_Dodge)
	VIRTUALPROPERTY_GET(unsigned int, UID, get_UID)
	VIRTUALPROPERTY_GET(unsigned short, X, get_X)
	VIRTUALPROPERTY_GET(unsigned short, Y, get_Y)
	VIRTUALPROPERTY_GET(char*, szName, get_szName)
	VIRTUALPROPERTY_GET(unsigned int, StatusFlags, get_StatusFlags)
	VIRTUALPROPERTY_GET(MAP_ID, Map, get_Map)
};

struct SpawnEntityPacket;
class CEntity : public IBaseEntity, public IMapObject
{
private:
	int m_Hitpoints;
	int m_MaxHitpoints;
	void* m_Owner;
	int m_MaxAttack;
	int m_MinAttack;
	int m_MagicAttack;
	unsigned short m_Defence;
	unsigned short m_MDefence;
	unsigned short m_PlusMDefence;
	char m_Dodge;
	ENTITY_FLAG m_EntityType;
	MAP_ID m_Map;
public:
	SpawnEntityPacket* Data;

	// Properties internally held
	//
	ENTITY_FLAG get_EntityType() { return m_EntityType; }
	PROPERTY_GET(ENTITY_FLAG, EntityType, get_EntityType)
	//
	MAP_OBJECT_TYPE get_MapObjType()
	{
		switch (m_EntityType)
		{
		case ENTITY_PLAYER: return MAP_OBJ_PLAYER;
		case ENTITY_MONSTER: return MAP_OBJ_MONSTER;
		case ENTITY_SOB: return MAP_OBJ_SOB;
		case ENTITY_PET: return MAP_OBJ_PET;
		}
		return MAP_OBJ_INVALID;
	}
	PROPERTY_GET(MAP_OBJECT_TYPE, MapObjType, get_MapObjType)
	//
	MAP_ID get_Map() { return m_Map; }
	void set_Map(MAP_ID value) { m_Map = value; }
	PROPERTY_BOTH(MAP_ID, Map, set_Map, get_Map)
	//
	int get_MaxHitpoints() { return m_MaxHitpoints; }
	void set_MaxHitpoints(int value) { m_MaxHitpoints = value; }
	PROPERTY_BOTH(int, MaxHitpoints, set_MaxHitpoints, get_MaxHitpoints)
	//
	void* get_Owner() { return m_Owner; }
	//
	int get_MaxAttack() { return m_MaxAttack; }
	void set_MaxAttack(int value) { m_MaxAttack = value; }
	PROPERTY_BOTH(int, MaxAttack, set_MaxAttack, get_MaxAttack)
	//
	int get_MinAttack() { return m_MinAttack; }
	void set_MinAttack(int value) { m_MinAttack = value; }
	PROPERTY_BOTH(int, MinAttack, set_MinAttack, get_MinAttack)
	//
	int get_MagicAttack() { return m_MagicAttack; }
	void set_MagicAttack(int value) { m_MagicAttack = value; }
	PROPERTY_BOTH(int, MagicAttack, set_MagicAttack, get_MagicAttack)
	//
	unsigned short get_Defence() { return m_Defence; }
	void set_Defence(unsigned short value) { m_Defence = value; }
	PROPERTY_BOTH(unsigned short, Defence, set_Defence, get_Defence)
	//
	unsigned short get_MDefence() { return m_MDefence; }
	void set_MDefence(unsigned short value) { m_MDefence = value; }
	PROPERTY_BOTH(unsigned short, MDefence, set_MDefence, get_MDefence)
	//
	unsigned short get_PlusMDefence() { return m_PlusMDefence; }
	void set_PlusMDefence(unsigned short value) { m_PlusMDefence = value; }
	PROPERTY_BOTH(unsigned short, PlusMDefence, set_PlusMDefence, get_PlusMDefence)
	//
	char get_Dodge() { return m_Dodge; }
	void set_Dodge(char value) { m_Dodge = value; }
	PROPERTY_BOTH(char, Dodge, set_Dodge, get_Dodge)
	//

	// Properties based on the "Data" variable
	bool get_Dead();
	void set_Dead(bool value);
	//
	int get_Hitpoints();
	void set_Hitpoints(int value);
	//
	unsigned int get_UID();
	void set_UID(unsigned int value);
	PROPERTY_BOTH(unsigned int, UID, set_UID, get_UID)
	//
	unsigned short get_X();
	void set_X(unsigned short value);
	PROPERTY_BOTH(unsigned short, X, set_X, get_X)
	//
	unsigned short get_Y();
	void set_Y(unsigned short value);
	PROPERTY_BOTH(unsigned short, Y, set_Y, get_Y)
	//
	char* get_szName();
	void set_szName(char* value);
	PROPERTY_BOTH(char*, szName, set_szName, get_szName)
	//
	unsigned int get_StatusFlags();
	void set_StatusFlags(unsigned int value);
	PROPERTY_BOTH(unsigned int, StatusFlags, set_StatusFlags, get_StatusFlags)
	//

	CEntity(void* pOwner, ENTITY_FLAG EntityType);
	~CEntity(void);
};
#endif