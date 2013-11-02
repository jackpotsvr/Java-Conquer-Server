using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Timers;
using System.Threading;

namespace COServer_Project
{
    public class Character
    {
        public Client MyClient;

        public uint UID = 0;
        public ushort Model = 0;
        public ushort Avatar = 0;
        public ushort RealModel = 0;
        public ushort RealAvatar = 0;
        public bool Alive = true;
        public uint SkillExpNull = 0;
        public byte Stamina = 0;
        public bool Vending = false;

        public string Name = "";
        public string Spouse = "";
        public string PackedInventory = "";
        public string PackedEquips = "";
        public string PackedSkills = "";
        public string PackedProfs = "";
        public string PackedWHs = "";
        public string PackedFriends = "";
        public string PackedEnemies = "";
        public bool Attacking = false;

        public uint EPotXP = 0;
        public uint EPotXP2 = 0;
        public bool EPotRate = false;
        public bool EPot = false;
        public System.Timers.Timer EPotTimer = new System.Timers.Timer();

        public byte Level = 0;
        public byte Job = 0;
        public byte FirstJob = 0;
        public byte Rank = 0;
        public uint Donation = 0;
        public uint FCPs = 0;

        public byte Action = 100;
        public byte Direction = 3;
        public byte PKMode = 0;

        public uint LuckTime = 0;
        public DateTime PrayCasted = DateTime.Now;
        public ushort PrayX = 0;
        public ushort PrayY = 0;
        public bool CanPray = false;

        public byte RBCount = 0;
        public ulong Exp = 0;
        public uint WHSilvers = 0;
        public uint Silvers = 0;
        public uint CPs = 0;
        public ushort PKPoints = 0;
        public uint VP = 0;

        public ushort Str = 0;
        public ushort Agi = 0;
        public ushort Vit = 0;
        public ushort Spi = 0;
        public ushort StatP = 0;
        public ushort Mana = 0;
        public ushort MaxHP = 0;
        public ushort CurHP = 0;
        public ushort Hair = 0;
        public ushort ShowHair = 0;
        public ushort LocX = 0;
        public ushort LocY = 0;
        public ushort LocMap = 0;
        public ushort PrevX = 0;
        public ushort PrevY = 0;
        public ushort RealAgi = 0;
        public double AddAtkPc = 1;
        public double AddMAtkPc = 1;
        public double AddExpPc = 1;
        public double AddProfPc = 1;        

        public ushort Defense = 0;
        public uint MDefense = 0;
        public double MinAtk = 0;
        public double MaxAtk = 0;
        public double MAtk = 0;
        public byte Dodge = 0;

        public ushort PrevMap = 0;

        public Hashtable Skills = new Hashtable();
        public Hashtable Skill_Exps = new Hashtable();
        public Hashtable Profs = new Hashtable();
        public Hashtable Prof_Exps = new Hashtable();
        public Hashtable Friends = new Hashtable();
        public Hashtable Enemies = new Hashtable();

        public string[] Equips = new string[10];
        public string[] Inventory = new string[42];
        public string[] TCWH = new string[20];
        public string[] PCWH = new string[20];
        public string[] BIWH = new string[20];
        public string[] ACWH = new string[20];
        public string[] DCWH = new string[20];
        public string[] MAWH = new string[40];

        public uint[] Equips_UIDs = new uint[10];
        public uint[] Inventory_UIDs = new uint[41];
        public uint[][] WHIDs = new uint[6][];

        public byte ItemsInInventory = 0;
        public byte AddedSkills = 0;
        public byte TCWHCount = 0;
        public byte PCWHCount = 0;
        public byte BIWHCount = 0;
        public byte ACWHCount = 0;
        public byte DCWHCount = 0;
        public byte MAWHCount = 0;

        public SingleMob Guard = null;
        public SingleMob MobTarget = null;
        public Character PTarget = null;
        public SingleNPC TGTarget = null;
        public byte AtkType = 0;
        public ushort Potency = 0;
        public ushort SkillLooping = 0;
        public ushort SkillLoopingX = 0;
        public ushort SkillLoopingY = 0;
        public uint SkillLoopingTarget = 0;
        public bool BlueName = false;
        public bool Poisoned = false;
        public bool TeamLeader = false;
        public bool AllSuper = false;
        public bool SMOn = false;
        public bool Invisible = false;
        public bool CycloneOn = false;
        public bool XpList = false;
        public bool CastingPray = false;
        public bool Praying = false;
        public byte XpCircle = 0;
        public ArrayList Team = new ArrayList(4);
        public Character MyTeamLeader = null;
        public byte PlayersInTeam = 0;
        public byte TeamCount = 0;
        public bool JoinForbidden = false;
        public bool Trading = false;
        public uint TradingWith = 0;
        public ArrayList MyTradeSide = new ArrayList(20);
        public uint TradingSilvers = 0;
        public uint TradingCPs = 0;
        public bool TradeOK = false;
        public byte MyTradeSideCount = 0;
        public bool Flying = false;

        public bool StigBuff = false;
        public bool AccuracyBuff = false;
        public bool ShieldBuff = false;
        public double ExtraXP = 0;
        public bool AccuracyOn = false;


        public uint TargetUID = 0;
        public uint RequestFriendWith = 0;

        public uint GuildDonation = 0;
        public ushort GuildID = 0;
        public byte GuildPosition = 0;

        public Guild MyGuild;
        public DateTime LastAttack;
        public DateTime LastXPC;
        public DateTime Death;
        public DateTime LastSave;
        public DateTime XPActivated;
        public DateTime GotBlueName;
        public DateTime LostPKP;
        public DateTime LastSwing = DateTime.Now;
        public DateTime AccuracyActivated = DateTime.Now;
        public DateTime LastTargetting = DateTime.Now;
        public DateTime LastGWList = DateTime.Now;
        public DateTime FlyActivated = DateTime.Now;
        public DateTime Stigged = DateTime.Now;
        public byte FlyType = 0;
        bool DeathSent = false;
        public bool Mining;
        public byte StigLevel = 0;
        public byte dexp = 0;
        public uint dexptime = 0;
        public string QuestFrom = "";
        public string QuestMob = "";
        public uint QuestKO = 0;
        public uint QuestToKill = 0;
        public byte PrevJob = 0;
       
        public uint KO = 0;
        public uint OldKO = 0;
        public System.Timers.Timer TheTimer = new System.Timers.Timer();
       

        #region Reborn
        public void ReBorn(byte ToJob)
        {
            try
            {
                RBCount++;

                if ((Level == 110) || (Level == 111) && (Job == 135))
                {
                    StatP += 0;
                }
                else if ((Level == 112) || (Level == 113) && (Job == 135))
                {
                    StatP += 1;
                }
                else if ((Level == 114) || (Level == 115) && (Job == 135))
                {
                    StatP += 3;
                }
                else if ((Level == 116) || (Level == 117) && (Job == 135))
                {
                    StatP += 6;
                }
                else if ((Level == 118) || (Level == 119) && (Job == 135))
                {
                    StatP += 10;
                }
                else if (Level == 120)
                {
                    if (Job == 135)
                    {
                        StatP += 15;
                    }
                    else
                    {
                        StatP += 0;
                    }
                }
                else if (Level == 121)
                {
                    if (Job == 135)
                    {
                        StatP += 15;
                    }
                    else
                    {
                        StatP += 1;
                    }
                }
                else if (Level == 122)
                {
                    if (Job == 135)
                    {
                        StatP += 21;
                    }
                    else
                    {
                        StatP += 3;
                    }
                }
                else if (Level == 123)
                {
                    if (Job == 135)
                    {
                        StatP += 21;
                    }
                    else
                    {
                        StatP += 6;
                    }
                }
                else if (Level == 124)
                {
                    if (Job == 135)
                    {
                        StatP += 28;
                    }
                    else
                    {
                        StatP += 10;
                    }
                }
                else if (Level == 125)
                {
                    if (Job == 135)
                    {
                        StatP += 28;
                    }
                    else
                    {
                        StatP += 15;
                    }
                }
                else if (Level == 126)
                {
                    if (Job == 135)
                    {
                        StatP += 36;
                    }
                    else
                    {
                        StatP += 21;
                    }
                }
                else if (Level == 127)
                {
                    if (Job == 135)
                    {
                        StatP += 36;
                    }
                    else
                    {
                        StatP += 28;
                    }
                }
                else if (Level == 128)
                {
                    if (Job == 135)
                    {
                        StatP += 45;
                    }
                    else
                    {
                        StatP += 36;
                    }
                }
                else if (Level == 129)
                {
                    if (Job == 135)
                    {
                        StatP += 45;
                    }
                    else
                    {
                        StatP += 45;
                    }
                }
                else if (Level == 130 || Level == 131 || Level == 132 || Level == 133 || Level == 134 || Level == 135)
                {
                    if (Job == 135)
                    {
                        StatP += 55;
                    }
                    else
                    {
                        StatP += 55;
                    }
                }
                MyClient.SendPacket(General.MyPackets.Vital(UID, 11, StatP));

                Level = 15;
                Exp = 0;
                Skills.Clear();
                Skill_Exps.Clear();

                if (Job == 25)//Warrior
                {
                    FirstJob = 25;
                    if (ToJob == 41)//Archer
                    {
                        LearnSkill(1020, 0);//Shield
                        LearnSkill(1040, 0);//Roar
                        LearnSkill(4000, 1);//SummonGuard
                    }
                    if (ToJob == 11)//Trojan
                    {
                        LearnSkill(1015, 0);//Accuracy
                        LearnSkill(1040, 0);//Roar
                        LearnSkill(1320, 0);//FlyingMoon
                        LearnSkill(4000, 1);//SummonGuard
                        LearnSkill(1020, 0);//Shield
                    }
                    if (ToJob == 21)//Warrior
                    {
                        LearnSkill(3060, 1);//Reflect
                        LearnSkill(4000, 1);//SummonGuard
                    }
                    if (ToJob == 142)//FireTaoist
                    {
                        LearnSkill(1020, 0);//Shield
                        LearnSkill(1040, 0);//Roar
                        LearnSkill(4000, 1);//SummonGuard
                        LearnSkill(1020, 0);//Shield
                    }
                    if (ToJob == 132)//WaterTaoist
                    {
                        LearnSkill(1025, 0);//Superman
                        LearnSkill(1020, 0);//Shield
                        LearnSkill(1040, 0);//Roar
                        LearnSkill(4000, 1);//SummonGuard
                        LearnSkill(1020, 0);//Shield
                        LearnSkill(5130, 0);//Icicle
                        LearnSkill(5131, 0);//Avalanch
                        LearnSkill(5132, 0);//IceCircle
                    }
                }
                if (Job == 15)//Trojan
                {
                    FirstJob = 15;
                    if (ToJob == 41)//Archer
                    {
                        LearnSkill(1110, 0);//Cyclone
                        LearnSkill(1190, 1);//SpiritualHealing
                        LearnSkill(4000, 1);//SummonGuard
                    }
                    if (ToJob == 11)//Trojan
                    {
                        LearnSkill(3050, 1);//CruelShade
                        LearnSkill(4000, 1);//SummonGuard
                    }
                    if (ToJob == 21)//Warrior
                    {
                        LearnSkill(1110, 0);//Cyclone
                        LearnSkill(1190, 1);//SpiritualHealing
                        LearnSkill(9999, 9);//IronShirt
                        LearnSkill(4000, 1);//SummonGuard
                        LearnSkill(3060, 1);//Reflect
                    }
                    if (ToJob == 142)//FireTaoist
                    {
                        LearnSkill(1110, 0);//Cyclone
                        LearnSkill(1190, 1);//SpiritualHealing
                        LearnSkill(1270, 1);//Robot
                        LearnSkill(4000, 1);//SummonGuard
                    }
                    if (ToJob == 132)//WaterTaoist
                    {
                        LearnSkill(1110, 0);//Cyclone
                        LearnSkill(1190, 1);//SpiritualHealing
                        LearnSkill(1270, 1);//Robot
                        LearnSkill(4000, 1);//SummonGuard
                        LearnSkill(5130, 0);//Icicle
                        LearnSkill(5131, 0);//Avalanch
                        LearnSkill(5132, 0);//IceCircle
                    }
                }
                if (Job == 45)//Archer
                {
                    FirstJob = 45;
                    if (ToJob == 41)//Archer
                    {
                        LearnSkill(9999, 9);//FreezingArrow
                        LearnSkill(4000, 1);//SummonGuard
                    }
                    if (ToJob == 11)//Trojan
                    {
                        LearnSkill(4000, 1);//SummonGuard
                    }
                    if (ToJob == 21)//Warrior
                    {
                        LearnSkill(4000, 1);//SummonGuard
                        LearnSkill(3060, 1);//Reflect
                    }
                    if (ToJob == 142)//FireTaoist
                    {
                        LearnSkill(4000, 1);//SummonGuard
                    }
                    if (ToJob == 132)//WaterTaoist
                    {
                        LearnSkill(4000, 1);//SummonGuard
                    }
                }
                if (Job == 145)//FireTaoist
                {
                    FirstJob = 145;
                    if (ToJob == 41)//Archer
                    {
                        LearnSkill(1000, 1);//Thunder
                        LearnSkill(1001, 1);//Fire
                        LearnSkill(1005, 1);//Cure
                        LearnSkill(1195, 1);//Meditation
                        LearnSkill(4000, 1);//SummonGuard
                    }
                    if (ToJob == 11)//Trojan
                    {
                        LearnSkill(1000, 1);//Thunder
                        LearnSkill(1001, 1);//Fire
                        LearnSkill(1005, 1);//Cure
                        LearnSkill(1195, 1);//Meditation
                        LearnSkill(4000, 1);//SummonGuard
                    }
                    if (ToJob == 21)//Warrior
                    {
                        LearnSkill(1000, 1);//Thunder
                        LearnSkill(1001, 1);//Fire
                        LearnSkill(1005, 1);//Cure
                        LearnSkill(1195, 1);//Meditation
                        LearnSkill(3060, 1);//Reflect
                        LearnSkill(4000, 1);//SummonGuard
                    }
                    if (ToJob == 142)//FireTaoist
                    {
                        LearnSkill(3080, 1);//Dodge
                        LearnSkill(4000, 1);//SummonGuard
                    }
                    if (ToJob == 132)//WaterTaoist
                    {
                        LearnSkill(1120, 1);//FireCircle
                        LearnSkill(4000, 1);//SummonGuard
                        LearnSkill(5130, 0);//Icicle
                        LearnSkill(5131, 0);//Avalanch
                        LearnSkill(5132, 0);//IceCircle
                    }
                }
                if (Job == 135)//WaterTaoist
                {
                    FirstJob = 135;
                    if (ToJob == 41)//Archer
                    {
                        LearnSkill(1005, 1);//Cure
                        LearnSkill(1095, 1);//Stigma
                        LearnSkill(1000, 1);//Thunder
                        LearnSkill(1075, 1);//Invisibility
                        LearnSkill(1090, 1);//Magic Sheild
                        LearnSkill(1195, 1);//Meditation
                        LearnSkill(4000, 1);//SummonGuard
                    }
                    if (ToJob == 11)//Trojan
                    {
                        LearnSkill(1005, 1);//Cure
                        LearnSkill(1095, 1);//Stigma
                        LearnSkill(1000, 1);//Thunder
                        LearnSkill(1085, 1);//Star of Accuracy
                        LearnSkill(1090, 1);//Magic Sheild
                        LearnSkill(1195, 1);//Meditation
                        LearnSkill(4000, 1);//SummonGuard
                    }
                    if (ToJob == 21)//Warrior
                    {
                        LearnSkill(1005, 1);//Cure
                        LearnSkill(1095, 1);//Stigma
                        LearnSkill(1000, 1);//Thunder
                        LearnSkill(1085, 1);//Star of Accuracy
                        LearnSkill(1090, 1);//Magic Sheild
                        LearnSkill(1195, 1);//Meditation
                        LearnSkill(3060, 1);//Reflect
                        LearnSkill(4000, 1);//SummonGuard
                    }
                    if (ToJob == 142)//FireTaoist
                    {
                        LearnSkill(1175, 1);//Adv. Cure
                        LearnSkill(1075, 1);//Invisibility
                        LearnSkill(1050, 0);//Revive
                        LearnSkill(1055, 1);//HealingRain
                        LearnSkill(4000, 1);//SummonGuard
                    }
                    if (ToJob == 132)//WaterTaoist
                    {
                        LearnSkill(3090, 1);//Pervade
                        LearnSkill(4000, 1);//SummonGuard
                    }
                }
                Potency += 5;

                Job = ToJob;

                InternalDatabase.GetStats(this);
                GetEquipStats(1, true);
                GetEquipStats(2, true);
                GetEquipStats(3, true);
                GetEquipStats(4, true);
                GetEquipStats(5, true);
                GetEquipStats(6, true);
                GetEquipStats(7, true);
                GetEquipStats(8, true);
                MinAtk = Str;
                MaxAtk = Str;
                MaxHP = BaseMaxHP();
                Potency = Level;
                GetEquipStats(1, false);
                GetEquipStats(2, false);
                GetEquipStats(3, false);
                GetEquipStats(4, false);
                GetEquipStats(5, false);
                GetEquipStats(6, false);
                GetEquipStats(7, false);
                GetEquipStats(8, false);
                CurHP = MaxHP;
                if (Job == 11)
                {
                    Str = 5;
                    Agi = 2;
                    Vit = 3;
                    Spi = 0;
                }
                if (Job == 21)
                {
                    Str = 5;
                    Agi = 2;
                    Vit = 3;
                    Spi = 0;
                }
                if (Job == 41)
                {
                    Str = 2;
                    Agi = 7;
                    Vit = 1;
                    Spi = 0;
                }
                if (Job == 132)
                {
                    Str = 0;
                    Agi = 2;
                    Vit = 3;
                    Spi = 5;
                }
                if (Job == 142)
                {
                    Str = 0;
                    Agi = 2;
                    Vit = 3;
                    Spi = 5;
                }
                MyClient.SendPacket(General.MyPackets.Vital((long)UID, 7, Job));
                MyClient.SendPacket(General.MyPackets.Vital((long)UID, 16, Str));
                MyClient.SendPacket(General.MyPackets.Vital((long)UID, 17, Agi));
                MyClient.SendPacket(General.MyPackets.Vital((long)UID, 15, Vit));
                MyClient.SendPacket(General.MyPackets.Vital((long)UID, 14, Spi));
                MyClient.SendPacket(General.MyPackets.GeneralData((long)UID, 0, 0, 0, 92));
                MyClient.SendPacket(General.MyPackets.Vital((long)UID, 0, CurHP));
                StatP += 20;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 11, StatP));

