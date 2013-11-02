#include <iostream>
#include "CThreadHandler.h"





CThreadHandler::~CThreadHandler()
{
    //dtor
}

void * CThreadHandler::RunFunction(void * input)
{
    CThreadHandler * pHandler = (CThreadHandler*)input;
    pHandler->pWork(pHandler->pParameter);

    delete pHandler->hThread;
    delete pHandler;
}

CThreadHandler::CThreadHandler()
{

}


void CThreadHandler::StartThread( void (*input)(void *), void * parameter )
{

    //pthread_t *hThread = new pthread_t();
    CThreadHandler * pHandler = new CThreadHandler;

    pHandler->hThread = new pthread_t();
    pHandler->pWork = input;
    pHandler->pParameter = parameter;

    int rc = pthread_create(pHandler->hThread, NULL, RunFunction, pHandler);
    Sleep(1);
    if (rc) {
        std::cout << "Error: unable to create thread," << rc << std::endl;
    }
}



