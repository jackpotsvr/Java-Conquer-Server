#include "StdAfx.h"
#include "DataMap.h"

//
// ------ CDataMap Class Implementation
//
// Static Variables
unsigned short CDataMap::m_NextServerID = 1;
// Static Functions 
unsigned short CDataMap::NextServerID() { return m_NextServerID++; }
//
//
// Ctor and Dtor
CDataMap::CDataMap(char* FileName)
{
	FILE* fp = fopen(FileName, "r");
	assert(fp);

	fpos_t file_pos = 268;
	fsetpos(fp, &file_pos);
	
	fread(&this->m_MaxX, sizeof(int), 1, fp);
	fread(&this->m_MaxY, sizeof(int), 1, fp);
	this->m_Flags = new DWORD[m_MaxX * m_MaxY];
#ifdef _DMAPS_COMPRESSED_
	for (int x = 0; x < m_MaxX; x++)
#else
	for (int y = 0; y < m_MaxY; y++)
#endif
	{
#ifdef _DMAPS_COMPRESSED_
		for (int y = 0; y < m_MaxY; y++)
#else
		for (int x = 0; x < m_MaxX; x++)
#endif
		{
			char baccess;	
			fread(&baccess, sizeof(char), 1, fp);
#ifndef _DMAPS_COMPRESSED_
			fgetpos(fp, &file_pos);
			file_pos += 5;
			fsetpos(fp, &file_pos);
#endif
			if (baccess == 0)
			{
				this->m_Flags[POS2INDEX(x, y, m_MaxX, m_MaxY)] = DMAP_FLAGS_ACCESS;
			}
			else
			{
				this->m_Flags[POS2INDEX(x, y, m_MaxX, m_MaxY)] = DMAP_FLAGS_NONE;
			}
		}
	}
}

CDataMap::~CDataMap(void)
{
	if (m_Flags != NULL)
		delete[] m_Flags;
}
//
