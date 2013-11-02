#ifndef _PACKET_PROCESSOR_LOGIN_SEQ_H_
#define _PACKET_PROCESSOR_LOGIN_SEQ_H_

struct PACKET_HEAD;
struct DataPacket;
class CGameClient;

extern void StartLogin(CGameClient *Client, PACKET_HEAD *pHead);
extern void SetLocation(CGameClient *Client, DataPacket *Ptr);
extern void SendHotkeys(CGameClient *Client, DataPacket *Ptr);
extern void SendSpells(CGameClient *Client, DataPacket *Ptr);
extern void SendProficiencies(CGameClient *Client, DataPacket *Ptr);
extern void SendGuild(CGameClient *Client, DataPacket *Ptr);
extern void CompleteLogin(CGameClient *Client);

#endif