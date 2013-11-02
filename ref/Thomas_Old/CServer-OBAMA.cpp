#include <iostream>
#include <string>

#include "CThreadHandler.h"
#include "CServer.h"


#define SUCCESS 0
#define FAILURE 1



CServer::CServer()
{

}

CServer::~CServer()
{
    closesocket(sListen);
    closesocket(sConnect);
    WSACleanup();
}

char CServer::StartServer(char * strIP, unsigned short usiPort)
{
    bListening = true;
    std::string str;

    WSAData wsaData;
    WORD DLLVERSION;
    DLLVERSION = MAKEWORD(2, 1);

    WSAStartup(DLLVERSION, &wsaData);

    SOCKADDR_IN addr;



    int length = sizeof(addr);

    sConnect = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);


    addr.sin_addr.s_addr = inet_addr(strIP);
    addr.sin_family = AF_INET;
    addr.sin_port = htons(usiPort);

    sListen = socket(AF_INET, SOCK_STREAM, NULL);

    bind(sListen, (SOCKADDR*)&addr, sizeof(addr));


    listen(sListen, SOMAXCONN);

    for(;bListening;) {

    if (sAccept = accept(sListen, (SOCKADDR*)&addr,&length)) {
        //CThreadHandler::StartThread(this->Server);
        sServer = sAccept;
        CThreadHandler::StartThread(pRoutine, (void*)this);
    }


    }

    return 0;

}

/**
void CServer::LoadServerCServer(CServer* Server)
{
    Server->pRoutine( (void*)Server );
}
*/



int CServer::Poll4RecvOk(SOCKET sAccept)
{
    std::cout << "POLL4RECV" << std::endl;
    struct fd_set readfds;
    struct timeval timeout;
    int isAccept;

    FD_ZERO(&readfds);
    FD_SET(sAccept, &readfds);

    isAccept = sAccept + 1;

    timeout.tv_sec = 1;
    timeout.tv_usec = 1000;

    int irSuccess = select(isAccept, &readfds, 0, 0, &timeout);

    return irSuccess;
}

int CServer::Poll4SendOk(SOCKET sAccept){
    struct fd_set sendfds;
    struct timeval timeout;
    int isAccept;

    FD_ZERO(&sendfds);
    FD_SET(sAccept, &sendfds);

    isAccept = sAccept + 1;

    timeout.tv_sec = 1;
    timeout.tv_usec = 1000;

    int irSuccess = select(isAccept, 0, &sendfds, 0, &timeout);

    return irSuccess;
};

//void * CServer::pFunction = &CServer::LoadServer(void);