                for (byte i = 1; i < 9; i++)
                {
                    if (Equips[i] == null || Equips[i] == "") continue;
                    string I = Equips[i];
                    string[] II = I.Split('-');
                    uint IID = uint.Parse(II[0]);
                    byte Quality = (byte)Other.ItemQuality(IID);

                    if (i == 1)
                    {
                        string NewID = "";

                        if (Other.WeaponType(IID) == 111 || Other.WeaponType(IID) == 113 || Other.WeaponType(IID) == 114 || Other.WeaponType(IID) == 118 || Other.WeaponType(IID) == 117)
                        {
                            NewID = II[0].Remove(4, 2);
                            NewID = NewID + "0" + Quality.ToString();

                            Equips[i] = NewID + "-" + II[1] + "-" + II[2] + "-" + II[3] + "-" + II[4] + "-" + II[5];
                            II[0] = NewID;
                            MyClient.SendPacket(General.MyPackets.AddItem(Equips_UIDs[i], int.Parse(II[0]), byte.Parse(II[1]), byte.Parse(II[2]), byte.Parse(II[3]), byte.Parse(II[4]), byte.Parse(II[5]), i, 100, 100));
                        }
                        else if (Other.WeaponType(IID) == 112)
                        {
                            byte Type = byte.Parse(II[0][4].ToString());
                            byte Color = byte.Parse(II[0][3].ToString());
                            NewID = "11" + Type.ToString() + Color.ToString() + "0" + Quality.ToString();
                            Equips[i] = NewID + "-" + II[1] + "-" + II[2] + "-" + II[3] + "-" + II[4] + "-" + II[5];
                            II[0] = NewID;
                            MyClient.SendPacket(General.MyPackets.AddItem(Equips_UIDs[i], int.Parse(II[0]), byte.Parse(II[1]), byte.Parse(II[2]), byte.Parse(II[3]), byte.Parse(II[4]), byte.Parse(II[5]), i, 100, 100));
                        }
                    }
                    else if (i == 2)
                    {
                        string NewID = "";

                        NewID = II[0].Remove(3, 3);
                        NewID += "00" + Quality.ToString();
                        Equips[i] = NewID + "-" + II[1] + "-" + II[2] + "-" + II[3] + "-" + II[4] + "-" + II[5];
                        II[0] = NewID;
                        MyClient.SendPacket(General.MyPackets.AddItem(Equips_UIDs[i], int.Parse(II[0]), byte.Parse(II[1]), byte.Parse(II[2]), byte.Parse(II[3]), byte.Parse(II[4]), byte.Parse(II[5]), i, 100, 100));
                    }
                    else if (i == 3)
                    {
                        string NewID = "";
                        if (Other.WeaponType(IID) == 130 || Other.WeaponType(IID) == 131 || Other.WeaponType(IID) == 133 || Other.WeaponType(IID) == 134)
                        {
                            NewID = II[0].Remove(4, 2);
                            NewID = NewID + "0" + Quality.ToString();

                            Equips[i] = NewID + "-" + II[1] + "-" + II[2] + "-" + II[3] + "-" + II[4] + "-" + II[5];
                            II[0] = NewID;
                            MyClient.SendPacket(General.MyPackets.AddItem(Equips_UIDs[i], int.Parse(II[0]), byte.Parse(II[1]), byte.Parse(II[2]), byte.Parse(II[3]), byte.Parse(II[4]), byte.Parse(II[5]), i, 100, 100));
                        }
                        else if (Other.WeaponType(IID) == 135 || Other.WeaponType(IID) == 136 || Other.WeaponType(IID) == 138 || Other.WeaponType(IID) == 139)
                        {
                            byte Type = byte.Parse(II[0][2].ToString());
                            byte Color = byte.Parse(II[0][3].ToString());
                            Type -= 5;
                            NewID = "13" + Type.ToString() + Color.ToString() + "0" + Quality.ToString();
                            Equips[i] = NewID + "-" + II[1] + "-" + II[2] + "-" + II[3] + "-" + II[4] + "-" + II[5];
                            II[0] = NewID;
                            MyClient.SendPacket(General.MyPackets.AddItem(Equips_UIDs[i], int.Parse(II[0]), byte.Parse(II[1]), byte.Parse(II[2]), byte.Parse(II[3]), byte.Parse(II[4]), byte.Parse(II[5]), i, 100, 100));
                        }
                    }
                    else if (i == 4)
                    {
                        string NewID = "";

                        NewID = II[0].Remove(3, 3);
                        NewID += "02" + Quality.ToString();
                        Equips[i] = NewID + "-" + II[1] + "-" + II[2] + "-" + II[3] + "-" + II[4] + "-" + II[5];
                        II[0] = NewID;
                        MyClient.SendPacket(General.MyPackets.AddItem(Equips_UIDs[i], int.Parse(II[0]), byte.Parse(II[1]), byte.Parse(II[2]), byte.Parse(II[3]), byte.Parse(II[4]), byte.Parse(II[5]), i, 100, 100));
                    }
                    else if (i == 5)
                    {
                        string NewID = "";

                        if (Other.WeaponType(IID) == 900)
                        {
                            NewID = II[0].Remove(4, 2);
                            NewID += "0" + Quality.ToString();
                            Equips[i] = NewID + "-" + II[1] + "-" + II[2] + "-" + II[3] + "-" + II[4] + "-" + II[5];
                            II[0] = NewID;
                            MyClient.SendPacket(General.MyPackets.AddItem(Equips_UIDs[i], int.Parse(II[0]), byte.Parse(II[1]), byte.Parse(II[2]), byte.Parse(II[3]), byte.Parse(II[4]), byte.Parse(II[5]), i, 100, 100));
                        }
                        else if (Other.ItemType(IID) == 4 || Other.ItemType(IID) == 5)
                        {
                            NewID = II[0].Remove(3, 3);
                            NewID += "02" + Quality.ToString();
                            Equips[i] = NewID + "-" + II[1] + "-" + II[2] + "-" + II[3] + "-" + II[4] + "-" + II[5];
                            II[0] = NewID;
                            MyClient.SendPacket(General.MyPackets.AddItem(Equips_UIDs[i], int.Parse(II[0]), byte.Parse(II[1]), byte.Parse(II[2]), byte.Parse(II[3]), byte.Parse(II[4]), byte.Parse(II[5]), i, 100, 100));
                        }
                    }
                    else if (i == 6)
                    {
                        string NewID = "";

                        NewID = II[0].Remove(3, 3);
                        NewID += "01" + Quality.ToString();
                        Equips[i] = NewID + "-" + II[1] + "-" + II[2] + "-" + II[3] + "-" + II[4] + "-" + II[5];
                        II[0] = NewID;
                        MyClient.SendPacket(General.MyPackets.AddItem(Equips_UIDs[i], int.Parse(II[0]), byte.Parse(II[1]), byte.Parse(II[2]), byte.Parse(II[3]), byte.Parse(II[4]), byte.Parse(II[5]), i, 100, 100));
                    }
                    else if (i == 8)
                    {
                        string NewID = "";

                        NewID = II[0].Remove(3, 3);
                        NewID += "01" + Quality.ToString();
                        Equips[i] = NewID + "-" + II[1] + "-" + II[2] + "-" + II[3] + "-" + II[4] + "-" + II[5];
                        II[0] = NewID;
                        MyClient.SendPacket(General.MyPackets.AddItem(Equips_UIDs[i], int.Parse(II[0]), byte.Parse(II[1]), byte.Parse(II[2]), byte.Parse(II[3]), byte.Parse(II[4]), byte.Parse(II[5]), i, 100, 100));
                    }

                }

                MyClient.SendPacket(General.MyPackets.Vital(UID, 13, Level));

                MyClient.SendPacket(General.MyPackets.String(UID, 10, "hitstar"));
                
