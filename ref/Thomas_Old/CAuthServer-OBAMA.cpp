#include <iostream>
#include <string.h>
#include "CAuthServer.h"
#include "CPackets.h"
#include "CCryptography.h"
#include "CThreadHandler.h"


void CAuthServer::AuthServer(void * input)
{
    CAuthServer AuthServer;
}

void CAuthServer::HandleAuthenication(void * input)
{
    CAuthServer * pAuthServer = (CAuthServer *)input;
    char IP[16] = "127.000.000.001";
    CCryptography * pCryptography = new CCryptography;

    pAuthServer->bContinue = true;
    for (;pAuthServer->bContinue;)
    {

        if (pAuthServer->Poll4RecvOk(pAuthServer->sServer) < 1){ break; }
        //if (CAuthServer::Poll4RecvOk(pAuthServer->sServer) < 1) { break; }

        for(int inreceived = 1; inreceived == 1; inreceived = recv(pAuthServer->sServer, pAuthServer->inbuf, 200, NULL));


        if (pAuthServer->Poll4SendOk(pAuthServer->sServer) < 1){ break; }

        pAuthServer->buflength = strlen(pAuthServer->inbuf);

        //std::cout << ((pCryptography->UsingAlternate) ? "TRUE" : "FALSE") << std::endl;
        Sleep(1);
        pCryptography->Decrypt(pAuthServer->inbuf, pAuthServer->inbuf, pAuthServer->buflength);

        CPackets::SHEADER* header = (CPackets::SHEADER*)pAuthServer->inbuf;


        //std::cout << pAuthServer->buflength << std::endl;



        if (header->uiSize == pAuthServer->buflength)
        {
           if (header->Type == 0x41b)
           {
                CPackets::SAuthLoginPacket * Packet = (CPackets::SAuthLoginPacket*)pAuthServer->inbuf;
                std::cout << "Login Request: " << Packet->szUsername << std::endl;
           }

           unsigned long TheKeys = (unsigned int)(rand() % 0x98968 << 32);

           char * Key1 = new char[4];
           char * Key2 = new char[4];
           long * ulKey1 = new long;
           long * ulKey2 = new long;

           Key1[0] = (char)((unsigned long)(TheKeys & 0xff00000000000000L) >> 56);
           Key1[1] = (char)((TheKeys & 0xff000000000000) >> 48);
           Key1[2] = (char)((TheKeys & 0xff0000000000) >> 40);
           Key1[3] = (char)((TheKeys & 0xff00000000) >> 32);
           Key2[0] = (char)((TheKeys & 0xff000000) >> 24);
           Key2[1] = (char)((TheKeys & 0xff0000) >> 16);
           Key2[2] = (char)((TheKeys & 0xff00) >> 8);
           Key2[3] = (char)(TheKeys & 0xff);

           CPackets::SAuthLoginReplyPacket * ReplyPacket = new CPackets::SAuthLoginReplyPacket;

           memcpy((void*)ulKey1, (void*)Key1, sizeof(*ulKey1));
           memcpy((void*)ulKey2, (void*)Key2, sizeof(*ulKey2));

           strcpy(ReplyPacket->szIPAddress, IP);


           ReplyPacket->ulPort = 5816;
           ReplyPacket->ulKey1 = *ulKey1;
           ReplyPacket->ulKey2 = *ulKey2;
           ReplyPacket->AuthLoginReplyPacket();

           ///std::cout << ReplyPacket->Head.uiSize << " | ";
           pAuthServer->PACKETout = new char[sizeof(*ReplyPacket)];

           pCryptography->Encrypt(ReplyPacket, pAuthServer->PACKETout, sizeof(*ReplyPacket));

            pAuthServer->PACKEToutlength = sizeof(*(ReplyPacket));
            ///std::cout << strlen(pAuthServer->PACKETout) << std::endl;


            send(pAuthServer->sServer, pAuthServer->PACKETout, pAuthServer->PACKEToutlength, NULL);

            delete ReplyPacket;
            delete[] pAuthServer->PACKETout;
            delete ulKey1;
            delete ulKey2;
            delete[] Key1;
            delete[] Key2;
            delete pCryptography;

            pAuthServer->pRoutine = NULL;
        }

    }

    closesocket(pAuthServer->sServer);
}

CAuthServer::CAuthServer() // default constructor
{
    char * IP = STD_SERVER_IP;
    unsigned short Port = STD_AUTH_PORT;

    pRoutine = HandleAuthenication;

    StartServer(IP, Port);
}

CAuthServer::CAuthServer(char * IP, unsigned short Port)
{
    //StartServer(IP, Port);
}

CAuthServer::~CAuthServer()
{
    //dtor
}
