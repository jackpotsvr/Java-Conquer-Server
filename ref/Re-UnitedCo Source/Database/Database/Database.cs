using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using MySql.Data.MySqlClient;

public class ExternalDatabase
{
    public static bool AllowQuerys = true;
    public static Thread MysqlThread;
    public static ThreadStart TStart;
    public static Queue<MySqlCommand> DatabaseQueue = new Queue<MySqlCommand>();
    public static MySqlConnection Connection;
    public static ushort[][] Portals;
    public static uint[][] NPCs;
    public static uint[][] Items;
    public static string[][] DBPlusInfo;
    public static string[][] Mobs;
    public static uint[][] MobSpawns;
    public static uint ExpRate;
    public static uint ProfExpRate;
    public static ushort[][] RevPoints;
    public static ushort[] NoPKMaps = new ushort[] { 1002, 1036, 1010, 500, 1004, 700, 1004, 601, 1006, 1011, 1012 };
    public static ushort[] PKMaps = new ushort[] { 1038, 1080, 1090, 1000, 1003, 1015, 1016, 1020, 1039, 1351, 1352, 1353, 1354 };
    public static Hashtable Skills = new Hashtable();
    public static ushort[][][] SkillAttributes = new ushort[9877][][];
    public static Hashtable SkillsDone = new Hashtable();
    public static ushort GC1X = 530;
    public static ushort GC1Y = 556;
    public static ushort GC1Map = 1075;
    public static ushort GC2X = 10;
    public static ushort GC2Y = 90;
    public static ushort GC2Map = 1080;
    public static ushort GC3X = 116;
    public static ushort GC3Y = 164;
    public static ushort GC3Map = 1090;
    public static ushort GC4X = 43;
    public static ushort GC4Y = 75;
    public static ushort GC4Map = 1099;
    public static bool MysqlConnected = false;
    public static string DBUserName;
    public static string DBUserPass;
    public static string[] ForbiddenNames = new string[] { "LOTF", "GM", "Tanel", "PM", "IcedEarth", "killer1242", "GameMaster", "Administrator", "Rukia", "Gothika", "JukseY", "[]", "<", ">", "*", "SteaL", "StarStruck", "Lizzio", "Andrew", "~Fury~", };

    public static void DatabaseConnect()
    {
        while (DBUserName == "" || DBUserPass == "") { }
        Connect(DBUserName, DBUserPass);
        MysqlConnected = true;
        while (true)
        {
            if (Connection.State == ConnectionState.Broken || Connection.State == ConnectionState.Closed)
            {
                MysqlConnected = false;
                Connect(DBUserName, DBUserPass);
                MysqlConnected = true;
            }
            Thread.Sleep(1);
        }
    }

    public static void StartDBConn()
    {
        TStart = new ThreadStart(DatabaseConnect);
        MysqlThread = new Thread(TStart);
        MysqlThread.Start();
    }

    public static bool Connect(string user, string pass)
    {
        try
        {
            Connection = new MySqlConnection("Server='localhost';Database='coproj';Username=" + MakeSafeString(user) + ";Password=" + MakeSafeString(pass));
            Connection.Open();
            return true;
        }
        catch
        {
            return false;
        }
    }
    public static string MakeSafeString(string Input)
    {
        string NewString;
        NewString = Input.Replace("'", "''");
        NewString = NewString.Replace("\"", "\"\"");
        NewString = NewString.Replace("/*", "");
        NewString = NewString.Replace("*/", "");
        return NewString;
    }

    public static void ChangeOnlineStatus(string AccName, byte To)
    {
        if (ExternalDatabase.AllowQuerys)
            DatabaseQueue.Enqueue(new MySqlCommand("UPDATE `Accounts` SET `Online` = " + To + " WHERE `AccountID` = '" + AccName + "'", Connection));
    }

    public static void AllOffline()
    {
        if (ExternalDatabase.AllowQuerys)
            DatabaseQueue.Enqueue(new MySqlCommand("UPDATE `Accounts` SET `Online` = " + 0, Connection));
    }

    public static void Ban(string Acc)
    {
        if (ExternalDatabase.AllowQuerys)
            DatabaseQueue.Enqueue(new MySqlCommand("UPDATE `Accounts` SET `LogonType` = 3 WHERE `AccountID` = '" + Acc + "'", Connection));
    }

    public static void DisbandGuild(ushort GUID)
    {
        if (ExternalDatabase.AllowQuerys)
            DatabaseQueue.Enqueue(new MySqlCommand("DELETE FROM `Guilds` WHERE `GuildID` = " + GUID, Connection));
    }

    public static void NoGuild(uint UID)
    {
        if (ExternalDatabase.AllowQuerys)
            DatabaseQueue.Enqueue(new MySqlCommand("UPDATE `Characters` SET `GuildPos` = 0, `MyGuild` = 0, `GuildDonation` = 0 WHERE `UID` = " + UID, Connection));
    }

    public static bool NewGuard(ushort X, ushort Y, ushort Map)
    {
        try
        {
            if (ExternalDatabase.AllowQuerys)
                DatabaseQueue.Enqueue(new MySqlCommand("INSERT INTO mobspawns (SpawnWhatID,SpawnNr,XStart,YStart,XEnd,YEnd,Map) VALUES (502,1," + X + "," + Y + "," + X + "," + Y + "," + Map + ")", Connection));
            return true;
        }
        catch { return false; }
    }
    public static bool NewSpawn(ushort Map, ushort XStart, ushort YStart, ushort XEnd, ushort YEnd, ushort SpawnNr, uint MobID)
    {
        try
        {
            if (ExternalDatabase.AllowQuerys)
                DatabaseQueue.Enqueue(new MySqlCommand("INSERT INTO mobspawns (SpawnWhatID,SpawnNr,XStart,YStart,XEnd,YEnd,Map) VALUES (" + MobID + "," + SpawnNr + "," + XStart + "," + YStart + "," + XEnd + "," + YEnd + "," + Map + ")", Connection));
            return true;
        }
        catch { return false; }
    }

    public enum SkillType : ushort
    {
        RangeSkillMelee = 0,
        RangeSkillRanged = 1,
        SingleTargetSkillMagic = 2,
        TargetRangeSkillMagic = 3,
        DirectSkillMelee = 4,
        RangeSectorSkillRanged = 5,
        SingleTargetSkillMagicHeal = 6,
        SelfUseSkill = 7,
        BuffSkill = 8,
        RangeSkillHeal = 11,
        SingleTargetSkillMelee = 12,
        SingleTargetSkillRanged = 13
    }

