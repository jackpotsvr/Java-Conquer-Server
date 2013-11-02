#include "StdAfx.h"
#include "ServerSocket.h"
#include "FlexibleArray.h"
#include <process.h>
#pragma comment(lib, "ws2_32.lib")

//
// ------ CCustomWinsockClient Class Implementation ------
//
// Ctor and Dtor
CCustomWinsockClient::CCustomWinsockClient(CServerSocket* server, 
										   SOCKET client, 
										   sockaddr_in addr,
										   int BufferLength)
{
	Server = server;
	hClient = client;
	Address = addr;
	Buffer = new unsigned char[BufferLength];
}
CCustomWinsockClient::~CCustomWinsockClient(void)
{
	delete[] Buffer;
	closesocket(hClient);
}
//
// Send Function
int CCustomWinsockClient::Send(void* Data, int Length)
{
	int sent = send(hClient, (const char*)Data, Length, 0);
	if (sent == SOCKET_ERROR)
		return SOCKET_ERROR;
	else if (sent < Length)
		return 1;
	return 0;
}
//
// Disconnect Function
int CCustomWinsockClient::Disconnect()
{
	return ::closesocket(hClient);
}
//
//
// ------ CServerSocket Class Implementation ------
//
// Static Thread Functions
void CServerSocket::AcceptThreadFunction(void* pArg)
{
	CServerSocket* server = (CServerSocket*)pArg;
	while (server->m_Enabled)
	{
		sockaddr_in client_addr;
		int addr_len = sizeof(client_addr);
		SOCKET hClient = accept(server->Connection, (sockaddr*)&client_addr, &addr_len);
		if (hClient == INVALID_SOCKET)
		{
			if (!server->m_Enabled)
				break;
			continue;
		}
		
		CCustomWinsockClient* winsockClient = new CCustomWinsockClient(server, hClient, client_addr, server->m_ClientBufferSize);
		server->m_Clients->Add(winsockClient);

		if (server->OnClientConnect != NULL)
			server->OnClientConnect(winsockClient, server);

		Sleep(1);
	}
	server->m_AcceptThread = NULL;
}
void CServerSocket::ReceiveThreadFunction(void* pArg)
{
	CServerSocket* server = (CServerSocket*)pArg;

	while (server->m_Enabled)
	{
		CCustomWinsockClient* client;
		server->m_Clients->ObtainSyncHandle();

		for (int i = 0; i < server->m_Clients->Count; i++)
		{
			client = server->m_Clients->Elements[i];
			fd_set fd;
			fd.fd_array[0] = client->GetConnection();
			fd.fd_count = 1;
			timeval val;
			val.tv_usec = 1000;
			val.tv_sec = 0;
			bool remove_socket = false;

			int error = select(0, &fd, NULL, NULL, &val);
			if (error > 0)
			{
				int recv_len = recv(client->GetConnection(), (char*)client->GetBuffer(), server->m_ClientBufferSize, 0); 
				error = recv_len;
				if (error == 0)
				{
					remove_socket = true;
				}
				else if (error != SOCKET_ERROR)
				{
					unsigned char* temp_buffer = new unsigned char[recv_len];
					memcpy(temp_buffer, client->GetBuffer(), recv_len);
					if (server->OnClientReceive != NULL)
						server->OnClientReceive(client, server, temp_buffer, recv_len);
					delete[] temp_buffer;
				}
			}
				
			if (error == SOCKET_ERROR)
			{
				if (!server->m_Enabled)
					return;
				else
				{
					if (server->OnClientError != NULL)
						server->OnClientError(client, server, WSAGetLastError());
					remove_socket = true;
				}
			}

			if (remove_socket)
			{
				if (server->OnClientDisconnect != NULL)
					server->OnClientDisconnect(client, server);
				server->m_Clients->RemoveAt(i, false);
				delete client;
				i--;
			}
		}
		server->m_Clients->FreeSyncHandle();
		Sleep(1);
	}
	server->m_ReceiveThread = NULL;
}
//
// ClientBufferSize Property
int CServerSocket::get_ClientBufferSize()
{
	return m_ClientBufferSize;
}
void CServerSocket::set_ClientBufferSize(int clientbuffersize)
{
	if (m_Enabled)
		throw "You cannot modify the value of the client buffer size while the server is active!";
	m_ClientBufferSize = clientbuffersize;
}
//
// Port Property
WORD CServerSocket::get_Port()
{
	return htons(m_Port);
}
void CServerSocket::set_Port(WORD port)
{
	if (m_Enabled)
		throw "You cannot modify the value of the port while the server is active!";
	m_Port = htons(port);
}
//
// Enabled Property
BOOL CServerSocket::get_Enabled()
{
	return m_Enabled;
}
void CServerSocket::set_Enabled(BOOL enabled)
{
	if (m_Enabled != enabled)
	{
		m_Enabled = enabled;
		if (m_Enabled)
		{
			m_Clients->Clear();
			Connection = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
			if (Connection == INVALID_SOCKET)
				throw WSAGetLastError();
	
			sockaddr_in addr;
			addr.sin_family = AF_INET;
			addr.sin_addr.s_addr = inet_addr("0.0.0.0");
			addr.sin_port = m_Port;
			if (bind(Connection, (sockaddr*)&addr, sizeof(addr)) == SOCKET_ERROR)
				throw WSAGetLastError();

			if (listen(Connection, Backlog) == SOCKET_ERROR)
				throw WSAGetLastError();

			m_AcceptThread = (HANDLE)_beginthread(AcceptThreadFunction, 0, this);
			m_ReceiveThread = (HANDLE)_beginthread(ReceiveThreadFunction, 0, this);
		}
		else
		{
			if (closesocket(Connection) != 0)
				throw WSAGetLastError();
			WaitForSingleObject(m_ReceiveThread, INFINITE);
			WaitForSingleObject(m_AcceptThread, INFINITE);

			if (OnClientDisconnect != NULL)
			{
				m_Clients->ObtainSyncHandle();
				for (int i = 0; i < m_Clients->Count; i++)
				{
					OnClientDisconnect(m_Clients->Elements[i], this);
					delete m_Clients->Elements[i];
				}
				m_Clients->FreeSyncHandle();
			}
			m_Clients->Clear();
		}
	}
}
//
// Ctor and Dtor
CServerSocket::CServerSocket(void)
{
	WSAData data;
	if (WSAStartup(0x202, &data) == SOCKET_ERROR)
		throw WSAGetLastError();
	::InitializeCriticalSection(&m_Clients_Session);

	OnClientConnect = NULL;
	OnClientDisconnect = NULL;
	OnClientReceive = NULL;
	OnClientError = NULL;

	m_Clients = new CFlexibleArray<CCustomWinsockClient*>();
	m_Enabled = FALSE;
	m_ReceiveThread = NULL;
	m_AcceptThread = NULL;

	Backlog = SOMAXCONN;
	m_ClientBufferSize = 4096;
}
CServerSocket::~CServerSocket(void)
{
	Enabled = false;
	::DeleteCriticalSection(&m_Clients_Session);
	delete m_Clients;
}
//
