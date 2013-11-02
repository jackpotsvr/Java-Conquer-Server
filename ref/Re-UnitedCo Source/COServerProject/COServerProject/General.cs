using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Timers;
using System.Data;
using System.ComponentModel;
using MySql.Data.MySqlClient;

namespace COServer_Project
{

    public class General
    {
        static void Main()
        {           
            new General();
        }

        public static System.IO.StreamWriter sw = new System.IO.StreamWriter(Application.StartupPath + @"\ServerLog.txt", true);
        public static Hashtable Players = new Hashtable();
        public static System.Timers.Timer Thetimer;
        public static System.Timers.Timer MsgTimer;
        public static int MsgCount = 1;
        

        public uint Status;

        public static Thread MySqlComThread;
        public static ThreadStart TStart;

        private struct AuthBinder
        {
            public Client Client;
            public string Acc;
            public byte Auth;
        }

        public static ServerSocket AuthServer;
        public static ServerSocket GameServer;
        public static string ServerIP;
        public static bool ServerOnline;

        public static Hashtable AuthServerClients;
        public static Hashtable KeyClients;

        public static Packets MyPackets = new Packets();
        public static Random Rand = new Random();

        public static System.IO.TextWriter TW;

        public static void DatabaseSend()
        {
            while (true)
            {
                if (ExternalDatabase.MysqlConnected)
                {
                    if (ExternalDatabase.DatabaseQueue.Count > 0)
                    {
                        MySqlCommand Command = ExternalDatabase.DatabaseQueue.Dequeue();
                        if (Command != null)
                            try { Command.ExecuteNonQuery(); }
                            catch { }
                    }
                }
                Thread.Sleep(1);
            }
        }

        public static void ServerRestart()
        {
            Process nServ = new Process();
            nServ.StartInfo.FileName = Application.StartupPath + @"\CoMy.exe";
            nServ.Start();
            World.SaveAllChars();
            Environment.Exit(0);
        }
        public unsafe General()
        {
            try
            {
                Console.Title = "Re-UnitedCo Source";
                Ini Config = new Ini(System.Windows.Forms.Application.StartupPath + @"\Config.ini");
                ServerIP = Config.ReadValue("Server", "ServerIP");
                ExternalDatabase.DBUserName = Config.ReadValue("Server", "DBUserName");
                ExternalDatabase.DBUserPass = Config.ReadValue("Server", "DBUserPass");

                ExternalDatabase.StartDBConn();

                while (!ExternalDatabase.MysqlConnected) { }

                TStart = new ThreadStart(DatabaseSend);
                MySqlComThread = new Thread(TStart);
                MySqlComThread.Start();

                ExternalDatabase.ExpRate = uint.Parse(Config.ReadValue("Server", "XPRate"));
                ExternalDatabase.ProfExpRate = uint.Parse(Config.ReadValue("Server", "ProfXPRate"));                
                InternalDatabase.GetPortals();
                ExternalDatabase.LoadNPCs();
                ExternalDatabase.LoadMobs();
                ExternalDatabase.LoadItems();
                ExternalDatabase.LoadMobSpawns();
                ExternalDatabase.LoadRevPoints();
                Mobs.SpawnAllMobs();
                ExternalDatabase.GetPlusInfo();
                NPCs.SpawnAllNPCs();
                ExternalDatabase.DefineSkills();
                InternalDatabase.LoadGuilds();
                ExternalDatabase.AllOffline();

                AuthServerClients = new Hashtable();
                KeyClients = new Hashtable();

                AuthServer = new ServerSocket();
                AuthServer.Port = 9958;
                AuthServer.MaxPacketSize = 200;
                AuthServer.MaxThreads = 300;
                AuthServer.OnClientConnect += new SocketEvent(AuthConnHandler);
                AuthServer.OnReceivePacket += new SocketEvent(AuthPacketHandler);
                AuthServer.Enabled = true;

                GameServer = new ServerSocket();
                GameServer.Port = 5816;
                GameServer.MaxPacketSize = 4096;
                GameServer.MaxThreads = 300;
                GameServer.OnClientDisconnect += new SocketDisconnectEvent(GameDisconnectionHandler);
                GameServer.OnReceivePacket += new SocketEvent(GamePacketHandler);
                GameServer.Enabled = true;

                Console.WriteLine("Coded by irritantgassie");
                Console.WriteLine("Press enter to close to prevent rollbacks!");

                #region timers
                Thetimer = new System.Timers.Timer();
                Thetimer.Interval = 12000;
                Thetimer.Elapsed += new ElapsedEventHandler(Thetimer_Elapsed);
                Thetimer.Start();
                MsgTimer = new System.Timers.Timer();
                MsgTimer.Interval = 240000;
                MsgTimer.Elapsed += new ElapsedEventHandler(MsgTimer_Elapsed);
                MsgTimer.Start();
 
                #endregion

                Console.ReadLine();
                DoStuff();

            }
            catch (Exception Exc) { WriteLine(Exc.ToString()); }
        }
        #region Timers
        public static void Thetimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            World.SaveAllChars();
        }
        #endregion

