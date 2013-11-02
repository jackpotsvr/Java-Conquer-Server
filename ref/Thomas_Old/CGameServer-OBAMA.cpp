#include <string.h>
#include <iostream>
#include "CCryptography.h"
#include "CPackets.h"
#include "CGameServer.h"
#include <windows.h>


void CGameServer::GameServer(void * input)
{
    CGameServer GameServer;
}

void CGameServer::HandleGame(void * input)
{
    CGameServer * pGameServer = (CGameServer *)input;
    CCryptography * pCryptography = new CCryptography; /// = new CCryptography;

    pGameServer->bContinue = true;
    for (;pGameServer->bContinue;)
    {
        if (pGameServer->Poll4RecvOk(pGameServer->sServer) < 1){ break; }

        for(int inreceived = 1; inreceived == 1; inreceived = recv(pGameServer->sServer, pGameServer->inbuf, 200, NULL));

        if (pGameServer->Poll4SendOk(pGameServer->sServer) < 1){ break; }

        std::cout << pGameServer->inbuf << std::endl;

        pGameServer->buflength = strlen(pGameServer->inbuf);
        pCryptography->Decrypt(pGameServer->inbuf, pGameServer->inbuf, pGameServer->buflength);
        CPackets::SHEADER* pHeader = (CPackets::SHEADER*)pGameServer->inbuf;

        switch(pHeader->Type)
        {
            case 1010:
            {
                std::cout << "RECEIVING PACKET 1010!" << std::endl;
            }
            case 1052: //1052
            {

                struct Packet_Data
                {
                    CPackets::SHEADER Header;
                    unsigned long Key2;
                    unsigned long Key1;
                };

                Packet_Data* pData = (Packet_Data *)pGameServer->inbuf;
                pCryptography->SetKeys(&pData->Key1, &pData->Key2);
                //pCryptography->UsingAlternate = true;

                CPackets::SNEW_ROLE_PACKET* PACKET = new CPackets::SNEW_ROLE_PACKET;
                //CPackets::SANSWER_OK_PACKET* PACKET = new CPackets::SANSWER_OK_PACKET;

                pGameServer->PACKEToutlength = PACKET->Head.uiSize;



                pGameServer->PACKETout = new char [pGameServer->PACKEToutlength];

                pCryptography->Encrypt((void*)PACKET, (void*)pGameServer->PACKETout, pGameServer->PACKEToutlength);

                send(pGameServer->sServer, pGameServer->PACKETout, pGameServer->PACKEToutlength, NULL);
            }

            default:
            {
                std::cout << "PACKET: " << pHeader->Type << " is yet still under development!" <<  std::endl;
            }

            delete[] pGameServer->PACKETout;

        }


    closesocket(pGameServer->sServer);
    }
}

CGameServer::CGameServer()
{
    pRoutine = HandleGame;
    char * IP = STD_SERVER_IP;
    unsigned short Port = STD_GAME_PORT;
    StartServer(IP, Port);
}

CGameServer::CGameServer(char * IP, unsigned short Port)
{

    StartServer(IP, Port);
}

CGameServer::~CGameServer()
{

}

