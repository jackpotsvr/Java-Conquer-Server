#include <iostream>
#include <windows.h>
#include <string>
#include <pthread.h>
#include "CThreadHandler.h"
#include "CAuthServer.h"
#include "CGameServer.h"


using namespace std;



int main()
{
    typedef void(*pFunction)(void *);

    SetConsoleTitle("Conquer Online 5017 Server");

    pFunction pAuthServer = CAuthServer::AuthServer;
    pFunction pGameServer = CGameServer::GameServer;


    CThreadHandler::StartThread(pAuthServer, NULL);
    std::cout << "GETS HERE?!" << std::endl;
    CThreadHandler::StartThread(pGameServer, NULL);

    cin.get();

    return 0;
}
