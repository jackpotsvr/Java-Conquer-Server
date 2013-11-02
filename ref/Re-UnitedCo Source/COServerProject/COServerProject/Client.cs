using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using System.Collections;

namespace COServer_Project
{
    public class Client
    {
        public Cryptographer Crypto;
        public HybridSocket ListenSock;
        public int MessageId;
        public string Account = "";
        public byte Authentication;
        public Timer ClientTimer;
        public Character MyChar;
        public bool Online = true;
        public uint Status;
        public int CurrentNPC = 0;
        public bool There = false;
        IPEndPoint IPE;
        bool UppAgree = false;
        Timer ShutdownTimer = new Timer();
        Timer ShutDownTimerMsg4min = new Timer();
        Timer ShutDownTimerMsg3min = new Timer();
        Timer ShutDownTimerMsg2min = new Timer();
        Timer ShutDownTimerMsg1min = new Timer();
        Timer ShutDownTimerMsg30sec = new Timer();
        Timer UpdateRestart = new Timer();
        Timer UpdateRestart30sec = new Timer();
        Timer GuildWarStart = new Timer();
        Timer GuildWarStop = new Timer();
        Timer GuildWarStop1min = new Timer();
        Timer GuildWarStop2min = new Timer();
        Timer GuildWarStop3min = new Timer();
        Timer GuildWarStop4min = new Timer();
        Timer GuildWarStop5min = new Timer();
        Timer GuildWarStop30sec = new Timer();

        public int RGemId = 0;
        public int RItemype = 0;
        public int ChColor = 0;
        public byte GemId = 0;
        public string RBGem = "";

        HybridSocket MySocket
        {
            get { return ListenSock; }
            set { ListenSock = value; }
        }

        public Client()
        {
            Crypto = new Cryptographer();
        }

        public void GetIPE()
        {
            IPE = (IPEndPoint)ListenSock.WinSock.RemoteEndPoint;
        }

        public unsafe void GetPacket(byte[] data)
        {
            byte[] Data = data;

            Crypto.Decrypt(ref Data);

            ushort PacketId = (ushort)((Data[3] << 8) | Data[2]);
            int PacketType;

            if (PacketId == 53101 || PacketId == 53110)
                Drop();

            #region guilds
            switch (PacketId)
            {
                case 1024:
                    {
                        if (MyChar.StatP > 0)
                        {
                            MyChar.StatP--;
                            MyChar.Str += Data[4];
                            MyChar.Agi += Data[5];
                            MyChar.Vit += Data[6];
                            MyChar.Spi += Data[7];
                        }
                        break;
                    }
                case 2050:
                    {
                        if (Data[4] == 3 && MyChar.CPs >= 5 && World.Broadcast == true)
                        {
                            MyChar.CPs -= 5;
                            SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                            byte Len = Data[13];
                            string Message = "";
                            for (int i = 0; i < Len; i++)
                            {
                                Message += Convert.ToChar(Data[14 + i]);
                            }
                            World.SendMsgToAll(Message, MyChar.Name, 2500);
                            General.BrCastMSG();
                            World.BroadcastSend = false;
                        }
                        else
                        {
                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Please wait untill, the old broadcast time is done.", 2000));
                        }
                        break;
                    }
                case 1112://See guild member's status
                    {
                        string Name = "";
                        int Pos = 9;
                        while (Pos < Data.Length)
                        {
                            Name += Convert.ToChar(Data[Pos]);
                            Pos++;
                        }
                        break;
                    }
                case 1015://Send guild members
                    {
                        if (MyChar.MyGuild == null)
                            return;
                        string Total = "";
                        string OnlineMembers = "";
                        string OfflineMembers = "";
                        byte Count = 1;

                        string[] Splitter = MyChar.MyGuild.Creator.Split(':');
                        if (World.AllChars.Contains(uint.Parse(Splitter[1])))
                            OnlineMembers += Convert.ToChar((Splitter[0] + Convert.ToChar(32) + Splitter[2] + Convert.ToChar(32) + "1").Length) + (Splitter[0] + Convert.ToChar(32) + Splitter[2] + Convert.ToChar(32) + "1");
                        else
                            OfflineMembers += Convert.ToChar((Splitter[0] + Convert.ToChar(32) + Splitter[2] + Convert.ToChar(32) + "0").Length) + (Splitter[0] + Convert.ToChar(32) + Splitter[2] + Convert.ToChar(32) + "0");


                        foreach (DictionaryEntry DE in MyChar.MyGuild.DLs)
                        {
                            string DL = (string)DE.Value;
                            Count++;
                            Splitter = DL.Split(':');
                            if (World.AllChars.Contains(uint.Parse(Splitter[1])))
                                OnlineMembers += Convert.ToChar((Splitter[0] + Convert.ToChar(32) + Splitter[2] + Convert.ToChar(32) + "1").Length) + (Splitter[0] + Convert.ToChar(32) + Splitter[2] + Convert.ToChar(32) + "1");
                            else
                                OfflineMembers += Convert.ToChar((Splitter[0] + Convert.ToChar(32) + Splitter[2] + Convert.ToChar(32) + "0").Length) + (Splitter[0] + Convert.ToChar(32) + Splitter[2] + Convert.ToChar(32) + "0");
                        }

                        foreach (DictionaryEntry DE in MyChar.MyGuild.Members)
                        {
                            string NM = (string)DE.Value;
                            Count++;
                            Splitter = NM.Split(':');
                            if (World.AllChars.Contains(uint.Parse(Splitter[1])))
                                OnlineMembers += Convert.ToChar((Splitter[0] + Convert.ToChar(32) + Splitter[2] + Convert.ToChar(32) + "1").Length) + (Splitter[0] + Convert.ToChar(32) + Splitter[2] + Convert.ToChar(32) + "1");
                            else
                                OfflineMembers += Convert.ToChar((Splitter[0] + Convert.ToChar(32) + Splitter[2] + Convert.ToChar(32) + "0").Length) + (Splitter[0] + Convert.ToChar(32) + Splitter[2] + Convert.ToChar(32) + "0");
                        }

                        Total = OnlineMembers + OfflineMembers;

                        SendPacket(Data);
                        SendPacket(General.MyPackets.StringGuild(11, 11, Total, Count));

                        break;
                    }
                case 1107:
                    {
                        byte Type = Data[4];
                        switch (Type)
                        {
                            case 1://Join guild request
                                {
                                    if (MyChar.MyGuild == null)
                                    {
                                        uint UID = BitConverter.ToUInt32(Data, 8);

                                        Character JoinWho = (Character)World.AllChars[UID];
                                        if (JoinWho.MyGuild != null && JoinWho.GuildPosition == 100 || JoinWho.GuildPosition == 90)
                                            JoinWho.MyClient.SendPacket(General.MyPackets.SendGuild(MyChar.UID, 1));
                                    }

                                    break;
                                }
                            case 2://Accept join request
                                {
                                    uint UID = BitConverter.ToUInt32(Data, 8);
                                    if (World.AllChars.Contains(UID))
                                    {
                                        Character WhoJoins = (Character)World.AllChars[UID];
                                        if (WhoJoins.MyGuild == null)
                                        {
                                            WhoJoins.GuildID = MyChar.MyGuild.GuildID;
                                            WhoJoins.MyGuild = MyChar.MyGuild;
                                            WhoJoins.GuildPosition = 50;
                                            MyChar.MyGuild.PlayerJoins(WhoJoins);
                                            World.UpdateSpawn(WhoJoins);

                                            WhoJoins.MyClient.SendPacket(General.MyPackets.GuildName(WhoJoins.GuildID, WhoJoins.MyGuild.GuildName));
                                            WhoJoins.MyClient.SendPacket(General.MyPackets.GuildInfo(WhoJoins.MyGuild, WhoJoins));
                                        }
                                    }
                                    break;
                                }
                            case 3://Leave the guild
                                {
                                    if (MyChar.MyGuild != null && MyChar.GuildPosition != 100)
                                    {
                                        SendPacket(General.MyPackets.SendGuild(MyChar.MyGuild.GuildID, 19));
                                        MyChar.MyGuild.PlayerQuits(MyChar);
                                        MyChar.GuildDonation = 0;
                                        MyChar.GuildID = 0;
                                        MyChar.GuildPosition = 0;
                                        MyChar.MyGuild = null;
                                        World.UpdateSpawn(MyChar);
                                        World.SpawnOthersToMe(MyChar, false);
                                    }
                                    break;
                                }
                            case 12://Guild status
                                {
                                    if (MyChar.MyGuild != null)
                                    {
                                        SendPacket(General.MyPackets.GuildName(MyChar.GuildID, MyChar.MyGuild.GuildName));
                                        SendPacket(General.MyPackets.GuildInfo(MyChar.MyGuild, MyChar));
                                        SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, MyChar.MyGuild.Bulletin, 2111));
                                        SendPacket(General.MyPackets.GeneralData(MyChar.UID, 0, 0, 0, 97));
                                    }
                                    break;
                                }
                            case 11://Donate
                                {
                                    if (MyChar.MyGuild != null)
                                    {
                                        uint Amount = BitConverter.ToUInt32(Data, 8);

                                        if (MyChar.Silvers >= Amount)
                                        {
                                            MyChar.MyGuild.Fund += Amount;
                                            MyChar.Silvers -= Amount;
                                            MyChar.GuildDonation += Amount;
                                            MyChar.MyGuild.Refresh(MyChar);
                                            SendPacket(General.MyPackets.GuildInfo(MyChar.MyGuild, MyChar));
                                            SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                            World.SendMsgToAll(MyChar.Name + " has donated " + Amount.ToString() + " silvers to " + MyChar.MyGuild.GuildName + ".", "SYSTEM", 2005);
                                        }
                                    }

                                    break;
                                }
                        }
                        break;
                    }
            #endregion
                #region friends
                case 1019:
                    {
                        uint UID = BitConverter.ToUInt32(Data, 4);
                        byte Type = Data[8];
                        switch (Type)
                        {
                            case 14:
                                {
                                    World.SendMsgToAll(MyChar.Name + " has broken friendship with " + (string)MyChar.Friends[UID], "SYSTEM", 2005);
                                    MyChar.Friends.Remove(UID);
                                    SendPacket(General.MyPackets.FriendEnemyPacket(UID, "", 14, 0));

                                    if (World.AllChars.Contains(UID))
                                    {
                                        Character Char = (Character)World.AllChars[UID];
                                        if (Char != null && Char.MyClient != null)
                                        {
                                            Char.MyClient.SendPacket(General.MyPackets.FriendEnemyPacket(MyChar.UID, "", 14, 0));
                                            Char.Friends.Remove(MyChar.UID);
                                        }
                                        else
                                            ExternalDatabase.RemoveFromFriend(MyChar.UID, UID);
                                    }
                                    else
                                        ExternalDatabase.RemoveFromFriend(MyChar.UID, UID);

                                    break;
                                }
                            case 10:
                                {
                                    Character Other = (Character)World.AllChars[UID];
                                    if (Other == null)
                                        return;
                                    if (Other.RequestFriendWith == MyChar.UID)
                                    {
                                        MyChar.AddFriend(Other);
                                        Other.AddFriend(MyChar);
                                        World.SendMsgToAll(MyChar.Name + " and " + Other.Name + " are friends from now on.", "SYSTEM", 2005);
                                    }
                                    else
                                    {
                                        MyChar.RequestFriendWith = UID;
                                        Other.MyClient.SendPacket(General.MyPackets.SendMsg(Other.MyClient.MessageId, "SYSTEM", Other.Name, MyChar.Name + " requested to add friends.", 2005));
                                    }
                                    break;
                                }
                        }

                        break;
                    }
                #endregion
                #region trades
                case 1056:
                    {
                        uint UID = BitConverter.ToUInt32(Data, 4);

                        byte Type = Data[8];

                        switch (Type)
                        {
                            case 1://Request trade
                                {
                                    Character Who = (Character)World.AllChars[UID];
                                    if (UID != MyChar.TradingWith)
                                    {
                                        Who.MyClient.SendPacket(General.MyPackets.TradePacket(MyChar.UID, 1));
                                        MyChar.TradingWith = UID;
                                        Who.TradingWith = MyChar.UID;
                                        SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "[Trade]Request for trading has been sent out.", 2005));
                                    }
                                    else
                                    {
                                        Who.Trading = true;
                                        MyChar.Trading = true;
                                        Who.MyClient.SendPacket(General.MyPackets.TradePacket(MyChar.UID, 3));
                                        SendPacket(General.MyPackets.TradePacket(Who.UID, 3));
                                    }
                                    break;
                                }
                            case 2://Close trade
                                {
                                    if (MyChar.Trading)
                                    {
                                        Character Who = (Character)World.AllChars[MyChar.TradingWith];
                                        if (Who != null)
                                        {
                                            Who.MyClient.SendPacket(General.MyPackets.TradePacket(MyChar.TradingWith, 5));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Trading failed!", 2005));
                                            Who.MyClient.SendPacket(General.MyPackets.SendMsg(Who.MyClient.MessageId, "SYSTEM", Who.Name, "Trading failed!", 2005));
                                            Who.Trading = false;
                                            MyChar.Trading = false;
                                            Who.TradingWith = 0;
                                            MyChar.TradingWith = 0;
                                            foreach (uint iuid in MyChar.MyTradeSide)
                                            {
                                                string Item = MyChar.FindItem(iuid);
                                                string[] Splitter = Item.Split('-');
                                                SendPacket(General.MyPackets.AddItem(iuid, int.Parse(Splitter[0]), byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), 0, 100, 100));
                                            }
                                            foreach (uint iuid in Who.MyTradeSide)
                                            {
                                                string Item = Who.FindItem(iuid);
                                                string[] Splitter = Item.Split('-');
                                                Who.MyClient.SendPacket(General.MyPackets.AddItem(iuid, int.Parse(Splitter[0]), byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), 0, 100, 100));
                                            }
                                            MyChar.MyTradeSide = new ArrayList(20);
                                            Who.MyTradeSide = new ArrayList(20);
                                            MyChar.TradingCPs = 0;
                                            MyChar.TradingSilvers = 0;
                                            MyChar.TradeOK = false;
                                            Who.TradeOK = false;
                                            Who.MyTradeSideCount = 0;
                                            MyChar.MyTradeSideCount = 0;
                                        }
                                    }

                                    break;
                                }
                            case 6://Add an item
                                {
                                    Character Who = (Character)World.AllChars[MyChar.TradingWith];
                                    if (Who.ItemsInInventory + MyChar.MyTradeSideCount < 40)
                                    {
                                        string Item = MyChar.FindItem(UID);
                                        Who.MyClient.SendPacket(General.MyPackets.TradeItem(UID, Item));
                                        MyChar.MyTradeSide.Add(UID);
                                        MyChar.MyTradeSideCount++;
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.TradePacket(UID, 11));
                                        SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "[Trade]Your trade partner can't hold any more items.", 2005));
                                    }
                                    break;
                                }
                            case 7://Specify money
                                {
                                    MyChar.TradingSilvers = UID;
                                    Character Who = (Character)World.AllChars[MyChar.TradingWith];
                                    Who.MyClient.SendPacket(General.MyPackets.TradePacket(UID, 8));

                                    break;
                                }
                            case 10://OK
                                {
                                    Character Who = (Character)World.AllChars[MyChar.TradingWith];

                                    if (Who.TradeOK)
                                    {
                                        Who.MyClient.SendPacket(General.MyPackets.TradePacket(MyChar.TradingWith, 5));
                                        SendPacket(General.MyPackets.TradePacket(MyChar.UID, 5));
                                        foreach (uint itemuid in Who.MyTradeSide)
                                        {
                                            string item = Who.FindItem(itemuid);
                                            MyChar.AddItem(item, 0, itemuid);
                                            Who.RemoveItem(itemuid);
                                        }
                                        foreach (uint itemuid in MyChar.MyTradeSide)
                                        {
                                            string item = MyChar.FindItem(itemuid);
                                            Who.AddItem(item, 0, itemuid);
                                            MyChar.RemoveItem(itemuid);
                                        }

                                        MyChar.Silvers += Who.TradingSilvers;
                                        MyChar.CPs += Who.TradingCPs;

                                        MyChar.Silvers -= MyChar.TradingSilvers;
                                        MyChar.CPs -= MyChar.TradingCPs;

                                        Who.Silvers += MyChar.TradingSilvers;
                                        Who.CPs += MyChar.TradingCPs;

                                        Who.Silvers -= Who.TradingSilvers;
                                        Who.CPs -= Who.TradingCPs;

                                        SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Trading succeeded.", 2005));
                                        Who.MyClient.SendPacket(General.MyPackets.SendMsg(Who.MyClient.MessageId, "SYSTEM", Who.Name, "Trading succeeded.", 2005));

                                        MyChar.Trading = false;
                                        MyChar.TradingWith = 0;
                                        MyChar.MyTradeSideCount = 0;
                                        MyChar.TradeOK = false;
                                        MyChar.MyTradeSide = new ArrayList(20);
                                        MyChar.TradingCPs = 0;
                                        MyChar.TradingSilvers = 0;

                                        Who.MyTradeSide = new ArrayList(20);
                                        Who.TradingCPs = 0;
                                        Who.TradingSilvers = 0;
                                        Who.TradeOK = false;
                                        Who.TradingWith = 0;
                                        Who.Trading = false;
                                        Who.MyTradeSideCount = 0;


                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                        Who.MyClient.SendPacket(General.MyPackets.Vital(Who.UID, 4, Who.Silvers));
                                        Who.MyClient.SendPacket(General.MyPackets.Vital(Who.UID, 30, Who.CPs));
                                    }
                                    else
                                    {
                                        MyChar.TradeOK = true;
                                        Who.MyClient.SendPacket(General.MyPackets.TradePacket(0, 10));
                                    }
                                    break;
                                }
                            case 13://Specify CPs
                                {
                                    MyChar.TradingCPs = UID;
                                    Character Who = (Character)World.AllChars[MyChar.TradingWith];
                                    Who.MyClient.SendPacket(General.MyPackets.TradePacket(UID, 12));
                                    break;
                                }
                        }

                        break;
                    }
                #endregion
                #region teams
                case 1023:
                    {
                        byte Type = Data[4];
                        switch (Type)
                        {
                            case 0://Create
                                {
                                    MyChar.TeamLeader = true;
                                    MyChar.MyTeamLeader = MyChar;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 26, MyChar.GetStat()));
                                    SendPacket(General.MyPackets.TeamPacket(MyChar.UID, 0));
                                    World.UpdateSpawn(MyChar);
                                    break;
                                }
                            case 1://Join request
                                {
                                    uint JoinWho = (uint)(Data[8] + (Data[9] << 8) + (Data[10] << 16) + (Data[11] << 24));
                                    Character Who = (Character)World.AllChars[JoinWho];
                                    if (Who.TeamLeader)
                                    {
                                        if (!Who.JoinForbidden)
                                        {
                                            if (Who.PlayersInTeam < 4)
                                            {
                                                Who.MyClient.SendPacket(General.MyPackets.TeamPacket(MyChar.UID, 1));
                                                SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "[Team]Request to join team has been sent out.", 2005));
                                            }
                                            else
                                                SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "[Team]The team is full.", 2005));
                                        }
                                        else
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "The team doesn't accept new members.", 2005));
                                    }
                                    else
                                        SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "[Team]The target has not created a team.", 2005));

                                    break;
                                }
                            case 2://Exit team
                                {
                                    MyChar.MyTeamLeader.TeamRemove(MyChar, false);
                                    break;
                                }
                            case 3://I accept invitation
                                {
                                    uint WhoInvited = (uint)(Data[8] + (Data[9] << 8) + (Data[10] << 16) + (Data[11] << 24));
                                    Character Who = (Character)World.AllChars[WhoInvited];
                                    if (Who != null)
                                        Who.TeamAdd(MyChar);

                                    break;
                                }
                            case 4://Invite request
                                {
                                    uint InviteWho = (uint)(Data[8] + (Data[9] << 8) + (Data[10] << 16) + (Data[11] << 24));
                                    Character Invited = (Character)World.AllChars[InviteWho];
                                    if (!Invited.TeamLeader && Invited.MyTeamLeader == null && MyChar.TeamLeader && MyChar.PlayersInTeam < 4)
                                    {
                                        Invited.MyClient.SendPacket(General.MyPackets.TeamPacket(MyChar.UID, 6));
                                        Invited.MyClient.SendPacket(General.MyPackets.TeamPacket(MyChar.UID, 4));
                                    }

                                    break;
                                }
                            case 5://I accept your join request
                                {
                                    uint WhoJoins = (uint)(Data[8] + (Data[9] << 8) + (Data[10] << 16) + (Data[11] << 24));
                                    Character Joiner = (Character)World.AllChars[WhoJoins];

                                    MyChar.TeamAdd(Joiner);

                                    break;
                                }
                            case 6://Dismiss
                                {
                                    MyChar.TeamDismiss();
                                    break;
                                }
                            case 7://Kick
                                {
                                    uint KickWho = (uint)(Data[8] + (Data[9] << 8) + (Data[10] << 16) + (Data[11] << 24));
                                    Character Kicked = (Character)World.AllChars[KickWho];

                                    MyChar.TeamRemove(Kicked, true);
                                    break;
                                }
                            case 8://Forbid joining
                                {
                                    MyChar.JoinForbidden = true;
                                    break;
                                }
                            case 9://UnForbid joining
                                {
                                    MyChar.JoinForbidden = false;
                                    break;
                                }
                        }
                        break;
                    }
                #endregion
                #region warehouse
                case 1102:
                    {
                        uint NPCID = (uint)((Data[7] << 24) + (Data[6] << 16) + (Data[5] << 8) + (Data[4]));
                        uint ItemUID = (uint)((Data[15] << 24) + (Data[14] << 16) + (Data[13] << 8) + (Data[12]));
                        byte Type = Data[8];
                        byte WHID = 0;

                        if (NPCID == 8)
                            WHID = 0;
                        else if (NPCID == 81)
                            WHID = 1;
                        else if (NPCID == 82)
                            WHID = 2;
                        else if (NPCID == 83)
                            WHID = 3;
                        else if (NPCID == 84)
                            WHID = 4;
                        else if (NPCID == 85)
                            WHID = 5;


                        if (Type == 0)
                        {
                            SendPacket(General.MyPackets.WhItems(MyChar, WHID, (ushort)NPCID));
                        }
                        else if (Type == 1)//Throw an item into warehouse
                        {
                            if (WHID == 0 && MyChar.TCWHCount < 20 || WHID == 1 && MyChar.PCWHCount < 20 || WHID == 2 && MyChar.ACWHCount < 20 || WHID == 3 && MyChar.DCWHCount < 20 || WHID == 4 && MyChar.BIWHCount < 20 || WHID == 5 && MyChar.MAWHCount < 40)
                            {
                                string Item = MyChar.FindItem(ItemUID);
                                MyChar.RemoveItem(ItemUID);
                                MyChar.AddWHItem(Item, ItemUID, WHID);
                                SendPacket(General.MyPackets.WhItems(MyChar, WHID, (ushort)NPCID));
                            }
                        }
                        else if (Type == 2)//Take an item from warehouse
                        {
                            if (MyChar.ItemsInInventory < 40)
                            {
                                string Item = MyChar.FindWHItem(ItemUID, WHID);
                                //"1088000-0-0-0-0-0"
                                MyChar.RemoveWHItem(ItemUID);
                                SendPacket(General.MyPackets.WhItems(MyChar, WHID, (ushort)NPCID));
                                MyChar.AddItem(Item, 0, ItemUID);
                            }
                        }

                        break;
                    }
                #endregion
                #region addingpluses
                case 2036:
                    {
                        uint MainUID = (uint)((Data[11] << 24) + (Data[10] << 16) + (Data[9] << 8) + Data[8]);
                        uint Minor1UID = (uint)((Data[15] << 24) + (Data[14] << 16) + (Data[13] << 8) + Data[12]);
                        uint Minor2UID = (uint)((Data[19] << 24) + (Data[18] << 16) + (Data[17] << 8) + Data[16]);
                        uint Gem1 = (uint)((Data[23] << 24) + (Data[22] << 16) + (Data[21] << 8) + Data[20]);
                        uint Gem2 = (uint)((Data[24] << 24) + (Data[23] << 16) + (Data[22] << 8) + Data[21]);

                        string MainItem = MyChar.FindItem(MainUID);
                        string Minor1Item = MyChar.FindItem(Minor1UID);
                        string Minor2Item = MyChar.FindItem(Minor2UID);

                        if (MainItem == null || Minor1Item == null || Minor2Item == null)
                            return;

                        byte MainPlus = 0;
                        byte Minor1Plus = 0;
                        byte Minor2Plus = 0;
                        uint MainItemId = 0;
                        uint Minor1ItemId = 0;
                        uint Minor2ItemId = 0;

                        string[] Splitter;
                        string[] MainItemE;
                        MainItemE = MainItem.Split('-');
                        MainPlus = byte.Parse(MainItemE[1]);
                        MainItemId = uint.Parse(MainItemE[0]);
                        Splitter = Minor1Item.Split('-');
                        Minor1Plus = byte.Parse(Splitter[1]);
                        Minor1ItemId = uint.Parse(Splitter[0]);
                        Splitter = Minor2Item.Split('-');
                        Minor2Plus = byte.Parse(Splitter[1]);
                        Minor2ItemId = uint.Parse(Splitter[0]);

                        if (Minor1Plus == Minor2Plus)
                            if (Minor1Plus == MainPlus || MainPlus == 0 && Minor1Plus == 1)
                                if (Other.ItemType2(Minor1ItemId) == Other.ItemType2(Minor2ItemId) && Other.ItemType2(Minor2ItemId) == Other.ItemType2(MainItemId) || (Other.ItemType(MainItemId) == 4 && Other.ItemType(Minor1ItemId) == 4 && Other.ItemType(Minor2ItemId) == 4) || Other.ItemType(MainItemId) == 5 && Other.ItemType(Minor1ItemId) == 5 && Other.ItemType(Minor2ItemId) == 5)
                                {
                                    MainPlus++;
                                    MyChar.RemoveItem(Minor1UID);
                                    MyChar.RemoveItem(Minor2UID);
                                    MyChar.RemoveItem(MainUID);
                                    MyChar.AddItem(MainItemE[0] + "-" + MainPlus + "-" + MainItemE[2] + "-" + MainItemE[3] + "-" + MainItemE[4] + "-" + MainItemE[5], 0, MainUID);
                                    return;
                                }

                        if (Minor1Plus == Minor2Plus)
                            if (Minor1Plus == MainPlus || MainPlus == 0 && Minor1Plus == 1)
                                if (Other.ItemType2(Minor1ItemId) == 73 || Other.ItemType2(Minor2ItemId) == 73)
                                    if ((Other.ItemType2(Minor1ItemId) == 73 && Other.ItemType2(Minor2ItemId) == 73) || (Other.ItemType2(Minor1ItemId) == 73 && Other.ItemType2(Minor2ItemId) != 73 && Other.ItemType2(MainItemId) == Other.ItemType2(Minor2ItemId)) || (Other.ItemType2(Minor2ItemId) == 73 && Other.ItemType2(Minor1ItemId) != 73 && Other.ItemType2(MainItemId) == Other.ItemType2(Minor1ItemId)))
                                    {
                                        MainPlus++;
                                        MyChar.RemoveItem(Minor1UID);
                                        MyChar.RemoveItem(Minor2UID);
                                        MyChar.RemoveItem(MainUID);
                                        MyChar.AddItem(MainItemE[0] + "-" + MainPlus + "-" + MainItemE[2] + "-" + MainItemE[3] + "-" + MainItemE[4] + "-" + MainItemE[5], 0, MainUID);
                                    }

                        break;
                    }
                #endregion
                #region money
                case 1101:
                    {
                        MyChar.Ready = false;
                        if (MyChar.ItemsInInventory > 39)
                            return;
                        uint ItemUID = (uint)((Data[7] << 24) + (Data[6] << 16) + (Data[5] << 8) + Data[4]);
                        DroppedItem TehItem = null;
                        if (DroppedItems.AllDroppedItems.Contains(ItemUID))
                            TehItem = (DroppedItem)DroppedItems.AllDroppedItems[ItemUID];

                        if (TehItem != null)
                        {
                            if (TehItem.Money == 0)
                                MyChar.AddItem(TehItem.Item, 0, ItemUID);
                            else
                            {
                                MyChar.Silvers += TehItem.Money;
                                SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "You have picked up " + TehItem.Money + " silvers.", 2005));
                            }

                            DroppedItems.AllDroppedItems.Remove(ItemUID);
                            World.ItemDissappears(TehItem);
                        }
                        MyChar.Ready = true;
                        break;
                    }
                #endregion
                #region attack
                case 1022:
                    {
                        MyChar.Ready = false;
                        int AttackType = (Data[23] << 24) + (Data[22] << 16) + (Data[21] << 8) + (Data[20]);
                        if (DateTime.Now < MyChar.LastTargetting.AddMilliseconds(255))
                            return;

                        MyChar.LastTargetting = DateTime.Now;

                        if (AttackType == 21)
                        {
                            MyChar.PTarget = null;
                            MyChar.TGTarget = null;
                            MyChar.MobTarget = null;
                            MyChar.Attacking = false;
                            ushort SkillId = Convert.ToUInt16(((long)Data[24] & 0xFF) | (((long)Data[25] & 0xFF) << 8));
                            SkillId ^= (ushort)0x915d;
                            SkillId ^= (ushort)MyChar.UID;
                            SkillId = (ushort)(SkillId << 0x3 | SkillId >> 0xd);
                            SkillId -= 0xeb42;

                            long x = (Data[16] & 0xFF) | ((Data[17] & 0xFF) << 8);
                            long y = (Data[18] & 0xFF) | ((Data[19] & 0xFF) << 8);

                            x = x ^ (uint)(MyChar.UID & 0xffff) ^ 0x2ed6;
                            x = ((x << 1) | ((x & 0x8000) >> 15)) & 0xffff;
                            x |= 0xffff0000;
                            x -= 0xffff22ee;

                            y = y ^ (uint)(MyChar.UID & 0xffff) ^ 0xb99b;
                            y = ((y << 5) | ((y & 0xF800) >> 11)) & 0xffff;
                            y |= 0xffff0000;
                            y -= 0xffff8922;

                            uint Target = ((uint)Data[12] & 0xFF) | (((uint)Data[13] & 0xFF) << 8) | (((uint)Data[14] & 0xFF) << 16) | (((uint)Data[15] & 0xFF) << 24);
                            Target = ((((Target & 0xffffe000) >> 13) | ((Target & 0x1fff) << 19)) ^ 0x5F2D2463 ^ MyChar.UID) - 0x746F4AE6;

                            if (SkillId != 1110 && SkillId != 1015 && SkillId != 1020 && SkillId != 1025)
                            {
                                if (MyChar.LocMap == 1039 || SkillId == 1002 || SkillId == 1000 && SkillId != 1110 && SkillId != 1015 && SkillId != 1020 && SkillId != 1025)
                                {
                                    MyChar.SkillLooping = SkillId;
                                    MyChar.SkillLoopingX = (ushort)x;
                                    MyChar.SkillLoopingY = (ushort)y;
                                    MyChar.SkillLoopingTarget = Target;
                                    MyChar.AtkType = 21;
                                    MyChar.Attack();
                                    MyChar.Attacking = true;
                                }
                                else
                                    MyChar.UseSkill(SkillId, (ushort)x, (ushort)y, Target);
                            }
                            else
                                MyChar.UseSkill(SkillId, (ushort)x, (ushort)y, Target);
                        }
                        if (AttackType == 8)//Marriage Proposal
                        {
                            fixed (byte* Ptr = Data)
                            {
                                uint TargetUID = *(uint*)(Ptr + 12);
                                Character Target = (Character)World.AllChars[TargetUID];
                                *(uint*)(Ptr + 8) = MyChar.UID;
                                {
                                    if (MyChar.Model == 2001 || MyChar.Model == 2002)
                                    {
                                        if (Target.Model == 1003 || Target.Model == 1004)
                                        {
                                            if (Target.Spouse == "" || Target.Spouse == "None")
                                            {
                                                Target.MyClient.SendPacket(Data);
                                            }
                                            else
                                            {
                                                SendPacket(General.MyPackets.NPCSay(Target.Name + " has already got an Spouse."));
                                                SendPacket(General.MyPackets.NPCLink("Ok.", 255));
                                                SendPacket(General.MyPackets.NPCSetFace(30));
                                                SendPacket(General.MyPackets.NPCFinish());
                                            }
                                        }
                                        else
                                        {
                                            SendPacket(General.MyPackets.NPCSay("No Marriage For Same Gender."));
                                            SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                            SendPacket(General.MyPackets.NPCSetFace(30));
                                            SendPacket(General.MyPackets.NPCFinish());
                                        }
                                    }
                                    if (MyChar.Model == 1003 || MyChar.Model == 1004)
                                    {
                                        if (Target.Model == 2001 || Target.Model == 2002)
                                        {
                                            if (Target.Spouse == "" || Target.Spouse == "None")
                                            {
                                                Target.MyClient.SendPacket(Data);
                                            }
                                            else
                                            {
                                                SendPacket(General.MyPackets.NPCSay(Target.Name + " has already got an Spouse."));
                                                SendPacket(General.MyPackets.NPCLink("Ok.", 255));
                                                SendPacket(General.MyPackets.NPCSetFace(30));
                                                SendPacket(General.MyPackets.NPCFinish());
                                            }
                                        }
                                        else
                                        {
                                            SendPacket(General.MyPackets.NPCSay("No Marriage For Same Gender."));
                                            SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                            SendPacket(General.MyPackets.NPCSetFace(30));
                                            SendPacket(General.MyPackets.NPCFinish());
                                        }
                                    }
                                }
                            }
                        }
                        if (AttackType == 9)//Marriage Accept
                        {
                            fixed (byte* Ptr = Data)
                            {
                                uint UID = *(uint*)(Ptr + 12);
                                Character Char = (Character)World.AllChars[UID];
                                {
                                    if (Char.Model == 2001 || Char.Model == 2002)
                                    {
                                        if (MyChar.Model == 1003 || MyChar.Model == 1004)
                                        {
                                            Char.Spouse = MyChar.Name;
                                            MyChar.Spouse = Char.Name;
                                            MyChar.SaveSpouse();
                                            Char.SaveSpouse();
                                            SendPacket(General.MyPackets.CharacterInfo(MyChar));
                                            World.SendMsgToAll(Char.Name + " and " + MyChar.Name + " has been united as husband and wife!", "LoveStone", 2011);
                                        }
                                        else
                                        {
                                            SendPacket(General.MyPackets.NPCSay("No Marriage For Same Gender."));
                                            SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                            SendPacket(General.MyPackets.NPCSetFace(30));
                                            SendPacket(General.MyPackets.NPCFinish());
                                        }
                                    }
                                    if (Char.Model == 1003 || Char.Model == 1004)
                                    {
                                        if (MyChar.Model == 2001 || MyChar.Model == 2002)
                                        {
                                            Char.Spouse = MyChar.Name;
                                            MyChar.Spouse = Char.Name;
                                            MyChar.SaveSpouse();
                                            Char.SaveSpouse();
                                            SendPacket(General.MyPackets.CharacterInfo(MyChar));
                                            World.SendMsgToAll(Char.Name + " and " + MyChar.Name + " has been united as husband and wife!", "LoveStone", 2011);
                                        }
                                        else
                                        {
                                            SendPacket(General.MyPackets.NPCSay("No Marriage For Same Gender."));
                                            SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                            SendPacket(General.MyPackets.NPCSetFace(30));
                                            SendPacket(General.MyPackets.NPCFinish());
                                        }
                                    }
                                }
                            }
                        }
                        if (AttackType == 2 || AttackType == 25)
                        {
                            uint Target = Data[0x0f];
                            Target = (Target << 8) | Data[0x0e];
                            Target = (Target << 8) | Data[0x0d];
                            Target = (Target << 8) | Data[0x0c];
                            MyChar.TargetUID = Target;

                            if (Target < 7000 && Target >= 5000)
                            {
                                SingleNPC ThisTGO = (SingleNPC)NPCs.AllNPCs[Target];
                                MyChar.MobTarget = null;

                                MyChar.AtkType = (byte)AttackType;
                                MyChar.TGTarget = ThisTGO;

                            }
                            else if (Target > 400000 && Target < 500000)
                            {
                                if (MyChar.Guard != null)
                                {
                                    if (Target != MyChar.Guard.UID)
                                    {
                                        SingleMob TargetMob = (SingleMob)Mobs.AllMobs[Target];

                                        MyChar.MobTarget = null;

                                        if (TargetMob != null)
                                        {
                                            MyChar.AtkType = (byte)AttackType;
                                            MyChar.MobTarget = TargetMob;
                                        }
                                    }
                                }
                                else
                                {
                                    SingleMob TargetMob = (SingleMob)Mobs.AllMobs[Target];

                                    MyChar.MobTarget = null;

                                    if (TargetMob != null)
                                    {
                                        MyChar.AtkType = (byte)AttackType;
                                        MyChar.MobTarget = TargetMob;
                                    }
                                }
                            }
                            else
                            {
                                foreach (DictionaryEntry DE in World.AllChars)
                                {
                                    Character ThisChar = (Character)DE.Value;
                                    if (ThisChar.UID == Target)
                                    {
                                        MyChar.PTarget = ThisChar;
                                        MyChar.AtkType = (byte)AttackType;
                                        break;
                                    }
                                }
                            }
                            if (MyChar.AtkType == 2 && MyChar.PTarget != null && MyChar.PTarget.Flying)
                                MyChar.PTarget = null;

                            MyChar.Attacking = true;
                            MyChar.Attack();
                        }
                        MyChar.Ready = true;
                        break;
                    }
                #endregion
                #region gemsocketting
                case 1027:
                    {
                        MyChar.Ready = false;
                        uint MainItemUID = (uint)((Data[11] << 24) + (Data[10] << 16) + (Data[9] << 8) + Data[8]);
                        uint GemUID = (uint)((Data[15] << 24) + (Data[14] << 16) + (Data[13] << 8) + Data[12]);
                        string RealItem = "";
                        string RealGem = "";

                        int Mode = Data[18];
                        int Slot = Data[16];
                        int Counter = 0;

                        if (Mode == 0)
                        {
                            if (GemUID != 0)
                            {
                                foreach (ulong uid in MyChar.Inventory_UIDs)
                                {
                                    if (GemUID == uid)
                                        RealGem = MyChar.Inventory[Counter];
                                    Counter++;
                                }
                            }
                            Counter = 0;

                            if (MainItemUID != 0)
                            {
                                foreach (ulong uid in MyChar.Inventory_UIDs)
                                {
                                    if (MainItemUID == uid)
                                        RealItem = MyChar.Inventory[Counter];
                                    Counter++;
                                }
                            }

                            if (RealItem != "")
                                if (RealGem != "")
                                {
                                    string[] ItemParts = RealItem.Split('-');
                                    string[] GemParts = RealGem.Split('-');
                                    if (Slot == 1)
                                    {
                                        ItemParts[4] = Convert.ToString(uint.Parse(GemParts[0]) - 700000);
                                    }
                                    if (Slot == 2)
                                    {
                                        ItemParts[5] = Convert.ToString(uint.Parse(GemParts[0]) - 700000);
                                    }
                                    MyChar.RemoveItem(MainItemUID);
                                    MyChar.RemoveItem(GemUID);
                                    MyChar.AddItem(ItemParts[0] + "-" + ItemParts[1] + "-" + ItemParts[2] + "-" + ItemParts[3] + "-" + ItemParts[4] + "-" + ItemParts[5], 0, MainItemUID);
                                }
                        }
                        else
                        {
                            if (MainItemUID != 0)
                            {
                                foreach (ulong uid in MyChar.Inventory_UIDs)
                                {
                                    if (MainItemUID == uid)
                                        RealItem = MyChar.Inventory[Counter];
                                    Counter++;
                                }
                            }
                            if (RealItem == null)
                                return;
                            string[] ItemParts = RealItem.Split('-');
                            if (Slot == 1)
                                if (ItemParts[4] != "0")
                                {
                                    if (ItemParts[5] == "0" || ItemParts[5] == "255")
                                        ItemParts[4] = "255";
                                    else if (ItemParts[5] != "0")
                                    {
                                        ItemParts[4] = ItemParts[5];
                                        ItemParts[5] = "255";
                                    }
                                }
                            if (Slot == 2)
                                if (ItemParts[5] != "0")
                                {
                                    ItemParts[5] = "255";
                                }
                            MyChar.RemoveItem(MainItemUID);
                            MyChar.AddItem(ItemParts[0] + "-" + ItemParts[1] + "-" + ItemParts[2] + "-" + ItemParts[3] + "-" + ItemParts[4] + "-" + ItemParts[5], 0, MainItemUID);
                        }

                        if (Data.Length == 40)
                        {
                            MainItemUID = (uint)((Data[31] << 24) + (Data[30] << 16) + (Data[29] << 8) + Data[28]);
                            GemUID = (uint)((Data[35] << 24) + (Data[34] << 16) + (Data[33] << 8) + Data[32]);
                            RealItem = "";
                            RealGem = "";

                            Mode = Data[38];
                            Slot = Data[36];
                            Counter = 0;

                            if (Mode == 0)
                            {
                                if (GemUID != 0)
                                {
                                    foreach (ulong uid in MyChar.Inventory_UIDs)
                                    {
                                        if (GemUID == uid)
                                            RealGem = MyChar.Inventory[Counter];
                                        Counter++;
                                    }
                                }
                                Counter = 0;

                                if (MainItemUID != 0)
                                {
                                    foreach (ulong uid in MyChar.Inventory_UIDs)
                                    {
                                        if (MainItemUID == uid)
                                            RealItem = MyChar.Inventory[Counter];
                                        Counter++;
                                    }
                                }

                                if (RealItem != "")
                                    if (RealGem != "")
                                    {
                                        string[] ItemParts = RealItem.Split('-');
                                        string[] GemParts = RealGem.Split('-');
                                        if (Slot == 1)
                                        {
                                            ItemParts[4] = Convert.ToString(uint.Parse(GemParts[0]) - 700000);
                                        }
                                        if (Slot == 2)
                                        {
                                            ItemParts[5] = Convert.ToString(uint.Parse(GemParts[0]) - 700000);
                                        }
                                        MyChar.RemoveItem(MainItemUID);
                                        MyChar.RemoveItem(GemUID);
                                        MyChar.AddItem(ItemParts[0] + "-" + ItemParts[1] + "-" + ItemParts[2] + "-" + ItemParts[3] + "-" + ItemParts[4] + "-" + ItemParts[5], 0, MainItemUID);
                                    }
                            }
                            else
                            {
                                string[] ItemParts = RealItem.Split('-');
                                if (Slot == 1)
                                {
                                    ItemParts[4] = "0";
                                }
                                if (Slot == 2)
                                {
                                    ItemParts[5] = "0";
                                }
                                MyChar.RemoveItem(MainItemUID);
                                MyChar.AddItem(ItemParts[0] + "-" + ItemParts[1] + "-" + ItemParts[2] + "-" + ItemParts[3] + "-" + ItemParts[4] + "-" + ItemParts[5], 0, MainItemUID);
                            }
                        }
                        MyChar.Ready = true;
                        break;
                    }
                #endregion
                #region npctalk
                case 2031:
                    {
                        #region Birth Village
                        if (CurrentNPC == 10291)
                        {
                            SendPacket(General.MyPackets.NPCSay("Ahh..What do we have here? A newcomer. Does he have what it takes to make it in this world? Can he withstand the brutal forces this place has to offer? Can he, EHM! Oh my, I'm sorry, I got carried away."));
                            SendPacket(General.MyPackets.NPCLink("Thats fine, I was about to go look around", 255));
                            SendPacket(General.MyPackets.NPCLink("Take me to TwinCity", 1));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 10292)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hey you! You're going to need to know that my brethrin are in every city. Their purpose is to transport fighters above level 20 who wish to train while they're away from the computer!!"));
                            SendPacket(General.MyPackets.NPCLink("Cool!", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 10293)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hi there! I'm the Blacksmith. I sell all kinds of things from arrows to hats, helmets, caps and weapons of all sorts. You can find me in every city if you look hard."));
                            SendPacket(General.MyPackets.NPCLink("Sweet", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 10294)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hi " + MyChar.Name + "! I'm the Pharmacist. I sell different size potions that can restore your health and magic meters by certain amounts. You can find me in every city."));
                            SendPacket(General.MyPackets.NPCLink("Awesome.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 10295)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hello young one. I am the Warehouseman. Everyone trusts me with their most valuable items. Hand over your bulky items and I'll keep them safe forever. Or until you come back for them."));
                            SendPacket(General.MyPackets.NPCLink("Great. I'll stop by sometime.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 10296)
                        {
                            SendPacket(General.MyPackets.NPCSay("Wait, wait!! I can not bare to see a young fighter go off into the danger by himself. Please take this! And for the life of you, stay alive!"));
                            SendPacket(General.MyPackets.NPCLink("Oh wow. Thank you so much sir", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 10297)
                        {
                            SendPacket(General.MyPackets.NPCSay("Have you visited everyone here already?"));
                            SendPacket(General.MyPackets.NPCLink("Yes I have", 1));
                            SendPacket(General.MyPackets.NPCLink("Uhm, no not yet. Sorry", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        #endregion
                        if (CurrentNPC == 7666)//PKCity
                        {
                            SendPacket(General.MyPackets.NPCSay("So you want to enter the PK City?."));
                            SendPacket(General.MyPackets.NPCLink("Oh yea, thanks.)", 1));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 7777)//PKCity Out
                        {
                            SendPacket(General.MyPackets.NPCSay("So you want to leave again?"));
                            SendPacket(General.MyPackets.NPCLink("Yes please.)", 1));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }  
                        if (CurrentNPC == 999999) // +13 - +15 NPC
                        {
                            SendPacket(General.MyPackets.NPCSay("You can upgrade your +12 items to +15!"));
                            SendPacket(General.MyPackets.NPCLink("I want to upgrade it!", 1));
                            SendPacket(General.MyPackets.NPCLink("How does it works ?", 2));
                            SendPacket(General.MyPackets.NPCLink("No Thanks!", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 304)
                        {
                            if (MyChar.Spouse == "" || MyChar.Spouse == "None")
                            {
                                SendPacket(General.MyPackets.NPCSay("Hey! You are single!"));
                                SendPacket(General.MyPackets.NPCLink("I know.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else
                            {
                                SendPacket(General.MyPackets.NPCSay("Stars in the sky represent true love and commitment..."));
                                SendPacket(General.MyPackets.NPCSay(" unfortunately, some stars don't shine as bright, and therefore, need to be destroyed."));
                                SendPacket(General.MyPackets.NPCSay(" Marriage represents those stars. I can divorce you if you and your spouse"));
                                SendPacket(General.MyPackets.NPCSay(" no longer want to be together."));
                                SendPacket(General.MyPackets.NPCLink("I want to be divorced.", 1));
                                SendPacket(General.MyPackets.NPCLink("I am happily Married.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                        }
                        if (CurrentNPC == 9950)
                        {
                            SendPacket(General.MyPackets.NPCSay("Do you want to have your size changed so that you can become much more adorable? Now here is a precious chance for you."));
                            SendPacket(General.MyPackets.NPCLink("I want to change my size.", 1));
                            SendPacket(General.MyPackets.NPCLink("I don`t want to change.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 3825)
                        {
                            if (MyChar.Level >= 40)
                            {
                                SendPacket(General.MyPackets.NPCSay("The Dragonball is a supernatural item which can not only upgrade the"));
                                SendPacket(General.MyPackets.NPCSay(" level and quality of your gears, but also help you level up quickly. You"));
                                SendPacket(General.MyPackets.NPCSay(" know, there is infinite energy contained in a Dragonball. And I will help you"));
                                SendPacket(General.MyPackets.NPCSay(" draw energy from the Dragonball to increase your exp, and improve your level"));
                                SendPacket(General.MyPackets.NPCLink("Wow! I can't wait to start.", 1));
                                SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else
                            {
                                SendPacket(General.MyPackets.NPCSay("You have to be at least level 40 to draw the energy of the Dragonball."));
                                SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                        }
                        if (CurrentNPC == 1279)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hey there,want to enter Dis City. Only for level 110+, and you get alot of exp for every stage you enter,and exp from 10 expballs if you get to the last stage. But remember, its dangerous."));
                            SendPacket(General.MyPackets.NPCLink("Yes please,send me there", 1));
                            SendPacket(General.MyPackets.NPCLink("No thanks,not ready yet.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 1280)
                        {
                            SendPacket(General.MyPackets.NPCSay("Are you sure you want to go to stage 2? Remember that you need 5 SoulStones."));
                            SendPacket(General.MyPackets.NPCLink("Yes please,send me there", 1));
                            SendPacket(General.MyPackets.NPCLink("No thanks,not ready yet.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 1281)
                        {
                            SendPacket(General.MyPackets.NPCSay("Are you sure you want to go to stage 3? Remember that you need to kill 1300 Monster first"));
                            SendPacket(General.MyPackets.NPCLink("Yes please,send me there", 1));
                            SendPacket(General.MyPackets.NPCLink("No thanks,not ready yet.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 128)
                        {
                            if (MyChar.InventoryContains(721000, 1))
                            {
                                SendPacket(General.MyPackets.NPCSay("My sister is in the City of Birds, there can Find."));
                                SendPacket(General.MyPackets.NPCLink("Oh I see", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }


                            else
                            {
                                SendPacket(General.MyPackets.NPCSay("Our feelings are very deep, and we loved her. When was I felt very sad."));
                                SendPacket(General.MyPackets.NPCLink("¿Que puedo hacer por tí?", 1));
                                SendPacket(General.MyPackets.NPCLink("It is very sad", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());


                            }
                        }
                        if (CurrentNPC == 129)
                        {
                            if (MyChar.InventoryContains(721000, 1))
                            {
                                SendPacket(General.MyPackets.NPCSay("I feel good or bad I need to live and I can not leave aside my feelings by Jorge."));
                                SendPacket(General.MyPackets.NPCLink("That you worried about??", 1));
                                SendPacket(General.MyPackets.NPCLink("That good words", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else
                            {
                                if (MyChar.InventoryContains(721002, 1))
                                {
                                    SendPacket(General.MyPackets.NPCSay("A gift from my boyfriend?. Thanks! Recompensaré you with a tear of meteor for help."));
                                    SendPacket(General.MyPackets.NPCLink("Thanks.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                    if (MyChar.InventoryContains(721002, 1))
                                    {
                                        MyChar.RemoveItem(MyChar.ItemNext(721002));
                                        MyChar.AddItem("1088002-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                }


                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("I feel good or bad I need to live and I can not leave aside my feelings by Jorge."));
                                    SendPacket(General.MyPackets.NPCLink("That good words", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }
                        if (CurrentNPC == 130)
                        {

                            if (MyChar.InventoryContains(1000030, 1))
                            {


                                SendPacket(General.MyPackets.NPCSay("It is very hot, you got me wine?."));
                                SendPacket(General.MyPackets.NPCLink("Nectar I Want? ", 1));
                                SendPacket(General.MyPackets.NPCLink("No, I do not have", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());

                            }


                            else
                            {
                                SendPacket(General.MyPackets.NPCSay("It is very hot, you got me wine?."));
                                SendPacket(General.MyPackets.NPCLink("No, sorry", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                        }
                        if (CurrentNPC == 8410) // Nobility Guy
                        {
                            if (MyChar.Rank > 2 && MyChar.Rank < 10)
                            {
                                SendPacket(General.MyPackets.NPCSay("Which rank would you like to become? Only thing you have to do is pay me!"));
                                SendPacket(General.MyPackets.NPCLink("King - 500 Millon", 1));
                                SendPacket(General.MyPackets.NPCLink("Queen - 300 Millon", 2));
                                SendPacket(General.MyPackets.NPCLink("Duke - 70 Millon", 3));
                                SendPacket(General.MyPackets.NPCLink("Prince - 50 Millon", 4));
                                SendPacket(General.MyPackets.NPCLink("Baron - 35 Millon", 5));
                                SendPacket(General.MyPackets.NPCLink("Knight - 15 Millon", 6));
                                SendPacket(General.MyPackets.NPCLink("Earl - 3 Millon", 7));
                                SendPacket(General.MyPackets.NPCLink("I'm too poor!", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.Rank == 1 && MyChar.Model == 1003 || MyChar.Rank == 1 && MyChar.Model == 1004)
                            {
                                SendPacket(General.MyPackets.NPCSay("Your a King! Why would you want to be anything else?"));
                                SendPacket(General.MyPackets.NPCLink("I'm too poor!", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.Rank == 2 && MyChar.Model == 2001 || MyChar.Rank == 2 && MyChar.Model == 2002)
                            {
                                SendPacket(General.MyPackets.NPCSay("Your a Queen! Why would you want to be anything else?"));
                                SendPacket(General.MyPackets.NPCLink("I'm too poor!", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }

                        }
                        MyChar.Ready = false;
                        int NPCID = (Data[7] << 24) + (Data[6] << 16) + (Data[5] << 8) + Data[4];
                        SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "You tried to talk to npc(ID: " + NPCID + ")", 2005));
                        int Control = (int)Data[10];
                        CurrentNPC = NPCID;
                        if (CurrentNPC == 912)//Give CPS
                        {
                            SendPacket(General.MyPackets.NPCSay("Hi, my name in ~Fury~. I can give u some free cps so u dont have to bug a GM anymore?"));
                            SendPacket(General.MyPackets.NPCLink("Ok.", 1));
                            SendPacket(General.MyPackets.NPCLink("No Thanks.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 390)
                        {
                            if (MyChar.Spouse == "" || MyChar.Spouse == "None")
                            {
                                SendPacket(General.MyPackets.NPCSay("Do you wish to propose to your sweetheart?"));
                                SendPacket(General.MyPackets.NPCSay("Remember that marriage is a serious commitment, do not enter into it lightly."));
                                SendPacket(General.MyPackets.NPCLink("Yes I want to propose", 1));
                                SendPacket(General.MyPackets.NPCLink("Get double exp. time.", 3));
                                SendPacket(General.MyPackets.NPCLink("Nooo I'm not ready", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else
                            {
                                SendPacket(General.MyPackets.NPCSay("You have already spouse."));
                                SendPacket(General.MyPackets.NPCLink("Sorry", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                        }
                        if (CurrentNPC == 2020)
                        {
                            SendPacket(General.MyPackets.NPCSay("Welcome to the server"));
                            SendPacket(General.MyPackets.NPCSay("Ask a GM or PM for help!"));
                            SendPacket(General.MyPackets.NPCLink("Thanks", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        #region TC Monster Hunter Quest
                        if (CurrentNPC == 280)//TC CloudSaint'sJar Quest
                        {
                            if (MyChar.QuestFrom == "")
                            {
                                if (MyChar.Level <= 24)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Glad to see you are here! Twin City is being besieged by monsters recently. If you can help us out, your reward will be EXP worth half an ExpBall and a Meteor. But remember you can only get 3 opportunities everyday."));
                                    SendPacket(General.MyPackets.NPCLink("Go kill Turtledoves (Level 7)", 1));
                                    SendPacket(General.MyPackets.NPCLink("Go kill Robins (Level 12)", 2));
                                    SendPacket(General.MyPackets.NPCLink("Go kill Apparitions (Level 17)", 3));
                                    SendPacket(General.MyPackets.NPCLink("Go kill Poltergeists (Level 22)", 4));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 44)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Phoenix City, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Phoenix City.", 10));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 64)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Ape Mountain, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Ape Mountain.", 11));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 84)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Desert City, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Desert City.", 12));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 99)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Bird Island, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Bird Island.", 13));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 119)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Mystic Castle, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Mystic Castle.", 14));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level >= 120)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You have made the land peaceful! There is nothing left for you to do!"));
                                    SendPacket(General.MyPackets.NPCLink("Okay.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            else if (MyChar.QuestFrom == "TC")
                            {
                                if (MyChar.QuestKO >= 100 && MyChar.InventoryContains(750000, 1))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(750000));
                                    MyChar.QuestKO = 0;
                                    MyChar.QuestMob = "";
                                    MyChar.QuestFrom = "";
                                    MyChar.AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    SendPacket(General.MyPackets.NPCSay("Good job! You killed 100 Monsters! Here is your Exp worth half an ExpBall and a Meteor."));
                                    SendPacket(General.MyPackets.NPCLink("Thanks.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                    MyChar.Save();
                                }
                                else if (MyChar.QuestKO >= 0)
                                {
                                    if (MyChar.InventoryContains(750000, 1))
                                    {
                                        SendPacket(General.MyPackets.NPCSay("Why do you hurry to come back? What happened? How about your quest to kill 300 " + MyChar.QuestMob + "?"));
                                        SendPacket(General.MyPackets.NPCLink("I'll get to it.", 255));
                                        SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("Alas! Where is the CloudSaint'sJar that I lent you? If you can't find it, I suggest you to give up the quest. But don't worry, you can get a new quest again."));
                                        SendPacket(General.MyPackets.NPCLink("Sorry its my fault.", 9));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                            else if (MyChar.QuestFrom == "PC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from PCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "AC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from ACCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "DC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from DCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "BI")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from BICaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "MC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from MCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                        }
                        #endregion
                        #region PC Monster Hunter Quest
                        if (CurrentNPC == 281)//PC CloudSaint'sJar Quest
                        {
                            if (MyChar.QuestFrom == "")
                            {
                                if (MyChar.Level <= 24)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Twin City, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Twin City.", 9));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 44)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Glad to see you are here! Phoenix City is being besieged by monsters recently. If you can help us out, your reward will be EXP worth a ExpBall and Meteor. But remember you can only get 3 opportunities everyday."));
                                    SendPacket(General.MyPackets.NPCLink("Go kill WingedSnakes (Level 27)", 1));
                                    SendPacket(General.MyPackets.NPCLink("Go kill Bandits (Level 32)", 2));
                                    SendPacket(General.MyPackets.NPCLink("Go kill FireRats (Level 42)", 3));
                                    SendPacket(General.MyPackets.NPCLink("Go kill FireSpirits (Level 47)", 4));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 64)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Ape Mountain, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Ape Mountain.", 11));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 84)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Desert City, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Desert City.", 12));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 99)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Bird Island, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Bird Island.", 13));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 119)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Mystic Castle, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Mystic Castle.", 14));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level >= 120)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You have made the land peaceful! There is nothing left for you to do!"));
                                    SendPacket(General.MyPackets.NPCLink("Okay.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            else if (MyChar.QuestFrom == "PC")
                            {
                                if (MyChar.QuestKO >= 300 && MyChar.InventoryContains(750000, 1))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(750000));
                                    MyChar.QuestKO = 0;
                                    MyChar.QuestMob = "";
                                    MyChar.QuestFrom = "";
                                    MyChar.AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    SendPacket(General.MyPackets.NPCSay("Good job! You killed 300 Monsters! Here is your Exp and Meteor."));
                                    SendPacket(General.MyPackets.NPCLink("Thanks.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                    MyChar.Save();
                                }
                                else if (MyChar.QuestKO >= 0)
                                {
                                    if (MyChar.InventoryContains(750000, 1))
                                    {
                                        SendPacket(General.MyPackets.NPCSay("Why do you hurry to come back? What happened? How about your quest to kill 300 " + MyChar.QuestMob + "?"));
                                        SendPacket(General.MyPackets.NPCLink("I'll get to it.", 255));
                                        SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("Alas! Where is the CloudSaint'sJar that I lent you? If you can't find it, I suggest you to give up the quest. But don't worry, you can get a new quest again."));
                                        SendPacket(General.MyPackets.NPCLink("Sorry its my fault.", 9));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                            else if (MyChar.QuestFrom == "TC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from TCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "AC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from ACCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "DC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from DCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "BI")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from BICaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "MC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from MCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                        }
                        #endregion
                        #region AC Monster Hunter Quest
                        if (CurrentNPC == 282)//AC CloudSaint'sJar Quest
                        {
                            if (MyChar.QuestFrom == "")
                            {
                                if (MyChar.Level <= 24)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Twin City, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Twin City.", 9));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 44)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Pheonix City, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Pheonix City.", 10));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 64)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Glad to see you are here! Ape Mountain is being besieged by monsters recently. If you can help us out, your reward will be EXP worth a ExpBall and Meteor. But remember you can only get 3 opportunities everyday."));
                                    SendPacket(General.MyPackets.NPCLink("Go kill Macaquees (Level 47)", 1));
                                    SendPacket(General.MyPackets.NPCLink("Go kill GiantAps (Level 52)", 2));
                                    SendPacket(General.MyPackets.NPCLink("Go kill ThunderApes (Level 57)", 3));
                                    SendPacket(General.MyPackets.NPCLink("Go kill SnakeMen (Level 62)", 4));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 84)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Desert City, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Desert City.", 12));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 99)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Bird Island, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Bird Island.", 13));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 119)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Mystic Castle, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Mystic Castle.", 14));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level >= 120)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You have made the land peaceful! There is nothing left for you to do!"));
                                    SendPacket(General.MyPackets.NPCLink("Okay.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            else if (MyChar.QuestFrom == "AC")
                            {
                                if (MyChar.QuestKO >= 300 && MyChar.InventoryContains(750000, 1))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(750000));
                                    MyChar.QuestKO = 0;
                                    MyChar.QuestMob = "";
                                    MyChar.QuestFrom = "";
                                    MyChar.AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    SendPacket(General.MyPackets.NPCSay("Good job! You killed 300 Monsters! Here is your Exp and Meteor."));
                                    SendPacket(General.MyPackets.NPCLink("Thanks.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                    MyChar.Save();
                                }
                                else if (MyChar.QuestKO >= 0)
                                {
                                    if (MyChar.InventoryContains(750000, 1))
                                    {
                                        SendPacket(General.MyPackets.NPCSay("Why do you hurry to come back? What happened? How about your quest to kill 300 " + MyChar.QuestMob + "?"));
                                        SendPacket(General.MyPackets.NPCLink("I'll get to it.", 255));
                                        SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("Alas! Where is the CloudSaint'sJar that I lent you? If you can't find it, I suggest you to give up the quest. But don't worry, you can get a new quest again."));
                                        SendPacket(General.MyPackets.NPCLink("Sorry its my fault.", 9));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                            else if (MyChar.QuestFrom == "TC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from TCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "PC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from PCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "DC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from DCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "BI")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from BICaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "MC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from MCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                        }
                        #endregion
                        #region DC Monster Hunter Quest
                        if (CurrentNPC == 283)//DC CloudSaint'sJar Quest
                        {
                            if (MyChar.QuestFrom == "")
                            {
                                if (MyChar.Level <= 24)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Twin City, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Twin City.", 9));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 44)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Phoenix City, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Phoenix City.", 10));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 64)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Ape Mountain, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Ape Mountain.", 11));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                }
                                else if (MyChar.Level <= 84)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Glad to see you are here! Desert City is being besieged by monsters recently. If you can help us out, your reward will be EXP worth a ExpBall and Meteor. But remember you can only get 3 opportunities everyday."));
                                    SendPacket(General.MyPackets.NPCLink("Go kill SandMonsters (Level 67)", 1));
                                    SendPacket(General.MyPackets.NPCLink("Go kill HillMonsters (Level 72)", 2));
                                    SendPacket(General.MyPackets.NPCLink("Go kill RockMonsters (Level 77)", 3));
                                    SendPacket(General.MyPackets.NPCLink("Go kill BladeGhosts (Level 82)", 4));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 99)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Bird Island, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Bird Island.", 13));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 119)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Mystic Castle, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Mystic Castle.", 14));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level >= 120)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You have made the land peaceful! There is nothing left for you to do!"));
                                    SendPacket(General.MyPackets.NPCLink("Okay.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            else if (MyChar.QuestFrom == "DC")
                            {
                                if (MyChar.QuestKO >= 300 && MyChar.InventoryContains(750000, 1))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(750000));
                                    MyChar.QuestKO = 0;
                                    MyChar.QuestMob = "";
                                    MyChar.QuestFrom = "";
                                    MyChar.AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    SendPacket(General.MyPackets.NPCSay("Good job! You killed 300 Monsters! Here is your Exp and Meteor."));
                                    SendPacket(General.MyPackets.NPCLink("Thanks.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                    MyChar.Save();
                                }
                                else if (MyChar.QuestKO >= 0)
                                {
                                    if (MyChar.InventoryContains(750000, 1))
                                    {
                                        SendPacket(General.MyPackets.NPCSay("Why do you hurry to come back? What happened? How about your quest to kill 300 " + MyChar.QuestMob + "?"));
                                        SendPacket(General.MyPackets.NPCLink("I'll get to it.", 255));
                                        SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("Alas! Where is the CloudSaint'sJar that I lent you? If you can't find it, I suggest you to give up the quest. But don't worry, you can get a new quest again."));
                                        SendPacket(General.MyPackets.NPCLink("Sorry its my fault.", 9));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                            else if (MyChar.QuestFrom == "TC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from TCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "PC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from PCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "AC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from ACCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "BI")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from BICaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "MC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from MCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                        }
                        #endregion
                        #region BI Monster Hunter Quest
                        if (CurrentNPC == 284)//BI CloudSaint'sJar Quest
                        {
                            if (MyChar.QuestFrom == "")
                            {
                                if (MyChar.Level <= 24)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Twin City, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Twin City.", 9));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 44)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Pheonix City, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Pheonix City.", 10));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 64)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Ape Mountain, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Ape Mountain.", 11));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                }
                                else if (MyChar.Level <= 84)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Desert City, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Desert City.", 12));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                }
                                else if (MyChar.Level <= 99)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Glad to see you are here! Bird Island is being besieged by monsters recently. If you can help us out, your reward will be EXP worth a ExpBall and Meteor. But remember you can only get 3 opportunities everyday."));
                                    SendPacket(General.MyPackets.NPCLink("Go kill Birdmen (Level 87)", 1));
                                    SendPacket(General.MyPackets.NPCLink("Go kill Hawkings (Level 92)", 2));
                                    SendPacket(General.MyPackets.NPCLink("Go kill BanditL97, BanditL98, or Robbers (Level 97)", 3));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 119)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Mystic Castle, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Mystic Castle.", 14));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level >= 120)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You have made the land peaceful! There is nothing left for you to do!"));
                                    SendPacket(General.MyPackets.NPCLink("Okay.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            else if (MyChar.QuestFrom == "BI")
                            {
                                if (MyChar.QuestKO >= 300 && MyChar.InventoryContains(750000, 1))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(750000));
                                    MyChar.QuestKO = 0;
                                    MyChar.QuestMob = "";
                                    MyChar.QuestFrom = "";
                                    MyChar.AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    SendPacket(General.MyPackets.NPCSay("Good job! You killed 300 Monsters! Here is your Exp and Meteor."));
                                    SendPacket(General.MyPackets.NPCLink("Thanks.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                    MyChar.Save();
                                }
                                else if (MyChar.QuestKO >= 0)
                                {
                                    if (MyChar.InventoryContains(750000, 1))
                                    {
                                        SendPacket(General.MyPackets.NPCSay("Why do you hurry to come back? What happened? How about your quest to kill 300 " + MyChar.QuestMob + "?"));
                                        SendPacket(General.MyPackets.NPCLink("I'll get to it.", 255));
                                        SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("Alas! Where is the CloudSaint'sJar that I lent you? If you can't find it, I suggest you to give up the quest. But don't worry, you can get a new quest again."));
                                        SendPacket(General.MyPackets.NPCLink("Sorry its my fault.", 9));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                            else if (MyChar.QuestFrom == "TC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from TCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "PC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from PCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "AC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from ACCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "DC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from DCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "MC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from MCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                        }
                        #endregion
                        #region MC Monster Hunter Quest
                        if (CurrentNPC == 285)//MC CloudSaint'sJar Quest
                        {
                            if (MyChar.QuestFrom == "")
                            {
                                if (MyChar.Level <= 24)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Twin City, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Twin City.", 9));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 44)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Phoenix City, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Phoenix City.", 10));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 64)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Ape Mountain, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Ape Mountain.", 11));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                }
                                else if (MyChar.Level <= 84)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Desert City, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Desert City.", 12));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                }
                                else if (MyChar.Level <= 99)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are powerful now. I suggest you go to the Captain of Bird Island, because I heard he is annoyed by the rampant monsters there. I believe you can help him a lot. I will teleport you there to save time."));
                                    SendPacket(General.MyPackets.NPCLink("Please send me to Bird Island.", 13));
                                    SendPacket(General.MyPackets.NPCLink("I don't want to go there.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level <= 119)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Glad to see you are here! Mystic Castle is being besieged by monsters recently. If you can help us out, your reward will be EXP worth a ExpBall and Meteor. But remember you can only get 3 opportunities everyday."));
                                    SendPacket(General.MyPackets.NPCLink("Go kill Tombats (Level 102)", 1));
                                    SendPacket(General.MyPackets.NPCLink("Go kill Bloodybats (Level 107)", 2));
                                    SendPacket(General.MyPackets.NPCLink("Go kill BullMonsters (Level 112)", 3));
                                    SendPacket(General.MyPackets.NPCLink("Go kill RedDevils (Level 117)", 4));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level >= 120)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You have made the land peaceful! There is nothing left for you to do!"));
                                    SendPacket(General.MyPackets.NPCLink("Okay.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            else if (MyChar.QuestFrom == "MC")
                            {
                                if (MyChar.QuestKO >= 300 && MyChar.InventoryContains(750000, 1))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(750000));
                                    MyChar.QuestKO = 0;
                                    MyChar.QuestMob = "";
                                    MyChar.QuestFrom = "";
                                    MyChar.AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    SendPacket(General.MyPackets.NPCSay("Good job! You killed 300 Monsters! Here is your Exp and Meteor."));
                                    SendPacket(General.MyPackets.NPCLink("Thanks.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                    MyChar.Save();
                                }
                                else if (MyChar.QuestKO >= 0)
                                {
                                    if (MyChar.InventoryContains(750000, 1))
                                    {
                                        SendPacket(General.MyPackets.NPCSay("Why do you hurry to come back? What happened? How about your quest to kill 300 " + MyChar.QuestMob + "?"));
                                        SendPacket(General.MyPackets.NPCLink("I'll get to it.", 255));
                                        SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("Alas! Where is the CloudSaint'sJar that I lent you? If you can't find it, I suggest you to give up the quest. But don't worry, you can get a new quest again."));
                                        SendPacket(General.MyPackets.NPCLink("Sorry its my fault.", 9));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                            else if (MyChar.QuestFrom == "TC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from TCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "PC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from PCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "AC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from ACCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "DC")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from DCCaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else if (MyChar.QuestFrom == "BI")
                            {
                                SendPacket(General.MyPackets.NPCSay("The quest you got from BICaptain hasn't been finished yet. Please finish it or give up before getting the new quest from me."));
                                SendPacket(General.MyPackets.NPCLink("I'm going to finish it right away.", 255));
                                SendPacket(General.MyPackets.NPCLink("I want to end the Quest.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                        }
                        #endregion

                        if (CurrentNPC == 7500)
                        {
                            SendPacket(General.MyPackets.NPCSay("Got many dragon balls you want to get rid of? Well i can give you 50 cps for each dragon ball you give me."));
                            SendPacket(General.MyPackets.NPCLink("Yes, i want CPs for my dragon ball!", 1));
                            SendPacket(General.MyPackets.NPCLink("I don't have a dragon ball.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 8487)
                        {
                            SendPacket(General.MyPackets.NPCSay("Having problems with inventory? I can clear it for you, so you wouldn't bug a game master anymore."));
                            SendPacket(General.MyPackets.NPCLink("Yea i want to clear my inventory.", 1));
                            SendPacket(General.MyPackets.NPCLink("No i don't.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 9812)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hunting meteors and dragonballs is an exciting thing. However, they also pile up in your inventories."));
                            SendPacket(General.MyPackets.NPCLink("Agreed. How do you deal with it?", 1));
                            SendPacket(General.MyPackets.NPCLink("I am poor and do not have the problem.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 6700)
                        {
                            SendPacket(General.MyPackets.NPCSay("Do you want to heal the pole?"));
                            SendPacket(General.MyPackets.NPCLink("Yes", 1));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 6701 || CurrentNPC == 6702)
                        {
                            SendPacket(General.MyPackets.NPCSay("What do you want to do?"));
                            SendPacket(General.MyPackets.NPCLink("Open/Close the gate.", 1));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 959)
                        {
                            SendPacket(General.MyPackets.NPCSay("I'm here to compose your gems. 15 normal gems to refined and 20 refined gems to super gem."));
                            SendPacket(General.MyPackets.NPCLink("I wanna compose PhoenixGems.", 1));
                            SendPacket(General.MyPackets.NPCLink("I wanna compose DragonGems.", 2));
                            SendPacket(General.MyPackets.NPCLink("I wanna compose FuryGems.", 3));
                            SendPacket(General.MyPackets.NPCLink("I wanna compose RainbowGems.", 4));
                            SendPacket(General.MyPackets.NPCLink("I wanna compose KylinGems.", 5));
                            SendPacket(General.MyPackets.NPCLink("I wanna compose VioletGems.", 6));
                            SendPacket(General.MyPackets.NPCLink("I wanna compose MoonGems.", 7));
                            SendPacket(General.MyPackets.NPCLink("I wanna compose TortoiseGems.", 8));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 999)
                        {
                            UppAgree = false;
                            SendPacket(General.MyPackets.NPCSay("MagicArtisan is very trustworthy, he can always upgrade the level and the quality of your equipment without failure. However, he is hopeless in upgrading the high-level equipment. I can do what he can't do. What are you going to upgrade?"));
                            SendPacket(General.MyPackets.NPCLink("Can you tell me more?", 1));
                            SendPacket(General.MyPackets.NPCLink("Upgrade my  weapon.", 2));
                            SendPacket(General.MyPackets.NPCLink("Necklace/Bag.", 3));
                            SendPacket(General.MyPackets.NPCLink("Heavy Ring, Bracelet.", 4));
                            SendPacket(General.MyPackets.NPCLink("Boots.", 5));
                            SendPacket(General.MyPackets.NPCLink("Upgrade armor.", 6));
                            SendPacket(General.MyPackets.NPCLink("Upgrade head gear.", 7));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 127)
                        {
                            if (MyChar.RBCount >= 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("You are already reborn. I cannot help you."));
                                SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else
                            {
                                if (MyChar.Level >= 120)
                                {
                                    SendPacket(General.MyPackets.NPCSay("I devote all my life to the research of eternity, finally I know the aranum of Rebirth of the life."));
                                    SendPacket(General.MyPackets.NPCLink("I would like to know it.", 1));
                                    SendPacket(General.MyPackets.NPCLink("Just passing by.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level >= 110 && MyChar.Job == 135)
                                {
                                    SendPacket(General.MyPackets.NPCSay("I devote all my life to the research of eternity, finally I know the aranum of Rebirth of the life."));
                                    SendPacket(General.MyPackets.NPCLink("I would like to know it.", 1));
                                    SendPacket(General.MyPackets.NPCLink("Just passing by.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level > 119)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You can only reborn if you are level 120 or over."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }
                        if (CurrentNPC == 1278)
                        {
                            if (MyChar.RBCount >= 2)
                            {
                                SendPacket(General.MyPackets.NPCSay("You are already reborn. I cannot help you."));
                                SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            else
                            {
                                if (MyChar.Level >= 120)
                                {
                                    SendPacket(General.MyPackets.NPCSay("I devote all my life to the research of eternity, finally I know the aranum of Second Rebirth."));
                                    SendPacket(General.MyPackets.NPCLink("I would like to know it.", 1));
                                    SendPacket(General.MyPackets.NPCLink("Just passing by.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level >= 110 && MyChar.Job == 137)
                                {
                                    SendPacket(General.MyPackets.NPCSay("I devote all my life to the research of eternity, finally I know the aranum of Second Rebirth."));
                                    SendPacket(General.MyPackets.NPCLink("I would like to know it.", 1));
                                    SendPacket(General.MyPackets.NPCLink("Just passing by.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Level > 119)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You can only reborn if you are level 120 or over."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }
                        if (CurrentNPC == 700)
                        {
                            SendPacket(General.MyPackets.NPCSay("I am a master of upgrading items. My skills are almost transcendant. I never fail. I have heard of other who can upgrade items, but I also know that they sometimes fail and break their customer's items. I charge a higher fee for my services, but I never dissappoint a customer. What is your desire?"));
                            SendPacket(General.MyPackets.NPCLink("Upgrade item quality.", 1));
                            SendPacket(General.MyPackets.NPCLink("Upgrade item level.", 2));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }

                        if (CurrentNPC < 618 && CurrentNPC > 613)
                        {
                            SendPacket(General.MyPackets.NPCSay("Would you like to go back to Guild Area in Twin City? I will teleport you there and charge 500 silvers."));
                            SendPacket(General.MyPackets.NPCLink("Please teleport me there.", 1));
                            SendPacket(General.MyPackets.NPCLink("No. Wait a moment.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC < 614 && CurrentNPC > 609)
                        {
                            SendPacket(General.MyPackets.NPCSay("Are you heading for the next teleporter? It will cost you 1000 silvers."));
                            SendPacket(General.MyPackets.NPCLink("Please teleport me there.", 1));
                            SendPacket(General.MyPackets.NPCLink("No. Wait a moment.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 601)
                        {
                            SendPacket(General.MyPackets.NPCSay("Are you going to leave here?"));
                            SendPacket(General.MyPackets.NPCLink("Yes.", 1));
                            SendPacket(General.MyPackets.NPCLink("No. Wait a moment.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 600)
                        {
                            SendPacket(General.MyPackets.NPCSay("What can i do for you?"));
                            SendPacket(General.MyPackets.NPCLink("Enter the guild arena.", 1));
                            SendPacket(General.MyPackets.NPCLink("Our guild won the guild war, i am here to claim the price.", 2));
                            SendPacket(General.MyPackets.NPCLink("Just passing by.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 573)
                        {
                            SendPacket(General.MyPackets.NPCSay("Are you heading towards Twin City mine? I can teleport you there for free."));
                            SendPacket(General.MyPackets.NPCLink("Sure, thanks!", 1));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 198)
                        {
                            SendPacket(General.MyPackets.NPCSay("This is the way to the Desert City. Although you are excellent, it is dangerous to go ahead."));
                            SendPacket(General.MyPackets.NPCLink("But i still wanna go!", 1));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC >= 104 && CurrentNPC <= 107)
                        {
                            SendPacket(General.MyPackets.NPCSay("Where are you heading for? I can teleport you for a price of 100 silver."));
                            SendPacket(General.MyPackets.NPCLink("Twin City", 1));
                            if (MyChar.LocMap == 1000)
                                SendPacket(General.MyPackets.NPCLink("Mystic castle", 3));
                            SendPacket(General.MyPackets.NPCLink("Market", 2));
                            SendPacket(General.MyPackets.NPCLink("Just passing by.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(134));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 180)
                        {
                            SendPacket(General.MyPackets.NPCSay("Do you want to leave training grounds?"));
                            SendPacket(General.MyPackets.NPCLink("Yes.", 1));
                            SendPacket(General.MyPackets.NPCLink("No, just passing by.", 2));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 155)
                        {
                            SendPacket(General.MyPackets.NPCSay("I am in charge of all the guilds in TwinCity, You may consult me for anything related to the guilds."));
                            SendPacket(General.MyPackets.NPCLink("Create a Guild.", 1));
                            SendPacket(General.MyPackets.NPCLink("Deputize.", 3));
                            SendPacket(General.MyPackets.NPCLink("Disband my guild.", 5));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC < 506 && CurrentNPC > 499)
                        {
                            SendPacket(General.MyPackets.NPCSay("Would u like to choose this box?"));
                            SendPacket(General.MyPackets.NPCLink("Yes.", 1));
                            SendPacket(General.MyPackets.NPCLink("No Thanks.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 1846)
                        {
                            SendPacket(General.MyPackets.NPCSay("Would u like to try your luck in the lottery! It only cost 27 CPs."));
                            SendPacket(General.MyPackets.NPCLink("Yes.", 1));
                            SendPacket(General.MyPackets.NPCLink("No Thanks.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 8)
                        {
                            SendPacket(General.MyPackets.ETCPacket(MyChar, 4));
                        }
                        if (CurrentNPC == 81)
                        {
                            SendPacket(General.MyPackets.ETCPacket(MyChar, 4));
                        }
                        if (CurrentNPC == 82)
                        {
                            SendPacket(General.MyPackets.ETCPacket(MyChar, 4));
                        }
                        if (CurrentNPC == 83)
                        {
                            SendPacket(General.MyPackets.ETCPacket(MyChar, 4));
                        }
                        if (CurrentNPC == 84)
                        {
                            SendPacket(General.MyPackets.ETCPacket(MyChar, 4));
                        }
                        if (CurrentNPC == 85)
                        {
                            SendPacket(General.MyPackets.ETCPacket(MyChar, 4));
                        }
                        if (CurrentNPC == 278)
                        {
                            SendPacket(General.MyPackets.NPCSay("This machine here can compose, bless and enchant your items."));
                            SendPacket(General.MyPackets.NPCLink("Compose", 1));
                            SendPacket(General.MyPackets.NPCLink("Enchant", 2));
                            SendPacket(General.MyPackets.NPCLink("Bless", 10));
                            SendPacket(General.MyPackets.NPCLink("No Thanks", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 10009)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hey,i can plus ur items higher then the composer, wat would u like me to compose it to."));
                            SendPacket(General.MyPackets.NPCLink("+10", 1));
                            SendPacket(General.MyPackets.NPCLink("+11", 11));
                            SendPacket(General.MyPackets.NPCLink("+12", 21));
                            SendPacket(General.MyPackets.NPCLink("Nothing Thanks.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 80801)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hey i know there aren't +2,+7 and +8 stones in shopping mall so you can buy them from me,What would u like?"));
                            SendPacket(General.MyPackets.NPCLink("+2Stone(50 Cps)", 1));
                            SendPacket(General.MyPackets.NPCLink("+7Stone(4054 Cps)", 2));
                            SendPacket(General.MyPackets.NPCLink("+8Stone(8048 Cps)", 3));
                            SendPacket(General.MyPackets.NPCLink("Clean Water(500 Cps)", 4));
                            SendPacket(General.MyPackets.NPCLink("MoonCake1(765 Cps)", 5));
                            SendPacket(General.MyPackets.NPCLink("MoonCake2(8500 Cps)", 6));
                            SendPacket(General.MyPackets.NPCLink("Nothing", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 1234)//Lab 1 General 
                        {
                            SendPacket(General.MyPackets.NPCSay("I am EastGeneral. This is First floor of Labyrinth. If you want to enter next floor, you should give me a SkyToken. Or do you want to go back to Twin City?"));
                            SendPacket(General.MyPackets.NPCLink("To the next floor.", 1));
                            SendPacket(General.MyPackets.NPCLink("To Twin City.", 2));
                            SendPacket(General.MyPackets.NPCLink("I will stay here.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 1153)//Lab 2 General
                        {
                            SendPacket(General.MyPackets.NPCSay("I am WestGeneral. This is Second floor of Labyrinth. If you want to enter next floor, you should give me a EarthToken. Or do you want to go back to Twin City?"));
                            SendPacket(General.MyPackets.NPCLink("To the next floor.", 1));
                            SendPacket(General.MyPackets.NPCLink("To Twin City.", 2));
                            SendPacket(General.MyPackets.NPCLink("I will stay here.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 1154)//Lab 3 General
                        {
                            SendPacket(General.MyPackets.NPCSay("I am SouthGeneral. This is Third floor of Labyrinth. If you want to enter next floor, you should give me a SoulToken. Or do you want to go back to Twin City?"));
                            SendPacket(General.MyPackets.NPCLink("To the next floor.", 1));
                            SendPacket(General.MyPackets.NPCLink("To Twin City.", 2));
                            SendPacket(General.MyPackets.NPCLink("I will stay here.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 1147)//Lab 4 - Back to tc
                        {
                            SendPacket(General.MyPackets.NPCSay("I am the gate to TC. Would you like to go to TC?"));
                            SendPacket(General.MyPackets.NPCLink("Yes.. Please.", 1));
                            SendPacket(General.MyPackets.NPCLink("I will stay here.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 5555) //Shelby
                        {
                            SendPacket(General.MyPackets.NPCSay("We hope all can help each other. If you power level the newbies, we may reward you according to your virtue points. Are you interested?"));
                            SendPacket(General.MyPackets.NPCLink("Tell me more details.", 1));
                            SendPacket(General.MyPackets.NPCLink("Check my virtue points.", 2));
                            SendPacket(General.MyPackets.NPCLink("Claim Prize.", 3));
                            SendPacket(General.MyPackets.NPCLink("Just passing by.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 1152) //Simon
                        {
                            SendPacket(General.MyPackets.NPCSay("Great rewards will attract many people. I am looking for brave people to help me take my patrimony back. Can you help me? The rewards are handsome."));
                            SendPacket(General.MyPackets.NPCLink("Please tell me more.", 1));
                            SendPacket(General.MyPackets.NPCLink("What rewards?", 2));
                            SendPacket(General.MyPackets.NPCLink("I got SunDiamonds", 3));
                            SendPacket(General.MyPackets.NPCLink("I got MoonDiamonds", 4));
                            SendPacket(General.MyPackets.NPCLink("I got StarDiamonds", 5));
                            SendPacket(General.MyPackets.NPCLink("I got CloudDiamonds", 6));
                            SendPacket(General.MyPackets.NPCLink("I got nothing", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 10002) //Socket Gourd/Garment Quest
                        {
                            SendPacket(General.MyPackets.NPCSay("Hey, Would u like me to make a socket in ur gourd or garment?"));
                            SendPacket(General.MyPackets.NPCSay(" You must prepare 10 DragonBalls and the Gem you want to Socket."));
                            SendPacket(General.MyPackets.NPCSay(" Please make sure you only have ONE gem in your inventory."));
                            SendPacket(General.MyPackets.NPCLink("Socket Garment.", 10));
                            SendPacket(General.MyPackets.NPCLink("Socket Gourd.", 20));
                            SendPacket(General.MyPackets.NPCLink("Change Socket.", 1));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 104800)
                        {
                            SendPacket(General.MyPackets.NPCSay("I can take you to visit the jail, but due to the risk of your health, I will have to charge you 1000 silvers to enter.?"));
                            SendPacket(General.MyPackets.NPCLink("Ok", 1));
                            SendPacket(General.MyPackets.NPCLink("No Thanks", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 104801)
                        {
                            SendPacket(General.MyPackets.NPCSay("Would u like to leave jail"));
                            SendPacket(General.MyPackets.NPCLink("Yes", 1));
                            SendPacket(General.MyPackets.NPCLink("No, just passing by", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 9534)
                        {
                            SendPacket(General.MyPackets.NPCSay("You can change your name in exchange for a Dragonball. Would you like to change your name?"));
                            SendPacket(General.MyPackets.NPCLink("Yes please.", 1));
                            SendPacket(General.MyPackets.NPCLink("No thanks.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 1010)
                        {

                            SendPacket(General.MyPackets.NPCSay("Hi, my name is Lee Zhen and i can socket your equipment. I can make you a first socket into your equipment with 3 ToughDrills and the second with 9 ToughDrills."));
                            SendPacket(General.MyPackets.NPCLink("1st socket making", 1));
                            SendPacket(General.MyPackets.NPCLink("2nd socket making", 2));
                            SendPacket(General.MyPackets.NPCLink("Just passing by.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }

                        if (CurrentNPC == 2)
                        {
                            SendPacket(General.MyPackets.NPCSay("Do you want to dye your armor, headgear or shield?"));
                            SendPacket(General.MyPackets.NPCLink("Armor", 1));
                            SendPacket(General.MyPackets.NPCLink("Headgear", 2));
                            SendPacket(General.MyPackets.NPCLink("Shield", 3));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }

                        if (CurrentNPC == 30015)
                        {
                            SendPacket(General.MyPackets.NPCSay("Our shop is famous for dyeing. If you want to have your equipment dyed please wear the item before you enter. You have a wide choice of colors. One meteor will be charged before you try the colors. Do you want a try?"));
                            SendPacket(General.MyPackets.NPCLink("Yes, here is a meteor.", 1));
                            SendPacket(General.MyPackets.NPCLink("Can you dye my armor black?", 2));
                            SendPacket(General.MyPackets.NPCLink("Just passing by.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }

                        if (CurrentNPC == 12)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hello, I am a taoist trainer. What can i do for you?"));
                            SendPacket(General.MyPackets.NPCLink("I would like to promote myself.", 1));
                            SendPacket(General.MyPackets.NPCLink("I want to learn new skills.", 100));
                            SendPacket(General.MyPackets.NPCLink("Just passing by.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }

                        if (CurrentNPC == 264)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hello, I am the water taoist trainer. What can i do for you?"));
                            SendPacket(General.MyPackets.NPCLink("I would like to promote myself.", 1));
                            SendPacket(General.MyPackets.NPCLink("I want to learn new skills.", 100));
                            SendPacket(General.MyPackets.NPCLink("Just passing by.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }

                        if (CurrentNPC == 17)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hello, I am a trojan trainer. What can i do for you?"));
                            SendPacket(General.MyPackets.NPCLink("I would like to promote myself.", 1));
                            SendPacket(General.MyPackets.NPCLink("I want to learn new skills.", 222));
                            SendPacket(General.MyPackets.NPCLink("Just passing by.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }

                        if (CurrentNPC == 16)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hello, I am a warrior trainer. What can i do for you?"));
                            SendPacket(General.MyPackets.NPCLink("I would like to promote myself.", 1));
                            SendPacket(General.MyPackets.NPCLink("I want to learn new skills.", 2));
                            SendPacket(General.MyPackets.NPCLink("Just passing by.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 6)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hello, I am an archer trainer. What can i do for you?"));
                            SendPacket(General.MyPackets.NPCLink("I would like to promote myself.", 1));
                            SendPacket(General.MyPackets.NPCLink("I want to learn new skills.", 20));
                            SendPacket(General.MyPackets.NPCLink("Just passing by.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }

                        if (CurrentNPC == 44)
                        {
                            SendPacket(General.MyPackets.NPCSay("It is very difficult to own a gemmy weapon. I am famous for my excellent skills. What can i do for you?"));
                            SendPacket(General.MyPackets.NPCLink("Make a socket in my weapon.", 1));
                            SendPacket(General.MyPackets.NPCLink("Just passing by.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }

                        if (CurrentNPC == 21 || CurrentNPC == 181 || CurrentNPC == 187 || CurrentNPC == 183 || CurrentNPC == 184)
                        {
                            SendPacket(General.MyPackets.NPCSay("If you are level 20 or above, you may train in the training ground. Would you like me to teleport you there for 1000 silver."));
                            SendPacket(General.MyPackets.NPCLink("Yes, please.", 1));
                            SendPacket(General.MyPackets.NPCLink("No, thanks.", 2));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }

                        if (CurrentNPC == 103)
                        {
                            SendPacket(General.MyPackets.NPCSay("Where are you heading for? I can teleport you for a price of 100 silver."));
                            SendPacket(General.MyPackets.NPCLink("Phoenix Castle", 1));
                            SendPacket(General.MyPackets.NPCLink("Desert City", 2));
                            SendPacket(General.MyPackets.NPCLink("Ape Mountain", 3));
                            SendPacket(General.MyPackets.NPCLink("Bird Island", 4));
                            SendPacket(General.MyPackets.NPCLink("Mine Cave", 5));
                            SendPacket(General.MyPackets.NPCLink("Market", 6));
                            SendPacket(General.MyPackets.NPCLink("Just passing by.", 7));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 211)
                        {
                            SendPacket(General.MyPackets.NPCSay("Do you want to leave the market? I can teleport you for free."));
                            SendPacket(General.MyPackets.NPCLink("Yeah. Thanks.", 1));
                            SendPacket(General.MyPackets.NPCLink("No, I shall stay here.", 2));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }

                        if (CurrentNPC == 266)
                        {
                            SendPacket(General.MyPackets.NPCSay("Now i can offer three types of hairstyles: New styles, nostalgic styles and special styles. Would you like to cost 500 silvers to make a change?"));
                            SendPacket(General.MyPackets.NPCLink("New styles.", 1));
                            SendPacket(General.MyPackets.NPCLink("Nostalgic styles.", 2));
                            SendPacket(General.MyPackets.NPCLink("Special styles.", 3));
                            SendPacket(General.MyPackets.NPCLink("Keep my current style.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 1250)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hello,I'm The Jail Telporter, i can telport u ,what Do u want"));
                            SendPacket(General.MyPackets.NPCLink("Iwant to go out.", 1));
                            SendPacket(General.MyPackets.NPCLink("Just passing by.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        } 
                        if (CurrentNPC == 104801)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hahaha, had enough already? Thats fine, I can let you out if you wish.  Do u want to be let out?"));
                            SendPacket(General.MyPackets.NPCLink("Yes", 1));
                            SendPacket(General.MyPackets.NPCLink("No", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }

                        if (CurrentNPC == 104802)
                        {
                            SendPacket(General.MyPackets.NPCSay("I can let you enter the area, are you sure you would like to go?"));
                            SendPacket(General.MyPackets.NPCLink("Yes", 1));
                            SendPacket(General.MyPackets.NPCLink("No", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }

                        if (CurrentNPC == 104803)
                        {
                            SendPacket(General.MyPackets.NPCSay("I can exchange a CelestialStone for one of your CleanWaters.  Do you accept?"));
                            SendPacket(General.MyPackets.NPCLink("Yes", 1));
                            SendPacket(General.MyPackets.NPCLink("No", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 104806)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hello, I can take you to the Lab, but due to the health policies, I must charge you 1 DragonBall, do you accept?"));
                            SendPacket(General.MyPackets.NPCLink("Yes", 1));
                            SendPacket(General.MyPackets.NPCLink("No", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }

                        if (CurrentNPC == 104807)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hello, I can take you to the Lab 2, but due to the health policies, I must charge you 1 DragonBall, do you accept?"));
                            SendPacket(General.MyPackets.NPCLink("Yes", 1));
                            SendPacket(General.MyPackets.NPCLink("No", 255));
                            SendPacket(General.MyPackets.NPCLink("I want to go to Twin City", 3));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }

                        if (CurrentNPC == 104808)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hello, I can take you to the Lab 3, but due to the health policies, I must charge you 1 DragonBall, do you accept?"));
                            SendPacket(General.MyPackets.NPCLink("Yes", 1));
                            SendPacket(General.MyPackets.NPCLink("No", 255));
                            SendPacket(General.MyPackets.NPCLink("I want to go to Twin City", 3));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }

                        if (CurrentNPC == 104809)
                        {
                            SendPacket(General.MyPackets.NPCSay("Hello, I can take you to the Lab 4, but due to the health policies, I must charge you 1 DragonBall, do you accept?"));
                            SendPacket(General.MyPackets.NPCLink("Yes", 1));
                            SendPacket(General.MyPackets.NPCLink("No", 255));
                            SendPacket(General.MyPackets.NPCLink("I want to go to Twin City", 3));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 104813)
                        {
                            SendPacket(General.MyPackets.NPCSay("I can exchange 200,000 silvers into 20 cps.  Do you want to trade?"));
                            SendPacket(General.MyPackets.NPCLink("Yes", 1));
                            SendPacket(General.MyPackets.NPCLink("No", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 105016)
                        {
                            SendPacket(General.MyPackets.NPCSay("Do You want start SkyPass quest?"));
                            SendPacket(General.MyPackets.NPCLink("Yes", 1));
                            SendPacket(General.MyPackets.NPCLink("No", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 105005 || CurrentNPC == 105006 || CurrentNPC == 105007 || CurrentNPC == 105008 || CurrentNPC == 105009)
                        {
                            int skynpc = ((CurrentNPC - 105005) + 1);
                            string skynpcstr = "";
                            if (skynpc == 1)
                                skynpcstr = "Do You wana try go to the 2nd Stage?";
                            if (skynpc == 2)
                                skynpcstr = "Do You wana try go to the 3rd Stage?";
                            if (skynpc == 3)
                                skynpcstr = "Do You wana try go to the 4th Stage?";
                            if (skynpc == 4)
                                skynpcstr = "Do You wana try go to the 5th Stage?";
                            if (skynpc == 5)
                                skynpcstr = "Almost done do You wana try finish Quest?";
                            SendPacket(General.MyPackets.NPCSay(skynpcstr));
                            SendPacket(General.MyPackets.NPCLink("Yes", (byte)skynpc));
                            SendPacket(General.MyPackets.NPCLink("No please teleport me to TC", 6));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 105010 || CurrentNPC == 105011 || CurrentNPC == 105012 || CurrentNPC == 105013 || CurrentNPC == 105014)
                        {
                            int skynpc = ((CurrentNPC - 105010) + 1);
                            string skynpcstr = "";
                            if (skynpc == 1 && MyChar.InventoryContains(721100, 1))
                            {
                                {
                                    skynpcstr = "I can let You out if you have PassToken1";
                                    SendPacket(General.MyPackets.NPCSay(skynpcstr));
                                    SendPacket(General.MyPackets.NPCLink("Yay!", (byte)skynpc));
                                }
                            }
                            else if (skynpc == 2 && MyChar.InventoryContains(721101, 1))
                            {
                                skynpcstr = "I can let You out if you have PassToken2";
                                SendPacket(General.MyPackets.NPCSay(skynpcstr));
                                SendPacket(General.MyPackets.NPCLink("Yay!", (byte)skynpc));
                            }
                            else if (skynpc == 3 && MyChar.InventoryContains(721102, 1))
                            {
                                skynpcstr = "I can let You out if you have PassToken3";
                                SendPacket(General.MyPackets.NPCSay(skynpcstr));
                                SendPacket(General.MyPackets.NPCLink("Yay!", (byte)skynpc));
                            }
                            else if (skynpc == 4 && MyChar.InventoryContains(721103, 1))
                            {
                                skynpcstr = "I can let You out if you have PassToken4";
                                SendPacket(General.MyPackets.NPCSay(skynpcstr));
                                SendPacket(General.MyPackets.NPCLink("Yay!", (byte)skynpc));
                            }
                            else if (skynpc == 5 && MyChar.InventoryContains(721108, 1))
                            {
                                skynpcstr = "I can let You out if you have PassTokenL120";
                                SendPacket(General.MyPackets.NPCSay(skynpcstr));
                                SendPacket(General.MyPackets.NPCLink("Yay!", (byte)skynpc));
                            }
                            else
                            {
                                skynpcstr = "Sorry, You dont have PassToken!";
                                SendPacket(General.MyPackets.NPCSay(skynpcstr));
                                SendPacket(General.MyPackets.NPCLink("Damn", 255));
                            }
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 105015)
                        {
                            SendPacket(General.MyPackets.NPCSay("Congratulations! You have finished SkyPass Quest!"));
                            SendPacket(General.MyPackets.NPCSay("Please chose Your revard"));
                            SendPacket(General.MyPackets.NPCLink("4 DBscrolls", 1));
                            SendPacket(General.MyPackets.NPCLink("10 Metscrolls", 2));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }
                        if (CurrentNPC == 104799)
                        {
                            SendPacket(General.MyPackets.NPCSay("So you want to second reborn ?, You need a exemption token but u dont need to use it!"));
                            SendPacket(General.MyPackets.NPCLink("I`ve got the ExemtionToken", 1));
                            SendPacket(General.MyPackets.NPCLink("I dont want to second reborn!.", 255));
                            SendPacket(General.MyPackets.NPCSetFace(30));
                            SendPacket(General.MyPackets.NPCFinish());
                        }

                        MyChar.Ready = true;
                        break;
                    }
                #endregion
                #region guilddls
                case 2032:
                    {
                        MyChar.Ready = false;
                        int Control = (int)Data[10];

                        if (Control == 0)
                        {
                            string Name = "";
                            for (int i = 14; i < 14 + Data[13]; i++)
                            {
                                Name += Convert.ToChar(Data[i]);
                            }

                            uint CharID = 0;
                            byte Pos = 0;

                            foreach (DictionaryEntry DE in MyChar.MyGuild.Members)
                            {
                                string nm = (string)DE.Value;
                                string[] Splitter = nm.Split(':');

                                if (Splitter[0] == Name)
                                {
                                    CharID = uint.Parse(Splitter[1]);
                                    Pos = 50;
                                }
                            }
                            foreach (DictionaryEntry DE in MyChar.MyGuild.DLs)
                            {
                                string dl = (string)DE.Value;
                                string[] Splitter = dl.Split(':');

                                if (Splitter[0] == Name)
                                {
                                    CharID = uint.Parse(Splitter[1]);
                                    Pos = 90;
                                }
                            }
                            if (CharID != 0)
                                MyChar.MyGuild.KickPlayer(CharID, Name, Pos);

                        }
                #endregion
                        #region questmobs
                        if (MyChar.QuestMob == "")
                        {
                            if (CurrentNPC == 280)
                            {
                                if (Control == 1)
                                    MyChar.QuestMob = "Pheasant";
                                if (Control == 2)
                                    MyChar.QuestMob = "Turtledove";
                                if (Control == 3)
                                    MyChar.QuestMob = "Robin";
                                if (Control == 4)
                                    MyChar.QuestMob = "Apparition";
                                if (Control == 5)
                                    MyChar.QuestMob = "Poltergeist";
                            }
                            if (CurrentNPC == 281)
                            {
                                if (Control == 1)
                                    MyChar.QuestMob = "WingedSnake";
                                if (Control == 2)
                                    MyChar.QuestMob = "Bandit";
                                if (Control == 3)
                                    MyChar.QuestMob = "FireRat";
                                if (Control == 4)
                                    MyChar.QuestMob = "FireSpirit";
                            }

                            if (CurrentNPC == 282)
                            {
                                if (Control == 1)
                                    MyChar.QuestMob = "Macaque";
                                if (Control == 2)
                                    MyChar.QuestMob = "GiantApe";
                                if (Control == 3)
                                    MyChar.QuestMob = "ThunderApe";
                                if (Control == 4)
                                    MyChar.QuestMob = "SnakeMan";
                            }
                            if (CurrentNPC == 283)
                            {
                                if (Control == 1)
                                    MyChar.QuestMob = "SandMonster";
                                if (Control == 2)
                                    MyChar.QuestMob = "HillMonster";
                                if (Control == 3)
                                    MyChar.QuestMob = "RockMonster";
                                if (Control == 4)
                                    MyChar.QuestMob = "BladeGhost";
                            }
                            if (CurrentNPC == 284)
                            {
                                if (Control == 1)
                                    MyChar.QuestMob = "BirdMan";
                                if (Control == 2)
                                    MyChar.QuestMob = "HawKing";
                            }
                            if (CurrentNPC == 285)
                            {
                                if (Control == 1)
                                    MyChar.QuestMob = "TombBat";
                                if (Control == 2)
                                    MyChar.QuestMob = "BloodyBat";
                                if (Control == 3)
                                    MyChar.QuestMob = "BullMonster";
                                if (Control == 4)
                                    MyChar.QuestMob = "RedDevilL117";
                            }
                            MyChar.QuestKO = 0;
                        }
                        else
                        {
                            if (Control == 10 && CurrentNPC <= 285 && CurrentNPC >= 280)
                            {
                                MyChar.QuestMob = "";
                                MyChar.QuestKO = 0;
                                SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "You have quit the quest.", 2005));
                            }
                            if (Control == 11 && CurrentNPC <= 285 && CurrentNPC >= 280)
                            {
                                MyChar.QuestMob = "";
                                MyChar.QuestKO = 0;
                                SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "You have finished the quest and got 1 meteor and 1 experience ball.", 2005));
                                MyChar.AddItem("723700-0-0-0-0-0", 0, (uint)General.Rand.Next(99999999));
                                MyChar.AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(99999999));
                            }
                        }
                        #endregion
                        #region npcdo
                        #region Birth Village
                        if (CurrentNPC == 10291)
                        {
                            if (Control == 1)
                            {
                                MyChar.Teleport(1002, 439, 389);
                                SendPacket(General.MyPackets.NPCSay("Good luck in this world"));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                        }
                        if (CurrentNPC == 10297)
                        {
                            if (Control == 1)
                            {
                                MyChar.Teleport(1002, 439, 389);
                                SendPacket(General.MyPackets.NPCSay("Great! Then you may be on your way."));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                        }
                        #endregion
                        if (CurrentNPC == 7666)
                        {
                            if (Control == 1)
                            {
                                World.SendMsgToAll(MyChar.Name + " has entered PKCity, enter and fight him/her.", "SYSTEM", 2011);
                                MyChar.Teleport(1505, 154, 219);
                            }
                        }
                        if (CurrentNPC == 7777)
                        {
                            if (Control == 1)
                            {
                                MyChar.Teleport(1002, 438, 377);
                            }
                        }  
                        if (CurrentNPC == 999999)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("Choose what u want to do!"));
                                SendPacket(General.MyPackets.NPCLink("Upgrade from +12 to +13", 3));
                                SendPacket(General.MyPackets.NPCLink("Upgrade from +13 to +14", 11));
                                SendPacket(General.MyPackets.NPCLink("Upgrade from +14 to +15", 19));
                                SendPacket(General.MyPackets.NPCLink("No thanks!", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 2)
                            {
                                SendPacket(General.MyPackets.NPCSay("You need cps to upgrade you gears!"));
                                SendPacket(General.MyPackets.NPCSay("From +12 to +13 costs 5kk (5.000.000)"));
                                SendPacket(General.MyPackets.NPCSay("From +13 to +14 costs 7kk (7.000.000)"));
                                SendPacket(General.MyPackets.NPCSay("From +14 to +15 costs 9kk (9.000.000)"));
                                SendPacket(General.MyPackets.NPCLink("Ok, Thank you!", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 3)
                            {
                                SendPacket(General.MyPackets.NPCSay("Costs 5000000 Cps. After Plusing Please Take Off The Item And Put It Back On. Which item do you want to +13?"));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my headgear.", 4));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my necklace.", 5));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my armor.", 6));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my weapon.", 7));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my ring.", 8));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my boots.", 9));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my shield.", 10));
                                SendPacket(General.MyPackets.NPCLink("No thanks.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control >= 4 && Control <= 10)
                            {
                                if (MyChar.CPs >= 5000000)
                                {
                                    string TheEquip = "";

                                    if (Control == 4)
                                        TheEquip = MyChar.Equips[1];
                                    if (Control == 5)
                                        TheEquip = MyChar.Equips[2];
                                    if (Control == 6)
                                        TheEquip = MyChar.Equips[3];
                                    if (Control == 7)
                                        TheEquip = MyChar.Equips[4];
                                    if (Control == 8)
                                        TheEquip = MyChar.Equips[6];
                                    if (Control == 9)
                                        TheEquip = MyChar.Equips[8];
                                    if (Control == 10)
                                        TheEquip = MyChar.Equips[5];

                                    byte Pos = 0;

                                    if (Control == 4)
                                        Pos = 1;
                                    if (Control == 5)
                                        Pos = 2;
                                    if (Control == 6)
                                        Pos = 3;
                                    if (Control == 7)
                                        Pos = 4;
                                    if (Control == 8)
                                        Pos = 6;
                                    if (Control == 9)
                                        Pos = 8;
                                    if (Control == 10)
                                        Pos = 5;


                                    string[] Splitter = TheEquip.Split('-');
                                    uint ItemId = uint.Parse(Splitter[0]);
                                    uint NewPlus = 13;
                                    uint OldPlus = uint.Parse(Splitter[1]);
                                    uint ReqPlus = 12;

                                    if (OldPlus == ReqPlus)
                                    {
                                        MyChar.CPs -= 5000000;
                                        MyChar.GetEquipStats(Pos, true);
                                        MyChar.Equips[Pos] = ItemId.ToString() + "-" + NewPlus + "-" + Splitter[2] + "-" + Splitter[3] + "-" + Splitter[4] + "-" + Splitter[5];
                                        MyChar.GetEquipStats(Pos, false);

                                        MyChar.SendEquips(false);
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                        SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[Pos], (int)ItemId, byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), Pos, 100, 100));
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("I'm Sorry Your Item Is Not +12"));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCLink("Ok...", 255));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                            if (Control == 11)
                            {
                                SendPacket(General.MyPackets.NPCSay("Costs 7000000 Cps. After Plusing Please Take Off The Item And Put It Back On. Which item do you want to +14?"));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my headgear.", 12));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my necklace.", 13));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my armor.", 14));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my weapon.", 15));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my ring.", 16));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my boots.", 17));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my shield.", 18));
                                SendPacket(General.MyPackets.NPCLink("No thanks.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control >= 12 && Control <= 18)
                            {
                                if (MyChar.CPs >= 7000000)
                                {
                                    string TheEquip = "";

                                    if (Control == 12)
                                        TheEquip = MyChar.Equips[1];
                                    if (Control == 13)
                                        TheEquip = MyChar.Equips[2];
                                    if (Control == 14)
                                        TheEquip = MyChar.Equips[3];
                                    if (Control == 15)
                                        TheEquip = MyChar.Equips[4];
                                    if (Control == 16)
                                        TheEquip = MyChar.Equips[6];
                                    if (Control == 17)
                                        TheEquip = MyChar.Equips[8];
                                    if (Control == 18)
                                        TheEquip = MyChar.Equips[5];

                                    byte Pos = 0;

                                    if (Control == 12)
                                        Pos = 1;
                                    if (Control == 13)
                                        Pos = 2;
                                    if (Control == 14)
                                        Pos = 3;
                                    if (Control == 15)
                                        Pos = 4;
                                    if (Control == 16)
                                        Pos = 6;
                                    if (Control == 17)
                                        Pos = 8;
                                    if (Control == 18)
                                        Pos = 5;


                                    string[] Splitter = TheEquip.Split('-');
                                    uint ItemId = uint.Parse(Splitter[0]);
                                    uint NewPlus = 14;
                                    uint OldPlus = uint.Parse(Splitter[1]);
                                    uint ReqPlus = 13;

                                    if (OldPlus == ReqPlus)
                                    {
                                        MyChar.CPs -= 7000000;
                                        MyChar.GetEquipStats(Pos, true);
                                        MyChar.Equips[Pos] = ItemId.ToString() + "-" + NewPlus + "-" + Splitter[2] + "-" + Splitter[3] + "-" + Splitter[4] + "-" + Splitter[5];
                                        MyChar.GetEquipStats(Pos, false);

                                        MyChar.SendEquips(false);
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                        SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[Pos], (int)ItemId, byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), Pos, 100, 100));
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("I'm Sorry Your Item Is Not +13"));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCLink("Ok...", 255));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                            if (Control == 19)
                            {
                                SendPacket(General.MyPackets.NPCSay("Costs 9000000 Cps. After Plusing Please Take Off The Item And Put It Back On. Which item do you want to +15?"));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my headgear.", 20));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my necklace.", 21));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my armor.", 22));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my weapon.", 23));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my ring.", 24));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my boots.", 25));
                                SendPacket(General.MyPackets.NPCLink("Upgrade my shield.", 26));
                                SendPacket(General.MyPackets.NPCLink("No thanks.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control >= 20 && Control <= 26)
                            {
                                if (MyChar.CPs >= 9000000)
                                {
                                    string TheEquip = "";

                                    if (Control == 20)
                                        TheEquip = MyChar.Equips[1];
                                    if (Control == 21)
                                        TheEquip = MyChar.Equips[2];
                                    if (Control == 22)
                                        TheEquip = MyChar.Equips[3];
                                    if (Control == 23)
                                        TheEquip = MyChar.Equips[4];
                                    if (Control == 24)
                                        TheEquip = MyChar.Equips[6];
                                    if (Control == 25)
                                        TheEquip = MyChar.Equips[8];
                                    if (Control == 26)
                                        TheEquip = MyChar.Equips[5];

                                    byte Pos = 0;

                                    if (Control == 20)
                                        Pos = 1;
                                    if (Control == 21)
                                        Pos = 2;
                                    if (Control == 22)
                                        Pos = 3;
                                    if (Control == 23)
                                        Pos = 4;
                                    if (Control == 24)
                                        Pos = 6;
                                    if (Control == 25)
                                        Pos = 8;
                                    if (Control == 26)
                                        Pos = 5;


                                    string[] Splitter = TheEquip.Split('-');
                                    uint ItemId = uint.Parse(Splitter[0]);
                                    uint NewPlus = 15;
                                    uint OldPlus = uint.Parse(Splitter[1]);
                                    uint ReqPlus = 14;

                                    if (OldPlus == ReqPlus)
                                    {
                                        MyChar.CPs -= 9000000;
                                        MyChar.GetEquipStats(Pos, true);
                                        MyChar.Equips[Pos] = ItemId.ToString() + "-" + NewPlus + "-" + Splitter[2] + "-" + Splitter[3] + "-" + Splitter[4] + "-" + Splitter[5];
                                        MyChar.GetEquipStats(Pos, false);

                                        MyChar.SendEquips(false);
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                        SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[Pos], (int)ItemId, byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), Pos, 100, 100));
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("I'm Sorry Your Item Is Not +14"));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCLink("Ok...", 255));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                        }
                        if (CurrentNPC == 3050)
                        {
                            string Suggestion = "";
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("What is your suggestion?"));
                                SendPacket(General.MyPackets.NPCLink2("Suggestion:", 2));
                                SendPacket(General.MyPackets.NPCSetFace(0));
                                SendPacket(General.MyPackets.NPCFinish());

                                for (int i = 99; i < 99 + Data[13]; i++)
                                {
                                    Suggestion += Convert.ToChar(Data[i]);
                                }
                                if (Control == 2)
                                {

                                    SendPacket(General.MyPackets.NPCSay("If this suggestion is a joke or fake, you will get punished."));
                                    SendPacket(General.MyPackets.NPCLink("Post", 3));
                                    SendPacket(General.MyPackets.NPCLink("Cancel", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(0));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 3)
                            {
                                System.IO.StreamWriter WriteSuggestion = new System.IO.StreamWriter(System.Windows.Forms.Application.StartupPath + "/Suggestions.txt", true);
                                WriteSuggestion.WriteLine(MyChar.Name + "'s suggestion : " + Suggestion);
                                WriteSuggestion.Flush();

                            }
                        }
                        if (CurrentNPC == 9950)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("If you pay me one dragonball, I can have your size changed. You will become more attractive and start a fresh life. "));
                                SendPacket(General.MyPackets.NPCSay("By the way, to avoid some unexpected things, make sure you are not in any disguise form."));
                                SendPacket(General.MyPackets.NPCLink("Here is a dragonball.", 2));
                                SendPacket(General.MyPackets.NPCLink("I have no dragonball.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 2)
                            {
                                if (MyChar.InventoryContains(1088000, 1))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));

                                    if (MyChar.Model == 1004)
                                        MyChar.Model -= 1;
                                    else if (MyChar.Model == 1003)
                                        MyChar.Model += 1;
                                    if (MyChar.Model == 2002)
                                        MyChar.Model -= 1;
                                    else if (MyChar.Model == 2001)
                                        MyChar.Model += 1;


                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 12, ulong.Parse(MyChar.Avatar.ToString() + MyChar.Model.ToString())));
                                    World.UpdateSpawn(MyChar);

                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have a DragonBall."));
                                    SendPacket(General.MyPackets.NPCLink("OK, I Know.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        } 
                        if (CurrentNPC == 7500)
                        {
                            if (MyChar.InventoryContains(1088000, 1))
                            {

                                MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                MyChar.CPs += 50;
                                SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                            }
                        }
                        if (CurrentNPC == 104799)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.RBCount==1)
                                {
                                    if (MyChar.Level >= 120 && MyChar.InventoryContains(723701, 1) || MyChar.Job == 135 && MyChar.Level >= 110 && MyChar.InventoryContains(723701, 1))
                                    {
                                        if (MyChar.Job == 15 || MyChar.Job == 25 || MyChar.Job == 45 || MyChar.Job == 135 || MyChar.Job == 145)
                                        {
                                            SendPacket(General.MyPackets.NPCSay("While going through the rebirth you can change your class if you'd like."));
                                            SendPacket(General.MyPackets.NPCSay("What class would you like to reborn to?"));
                                            SendPacket(General.MyPackets.NPCLink("Trojan", 2));
                                            SendPacket(General.MyPackets.NPCLink("Warrior", 3));
                                            SendPacket(General.MyPackets.NPCLink("Archer", 4));
                                            SendPacket(General.MyPackets.NPCLink("Water Taoist", 5));
                                            SendPacket(General.MyPackets.NPCLink("Fire Taoist", 6));
                                            SendPacket(General.MyPackets.NPCLink("I changed my mind!", 255));
                                            SendPacket(General.MyPackets.NPCSetFace(30));
                                            SendPacket(General.MyPackets.NPCFinish());
                                        }
                                        else
                                        {
                                            SendPacket(General.MyPackets.NPCSay("You have to promote first!"));
                                            SendPacket(General.MyPackets.NPCLink("Ok, I will go promote!!", 255));
                                            SendPacket(General.MyPackets.NPCSetFace(30));
                                            SendPacket(General.MyPackets.NPCFinish());
                                        }
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("You have to reach level 120 first, before you can second reborn."));
                                        SendPacket(General.MyPackets.NPCLink("Ok, I will go level!", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You aren`t first reborn yet!"));
                                    SendPacket(General.MyPackets.NPCLink("Ok, I will do the first reborn quest!", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 2)
                            {
                                MyChar.ReBorn(11);
                                MyChar.LearnSkill(9876, 0);
                                MyChar.RemoveItem(MyChar.ItemNext(723701));
                            }
                            if (Control == 3)
                            {
                                MyChar.ReBorn(21);
                                MyChar.LearnSkill(9876, 0);
                                MyChar.RemoveItem(MyChar.ItemNext(723701));
                            }
                            if (Control == 4)
                            {
                                MyChar.ReBorn(41);
                                MyChar.LearnSkill(9876, 0);
                                MyChar.RemoveItem(MyChar.ItemNext(723701));
                            }
                            if (Control == 5)
                            {
                                MyChar.ReBorn(132);
                                MyChar.LearnSkill(9876, 0);
                                MyChar.RemoveItem(MyChar.ItemNext(723701));
                            }
                            if (Control == 6)
                            {
                                MyChar.ReBorn(142);
                                MyChar.LearnSkill(9876, 0);
                                MyChar.RemoveItem(MyChar.ItemNext(723701));
                            }
                        }
                        if (CurrentNPC == 6701 || CurrentNPC == 6702)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.MyGuild != null && MyChar.MyGuild == World.PoleHolder)
                                {
                                    SingleNPC Gate = (SingleNPC)NPCs.AllNPCs[(uint)CurrentNPC];
                                    if (Gate.Type == 240 || Gate.Type == 270)
                                        Gate.Type += 10;
                                    else if (Gate.Type == 250 || Gate.Type == 280)
                                        Gate.Type -= 10;
                                    World.NPCSpawns(Gate);
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Only the guild that is dominating the pole can control the gates."));
                                    SendPacket(General.MyPackets.NPCLink("I see", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }
                        if (CurrentNPC == 8487)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("You're inventory will be cleared; Ready?"));
                                SendPacket(General.MyPackets.NPCLink("Yea i think i am!", 6));
                                SendPacket(General.MyPackets.NPCLink("No, i changed my mind.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 6)
                            {
                                foreach (uint uid in MyChar.Inventory_UIDs)
                                {
                                    if (uid != 0)
                                        SendPacket(General.MyPackets.RemoveItem(uid, 0, 3));
                                }
                                MyChar.Inventory_UIDs = new uint[41];
                                MyChar.Inventory = new string[41];
                                MyChar.ItemsInInventory = 0;
                                SendPacket(General.MyPackets.NPCSay("Ok, done!"));
                                SendPacket(General.MyPackets.NPCLink("Thanks...", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                        }
                        if (CurrentNPC == 9812)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("I can pack dragonballs and meteors for you. Give me 10 meteors or 10 dragonballs, I will make them into a MeteorScroll or a DBScroll that occupies only one slot. Just right click on it, it will return into 10 meteors or 10 dragonballs again."));
                                SendPacket(General.MyPackets.NPCSay("As a special offer i can give you 4 meteor scrolls for one of your dragon balls and if you will give me 6 meteor scrolls."));
                                SendPacket(General.MyPackets.NPCLink("Cool. Please pack my meteors.", 2));
                                SendPacket(General.MyPackets.NPCLink("Cool. Please pack my dragonballs.", 3));
                                SendPacket(General.MyPackets.NPCLink("Ok. I'll take 4 meteor scrolls for my dragon ball.", 4));
                                SendPacket(General.MyPackets.NPCLink("Ok. I'll give you 6 meteor scrolls and you give me a dragon ball.", 5));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 2)
                            {
                                if (MyChar.InventoryContains(1088001, 10))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(1088001));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088001));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088001));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088001));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088001));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088001));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088001));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088001));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088001));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088001));
                                    MyChar.AddItem("720027-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                }
                            }
                            if (Control == 3)
                            {
                                if (MyChar.InventoryContains(1088000, 10))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.AddItem("720028-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                }
                            }
                            if (Control == 4)
                            {
                                if (MyChar.InventoryContains(1088000, 1) && MyChar.ItemsInInventory < 37)
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.AddItem("720027-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.AddItem("720027-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.AddItem("720027-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.AddItem("720027-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                }
                            }
                            if (Control == 5)
                            {
                                if (MyChar.InventoryContains(720027, 6))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(720027));
                                    MyChar.RemoveItem(MyChar.ItemNext(720027));
                                    MyChar.RemoveItem(MyChar.ItemNext(720027));
                                    MyChar.RemoveItem(MyChar.ItemNext(720027));
                                    MyChar.RemoveItem(MyChar.ItemNext(720027));
                                    MyChar.RemoveItem(MyChar.ItemNext(720027));
                                    MyChar.AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                }
                            }
                        }
                        if (CurrentNPC == 8410)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.Silvers >= 500000000 && MyChar.Model == 1003) // 500 Million
                                {
                                    MyChar.Silvers -= 500000000;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    MyChar.Rank = 1;
                                    MyChar.Donation += 500000000;
                                    MyChar.SaveRank();
                                    MyChar.SaveDonation();
                                    World.UpdateSpawn(MyChar);
                                    World.SendMsgToAll(" " + MyChar.Name + " is a King now! ", "SYSTEM", 2010);

                                }
                                else if (MyChar.Silvers < 500000000)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You dont have enough silvers to become this rank!"));
                                    SendPacket(General.MyPackets.NPCLink("Oh sorry...", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Model == 2001 || MyChar.Model == 2002)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are a lady, so you must become a Queen!"));
                                    SendPacket(General.MyPackets.NPCLink("Oh sorry...", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 2)
                            {
                                if (MyChar.Silvers >= 500000000 && MyChar.Model == 1005) // 300 Million
                                {
                                    MyChar.Silvers -= 500000000;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    MyChar.Rank = 2;
                                    MyChar.Donation += 500000000;
                                    MyChar.SaveRank();
                                    MyChar.SaveDonation();
                                    World.UpdateSpawn(MyChar);
                                    World.SendMsgToAll(" " + MyChar.Name + " is a Queen now! ", "SYSTEM", 2010);
                                }
                                else if (MyChar.Silvers < 500000000)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You dont have enough silvers to become this rank!"));
                                    SendPacket(General.MyPackets.NPCLink("Oh sorry...", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Model == 1003 || MyChar.Model == 1004)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are a man, so you must become a King!"));
                                    SendPacket(General.MyPackets.NPCLink("Oh sorry...", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 3)
                            {
                                if (MyChar.Silvers >= 70000000) // 70 Million
                                {
                                    MyChar.Silvers -= 70000000;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    MyChar.Rank = 3;
                                    MyChar.Donation += 70000000;
                                    MyChar.SaveRank();
                                    MyChar.SaveDonation();
                                    World.UpdateSpawn(MyChar);
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You dont have enough silvers to become this rank!"));
                                    SendPacket(General.MyPackets.NPCLink("Oh sorry...", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 4)
                            {
                                if (MyChar.Silvers >= 50000000) // 50 Million
                                {
                                    MyChar.Silvers -= 50000000;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    MyChar.Rank = 4;
                                    MyChar.Donation += 50000000;
                                    MyChar.SaveRank();
                                    MyChar.SaveDonation();
                                    World.UpdateSpawn(MyChar);
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You dont have enough silvers to become this rank!"));
                                    SendPacket(General.MyPackets.NPCLink("Oh sorry...", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 5)
                            {
                                if (MyChar.Silvers >= 35000000) // 35 Million
                                {
                                    MyChar.Silvers -= 35000000;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    MyChar.Rank = 5;
                                    MyChar.Donation += 35000000;
                                    MyChar.SaveRank();
                                    MyChar.SaveDonation();
                                    World.UpdateSpawn(MyChar);
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You dont have enough silvers to become this rank!"));
                                    SendPacket(General.MyPackets.NPCLink("Oh sorry...", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 6)
                            {
                                if (MyChar.Silvers >= 15000000) // 15 Million
                                {
                                    MyChar.Silvers -= 15000000;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    MyChar.Rank = 6;
                                    MyChar.Donation += 15000000;
                                    MyChar.SaveRank();
                                    MyChar.SaveDonation();
                                    World.UpdateSpawn(MyChar);
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You dont have enough silvers to become this rank!"));
                                    SendPacket(General.MyPackets.NPCLink("Oh sorry...", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 7)
                            {
                                if (MyChar.Silvers >= 3000000) // 3 Million
                                {
                                    MyChar.Silvers -= 3000000;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    MyChar.Rank = 7;
                                    MyChar.Donation += 3000000;
                                    MyChar.SaveRank();
                                    MyChar.SaveDonation();
                                    World.UpdateSpawn(MyChar);
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You dont have enough silvers to become this rank!"));
                                    SendPacket(General.MyPackets.NPCLink("Oh sorry...", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }
                        if (CurrentNPC == 128)
                        {
                            if (Control == 1)
                            {


                                SendPacket(General.MyPackets.NPCSay("My sister taught me this song, she is very sad that can not be with her boyfriend."));
                                SendPacket(General.MyPackets.NPCLink("People stained and those misfortunes, that's normal", 2));
                                SendPacket(General.MyPackets.NPCLink("Sorry.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 2)
                            {
                                SendPacket(General.MyPackets.NPCSay("My sister was not aware that her boyfriend is a sinverguensa and is waiting for her return, she is very sad, take her this letter please."));
                                SendPacket(General.MyPackets.NPCLink("good...", 3));
                                SendPacket(General.MyPackets.NPCLink("Sorry, is far.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 3)
                            {

                                {
                                    MyChar.AddItem("721000-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));

                                }
                            }
                        }

                        if (CurrentNPC == 129)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("My boyfriend George is gone, I asked many people but I have no news of him. Can you help me find it??"));
                                SendPacket(General.MyPackets.NPCLink("Ok, I will look", 2));
                                SendPacket(General.MyPackets.NPCLink("I am very busy.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }

                            if (Control == 2)
                            {
                                SendPacket(General.MyPackets.NPCSay("Did I get a letter from my sister? Long ago I do not see it."));
                                SendPacket(General.MyPackets.NPCLink("yes.", 3));
                                SendPacket(General.MyPackets.NPCLink("I am leaving my other missions.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 3)
                            {
                                SendPacket(General.MyPackets.NPCSay("The bag that I will give me the gift as Jorge meaning of our love, help me to surrender and tell him what I am waiting."));
                                SendPacket(General.MyPackets.NPCLink("Do not worry you help.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());

                                if (MyChar.InventoryContains(721000, 1))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(721000));
                                    MyChar.AddItem("721001-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                }
                                if (Control == 4)
                                {
                                    SendPacket(General.MyPackets.NPCSay("¿aqui ta tu vrga."));
                                    SendPacket(General.MyPackets.NPCLink("pol fin.", 5));
                                    SendPacket(General.MyPackets.NPCLink("Me voy ", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                if (Control == 5)
                                {
                                    SendPacket(General.MyPackets.NPCSay("La bolsita que te voy a dar me la regalo Jorge como significado de nuestro amor, ayudame a entregarsela y dile que lo estoy esperando."));
                                    SendPacket(General.MyPackets.NPCLink("No te preocupes te ayudaré.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                    if (MyChar.InventoryContains(721002, 1))
                                    {
                                        MyChar.RemoveItem(MyChar.ItemNext(721002));
                                        MyChar.AddItem("1088002-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                }
                            }
                        }
                        if (CurrentNPC == 130)
                        {
                            if (Control == 1)

                                if (MyChar.InventoryContains(721001, 1))
                                {
                                    SendPacket(General.MyPackets.NPCSay("Gracias a veces hace falta agua por aqui, eres muy bueno.¿Hey traes la EstrellaDelAmor?"));
                                    SendPacket(General.MyPackets.NPCLink("Si, lo traigo", 2));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }


                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Quedatelo te podría servir mas adelante."));
                                    SendPacket(General.MyPackets.NPCLink("Como quieras...", 5));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }


                            if (Control == 2)

                                if (MyChar.InventoryContains(1088001, 1))
                                {
                                    SendPacket(General.MyPackets.NPCSay("Nosotros nos amamos y no queria alejarme de ella. Pero mi deseo de viajar por el mundo y buscar justicia nos separó."));
                                    SendPacket(General.MyPackets.NPCLink("Tienes razón", 3));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }


                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Te hace falta un Meteoro."));
                                    SendPacket(General.MyPackets.NPCLink("Voy a buscarlo", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            if (Control == 3)
                            {
                                SendPacket(General.MyPackets.NPCSay("Que bién un Meteoro. Le grabaré su nombre en el para que se lo lleves de mi parte."));
                                SendPacket(General.MyPackets.NPCLink("Ok se lo llevo", 4));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 4)
                            {
                                SendPacket(General.MyPackets.NPCSay("La bolsita que te voy a dar me la regalo Jorge como significado de nuestro amor, ayudame a entregarsela y dile que lo estoy esperando."));
                                SendPacket(General.MyPackets.NPCLink("No te preocupes te ayudaré.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());

                                if (MyChar.InventoryContains(721001, 1))
                                    if (MyChar.InventoryContains(1088001, 1))
                                        if (MyChar.InventoryContains(1000030, 1))
                                        {
                                            MyChar.RemoveItem(MyChar.ItemNext(721001));
                                            MyChar.RemoveItem(MyChar.ItemNext(1088001));
                                            MyChar.RemoveItem(MyChar.ItemNext(1000030));
                                            MyChar.AddItem("721002-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                        }
                                if (Control == 5)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Quedatelo , te podría servir mas adelante."));
                                    SendPacket(General.MyPackets.NPCLink("Ok me lo llevo", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }
                        if (CurrentNPC == 912)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.FCPs == 0)
                                {
                                    MyChar.FCPs += 1;
                                    MyChar.CPs += 10000;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                    MyChar.SaveFCPs();
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You have got some CPS from me. Get Lost"));
                                    SendPacket(General.MyPackets.NPCLink("ok.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }
                        if (CurrentNPC == 390)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("Click on your sweetheart to propose to them"));
                                SendPacket(General.MyPackets.NPCLink("OK", 2));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 2)
                            {
                                if (MyChar.Spouse == "" || MyChar.Spouse == "None")
                                {
                                    MyChar.MyClient.SendPacket(Packets.MarriageMouse(MyChar.UID));
                                }
                            }
                        }
                        if (CurrentNPC == 304)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("Currently, I have an abundant supply of Meteor Tears, but need Meteors. "));
                                SendPacket(General.MyPackets.NPCSay("If you give me one Meteor, I can divorce you from your once-loved one."));
                                SendPacket(General.MyPackets.NPCLink("Here is your Meteor", 2));
                                SendPacket(General.MyPackets.NPCLink("I love my Spouse", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 2)
                            {
                                if (MyChar.InventoryContains(1088001, 1))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(1088001));
                                    SendPacket(General.MyPackets.NPCSay("Okay, you have a meteor. Are you POSITIVE you want to divorce your spouse?"));
                                    SendPacket(General.MyPackets.NPCLink("Yes.", 3));
                                    SendPacket(General.MyPackets.NPCLink("I am not positive...", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You appear to not have a meteor...I cannot divorce you. "));
                                    SendPacket(General.MyPackets.NPCLink("Okay.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 3)
                            {
                                MyChar.RemoveItem(MyChar.ItemNext(1088001));
                                SendPacket(General.MyPackets.String(MyChar.UID, 6, "None"));
                                foreach (DictionaryEntry DE in World.AllChars)
                                {
                                    Character charc = (Character)DE.Value;
                                    if (charc.Name == MyChar.Spouse)
                                    {
                                        charc.MyClient.SendPacket(General.MyPackets.String(charc.UID, 6, "None"));
                                        MyChar.Spouse = "None";
                                    }
                                }
                                InternalDatabase.RemoveSpouse(MyChar.UID);
                                SendPacket(General.MyPackets.NPCSay("I was able to divorce you. Hope you can cope with the loss."));
                                SendPacket(General.MyPackets.NPCLink("Okay.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                        }
                        if (CurrentNPC == 959)
                        {
                            if (Control <= 8 && Control >= 1)
                            {
                                GemId = (byte)Control;
                                SendPacket(General.MyPackets.NPCSay("What quality gem would you like your gems to be composed into?"));
                                SendPacket(General.MyPackets.NPCLink("Refined one.", 9));
                                SendPacket(General.MyPackets.NPCLink("Super one.", 10));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 9)
                            {
                                uint TheGem = (uint)(700001 + (GemId - 1) * 10);

                                if (MyChar.InventoryContains(TheGem, 15))
                                {
                                    for (int i = 0; i < 15; i++)
                                    {
                                        MyChar.RemoveItem(MyChar.ItemNext(TheGem));
                                    }
                                    MyChar.AddItem((TheGem + 1).ToString() + "-0-0-0-0-0", 0, (uint)General.Rand.Next(364573656));
                                }
                            }
                            if (Control == 10)
                            {
                                uint TheGem = (uint)(700002 + (GemId - 1) * 10);

                                if (MyChar.InventoryContains(TheGem, 20))
                                {
                                    for (int i = 0; i < 20; i++)
                                    {
                                        MyChar.RemoveItem(MyChar.ItemNext(TheGem));
                                    }
                                    MyChar.AddItem((TheGem + 1).ToString() + "-0-0-0-0-0", 0, (uint)General.Rand.Next(364573656));
                                }
                            }
                        }
                        if (CurrentNPC == 104801)
                        {
                            if (Control == 1)
                            {
                                MyChar.Teleport(1002, 514, 355);
                            }
                        }

                        if (CurrentNPC == 104802)
                        {
                            if (Control == 1)
                            {
                                MyChar.Teleport(1005, 051, 069);
                            }
                        }

                        if (CurrentNPC == 104803)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.InventoryContains(721258, 1))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(721258));
                                    MyChar.AddItem("721259-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "You do not have a cleanwater", 2000));
                                }
                            }
                        }
                        if (CurrentNPC == 999)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("When the required level of an equipment is above 119. Magic Artisan will be unable to upgrade it's level. So i am here to make it possible. Whatever to be upgraded, I will charge one Dragon Ball."));
                                SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control <= 7 && Control >= 2)
                            {
                                if (!UppAgree)
                                {
                                    SendPacket(General.MyPackets.NPCSay("It requires a Dragon Ball. Are you ready for that?"));
                                    SendPacket(General.MyPackets.NPCLink("Yeah.", (byte)Control));
                                    SendPacket(General.MyPackets.NPCLink("Not yet.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                    UppAgree = true;
                                }
                                else
                                {
                                    byte Pos = 0;

                                    if (Control == 2)
                                        Pos = 4;
                                    if (Control == 3)
                                        Pos = 2;
                                    if (Control == 4)
                                        Pos = 6;
                                    if (Control == 5)
                                        Pos = 8;
                                    if (Control == 6)
                                        Pos = 3;
                                    if (Control == 7)
                                        Pos = 1;

                                    string[] Splitter = MyChar.Equips[Pos].Split('-');
                                    uint ItemId = uint.Parse(Splitter[0]);

                                    if (!Other.Upgradable(ItemId))
                                        return;
                                    if (MyChar.Level >= Other.ItemInfo(Other.EquipNextLevel(ItemId))[3])
                                    {
                                        if (!Other.EquipMaxedLvl(ItemId))
                                        {
                                            if (Other.ItemInfo(ItemId)[3] >= 120)
                                            {
                                                if (MyChar.InventoryContains(1088000, 1))
                                                {
                                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));

                                                    ItemId = Other.EquipNextLevel(ItemId);

                                                    MyChar.GetEquipStats(Pos, true);
                                                    MyChar.Equips[Pos] = ItemId.ToString() + "-" + Splitter[1] + "-" + Splitter[2] + "-" + Splitter[3] + "-" + Splitter[4] + "-" + Splitter[5];
                                                    MyChar.GetEquipStats(Pos, false);

                                                    SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[Pos], (int)ItemId, byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), Pos, 100, 100));

                                                    SendPacket(General.MyPackets.NPCSay("Your item has been upgraded."));
                                                    SendPacket(General.MyPackets.NPCLink("Thanks a lot!", 255));
                                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                                    SendPacket(General.MyPackets.NPCFinish());
                                                }
                                                else
                                                {
                                                    SendPacket(General.MyPackets.NPCSay("You don't have a Dragon Ball."));
                                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                                    SendPacket(General.MyPackets.NPCFinish());
                                                }
                                            }
                                            else
                                            {
                                                SendPacket(General.MyPackets.NPCSay("Your equipment is below level 120. Dont waste a Dragon Ball on that, when you can still upgrade it with meteors."));
                                                SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                                SendPacket(General.MyPackets.NPCSetFace(30));
                                                SendPacket(General.MyPackets.NPCFinish());
                                            }
                                        }
                                        else
                                        {
                                            SendPacket(General.MyPackets.NPCSay("Your equip is on the max level already. I can't help you anymore."));
                                            SendPacket(General.MyPackets.NPCLink("Ok.", 255));
                                            SendPacket(General.MyPackets.NPCSetFace(30));
                                            SendPacket(General.MyPackets.NPCFinish());
                                        }
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("You aren't high level enough to wear the item after upgrading it."));
                                        SendPacket(General.MyPackets.NPCLink("Ok.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                        }
                        if (CurrentNPC == 700)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("Ah, excellent. Upgrading the quality increases the effectiveness of an item. Which item do you want to upgrade?"));
                                SendPacket(General.MyPackets.NPCLink("Upgrade helmet or earring.", 3));
                                SendPacket(General.MyPackets.NPCLink("Upgrade necklace quality.", 4));
                                SendPacket(General.MyPackets.NPCLink("Upgrade armor quality.", 5));
                                SendPacket(General.MyPackets.NPCLink("Upgrade weapon quality.", 6));
                                SendPacket(General.MyPackets.NPCLink("Upgrade ring, heavy ring quality.", 7));
                                SendPacket(General.MyPackets.NPCLink("Upgrade boots quality.", 8));
                                SendPacket(General.MyPackets.NPCLink("Upgrade shield quality.", 9));
                                SendPacket(General.MyPackets.NPCLink("Goodbye.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                                UppAgree = false;
                            }
                            if (Control == 2)
                            {
                                SendPacket(General.MyPackets.NPCSay("I see. Upgrading an item's level makes it more powerful, but also harder to use, increasing the item level prerequisite."));
                                SendPacket(General.MyPackets.NPCLink("Helmet, Earring", 10));
                                SendPacket(General.MyPackets.NPCLink("Upgrade necklace level.", 11));
                                SendPacket(General.MyPackets.NPCLink("Upgrade armor level.", 12));
                                SendPacket(General.MyPackets.NPCLink("Upgrade weapon level.", 13));
                                SendPacket(General.MyPackets.NPCLink("Upgrade ring, heavy ring level.", 14));
                                SendPacket(General.MyPackets.NPCLink("Upgrade boots level.", 15));
                                SendPacket(General.MyPackets.NPCLink("Upgrade shield level.", 16));
                                SendPacket(General.MyPackets.NPCLink("Goodbye.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                                UppAgree = false;
                            }
                            if (Control <= 9 && Control >= 3)
                            {
                                string TheEquip = "";

                                if (Control == 3)
                                    TheEquip = MyChar.Equips[1];
                                if (Control == 4)
                                    TheEquip = MyChar.Equips[2];
                                if (Control == 5)
                                    TheEquip = MyChar.Equips[3];
                                if (Control == 6)
                                    TheEquip = MyChar.Equips[4];
                                if (Control == 7)
                                    TheEquip = MyChar.Equips[6];
                                if (Control == 8)
                                    TheEquip = MyChar.Equips[8];
                                if (Control == 9)
                                    TheEquip = MyChar.Equips[5];

                                byte Pos = 0;

                                if (Control == 3)
                                    Pos = 1;
                                if (Control == 4)
                                    Pos = 2;
                                if (Control == 5)
                                    Pos = 3;
                                if (Control == 6)
                                    Pos = 4;
                                if (Control == 7)
                                    Pos = 6;
                                if (Control == 8)
                                    Pos = 8;
                                if (Control == 9)
                                    Pos = 5;


                                string[] Splitter = TheEquip.Split('-');
                                uint ItemId = uint.Parse(Splitter[0]);

                                if (!Other.Upgradable(ItemId) || Other.ItemQuality(ItemId) == 9)
                                    return;

                                byte RequiredDBs = 0;
                                RequiredDBs = (byte)(Other.ItemInfo(ItemId)[3] / 20);
                                if (RequiredDBs == 0)
                                    RequiredDBs = 1;

                                if (Other.ItemQuality(ItemId) == 6)
                                    RequiredDBs += 2;
                                if (Other.ItemQuality(ItemId) == 7)
                                    RequiredDBs += 3;
                                if (Other.ItemQuality(ItemId) == 8)
                                    RequiredDBs += 4;

                                if (!UppAgree)
                                {
                                    SendPacket(General.MyPackets.NPCSay("It will take " + RequiredDBs + " dragon balls to upgrade it. Do you still want to upgrade?"));
                                    SendPacket(General.MyPackets.NPCLink("Yes", (byte)Control));
                                    SendPacket(General.MyPackets.NPCLink("No, i changed my mind.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else
                                {
                                    if (MyChar.InventoryContains(1088000, RequiredDBs))
                                    {
                                        for (int i = 0; i < RequiredDBs; i++)
                                        {
                                            MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                        }

                                        if (Other.ItemQuality(ItemId) < 6)
                                            ItemId = Other.ItemQualityChange(ItemId, 6);
                                        else
                                            ItemId++;

                                        MyChar.GetEquipStats(Pos, true);
                                        MyChar.Equips[Pos] = ItemId.ToString() + "-" + Splitter[1] + "-" + Splitter[2] + "-" + Splitter[3] + "-" + Splitter[4] + "-" + Splitter[5];
                                        MyChar.GetEquipStats(Pos, false);

                                        SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[Pos], (int)ItemId, byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), Pos, 100, 100));
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("You don't have enough dragon balls."));
                                        SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }

                                UppAgree = true;
                            }
                            if (Control <= 16 && Control >= 10)
                            {
                                string TheEquip = "";

                                if (Control == 10)
                                    TheEquip = MyChar.Equips[1];
                                if (Control == 11)
                                    TheEquip = MyChar.Equips[2];
                                if (Control == 12)
                                    TheEquip = MyChar.Equips[3];
                                if (Control == 13)
                                    TheEquip = MyChar.Equips[4];
                                if (Control == 14)
                                    TheEquip = MyChar.Equips[6];
                                if (Control == 15)
                                    TheEquip = MyChar.Equips[8];
                                if (Control == 16)
                                    TheEquip = MyChar.Equips[5];

                                byte Pos = 0;

                                if (Control == 10)
                                    Pos = 1;
                                if (Control == 11)
                                    Pos = 2;
                                if (Control == 12)
                                    Pos = 3;
                                if (Control == 13)
                                    Pos = 4;
                                if (Control == 14)
                                    Pos = 6;
                                if (Control == 15)
                                    Pos = 8;
                                if (Control == 16)
                                    Pos = 5;

                                string[] Splitter = TheEquip.Split('-');
                                uint ItemId = uint.Parse(Splitter[0]);

                                if (!Other.Upgradable(ItemId))
                                    return;

                                byte RequiredMets = 0;
                                if ((Other.ItemInfo(ItemId)[3] < 120 && Other.ItemType2(ItemId) != 90) ||
                                                        (Other.ItemInfo(ItemId)[3] < 110))
                                {
                                    RequiredMets = (byte)(Other.ItemInfo(ItemId)[3] / 10);
                                    if (RequiredMets == 0)
                                        RequiredMets = 1;
                                }
                                if (RequiredMets != 0)
                                {
                                    if (Other.ItemQuality(ItemId) < 7)
                                        RequiredMets = 2;
                                    if (Other.ItemQuality(ItemId) == 7)
                                        RequiredMets = (byte)(2 + RequiredMets / 5);
                                    if (Other.ItemQuality(ItemId) == 8)
                                        RequiredMets = (byte)(RequiredMets * 2.6);
                                    if (Other.ItemQuality(ItemId) == 9)
                                        RequiredMets = (byte)(RequiredMets * 3.1);
                                }

                                if (RequiredMets != 0)
                                {
                                    if (!UppAgree)
                                    {
                                        SendPacket(General.MyPackets.NPCSay("It will take " + RequiredMets + " meteors to upgrade it. Do you still want to upgrade?"));
                                        SendPacket(General.MyPackets.NPCLink("Yes", (byte)Control));
                                        SendPacket(General.MyPackets.NPCLink("No, i changed my mind.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                    else
                                    {
                                        if (MyChar.InventoryContains(1088001, RequiredMets) && MyChar.Level >= Other.ItemInfo(Other.EquipNextLevel(ItemId))[3])
                                        {
                                            ItemId = Other.EquipNextLevel(ItemId);

                                            for (int i = 0; i < RequiredMets; i++)
                                            {
                                                MyChar.RemoveItem(MyChar.ItemNext(1088001));
                                            }

                                            MyChar.GetEquipStats(Pos, true);
                                            MyChar.Equips[Pos] = ItemId.ToString() + "-" + Splitter[1] + "-" + Splitter[2] + "-" + Splitter[3] + "-" + Splitter[4] + "-" + Splitter[5];
                                            MyChar.GetEquipStats(Pos, false);

                                            SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[Pos], (int)ItemId, byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), Pos, 100, 100));

                                            SendPacket(General.MyPackets.NPCSay("Your item has been upgraded. Look and behold my marvelous upgrading skill! Isn't it amazing?"));
                                            SendPacket(General.MyPackets.NPCLink("Thanks a lot!", 255));
                                            SendPacket(General.MyPackets.NPCSetFace(30));
                                            SendPacket(General.MyPackets.NPCFinish());
                                        }
                                        else
                                        {
                                            SendPacket(General.MyPackets.NPCSay("You don't have enough meteors or you are not able to equip the item after upgrade."));
                                            SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                            SendPacket(General.MyPackets.NPCSetFace(30));
                                            SendPacket(General.MyPackets.NPCFinish());
                                        }

                                        UppAgree = false;
                                    }
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("I cannot upgrade your item anymore. It is on too high level."));
                                    SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                UppAgree = true;
                            }
                        }

                        if (CurrentNPC < 618 && CurrentNPC > 613)
                            if (MyChar.Silvers >= 500)
                            {
                                MyChar.Teleport(1038, 350, 339);
                                MyChar.Silvers -= 500;
                                SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                            }
                        if (CurrentNPC == 610)
                            if (MyChar.Silvers >= 1000)
                                if (ExternalDatabase.GC1Map != 0)
                                {
                                    MyChar.Teleport(ExternalDatabase.GC1Map, (ushort)(ExternalDatabase.GC1X - 2), ExternalDatabase.GC1Y);
                                    MyChar.Silvers -= 1000;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                }
                        if (CurrentNPC == 611)
                            if (MyChar.Silvers >= 1000)
                                if (ExternalDatabase.GC2Map != 0)
                                {
                                    MyChar.Teleport(ExternalDatabase.GC2Map, (ushort)(ExternalDatabase.GC2X - 2), ExternalDatabase.GC2Y);
                                    MyChar.Silvers -= 1000;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                }
                        if (CurrentNPC == 612)
                            if (MyChar.Silvers >= 1000)
                                if (ExternalDatabase.GC3Map != 0)
                                {
                                    MyChar.Teleport(ExternalDatabase.GC3Map, (ushort)(ExternalDatabase.GC3X - 2), ExternalDatabase.GC3Y);
                                    MyChar.Silvers -= 1000;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                }
                        if (CurrentNPC == 613)
                            if (MyChar.Silvers >= 1000)
                                if (ExternalDatabase.GC4Map != 0)
                                {
                                    MyChar.Teleport(ExternalDatabase.GC4Map, (ushort)(ExternalDatabase.GC4X - 2), ExternalDatabase.GC4Y);
                                    MyChar.Silvers -= 1000;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                }
                        if (CurrentNPC == 198)
                            if (Control == 1)
                                MyChar.Teleport(1000, 971, 666);

                        if (CurrentNPC == 573)
                            if (Control == 1)
                                MyChar.Teleport(1028, 160, 96);

                        if (CurrentNPC == 600)
                        {
                            if (Control == 1)
                                MyChar.Teleport(1038, 351, 341);
                            if (Control == 2)
                            {
                                if (MyChar.MyGuild != null && MyChar.MyGuild == World.PoleHolder && MyChar.GuildPosition == 100 && !MyChar.MyGuild.ClaimedPrize && MyChar.ItemsInInventory < 40)
                                {
                                    MyChar.AddItem("720028-0-0-0-0-0", 0, (uint)General.Rand.Next(345636635));
                                    MyChar.MyGuild.ClaimedPrize = true;
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Either the prize has been taken already, your guild has not been victorious the last time or you are not a deputy leader nor guild leader."));
                                    SendPacket(General.MyPackets.NPCLink("Ok.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }

                        if (CurrentNPC == 601)
                            if (Control == 1)
                                MyChar.Teleport(1002, 430, 380);

                        if (CurrentNPC >= 104 && CurrentNPC <= 107)
                        {
                            if (Control == 1)
                            {
                                ushort ToX = 300;
                                ushort ToY = 300;
                                if (CurrentNPC == 107)
                                {
                                    ToX = 1010;
                                    ToY = 710;
                                }
                                if (CurrentNPC == 104)
                                {
                                    ToX = 11;
                                    ToY = 376;
                                }
                                if (CurrentNPC == 105)
                                {
                                    ToX = 381;
                                    ToY = 21;
                                }
                                if (CurrentNPC == 106)
                                {
                                    ToX = 971;
                                    ToY = 666;
                                }
                                MyChar.Teleport(MyChar.LocMap, ToX, ToY);
                            }
                            if (Control == 2)
                                MyChar.Teleport(1036, 211, 196);
                            if (Control == 3)
                                MyChar.Teleport(1000, 84, 328);
                        }
                        if (CurrentNPC == 155)
                        {
                            if (Control == 6)
                            {
                                MyChar.MyGuild.Disband(MyChar);
                            }
                            if (Control == 5)
                            {
                                if (MyChar.GuildPosition == 100)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Are you sure you want to disband your guild?"));
                                    SendPacket(General.MyPackets.NPCLink("Yes.", 6));
                                    SendPacket(General.MyPackets.NPCLink("No, actually.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Only guild leader can disband his/her guild."));
                                    SendPacket(General.MyPackets.NPCLink("Ok.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 3)
                            {
                                SendPacket(General.MyPackets.NPCSay("Enter the name of your guildmate you want to deputize."));
                                SendPacket(General.MyPackets.NPCLink2("Deputize", 4));
                                SendPacket(General.MyPackets.NPCLink("Nevermind.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 4)
                            {
                                if (MyChar.GuildPosition == 100)
                                {
                                    if (MyChar.MyGuild.DLs.Count <= 6)
                                    {
                                        string Name = "";
                                        for (int i = 14; i < 14 + Data[13]; i++)
                                        {
                                            Name += Convert.ToChar(Data[i]);
                                        }

                                        uint CharID = 0;

                                        foreach (DictionaryEntry DE in MyChar.MyGuild.Members)
                                        {
                                            string nm = (string)DE.Value;
                                            string[] Splitter = nm.Split(':');

                                            if (Splitter[0] == Name)
                                                CharID = uint.Parse(Splitter[1]);
                                        }

                                        if (World.AllChars.Contains(CharID))
                                        {
                                            if (MyChar.MyGuild.Members.Contains(CharID))
                                                MyChar.MyGuild.Members.Remove(CharID);
                                            Character Char = (Character)World.AllChars[CharID];
                                            Char.GuildPosition = 90;

                                            MyChar.MyGuild.DLs.Add(CharID, Char.Name + ":" + Char.UID.ToString() + ":" + Char.Level.ToString() + ":" + Char.GuildDonation.ToString());

                                            Char.MyClient.SendPacket(General.MyPackets.GuildInfo(Char.MyGuild, Char));
                                        }
                                        else
                                        {
                                            SendPacket(General.MyPackets.NPCSay("The player you want to deputize must be in your guild and online."));
                                            SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                            SendPacket(General.MyPackets.NPCSetFace(30));
                                            SendPacket(General.MyPackets.NPCFinish());
                                        }
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("A guild can have only 6 deputy leaders."));
                                        SendPacket(General.MyPackets.NPCLink("Ok.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Only guild leaders can deputize, you are not one."));
                                    SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 1)
                            {
                                if (MyChar.MyGuild == null && MyChar.GuildPosition == 0)
                                {
                                    SendPacket(General.MyPackets.NPCSay("It will cost you 1,000,000 silvers, and you need to be level 95 at least."));
                                    SendPacket(General.MyPackets.NPCLink2("Create Guild", 2));
                                    SendPacket(General.MyPackets.NPCLink("Nevermind.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 2)
                            {
                                if (MyChar.MyGuild == null && MyChar.GuildPosition == 0)
                                {
                                    if (MyChar.Silvers >= 1000000)
                                    {
                                        if (MyChar.Level >= 95)
                                        {
                                            string GuildName = "";
                                            for (int i = 14; i < 14 + Data[13]; i++)
                                            {
                                                GuildName += Convert.ToChar(Data[i]);
                                            }

                                            MyChar.Silvers -= 1000000;
                                            SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));

                                            ushort GuildID = (ushort)General.Rand.Next(0, 65000);

                                            InternalDatabase.NewGuild(GuildID, GuildName, MyChar);
                                            Guilds.NewGuild(GuildID, GuildName, MyChar);
                                            MyChar.GuildID = GuildID;
                                            MyChar.GuildPosition = 100;
                                            MyChar.GuildDonation = 1000000;
                                            MyChar.MyGuild = (Guild)Guilds.AllGuilds[GuildID];

                                            SendPacket(General.MyPackets.GuildName(MyChar.GuildID, MyChar.MyGuild.GuildName));
                                            SendPacket(General.MyPackets.GuildInfo(MyChar.MyGuild, MyChar));
                                            SendPacket(General.MyPackets.GuildName(MyChar.GuildID, MyChar.MyGuild.GuildName));
                                        }
                                        else
                                        {
                                            SendPacket(General.MyPackets.NPCSay("You aren't high level enough."));
                                            SendPacket(General.MyPackets.NPCLink("Ok.", 255));
                                            SendPacket(General.MyPackets.NPCSetFace(30));
                                            SendPacket(General.MyPackets.NPCFinish());
                                        }
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("You don't have enough silvers."));
                                        SendPacket(General.MyPackets.NPCLink("Ok.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                        }
                        if (CurrentNPC < 506 && CurrentNPC > 499)
                        {
                            if (Control == 1)
                            {
                                if (Other.ChanceSuccess(25))
                                {
                                    byte Quality = 8;

                                    if (Other.ChanceSuccess(15))
                                        Quality = 9;

                                    byte Plus = 0;

                                    if (Quality == 8)
                                        if (Other.ChanceSuccess(1))
                                            Plus = 8;

                                    byte Soc1 = 0;
                                    byte Soc2 = 0;

                                    if (Plus == 0)
                                        if (Quality == 8)
                                            if (Other.ChanceSuccess(1))
                                            {
                                                Soc1 = 255;
                                                if (Other.ChanceSuccess(0.5))
                                                    Soc2 = 255;
                                            }

                                    byte Enchant = (byte)General.Rand.Next(0, 255);
                                    byte Bless = 0;
                                    if (Other.ChanceSuccess(20))
                                        Bless = (byte)General.Rand.Next(0, 3);

                                    uint EquipID = Other.GenerateEquip((byte)General.Rand.Next(15, 119), Quality);
                                    while (EquipID == 0)
                                    {
                                        EquipID = Other.GenerateEquip((byte)General.Rand.Next(15, 119), Quality);
                                    }

                                    MyChar.AddItem(EquipID.ToString() + "-" + Plus.ToString() + "-" + Bless.ToString() + "-" + Enchant.ToString() + "-" + Soc1.ToString() + "-" + Soc2.ToString(), 0, (uint)General.Rand.Next(263573635));
                                }
                                else if (Other.ChanceSuccess(15))
                                {
                                    MyChar.AddItem(Other.GenerateGarment().ToString() + "-0-0-0-0-0", 0, (uint)General.Rand.Next(263573635));
                                }
                                else if (Other.ChanceSuccess(20))
                                {
                                    uint ID = Other.GenerateEtc();
                                    if (Other.ItemType2(ID) == 73)
                                        MyChar.AddItem(ID.ToString() + "-" + (ID - 730000) + "-0-0-0-0", 0, (uint)General.Rand.Next(263573635));
                                    else
                                        MyChar.AddItem(ID.ToString() + "-0-0-0-0-0", 0, (uint)General.Rand.Next(263573635));
                                }
                                else if (Other.ChanceSuccess(25))
                                {
                                    MyChar.AddItem(Other.GenerateSpecial().ToString() + "-0-0-0-0-0", 0, (uint)General.Rand.Next(263573635));
                                }
                                else
                                {
                                    uint ID = Other.GenerateCrap();
                                    if (ID != 730001)
                                        MyChar.AddItem(ID.ToString() + "-0-0-0-0-0", 0, (uint)General.Rand.Next(263573635));
                                    else
                                        MyChar.AddItem("730001-1-0-0-0-0", 0, (uint)General.Rand.Next(263573635));
                                }


                                MyChar.Teleport(1036, 200, 200);
                            }
                        }

                        if (CurrentNPC == 1846)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.CPs >= 27)
                                {
                                    if (MyChar.ItemsInInventory < 36)
                                    {
                                        MyChar.Teleport(700, 50, 50);
                                        MyChar.CPs -= 27;
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("You must have 5 free spots in your inventory"));
                                        SendPacket(General.MyPackets.NPCLink("Ok, one second", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You don't have enough CPs."));
                                    SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }
                        if (CurrentNPC == 278)//enchant bless and compose
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.ETCPacket(MyChar, 1));
                            }
                            if (Control == 2)
                            {
                                SendPacket(General.MyPackets.NPCSay("Costs 100 Cps. After Enchanting Please Take Off The Item And Put It Back On. Which item do you want to Enchant?"));
                                SendPacket(General.MyPackets.NPCLink("Enchant helmet or earring.", 3));
                                SendPacket(General.MyPackets.NPCLink("Enchant necklace.", 4));
                                SendPacket(General.MyPackets.NPCLink("Enchant armor.", 5));
                                SendPacket(General.MyPackets.NPCLink("Enchant weapon.", 6));
                                SendPacket(General.MyPackets.NPCLink("Enchant ring, heavy ring.", 7));
                                SendPacket(General.MyPackets.NPCLink("Enchant boots.", 8));
                                SendPacket(General.MyPackets.NPCLink("Enchant shield.", 9));
                                SendPacket(General.MyPackets.NPCLink("Goodbye.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control >= 3 && Control <= 9)
                            {
                                if (MyChar.CPs >= 100)
                                {
                                    string TheEquip = "";

                                    if (Control == 3)
                                        TheEquip = MyChar.Equips[1];
                                    if (Control == 4)
                                        TheEquip = MyChar.Equips[2];
                                    if (Control == 5)
                                        TheEquip = MyChar.Equips[3];
                                    if (Control == 6)
                                        TheEquip = MyChar.Equips[4];
                                    if (Control == 7)
                                        TheEquip = MyChar.Equips[6];
                                    if (Control == 8)
                                        TheEquip = MyChar.Equips[8];
                                    if (Control == 9)
                                        TheEquip = MyChar.Equips[5];

                                    byte Pos = 0;

                                    if (Control == 3)
                                        Pos = 1;
                                    if (Control == 4)
                                        Pos = 2;
                                    if (Control == 5)
                                        Pos = 3;
                                    if (Control == 6)
                                        Pos = 4;
                                    if (Control == 7)
                                        Pos = 6;
                                    if (Control == 8)
                                        Pos = 8;
                                    if (Control == 9)
                                        Pos = 5;

                                    MyChar.CPs -= 100;
                                    string[] Splitter = TheEquip.Split('-');
                                    uint ItemId = uint.Parse(Splitter[0]);
                                    uint OldEnchant = uint.Parse(Splitter[3]);
                                    uint NewEnchant = (uint)General.Rand.Next(1, 255);//fix

                                    if (OldEnchant <= NewEnchant)
                                    {

                                        MyChar.GetEquipStats(Pos, true);
                                        MyChar.Equips[Pos] = ItemId.ToString() + "-" + Splitter[1] + "-" + Splitter[2] + "-" + NewEnchant + "-" + Splitter[4] + "-" + Splitter[5];
                                        MyChar.GetEquipStats(Pos, false);

                                        MyChar.SendEquips(false);
                                        SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[Pos], (int)ItemId, byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), Pos, 100, 100));
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("I'm Sorry The New enchantment was smaller than your old one."));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCLink("Ok...", 255));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                            if (Control == 10)
                            {
                                SendPacket(General.MyPackets.NPCSay("Costs 100 Cps. After Blessing Please Take Off The Item And Put It Back On. Which item do you want to Bless?"));
                                SendPacket(General.MyPackets.NPCLink("Bless helmet or earring.", 13));
                                SendPacket(General.MyPackets.NPCLink("Bless necklace.", 14));
                                SendPacket(General.MyPackets.NPCLink("Bless armor.", 15));
                                SendPacket(General.MyPackets.NPCLink("Bless weapon.", 16));
                                SendPacket(General.MyPackets.NPCLink("Bless ring, heavy ring.", 17));
                                SendPacket(General.MyPackets.NPCLink("Bless boots.", 18));
                                SendPacket(General.MyPackets.NPCLink("Bless shield.", 19));
                                SendPacket(General.MyPackets.NPCLink("Goodbye.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control >= 13 && Control <= 19)
                            {
                                if (MyChar.CPs >= 100)
                                {
                                    string TheEquip = "";

                                    if (Control == 13)
                                        TheEquip = MyChar.Equips[1];
                                    if (Control == 14)
                                        TheEquip = MyChar.Equips[2];
                                    if (Control == 15)
                                        TheEquip = MyChar.Equips[3];
                                    if (Control == 16)
                                        TheEquip = MyChar.Equips[4];
                                    if (Control == 17)
                                        TheEquip = MyChar.Equips[6];
                                    if (Control == 18)
                                        TheEquip = MyChar.Equips[8];
                                    if (Control == 19)
                                        TheEquip = MyChar.Equips[5];

                                    byte Pos = 0;

                                    if (Control == 13)
                                        Pos = 1;
                                    if (Control == 14)
                                        Pos = 2;
                                    if (Control == 15)
                                        Pos = 3;
                                    if (Control == 16)
                                        Pos = 4;
                                    if (Control == 17)
                                        Pos = 6;
                                    if (Control == 18)
                                        Pos = 8;
                                    if (Control == 19)
                                        Pos = 5;

                                    MyChar.CPs -= 100;
                                    string[] Splitter = TheEquip.Split('-');
                                    uint ItemId = uint.Parse(Splitter[0]);
                                    uint NewBless = (uint)General.Rand.Next(1, 7);
                                    uint OldBless = uint.Parse(Splitter[2]);

                                    if (NewBless > OldBless)
                                    {

                                        MyChar.GetEquipStats(Pos, true);
                                        MyChar.Equips[Pos] = ItemId.ToString() + "-" + Splitter[1] + "-" + NewBless + "-" + Splitter[3] + "-" + Splitter[4] + "-" + Splitter[5];
                                        MyChar.GetEquipStats(Pos, false);
                                        MyChar.SendEquips(false);

                                        SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[Pos], (int)ItemId, byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), Pos, 100, 100));
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("I'm Sorry The New Bless was smaller than your old one."));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCLink("Ok...", 255));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                        }
                        if (CurrentNPC == 10009)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("Costs 100000 Cps. After Plusing Please Take Off The Item And Put It Back On. Which item do you want to +10?"));
                                SendPacket(General.MyPackets.NPCLink("+10 my helmet or earring.", 3));
                                SendPacket(General.MyPackets.NPCLink("+10 my necklace.", 4));
                                SendPacket(General.MyPackets.NPCLink("+10 my armor.", 5));
                                SendPacket(General.MyPackets.NPCLink("+10 my weapon.", 6));
                                SendPacket(General.MyPackets.NPCLink("+10 my ring, heavy ring.", 7));
                                SendPacket(General.MyPackets.NPCLink("+10 my boots.", 8));
                                SendPacket(General.MyPackets.NPCLink("+10 my shield.", 9));
                                SendPacket(General.MyPackets.NPCLink("Goodbye.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control >= 3 && Control <= 9)
                            {
                                if (MyChar.CPs >= 100000)
                                {
                                    string TheEquip = "";

                                    if (Control == 3)
                                        TheEquip = MyChar.Equips[1];
                                    if (Control == 4)
                                        TheEquip = MyChar.Equips[2];
                                    if (Control == 5)
                                        TheEquip = MyChar.Equips[3];
                                    if (Control == 6)
                                        TheEquip = MyChar.Equips[4];
                                    if (Control == 7)
                                        TheEquip = MyChar.Equips[6];
                                    if (Control == 8)
                                        TheEquip = MyChar.Equips[8];
                                    if (Control == 9)
                                        TheEquip = MyChar.Equips[5];

                                    byte Pos = 0;

                                    if (Control == 3)
                                        Pos = 1;
                                    if (Control == 4)
                                        Pos = 2;
                                    if (Control == 5)
                                        Pos = 3;
                                    if (Control == 6)
                                        Pos = 4;
                                    if (Control == 7)
                                        Pos = 6;
                                    if (Control == 8)
                                        Pos = 8;
                                    if (Control == 9)
                                        Pos = 5;


                                    string[] Splitter = TheEquip.Split('-');
                                    uint ItemId = uint.Parse(Splitter[0]);
                                    uint NewPlus = 10;
                                    uint OldPlus = uint.Parse(Splitter[1]);
                                    uint ReqPlus = 9;

                                    if (OldPlus == ReqPlus)
                                    {
                                        MyChar.CPs -= 100000;
                                        MyChar.GetEquipStats(Pos, true);
                                        MyChar.Equips[Pos] = ItemId.ToString() + "-" + NewPlus + "-" + Splitter[2] + "-" + Splitter[3] + "-" + Splitter[4] + "-" + Splitter[5];
                                        MyChar.GetEquipStats(Pos, false);

                                        MyChar.SendEquips(false);
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                        SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[Pos], (int)ItemId, byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), Pos, 100, 100));
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("I'm Sorry Your Item Is Not +9"));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCLink("Ok...", 255));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                            if (Control == 11)
                            {
                                SendPacket(General.MyPackets.NPCSay("Costs 200000 Cps. After Plusing Please Take Off The Item And Put It Back On. Which item do you want to +11?"));
                                SendPacket(General.MyPackets.NPCLink("+11 my helmet or earring.", 13));
                                SendPacket(General.MyPackets.NPCLink("+11 my necklace.", 14));
                                SendPacket(General.MyPackets.NPCLink("+11 my armor.", 15));
                                SendPacket(General.MyPackets.NPCLink("+11 my weapon.", 16));
                                SendPacket(General.MyPackets.NPCLink("+11 my ring, heavy ring.", 17));
                                SendPacket(General.MyPackets.NPCLink("+11 my boots.", 18));
                                SendPacket(General.MyPackets.NPCLink("+11 my shield.", 19));
                                SendPacket(General.MyPackets.NPCLink("Goodbye.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control >= 13 && Control <= 19)
                            {
                                if (MyChar.CPs >= 200000)
                                {
                                    string TheEquip = "";

                                    if (Control == 13)
                                        TheEquip = MyChar.Equips[1];
                                    if (Control == 14)
                                        TheEquip = MyChar.Equips[2];
                                    if (Control == 15)
                                        TheEquip = MyChar.Equips[3];
                                    if (Control == 16)
                                        TheEquip = MyChar.Equips[4];
                                    if (Control == 17)
                                        TheEquip = MyChar.Equips[6];
                                    if (Control == 18)
                                        TheEquip = MyChar.Equips[8];
                                    if (Control == 19)
                                        TheEquip = MyChar.Equips[5];

                                    byte Pos = 0;

                                    if (Control == 13)
                                        Pos = 1;
                                    if (Control == 14)
                                        Pos = 2;
                                    if (Control == 15)
                                        Pos = 3;
                                    if (Control == 16)
                                        Pos = 4;
                                    if (Control == 7)
                                        Pos = 6;
                                    if (Control == 18)
                                        Pos = 8;
                                    if (Control == 19)
                                        Pos = 5;


                                    string[] Splitter = TheEquip.Split('-');
                                    uint ItemId = uint.Parse(Splitter[0]);
                                    uint NewPlus = 11;
                                    uint OldPlus = uint.Parse(Splitter[1]);
                                    uint ReqPlus = 10;

                                    if (OldPlus == ReqPlus)
                                    {
                                        MyChar.CPs -= 200000;
                                        MyChar.GetEquipStats(Pos, true);
                                        MyChar.Equips[Pos] = ItemId.ToString() + "-" + NewPlus + "-" + Splitter[2] + "-" + Splitter[3] + "-" + Splitter[4] + "-" + Splitter[5];
                                        MyChar.GetEquipStats(Pos, false);

                                        MyChar.SendEquips(false);
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                        SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[Pos], (int)ItemId, byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), Pos, 100, 100));
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("I'm Sorry Your Item Is Not +10"));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCLink("Ok...", 255));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                            if (Control == 21)
                            {
                                SendPacket(General.MyPackets.NPCSay("Costs 300000 Cps. After Plusing Please Take Off The Item And Put It Back On. Which item do you want to +10?"));
                                SendPacket(General.MyPackets.NPCLink("+12 my helmet or earring.", 23));
                                SendPacket(General.MyPackets.NPCLink("+12 my necklace.", 24));
                                SendPacket(General.MyPackets.NPCLink("+12 my armor.", 25));
                                SendPacket(General.MyPackets.NPCLink("+12 my weapon.", 26));
                                SendPacket(General.MyPackets.NPCLink("+12 my ring, heavy ring.", 27));
                                SendPacket(General.MyPackets.NPCLink("+12 my boots.", 28));
                                SendPacket(General.MyPackets.NPCLink("+12 my shield.", 29));
                                SendPacket(General.MyPackets.NPCLink("Goodbye.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control >= 23 && Control <= 29)
                            {
                                if (MyChar.CPs >= 300000)
                                {
                                    string TheEquip = "";

                                    if (Control == 23)
                                        TheEquip = MyChar.Equips[1];
                                    if (Control == 24)
                                        TheEquip = MyChar.Equips[2];
                                    if (Control == 25)
                                        TheEquip = MyChar.Equips[3];
                                    if (Control == 26)
                                        TheEquip = MyChar.Equips[4];
                                    if (Control == 27)
                                        TheEquip = MyChar.Equips[6];
                                    if (Control == 28)
                                        TheEquip = MyChar.Equips[8];
                                    if (Control == 29)
                                        TheEquip = MyChar.Equips[5];

                                    byte Pos = 0;

                                    if (Control == 23)
                                        Pos = 1;
                                    if (Control == 24)
                                        Pos = 2;
                                    if (Control == 25)
                                        Pos = 3;
                                    if (Control == 26)
                                        Pos = 4;
                                    if (Control == 27)
                                        Pos = 6;
                                    if (Control == 28)
                                        Pos = 8;
                                    if (Control == 29)
                                        Pos = 5;


                                    string[] Splitter = TheEquip.Split('-');
                                    uint ItemId = uint.Parse(Splitter[0]);
                                    uint NewPlus = 12;
                                    uint OldPlus = uint.Parse(Splitter[1]);
                                    uint ReqPlus = 11;

                                    if (OldPlus == ReqPlus)
                                    {
                                        MyChar.CPs -= 300000;
                                        MyChar.GetEquipStats(Pos, true);
                                        MyChar.Equips[Pos] = ItemId.ToString() + "-" + NewPlus + "-" + Splitter[2] + "-" + Splitter[3] + "-" + Splitter[4] + "-" + Splitter[5];
                                        MyChar.GetEquipStats(Pos, false);

                                        MyChar.SendEquips(false);
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                        SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[Pos], (int)ItemId, byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), Pos, 100, 100));
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("I'm Sorry Your Item Is Not +11"));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCLink("Ok...", 255));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }

                        }
                        if (CurrentNPC == 80801)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.ItemsInInventory < 40)
                                {
                                    if (MyChar.CPs >= 50)
                                    {
                                        MyChar.CPs -= 50;
                                        MyChar.AddItem("730002-2-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                   
                                        
                                        
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("Sorry, you do not have enough CPS"));
                                        SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                            if (Control == 2)
                            {
                                if (MyChar.ItemsInInventory < 40)
                                {
                                    if (MyChar.CPs >= 4054)
                                    {
                                        MyChar.CPs -= 4054;
                                        MyChar.AddItem("730007-7-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("Sorry, you do not have enough CPS"));
                                        SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                            if (Control == 3)
                            {
                                if (MyChar.ItemsInInventory < 40)
                                {
                                    if (MyChar.CPs >= 8048)
                                    {
                                        MyChar.CPs -= 8048;
                                        MyChar.AddItem("730008-8-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("Sorry, you do not have enough CPS"));
                                        SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                            if (Control == 4)
                            {
                                if (MyChar.ItemsInInventory < 40)
                                {
                                    if (MyChar.CPs >= 500)
                                    {
                                        MyChar.CPs -= 500;
                                        MyChar.AddItem("721258-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("Sorry, you do not have enough CPS"));
                                        SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                            if (Control == 5)
                            {
                                if (MyChar.ItemsInInventory < 33)
                                {
                                    if (MyChar.CPs >= 765)
                                    {
                                        MyChar.CPs -= 765;
                                        MyChar.AddItem("721160-2-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("Sorry, you do not have enough CPS"));
                                        SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                            if (Control == 6)
                            {
                                if (MyChar.ItemsInInventory < 33)
                                {
                                    if (MyChar.CPs >= 8500)
                                    {
                                        MyChar.CPs -= 8500;
                                        MyChar.AddItem("721163-2-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("Sorry, you do not have enough CPS"));
                                        SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                        }
                        if (CurrentNPC == 1234)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.InventoryContains(721537, 1))
                                {
                                    MyChar.Teleport(1352, 028, 223);
                                    MyChar.RemoveItem(MyChar.ItemNext(721537));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You don't have a SkyToken!"));
                                    SendPacket(General.MyPackets.NPCLink("Sigh...", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }
                        if (CurrentNPC == 1147)
                        {
                            if (Control == 1)
                            {
                                MyChar.Teleport(1002, 300, 233);
                            }
                        }
                        if (CurrentNPC == 1153)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.InventoryContains(721538, 1))
                                {
                                    MyChar.Teleport(1353, 028, 268);
                                    MyChar.RemoveItem(MyChar.ItemNext(721538));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You don't have an EarthToken!"));
                                    SendPacket(General.MyPackets.NPCLink("Sigh...", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 2)
                            {
                                MyChar.Teleport(1002, 300, 233);
                            }
                        }
                        if (CurrentNPC == 1154)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.InventoryContains(721539, 1))
                                {
                                    MyChar.Teleport(1354, 009, 290);
                                    MyChar.RemoveItem(MyChar.ItemNext(721539));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You don't have a SoulToken!"));
                                    SendPacket(General.MyPackets.NPCLink("Sigh...", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 2)
                            {
                                MyChar.Teleport(1002, 300, 233);
                            }
                        }
                        if (CurrentNPC == 1152)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("My ancestors built a Labyrinth long before. Many treasures were stored there like SunDiamonds, MoonDiamonds, StarDiamonds, and so on. But it was occupied by fierce monsters soon. They expelled our clansman and kept the treasure."));
                                SendPacket(General.MyPackets.NPCLink("Its a pity.", 7));
                                SendPacket(General.MyPackets.NPCLink("I have no intrest.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 2)
                            {
                                SendPacket(General.MyPackets.NPCSay("2 meteors for 17 SunDiamonds, 4 meteors for 17 MoonDiamonds, a normal gem for 17 StarDiamonds and an AncestorBox for 17 CloudDiamonds. If you are lucky enough, you will get a big suprise from the box."));
                                SendPacket(General.MyPackets.NPCLink("I see. Thank you.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }

                            if (Control == 3)//SunDiamonds 721533
                            {
                                if (MyChar.InventoryContains(721533, 17))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(721533));
                                    MyChar.RemoveItem(MyChar.ItemNext(721533));
                                    MyChar.RemoveItem(MyChar.ItemNext(721533));
                                    MyChar.RemoveItem(MyChar.ItemNext(721533));
                                    MyChar.RemoveItem(MyChar.ItemNext(721533));
                                    MyChar.RemoveItem(MyChar.ItemNext(721533));
                                    MyChar.RemoveItem(MyChar.ItemNext(721533));
                                    MyChar.RemoveItem(MyChar.ItemNext(721533));
                                    MyChar.RemoveItem(MyChar.ItemNext(721533));
                                    MyChar.RemoveItem(MyChar.ItemNext(721533));
                                    MyChar.RemoveItem(MyChar.ItemNext(721533));
                                    MyChar.RemoveItem(MyChar.ItemNext(721533));
                                    MyChar.RemoveItem(MyChar.ItemNext(721533));
                                    MyChar.RemoveItem(MyChar.ItemNext(721533));
                                    MyChar.RemoveItem(MyChar.ItemNext(721533));
                                    MyChar.RemoveItem(MyChar.ItemNext(721533));
                                    MyChar.RemoveItem(MyChar.ItemNext(721533));
                                    MyChar.AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are kidding me. How dare you come here to claim a prize with so few SunDiamonds?"));
                                    SendPacket(General.MyPackets.NPCLink("Wait. I will get more", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 4)//MoonDiamonds 721534
                            {
                                if (MyChar.InventoryContains(721534, 17))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(721534));
                                    MyChar.RemoveItem(MyChar.ItemNext(721534));
                                    MyChar.RemoveItem(MyChar.ItemNext(721534));
                                    MyChar.RemoveItem(MyChar.ItemNext(721534));
                                    MyChar.RemoveItem(MyChar.ItemNext(721534));
                                    MyChar.RemoveItem(MyChar.ItemNext(721534));
                                    MyChar.RemoveItem(MyChar.ItemNext(721534));
                                    MyChar.RemoveItem(MyChar.ItemNext(721534));
                                    MyChar.RemoveItem(MyChar.ItemNext(721534));
                                    MyChar.RemoveItem(MyChar.ItemNext(721534));
                                    MyChar.RemoveItem(MyChar.ItemNext(721534));
                                    MyChar.RemoveItem(MyChar.ItemNext(721534));
                                    MyChar.RemoveItem(MyChar.ItemNext(721534));
                                    MyChar.RemoveItem(MyChar.ItemNext(721534));
                                    MyChar.RemoveItem(MyChar.ItemNext(721534));
                                    MyChar.RemoveItem(MyChar.ItemNext(721534));
                                    MyChar.RemoveItem(MyChar.ItemNext(721534));
                                    MyChar.AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are kidding me. How dare you come here to claim a prize with so few MoonDiamonds?"));
                                    SendPacket(General.MyPackets.NPCLink("Wait. I will get more", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 5)//StarDiamonds 721535
                            {
                                if (MyChar.InventoryContains(721535, 17))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(721535));
                                    MyChar.RemoveItem(MyChar.ItemNext(721535));
                                    MyChar.RemoveItem(MyChar.ItemNext(721535));
                                    MyChar.RemoveItem(MyChar.ItemNext(721535));
                                    MyChar.RemoveItem(MyChar.ItemNext(721535));
                                    MyChar.RemoveItem(MyChar.ItemNext(721535));
                                    MyChar.RemoveItem(MyChar.ItemNext(721535));
                                    MyChar.RemoveItem(MyChar.ItemNext(721535));
                                    MyChar.RemoveItem(MyChar.ItemNext(721535));
                                    MyChar.RemoveItem(MyChar.ItemNext(721535));
                                    MyChar.RemoveItem(MyChar.ItemNext(721535));
                                    MyChar.RemoveItem(MyChar.ItemNext(721535));
                                    MyChar.RemoveItem(MyChar.ItemNext(721535));
                                    MyChar.RemoveItem(MyChar.ItemNext(721535));
                                    MyChar.RemoveItem(MyChar.ItemNext(721535));
                                    MyChar.RemoveItem(MyChar.ItemNext(721535));
                                    MyChar.RemoveItem(MyChar.ItemNext(721535));
                                    MyChar.AddItem("700011-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are kidding me. How dare you come here to claim a prize with so few StarDiamonds?"));
                                    SendPacket(General.MyPackets.NPCLink("Wait. I will get more", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 6)//CloudDiamonds 721536
                            {
                                if (MyChar.InventoryContains(721536, 17))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(721536));
                                    MyChar.RemoveItem(MyChar.ItemNext(721536));
                                    MyChar.RemoveItem(MyChar.ItemNext(721536));
                                    MyChar.RemoveItem(MyChar.ItemNext(721536));
                                    MyChar.RemoveItem(MyChar.ItemNext(721536));
                                    MyChar.RemoveItem(MyChar.ItemNext(721536));
                                    MyChar.RemoveItem(MyChar.ItemNext(721536));
                                    MyChar.RemoveItem(MyChar.ItemNext(721536));
                                    MyChar.RemoveItem(MyChar.ItemNext(721536));
                                    MyChar.RemoveItem(MyChar.ItemNext(721536));
                                    MyChar.RemoveItem(MyChar.ItemNext(721536));
                                    MyChar.RemoveItem(MyChar.ItemNext(721536));
                                    MyChar.RemoveItem(MyChar.ItemNext(721536));
                                    MyChar.RemoveItem(MyChar.ItemNext(721536));
                                    MyChar.RemoveItem(MyChar.ItemNext(721536));
                                    MyChar.RemoveItem(MyChar.ItemNext(721536));
                                    MyChar.RemoveItem(MyChar.ItemNext(721536));
                                    MyChar.AddItem("721540-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are kidding me. How dare you come here to claim a prize with so few CloudDiamonds?"));
                                    SendPacket(General.MyPackets.NPCLink("Wait. I will get more", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }

                            if (Control == 7)
                            {
                                SendPacket(General.MyPackets.NPCSay("I have always been here waiting for brave people to help me. Of course I can't trust in those who do not have at least 2000 virtue points"));
                                SendPacket(General.MyPackets.NPCLink("How about me?", 8));
                                SendPacket(General.MyPackets.NPCLink("Sorry thats too tough for me", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 8)
                            {
                                if (MyChar.VP >= 2000)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Great. You are kind-hearted. I believe you are able to help me out. Let me tell you something about the Labyrinth. Then you can have a good preparation."));
                                    SendPacket(General.MyPackets.NPCLink("Thank you.", 9));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have enough virtue points."));
                                    SendPacket(General.MyPackets.NPCLink("Sigh...", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 9)
                            {
                                SendPacket(General.MyPackets.NPCSay("SkyToken, EarthToken and SoulToken you need to enter the next floor. While the eak monsters usually drop treasure. After you get a token, find a general who will send you to the next floor. Some boss monsters drop rare items."));
                                SendPacket(General.MyPackets.NPCLink("Thanks, enter Lab.", 10));
                                SendPacket(General.MyPackets.NPCLink("I must leave now.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 10)
                            {
                                MyChar.VP -= 2000;
                                MyChar.Teleport(1351, 016, 128);
                            }
                        }
                        if (CurrentNPC == 5555)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("If you are above level 70 and try to power level the newbies (at least 20 levels lower than you), you may gain virtue points."));
                                SendPacket(General.MyPackets.NPCLink("What are virtue points?", 4));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 2)
                            {
                                SendPacket(General.MyPackets.NPCSay("Your current virtue points are " + MyChar.VP + ", please try to gain more."));
                                SendPacket(General.MyPackets.NPCLink("Thanks", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 3)
                            {
                                SendPacket(General.MyPackets.NPCSay("Which prize do you prefer?"));
                                SendPacket(General.MyPackets.NPCLink("Meteor(2,000VPs)", 6));
                                SendPacket(General.MyPackets.NPCLink("ExpBall(5,000VPs)", 7));
                                SendPacket(General.MyPackets.NPCLink("Dragonball(10,000VPs)", 8));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 4)
                            {
                                SendPacket(General.MyPackets.NPCSay("The more newbies you power level, the more virtue points you gain. I shall give you a good reward for a certain amount of virtue points."));
                                SendPacket(General.MyPackets.NPCLink("What prize can I expect?", 5));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 5)
                            {
                                SendPacket(General.MyPackets.NPCSay("I shall reward you an ExpBall for 5,000 virtue points, a Dragonball for 10,000 virtue points, or a Meteor for 2000 virtue points."));
                                SendPacket(General.MyPackets.NPCLink("I see", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 6)
                            {
                                if (MyChar.VP >= 2000)
                                {
                                    MyChar.VP -= 2000;
                                    MyChar.AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You only have " + MyChar.VP + ", you cannot get a Meteor."));
                                    SendPacket(General.MyPackets.NPCLink("Sorry.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 7)
                            {
                                if (MyChar.VP >= 5000)
                                {
                                    MyChar.VP -= 5000;
                                    MyChar.AddItem("723700-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You only have " + MyChar.VP + ", you cannot get an ExpBall."));
                                    SendPacket(General.MyPackets.NPCLink("Sorry.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 8)
                            {
                                if (MyChar.VP >= 10000)
                                {
                                    MyChar.VP -= 10000;
                                    MyChar.AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You only have " + MyChar.VP + ", you cannot get a Dragonball."));
                                    SendPacket(General.MyPackets.NPCLink("Sorry.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }
                        if (CurrentNPC == 10002)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("I will only charge 1 DragonBall to change the socketed Gem."));
                                SendPacket(General.MyPackets.NPCSay(" Change which socket?"));
                                SendPacket(General.MyPackets.NPCLink("Garment.", 2));
                                SendPacket(General.MyPackets.NPCLink("Gourd.", 3));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 2 || Control == 3)
                            {
                                string name = "Item";
                                if (Control == 2)
                                    name = "Garment";
                                if (Control == 3)
                                    name = "Gourd";

                                byte into = 0;

                                if (Control == 10 && MyChar.Equips[9] != null && MyChar.Equips[9] != "0")
                                    into = 9;
                                else if (Control == 20 && MyChar.Equips[7] != null && MyChar.Equips[7] != "0")
                                    into = 7;
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You don't have a " + name + " to socket!"));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                    return;
                                }

                                if (MyChar.InventoryContains(1088000, 1))
                                {
                                    string[] item = MyChar.Equips[into].Split('-');
                                    if (item[4] != "0")
                                        return;

                                    if (MyChar.InventoryContains(700001, 1))
                                        item[4] = "01";
                                    else if (MyChar.InventoryContains(700002, 1))
                                        item[4] = "02";
                                    else if (MyChar.InventoryContains(700003, 1))
                                        item[4] = "03";
                                    else if (MyChar.InventoryContains(700011, 1))
                                        item[4] = "11";
                                    else if (MyChar.InventoryContains(700012, 1))
                                        item[4] = "12";
                                    else if (MyChar.InventoryContains(700013, 1))
                                        item[4] = "13";
                                    else if (MyChar.InventoryContains(700021, 1))
                                        item[4] = "21";
                                    else if (MyChar.InventoryContains(700022, 1))
                                        item[4] = "22";
                                    else if (MyChar.InventoryContains(700023, 1))
                                        item[4] = "23";
                                    else if (MyChar.InventoryContains(700031, 1))
                                        item[4] = "31";
                                    else if (MyChar.InventoryContains(700032, 1))
                                        item[4] = "32";
                                    else if (MyChar.InventoryContains(700033, 1))
                                        item[4] = "33";
                                    else if (MyChar.InventoryContains(700041, 1))
                                        item[4] = "41";
                                    else if (MyChar.InventoryContains(700042, 1))
                                        item[4] = "42";
                                    else if (MyChar.InventoryContains(700043, 1))
                                        item[4] = "43";
                                    else if (MyChar.InventoryContains(700051, 1))
                                        item[4] = "51";
                                    else if (MyChar.InventoryContains(700052, 1))
                                        item[4] = "52";
                                    else if (MyChar.InventoryContains(700053, 1))
                                        item[4] = "53";
                                    else if (MyChar.InventoryContains(700061, 1))
                                        item[4] = "61";
                                    else if (MyChar.InventoryContains(700062, 1))
                                        item[4] = "62";
                                    else if (MyChar.InventoryContains(700063, 1))
                                        item[4] = "63";
                                    else if (MyChar.InventoryContains(700071, 1))
                                        item[4] = "71";
                                    else if (MyChar.InventoryContains(700072, 1))
                                        item[4] = "72";
                                    else if (MyChar.InventoryContains(700073, 1))
                                        item[4] = "73";
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("You don't have a Gem to Socket."));
                                        SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                        return;
                                    }

                                    MyChar.RemoveItem(MyChar.ItemNext(uint.Parse("7000" + item[4])));

                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));

                                    MyChar.Equips[into] = item[0] + "-" + item[1] + "-" + item[2] + "-" + item[3] + "-" + item[4] + "-" + item[5];
                                    SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[into], int.Parse(item[0]), byte.Parse(item[1]), byte.Parse(item[2]), byte.Parse(item[3]), byte.Parse(item[4]), byte.Parse(item[5]), (byte)into, 100, 100));

                                    SendPacket(General.MyPackets.NPCSay("Your " + name + " has been socketed sucessfully!"));
                                    SendPacket(General.MyPackets.NPCLink("Thanks!", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You don't have enough DragonBalls."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 10 || Control == 20)
                            {
                                string name = "Item";
                                if (Control == 10)
                                    name = "Garment";
                                if (Control == 20)
                                    name = "Gourd";

                                byte into = 0;

                                if (Control == 10 && MyChar.Equips[9] != null && MyChar.Equips[9] != "0")
                                    into = 9;
                                else if (Control == 20 && MyChar.Equips[7] != null && MyChar.Equips[7] != "0")
                                    into = 7;
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You don't have a " + name + " to socket!"));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                    return;
                                }

                                if (MyChar.InventoryContains(1088000, 12))
                                {
                                    string[] item = MyChar.Equips[into].Split('-');
                                    if (item[4] != "0")
                                        return;

                                    if (MyChar.InventoryContains(700001, 1))
                                        item[4] = "01";
                                    else if (MyChar.InventoryContains(700002, 1))
                                        item[4] = "02";
                                    else if (MyChar.InventoryContains(700003, 1))
                                        item[4] = "03";
                                    else if (MyChar.InventoryContains(700011, 1))
                                        item[4] = "11";
                                    else if (MyChar.InventoryContains(700012, 1))
                                        item[4] = "12";
                                    else if (MyChar.InventoryContains(700013, 1))
                                        item[4] = "13";
                                    else if (MyChar.InventoryContains(700021, 1))
                                        item[4] = "21";
                                    else if (MyChar.InventoryContains(700022, 1))
                                        item[4] = "22";
                                    else if (MyChar.InventoryContains(700023, 1))
                                        item[4] = "23";
                                    else if (MyChar.InventoryContains(700031, 1))
                                        item[4] = "31";
                                    else if (MyChar.InventoryContains(700032, 1))
                                        item[4] = "32";
                                    else if (MyChar.InventoryContains(700033, 1))
                                        item[4] = "33";
                                    else if (MyChar.InventoryContains(700041, 1))
                                        item[4] = "41";
                                    else if (MyChar.InventoryContains(700042, 1))
                                        item[4] = "42";
                                    else if (MyChar.InventoryContains(700043, 1))
                                        item[4] = "43";
                                    else if (MyChar.InventoryContains(700051, 1))
                                        item[4] = "51";
                                    else if (MyChar.InventoryContains(700052, 1))
                                        item[4] = "52";
                                    else if (MyChar.InventoryContains(700053, 1))
                                        item[4] = "53";
                                    else if (MyChar.InventoryContains(700061, 1))
                                        item[4] = "61";
                                    else if (MyChar.InventoryContains(700062, 1))
                                        item[4] = "62";
                                    else if (MyChar.InventoryContains(700063, 1))
                                        item[4] = "63";
                                    else if (MyChar.InventoryContains(700071, 1))
                                        item[4] = "71";
                                    else if (MyChar.InventoryContains(700072, 1))
                                        item[4] = "72";
                                    else if (MyChar.InventoryContains(700073, 1))
                                        item[4] = "73";
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("You don't have a Gem to Socket."));
                                        SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                        return;
                                    }

                                    MyChar.RemoveItem(MyChar.ItemNext(uint.Parse("7000" + item[4])));

                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));

                                    MyChar.Equips[into] = item[0] + "-" + item[1] + "-" + item[2] + "-" + item[3] + "-" + item[4] + "-" + item[5];
                                    SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[into], int.Parse(item[0]), byte.Parse(item[1]), byte.Parse(item[2]), byte.Parse(item[3]), byte.Parse(item[4]), byte.Parse(item[5]), (byte)into, 100, 100));

                                    SendPacket(General.MyPackets.NPCSay("Your " + name + " has been socketed sucessfully!"));
                                    SendPacket(General.MyPackets.NPCLink("Thanks!", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You don't have enough DragonBalls."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }
                        if (CurrentNPC == 104800)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.Silvers >= 1000)
                                {
                                    MyChar.Silvers -= 1000;
                                    MyChar.Teleport(6000, 027, 073);
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have enough money"));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }
                        if (CurrentNPC == 1250)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("if You Want to Go out,you must be not red name or Black name."));
                                SendPacket(General.MyPackets.NPCSay("if You Want to Gou out,you must have PK point Less than 30."));
                                SendPacket(General.MyPackets.NPCLink("Let me out.", 11));
                                SendPacket(General.MyPackets.NPCLink("What about me??.", 22));
                                SendPacket(General.MyPackets.NPCLink("just Passing by.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());

                            }
                            if (Control == 11)
                            {
                                if (MyChar.PKPoints < 30)
                                {
                                    MyChar.Teleport(1002, 350, 320);
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry,Your Pk points is more 29 u can't out yet."));
                                    SendPacket(General.MyPackets.NPCLink("How I can get out.", 22));
                                    SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                }

                            }
                            if (Control == 22)
                            {
                                if (MyChar.PKPoints < 30)
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are free now ,i will telport you."));
                                    SendPacket(General.MyPackets.NPCLink("Thank you.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                    MyChar.Teleport(1002, 429, 378);
                                }
                                else if (MyChar.PKPoints > 29 && MyChar.PKPoints < 99)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry,You are Red name, you can't out until Your pk points Less than 29."));
                                    SendPacket(General.MyPackets.NPCSay("or You Can Pay 4,000,000 Cash,it will award to your Killer."));
                                    SendPacket(General.MyPackets.NPCLink("Ok, i will pay.", 40));
                                    SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                }
                                else if (MyChar.PKPoints > 100)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry,You are Black name, you can't out until Your pk points Less than 29."));
                                    SendPacket(General.MyPackets.NPCSay("or You Can Pay 54 Cps Cash,it will award to your Killer."));
                                    SendPacket(General.MyPackets.NPCLink("Ok, i will pay.", 54));
                                    SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                }
                            }
                            if (Control == 40)
                            {
                                MyChar.Teleport(1002, 427, 378);
                                MyChar.Silvers -= 4000000;
                                SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));

                                if (MyChar.Silvers > 3999999)
                                {
                                    MyChar.Silvers -= 4000000;
                                    MyChar.Teleport(1002, 427, 378);
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    MyChar.Teleport(1002, 350, 320);
                                    World.SendMsgToAll("The Killer " + MyChar.Name + "has pay for his crimes and donate 4,000,000", " System", 2011);
                                    SendPacket(General.MyPackets.NPCSay("i will take 4,000,000 and teleport you."));
                                    SendPacket(General.MyPackets.NPCSay("the money will go to Your Killer."));
                                    SendPacket(General.MyPackets.NPCLink("Thank you.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());


                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry You don't have Enought Silvers"));
                                    SendPacket(General.MyPackets.NPCLink("damn.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 54)
                            {
                                if (MyChar.CPs > 53)
                                {
                                    MyChar.Teleport(1002, 427, 378);
                                    MyChar.CPs -= 54;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                    World.SendMsgToAll("The Murder \"" + MyChar.Name + "has payfor his crimes and donate 54 Cps", " System", 2011);

                                    SendPacket(General.MyPackets.NPCSay("i will take 54 cps and teleport you."));
                                    SendPacket(General.MyPackets.NPCSay("the money will go to Your Killer."));
                                    SendPacket(General.MyPackets.NPCLink("Thank you.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry You don't have Enought Cps"));
                                    SendPacket(General.MyPackets.NPCLink("damn.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }


                        }
                        if (CurrentNPC == 9534)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.InventoryContains(1088000, 1))
                                {
                                    SendPacket(General.MyPackets.NPCSay("Enter your new name."));
                                    SendPacket(General.MyPackets.NPCLink2("Name:", 2));
                                    SendPacket(General.MyPackets.NPCLink("Cancel.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You don't have a Dragonball."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 2)
                            {
                                string OldName = MyChar.Name;
                                string NewName = "";
                                bool ValidName = true;
                                for (int i = 14; i < 14 + Data[13]; i++)
                                {
                                    NewName += Convert.ToChar(Data[i]);
                                }
                                if (NewName.IndexOfAny(new char[3] { ' ', '[', ']' }) > -1)
                                {
                                    ValidName = false;
                                }

                                foreach (string name in ExternalDatabase.ForbiddenNames)
                                {
                                    if (name == NewName)
                                    {
                                        ValidName = false;
                                        break;
                                    }
                                }

                                try
                                {
                                    if (ValidName)
                                    {
                                        MyChar.RemoveItem(MyChar.ItemNext(1088000));

                                        MyChar.Name = NewName;

                                        World.SendMsgToAll(OldName + " changed his/her name to " + NewName + ".", "SYSTEM", 2005);

                                        SendPacket(General.MyPackets.CharacterInfo(MyChar));
                                        World.SpawnMeToOthers(MyChar, false);
                                        MyChar.SendEquips(false);
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("That name is not valid!"));
                                        SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                                catch (Exception Exc) { General.WriteLine(Exc.ToString()); }


                                break;
                            }
                        }
                        if (CurrentNPC == 1010)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("Choose what do you want the first socket to be created in."));
                                SendPacket(General.MyPackets.NPCLink("Headgear", 3));
                                SendPacket(General.MyPackets.NPCLink("Necklace/ bag", 4));
                                SendPacket(General.MyPackets.NPCLink("Armor/ gown", 5));
                                SendPacket(General.MyPackets.NPCLink("Ring/ bracelet", 6));
                                SendPacket(General.MyPackets.NPCLink("Boots", 7));
                                SendPacket(General.MyPackets.NPCLink("Just passing by.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 2)
                            {
                                SendPacket(General.MyPackets.NPCSay("Choose what do you want the second socket to be created in."));
                                SendPacket(General.MyPackets.NPCLink("Headgear", 8));
                                SendPacket(General.MyPackets.NPCLink("Necklace/ bag", 9));
                                SendPacket(General.MyPackets.NPCLink("Armor/ gown", 10));
                                SendPacket(General.MyPackets.NPCLink("Ring/ bracelet", 11));
                                SendPacket(General.MyPackets.NPCLink("Boots", 12));
                                SendPacket(General.MyPackets.NPCLink("Just passing by.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 3 || Control == 4 || Control == 5 || Control == 6 || Control == 7)
                            {
                                int into = 0;

                                if (Control == 3 && MyChar.Equips[1] != null && MyChar.Equips[1] != "0")
                                    into = 1;
                                else if (Control == 4 && MyChar.Equips[2] != null && MyChar.Equips[2] != "0")
                                    into = 2;
                                else if (Control == 5 && MyChar.Equips[3] != null && MyChar.Equips[3] != "0")
                                    into = 3;
                                else if (Control == 6 && MyChar.Equips[6] != null && MyChar.Equips[6] != "0")
                                    into = 6;
                                else if (Control == 7 && MyChar.Equips[8] != null && MyChar.Equips[8] != "0")
                                    into = 8;
                                else
                                    return;

                                if (MyChar.InventoryContains(1200005, 3))
                                {
                                    string[] item = MyChar.Equips[into].Split('-');
                                    if (item[4] != "0")
                                        return;


                                    MyChar.RemoveItem(MyChar.ItemNext(1200005));
                                    MyChar.RemoveItem(MyChar.ItemNext(1200005));
                                    MyChar.RemoveItem(MyChar.ItemNext(1200005));


                                    item[4] = "255";

                                    MyChar.Equips[into] = item[0] + "-" + item[1] + "-" + item[2] + "-" + item[3] + "-" + item[4] + "-" + item[5];
                                    SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[into], int.Parse(item[0]), byte.Parse(item[1]), byte.Parse(item[2]), byte.Parse(item[3]), byte.Parse(item[4]), byte.Parse(item[5]), (byte)into, 100, 100));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You don't have enough ToughDrills."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }

                            if (Control == 8 || Control == 9 || Control == 10 || Control == 11 || Control == 12)
                            {
                                int into = 0;

                                if (Control == 8 && MyChar.Equips[1] != null && MyChar.Equips[1] != "0")
                                    into = 1;
                                else if (Control == 9 && MyChar.Equips[2] != null && MyChar.Equips[2] != "0")
                                    into = 2;
                                else if (Control == 10 && MyChar.Equips[3] != null && MyChar.Equips[3] != "0")
                                    into = 3;
                                else if (Control == 11 && MyChar.Equips[6] != null && MyChar.Equips[6] != "0")
                                    into = 6;
                                else if (Control == 12 && MyChar.Equips[8] != null && MyChar.Equips[8] != "0")
                                    into = 8;
                                else
                                    return;

                                if (MyChar.InventoryContains(1200005, 9))
                                {
                                    string[] item = MyChar.Equips[into].Split('-');
                                    if (item[5] != "0" || item[4] == "0")
                                        return;

                                    MyChar.RemoveItem(MyChar.ItemNext(1200005));
                                    MyChar.RemoveItem(MyChar.ItemNext(1200005));
                                    MyChar.RemoveItem(MyChar.ItemNext(1200005));
                                    MyChar.RemoveItem(MyChar.ItemNext(1200005));
                                    MyChar.RemoveItem(MyChar.ItemNext(1200005));
                                    MyChar.RemoveItem(MyChar.ItemNext(1200005));
                                    MyChar.RemoveItem(MyChar.ItemNext(1200005));
                                    MyChar.RemoveItem(MyChar.ItemNext(1200005));
                                    MyChar.RemoveItem(MyChar.ItemNext(1200005));

                                    item[5] = "255";

                                    MyChar.Equips[into] = item[0] + "-" + item[1] + "-" + item[2] + "-" + item[3] + "-" + item[4] + "-" + item[5];
                                    SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[into], int.Parse(item[0]), byte.Parse(item[1]), byte.Parse(item[2]), byte.Parse(item[3]), byte.Parse(item[4]), byte.Parse(item[5]), (byte)into, 100, 100));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You don't have enough ToughDrills."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }

                        if (CurrentNPC == 2)
                        {
                            if (Control == 1 || Control == 2 || Control == 3)
                            {
                                ChColor = Control;
                                SendPacket(General.MyPackets.NPCSay("There are seven colors you can choose from. You can try all the colors for no more charges. What color do you like the best?"));
                                SendPacket(General.MyPackets.NPCLink("Orange", 4));
                                SendPacket(General.MyPackets.NPCLink("Light Blue", 5));
                                SendPacket(General.MyPackets.NPCLink("Red", 6));
                                SendPacket(General.MyPackets.NPCLink("Blue", 7));
                                SendPacket(General.MyPackets.NPCLink("Yellow", 8));
                                SendPacket(General.MyPackets.NPCLink("Purple", 9));
                                SendPacket(General.MyPackets.NPCLink("White", 10));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 4 || Control == 5 || Control == 6 || Control == 7 || Control == 8 || Control == 9 || Control == 10)
                            {
                                try
                                {
                                    string[] item;
                                    string[] item2 = null;
                                    if (ChColor == 3)
                                        item2 = MyChar.Equips[5].Split('-');
                                    string newitem;

                                    if (ChColor == 1 && MyChar.Equips[3] != null && MyChar.Equips[3] != "0")
                                        item = MyChar.Equips[3].Split('-');
                                    else if (ChColor == 2 && MyChar.Equips[1] != null && MyChar.Equips[1] != "0")
                                        item = MyChar.Equips[1].Split('-');
                                    else if (ChColor == 3)
                                        if (MyChar.Equips[5] != null && Other.WeaponType(uint.Parse(item2[0])) == 900)
                                            item = MyChar.Equips[5].Split('-');
                                        else
                                        {
                                            SendPacket(General.MyPackets.NPCSay("You don't have a shield equipped."));
                                            SendPacket(General.MyPackets.NPCLink("I'm sorry.", 255));
                                            SendPacket(General.MyPackets.NPCSetFace(30));
                                            SendPacket(General.MyPackets.NPCFinish());
                                            return;
                                        }
                                    else
                                        return;

                                    newitem = item[0];
                                    newitem = newitem.Remove(newitem.Length - 3, 1);

                                    if (Control == 4)
                                        newitem = newitem.Insert(newitem.Length - 2, "3");
                                    if (Control == 5)
                                        newitem = newitem.Insert(newitem.Length - 2, "4");
                                    if (Control == 6)
                                        newitem = newitem.Insert(newitem.Length - 2, "5");
                                    if (Control == 7)
                                        newitem = newitem.Insert(newitem.Length - 2, "6");
                                    if (Control == 8)
                                        newitem = newitem.Insert(newitem.Length - 2, "7");
                                    if (Control == 9)
                                        newitem = newitem.Insert(newitem.Length - 2, "8");
                                    if (Control == 10)
                                        newitem = newitem.Insert(newitem.Length - 2, "9");

                                    if (ChColor == 1)
                                    {
                                        MyChar.Equips[3] = newitem + "-" + item[1] + "-" + item[2] + "-" + item[3] + "-" + item[4] + "-" + item[5];
                                        SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[3], int.Parse(newitem), byte.Parse(item[1]), byte.Parse(item[2]), byte.Parse(item[3]), byte.Parse(item[4]), byte.Parse(item[5]), 3, 100, 100));
                                    }
                                    if (ChColor == 2)
                                    {
                                        MyChar.Equips[1] = newitem + "-" + item[1] + "-" + item[2] + "-" + item[3] + "-" + item[4] + "-" + item[5];
                                        SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[1], int.Parse(newitem), byte.Parse(item[1]), byte.Parse(item[2]), byte.Parse(item[3]), byte.Parse(item[4]), byte.Parse(item[5]), 1, 100, 100));
                                    }
                                    if (ChColor == 3)
                                    {
                                        MyChar.Equips[5] = newitem + "-" + item[1] + "-" + item[2] + "-" + item[3] + "-" + item[4] + "-" + item[5];
                                        SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[5], int.Parse(newitem), byte.Parse(item[1]), byte.Parse(item[2]), byte.Parse(item[3]), byte.Parse(item[4]), byte.Parse(item[5]), 5, 100, 100));
                                    }
                                    ChColor = 0;
                                }
                                catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); }
                            }
                        }

                        if (CurrentNPC == 30015)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.InventoryContains(1088001, 1))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(1088001));
                                    MyChar.Teleport(1008, 22, 26);
                                }
                            }
                            if (Control == 2)
                            {
                                SendPacket(General.MyPackets.NPCSay("Sure. Once your armor is dyed black, it will never fade no matter how it is updated, unless i dye it to other colors, i wil charge 1 dragon ball to do it."));
                                SendPacket(General.MyPackets.NPCLink("Yes, here is a Dragonball.", 3));
                                SendPacket(General.MyPackets.NPCLink("Let me think it over.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 3)
                            {
                                if (MyChar.Equips[3] != null && MyChar.Equips[3] != "0")
                                {
                                    if (MyChar.InventoryContains(1088000, 1))
                                    {
                                        MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                        string[] item = MyChar.Equips[3].Split('-');
                                        string newitem = item[0];
                                        newitem = newitem.Remove(newitem.Length - 3, 1);
                                        newitem = newitem.Insert(newitem.Length - 2, "2");
                                        MyChar.Equips[3] = newitem + "-" + item[1] + "-" + item[2] + "-" + item[3] + "-" + item[4] + "-" + item[5];
                                        SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[3], int.Parse(newitem), byte.Parse(item[1]), byte.Parse(item[2]), byte.Parse(item[3]), byte.Parse(item[4]), byte.Parse(item[5]), 3, 100, 100));
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("You don't have a Dragonball."));
                                        SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                        }
                        if (CurrentNPC == 3825)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.Level >= 40 && MyChar.Level <= 59)
                                {
                                    MyChar.AddExp((ulong)(1295000 + MyChar.Level * 1000000), false);
                                }
                                else if (MyChar.Level >= 60 && MyChar.Level <= 69)
                                {
                                    MyChar.AddExp((ulong)(1295000 + MyChar.Level * 1200000), false);
                                }
                                else if (MyChar.Level >= 70 && MyChar.Level <= 79)
                                {
                                    MyChar.AddExp((ulong)(1295000 + MyChar.Level * 1400000), false);
                                }
                                else if (MyChar.Level >= 80 && MyChar.Level <= 89)
                                {
                                    MyChar.AddExp((ulong)(1295000 + MyChar.Level * 1700000), false);
                                }
                                else if (MyChar.Level >= 90 && MyChar.Level <= 99)
                                {
                                    MyChar.AddExp((ulong)(1295000 + MyChar.Level * 2000000), false);
                                }
                                else if (MyChar.Level >= 100 && MyChar.Level <= 109)
                                {
                                    MyChar.AddExp((ulong)(1295000 + MyChar.Level * 2400000), false);
                                }
                                else if (MyChar.Level >= 110 && MyChar.Level <= 119)
                                {
                                    MyChar.AddExp((ulong)(1295000 + MyChar.Level * 2900000), false);
                                }
                                else if (MyChar.Level >= 120 && MyChar.Level <= 129)
                                {
                                    MyChar.AddExp((ulong)(1295000 + MyChar.Level * 3500000), false);
                                }
                                else if (MyChar.Level >= 130 && MyChar.Level <= 134)
                                {
                                    MyChar.AddExp((ulong)(1295000 + MyChar.Level * 3500000), false);
                                }
                                SendPacket(General.MyPackets.NPCSay("You can draw energy from a Dragonball. Bring a Dragonball to me and I will transfer the energy contained in it to your body. So you can use the dragonball to level up instantly, you will gain exp!"));
                                SendPacket(General.MyPackets.NPCLink("Lets Get Started!", 2));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 2)
                            {
                                if (MyChar.InventoryContains(1088000, 1))
                                {
                                    if (MyChar.Level >= 40 && MyChar.Level <= 59)
                                    {
                                        MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                        MyChar.AddExp((ulong)(1295000 + MyChar.Level * 1000000), false);
                                    }
                                    else if (MyChar.Level >= 60 && MyChar.Level <= 69)
                                    {
                                        MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                        MyChar.AddExp((ulong)(1295000 + MyChar.Level * 1200000), false);
                                    }
                                    else if (MyChar.Level >= 70 && MyChar.Level <= 79)
                                    {
                                        MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                        MyChar.AddExp((ulong)(1295000 + MyChar.Level * 1400000), false);
                                    }
                                    else if (MyChar.Level >= 80 && MyChar.Level <= 89)
                                    {
                                        MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                        MyChar.AddExp((ulong)(1295000 + MyChar.Level * 1700000), false);
                                    }
                                    else if (MyChar.Level >= 90 && MyChar.Level <= 99)
                                    {
                                        MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                        MyChar.AddExp((ulong)(1295000 + MyChar.Level * 2000000), false);
                                    }
                                    else if (MyChar.Level >= 100 && MyChar.Level <= 109)
                                    {
                                        MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                        MyChar.AddExp((ulong)(1295000 + MyChar.Level * 2400000), false);
                                    }
                                    else if (MyChar.Level >= 110 && MyChar.Level <= 119)
                                    {
                                        MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                        MyChar.AddExp((ulong)(1295000 + MyChar.Level * 2900000), false);
                                    }
                                    else if (MyChar.Level >= 120 && MyChar.Level <= 129)
                                    {
                                        MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                        MyChar.AddExp((ulong)(1295000 + MyChar.Level * 3500000), false);
                                    }
                                    else if (MyChar.Level >= 130 && MyChar.Level <= 134)
                                    {
                                        MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                        MyChar.AddExp((ulong)(1295000 + MyChar.Level * 3900000), false);
                                    }
                                    else if (MyChar.Level == 135)
                                    {
                                        SendPacket(General.MyPackets.NPCSay("You have reached the maximum level, you cannot gain anymore exp."));
                                        SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You don't have a Dragonball."));
                                    SendPacket(General.MyPackets.NPCLink("Sorry.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }
                        if (CurrentNPC == 1279)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("Are you really sure you want to enter?"));
                                if (MyChar.Level >= 110)
                                    SendPacket(General.MyPackets.NPCLink("Yeah,im sure", 2));
                                SendPacket(General.MyPackets.NPCLink("No,I changed my mind.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 2)
                            {
                                if (MyChar.Level >= 110)
                                    MyChar.Teleport(2021, 243, 421);

                                MyChar.AddItem("723700-0-0-0-0-0", 0, (uint)General.Rand.Next(99999999));
                                MyChar.AddItem("723700-0-0-0-0-0", 0, (uint)General.Rand.Next(99999999));
                                MyChar.AddItem("723700-0-0-0-0-0", 0, (uint)General.Rand.Next(99999999));
                                MyChar.AddItem("723700-0-0-0-0-0", 0, (uint)General.Rand.Next(99999999));
                            }
                            else if (MyChar.Level <= 110)
                            {
                                SendPacket(General.MyPackets.NPCSay("You are not level 110 yet,go level!"));
                                SendPacket(General.MyPackets.NPCLink("I'm sorry.", 255));
                                SendPacket(General.MyPackets.NPCLink("Terribly sorry!", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                        }



                        if (CurrentNPC == 1280)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("Do you wish to enter stage two?"));
                                SendPacket(General.MyPackets.NPCLink("Yeah,im sure", 2));
                                SendPacket(General.MyPackets.NPCLink("No,I changed my mind.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 2)
                            {
                                if (MyChar.InventoryContains(723085, 5))

                                    MyChar.RemoveItem(MyChar.ItemNext(723085));
                                MyChar.RemoveItem(MyChar.ItemNext(723085));
                                MyChar.RemoveItem(MyChar.ItemNext(723085));
                                MyChar.RemoveItem(MyChar.ItemNext(723085));
                                MyChar.RemoveItem(MyChar.ItemNext(723085));

                                MyChar.Teleport(2022, 241, 338);

                                MyChar.AddItem("723700-0-0-0-0-0", 0, (uint)General.Rand.Next(99999999));
                                MyChar.AddItem("723700-0-0-0-0-0", 0, (uint)General.Rand.Next(99999999));
                                MyChar.AddItem("723700-0-0-0-0-0", 0, (uint)General.Rand.Next(99999999));
                                MyChar.AddItem("723700-0-0-0-0-0", 0, (uint)General.Rand.Next(99999999));

                            }

                        }
                        if (CurrentNPC == 1281)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("Do you wish to enter stage three?"));
                                SendPacket(General.MyPackets.NPCLink("Yeah,im sure", 2));
                                SendPacket(General.MyPackets.NPCLink("No,I changed my mind.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 2)
                            {
                                if (MyChar.QuestKO > 5)
                                {
                                    MyChar.Teleport(2024, 200, 200);
                                    MyChar.QuestKO = 0;
                                    MyChar.QuestMob = "";
                                    SendPacket(General.MyPackets.NPCLink("Lets go!", 3));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());



                                }
                            }
                        }
                        if (CurrentNPC == 1278)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("If you want to rebirth, you should reach a certain level, get the hightest occupation"));
                                SendPacket(General.MyPackets.NPCSay(" title and get a ExemptionToken. After the rebirth, you can distribute your attribute"));
                                SendPacket(General.MyPackets.NPCSay(" more freely. And you can learn more powerful skill."));
                                SendPacket(General.MyPackets.NPCLink("What is ExemptionToken?", 8));
                                SendPacket(General.MyPackets.NPCLink("I would like to Reborn.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 8)
                            {
                                SendPacket(General.MyPackets.NPCSay("ExemptionToken syncrinizes seven Refine gems in the world, and I will pave the way for rebirth."));
                                SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 7)
                            {
                                if (MyChar.InventoryContains(723701, 1))
                                {
                                    SendPacket(General.MyPackets.NPCSay("What class would you like to reborn into?"));
                                    SendPacket(General.MyPackets.NPCLink("Trojan", 2));
                                    SendPacket(General.MyPackets.NPCLink("Warrior", 3));
                                    SendPacket(General.MyPackets.NPCLink("Archer", 4));
                                    SendPacket(General.MyPackets.NPCLink("Fire Taoist", 5));
                                    SendPacket(General.MyPackets.NPCLink("WaterTaoist", 6));
                                    SendPacket(General.MyPackets.NPCLink("Let me think it over.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You cannot reborn if you don't have a celestial stone."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 2)
                            {
                                if (MyChar.InventoryContains(723701, 1))
                                {
                                    if (RBGem == "Dragon")
                                        MyChar.AddItem("700013-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Phoenix")
                                        MyChar.AddItem("700003-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Rainbow")
                                        MyChar.AddItem("700033-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Fury")
                                        MyChar.AddItem("700023-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Kylin")
                                        MyChar.AddItem("700043-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Violet")
                                        MyChar.AddItem("700053-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Moon")
                                        MyChar.AddItem("700063-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Tortoise")
                                        MyChar.AddItem("700073-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.RemoveItem(MyChar.ItemNext(723701));
                                    MyChar.ReBorn(11);
                                }
                            }
                            if (Control == 3)
                            {

                                if (MyChar.InventoryContains(723701, 1))
                                {
                                    if (RBGem == "Dragon")
                                        MyChar.AddItem("700013-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Phoenix")
                                        MyChar.AddItem("700003-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Rainbow")
                                        MyChar.AddItem("700033-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Fury")
                                        MyChar.AddItem("700023-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Kylin")
                                        MyChar.AddItem("700043-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Violet")
                                        MyChar.AddItem("700053-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Moon")
                                        MyChar.AddItem("700063-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Tortoise")
                                        MyChar.AddItem("700073-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.RemoveItem(MyChar.ItemNext(723701));
                                    MyChar.ReBorn(21);
                                }
                            }
                            if (Control == 4)
                            {

                                if (MyChar.InventoryContains(723701, 1))
                                {
                                    if (RBGem == "Dragon")
                                        MyChar.AddItem("700013-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Phoenix")
                                        MyChar.AddItem("700003-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Rainbow")
                                        MyChar.AddItem("700033-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Fury")
                                        MyChar.AddItem("700023-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Kylin")
                                        MyChar.AddItem("700043-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Violet")
                                        MyChar.AddItem("700053-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Moon")
                                        MyChar.AddItem("700063-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Tortoise")
                                        MyChar.AddItem("700073-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.RemoveItem(MyChar.ItemNext(723701));
                                    MyChar.ReBorn(41);
                                }
                            }
                            if (Control == 5)
                            {
                                if (MyChar.InventoryContains(723701, 1))
                                {
                                    if (RBGem == "Dragon")
                                        MyChar.AddItem("700013-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Phoenix")
                                        MyChar.AddItem("700003-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Rainbow")
                                        MyChar.AddItem("700033-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Fury")
                                        MyChar.AddItem("700023-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Kylin")
                                        MyChar.AddItem("700043-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Violet")
                                        MyChar.AddItem("700053-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Moon")
                                        MyChar.AddItem("700063-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Tortoise")
                                        MyChar.AddItem("700073-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.RemoveItem(MyChar.ItemNext(723701));
                                    MyChar.ReBorn(142);
                                }
                            }
                            if (Control == 6)
                            {
                                if (MyChar.InventoryContains(723701, 1))
                                {
                                    if (RBGem == "Dragon")
                                        MyChar.AddItem("700013-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Phoenix")
                                        MyChar.AddItem("700003-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Rainbow")
                                        MyChar.AddItem("700033-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Fury")
                                        MyChar.AddItem("700023-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Kylin")
                                        MyChar.AddItem("700043-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Violet")
                                        MyChar.AddItem("700053-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Moon")
                                        MyChar.AddItem("700063-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Tortoise")
                                        MyChar.AddItem("700073-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.RemoveItem(MyChar.ItemNext(723701));
                                    MyChar.ReBorn(132);
                                }
                            }
                            if (Control == 9)
                            {
                                SendPacket(General.MyPackets.NPCSay("When you reborn, you get a Super Gem! What gem would you like?"));
                                SendPacket(General.MyPackets.NPCLink("Dragon", 10));
                                SendPacket(General.MyPackets.NPCLink("Phoenix", 11));
                                SendPacket(General.MyPackets.NPCLink("Rainbow", 12));
                                SendPacket(General.MyPackets.NPCLink("Moon", 13));
                                SendPacket(General.MyPackets.NPCLink("Violet", 14));
                                SendPacket(General.MyPackets.NPCLink("Kylin", 15));
                                SendPacket(General.MyPackets.NPCLink("Fury", 16));
                                SendPacket(General.MyPackets.NPCLink("Tortoise", 17));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 10)
                            {
                                RBGem = "Dragon";
                                SendPacket(General.MyPackets.NPCSay("You have chosen a " + RBGem + " Gem, is this right?"));
                                SendPacket(General.MyPackets.NPCLink("No.", 9));
                                SendPacket(General.MyPackets.NPCLink("Yes.", 7));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 11)
                            {
                                RBGem = "Phoenix";
                                SendPacket(General.MyPackets.NPCSay("You have chosen a " + RBGem + " Gem, is this right?"));
                                SendPacket(General.MyPackets.NPCLink("No.", 9));
                                SendPacket(General.MyPackets.NPCLink("Yes.", 7));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 12)
                            {
                                RBGem = "Rainbow";
                                SendPacket(General.MyPackets.NPCSay("You have chosen a " + RBGem + " Gem, is this right?"));
                                SendPacket(General.MyPackets.NPCLink("No.", 9));
                                SendPacket(General.MyPackets.NPCLink("Yes.", 7));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 13)
                            {
                                RBGem = "Moon";
                                SendPacket(General.MyPackets.NPCSay("You have chosen a " + RBGem + " Gem, is this right?"));
                                SendPacket(General.MyPackets.NPCLink("No.", 9));
                                SendPacket(General.MyPackets.NPCLink("Yes.", 7));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 14)
                            {
                                RBGem = "Violet";
                                SendPacket(General.MyPackets.NPCSay("You have chosen a " + RBGem + " Gem, is this right?"));
                                SendPacket(General.MyPackets.NPCLink("No.", 9));
                                SendPacket(General.MyPackets.NPCLink("Yes.", 7));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 15)
                            {
                                RBGem = "Kylin";
                                SendPacket(General.MyPackets.NPCSay("You have chosen a " + RBGem + " Gem, is this right?"));
                                SendPacket(General.MyPackets.NPCLink("No.", 9));
                                SendPacket(General.MyPackets.NPCLink("Yes.", 7));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 16)
                            {
                                RBGem = "Fury";
                                SendPacket(General.MyPackets.NPCSay("You have chosen a " + RBGem + " Gem, is this right?"));
                                SendPacket(General.MyPackets.NPCLink("No.", 9));
                                SendPacket(General.MyPackets.NPCLink("Yes.", 7));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 17)
                            {
                                RBGem = "Tortoise";
                                SendPacket(General.MyPackets.NPCSay("You have chosen a " + RBGem + " Gem, is this right?"));
                                SendPacket(General.MyPackets.NPCLink("No.", 9));
                                SendPacket(General.MyPackets.NPCLink("Yes.", 7));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            MyChar.LearnSkill(9876, 0);
                        }
                        if (CurrentNPC == 127)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("If you want to rebirth, you should reach a certain level, get the hightest occupation"));
                                SendPacket(General.MyPackets.NPCSay(" title and get a CelestialStone. After the rebirth, you can distribute your attribute"));
                                SendPacket(General.MyPackets.NPCSay(" more freely. And you can learn more powerful skill."));
                                SendPacket(General.MyPackets.NPCLink("What is CelestialStone?", 8));
                                SendPacket(General.MyPackets.NPCLink("I would like to Reborn.", 9));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 8)
                            {
                                SendPacket(General.MyPackets.NPCSay("CelestialStone syncrinizes seven gems in the world, and I will pave the way for rebirth."));
                                SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 7)
                            {
                                if (MyChar.InventoryContains(721259, 1))
                                {
                                    SendPacket(General.MyPackets.NPCSay("What class would you like to reborn into?"));
                                    SendPacket(General.MyPackets.NPCLink("Trojan", 2));
                                    SendPacket(General.MyPackets.NPCLink("Warrior", 3));
                                    SendPacket(General.MyPackets.NPCLink("Archer", 4));
                                    SendPacket(General.MyPackets.NPCLink("Fire Taoist", 5));
                                    SendPacket(General.MyPackets.NPCLink("WaterTaoist", 6));
                                    SendPacket(General.MyPackets.NPCLink("Let me think it over.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You cannot reborn if you don't have a celestial stone."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 2)
                            {
                                if (MyChar.InventoryContains(721259, 1))
                                {
                                    if (RBGem == "Dragon")
                                        MyChar.AddItem("700013-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Phoenix")
                                        MyChar.AddItem("700003-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Rainbow")
                                        MyChar.AddItem("700033-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Fury")
                                        MyChar.AddItem("700023-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Kylin")
                                        MyChar.AddItem("700043-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Violet")
                                        MyChar.AddItem("700053-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Moon")
                                        MyChar.AddItem("700063-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Tortoise")
                                        MyChar.AddItem("700073-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.RemoveItem(MyChar.ItemNext(721259));
                                    MyChar.ReBorn(11);
                                }
                            }
                            if (Control == 3)
                            {

                                if (MyChar.InventoryContains(721259, 1))
                                {
                                    if (RBGem == "Dragon")
                                        MyChar.AddItem("700013-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Phoenix")
                                        MyChar.AddItem("700003-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Rainbow")
                                        MyChar.AddItem("700033-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Fury")
                                        MyChar.AddItem("700023-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Kylin")
                                        MyChar.AddItem("700043-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Violet")
                                        MyChar.AddItem("700053-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Moon")
                                        MyChar.AddItem("700063-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Tortoise")
                                        MyChar.AddItem("700073-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.RemoveItem(MyChar.ItemNext(721259));
                                    MyChar.ReBorn(21);
                                }
                            }
                            if (Control == 4)
                            {

                                if (MyChar.InventoryContains(721259, 1))
                                {
                                    if (RBGem == "Dragon")
                                        MyChar.AddItem("700013-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Phoenix")
                                        MyChar.AddItem("700003-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Rainbow")
                                        MyChar.AddItem("700033-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Fury")
                                        MyChar.AddItem("700023-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Kylin")
                                        MyChar.AddItem("700043-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Violet")
                                        MyChar.AddItem("700053-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Moon")
                                        MyChar.AddItem("700063-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Tortoise")
                                        MyChar.AddItem("700073-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.RemoveItem(MyChar.ItemNext(721259));
                                    MyChar.ReBorn(41);
                                }
                            }
                            if (Control == 5)
                            {
                                if (MyChar.InventoryContains(721259, 1))
                                {
                                    if (RBGem == "Dragon")
                                        MyChar.AddItem("700013-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Phoenix")
                                        MyChar.AddItem("700003-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Rainbow")
                                        MyChar.AddItem("700033-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Fury")
                                        MyChar.AddItem("700023-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Kylin")
                                        MyChar.AddItem("700043-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Violet")
                                        MyChar.AddItem("700053-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Moon")
                                        MyChar.AddItem("700063-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Tortoise")
                                        MyChar.AddItem("700073-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.RemoveItem(MyChar.ItemNext(721259));
                                    MyChar.ReBorn(142);
                                }
                            }
                            if (Control == 6)
                            {
                                if (MyChar.InventoryContains(721259, 1))
                                {
                                    if (RBGem == "Dragon")
                                        MyChar.AddItem("700013-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Phoenix")
                                        MyChar.AddItem("700003-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Rainbow")
                                        MyChar.AddItem("700033-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Fury")
                                        MyChar.AddItem("700023-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Kylin")
                                        MyChar.AddItem("700043-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Violet")
                                        MyChar.AddItem("700053-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Moon")
                                        MyChar.AddItem("700063-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    if (RBGem == "Tortoise")
                                        MyChar.AddItem("700073-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.RemoveItem(MyChar.ItemNext(721259));
                                    MyChar.ReBorn(132);
                                }
                            }
                            if (Control == 9)
                            {
                                SendPacket(General.MyPackets.NPCSay("When you reborn, you get a Super Gem! What gem would you like?"));
                                SendPacket(General.MyPackets.NPCLink("Dragon", 10));
                                SendPacket(General.MyPackets.NPCLink("Phoenix", 11));
                                SendPacket(General.MyPackets.NPCLink("Rainbow", 12));
                                SendPacket(General.MyPackets.NPCLink("Moon", 13));
                                SendPacket(General.MyPackets.NPCLink("Violet", 14));
                                SendPacket(General.MyPackets.NPCLink("Kylin", 15));
                                SendPacket(General.MyPackets.NPCLink("Fury", 16));
                                SendPacket(General.MyPackets.NPCLink("Tortoise", 17));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 10)
                            {
                                RBGem = "Dragon";
                                SendPacket(General.MyPackets.NPCSay("You have chosen a " + RBGem + " Gem, is this right?"));
                                SendPacket(General.MyPackets.NPCLink("No.", 9));
                                SendPacket(General.MyPackets.NPCLink("Yes.", 7));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 11)
                            {
                                RBGem = "Phoenix";
                                SendPacket(General.MyPackets.NPCSay("You have chosen a " + RBGem + " Gem, is this right?"));
                                SendPacket(General.MyPackets.NPCLink("No.", 9));
                                SendPacket(General.MyPackets.NPCLink("Yes.", 7));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 12)
                            {
                                RBGem = "Rainbow";
                                SendPacket(General.MyPackets.NPCSay("You have chosen a " + RBGem + " Gem, is this right?"));
                                SendPacket(General.MyPackets.NPCLink("No.", 9));
                                SendPacket(General.MyPackets.NPCLink("Yes.", 7));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 13)
                            {
                                RBGem = "Moon";
                                SendPacket(General.MyPackets.NPCSay("You have chosen a " + RBGem + " Gem, is this right?"));
                                SendPacket(General.MyPackets.NPCLink("No.", 9));
                                SendPacket(General.MyPackets.NPCLink("Yes.", 7));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 14)
                            {
                                RBGem = "Violet";
                                SendPacket(General.MyPackets.NPCSay("You have chosen a " + RBGem + " Gem, is this right?"));
                                SendPacket(General.MyPackets.NPCLink("No.", 9));
                                SendPacket(General.MyPackets.NPCLink("Yes.", 7));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 15)
                            {
                                RBGem = "Kylin";
                                SendPacket(General.MyPackets.NPCSay("You have chosen a " + RBGem + " Gem, is this right?"));
                                SendPacket(General.MyPackets.NPCLink("No.", 9));
                                SendPacket(General.MyPackets.NPCLink("Yes.", 7));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 16)
                            {
                                RBGem = "Fury";
                                SendPacket(General.MyPackets.NPCSay("You have chosen a " + RBGem + " Gem, is this right?"));
                                SendPacket(General.MyPackets.NPCLink("No.", 9));
                                SendPacket(General.MyPackets.NPCLink("Yes.", 7));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 17)
                            {
                                RBGem = "Tortoise";
                                SendPacket(General.MyPackets.NPCSay("You have chosen a " + RBGem + " Gem, is this right?"));
                                SendPacket(General.MyPackets.NPCLink("No.", 9));
                                SendPacket(General.MyPackets.NPCLink("Yes.", 7));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                        }
                        if (CurrentNPC == 1281)
                        {
                            if (Control == 3)
                                MyChar.QuestMob = "DisBoss";
                        }
                        if (CurrentNPC == 1280)
                        {
                            if (Control == 2)
                                MyChar.QuestMob = "Phantoms";
                        }

                        #region FireTaoTrainer
                        if (CurrentNPC == 12)
                        {
                            if (Control == 100)
                            {
                                if (MyChar.Job < 146 && MyChar.Job > 139)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Here are the skills i can teach you, make sure you have the required level."));
                                    SendPacket(General.MyPackets.NPCLink("Fire Circle: Lv 65", 101));
                                    SendPacket(General.MyPackets.NPCLink("Fire Ring: Lv 80", 102));
                                    SendPacket(General.MyPackets.NPCLink("Fire Meteor: Lv 40", 103));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are not a fire taoist, go away!"));
                                    SendPacket(General.MyPackets.NPCLink("I'm sorry.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (MyChar.Job < 146 && MyChar.Job > 139)
                            {
                                if (Control == 250)
                                {
                                    if (MyChar.Level >= 70)
                                        if (MyChar.Job < 136 && MyChar.Job > 129)
                                        {
                                            MyChar.LearnSkill(1120, 0);
                                        }
                                }
                                if (Control == 101)
                                {
                                    if (MyChar.Level >= 65)
                                    {
                                        MyChar.LearnSkill(1120, 0);
                                    }
                                }
                                if (Control == 102)
                                {
                                    if (MyChar.Level >= 80)
                                    {
                                        MyChar.LearnSkill(1150, 0);
                                    }
                                }
                                if (Control == 103)
                                {
                                    if (MyChar.Level >= 40)
                                    {
                                        MyChar.LearnSkill(1180, 0);
                                    }
                                }
                            }
                            if (Control == 1)
                            {
                                if (MyChar.Job == 100)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Taoist you need to be level 15 or higher."));
                                }
                                if (MyChar.Job == 101)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Water Taoist or Fire Taoist you need to be level 40 or higher. What job will you choose?"));
                                }
                                if (MyChar.Job == 132)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Water Wizard you need to be level 70 or higher."));
                                }
                                if (MyChar.Job == 133)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Water Master you need to be level 100 or higher."));
                                }
                                if (MyChar.Job == 134)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Water Saint you need to be level 110 or higher."));
                                }
                                if (MyChar.Job == 142)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Fire Wizard you need to be level 70 or higher."));
                                }
                                if (MyChar.Job == 143)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Fire Master you need to be level 100 or higher."));
                                }
                                if (MyChar.Job == 144)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Fire Saint you need to be level 110 or higher."));
                                }
                                if (MyChar.Job != 135 && MyChar.Job != 145 && MyChar.Job != 101)
                                {
                                    if (MyChar.Job < 136 && MyChar.Job > 131)
                                        SendPacket(General.MyPackets.NPCLink("I want to promote.", 2));
                                    else
                                        SendPacket(General.MyPackets.NPCLink("I want to promote.", 3));
                                    SendPacket(General.MyPackets.NPCLink("I think i am not qualified for that yet.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Job == 101)
                                {
                                    SendPacket(General.MyPackets.NPCLink("Water Taoist", 2));
                                    SendPacket(General.MyPackets.NPCLink("Fire Taoist", 3));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You cannot promote anymore, i can't help you."));
                                    SendPacket(General.MyPackets.NPCLink("Sorry.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 2)
                            {
                                bool Promoted = false;

                                if (MyChar.Job == 100 && MyChar.Level >= 15)
                                {
                                    MyChar.Job = 101;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 101 && MyChar.Level >= 40)
                                {
                                    MyChar.Job = 132;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 132 && MyChar.Level >= 70)
                                {
                                    MyChar.Job = 133;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 133 && MyChar.Level >= 100)
                                {
                                    if (MyChar.RBCount == 1)
                                    {
                                        MyChar.AddItem("134287-0-0-0-31-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 2)
                                    {
                                        MyChar.AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    MyChar.Job = 134;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 134 && MyChar.Level >= 110)
                                {
                                    if (MyChar.RBCount == 0)
                                    {
                                        MyChar.AddItem("700032-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 1)
                                    {
                                        MyChar.AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 2)
                                    {
                                        MyChar.AddItem("160199-0-0-0-33-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    MyChar.Job = 135;
                                    Promoted = true;
                                }

                                if (Promoted)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Congratulations! You have promoted yourself."));
                                    SendPacket(General.MyPackets.NPCLink("Yay!.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 7, MyChar.Job));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You can't promote yet."));
                                    SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }

                            if (Control == 3)
                            {
                                bool Promoted = false;

                                if (MyChar.Job == 100 && MyChar.Level >= 15)
                                {
                                    MyChar.Job = 101;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 101 && MyChar.Level >= 40)
                                {
                                    MyChar.Job = 142;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 142 && MyChar.Level >= 70)
                                {
                                    MyChar.Job = 143;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 143 && MyChar.Level >= 100)
                                {
                                    if (MyChar.RBCount == 1)
                                    {
                                        MyChar.AddItem("134287-0-0-0-31-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 2)
                                    {
                                        MyChar.AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    MyChar.Job = 144;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 144 && MyChar.Level >= 110)
                                {
                                    if (MyChar.RBCount == 0)
                                    {
                                        MyChar.AddItem("700002-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 1)
                                    {
                                        MyChar.AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 2)
                                    {
                                        MyChar.AddItem("160199-0-0-0-33-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    MyChar.Job = 145;
                                    Promoted = true;
                                }

                                if (Promoted)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Congratulations! You have promoted yourself."));
                                    SendPacket(General.MyPackets.NPCLink("Yay!.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 7, MyChar.Job));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You can't promote yet."));
                                    SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }

                        #endregion
                        #region WaterTaoTrainer
                        if (CurrentNPC == 264)
                        {
                            if (Control == 100)
                            {
                                if (MyChar.Job < 136 && MyChar.Job > 129)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Here are the skills i can teach you, make sure you have the required level."));
                                    SendPacket(General.MyPackets.NPCSay("Want To Learn More?"));
                                    SendPacket(General.MyPackets.NPCLink("Stigma: Lv 55", 101));
                                    SendPacket(General.MyPackets.NPCLink("Pray: Lv 70", 102));
                                    SendPacket(General.MyPackets.NPCLink("Meditation: Lv 44", 103));
                                    SendPacket(General.MyPackets.NPCLink("Icicle: Lv 70", 104));
                                    SendPacket(General.MyPackets.NPCLink("Avalanch: Lv 100", 105));
                                    SendPacket(General.MyPackets.NPCLink("IceCircle: Lv 110", 106));
                                    SendPacket(General.MyPackets.NPCLink("AdvancedCure: Lv 81", 107));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }


                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You are not a water taoist, go away!"));
                                    SendPacket(General.MyPackets.NPCLink("I'm sorry, please dont hurt me", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (MyChar.Job < 136 && MyChar.Job > 129)
                            {
 
                                if (Control == 107)
                                {
                                    if (MyChar.Level >= 81)
                                    {
                                        MyChar.LearnSkill(1175, 0);
                                    }
                                }
                                if (Control == 104)
                                {
                                    if (MyChar.Level >= 70)
                                    {
                                        MyChar.LearnSkill(5130, 0);
                                    }
                                }
                                if (Control == 105)
                                {
                                    if (MyChar.Level >= 100)
                                    {
                                        MyChar.LearnSkill(5131, 0);
                                    }
                                }
                                if (Control == 106)
                                {
                                    if (MyChar.Level >= 110)
                                    {
                                        MyChar.LearnSkill(5132, 0);
                                    }
                                }

                                if (Control == 101)
                                {
                                    if (MyChar.Level >= 55)
                                    {
                                        MyChar.LearnSkill(1095, 0);
                                    }
                                }
                                if (Control == 102)
                                {
                                    if (MyChar.Level >= 70)
                                    {
                                        MyChar.LearnSkill(1100, 0);
                                    }
                                }
                                if (Control == 103)
                                {
                                    if (MyChar.Level >= 44)
                                    {
                                        MyChar.LearnSkill(1195, 0);
                                    }
                                }
                            }
                            if (Control == 1)
                            {
                                if (MyChar.Job == 100)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Taoist you need to be level 15 or higher."));
                                }
                                if (MyChar.Job == 101)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Water Taoist or Fire Taoist you need to be level 40 or higher. What job will you choose?"));
                                }
                                if (MyChar.Job == 132)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Water Wizard you need to be level 70 or higher."));
                                }
                                if (MyChar.Job == 133)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Water Master you need to be level 100 or higher."));
                                }
                                if (MyChar.Job == 134)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Water Saint you need to be level 110 or higher."));
                                }
                                if (MyChar.Job == 142)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Fire Wizard you need to be level 70 or higher."));
                                }
                                if (MyChar.Job == 143)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Fire Master you need to be level 100 or higher."));
                                }
                                if (MyChar.Job == 144)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Fire Saint you need to be level 110 or higher."));
                                }
                                if (MyChar.Job != 135 && MyChar.Job != 145 && MyChar.Job != 101)
                                {
                                    if (MyChar.Job < 136 && MyChar.Job > 131)
                                        SendPacket(General.MyPackets.NPCLink("I want to promote.", 2));
                                    else
                                        SendPacket(General.MyPackets.NPCLink("I want to promote.", 3));
                                    SendPacket(General.MyPackets.NPCLink("I think i am not qualified for that yet.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else if (MyChar.Job == 101)
                                {
                                    SendPacket(General.MyPackets.NPCLink("Water Taoist", 2));
                                    SendPacket(General.MyPackets.NPCLink("Fire Taoist", 3));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You cannot promote anymore, i can't help you."));
                                    SendPacket(General.MyPackets.NPCLink("Sorry.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 2)
                            {
                                bool Promoted = false;

                                if (MyChar.Job == 100 && MyChar.Level >= 15)
                                {
                                    MyChar.Job = 101;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 101 && MyChar.Level >= 40)
                                {
                                    MyChar.Job = 132;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 132 && MyChar.Level >= 70)
                                {
                                    MyChar.Job = 133;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 133 && MyChar.Level >= 100)
                                {
                                    if (MyChar.RBCount == 1)
                                    {
                                        MyChar.AddItem("134287-0-0-0-31-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 2)
                                    {
                                        MyChar.AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    MyChar.Job = 134;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 134 && MyChar.Level >= 110)
                                {
                                    if (MyChar.RBCount == 0)
                                    {
                                        MyChar.AddItem("700032-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 1)
                                    {
                                        MyChar.AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 2)
                                    {
                                        MyChar.AddItem("160199-0-0-0-33-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    MyChar.Job = 135;
                                    Promoted = true;
                                }

                                if (Promoted)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Congratulations! You have promoted yourself."));
                                    SendPacket(General.MyPackets.NPCLink("Yay!.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 7, MyChar.Job));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You can't promote yet."));
                                    SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }

                            if (Control == 3)
                            {
                                bool Promoted = false;

                                if (MyChar.Job == 100 && MyChar.Level >= 15)
                                {
                                    MyChar.Job = 101;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 101 && MyChar.Level >= 40)
                                {
                                    MyChar.Job = 142;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 142 && MyChar.Level >= 70)
                                {
                                    MyChar.Job = 143;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 143 && MyChar.Level >= 100)
                                {
                                    if (MyChar.RBCount == 1)
                                    {
                                        MyChar.AddItem("134287-0-0-0-31-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 2)
                                    {
                                        MyChar.AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    MyChar.Job = 144;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 144 && MyChar.Level >= 110)
                                {
                                    if (MyChar.RBCount == 0)
                                    {
                                        MyChar.AddItem("700002-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 1)
                                    {
                                        MyChar.AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 2)
                                    {
                                        MyChar.AddItem("160199-0-0-0-33-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    MyChar.Job = 145;
                                    Promoted = true;
                                }

                                if (Promoted)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Congratulations! You have promoted yourself."));
                                    SendPacket(General.MyPackets.NPCLink("Yay!.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 7, MyChar.Job));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You can't promote yet."));
                                    SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }

                        #endregion

                        if (CurrentNPC == 17)
                        {
                            if (MyChar.Job < 16 && MyChar.Job > 9 || Control == 255)
                            {
                                if (Control == 222)
                                {

                                    SendPacket(General.MyPackets.NPCSay("Ok, choose the skill you want to learn."));
                                    SendPacket(General.MyPackets.NPCLink("Hercules: Lv 40", 31));
                                    SendPacket(General.MyPackets.NPCLink("Spiritual healing: Lv 40", 32));
                                    SendPacket(General.MyPackets.NPCLink("Robot: Lv 40", 33));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                }
                                if (Control == 31)
                                {
                                    if (MyChar.Level >= 40)
                                        MyChar.LearnSkill(1115, 0);
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("You are lower than level 40, you can't learn it."));
                                        SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                                if (Control == 32)
                                {
                                    if (MyChar.Level >= 40)
                                        MyChar.LearnSkill(1190, 0);
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("You are lower than level 40, you can't learn it."));
                                        SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                                if (Control == 33)
                                {
                                    if (MyChar.Level >= 40)
                                        MyChar.LearnSkill(1270, 0);
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("You are lower than level 40, you can't learn it."));
                                        SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }

                            if (Control == 1)
                            {
                                if (MyChar.Job == 10)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Trojan you need to be level 15 or higher."));
                                }
                                if (MyChar.Job == 11)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Veteran Trojan you need to be level 40 or higher."));
                                }
                                if (MyChar.Job == 12)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Tiger Trojan you need to be level 70 or higher."));
                                }
                                if (MyChar.Job == 13)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Dragon Trojan you need to be level 100 or higher."));
                                }
                                if (MyChar.Job == 14)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Trojan Master you need to be level 110 or higher."));
                                }
                                if (MyChar.Job != 15)
                                {
                                    SendPacket(General.MyPackets.NPCLink("I want to promote.", 2));
                                    SendPacket(General.MyPackets.NPCLink("I think i am not qualified for that yet.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You cannot promote anymore, i can't help you."));
                                    SendPacket(General.MyPackets.NPCLink("Sorry.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 2)
                            {
                                bool Promoted = false;

                                if (MyChar.Job == 10 && MyChar.Level >= 15)
                                {
                                    MyChar.Job = 11;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 11 && MyChar.Level >= 40)
                                {
                                    MyChar.Job = 12;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 12 && MyChar.Level >= 70)
                                {
                                    MyChar.Job = 13;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 13 && MyChar.Level >= 100)
                                {
                                    if (MyChar.RBCount == 1)
                                    {
                                        MyChar.AddItem("130287-0-0-0-11-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 2)
                                    {
                                        MyChar.AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    MyChar.Job = 14;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 14 && MyChar.Level >= 110)
                                {
                                    if (MyChar.RBCount == 0)
                                    {
                                        MyChar.AddItem("700012-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 1)
                                    {
                                        MyChar.AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 2)
                                    {
                                        MyChar.AddItem("160199-0-0-0-13-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    MyChar.Job = 15;
                                    Promoted = true;
                                }

                                if (Promoted)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Congratulations! You have promoted yourself."));
                                    SendPacket(General.MyPackets.NPCLink("Yay!.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 7, MyChar.Job));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You can't promote yet."));
                                    SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }

                        if (CurrentNPC == 6)
                        {
                            if (MyChar.Job < 46 && MyChar.Job > 39 || Control == 255)
                            {
                                if (Control == 20)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Ok, choose the skill you want to learn."));
                                    SendPacket(General.MyPackets.NPCLink("XP Fly: Lv 15", 24));
                                    SendPacket(General.MyPackets.NPCLink("Scatter: Lv 27", 21));
                                    SendPacket(General.MyPackets.NPCLink("Rapid Fire: Lv 40", 22));
                                    SendPacket(General.MyPackets.NPCLink("Intensify: Lv 47", 23));
                                    SendPacket(General.MyPackets.NPCLink("Arrow rain: Lv 70", 25));
                                    SendPacket(General.MyPackets.NPCLink("Fly: Lv 70", 27));
                                    SendPacket(General.MyPackets.NPCLink("Advanced fly: Lv 100", 26));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                if (Control == 21)
                                {
                                    if (MyChar.Level < 27)
                                    {

                                        SendPacket(General.MyPackets.NPCSay("You are lower than level 27, you can't learn it."));
                                        SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                    else
                                    {
                                        MyChar.LearnSkill(8001, 0);
                                        SendPacket(General.MyPackets.NPCSay("Congratulations! You learned scatter."));
                                        SendPacket(General.MyPackets.NPCLink("Yay!", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                                if (Control == 22)
                                {
                                    if (MyChar.Level < 40)
                                    {

                                        SendPacket(General.MyPackets.NPCSay("You are lower than level 40, you can't learn it."));
                                        SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                    else
                                    {
                                        MyChar.LearnSkill(8000, 0);
                                        SendPacket(General.MyPackets.NPCSay("Congratulations! You learned rapid fire."));
                                        SendPacket(General.MyPackets.NPCLink("Yay!", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                                if (Control == 23)
                                {
                                    if (MyChar.Level < 47)
                                    {

                                        SendPacket(General.MyPackets.NPCSay("You are lower than level 47, you can't learn it."));
                                        SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                    else
                                    {
                                        MyChar.LearnSkill(9000, 0);
                                        SendPacket(General.MyPackets.NPCSay("Congratulations! You learned intefsify."));
                                        SendPacket(General.MyPackets.NPCLink("Yay!", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                                if (Control == 24)
                                {
                                    if (MyChar.Level < 15)
                                    {

                                        SendPacket(General.MyPackets.NPCSay("You are lower than level 15, you can't learn it."));
                                        SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                    else
                                    {
                                        MyChar.LearnSkill(8002, 0);
                                        SendPacket(General.MyPackets.NPCSay("Congratulations! You learned XP fly."));
                                        SendPacket(General.MyPackets.NPCLink("Yay!", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                                if (Control == 25)
                                {
                                    if (MyChar.Level < 70)
                                    {

                                        SendPacket(General.MyPackets.NPCSay("You are lower than level 70, you can't learn it."));
                                        SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                    else
                                    {
                                        MyChar.LearnSkill(8030, 0);
                                        SendPacket(General.MyPackets.NPCSay("Congratulations! You learned arrow rain."));
                                        SendPacket(General.MyPackets.NPCLink("Yay!", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }

                                if (Control == 26)
                                {
                                    if (MyChar.Level < 100)
                                    {

                                        SendPacket(General.MyPackets.NPCSay("You are lower than level 100, you can't learn it."));
                                        SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                    else
                                    {
                                        MyChar.LearnSkill2(8003, 1);
                                        SendPacket(General.MyPackets.NPCSay("Congratulations! You learned advanced fly."));
                                        SendPacket(General.MyPackets.NPCLink("Yay!", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                                if (Control == 27)
                                {
                                    if (MyChar.Level < 70)
                                    {
                                        SendPacket(General.MyPackets.NPCSay("You are lower than level 70, you can't learn it."));
                                        SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                    else
                                    {
                                        MyChar.LearnSkill(8003, 0);
                                        SendPacket(General.MyPackets.NPCSay("Congratulations! You learned fly."));
                                        SendPacket(General.MyPackets.NPCLink("Yay!", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                            else
                            {
                                SendPacket(General.MyPackets.NPCSay("You are not an archer, you can't learn them!"));
                                SendPacket(General.MyPackets.NPCLink("Ok.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 1)
                            {
                                if (MyChar.Job == 40)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Archer you need to be level 15 or higher."));
                                }
                                if (MyChar.Job == 41)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Eagle Archer you need to be level 40 or higher."));
                                }
                                if (MyChar.Job == 42)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Tiger Archer you need to be level 70 or higher."));
                                }
                                if (MyChar.Job == 43)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Dragon Archer you need to be level 100 or higher."));
                                }
                                if (MyChar.Job == 44)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Archer Master you need to be level 110 or higher."));
                                }
                                if (MyChar.Job != 45)
                                {
                                    SendPacket(General.MyPackets.NPCLink("I want to promote.", 2));
                                    SendPacket(General.MyPackets.NPCLink("I think i am not qualified for that yet.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You cannot promote anymore, i can't help you."));
                                    SendPacket(General.MyPackets.NPCLink("Sorry.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 2)
                            {
                                bool Promoted = false;

                                if (MyChar.Job == 40 && MyChar.Level >= 15)
                                {
                                    MyChar.Job = 41;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 41 && MyChar.Level >= 40)
                                {
                                    MyChar.Job = 42;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 42 && MyChar.Level >= 70)
                                {
                                    MyChar.Job = 43;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 43 && MyChar.Level >= 100)
                                {
                                    if (MyChar.RBCount == 1)
                                    {
                                        MyChar.AddItem("133277-0-0-0-11-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 2)
                                    {
                                        MyChar.AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    MyChar.Job = 44;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 44 && MyChar.Level >= 110)
                                {
                                    if (MyChar.RBCount == 0)
                                    {
                                        MyChar.AddItem("700012-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 1)
                                    {
                                        MyChar.AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 2)
                                    {
                                        MyChar.AddItem("160199-0-0-0-13-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    MyChar.Job = 45;
                                    Promoted = true;
                                }

                                if (Promoted)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Congratulations! You have promoted yourself."));
                                    SendPacket(General.MyPackets.NPCLink("Yay!.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 7, MyChar.Job));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You can't promote yet."));
                                    SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }

                        if (CurrentNPC == 16)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.Job == 20)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Warrior you need to be level 15 or higher."));
                                }
                                if (MyChar.Job == 21)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Brass Warrior you need to be level 40 or higher."));
                                }
                                if (MyChar.Job == 22)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Silver Warrior you need to be level 70 or higher."));
                                }
                                if (MyChar.Job == 23)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Gold Warrior you need to be level 100 or higher."));
                                }
                                if (MyChar.Job == 24)
                                {
                                    SendPacket(General.MyPackets.NPCSay("To promote yourself to Warrior Master you need to be level 110 or higher."));
                                }
                                if (MyChar.Job != 25)
                                {
                                    SendPacket(General.MyPackets.NPCLink("I want to promote.", 2));
                                    SendPacket(General.MyPackets.NPCLink("I think i am not qualified for that yet.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You cannot promote anymore, i can't help you."));
                                    SendPacket(General.MyPackets.NPCLink("Sorry.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 2)
                            {
                                bool Promoted = false;

                                if (MyChar.Job == 20 && MyChar.Level >= 15)
                                {
                                    MyChar.Job = 21;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 21 && MyChar.Level >= 40)
                                {
                                    MyChar.Job = 22;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 22 && MyChar.Level >= 70)
                                {
                                    MyChar.Job = 23;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 23 && MyChar.Level >= 100)
                                {
                                    if (MyChar.RBCount == 1)
                                    {
                                        MyChar.AddItem("131287-0-0-0-11-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 2)
                                    {
                                        MyChar.AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    MyChar.Job = 24;
                                    Promoted = true;
                                }
                                else if (MyChar.Job == 24 && MyChar.Level >= 110)
                                {
                                    if (MyChar.RBCount == 0)
                                    {
                                        MyChar.AddItem("700012-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 1)
                                    {
                                        MyChar.AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    else if (MyChar.RBCount == 2)
                                    {
                                        MyChar.AddItem("160199-0-0-0-13-0", 0, (uint)General.Rand.Next(36457836));
                                    }
                                    MyChar.Job = 25;
                                    Promoted = true;
                                }

                                if (Promoted)
                                {
                                    SendPacket(General.MyPackets.NPCSay("Congratulations! You have promoted yourself."));
                                    SendPacket(General.MyPackets.NPCLink("Yay!.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 7, MyChar.Job));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You can't promote yet."));
                                    SendPacket(General.MyPackets.NPCLink("Damn.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }
                        if (CurrentNPC == 63421)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.CPs >= 27)
                                {
                                    MyChar.EPotXP = 3600;
                                    MyChar.EPotXP2 = MyChar.EPotXP * 2;
                                    MyChar.EPotRate = true;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 19, MyChar.EPotXP));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 26, MyChar.GetStat()));
                                    World.UpdateSpawn(MyChar);
                                    MyChar.CPs -= 27;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You do not have 27 CPs"));

                                    SendPacket(General.MyPackets.NPCLink("Ah shit, sorry!", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }
                        if (CurrentNPC == 44)
                        {
                            if (Control == 1)
                            {
                                SendPacket(General.MyPackets.NPCSay("If you give me a Dragon Ball for first socket or 5 for second, i shall try my best to help you."));
                                SendPacket(General.MyPackets.NPCLink("Ok Here is/are the Dragon Ball(s).", 2));
                                SendPacket(General.MyPackets.NPCLink("I changed my mind.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 2)
                            {
                                SendPacket(General.MyPackets.NPCSay("Please equip the right handed weapon you want to socket first. Now i am going to make a socket in you weapon."));
                                SendPacket(General.MyPackets.NPCLink("I am ready.", 3));
                                SendPacket(General.MyPackets.NPCLink("I changed my mind.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 3)
                            {
                                if (MyChar.Equips[4] != null || MyChar.Equips[4] != "0")
                                {
                                    string[] item = MyChar.Equips[4].Split('-');

                                    if (item[4] == "0" && MyChar.InventoryContains(1088000, 1) || item[5] == "0" && MyChar.InventoryContains(1088000, 5))
                                    {
                                        if (item[4] == "0")
                                        {
                                            item[4] = "255";
                                            MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                            MyChar.GetEquipStats(4, true);
                                            MyChar.Equips[4] = item[0] + "-" + item[1] + "-" + item[2] + "-" + item[3] + "-" + item[4] + "-" + item[5];
                                            MyChar.GetEquipStats(4, false);
                                            SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[4], int.Parse(item[0]), byte.Parse(item[1]), byte.Parse(item[2]), byte.Parse(item[3]), byte.Parse(item[4]), byte.Parse(item[5]), 4, 100, 100));
                                        }
                                        else if (item[5] == "0")
                                        {
                                            item[5] = "255";
                                            MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                            MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                            MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                            MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                            MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                            MyChar.GetEquipStats(4, true);
                                            MyChar.Equips[4] = item[0] + "-" + item[1] + "-" + item[2] + "-" + item[3] + "-" + item[4] + "-" + item[5];
                                            MyChar.GetEquipStats(4, false);
                                            SendPacket(General.MyPackets.AddItem((long)MyChar.Equips_UIDs[4], int.Parse(item[0]), byte.Parse(item[1]), byte.Parse(item[2]), byte.Parse(item[3]), byte.Parse(item[4]), byte.Parse(item[5]), 4, 100, 100));
                                        }
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("You don't have enough Dragon Ball(s)."));
                                        SendPacket(General.MyPackets.NPCLink("Ok.", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                            }
                        }

                        if (CurrentNPC == 266)
                        {
                            if (Control == 1 || Control == 111)
                            {
                                if (Control == 111)
                                {
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 27, MyChar.Hair));
                                }
                                SendPacket(General.MyPackets.NPCSay("Which style would you like to choose from?"));
                                SendPacket(General.MyPackets.NPCLink("New HairStyle01", 4));
                                SendPacket(General.MyPackets.NPCLink("New HairStyle02", 5));
                                SendPacket(General.MyPackets.NPCLink("New HairStyle03", 6));
                                SendPacket(General.MyPackets.NPCLink("New HairStyle04", 7));
                                SendPacket(General.MyPackets.NPCLink("New HairStyle05", 8));
                                SendPacket(General.MyPackets.NPCLink("New HairStyle06", 9));
                                SendPacket(General.MyPackets.NPCLink("New HairStyle07", 10));
                                SendPacket(General.MyPackets.NPCLink("Next.", 11));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 11)
                            {
                                SendPacket(General.MyPackets.NPCSay("Which style would you like to choose from?"));
                                SendPacket(General.MyPackets.NPCLink("New HairStyle08", 12));
                                SendPacket(General.MyPackets.NPCLink("New HairStyle09", 13));
                                SendPacket(General.MyPackets.NPCLink("New HairStyle10", 14));
                                SendPacket(General.MyPackets.NPCLink("New HairStyle11", 15));
                                SendPacket(General.MyPackets.NPCLink("New HairStyle12", 16));
                                SendPacket(General.MyPackets.NPCLink("Previous", 1));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 2 || Control == 112)
                            {
                                if (Control == 112)
                                {
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 27, MyChar.Hair));
                                }
                                SendPacket(General.MyPackets.NPCSay("Which style would you like to choose from?"));
                                SendPacket(General.MyPackets.NPCLink("Nostalgic01", 17));
                                SendPacket(General.MyPackets.NPCLink("Nostalgic02", 18));
                                SendPacket(General.MyPackets.NPCLink("Nostalgic03", 19));
                                SendPacket(General.MyPackets.NPCLink("Nostalgic04", 20));
                                SendPacket(General.MyPackets.NPCLink("Nostalgic05", 21));
                                SendPacket(General.MyPackets.NPCLink("Nostalgic06", 22));
                                SendPacket(General.MyPackets.NPCLink("Nostalgic07", 23));
                                SendPacket(General.MyPackets.NPCLink("I changed my mind.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 3 || Control == 113)
                            {
                                if (Control == 113)
                                {
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 27, MyChar.Hair));
                                }
                                SendPacket(General.MyPackets.NPCSay("Which style would you like to choose from?"));
                                SendPacket(General.MyPackets.NPCLink("Special01", 25));
                                SendPacket(General.MyPackets.NPCLink("Special02", 26));
                                SendPacket(General.MyPackets.NPCLink("Special03", 27));
                                SendPacket(General.MyPackets.NPCLink("Special04", 28));
                                SendPacket(General.MyPackets.NPCLink("Special05", 29));
                                SendPacket(General.MyPackets.NPCLink("I changed my mind.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                            if (Control == 254)
                            {
                                if (MyChar.Silvers >= 500)
                                {
                                    MyChar.Silvers -= 500;
                                    MyChar.Hair = MyChar.ShowHair;

                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                }
                            }
                            if (Control > 3 && Control < 30 && Control != 11)
                            {
                                if (MyChar.Silvers >= 500)
                                {
                                    SendPacket(General.MyPackets.NPCSay("It's completed. Are you satisfied with your new hairstyle?"));
                                    SendPacket(General.MyPackets.NPCLink("Cool!", 254));
                                    if (Control < 17)
                                        SendPacket(General.MyPackets.NPCLink("I want to change it.", 111));
                                    else if (Control < 25)
                                        SendPacket(General.MyPackets.NPCLink("I want to change it.", 112));
                                    else if (Control < 30)
                                        SendPacket(General.MyPackets.NPCLink("I want to change it.", 113));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());

                                    if (Control == 4)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "30");
                                    if (Control == 5)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "31");
                                    if (Control == 6)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "32");
                                    if (Control == 7)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "33");
                                    if (Control == 8)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "34");
                                    if (Control == 9)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "35");
                                    if (Control == 10)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "36");
                                    if (Control == 12)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "37");
                                    if (Control == 13)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "38");
                                    if (Control == 14)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "39");
                                    if (Control == 15)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "40");
                                    if (Control == 16)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "41");

                                    if (Control == 17)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "11");
                                    if (Control == 18)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "12");
                                    if (Control == 19)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "13");
                                    if (Control == 20)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "14");
                                    if (Control == 21)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "15");
                                    if (Control == 22)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "16");
                                    if (Control == 23)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "17");
                                    if (Control == 24)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "10");

                                    if (Control == 25)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "21");
                                    if (Control == 26)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "22");
                                    if (Control == 27)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "23");
                                    if (Control == 28)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "24");
                                    if (Control == 29)
                                        MyChar.ShowHair = ushort.Parse(Convert.ToString(MyChar.Hair)[0] + "25");

                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 27, MyChar.ShowHair));
                                    World.UpdateSpawn(MyChar);
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You have not enough money."));
                                    SendPacket(General.MyPackets.NPCLink("I'm sorry.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }

                        if (CurrentNPC == 211 || CurrentNPC == 180)
                        {
                            if (Control == 1)
                            {
                                ushort XTo = 429;
                                ushort YTo = 378;

                                if (MyChar.PrevMap == 1015)
                                {
                                    XTo = 717;
                                    YTo = 571;
                                }
                                if (MyChar.PrevMap == 1000)
                                {
                                    XTo = 500;
                                    YTo = 650;
                                }
                                if (MyChar.PrevMap == 1011)
                                {
                                    XTo = 188;
                                    YTo = 264;
                                }
                                if (MyChar.PrevMap == 1020)
                                {
                                    XTo = 565;
                                    YTo = 562;
                                }

                                MyChar.Teleport(MyChar.PrevMap, XTo, YTo);
                            }
                        }

                        if (CurrentNPC == 21 || CurrentNPC == 181 || CurrentNPC == 187 || CurrentNPC == 183 || CurrentNPC == 184)
                        {
                            if (Control == 1)
                            {
                                MyChar.Teleport(1039, 217, 215);
                                SendPacket(General.MyPackets.NPCSay("The stake or scarecrow is level 20 or above. If its level is higher than yours, you cannot attack it."));
                                SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                SendPacket(General.MyPackets.NPCSetFace(30));
                                SendPacket(General.MyPackets.NPCFinish());
                            }
                        }
                        if (CurrentNPC == 104804)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.Silvers >= 100)
                                {
                                    MyChar.Teleport(1002, 958, 555);
                                    MyChar.Silvers -= 100;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have 100 silver."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 2)
                            {
                                if (MyChar.Silvers >= 100)
                                {
                                    MyChar.Teleport(1002, 69, 473);
                                    MyChar.Silvers -= 100;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have 100 silver."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 3)
                            {
                                if (MyChar.Silvers >= 100)
                                {
                                    MyChar.Teleport(1002, 555, 957);
                                    MyChar.Silvers -= 100;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have 100 silver."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 4)
                            {
                                if (MyChar.Silvers >= 100)
                                {
                                    MyChar.Teleport(1002, 232, 190);
                                    MyChar.Silvers -= 100;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have 100 silver."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 5)
                            {
                                if (MyChar.Silvers >= 100)
                                {
                                    MyChar.Teleport(1002, 53, 399);
                                    MyChar.Silvers -= 100;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have 100 silver."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 6)
                            {
                                if (MyChar.Silvers >= 100)
                                {
                                    MyChar.Teleport(1036, 211, 196);
                                    MyChar.Silvers -= 100;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have 100 silver."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 7)
                            {
                                if (MyChar.Silvers >= 100)
                                {
                                    MyChar.Teleport(1036, 211, 196);
                                    MyChar.Silvers -= 100;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have 100 silver."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 8)
                            {
                                if (MyChar.Silvers >= 100)
                                {
                                    MyChar.Teleport(1036, 211, 196);
                                    MyChar.Silvers -= 100;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have 100 silver."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 8)
                            {
                                if (MyChar.Silvers >= 100)
                                {
                                    MyChar.Teleport(1036, 211, 196);
                                    MyChar.Silvers -= 100;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have 100 silver."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 9)
                            {
                                if (MyChar.Silvers >= 100)
                                {
                                    MyChar.Teleport(1036, 211, 196);
                                    MyChar.Silvers -= 100;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have 100 silver."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 10)
                            {
                                if (MyChar.Silvers >= 100)
                                {
                                    MyChar.Teleport(1036, 211, 196);
                                    MyChar.Silvers -= 100;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have 100 silver."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }

                        if (CurrentNPC == 104806)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.InventoryContains(1088000, 1))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(1088000));
                                    MyChar.Teleport(1351, 211, 322);
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have a DragonBall"));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }

                        if (CurrentNPC == 104807)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.InventoryContains(721537, 1))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(721537));
                                    MyChar.Teleport(1352, 31, 227);
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have a DragonBall"));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 3)
                            {
                                MyChar.Teleport(1002, 438, 377);
                            }
                        }

                        if (CurrentNPC == 104808)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.InventoryContains(721538, 1))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(721538));
                                    MyChar.Teleport(1353, 29, 265);
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have a DragonBall"));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 3)
                            {
                                MyChar.Teleport(1002, 438, 377);
                            }
                        }

                        if (CurrentNPC == 104809)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.InventoryContains(721539, 1))
                                {
                                    MyChar.RemoveItem(MyChar.ItemNext(721539));
                                    MyChar.Teleport(1354, 17, 298);
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have a DragonBall"));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 3)
                            {
                                MyChar.Teleport(1002, 438, 377);
                            }
                        }

                        if (CurrentNPC == 104810)
                        {
                            if (Control == 1)
                            {
                                MyChar.Teleport(1002, 438, 377);
                            }
                        }
                        if (CurrentNPC == 105016)
                        {
                            if (Control == 1)
                            {
                                MyChar.Teleport(1040, 595, 383);
                            }
                        }
                        if (CurrentNPC == 105005 || CurrentNPC == 105006 || CurrentNPC == 105007 || CurrentNPC == 105008 || CurrentNPC == 105009)
                        {
                            uint skyrnd = (uint)General.Rand.Next(3);

                            if (Control == 1)
                            {
                                if (skyrnd == 1)
                                    MyChar.Teleport(1040, 543, 330);
                                else
                                    MyChar.Teleport(1040, 368, 588);
                            }
                            if (Control == 2)
                            {
                                if (skyrnd == 1)
                                    MyChar.Teleport(1040, 492, 280);
                                else
                                    MyChar.Teleport(1040, 320, 540);
                            }
                            if (Control == 3)
                            {
                                if (skyrnd == 1)
                                    MyChar.Teleport(1040, 436, 224);
                                else
                                    MyChar.Teleport(1040, 272, 492);
                            }
                            if (Control == 4)
                            {
                                if (skyrnd == 1)
                                    MyChar.Teleport(1040, 393, 181);
                                else
                                    MyChar.Teleport(1040, 224, 444);
                            }
                            if (Control == 5)
                            {
                                if (skyrnd == 1)
                                    MyChar.Teleport(1040, 141, 240);
                                else
                                    MyChar.Teleport(1040, 176, 396);
                            }
                            if (Control == 6)
                            {
                                MyChar.Teleport(1002, 429, 378);
                            }
                        }
                        if (CurrentNPC == 105010 || CurrentNPC == 105011 || CurrentNPC == 105012 || CurrentNPC == 105013 || CurrentNPC == 105014)
                        {

                            if (Control == 1)
                            {
                                MyChar.RemoveItem(MyChar.ItemNext(721100));
                                MyChar.Teleport(1040, 595, 383);
                            }
                            if (Control == 2)
                            {
                                MyChar.RemoveItem(MyChar.ItemNext(721101));
                                MyChar.Teleport(1040, 543, 330);
                            }
                            if (Control == 3)
                            {
                                MyChar.RemoveItem(MyChar.ItemNext(721102));
                                MyChar.Teleport(1040, 492, 280);
                            }

                            if (Control == 4)
                            {
                                MyChar.RemoveItem(MyChar.ItemNext(721103));
                                MyChar.Teleport(1040, 436, 224);
                            }
                            if (Control == 5)
                            {
                                MyChar.RemoveItem(MyChar.ItemNext(721108));
                                MyChar.Teleport(1040, 393, 181);
                            }
                        }
                        if (CurrentNPC == 105015)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.ItemsInInventory < 36)
                                {
                                    MyChar.AddItem("720028-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.AddItem("720028-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.AddItem("720028-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.AddItem("720028-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                    MyChar.Teleport(1002, 429, 378);
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("You dont have enough free space in invectory"));
                                    SendPacket(General.MyPackets.NPCLink("My bad", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 2)
                            {
                                ushort skyrnd = (ushort)General.Rand.Next(2);
                                if (skyrnd == 1)
                                {
                                    if (MyChar.ItemsInInventory < 31)
                                    {
                                        MyChar.AddItem("720027-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                        MyChar.AddItem("720027-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                        MyChar.AddItem("720027-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                        MyChar.AddItem("720027-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                        MyChar.AddItem("720027-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                        MyChar.AddItem("720027-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                        MyChar.AddItem("720027-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                        MyChar.AddItem("720027-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                        MyChar.AddItem("720027-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                        MyChar.AddItem("720027-0-0-0-0-0", 0, (uint)General.Rand.Next(36457836));
                                        MyChar.Teleport(1002, 429, 378);
                                    }
                                    else
                                    {
                                        SendPacket(General.MyPackets.NPCSay("You dont have enough free space in invectory"));
                                        SendPacket(General.MyPackets.NPCLink("My bad", 255));
                                        SendPacket(General.MyPackets.NPCSetFace(30));
                                        SendPacket(General.MyPackets.NPCFinish());
                                    }
                                }
                                else
                                    MyChar.Teleport(1002, 429, 378);
                            }
                        }
                        if (CurrentNPC == 104813)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.Silvers >= 200000)
                                {
                                    MyChar.Silvers -= 200000;
                                    MyChar.CPs += 20;
                                }
                            }
                        }
                        if (CurrentNPC == 103)
                        {
                            if (Control == 1)
                            {
                                if (MyChar.Silvers >= 100)
                                {
                                    MyChar.Teleport(1002, 958, 555);
                                    MyChar.Silvers -= 100;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have 100 silver."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 2)
                            {
                                if (MyChar.Silvers >= 100)
                                {
                                    MyChar.Teleport(1002, 69, 473);
                                    MyChar.Silvers -= 100;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have 100 silver."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 3)
                            {
                                if (MyChar.Silvers >= 100)
                                {
                                    MyChar.Teleport(1002, 555, 957);
                                    MyChar.Silvers -= 100;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have 100 silver."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 4)
                            {
                                if (MyChar.Silvers >= 100)
                                {
                                    MyChar.Teleport(1002, 232, 190);
                                    MyChar.Silvers -= 100;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have 100 silver."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 5)
                            {
                                if (MyChar.Silvers >= 100)
                                {
                                    MyChar.Teleport(1002, 53, 399);
                                    MyChar.Silvers -= 100;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have 100 silver."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                            if (Control == 6)
                            {
                                if (MyChar.Silvers >= 100)
                                {
                                    MyChar.Teleport(1036, 211, 196);
                                    MyChar.Silvers -= 100;
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                }
                                else
                                {
                                    SendPacket(General.MyPackets.NPCSay("Sorry, you do not have 100 silver."));
                                    SendPacket(General.MyPackets.NPCLink("I see.", 255));
                                    SendPacket(General.MyPackets.NPCSetFace(30));
                                    SendPacket(General.MyPackets.NPCFinish());
                                }
                            }
                        }
                        break;
                    }
                        #endregion
                #region movement
                case 1005:
                    {
                        sbyte AddX = 0;
                        sbyte AddY = 0;
                        byte Dir = (byte)(Data[8] % 8);
                        MyChar.Direction = Dir;

                        switch (Dir)
                        {
                            case 0:
                                {
                                    AddY = 1;
                                    break;
                                }
                            case 1:
                                {
                                    AddX = -1;
                                    AddY = 1;
                                    break;
                                }
                            case 2:
                                {
                                    AddX = -1;
                                    break;
                                }
                            case 3:
                                {
                                    AddX = -1;
                                    AddY = -1;
                                    break;
                                }
                            case 4:
                                {
                                    AddY = -1;
                                    break;
                                }
                            case 5:
                                {
                                    AddX = 1;
                                    AddY = -1;
                                    break;
                                }
                            case 6:
                                {
                                    AddX = 1;
                                    break;
                                }
                            case 7:
                                {
                                    AddY = 1;
                                    AddX = 1;
                                    break;
                                }
                        }
                        World.PlayerMoves(MyChar, Data);

                        MyChar.PrevX = MyChar.LocX;
                        MyChar.PrevY = MyChar.LocY;
                        MyChar.LocX = (ushort)(MyChar.LocX + AddX);
                        MyChar.LocY = (ushort)(MyChar.LocY + AddY);

                        MyChar.TargetUID = 0;
                        MyChar.MobTarget = null;
                        MyChar.PTarget = null;
                        MyChar.TGTarget = null;
                        MyChar.AtkType = 0;
                        MyChar.SkillLooping = 0;
                        MyChar.Action = 100;
                        World.SpawnMeToOthers(MyChar, true);
                        World.SpawnOthersToMe(MyChar, true);
                        World.SurroundNPCs(MyChar, true);
                        World.SurroundMobs(MyChar, true);
                        World.SurroundDroppedItems(MyChar, true);
                        MyChar.Attacking = false;
                        if (MyChar.Mining)
                            MyChar.Mining = false;

                        break;
                    }
                #endregion
                #region chat
                case 1004:
                    {
                        short ChatType = (short)(Data[8] | (Data[9] << 8));
                        int Pos = 26;
                        int Length = 0;
                        string From = "";
                        string To = "";
                        string Message = "";
                        for (int Count = 0; Count < Data[25]; Count += 1)
                        {
                            From += Convert.ToChar(Data[Pos]);
                            Pos += 1;
                        }

                        Length = Data[Pos];
                        Pos += 1;

                        for (int Count = 0; Count < Length; Count += 1)
                        {
                            To += Convert.ToChar(Data[Pos]);
                            Pos += 1;
                        }

                        Pos += 1;
                        Length = Data[Pos];
                        Pos += 1;

                        for (int Count = 0; Count < Length; Count += 1)
                        {
                            if (Pos <= Data.Length)
                                Message += Convert.ToChar(Data[Pos]);
                            Pos += 1;
                        }
                        #region commands
                        if (Message[0] == '/')
                        {
                            if (Message == "/save")
                                MyChar.Save();

                            if (Message == "/coords")
                                SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Coords: " + MyChar.LocX + ", " + MyChar.LocY, 2000));
                            #region playercommands
                            try
                            {

                                if (Message[0] == '/')
                                {
                                    string[] Splitter = Message.Split(' ');

                                    if (Splitter[0] == "/dc")
                                    {
                                        Drop();
                                    }
                                    if (Splitter[0] == "/quest")
                                    {
                                        SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "You have to kill 300 " + MyChar.QuestMob + "s and you have killed " + MyChar.QuestKO + " so far.", 2000));
                                    }
                            #endregion
                                    #region VIPCommands
                                    if (Status == 5)
                                    {
                                        if (Splitter[0] == "/vipmap")
                                        {
                                                MyChar.Teleport(1011, 188, 264);
                                        }   
                                    }
                                    #endregion
                                    #region GMPMcommands
                                    if (Status == 8)
                                    {

                                        
                                        if (Splitter[0] == "/gwstart")
                                        {
                                            World.GWOn = true;
                                            World.SendMsgToAll("GuildWar has started!", "SYSTEM", 2011);
                                        }
                                        if (Splitter[0] == "/gwend")
                                        {
                                            World.GWOn = false;
                                            World.SendMsgToAll("GuildWar has ended!", "SYSTEM", 2011);
                                        }
                                        if (Splitter[0] == "/mm")
                                        {
                                            MyChar.Teleport(ushort.Parse(Splitter[1]), ushort.Parse(Splitter[2]), ushort.Parse(Splitter[3]));
                                        }
                                        if (Splitter[0] == "/setpkp")
                                        {
                                            ushort PKPoints = ushort.Parse(Splitter[1]);
                                            MyChar.PKPoints = PKPoints;
                                            World.UpdateSpawn(MyChar);
                                            SendPacket(General.MyPackets.CharacterInfo(MyChar));
                                            MyChar.SendEquips(false);
                                            SendPacket(General.MyPackets.Vital(MyChar.UID, 26, MyChar.GetStat()));

                                        }
                                        if (Splitter[0] == "/item2")
                                        {
                                            Ini ItemNames = new Ini(System.Windows.Forms.Application.StartupPath + @"\ItemNamesToId.ini");

                                            string ItemName = Splitter[2];
                                            string ItemQuality = Splitter[1];
                                            byte Plus = byte.Parse(Splitter[3]);
                                            byte Bless = byte.Parse(Splitter[4]);
                                            byte Enchant = byte.Parse(Splitter[5]);
                                            byte Soc1 = byte.Parse(Splitter[6]);
                                            byte Soc2 = byte.Parse(Splitter[7]);
                                            byte Position = byte.Parse(Splitter[8]);

                                            uint ItemId = 0;
                                            ItemId = uint.Parse(ItemNames.ReadValue("Items", ItemName));

                                            if (ItemId == 0)
                                                return;

                                            byte Quality = 1;

                                            if (ItemQuality == "One")
                                                Quality = 1;
                                            else if (ItemQuality == "Normal")
                                                Quality = 5;
                                            else if (ItemQuality == "Unique")
                                                Quality = 7;
                                            else if (ItemQuality == "Refined")
                                                Quality = 6;
                                            else if (ItemQuality == "Elite")
                                                Quality = 8;
                                            else if (ItemQuality == "Super")
                                                Quality = 9;
                                            else
                                                Quality = (byte)Other.ItemQuality(ItemId);

                                            ItemId = Other.ItemQualityChange(ItemId, Quality);

                                            if (MyChar.ItemsInInventory < 40)
                                                MyChar.AddItem(ItemId.ToString() + "-" + Plus.ToString() + "-" + Bless.ToString() + "-" + Enchant.ToString() + "-" + Soc1.ToString() + "-" + Soc2.ToString(), Position, (uint)General.Rand.Next(57458353));

                                        }
                                        if (Splitter[0] == "/dc")
                                        {
                                            Drop();
                                        }
                                        if (Splitter[0] == "/vp")
                                        {
                                            uint NewVP = uint.Parse(Splitter[1]);

                                            MyChar.VP = NewVP;
                                        }                                        
                                        if (Splitter[0] == "/refresh")
                                        {
                                            MyChar.Teleport(MyChar.LocMap, MyChar.LocX, MyChar.LocY);
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "area updated!!", 2005));
                                        }
                                        if (Splitter[0] == "/drop")
                                        {
                                            uint MoneyDrops = 0;
                                            byte Repeat = byte.Parse(Splitter[2]);
                                            for (int i = 0; i < Repeat; i++)
                                            {
                                                if (Splitter[1] == "db")
                                                {
                                                    string Item = "1088000-0-0-0-0-0";
                                                    DroppedItem item = DroppedItems.DropItem(Item, (uint)(MyChar.LocX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(MyChar.LocY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)MyChar.LocMap, MoneyDrops);
                                                    World.ItemDrops(item);
                                                }
                                                if (Splitter[1] == "met")
                                                {
                                                    string Item = "1088001-0-0-0-0-0";
                                                    DroppedItem item = DroppedItems.DropItem(Item, (uint)(MyChar.LocX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(MyChar.LocY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)MyChar.LocMap, MoneyDrops);
                                                    World.ItemDrops(item);
                                                }
                                            }
                                        }                                        
                                        if (Splitter[0] == "/recall")
                                        {
                                            foreach (DictionaryEntry DE in World.AllChars)
                                            {
                                                Character Char = (Character)DE.Value;
                                                if (Char.Name == Splitter[1])
                                                {
                                                    Char.Teleport(MyChar.LocMap, MyChar.LocX, MyChar.LocY);
                                                    break;
                                                }
                                                else
                                                {
                                                    SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Sorry the Character:" + Splitter[1] + " is offline...Please try again later", 2000));
                                                }
                                            }

                                        }
                                        if (Splitter[0] == "/rev")
                                        {
                                            MyChar.CurHP = MyChar.MaxHP;
                                            MyChar.Alive = true;
                                            MyChar.MyClient.SendPacket(General.MyPackets.Status1(MyChar.UID, 0));
                                            MyChar.MyClient.SendPacket(General.MyPackets.Status3(MyChar.UID));
                                            MyChar.MyClient.SendPacket(General.MyPackets.Vital(MyChar.UID, 26, MyChar.GetStat()));
                                            MyChar.MyClient.SendPacket(General.MyPackets.CharacterInfo(MyChar));
                                            MyChar.SendEquips(false);
                                            MyChar.BlueName = false;
                                            MyChar.MyClient.SendPacket(General.MyPackets.Vital(MyChar.UID, 26, MyChar.GetStat()));
                                            MyChar.Stamina = 100;
                                            MyChar.MyClient.SendPacket(General.MyPackets.Vital(MyChar.UID, 9, MyChar.Stamina));
                                            MyChar.MyClient.SendPacket(General.MyPackets.String(MyChar.UID, 10, "born3"));
                                            World.UpdateSpawn(MyChar);
                                        }
                                        if (Splitter[0] == "/sr")// restarts the server from ingame
                                        {
                                            World.SendMsgToAll("Server Restarting Please Log Off Immediately To Prevent Data Loss", "SYSTEM", 2011);
                                            Console.WriteLine("Server Restarting from In-Game");
                                            General.ServerRestart();
                                        }
                                        if (Splitter[0] == "/goto")
                                        {
                                            foreach (DictionaryEntry DE in World.AllChars)
                                            {
                                                Character Char = (Character)DE.Value;
                                                if (Char.Name == Splitter[1])
                                                {
                                                    MyChar.Teleport(Char.LocMap, Char.LocX, Char.LocY);
                                                    break;
                                                }
                                                else
                                                {
                                                    SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Sorry the Character:" + Splitter[1] + " is offline...Please try again later", 2000));
                                                }
                                            }
                                        }
                                        if (Splitter[0] == "/gmc")
                                        {
                                            Message = Message.Remove(0, 2);
                                            World.SendMsgToAll(Message, MyChar.Name, 2011);
                                        }
                                        if (Splitter[0] == "/xp")
                                        {
                                            MyChar.XpList = true;
                                            SendPacket(General.MyPackets.Vital(MyChar.UID, 26, MyChar.GetStat()));
                                        }
                                        if (Splitter[0] == "/changename")
                                        {
                                            string newname = Splitter[1];
                                            MyChar.Name = newname;
                                            SendPacket(General.MyPackets.CharacterInfo(MyChar));
                                            World.SpawnMeToOthers(MyChar, false);
                                            MyChar.SendEquips(false);
                                        }
                                        if (Splitter[0] == "/skill")
                                        {
                                            #region TrojanSkills
                                            if (Splitter[1] == "trojan")
                                            {
                                                MyChar.LearnSkill2(short.Parse("1190"), byte.Parse("2"));
                                                MyChar.LearnSkill2(short.Parse("1115"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1110"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1015"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1045"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1046"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("5030"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("1250"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("5050"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("5010"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("1260"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("5020"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("1290"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("5040"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("1300"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("7000"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("7010"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("7020"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("7030"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("7040"), byte.Parse("9"));
                                            }

                                            #endregion

                                            #region WarriorSkills

                                            if (Splitter[1] == "warrior")
                                            {
                                                MyChar.LearnSkill2(short.Parse("1015"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1020"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1025"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1040"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1051"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1320"), byte.Parse("2"));
                                                MyChar.LearnSkill2(short.Parse("1045"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1046"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("5030"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("1250"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("5050"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("5010"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("1260"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("5020"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("1290"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("5040"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("1300"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("7000"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("7010"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("7020"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("7030"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("7040"), byte.Parse("9"));
                                            }

                                            #endregion

                                            #region Archer Skills
                                            if (Splitter[1] == "archer")
                                            {
                                                MyChar.LearnSkill2(short.Parse("8001"), byte.Parse("5"));
                                                MyChar.LearnSkill2(short.Parse("8000"), byte.Parse("5"));
                                                MyChar.LearnSkill2(short.Parse("8003"), byte.Parse("1"));
                                                MyChar.LearnSkill2(short.Parse("8002"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("8030"), byte.Parse("0"));

                                            }

                                            #endregion
                                            #region FireSkills
                                            if (Splitter[1] == "fire")
                                            {
                                                MyChar.LearnSkill2(short.Parse("1002"), byte.Parse("3"));
                                                MyChar.LearnSkill2(short.Parse("1005"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1010"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1120"), byte.Parse("3"));
                                                MyChar.LearnSkill2(short.Parse("1125"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1150"), byte.Parse("7"));
                                                MyChar.LearnSkill2(short.Parse("1160"), byte.Parse("3"));
                                                MyChar.LearnSkill2(short.Parse("1165"), byte.Parse("3"));
                                                MyChar.LearnSkill2(short.Parse("1180"), byte.Parse("7"));
                                                MyChar.LearnSkill2(short.Parse("1195"), byte.Parse("2"));
                                                MyChar.LearnSkill2(short.Parse("5001"), byte.Parse("0"));

                                            }
                                            #endregion

                                            #region WaterSkills
                                            if (Splitter[1] == "water")
                                            {
                                                MyChar.LearnSkill2(short.Parse("5001"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1001"), byte.Parse("3"));
                                                MyChar.LearnSkill2(short.Parse("1005"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1010"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1075"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1095"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1090"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1085"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1100"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1170"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1175"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1195"), byte.Parse("2"));
                                                MyChar.LearnSkill2(short.Parse("1260"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("1510"), byte.Parse("9"));
                                            }
                                            #endregion
                                            #region allskills
                                            if (Splitter[1] == "all")
                                            {
                                                MyChar.LearnSkill2(short.Parse("1190"), byte.Parse("2"));
                                                MyChar.LearnSkill2(short.Parse("1115"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1110"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1015"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1045"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1046"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1015"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1020"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1025"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1040"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1051"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1320"), byte.Parse("2"));
                                                MyChar.LearnSkill2(short.Parse("1045"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1046"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("5030"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("1250"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("5050"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("5010"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("1260"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("5020"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("1290"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("5040"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("1300"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("7000"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("7010"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("7020"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("7030"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("7040"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("8001"), byte.Parse("5"));
                                                MyChar.LearnSkill2(short.Parse("8000"), byte.Parse("5"));
                                                MyChar.LearnSkill2(short.Parse("8003"), byte.Parse("1"));
                                                MyChar.LearnSkill2(short.Parse("8002"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("8030"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1002"), byte.Parse("3"));
                                                MyChar.LearnSkill2(short.Parse("1005"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1010"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1120"), byte.Parse("3"));
                                                MyChar.LearnSkill2(short.Parse("1125"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1150"), byte.Parse("7"));
                                                MyChar.LearnSkill2(short.Parse("1160"), byte.Parse("3"));
                                                MyChar.LearnSkill2(short.Parse("1165"), byte.Parse("3"));
                                                MyChar.LearnSkill2(short.Parse("1180"), byte.Parse("7"));
                                                MyChar.LearnSkill2(short.Parse("1195"), byte.Parse("2"));
                                                MyChar.LearnSkill2(short.Parse("5001"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("5001"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1001"), byte.Parse("3"));
                                                MyChar.LearnSkill2(short.Parse("1005"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1010"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1075"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1095"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1090"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1085"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1100"), byte.Parse("0"));
                                                MyChar.LearnSkill2(short.Parse("1170"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1175"), byte.Parse("4"));
                                                MyChar.LearnSkill2(short.Parse("1195"), byte.Parse("2"));
                                                MyChar.LearnSkill2(short.Parse("1260"), byte.Parse("9"));
                                                MyChar.LearnSkill2(short.Parse("1510"), byte.Parse("9"));
                                            }
                                            #endregion
                                            else
                                            {
                                                MyChar.LearnSkill2(short.Parse(Splitter[1]), byte.Parse(Splitter[2]));
                                            }
                                        }  

                                        if (Splitter[0] == "/model")
                                        {
                                            if (Splitter[1] == "smale")
                                                MyChar.Model = 1003;
                                            if (Splitter[1] == "lmale")
                                                MyChar.Model = 1004;
                                            if (Splitter[1] == "sfemale")
                                                MyChar.Model = 2001;
                                            if (Splitter[1] == "lfemale")
                                                MyChar.Model = 2002;

                                            SendPacket(General.MyPackets.Vital(MyChar.UID, 12, ulong.Parse(MyChar.Avatar.ToString() + MyChar.Model.ToString())));
                                            World.UpdateSpawn(MyChar);
                                        }
                                        if (Splitter[0] == "/info")
                                        {
                                            string BackMsg = "";
                                            foreach (DictionaryEntry DE in World.AllChars)
                                            {
                                                Character Char = (Character)DE.Value;
                                                BackMsg += Char.Name + ", ";
                                            }
                                            if (BackMsg.Length > 1)
                                                BackMsg = BackMsg.Remove(BackMsg.Length - 2, 2);
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Players Online: " + World.AllChars.Count, 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, BackMsg, 2000));
                                        }
                                        if (Splitter[0] == "/scroll")
                                        {                                
                                            if (Splitter[1] == "pc")
                                                MyChar.Teleport(1011, 188, 264);
                                            if (Splitter[1] == "dc")
                                                MyChar.Teleport(1000, 500, 650);
                                            if (Splitter[1] == "bi")
                                                MyChar.Teleport(1015, 723, 573);
                                            if (Splitter[1] == "am")
                                                MyChar.Teleport(1020, 565, 562);
                                            if (Splitter[1] == "ma")
                                                MyChar.Teleport(1036, 198, 194);
                                            if (Splitter[1] == "tc")
                                                MyChar.Teleport(1002, 429, 378);
                                        }                                 
                                        if (Splitter[0] == "/item")
                                        {
                                            Ini ItemNames = new Ini(System.Windows.Forms.Application.StartupPath + @"\ItemNamesToId.ini");
                                            string ItemName = Splitter[2];
                                            string ItemQuality = Splitter[1];
                                            byte Plus = byte.Parse(Splitter[3]);
                                            byte Bless = byte.Parse(Splitter[4]);
                                            byte Enchant = byte.Parse(Splitter[5]);
                                            byte Soc1 = byte.Parse(Splitter[6]);
                                            byte Soc2 = byte.Parse(Splitter[7]);

                                            uint ItemId = 0;
                                            ItemId = uint.Parse(ItemNames.ReadValue("Items", ItemName));

                                            if (ItemId == 0)
                                                return;

                                            byte Quality = 1;

                                            if (ItemQuality == "One")
                                                Quality = 1;
                                            else if (ItemQuality == "Normal")
                                                Quality = 5;
                                            else if (ItemQuality == "Unique")
                                                Quality = 7;
                                            else if (ItemQuality == "Refined")
                                                Quality = 6;
                                            else if (ItemQuality == "Elite")
                                                Quality = 8;
                                            else if (ItemQuality == "Super")
                                                Quality = 9;
                                            else
                                                Quality = (byte)Other.ItemQuality(ItemId);

                                            ItemId = Other.ItemQualityChange(ItemId, Quality);

                                            if (MyChar.ItemsInInventory < 40)
                                                MyChar.AddItem(ItemId.ToString() + "-" + Plus.ToString() + "-" + Bless.ToString() + "-" + Enchant.ToString() + "-" + Soc1.ToString() + "-" + Soc2.ToString(), 0, (uint)General.Rand.Next(57458353));
                                        }
                                        if (Splitter[0] == "/clearpkp")
                                        {
                                            MyChar.PKPoints = 0;
                                            World.UpdateSpawn(MyChar);
                                            SendPacket(General.MyPackets.CharacterInfo(MyChar));
                                            MyChar.SendEquips(false);
                                            SendPacket(General.MyPackets.Vital(MyChar.UID, 26, MyChar.GetStat()));
                                        }
                                        if (Splitter[0] == "/pkp")
                                        {
                                            MyChar.PKPoints = 100;
                                            World.UpdateSpawn(MyChar);
                                            SendPacket(General.MyPackets.CharacterInfo(MyChar));
                                            MyChar.SendEquips(false);
                                            SendPacket(General.MyPackets.Vital(MyChar.UID, 26, MyChar.GetStat()));
                                        }
                                        if (Splitter[0] == "/kick")
                                        {
                                            foreach (DictionaryEntry DE in World.AllChars)
                                            {
                                                Character Char = (Character)DE.Value;

                                                if (Char.Name == Splitter[1])
                                                {
                                                    World.SendMsgToAll(Splitter[1] + " has been kicked by " + MyChar.Name, "SYSTEM", 2011);
                                                    Char.MyClient.Drop();
                                                }
                                                else
                                                {
                                                    SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Sorry the Character:" + Splitter[1] + " is offline...Please try again later", 2000));
                                                }
                                            }
                                        }
                                        if (Splitter[0] == "/ban")
                                        {
                                            foreach (DictionaryEntry DE in World.AllChars)
                                            {
                                                Character Char = (Character)DE.Value;

                                                if (Char.Name == Splitter[1])
                                                {
                                                    World.SendMsgToAll(Splitter[1] + " has been banned by " + MyChar.Name, "SYSTEM", 2011);
                                                    ExternalDatabase.Ban(Char.MyClient.Account);
                                                    Char.MyClient.Drop();
                                                }
                                                else
                                                {
                                                    SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Sorry the Character:" + Splitter[1] + " is offline...Please try again later", 2000));
                                                }
                                            }
                                        }
                                        if (Splitter[0] == "/effect")
                                        {
                                            SendPacket(General.MyPackets.String(MyChar.UID, 10, Splitter[1]));
                                        }
                                        if (Splitter[0] == "/level")
                                        {
                                            byte NewLvl = byte.Parse(Splitter[1]);
                                            MyChar.Level = NewLvl;
                                            MyChar.Exp = 0;
                                            InternalDatabase.GetStats(MyChar);
                                            MyChar.GetEquipStats(1, true);
                                            MyChar.GetEquipStats(2, true);
                                            MyChar.GetEquipStats(3, true);
                                            MyChar.GetEquipStats(4, true);
                                            MyChar.GetEquipStats(5, true);
                                            MyChar.GetEquipStats(6, true);
                                            MyChar.GetEquipStats(7, true);
                                            MyChar.GetEquipStats(8, true);
                                            MyChar.MinAtk = MyChar.Str;
                                            MyChar.MaxAtk = MyChar.Str;
                                            MyChar.MaxHP = MyChar.BaseMaxHP();
                                            MyChar.Potency = MyChar.Level;
                                            MyChar.GetEquipStats(1, false);
                                            MyChar.GetEquipStats(2, false);
                                            MyChar.GetEquipStats(3, false);
                                            MyChar.GetEquipStats(4, false);
                                            MyChar.GetEquipStats(5, false);
                                            MyChar.GetEquipStats(6, false);
                                            MyChar.GetEquipStats(7, false);
                                            MyChar.GetEquipStats(8, false);
                                            MyChar.CurHP = MyChar.MaxHP;
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 13, MyChar.Level));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 16, MyChar.Str));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 17, MyChar.Agi));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 15, MyChar.Vit));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 14, MyChar.Spi));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 5, MyChar.Exp));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 2, MyChar.MaxMana()));
                                            SendPacket(General.MyPackets.GeneralData((long)MyChar.UID, 0, 0, 0, 92));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 0, MyChar.CurHP));
                                            if (MyChar.MyGuild != null)
                                                MyChar.MyGuild.Refresh(MyChar);
                                        }
                                        if (Splitter[0] == "/job")
                                        {
                                            byte NewJob = byte.Parse(Splitter[1]);
                                            MyChar.Job = NewJob;
                                            InternalDatabase.GetStats(MyChar);
                                            MyChar.GetEquipStats(1, true);
                                            MyChar.GetEquipStats(2, true);
                                            MyChar.GetEquipStats(3, true);
                                            MyChar.GetEquipStats(4, true);
                                            MyChar.GetEquipStats(5, true);
                                            MyChar.GetEquipStats(6, true);
                                            MyChar.GetEquipStats(7, true);
                                            MyChar.GetEquipStats(8, true);
                                            MyChar.MinAtk = MyChar.Str;
                                            MyChar.MaxAtk = MyChar.Str;
                                            MyChar.MaxHP = MyChar.BaseMaxHP();
                                            MyChar.Potency = MyChar.Level;
                                            MyChar.GetEquipStats(1, false);
                                            MyChar.GetEquipStats(2, false);
                                            MyChar.GetEquipStats(3, false);
                                            MyChar.GetEquipStats(4, false);
                                            MyChar.GetEquipStats(5, false);
                                            MyChar.GetEquipStats(6, false);
                                            MyChar.GetEquipStats(7, false);
                                            MyChar.GetEquipStats(8, false);
                                            MyChar.CurHP = MyChar.MaxHP;
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 7, MyChar.Job));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 16, MyChar.Str));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 17, MyChar.Agi));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 15, MyChar.Vit));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 14, MyChar.Spi));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 2, MyChar.MaxMana()));
                                            SendPacket(General.MyPackets.GeneralData((long)MyChar.UID, 0, 0, 0, 92));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 0, MyChar.CurHP));
                                        }
                                        if (Splitter[0] == "/prof")
                                        {
                                            if (MyChar.Profs.Contains(short.Parse(Splitter[1])))
                                                MyChar.Profs.Remove(short.Parse(Splitter[1]));

                                            if (MyChar.Prof_Exps.Contains(short.Parse(Splitter[1])))
                                                MyChar.Prof_Exps.Remove(short.Parse(Splitter[1]));

                                            MyChar.Profs.Add(short.Parse(Splitter[1]), byte.Parse(Splitter[2]));
                                            MyChar.Prof_Exps.Add(short.Parse(Splitter[1]), uint.Parse("0"));
                                            SendPacket(General.MyPackets.Prof(short.Parse(Splitter[1]), byte.Parse(Splitter[2]), 0));
                                        }
                                        if (Splitter[0] == "/gold")
                                        {
                                            uint NewSilvers = uint.Parse(Splitter[1]);

                                            MyChar.Silvers = NewSilvers;
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 4, MyChar.Silvers));
                                        }
                                        if (Splitter[0] == "/cps")
                                        {
                                            uint NewCPs = uint.Parse(Splitter[1]);

                                            MyChar.CPs = NewCPs;
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 30, MyChar.CPs));
                                        }
                                        if (Splitter[0] == "/gm")
                                        {
                                            if (Splitter[1] == "tro")
                                            {
                                                MyChar.AddItem("420339-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));
                                                MyChar.AddItem("410339-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));
                                                MyChar.AddItem("480339-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));
                                                MyChar.AddItem("160249-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));//blizzard
                                                MyChar.AddItem("120249-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));//tornado
                                                MyChar.AddItem("150249-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));//thunder
                                                MyChar.AddItem("112389-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));
                                                MyChar.AddItem("135299-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));
                                            }
                                            if (Splitter[1] == "tao")
                                            {
                                                MyChar.AddItem("139299-12-7-255-3-3", 0, (uint)General.Rand.Next(57458353));
                                                MyChar.AddItem("112349-12-7-255-3-3", 0, (uint)General.Rand.Next(57458353));
                                                MyChar.AddItem("121249-12-7-255-3-3", 0, (uint)General.Rand.Next(57458353));
                                                MyChar.AddItem("152259-12-7-255-3-3", 0, (uint)General.Rand.Next(57458353));
                                                MyChar.AddItem("160249-12-7-255-3-3", 0, (uint)General.Rand.Next(57458353));//blizzard
                                                MyChar.AddItem("421339-12-7-255-3-3", 0, (uint)General.Rand.Next(57458353));
                                            }
                                            if (Splitter[1] == "arch")
                                            {
                                                MyChar.AddItem("138299-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));
                                                MyChar.AddItem("160249-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));//blizzard
                                                MyChar.AddItem("120249-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));//tornado
                                                MyChar.AddItem("150249-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));//thunder
                                                MyChar.AddItem("112339-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));
                                                MyChar.AddItem("500329-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));
                                                MyChar.AddItem("1050002-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));

                                            }
                                            if (Splitter[1] == "war")
                                            {
                                                MyChar.AddItem("480339-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));
                                                MyChar.AddItem("136299-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));
                                                MyChar.AddItem("160249-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));//blizzard
                                                MyChar.AddItem("120249-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));//tornado
                                                MyChar.AddItem("150249-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));//thunder
                                                MyChar.AddItem("112419-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));
                                                MyChar.AddItem("560339-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));
                                                MyChar.AddItem("561339-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));
                                                MyChar.AddItem("900399-12-7-255-13-13", 0, (uint)General.Rand.Next(57458353));
                                            }
                                        }
                                        if (Splitter[0] == "/killmap")
                                        {
                                            foreach (ushort[] revp in ExternalDatabase.RevPoints)
                                                foreach (DictionaryEntry DE in World.AllChars)
                                                {
                                                    Character Char = (Character)DE.Value;

                                                    int EModel;
                                                    if (Char.Model == 1003 || Char.Model == 1004)
                                                        EModel = 15099;
                                                    else
                                                        EModel = 15199;

                                                    if (revp[0] == Char.LocMap)
                                                    {
                                                        Char.Die();
                                                        Char.CurHP = 0;
                                                        Char.Death = DateTime.Now;
                                                        Char.Alive = false;
                                                        Char.XpList = false;
                                                        Char.SMOn = false;
                                                        Char.CycloneOn = false;
                                                        Char.XpCircle = 0;
                                                        Char.Flying = false;
                                                        Char.MyClient.SendPacket(General.MyPackets.Vital(Char.UID, 26, Char.GetStat()));
                                                        Char.MyClient.SendPacket(General.MyPackets.Status1(Char.UID, EModel));
                                                        Char.MyClient.SendPacket(General.MyPackets.Death(Char));
                                                        Char.MyClient.SendPacket(General.MyPackets.SendMsg(MyChar.MyClient.MessageId, "SYSTEM", MyChar.Name, "Map ID " + revp[0] + " Has just been killed", 2011));
                                                        World.UpdateSpawn(Char);
                                                    }
                                                }
                                        }

                                        // Shutdown Commands
                                    }
                                    #endregion
                                    #region PMcommands
                                    if (Status == 7)
                                    {
                                        if (Splitter[0] == "/refresh")
                                        {
                                            MyChar.Teleport(MyChar.LocMap, MyChar.LocX, MyChar.LocY);
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "area updated!!", 2005));
                                        }
                                        if (Splitter[0] == "/mm")
                                        {
                                            MyChar.Teleport(ushort.Parse(Splitter[1]), ushort.Parse(Splitter[2]), ushort.Parse(Splitter[3]));
                                        }
                                        if (Splitter[0] == "/setpkp")
                                        {
                                            ushort PKPoints = ushort.Parse(Splitter[1]);
                                            MyChar.PKPoints = PKPoints;
                                            World.UpdateSpawn(MyChar);
                                            SendPacket(General.MyPackets.CharacterInfo(MyChar));
                                            MyChar.SendEquips(false);
                                            SendPacket(General.MyPackets.Vital(MyChar.UID, 26, MyChar.GetStat()));

                                        }
                                        if (Splitter[0] == "/kick")
                                        {
                                            foreach (DictionaryEntry DE in World.AllChars)
                                            {
                                                Character Char = (Character)DE.Value;

                                                if (Char.Name == Splitter[1])
                                                {
                                                    World.SendMsgToAll(Splitter[1] + " has been kicked by " + MyChar.Name, "SYSTEM", 2011);
                                                    Char.MyClient.Drop();
                                                }
                                            }
                                        }
                                        if (Splitter[0] == "/item")
                                        {
                                            Ini ItemNames = new Ini(System.Windows.Forms.Application.StartupPath + @"\ItemNamesToId.ini");
                                            string ItemName = Splitter[2];
                                            string ItemQuality = Splitter[1];
                                            byte Plus = byte.Parse(Splitter[3]);
                                            byte Bless = byte.Parse(Splitter[4]);
                                            byte Enchant = byte.Parse(Splitter[5]);
                                            byte Soc1 = byte.Parse(Splitter[6]);
                                            byte Soc2 = byte.Parse(Splitter[7]);

                                            uint ItemId = 0;
                                            ItemId = uint.Parse(ItemNames.ReadValue("Items", ItemName));

                                            if (ItemId == 0)
                                                return;

                                            byte Quality = 1;

                                            if (ItemQuality == "One")
                                                Quality = 1;
                                            else if (ItemQuality == "Normal")
                                                Quality = 5;
                                            else if (ItemQuality == "Unique")
                                                Quality = 7;
                                            else if (ItemQuality == "Refined")
                                                Quality = 6;
                                            else if (ItemQuality == "Elite")
                                                Quality = 8;
                                            else if (ItemQuality == "Super")
                                                Quality = 9;
                                            else
                                                Quality = (byte)Other.ItemQuality(ItemId);

                                            ItemId = Other.ItemQualityChange(ItemId, Quality);

                                            if (MyChar.ItemsInInventory < 40)
                                                MyChar.AddItem(ItemId.ToString() + "-" + Plus.ToString() + "-" + Bless.ToString() + "-" + Enchant.ToString() + "-" + Soc1.ToString() + "-" + Soc2.ToString(), 0, (uint)General.Rand.Next(57458353));
                                        }
                                        if (Splitter[0] == "/sr")// restarts the server from ingame
                                        {
                                            World.SendMsgToAll("Server Restarting!", "SYSTEM", 2011);
                                            Console.WriteLine("Server Restarting from In-Game");
                                            General.ServerRestart();
                                        }
                                        if (Splitter[0] == "/killmap")
                                        {
                                            foreach (ushort[] revp in ExternalDatabase.RevPoints)
                                                foreach (DictionaryEntry DE in World.AllChars)
                                                {
                                                    Character Char = (Character)DE.Value;

                                                    int EModel;
                                                    if (Char.Model == 1003 || Char.Model == 1004)
                                                        EModel = 15099;
                                                    else
                                                        EModel = 15199;

                                                    if (revp[0] == Char.LocMap)
                                                    {
                                                        Char.Die();
                                                        Char.CurHP = 0;
                                                        Char.Death = DateTime.Now;
                                                        Char.Alive = false;
                                                        Char.XpList = false;
                                                        Char.SMOn = false;
                                                        Char.CycloneOn = false;
                                                        Char.XpCircle = 0;
                                                        Char.Flying = false;
                                                        Char.MyClient.SendPacket(General.MyPackets.Vital(Char.UID, 26, Char.GetStat()));
                                                        Char.MyClient.SendPacket(General.MyPackets.Status1(Char.UID, EModel));
                                                        Char.MyClient.SendPacket(General.MyPackets.Death(Char));
                                                        Char.MyClient.SendPacket(General.MyPackets.SendMsg(MyChar.MyClient.MessageId, "SYSTEM", MyChar.Name, "Map ID " + revp[0] + " Has just been killed", 2011));
                                                        World.UpdateSpawn(Char);
                                                    }
                                                }
                                        }
                                        if (Splitter[0] == "/effect")
                                        {
                                            SendPacket(General.MyPackets.String(MyChar.UID, 10, Splitter[1]));
                                        }
                                        if (Splitter[0] == "/xp")
                                        {
                                            MyChar.XpList = true;
                                            SendPacket(General.MyPackets.Vital(MyChar.UID, 26, MyChar.GetStat()));
                                        }
                                        if (Splitter[0] == "/ban")
                                        {
                                            foreach (DictionaryEntry DE in World.AllChars)
                                            {
                                                Character Char = (Character)DE.Value;

                                                if (Char.Name == Splitter[1])
                                                {
                                                    World.SendMsgToAll(Splitter[1] + " has been banned by " + MyChar.Name, "SYSTEM", 2011);
                                                    ExternalDatabase.Ban(Char.MyClient.Account);
                                                    Char.MyClient.Drop();
                                                }
                                                else
                                                {
                                                    SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Sorry the Character:" + Splitter[1] + " is offline...Please try again later", 2000));
                                                }
                                            }
                                        }
                                        if (Splitter[0] == "/clearpkp")
                                        {
                                            MyChar.PKPoints = 0;
                                            World.UpdateSpawn(MyChar);
                                            SendPacket(General.MyPackets.CharacterInfo(MyChar));
                                            MyChar.SendEquips(false);
                                            SendPacket(General.MyPackets.Vital(MyChar.UID, 26, MyChar.GetStat()));
                                        }
                                        if (Splitter[0] == "/pkp")
                                        {
                                            MyChar.PKPoints = 100;
                                            World.UpdateSpawn(MyChar);
                                            SendPacket(General.MyPackets.CharacterInfo(MyChar));
                                            MyChar.SendEquips(false);
                                            SendPacket(General.MyPackets.Vital(MyChar.UID, 26, MyChar.GetStat()));
                                        }
                                        if (Splitter[0] == "/level")
                                        {
                                            byte NewLvl = byte.Parse(Splitter[1]);
                                            MyChar.Level = NewLvl;
                                            MyChar.Exp = 0;
                                            InternalDatabase.GetStats(MyChar);
                                            MyChar.GetEquipStats(1, true);
                                            MyChar.GetEquipStats(2, true);
                                            MyChar.GetEquipStats(3, true);
                                            MyChar.GetEquipStats(4, true);
                                            MyChar.GetEquipStats(5, true);
                                            MyChar.GetEquipStats(6, true);
                                            MyChar.GetEquipStats(7, true);
                                            MyChar.GetEquipStats(8, true);
                                            MyChar.MinAtk = MyChar.Str;
                                            MyChar.MaxAtk = MyChar.Str;
                                            MyChar.MaxHP = MyChar.BaseMaxHP();
                                            MyChar.Potency = MyChar.Level;
                                            MyChar.GetEquipStats(1, false);
                                            MyChar.GetEquipStats(2, false);
                                            MyChar.GetEquipStats(3, false);
                                            MyChar.GetEquipStats(4, false);
                                            MyChar.GetEquipStats(5, false);
                                            MyChar.GetEquipStats(6, false);
                                            MyChar.GetEquipStats(7, false);
                                            MyChar.GetEquipStats(8, false);
                                            MyChar.CurHP = MyChar.MaxHP;
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 13, MyChar.Level));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 16, MyChar.Str));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 17, MyChar.Agi));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 15, MyChar.Vit));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 14, MyChar.Spi));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 5, MyChar.Exp));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 2, MyChar.MaxMana()));
                                            SendPacket(General.MyPackets.GeneralData((long)MyChar.UID, 0, 0, 0, 92));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 0, MyChar.CurHP));
                                            if (MyChar.MyGuild != null)
                                                MyChar.MyGuild.Refresh(MyChar);
                                        }
                                        if (Splitter[0] == "/skill")
                                        {
                                            MyChar.LearnSkill2(short.Parse(Splitter[1]), byte.Parse(Splitter[2]));
                                        }
                                        if (Splitter[0] == "/job")
                                        {
                                            byte NewJob = byte.Parse(Splitter[1]);
                                            MyChar.Job = NewJob;
                                            InternalDatabase.GetStats(MyChar);
                                            MyChar.GetEquipStats(1, true);
                                            MyChar.GetEquipStats(2, true);
                                            MyChar.GetEquipStats(3, true);
                                            MyChar.GetEquipStats(4, true);
                                            MyChar.GetEquipStats(5, true);
                                            MyChar.GetEquipStats(6, true);
                                            MyChar.GetEquipStats(7, true);
                                            MyChar.GetEquipStats(8, true);
                                            MyChar.MinAtk = MyChar.Str;
                                            MyChar.MaxAtk = MyChar.Str;
                                            MyChar.MaxHP = MyChar.BaseMaxHP();
                                            MyChar.Potency = MyChar.Level;
                                            MyChar.GetEquipStats(1, false);
                                            MyChar.GetEquipStats(2, false);
                                            MyChar.GetEquipStats(3, false);
                                            MyChar.GetEquipStats(4, false);
                                            MyChar.GetEquipStats(5, false);
                                            MyChar.GetEquipStats(6, false);
                                            MyChar.GetEquipStats(7, false);
                                            MyChar.GetEquipStats(8, false);
                                            MyChar.CurHP = MyChar.MaxHP;
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 7, MyChar.Job));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 16, MyChar.Str));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 17, MyChar.Agi));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 15, MyChar.Vit));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 14, MyChar.Spi));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 2, MyChar.MaxMana()));
                                            SendPacket(General.MyPackets.GeneralData((long)MyChar.UID, 0, 0, 0, 92));
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 0, MyChar.CurHP));
                                        }
                                        if (Splitter[0] == "/prof")
                                        {
                                            if (MyChar.Profs.Contains(short.Parse(Splitter[1])))
                                                MyChar.Profs.Remove(short.Parse(Splitter[1]));

                                            if (MyChar.Prof_Exps.Contains(short.Parse(Splitter[1])))
                                                MyChar.Prof_Exps.Remove(short.Parse(Splitter[1]));

                                            MyChar.Profs.Add(short.Parse(Splitter[1]), byte.Parse(Splitter[2]));
                                            MyChar.Prof_Exps.Add(short.Parse(Splitter[1]), uint.Parse("0"));
                                            SendPacket(General.MyPackets.Prof(short.Parse(Splitter[1]), byte.Parse(Splitter[2]), 0));
                                        }
                                        if (Splitter[0] == "/gold")
                                        {
                                            uint NewSilvers = uint.Parse(Splitter[1]);

                                            MyChar.Silvers = NewSilvers;
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 4, MyChar.Silvers));
                                        }
                                        if (Splitter[0] == "/cps")
                                        {
                                            uint NewCPs = uint.Parse(Splitter[1]);

                                            MyChar.CPs = NewCPs;
                                            SendPacket(General.MyPackets.Vital((long)MyChar.UID, 30, MyChar.CPs));
                                        }
                                        if (Splitter[0] == "/gmc")
                                        {
                                            Message = Message.Remove(0, 2);
                                            World.SendMsgToAll(Message, MyChar.Name, 2011);
                                        }
                                        if (Splitter[0] == "/info")
                                        {
                                            string BackMsg = "";
                                            foreach (DictionaryEntry DE in World.AllChars)
                                            {
                                                Character Char = (Character)DE.Value;
                                                BackMsg += Char.Name + ", ";
                                            }
                                            if (BackMsg.Length > 1)
                                                BackMsg = BackMsg.Remove(BackMsg.Length - 2, 2);
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Players Online: " + World.AllChars.Count, 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, BackMsg, 2000));
                                        }
                                        if (Splitter[0] == "/changename")
                                        {
                                            string newname = Splitter[1];
                                            MyChar.Name = newname;
                                            SendPacket(General.MyPackets.CharacterInfo(MyChar));
                                            World.SpawnMeToOthers(MyChar, false);
                                            MyChar.SendEquips(false);
                                        }
                                        if (Splitter[0] == "/recall")
                                        {
                                            foreach (DictionaryEntry DE in World.AllChars)
                                            {
                                                Character Char = (Character)DE.Value;
                                                if (Char.Name == Splitter[1])
                                                {
                                                    Char.Teleport(MyChar.LocMap, MyChar.LocX, MyChar.LocY);
                                                    break;
                                                }
                                            }
                                        }
                                        if (Splitter[0] == "/goto")
                                        {
                                            foreach (DictionaryEntry DE in World.AllChars)
                                            {
                                                Character Char = (Character)DE.Value;
                                                if (Char.Name == Splitter[1])
                                                {
                                                    MyChar.Teleport(Char.LocMap, Char.LocX, Char.LocY);
                                                    break;
                                                }
                                            }
                                        }
                                        if (Splitter[0] == "/gwstart")
                                        {
                                            World.GWOn = true;
                                            World.SendMsgToAll("GuildWar has started!", "SYSTEM", 2011);
                                        }
                                        if (Splitter[0] == "gwend")
                                        {
                                            World.GWOn = false;
                                            World.SendMsgToAll("GuildWar has ended!", "SYSTEM", 2011);
                                        }
                                        if (Splitter[0] == "/dc")
                                        {
                                            Drop();
                                        }
                                        if (Splitter[0] == "/scroll")
                                        {
                                            if (Splitter[1] == "pc")
                                                MyChar.Teleport(1011, 188, 264);
                                            if (Splitter[1] == "dc")
                                                MyChar.Teleport(1000, 500, 650);
                                            if (Splitter[1] == "bi")
                                                MyChar.Teleport(1015, 723, 573);
                                            if (Splitter[1] == "am")
                                                MyChar.Teleport(1020, 565, 562);
                                            if (Splitter[1] == "ma")
                                                MyChar.Teleport(1036, 198, 194);
                                            if (Splitter[1] == "tc")
                                                MyChar.Teleport(1002, 429, 378);
                                        }
                                    }
                                    #endregion
                                    #region phhelpcommands
                                    if (Status == 7)
                                    {
                                        if (Splitter[0] == "/help")
                                        {
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "The List of Commands", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "For traveling commands type /travel", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "For controlling other players type /control", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "For personal commands type /personal", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "For misc. commands type /other", 2000));
                                        }
                                        if (Splitter[0] == "/travel")
                                        {
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Traveling Commands", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/scroll", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/recall", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/goto", 2000));
                                        }
                                        if (Splitter[0] == "/control")
                                        {
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Control Commands", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/kick", 2000));
                                        }
                                        if (Splitter[0] == "/personal")
                                        {
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Personal Commands", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/level", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/item", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/silvers", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/cps", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/changename", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/job", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/skill", 2000));

                                        }
                                        if (Splitter[0] == "/other")
                                        {
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Other Commands", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/c", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/info", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/dc", 2000));
                                        }
                                    }
                                    #endregion
                                    #region gmhelpcommands
                                    if (Status == 8)
                                    {
                                        if (Splitter[0] == "/help")
                                        {
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "The List of Commands", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "For traveling commands type /travel", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "For controlling other players type /control", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "For personal commands type /personal", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "For personal commands type /server", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "For misc. commands type /other", 2000));
                                        }
                                        if (Splitter[0] == "/travel")
                                        {
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Traveling Commands", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/scroll", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/recall", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/goto", 2000));
                                        }
                                        if (Splitter[0] == "/control")
                                        {
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Control Commands", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/kick", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/ban", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/ipban", 2000));
                                        }
                                        if (Splitter[0] == "/personal")
                                        {
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Personal Commands", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/level", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/silvers", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/cps", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/changename", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/job", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/item", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/kill", 2000));
                                        }
                                        if (Splitter[0] == "/other")
                                        {
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Other Commands", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/c", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/info", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/dc", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/killmap", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/gwstart", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/gwstop", 2000));
                                        }
                                        if (Splitter[0] == "/server")
                                        {
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Server Commands", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/shutdownserver", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/stopshutdown", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/updaterestart", 2000));
                                            SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "/stoprestart", 2000));
                                        }
                                    }
                                    #endregion
                                }
                            }
                        #endregion
                            catch { }
                        }
                        else
                        {
                            World.Chat(MyChar, ChatType, Data, To, Message);
                        }
                        if (ChatType == 2111)
                            if (MyChar.MyGuild != null)
                            {
                                MyChar.MyGuild.Bulletin = Message;
                                SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, MyChar.MyGuild.Bulletin, 2111));
                            }
                        break;
                    }
                #endregion
                #region whmoney
                case 1009:
                    {
                        PacketType = Data[0x0c];
                        switch (PacketType)
                        {
                            case 9:
                                {
                                    uint NPCID = (uint)((Data[7] << 24) + (Data[6] << 16) + (Data[5] << 8) + (Data[4]));

                                    byte WH = 0;

                                    if (NPCID == 8)
                                        WH = 0;
                                    else if (NPCID == 81)
                                        WH = 1;
                                    else if (NPCID == 82)
                                        WH = 2;
                                    else if (NPCID == 83)
                                        WH = 3;
                                    else if (NPCID == 84)
                                        WH = 4;
                                    else if (NPCID == 85)
                                        WH = 5;

                                    SendPacket(General.MyPackets.OpenWarehouse(NPCID, MyChar.WHSilvers));
                                    SendPacket(General.MyPackets.WhItems(MyChar, WH, (ushort)NPCID));
                                    break;
                                }
                            case 10://Deposit
                                {
                                    uint Amount = (uint)((Data[11] << 24) + (Data[10] << 16) + (Data[9] << 8) + (Data[8]));

                                    if (MyChar.Silvers >= Amount)
                                    {
                                        MyChar.Silvers -= Amount;
                                        MyChar.WHSilvers += Amount;
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 10, MyChar.WHSilvers));
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    }
                                    break;
                                }
                            case 11://Withdraw
                                {
                                    uint Amount = (uint)((Data[11] << 24) + (Data[10] << 16) + (Data[9] << 8) + (Data[8]));

                                    if (MyChar.WHSilvers >= Amount)
                                    {
                                        MyChar.Silvers += Amount;
                                        MyChar.WHSilvers -= Amount;
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 10, MyChar.WHSilvers));
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                    }

                                    break;
                                }
                #endregion
                            #region itemupgrade
                            case 20:
                                {
                                    MyChar.Ready = false;
                                    uint UppedItemUID = (uint)((Data[7] << 24) + (Data[6] << 16) + (Data[5] << 8) + Data[4]);
                                    uint UppingItemUID = (uint)((Data[11] << 24) + (Data[10] << 16) + (Data[9] << 8) + Data[8]);

                                    string UppedItem = "";
                                    string UppingItem = "";
                                    int Counter = 0;

                                    foreach (uint uid in MyChar.Inventory_UIDs)
                                    {
                                        if (uid == UppedItemUID)
                                            UppedItem = MyChar.Inventory[Counter];

                                        Counter++;
                                    }

                                    Counter = 0;

                                    foreach (uint uid in MyChar.Inventory_UIDs)
                                    {
                                        if (uid == UppingItemUID)
                                            UppingItem = MyChar.Inventory[Counter];

                                        Counter++;
                                    }

                                    string[] Splitter = UppedItem.Split('-');
                                    uint UppedItem2 = uint.Parse(Splitter[0]);
                                    string[] Splitter2 = UppingItem.Split('-');
                                    uint UppingItem2 = uint.Parse(Splitter2[0]);

                                    if (UppingItem2 == 1088001)
                                        if (Other.Upgradable(UppedItem2))
                                            if (Other.ItemInfo(UppedItem2)[3] < 130)
                                                if (Other.ItemType2(UppedItem2) == 11 && Other.WeaponType(UppedItem2) != 117 && Other.ItemInfo(UppedItem2)[3] < 120 || Other.WeaponType(UppedItem2) == 117 && Other.ItemInfo(UppedItem2)[3] < 112 || Other.ItemType2(UppedItem2) == 13 && Other.ItemInfo(UppedItem2)[3] < 120 || Other.ItemType2(UppedItem2) == 15 && Other.ItemInfo(UppedItem2)[3] < 127 || Other.ItemType2(UppedItem2) == 16 && Other.ItemInfo(UppedItem2)[3] < 129 || Other.ItemType(UppedItem2) == 4 || Other.ItemType(UppedItem2) == 5 || Other.ItemType2(UppedItem2) == 12 || Other.WeaponType(UppedItem2) == 132 && Other.ItemInfo(UppedItem2)[3] <= 12)
                                                {
                                                    bool Success = false;
                                                    double RemoveChance = 0;

                                                    RemoveChance = Other.ItemInfo(UppedItem2)[3] / 3;

                                                    if (Other.ItemQuality(UppedItem2) == 3 || Other.ItemQuality(UppedItem2) == 4 || Other.ItemQuality(UppedItem2) == 5)
                                                        if (Other.ChanceSuccess(90 - RemoveChance))
                                                            Success = true;

                                                    if (Other.ItemQuality(UppedItem2) == 6)
                                                        if (Other.ChanceSuccess(75 - RemoveChance))
                                                            Success = true;

                                                    if (Other.ItemQuality(UppedItem2) == 7)
                                                        if (Other.ChanceSuccess(60 - RemoveChance))
                                                            Success = true;

                                                    if (Other.ItemQuality(UppedItem2) == 8)
                                                        if (Other.ChanceSuccess(50 - RemoveChance))
                                                            Success = true;

                                                    if (Other.ItemQuality(UppedItem2) == 9)
                                                        if (Other.ChanceSuccess(45 - RemoveChance))
                                                            Success = true;

                                                    if (Success)
                                                    {
                                                        MyChar.RemoveItem((ulong)UppedItemUID);

                                                        UppedItem2 = Other.EquipNextLevel(UppedItem2);

                                                        if (Splitter[4] == "0")
                                                            if (Other.ChanceSuccess(0.5))
                                                                Splitter[4] = "255";

                                                        if (Splitter[5] == "0")
                                                            if (Splitter[4] != "0")
                                                                if (Other.ChanceSuccess(0.25))
                                                                    Splitter[5] = "255";

                                                        MyChar.AddItem(Convert.ToString(UppedItem2) + "-" + Splitter[1] + "-" + Splitter[2] + "-" + Splitter[3] + "-" + Splitter[4] + "-" + Splitter[5], 0, UppedItemUID);
                                                    }

                                                    MyChar.RemoveItem((ulong)UppingItemUID);
                                                }
                                    MyChar.Ready = true;
                                    break;
                                }
                            case 19:
                                {
                                    MyChar.Ready = false;
                                    uint UppedItemUID = (uint)((Data[7] << 24) + (Data[6] << 16) + (Data[5] << 8) + Data[4]);
                                    uint UppingItemUID = (uint)((Data[11] << 24) + (Data[10] << 16) + (Data[9] << 8) + Data[8]);

                                    string UppedItem = "";
                                    string UppingItem = "";
                                    int Counter = 0;

                                    foreach (uint uid in MyChar.Inventory_UIDs)
                                    {
                                        if (uid == UppedItemUID)
                                            UppedItem = MyChar.Inventory[Counter];

                                        Counter++;
                                    }

                                    Counter = 0;

                                    foreach (uint uid in MyChar.Inventory_UIDs)
                                    {
                                        if (uid == UppingItemUID)
                                            UppingItem = MyChar.Inventory[Counter];

                                        Counter++;
                                    }

                                    string[] Splitter = UppedItem.Split('-');
                                    uint UppedItem2 = uint.Parse(Splitter[0]);
                                    string[] Splitter2 = UppingItem.Split('-');
                                    uint UppingItem2 = uint.Parse(Splitter2[0]);

                                    if (UppingItem2 == 1088000)
                                        if (Other.Upgradable(UppedItem2))
                                            if (Other.ItemQuality(UppedItem2) != 9)
                                            {
                                                bool Success = false;
                                                double RemoveChance = 0;

                                                RemoveChance = Other.ItemInfo(UppedItem2)[3] / 3;

                                                if (Other.ItemQuality(UppedItem2) == 3 || Other.ItemQuality(UppedItem2) == 4 || Other.ItemQuality(UppedItem2) == 5)
                                                    if (Other.ChanceSuccess(64 - RemoveChance))
                                                        Success = true;

                                                if (Other.ItemQuality(UppedItem2) == 6)
                                                    if (Other.ChanceSuccess(54 - RemoveChance))
                                                        Success = true;

                                                if (Other.ItemQuality(UppedItem2) == 7)
                                                    if (Other.ChanceSuccess(48 - RemoveChance))
                                                        Success = true;

                                                if (Other.ItemQuality(UppedItem2) == 8)
                                                    if (Other.ChanceSuccess(44 - RemoveChance))
                                                        Success = true;

                                                if (Success)
                                                {
                                                    if (Other.ItemQuality(UppedItem2) == 3 || Other.ItemQuality(UppedItem2) == 4)
                                                        UppedItem2 += 6 - (uint)Other.ItemQuality(UppedItem2);
                                                    else
                                                        UppedItem2 += 1;

                                                    MyChar.RemoveItem((ulong)UppedItemUID);

                                                    if (Splitter[4] == "0")
                                                        if (Other.ChanceSuccess(1))
                                                            Splitter[4] = "255";

                                                    if (Splitter[5] == "0")
                                                        if (Splitter[4] != "0")
                                                            if (Other.ChanceSuccess(0.5))
                                                                Splitter[5] = "255";

                                                    MyChar.AddItem(Convert.ToString(UppedItem2) + "-" + Splitter[1] + "-" + Splitter[2] + "-" + Splitter[3] + "-" + Splitter[4] + "-" + Splitter[5], 0, UppedItemUID);
                                                }

                                                MyChar.RemoveItem((uint)UppingItemUID);
                                            }
                                    MyChar.Ready = true;
                                    break;
                                }
                            #endregion
                            #region itemdrops
                            case 27:
                                {
                                    SendPacket(Data);
                                    break;
                                }
                            case 3:
                                {
                                    MyChar.Ready = false;
                                    uint ItemUID = (uint)((Data[7] << 24) + (Data[6] << 16) + (Data[5] << 8) + Data[4]);

                                    int Count = 0;

                                    foreach (uint uid in MyChar.Inventory_UIDs)
                                    {
                                        if (uid == ItemUID)
                                        {
                                            string Item = MyChar.Inventory[Count];

                                            DroppedItem e = DroppedItems.DropItem(Item, MyChar.LocX, MyChar.LocY, MyChar.LocMap, 0);
                                            World.ItemDrops(e);

                                            MyChar.RemoveItem(ItemUID);
                                        }
                                        Count++;
                                    }
                                    MyChar.Ready = true;
                                    break;
                                }
                            #endregion
                            #region invitems
                            case 2:
                                {
                                    MyChar.Ready = false;
                                    uint ItemUID = (uint)((Data[11] << 24) + (Data[10] << 16) + (Data[9] << 8) + Data[8]);
                                    int Count = 0;
                                    foreach (uint uid in MyChar.Inventory_UIDs)
                                    {
                                        if (uid == ItemUID)
                                        {
                                            string Item = MyChar.Inventory[Count];
                                            string[] Splitter = Item.Split('-');
                                            uint ItemId = uint.Parse(Splitter[0]);

                                            foreach (uint[] item in ExternalDatabase.Items)
                                            {
                                                if (item[0] == ItemId)
                                                {
                                                    uint SellFor = item[7] / 3;

                                                    MyChar.RemoveItem(ItemUID);
                                                    MyChar.Silvers += SellFor;

                                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                                    break;
                                                }
                                            }
                                            break;
                                        }
                                        Count++;
                                    }
                                    MyChar.Ready = true;
                                    break;
                                }
                            #endregion
                            #region shops
                            case 1:
                                {
                                    MyChar.Ready = false;
                                    uint ItemID = (uint)((Data[0x0b] << 24) + (Data[0x0a] << 16) + (Data[0x09] << 8) + Data[0x08]);
                                    uint CPsVal = Data[18];
                                    uint Value = (uint)((Data[0x07] << 24) + (Data[0x06] << 16) + (Data[0x05] << 8) + Data[0x04]);
                                    byte Amount = Data[20];
                                    int Money = Data[5];
                                    if (Amount == 0)
                                        Amount = 1;

                                    string TehShop = System.IO.File.ReadAllText(System.Windows.Forms.Application.StartupPath + @"\Shop.dat");

                                    try
                                    {
                                        if (Other.CharExist(Convert.ToString(ItemID), TehShop))
                                        {
                                            foreach (uint[] item in ExternalDatabase.Items)
                                            {
                                                if (ItemID == item[0])
                                                {
                                                    Value = item[7];
                                                    CPsVal = item[15];
                                                }
                                            }
                                            for (int i = 0; i < Amount; i++)
                                            {
                                                if (MyChar.ItemsInInventory > 39)
                                                    return;
                                                if (MyChar.Silvers >= Value && CPsVal == 0 || MyChar.CPs > CPsVal && CPsVal != 0)
                                                {
                                                    if (CPsVal == 0)
                                                        MyChar.Silvers -= Value;
                                                    if (CPsVal > 0)
                                                        MyChar.CPs -= CPsVal;

                                                    if (MyChar.Silvers < 0)
                                                        MyChar.Silvers = 0;
                                                    if (MyChar.CPs < 0)
                                                        MyChar.CPs = 0;

                                                    byte WithPlus = 0;
                                                    if (ItemID == 730003)
                                                        WithPlus = 3;
                                                    if (ItemID == 730004)
                                                        WithPlus = 4;
                                                    if (ItemID == 730005)
                                                        WithPlus = 5;
                                                    if (ItemID == 730006)
                                                        WithPlus = 6;
                                                    if (ItemID == 730007)
                                                        WithPlus = 7;
                                                    if (ItemID == 730008)
                                                        WithPlus = 8;

                                                    MyChar.AddItem(Convert.ToString(ItemID) + "-" + WithPlus + "-0-0-0-0", 0, (uint)General.Rand.Next(10000000));
                                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));
                                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                                                }
                                                else
                                                {
                                                    SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "You don't have " + Value + " silvers or " + CPsVal + " CPs.", 2005));
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            General.WriteLine("There is no such item in Shop.dat.(" + ItemID + ")");
                                        }
                                    }
                                    catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); }
                                    MyChar.Ready = true;
                                    break;
                                }
                            #endregion
                            #region equipt
                            case 4:
                                {
                                    MyChar.Ready = false;
                                    ulong ItemUID = (ulong)((Data[7] << 24) + (Data[6] << 16) + (Data[5] << 8) + Data[4]);
                                    byte RequestPos = Data[8];

                                    string TheItem = "";
                                    uint TheUID = 0;
                                    int count = 0;
                                    uint ItemID = 0;

                                    foreach (uint uid in MyChar.Inventory_UIDs)
                                    {
                                        if (ItemUID == uid)
                                        {
                                            TheUID = uid;
                                            TheItem = MyChar.Inventory[count];
                                            string[] Splitter = TheItem.Split('-');
                                            ItemID = uint.Parse(Splitter[0]);
                                        }

                                        count++;
                                    }
                                    if (ItemID == 1050000 || ItemID == 1050001 || ItemID == 1050002)
                                        RequestPos = 5;
                                    if (RequestPos != 0)
                                    {
                                        if (Other.CanEquip(TheItem, MyChar))
                                        {
                                            if (Other.ItemType(ItemID) != 5 && RequestPos != 0)
                                            {
                                                MyChar.RemoveItem(TheUID);
                                                if (MyChar.Equips[RequestPos] == null || MyChar.Equips[RequestPos] == "0")
                                                {
                                                    MyChar.AddItem(TheItem, RequestPos, TheUID);
                                                }
                                                else
                                                {
                                                    MyChar.UnEquip(RequestPos);
                                                    MyChar.AddItem(TheItem, RequestPos, TheUID);
                                                }
                                                World.UpdateSpawn(MyChar);
                                            }
                                            else if (RequestPos != 0)
                                                if (MyChar.ItemsInInventory < 40)
                                                {
                                                    if (MyChar.Equips[4] == null && MyChar.Equips[5] == null)
                                                    {
                                                        MyChar.AddItem(TheItem, RequestPos, TheUID);
                                                        World.UpdateSpawn(MyChar);
                                                        MyChar.RemoveItem(TheUID);
                                                    }
                                                    else
                                                    {
                                                        MyChar.RemoveItem(TheUID);
                                                        if (MyChar.Equips[4] != null)
                                                        {
                                                            MyChar.UnEquip(4);
                                                        }
                                                        if (MyChar.Equips[5] != null)
                                                        {
                                                            MyChar.UnEquip(5);
                                                        }
                                                        MyChar.AddItem(TheItem, 4, TheUID);

                                                        World.UpdateSpawn(MyChar);
                                                    }
                                                }

                                        }
                                    }
                                    else
                                    {
                                        MyChar.UseItem(TheUID, TheItem);
                                    }
                                    MyChar.Ready = true;
                                    break;
                                }
                            case 6:
                                {
                                    MyChar.Ready = false;
                                    if (MyChar.ItemsInInventory > 39)
                                        return;

                                    ulong ItemUID = (ulong)((Data[7] << 24) + (Data[6] << 16) + (Data[5] << 8) + Data[4]);

                                    int count = 0;
                                    foreach (ulong uid in MyChar.Equips_UIDs)
                                    {
                                        if (uid == ItemUID)
                                        {
                                            MyChar.UnEquip((byte)count);
                                        }
                                        count++;
                                    }

                                    World.UpdateSpawn(MyChar);

                                    MyChar.Ready = true;
                                    break;
                                }
                        }

                    }
                    break;
                            #endregion
                #region actions
                case 1010:
                    {
                        PacketType = Data[22];

                        switch (PacketType)
                        {
                            case 120:
                                {
                                    if (MyChar.Flying)
                                    {
                                        MyChar.Flying = false;
                                        SendPacket(General.MyPackets.Vital(MyChar.UID, 26, MyChar.GetStat()));
                                    }
                                    break;
                                }
                            case 99:
                                {
                                    if (MyChar.LocMap == 1028)
                                        MyChar.Mining = true;
                                    break;
                                }
                            case 54:
                                {
                                    uint VUID = (uint)((Data[15] << 24) + (Data[14] << 16) + (Data[13] << 8) + Data[12]);
                                    Character ViewedChar = (Character)World.AllChars[VUID];
                                    break;
                                }
                            case 140:
                                {
                                    uint UID = BitConverter.ToUInt32(Data, 12);
                                    Character Char = (Character)World.AllChars[UID];
                                    if (Char != null)
                                        SendPacket(General.MyPackets.FriendEnemyInfoPacket(Char, 0));
                                    break;
                                }
                            case 94:
                                {
                                    {
                                        MyChar.Revive(false);
                                    }
                                    break;
                                }
                            case 117:
                                {
                                    MyChar.Ready = false;
                                    int Value1 = (Data[7] << 24) + (Data[6] << 16) + (Data[5] << 8) + Data[4];
                                    uint CharID = (uint)((Data[11] << 24) + (Data[10] << 16) + (Data[9] << 8) + Data[8]);
                                    uint VUID = (uint)((Data[15] << 24) + (Data[14] << 16) + (Data[13] << 8) + Data[12]);

                                    Character ViewedChar = (Character)World.AllChars[VUID];
                                    string[] Splitter;
                                    SendPacket(General.MyPackets.ViewEquip(ViewedChar));

                                    if (ViewedChar.Equips[1] != null && ViewedChar.Equips[1] != "0")
                                    {
                                        Splitter = ViewedChar.Equips[1].Split('-');
                                        SendPacket(General.MyPackets.ViewEquipAdd(VUID, uint.Parse(Splitter[0]), byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), 1, 100, 100));
                                    }
                                    else
                                        SendPacket(General.MyPackets.ViewEquipAdd(VUID, 0, 0, 0, 0, 0, 0, 1, 0, 0));

                                    if (ViewedChar.Equips[2] != null && ViewedChar.Equips[2] != "0")
                                    {
                                        Splitter = ViewedChar.Equips[2].Split('-');
                                        SendPacket(General.MyPackets.ViewEquipAdd(VUID, uint.Parse(Splitter[0]), byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), 2, 100, 100));
                                    }
                                    else
                                        SendPacket(General.MyPackets.ViewEquipAdd(VUID, 0, 0, 0, 0, 0, 0, 2, 0, 0));

                                    if (ViewedChar.Equips[3] != null && ViewedChar.Equips[3] != "0")
                                    {
                                        Splitter = ViewedChar.Equips[3].Split('-');
                                        SendPacket(General.MyPackets.ViewEquipAdd(VUID, uint.Parse(Splitter[0]), byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), 3, 100, 100));
                                    }
                                    else
                                        SendPacket(General.MyPackets.ViewEquipAdd(VUID, 0, 0, 0, 0, 0, 0, 3, 0, 0));

                                    if (ViewedChar.Equips[4] != null && ViewedChar.Equips[4] != "0")
                                    {
                                        Splitter = ViewedChar.Equips[4].Split('-');
                                        SendPacket(General.MyPackets.ViewEquipAdd(VUID, uint.Parse(Splitter[0]), byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), 4, 100, 100));
                                    }
                                    else
                                        SendPacket(General.MyPackets.ViewEquipAdd(VUID, 0, 0, 0, 0, 0, 0, 4, 0, 0));
                                    if (ViewedChar.Equips[5] != null && ViewedChar.Equips[5] != "0")
                                    {
                                        Splitter = ViewedChar.Equips[5].Split('-');
                                        SendPacket(General.MyPackets.ViewEquipAdd(VUID, uint.Parse(Splitter[0]), byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), 5, 100, 100));
                                    }
                                    else
                                        SendPacket(General.MyPackets.ViewEquipAdd(VUID, 0, 0, 0, 0, 0, 0, 5, 0, 0));

                                    if (ViewedChar.Equips[6] != null && ViewedChar.Equips[6] != "0")
                                    {
                                        Splitter = ViewedChar.Equips[6].Split('-');
                                        SendPacket(General.MyPackets.ViewEquipAdd(VUID, uint.Parse(Splitter[0]), byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), 6, 100, 100));
                                    }
                                    else
                                        SendPacket(General.MyPackets.ViewEquipAdd(VUID, 0, 0, 0, 0, 0, 0, 6, 0, 0));

                                    if (ViewedChar.Equips[7] != null && ViewedChar.Equips[7] != "0")
                                    {
                                        Splitter = ViewedChar.Equips[7].Split('-');
                                        SendPacket(General.MyPackets.ViewEquipAdd(VUID, uint.Parse(Splitter[0]), byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), 7, 100, 100));
                                    }
                                    else
                                        SendPacket(General.MyPackets.ViewEquipAdd(VUID, 0, 0, 0, 0, 0, 0, 7, 0, 0));

                                    if (ViewedChar.Equips[8] != null && ViewedChar.Equips[8] != "0")
                                    {
                                        Splitter = ViewedChar.Equips[8].Split('-');
                                        SendPacket(General.MyPackets.ViewEquipAdd(VUID, uint.Parse(Splitter[0]), byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), 8, 100, 100));
                                    }
                                    else
                                        SendPacket(General.MyPackets.ViewEquipAdd(VUID, 0, 0, 0, 0, 0, 0, 8, 0, 0));

                                    if (ViewedChar.Equips[9] != null && ViewedChar.Equips[9] != "0")
                                    {
                                        Splitter = ViewedChar.Equips[9].Split('-');
                                        SendPacket(General.MyPackets.ViewEquipAdd(VUID, uint.Parse(Splitter[0]), byte.Parse(Splitter[1]), byte.Parse(Splitter[2]), byte.Parse(Splitter[3]), byte.Parse(Splitter[4]), byte.Parse(Splitter[5]), 9, 100, 100));
                                    }
                                    else
                                        SendPacket(General.MyPackets.ViewEquipAdd(VUID, 0, 0, 0, 0, 0, 0, 9, 0, 0));


                                    MyChar.Ready = true;
                                    break;
                                }
                #endregion
                            #region avatar
                            case 142:
                                {
                                    MyChar.Ready = false;
                                    SendPacket(Data);
                                    uint Face = Data[12];

                                    if (Face > 200)
                                        Face -= 200;

                                    uint Multiply = (uint)(Data[13] * 56);
                                    Face += Multiply;

                                    MyChar.Avatar = (byte)Face;
                                    MyChar.Silvers -= 500;

                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 4, MyChar.Silvers));

                                    World.UpdateSpawn(MyChar);
                                    MyChar.Ready = true;
                                    break;
                                }
                            #endregion
                            #region mining
                            case 133:
                                {
                                    if (MyChar.Mining)
                                        MyChar.Mining = false;

                                    SendPacket(Data);
                                    short PrevX = (short)((Data[0x11] << 8) + Data[0x10]);
                                    short PrevY = (short)((Data[0x13] << 8) + Data[0x12]);
                                    short NewX = (short)((Data[0xd] << 8) + Data[0xc]);
                                    short NewY = (short)((Data[0xf] << 8) + Data[0xe]);

                                    MyChar.Attacking = false;
                                    MyChar.TargetUID = 0;
                                    MyChar.MobTarget = null;
                                    MyChar.TGTarget = null;
                                    MyChar.PTarget = null;
                                    MyChar.SkillLooping = 0;
                                    MyChar.AtkType = 0;
                                    MyChar.PrevX = MyChar.LocX;
                                    MyChar.PrevY = MyChar.LocY;
                                    MyChar.LocX = (ushort)NewX;
                                    MyChar.LocY = (ushort)NewY;
                                    MyChar.Action = 100;

                                    World.SpawnMeToOthers(MyChar, true);
                                    World.SpawnOthersToMe(MyChar, true);
                                    World.PlayerMoves(MyChar, Data);
                                    World.SurroundNPCs(MyChar, true);
                                    World.SurroundMobs(MyChar, true);
                                    World.SurroundDroppedItems(MyChar, true);

                                    break;
                                }
                            #endregion
                            #region welcome
                            case 74:
                                {
                                    if (There)
                                        return;
                                    if (MyChar == null)
                                        return;

                                    SendPacket(General.MyPackets.PlacePacket1(MyChar));
                                    SendPacket(General.MyPackets.PlacePacket2(MyChar));
                                    SendPacket(General.MyPackets.PlacePacket3(MyChar));
                                    SendPacket(General.MyPackets.LogonPacket());
                                    SendPacket(General.MyPackets.ShowMinimap(true));
                                    SendPacket(General.MyPackets.GeneralData((long)(MyChar.UID), 3, 0, 0, 96));

                                    SendPacket(General.MyPackets.Vital(MyChar.UID, 26, MyChar.GetStat()));
                                    MyChar.StartXPCircle();

                                    MyChar.PKMode = 3;

                                    World.SpawnMeToOthers(MyChar, false);
                                    World.SpawnOthersToMe(MyChar, false);
                                    World.SurroundNPCs(MyChar, false);
                                    World.SurroundMobs(MyChar, false);
                                    World.SurroundDroppedItems(MyChar, false);
                                    MyChar.UnPackInventory();
                                    MyChar.SendInventory();
                                    MyChar.UnPackEquips();
                                    MyChar.SendEquips(true);
                                    MyChar.UnPackSkills();
                                    MyChar.SendSkills();
                                    MyChar.UnPackProfs();
                                    MyChar.SendProfs();
                                    MyChar.UnPackWarehouses();
                                    MyChar.UnPackEnemies();
                                    MyChar.UnPackFriends();
                                    Status = ExternalDatabase.GetStatus(Account);

                                    foreach (DictionaryEntry DE in Guilds.AllGuilds)
                                    {
                                        Guild AGuild = (Guild)DE.Value;
                                        SendPacket(General.MyPackets.GuildName(AGuild.GuildID, AGuild.GuildName));
                                    }
                                    if (MyChar.MyGuild != null)
                                    {
                                        SendPacket(General.MyPackets.GuildInfo(MyChar.MyGuild, MyChar));
                                        SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, MyChar.MyGuild.Bulletin, 2111));
                                        SendPacket(General.MyPackets.GuildName(MyChar.GuildID, MyChar.MyGuild.GuildName));
                                        SendPacket(General.MyPackets.GeneralData(MyChar.UID, 0, 0, 0, 97));
                                    }

                                    if (MyChar.RBCount == 2)
                                    {
                                        SendPacket(General.MyPackets.String(MyChar.UID, 10, "2NDMetempsychosis"));
                                    }
                                    if (MyChar.Rank == 7)
                                        SendPacket(General.MyPackets.String(MyChar.UID, 10, "letter1"));
                                    if (MyChar.Rank == 6)
                                        SendPacket(General.MyPackets.String(MyChar.UID, 10, "letter2"));
                                    if (MyChar.Rank == 5)
                                        SendPacket(General.MyPackets.String(MyChar.UID, 10, "letter3"));
                                    if (MyChar.Rank == 4)
                                        SendPacket(General.MyPackets.String(MyChar.UID, 10, "letter4"));
                                    if (MyChar.Rank == 3)
                                        SendPacket(General.MyPackets.String(MyChar.UID, 10, "letter5"));
                                    if (MyChar.Rank == 2)
                                    {
                                        SendPacket(General.MyPackets.String(MyChar.UID, 10, "coronet3"));
                                        SendPacket(General.MyPackets.String(MyChar.UID, 10, "letter6"));
                                    }
                                    if (MyChar.Rank == 1)
                                    {
                                        SendPacket(General.MyPackets.String(MyChar.UID, 10, "coronet4"));
                                        SendPacket(General.MyPackets.String(MyChar.UID, 10, "letter7"));
                                    }

                                    SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Welcome to NonameCo", 2000));
                                    SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "We are looking for GM`s!", 2000));
                                    SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "http://nonameco.ucoz.com/", 2000));
                                    SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Exp Rate:" + ExternalDatabase.ExpRate + "x", 2000));
                                    SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Prof Rate:" + ExternalDatabase.ProfExpRate + "x", 2000));
                                    SendPacket(General.MyPackets.SendMsg(MessageId, "SYSTEM", MyChar.Name, "Players Online: " + World.AllChars.Count, 2005));
                                    SendPacket(General.MyPackets.GeneralData(MyChar.UID, 0, 0, 0, 75));
                                    There = true;
                                    World.SpawnMeToOthers(MyChar, true);

                                    break;
                                }
                            #endregion
                            #region pvp
                            case 96:
                                {
                                    MyChar.Ready = false;
                                    SendPacket(General.MyPackets.GeneralData((long)(MyChar.UID), Data[12], 0, 0, 96));
                                    MyChar.PKMode = Data[12];
                                    MyChar.Ready = true;
                                    break;
                                }
                            #endregion
                            #region moremovement
                            case 79:
                                {
                                    MyChar.Ready = false;
                                    MyChar.Direction = Data[20];
                                    World.PlayerMoves(MyChar, Data);
                                    MyChar.Ready = true;
                                    break;
                                }
                            case 81:
                                {
                                    MyChar.Ready = false;
                                    MyChar.Action = Data[12];
                                    World.PlayerMoves(MyChar, Data);
                                    MyChar.Ready = true;
                                    break;
                                }
                            case 85:
                                {
                                    MyChar.Ready = false;
                                    if (MyChar.CurHP >= 1)
                                    {
                                        foreach (DictionaryEntry DE in InternalDatabase.portals)
                                        {
                                            Portals portal = (Portals)DE.Value;
                                            ushort SMAP = portal.Smap;
                                            ushort SX = portal.SX;
                                            ushort SY = portal.SY;
                                            if (MyChar.LocMap == portal.Smap)
                                            {
                                                if (MyChar.LocX >= SX - 2 && MyChar.LocX <= SX + 2 && MyChar.LocY >= SY - 2 && MyChar.LocY <= SY + 2)
                                                {

                                                    MyChar.Teleport(portal.Emap, portal.EX, portal.EY);
                                                    MyChar.Ready = true;
                                                    
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    break;
                            #endregion
                case 1001:
                    {
                        uint Model = 0;
                        uint Avatar = 0;
                        byte Class = 0;
                        string CharName = "";
                        bool ValidName = true;

                        Model = (uint)Data[0x35];
                        Model = (Model << 8) | (uint)(Data[0x34]);

                        Class = Data[0x36];

                        int x = 0x14;
                        while (x < 0x24 && Data[x] != 0x00)
                        {
                            CharName += Convert.ToChar(Data[x]);
                            x++;
                        }
                        if (Model == 1003)
                            Avatar = 67;
                        else if (Model == 1004)
                            Avatar = 67;
                        else if (Model == 2001)
                            Avatar = 201;
                        else if (Model == 2002)
                            Avatar = 201;

                        if (CharName.IndexOfAny(new char[3] { ' ', '[', ']' }) > -1)
                        {
                            ValidName = false;
                        }

                        foreach (string name in ExternalDatabase.ForbiddenNames)
                        {
                            if (name == CharName)
                            {
                                ValidName = false;
                                break;
                            }
                        }

                        if (CharName.IndexOfAny(new char[3] { ' ', '[', ']' }) > -1)
                        {
                            ValidName = false;
                        }

                        foreach (string name in ExternalDatabase.ForbiddenNames)
                        {
                            if (name == CharName)
                            {
                                ValidName = false;
                                break;
                            }
                        }

                        try
                        {
                            if (ValidName)
                            {
                                bool Success = InternalDatabase.CreateCharacter(CharName, Class, Model, Avatar, this);
                                if (Success)
                                    Console.WriteLine("New character: " + CharName);
                                Online = false;
                                MySocket.Disconnect();
                            }
                            else
                            {
                                Online = false;
                                MySocket.Disconnect();
                            }
                        }
                        catch
                        {
                            return;
                        }
                    }
                    break;
            }
        }
        #region autotimers
        void GuildWarStop5min_Elapsed(object sender, ElapsedEventArgs e)
        {
            GuildWarStop5min.Stop();
            World.SendMsgToAll("Guild War will end in 5 minutes", "SYSTEM", 2011);
            GuildWarStop5min.Enabled = false;

            GuildWarStop4min.Interval = 60000;
            GuildWarStop4min.Elapsed += new ElapsedEventHandler(GuildWarStop4min_Elapsed);
            GuildWarStop4min.Start();
        }

        void GuildWarStop4min_Elapsed(object sender, ElapsedEventArgs e)
        {
            GuildWarStop4min.Stop();
            World.SendMsgToAll("Guild War will end in 4 minutes", "SYSTEM", 2011);
            GuildWarStop4min.Enabled = false;

            GuildWarStop3min.Interval = 60000;
            GuildWarStop3min.Elapsed += new ElapsedEventHandler(GuildWarStop3min_Elapsed);
            GuildWarStop3min.Start();
        }

        void GuildWarStop3min_Elapsed(object sender, ElapsedEventArgs e)
        {
            GuildWarStop3min.Stop();
            World.SendMsgToAll("Guild War will end in 3 minutes", "SYSTEM", 2011);
            GuildWarStop3min.Enabled = false;

            GuildWarStop2min.Interval = 60000;
            GuildWarStop2min.Elapsed += new ElapsedEventHandler(GuildWarStop2min_Elapsed);
            GuildWarStop2min.Start();
        }

        void GuildWarStop2min_Elapsed(object sender, ElapsedEventArgs e)
        {
            GuildWarStop2min.Stop();
            World.SendMsgToAll("Guild War will end in 2 minutes", "SYSTEM", 2011);
            GuildWarStop2min.Enabled = false;

            GuildWarStop1min.Interval = 60000;
            GuildWarStop1min.Elapsed += new ElapsedEventHandler(GuildWarStop1min_Elapsed);
            GuildWarStop1min.Start();
        }

        void GuildWarStop1min_Elapsed(object sender, ElapsedEventArgs e)
        {
            GuildWarStop1min.Stop();
            World.SendMsgToAll("Guild War will end in 1 minute", "SYSTEM", 2011);
            GuildWarStop1min.Enabled = false;

            GuildWarStop30sec.Interval = 30000;
            GuildWarStop30sec.Elapsed += new ElapsedEventHandler(GuildWarStop30sec_Elapsed);
            GuildWarStop30sec.Start();
        }

        void GuildWarStop30sec_Elapsed(object sender, ElapsedEventArgs e)
        {
            GuildWarStop30sec.Stop();
            World.SendMsgToAll("Guild War will end and servers will restart in 30 seconds", "SYSTEM", 2011);
            GuildWarStop30sec.Enabled = false;
        }

        void GuildWarStop_Elapsed(object sender, ElapsedEventArgs e)
        {
            GuildWarStop.Stop();
            GuildWarStart.Enabled = false;
            General.ServerRestart();
        }

        void UpdateRestart30sec_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateRestart30sec.Stop();
            World.SendMsgToAll("The server is going to be restarted for a quick update 1-5 minutes", "SYSTEM", 2011);
        }

        void UpdateRestart_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateRestart.Stop();
            World.SaveAllChars();
            Environment.Exit(0);
        }

        void ShutDownTimerMsg4min_Elapsed(object sender, ElapsedEventArgs e)
        {
            World.SendMsgToAll("Server going down for maintenance in 4 minutes", "SYSTEM", 2011);
            ShutDownTimerMsg3min.Interval = 60000;
            ShutDownTimerMsg3min.Elapsed += new ElapsedEventHandler(ShutDownTimerMsg3min_Elapsed);
            ShutDownTimerMsg3min.Start();
            ShutDownTimerMsg4min.Enabled = false;
        }

        void ShutDownTimerMsg3min_Elapsed(object sender, ElapsedEventArgs e)
        {
            World.SendMsgToAll("Server going down for maintenance in 3 minutes", "SYSTEM", 2011);
            ShutDownTimerMsg2min.Interval = 60000;
            ShutDownTimerMsg2min.Elapsed += new ElapsedEventHandler(ShutDownTimerMsg2min_Elapsed);
            ShutDownTimerMsg2min.Start();
            ShutDownTimerMsg3min.Enabled = false;
        }

        void ShutDownTimerMsg2min_Elapsed(object sender, ElapsedEventArgs e)
        {
            World.SendMsgToAll("Server is going down for maintenance in 2 minutes", "SYSTEM", 2011);
            ShutDownTimerMsg1min.Interval = 60000;
            ShutDownTimerMsg1min.Elapsed += new ElapsedEventHandler(ShutDownTimerMsg1min_Elapsed);
            ShutDownTimerMsg1min.Start();
            ShutDownTimerMsg2min.Enabled = false;
        }

        void ShutDownTimerMsg1min_Elapsed(object sender, ElapsedEventArgs e)
        {
            World.SendMsgToAll("Server is going down for maintenance in 1 minute", "SYSTEM", 2011);
            ShutDownTimerMsg30sec.Interval = 30000;
            ShutDownTimerMsg30sec.Elapsed += new ElapsedEventHandler(ShutDownTimerMsg30sec_Elapsed);
            ShutDownTimerMsg30sec.Start();
            ShutDownTimerMsg1min.Enabled = false;
        }

        void ShutDownTimerMsg30sec_Elapsed(object sender, ElapsedEventArgs e)
        {
            World.SendMsgToAll("Server is going down for maintenance in 30 seconds in order to check server status, visit www.powersourceco.com", "SYSTEM", 2011);
            ShutDownTimerMsg30sec.Enabled = false;
        }

        void ShutdownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            World.SaveAllChars();
            Environment.Exit(0);
        }
        #endregion
        #region misc

        public void SendPacket(byte[] Dat)
        {
            try
            {
                if (ListenSock != null)
                    if (ListenSock.WinSock.Connected)
                    {
                        int Len = Dat[0] + (Dat[1] << 8);
                        if (Dat.Length != Len)
                            return;

                        System.Threading.Monitor.TryEnter(this, new TimeSpan(0, 0, 0, 8, 0));

                        byte[] Data = new byte[Dat.Length];
                        Dat.CopyTo(Data, 0);

                        Crypto.Encrypt(ref Data);
                        ListenSock.WinSock.Send(Data);

                        System.Threading.Monitor.Exit(this);
                    }
            }
            catch { }
        }

        public void Drop()
        {
            try
            {
                if (Online)
                {
                    Online = false;
                    ExternalDatabase.ChangeOnlineStatus(Account, 0);             

                    if (MyChar != null)
                    {
                        World.RemoveEntity(MyChar);
                        if (MyChar.TheTimer != null)
                        {
                            MyChar.TheTimer.Stop();
                            MyChar.TheTimer.Dispose();
                            MyChar.TheTimer = null;
                        }
                        if (MyChar.LocMap == 700)
                        {
                            MyChar.CPs += 27;
                            SendPacket(General.MyPackets.Vital(MyChar.UID, 30, MyChar.CPs));
                            MyChar.Teleport(1036, 200, 200);
                        }

                        foreach (DictionaryEntry DE in MyChar.Friends)
                        {
                            uint FriendID = (uint)DE.Key;
                            if (World.AllChars.Contains(FriendID))
                            {
                                Character Friend = (Character)World.AllChars[FriendID];
                                if (Friend != null)
                                {
                                    Friend.MyClient.SendPacket(General.MyPackets.FriendEnemyPacket(MyChar.UID, MyChar.Name, 14, 0));
                                    Friend.MyClient.SendPacket(General.MyPackets.FriendEnemyPacket(MyChar.UID, MyChar.Name, 15, 0));
                                    Friend.MyClient.SendPacket(General.MyPackets.SendMsg(Friend.MyClient.MessageId, "SYSTEM", Friend.Name, "Your friend " + MyChar.Name + " has logged out.", 2005));
                                }
                            }
                        }
                        InternalDatabase.SaveChar(MyChar);
                        MyChar.Trading = false;
                        MyChar.TradingSilvers = 0;
                        MyChar.TradingCPs = 0;
                        MyChar.TradeOK = false;
                        MyChar.Trading = false;
                        MyChar.AtkType = 0;
                        MyChar.JoinForbidden = false;
                        MyChar.SkillLoopingTarget = 0;
                        MyChar.SkillLoopingX = 0;
                        MyChar.SkillLoopingY = 0;
                        MyChar.SkillLooping = 0;
                        MyChar.SMOn = false;
                        MyChar.CycloneOn = false;
                        MyChar.CastingPray = false;
                        MyChar.Inventory = new string[41];
                        MyChar.Equips = new string[10];
                        MyChar.TCWH = new string[20];
                        MyChar.PCWH = new string[20];
                        MyChar.ACWH = new string[20];
                        MyChar.DCWH = new string[20];
                        MyChar.BIWH = new string[20];
                        MyChar.MAWH = new string[40];
                        MyChar.Skills.Clear();
                        MyChar.Skill_Exps.Clear();
                        MyChar.Profs.Clear();
                        MyChar.Prof_Exps.Clear();
                        MyChar.Friends.Clear();
                        MyChar.Enemies.Clear();

                        if (MyChar.Trading)
                        {
                            Character Who = (Character)World.AllChars[MyChar.TradingWith];
                            Who.MyClient.SendPacket(General.MyPackets.TradePacket(MyChar.TradingWith, 5));
                            Who.TradingSilvers = 0;
                            Who.TradingCPs = 0;
                            Who.TradeOK = false;
                            Who.Trading = false;
                            MyChar.TradingWith = 0;
                            Who.MyClient.SendPacket(General.MyPackets.SendMsg(Who.MyClient.MessageId, "SYSTEM", Who.Name, "Trading failed!", 2005));
                        }

                        if (MyChar.TeamLeader)
                            MyChar.TeamDismiss();

                        if (MyChar.MyTeamLeader != null && !MyChar.TeamLeader)
                            MyChar.MyTeamLeader.TeamRemove(MyChar, false);
                        
                        try
                        {
                            if (World.AllChars.Contains(MyChar.UID))
                                World.AllChars.Remove(MyChar.UID);                          
                        }
                        catch (Exception Exc) { General.WriteLine(Exc.ToString()); }
                                                
                        MyChar = null;
                    }
                    ListenSock.WinSock.Close();                    
                    ListenSock = null;
                }
            }
            catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); }
        }
        #endregion
    }
}