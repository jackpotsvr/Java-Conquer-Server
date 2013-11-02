#pragma once
#ifndef _DATAMAP_H_
#define _DATAMAP_H_

//#define _DMAPS_COMPRESSED_
/*

This should be defined if your loading 
file(s) of my (compressed dmap file) structure...

struct HybridDMap
{
	int MaxX;
	int MaxY;
	bool Coordinates[MaxX*MaxY];
};

*/

enum DMAP_FLAGS
{
	DMAP_FLAGS_NONE = 0,
	DMAP_FLAGS_ACCESS = 1,
	DMAP_FLAGS_PLAYER_ON_TILE = 2,
	DMAP_FLAGS_MONSTER_ON_TILE = 4,
	DMAP_FLAGS_ITEM_ON_TILE = 8
};

inline bool TILE_ACCESS(DWORD flag) { return (flag & DMAP_FLAGS_ACCESS) == DMAP_FLAGS_ACCESS; }
inline bool TILE_PLAYER_ON(DWORD flag) { return (flag & DMAP_FLAGS_PLAYER_ON_TILE) == DMAP_FLAGS_PLAYER_ON_TILE; }
inline bool TILE_MONSTER_ON(DWORD flag) { return (flag & DMAP_FLAGS_MONSTER_ON_TILE) == DMAP_FLAGS_MONSTER_ON_TILE; }
inline bool TILE_ITEM_ON(DWORD flag) { return (flag & DMAP_FLAGS_ITEM_ON_TILE) == DMAP_FLAGS_ITEM_ON_TILE; }
inline int POS2INDEX(int x, int y, int cx, int cy) { return (x + y*cx); }

class CDataMap
{
private:
	static unsigned short m_NextServerID;
	DWORD* m_Flags;

	int m_MaxX;
	int m_MaxY;
public:
	int get_MaxX() { return m_MaxX; }
	int get_MaxY() { return m_MaxY; }

	DWORD GetTile(int x, int y) { return m_Flags[POS2INDEX(x, y, m_MaxX, m_MaxY)]; }
	void SetTile(int x, int y, DWORD value) { m_Flags[POS2INDEX(x, y, m_MaxX, m_MaxY)] = value; }

	__declspec(property(get = get_MaxX)) int MaxX;
	__declspec(property(get = get_MaxY)) int MaxY;

	static unsigned short NextServerID();

	CDataMap(char* FileName);
	~CDataMap(void);
};
#endif
