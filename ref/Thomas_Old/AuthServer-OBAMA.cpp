#include <iostream>
#include <string>
#include "AuthServer.h"
#include <winsock.h>
#include <windows.h>

AuthServer::AuthServer()
{
    std::string receive;
    std::cout << "Auth Server starting..." << std::endl;
    WSAData wsaData;
    WORD DLLVERSION;
    DLLVERSION = MAKEWORD(2, 1);

    WSAStartup(DLLVERSION, &wsaData);

    SOCKADDR_IN addr;

    int length = sizeof(addr);

    sConnect = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

    //addr.sin_addr.s_addr = inet_addr(IP);
    addr.sin_addr.s_addr = inet_addr("127.0.0.1");
    addr.sin_family = AF_INET;
    addr.sin_port = htons(9958);

    sListen = socket(AF_INET, SOCK_STREAM, NULL);
    bind(sListen, (SOCKADDR*)&addr, sizeof(addr));

    listen(sListen, SOMAXCONN);
    std::cout << "Started to listen.." << std::endl;
    if (sAccept = accept(sListen, (SOCKADDR*)&addr,&length))
    {
    std::cout << "Socket acepted.." << std::endl;

    recv(sAccept, inbuf, 1, NULL);

    std::cout << "Received" << std::endl;
    }
    else
    {
        std::cout << "Not succesfully acepted." << std::endl;
    }




    //ctor
}

AuthServer::~AuthServer()
{
    //closesocket(sListen);
    //closesocket(sConnect);
    //WSACleanup();
    //dtor
    std::cout << "DTOR" << std::endl;
}
