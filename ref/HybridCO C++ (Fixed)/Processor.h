#pragma once
#ifndef _PROCESSOR_H_
#define _PROCESSOR_H_

struct PACKET_HEAD;
class CGameClient;

class CPacketProcessor
{
public:
	static void Process(CGameClient* Client, PACKET_HEAD* Header);
};

class CMsgPacket;
class CCommandProcessor
{
public:
	static void Process(CGameClient* Client, CMsgPacket* Command);
};

#endif
