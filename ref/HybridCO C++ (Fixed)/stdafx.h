// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#define _CRT_SECURE_NO_WARNINGS

#pragma once
#ifndef _STDAFX_H_
#define _STDAFX_H_


#include "targetver.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <tchar.h>
#include <assert.h>

// Macros

#define WriteLn(str) printf("%s\r\n", str)

#define VIRTUALPROPERTY_BOTH(type, name, setter, getter) \
	virtual void setter ( type value ) = NULL; \
	virtual type getter () = NULL; \
	__declspec(property(put = setter, get = getter)) type name;

#define VIRTUALPROPERTY_GET(type, name, getter) \
	virtual type getter () = NULL; \
	__declspec(property(get = getter)) type name;

#define PROPERTY_BOTH(type, name, setter, getter) __declspec(property(put = setter, get = getter)) type name;
#define PROPERTY_GET(type, name, getter) __declspec(property(get = getter)) type name;

#define DRAGON_GEM 1
#define PHOENIX_GEM 0

// Constants
#define INVALID_NAME_STR "INVALID_NAME\0\0\0\0"
#define MAX_VIEW_DISTANCE 23
#define MAX_SCREEN_WIDTH 16
#define NOT_IMPL "Method not implemented."
#define NONE_STR "None\0\0\0\0\0\0\0\0\0\0\0\0"

// Types

typedef unsigned long OBJID;
typedef unsigned short WORD;
typedef unsigned long DWORD;
typedef unsigned char BYTE;
typedef int BOOL;
class IMapObject;
class IClassPacket;

struct MAP_ID
{
	unsigned short ServerID;
	unsigned short ID;

	MAP_ID(unsigned int Id)
	{
		ServerID = 0;
		ID = Id;
	}
	MAP_ID() 
	{ 
		ServerID = 0;
		ID = 0;
	}
	bool operator ==(const MAP_ID& m) 
	{
		return (this->ServerID == m.ServerID) && (this->ID == m.ID);
	}
};

class CSplitString
{
private:
	char** m_Elements;
	int m_Count;
public:
	char** get_Elements() { return m_Elements; }
	int get_Count() { return m_Count; }
	__declspec(property(get = get_Elements)) char** Elements;
	__declspec(property(get = get_Count)) int Count;

	CSplitString(char* String, char Delimeter);
	~CSplitString();
};

typedef int (*StdConquerCallback)(IMapObject* Sender, IMapObject* Parameter, void* Argument);

// Functions

inline int StrToInt(char* szInteger)
{
	int result = atoi(szInteger);
	if (result == 0)
	{
		if (strcmp(szInteger, "0") != 0)
		{
			char err[50];
			sprintf(err, "`%s` is not a valid integer.", szInteger);
			throw err;
		}
	}
	return result;
}
extern void PrintHexStr(unsigned char* Ptr, int Size);
extern inline unsigned short Distance(unsigned short x1, unsigned short y1, 
									  unsigned short x2, unsigned short y2);
extern void SendRangePacket(void* Packet, IMapObject* Entity, 
							bool SendSelf=false, 
							StdConquerCallback Callback=NULL,
							bool Delete=false);
extern void SendRangePacket(IClassPacket* Packet, IMapObject* Entity, 
							bool SendSelf=false, 
							StdConquerCallback Callback=NULL,
							bool Delete=true);

template <typename T>
void fread(void* _DstBuf, T Default, FILE* _File)
{
	if (fread(_DstBuf, sizeof(T), 1, _File) != 1)
	{
		memcpy(_DstBuf, &Default, sizeof(T));
	}
}
template <typename T>
void fread(void* _DstBuf, int Count, T* Default, FILE* _File) 
{
	if (fread(_DstBuf, sizeof(T), Count, _File) != Count)
	{
		memcpy(_DstBuf, Default, sizeof(T) * Count);
	}
}
template <typename T>
T freadval(T Default, int Count, FILE* _File)
{
	T temp;
	if (fread(&temp, sizeof(T), Count, _File) != Count)
		return Default;
	return temp;
}
template <typename T>
void fwriteval(T value, FILE* _File)
{
	fwrite(&value, sizeof(T), 1, _File);
}

#endif
// TODO: reference additional headers your program requires here
