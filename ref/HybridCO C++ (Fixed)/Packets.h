#ifndef _PACKETS_H_
#define _PACKETS_H_
#include <windows.h>
#pragma comment(lib, "winmm.lib")

enum PACKET_TYPE : unsigned short
{
	AUTHLOGINPACKET_ID = 0x41B,
	AUTHLOGINPACKETREPLY_ID = 0x41F,
	SPAWNENTITYPACKET_ID = 0x3F6,
	MSGPACKET_ID = 0x3EC,
	CHARINFOPACKET_ID = 0x3EE,
	DATAPACKET_ID = 0x3F2,
	SYNCPACKET_ID = 0x3F9,
	ITEMPACKET_ID = 0x3F0
};

struct PACKET_HEAD
{
	WORD Size;
	PACKET_TYPE Type;
};

class IClassPacket
{
public:
	virtual void Deserialize(unsigned char* Ptr) = NULL;
	virtual WORD Size() = NULL;
	virtual void Serialize(unsigned char* Ptr) = NULL;
};

enum DATA_ID : unsigned short
{
	DATA_ID_END_FLY = 0x78,
	DATA_ID_TELEPORT_JUMP = 0x81,
	DATA_ID_GUI_DIALOG = 0x7E,
	DATA_ID_SET_LOCATION = 0x4A,
	DATA_ID_SET_MAP_COLOR = 0x68,
	DATA_ID_JUMP = 0x85,
	DATA_ID_LEVEL_UP = 0x5C,
	DATA_ID_FRIEND_INFO = 0x8C,
	DATA_ID_TELEPORT = 0x56,
	DATA_ID_LOAD_SCREEN = 0x72,
	DATA_ID_REMOVE_ENTITY = 0x84,
	DATA_ID_CHANGE_PK_MODE = 0x60,
	DATA_ID_REVIVE = 0x5E,
	DATA_ID_REQUEST_ENTITY = 0x66,
	DATA_ID_CHANGE_ACTION = 0x51,
	DATA_ID_CHANGE_DIRECTION = 0x4F,
	DATA_ID_HOTKEYS = 0x4B,
	DATA_ID_CONFIRM_ASSOCIATES = 0x4C,
	DATA_ID_CONFIRM_PROFICIENCIES = 0x4D,
	DATA_ID_CONFIRM_SPELLS = 0x4E,
	DATA_ID_CONFIRM_GUILD = 0x61,
	DATA_ID_LOGIN = 0x82,
	DATA_ID_CHANGE_AVATAR = 0x8E,
	DATA_ID_ENTER_PORTAL = 0x55,
	DATA_ID_DELETE_CHARACTER = 0x5F,
	DATA_ID_END_TRANSFORM = 0x76,
	DATA_ID_MINING = 0x63,
	DATA_ID_START_VEND = 0x6F,
	DATA_ID_SPAWN_EFFECT = 0x83,
	DATA_ID_NONE = 0x00
};

struct DataPacket
{
	PACKET_HEAD Head;
	DWORD TimeStamp;
	DWORD UID;
	DWORD dwParam;
	WORD wParam1;
	WORD wParam2;
	WORD wParam3;
	DATA_ID ID;
	//Confirmed:4351

	DataPacket()
	{
		Head.Size = sizeof(DataPacket);
		Head.Type = DATAPACKET_ID;
		TimeStamp = timeGetTime();
		ID = DATA_ID_NONE;
		wParam3 = 0;
	}
};

enum STATUS_FLAG : DWORD
{
	STATUS_FLAG_NONE = 0x00,
	STATUS_FLAG_FLASH = 0x01,
	STATUS_FLAG_POISONED = 0x02,
	STATUS_FLAG_XPSKILLS = 0x10,
	STATUS_FLAG_GHOST = 0x20,
	STATUS_FLAG_TEAMLEADER = 0x40,
	STATUS_FLAG_XPSHIELD = 0x100,
	STATUS_FLAG_STIGMA = 0x200,
	STATUS_FLAG_DEAD = 0x400,
	STATUS_FLAG_REDNAME = 0x4000,
	STATUS_FLAG_BLACKNAME = 0x8000,
	STATUS_FLAG_SUPERMAN = 0x40000,
	STATUS_FLAG_INVISIBLE = 0x400000,
	STATUS_FLAG_CYCLONE = 0x800000,
	STATUS_FLAG_FLY = 0x8000000,

	STATUS_FLAG_Q_Curse = 0x01,
	STATUS_FLAG_Q_HeavenBless = 0x02
};

