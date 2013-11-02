#include "StdAfx.h"
#include "Packets.h"
#include "Entity.h"

//
// ------ CEntity Class Implementation ------
//
//
// Properties
unsigned short CEntity::get_X() { return Data->X; }
void CEntity::set_X(unsigned short value) { Data->X = value; }
unsigned short CEntity::get_Y() { return Data->Y; }
void CEntity::set_Y(unsigned short value) { Data->Y = value; }
unsigned int CEntity::get_UID() { return Data->UID; }
void CEntity::set_UID(unsigned int value) { Data->UID = value; }
char* CEntity::get_szName() { return Data->szName; }
void CEntity::set_szName(char* value) 
{
	Data->StrsLength = __min(strlen(value), 15);
	Data->StrsCount = 1;
	memset(Data->szName, 0, 16);
	memcpy(Data->szName, value, Data->StrsLength);
}
void CEntity::set_Hitpoints(int value) 
{ 
	m_Hitpoints = value;
	Data->Hitpoints = __min(m_Hitpoints, 0xFFFF); 
}
int CEntity::get_Hitpoints() { return m_Hitpoints; }
unsigned int CEntity::get_StatusFlags() { return Data->StatusFlags; }
void CEntity::set_StatusFlags(unsigned int value) { Data->StatusFlags = value; }
bool CEntity::get_Dead() { return Data->Hitpoints > 0; }
void CEntity::set_Dead(bool value)
{
	//todo
}
//
// Ctor and Dtor
CEntity::CEntity(void* pOwner, ENTITY_FLAG EntityType)
{
	Data = new SpawnEntityPacket();
	Data->Head.Size = sizeof(SpawnEntityPacket);
	Data->Head.Type = SPAWNENTITYPACKET_ID;

	m_MaxHitpoints = 0;
	m_Owner = pOwner;
	m_EntityType = EntityType;
	m_MaxAttack = 0;
	m_MinAttack = 0;
	m_MagicAttack = 0;
	m_Defence = 0;
	m_MDefence = 0;
	m_PlusMDefence = 0;
	m_Dodge = 0;
}
CEntity::~CEntity(void)
{
	delete Data;
}
//