                World.SendMsgToAll(Name + " completed First Rebirth!", "SYSTEM", 2011);
                World.SendMsgToAll(Name + " completed First Rebirth!", "SYSTEM", 2005);
                if (RBCount == 2)
                {
                    World.SendMsgToAll(Name + " Completed Second Rebirth!", "SYSTEM", 2011);
                    World.SendMsgToAll(Name + " completed Second Rebirth!", "SYSTEM", 2005);
                }
                World.UpdateSpawn(this);
            }
            catch (Exception Exc) { Console.WriteLine(Exc); }
            World.UpdateSpawn(this);
            MyClient.Drop();
        }
        #endregion
        #region mining
        public void SwingPickAxe()
        {
            if (Mining)
            {
                string[] Splitter = Equips[4].Split('-');
                if (LocMap == 1028 && Splitter[0] == "562000" || Splitter[0] == "562001")
                {
                    LastSwing = DateTime.Now;
                    MyClient.SendPacket(General.MyPackets.GeneralData(UID, 99, LocX, LocY, 99));
                    World.PlayerMoves(this, General.MyPackets.GeneralData(UID, 99, LocX, LocY, 99));

                    if (ItemsInInventory < 40)
                        if (Other.ChanceSuccess(25))
                        {
                            uint ItemId = (uint)(1072010 + General.Rand.Next(9));
                            if (Other.ChanceSuccess(35))
                                ItemId = (uint)(1072040 + General.Rand.Next(9));
                            if (Other.ChanceSuccess(20))
                                ItemId = (uint)(1072050 + General.Rand.Next(9));
                            if (Other.ChanceSuccess(10))
                                ItemId = 1072031;
                            if (Other.ChanceSuccess(2))
                            {
                                ItemId = (uint)(700001 + General.Rand.Next(7) * 10);
                                if (Other.ChanceSuccess(40))
                                    ItemId++;
                                if (Other.ChanceSuccess(15))
                                    ItemId++;
                            }
                            AddItem(ItemId.ToString() + "-0-0-0-0-0", 0, (uint)General.Rand.Next(23453246));
                        }
                }
                else
                    Mining = false;
            }
        }
        #endregion

        public void GemEffect()
        {
            int into = 0;
            if (Equips[1] != null && Equips[1] != "0")
                into = 1;
            else if (Equips[2] != null && Equips[2] != "0")
                into = 2;
            else if (Equips[3] != null && Equips[3] != "0")
                into = 3;
            else if (Equips[6] != null && Equips[6] != "0")
                into = 6;
            else if (Equips[8] != null && Equips[8] != "0")
                into = 8;
            else
                return;
            string[] item = Equips[into].Split('-');

            if (item[4] == "13")
            {
                if (Other.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in World.AllChars)
                    {
                        Character Chaar = (Character)DE.Value;
                        if (Chaar.Name != Name)
                        {
                            Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "goldendragon"));
                        }
                    }
                    MyClient.SendPacket(General.MyPackets.String(UID, 10, "goldendragon"));
                }
            }
            if (item[5] == "13")
            {
                if (Other.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in World.AllChars)
                    {
                        Character Chaar = (Character)DE.Value;
                        if (Chaar.Name != Name)
                        {
                            Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "goldendragon"));
                        }
                    }
                    MyClient.SendPacket(General.MyPackets.String(UID, 10, "phoegoldendragonnix"));
                }
            }
            if (item[4] == "3")
            {
                if (Other.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in World.AllChars)
                    {
                        Character Chaar = (Character)DE.Value;
                        if (Chaar.Name != Name)
                        {
                            Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "phoenix"));
                        }
                    }
                    MyClient.SendPacket(General.MyPackets.String(UID, 10, "phoenix"));
                }
            }
            if (item[5] == "3")
            {
                if (Other.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in World.AllChars)
                    {
                        Character Chaar = (Character)DE.Value;
                        if (Chaar.Name != Name)
                        {
                            Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "phoenix"));
                        }
                    }
                    MyClient.SendPacket(General.MyPackets.String(UID, 10, "phoenix"));
                }
            }
            if (item[4] == "33")
            {
                if (Other.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in World.AllChars)
                    {
                        Character Chaar = (Character)DE.Value;
                        if (Chaar.Name != Name)
                        {
                            Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "rainbow"));
                        }
                    }
                    MyClient.SendPacket(General.MyPackets.String(UID, 10, "rainbow"));
                }
            }
            if (item[5] == "33")
            {
                if (Other.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in World.AllChars)
                    {
                        Character Chaar = (Character)DE.Value;
                        if (Chaar.Name != Name)
                        {
                            Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "rainbow"));

                        }
                    }
                    MyClient.SendPacket(General.MyPackets.String(UID, 10, "rainbow"));
                }
            }
            if (item[4] == "53")
            {
                if (Other.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in World.AllChars)
                    {
                        Character Chaar = (Character)DE.Value;
                        if (Chaar.Name != Name)
                        {
                            Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "fastflash"));
                        }
                    }
                    MyClient.SendPacket(General.MyPackets.String(UID, 10, "fastflash"));
                }
            }
            if (item[5] == "53")
            {
                if (Other.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in World.AllChars)
                    {
                        Character Chaar = (Character)DE.Value;
                        if (Chaar.Name != Name)
                        {
                            Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "fastflash"));
                        }
                    }
                    MyClient.SendPacket(General.MyPackets.String(UID, 10, "fastflash"));
                }
            }
            if (item[4] == "63")
            {
                if (Other.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in World.AllChars)
                    {
                        Character Chaar = (Character)DE.Value;
                        if (Chaar.Name != Name)
                        {
                            Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "moon"));
                        }
                    }
                    MyClient.SendPacket(General.MyPackets.String(UID, 10, "moon"));
                }
            }
            if (item[5] == "63")
            {
                if (Other.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in World.AllChars)
                    {
                        Character Chaar = (Character)DE.Value;
                        if (Chaar.Name != Name)
                        {
                            Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "moon"));
                        }
                    }
                    MyClient.SendPacket(General.MyPackets.String(UID, 10, "moon"));
                }
            }
            if (item[4] == "43")
            {
                if (Other.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in World.AllChars)
                    {
                        Character Chaar = (Character)DE.Value;
                        if (Chaar.Name != Name)
                        {
                            Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "goldenkylin"));
                        }
                    }
                    MyClient.SendPacket(General.MyPackets.String(UID, 10, "goldenkylin"));
                }
            }
            if (item[5] == "43")
            {
                if (Other.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in World.AllChars)
                    {
                        Character Chaar = (Character)DE.Value;
                        if (Chaar.Name != Name)
                        {
                            Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "goldenkylin"));
                        }
                    }
                    MyClient.SendPacket(General.MyPackets.String(UID, 10, "goldenkylin"));
                }
            }
            if (item[5] == "53")
            {
                if (Other.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in World.AllChars)
                    {
                        Character Chaar = (Character)DE.Value;
                        if (Chaar.Name != Name)
                        {
                            Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "purpleray"));
                        }
                    }
                    MyClient.SendPacket(General.MyPackets.String(UID, 10, "purpleray"));
                }
            }

            if (item[4] == "53")
            {
                if (Other.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in World.AllChars)
                    {
                        Character Chaar = (Character)DE.Value;
                        if (Chaar.Name != Name)
                        {
                            Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "purpleray"));
                        }
                    }
                    MyClient.SendPacket(General.MyPackets.String(UID, 10, "purpleray"));
                }
            }
              if (item[4] == "73")
            {
                if (Other.ChanceSuccess(10))
                {
                    foreach (DictionaryEntry DE in World.AllChars)
                    {
                        Character Chaar = (Character)DE.Value;
                        if (Chaar.Name != Name)
                        {
                            Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "recovery"));
                        }
                    }
                    MyClient.SendPacket(General.MyPackets.String(UID, 10, "recovery"));
                }
            }
              if (item[4] == "73")
              {
                  if (Other.ChanceSuccess(10))
                  {
                      foreach (DictionaryEntry DE in World.AllChars)
                      {
                          Character Chaar = (Character)DE.Value;
                          if (Chaar.Name != Name)
                          {
                              Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "recovery"));
                          }
                      }
                      MyClient.SendPacket(General.MyPackets.String(UID, 10, "recovery"));
                  }
              }
        }

        #region skilltimers
        void TimerElapsed(object source, ElapsedEventArgs e)
        {
            if (EPotRate == true)
            {
                EPotXP2 -= 1;
            }
            if (EPotXP == 8)
            {
                if (EPotRate)
                {
                    MyClient.CurrentNPC = 63421;
                    MyClient.SendPacket(General.MyPackets.NPCSay("Your exppot has ended,and you now have normal Exp."));
                    MyClient.SendPacket(General.MyPackets.NPCSay("Would you like to use 27 CPs to active a new ExpPot?"));
                    MyClient.SendPacket(General.MyPackets.NPCLink("Yeah,why not", 1));
                    MyClient.SendPacket(General.MyPackets.NPCLink("No, thank you", 255));
                    MyClient.SendPacket(General.MyPackets.NPCSetFace(30));
                    MyClient.SendPacket(General.MyPackets.NPCFinish());
                }
            }
            //--This code stops exppot time--
            if (EPotXP2 <= 0)
            {
                if (EPotRate)
                {
                    EPotRate = false;
                    EPotXP = 0;
                    EPotXP2 = 0;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 19, EPotXP));
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                    World.UpdateSpawn(this);
                }
            }
            if (CastingPray)//Caster
                if (LuckTime < (2 * 60 * 60 * 1000))
                {
                    LuckTime += 1500;
                    if (LuckTime >= (2 * 60 * 60 * 1000))
                        LuckTime = (2 * 60 * 60 * 1000);
                }
            if (Praying)//Others
                if (LuckTime < (2 * 60 * 60 * 1000))
                {
                    LuckTime += 500;
                    if (LuckTime >= (2 * 60 * 60 * 1000))
                        LuckTime = (2 * 60 * 60 * 1000);
                }
            //------------------
            //--Lose LuckyTime--
            if (!Praying && !CastingPray)
                if (LuckTime > 0)
                {
                    LuckTime -= 500;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 29, LuckTime));
                }
            //------------------
            //--Caster Stops Luckytime--
            if (CastingPray)
            {
                if (LocX != PrayX || LocY != PrayY)
                {
                    CastingPray = false;
                    Praying = false;

                    MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                    World.UpdateSpawn(this);
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 29, LuckTime));
                    PrayX = 0;
                    PrayY = 0;
                    //World.PlayersPraying.Remove(this);
                }
                else if (!Alive)
                {
                    CastingPray = false;
                    Praying = false;

                    MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                    World.UpdateSpawn(this);
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 29, LuckTime));
                    PrayX = 0;
                    PrayY = 0;
                }
            }
            //--------------------------
            //--Others Start Praying--
            foreach (Character Caster in World.PlayersPraying)
            {
                if (LocMap == Caster.LocMap)
                    if (this != Caster)
                        if (Caster.CastingPray)
                            if ((MyMath.PointDistance(LocX, LocY, Caster.LocX, Caster.LocY) < 4) || (LocX == Caster.LocX && LocY == Caster.LocY))
                                if (!Praying && !CastingPray)
                                {
                                    Thread.Sleep(TimeSpan.FromSeconds(3));
                                    {
                                        if (!Mining)
                                        {
                                            Praying = true;
                                            MyClient.SendPacket(General.MyPackets.Vital(UID, 29, LuckTime));
                                            MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                                            World.UpdateSpawn(this);
                                        }
                                    }
                                }
            }
            //------------------------
            //--Others Stop Praying--
            foreach (Character Caster in World.PlayersPraying)
            {
                //if (LocMap == Caster.LocMap)
                if (this != Caster)
                    if (MyMath.PointDistance(LocX, LocY, Caster.LocX, Caster.LocY) > 3)
                    {
                        if (Praying)
                        {
                            Praying = false;
                            MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                            World.UpdateSpawn(this);
                            MyClient.SendPacket(General.MyPackets.Vital(UID, 29, LuckTime));
                        }
                    }
                    else if (!Caster.CastingPray)
                    {
                        if (Praying)
                        {
                            Praying = false;
                            MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                            World.UpdateSpawn(this);
                            MyClient.SendPacket(General.MyPackets.Vital(UID, 29, LuckTime));
                        }
                    }
            }
            //------------------------
            if (StigBuff)
                if (DateTime.Now > Stigged.AddSeconds(20 + StigLevel * 5))
                {
                    StigBuff = false;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                    World.UpdateSpawn(this);
                }
            if (Attacking)
                GemEffect();
            Attack();
            if (LocMap == 1038)
                if (DateTime.Now > LastGWList.AddMilliseconds(5000))
                {
                    LastGWList = DateTime.Now;
                    SendGuildWar();
                }
            if (Action == 250)
                if (Stamina < 100)
                {
                    Stamina += 8;
                    if (Stamina > 100)
                        Stamina = 100;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 9, Stamina));
                }
            if (Action == 100)
                if (Stamina < 100)
                {
                    Stamina += 1 / 2;
                    if (Stamina > 100)
                        Stamina = 100;
                    else if (Stamina > 100)
                        Stamina = 150;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 9, Stamina));
                }
            if (Action == 230)
            {
                if (Equips[3] != null)
                {
                    FullSuper();
                    string TheEquip = Equips[3];
                    string[] Splitter = TheEquip.Split('-');
                    uint ItemId = uint.Parse(Splitter[0]);
                    if (Other.ItemQuality(ItemId) == 9 && (Equips[1] != null || Equips[4] == null) && (Equips[2] != null || Equips[2] == null) && (Equips[4] != null || Equips[4] == null) && (Equips[8] != null || Equips[8] == null) && (Equips[6] != null || Equips[6] == null))
                    {
                        if (Job <= 16 && Job >= 9)
                        {
                            foreach (DictionaryEntry DE in World.AllChars)
                            {
                                Character Chaar = (Character)DE.Value;
                                if (Chaar.Name != Name)
                                {
                                    Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "warrior-s"));
                                }
                            }
                            MyClient.SendPacket(General.MyPackets.String(UID, 10, "warrior-s"));
                            Action = 100;
                        }
                        if (Job <= 26 && Job >= 19)
                        {
                            foreach (DictionaryEntry DE in World.AllChars)
                            {
                                Character Chaar = (Character)DE.Value;
                                if (Chaar.Name != Name)
                                {
                                    Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "fighter-s"));
                                }
                            }
                            MyClient.SendPacket(General.MyPackets.String(UID, 10, "fighter-s"));
                            Action = 100;

                        }
                        if (Job <= 46 && Job >= 39)
                        {
                            foreach (DictionaryEntry DE in World.AllChars)
                            {
                                Character Chaar = (Character)DE.Value;
                                if (Chaar.Name != Name)
                                {
                                    Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "archer-s"));
                                }
                            }
                            MyClient.SendPacket(General.MyPackets.String(UID, 10, "archer-s"));
                            Action = 100;
                        }
                        if (Job <= 146 && Job >= 100)
                        {
                            foreach (DictionaryEntry DE in World.AllChars)
                            {
                                Character Chaar = (Character)DE.Value;
                                if (Chaar.Name != Name)
                                {
                                    Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "taoist-s"));
                                }
                            }
                            MyClient.SendPacket(General.MyPackets.String(UID, 10, "taoist-s"));
                            Action = 100;
                        }
                    }
                    if (AllSuper == true && Other.ItemQuality(ItemId) == 9 && Equips[1] != null && Equips[2] != null && Equips[3] != null && Equips[4] != null && Equips[8] != null && Equips[6] != null)
                    {
                        if (Job <= 16 && Job >= 9)
                        {
                            foreach (DictionaryEntry DE in World.AllChars)
                            {
                                Character Chaar = (Character)DE.Value;
                                if (Chaar.Name != Name)
                                {
                                    Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "warrior"));
                                }
                            }
                            MyClient.SendPacket(General.MyPackets.String(UID, 10, "warrior"));
                            Action = 100;
                        }
                        if (Job <= 26 && Job >= 19)
                        {
                            foreach (DictionaryEntry DE in World.AllChars)
                            {
                                Character Chaar = (Character)DE.Value;
                                if (Chaar.Name != Name)
                                {
                                    Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "fighter"));
                                }
                            }
                            MyClient.SendPacket(General.MyPackets.String(UID, 10, "fighter"));
                            Action = 100;

                        }
                        if (Job <= 46 && Job >= 39)
                        {
                            foreach (DictionaryEntry DE in World.AllChars)
                            {
                                Character Chaar = (Character)DE.Value;
                                if (Chaar.Name != Name)
                                {
                                    Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "archer"));
                                }
                            }
                            MyClient.SendPacket(General.MyPackets.String(UID, 10, "archer"));
                            Action = 100;
                        }
                        if (Job <= 146 && Job >= 100)
                        {
                            foreach (DictionaryEntry DE in World.AllChars)
                            {
                                Character Chaar = (Character)DE.Value;
                                if (Chaar.Name != Name)
                                {
                                    Chaar.MyClient.SendPacket(General.MyPackets.String(UID, 10, "taoist"));
                                }
                            }
                            MyClient.SendPacket(General.MyPackets.String(UID, 10, "taoist"));
                            Action = 100;
                        }
                    }
                }
            }
            if (Stamina < 100)
                {
                    Stamina += 8;
                    if (Stamina > 100)
                        Stamina = 100;
                    else if (Stamina > 150)
                        Stamina = 150;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 9, Stamina));
                }
            if (AccuracyOn)
                if (DateTime.Now > AccuracyActivated.AddSeconds(200))
                    AccuracyOn = false;

            if (Flying)
                if (DateTime.Now > FlyActivated.AddSeconds(40 + FlyType * 20))
                {
                    Flying = false;
                    FlyType = 0;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                    World.UpdateSpawn(this);
                }

            if (DateTime.Now > LastSwing.AddMilliseconds(2500))
                SwingPickAxe();

            if (!CycloneOn)
            {
                if (AtkType == 2 || AtkType == 21)
                {
                    if (DateTime.Now > LastAttack.AddMilliseconds(1000))
                    {
                        if (Attacking)
                            Attack();
                    }
                }
                else if (Attacking)
                    Attack();
            }
            else if (PTarget != null && !PTarget.Flying || AtkType != 2 || PTarget == null)
                Attack();


            if (DateTime.Now > LastXPC.AddMilliseconds(3000))
                AddXPC();

            if (!Alive)
                if (!DeathSent)
                    if (DateTime.Now > Death.AddMilliseconds(1500))
                        Die();

            if (DateTime.Now > LastSave.AddMilliseconds(15000))
                Save();

            if (DateTime.Now > XPActivated.AddMilliseconds(ExtraXP))
                if (SMOn || CycloneOn)
                    XPEnd();

            if (DateTime.Now > GotBlueName.AddMilliseconds(35000))
                if (BlueName)
                    BlueNameGone();

            if (DateTime.Now > LostPKP.AddMilliseconds(180000))
                if (PKPoints > 0)
                    PKTimer_Elapsed();
        }

        private void FullSuper()
        {
            if (Equips[1] != null && Equips[2] != null && Equips[3] != null && Equips[4] != null && Equips[8] != null && Equips[6] != null)
            {
                string TheEquip1 = Equips[1];
                string TheEquip2 = Equips[2];
                string TheEquip3 = Equips[3];
                string TheEquip4 = Equips[4];
                string TheEquip8 = Equips[8];
                string TheEquip6 = Equips[6];

                string[] Splitter1 = TheEquip1.Split('-');
                uint ItemId1 = uint.Parse(Splitter1[0]);

                string[] Splitter2 = TheEquip2.Split('-');
                uint ItemId2 = uint.Parse(Splitter2[0]);

                string[] Splitter3 = TheEquip3.Split('-');
                uint ItemId3 = uint.Parse(Splitter3[0]);

                string[] Splitter4 = TheEquip4.Split('-');
                uint ItemId4 = uint.Parse(Splitter4[0]);

                string[] Splitter8 = TheEquip8.Split('-');
                uint ItemId8 = uint.Parse(Splitter8[0]);

                string[] Splitter6 = TheEquip6.Split('-');
                uint ItemId6 = uint.Parse(Splitter6[0]);

                if (Other.ItemQuality(ItemId1) == 9 && Other.ItemQuality(ItemId2) == 9 && Other.ItemQuality(ItemId3) == 9 && Other.ItemQuality(ItemId4) == 9 && Other.ItemQuality(ItemId8) == 9 && Other.ItemQuality(ItemId6) == 9)
                {
                    AllSuper = true;

                }
            }
        }
        public Character()
        {
            TheTimer.Interval = 500;
            TheTimer.Elapsed += new ElapsedEventHandler(TimerElapsed);
            TheTimer.Start();
            LastAttack = DateTime.Now;
            LastXPC = DateTime.Now;
            Death = DateTime.Now;
            LastSave = DateTime.Now;
            LostPKP = DateTime.Now;


            WHIDs[0] = new uint[21];
            WHIDs[1] = new uint[21];
            WHIDs[2] = new uint[21];
            WHIDs[3] = new uint[21];
            WHIDs[4] = new uint[21];
            WHIDs[5] = new uint[41];
        }
        public void PKTimer_Elapsed()
        {
            LostPKP = DateTime.Now;
            if (PKPoints > 0)
            {
                PKPoints -= 1;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 6, PKPoints));

                if ((PKPoints < 30 && PKPoints + 1 > 29) || (PKPoints < 100 && PKPoints + 1 > 99))
                {
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                    World.UpdateSpawn(this);
                }
            }
        }
        #endregion
        public void AddFriend(Character Who)
        {
            Friends.Add(Who.UID, Who.Name);
            MyClient.SendPacket(General.MyPackets.FriendEnemyPacket(Who.UID, Who.Name, 15, 1));
        }

        #region teamcontrol
        public bool TeamAdd(Character Player)
        {
            if (PlayersInTeam < 4)
            {
                Player.MyTeamLeader = this;
                Player.MyClient.SendPacket(General.MyPackets.PlayerJoinsTeam(this));
                for (int i = 0; i < Team.Count; i++)
                {
                    Character Member = (Character)Team[i];
                    if (Member != null)
                    {
                        Member.MyClient.SendPacket(General.MyPackets.PlayerJoinsTeam(Player));
                        Member.Team.Add(Player);

                        Player.MyClient.SendPacket(General.MyPackets.PlayerJoinsTeam(Member));
                    }
                }
                Team.Add(Player);
                Player.Team = Team;
                MyClient.SendPacket(General.MyPackets.PlayerJoinsTeam(Player));
                Player.MyClient.SendPacket(General.MyPackets.PlayerJoinsTeam(Player));
                PlayersInTeam++;
                return true;
            }
            else
                return false;
        }

        public void TeamRemove(Character Player, bool Kick)
        {
            Player.MyTeamLeader = null;
            if (Kick)
                Player.MyClient.SendPacket(General.MyPackets.TeamPacket(Player.UID, 7));
            else
                Player.MyClient.SendPacket(General.MyPackets.TeamPacket(Player.UID, 2));

            if (Kick)
                MyClient.SendPacket(General.MyPackets.TeamPacket(Player.UID, 7));
            else
                MyClient.SendPacket(General.MyPackets.TeamPacket(Player.UID, 2));

            Team.Remove(Player);

            for (int i = 0; i < Team.Count; i++)
            {
                Character Member = (Character)Team[i];
                if (Member != null)
                {
                    if (Kick)
                        Member.MyClient.SendPacket(General.MyPackets.TeamPacket(Player.UID, 7));
                    else
                        Member.MyClient.SendPacket(General.MyPackets.TeamPacket(Player.UID, 2));

                    Member.Team.Remove(Player);

                    Player.MyClient.SendPacket(General.MyPackets.TeamPacket(Member.UID, 2));
                }
            }
            Player.Team = new ArrayList(4);
            PlayersInTeam--;
        }

        public void TeamDismiss()
        {
            TeamLeader = false;
            MyTeamLeader = null;
            PlayersInTeam = 0;

            MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
            World.UpdateSpawn(this);

            for (int i = 0; i < Team.Count; i++)
            {
                Character Member = (Character)Team[i];
                Member.MyTeamLeader = null;
                Member.Team = new ArrayList(4);
                Member.MyClient.SendPacket(General.MyPackets.TeamPacket(UID, 6));
                Member.MyClient.SendPacket(General.MyPackets.TeamPacket(Member.UID, 6));
            }
            MyClient.SendPacket(General.MyPackets.TeamPacket(UID, 6));
        }
        #endregion


        public string FindItem(uint ItemUID)
        {
            int Count = 0;
            foreach (uint item in Inventory_UIDs)
            {
                if (item == ItemUID)
                    return Inventory[Count];


                Count++;
            }
            return null;
        }

        public string FindWHItem(uint ItemUID, byte WH)
        {
            int Count = 0;
            foreach (uint item in WHIDs[WH])
            {
                if (item == ItemUID)
                {
                    if (WH == 0)
                        return TCWH[Count];
                    else if (WH == 1)
                        return PCWH[Count];
                    else if (WH == 2)
                        return ACWH[Count];
                    else if (WH == 3)
                        return DCWH[Count];
                    else if (WH == 4)
                        return BIWH[Count];
                    else if (WH == 5)
                        return MAWH[Count];
                    else
                        return null;
                }
                Count++;
            }
            return null;
        }

        public void StartXPCircle()
        {

        }

        public void AddXPC()
        {
            LastXPC = DateTime.Now;
            XpCircle += 1;

            if (XpCircle >= 100)
            {
                XpList = true;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                XpCircle = 0;
            }
        }
        public ulong GetStat()
        {
            ulong it = 0;

            if (PKPoints >= 30 && PKPoints < 100)
                it += 16384;
            if (PKPoints >= 100)
                it += 32768;
            if (BlueName)
                it++;
            if (TeamLeader)
                it += 64;
            if (Poisoned)
                it += 2;
            if (SMOn)
                it += 262144;
            if (Invisible)
                it += 4194304;
            if (AccuracyBuff)
                it += 128;
            if (ShieldBuff)
                it += 256;
            if (StigBuff)
                it += 512;
            if (CycloneOn)
                it += 8388608;
            if (XpList)
                it += 16;
            if (CastingPray)
                it += 0x40000000;
            if (Praying)
                it += 0x80000000;
            if (!Alive)
                it += 1024;
            if (Flying)
                it += 134217728;

            return it;
        }

        #region revive
        public void Revive(bool Tele)
        {
            if (Alive)
                return;

            if (Tele)
            {
                foreach (ushort[] revp in ExternalDatabase.RevPoints)
                {
                    if (PKPoints > 99)
                    {
                        Teleport(6000, 60, 60);
                    } 
                    if (revp[0] == LocMap)
                    {
                        Stamina = 100; 
                        Teleport(revp[1], revp[2], revp[3]);
                        General.Blackname();
                        Stamina = 100;
                        break;
                    }
                }
            }

            CurHP = MaxHP;
            Alive = true;

            MyClient.SendPacket(General.MyPackets.Status1(UID, 0));
            MyClient.SendPacket(General.MyPackets.Status3(UID));
            MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));

            MyClient.SendPacket(General.MyPackets.CharacterInfo(this));
            SendEquips(false);
            BlueName = false;

            MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));

            Stamina = 100;
            MyClient.SendPacket(General.MyPackets.Vital(UID, 9, Stamina));
            World.UpdateSpawn(this);
            Stamina = 100;
            MyClient.SendPacket(General.MyPackets.Vital(UID, 9, Stamina));
            DeathSent = false;

        }
        #endregion

        #region die
        public void Die()
        {
            try
            {
                DeathSent = true;
                World.UpdateSpawn(this);

                int EModel;
                if (Model == 1003 || Model == 1004)
                    EModel = 15099;
                else
                    EModel = 15199;

                XpList = false;
                SMOn = false;
                CycloneOn = false;
                XpCircle = 0;

                MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                MyClient.SendPacket(General.MyPackets.Status1(UID, EModel));
                MyClient.SendPacket(General.MyPackets.Death(this));
                General.Blackname();

                if (PKPoints >= 30)
                {
                    if (Other.ChanceSuccess(30))
                    {
                        Random lol = new Random();
                        int x = lol.Next(0, 9);
                        if (Equips[x] == null)
                            return;
                        uint TheItemUID = Equips_UIDs[x];
                        UnEquip((byte)x);
                        int Count = 0;
                        foreach (uint uid in Inventory_UIDs)
                        {
                            if (uid == TheItemUID)
                            {
                                string Item = Inventory[Count];
                                DroppedItem e = DroppedItems.DropItem(Item, (uint)(LocX - General.Rand.Next(2) + General.Rand.Next(2)), (uint)(LocY - General.Rand.Next(2) + General.Rand.Next(2)), (uint)LocMap, 0);
                                World.ItemDrops(e);

                                RemoveItem(TheItemUID);
                            }
                            Count++;
                        }
                    }
                }
            }
            catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); }
        }
        
        public bool GetHitDie(uint Damage)
        {
            if (Action == 250)
            {
                if (Stamina >= 35)
                    Stamina -= 25;
                else
                    Stamina = 0;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 9, Stamina));
            }
            Action = 100;
            if (Damage >= CurHP)
            {
                CurHP = 0;
                Death = DateTime.Now;
                Alive = false;

                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));

                return true;
            }
            else
            {
                CurHP -= (ushort)Damage;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));
                return false;
            }
        }
        #endregion
        #region xpend
        public void XPEnd()
        {
            CycloneOn = false;
            SMOn = false;
            AccuracyOn = false;
            MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
            World.UpdateSpawn(this);
        }
        #endregion
        #region manamax 
        public ushort MaxMana()
        {
            ushort mana = (ushort)(Spi * 15);
            if (Job == 133 || Job == 143)
                mana = (ushort)((double)mana * 1.33333333333333333333333);
            if (Job == 134 || Job == 144)
                mana = (ushort)((double)mana * 1.66666666666666666666666);
            if (Job == 135 || Job == 145)
                mana *= 2;

            return mana;
        }
        #endregion
        #region skillattrib        
        public void UseSkill(ushort SkillId, ushort X, ushort Y, uint TargID)
        {
            if (!Alive)
                return;
            Ready = false;
            if (Skills.Contains((short)SkillId))
                if (ExternalDatabase.SkillsDone.Contains((int)SkillId))
                {
                    Action = 100;
                    byte SkillLvl = (byte)Skills[(short)SkillId];
                    Hashtable MobTargets = new Hashtable();
                    Hashtable PlayerTargets = new Hashtable();
                    Hashtable NPCTargets = new Hashtable();

                    ushort[] SkillAttributes = ExternalDatabase.SkillAttributes[SkillId][SkillLvl];
                    #region SkillAttributes8
                    if (SkillAttributes[0] == 8)
                    {
                        Character Target = (Character)World.AllChars[TargID];

                        if (Target == null)
                            return;

                        if (SkillId == 1095)
                        {
                            Target.StigBuff = true;
                            Target.Stigged = DateTime.Now;
                            Target.StigLevel = SkillLvl;
                            Target.MyClient.SendPacket(General.MyPackets.Vital(Target.UID, 26, Target.GetStat()));
                            World.UpdateSpawn(Target);
                            Target.MyClient.SendPacket(General.MyPackets.SendMsg(Target.MyClient.MessageId, "System", Target.Name, "Stigma activated: +" + (10 + Target.StigLevel * 5) + " % attack for " + (20 + Target.StigLevel * 5) + " seconds.", 2005));
                        }                        
                        if (SkillId == 1190 && Stamina >= 100)
                        {
                            if (SkillLvl == 0)
                            {
                                CurHP += 500;
                                if (CurHP > MaxHP)
                                    CurHP = MaxHP;
                                Stamina = 0;
                                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));
                                MyClient.SendPacket(General.MyPackets.Vital(UID, 9, Stamina));
                                World.UsingSkill(this, (short)SkillId, SkillLvl, UID, 500, (short)LocX, (short)LocY);
                            }
                            if (SkillLvl == 1)
                            {
                                CurHP += 800;
                                if (CurHP > MaxHP)
                                    CurHP = MaxHP;
                                Stamina = 0;
                                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));
                                MyClient.SendPacket(General.MyPackets.Vital(UID, 9, Stamina));
                                World.UsingSkill(this, (short)SkillId, SkillLvl, UID, 800, (short)LocX, (short)LocY);
                            }
                            if (SkillLvl == 2)
                            {
                                CurHP += 1300;
                                if (CurHP > MaxHP)
                                    CurHP = MaxHP;
                                Stamina = 0;
                                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));
                                MyClient.SendPacket(General.MyPackets.Vital(UID, 9, Stamina));
                                World.UsingSkill(this, (short)SkillId, SkillLvl, UID, 1300, (short)LocX, (short)LocY);
                            }
                        }
                        if (SkillId == 1100)
                            if (!Target.Alive)
                                Target.Revive(false);

                        if (SkillId == 1050)
                            if (!Target.Alive)
                                Target.Revive(false);

                        World.UsingSkill(this, (short)SkillId, SkillLvl, TargID, 0, (short)Target.LocX, (short)Target.LocY);
                    }
                    #endregion
                    #region SkillAttributes7
                    if (SkillAttributes[0] == 7)
                    {
                        if (SkillId == 4000 && Stamina >= 100)
                        {
                            if (Guard == null)
                            {
                                if (SkillLvl == 0)

                                    Mobs.NewRBGuard(Convert.ToInt16(MyClient.MyChar.LocX), Convert.ToInt16(MyClient.MyChar.LocY), Convert.ToInt16(MyClient.MyChar.LocMap), MyClient.MyChar.UID, 0);
                                if (SkillLvl == 1)
                                    Mobs.NewRBGuard(Convert.ToInt16(MyClient.MyChar.LocX), Convert.ToInt16(MyClient.MyChar.LocY), Convert.ToInt16(MyClient.MyChar.LocMap), MyClient.MyChar.UID, 1);
                                if (SkillLvl == 2)
                                    Mobs.NewRBGuard(Convert.ToInt16(MyClient.MyChar.LocX), Convert.ToInt16(MyClient.MyChar.LocY), Convert.ToInt16(MyClient.MyChar.LocMap), MyClient.MyChar.UID, 2);
                                if (SkillLvl == 3)
                                    Mobs.NewRBGuard(Convert.ToInt16(MyClient.MyChar.LocX), Convert.ToInt16(MyClient.MyChar.LocY), Convert.ToInt16(MyClient.MyChar.LocMap), MyClient.MyChar.UID, 3);

                            }
                            else
                            {
                                Guard.Dissappear();

                                if (SkillLvl == 0)
                                    Mobs.NewRBGuard(Convert.ToInt16(MyClient.MyChar.LocX), Convert.ToInt16(MyClient.MyChar.LocY), Convert.ToInt16(MyClient.MyChar.LocMap), MyClient.MyChar.UID, 0);
                                if (SkillLvl == 1)
                                    Mobs.NewRBGuard(Convert.ToInt16(MyClient.MyChar.LocX), Convert.ToInt16(MyClient.MyChar.LocY), Convert.ToInt16(MyClient.MyChar.LocMap), MyClient.MyChar.UID, 1);
                                if (SkillLvl == 2)
                                    Mobs.NewRBGuard(Convert.ToInt16(MyClient.MyChar.LocX), Convert.ToInt16(MyClient.MyChar.LocY), Convert.ToInt16(MyClient.MyChar.LocMap), MyClient.MyChar.UID, 2);
                                if (SkillLvl == 3)
                                    Mobs.NewRBGuard(Convert.ToInt16(MyClient.MyChar.LocX), Convert.ToInt16(MyClient.MyChar.LocY), Convert.ToInt16(MyClient.MyChar.LocMap), MyClient.MyChar.UID, 3);
                            }
                            Stamina = 0;
                            ;
                            MyClient.SendPacket(General.MyPackets.Vital(UID, 9, Stamina));
                        }
                        if (SkillId == 9876 && Stamina >= 100)//Bless
                        {
                            if (!CastingPray && !Praying)
                            {
                                if (!Mining)
                                {
                                    //Created by Kinshi88
                                    PrayCasted = DateTime.Now;
                                    CastingPray = true;
                                    Stamina = 0;
                                    PrayX = LocX;
                                    PrayY = LocY;
                                    if (World.PlayersPraying.Contains(this))
                                    {
                                        World.PlayersPraying.Remove(this);
                                        World.PlayersPraying.Add(this);
                                    }
                                    else
                                        World.PlayersPraying.Add(this);

                                    MyClient.SendPacket(General.MyPackets.Vital(UID, 29, LuckTime));
                                    MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                                    MyClient.SendPacket(General.MyPackets.Vital(UID, 9, Stamina));
                                    World.UpdateSpawn(MyClient.MyChar);
                                }
                                else
                                    MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "SYSTEM", Name, "Cannot cast Bless if you are Mining!", 2005));
                            }
                        }
                        if (SkillId == 4000 && Stamina >= 100)
                        {
                            if (Guard == null)
                            {
                                if (SkillLvl == 0)

                                    Mobs.NewRBGuard(Convert.ToInt16(MyClient.MyChar.LocX), Convert.ToInt16(MyClient.MyChar.LocY), Convert.ToInt16(MyClient.MyChar.LocMap), MyClient.MyChar.UID, 0);
                                if (SkillLvl == 1)
                                    Mobs.NewRBGuard(Convert.ToInt16(MyClient.MyChar.LocX), Convert.ToInt16(MyClient.MyChar.LocY), Convert.ToInt16(MyClient.MyChar.LocMap), MyClient.MyChar.UID, 1);
                                if (SkillLvl == 2)
                                    Mobs.NewRBGuard(Convert.ToInt16(MyClient.MyChar.LocX), Convert.ToInt16(MyClient.MyChar.LocY), Convert.ToInt16(MyClient.MyChar.LocMap), MyClient.MyChar.UID, 2);
                                if (SkillLvl == 3)
                                    Mobs.NewRBGuard(Convert.ToInt16(MyClient.MyChar.LocX), Convert.ToInt16(MyClient.MyChar.LocY), Convert.ToInt16(MyClient.MyChar.LocMap), MyClient.MyChar.UID, 3);

                            }
                            else
                            {
                                Guard.Dissappear();

                                if (SkillLvl == 0)
                                    Mobs.NewRBGuard(Convert.ToInt16(MyClient.MyChar.LocX), Convert.ToInt16(MyClient.MyChar.LocY), Convert.ToInt16(MyClient.MyChar.LocMap), MyClient.MyChar.UID, 0);
                                if (SkillLvl == 1)
                                    Mobs.NewRBGuard(Convert.ToInt16(MyClient.MyChar.LocX), Convert.ToInt16(MyClient.MyChar.LocY), Convert.ToInt16(MyClient.MyChar.LocMap), MyClient.MyChar.UID, 1);
                                if (SkillLvl == 2)
                                    Mobs.NewRBGuard(Convert.ToInt16(MyClient.MyChar.LocX), Convert.ToInt16(MyClient.MyChar.LocY), Convert.ToInt16(MyClient.MyChar.LocMap), MyClient.MyChar.UID, 2);
                                if (SkillLvl == 3)
                                    Mobs.NewRBGuard(Convert.ToInt16(MyClient.MyChar.LocX), Convert.ToInt16(MyClient.MyChar.LocY), Convert.ToInt16(MyClient.MyChar.LocMap), MyClient.MyChar.UID, 3);
                            }
                            Stamina = 0;
                            ;
                            MyClient.SendPacket(General.MyPackets.Vital(UID, 9, Stamina));
                        }
                        if (SkillId == 8003 && Stamina >= 100)
                        {
                            Flying = true;
                            XpList = false;
                            if (SkillLvl == 0)
                                FlyType = 0;
                            else if (SkillLvl == 1)
                                FlyType = 1;
                            Stamina = 0;
                            FlyActivated = DateTime.Now;

                            MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                            MyClient.SendPacket(General.MyPackets.Vital(UID, 9, Stamina));
                        }
                        if (SkillId == 8002)
                        {
                            Flying = true;
                            XpList = false;
                            FlyType = 0;
                            FlyActivated = DateTime.Now;

                            MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                        }
                        if (SkillId == 1110)
                        {
                            CycloneOn = true;
                            XpList = false;
                            XpCircle = 0;
                            XPActivated = DateTime.Now;
                            ExtraXP = 20000;

                            MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                        }
                        if (SkillId == 1025)
                        {
                            SMOn = true;
                            XpList = false;
                            XpCircle = 0;
                            XPActivated = DateTime.Now;
                            ExtraXP = 20000;

                            MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                        }
                        if (SkillId == 1015)
                        {
                            XpList = false;
                            AccuracyOn = true;
                            AccuracyActivated = DateTime.Now;
                            MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                            MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "SYSTEM", Name, "Accuracy XP: Your accuracy wil be increased for 200 seconds.", 2005));
                        }
                        World.UsingSkill(this, (short)SkillId, SkillLvl, UID, 0, (short)LocX, (short)LocY);
                        World.UpdateSpawn(this);
                    }
                    #endregion
                    #region SkillAttributes5
                    if (SkillAttributes[0] == 5)
                    {

                        foreach (DictionaryEntry DE in Mobs.AllMobs)
                        {
                            SingleMob Mob = (SingleMob)DE.Value;

                            if (Mob.Alive)
                                if (Mob.Map == LocMap)
                                    if (MyMath.PointDistance(LocX, LocY, Mob.PosX, Mob.PosY) < SkillAttributes[1])
                                    {
                                        if (PKMode == 0)
                                            if (Mob.MType == 1)
                                            {
                                                GotBlueName = DateTime.Now;
                                                BlueName = true;
                                                MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                                                World.UpdateSpawn(this);
                                            }

                                        double MobAngle = MyMath.PointDirecton(LocX, LocY, Mob.PosX, Mob.PosY);
                                        double Aim = MyMath.PointDirecton(LocX, LocY, X, Y);
                                        int Sector = (int)SkillAttributes[2];

                                        if (PKMode == 0 || Mob.MType != 1)
                                        {
                                            if (MobAngle < Aim + Sector)
                                                if (MobAngle > Aim - Sector)
                                                    if (!MobTargets.Contains(Mob))
                                                        MobTargets.Add(Mob, Other.CalculateDamage(this, Mob, 2, SkillId, SkillLvl));

                                            if (Aim < Sector)
                                                if (MobAngle + Sector - Aim > 360)
                                                    if (MobAngle > 360 - Sector + Aim)
                                                        if (!MobTargets.Contains(Mob))
                                                            MobTargets.Add(Mob, Other.CalculateDamage(this, Mob, 2, SkillId, SkillLvl));

                                            if (360 - Aim < Sector)
                                                if (MobAngle < 90 - (360 - Aim))
                                                    if (!MobTargets.Contains(Mob))
                                                        MobTargets.Add(Mob, Other.CalculateDamage(this, Mob, 2, SkillId, SkillLvl));
                                        }
                                    }
                        }

                        foreach (DictionaryEntry DE in NPCs.AllNPCs)
                        {
                            SingleNPC NPC = (SingleNPC)DE.Value;

                            if (NPC.Flags == 21 || NPC.Flags == 22 || NPC.Flags == 10)
                                if (Level >= NPC.Level)
                                    if (NPC.Map == LocMap)
                                        if (MyMath.PointDistance(LocX, LocY, NPC.X, NPC.Y) <= SkillAttributes[1])
                                        {
                                            double NPCAngle = MyMath.PointDirecton(LocX, LocY, NPC.X, NPC.Y);
                                            double Aim = MyMath.PointDirecton(LocX, LocY, X, Y);
                                            int Sector = (int)SkillAttributes[2];

                                            if (NPCAngle < Aim + Sector)
                                                if (NPCAngle > Aim - Sector)
                                                    if (!NPCTargets.Contains(NPC))
                                                        NPCTargets.Add(NPC, Other.CalculateDamage(this, NPC, 2, SkillId, SkillLvl));

                                            if (Aim < Sector)
                                                if (NPCAngle + Sector - Aim > 360)
                                                    if (NPCAngle > 360 - Sector + Aim)
                                                        if (!NPCTargets.Contains(NPC))
                                                            NPCTargets.Add(NPC, Other.CalculateDamage(this, NPC, 2, SkillId, SkillLvl));

                                            if (360 - Aim < Sector)
                                                if (NPCAngle < 90 - (360 - Aim))
                                                    if (!NPCTargets.Contains(NPC))
                                                        NPCTargets.Add(NPC, Other.CalculateDamage(this, NPC, 2, SkillId, SkillLvl));
                                        }

                        }

                        if (!Other.NoPK(LocMap))
                            if (PKMode == 2 || PKMode == 0)
                            {
                                foreach (DictionaryEntry DE in World.AllChars)
                                {
                                    Character Char = (Character)DE.Value;

                                    if (Char.LocMap == LocMap)
                                        if (Char != this)
                                            if (Char.Alive)
                                                if (MyMath.PointDistance(LocX, LocY, Char.LocX, Char.LocY) <= SkillAttributes[1])
                                                {
                                                    double CharAngle = MyMath.PointDirecton(LocX, LocY, Char.LocX, Char.LocY);
                                                    double Aim = MyMath.PointDirecton(LocX, LocY, X, Y);
                                                    int Sector = (int)SkillAttributes[2];

                                                    if (CharAngle < Aim + Sector)
                                                        if (CharAngle > Aim - Sector)
                                                            if (!PlayerTargets.Contains(Char))
                                                                PlayerTargets.Add(Char, Other.CalculateDamage(this, Char, 2, SkillId, SkillLvl));

                                                    if (Aim < Sector)
                                                        if (CharAngle + Sector - Aim > 360)
                                                            if (CharAngle > 360 - Sector + Aim)
                                                                if (!PlayerTargets.Contains(Char))
                                                                    PlayerTargets.Add(Char, Other.CalculateDamage(this, Char, 2, SkillId, SkillLvl));

                                                    if (360 - Aim < Sector)
                                                        if (CharAngle < 90 - (360 - Aim))
                                                            if (!PlayerTargets.Contains(Char))
                                                                PlayerTargets.Add(Char, Other.CalculateDamage(this, Char, 2, SkillId, SkillLvl));
                                                }
                                }
                            }
                        World.UsingSkill(this, MobTargets, NPCTargets, PlayerTargets, (short)X, (short)Y, (short)SkillId, SkillLvl);
                    }
                    #endregion
                    #region SkillAttributes3n11
                    if (SkillAttributes[0] == 3)
                    {
                        if (SkillAttributes[0] == 3 && SkillAttributes[5] == 1)
                        {
                            XpList = false;
                            MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                        }
                        if (TargID < 7000 && TargID >= 5000)
                        {
                            SingleNPC Target = (SingleNPC)NPCs.AllNPCs[TargID];
                            X = (ushort)Target.X;
                            Y = (ushort)Target.Y;
                        }
                        else if (TargID > 400000 && TargID <= 500000)
                        {
                            SingleMob Target = (SingleMob)Mobs.AllMobs[TargID];
                            X = (ushort)Target.PosX;
                            Y = (ushort)Target.PosY;
                        }
                        else
                        {
                            Character Target = (Character)World.AllChars[TargID];
                            X = Target.LocX;
                            Y = Target.LocY;
                        }

                        foreach (DictionaryEntry DE in Mobs.AllMobs)
                        {
                            SingleMob Mob = (SingleMob)DE.Value;

                            if (Mob.Map == LocMap)
                                if (Mob.Alive)
                                    if (MyMath.PointDistance(X, Y, Mob.PosX, Mob.PosY) <= SkillAttributes[1])
                                    {
                                        if (PKMode == 0)
                                            if (Mob.MType == 1 || Mob.MType == 7)
                                            {
                                                GotBlueName = DateTime.Now;
                                                BlueName = true;
                                                MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                                                World.UpdateSpawn(this);
                                            }

                                        if (PKMode == 0 || Mob.MType != 1)
                                            MobTargets.Add(Mob, Other.CalculateDamage(this, Mob, 3, SkillId, SkillLvl));
                                    }
                        }

                        foreach (DictionaryEntry DE in NPCs.AllNPCs)
                        {
                            SingleNPC NPC = (SingleNPC)DE.Value;
                            if (Level >= NPC.Level)

                                if (NPC.Map == LocMap)
                                    if (NPC.Flags == 21 || NPC.Flags == 22 || NPC.Flags == 10)
                                        if (MyMath.PointDistance(X, Y, NPC.X, NPC.Y) <= SkillAttributes[1])
                                            NPCTargets.Add(NPC, Other.CalculateDamage(this, NPC, 3, SkillId, SkillLvl));

                        }

                        if (!Other.NoPK(LocMap))
                            if (PKMode == 2 || PKMode == 0)
                            {
                                foreach (DictionaryEntry DE in World.AllChars)
                                {
                                    Character Char = (Character)DE.Value;

                                    if (Char.LocMap == LocMap)
                                        if (Char != this)
                                            if (Char.Alive)
                                                if (MyMath.PointDistance(X, Y, Char.LocX, Char.LocY) <= SkillAttributes[1])
                                                {
                                                    PlayerTargets.Add(Char, Other.CalculateDamage(this, Char, 3, SkillId, SkillLvl));
                                                }
                                }
                            }
                        World.UsingSkill(this, MobTargets, NPCTargets, PlayerTargets, (short)X, (short)Y, (short)SkillId, SkillLvl);
                    }
                    #endregion
                    #region SkillAttributes2n6n12
                    if (SkillAttributes[0] == 2 || SkillAttributes[0] == 6 || SkillAttributes[0] == 12 || SkillAttributes[0] == 13)
                    {
                        if (LocMap != 1039 && Stamina >= SkillAttributes[4])
                        {
                            Stamina -= (byte)SkillAttributes[4];
                            MyClient.SendPacket(General.MyPackets.Vital(UID, 9, Stamina));
                        }
                        else if (LocMap != 1039)
                            return;

                        short Heal = 0;
                        if (TargID < 7000 && TargID >= 5000)
                        {
                            SingleNPC Target = (SingleNPC)NPCs.AllNPCs[TargID];

                            X = (ushort)Target.X;
                            Y = (ushort)Target.Y;

                            if (SkillAttributes[0] == 6)
                            {
                                Heal = (short)SkillAttributes[3];

                                if (Target.MaxHP - Target.CurHP < Heal)
                                    Heal = (short)(Target.MaxHP - Target.CurHP);

                                Target.CurHP += (ushort)Heal;
                            }
                            if (Level >= Target.Level)
                            {
                                if (SkillAttributes[0] == 2)
                                    NPCTargets.Add(Target, Other.CalculateDamage(this, Target, 3, SkillId, SkillLvl));

                                if (SkillAttributes[0] == 12)
                                    NPCTargets.Add(Target, Other.CalculateDamage(this, Target, 1, SkillId, SkillLvl));

                                if (SkillAttributes[0] == 13)
                                    NPCTargets.Add(Target, Other.CalculateDamage(this, Target, 2, SkillId, SkillLvl));
                            }
                        }
                        else if (TargID > 400000 && TargID <= 500000)
                        {
                            SingleMob Target = (SingleMob)Mobs.AllMobs[TargID];

                            X = (ushort)Target.PosX;
                            Y = (ushort)Target.PosY;
                            if (SkillAttributes[0] == 6)
                            {
                                Heal = (short)SkillAttributes[3];

                                if (Target.MaxHP - Target.CurHP < Heal)
                                    Heal = (short)(Target.MaxHP - Target.CurHP);

                                Target.CurHP += (uint)Heal;
                            }
                            if (SkillAttributes[0] == 2)
                            {
                                if (PKMode == 0)
                                    if (Target.MType == 1 || Target.MType == 7)
                                    {
                                        GotBlueName = DateTime.Now;
                                        BlueName = true;
                                        MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                                        World.UpdateSpawn(this);
                                    }
                                if (PKMode == 0 || Target.MType != 1)
                                    MobTargets.Add(Target, Other.CalculateDamage(this, Target, 3, SkillId, SkillLvl));
                            }
                            if (SkillAttributes[0] == 12)
                            {
                                if (PKMode == 0)
                                    if (Target.MType == 1)
                                    {
                                        GotBlueName = DateTime.Now;
                                        BlueName = true;
                                        MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                                        World.UpdateSpawn(this);
                                    }

                                if (PKMode == 0 || Target.MType != 1)
                                    MobTargets.Add(Target, Other.CalculateDamage(this, Target, 1, SkillId, SkillLvl));
                            }
                            if (SkillAttributes[0] == 13)
                            {
                                if (PKMode == 0)
                                    if (Target.MType == 1 || Target.MType == 7)
                                    {
                                        GotBlueName = DateTime.Now;
                                        BlueName = true;
                                        MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                                        World.UpdateSpawn(this);
                                    }

                                if (PKMode == 0 || Target.MType != 1 && Target.MType != 7)
                                    MobTargets.Add(Target, Other.CalculateDamage(this, Target, 2, SkillId, SkillLvl));
                            }
                        }
                        else
                        {
                            if (!Other.NoPK(LocMap))
                                if (PKMode == 2 || PKMode == 0)
                                {

                                    Character Target = (Character)World.AllChars[TargID];
                                    if (Target.Alive)
                                        if (!Target.Flying || SkillAttributes[0] != 12)
                                        {
                                            X = Target.LocX;
                                            Y = Target.LocY;

                                            if (SkillAttributes[0] == 6)
                                            {
                                                Heal = (short)SkillAttributes[3];

                                                if (Target.MaxHP - Target.CurHP < Heal)
                                                    Heal = (short)(Target.MaxHP - Target.CurHP);

                                                Target.CurHP += (ushort)Heal;

                                                Target.MyClient.SendPacket(General.MyPackets.Vital(Target.UID, 0, Target.CurHP));
                                            }
                                            if (SkillAttributes[0] == 2)
                                            {
                                                PlayerTargets.Add(Target, Other.CalculateDamage(this, Target, 3, SkillId, SkillLvl));
                                            }
                                            if (SkillAttributes[0] == 12)
                                            {
                                                PlayerTargets.Add(Target, Other.CalculateDamage(this, Target, 1, SkillId, SkillLvl));
                                            }
                                            if (SkillAttributes[0] == 13)
                                            {
                                                PlayerTargets.Add(Target, Other.CalculateDamage(this, Target, 2, SkillId, SkillLvl));
                                            }
                                        }
                                }
                        }
                        if (SkillAttributes[0] == 2 || SkillAttributes[0] == 12 || SkillAttributes[0] == 13)
                        {
                            if (SkillAttributes[4] == 255)
                            {
                                XpList = false;
                                MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                            }
                            World.UsingSkill(this, MobTargets, NPCTargets, PlayerTargets, (short)X, (short)Y, (short)SkillId, SkillLvl);
                        }
                        else
                            World.UsingSkill(this, (short)SkillId, SkillLvl, TargID, (uint)Heal, (short)X, (short)Y);
                    }

                    #endregion
                    #region SkillAttributes1n0
                    if (SkillAttributes[0] == 1 || SkillAttributes[0] == 0 || SkillAttributes[0] == 14)
                    {
                        if (Stamina < SkillAttributes[4])
                            return;
                        if (LocMap != 1039)
                        {
                            Stamina -= (byte)SkillAttributes[4];
                            MyClient.SendPacket(General.MyPackets.Vital(UID, 9, Stamina));
                        }

                        if (SkillAttributes[0] == 1 || SkillAttributes[0] == 14 && SkillAttributes[5] == 1)
                        {
                            XpList = false;
                            MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                        }
                        foreach (DictionaryEntry DE in Mobs.AllMobs)
                        {
                            SingleMob Mob = (SingleMob)DE.Value;
                            if (Mob.Map == LocMap)
                                if (Mob.Alive)
                                    if (MyMath.PointDistance(LocX, LocY, Mob.PosX, Mob.PosY) <= SkillAttributes[1])
                                    {
                                        if (PKMode == 0)
                                            if (Mob.MType == 1)
                                            {
                                                GotBlueName = DateTime.Now;
                                                BlueName = true;
                                                MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                                                World.UpdateSpawn(this);
                                            }
                                        if (PKMode == 0 || Mob.MType != 1 && Mob.MType != 7)
                                        {
                                            if (SkillAttributes[0] == 0)
                                                MobTargets.Add(Mob, Other.CalculateDamage(this, Mob, 1, SkillId, SkillLvl));
                                            else if (SkillAttributes[0] == 1)
                                                MobTargets.Add(Mob, Other.CalculateDamage(this, Mob, 3, SkillId, SkillLvl));
                                            else if (SkillAttributes[0] == 14)
                                                MobTargets.Add(Mob, Other.CalculateDamage(this, Mob, 2, SkillId, SkillLvl));
                                        }
                                    }
                        }
                        foreach (DictionaryEntry DE in NPCs.AllNPCs)
                        {
                            SingleNPC NPC = (SingleNPC)DE.Value;
                            if (Level >= NPC.Level)
                                if (NPC.Map == LocMap)
                                    if (NPC.Flags == 21 || NPC.Flags == 22 || NPC.Flags == 10)
                                        if (MyMath.PointDistance(LocX, LocY, NPC.X, NPC.Y) <= SkillAttributes[1])
                                        {
                                            if (SkillAttributes[0] == 0 || SkillAttributes[0] == 10)
                                                NPCTargets.Add(NPC, Other.CalculateDamage(this, NPC, 1, SkillId, SkillLvl));
                                            else if (SkillAttributes[0] == 1)
                                                NPCTargets.Add(NPC, Other.CalculateDamage(this, NPC, 3, SkillId, SkillLvl));
                                            else if (SkillAttributes[0] == 14)
                                                NPCTargets.Add(NPC, Other.CalculateDamage(this, NPC, 2, SkillId, SkillLvl));
                                        }
                        }
                        if (!Other.NoPK(LocMap))
                            if (PKMode == 2 || PKMode == 0)
                            {
                                foreach (DictionaryEntry DE in World.AllChars)
                                {
                                    Character Char = (Character)DE.Value;
                                    if (!Char.Flying)
                                        if (Char.LocMap == LocMap)
                                            if (Char != this)
                                                if (Char.Alive)
                                                    if (MyMath.PointDistance(LocX, LocY, Char.LocX, Char.LocY) <= SkillAttributes[1])
                                                    {
                                                        if (SkillAttributes[0] == 0)
                                                        {
                                                            PlayerTargets.Add(Char, Other.CalculateDamage(this, Char, 1, SkillId, SkillLvl));
                                                        }
                                                        else if (SkillAttributes[0] == 1)
                                                        {
                                                            PlayerTargets.Add(Char, Other.CalculateDamage(this, Char, 0, SkillId, SkillLvl));
                                                        }
                                                        else
                                                        {
                                                            PlayerTargets.Add(Char, Other.CalculateDamage(this, Char, 2, SkillId, SkillLvl));
                                                        }
                                                    }
                                }
                            }
                        World.UsingSkill(this, MobTargets, NPCTargets, PlayerTargets, (short)X, (short)Y, (short)SkillId, SkillLvl);
                    }
                    #endregion
                    #region SkillAttributes4

                    if (SkillAttributes[0] == 4 && Stamina >= SkillAttributes[4])
                    {
                        if (LocMap != 1039)
                        {
                            Stamina -= (byte)SkillAttributes[4];
                            MyClient.SendPacket(General.MyPackets.Vital(UID, 9, Stamina));
                        }
                        foreach (DictionaryEntry DE in Mobs.AllMobs)
                        {
                            SingleMob Mob = (SingleMob)DE.Value;

                            if (Mob.Map == LocMap)
                                if (Mob.Alive)
                                    if (MyMath.PointDistance(LocX, LocY, Mob.PosX, Mob.PosY) < SkillAttributes[1])
                                        if (MyMath.PointDirecton(LocX, LocY, Mob.PosX, Mob.PosY) == MyMath.PointDirecton(LocX, LocY, X, Y))
                                        {
                                            if (PKMode == 0)
                                                if (Mob.MType == 1)
                                                {
                                                    GotBlueName = DateTime.Now;
                                                    BlueName = true;
                                                    MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                                                    World.UpdateSpawn(this);
                                                }
                                            if (PKMode == 0 || Mob.MType != 1 && Mob.MType != 7)
                                                MobTargets.Add(Mob, Other.CalculateDamage(this, Mob, 0, SkillId, SkillLvl));
                                        }
                        }
                        foreach (DictionaryEntry DE in NPCs.AllNPCs)
                        {
                            SingleNPC NPC = (SingleNPC)DE.Value;

                            if (Level >= NPC.Level)
                                if (NPC.Map == LocMap)
                                    if (MyMath.PointDistance(LocX, LocY, NPC.X, NPC.Y) < SkillAttributes[1])
                                        if (MyMath.PointDirecton(LocX, LocY, NPC.X, NPC.Y) == MyMath.PointDirecton(LocX, LocY, X, Y))
                                            if (NPC.Flags == 21 || NPC.Flags == 22 || NPC.Flags == 10)
                                                NPCTargets.Add(NPC, Other.CalculateDamage(this, NPC, 0, SkillId, SkillLvl));

                        }
                        if (PKMode == 2 || PKMode == 0)
                            if (!Other.NoPK(LocMap))
                            {
                                foreach (DictionaryEntry DE in World.AllChars)
                                {
                                    Character Char = (Character)DE.Value;

                                    if (!Char.Flying)
                                        if (Char.LocMap == LocMap)
                                            if (Char != this)
                                                if (Char.Alive)
                                                    if (MyMath.PointDistance(LocX, LocY, Char.LocX, Char.LocY) < SkillAttributes[1])
                                                        if (MyMath.PointDirecton(LocX, LocY, Char.LocX, Char.LocY) == MyMath.PointDirecton(LocX, LocY, X, Y))
                                                        {
                                                            PlayerTargets.Add(Char, Other.CalculateDamage(this, Char, 0, SkillId, SkillLvl));
                                                        }

                                }
                            }
                        World.UsingSkill(this, MobTargets, NPCTargets, PlayerTargets, (short)X, (short)Y, (short)SkillId, SkillLvl);
                    }
                    #endregion

                }
            Ready = true;
        }
        #endregion
        public void SaveRank()
        {
            LastSave = DateTime.Now;
            if (MyClient.There)
                if (MyClient.Online)
                {
                    InternalDatabase.SaveChar(this);
                    InternalDatabase.SaveRank(this);
                }
        }
        public void SaveDonation()
        {
            LastSave = DateTime.Now;
            if (MyClient.There)
                if (MyClient.Online)
                {
                    InternalDatabase.SaveChar(this);
                    InternalDatabase.SaveDonation(this);
                }
        }
        public void SaveFCPs()
        {
            LastSave = DateTime.Now;
            if (MyClient.There)
                if (MyClient.Online)
                {
                    InternalDatabase.SaveChar(this);
                    InternalDatabase.SaveFCPs(this);
                }
        }
        public void SaveSpouse()
        {
            if (MyClient.There)
                if (MyClient.Online)
                    InternalDatabase.SaveSpouse(this);
        }
        public void Save()
        {
            LastSave = DateTime.Now;
            if (MyClient.There)
                if (MyClient.Online)
                    InternalDatabase.SaveChar(this);
        }

        public bool Ready = true;

        public bool InventoryContains(uint Id, uint Amount)
        {
            Ready = false;
            uint Count = 0;

            foreach (string item in Inventory)
            {
                if (item != null && item != "")
                {
                    string[] Splitter = item.Split('-');
                    if (uint.Parse(Splitter[0]) == Id)
                        Count++;
                }
            }

            Ready = true;

            if (Count >= Amount)
                return true;
            else
                return false;
        }

        public uint ItemNext(uint Id)
        {
            Ready = false;
            int Count = 0;
            uint IUID = 0;

            foreach (string item in Inventory)
            {
                if (item != null && item != "")
                {
                    string[] Splitter = item.Split('-');
                    if (uint.Parse(Splitter[0]) == Id)
                    {
                        IUID = (uint)Inventory_UIDs[Count];
                        break;
                    }
                }
                Count++;
            }

            Ready = true;

            return IUID;
        }

        public bool AddExp(ulong Amount, bool CountMisc)
        {
            Ready = false;
            if (RBCount < 1)
                Amount /= 3;
            if (CountMisc && !EPot)
                Exp += (ulong)(Amount * ExternalDatabase.ExpRate * AddExpPc * (Convert.ToDouble(Potency) / 100 + 1));
            else if (EPot)
                Exp += (ulong)((Amount * ExternalDatabase.ExpRate * AddExpPc * (Convert.ToDouble(Potency) / 100 + 1)) * 2);

            else
                if (EPot)
                {
                    Exp *= 2;
                }

            Exp += Amount;
            if (EPotRate == true)
                Exp += (ulong)((double)Amount * (double)ExternalDatabase.ExpRate * (double)AddExpPc * (Convert.ToDouble(Potency) / 100 + 1));

            bool Leveled = false;

            if (Exp > ExternalDatabase.NeededXP(Level) && Level < 140)
            {
                Leveled = true;
                while (Exp > ExternalDatabase.NeededXP(Level) && Level < 140)
                {
                    Exp -= ExternalDatabase.NeededXP(Level);
                    Level++;
                    if (RBCount > 0)
                    {
                        StatP += 5;
                        MyClient.SendPacket(General.MyPackets.Vital(UID, 11, StatP));
                    }
                    if (Level == 3)
                    {
                        if (Job < 16 && Job > 9)
                            LearnSkill(1110, 0);
                        if (Job < 16 && Job > 9 || Job < 26 && Job > 19)
                            LearnSkill(1015, 0);
                        if (Job < 26 && Job > 19)
                            LearnSkill(1025, 0);
                    }
                }
            }
            if (Leveled)
            {
                if (MyGuild != null)
                    MyGuild.Refresh(this);
                if (RBCount < 1)
                    InternalDatabase.GetStats(this);
                GetEquipStats(1, true);
                GetEquipStats(2, true);
                GetEquipStats(3, true);
                GetEquipStats(4, true);
                GetEquipStats(5, true);
                GetEquipStats(6, true);
                GetEquipStats(8, true);
                MaxHP = BaseMaxHP();
                MinAtk = Str;
                MaxAtk = Str;
                Potency = Level;
                GetEquipStats(1, false);
                GetEquipStats(2, false);
                GetEquipStats(3, false);
                GetEquipStats(4, false);
                GetEquipStats(5, false);
                GetEquipStats(6, false);
                GetEquipStats(8, false);

                CurHP = MaxHP;

                MyClient.SendPacket(General.MyPackets.Vital((long)UID, 13, Level));
                MyClient.SendPacket(General.MyPackets.Vital((long)UID, 16, Str));
                MyClient.SendPacket(General.MyPackets.Vital((long)UID, 17, Agi));
                MyClient.SendPacket(General.MyPackets.Vital((long)UID, 15, Vit));
                MyClient.SendPacket(General.MyPackets.Vital((long)UID, 14, Spi));
                MyClient.SendPacket(General.MyPackets.Vital((long)UID, 0, CurHP));
                World.LevelUp(this);
            }
            MyClient.SendPacket(General.MyPackets.Vital((long)UID, 5, Exp));
            Ready = true;

            if (Leveled)
                return true;
            else
                return false;
        }

        public void AddSkillExp(short Type, uint Amount)
        {
            if (Type == 4000) Amount = 1;
            Ready = false;
            Amount *= ExternalDatabase.ProfExpRate;
            if (Skills.Contains((short)Type))
            {

                bool SkillLeveled = false;
                byte Skill_Lv = (byte)Skills[(short)Type];
                uint Skill_Exp;
                if (Skill_Exps.Contains((short)Type))
                    Skill_Exp = (uint)Skill_Exps[(short)Type];
                else
                    Skill_Exp = 0;

                int MaxSkillLv = 9;

                MaxSkillLv = (int)ExternalDatabase.SkillsDone[(int)Type];

                Skill_Exp += Amount;

                if (Skill_Exp >= ExternalDatabase.NeededSkillExp((short)Type, Skill_Lv) && Skill_Lv < MaxSkillLv)
                {
                    Skill_Lv++;
                    Skill_Exp = 0;
                    SkillLeveled = true;
                }
                if (SkillLeveled)
                {
                    Skills.Remove((short)Type);
                    Skills.Add((short)Type, Skill_Lv);
                    MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "SYSTEM", Name, "Your skill level has been improved.", 2005));
                }
                if (Skill_Exps.Contains((short)Type))
                    Skill_Exps.Remove((short)Type);
                Skill_Exps.Add(Type, Skill_Exp);
                MyClient.SendPacket(General.MyPackets.LearnSkill(Type, Skill_Lv, Skill_Exp));
            }

            Ready = true;
        }

        public void AddProfExp(short Type, uint Amount)
        {
            Ready = false;
            Amount *= ExternalDatabase.ProfExpRate;
            if (Prof_Exps.Contains(Type))
            {

                bool ProfLeveled = false;
                byte Prof_Lev = (byte)Profs[Type];
                uint Prof_Exp = (uint)Prof_Exps[Type];

                Prof_Exp += Amount;
                if (Prof_Exp > ExternalDatabase.NeededProfXP(Prof_Lev))
                {
                    Prof_Lev++;
                    Prof_Exp = 0;
                    ProfLeveled = true;
                }
                if (ProfLeveled)
                {
                    Profs.Remove(Type);
                    Profs.Add(Type, Prof_Lev);
                    MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "SYSTEM", Name, "Your profociency level has been improved.", 2005));
                }
                Prof_Exps.Remove(Type);
                Prof_Exps.Add(Type, Prof_Exp);
                MyClient.SendPacket(General.MyPackets.Prof(Type, Prof_Lev, Prof_Exp));
            }
            else
            {
                byte Lev = 1;
                uint PExp = 0;
                Profs.Add(Type, Lev);
                Prof_Exps.Add(Type, PExp);
                MyClient.SendPacket(General.MyPackets.Prof(Type, Lev, PExp));
            }
            Ready = true;
        }

        public bool WeaponSkill()
        {
            string[] Splitter;
            bool Use = false;
            int Hand = 1;

            if (Equips[5] != null && Equips[5] != "0")
                if (Other.ChanceSuccess(50))
                    Hand = 2;

            if (Equips[4] != null && Equips[4] != "0" && Hand == 1)
            {
                Splitter = Equips[4].Split('-');
                int WepType = Other.WeaponType(uint.Parse(Splitter[0]));

                if (WepType == 480)
                {
                    if (Skills.Contains((short)7020))
                    {
                        byte SkillLvl = (byte)Skills[(short)7020];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)7020][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(7020, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 530)
                {
                    if (Skills.Contains((short)5050))
                    {
                        byte SkillLvl = (byte)Skills[(short)5050];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)5050][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(5050, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 580)
                {
                    if (Skills.Contains((short)5020))
                    {
                        byte SkillLvl = (byte)Skills[(short)5020];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)5020][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(5020, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 560)
                {
                    if (Skills.Contains((short)1260))
                    {
                        byte SkillLvl = (byte)Skills[(short)1260];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)1260][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(1260, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 510)
                {
                    if (Skills.Contains((short)1250))
                    {
                        byte SkillLvl = (byte)Skills[(short)1250];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)1250][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(1250, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 540)
                {
                    if (Skills.Contains((short)1300))
                    {
                        byte SkillLvl = (byte)Skills[(short)1300];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)1300][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(1300, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 481)
                {
                    if (Skills.Contains((short)7030))
                    {
                        byte SkillLvl = (byte)Skills[(short)7030];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)7030][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(7030, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 430)
                {
                    if (Skills.Contains((short)7000))
                    {
                        byte SkillLvl = (byte)Skills[(short)7000];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)7000][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(7000, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 450)
                {
                    if (Skills.Contains((short)7010))
                    {
                        byte SkillLvl = (byte)Skills[(short)7010];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)7010][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(7010, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 440)
                {
                    if (Skills.Contains((short)7040))
                    {
                        byte SkillLvl = (byte)Skills[(short)7040];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)7040][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(7040, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 460)
                {
                    if (Skills.Contains((short)5040))
                    {
                        byte SkillLvl = (byte)Skills[(short)5040];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)5040][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(5040, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 421)
                {
                    if (Skills.Contains((short)5030))
                    {
                        byte SkillLvl = (byte)Skills[(short)5030];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)5030][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(5030, 0, 0, 0);
                            Use = true;
                        }
                    }
                }

                else if (WepType == 490)
                {
                    if (Skills.Contains((short)1290))
                    {
                        byte SkillLvl = (byte)Skills[(short)1290];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)1290][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(1290, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 420 && !Use)
                {
                    if (TargetUID != 0)
                        if (Skills.Contains((short)5030))
                        {
                            byte SkillLvl = (byte)Skills[(short)5030];
                            byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)5030][(int)SkillLvl][5];
                            if (Other.ChanceSuccess(Chance))
                            {
                                UseSkill(5030, 0, 0, TargetUID);
                                Use = true;
                            }
                        }
                }
                else if (WepType == 561 && !Use)
                {
                    if (Skills.Contains((short)5010))
                    {
                        byte SkillLvl = (byte)Skills[(short)5010];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)5010][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(5010, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
            }
            if (Equips[5] != null && Equips[5] != "0" && Hand == 2)
            {
                Splitter = Equips[5].Split('-');
                int WepType = Other.WeaponType(uint.Parse(Splitter[0]));

                if (WepType == 480 && !Use)
                {
                    if (Skills.Contains((short)7020))
                    {
                        byte SkillLvl = (byte)Skills[(short)7020];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)7020][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(7020, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 490 && !Use)
                {
                    if (Skills.Contains((short)1290))
                    {
                        byte SkillLvl = (byte)Skills[(short)1290];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)1290][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(1290, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 530 && !Use)
                {
                    if (Skills.Contains((short)5050))
                    {
                        byte SkillLvl = (byte)Skills[(short)5050];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)5050][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(5050, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 580 && !Use)
                {
                    if (Skills.Contains((short)5020))
                    {
                        byte SkillLvl = (byte)Skills[(short)5020];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)5020][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(5020, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 560 && !Use)
                {
                    if (Skills.Contains((short)1260))
                    {
                        byte SkillLvl = (byte)Skills[(short)1260];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)1260][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(1260, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 510 && !Use)
                {
                    if (Skills.Contains((short)1250))
                    {
                        byte SkillLvl = (byte)Skills[(short)1250];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)1250][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(1250, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 540 && !Use)
                {
                    if (Skills.Contains((short)1300))
                    {
                        byte SkillLvl = (byte)Skills[(short)1300];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)1300][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(1300, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 481 && !Use)
                {
                    if (Skills.Contains((short)7030))
                    {
                        byte SkillLvl = (byte)Skills[(short)7030];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)7030][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(7030, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 430 && !Use)
                {
                    if (Skills.Contains((short)7000))
                    {
                        byte SkillLvl = (byte)Skills[(short)7000];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)7000][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(7000, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 450 && !Use)
                {
                    if (Skills.Contains((short)7010))
                    {
                        byte SkillLvl = (byte)Skills[(short)7010];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)7010][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(7010, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 440 && !Use)
                {
                    if (Skills.Contains((short)7040))
                    {
                        byte SkillLvl = (byte)Skills[(short)7040];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)7040][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(7040, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 460 && !Use)
                {
                    if (Skills.Contains((short)5040))
                    {
                        byte SkillLvl = (byte)Skills[(short)5040];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)5040][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(5040, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 421 && !Use)
                {
                    if (Skills.Contains((short)5030))
                    {
                        byte SkillLvl = (byte)Skills[(short)5030];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)5030][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(5030, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
                else if (WepType == 420 && !Use)
                {
                    if (TargetUID != 0)
                        if (Skills.Contains((short)5030))
                        {
                            byte SkillLvl = (byte)Skills[(short)5030];
                            byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)5030][(int)SkillLvl][5];
                            if (Other.ChanceSuccess(Chance))
                            {
                                UseSkill(5030, 0, 0, TargetUID);
                                Use = true;
                            }
                        }
                }
                else if (WepType == 420 && !Use)
                {
                    if (TargetUID != 0)
                        if (Skills.Contains((short)5030))
                        {
                            byte SkillLvl = (byte)Skills[(short)5030];
                            byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)5030][(int)SkillLvl][5];
                            if (Other.ChanceSuccess(Chance))
                            {
                                UseSkill(5030, 0, 0, TargetUID);
                                Use = true;
                            }
                        }
                }
                else if (WepType == 561 && !Use)
                {
                    if (Skills.Contains((short)5010))
                    {
                        byte SkillLvl = (byte)Skills[(short)5010];
                        byte Chance = (byte)ExternalDatabase.SkillAttributes[(int)7020][(int)SkillLvl][5];
                        if (Other.ChanceSuccess(Chance))
                        {
                            UseSkill(5010, 0, 0, 0);
                            Use = true;
                        }
                    }
                }
            }
            return Use;
        }

        public void BlueNameGone()
        {
            BlueName = false;
            MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
            World.UpdateSpawn(this);
        }
        public void DirectCPs()
        {
            if (!Alive)
            {
                MobTarget = null;
                SkillLoopingTarget = 0;
                TGTarget = null;
                PTarget = null;
                BlueName = false;
                return;
            }
            Ready = false;
            try
            {
                if (MobTarget != null)
                {
                    if (RBCount == 2)
                    {
                        if (Other.ChanceSuccess(40))
                        {
                            CPs += 50000;
                            MyClient.SendPacket(General.MyPackets.Vital(UID, 30, CPs));
                            MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "SYSTEM", Name, "You got free 50000 CPs from killing Monsters.", 2005));
                        }
                        else if (Other.ChanceSuccess(30))
                        {
                            CPs += 50000;
                            MyClient.SendPacket(General.MyPackets.Vital(UID, 30, CPs));
                            MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "SYSTEM", Name, "You got free 50000 CPs from killing Monsters.", 2005));
                        }
                        else if (Other.ChanceSuccess(20))
                        {
                            CPs += 50000;
                            MyClient.SendPacket(General.MyPackets.Vital(UID, 30, CPs));
                            MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "SYSTEM", Name, "You got free 50000 CPs from killing Monsters.", 2005));
                        }
                    }
                    if (RBCount >= 0 && RBCount <= 2)
                    {
                        if (Other.ChanceSuccess(50))
                        {
                            CPs += 50000;
                            MyClient.SendPacket(General.MyPackets.Vital(UID, 30, CPs));
                            MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "SYSTEM", Name, "You got free 50000 CPs from killing Monsters.", 2005));
                        }
                        if (Other.ChanceSuccess(40))
                        {
                            CPs += 50000;
                            MyClient.SendPacket(General.MyPackets.Vital(UID, 30, CPs));
                            MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "SYSTEM", Name, "You got free 50000 CPs from killing Monsters.", 2005));
                        }
                    }
                }
                Ready = true;
            }
            catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); }
        }
        public void MissAttack(SingleMob Mob)
        {
            World.AttackMiss(this, AtkType, Mob.PosX, Mob.PosY);
        }
        public void MissAttack(SingleNPC NPC)
        {
            World.AttackMiss(this, AtkType, NPC.X, NPC.Y);
        }
        public void MissAttack(Character Player)
        {
            World.AttackMiss(this, AtkType, (short)Player.LocX, (short)Player.LocY);
        }

        #region attack
        public void Attack()
        {
            LastAttack = DateTime.Now;
            if (!Alive)
            {
                MobTarget = null;
                SkillLoopingTarget = 0;
                TGTarget = null;
                PTarget = null;
                return;
            }
            Ready = false;
            try
            {
                if (AtkType == 25 || AtkType == 2)
                {
                    if (!WeaponSkill())
                    {
                        if (MobTarget != null)
                        {
                            if (MobTarget.Alive)
                                if (MobTarget.Map == LocMap)
                                    if (MyMath.PointDistance(LocX, LocY, MobTarget.PosX, MobTarget.PosY) < 6 || AtkType == 25)
                                    {
                                        if (!SMOn && !AccuracyOn)
                                        {
                                            if (!Other.ChanceSuccess(RealAgi / 2 + Math.Abs((110 - Level) / 2)))
                                            {
                                                MissAttack(MobTarget);
                                                return;
                                            }
                                        }
                                        else if (SMOn)
                                        {
                                            if (!Other.ChanceSuccess(RealAgi + Math.Abs((110 - Level) / 2)))
                                            {
                                                MissAttack(MobTarget);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            if (!Other.ChanceSuccess(RealAgi * 1.2 + Math.Abs((110 - Level) / 2)))
                                            {
                                                MissAttack(MobTarget);
                                                return;
                                            }
                                        }

                                        double LDF = (Level - MobTarget.Level + 7) / 5;
                                        LDF = Math.Max(LDF, 1);
                                        double eDMG = ((Convert.ToUInt32(LDF) - 1) * .8) + 1;

                                        double AttackDMG = (double)General.Rand.Next((int)MinAtk, (int)MaxAtk) * eDMG;

                                        if (StigBuff)
                                            AttackDMG = (int)(AttackDMG * (double)(1 + ((double)(10 + StigLevel) / 100)));

                                        AttackDMG = ((double)AttackDMG * AddAtkPc);

                                        if (SMOn)
                                            AttackDMG *= 10;

                                        double ProfExpAdd = 0;

                                        double ExpQuality = 0;

                                        if (MobTarget.Level + 4 < Level)
                                            ExpQuality = 0.1;
                                        if (MobTarget.Level + 4 >= Level)
                                            ExpQuality = 1;
                                        if (MobTarget.Level >= Level)
                                            ExpQuality = 1.1;
                                        if (MobTarget.Level - 4 > Level)
                                            ExpQuality = 1.3;

                                        double EAddExp = 0;
                                        ulong UAddExp = 0;
                                        uint MobCurHP = MobTarget.CurHP;

                                        if (MobTarget.MType == 1)
                                            AttackDMG /= 100;

                                        if (MobTarget.MType == 1 || MobTarget.MType == 7)
                                        {
                                            BlueName = true;
                                            GotBlueName = DateTime.Now;
                                            MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                                            World.UpdateSpawn(this);
                                        }
                                        if (Other.ChanceSuccess(9.7) && Stamina < 100)
                                        {
                                            Stamina = 100;
                                            MyClient.SendPacket(General.MyPackets.Vital(UID, 9, Stamina));
                                            MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "System", Name, "You got lucky,and got your stamina refilled!", 2005));
                                            MyClient.SendPacket(General.MyPackets.String(UID, 10, "LuckyGuy"));
                                        }





                                        if (Other.ChanceSuccess(6.7))
                                        {
                                            XpCircle = 100;
                                            XpList = true;
                                            MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                                            XpCircle = 0;
                                            MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "System", Name, "You got lucky,and your XP skill activated!", 2005));
                                            MyClient.SendPacket(General.MyPackets.String(UID, 10, "LuckyGuy"));
                                        }


                                        if (Other.ChanceSuccess(6.9))
                                        {
                                            AttackDMG *= 2;
                                            MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "System", Name, "You got lucky,and attacked double damage!", 2005));
                                            MyClient.SendPacket(General.MyPackets.String(UID, 10, "LuckyGuy"));
                                        }
                                        if (MobTarget.GetDamage((uint)AttackDMG))
                                        {
                                            if (MobTarget.Name == QuestMob)
                                            {
                                                QuestKO++;
                                                if (QuestKO >= 300)
                                                {
                                                    MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "SYSTEM", Name, "You have killed enough monsters for the quest. Go report to the captain.", 2005));
                                                }
                                            }
                                            Attacking = false;
                                            if (Level >= 70)
                                            {
                                                foreach (Character Member in Team)
                                                {
                                                    if (!Member.Alive)
                                                        continue;
                                                    double PlvlExp = 1;
                                                    if (MobTarget.Level - 20 <= Member.Level)
                                                        PlvlExp = 0.1;
                                                    if (Member != null)
                                                        if (Member.Level + 20 < Level)
                                                            if (Member.LocMap == LocMap)
                                                                if (MyMath.PointDistance(Member.LocX, Member.LocY, LocX, LocY) < 30)
                                                                    if (Member.AddExp((ulong)((double)(52 + (Member.Level * 30)) * PlvlExp), true))
                                                                    {
                                                                        VP += (uint)((Member.Level * 17 / 13 * 12 / 2) + Member.Level * 3);
                                                                        Member.MyTeamLeader.MyClient.SendPacket(General.MyPackets.SendMsg(Member.MyClient.MessageId, "SYSTEM", Member.Name, Name + " has gained " + ((Member.Level * 17 / 13 * 12 / 2) + Member.Level * 3) + " virtue points.", 2003));
                                                                        foreach (Character Member2 in Team)
                                                                        {
                                                                            if (Member2 != null)
                                                                                Member2.MyClient.SendPacket(General.MyPackets.SendMsg(Member2.MyClient.MessageId, "SYSTEM", Member2.Name, Name + " has gained " + ((Member.Level * 17 / 13 * 12 / 2) + Member.Level * 3) + " virtue points.", 2003));
                                                                        }
                                                                    }
                                                }
                                            }
                                            XpCircle++;
                                            if (CycloneOn || SMOn)
                                                ExtraXP += 820;
                                            TargetUID = 0;
                                            EAddExp = MobCurHP * ExpQuality;
                                            UAddExp = Convert.ToUInt64(EAddExp);
                                            AddExp(UAddExp, true);
                                            ProfExpAdd = MobCurHP;
                                            MobTarget.Death = DateTime.Now;

                                            if (Equips[4] != null && Equips[4] != "0")
                                            {
                                                string[] Splitter2 = Equips[4].Split('-');
                                                if (Other.ItemType(uint.Parse(Splitter2[0])) == 4 || Other.ItemType(uint.Parse(Splitter2[0])) == 5)
                                                    AddProfExp((short)Other.WeaponType(uint.Parse(Splitter2[0])), (uint)(ProfExpAdd * AddProfPc));
                                            }
                                            if (Equips[5] != null && Equips[5] != "0")
                                            {
                                                string[] Splitter2 = Equips[5].Split('-');
                                                if (Other.ItemType(uint.Parse(Splitter2[0])) == 4 || Other.ItemType(uint.Parse(Splitter2[0])) == 5)
                                                    AddProfExp((short)Other.WeaponType(uint.Parse(Splitter2[0])), (uint)(ProfExpAdd * AddProfPc));
                                            }

                                            World.AttackMob(this, MobTarget, AtkType, (uint)AttackDMG);
                                            World.AttackMob(this, MobTarget, 14, 0);

                                            World.MobDissappear(MobTarget);
                                            MobTarget.Death = DateTime.Now;

                                            MobTarget = null;
                                        }
                                        else
                                        {
                                            EAddExp = AttackDMG * ExpQuality;
                                            UAddExp = Convert.ToUInt64(EAddExp);
                                            AddExp(UAddExp, true);

                                            ProfExpAdd = AttackDMG;

                                            if (Equips[4] != null && Equips[4] != "0")
                                            {
                                                string[] Splitter2 = Equips[4].Split('-');
                                                if (Other.ItemType(uint.Parse(Splitter2[0])) == 4 || Other.ItemType(uint.Parse(Splitter2[0])) == 5)
                                                    AddProfExp((short)Other.WeaponType(uint.Parse(Splitter2[0])), (uint)(ProfExpAdd * AddProfPc));
                                            }
                                            if (Equips[5] != null && Equips[5] != "0")
                                            {
                                                string[] Splitter2 = Equips[5].Split('-');
                                                if (Other.ItemType(uint.Parse(Splitter2[0])) == 4 || Other.ItemType(uint.Parse(Splitter2[0])) == 5)
                                                    AddProfExp((short)Other.WeaponType(uint.Parse(Splitter2[0])), (uint)(ProfExpAdd * AddProfPc));
                                            }

                                            World.AttackMob(this, MobTarget, AtkType, (uint)AttackDMG);

                                        }
                                    }
                                    else
                                    {
                                        MobTarget = null;
                                    }
                        }
                        else if (PTarget != null && (PKMode == 2 || PKMode == 0) && !Other.NoPK(LocMap) && PTarget.Alive)
                        {
                            if (PTarget.Alive)
                                if (LocMap == PTarget.LocMap)
                                    if (AtkType == 2 && MyMath.PointDistance(LocX, LocY, PTarget.LocX, PTarget.LocY) < 5 || AtkType == 25 && MyMath.PointDistance(LocX, LocY, PTarget.LocX, PTarget.LocY) < 16)
                                    {
                                        int Damage = General.Rand.Next((int)MinAtk, (int)MaxAtk);

                                        if (!SMOn && !AccuracyOn)
                                        {
                                            if (!Other.ChanceSuccess(RealAgi / 1.7 + Math.Abs((110 - Level) / 2)))
                                            {
                                                MissAttack(PTarget);
                                                return;
                                            }
                                        }
                                        else if (SMOn)
                                        {
                                            if (!Other.ChanceSuccess(RealAgi + Math.Abs((110 - Level) / 2)))
                                            {
                                                MissAttack(PTarget);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            if (!Other.ChanceSuccess(RealAgi * 1.2 + Math.Abs((110 - Level) / 2)))
                                            {
                                                MissAttack(PTarget);
                                                return;
                                            }
                                        }
                                        double reborn = 1;

                                        int Atk = General.Rand.Next((int)MyClient.MyChar.MinAtk, (int)MyClient.MyChar.MaxAtk);

                                        Atk = (int)((double)Atk * MyClient.MyChar.AddAtkPc);

                                        if (SMOn)
                                            Atk *= 2;
                                        if (StigBuff)
                                            Atk = (int)(Atk * (double)(1 + ((double)(10 + MyClient.MyChar.StigLevel) / 100)));

                                        if (PTarget.RBCount == 1)
                                            reborn = 0.7;
                                        else if (PTarget.RBCount == 2)
                                            reborn = 0.4;
                                        double Dodge = (double)-(PTarget.Dodge / 100);
                                        if (Dodge == 0)
                                            Dodge = 1;
                                        Damage = (int)(((Atk - PTarget.Defense) * reborn) * Dodge);

                                        if (Damage < 1)
                                            Damage = 1;

                                        if (PTarget.PKPoints < 100)
                                            if (!PTarget.BlueName)
                                                if (!Other.CanPK(LocMap))
                                                    if (LocMap != 1005 && LocMap != 6000 && LocMap != 1038)
                                                    {
                                                        GotBlueName = DateTime.Now;
                                                        BlueName = true;
                                                        MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                                                        World.UpdateSpawn(this);
                                                    }

                                        World.PVP(this, PTarget, AtkType, (uint)Damage);

                                        if (PTarget.GetHitDie((uint)Damage))
                                        {
                                            Attacking = false;
                                            if (!Other.CanPK(LocMap))
                                                if (!PTarget.BlueName)
                                                    if (PTarget.PKPoints < 100)
                                                    {
                                                        GotBlueName = DateTime.Now;
                                                        BlueName = true;
                                                        PKPoints += 10;
                                                        MyClient.SendPacket(General.MyPackets.Vital(UID, 6, PKPoints));
                                                        if ((PKPoints > 29 && PKPoints - 10 < 30) || (PKPoints > 99 && PKPoints - 10 < 100))
                                                        {
                                                            MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                                                            World.UpdateSpawn(this);
                                                        }
                                                    }
                                            World.PVP(this, PTarget, 14, 0);
                                            PTarget.PTarget = null;
                                            PTarget.MobTarget = null;
                                            PTarget = null;
                                        }
                                    }
                                    else
                                        PTarget = null;
                        }
                        else if (TGTarget != null)
                            if (Level >= TGTarget.Level)
                            {
                                {
                                    if (!SMOn && !AccuracyOn)
                                    {
                                        if (!Other.ChanceSuccess(RealAgi / 1.7 + Math.Abs((110 - Level) / 2)))
                                        {
                                            MissAttack(TGTarget);
                                            return;
                                        }
                                    }
                                    else if (SMOn)
                                    {
                                        if (!Other.ChanceSuccess(RealAgi + Math.Abs((110 - Level) / 2)))
                                        {
                                            MissAttack(TGTarget);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        if (!Other.ChanceSuccess(RealAgi * 1.2 + Math.Abs((110 - Level) / 2)))
                                        {
                                            MissAttack(TGTarget);
                                            return;
                                        }
                                    }

                                    uint Damage = (uint)General.Rand.Next((int)MinAtk, (int)MaxAtk);
                                    uint THP = TGTarget.CurHP;
                                    Damage = (uint)((double)Damage * AddAtkPc);
                                    if (StigBuff)
                                        Damage = (uint)(Damage * (double)(1 + ((double)(10 + StigLevel) / 100)));

                                    double ExpQuality = 1;

                                    if (TGTarget.Level + 5 < Level)
                                        ExpQuality = 0.1;

                                    if (TGTarget.GetDamageDie((uint)Damage, this))
                                    {
                                        AddExp((ulong)(THP / 10 * ExpQuality), true);
                                        uint ProfExpAdd = THP / 10;
                                        if (Equips[4] != null && Equips[4] != "0")
                                        {
                                            string[] Splitter2 = Equips[4].Split('-');
                                            if (Other.ItemType(uint.Parse(Splitter2[0])) == 4 || Other.ItemType(uint.Parse(Splitter2[0])) == 5)
                                                AddProfExp((short)Other.WeaponType(uint.Parse(Splitter2[0])), (uint)(ProfExpAdd * AddProfPc));
                                        }
                                        if (Equips[5] != null && Equips[5] != "0")
                                        {
                                            string[] Splitter2 = Equips[5].Split('-');
                                            if (Other.ItemType(uint.Parse(Splitter2[0])) == 4 || Other.ItemType(uint.Parse(Splitter2[0])) == 5)
                                                AddProfExp((short)Other.WeaponType(uint.Parse(Splitter2[0])), (uint)(ProfExpAdd * AddProfPc));
                                        };

                                        World.PlAttacksTG(TGTarget, this, (byte)AtkType, (uint)Damage);
                                        World.PlAttacksTG(TGTarget, this, 14, (uint)Damage);

                                    }
                                    else
                                    {
                                        AddExp((ulong)(Damage / 10 * ExpQuality), true);
                                        uint ProfExpAdd = Damage / 10;

                                        if (Equips[4] != null && Equips[4] != "0")
                                        {
                                            string[] Splitter2 = Equips[4].Split('-');
                                            if (Other.ItemType(uint.Parse(Splitter2[0])) == 4 || Other.ItemType(uint.Parse(Splitter2[0])) == 5)
                                                AddProfExp((short)Other.WeaponType(uint.Parse(Splitter2[0])), (uint)(ProfExpAdd * AddProfPc));
                                        }
                                        if (Equips[5] != null && Equips[5] != "0")
                                        {
                                            string[] Splitter2 = Equips[5].Split('-');
                                            if (Other.ItemType(uint.Parse(Splitter2[0])) == 4 || Other.ItemType(uint.Parse(Splitter2[0])) == 5)
                                                AddProfExp((short)Other.WeaponType(uint.Parse(Splitter2[0])), (uint)(ProfExpAdd * AddProfPc));
                                        };

                                        World.PlAttacksTG(TGTarget, this, (byte)AtkType, (uint)Damage);


                                    }
                                }
                            }
                            else
                                MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "SYSTEM", Name, "You can't attack this, you are not high level enough. Find a lower level one.", 2000));

                        Ready = true;
                    }
                }
                else if (AtkType == 21)
                {
                    if (SkillLooping != 0)
                    {
                        if (SkillLoopingTarget != 0)
                        {
                            if (SkillLoopingTarget > 400000 && SkillLoopingTarget < 500000)
                            {
                                SingleMob Targ = (SingleMob)Mobs.AllMobs[SkillLoopingTarget];
                                if (Targ == null || !Targ.Alive)
                                    return;
                                else
                                    UseSkill(SkillLooping, (ushort)Targ.PosX, (ushort)Targ.PosY, SkillLoopingTarget);

                            }
                            else if (SkillLoopingTarget < 7000 && SkillLoopingTarget >= 5000)
                            {
                                SingleNPC Targ = (SingleNPC)NPCs.AllNPCs[SkillLoopingTarget];
                                if (Targ == null)
                                    return;
                                else
                                    UseSkill(SkillLooping, (ushort)Targ.X, (ushort)Targ.Y, SkillLoopingTarget);

                            }
                            else
                            {
                                Character Targ = (Character)World.AllChars[SkillLoopingTarget];
                                if (Targ == null || !Targ.Alive)
                                    return;
                                else
                                    UseSkill(SkillLooping, Targ.LocX, Targ.LocY, SkillLoopingTarget);
                            }
                        }
                        else
                        {
                            UseSkill(SkillLooping, SkillLoopingX, SkillLoopingY, 0);
                        }
                    }
                    else
                    {
                        SkillLooping = 0;
                    }
                }
                Ready = true;

            }
            catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); }
        }
        #endregion

        public void AddGemAtk()
        {
            Ready = false;
            MAtk *= AddMAtkPc;
            Ready = true;
        }
        public void RemoveGemAtk()
        {
            Ready = false;
            MAtk /= AddMAtkPc;
            Ready = true;
        }

        public void GetEquipStats(byte Pos, bool UnEquip)
        {
            Ready = false;
            if (Equips[Pos] != "0" && Equips[Pos] != null)
            {
                string[] Splitter = Equips[Pos].Split('-');

                byte ItemPlus = byte.Parse(Splitter[1]);
                byte ItemBless = byte.Parse(Splitter[2]);
                byte ItemEnchant = byte.Parse(Splitter[3]);
                byte ItemGem1 = byte.Parse(Splitter[4]);
                byte ItemGem2 = byte.Parse(Splitter[5]);

                uint[] TheItem = Other.ItemInfo(uint.Parse(Splitter[0]));

                string PItemID = Splitter[0];
                uint ItemId = uint.Parse(Splitter[0]);

                if (Pos == 1 || Pos == 3)
                {
                    PItemID = PItemID.Remove(3, 1);
                    PItemID = PItemID.Insert(3, "0");
                    PItemID = PItemID.Remove(5, 1);
                    PItemID = PItemID.Insert(5, "0");
                }
                if (Pos == 2 || Pos == 6 || Pos == 8)
                {
                    PItemID = PItemID.Remove(5, 1);
                    PItemID = PItemID.Insert(5, "0");
                }
                if (Pos == 4 || Pos == 5)
                {
                    if (Other.ItemType(ItemId) == 4 && Other.WeaponType(ItemId) != 421)
                    {
                        PItemID = PItemID.Remove(5, 1);
                        PItemID = PItemID.Remove(0, 3);
                        PItemID = "444" + PItemID + "0";
                    }
                    if (Other.ItemType(ItemId) == 5 && Other.WeaponType(ItemId) != 500)
                    {
                        PItemID = PItemID.Remove(5, 1);
                        PItemID = PItemID.Remove(0, 3);
                        PItemID = "555" + PItemID + "0";
                    }
                    if (Other.WeaponType(ItemId) == 500 || Other.WeaponType(ItemId) == 421)
                    {
                        PItemID = PItemID.Remove(5, 1);
                        PItemID = PItemID.Insert(5, "0");
                    }
                    if (Other.WeaponType(ItemId) == 900)
                    {
                        PItemID = PItemID.Remove(3, 1);
                        PItemID = PItemID.Insert(3, "0");
                        PItemID = PItemID.Remove(5, 1);
                        PItemID = PItemID.Insert(5, "0");
                    }
                }


                string[] PItem = new string[10] { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" };

                foreach (string[] Item in ExternalDatabase.DBPlusInfo)
                {
                    if (PItemID == Item[0])
                        if (ItemPlus == byte.Parse(Item[1]))
                        {
                            PItem = Item;
                            break;
                        }
                }
                ushort ExtraHP = 0;
                ushort ExtraAtk = 0;

                if (ItemId == 137310)
                    ExtraHP = 30000;
                if (ItemId == 150000)
                    ExtraHP = 800;

                ushort AddDef = (ushort)(TheItem[10] + ushort.Parse(PItem[5]));
                ushort AddMDef = (ushort)(TheItem[11] + ushort.Parse(PItem[7]));
                ushort AddMinAtk = 0;
                ushort AddMaxAtk = 0;
                if (Pos != 5)
                {
                    AddMinAtk = (ushort)(TheItem[8] + uint.Parse(PItem[3]) + ExtraAtk);
                    AddMaxAtk = (ushort)(TheItem[9] + uint.Parse(PItem[4]) + ExtraAtk);
                }
                else
                {
                    AddMinAtk = (ushort)((TheItem[8] + uint.Parse(PItem[3]) + ExtraAtk) / 2);
                    AddMaxAtk = (ushort)((TheItem[9] + uint.Parse(PItem[4]) + ExtraAtk) / 2);
                }
                ushort AddMAtk = (ushort)(TheItem[12] + ushort.Parse(PItem[6]));
                ushort AddHP = (ushort)(ItemEnchant + ExtraHP + ushort.Parse(PItem[2]));
                ushort AddDex = (ushort)(TheItem[14] + ushort.Parse(PItem[8]));
                byte AddDodge = (byte)(TheItem[13] + byte.Parse(PItem[9]));

                ushort SockPot = 0;
                ushort QualityPot = 0;

                if (Other.ItemQuality(ItemId) == 9)
                    QualityPot = 4;
                if (Other.ItemQuality(ItemId) == 8)
                    QualityPot = 3;
                if (Other.ItemQuality(ItemId) == 7)
                    QualityPot = 2;
                if (Other.ItemQuality(ItemId) == 6)
                    QualityPot = 1;

                if (ItemGem1 != 0)
                    SockPot++;
                if (Other.ItemQuality((uint)ItemGem1) == 3)
                    SockPot++;
                if (ItemGem2 != 0)
                    SockPot++;
                if (Other.ItemQuality((uint)ItemGem1) == 3)
                    SockPot++;

                ushort AddPotency = (ushort)(SockPot + ItemPlus + QualityPot);

                if (!UnEquip)
                {
                    Potency += AddPotency;
                    Defense += AddDef;
                    MDefense += AddMDef;
                    MinAtk += AddMinAtk;
                    MaxAtk += AddMaxAtk;
                    MAtk += AddMAtk;
                    MaxHP += AddHP;
                    RealAgi += AddDex;
                    Dodge += AddDodge;

                    if (ItemGem1 == 1)
                        AddMAtkPc += 0.05;
                    if (ItemGem2 == 1)
                        AddMAtkPc += 0.05;
                    if (ItemGem1 == 2)
                        AddMAtkPc += 0.1;
                    if (ItemGem2 == 2)
                        AddMAtkPc += 0.1;
                    if (ItemGem1 == 3)
                        AddMAtkPc += 0.15;
                    if (ItemGem2 == 3)
                        AddMAtkPc += 0.15;

                    if (ItemGem1 == 11)
                        AddAtkPc += 0.05;
                    if (ItemGem2 == 11)
                        AddAtkPc += 0.05;
                    if (ItemGem1 == 12)
                        AddAtkPc += 0.1;
                    if (ItemGem2 == 12)
                        AddAtkPc += 0.1;
                    if (ItemGem1 == 13)
                        AddAtkPc += 0.15;
                    if (ItemGem2 == 13)
                        AddAtkPc += 0.15;

                    if (ItemGem1 == 21)
                        AddExpPc += 0.1;
                    if (ItemGem2 == 21)
                        AddExpPc += 0.1;
                    if (ItemGem1 == 22)
                        AddExpPc += 0.15;
                    if (ItemGem2 == 22)
                        AddExpPc += 0.15;
                    if (ItemGem1 == 23)
                        AddExpPc += 0.25;
                    if (ItemGem2 == 23)
                        AddExpPc += 0.25;

                    if (ItemGem1 == 51)
                        AddProfPc += 0.3;
                    if (ItemGem2 == 51)
                        AddProfPc += 0.3;
                    if (ItemGem1 == 52)
                        AddProfPc += 0.5;
                    if (ItemGem2 == 52)
                        AddProfPc += 0.5;
                    if (ItemGem1 == 53)
                        AddProfPc += 1;
                    if (ItemGem2 == 53)
                        AddProfPc += 1;
                }
                else
                {
                    Potency -= AddPotency;
                    Defense -= AddDef;
                    MDefense -= AddMDef;
                    MinAtk -= AddMinAtk;
                    MaxAtk -= AddMaxAtk;
                    MAtk -= AddMAtk;
                    MaxHP -= AddHP;
                    RealAgi -= AddDex;
                    Dodge -= AddDodge;

                    if (ItemGem1 == 1)
                        AddMAtkPc -= 0.05;
                    if (ItemGem2 == 1)
                        AddMAtkPc -= 0.05;
                    if (ItemGem1 == 2)
                        AddMAtkPc -= 0.1;
                    if (ItemGem2 == 2)
                        AddMAtkPc -= 0.1;
                    if (ItemGem1 == 3)
                        AddMAtkPc -= 0.15;
                    if (ItemGem2 == 3)
                        AddMAtkPc -= 0.15;

                    if (ItemGem1 == 11)
                        AddAtkPc -= 0.05;
                    if (ItemGem2 == 11)
                        AddAtkPc -= 0.05;
                    if (ItemGem1 == 12)
                        AddAtkPc -= 0.1;
                    if (ItemGem2 == 12)
                        AddAtkPc -= 0.1;
                    if (ItemGem1 == 13)
                        AddAtkPc -= 0.15;
                    if (ItemGem2 == 13)
                        AddAtkPc -= 0.15;

                    if (ItemGem1 == 21)
                        AddExpPc -= 0.1;
                    if (ItemGem2 == 21)
                        AddExpPc -= 0.1;
                    if (ItemGem1 == 22)
                        AddExpPc -= 0.15;
                    if (ItemGem2 == 22)
                        AddExpPc -= 0.15;
                    if (ItemGem1 == 23)
                        AddExpPc -= 0.25;
                    if (ItemGem2 == 23)
                        AddExpPc -= 0.25;

                    if (ItemGem1 == 51)
                        AddProfPc -= 0.3;
                    if (ItemGem2 == 51)
                        AddProfPc -= 0.3;
                    if (ItemGem1 == 52)
                        AddProfPc -= 0.5;
                    if (ItemGem2 == 52)
                        AddProfPc -= 0.5;
                    if (ItemGem1 == 53)
                        AddProfPc -= 1;
                    if (ItemGem2 == 53)
                        AddProfPc -= 1;
                }
            }
            Ready = true;
        }

        public void UnEquip(byte From)
        {
            if (ItemsInInventory > 40)
                return;

            Ready = false;
            try
            {
                GetEquipStats(From, true);
                MyClient.SendPacket(General.MyPackets.RemoveItem((long)Equips_UIDs[From], From, 6));
                AddItem(Equips[From], 0, Equips_UIDs[From]);
                Equips[From] = null;
                Equips_UIDs[From] = 0;
            }
            catch (Exception Exc)
            {
                General.WriteLine(Convert.ToString(Exc));
            }
            Ready = true;
        }

        public void UseItem(ulong ItemUID, string Item)
        {
            Ready = false;
            string[] ItemParts = Item.Split('-');
            if (ItemParts[0] == "720027")
            {
                if (ItemsInInventory <= 30)
                {
                    AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "601219")//Katana
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601239")//Katana
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601009")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601219")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601019")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601029")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601039")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601049")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601059")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601069")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601079")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601089")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601099")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601109")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601119")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601129")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601139")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601149")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601159")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601169")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601179")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601189")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601199")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601209")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601219")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601229")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601239")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601249")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601259")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601269")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601279")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601289")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601299")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601309")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601319")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601329")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "601339")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "135009")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "135019")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "135029")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "135039")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "135049")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "135059")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "135069")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "135079")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "135089")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "135099")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "135109")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "112009")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "112019")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "112029")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "112039")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "112049")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "112059")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "112069")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "112079")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "112089")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "112099")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "112100")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "112109")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "202009")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "201009")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "900109")
            {
                if (ItemsInInventory < 40)
                {
                    if (Equips[4] == null && Equips[5] == null)
                    {
                        AddItem(Item, 4, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] == null)
                    {
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                    else if (Equips[4] != null && Equips[5] != null)
                    {
                        UnEquip(5);
                        AddItem(Item, 5, (uint)ItemUID);
                        World.UpdateSpawn(this);
                        RemoveItem(ItemUID);
                    }
                }
            }
            else if (ItemParts[0] == "720028")
            {
                if (ItemsInInventory <= 30)
                {
                    AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "1088002")
            {
                if (ItemsInInventory <= 39)
                {
                    AddItem("1088001-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "723711")
            {
                if (ItemsInInventory <= 30)
                {
                    AddItem("1088002-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088002-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088002-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088002-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088002-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088002-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088002-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088002-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088002-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("1088002-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "722312")//blesskey
            {
                if (ItemsInInventory <= 40)
                {
                    Silvers += 100000000;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 4, Silvers));
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "722313")//beliefkey
            {
                if (ItemsInInventory <= 40)
                {
                    Silvers += 200000000;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 4, Silvers));
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "722019")//bonekey
            {
                if (ItemsInInventory <= 40)
                {
                    Silvers += 300000000;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 4, Silvers));
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "723712")//+1 stone pack
            {
                if (ItemsInInventory <= 35)
                {
                    AddItem("730001-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("730001-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("730001-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("730001-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("730001-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "721160")//MoonCake1(Refine Gems)
            {
                if (ItemsInInventory <= 33)
                {
                    AddItem("700012-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("700002-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("700022-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("700032-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("700042-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("700052-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("700062-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "721163")//MoonCake2 (Super Gems)
            {
                if (ItemsInInventory <= 33)
                {
                    AddItem("700013-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("700003-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("700023-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("700033-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("700043-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("700053-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    AddItem("700063-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                    RemoveItem(ItemUID);
                }
            }
                else if (ItemParts[0] == "723713")//Class1 MoneyBag
                {
                    Silvers += 300000;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 4, Silvers));
                    RemoveItem(ItemUID);
                }
                else if (ItemParts[0] == "723714")//Class2 MoneyBag
                {
                    Silvers += 800000;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 4, Silvers));
                    RemoveItem(ItemUID);
                }
                else if (ItemParts[0] == "723715")//Class3 MoneyBag
                {
                    Silvers += 1200000;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 4, Silvers));
                    RemoveItem(ItemUID);
                }                
                else if (ItemParts[0] == "723716")//Class4 MoneyBag
                {
                    Silvers += 1800000;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 4, Silvers));
                    RemoveItem(ItemUID);
                }
                else if (ItemParts[0] == "723717")//Class5 MoneyBag
                {
                    Silvers += 5000000;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 4, Silvers));
                    RemoveItem(ItemUID);
                }
                else if (ItemParts[0] == "723718")//Class6 MoneyBag
                {
                    Silvers += 20000000;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 4, Silvers));
                    RemoveItem(ItemUID);
                }
                else if (ItemParts[0] == "723719")//Class7 MoneyBag
                {
                    Silvers += 25000000;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 4, Silvers));
                    RemoveItem(ItemUID);
                }
                else if (ItemParts[0] == "723720")//Class8 MoneyBag
                {
                    Silvers += 80000000;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 4, Silvers));
                    RemoveItem(ItemUID);
                }
                else if (ItemParts[0] == "723721")//Class9 MoneyBag
                {
                    Silvers += 100000000;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 4, Silvers));
                    RemoveItem(ItemUID);
                }
                else if (ItemParts[0] == "723722")//Class10 MoneyBag
                {
                    Silvers += 300000000;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 4, Silvers));
                    RemoveItem(ItemUID);
                }
                else if (ItemParts[0] == "723723")//Top MoneyBag
                {
                    Silvers += 500000000;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 4, Silvers));
                    RemoveItem(ItemUID);
                }
                else if (ItemParts[0] == "729910")//CPMiniBag
                {
                    CPs += 10000;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 3, CPs));
                    RemoveItem(ItemUID);
                    MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "SYSTEM", Name, "You got 10000 cps from the CPMiniBag, This will not be showed go to market and u have them", 2005));
                }
                else if (ItemParts[0] == "729911")//CPBag
                {
                    CPs += 30000;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 3, CPs));
                    RemoveItem(ItemUID);
                    MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "SYSTEM", Name, "You got 30000 cps from the CPBag, This will not be showed go to market and u have them", 2005));
                }
                else if (ItemParts[0] == "729912")//CPBackpack
                {
                    CPs += 50000;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 3, CPs));
                    RemoveItem(ItemUID);
                    MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "SYSTEM", Name, "You got 50000 cps from the CPBackpack, This will not be showed go to market and u have them", 2005));
                }
                else if (ItemParts[0] == "720028")//DBScroll
                {
                    if (ItemsInInventory <= 30)
                    {
                        AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                        AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                        AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                        AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                        AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                        AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                        AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                        AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                        AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                        AddItem("1088000-0-0-0-0-0", 0, (uint)General.Rand.Next(346623472));
                        RemoveItem(ItemUID);
                    }
                }            
                else if (ItemParts[0] == "723700")
            {
                if (Level < 100)
                    AddExp((ulong)(1295000 + Level * 100000), false);
                else if (Level < 110)
                    AddExp((ulong)(1395000 + Level * 160000), false);
                else if (Level < 115)
                    AddExp((ulong)(1595000 + Level * 200000), false);
                else if (Level < 120)
                    AddExp((ulong)(1895000 + Level * 240000), false);
                else if (Level < 125)
                    AddExp((ulong)(2095000 + Level * 300000), false);
                else if (Level < 130)
                    AddExp((ulong)(2395000 + Level * 360000), false);
                else if (Level < 135)
                    AddExp((ulong)(2895000 + Level * 400000), false);

                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "723017")//ExpPot
            {
                if (!EPotRate)
                {
                    EPotXP = 3600;
                    EPotXP2= EPotXP * 2;
                    EPotRate = true;
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 19, EPotXP));
                    MyClient.SendPacket(General.MyPackets.Vital(UID, 26, GetStat()));
                    World.UpdateSpawn(this);
                    RemoveItem(ItemUID);
                }
                else
                {
                    MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "SYSTEM", Name, "You still have doubleexp. Its a waste to use another one!", 2005));
                }
            }
            else if (ItemParts[0] == "1000000")//Stancher
            {
                CurHP += 20;
                if (CurHP > MaxHP)
                    CurHP = MaxHP;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "1000010")
            {
                CurHP += 100;
                if (CurHP > MaxHP)
                    CurHP = MaxHP;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "1000020")//Resolutive
            {
                CurHP += 250;
                if (CurHP > MaxHP)
                    CurHP = MaxHP;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "1000030")//Amrita
            {
                CurHP += 500;
                if (CurHP > MaxHP)
                    CurHP = MaxHP;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "1002000")//Panacea
            {
                CurHP += 800;
                if (CurHP > MaxHP)
                    CurHP = MaxHP;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "1002010")//Ginseng
            {
                CurHP += 1200;
                if (CurHP > MaxHP)
                    CurHP = MaxHP;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "1002020")//Vanilla
            {
                CurHP += 2000;
                if (CurHP > MaxHP)
                    CurHP = MaxHP;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "1002050")//Mil.Ginseng
            {
                CurHP += 3000;
                if (CurHP > MaxHP)
                    CurHP = MaxHP;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "720010")//Amrita
            {
                CurHP += 500;
                if (CurHP > MaxHP)
                    CurHP = MaxHP;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "720011")
            {
                CurHP += 800;
                if (CurHP > MaxHP)
                    CurHP = MaxHP;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "720012")
            {
                CurHP += 1200;
                if (CurHP > MaxHP)
                    CurHP = MaxHP;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "720013")
            {
                CurHP += 2000;
                if (CurHP > MaxHP)
                    CurHP = MaxHP;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "1060020")
            {
                if (LocMap == 6000 && PKPoints > 29)
                    return;
                Teleport(1002, 429, 378);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "1060021")
            {
                if (LocMap == 6000 && PKPoints > 29)
                    return;
                Teleport(1000, 500, 650);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "1060022")
            {
                if (LocMap == 6000 && PKPoints > 29)
                    return;
                Teleport(1020, 565, 562);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "1060023")
            {
                if (LocMap == 6000 && PKPoints > 29)
                    return;
                Teleport(1011, 188, 264);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "1060024")
            {
                if (LocMap == 6000 && PKPoints > 29)
                    return;
                Teleport(1015, 717, 571);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "1060030")
            {
                Hair = ushort.Parse("3" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                MyClient.SendPacket(General.MyPackets.Vital(UID, 27, Hair));
                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));
                RemoveItem(ItemUID);
                World.UpdateSpawn(this);
            }
            else if (ItemParts[0] == "1060040")
            {
                Hair = ushort.Parse("9" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                MyClient.SendPacket(General.MyPackets.Vital(UID, 27, Hair));
                MyClient.SendPacket(General.MyPackets.Vital(UID, 0, CurHP));
                RemoveItem(ItemUID);
                World.UpdateSpawn(this);
            }
            else if (ItemParts[0] == "1060050")
            {
                Hair = ushort.Parse("8" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                MyClient.SendPacket(General.MyPackets.Vital(UID, 27, Hair));
                RemoveItem(ItemUID);
                World.UpdateSpawn(this);
            }
            else if (ItemParts[0] == "1060060")
            {
                Hair = ushort.Parse("7" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                MyClient.SendPacket(General.MyPackets.Vital(UID, 27, Hair));
                RemoveItem(ItemUID);
                World.UpdateSpawn(this);
            }
            else if (ItemParts[0] == "1060070")
            {
                Hair = ushort.Parse("6" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                MyClient.SendPacket(General.MyPackets.Vital(UID, 27, Hair));
                RemoveItem(ItemUID);
                World.UpdateSpawn(this);
            }
            else if (ItemParts[0] == "1060080")
            {
                Hair = ushort.Parse("5" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                MyClient.SendPacket(General.MyPackets.Vital(UID, 27, Hair));
                RemoveItem(ItemUID);
                World.UpdateSpawn(this);
            }
            else if (ItemParts[0] == "1060090")
            {
                Hair = ushort.Parse("4" + Convert.ToString(Hair)[1] + Convert.ToString(Hair)[2]);
                MyClient.SendPacket(General.MyPackets.Vital(UID, 27, Hair));
                RemoveItem(ItemUID);
                World.UpdateSpawn(this);
            }
            else if (ItemParts[0] == "721533")
            {
                CPs += 3;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 30, CPs));
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "721536")
            {
                CPs += 5;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 30, CPs));
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "723038")
            {
                CPs += 10;
                MyClient.SendPacket(General.MyPackets.Vital(UID, 30, CPs));
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "723584")
            {
                string[] item = Equips[3].Split('-');
                string newitem = item[0];
                newitem = newitem.Remove(newitem.Length - 3, 1);
                newitem = newitem.Insert(newitem.Length - 2, "2");
                Equips[3] = newitem + "-" + item[1] + "-" + item[2] + "-" + item[3] + "-" + item[4] + "-" + item[5];
                MyClient.SendPacket(General.MyPackets.AddItem((long)Equips_UIDs[3], int.Parse(newitem), byte.Parse(item[1]), byte.Parse(item[2]), byte.Parse(item[3]), byte.Parse(item[4]), byte.Parse(item[5]), 3, 100, 100));
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "1060101")
            {
                if (Job >= 143)
                {
                    LearnSkill(1165, 0);
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "1060100")
            {
                if (Job >= 143)
                {
                    LearnSkill(1160, 0);
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "725025")
            {
                if (Job < 26 && Job > 19 && Level >= 40)
                {
                    LearnSkill(1320, 0);
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "725026")
            {
                LearnSkill(5010, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725027")
            {
                LearnSkill(5020, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725028")
            {
                if (Job > 130 && Level >= 70)
                {
                    LearnSkill(5001, 0);
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "725029")
            {
                LearnSkill(5030, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725030")
            {
                LearnSkill(5040, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725031")
            {
                LearnSkill(5050, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725040")
            {
                LearnSkill(7000, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725041")
            {
                LearnSkill(7010, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725042")
            {
                LearnSkill(7020, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725043")
            {
                LearnSkill(7030, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725044")
            {
                LearnSkill(7040, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725018")
            {
                LearnSkill(1380, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725019")
            {
                LearnSkill(1385, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725020")
            {
                LearnSkill(1390, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725021")
            {
                LearnSkill(1395, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725022")
            {
                LearnSkill(1400, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725023")
            {
                LearnSkill(1405, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725024")
            {
                LearnSkill(1410, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725000")
            {
                if (Spi >= 20)
                {
                    LearnSkill(1000, 0);
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "725001")
            {
                if (Spi >= 80)
                {
                    LearnSkill(1001, 0);
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "725002")
            {
                if (Spi >= 160)
                {
                    LearnSkill(1002, 0);
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "725003")
            {
                if (Spi >= 30)
                {
                    LearnSkill(1005, 0);
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "725004")
            {
                if (Spi >= 25)
                {
                    LearnSkill(1010, 0);
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "725005")
            {
                LearnSkill(1045, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725010")
            {
                LearnSkill(1046, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725011")
            {
                LearnSkill(1250, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725012")
            {
                LearnSkill(1260, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725013")
            {
                LearnSkill(1290, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725014")
            {
                LearnSkill(1300, 0);
                RemoveItem(ItemUID);
            }
            else if (ItemParts[0] == "725015")
            {
                if (Job > 129 && Job < 136)
                {
                    LearnSkill(1350, 0);
                    RemoveItem(ItemUID);
                }
            }
            else if (ItemParts[0] == "725016")
            {
                LearnSkill(1360, 0);
                RemoveItem(ItemUID);
            }
            else
            {
                MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "SYSTEM", Name, "This item's use is not implemented yet.", 2005));
            }
            Ready = true;
        }

        public void RemoveItem(ulong UID)
        {
            Ready = false;
            int count = 0;
            foreach (ulong uid in Inventory_UIDs)
            {
                count++;
                if (UID == uid)
                {
                    count--;
                    byte Do = (byte)(ItemsInInventory - count);
                    for (int p = 0; p < Do; p++)
                    {
                        Inventory[count + p] = Inventory[count + p + 1];
                        Inventory_UIDs[count + p] = Inventory_UIDs[count + p + 1];
                    }
                    Inventory[ItemsInInventory] = null;
                    Inventory_UIDs[ItemsInInventory] = 0;
                    ItemsInInventory -= 1;

                    MyClient.SendPacket(General.MyPackets.RemoveItem((long)UID, 0, 3));
                    break;
                }
            }
            Ready = true;
        }
        public void EPotTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
              EPot = false;
              MyClient.SendPacket(General.MyPackets.SendMsg(MyClient.MessageId, "SYSTEM", Name, "Your exppot is over, please buy a new one to refill!", 2000));
        }  
        public void LearnSkill(short SkillId, byte SkillLvl)
        {
            Ready = false;

            if (!Skills.Contains((short)SkillId))
            {
                Skills.Add((short)SkillId, SkillLvl);
                Skill_Exps.Add((short)SkillId, SkillExpNull);
            }

            MyClient.SendPacket(General.MyPackets.LearnSkill(SkillId, SkillLvl, 0));
            Ready = true;
        }

        public void LearnSkill2(short SkillId, byte SkillLvl)
        {
            Ready = false;

            if (!Skills.Contains((short)SkillId))
            {
                Skills.Add((short)SkillId, SkillLvl);
                Skill_Exps.Add((short)SkillId, SkillExpNull);
            }
            else
            {
                Skills.Remove((short)SkillId);
                Skill_Exps.Remove((short)SkillId);
                Skills.Add((short)SkillId, SkillLvl);
                Skill_Exps.Add((short)SkillId, SkillExpNull);
            }

            MyClient.SendPacket(General.MyPackets.LearnSkill(SkillId, SkillLvl, 0));
            Ready = true;
        }


        public void PackProfs()
        {
            if (!MyClient.There)
                return;
            Ready = false;
            IDictionaryEnumerator IE = Profs.GetEnumerator();
            PackedProfs = "";

            while (IE.MoveNext())
            {
                short prof_id = (short)IE.Key;
                byte prof_lv = (byte)IE.Value;

                PackedProfs += Convert.ToString(prof_id) + ":" + Convert.ToString(prof_lv) + ":" + Convert.ToString(Prof_Exps[prof_id]) + "~";
            }

            if (PackedProfs.Length > 0)
            {
                PackedProfs = PackedProfs.Remove(PackedProfs.Length - 1, 1);
            }
            Ready = true;
        }

        public void UnPackProfs()
        {
            Ready = false;
            if (PackedProfs.Length == 0)
                return;

            string[] profs = PackedProfs.Split('~');

            foreach (string prof in profs)
            {
                string[] ThisProf = prof.Split(':');

                Profs.Add(short.Parse(ThisProf[0]), byte.Parse(ThisProf[1]));
                Prof_Exps.Add(short.Parse(ThisProf[0]), uint.Parse(ThisProf[2]));
            }
            Ready = true;
        }

        public void SendProfs()
        {
            Ready = false;
            IDictionaryEnumerator IE = Profs.GetEnumerator();

            while (IE.MoveNext())
            {
                short prof_id = (short)IE.Key;
                byte prof_lvl = (byte)IE.Value;
                uint prof_exp = (uint)Prof_Exps[prof_id];

                MyClient.SendPacket(General.MyPackets.Prof(prof_id, prof_lvl, prof_exp));
            }
            Ready = true;
        }

        public void PackSkills()
        {
            if (!MyClient.There)
                return;
            Ready = false;
            IDictionaryEnumerator IE = Skills.GetEnumerator();
            PackedSkills = "";

            while (IE.MoveNext())
            {
                short skill_id = (short)IE.Key;
                byte skill_lv = (byte)IE.Value;

                PackedSkills += Convert.ToString(skill_id) + ":" + Convert.ToString(skill_lv) + ":" + Convert.ToString(Skill_Exps[skill_id]) + "~";
            }

            if (PackedSkills.Length > 0)
            {
                PackedSkills = PackedSkills.Remove(PackedSkills.Length - 1, 1);
            }
            Ready = true;
        }


        public void UnPackSkills()
        {
            Ready = false;
            if (PackedSkills.Length == 0)
                return;

            string[] skills = PackedSkills.Split('~');

            foreach (string skill in skills)
            {
                string[] ThisSkill = skill.Split(':');

                Skills.Add(short.Parse(ThisSkill[0]), byte.Parse(ThisSkill[1]));
                Skill_Exps.Add(short.Parse(ThisSkill[0]), uint.Parse(ThisSkill[2]));
            }
            Ready = true;
        }

        public void SendSkills()
        {
            Ready = false;
            IDictionaryEnumerator IE = Skills.GetEnumerator();

            while (IE.MoveNext())
            {
                short skill_id = (short)IE.Key;
                byte skill_lvl = (byte)IE.Value;
                uint skill_exp = (uint)Skill_Exps[skill_id];

                MyClient.SendPacket(General.MyPackets.LearnSkill(skill_id, skill_lvl, skill_exp));
            }
            Ready = true;
        }

        public ushort BaseMaxHP()
        {
            Ready = false;
            double hp = Vit * 24 + Str * 3 + Agi * 3 + Spi * 3;
            if (Job == 11)
                hp *= 1.05;
            if (Job == 12)
                hp *= 1.08;
            if (Job == 13)
                hp *= 1.1;
            if (Job == 14)
                hp *= 1.12;
            if (Job == 15)
                hp *= 1.15;
            Ready = true;
            return (ushort)hp;
        }

        public void RemoveWHItem(ulong UID)
        {
            Ready = false;
            int count = 0;
            int whcount = 0;

            foreach (uint[] wh in WHIDs)
            {
                count = 0;

                foreach (uint uid in wh)
                {
                    if (uid == UID)
                    {
                        if (whcount == 0)
                        {
                            byte Do = (byte)(TCWHCount - count - 1);
                            for (int p = 0; p < Do; p++)
                            {
                                TCWH[count + p] = TCWH[count + p + 1];
                                WHIDs[whcount][count + p] = WHIDs[whcount][count + p + 1];
                            }
                            TCWH[TCWHCount - 1] = null;
                            WHIDs[whcount][TCWHCount - 1] = 0;
                            TCWHCount -= 1;
                        }
                        else if (whcount == 1)
                        {
                            byte Do = (byte)(PCWHCount - count - 1);
                            for (int p = 0; p < Do; p++)
                            {
                                PCWH[count + p] = PCWH[count + p + 1];
                                WHIDs[whcount][count + p] = WHIDs[whcount][count + p + 1];
                            }
                            PCWH[PCWHCount - 1] = null;
                            WHIDs[whcount][PCWHCount - 1] = 0;
                            PCWHCount -= 1;
                        }
                        else if (whcount == 2)
                        {
                            byte Do = (byte)(ACWHCount - count - 1);
                            for (int p = 0; p < Do; p++)
                            {
                                ACWH[count + p] = ACWH[count + p + 1];
                                WHIDs[whcount][count + p] = WHIDs[whcount][count + p + 1];
                            }
                            ACWH[ACWHCount - 1] = null;
                            WHIDs[whcount][ACWHCount - 1] = 0;
                            ACWHCount -= 1;
                        }
                        else if (whcount == 3)
                        {
                            byte Do = (byte)(DCWHCount - count - 1);
                            for (int p = 0; p < Do; p++)
                            {
                                DCWH[count + p] = DCWH[count + p + 1];
                                WHIDs[whcount][count + p] = WHIDs[whcount][count + p + 1];
                            }
                            DCWH[DCWHCount - 1] = null;
                            WHIDs[whcount][DCWHCount - 1] = 0;
                            DCWHCount -= 1;
                        }
                        else if (whcount == 4)
                        {
                            byte Do = (byte)(BIWHCount - count - 1);
                            for (int p = 0; p < Do; p++)
                            {
                                BIWH[count + p] = BIWH[count + p + 1];
                                WHIDs[whcount][count + p] = WHIDs[whcount][count + p + 1];
                            }
                            BIWH[BIWHCount - 1] = null;
                            WHIDs[whcount][BIWHCount - 1] = 0;
                            BIWHCount -= 1;
                        }
                        else if (whcount == 5)
                        {
                            byte Do = (byte)(MAWHCount - count - 1);
                            for (int p = 0; p < Do; p++)
                            {
                                MAWH[count + p] = MAWH[count + p + 1];
                                WHIDs[whcount][count + p] = WHIDs[whcount][count + p + 1];
                            }
                            MAWH[MAWHCount - 1] = null;
                            WHIDs[whcount][MAWHCount - 1] = 0;
                            MAWHCount -= 1;
                        }
                    }
                    count++;
                }
                whcount++;
            }


            Ready = true;
        }

        public void AddWHItem(string ItemInf, uint UID, byte WHID)
        {
            if (WHID == 0)
            {
                WHIDs[WHID][TCWHCount] = UID;
                TCWH[TCWHCount] = ItemInf;
                TCWHCount++;
            }
            else if (WHID == 1)
            {
                WHIDs[WHID][PCWHCount] = UID;
                PCWH[PCWHCount] = ItemInf;
                PCWHCount++;
            }
            else if (WHID == 2)
            {
                WHIDs[WHID][ACWHCount] = UID;
                ACWH[ACWHCount] = ItemInf;
                ACWHCount++;
            }
            else if (WHID == 3)
            {
                WHIDs[WHID][DCWHCount] = UID;
                DCWH[DCWHCount] = ItemInf;
                DCWHCount++;
            }
            else if (WHID == 4)
            {
                WHIDs[WHID][BIWHCount] = UID;
                BIWH[BIWHCount] = ItemInf;
                BIWHCount++;
            }
            else if (WHID == 5)
            {
                WHIDs[WHID][MAWHCount] = UID;
                MAWH[MAWHCount] = ItemInf;
                MAWHCount++;
            }
        }

        public void AddItem(string ItemInfo, byte ToPos, uint UID)
        {
            Ready = false;
            string[] Splitter = ItemInfo.Split('-');

            int ItemId = 0;
            byte Plus = 0;
            byte Bless = 0;
            byte Enchant = 0;
            byte Soc1 = 0;
            byte Soc2 = 0;

            ItemId = int.Parse(Splitter[0]);
            Plus = byte.Parse(Splitter[1]);
            Bless = byte.Parse(Splitter[2]);
            Enchant = byte.Parse(Splitter[3]);
            Soc1 = byte.Parse(Splitter[4]);
            Soc2 = byte.Parse(Splitter[5]);

            if (ToPos == 0)
            {
                Inventory[ItemsInInventory] = ItemInfo;
                Inventory_UIDs[ItemsInInventory] = UID;
                ItemsInInventory++;
            }
            else if (ToPos < 10)
            {
                Equips[ToPos] = ItemInfo;
                Equips_UIDs[ToPos] = UID;
                GetEquipStats(ToPos, false);
            }

            MyClient.SendPacket(General.MyPackets.AddItem((long)UID, ItemId, Plus, Bless, Enchant, Soc1, Soc2, ToPos, 100, 100));
            Ready = true;
        }

        public void SendInventory()
        {
            Ready = false;
            string[] Splitter;

            int ItemId;
            byte Plus = 0;
            byte Bless = 0;
            byte Enchant = 0;
            byte Soc1 = 0;
            byte Soc2 = 0;

            int count = 0;
            foreach (string item in Inventory)
            {
                if (item != null)
                {
                    Splitter = item.Split('-');

                    ItemId = int.Parse(Splitter[0]);
                    Plus = byte.Parse(Splitter[1]);
                    Bless = byte.Parse(Splitter[2]);
                    Enchant = byte.Parse(Splitter[3]);
                    Soc1 = byte.Parse(Splitter[4]);
                    Soc2 = byte.Parse(Splitter[5]);

                    MyClient.SendPacket(General.MyPackets.AddItem((long)Inventory_UIDs[count], ItemId, Plus, Bless, Enchant, Soc1, Soc2, 0, 100, 100));
                }
                count++;
            }
            Ready = true;
        }
        public void SendEquips(bool GetStats)
        {
            Ready = false;
            string[] Splitter;

            int ItemId;
            byte Plus = 0;
            byte Bless = 0;
            byte Enchant = 0;
            byte Soc1 = 0;
            byte Soc2 = 0;

            int count = 0;
            foreach (string item in Equips)
            {
                if (item != null)
                {
                    Splitter = item.Split('-');

                    ItemId = int.Parse(Splitter[0]);
                    Plus = byte.Parse(Splitter[1]);
                    Bless = byte.Parse(Splitter[2]);
                    Enchant = byte.Parse(Splitter[3]);
                    Soc1 = byte.Parse(Splitter[4]);
                    Soc2 = byte.Parse(Splitter[5]);

                    MyClient.SendPacket(General.MyPackets.AddItem((long)Equips_UIDs[count], ItemId, Plus, Bless, Enchant, Soc1, Soc2, (byte)count, 100, 100));
                }
                count++;
            }
            if (GetStats)
            {
                GetEquipStats(1, false);
                GetEquipStats(2, false);
                GetEquipStats(3, false);
                GetEquipStats(4, false);
                GetEquipStats(5, false);
                GetEquipStats(6, false);
                GetEquipStats(8, false);
            }
            Ready = true;
        }

        public void PackInventory()
        {
            if (!MyClient.There)
                return;
            Ready = false;
            try
            {
                PackedInventory = "";
                int count = 0;
                foreach (string item in Inventory)
                {
                    if (item != null && item != "")
                    {
                        if (count == 0)
                            PackedInventory += item + "~";
                        else if (count > 0)
                            PackedInventory += "~" + item + "~";
                    }
                    else
                        break;

                }
                if (PackedInventory.Length > 1)
                    PackedInventory = PackedInventory.Remove(PackedInventory.Length - 1, 1);
            }
            catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); }
            Ready = true;
        }

        public void UnPackInventory()
        {
            Ready = false;
            if (PackedInventory.Length < 1)
                return;

            string[] Items = PackedInventory.Split('~');

            int count = 0;

            foreach (string item in Items)
            {
                if (item != null)
                    if (item.Length > 1)
                    {
                        AddItem(item, 0, (uint)General.Rand.Next(10000000));
                        count++;
                    }
                    else
                        break;
            }
            Ready = true;
        }

        public void UnPackEquips()
        {
            Ready = false;
            if (PackedEquips.Length < 1)
                return;

            string[] equips = PackedEquips.Split('~');

            int count = 0;

            foreach (string item in equips)
            {
                count++;
                if (item != null)
                    if (item.Length > 1)
                        if (item != "0")
                        {
                            Equips[count] = item;
                            Equips_UIDs[count] = (uint)General.Rand.Next(1000000);
                        }
            }
            Ready = true;
        }

        public void PackEquips()
        {
            if (!MyClient.There)
                return;
            Ready = false;
            PackedEquips = "";

            int count = 0;

            foreach (string item in Equips)
            {
                count++;
                if (item != null)
                {
                    PackedEquips += item + "~";
                }
                else if (count != 1)
                {
                    PackedEquips += "0~";
                }
            }
            if (PackedEquips.Length > 1)
                PackedEquips = PackedEquips.Remove(PackedEquips.Length - 1, 1);
            Ready = true;
        }

        public void UnPackFriends()
        {
            string[] Friendss = PackedFriends.Split('~');

            foreach (string friend in Friendss)
            {
                if (friend != null && friend.Length > 2)
                {
                    string[] Details = friend.Split(':');
                    Friends.Add(uint.Parse(Details[1]), Details[0]);
                    if (World.AllChars.Contains(uint.Parse(Details[1])))
                    {
                        Character Char = (Character)World.AllChars[uint.Parse(Details[1])];
                        if (Char.MyClient.Online)
                        {
                            MyClient.SendPacket(General.MyPackets.FriendEnemyPacket(uint.Parse(Details[1]), Details[0], 15, 1));
                            Char.MyClient.SendPacket(General.MyPackets.FriendEnemyPacket(UID, Name, 14, 0));
                            Char.MyClient.SendPacket(General.MyPackets.FriendEnemyPacket(UID, Name, 15, 1));
                            Char.MyClient.SendPacket(General.MyPackets.SendMsg(Char.MyClient.MessageId, "SYSTEM", Char.Name, "Your friend " + Name + " has logged on.", 2005));
                        }
                        else
                            MyClient.SendPacket(General.MyPackets.FriendEnemyPacket(uint.Parse(Details[1]), Details[0], 15, 0));
                    }
                    else
                        MyClient.SendPacket(General.MyPackets.FriendEnemyPacket(uint.Parse(Details[1]), Details[0], 15, 0));
                }
            }
        }

        public void PackFriends()
        {
            PackedFriends = "";

            foreach (DictionaryEntry DE in Friends)
            {
                PackedFriends += (string)DE.Value + ":" + (uint)DE.Key + "~";
            }
            if (PackedFriends.Length > 0)
                PackedFriends = PackedFriends.Remove(PackedFriends.Length - 1, 1);
        }

        public void UnPackEnemies()
        {
            string[] Enemiess = PackedEnemies.Split('~');

            foreach (string enemy in Enemiess)
            {
                if (enemy != null && enemy.Length > 2)
                {
                    string[] Details = enemy.Split(':');
                    Enemies.Add(uint.Parse(Details[1]), Details[0]);
                }
            }
        }

        public void PackEnemies()
        {
            PackedEnemies = "";

            foreach (DictionaryEntry DE in Enemies)
            {
                PackedEnemies += (uint)DE.Key + ":" + (string)DE.Value;
            }
            if (PackedEnemies.Length > 0)
                PackedEnemies = PackedEnemies.Remove(PackedEnemies.Length - 1, 1);
        }


        public void PackWarehouses()
        {
            if (!MyClient.There)
                return;

            PackedWHs = "";
            bool Last = false;

            foreach (string item in TCWH)
            {
                if (item != null && item != "")
                {
                    PackedWHs += item + "~";
                    Last = true;
                }
                else
                    break;
            }
            if (Last)
                PackedWHs = PackedWHs.Remove(PackedWHs.Length - 1, 1);
            PackedWHs += ":";

            Last = false;
            foreach (string item in PCWH)
            {
                if (item != null && item != "")
                {
                    PackedWHs += item + "~";
                    Last = true;
                }
                else
                    break;
            }
            if (Last)
                PackedWHs = PackedWHs.Remove(PackedWHs.Length - 1, 1);
            PackedWHs += ":";

            Last = false;
            foreach (string item in ACWH)
            {
                if (item != null && item != "")
                {
                    PackedWHs += item + "~";
                    Last = true;
                }
                else
                    break;
            }
            if (Last)
                PackedWHs = PackedWHs.Remove(PackedWHs.Length - 1, 1);
            PackedWHs += ":";

            Last = false;
            foreach (string item in DCWH)
            {
                if (item != null && item != "")
                {
                    PackedWHs += item + "~";
                    Last = true;
                }
                else
                    break;
            }
            if (Last)
                PackedWHs = PackedWHs.Remove(PackedWHs.Length - 1, 1);
            PackedWHs += ":";

            Last = false;
            foreach (string item in BIWH)
            {
                if (item != null && item != "")
                {
                    PackedWHs += item + "~";
                    Last = true;
                }
                else
                    break;
            }
            if (Last)
                PackedWHs = PackedWHs.Remove(PackedWHs.Length - 1, 1);
            PackedWHs += ":";

            Last = false;
            foreach (string item in MAWH)
            {
                if (item != null && item != "")
                {
                    PackedWHs += item + "~";
                    Last = true;
                }
                else
                    break;
            }
            if (Last)
                PackedWHs = PackedWHs.Remove(PackedWHs.Length - 1, 1);
        }

        public void UnPackWarehouses()
        {
            if (PackedWHs.Length < 1)
                return;

            Ready = false;
            string[] Warehouses = PackedWHs.Split(':');
            byte count = 0;

            foreach (string wh in Warehouses)
            {
                string[] Items = wh.Split('~');

                if (Items.Length < 1)
                    if (wh.Length > 1)
                        AddWHItem(wh, (uint)General.Rand.Next(300000), count);

                foreach (string item in Items)
                {
                    if (item != "")
                    {
                        AddWHItem(item, (uint)General.Rand.Next(300000), count);
                    }
                }
                count++;
            }

            Ready = true;
        }
        public static string[] ShuffleGuildScores()
        {
            string[] ret = new string[5];
            DictionaryEntry[] Vals = new DictionaryEntry[5];

            for (sbyte i = 0; i < 5; i++)
            {
                Vals[i] = new DictionaryEntry();
                Vals[i].Key = (ushort)0;
                Vals[i].Value = (int)0;
            }

            foreach (DictionaryEntry Score in World.GWScores)
            {
                sbyte Pos = -1;
                for (sbyte i = 0; i < 5; i++)
                {
                    if ((int)Score.Value > (int)Vals[i].Value)
                    {
                        Pos = i;
                        break;
                    }
                }
                if (Pos == -1)
                    continue;

                for (sbyte i = 4; i > Pos; i--)
                    Vals[i] = Vals[i - 1];

                Vals[Pos] = Score;
            }

            for (sbyte i = 0; i < 5; i++)
            {
                if ((ushort)Vals[i].Key == 0)
                {
                    ret[i] = "";
                    continue;
                }
                Guild eGuild = (Guild)Guilds.AllGuilds[(ushort)Vals[i].Key];
                ret[i] = "No  " + (i + 1).ToString() + ": " + eGuild.GuildName + "(" + (int)Vals[i].Value + ")";
            }

            return ret;
        }


        public void SendGuildWar()
        {
            string[] g = ShuffleGuildScores();
            byte C = 0;

            foreach (string t in g)
            {
                if (t != "")
                {
                    if (C == 0)
                        MyClient.SendPacket(General.MyPackets.SendMsg2(0, "SYSTEM", "ALLUSERS", t, true));
                    else
                        MyClient.SendPacket(General.MyPackets.SendMsg2(0, "SYSTEM", "ALLUSERS", t, false));
                }
                C++;
            }
        }

        public void Teleport(ushort map, ushort x, ushort y)
        {
            Attacking = false;
            PTarget = null;
            MobTarget = null;
            TGTarget = null;
            if (LocMap != 700 && LocMap != 1036)
                PrevMap = LocMap;
            Ready = false;
            World.RemoveEntity(this);
            LocMap = map;
            LocX = x;
            LocY = y;
            MyClient.SendPacket(General.MyPackets.GeneralData((long)UID, LocMap, LocX, LocY, 74));
            World.SpawnMeToOthers(this, false);
            World.SpawnOthersToMe(this, false);
            World.SurroundNPCs(this, false);
            World.SurroundMobs(this, false);

            if (LocMap == 1038)
                SendGuildWar();

            Ready = true;
        }
    }
}