enum SYNC_ID
{
	SYNC_ID_HP = 0x00,
	SYNC_ID_MAX_HP = 0x01,
	SYNC_ID_MP = 0x02,
	SYNC_ID_MAX_MP = 0x03,
	SYNC_ID_MONEY = 0x04,
	SYNC_ID_EXPERIENCE = 0x05,
	SYNC_ID_PKPOINTS = 0x06,
	SYNC_ID_JOB = 0x07,
	SYNC_ID_NONE = 0xFFFFFFFF,
	SYNC_ID_Q_RAISE_FLAG = 0x08,
	SYNC_ID_STAMINA = 0x09,
	SYNC_ID_STAT_POINTS = 0x0B,
	SYNC_ID_MODEL = 0x0C,
	SYNC_ID_LEVEL = 0x0D,
	SYNC_ID_SPIRIT = 0x0E,
	SYNC_ID_STAT_VITALITY = 0x0F,
	SYNC_ID_STAT_STRENGTH = 0x10,
	SYNC_ID_STAT_AGILITY= 0x11,
	SYNC_ID_RAISE_FLAG = 0x1A,
	SYNC_ID_HAIR = 0x1B,
	SYNC_ID_CONQUER_PTS = 0x1E,
	SYNC_ID_XPCIRCLE = 0x1F,
	SYNC_ID_DOUBLE_EXP_TIMER = 0x13,
	SYNC_ID_CURSE_TIMER = 0x15,
	SYNC_ID_LUCKY_TIMER = 0x1D,
	SYNC_ID_BLESSING_TIMER = 0x12
};

enum ITEM_MODE : unsigned short
{
	ITEM_MODE_STD = 1,
	ITEM_MODE_TRADE = 2,
	ITEM_MODE_VIEW = 4
};

enum ITEM_POSITION : unsigned short
{
	ITEM_POSITION_INVENTORY = 0,
	ITEM_POSITION_HEAD = 1,
	ITEM_POSITION_NECKLACE = 2,
	ITEM_POSITION_ARMOR = 3,
	ITEM_POSITION_RIGHT = 4,
	ITEM_POSITION_LEFT = 5,
	ITEM_POSITION_RING = 6,
	ITEM_POSITION_BOTTLE = 7,
	ITEM_POSITION_BOOTS = 8,
	ITEM_POSITION_GARMENT = 9
};

class CItem
{
public:
	VIRTUALPROPERTY_BOTH(DWORD, UID, set_UID, get_UID)
	VIRTUALPROPERTY_BOTH(OBJID, ID, set_ID, get_ID)
	VIRTUALPROPERTY_BOTH(BYTE, Bless, set_Bless, get_Bless)
	VIRTUALPROPERTY_BOTH(BYTE, Enchant, set_Enchant, get_Enchant)
	VIRTUALPROPERTY_BOTH(BYTE, Plus, set_Plus, get_Plus)
	VIRTUALPROPERTY_BOTH(WORD, Reborn, set_Reborn, get_Reborn)
	VIRTUALPROPERTY_GET(BYTE*, Sockets, get_Sockets); 
};

struct ItemPacket : CItem
{
	PACKET_HEAD Head;
	DWORD UID;
	DWORD ID;
	WORD Durability;
	WORD MaxDurability;
	ITEM_MODE Mode;
	ITEM_POSITION Position;
	DWORD dwUnknown; /* UNKNOWN */
	BYTE Sockets[2];
	WORD Reborn;
	BYTE Plus;
	BYTE Bless;
	BYTE Enchant;
	char Pad[6];

	DWORD get_UID() { return UID; }
	void set_UID(DWORD value) { UID = value; }
	OBJID get_ID() { return ID; }
	void set_ID(OBJID value) { ID = value; }
	BYTE get_Bless() { return Bless; }
	void set_Bless(BYTE value) { Bless = value; }
	BYTE get_Enchant() { return Enchant; }
	void set_Enchant(BYTE value) { Enchant = value; }
	BYTE get_Plus() { return Plus; }
	void set_Plus(BYTE value) { Plus = value; }
	WORD get_Reborn() { return Reborn; }
	void set_Reborn(WORD value) { Reborn = value; }
	BYTE* get_Sockets() { return Sockets; }

	ItemPacket()
	{
		Head.Size = sizeof(ItemPacket);
		Head.Type = ITEMPACKET_ID;
		MaxDurability = 100;
		Mode = ITEM_MODE_STD;
	}
};

class CSyncPacket : public IClassPacket
{
private:
	unsigned char* Buffer;
public:
	struct Data
	{
		SYNC_ID ID;
		DWORD loInt64;
		DWORD hiInt64;
		
		__int64 get_Value() 
		{ 
			__int64 value = hiInt64;
			value = (value << 32) | loInt64;
			return value;
		}
		void set_Value(__int64 value)
		{
			loInt64 = (value & 0xFFFFFFFF);
			hiInt64 = ((value >> 32) & 0xFFFFFFFF);
		}
		__declspec(property(put = set_Value, get = get_Value)) __int64 Value;

		Data(SYNC_ID id, __int64 value)
		{
			ID = id;
			Value = value;
		}
	};

	WORD Size() { return *((WORD*)(Buffer)); }
	void Serialize(unsigned char* Ptr) { memcpy(Ptr, Buffer, Size()); }
	void Deserialize(unsigned char* Ptr) { throw NOT_IMPL; }
	
