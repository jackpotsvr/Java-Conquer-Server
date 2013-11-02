#include "StdAfx.h"
#include "Processor.h"

#include "Client.h"
#include "Entity.h"
#include "Packets.h"
#include "DatabaseRoot.h"

#include "ServerSocket.h"


void CCommandProcessor::Process(CGameClient *Client, CMsgPacket *Command)
{
	CSplitString* Splitter = new CSplitString(Command->Message, ' ');
	
	int str_len = strlen(Splitter->Elements[0]);
	for (int i = 0; i < str_len; i++)
		Splitter->Elements[0][i] = tolower(Splitter->Elements[0][i]);

	try
	{
		if (strcmp(Splitter->Elements[0], "@quit") == 0)
		{
			Client->Socket->Disconnect();
		}
		else if (strcmp(Splitter->Elements[0], "@mm") == 0)
		{
			Client->TeleportTo(
				MAP_ID(StrToInt(Splitter->Elements[1])),
				StrToInt(Splitter->Elements[2]),
				StrToInt(Splitter->Elements[3])
			);
		}
		else if (strcmp(Splitter->Elements[0], "@item") == 0)
		{
			// @item [id] [plus] [bless] [enchant] [s1] [s2]
			if (Splitter->Count > 7)
			{	
				CItem* pItem = new ItemPacket();
				pItem->ID = StrToInt(Splitter->Elements[1]);
				pItem->Plus = StrToInt(Splitter->Elements[2]);
				pItem->Bless = StrToInt(Splitter->Elements[3]);
				pItem->Enchant = StrToInt(Splitter->Elements[4]);
				pItem->Sockets[0] = StrToInt(Splitter->Elements[5]);
				pItem->Sockets[1] = StrToInt(Splitter->Elements[6]);
				
			}
			else
			{
				throw "Too few arguments were passed";
			}
		}
	}
	catch (char* szErrMsg)
	{
		CMsgPacket* pMsg = new CMsgPacket();
		pMsg->ChatType = CHAT_TYPE_TOP_LEFT;
		pMsg->From = "SYSTEM";
		pMsg->To = "ALL";
		pMsg->Message = szErrMsg;
		Client->Send(pMsg);
	}

	delete Splitter;
}