using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;

namespace COServer_Project
{
    public delegate void SocketEvent(object Sender, HybridSocket Socket);
    public delegate void SocketErrorEvent(object Sender, HybridSocket Socket, SocketError Error);
    public delegate void SocketDisconnectEvent(object Sender, HybridSocket Socket, string Debug);

    public sealed class HybridSocket
    {
        [DllImport("ws2_32.dll", SetLastError = true)]
        private static extern unsafe int send([In] IntPtr socketHandle, [In] byte* pinnedBuffer, [In] uint len, [In] SocketFlags socketFlags);

        public object Wrapper = null;
        public ServerSocket Server;
        public bool Disconnected = false;
        public byte[] Buffer;
        public byte[] Packet
        {
            get
            {
                byte[] ret = new byte[BufferLength];
                Array.Copy(Buffer, 0, ret, 0, BufferLength);
                return ret;
            }
        }
        public Socket WinSock;
        public int UID = -1, BufferLength;
        public HybridSocket(uint BufferSize)
        {
            Buffer = new byte[BufferSize];
        }
        public void Disconnect()
        {
            try
            {
                if (WinSock == null)
                    return;
                WinSock.Shutdown(SocketShutdown.Both);
                WinSock.Close();
            }
            catch { }
            if (Server != null)
                Server.InvokeDisconnect(this, "Socket self-Disposed");
        }
        public string RemoteAddress
        {
            get
            {
                try
                {
                    return WinSock.RemoteEndPoint.ToString().Split(':')[0];
                }
                catch (Exception)
                {
                    if (Server != null)
                        Server.InvokeDisconnect(this, "Socket Disposed");
                    return "";
                }
            }
        }
        public unsafe void SendBlock(byte* buffer, uint buffsize)
        {
            send(WinSock.Handle, buffer, buffsize, SocketFlags.None);
        }
    }