	DWORD get_UID() { return *((DWORD*)(Buffer + 4)); }
	void set_UID(DWORD value) { *((DWORD*)(Buffer + 4)) = value; }
	__declspec(property(put = set_UID, get = get_UID)) DWORD UID;
	int get_Count() { return *((int*)(Buffer + 8)); }
	void set_Count(int value);
	__declspec(property(put = set_Count, get = get_Count)) int Count;
	CSyncPacket::Data* get_Elements() { return (CSyncPacket::Data*)(Buffer + 12); }
	__declspec(property(get = get_Elements)) CSyncPacket::Data* Elements;

	CSyncPacket(int _Count) { Buffer = NULL; set_Count(_Count); }
	CSyncPacket() { Buffer = NULL; }
	~CSyncPacket() { if (Buffer != NULL) delete[] Buffer; }
};

enum CHAT_TYPE
{
	CHAT_TYPE_TALK = 0x7D0,
	CHAT_TYPE_TEAM = 0x7D3,
	CHAT_TYPE_GUILD = 0x7D4,
	CHAT_TYPE_WHISPER = 0x7D1,
	CHAT_TYPE_SERVICE = 0x7DE,
	CHAT_TYPE_TOP_LEFT = 0x7D5,
	CHAT_TYPE_CENTER = 0x7DB,
	CHAT_TYPE_BROADCAST = 0x9C4,
	CHAT_TYPE_DIALOG = 0x835,
	CHAT_TYPE_TOP_RIGHT = 0x83D,
	CHAT_TYPE_CLR_TOP_RIGHT = 0x83C,
	CHAT_TYPE_MSG_BOARD = 0x899,
	CHAT_TYPE_NONE = 0x00
};

class CMsgPacket : public IClassPacket
{
private:
	int Msg_Len, To_Len, From_Len;
	char* m_Msg, *m_To, *m_From;
	bool new_Msg, new_To, new_From;
public:
	CHAT_TYPE ChatType;
	DWORD Color;
	DWORD SenderUID;

	char* get_Message();
	void set_Message(char* value);
	__declspec(property(put = set_Message, get = get_Message)) char* Message;
	char* get_To();
	void set_To(char* value);
	__declspec(property(put = set_To, get = get_To)) char* To;
	char* get_From();
	void set_From(char* value);
	__declspec(property(put = set_From, get = get_From)) char* From;
	
	CMsgPacket();
	~CMsgPacket();

	WORD Size();
	void Deserialize(unsigned char* Ptr);
	void Serialize(unsigned char* Ptr);
};

struct AuthLoginPacket
{
	PACKET_HEAD Head;
	char szUsername[16];
	DWORD Password[4];
	char szServer[16];
};

struct AuthLoginReplyPacket
{
	PACKET_HEAD Head;
	DWORD Key2;
	DWORD Key1;
	char szIPAddress[16];
	DWORD Port;

	AuthLoginReplyPacket()
	{
		Head.Size = sizeof(AuthLoginReplyPacket);
		Head.Type = AUTHLOGINPACKETREPLY_ID;
	}
};

struct SpawnEntityPacket
{
	PACKET_HEAD Head;	// 0
	DWORD UID;			// 4
	DWORD Model;		// 8
	DWORD StatusFlags;	// 12
	DWORD QStatusFlags;	// 16
	WORD GuildID;		// 20
	WORD GuildRank;		// 22
	DWORD Garment;		// 24
	DWORD Helmet;		// 28
	DWORD Armor;		// 32
	DWORD RightHand;	// 36
	DWORD LeftHand;		// 40
	DWORD unknown1;		// 44	/* UNKNOWN 1*/
	WORD Hitpoints;		// 48
	WORD Level;			// 50
	WORD X;				// 52
	WORD Y;				// 54
	WORD HairStyle;		// 56
	BYTE Facing;		// 58
	BYTE Action;		// 59
	WORD Reborn;		// 60
	DWORD unknown2;		// 64	/* UNKNOWN 2 */
	DWORD unknown3;		// 68	/* UNKNOWN 3 */
	DWORD unknown4;		// 72	/* UNKNOWN 4 */
	DWORD unknown5;		// 76	/* UNKNOWN 5 */
	BYTE StrsCount;		// 80
	BYTE StrsLength;	// 81
	char szName[16];	// 82
};

struct CharacterInfoPacket
{
	PACKET_HEAD Head;	// 0
	DWORD UID;			// 4
	DWORD Model;		// 8
	WORD HairStyle;		// 12
	int Money;			// 16
	int ConquerPoints;	// 20
	char Pad[22];		// 24
	char Stats[2 * 5];	// 46
	WORD Hitpoints;		// 56
	WORD Mana;			// 58
	WORD unknown1;		// 60 /* UNKNOWN1 */
	BYTE Level;			// 62
	BYTE Job;			// 63
	BYTE _0x05;			// 64
	BYTE Reborn;		// 65
	BYTE _0x01;			// 66
	
	static unsigned char* Create(char* Name, char* Spouse);
};

#endif