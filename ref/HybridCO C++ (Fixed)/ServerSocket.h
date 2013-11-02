#pragma once
#ifndef _SERVERSOCKET_H_
#define _SERVERSOCKET_H_
#include <windows.h>

class CServerSocket;

class CCustomWinsockClient
{
private:
	CServerSocket* Server;
	SOCKET hClient;
	sockaddr_in Address;
	unsigned char* Buffer;
public:
	// An async. variable that will stay consistent
	// it should be used as a pointer to a class that is harnesting
	// the CCustomWinsockClient class
	void* PublicWrapper;

	const CServerSocket* GetServer() { return Server; }
	const DWORD GetConnection() { return hClient; }
	const sockaddr_in GetAddress() { return Address; }
	const unsigned char* GetBuffer() { return Buffer; }

	// Returns 0 on full success
	// Returns 1 on success with data-truncation (less than the number of bytes specified in DataLen were sent)
	// Returns SOCKET_ERROR on failure
	int Send(void* Data, int DataLen);
	// Returns 0 on success, returns SOCKET_ERROR on failure
	int Disconnect();

	CCustomWinsockClient(CServerSocket* server, 
		SOCKET client, 
		sockaddr_in addr,
		int ClientBufferSize);
	~CCustomWinsockClient(void);
};

template <typename T>
class CFlexibleArray;

class CServerSocket
{
private:
	SOCKET Connection;
	WORD m_Port;
	BOOL m_Enabled;
	CFlexibleArray<CCustomWinsockClient*>* m_Clients;
	CRITICAL_SECTION m_Clients_Session;
	HANDLE m_ReceiveThread;
	HANDLE m_AcceptThread;
	int m_ClientBufferSize;

	static void AcceptThreadFunction(void* pArg);
	static void ReceiveThreadFunction(void* pArg);
public:
	int Backlog;

	void (*OnClientConnect)(CCustomWinsockClient* Client, CServerSocket* Server);
	void (*OnClientDisconnect)(CCustomWinsockClient* Client, CServerSocket* Server);
	void (*OnClientError)(CCustomWinsockClient* Client, CServerSocket* Server, int WSAError);
	void (*OnClientReceive)(CCustomWinsockClient* Client, CServerSocket* Server, unsigned char* Received, int ReceivedLength);

	WORD get_Port();
	// Throws a null-terminated string (const char*) if the server is enabled.
	void set_Port(WORD port);
	BOOL get_Enabled();
	// Throws a wsa errorcode if a winsock function fails.
	void set_Enabled(BOOL enabled);
	// Throws a null-terminated string (const char*) if the server is enabled.
	int get_ClientBufferSize();
	void set_ClientBufferSize(int clientbuffersize);

	__declspec(property(get = get_ClientBufferSize, put = set_ClientBufferSize)) int ClientBufferSize;
	__declspec(property(get = get_Port, put = set_Port)) WORD Port;
	__declspec(property(get = get_Enabled, put = set_Enabled)) BOOL Enabled;
	
	// Throws a wsa errorcode if a winsock function fails.
	CServerSocket(void);
	~CServerSocket(void);
};
#endif