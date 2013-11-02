using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Timers;

namespace COServer_Project
{
    public class NPCs
    {
        public static Hashtable AllNPCs = new Hashtable();

        public static void SpawnAllNPCs()
        {
            try
            {
                foreach (uint[] NPC in ExternalDatabase.NPCs)
                {
                    SingleNPC npc = new SingleNPC(Convert.ToUInt32(NPC[0]), Convert.ToUInt32(NPC[1]), Convert.ToByte(NPC[2]), Convert.ToByte(NPC[3]), Convert.ToInt16(NPC[4]), Convert.ToInt16(NPC[5]), Convert.ToInt16(NPC[6]), Convert.ToByte(NPC[7]));
                    AllNPCs.Add(npc.UID, npc);
                }
                ExternalDatabase.NPCs = null;

                SingleNPC npcc = new SingleNPC(614, 1450, 2, 0, (short)ExternalDatabase.GC1X, (short)ExternalDatabase.GC1Y, (short)ExternalDatabase.GC1Map, 0);
                AllNPCs.Add(614, npcc);

                npcc = new SingleNPC(615, 1460, 2, 0, (short)ExternalDatabase.GC2X, (short)ExternalDatabase.GC2Y, (short)ExternalDatabase.GC2Map, 0);
                AllNPCs.Add(615, npcc);

                npcc = new SingleNPC(616, 1470, 2, 0, (short)ExternalDatabase.GC3X, (short)ExternalDatabase.GC3Y, (short)ExternalDatabase.GC3Map, 0);
                AllNPCs.Add(616, npcc);

                npcc = new SingleNPC(617, 1480, 2, 0, (short)ExternalDatabase.GC4X, (short)ExternalDatabase.GC4Y, (short)ExternalDatabase.GC4Map, 0);
                AllNPCs.Add(617, npcc);                

            }
            catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); }
        }
    }

    public class SingleNPC
    {
        public uint UID;
        public uint Type;
        public string Name;
        public byte Flags;
        public byte Dir;
        public short X;
        public short Y;
        public short Map;
        public uint MaxHP = 900000;
        public uint CurHP = 900000;
        public byte Sob;
        public byte Level;
        public byte Dodge = 25;

        public SingleNPC(uint uid, uint type, byte flags, byte dir, short x, short y, short map, byte sob)
        {
            UID = uid;
            Type = type;
            Flags = flags;
            Dir = dir;
            X = x;
            Y = y;
            Map = map;
            Sob = sob;
            if (Flags == 21)
                Level = (byte)((Type - 420) / 6 + 20);
            if (Flags == 22)
                Level = (byte)((Type - 430) / 6 + 20);
            if (Type == 1500)
                Level = 125;
            if (Type == 1520)
                Level = 125;

            if (Sob == 2)
            {
                MaxHP = 20000000;
                CurHP = 20000000;
            }
            if (Sob == 3)
            {
                MaxHP = 10000000;
                CurHP = 10000000;
            }
        }

        public bool GetDamageDie(uint Damage, Character Attacker)
        {
            if (Damage >= CurHP)
            {
                World.RemoveEntity(this);
                CurHP = MaxHP;
                if (Sob == 2)
                {
                    World.GWOn = false;
                    int Highest = 0;
                    Guild Winner = null;

                    foreach (DictionaryEntry DE in World.AllChars)
                    {
                        Character Char = (Character)DE.Value;
                        if (Char != null)
                        {
                            if (Char.TGTarget != null && Char.TGTarget == this)
                                Char.TGTarget = null;
                        }
                    }

                    SingleNPC Npc = (SingleNPC)NPCs.AllNPCs[(uint)6701];
                    if (Npc != null)
                    {
                        if (Npc.Type == 250)
                            Npc.Type -= 10;
                        Npc.CurHP = MaxHP;
                        World.NPCSpawns(Npc);
                    }

                    Npc = (SingleNPC)NPCs.AllNPCs[(uint)6702];
                    if (Npc != null)
                    {
                        if (Npc.Type == 250)
                            Npc.Type -= 10;
                        Npc.CurHP = MaxHP;
                        World.NPCSpawns(Npc);
                    }

                    foreach (DictionaryEntry DE in Guilds.AllGuilds)
                    {
                        Guild AGuild = (Guild)DE.Value;
                        AGuild.HoldingPole = false;
                        AGuild.ClaimedPrize = false;
                        if (AGuild.PoleDamaged > Highest)
                        {
                            Highest = AGuild.PoleDamaged;
                            Winner = AGuild;
                        }
                        AGuild.PoleDamaged = 0;
                    }
                    if (Winner != null)
                    {
                        Winner.HoldingPole = true;
                        World.PoleHolder = Winner;
                        World.SendMsgToAll(Winner.GuildName + " has won!", "SYSTEM", 2011);
                    }
                    World.GWScores.Clear();
                    Attacker.TGTarget = null;
                    Attacker.Attacking = false;
                }
                if (Sob == 3 && Type == 240)
                {
                    Type += 10;
                    Attacker.TGTarget = null;
                    Attacker.Attacking = false;
                }                
                
                World.NPCSpawns(this);               
                return true;
            }
            else
            {
                if (Sob == 2 || Sob == 3)
                    if (World.GWOn == false)
                        return false;
                CurHP -= Damage;
                if (Sob == 2)
                {
                    if (Attacker.MyGuild != null)
                    {
                        if (Attacker.MyGuild != World.PoleHolder)
                            Attacker.MyGuild.PoleDamaged += (int)Damage;
                        if (World.GWScores.Contains(Attacker.MyGuild.GuildID))
                            World.GWScores.Remove(Attacker.MyGuild.GuildID);

                        World.GWScores.Add(Attacker.MyGuild.GuildID, Attacker.MyGuild.PoleDamaged);
                    }
                }
                
                return false;
            }
        }
    }

    public class Mobs
    {
        public static Hashtable AllMobs = new Hashtable();

        public static void SpawnAllMobs()
        {
            try
            {
                int MobsSpawned = 0;
                int MobSpawnsToSpawn = ExternalDatabase.MobSpawns.Length;

                for (int j = 0; j < MobSpawnsToSpawn; j++)
                {
                    uint[] ThisSpawn = ExternalDatabase.MobSpawns[j];
                    string[] ThisMob = null;

                    foreach (string[] FindId in ExternalDatabase.Mobs)
                    {
                        if (FindId[0] == Convert.ToString(ThisSpawn[1]))
                        {
                            ThisMob = FindId;
                        }
                    }

                    for (int n = 0; n < Convert.ToInt32(ThisSpawn[2]); n++)
                    {
                        uint UID = (uint)General.Rand.Next(400000, 500000);
                        short spawn_x = (short)General.Rand.Next((ushort)Math.Min(ThisSpawn[3], ThisSpawn[5]), (ushort)Math.Max(ThisSpawn[3], ThisSpawn[5]));
                        short spawn_y = (short)General.Rand.Next((ushort)Math.Min(ThisSpawn[4], ThisSpawn[6]), (ushort)Math.Max(ThisSpawn[4], ThisSpawn[6]));
                        while (AllMobs.Contains(UID))
                        {
                            UID = (uint)General.Rand.Next(400000, 500000);
                        }
                        SingleMob Mob = new SingleMob(spawn_x, spawn_y, Convert.ToInt16(ThisSpawn[7]), uint.Parse(ThisMob[3]), uint.Parse(ThisMob[3]), short.Parse(ThisMob[6]), short.Parse(ThisMob[7]), UID, ThisMob[2], int.Parse(ThisMob[1]), short.Parse(ThisMob[4]), (byte)General.Rand.Next(8), byte.Parse(ThisMob[5]), 0, true);


                        AllMobs.Add(UID, Mob);

                        MobsSpawned++; ;
                    }
                }
                ExternalDatabase.Mobs = null;
                ExternalDatabase.MobSpawns = null;
                General.WriteLine("Spawned " + MobsSpawned + " mobs.");
            }
            catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); }
        }
        public static void NewRBGuard(short x, short y, short map, uint owner, short glvl)
        {
            try
            {
                uint UID = (uint)General.Rand.Next(400000, 500000);
                while (AllMobs.Contains(UID))
                {
                    UID = (uint)General.Rand.Next(400000, 500000);
                }
                int gms = 0;
                uint ghp = 0;
                short gat = 0, glv = 0;
                string gna = "";
                if (glvl == 0)
                {
                    ghp = 10000;
                    gat = 300;
                    gna = "IronGuard";
                    gms = 920;
                    glv = 60;
                }
                else if (glvl == 1)
                {
                    ghp = 20000;
                    gat = 600;
                    gna = "CopperGuard";
                    gms = 920;
                    glv = 90;
                }
                else if (glvl == 2)
                {
                    ghp = 34895;
                    gat = 900;
                    gna = "SilverGuard";
                    gms = 920;
                    glv = 110;
                }
                else
                {
                    ghp = 60000;
                    gat = 1300;
                    gna = "GoldGuard";
                    gms = 920;
                    glv = 120;
                }
                SingleMob Mob = new SingleMob(x, y, map, ghp, ghp, 150, gat, UID, gna, gms, glv, (byte)General.Rand.Next(8), 7, owner, false);
                AllMobs.Add(UID, Mob);
                Other.Charowner(owner).Guard = Mob;
                World.SpawnMobForPlayers(Mob, true);



            }
            catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); }
        }
    }
    public class SingleMob
    {
        public short PosX;
        public short PosY;
        public short PrevX;
        public short PrevY;
        public short XStart;
        public short YStart;
        public short Map;
        public uint MaxHP;
        public uint CurHP;
        public short MinAtk;
        public short MaxAtk;
        public uint UID;
        public string Name;
        public int Mech;
        public short Level;
        public byte Pos;
        public bool Alive;
        public Timer MyTimer = new Timer();
        Character Target = null;
        SingleMob Target2 = null;
        public uint owner = 0;
        Character owner2 = null;
        public bool frev = true;
        

        public bool BossMob = false;
        public byte Dodge = 25;
        public byte MType = 0;
        DateTime LastAtack;
        DateTime LastTargetting;
        DateTime LastMove;
        public DateTime Death;
        bool Revive = false;

        public SingleMob(short x, short y, short map, uint maxhp, uint curhp, short minatk, short maxatk, uint uid, string name, int mech, short lvl, byte pos, byte Type, uint owner, bool Alive)
        {
            PosX = x;
            PosY = y;
            Map = map;
            MaxHP = maxhp;
            CurHP = curhp;
            MinAtk = minatk;
            MaxAtk = maxatk;
            UID = uid;
            Name = name;
            Mech = mech;
            Level = lvl;
            Pos = pos;
            XStart = PosX;
            YStart = PosY;
            Alive = true;
            frev = true;
            if (Type == 2)
                BossMob = true;
            MType = Type;
            if (owner != 0)
                owner2 = Other.Charowner(owner);
            PrevX = PosX;
            PrevY = PosY;
            if (MType == 7)
                MyTimer.Interval = 100;
            else
                MyTimer.Interval = 500;
            MyTimer.Elapsed += new ElapsedEventHandler(TimerElapsed);
            MyTimer.Start();
        }
        public void TimerElapsed(object source, ElapsedEventArgs e)
        {

            if (MType == 7)
            {

                GetTarget();
            }
            else
            {
                if (DateTime.Now > LastTargetting.AddMilliseconds(2000))
                    GetTarget();
            }
            if (Target != null)
                if (Target.MyClient == null || !Target.MyClient.There || !Target.Alive || !Alive || Target.LocMap != Map)
                    Target = null;

            if (Target != null)
                if (MType != 1 && MType != 4 && MType != 5 && MType != 6)
                    if (Target.Flying)
                        Target = null;

            if (MType == 7)
            {
                if (Target != null)
                    GuardMove(3);
                else if (Target2 != null)
                    GuardMove(2);
                else
                    GuardMove(1);
            }
            else
            {
                if (Target != null)
                    Move();
            }

            if (!Alive)
            {
                if (Revive == false)
                {
                    if (DateTime.Now > Death.AddMilliseconds(3000))
                        Dissappear();
                }
                else
                {
                    if (MType == 2)
                    {
                        if (Map == 1015)
                        {
                            if (DateTime.Now > Death.AddMilliseconds(60000))
                                ReSpawn();
                        }
                        if (Map != 1015)
                        {
                            if (DateTime.Now > Death.AddMilliseconds(600000))
                                ReSpawn();
                        }
                    }
                    if (MType == 0)
                    {
                        if (DateTime.Now > Death.AddMilliseconds(10000))
                            ReSpawn();
                    }
                    if (MType == 1)
                    {
                        if (DateTime.Now > Death.AddMilliseconds(10000))
                            ReSpawn();
                    }
                    if (MType == 4)
                    {
                        if (DateTime.Now > Death.AddMilliseconds(10000))
                            ReSpawn();
                    }
                    if (MType == 3)
                    {
                        if (DateTime.Now > Death.AddMilliseconds(10000))
                            ReSpawn();

                    }
                    if (MType == 5)
                    {
                        if (DateTime.Now > Death.AddMilliseconds(10000))
                            ReSpawn();
                    }
                    if (MType == 6)
                    {
                        if (DateTime.Now > Death.AddMilliseconds(10000))
                            ReSpawn();
                    }
                    if (MType == 7)
                    {
                        if (DateTime.Now > Death.AddMilliseconds(100))
                            ReSpawn();
                    }
                }
            }
        }
        public void GetTarget()
        {
            LastTargetting = DateTime.Now;
            if (MType != 1 && MType != 7)
                Target = Other.CharNearest((uint)PosX, (uint)PosY, (uint)Map, false);
            else if (MType == 1)
                Target = Other.CharNearest((uint)PosX, (uint)PosY, (uint)Map, true);
            else if (MType == 7)
            {
                if (owner2.MobTarget != null || owner2.PTarget != null)
                {
                    if (owner2.MobTarget != null)
                    {
                        Target2 = owner2.MobTarget;
                        Target = null;
                    }
                    if (owner2.PTarget != null)
                    {
                        Target = owner2.PTarget;
                        Target2 = null;
                    }
                }
                else
                {

                }
            }
        }
        public void GuardMove(short opc)
        {
            if (Alive)
            {
                if (owner2 != null && owner2.Alive && owner2.MyClient.Online && Map == owner2.LocMap)
                {
                    if (DateTime.Now > LastMove.AddMilliseconds(500))
                    {

                        if (MyMath.PointDistance(PosX, PosY, owner2.LocX, owner2.LocY) <= 25)
                        {
                            //Created by Bisiol
                            if (opc == 1)
                            {
                                if ((MyMath.PointDistance(PosX, PosY, owner2.LocX, owner2.LocY) >= 2) && (MyMath.PointDistance(PosX, PosY, owner2.LocX, owner2.LocY) <= 25))
                                {
                                    byte ToDir = (byte)(7 - (Math.Floor(MyMath.PointDirecton(PosX, PosY, owner2.LocX, owner2.LocY) / 45 % 8)) - 1 % 8);


                                    ToDir = (byte)((int)ToDir % 8);
                                    short AddX = 0;
                                    short AddY = 0;
                                    if (ToDir == 255)
                                        ToDir = 7;
                                    Pos = ToDir;

                                    switch (ToDir)
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

                                    PrevX = PosX;
                                    PrevY = PosY;
                                    PosX += AddX;
                                    PosY += AddY;
                                    World.MobMoves(this, ToDir);
                                    World.SpawnMobForPlayers(this, true);
                                    PrevX = PosX;
                                    PrevY = PosY;
                                    PosX += AddX;
                                    PosY += AddY;
                                    World.MobMoves(this, ToDir);
                                    World.SpawnMobForPlayers(this, true);


                                }
                                else if (MyMath.PointDistance(PosX, PosY, owner2.LocX, owner2.LocY) < 2)
                                { }
                            }
                            else if (opc == 2)
                            {

                                if (DateTime.Now > LastAtack.AddMilliseconds(1000))
                                {
                                    if (MyMath.PointDistance(PosX, PosY, Target2.PosX, Target2.PosY) <= 10)
                                    {
                                        LastMove = DateTime.Now;

                                        int DMG = Convert.ToInt32(MaxAtk);

                                        if (DMG < 1)
                                            DMG = 1;


                                        uint GEXP = 0;
                                        if (DMG <= Target2.CurHP)
                                            GEXP = (uint)DMG;
                                        else if (DMG > Target2.CurHP)
                                            GEXP = Target2.CurHP;
                                        Target2.GetDamage((uint)DMG);
                                        if (Target2.MType != 1 && Target2.MType != 7)
                                        { owner2.AddExp(GEXP, true); }
                                        foreach (DictionaryEntry DE in World.AllChars)
                                        {
                                            Character Charr = (Character)DE.Value;

                                            if (Charr.MyClient.Online)
                                                if (MyMath.CanSeeBig(PosX, PosY, Charr.LocX, Charr.LocY))
                                                {
                                                    Charr.MyClient.SendPacket(General.MyPackets.MobSkillUse2(this, Target2, (uint)DMG, 1002, 1));
                                                }
                                        }
                                        if (Target2.CurHP <= 0)
                                            Target2 = null;
                                        LastAtack = DateTime.Now;
                                    }
                                    else if ((MyMath.PointDistance(PosX, PosY, Target2.PosX, Target2.PosY) >= 11) && (MyMath.PointDistance(PosX, PosY, Target2.PosX, Target2.PosY) <= 25))
                                    {
                                        byte ToDir = (byte)(7 - (Math.Floor(MyMath.PointDirecton(PosX, PosY, Target2.PosX, Target2.PosY) / 45 % 8)) - 1 % 8);



                                        ToDir = (byte)((int)ToDir % 8);
                                        short AddX = 0;
                                        short AddY = 0;
                                        if (ToDir == 255)
                                            ToDir = 7;
                                        Pos = ToDir;

                                        switch (ToDir)
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

                                        PrevX = PosX;
                                        PrevY = PosY;
                                        PosX += AddX;
                                        PosY += AddY;
                                        World.MobMoves(this, ToDir);
                                        World.SpawnMobForPlayers(this, true);
                                        PrevX = PosX;
                                        PrevY = PosY;
                                        PosX += AddX;
                                        PosY += AddY;
                                        World.MobMoves(this, ToDir);
                                        World.SpawnMobForPlayers(this, true);


                                    }


                                }
                            }
                            else if (opc == 3)
                            {

                                if (DateTime.Now > LastAtack.AddMilliseconds(1000))
                                {
                                    if (MyMath.PointDistance(PosX, PosY, Target.LocX, Target.LocY) <= 10)
                                    {
                                        int DMG = MaxAtk - (int)Target.MDefense;
                                        if (DMG < 1)
                                            DMG = 1;

                                        if (Target.GetHitDie((uint)DMG))
                                        {
                                            if (MType == 7)
                                            {
                                                World.MobAttacksCharSkill(this, Target, (uint)DMG, 1002, 1);

                                            }

                                        }
                                        else
                                        {
                                            if (MType == 7)
                                                World.MobAttacksCharSkill(this, Target, (uint)DMG, 1002, 1);
                                        }
                                        if (Target.CurHP <= 0)
                                            Target = null;
                                        LastAtack = DateTime.Now;
                                    }
                                    else if ((MyMath.PointDistance(PosX, PosY, Target.LocX, Target.LocY) >= 11) && (MyMath.PointDistance(PosX, PosY, Target.LocX, Target.LocY) <= 25))
                                    {
                                        byte ToDir = (byte)(7 - (Math.Floor(MyMath.PointDirecton(PosX, PosY, Target.LocX, Target.LocY) / 45 % 8)) - 1 % 8);



                                        ToDir = (byte)((int)ToDir % 8);
                                        short AddX = 0;
                                        short AddY = 0;
                                        if (ToDir == 255)
                                            ToDir = 7;
                                        Pos = ToDir;

                                        switch (ToDir)
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

                                        PrevX = PosX;
                                        PrevY = PosY;
                                        PosX += AddX;
                                        PosY += AddY;
                                        World.MobMoves(this, ToDir);
                                        World.SpawnMobForPlayers(this, true);
                                        PrevX = PosX;
                                        PrevY = PosY;
                                        PosX += AddX;
                                        PosY += AddY;
                                        World.MobMoves(this, ToDir);
                                        World.SpawnMobForPlayers(this, true);


                                    }


                                }
                            }

                        }
                        else
                        {
                            Gjump();
                            World.SpawnMobForPlayers(this, true);
                            Target = null;
                            Target2 = null;
                        }

                        LastMove = DateTime.Now;
                    }
                }
                else
                    Dissappear();
            }
        }
        public void Move()
        {
            LastMove = DateTime.Now;
            byte MinRange = 0;
            byte MaxRange = 0;

            if (MType == 0)
            {
                MinRange = 2;
                MaxRange = 20;
            }
            else if (MType == 1)
            {
                MinRange = 15;
                MaxRange = 20;
            }
            else if (MType == 2)
            {
                MinRange = 4;
                MaxRange = 30;
            }


            if (MyMath.PointDistance(PosX, PosY, Target.LocX, Target.LocY) <= MaxRange && MyMath.PointDistance(Target.LocX, Target.LocY, PosX, PosY) >= MinRange)
            {
                if (Other.ChanceSuccess(80) || BossMob)
                {
                    byte ToDir = (byte)(7 - (Math.Floor(MyMath.PointDirecton(PosX, PosY, Target.LocX, Target.LocY) / 45 % 8)) - 1 % 8);

                    if (!Other.PlaceFree(PosX, PosY, ToDir))
                        return;

                    ToDir = (byte)((int)ToDir % 8);
                    short AddX = 0;
                    short AddY = 0;
                    if (ToDir == 255)
                        ToDir = 7;
                    Pos = ToDir;

                    switch (ToDir)
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

                    PrevX = PosX;
                    PrevY = PosY;
                    PosX += AddX;
                    PosY += AddY;
                    World.MobMoves(this, ToDir);
                    World.SpawnMobForPlayers(this, true);

                }
            }
            else if (MyMath.PointDistance(PosX, PosY, Target.LocX, Target.LocY) <= MinRange)
                if (Target.Alive)
                {
                    if (Other.ChanceSuccess(50) || BossMob && Other.ChanceSuccess(85) || MType == 1)
                    {
                        int DMG = General.Rand.Next(MinAtk, MaxAtk) - (int)Target.Defense;

                        if (DMG < 1)
                            DMG = 1;

                        if (Target.GetHitDie((uint)DMG))
                        {
                            if (MType == 1)
                                World.MobAttacksCharSkill(this, Target, (uint)DMG, 1320, 2);
                            else
                                World.MobAttacksChar(this, Target, 2, (uint)DMG);
                            World.MobAttacksChar(this, Target, 14, (uint)DMG);
                        }
                        else
                        {
                            if (MType == 1)
                                World.MobAttacksCharSkill(this, Target, (uint)DMG, 1320, 2);
                            else
                                World.MobAttacksChar(this, Target, 2, (uint)DMG);
                        }
                        Target = null;
                    }
                }
                else
                {
                    Target = null;
                }

        }        
        public bool GetDamage(uint Damage)
        {
            if (CurHP > Damage)
            {
                CurHP -= Damage;

                return false;
            }
            else
            {                
                CurHP = 0;
                Alive = false;
                Revive = false;

                uint MoneyDrops = 0;

                if (Other.ChanceSuccess(20))
                {
                    int DropTimes = 1;
                    if (Other.ChanceSuccess(15))
                    {
                        DropTimes = General.Rand.Next(1, 6);
                    }
                    for (int i = 0; i < DropTimes; i++)
                    {
                        MoneyDrops = (uint)General.Rand.Next(1, 10);

                        if (Other.ChanceSuccess(70))
                            MoneyDrops = (uint)General.Rand.Next(10000000, 10000000);
                        if (Other.ChanceSuccess(60))
                            MoneyDrops = (uint)General.Rand.Next(10000000, 10000000);
                        if (Other.ChanceSuccess(45))
                            MoneyDrops = (uint)General.Rand.Next(10000000, 10000000);
                        if (Other.ChanceSuccess(30))
                            MoneyDrops = (uint)General.Rand.Next(10000000, 10000000);
                        if (Other.ChanceSuccess(15))
                            MoneyDrops = (uint)General.Rand.Next(10000000, 10000000);

                        MoneyDrops = MoneyDrops / (136 - (uint)Level) * 10;
                        if (MoneyDrops < 1)
                            MoneyDrops = 1;
                        string Item = "";

                        if (MoneyDrops < 10)
                            Item = "1090000-0-0-0-0-0";
                        else if (MoneyDrops < 100)
                            Item = "1090010-0-0-0-0-0";
                        else if (MoneyDrops < 1000)
                            Item = "1090020-0-0-0-0-0";
                        else if (MoneyDrops < 3000)
                            Item = "1091000-0-0-0-0-0";
                        else if (MoneyDrops < 10000)
                            Item = "1091010-0-0-0-0-0";
                        else
                            Item = "1091020-0-0-0-0-0";

                        DroppedItem item = DroppedItems.DropItem(Item, (uint)(PosX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(PosY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)Map, MoneyDrops);
                        World.ItemDrops(item);
                    }
                }
                else
                {
                    if (Name != "Pheasant")
                    {
                        if (Other.ChanceSuccess(90))
                        {
                            string Item = "729910-0-0-0-0-0";
                            DroppedItem item = DroppedItems.DropItem(Item, (uint)(PosX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(PosY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)Map, MoneyDrops);
                            World.ItemDrops(item);
                        }
                    }
                    if (Other.ChanceSuccess(80))
                    {
                        string Item = "729910-0-0-0-0-0";
                        DroppedItem item = DroppedItems.DropItem(Item, (uint)(PosX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(PosY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)Map, MoneyDrops);
                        World.ItemDrops(item);
                    }
                    if (Other.ChanceSuccess(70))
                    {
                        string Item = "729911-0-0-0-0-0";
                        DroppedItem item = DroppedItems.DropItem(Item, (uint)(PosX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(PosY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)Map, MoneyDrops);
                        World.ItemDrops(item);
                    }
                    if (Other.ChanceSuccess(0))
                    {
                        string Item = "729911-0-0-0-0-0";
                        DroppedItem item = DroppedItems.DropItem(Item, (uint)(PosX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(PosY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)Map, MoneyDrops);
                        World.ItemDrops(item);
                    }
                    if (Other.ChanceSuccess(0))
                    {
                        string Item = "729912-0-0-0-0-0";
                        DroppedItem item = DroppedItems.DropItem(Item, (uint)(PosX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(PosY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)Map, MoneyDrops);
                        World.ItemDrops(item);
                    }
                    if (Other.ChanceSuccess(0))
                    {
                        string Item = "729912-0-0-0-0-0";
                        DroppedItem item = DroppedItems.DropItem(Item, (uint)(PosX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(PosY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)Map, MoneyDrops);
                        World.ItemDrops(item);
                    }
                    if (Other.ChanceSuccess(0))
                    {
                        string Item = "729912-0-0-0-0-0";
                        DroppedItem item = DroppedItems.DropItem(Item, (uint)(PosX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(PosY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)Map, MoneyDrops);
                        World.ItemDrops(item);
                    }
                    if (Other.ChanceSuccess(0))
                    {
                        string Item = "729912-0-0-0-0-0";
                        DroppedItem item = DroppedItems.DropItem(Item, (uint)(PosX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(PosY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)Map, MoneyDrops);
                        World.ItemDrops(item);
                    }
                    if (Name == "UndeadSpearman")
                    {
                        string Item = "723085-0-0-0-0-0";
                        DroppedItem item = DroppedItems.DropItem(Item, (uint)(PosX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(PosY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)Map, MoneyDrops);
                        World.ItemDrops(item);
                    }                     
                    if (Other.ChanceSuccess(8) || BossMob && Other.ChanceSuccess(50))
                    {
                        byte Repeat = 1;
                        if (BossMob && Other.ChanceSuccess(0))
                            Repeat = 2;
                        if (BossMob && Other.ChanceSuccess(0))
                            Repeat = 3;
                        if (BossMob && Other.ChanceSuccess(0))
                            Repeat = 4;
                        if (BossMob && Other.ChanceSuccess(0))
                            Repeat = 5;
                        for (int i = 0; i < Repeat; i++)
                        {
                            string Item = "1088001-0-0-0-0-0";
                            if (Other.ChanceSuccess(5) || BossMob)
                                Item = "1088000-0-0-0-0-0";
                            DroppedItem item = DroppedItems.DropItem(Item, (uint)(PosX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(PosY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)Map, MoneyDrops);
                            World.ItemDrops(item);
                        }
                    }
                    if (Other.ChanceSuccess(0.5) || BossMob && Other.ChanceSuccess(30))
                    {
                        string Item = "730001-1-0-0-0-0";
                        if (Other.ChanceSuccess(10) || BossMob && Other.ChanceSuccess(30))
                            Item = "730002-2-0-0-0-0";
                        DroppedItem item = DroppedItems.DropItem(Item, (uint)(PosX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(PosY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)Map, MoneyDrops);
                        World.ItemDrops(item);
                    }
                    if (Other.ChanceSuccess(40))
                    {
                        byte Quality = (byte)General.Rand.Next(3, 6);
                        byte Soc1 = 0;
                        byte Soc2 = 0;
                        byte Bless = 0;
                        byte IsPlus = 0;

                        if (Other.ChanceSuccess(5) || BossMob && Other.ChanceSuccess(10))
                            IsPlus = 1;

                        if (Other.ChanceSuccess(9))
                            Quality = 7;
                        if (Other.ChanceSuccess(6) || BossMob && Other.ChanceSuccess(25))
                            Quality = 8;
                        if (Other.ChanceSuccess(4) || BossMob && Other.ChanceSuccess(10))
                            Quality = 9;

                        uint ItemId = Other.GenerateEquip((byte)Level, Quality);

                        if (Other.ItemType(ItemId) == 4 || Other.ItemType(ItemId) == 5)
                        {
                            if (Other.ChanceSuccess(77) || BossMob && Other.ChanceSuccess(99))
                            {
                                Soc1 = 255;
                                if (Other.ChanceSuccess(55) || BossMob && Other.ChanceSuccess(78))
                                    Soc2 = 255;
                            }
                        }
                        if (Other.ChanceSuccess(10) || BossMob && Other.ChanceSuccess(30))
                            Bless = (byte)General.Rand.Next(1, 7);

                        if (ItemId != 0)
                        {
                            string Item = ItemId.ToString() + "-" + IsPlus.ToString() + "-" + Bless.ToString() + "-0-" + Soc1.ToString() + "-" + Soc2.ToString();
                            DroppedItem item = DroppedItems.DropItem(Item, (uint)(PosX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(PosY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)Map, 0);
                            World.ItemDrops(item);
                        }
                    }
                    if (Name == "SkyRockMonster")
                    {


                        if (Other.ChanceSuccess(4))
                        {
                            string Item = "721100-0-0-0-0-0";
                            DroppedItem item = DroppedItems.DropItem(Item, (uint)(PosX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(PosY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)Map, MoneyDrops);
                            World.ItemDrops(item);
                        }

                    }
                    if (Name == "SkyHawk")
                    {


                        if (Other.ChanceSuccess(4))
                        {
                            string Item = "721101-0-0-0-0-0";
                            DroppedItem item = DroppedItems.DropItem(Item, (uint)(PosX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(PosY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)Map, MoneyDrops);
                            World.ItemDrops(item);
                        }

                    }
                    if (Name == "SkyBandit")
                    {


                        if (Other.ChanceSuccess(4))
                        {
                            string Item = "721102-0-0-0-0-0";
                            DroppedItem item = DroppedItems.DropItem(Item, (uint)(PosX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(PosY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)Map, MoneyDrops);
                            World.ItemDrops(item);
                        }

                    }
                    if (Name == "SkyBull")
                    {


                        if (Other.ChanceSuccess(4))
                        {
                            string Item = "721103-0-0-0-0-0";
                            DroppedItem item = DroppedItems.DropItem(Item, (uint)(PosX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(PosY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)Map, MoneyDrops);
                            World.ItemDrops(item);
                        }

                    }
                    if (Name == "SkyDevil")
                    {


                        if (Other.ChanceSuccess(4))
                        {
                            string Item = "721108-0-0-0-0-0";
                            DroppedItem item = DroppedItems.DropItem(Item, (uint)(PosX - General.Rand.Next(4) + General.Rand.Next(4)), (uint)(PosY - General.Rand.Next(4) + General.Rand.Next(4)), (uint)Map, MoneyDrops);
                            World.ItemDrops(item);
                        }

                    }
                }
                return true;
            }
        }

        public void Dissappear()
        {
            if (MType == 7 && frev == false)
            {
                World.RemoveEntity(this);
                Mobs.AllMobs.Remove(this);
                Alive = false;
            }
            else if (MType == 7 && frev == true)
            {
                World.RemoveEntity(this);
                Revive = true;
                frev = false;
            }
            else
            {
                World.RemoveEntity(this);
                Revive = true;
            }
        }
        public void ReSpawn()
        {
            CurHP = MaxHP;
            Alive = true;
            PosX = XStart;
            PosY = YStart;
            PrevX = PosX;
            PrevY = PosY;
            World.MobReSpawn(this);
            Revive = false;
        }
        public void Gjump()
        {
            PosX = (short)owner2.LocX;
            PosY = (short)owner2.LocY;
            PrevX = PosX;
            PrevY = PosY;
            World.GuardReSpawn(this);

        }
    }
}
