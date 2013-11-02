using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;

namespace COServer_Project
{
    public class InternalDatabase
    {
        public static Hashtable portals = new Hashtable();
        public static Ini Stats = new Ini(System.Windows.Forms.Application.StartupPath + @"\Stats.ini");
        public static void SaveGuild(Guild TheGuild)
        {
            string PackedDLs = "";

            foreach (DictionaryEntry DE in TheGuild.DLs)
            {
                string dl = (string)DE.Value;
                PackedDLs += dl + "~";
            }
            if (PackedDLs.Length > 0)
                PackedDLs = PackedDLs.Remove(PackedDLs.Length - 1, 1);

            string PackedMembers = "";

            foreach (DictionaryEntry DE in TheGuild.Members)
            {
                string nm = (string)DE.Value;
                PackedMembers += nm + "~";
            }
            if (PackedMembers.Length > 0)
                PackedMembers = PackedMembers.Remove(PackedMembers.Length - 1, 1);

            string PackedAllies = "";

            foreach (string ally in TheGuild.Allies)
            {
                PackedAllies += ally + "~";
            }
            if (PackedAllies.Length > 0)
                PackedAllies = PackedAllies.Remove(PackedAllies.Length - 1, 1);

            string PackedEnemies = "";

            foreach (string enemy in TheGuild.Enemies)
            {
                PackedEnemies += enemy + "~";
            }
            if (PackedEnemies.Length > 0)
                PackedEnemies = PackedEnemies.Remove(PackedEnemies.Length - 1, 1);

            byte Pole = 0;
            if (TheGuild.HoldingPole)
                Pole = 1;
            if(ExternalDatabase.AllowQuerys)
                ExternalDatabase.DatabaseQueue.Enqueue(new MySqlCommand("UPDATE `Guilds` SET `Fund` = " + TheGuild.Fund + ", `GuildLeader` = '" + TheGuild.Creator + "', `MembersCount` = " + TheGuild.MembersCount + ", `GWWins` = " + TheGuild.GWWins + ", `HoldingPole` = " + Pole + ", `Bulletin` = '" + TheGuild.Bulletin + "',`DLs` = '" + PackedDLs + "',`NormalMembers` = '" + PackedMembers + "', `Allies` = '" + PackedAllies + "', `Enemies` = '" + PackedEnemies + "' WHERE `GuildID` = " + TheGuild.GuildID, ExternalDatabase.Connection));
        }
        public static bool NewGuild(ushort GuildID, string GuildName, Character Creator)
        {
            try
            {
                if (ExternalDatabase.AllowQuerys)
                    ExternalDatabase.DatabaseQueue.Enqueue(new MySqlCommand("INSERT INTO guilds (GuildID,GuildName,Fund,GuildLeader,MembersCount,DLs,NormalMembers,Allies,Enemies,GWWins,HoldingPole,Bulletin) VALUES (" + GuildID + ",'" + GuildName + "',1000000,'" + Creator.Name + ":" + Creator.UID.ToString() + ":" + Creator.Level.ToString() + ":1000000',1,'','','','',0,0,'New guild')", ExternalDatabase.Connection));
                return true;
            }
            catch { return false; }
        }
        public static void LoadGuilds()
        {
            MySqlDataAdapter DataAdapter = null;
            DataSet DSet = new DataSet();

            try
            {
                DataAdapter = new MySqlDataAdapter("SELECT * FROM `Guilds`", ExternalDatabase.Connection);
                DataAdapter.Fill(DSet, "Guild");

                if (DSet.Tables["Guild"].Rows.Count > 0)
                {
                    int GuildCount = DSet.Tables["Guild"].Rows.Count;

                    for (int i = 0; i < GuildCount; i++)
                    {
                        DataRow DR = DSet.Tables["Guild"].Rows[i];

                        string DLs = (string)DR["DLs"];
                        string[] RDLS = DLs.Split('~');
                        string NMs = (string)DR["NormalMembers"];
                        string[] RNMs = NMs.Split('~');
                        string GL = (string)DR["GuildLeader"];

                        Guilds.AddGuild((string)DR["GuildName"], Convert.ToUInt16((uint)DR["GuildID"]), GL, RDLS, RNMs, (uint)DR["Fund"], (uint)DR["GWWins"], Convert.ToByte((uint)DR["HoldingPole"]), (uint)DR["MembersCount"], (string)DR["Bulletin"], (string)DR["Allies"], (string)DR["Enemies"]);
                    }
                    General.WriteLine("Loaded " + GuildCount + " Guilds.");
                }
            }
            catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); }
        }
        public static void GetStats(Character Charr)
        {
            string str = "0";
            string agi = "0";
            string vit = "0";
            string spi = "0";
            string lv;

            if (Charr.Level > 120)
                lv = "120";
            else
                lv = Convert.ToString(Charr.Level);

            if (Charr.Job > 9 && Charr.Job < 16)
            {
                str = (Stats.ReadValue("Trojan", "Strength[" + lv + "]"));
                agi = (Stats.ReadValue("Trojan", "Agility[" + lv + "]"));
                vit = (Stats.ReadValue("Trojan", "Vitality[" + lv + "]"));
                spi = (Stats.ReadValue("Trojan", "Spirit[" + lv + "]"));
            }
            if (Charr.Job > 19 && Charr.Job < 26)
            {
                str = (Stats.ReadValue("Warrior", "Strength[" + lv + "]"));
                agi = (Stats.ReadValue("Warrior", "Agility[" + lv + "]"));
                vit = (Stats.ReadValue("Warrior", "Vitality[" + lv + "]"));
                spi = (Stats.ReadValue("Warrior", "Spirit[" + lv + "]"));
            }
            if (Charr.Job > 39 && Charr.Job < 46)
            {
                str = (Stats.ReadValue("Archer", "Strength[" + lv + "]"));
                agi = (Stats.ReadValue("Archer", "Agility[" + lv + "]"));
                vit = (Stats.ReadValue("Archer", "Vitality[" + lv + "]"));
                spi = (Stats.ReadValue("Archer", "Spirit[" + lv + "]"));
            }
            if (Charr.Job > 129 && Charr.Job < 136 || Charr.Job > 139 && Charr.Job < 146 || Charr.Job == 100 || Charr.Job == 101)
            {
                str = (Stats.ReadValue("Taoist", "Strength[" + lv + "]"));
                agi = (Stats.ReadValue("Taoist", "Agility[" + lv + "]"));
                vit = (Stats.ReadValue("Taoist", "Vitality[" + lv + "]"));
                spi = (Stats.ReadValue("Taoist", "Spirit[" + lv + "]"));
            }

            Charr.Str = ushort.Parse(str);
            Charr.Agi = ushort.Parse(agi);
            Charr.Vit = ushort.Parse(vit);
            Charr.Spi = ushort.Parse(spi);
        }
        public static void SaveRank(Character Charr)
        {
            MySqlCommand Command = null;
            Command = new MySqlCommand("UPDATE `Characters` SET `Rank` = '" + Charr.Rank + "' WHERE `Account` = '" + Charr.MyClient.Account + "'", ExternalDatabase.Connection);
            Command.ExecuteNonQuery();

        }
        public static void SaveDonation(Character Charr)
        {
            MySqlCommand Command = null;
            Command = new MySqlCommand("UPDATE `Characters` SET `Donation` = '" + Charr.Donation + "' WHERE `Account` = '" + Charr.MyClient.Account + "'", ExternalDatabase.Connection);
            Command.ExecuteNonQuery();
        }
        public static void SaveFCPs(Character Charr)
        {
            MySqlCommand Command = null;
            Command = new MySqlCommand("UPDATE `Characters` SET `FCPs` = '" + Charr.FCPs + "' WHERE `Account` = '" + Charr.MyClient.Account + "'", ExternalDatabase.Connection);
            Command.ExecuteNonQuery();
        }
        public static void SaveSpouse(Character Charr)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand("UPDATE `characters` SET `Spouse` = '" + Charr.Spouse + "' WHERE `Account` = '" + Charr.MyClient.Account + "'", ExternalDatabase.Connection);
                Command.ExecuteNonQuery();
            }
            catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); }
        }
        public static void RemoveSpouse(uint RemovedUID)
        {
            MySqlCommand Command = null;
            MySqlDataAdapter DataAdapter = null;
            DataAdapter = new MySqlDataAdapter("SELECT * FROM `characters` WHERE `UID` = " + RemovedUID, ExternalDatabase.Connection);

            DataSet DSet = new DataSet();
            DataAdapter.Fill(DSet, "Char");

            DataRow DR = DSet.Tables["Char"].Rows[0];
            string Spouse = (string)DR["Spouse"];

            Command = new MySqlCommand("UPDATE `characters` SET `Spouse` = 'None'", ExternalDatabase.Connection);
            Command.ExecuteNonQuery();
        }
        public static void SaveChar(Character Charr)
        {
            try
            {
                Charr.PackInventory();
                Charr.PackEquips();
                Charr.PackSkills();
                Charr.PackProfs();
                Charr.PackWarehouses();
                Charr.PackEnemies();
                Charr.PackFriends();
                if (ExternalDatabase.AllowQuerys)
                    ExternalDatabase.DatabaseQueue.Enqueue(new MySqlCommand("UPDATE `Characters` SET `CharName` = '" + Charr.Name + "', `Level` = " + Charr.Level + ",`Exp` = " + Charr.Exp + ",`GuildDonation` = " + Charr.GuildDonation + ",`Strength` = " + Charr.Str + ",`Agility` = " + Charr.Agi + ",`Vitality` = " + Charr.Vit + ",`Spirit` = " + Charr.Spi + ",`Job` = " + Charr.Job + ",`Model` = " + Charr.RealModel + ",`Money` = " + Charr.Silvers + ",`CPs` = " + Charr.CPs + ",`CurrentHP` = " + Charr.CurHP + ",`StatPoints` = " + Charr.StatP + ",`MyGuild` = " + Charr.GuildID + ",`GuildPos` = " + Charr.GuildPosition + ",`LocationMap` = " + Charr.LocMap + ",`LocationX` = " + Charr.LocX + ",`LocationY` = " + Charr.LocY + ",`Hair` = " + Charr.Hair + ",`Equipment` = '" + Charr.PackedEquips + "',`Inventory` = '" + Charr.PackedInventory + "',`PKPoints` = " + Charr.PKPoints + ",`PrevMap` = " + Charr.PrevMap + ", `Skills` = '" + Charr.PackedSkills + "', `Profs` = '" + Charr.PackedProfs + "',`RBCount` = " + Charr.RBCount + ",`Avatar` = " + Charr.Avatar + ",`WHMoney` = " + Charr.WHSilvers + ",`VP` = " + Charr.VP + ",`Warehouses` = '" + Charr.PackedWHs + "',`Friends` = '" + Charr.PackedFriends + "',`Enemies` = '" + Charr.PackedEnemies + "',`QuestMob` = '" + Charr.QuestMob + "',`QuestKO` = " + Charr.QuestKO + " WHERE `Account` = '" + Charr.MyClient.Account + "'", ExternalDatabase.Connection));
            }
            catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc));}
        }
        public static void GetCharInfo(Character Charr, string UserName)
        {
            MySqlDataAdapter DataAdapter = null;
            DataSet DSet = new DataSet();
            try
            {
                DataAdapter = new MySqlDataAdapter("SELECT * FROM `Characters` WHERE `Account` = '" + UserName + "'", ExternalDatabase.Connection);
                DataAdapter.Fill(DSet, "Accounts");
                if (DSet != null && DSet.Tables["Accounts"].Rows.Count > 0)
                {
                    DataRow DR = DSet.Tables["Accounts"].Rows[0];

                    Charr.UID = (uint)DR["UID"];
                    Charr.Name = (string)DR["CharName"];
                    Charr.Spouse = (string)DR["Spouse"];
                    Charr.Job = Convert.ToByte((uint)DR["Job"]);
                    Charr.Level = Convert.ToByte((uint)DR["Level"]);
                    Charr.Exp = (uint)DR["Exp"];
                    Charr.FCPs = (uint)DR["FCPs"];
                    Charr.Model = Convert.ToUInt16((uint)DR["Model"]);
                    Charr.Avatar = Convert.ToUInt16((uint)DR["Avatar"]);
                    Charr.Hair = Convert.ToUInt16((uint)DR["Hair"]);
                    Charr.LocX = Convert.ToUInt16((uint)DR["LocationX"]);
                    Charr.LocY = Convert.ToUInt16((uint)DR["LocationY"]);
                    Charr.LocMap = Convert.ToUInt16((uint)DR["LocationMap"]);
                    Charr.Str = Convert.ToUInt16((uint)DR["Strength"]);
                    Charr.Agi = Convert.ToUInt16((uint)DR["Agility"]);
                    Charr.Vit = Convert.ToUInt16((uint)DR["Vitality"]);
                    Charr.Spi = Convert.ToUInt16((uint)DR["Spirit"]);
                    Charr.Silvers = (uint)DR["Money"];
                    Charr.CPs = (uint)DR["CPs"];
                    Charr.CurHP = Convert.ToUInt16((uint)DR["CurrentHP"]);
                    Charr.PKPoints = Convert.ToUInt16((uint)DR["PKPoints"]);
                    Charr.RBCount = Convert.ToByte((uint)DR["RBCount"]);
                    Charr.PackedInventory = (string)DR["Inventory"];
                    Charr.PackedEquips = (string)DR["Equipment"];
                    Charr.PackedSkills = (string)DR["Skills"];
                    Charr.PackedProfs = (string)DR["Profs"];
                    Charr.WHSilvers = (uint)DR["WHMoney"];
                    Charr.QuestKO = (uint)DR["QuestKO"];
                    Charr.PackedWHs = (string)DR["Warehouses"];
                    Charr.PackedFriends = (string)DR["Friends"];
                    Charr.PackedEnemies = (string)DR["Enemies"];
                    Charr.QuestMob = (string)DR["QuestMob"];
                    Charr.VP = (uint)DR["VP"];
                    Charr.GuildDonation = (uint)DR["GuildDonation"];
                    Charr.StatP = Convert.ToUInt16((uint)DR["StatPoints"]);
                    Charr.GuildID = Convert.ToUInt16((uint)DR["MyGuild"]);
                    Charr.GuildPosition = Convert.ToByte((uint)DR["GuildPos"]);
                    Charr.PrevMap = Convert.ToUInt16((uint)DR["PrevMap"]);
                    Charr.Donation = (uint)DR["Donation"];
                    Charr.Rank = Convert.ToByte((uint)DR["Rank"]);
                    if (Guilds.AllGuilds.Contains(Charr.GuildID))
                        Charr.MyGuild = (Guild)Guilds.AllGuilds[Charr.GuildID];
                    Charr.MinAtk = Charr.Str;
                    Charr.MaxAtk = Charr.Str;
                    Charr.Potency = Charr.Level;
                    Charr.RealModel = Charr.Model;
                    Charr.RealAvatar = Charr.Avatar;
                    Charr.MaxHP = Charr.BaseMaxHP();
                    Charr.RealAgi = Charr.Agi;
                }
                else
                    General.WriteLine("Char not found.");
            }
            catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); }
        }
        public static bool CreateCharacter(string Name, byte Class, uint Model, uint Avatar, Client UClient)
        {
            try
            {
                string str = "0";
                string agi = "0";
                string vit = "0";
                string spi = "0";

                if (Class == 10)
                {
                    str = (Stats.ReadValue("Trojan", "Strength[1]"));
                    agi = (Stats.ReadValue("Trojan", "Agility[1]"));
                    vit = (Stats.ReadValue("Trojan", "Vitality[1]"));
                    spi = (Stats.ReadValue("Trojan", "Spirit[1]"));
                }
                if (Class == 20)
                {
                    str = (Stats.ReadValue("Warrior", "Strength[1]"));
                    agi = (Stats.ReadValue("Warrior", "Agility[1]"));
                    vit = (Stats.ReadValue("Warrior", "Vitality[1]"));
                    spi = (Stats.ReadValue("Warrior", "Spirit[1]"));
                }
                if (Class == 40)
                {
                    str = (Stats.ReadValue("Archer", "Strength[1]"));
                    agi = (Stats.ReadValue("Archer", "Agility[1]"));
                    vit = (Stats.ReadValue("Archer", "Vitality[1]"));
                    spi = (Stats.ReadValue("Archer", "Spirit[1]"));
                }
                if (Class == 100)
                {
                    str = (Stats.ReadValue("Taoist", "Strength[1]"));
                    agi = (Stats.ReadValue("Taoist", "Agility[1]"));
                    vit = (Stats.ReadValue("Taoist", "Vitality[1]"));
                    spi = (Stats.ReadValue("Taoist", "Spirit[1]"));
                }
                string hp = Convert.ToString((short.Parse(vit) * 24 + short.Parse(str) * 3 + short.Parse(agi) * 3 + short.Parse(spi) * 3));
                ulong uid = (uint)General.Rand.Next(1000001, 19999999);

                try
                {
                    if (ExternalDatabase.AllowQuerys)
                    {
                        ExternalDatabase.DatabaseQueue.Enqueue(new MySqlCommand("INSERT INTO characters (CharName,Account,Level,Exp,Strength,Agility,Vitality,Spirit,Job,Model,Money,CPs,CurrentHP,StatPoints,LocationMap,LocationX,LocationY,UID,Hair,Equipment,Inventory,PKPoints,Skills,Profs,RBCount,Avatar,WHMoney,Warehouses,VP,Friends,Enemies,GuildDonation,MyGuild,GuildPos,PrevMap,QuestMob,QuestKO) VALUES ('" + Name + "','" + UClient.Account + "',1,0," + str + "," + agi + "," + vit + "," + spi + "," + Class + "," + Model + ",100,0," + hp + ",0,1010,061,109," + uid + ",410,'','',0,'','',0," + Avatar + ",0,'',0,'','',0,0,0,1010,'',0)", ExternalDatabase.Connection));

                        ExternalDatabase.DatabaseQueue.Enqueue(new MySqlCommand("UPDATE `Accounts` SET `LogonType` = 1 WHERE `AccountID` = '" + UClient.Account + "'", ExternalDatabase.Connection));

                        ExternalDatabase.DatabaseQueue.Enqueue(new MySqlCommand("UPDATE `Accounts` SET `Charr` = '" + Name + "' WHERE `AccountID` = '" + UClient.Account + "'", ExternalDatabase.Connection));
                    }
                }
                catch { return false; }

                return true;
            }
            catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); return false; }
        }
        public static void GetPortals()
        {
            byte work = 0;
            MySqlDataAdapter DataAdapter = null;
            DataSet DSet = new DataSet();
            try
            {
                DataAdapter = new MySqlDataAdapter("SELECT * FROM `portals`", ExternalDatabase.Connection);
                work = 1;
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e);
                work = 0;
            }
            if (work == 0)
            {
                Console.WriteLine("Sorry.... The server cannot load the NPC Spawns");
            }
            else
            {
                DataAdapter.Fill(DSet, "portals");
                if (DSet.Tables["portals"].Rows.Count > 0)
                {
                    foreach (DataRow DR in DSet.Tables["portals"].Rows)
                    {
                        Portals portal = new Portals();
                        portal.PID = Convert.ToUInt16(DR["Id"]);
                        portal.Smap = Convert.ToUInt16(DR["FromMap"]);
                        portal.SX = Convert.ToUInt16(DR["FromX"]);
                        portal.SY = Convert.ToUInt16(DR["FromY"]);
                        portal.Emap = Convert.ToUInt16(DR["NewMap"]);
                        portal.EX = Convert.ToUInt16(DR["NewX"]);
                        portal.EY = Convert.ToUInt16(DR["NewY"]);
                        portals.Add(portal.PID, portal);
                    }
                }
            }
        }
        
    }
}
