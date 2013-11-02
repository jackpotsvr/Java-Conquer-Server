#include "StdAfx.h"
#include "Processor.h"

#include "Client.h"
#include "Entity.h"
#include "Packets.h"
#include "DatabaseRoot.h"

#include "LoginSequence.h"
#include "DataPackets.h"

void CPacketProcessor::Process(CGameClient *Client, PACKET_HEAD *Header)
{
	switch (Header->Type)
	{
	case 0x41C: StartLogin(Client, Header); break;
	case 0x3F2:
		{
			DataPacket *Ptr = (DataPacket*)Header;
			switch (Ptr->ID)
			{
			case DATA_ID_LOAD_SCREEN: LoadScreen(Client, Ptr); break;
			case DATA_ID_SET_LOCATION: SetLocation(Client, Ptr); break;
			case DATA_ID_HOTKEYS: SendHotkeys(Client, Ptr); break;
			case DATA_ID_CONFIRM_ASSOCIATES: Client->Send(Ptr); break; // TO-DO
			case DATA_ID_CONFIRM_SPELLS: Client->Send(Ptr); break; // TO-DO
			case DATA_ID_CONFIRM_PROFICIENCIES: Client->Send(Ptr); break; // TO-DO
			case DATA_ID_CONFIRM_GUILD: Client->Send(Ptr); break; // TO-DO
			case DATA_ID_LOGIN: CompleteLogin(Client); break;
			case DATA_ID_JUMP: PlayerJump(Client, Ptr); break;
			case DATA_ID_REQUEST_ENTITY: RequestEntity(Client, Ptr); break;
			}
			break;
		}
	case 0x3EC:
		{
			CMsgPacket *Msg = new CMsgPacket();
			Msg->Deserialize((unsigned char*)Header);

			if (strlen(Msg->Message) > 0)
			{
				if (Msg->Message[0] == '@')
				{
					CCommandProcessor::Process(Client, Msg);
					break;
				}
			}

			switch (Msg->ChatType)
			{
			case CHAT_TYPE_WHISPER:
				{
					CGameClient* whisp = CDatabaseRoot::Core->FindClient(Msg->To);
					if (whisp != NULL)
					{
						whisp->Send(Msg, false);
					}
					break;
				}
			case CHAT_TYPE_TALK: SendRangePacket(Msg, Client->Entity, false, NULL, false); break;
			}

			delete Msg;
			break;
		}
	}
}

