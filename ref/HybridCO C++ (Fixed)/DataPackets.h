#ifndef _PACKET_PROCESSOR_DATA_PACKETS_H_
#define _PACKET_PROCESSOR_DATA_PACKETS_H_

struct PACKET_HEAD;
struct DataPacket;
class CGameClient;

extern void LoadScreen(CGameClient *Client, DataPacket *Ptr); 
extern void CompleteLogin(CGameClient *Client);
extern void PlayerJump(CGameClient *Client, DataPacket *Ptr);
extern void RequestEntity(CGameClient *Client, DataPacket *Ptr);

#endif