        public static void MsgTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string text = "[GM]";
            if (MsgCount == 1)
                text = "Welcome to the server";
            else if (MsgCount == 2)
                text = "Welcome to the server";
            else if (MsgCount == 3)
                text = "Welcome to the server";
            else if (MsgCount == 4)
            {
                MsgCount = 0;
                text = "Have fun!";
            }
            World.SendMsgToAll(text, "", 2011);
            MsgCount++;

        }
        public static void BrCastMSG()
        {
            if (World.BroadcastSend == true && World.Broadcast == false)
            {
                Thread.Sleep(300000);
                World.BroadcastSend = false;
                World.Broadcast = true;
            }
        }
        public static void Blackname() //For sending pkers in jail
        {
            foreach (DictionaryEntry DE in World.AllChars)
            {
                Character Char = (Character)DE.Value;

                if (Char.PKPoints >= 100)
                {
                    World.SendMsgToAll(Char.Name + " have killed to much and been send to jail.", "SYSTEM", 2011);
                    Char.Teleport(6000, 55, 55);
                }
                else if (Char.PKPoints >= 30)
                {
                    Random C = new Random();
                    int Nr = C.Next(1, 3);
                    if (Nr == 1)
                    {
                        World.SendMsgToAll(Char.Name + " was captured, but got away from jail. Guard: We will get you!!", "SYSTEM", 2011);
                    }
                    if (Nr == 2)
                    {
                        World.SendMsgToAll(Char.Name + " have killed to much and been send to jail.", "SYSTEM", 2011);
                        Char.Teleport(6000, 55, 55);
                    }
                    if (Nr == 3)
                    {
                        World.SendMsgToAll(Char.Name + " was captured, but got away from jail. Guard: We will get you!!", "SYSTEM", 2011);
                    }
                }
            }
        }  
        public static void DoStuff()
        {
            try
            {
                foreach (DictionaryEntry DE in World.AllChars)
                {
                    Character Char = (Character)DE.Value;
                    ExternalDatabase.ChangeOnlineStatus(Char.MyClient.Account, 0);
                }
                World.PlayersOffLottery();
                World.SaveAllChars();
                sw.Flush();
                sw.Close();
                ExternalDatabase.AllOffline();
                ExternalDatabase.AllowQuerys = false;
                while (ExternalDatabase.DatabaseQueue.Count > 0) { Thread.Sleep(1); }
                Environment.Exit(Environment.ExitCode);
            }
            catch { }
        }

        public void GameDisconnectionHandler(object Sender, HybridSocket Socket, string Debug)
        {
            try
            {
                Client Cli = (Client)Socket.Wrapper;
                if (Cli != null && Cli.Online)
                    Cli.Drop();
            }
            catch (Exception Exc) { WriteLine(Exc.ToString()); }
        }
    
        public void AuthConnHandler(object Sender, HybridSocket Socket)
        {
            try
            {
                Socket Sock = Socket.WinSock;
                if (Sock == null)
                    return;
                IPEndPoint IPE = (IPEndPoint)Sock.RemoteEndPoint;

                Client NewClient = new Client();
                AuthBinder NewAB = new AuthBinder();

                NewAB.Client = NewClient;

                AuthServerClients.Add(Convert.ToString(IPE.Address) + ":" + IPE.Port, NewAB);
            }
            catch (Exception Exc) { WriteLine(Exc.ToString()); }
        }
        public void AuthPacketHandler(object Sender, HybridSocket Socket)
        {
            try
            {
                Socket Sock = Socket.WinSock;
                IPEndPoint IPE = (IPEndPoint)Sock.RemoteEndPoint;
                string Ip = Convert.ToString(IPE.Address);
                int Port = IPE.Port;

                byte[] Data = Socket.Packet;

                AuthBinder AB = (AuthBinder)AuthServerClients[Ip + ":" + Port];
                Client TheClient = AB.Client;

                if (Data.Length == 52)
                    TheClient.Crypto.Decrypt(ref Data);

                if (Data[0] == 0x34 && Data[1] == 0x00 && Data[2] == 0x1b && Data[3] == 0x04)
                {
                    try
                    {
                        int read = 0x04;
                        string ThisAcc = "";
                        string ThisPass = "";

                        while (read < 0x14 && Data[read] != 0x00)
                        {
                            ThisAcc += Convert.ToChar(Data[read]);
                            read += 1;
                        }

                        read = 0x14;

                        while (read < 36 && Data[read] != 0)
                        {
                            ThisPass += (Convert.ToString(Data[read], 16)).PadLeft(2, '0');
                            read += 1;
                        }

                        byte Auth = ExternalDatabase.Authenticate(ThisAcc, ThisPass);

                        if (Auth != 0)
                        {
                            General.WriteLine("Successful login for account " + ThisAcc);
                            AB.Auth = Auth;
                            AB.Acc = ThisAcc;
                            AB.Client.Account = ThisAcc;

                            ulong TheKeys = (uint)(Rand.Next(0x98968) << 32);
                            TheKeys = TheKeys << 32;
                            TheKeys = (uint)(TheKeys | (uint)Rand.Next(10000000));

                            KeyClients.Add(TheKeys, AB);

                            byte[] Key1 = new byte[4];
                            byte[] Key2 = new byte[4];

                            Key1[0] = (byte)((ulong)(TheKeys & 0xff00000000000000L) >> 56);
                            Key1[1] = (byte)((TheKeys & 0xff000000000000) >> 48);
                            Key1[2] = (byte)((TheKeys & 0xff0000000000) >> 40);
                            Key1[3] = (byte)((TheKeys & 0xff00000000) >> 32);
                            Key2[0] = (byte)((TheKeys & 0xff000000) >> 24);
                            Key2[1] = (byte)((TheKeys & 0xff0000) >> 16);
                            Key2[2] = (byte)((TheKeys & 0xff00) >> 8);
                            Key2[3] = (byte)(TheKeys & 0xff);

                            try
                            {
                                byte[] Pack = MyPackets.AuthResponse(ServerIP, Key1, Key2);
                                TheClient.Crypto.Encrypt(ref Pack);
                                Sock.Send(Pack);
                            }
                            catch { }

                            Socket.Disconnect();
                            AuthServerClients.Remove(Ip + ":" + Port);
                        }
                        else
                        {
                            Socket.Disconnect();
                            AuthServerClients.Remove(Ip + ":" + Port);
                        }
                    }
                    catch (Exception Exc) { WriteLine(Convert.ToString(Exc)); }
                }
            }
            catch (Exception Exc) { WriteLine(Exc.ToString()); }
        }

        public void GamePacketHandler(object Sender, HybridSocket Socket)
        {
            try
            {
                Socket Sock = Socket.WinSock;
                IPEndPoint IPE = (IPEndPoint)Sock.RemoteEndPoint;
                string Ip = Convert.ToString(IPE.Address);
                int Port = IPE.Port;

                byte[] Data = Socket.Packet;

                if (Socket.Wrapper != null)
                {
                    Client Cli = (Client)Socket.Wrapper;
                    Cli.GetPacket(Data);
                }                
                else
                {
                    Cryptographer TempCrypto = new Cryptographer();
                    TempCrypto.Decrypt(ref Data);

                    if (Data[0] == 0x1c && Data[1] == 0x00 && Data[2] == 0x1c && Data[3] == 0x04)
                    {
                        ulong Keys = Data[11];
                        Keys = (Keys << 8) | Data[10];
                        Keys = (Keys << 8) | Data[9];
                        Keys = (Keys << 8) | Data[8];
                        Keys = (Keys << 8) | Data[7];
                        Keys = (Keys << 8) | Data[6];
                        Keys = (Keys << 8) | Data[5];
                        Keys = (Keys << 8) | Data[4];

                        AuthBinder AB2 = (AuthBinder)KeyClients[Keys];
                        KeyClients.Remove(Keys);

                        Client ThisClient = AB2.Client;

                        Socket.Wrapper = ThisClient;

                        string Account = AB2.Acc;

                        ThisClient.Crypto = TempCrypto;
                        Console.WriteLine(); 
                        ThisClient.Crypto.SetKeys(new byte[4] { Data[11], Data[10], Data[9], Data[8] }, new byte[4] { Data[7], Data[6], Data[5], Data[4] });
                        ThisClient.Crypto.EnableAlternateKeys();

                        ThisClient.MessageId = Rand.Next(50000);
                        ThisClient.ListenSock = Socket;
                        ThisClient.GetIPE();

                        
                        if (AB2.Auth == 1)
                        {
                            ThisClient.MyChar = new Character();
                            ThisClient.MyChar.MyClient = ThisClient;
                            ThisClient.Account = Account;
                            InternalDatabase.GetCharInfo(ThisClient.MyChar, Account);

                            ThisClient.SendPacket(MyPackets.LanguageResponse((uint)ThisClient.MessageId));
                            ThisClient.SendPacket(MyPackets.CharacterInfo(ThisClient.MyChar));
                            ThisClient.SendPacket(MyPackets.AfterChar());

                            if (!World.AllChars.Contains(ThisClient.MyChar.UID))
                            {
                                World.AllChars.Add(ThisClient.MyChar.UID, ThisClient.MyChar);
                            }
                            else
                            {
                                ThisClient.MyChar = null;
                                ThisClient = null;
                                return;
                            }
                            ExternalDatabase.ChangeOnlineStatus(ThisClient.Account, 1);
                        }
                        else if (AB2.Auth == 2)
                        {
                            ThisClient.SendPacket(MyPackets.NewCharPacket(ThisClient.MessageId));
                        }
                        else
                        {
                            ThisClient.Drop();
                        }
                    }
                }
            }
            catch (Exception Exc) { WriteLine(Exc.ToString()); }
        }

        public static void WriteLine(string text)
        {
            try
            {
                Console.WriteLine(text);
                sw.WriteLine(text);
            }
            catch { }
        }
        
    }

    public class Ini
    {
        public IniFile TheIni;

        public Ini(string path)
        {
            TheIni = new IniFile(path);
        }

        public string ReadValue(string Section, string Key)
        {
            string it = TheIni.IniReadValue(Section, Key);
            return it;
        }
        public void WriteString(string Section, string Key, string Value)
        {
            TheIni.IniWriteValue(Section, Key, Value);
        }
    }

    public class IniFile
    {
        public string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
            string key, string def, StringBuilder retVal, int size, string filePath);

        public IniFile(string INIPath)
        {
            path = INIPath;
        }
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            return temp.ToString();

        }
    }
}