    public sealed class ClientSocket
    {
        public event SocketEvent OnClientConnecting;
        public event SocketEvent OnClientConnect;
        public event SocketEvent OnClientDisconnect;
        public event SocketErrorEvent OnClientError;
        public event SocketEvent OnReceivePacket;
        public HybridSocket Socket;
        private string SocketHost;
        public string Host
        {
            get { return SocketHost; }
            set
            {
                if (SocketEnabled) throw new Exception("Cannot Change Host While Client is Active.");
                SocketHost = value;
            }
        }
        private ushort SocketPort = 0;
        public ushort Port
        {
            get { return SocketPort; }
            set
            {
                if (SocketEnabled) throw new Exception("Cannot Change Port While Client is Active.");
                SocketPort = value;
            }
        }
        private uint SocketMaxPacketSize = 0xFFFF;
        public uint MaxPacketSize
        {
            get { return SocketMaxPacketSize; }
            set
            {
                if (SocketEnabled) throw new Exception("Cannot Change Max Packet Size While Client is Active.");
                SocketMaxPacketSize = value;
            }
        }
        private bool SocketEnabled = false;
        public bool Enabled
        {
            get { return SocketEnabled; }
            set
            {
                try
                {
                    SocketEnabled = value;
                    if (SocketEnabled)
                    {
                        if (Port == 0) throw new Exception("The Integer 0 is an Invalid Port Value");
                        if (Host == "") throw new Exception("The String <null> is an Invalid IPAddress Value");
                        if (MaxPacketSize == 0) throw new Exception("The Integer 0 is an Invalid Max Packet Size Value");

                        IPEndPoint Bind = new IPEndPoint(IPAddress.Parse(Host), (int)Port);
                        Socket = new HybridSocket(MaxPacketSize);
                        Socket.UID = 1;
                        Socket.WinSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        Socket.WinSock.Connect(Bind);
                        if (OnClientConnecting != null)
                            OnClientConnecting.Invoke(this, Socket);
                        if (Socket.WinSock.Connected)
                        {
                            if (OnClientConnect != null)
                                OnClientConnect.Invoke(this, Socket);
                            Socket.WinSock.BeginReceive(Socket.Buffer, 0, (int)MaxPacketSize, SocketFlags.None, new AsyncCallback(ReceivePacket), Socket);
                        }
                    }
                    else
                    {
                        if (OnClientDisconnect != null)
                            OnClientDisconnect.Invoke(this, Socket);
                        Socket.WinSock.Shutdown(SocketShutdown.Both);
                        Socket.WinSock.Close();
                        Enabled = false;
                    }
                }
                catch (Exception E)
                {
                    Enabled = false;
                    throw E;
                }
            }
        }
        public void ReceivePacket(IAsyncResult res)
        {
            Socket = (HybridSocket)res.AsyncState;
            SocketError Error;
            try
            {
                int Size = Socket.WinSock.EndReceive(res, out Error);
                if (Error == SocketError.Success && Size > 0)
                {
                    if (OnReceivePacket != null)
                        OnReceivePacket.Invoke(this, Socket);
                    Socket.WinSock.BeginReceive(Socket.Buffer, 0, (int)MaxPacketSize, SocketFlags.None, new AsyncCallback(ReceivePacket), Socket);
                }
                else
                {
                    if (OnClientError != null)
                        OnClientError(this, Socket, Error);
                    if (!Socket.WinSock.Connected)
                    {
                        if (OnClientDisconnect != null)
                            OnClientDisconnect.Invoke(this, Socket);
                        Enabled = false;
                    }
                }
            }
            catch (Exception)
            {
                if (OnClientDisconnect != null)
                    OnClientDisconnect.Invoke(this, Socket);
                Enabled = false;
            }
        }
    }
    public sealed class ServerSocket
    {
        public event SocketEvent OnClientConnect;
        public event SocketDisconnectEvent OnClientDisconnect;
        public event SocketEvent OnReceivePacket;
        public event SocketErrorEvent OnClientError;
        public Hashtable Connections = new Hashtable();
        public Socket Socket;
        private int NextUID = 0;
        private ushort SocketPort = 0;
        public ushort Port
        {
            get { return SocketPort; }
            set
            {

                if (SocketEnabled) throw new Exception("Cannot Change Port While Server is Active.");
                SocketPort = value;
            }
        }
        private ushort SocketMaxThreads = 100;
        public ushort MaxThreads
        {
            get { return SocketMaxThreads; }
            set
            {
                if (SocketEnabled) throw new Exception("Cannot Change Max Threads Value While Server is Active.");
                SocketMaxThreads = value;
            }
        }
        private uint SocketMaxPacketSize = 0xFFFF;
        public uint MaxPacketSize
        {
            get { return SocketMaxPacketSize; }
            set
            {
                if (SocketEnabled) throw new Exception("Cannot Change Max Packet Size While Server is Active.");
                SocketMaxPacketSize = value;
            }
        }
        private bool SocketEnabled = false;
        public bool Enabled
        {
            get { return SocketEnabled; }
            set
            {
                SocketEnabled = value;
                if (SocketEnabled)
                {
                    if (Port == 0) throw new Exception("The Integer 0 is an Invalid Port Value");
                    if (MaxPacketSize == 0) throw new Exception("The Integer 0 is an Invalid Max Packet Size Value");
                    if (MaxThreads == 0) throw new Exception("The Integer 0 is an Invalid Max Threads Value");

                    IPEndPoint Bind = new IPEndPoint(IPAddress.Any, (int)Port);
                    Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    Socket.Bind(Bind);
                    Socket.Listen((int)MaxThreads);
                    Socket.BeginAccept(new AsyncCallback(AcceptConnection), new HybridSocket(MaxPacketSize));
                }
                else
                {
                    try
                    {
                        foreach (DictionaryEntry DE in Connections)
                        {
                            HybridSocket HSocket = (HybridSocket)DE.Value;
                            InvokeDisconnect(HSocket, "Enabled = false");
                        }
                    }
                    catch { }
                    Socket.Close();                    
                    Connections.Clear();
                }
            }
        }
        private void AcceptConnection(IAsyncResult res)
        {
            try
            {
                NextUID++;
                HybridSocket HSocket = (HybridSocket)res.AsyncState;
                HSocket.WinSock = Socket.EndAccept(res);
                HSocket.UID = NextUID;
                HSocket.Server = this;
                Connections.Add(HSocket.UID, HSocket);
                if (OnClientConnect != null)
                    OnClientConnect.Invoke(this, HSocket);
                Socket.BeginAccept(new AsyncCallback(AcceptConnection), new HybridSocket(MaxPacketSize));
                HSocket.WinSock.BeginReceive(HSocket.Buffer, 0, (int)MaxPacketSize, SocketFlags.None, new AsyncCallback(ReceivePacket), HSocket);
            }
            catch (Exception)
            {
                    Socket.BeginAccept(new AsyncCallback(AcceptConnection), new HybridSocket(MaxPacketSize));
            }
        }
        private void ReceivePacket(IAsyncResult res)
        {
            HybridSocket HSocket = (HybridSocket)res.AsyncState;
            SocketError Error;
            try
            {
                if (HSocket.WinSock.Connected && !HSocket.Disconnected)
                {
                    HSocket.BufferLength = HSocket.WinSock.EndReceive(res, out Error);
                    if (Error == SocketError.Success && HSocket.BufferLength != 0)
                    {
                        if (OnReceivePacket != null)
                            OnReceivePacket.Invoke(this, HSocket);
                        HSocket.WinSock.BeginReceive(HSocket.Buffer, 0, (int)MaxPacketSize, SocketFlags.None, new AsyncCallback(ReceivePacket), HSocket);
                    }
                    else
                    {
                        if (Error != SocketError.Success)
                            if (OnClientError != null)
                                OnClientError.Invoke(this, HSocket, Error);
                        InvokeDisconnect(HSocket, "Client Received WINSOCK Error");
                    }
                }
                else
                {
                    InvokeDisconnect(HSocket, "Lost Connection");
                }
            }
            catch (Exception E)
            {
                InvokeDisconnect(HSocket, E.ToString());
            }
        }
        public void InvokeDisconnect(HybridSocket Socket, string Debug)
        {
            if (Socket == null)
                return;
            if (OnClientDisconnect != null)
                OnClientDisconnect.Invoke(this, Socket, Debug);
            Socket.Disconnected = true;
            Socket.Wrapper = null;
            Socket.Buffer = null;
            Socket.WinSock = null;
            Connections.Remove(Socket.UID);
            Socket = null;
        }
    }
}