    public static void DefineSkills()
    {
        //SkillType Range Sector Damage/Heal CostMana/Stamina ActivationChance

        //Healing Rain
        SkillAttributes[1055] = new ushort[5][];
        SkillAttributes[1055][0] = new ushort[6] { 11, 0, 0, 100, 0, 0 };
        SkillAttributes[1055][1] = new ushort[6] { 11, 0, 0, 200, 0, 0 };
        SkillAttributes[1055][2] = new ushort[6] { 11, 0, 0, 300, 0, 0 };
        SkillAttributes[1055][3] = new ushort[6] { 11, 0, 0, 400, 0, 0 };
        SkillAttributes[1055][4] = new ushort[6] { 11, 0, 0, 500, 0, 0 };
        SkillsDone.Add(1055, 4);

        //icycle
        SkillAttributes[5130] = new ushort[10][];
        SkillAttributes[5130][0] = new ushort[6] { 2, 0, 109, 505, 0, 0 };
        SkillAttributes[5130][1] = new ushort[6] { 2, 0, 131, 666, 0, 0 };
        SkillAttributes[5130][2] = new ushort[6] { 2, 0, 149, 882, 0, 0 };
        SkillAttributes[5130][3] = new ushort[6] { 2, 0, 162, 1166, 0, 0 };
        SkillsDone.Add(5130, 3);

        //IceCircle
        SkillAttributes[5131] = new ushort[4][];
        SkillAttributes[5131][0] = new ushort[6] { 3, 7, 90, 540, 0, 0 };
        SkillAttributes[5131][1] = new ushort[6] { 3, 8, 100, 650, 0, 0 };
        SkillAttributes[5131][2] = new ushort[6] { 3, 9, 115, 720, 0, 0 };
        SkillAttributes[5131][3] = new ushort[6] { 3, 11, 125, 770, 0, 0 };
        SkillsDone.Add(5131, 3);

        //avalanche
        SkillAttributes[5132] = new ushort[4][];
        SkillAttributes[5132][0] = new ushort[6] { 3, 4, 32, 180, 0, 0 };
        SkillAttributes[5132][1] = new ushort[6] { 3, 5, 36, 240, 0, 0 };
        SkillAttributes[5132][2] = new ushort[6] { 3, 6, 50, 310, 0, 0 };
        SkillAttributes[5132][3] = new ushort[6] { 3, 8, 64, 400, 0, 0 };
        SkillsDone.Add(5132, 3);

        //Summon Guard
        SkillAttributes[4000] = new ushort[4][];
        SkillAttributes[4000][0] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4000][1] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4000][2] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4000][3] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillsDone.Add(4000, 3);

        SkillAttributes[4010] = new ushort[4][];
        SkillAttributes[4010][0] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4010][1] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4010][2] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4010][3] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillsDone.Add(4010, 3);

        SkillAttributes[4020] = new ushort[4][];
        SkillAttributes[4020][0] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4020][1] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4020][2] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4020][3] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillsDone.Add(4020, 3);

        SkillAttributes[4030] = new ushort[4][];
        SkillAttributes[4030][0] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4030][1] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4030][2] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4030][3] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillsDone.Add(4030, 3);

        SkillAttributes[4060] = new ushort[4][];
        SkillAttributes[4060][0] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4060][1] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4060][2] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4060][3] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillsDone.Add(4060, 3);

        SkillAttributes[4070] = new ushort[4][];
        SkillAttributes[4070][0] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4070][1] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4070][2] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4070][3] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillsDone.Add(4070, 3);

        SkillAttributes[4050] = new ushort[4][];
        SkillAttributes[4050][0] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4050][1] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4050][2] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillAttributes[4050][3] = new ushort[6] { 7, 0, 0, 0, 100, 0 };
        SkillsDone.Add(4050, 3);

        //Random Teleport
        SkillAttributes[1080] = new ushort[1][];
        SkillAttributes[1080][0] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillsDone.Add(1080, 0);

        //Dash
        SkillAttributes[1051] = new ushort[1][];
        SkillAttributes[1051][0] = new ushort[6] { 0, 0, 0, 100, 0, 0 };
        SkillsDone.Add(1051, 0);

        //MagicShield
        SkillAttributes[1090] = new ushort[5][];
        SkillAttributes[1090][0] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillAttributes[1090][1] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillAttributes[1090][2] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillAttributes[1090][3] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillAttributes[1090][4] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillsDone.Add(1090, 4);

        //Invisibility
        SkillAttributes[1075] = new ushort[5][];
        SkillAttributes[1075][0] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillAttributes[1075][1] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillAttributes[1075][2] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillAttributes[1075][3] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillAttributes[1075][4] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillsDone.Add(1075, 4);



        //Bless
        SkillAttributes[9876] = new ushort[1][];
        SkillAttributes[9876][0] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillsDone.Add(9876, 1);

        //Reflect
        SkillAttributes[3060] = new ushort[1][];
        SkillAttributes[3060][0] = new ushort[6] { 7, 0, 0, 0, 0, 1 };
        SkillsDone.Add(3060, 0);

        //Dodge
        SkillAttributes[3080] = new ushort[3][];
        SkillAttributes[3080][0] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillAttributes[3080][1] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillAttributes[3080][2] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillsDone.Add(3080, 2);

        //CruelShade
        SkillAttributes[3050] = new ushort[4][];
        SkillAttributes[3050][0] = new ushort[6] { 2, 0, 0, 0, 100, 0 };
        SkillAttributes[3050][1] = new ushort[6] { 2, 0, 0, 0, 100, 0 };
        SkillAttributes[3050][2] = new ushort[6] { 2, 0, 0, 0, 100, 0 };
        SkillAttributes[3050][3] = new ushort[6] { 2, 0, 0, 0, 100, 0 };
        SkillsDone.Add(3050, 3);

        //Pervade
        SkillAttributes[3090] = new ushort[6][];
        SkillAttributes[3090][0] = new ushort[6] { 1, 6, 0, 0, 100, 0 };
        SkillAttributes[3090][1] = new ushort[6] { 1, 6, 0, 0, 100, 0 };
        SkillAttributes[3090][2] = new ushort[6] { 1, 6, 0, 0, 100, 0 };
        SkillAttributes[3090][3] = new ushort[6] { 1, 6, 0, 0, 100, 0 };
        SkillAttributes[3090][4] = new ushort[6] { 1, 6, 0, 0, 100, 0 };
        SkillAttributes[3090][5] = new ushort[6] { 1, 6, 0, 0, 100, 0 };
        SkillsDone.Add(3090, 5);

        //Robot (Golem)
        SkillAttributes[1270] = new ushort[8][];
        SkillAttributes[1270][0] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillAttributes[1270][1] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillAttributes[1270][2] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillAttributes[1270][3] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillAttributes[1270][4] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillAttributes[1270][5] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillAttributes[1270][6] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillAttributes[1270][7] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillsDone.Add(1270, 7);

        //DivineHare
        SkillAttributes[1350] = new ushort[5][];
        SkillAttributes[1350][0] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillAttributes[1350][1] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillAttributes[1350][2] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillAttributes[1350][3] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillAttributes[1350][4] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillsDone.Add(1350, 4);

        //SpiritualHealing
        SkillAttributes[1190] = new ushort[3][];
        SkillAttributes[1190][0] = new ushort[6] { 7, 0, 0, 500, 100, 0 };
        SkillAttributes[1190][1] = new ushort[6] { 7, 0, 0, 800, 100, 0 };
        SkillAttributes[1190][2] = new ushort[6] { 7, 0, 0, 1300, 100, 0 };
        SkillsDone.Add(1190, 2);

        //Meditation
        SkillAttributes[1195] = new ushort[3][];
        SkillAttributes[1195][0] = new ushort[6] { 7, 0, 0, 310, 100, 0 };
        SkillAttributes[1195][1] = new ushort[6] { 7, 0, 0, 600, 100, 0 };
        SkillAttributes[1195][2] = new ushort[6] { 7, 0, 0, 1020, 100, 0 };
        SkillsDone.Add(1195, 2);

        //Guard's Spell (GM ONLY)
        SkillAttributes[8036] = new ushort[2][];
        SkillAttributes[8036][0] = new ushort[6] { 2, 0, 0, 50000, 0, 0 };
        SkillAttributes[8036][1] = new ushort[6] { 2, 0, 0, 60000, 0, 0 };
        SkillsDone.Add(8036, 1);

        //FastBlade
        SkillAttributes[1045] = new ushort[5][];
        SkillAttributes[1045][0] = new ushort[6] { 4, 5, 0, 96, 20, 0 };
        SkillAttributes[1045][1] = new ushort[6] { 4, 6, 0, 96, 20, 0 };
        SkillAttributes[1045][2] = new ushort[6] { 4, 7, 0, 96, 20, 0 };
        SkillAttributes[1045][3] = new ushort[6] { 4, 8, 0, 100, 20, 0 };
        SkillAttributes[1045][4] = new ushort[6] { 4, 9, 0, 100, 20, 0 };
        SkillsDone.Add(1045, 4);

        //ScentSword
        SkillAttributes[1046] = new ushort[5][];
        SkillAttributes[1046][0] = new ushort[6] { 4, 5, 0, 93, 20, 0 };
        SkillAttributes[1046][1] = new ushort[6] { 4, 6, 0, 93, 20, 0 };
        SkillAttributes[1046][2] = new ushort[6] { 4, 7, 0, 93, 20, 0 };
        SkillAttributes[1046][3] = new ushort[6] { 4, 7, 0, 96, 20, 0 };
        SkillAttributes[1046][4] = new ushort[6] { 4, 9, 0, 96, 20, 0 };
        SkillsDone.Add(1046, 4);

        //Hercules
        SkillAttributes[1115] = new ushort[5][];
        SkillAttributes[1115][0] = new ushort[6] { 0, 3, 0, 35, 30, 0 };
        SkillAttributes[1115][1] = new ushort[6] { 0, 3, 0, 35, 30, 0 };
        SkillAttributes[1115][2] = new ushort[6] { 0, 4, 0, 35, 30, 0 };
        SkillAttributes[1115][3] = new ushort[6] { 0, 6, 0, 40, 30, 0 };
        SkillAttributes[1115][4] = new ushort[6] { 0, 6, 0, 40, 30, 0 };
        SkillsDone.Add(1115, 4);

        //--Weapon Skills
        //Rage
        SkillAttributes[7020] = new ushort[10][];
        SkillAttributes[7020][0] = new ushort[6] { 0, 2, 0, 110, 0, 20 };
        SkillAttributes[7020][1] = new ushort[6] { 0, 2, 0, 110, 0, 23 };
        SkillAttributes[7020][2] = new ushort[6] { 0, 2, 0, 110, 0, 26 };
        SkillAttributes[7020][3] = new ushort[6] { 0, 2, 0, 110, 0, 29 };
        SkillAttributes[7020][4] = new ushort[6] { 0, 2, 0, 140, 0, 31 };
        SkillAttributes[7020][5] = new ushort[6] { 0, 2, 0, 140, 0, 34 };
        SkillAttributes[7020][6] = new ushort[6] { 0, 3, 0, 140, 0, 37 };
        SkillAttributes[7020][7] = new ushort[6] { 0, 4, 0, 140, 0, 40 };
        SkillAttributes[7020][8] = new ushort[6] { 0, 4, 0, 140, 0, 43 };
        SkillAttributes[7020][9] = new ushort[6] { 0, 4, 0, 145, 0, 45 };
        SkillsDone.Add(7020, 9);

        //Snow
        SkillAttributes[5010] = new ushort[10][];
        SkillAttributes[5010][0] = new ushort[6] { 0, 2, 0, 110, 0, 20 };
        SkillAttributes[5010][1] = new ushort[6] { 0, 2, 0, 110, 0, 21 };
        SkillAttributes[5010][2] = new ushort[6] { 0, 2, 0, 110, 0, 21 };
        SkillAttributes[5010][3] = new ushort[6] { 0, 2, 0, 110, 0, 22 };
        SkillAttributes[5010][4] = new ushort[6] { 0, 3, 0, 140, 0, 22 };
        SkillAttributes[5010][5] = new ushort[6] { 0, 4, 0, 140, 0, 23 };
        SkillAttributes[5010][6] = new ushort[6] { 0, 4, 0, 140, 0, 24 };
        SkillAttributes[5010][7] = new ushort[6] { 0, 4, 0, 140, 0, 25 };
        SkillAttributes[5010][8] = new ushort[6] { 0, 5, 0, 140, 0, 28 };
        SkillAttributes[5010][9] = new ushort[6] { 0, 5, 0, 145, 0, 30 };
        SkillsDone.Add(5010, 9);

        //WideStrike
        SkillAttributes[1250] = new ushort[10][];
        SkillAttributes[1250][0] = new ushort[6] { 13, 2, 0, 410, 0, 20 };
        SkillAttributes[1250][1] = new ushort[6] { 13, 2, 0, 410, 0, 23 };
        SkillAttributes[1250][2] = new ushort[6] { 13, 2, 0, 410, 0, 26 };
        SkillAttributes[1250][3] = new ushort[6] { 13, 2, 0, 410, 0, 29 };
        SkillAttributes[1250][4] = new ushort[6] { 13, 3, 0, 440, 0, 31 };
        SkillAttributes[1250][5] = new ushort[6] { 13, 3, 0, 440, 0, 34 };
        SkillAttributes[1250][6] = new ushort[6] { 13, 3, 0, 440, 0, 37 };
        SkillAttributes[1250][7] = new ushort[6] { 13, 3, 0, 440, 0, 40 };
        SkillAttributes[1250][8] = new ushort[6] { 13, 3, 0, 440, 0, 43 };
        SkillAttributes[1250][9] = new ushort[6] { 13, 3, 0, 445, 0, 45 };
        SkillsDone.Add(1250, 9);

        //Boreas
        SkillAttributes[5050] = new ushort[10][];
        SkillAttributes[5050][0] = new ushort[6] { 13, 2, 0, 2410, 0, 20 };
        SkillAttributes[5050][1] = new ushort[6] { 13, 2, 0, 2410, 0, 23 };
        SkillAttributes[5050][2] = new ushort[6] { 13, 2, 0, 2410, 0, 26 };
        SkillAttributes[5050][3] = new ushort[6] { 13, 2, 0, 2410, 0, 29 };
        SkillAttributes[5050][4] = new ushort[6] { 13, 3, 0, 2440, 0, 31 };
        SkillAttributes[5050][5] = new ushort[6] { 13, 3, 0, 2440, 0, 34 };
        SkillAttributes[5050][6] = new ushort[6] { 13, 3, 0, 2440, 0, 37 };
        SkillAttributes[5050][7] = new ushort[6] { 13, 3, 0, 2440, 0, 40 };
        SkillAttributes[5050][8] = new ushort[6] { 13, 3, 0, 2440, 0, 43 };
        SkillAttributes[5050][9] = new ushort[6] { 13, 3, 0, 2445, 0, 45 };
        SkillsDone.Add(5050, 9);

        //StrandedMonster
        SkillAttributes[5020] = new ushort[10][];
        SkillAttributes[5020][0] = new ushort[6] { 13, 2, 0, 110, 0, 20 };
        SkillAttributes[5020][1] = new ushort[6] { 13, 2, 0, 110, 0, 23 };
        SkillAttributes[5020][2] = new ushort[6] { 13, 2, 0, 110, 0, 26 };
        SkillAttributes[5020][3] = new ushort[6] { 13, 2, 0, 110, 0, 29 };
        SkillAttributes[5020][4] = new ushort[6] { 13, 2, 0, 140, 0, 31 };
        SkillAttributes[5020][5] = new ushort[6] { 13, 2, 0, 140, 0, 34 };
        SkillAttributes[5020][6] = new ushort[6] { 13, 2, 0, 140, 0, 37 };
        SkillAttributes[5020][7] = new ushort[6] { 13, 2, 0, 140, 0, 40 };
        SkillAttributes[5020][8] = new ushort[6] { 13, 2, 0, 140, 0, 43 };
        SkillAttributes[5020][9] = new ushort[6] { 13, 2, 0, 145, 0, 45 };
        SkillsDone.Add(5020, 9);

        //Celestial
        SkillAttributes[7030] = new ushort[10][];
        SkillAttributes[7030][0] = new ushort[6] { 13, 2, 0, 110, 0, 20 };
        SkillAttributes[7030][1] = new ushort[6] { 13, 2, 0, 110, 0, 23 };
        SkillAttributes[7030][2] = new ushort[6] { 13, 2, 0, 110, 0, 26 };
        SkillAttributes[7030][3] = new ushort[6] { 13, 2, 0, 110, 0, 29 };
        SkillAttributes[7030][4] = new ushort[6] { 13, 2, 0, 140, 0, 31 };
        SkillAttributes[7030][5] = new ushort[6] { 13, 2, 0, 140, 0, 34 };
        SkillAttributes[7030][6] = new ushort[6] { 13, 2, 0, 140, 0, 37 };
        SkillAttributes[7030][7] = new ushort[6] { 13, 2, 0, 140, 0, 40 };
        SkillAttributes[7030][8] = new ushort[6] { 13, 2, 0, 140, 0, 43 };
        SkillAttributes[7030][9] = new ushort[6] { 13, 2, 0, 145, 0, 45 };
        SkillsDone.Add(7030, 9);

        //Earthquake
        SkillAttributes[7010] = new ushort[10][];
        SkillAttributes[7010][0] = new ushort[6] { 12, 2, 0, 110, 0, 20 };
        SkillAttributes[7010][1] = new ushort[6] { 12, 2, 0, 110, 0, 23 };
        SkillAttributes[7010][2] = new ushort[6] { 12, 2, 0, 110, 0, 26 };
        SkillAttributes[7010][3] = new ushort[6] { 12, 2, 0, 110, 0, 29 };
        SkillAttributes[7010][4] = new ushort[6] { 12, 2, 0, 140, 0, 31 };
        SkillAttributes[7010][5] = new ushort[6] { 12, 2, 0, 140, 0, 34 };
        SkillAttributes[7010][6] = new ushort[6] { 12, 2, 0, 140, 0, 37 };
        SkillAttributes[7010][7] = new ushort[6] { 12, 2, 0, 140, 0, 40 };
        SkillAttributes[7010][8] = new ushort[6] { 12, 2, 0, 140, 0, 43 };
        SkillAttributes[7010][9] = new ushort[6] { 12, 2, 0, 145, 0, 45 };
        SkillsDone.Add(7010, 9);

        //Roamer
        SkillAttributes[7040] = new ushort[10][];
        SkillAttributes[7040][0] = new ushort[6] { 0, 2, 0, 110, 0, 20 };
        SkillAttributes[7040][1] = new ushort[6] { 0, 2, 0, 110, 0, 23 };
        SkillAttributes[7040][2] = new ushort[6] { 0, 2, 0, 110, 0, 26 };
        SkillAttributes[7040][3] = new ushort[6] { 0, 2, 0, 110, 0, 29 };
        SkillAttributes[7040][4] = new ushort[6] { 0, 2, 0, 140, 0, 31 };
        SkillAttributes[7040][5] = new ushort[6] { 0, 2, 0, 140, 0, 34 };
        SkillAttributes[7040][6] = new ushort[6] { 0, 2, 0, 140, 0, 37 };
        SkillAttributes[7040][7] = new ushort[6] { 0, 2, 0, 140, 0, 40 };
        SkillAttributes[7040][8] = new ushort[6] { 0, 2, 0, 140, 0, 43 };
        SkillAttributes[7040][9] = new ushort[6] { 0, 2, 0, 145, 0, 45 };
        SkillsDone.Add(7040, 9);

        //Halt
        SkillAttributes[1300] = new ushort[10][];
        SkillAttributes[1300][0] = new ushort[6] { 13, 2, 0, 110, 0, 20 };
        SkillAttributes[1300][1] = new ushort[6] { 13, 2, 0, 110, 0, 23 };
        SkillAttributes[1300][2] = new ushort[6] { 13, 2, 0, 110, 0, 26 };
        SkillAttributes[1300][3] = new ushort[6] { 13, 2, 0, 110, 0, 29 };
        SkillAttributes[1300][4] = new ushort[6] { 13, 2, 0, 140, 0, 31 };
        SkillAttributes[1300][5] = new ushort[6] { 13, 2, 0, 140, 0, 34 };
        SkillAttributes[1300][6] = new ushort[6] { 13, 2, 0, 140, 0, 37 };
        SkillAttributes[1300][7] = new ushort[6] { 13, 2, 0, 140, 0, 40 };
        SkillAttributes[1300][8] = new ushort[6] { 13, 2, 0, 140, 0, 43 };
        SkillAttributes[1300][9] = new ushort[6] { 13, 2, 0, 145, 0, 45 };
        SkillsDone.Add(1300, 9);

        //Penetration
        SkillAttributes[1290] = new ushort[10][];
        SkillAttributes[1290][0] = new ushort[6] { 12, 2, 0, 150, 0, 20 };
        SkillAttributes[1290][1] = new ushort[6] { 12, 2, 0, 160, 0, 23 };
        SkillAttributes[1290][2] = new ushort[6] { 12, 2, 0, 170, 0, 26 };
        SkillAttributes[1290][3] = new ushort[6] { 12, 2, 0, 180, 0, 29 };
        SkillAttributes[1290][4] = new ushort[6] { 12, 2, 0, 190, 0, 31 };
        SkillAttributes[1290][5] = new ushort[6] { 12, 2, 0, 200, 0, 34 };
        SkillAttributes[1290][6] = new ushort[6] { 12, 2, 0, 210, 0, 37 };
        SkillAttributes[1290][7] = new ushort[6] { 12, 2, 0, 220, 0, 40 };
        SkillAttributes[1290][8] = new ushort[6] { 12, 2, 0, 230, 0, 43 };
        SkillAttributes[1290][9] = new ushort[6] { 12, 2, 0, 240, 0, 45 };
        SkillsDone.Add(1290, 9);

        //Seizer
        SkillAttributes[7000] = new ushort[10][];
        SkillAttributes[7000][0] = new ushort[6] { 12, 2, 0, 110, 0, 20 };
        SkillAttributes[7000][1] = new ushort[6] { 12, 2, 0, 110, 0, 23 };
        SkillAttributes[7000][2] = new ushort[6] { 12, 2, 0, 110, 0, 26 };
        SkillAttributes[7000][3] = new ushort[6] { 12, 2, 0, 110, 0, 29 };
        SkillAttributes[7000][4] = new ushort[6] { 12, 2, 0, 140, 0, 31 };
        SkillAttributes[7000][5] = new ushort[6] { 12, 2, 0, 140, 0, 34 };
        SkillAttributes[7000][6] = new ushort[6] { 12, 2, 0, 140, 0, 37 };
        SkillAttributes[7000][7] = new ushort[6] { 12, 2, 0, 140, 0, 40 };
        SkillAttributes[7000][8] = new ushort[6] { 12, 2, 0, 140, 0, 43 };
        SkillAttributes[7000][9] = new ushort[6] { 12, 2, 0, 145, 0, 45 };
        SkillsDone.Add(7000, 9);

        //Boom
        SkillAttributes[5040] = new ushort[10][];
        SkillAttributes[5040][0] = new ushort[6] { 12, 2, 0, 110, 0, 20 };
        SkillAttributes[5040][1] = new ushort[6] { 12, 2, 0, 110, 0, 23 };
        SkillAttributes[5040][2] = new ushort[6] { 12, 2, 0, 110, 0, 26 };
        SkillAttributes[5040][3] = new ushort[6] { 12, 2, 0, 110, 0, 29 };
        SkillAttributes[5040][4] = new ushort[6] { 12, 3, 0, 140, 0, 31 };
        SkillAttributes[5040][5] = new ushort[6] { 12, 3, 0, 140, 0, 34 };
        SkillAttributes[5040][6] = new ushort[6] { 12, 3, 0, 140, 0, 37 };
        SkillAttributes[5040][7] = new ushort[6] { 12, 3, 0, 140, 0, 40 };
        SkillAttributes[5040][8] = new ushort[6] { 12, 3, 0, 140, 0, 43 };
        SkillAttributes[5040][9] = new ushort[6] { 12, 3, 0, 145, 0, 45 };
        SkillsDone.Add(5040, 9);

        //Phoenix
        /*
        SkillAttributes[5030] = new ushort[10][];
        SkillAttributes[5030][0] = new ushort[6] { 12, 0, 0, 115, 0, 33 };
        SkillAttributes[5030][1] = new ushort[6] { 12, 0, 0, 116, 0, 38 };
        SkillAttributes[5030][2] = new ushort[6] { 12, 0, 0, 117, 0, 43 };
        SkillAttributes[5030][3] = new ushort[6] { 12, 0, 0, 118, 0, 48 };
        SkillAttributes[5030][4] = new ushort[6] { 12, 0, 0, 119, 0, 53 };
        SkillAttributes[5030][5] = new ushort[6] { 12, 0, 0, 120, 0, 58 };
        SkillAttributes[5030][6] = new ushort[6] { 12, 0, 0, 121, 0, 63 };
        SkillAttributes[5030][7] = new ushort[6] { 12, 0, 0, 122, 0, 68 };
        SkillAttributes[5030][8] = new ushort[6] { 12, 0, 0, 123, 0, 73 };
        SkillAttributes[5030][9] = new ushort[6] { 12, 0, 0, 124, 0, 78 };
        SkillsDone.Add(5030, 9);
         */

        //Speed Gun
        SkillAttributes[1260] = new ushort[10][];
        SkillAttributes[1260][0] = new ushort[6] { 12, 2, 0, 150, 0, 20 };
        SkillAttributes[1260][1] = new ushort[6] { 12, 2, 0, 160, 0, 23 };
        SkillAttributes[1260][2] = new ushort[6] { 12, 2, 0, 170, 0, 26 };
        SkillAttributes[1260][3] = new ushort[6] { 12, 2, 0, 180, 0, 29 };
        SkillAttributes[1260][4] = new ushort[6] { 12, 2, 0, 190, 0, 31 };
        SkillAttributes[1260][5] = new ushort[6] { 12, 2, 0, 200, 0, 34 };
        SkillAttributes[1260][6] = new ushort[6] { 12, 2, 0, 210, 0, 37 };
        SkillAttributes[1260][7] = new ushort[6] { 12, 2, 0, 220, 0, 40 };
        SkillAttributes[1260][8] = new ushort[6] { 12, 2, 0, 230, 0, 43 };
        SkillAttributes[1260][9] = new ushort[6] { 12, 2, 0, 240, 0, 45 };
        SkillsDone.Add(1260, 9);

        //Scatter
        SkillAttributes[8001] = new ushort[6][];
        SkillAttributes[8001][0] = new ushort[6] { 5, 12, 45, 50, 0, 0 };
        SkillAttributes[8001][1] = new ushort[6] { 5, 13, 60, 55, 0, 0 };
        SkillAttributes[8001][2] = new ushort[6] { 5, 14, 80, 60, 0, 0 };
        SkillAttributes[8001][3] = new ushort[6] { 5, 16, 100, 65, 0, 0 };
        SkillAttributes[8001][4] = new ushort[6] { 5, 18, 150, 70, 0, 0 };
        SkillAttributes[8001][5] = new ushort[6] { 5, 19, 180, 72, 0, 0 };
        SkillsDone.Add(8001, 5);

        //IronShirt
        SkillAttributes[5100] = new ushort[1][];
        SkillAttributes[5100][0] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillsDone.Add(5100, 0);

        //Superman
        SkillAttributes[1025] = new ushort[1][];
        SkillAttributes[1025][0] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillsDone.Add(1025, 0);

        //Cyclone
        SkillAttributes[1110] = new ushort[1][];
        SkillAttributes[1110][0] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillsDone.Add(1110, 0);

        //Arrow Rain
        SkillAttributes[8030] = new ushort[1][];
        SkillAttributes[8030][0] = new ushort[6] { 14, 8, 0, 100, 0, 1 };
        SkillsDone.Add(8030, 0);

        //Accuracy
        SkillAttributes[1015] = new ushort[1][];
        SkillAttributes[1015][0] = new ushort[6] { 7, 0, 0, 0, 0, 1 };
        SkillsDone.Add(1015, 0);

        //XP Fly
        SkillAttributes[8002] = new ushort[1][];
        SkillAttributes[8002][0] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillsDone.Add(8002, 0);

        //Advanced Fly
        SkillAttributes[8003] = new ushort[2][];
        SkillAttributes[8003][0] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillAttributes[8003][1] = new ushort[6] { 7, 0, 0, 0, 0, 0 };
        SkillsDone.Add(8003, 0);

        //Flying Moon
        SkillAttributes[1320] = new ushort[3][];
        SkillAttributes[1320][0] = new ushort[6] { 2, 0, 0, 480, 255, 0 };
        SkillAttributes[1320][1] = new ushort[6] { 2, 0, 0, 1950, 255, 0 };
        SkillAttributes[1320][2] = new ushort[6] { 2, 0, 0, 5890, 255, 0 };
        SkillsDone.Add(1320, 2);

        //FireCircle
        SkillAttributes[1120] = new ushort[4][];
        SkillAttributes[1120][0] = new ushort[6] { 3, 7, 90, 540, 0, 0 };
        SkillAttributes[1120][1] = new ushort[6] { 3, 7, 100, 650, 0, 0 };
        SkillAttributes[1120][2] = new ushort[6] { 3, 11, 109, 720, 0, 0 };
        SkillAttributes[1120][3] = new ushort[6] { 3, 11, 118, 770, 0, 0 };
        SkillsDone.Add(1120, 3);

        //Tornado
        SkillAttributes[1002] = new ushort[4][];
        SkillAttributes[1002][0] = new ushort[6] { 2, 0, 110, 505, 0, 0 };
        SkillAttributes[1002][1] = new ushort[6] { 2, 0, 132, 666, 0, 0 };
        SkillAttributes[1002][2] = new ushort[6] { 2, 0, 150, 882, 0, 0 };
        SkillAttributes[1002][3] = new ushort[6] { 2, 0, 163, 1166, 0, 0 };
        SkillsDone.Add(1002, 3);

        //Fire of Hell
        SkillAttributes[1165] = new ushort[4][];
        SkillAttributes[1165][0] = new ushort[6] { 3, 4, 0, 180, 0, 0 };
        SkillAttributes[1165][1] = new ushort[6] { 3, 4, 0, 240, 0, 0 };
        SkillAttributes[1165][2] = new ushort[6] { 3, 4, 0, 310, 0, 0 };
        SkillAttributes[1165][3] = new ushort[6] { 3, 4, 0, 400, 0, 0 };
        SkillsDone.Add(1165, 3);

        //Cure
        SkillAttributes[1005] = new ushort[5][];
        SkillAttributes[1005][0] = new ushort[6] { 6, 0, 0, 20, 0, 0 };
        SkillAttributes[1005][1] = new ushort[6] { 6, 0, 0, 70, 0, 0 };
        SkillAttributes[1005][2] = new ushort[6] { 6, 0, 0, 150, 0, 0 };
        SkillAttributes[1005][3] = new ushort[6] { 6, 0, 0, 280, 0, 0 };
        SkillAttributes[1005][4] = new ushort[6] { 6, 0, 0, 400, 0, 0 };
        SkillsDone.Add(1005, 4);

        //Advanced Cure
        SkillAttributes[1175] = new ushort[5][];
        SkillAttributes[1175][0] = new ushort[6] { 6, 0, 0, 500, 0, 0 };
        SkillAttributes[1175][1] = new ushort[6] { 6, 0, 0, 600, 0, 0 };
        SkillAttributes[1175][2] = new ushort[6] { 6, 0, 0, 700, 0, 0 };
        SkillAttributes[1175][3] = new ushort[6] { 6, 0, 0, 800, 0, 0 };
        SkillAttributes[1175][4] = new ushort[6] { 6, 0, 0, 900, 0, 0 };
        SkillsDone.Add(1175, 4);

        //Nectar
        SkillAttributes[1170] = new ushort[5][];
        SkillAttributes[1170][0] = new ushort[6] { 6, 0, 0, 600, 0, 0 };
        SkillAttributes[1170][1] = new ushort[6] { 6, 0, 0, 700, 0, 0 };
        SkillAttributes[1170][2] = new ushort[6] { 6, 0, 0, 800, 0, 0 };
        SkillAttributes[1170][3] = new ushort[6] { 6, 0, 0, 900, 0, 0 };
        SkillAttributes[1170][4] = new ushort[6] { 6, 0, 0, 1000, 0, 0 };
        SkillsDone.Add(1170, 4);

        //StarofAccuracy
        SkillAttributes[1085] = new ushort[5][];
        SkillAttributes[1085][0] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillAttributes[1085][1] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillAttributes[1085][2] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillAttributes[1085][3] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillAttributes[1085][4] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillsDone.Add(1085, 4);

        //Stigma
        SkillAttributes[1095] = new ushort[5][];
        SkillAttributes[1095][0] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillAttributes[1095][1] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillAttributes[1095][2] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillAttributes[1095][3] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillAttributes[1095][4] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillsDone.Add(1095, 4);

        //Thunder
        SkillAttributes[1000] = new ushort[5][];
        SkillAttributes[1000][0] = new ushort[6] { 2, 0, 29, 7, 0, 0 };
        SkillAttributes[1000][1] = new ushort[6] { 2, 0, 32, 16, 0, 0 };
        SkillAttributes[1000][2] = new ushort[6] { 2, 0, 36, 32, 0, 0 };
        SkillAttributes[1000][3] = new ushort[6] { 2, 0, 50, 57, 0, 0 };
        SkillAttributes[1000][4] = new ushort[6] { 2, 0, 64, 86, 0, 0 };
        SkillsDone.Add(1000, 4);

        //Fire
        SkillAttributes[1001] = new ushort[4][];
        SkillAttributes[1001][0] = new ushort[6] { 2, 0, 64, 130, 0, 0 };
        SkillAttributes[1001][1] = new ushort[6] { 2, 0, 70, 189, 0, 0 };
        SkillAttributes[1001][2] = new ushort[6] { 2, 0, 86, 275, 0, 0 };
        SkillAttributes[1001][3] = new ushort[6] { 2, 0, 100, 380, 0, 0 };
        SkillsDone.Add(1001, 3);

        //FireBall
        SkillAttributes[1150] = new ushort[8][];
        SkillAttributes[1150][0] = new ushort[6] { 2, 0, 0, 378, 0, 0 };
        SkillAttributes[1150][1] = new ushort[6] { 2, 0, 0, 550, 0, 0 };
        SkillAttributes[1150][2] = new ushort[6] { 2, 0, 0, 760, 0, 0 };
        SkillAttributes[1150][3] = new ushort[6] { 2, 0, 0, 1010, 0, 0 };
        SkillAttributes[1150][4] = new ushort[6] { 2, 0, 0, 1332, 0, 0 };
        SkillAttributes[1150][5] = new ushort[6] { 2, 0, 0, 1764, 0, 0 };
        SkillAttributes[1150][6] = new ushort[6] { 2, 0, 0, 2332, 0, 0 };
        SkillAttributes[1150][7] = new ushort[6] { 2, 0, 0, 2800, 0, 0 };
        SkillsDone.Add(1150, 7);

        //Fire Meteor
        SkillAttributes[1180] = new ushort[8][];
        SkillAttributes[1180][0] = new ushort[6] { 2, 0, 0, 760, 0, 0 };
        SkillAttributes[1180][1] = new ushort[6] { 2, 0, 0, 1040, 0, 0 };
        SkillAttributes[1180][2] = new ushort[6] { 2, 0, 0, 1250, 0, 0 };
        SkillAttributes[1180][3] = new ushort[6] { 2, 0, 0, 1480, 0, 0 };
        SkillAttributes[1180][4] = new ushort[6] { 2, 0, 0, 1810, 0, 0 };
        SkillAttributes[1180][5] = new ushort[6] { 2, 0, 0, 2210, 0, 0 };
        SkillAttributes[1180][6] = new ushort[6] { 2, 0, 0, 2700, 0, 0 };
        SkillAttributes[1180][7] = new ushort[6] { 2, 0, 0, 3250, 0, 0 };
        SkillsDone.Add(1180, 7);

        //Bomb
        SkillAttributes[1160] = new ushort[4][];
        SkillAttributes[1160][0] = new ushort[6] { 2, 0, 0, 855, 0, 0 };
        SkillAttributes[1160][1] = new ushort[6] { 2, 0, 0, 1498, 0, 0 };
        SkillAttributes[1160][2] = new ushort[6] { 2, 0, 0, 1985, 0, 0 };
        SkillAttributes[1160][3] = new ushort[6] { 2, 0, 0, 2623, 0, 0 };
        SkillsDone.Add(1160, 3);

        //Pray
        SkillAttributes[1100] = new ushort[1][];
        SkillAttributes[1100][0] = new ushort[6] { 8, 0, 0, 0, 0, 0 };
        SkillsDone.Add(1100, 0);

        //Shield
        SkillAttributes[1020] = new ushort[1][];
        SkillAttributes[1020][0] = new ushort[6] { 8, 0, 0, 0, 0, 1 };
        SkillsDone.Add(1020, 0);

        //Lightning
        SkillAttributes[1010] = new ushort[1][];
        SkillAttributes[1010][0] = new ushort[6] { 3, 5, 0, 50, 0, 1 };
        SkillsDone.Add(1010, 0);

        //Volcano
        SkillAttributes[1125] = new ushort[1][];
        SkillAttributes[1125][0] = new ushort[6] { 3, 8, 0, 300, 0, 1 };
        SkillsDone.Add(1125, 0);

        //SpeedLightning
        SkillAttributes[5001] = new ushort[1][];
        SkillAttributes[5001][0] = new ushort[6] { 1, 14, 0, 450, 0, 1 };
        SkillsDone.Add(5001, 0);

        //RapidFire
        SkillAttributes[8000] = new ushort[6][];
        SkillAttributes[8000][0] = new ushort[6] { 13, 8, 0, 190, 100, 0 };
        SkillAttributes[8000][1] = new ushort[6] { 13, 8, 0, 200, 100, 0 };
        SkillAttributes[8000][2] = new ushort[6] { 13, 8, 0, 250, 100, 0 };
        SkillAttributes[8000][3] = new ushort[6] { 13, 8, 0, 300, 100, 0 };
        SkillAttributes[8000][4] = new ushort[6] { 13, 8, 0, 350, 100, 0 };
        SkillAttributes[8000][5] = new ushort[6] { 13, 8, 0, 600, 100, 0 };
        SkillsDone.Add(8000, 5);

        //Restore
        SkillAttributes[1105] = new ushort[1][];
        SkillAttributes[1105][0] = new ushort[6] { 1, 14, 0, 1000, 0, 1 };
        SkillsDone.Add(1105, 0);
    }

    public static void RemoveFromFriend(uint RemoverUID, uint RemovedUID)
    {
        MySqlDataAdapter DataAdapter = new MySqlDataAdapter("SELECT * FROM `Characters` WHERE `UID` = " + RemovedUID, Connection);
        DataSet DSet = new DataSet();
        DataAdapter.Fill(DSet, "Char");

        DataRow DR = DSet.Tables["Char"].Rows[0];
        string Friends = (string)DR["Friends"];
        string NewFriends = "";
        string[] Friendss = Friends.Split('~');
        foreach (string friend in Friendss)
        {
            if (friend != null && friend.Length > 1)
            {
                string[] Splitter = friend.Split(':');
                if (Splitter[1] != RemoverUID.ToString())
                    NewFriends += friend + "~";
            }
        }
        if (NewFriends.Length > 0)
            NewFriends = NewFriends.Remove(NewFriends.Length - 1, 1);
        if (ExternalDatabase.AllowQuerys)
            DatabaseQueue.Enqueue(new MySqlCommand("UPDATE `Characters` SET `Friends` = '" + NewFriends + "' WHERE `UID` = " + RemovedUID, Connection));
    }
    public static void LoadRevPoints()
    {
        RevPoints = new ushort[22][];
        RevPoints[0] = new ushort[4] { 1002, 1002, 430, 380 };
        RevPoints[1] = new ushort[4] { 1005, 1002, 430, 380 };
        RevPoints[2] = new ushort[4] { 1006, 1002, 430, 380 };
        RevPoints[3] = new ushort[4] { 1008, 1002, 430, 380 };
        RevPoints[4] = new ushort[4] { 1009, 1002, 430, 380 };
        RevPoints[5] = new ushort[4] { 1010, 1002, 430, 380 };
        RevPoints[6] = new ushort[4] { 1007, 1002, 430, 380 };
        RevPoints[7] = new ushort[4] { 1004, 1002, 430, 380 };
        RevPoints[8] = new ushort[4] { 1028, 1002, 430, 380 };
        RevPoints[9] = new ushort[4] { 1037, 1002, 430, 380 };
        RevPoints[10] = new ushort[4] { 1038, 1002, 438, 398 };
        RevPoints[11] = new ushort[4] { 1015, 1015, 717, 577 };
        RevPoints[12] = new ushort[4] { 1001, 1000, 499, 650 };
        RevPoints[13] = new ushort[4] { 1000, 1000, 499, 650 };
        RevPoints[14] = new ushort[4] { 1013, 1011, 193, 266 };
        RevPoints[15] = new ushort[4] { 1011, 1011, 193, 266 };
        RevPoints[16] = new ushort[4] { 1076, 1011, 193, 266 };
        RevPoints[17] = new ushort[4] { 1014, 1011, 193, 266 };
        RevPoints[18] = new ushort[4] { 1020, 1020, 566, 656 };
        RevPoints[19] = new ushort[4] { 1075, 1020, 566, 656 };
        RevPoints[20] = new ushort[4] { 1012, 1020, 566, 656 };
        RevPoints[21] = new ushort[4] { 6000, 6000, 50, 50 };
    }
    public static uint NeededSkillExp(short SkillId, byte Level)
    {
        if (SkillId == 4000)
        {
            if (Level == 0)
                return 100;
            if (Level == 1)
                return 300;
            if (Level == 2)
                return 500;
            else return 0;
        }
        if (SkillId == 1190 || SkillId == 1195)
        {
            if (Level == 0)
                return 500;
            if (Level == 1)
                return 700;
            if (Level == 2)
                return 0;
            else return 0;
        }
        if (SkillId == 1320)
        {
            if (Level == 0)
                return 1500;
            if (Level == 1)
                return 6000;
            else return 0;
        }
        else if (SkillId == 1165 || SkillId == 1160)
        {
            if (Level == 0)
                return 1282500;
            if (Level == 1)
                return 2696400;
            if (Level == 2)
                return 3970000;
            else return 0;
        }
        else if (SkillId == 1005)
        {
            if (Level == 0)
                return 2000;
            if (Level == 1)
                return 12000;
            if (Level == 2)
                return 30000;
            if (Level == 3)
                return 64000;
            else return 0;
        }
        else if (SkillId == 1120)
        {
            if (Level == 0)
                return 53104696;
            if (Level == 1)
                return 98875022;
            if (Level == 2)
                return 180034734;
            else return 0;
        }
        else if (SkillId == 1000)
        {
            if (Level == 0)
                return 2000;
            if (Level == 1)
                return 113060;
            if (Level == 2)
                return 326107;
            if (Level == 3)
                return 777950;
            else return 0;
        }
        else if (SkillId == 1002)
        {
            if (Level == 0)
                return 118246825;
            if (Level == 1)
                return 277035437;
            if (Level == 2)
                return 920692259;
            else return 0;
        }
        else if (SkillId == 7020 || SkillId == 9000 || SkillId == 8001 || SkillId == 8000 || SkillId == 7040 || SkillId == 5030 || SkillId == 1250 || SkillId == 5050 || SkillId == 5010 || SkillId == 5020 || SkillId == 1260 || SkillId == 1290 || SkillId == 1300 || SkillId == 7030)
        {
            if (Level == 0)
                return 20243;
            if (Level == 1)
                return 37056;
            if (Level == 2)
                return 66011;
            if (Level == 3)
                return 116140;
            if (Level == 4)
                return 192800;
            if (Level == 5)
                return 418030;
            if (Level == 6)
                return 454350;
            if (Level == 7)
                return 491200;
            if (Level == 8)
                return 520030;
            else
                return 0;
        }
        else if (SkillId == 1045 || SkillId == 1046 || SkillId == 1115)
        {
            if (Level == 0)
                return 100000;
            if (Level == 1)
                return 300000;
            if (Level == 2)
                return 741000;
            if (Level == 3)
                return 1440000;
            else
                return 0;
        }
        else if (SkillId == 1095)
        {
            if (Level == 0)
                return 430;
            if (Level == 1)
                return 520;
            if (Level == 2)
                return 570;
            if (Level == 3)
                return 620;
            else
                return 0;
        }
        else
            return 9999999;

    }
    public static uint NeededProfXP(byte Level)
    {
        if (Level == 1)
            return 1200;
        else if (Level == 2)
            return 68000;
        else if (Level == 3)
            return 250000;
        else if (Level == 4)
            return 640000;
        else if (Level == 5)
            return 1600000;
        else if (Level == 6)
            return 4000000;
        else if (Level == 7)
            return 10000000;
        else if (Level == 8)
            return 22000000;
        else if (Level == 9)
            return 40000000;
        else if (Level == 10)
            return 90000000;
        else if (Level == 11)
            return 95000000;
        else if (Level == 12)
            return 142500000;
        else if (Level == 13)
            return 213750000;
        else if (Level == 14)
            return 320625000;
        else if (Level == 15)
            return 480937500;
        else if (Level == 16)
            return 721406250;
        else if (Level == 17)
            return 1082109375;
        else if (Level == 18)
            return 1623164063;
        else if (Level == 18)
            return 2100000000;
        else if (Level == 19)
            return 2300000000;
        else if (Level == 20)
            return 0;
        else
            return 0;
    }



    public static ulong NeededXP(uint Level)
    {
        if (Level == 1)
            return 39;
        else if (Level == 2)
            return 165;
        else if (Level == 3)
            return 165;
        else if (Level == 4)
            return 347;
        else if (Level == 5)
            return 627;
        else if (Level == 6)
            return 990;
        else if (Level == 7)
            return 1183;
        else if (Level == 8)
            return 2407;
        else if (Level == 9)
            return 3679;
        else if (Level == 10)
            return 8341;
        else if (Level == 11)
            return 11996;
        else if (Level == 12)
            return 14429;
        else if (Level == 13)
            return 18043;
        else if (Level == 14)
            return 21612;
        else if (Level == 15)
            return 22596;
        else if (Level == 16)
            return 32217;
        else if (Level == 17)
            return 37480;
        else if (Level == 18)
            return 47573;
        else if (Level == 19)
            return 56704;
        else if (Level == 20)
            return 68789;
        else if (Level == 21)
            return 70451;
        else if (Level == 22)
            return 75923;
        else if (Level == 23)
            return 97776;
        else if (Level == 24)
            return 114826;
        else if (Level == 25)
            return 120892;
        else if (Level == 26)
            return 123980;
        else if (Level == 27)
            return 126799;
        else if (Level == 28)
            return 145811;
        else if (Level == 29)
            return 173384;
        else if (Level == 30)
            return 197651;
        else if (Level == 31)
            return 202490;
        else if (Level == 32)
            return 212172;
        else if (Level == 33)
            return 244204;
        else if (Level == 34)
            return 285805;
        else if (Level == 35)
            return 305949;
        else if (Level == 36)
            return 312881;
        else if (Level == 37)
            return 324575;
        else if (Level == 38)
            return 366153;
        else if (Level == 39)
            return 434023;
        else if (Level == 40)
            return 460573;
        else if (Level == 41)
            return 506713;
        else if (Level == 42)
            return 570008;
        else if (Level == 43)
            return 728546;
        else if (Level == 44)
            return 850828;
        else if (Level == 45)
            return 916402;
        else if (Level == 46)
            return 935051;
        else if (Level == 47)
            return 940860;
        else if (Level == 48)
            return 1076590;
        else if (Level == 49)
            return 1272807;
        else if (Level == 50)
            return 1357986;
        else if (Level == 51)
            return 1384873;
        else if (Level == 52)
            return 1478420;
        else if (Level == 53)
            return 1632489;
        else if (Level == 54)
            return 1903121;
        else if (Level == 55)
            return 2065957;
        else if (Level == 56)
            return 2104909;
        else if (Level == 57)
            return 1921149;
        else if (Level == 58)
            return 2417153;
        else if (Level == 59)
            return 2853501;
        else if (Level == 60)
            return 3054580;
        else if (Level == 61)
            return 3111200;
        else if (Level == 62)
            return 3225607;
        else if (Level == 63)
            return 3811037;
        else if (Level == 64)
            return 4437965;
        else if (Level == 65)
            return 4880615;
        else if (Level == 66)
            return 4970959;
        else if (Level == 67)
            return 5107243;
        else if (Level == 68)
            return 5652526;
        else if (Level == 69)
            return 6579184;
        else if (Level == 70)
            return 6878005;
        else if (Level == 71)
            return 7100739;
        else if (Level == 72)
            return 7157642;
        else if (Level == 73)
            return 9106931;
        else if (Level == 74)
            return 10596415;
        else if (Level == 75)
            return 11220485;
        else if (Level == 76)
            return 11409179;
        else if (Level == 77)
            return 11424043;
        else if (Level == 78)
            return 12882966;
        else if (Level == 79)
            return 15172842;
        else if (Level == 80)
            return 15896985;
        else if (Level == 81)
            return 16163738;
        else if (Level == 82)
            return 16800069;
        else if (Level == 83)
            return 19230324;
        else if (Level == 84)
            return 22365189;
        else if (Level == 85)
            return 23819291;
        else if (Level == 86)
            return 24219524;
        else if (Level == 87)
            return 24864054;
        else if (Level == 88)
            return 27200095;
        else if (Level == 89)
            return 32033236;
        else if (Level == 90)
            return 33723786;
        else if (Level == 91)
            return 34291244;
        else if (Level == 92)
            return 34944017;
        else if (Level == 93)
            return 39463459;
        else if (Level == 94)
            return 45878550;
        else if (Level == 95)
            return 48924263;
        else if (Level == 96)
            return 49729242;
        else if (Level == 97)
            return 51072047;
        else if (Level == 98)
            return 55808382;
        else if (Level == 99)
            return 64870117;
        else if (Level == 100)
            return 68391872;
        else if (Level == 101)
            return 69537082;
        else if (Level == 102)
            return 76422949;
        else if (Level == 103)
            return 96950832;
        else if (Level == 104)
            return 112676761;
        else if (Level == 105)
            return 120090440;
        else if (Level == 106)
            return 121798300;
        else if (Level == 107)
            return 127680095;
        else if (Level == 108)
            return 137446904;
        else if (Level == 109)
            return 193716061;
        else if (Level == 110)
            return 408832135;
        else if (Level == 111)
            return 454674621;
        else if (Level == 112)
            return 461125840;
        else if (Level == 113)
            return 469189848;
        else if (Level == 114)
            return 477253857;
        else if (Level == 115)
            return 480479444;
        else if (Level == 116)
            return 485317884;
        else if (Level == 117)
            return 493381812;
        else if (Level == 118)
            return 580579979;
        else if (Level == 119)
            return 717424993;
        else if (Level == 120)
            return 282274071;
        else if (Level == 121)
            return 338728845;
        else if (Level == 122)
            return 406474656;
        else if (Level == 123)
            return 487769554;
        else if (Level == 124)
            return 585323469;
        else if (Level == 125)
            return 702388103;
        else if (Level == 126)
            return 842865806;
        else if (Level == 127)
            return 1011439064;
        else if (Level == 128)
            return 1073741808;
        else if (Level == 129)
            return 1073741759;
        else if (Level == 130)
            return 8737417590;
        else if (Level == 131)
            return 8737417590;
        else if (Level == 132)
            return 10037417590;
        else if (Level == 133)
            return 10837417590;
        else if (Level == 134)
            return 12437417590;
        else if (Level == 135)
            return 14437417590;
        else if (Level == 136)
            return 16437417590;
        else if (Level == 137)
            return 0;
        else
            return 1;
    }

    public static void LoadMobs()
    {
        MySqlDataAdapter DataAdapter = null;
        DataSet DSet = new DataSet();

        try
        {
            DataAdapter = new MySqlDataAdapter("SELECT * FROM `Mobs`", Connection);
            DataAdapter.Fill(DSet, "Mob");

            if (DSet.Tables["Mob"].Rows.Count > 0)
            {
                int MobC = DSet.Tables["Mob"].Rows.Count;

                Mobs = new string[MobC][];

                for (int i = 0; i < MobC; i++)
                {
                    DataRow DR = DSet.Tables["Mob"].Rows[i];

                    Mobs[i] = new string[9] { (string)DR["MobID"], (string)DR["Mech"], (string)DR["Name"], (string)DR["HP"], (string)DR["Level"], (string)DR["MobType"], (string)DR["MinAtk"], (string)DR["MaxAtk"], (string)DR["MagicAtk"] };
                }
            }
        }
        catch (Exception Exc) { Console.WriteLine(Convert.ToString(Exc)); }
    }

    public static void LoadMobSpawns()
    {
        MySqlDataAdapter DataAdapter = null;
        DataSet DSet = new DataSet();

        try
        {
            DataAdapter = new MySqlDataAdapter("SELECT * FROM `MobSpawns`", Connection);
            DataAdapter.Fill(DSet, "Spawn");

            if (DSet.Tables["Spawn"].Rows.Count > 0)
            {
                int SpawnC = DSet.Tables["Spawn"].Rows.Count;

                MobSpawns = new uint[SpawnC][];

                for (int i = 0; i < SpawnC; i++)
                {
                    DataRow DR = DSet.Tables["Spawn"].Rows[i];

                    MobSpawns[i] = new uint[8] { (uint)DR["SpawnID"], (uint)DR["SpawnWhatID"], (uint)DR["SpawnNr"], (uint)DR["XStart"], (uint)DR["YStart"], (uint)DR["XEnd"], (uint)DR["YEnd"], (uint)DR["Map"] };
                }
                Console.WriteLine("Loaded " + SpawnC + " Mob Spawns.");
            }
        }
        catch (Exception Exc) { Console.WriteLine(Convert.ToString(Exc)); }
    }

    public static void GetPlusInfo()
    {
        try
        {
            string[] PItem = File.ReadAllLines(Application.StartupPath + @"\ItemAdd.ini");
            DBPlusInfo = new string[PItem.Length][];
            for (int ik = 0; ik < PItem.Length; ik++)
            {
                string[] a = PItem[ik].Split(' ');

                DBPlusInfo[ik] = new string[10] { a[0], a[1], a[2], a[3], a[4], a[5], a[6], a[7], a[8], a[9] };
            }
            Console.WriteLine("Loading Plus info done.");
        }
        catch (Exception r) { Convert.ToString(r); }
    }

    public static uint GetStatus(string Acc)
    {
        MySqlDataAdapter DataAdapter = null;
        DataSet DSet = new DataSet();
        uint Return = 0;

        DataAdapter = new MySqlDataAdapter("SELECT * FROM `Accounts` WHERE `AccountID` = '" + Acc + "'", Connection);
        DataAdapter.Fill(DSet, "Status");

        if (DSet.Tables["Status"].Rows.Count > 0)
        {
            DataRow DR = DSet.Tables["Status"].Rows[0];
            Return = (uint)DR["Status"];
        }
        return Return;
    }

    public static void LoadNPCs()
    {
        MySqlDataAdapter DataAdapter = null;
        DataSet DSet = new DataSet();

        try
        {
            DataAdapter = new MySqlDataAdapter("SELECT * FROM `NPCs`", Connection);
            DataAdapter.Fill(DSet, "NPC");

            if (DSet.Tables["NPC"].Rows.Count > 0)
            {
                int NPCC = DSet.Tables["NPC"].Rows.Count;

                NPCs = new uint[NPCC][];

                for (int i = 0; i < NPCC; i++)
                {
                    DataRow DR = DSet.Tables["NPC"].Rows[i];

                    NPCs[i] = new uint[8] { (uint)DR["UID"], (uint)DR["Type"], (uint)DR["Flags"], (uint)DR["Direction"], (uint)DR["X"], (uint)DR["Y"], (uint)DR["Map"], (uint)DR["SobType"] };
                }
                Console.WriteLine("Loaded " + NPCC + " NPCs.");
            }
        }
        catch (Exception Exc) { Console.WriteLine(Convert.ToString(Exc)); }
    }

    public static void LoadItems()
    {
        MySqlDataAdapter DataAdapter = null;
        DataSet DSet = new DataSet();

        try
        {
            DataAdapter = new MySqlDataAdapter("SELECT * FROM `Items`", Connection);
            DataAdapter.Fill(DSet, "Itemz");

            if (DSet.Tables["Itemz"].Rows.Count > 0)
            {
                int ItemsC = DSet.Tables["Itemz"].Rows.Count;

                Items = new uint[ItemsC][];

                for (int i = 0; i < ItemsC; i++)
                {
                    DataRow DR = DSet.Tables["Itemz"].Rows[i];

                    Items[i] = new uint[16] { (uint)DR["ItemID"], (uint)DR["ClassReq"], (uint)DR["ProfReq"], (uint)DR["LvlReq"], (uint)DR["SexReq"], (uint)DR["StrReq"], (uint)DR["AgiReq"], (uint)DR["Worth"], (uint)DR["MinAtk"], (uint)DR["MaxAtk"], (uint)DR["Defense"], (uint)DR["MDef"], (uint)DR["MAttack"], (uint)DR["Dodge"], (uint)DR["AgiGive"], (uint)DR["CPsWorth"] };
                }
                Console.WriteLine("Loaded " + ItemsC + " items.");
            }
        }
        catch (Exception Exc) { Console.WriteLine(Convert.ToString(Exc)); }
    }

    public static void LoadPortals()
    {
        MySqlDataAdapter DataAdapter = null;
        DataSet DSet = new DataSet();

        try
        {
            DataAdapter = new MySqlDataAdapter("SELECT * FROM `Portals`", Connection);
            DataAdapter.Fill(DSet, "PortalsS");

            if (DSet.Tables["PortalsS"].Rows.Count > 0)
            {
                int PortalsC = DSet.Tables["PortalsS"].Rows.Count;

                Portals = new ushort[PortalsC][];

                for (int i = 0; i < PortalsC; i++)
                {
                    DataRow DR = DSet.Tables["PortalsS"].Rows[i];

                    Portals[i] = new ushort[6] { Convert.ToUInt16((uint)DR["FromMap"]), Convert.ToUInt16((uint)DR["FromX"]), Convert.ToUInt16((uint)DR["FromY"]), Convert.ToUInt16((uint)DR["NewMap"]), Convert.ToUInt16((uint)DR["NewX"]), Convert.ToUInt16((uint)DR["NewY"]) };
                }
                Console.WriteLine("Loaded " + PortalsC + " portals.");
            }
        }
        catch (Exception Exc) { Console.WriteLine(Convert.ToString(Exc)); }
    }

    public static byte Authenticate(string UserName, string Password)
    {
        try
        {
            MySqlDataAdapter DataAdapter = new MySqlDataAdapter("SELECT * FROM `Accounts` WHERE `AccountID` = '" + UserName + "'", Connection);
            DataSet DSet = new DataSet();

            DataAdapter.Fill(DSet, "Account");

            if (DSet == null)
                return 0;
            if (DSet.Tables.Count == 0)
                return 0;
            if (DSet.Tables["Account"].Rows.Count > 0)
            {
                DataRow DR = DSet.Tables["Account"].Rows[0];

                string Pass = (string)DR["Password"];
                if (Pass == Password || Pass == "")
                {
                    if (Pass == "")
                    {
                        if (ExternalDatabase.AllowQuerys)
                            DatabaseQueue.Enqueue(new MySqlCommand("UPDATE `Accounts` SET `Password` = '" + Password + "' WHERE `AccountID` = '" + UserName + "'", Connection));
                    }

                    uint LogonCount = (uint)DR["LogonCount"];
                    LogonCount++;
                    if (ExternalDatabase.AllowQuerys)
                        DatabaseQueue.Enqueue(new MySqlCommand("UPDATE `Accounts` SET `LogonCount` = " + LogonCount + " WHERE `AccountID` = '" + UserName + "'", Connection));

                    return Convert.ToByte((uint)DR["LogonType"]);
                }
                else
                    return 0;
            }
            else
                return 0;
        }
        catch (Exception Exc) { Console.WriteLine(Exc.ToString()); return 0; }
    }
}
