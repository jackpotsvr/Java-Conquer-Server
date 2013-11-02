using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace COServer_Project
{
    public unsafe class Packets
    {
        public static byte[] MarriageMouse(uint CharUID)
        {
            byte[] Mouse = new byte[24];
            fixed (byte* Ptr = Mouse)
            {
                *(ushort*)(Ptr + 0) = 24;
                *(ushort*)(Ptr + 2) = 1010;
                *(uint*)(Ptr + 8) = CharUID;
                *(uint*)(Ptr + 12) = 1067;
                *(uint*)(Ptr + 22) = 116;
                return Mouse;
            }
        }
        public byte[] StallWindow(uint ID, Character C)
        {
            ushort PacketType = 0x3f2;
            byte[] Packet = new byte[24];
            fixed (byte* Ptr = Packet)
            {
                *((ushort*)(Ptr)) = (ushort)Packet.Length;
                *((ushort*)(Ptr + 2)) = (ushort)PacketType;
                *((uint*)(Ptr + 4)) = (uint)758292;
                *((uint*)(Ptr + 8)) = (uint)C.UID;
                *((uint*)(Ptr + 12)) = (uint)ID;
                *((ushort*)(Ptr + 16)) = (ushort)272;
                *((ushort*)(Ptr + 18)) = (ushort)198;
                *((ushort*)(Ptr + 20)) = (ushort)6;
                *((ushort*)(Ptr + 22)) = (ushort)110;
            }
            return Packet;
        }
        public byte[] GuildInfo(Guild TheGuild, Character Player)
        {
            ushort PacketType = 0x452;
            string[] Splitter = TheGuild.Creator.Split(':');

            byte[] Packet = new byte[40];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((ushort*)(p + 4)) = (ushort)TheGuild.GuildID;
                *((uint*)(p + 8)) = (uint)Player.GuildDonation;
                *((uint*)(p + 12)) = (uint)TheGuild.Fund;
                *((uint*)(p + 16)) = (uint)TheGuild.MembersCount;
                *(p + 20) = Player.GuildPosition;

                for (int i = 0; i < Splitter[0].Length; i++)
                {
                    *(p + 21 + i) = Convert.ToByte(Splitter[0][i]);
                }
            }

            return Packet;
        }
        public byte[] SendGuild(uint GuildID, byte Type)
        {
            ushort PacketType = 0x453;
            byte[] Packet = new byte[12];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *(p + 4) = Type;
                *((uint*)(p + 8)) = (uint)GuildID;
            }
            return Packet;
        }
        public byte[] GuildName(ushort ID, string Name)
        {
            ushort PacketType = 0x3f7;
            byte[] Packet = new byte[11 + Name.Length];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((ushort*)(p + 4)) = (ushort)ID;
                *(p + 8) = 3;
                *(p + 9) = 1;
                *(p + 10) = (byte)Name.Length;

                for (int i = 0; i < Name.Length; i++)
                {
                    *(p + 11 + i) = Convert.ToByte(Name[i]);
                }
            }
            return Packet;
        }
        public byte[] FriendEnemyInfoPacket(Character Char, byte Enemy)
        {
            ushort PacketType = 0x7f1;
            byte[] Packet = new byte[36];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)Char.UID;
                *((uint*)(p + 8)) = uint.Parse((Char.Avatar.ToString() + Char.Model.ToString()));
                *(p + 12) = Char.Level;
                *(p + 13) = Char.Job;
                *((ushort*)(p + 14)) = (ushort)Char.PKPoints;
                *(p + 36) = Enemy;
            }
            return Packet;
        }
        public byte[] FriendEnemyPacket(uint uid, string name, byte Mode, byte Online)
        {
            ushort PacketType = 0x3fb;
            byte[] Packet = new byte[36];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)uid;

                *(p + 8) = Mode;
                *(p + 9) = Online;
                *(p + 10) = 0;
                *(p + 11) = 0;
                *(p + 12) = 0;
                *(p + 13) = 0;
                *(p + 14) = 0;
                *(p + 15) = 0;
                *(p + 16) = 0;
                *(p + 17) = 0;
                *(p + 18) = 0;
                *(p + 19) = 0;

                for (int i = 0; i < name.Length; i++)
                {
                    *(p + 20 + i) = Convert.ToByte(name[i]);
                }
            }
            return Packet;
        }
        public byte[] TradeItem(uint ItemUID, string Item)
        {
            ushort PacketType = 0x3f0;
            byte[] Packet = new byte[32];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)ItemUID;

                string[] Splitter = Item.Split('-');

                *((uint*)(p + 8)) = uint.Parse(Splitter[0]);
                *(p + 12) = 1 & 0xff;
                *(p + 14) = 1 & 0xff;
                *(p + 16) = 2 & 0xff;
                *(p + 18) = 0xff;
                *(p + 19) = 0;
                *(p + 20) = 0;
                *(p + 21) = 0;
                *(p + 22) = 0;
                *(p + 23) = 0;
                *(p + 24) = byte.Parse(Splitter[4]);
                *(p + 25) = byte.Parse(Splitter[5]);
                *(p + 26) = 1;
                *(p + 27) = 2;
                *(p + 28) = byte.Parse(Splitter[1]);
                *(p + 29) = byte.Parse(Splitter[2]);
                *(p + 30) = byte.Parse(Splitter[3]);
            }
            return Packet;
        }
        public byte[] TradePacket(uint UID, byte Type)
        {
            ushort PacketType = 0x420;
            byte[] Packet = new byte[12];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)UID;
                *((uint*)(p + 8)) = (uint)Type;
            }
            return Packet;
        }
        public byte[] PlayerJoinsTeam(Character Player)
        {
            ushort PacketType = 0x402;
            uint Model = uint.Parse(Convert.ToString(Player.Avatar) + Convert.ToString(Player.Model));

            byte[] Packet = new byte[36];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *(p + 5) = 1;

                for (int i = 0; i < Player.Name.Length; i++)
                {
                    *(p + 8 + i) = Convert.ToByte(Player.Name[i]);
                }
                *((uint*)(p + 24)) = (uint)Player.UID;
                *((uint*)(p + 28)) = (uint)Model;
                *((ushort*)(p + 32)) = (ushort)Player.MaxHP;
                *((ushort*)(p + 34)) = (ushort)Player.CurHP;
            }
            return Packet;
        }
        public byte[] TeamPacket(uint CharID, byte Mode)
        {
            ushort PacketType = 0x3ff;
            byte[] Packet = new byte[12];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)Mode;
                *((uint*)(p + 8)) = (uint)CharID;
            }

            return Packet;
        }
        public byte[] WhItems(Character Player, byte WH, ushort NPCID)
        {
            byte Count = 0;
            if (WH == 0)
                Count = Player.TCWHCount;
            if (WH == 1)
                Count = Player.PCWHCount;
            if (WH == 2)
                Count = Player.ACWHCount;
            if (WH == 3)
                Count = Player.DCWHCount;
            if (WH == 4)
                Count = Player.BIWHCount;
            if (WH == 5)
                Count = Player.MAWHCount;

            string[] WareHouse;
            if (WH != 5)
                WareHouse = new string[] { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null };
            else
                WareHouse = new string[] { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null };
            if (WH == 0)
                WareHouse = Player.TCWH;
            if (WH == 1)
                WareHouse = Player.PCWH;
            if (WH == 2)
                WareHouse = Player.ACWH;
            if (WH == 3)
                WareHouse = Player.DCWH;
            if (WH == 4)
                WareHouse = Player.BIWH;
            if (WH == 5)
                WareHouse = Player.MAWH;

            ushort PacketType = 1102;
            byte[] Packet = new byte[16 + (20 * Count)];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)NPCID;
                *((uint*)(p + 12)) = (uint)Count;

                int count = 0;

                foreach (string item in WareHouse)
                {
                    if (item != null)
                    {
                        string[] Splitter = item.Split('-');

                        *((uint*)(p + +16 + count * 20)) = (uint)Player.WHIDs[WH][count];
                        *((uint*)(p + +20 + count * 20)) = uint.Parse(Splitter[0]);
                        *(p + 25 + count * 20) = byte.Parse(Splitter[4]);
                        *(p + 26 + count * 20) = byte.Parse(Splitter[5]);
                        *(p + 29 + count * 20) = byte.Parse(Splitter[1]);
                        *(p + 30 + count * 20) = byte.Parse(Splitter[2]);
                        *(p + 32 + count * 20) = byte.Parse(Splitter[3]);
                    }
                    count++;
                }
            }
            return Packet;
        }

        public byte[] OpenWarehouse(uint NPCID, uint Money)
        {
            ushort PacketType = 1009;
            byte[] Packet = new byte[20];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)NPCID;
                *((uint*)(p + 8)) = (uint)Money;
                *(p + 12) = (byte)(9 & 0xff);
            }

            return Packet;
        }

        public byte[] ETCPacket(Character Char, ushort Type)
        {
            ushort PacketType = 1010;
            byte[] Packet = new byte[24];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 8)) = (uint)Char.UID;
                *((uint*)(p + 12)) = (uint)Type;
                *(p + 16) = 0xcf;
                *(p + 17) = 2;
                *(p + 18) = 34;
                *(p + 19) = 2;
                *(p + 20) = 3;
                *(p + 22) = 126;
            }
            return Packet;
        }

        public byte[] Status3(long CharId)
        {
            ushort PacketType = 1017;
            byte[] Packet = new byte[28];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)CharId;
                *(p + 8) = 1;
                *(p + 12) = 12;
                *(p + 16) = 0x61;
                *(p + 17) = 0xb3;
                *(p + 18) = 0x1e;
            }
            return Packet;
        }
        public byte[] Status2(long CharId, int Val)
        {
            ushort PacketType = 1017;
            byte[] Packet = new byte[28];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)CharId;
                *(p + 8) = (byte)(1 & 0xff);
                *(p + 12) = (byte)(26 & 0xff);
                *(p + 16) = (byte)(Val & 0xff);
                *(p + 17) = (byte)((Val >> 8) & 0xff);
            }
            return Packet;
        }
        public byte[] Status1(long CharId, int Val)
        {
            ushort PacketType = 1017;
            byte[] Packet = new byte[28];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)CharId;
                *(p + 8) = (byte)(1 & 0xff);
                *(p + 12) = (byte)(12 & 0xff);
                *(p + 16) = 225;
                *(p + 17) = 226;
                *(p + 18) = (byte)(Val & 0xff);
                *(p + 19) = (byte)((Val >> 8) & 0xff);
            }

            return Packet;
        }
        public byte[] Death(Character Char)
        {
            ushort PacketType = 1017;
            byte[] Packet = new byte[36];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)Char.UID;
                *(p + 8) = 2;
                *(p + 13) = 0xff;
                *(p + 14) = 0xff;
                *(p + 15) = 0xff;
                *(p + 16) = 0xff;
                *(p + 21) = 26;
            }

            return Packet;
        }
        public byte[] ViewEquip(Character Char)
        {
            ushort PacketType = 0x3f7;
            string Spouse = Char.Spouse;

            byte[] Packet = new byte[11 + Spouse.Length];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)Char.UID;
                *(p + 8) = 0x10;
                *(p + 9) = 0x01;
                *(p + 10) = (byte)Spouse.Length;

                for (int i = 0; i < Spouse.Length; i++)
                {
                    *(p + 11 + i) = Convert.ToByte(Spouse[i]);
                }
            }
            return Packet;
        }

        public byte[] MobSkillUse(SingleMob Mob, Character Attacked, uint DMG, ushort SkillId, byte SkillLevel)
        {
            ushort PacketType = 1105;
            byte[] Packet = new byte[32];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)Mob.UID;
                *((ushort*)(p + 8)) = (ushort)Attacked.LocX;
                *((ushort*)(p + 10)) = (ushort)Attacked.LocY;
                *((ushort*)(p + 12)) = (ushort)SkillId;
                *((ushort*)(p + 14)) = (ushort)SkillLevel;
                *(p + 16) = 1;
                *((uint*)(p + 20)) = (uint)Attacked.UID;
                *((uint*)(p + 24)) = (uint)DMG;
            }
            return Packet;
        }
        public byte[] MobSkillUse2(SingleMob Mob, SingleMob Attacked, uint DMG, ushort SkillId, byte SkillLevel)
        {
            ushort PacketType = 1105;
            byte[] Packet = new byte[32];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)Mob.UID;
                *((ushort*)(p + 8)) = (ushort)Attacked.PosX;
                *((ushort*)(p + 10)) = (ushort)Attacked.PosY;
                *((ushort*)(p + 12)) = (ushort)SkillId;
                *((ushort*)(p + 14)) = (ushort)SkillLevel;
                *(p + 16) = 1;
                *((uint*)(p + 20)) = (uint)Attacked.UID;
                *((uint*)(p + 24)) = (uint)DMG;
            }
            return Packet;
        }
        public byte[] SkillUse(Character Charr, Hashtable Targets, Hashtable PTargets, Hashtable NPCTargets, short AimX, short AimY, short SkillId, byte SkillLvl, byte Switch, uint OneTarget, uint TargetDMG)
        {
            ushort PacketType = 1105;
            int Len = 32;
            if (Switch == 0)
                Len = 20 + Targets.Count * 12 + PTargets.Count * 12 + NPCTargets.Count * 12;

            byte[] Packet = new byte[Len];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;

                *((uint*)(p + 4)) = (uint)Charr.UID;

                if (Switch == 0 || Switch == 2)
                {
                    *((ushort*)(p + 8)) = (ushort)AimX;
                    *((ushort*)(p + 10)) = (ushort)AimY;
                }
                else if (Switch == 1)
                    *((uint*)(p + 8)) = (uint)OneTarget;

                *((ushort*)(p + 12)) = (ushort)SkillId;
                *((ushort*)(p + 14)) = (ushort)SkillLvl;

                if (Switch == 0)
                    *((uint*)(p + 16)) = (uint)(Targets.Count + PTargets.Count + NPCTargets.Count);
                else
                    *(p + 16) = 1;


                int Count = 0;

                if (Switch == 0)
                {
                    foreach (DictionaryEntry DE in Targets)
                    {
                        *((uint*)(p + +20 + Count)) = (uint)((SingleMob)DE.Key).UID;
                        *((uint*)(p + +24 + Count)) = (uint)(uint)DE.Value;

                        Count += 12;
                    }
                    foreach (DictionaryEntry DE in PTargets)
                    {
                        *((uint*)(p + +20 + Count)) = (uint)((Character)DE.Key).UID;
                        *((uint*)(p + +24 + Count)) = (uint)(uint)DE.Value;

                        Count += 12;
                    }
                    foreach (DictionaryEntry DE in NPCTargets)
                    {
                        *((uint*)(p + +20 + Count)) = (uint)((SingleNPC)DE.Key).UID;
                        *((uint*)(p + +24 + Count)) = (uint)(uint)DE.Value;

                        Count += 12;
                    }
                }
                else
                {
                    *((uint*)(p + 20)) = (uint)OneTarget;
                    *((uint*)(p + 24)) = (uint)TargetDMG;
                }
            }

            return Packet;
        }

        public byte[] ViewEquipAdd(uint ViewedCUID, uint ItemId, byte Plus, byte Bless, byte Enchant, byte Soc1, byte Soc2, byte Location, uint MaxDura, uint CurDura)
        {
            ushort PacketType = 1008;
            byte[] Packet = new byte[36];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)ViewedCUID;
                *((uint*)(p + 8)) = (uint)ItemId;
                if (ItemId == 1050002 || ItemId == 1050001 || ItemId == 1050000)
                {
                    *((ushort*)(p + 12)) = (ushort)CurDura;
                    *((ushort*)(p + 14)) = (ushort)MaxDura;
                }
                else
                {
                    *(p + 12) = (byte)((byte)Math.Abs(200 - CurDura) & 0xff);
                    *(p + 13) = (byte)((byte)((CurDura) / 2.56) & 0xff);
                    *(p + 14) = (byte)((byte)Math.Abs(200 - MaxDura) & 0xff);
                    *(p + 15) = (byte)((byte)((MaxDura) / 2.56) & 0xff);
                }
                *(p + 16) = 4;
                *(p + 18) = Location;
                *(p + 19) = Soc1;
                *(p + 24) = Soc1;
                *(p + 25) = Soc2;
                *(p + 28) = Plus;
                *(p + 29) = Bless;
                *(p + 30) = Enchant;
            }
            return Packet;
        }

        public byte[] MobMoves(uint MobUID, byte Dir)
        {
            ushort PacketType = 1005;
            byte[] Packet = new byte[12];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)MobUID;
                *(p + 8) = Dir;
                *(p + 9) = 1;
            }
            return Packet;
        }

        public byte[] ItemDrop(uint ItemUID, uint ItemId, uint X, uint Y)
        {
            ushort PacketType = 1101;
            byte[] Packet = new byte[20];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)ItemUID;
                *((uint*)(p + 8)) = (uint)ItemId;
                *((ushort*)(p + 12)) = (ushort)X;
                *((ushort*)(p + 14)) = (ushort)Y;
                *(p + 16) = 1;
            }
            return Packet;
        }

        public byte[] ItemDropRemove(uint ItemUID)
        {
            ushort PacketType = 1101;
            byte[] Packet = new byte[20];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)ItemUID;
                *(p + 8) = 0x4d;
                *(p + 9) = 0xa2;
                *((ushort*)(p + 10)) = (ushort)General.Rand.Next(1, 9);
                *((ushort*)(p + 12)) = (ushort)General.Rand.Next(99, 153);
                *((ushort*)(p + 14)) = (ushort)General.Rand.Next(208, 217);
                *(p + 16) = 2;
            }
            return Packet;
        }

        public byte[] MobFade(long uid)
        {
            ushort PacketType = 1017;
            byte[] Packet = new byte[28];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)uid;

                *(p + 8) = 1;
                *(p + 12) = 26;
                *((ushort*)(p + 16)) = (ushort)2080;
            }
            return Packet;
        }

        public byte[] Attack(uint UID, uint Target, short TargetX, short TargetY, byte AttackType, uint Damage)
        {
            ushort PacketType = 1022;
            byte[] Packet = new byte[28];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 8)) = (uint)UID;
                *((uint*)(p + 12)) = (uint)Target;
                *((ushort*)(p + 16)) = (ushort)TargetX;
                *((ushort*)(p + 18)) = (ushort)TargetY;
                *((uint*)(p + 20)) = (uint)AttackType;
                *((uint*)(p + 24)) = (uint)Damage;
            }
            return Packet;
        }

        public byte[] SpawnMob(SingleMob Mob)
        {
            ushort PacketType = 0x3f6;
            byte[] Packet = new byte[85 + Mob.Name.Length];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)Mob.UID;
                *((uint*)(p + 8)) = (uint)Mob.Mech;
                *((ushort*)(p + 48)) = (ushort)Mob.CurHP;
                *((ushort*)(p + 50)) = (ushort)Mob.Level;
                *((ushort*)(p + 52)) = (ushort)Mob.PosX;
                *((ushort*)(p + 54)) = (ushort)Mob.PosY;

                *(p + 58) = Mob.Pos;
                *(p + 59) = 100;
                *(p + 80) = 1;
                *(p + 81) = (byte)Mob.Name.Length;

                for (int i = 0; i < Mob.Name.Length; i++)
                {
                    *(p + 82 + i) = Convert.ToByte(Mob.Name[i]);
                }
            }
            return Packet;
        }

        public byte[] ItemUsage(long ItemUID, int Position, int Packettype)
        {
            ushort PacketType = 1009;
            byte[] Packet = new byte[20];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)ItemUID;
                *((uint*)(p + 8)) = (uint)Position;
                *((uint*)(p + 12)) = (uint)Packettype;
            }
            return Packet;
        }

        public byte[] LearnSkill(short skill_id, byte lvl, uint skill_exp)
        {
            ushort PacketType = 1103;
            byte[] Packet = new byte[12];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)skill_exp;
                *((ushort*)(p + 8)) = (ushort)skill_id;
                *((ushort*)(p + 10)) = (ushort)lvl;
            }

            return Packet;
        }

        public byte[] Vital(long CharId, int Type, ulong Value)
        {
            ushort PacketType = 1017;
            byte[] Packet = new byte[28];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)CharId;
                *(p + 8) = 1;
                *((uint*)(p + 12)) = (uint)Type;
                *((uint*)(p + 16)) = (uint)Value;
            }

            return Packet;
        }

        public byte[] NPCSay(string Text)
        {
            ushort PacketType = 2032;
            byte[] Packet = new byte[16 + Text.Length];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *(p + 10) = 0xff;
                *(p + 11) = 1;
                *(p + 12) = 1;
                *(p + 13) = (byte)Text.Length;
                for (int i = 0; i < Text.Length; i++)
                {
                    *(p + 14 + i) = Convert.ToByte(Text[i]);
                }
            }
            return Packet;
        }        

        public byte[] NPCLink(string Text, byte DialNr)
        {
            ushort PacketType = 2032;
            byte[] Packet = new byte[16 + Text.Length];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *(p + 10) = DialNr; 
                *(p + 11) = 2;
                *(p + 12) = 1;
                *(p + 13) = (byte)Text.Length;
                for (int i = 0; i < Text.Length; i++)
                {
                    *(p + 14 + i) = Convert.ToByte(Text[i]);
                }
            }
            return Packet;
        }
        public byte[] NPCLink2(string Text, byte DialNr)
        {
            ushort PacketType = 2032;
            byte[] Packet = new byte[16 + Text.Length];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *(p + 10) = DialNr;
                *(p + 11) = 3;
                *(p + 12) = 1;
                *(p + 13) = (byte)Text.Length;
                for (int i = 0; i < Text.Length; i++)
                {
                    *(p + 14 + i) = Convert.ToByte(Text[i]);
                }
            }
            return Packet;
        }
        public byte[] NPCSetFace(short Face)
        {
            ushort PacketType = 2032;
            byte[] Packet = new byte[16];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *(p + 4) = 10;
                *(p + 6) = 10;
                *((ushort*)(p + 8)) = (ushort)Face;
                *(p + 10) = 0xff;
                *(p + 11) = 4;
            }
            return Packet;
        }
        public byte[] NPCFinish()
        {
            ushort PacketType = 2032;
            byte[] Packet = new byte[16];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *(p + 10) = 0xff;
                *(p + 11) = 100;
            }

            return Packet;
        }
        public byte[] Prof(short Type, byte Lvl, uint Exp)
        {
            ushort PacketType = 0x401;
            byte[] Packet = new byte[16];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)Type;
                *((uint*)(p + 8)) = (uint)Lvl;
                *((uint*)(p + 12)) = (uint)Exp;
            }

            return Packet;
        }

        public byte[] Prof2(short Type, byte Lvl, uint Exp)
        {
            ushort PacketType = 1005;
            byte[] Packet = new byte[12];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)Exp;
                *((ushort*)(p + 8)) = (ushort)Type;
                *((ushort*)(p + 10)) = (ushort)Lvl;
            }

            return Packet;
        }


        public byte[] SpawnNPC(SingleNPC NPC)
        {
            ushort PacketType = 2030;
            byte[] Packet = new byte[20];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)NPC.UID;
                *((ushort*)(p + 8)) = (ushort)NPC.X;
                *((ushort*)(p + 10)) = (ushort)NPC.Y;
                *((ushort*)(p + 12)) = (ushort)NPC.Type;
                *((ushort*)(p + 14)) = (ushort)NPC.Flags;
                *(p + 16) = NPC.Dir;
            }

            return Packet;
        }

        public byte[] RemoveItem(long UID, byte pos, byte type)
        {
            ushort PacketType = 1009;
            byte[] Packet = new byte[20];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)UID;
                *((uint*)(p + 8)) = (uint)pos;
                *((uint*)(p + 12)) = (uint)type;
            }

            return Packet;
        }

        public byte[] AddItem(long UID, int itemid, byte Plus, byte Bless, byte Enchant, byte soc1, byte soc2, byte Location, int CurArrows, int MaxArrows)
        {
            ushort PacketType = 0x3f0;
            string IDE = Convert.ToString(itemid).Remove(2, Convert.ToString(itemid).Length - 2);

            byte[] Packet = new byte[36];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;

                *((uint*)(p + 4)) = (uint)UID;
                *((uint*)(p + 8)) = (uint)itemid;

                if (itemid == 1050002 || itemid == 1050001 || itemid == 1050000)
                {
                    *((ushort*)(p + 12)) = (ushort)CurArrows;
                    *((ushort*)(p + 14)) = (ushort)MaxArrows;
                }
                else if (IDE == "72" || IDE == "10" || IDE == "79" || IDE == "78" || IDE == "72" || IDE == "71" || IDE == "70" && itemid != 1050002 && itemid != 1050001 && itemid != 1050000)
                { }
                else
                {
                    *(p + 12) = (byte)(Math.Abs(200 - CurArrows) & 0xff);
                    *(p + 13) = (byte)((byte)(CurArrows / 2.56) & 0xff);
                    *(p + 14) = (byte)(Math.Abs(200 - MaxArrows) & 0xff);
                    *(p + 15) = (byte)((byte)(MaxArrows / 2.56) & 0xff);
                }
                *(p + 16) = 1;
                *(p + 18) = Location;
                *(p + 19) = soc1;
                *(p + 24) = soc1;
                *(p + 25) = soc2;
                *(p + 28) = Plus;
                *(p + 29) = Bless;
                *(p + 30) = Enchant;
            }
            return Packet;
        }

        public byte[] SendMsg(long MessageId, string from, string to, string msg, short type)
        {
            ushort PacketType = 1004;
            byte[] Packet = new byte[29 + from.Length + to.Length + msg.Length];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *(p + 5) = 0xff;
                *(p + 6) = 0xff;
                *((ushort*)(p + 8)) = (ushort)type;
                *((uint*)(p + 12)) = (uint)MessageId;

                *(p + 24) = 4;
                *(p + 25) = (byte)from.Length;

                for (int i = 0; i < from.Length; i++)
                {
                    *(p + 26 + i) = Convert.ToByte(from[i]);
                }

                *(p + 26 + from.Length) = (byte)to.Length;

                for (int i = 0; i < to.Length; i++)
                {
                    *(p + 27 + i + from.Length) = Convert.ToByte(to[i]);
                }
                *(p + 27 + from.Length + to.Length) = 0;
                *(p + 28 + from.Length + to.Length) = (byte)msg.Length;

                for (int i = 0; i < msg.Length; i++)
                {
                    *(p + 29 + i + from.Length + to.Length) = Convert.ToByte(msg[i]);
                }
            }
            return Packet;
        }

        public byte[] SendMsg2(long MessageId, string from, string to, string msg, bool First)
        {
            ushort PacketType = 1004;
            byte[] Packet = new byte[29 + from.Length + to.Length + msg.Length];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *(p + 4) = 0xff;
                *(p + 5) = 0xff;
                *(p + 6) = 0xff;
                if (First)
                    *(p + 8) = 0x3c;
                else
                    *(p + 8) = 0x3d;
                *(p + 9) = 8;
                *((uint*)(p + 12)) = (uint)MessageId;

                *(p + 24) = 4;
                *(p + 25) = (byte)from.Length;

                for (int i = 0; i < from.Length; i++)
                {
                    *(p + 26 + i) = Convert.ToByte(from[i]);
                }

                *(p + 26 + from.Length) = (byte)to.Length;

                for (int i = 0; i < to.Length; i++)
                {
                    *(p + 27 + i + from.Length) = Convert.ToByte(to[i]);
                }
                *(p + 27 + from.Length + to.Length) = 0;
                *(p + 28 + from.Length + to.Length) = (byte)msg.Length;

                for (int i = 0; i < msg.Length; i++)
                {
                    *(p + 29 + i + from.Length + to.Length) = Convert.ToByte(msg[i]);
                }
            }
            return Packet;
        }


        public byte[] GeneralData(long Identifier, long Value1, ushort Value2, ushort Value3, short Type)
        {
            ushort PacketType = 1010;
            byte[] Packet = new byte[24];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)Environment.TickCount;
                *((uint*)(p + 8)) = (uint)Identifier;
                *((uint*)(p + 12)) = (uint)Value1;
                *((ushort*)(p + 16)) = (ushort)Value2;
                *((ushort*)(p + 18)) = (ushort)Value3;
                *((ushort*)(p + 22)) = (ushort)Type;
            }

            return Packet;
        }

        public byte[] SpawnSobNPC(SingleNPC NPC)
        {
            ushort PacketType = 1109;
            byte[] Packet = new byte[28];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)NPC.UID;
                *((uint*)(p + 8)) = (uint)NPC.MaxHP;
                *((uint*)(p + 12)) = (uint)NPC.CurHP;
                *((ushort*)(p + 16)) = (ushort)NPC.X;
                *((ushort*)(p + 18)) = (ushort)NPC.Y;
                *((ushort*)(p + 20)) = (ushort)(NPC.Type +NPC.Dir);
                *((ushort*)(p + 22)) = (ushort)NPC.Flags;
                *(p + 24) = 17;
            }
            return Packet;
        }
        public byte[] SpawnShopFlag(SingleNPC NPC)
        {
            ushort PacketType = 1109;
            byte[] Packet = new byte[28];

            fixed (byte* p = Packet)
            {                
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)NPC.UID;
                *((ushort*)(p + 16)) = (ushort)NPC.X;
                *((ushort*)(p + 18)) = (ushort)NPC.Y;
                *((ushort*)(p + 20)) = (ushort)1086;
                *((ushort*)(p + 22)) = (ushort)16;
            }
            return Packet;
        }
        public byte[] SpawnSobNPC2(SingleNPC NPC)
        {
            ushort PacketType = 1109;
            byte[] Packet = new byte[28];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)NPC.UID;
                *((ushort*)(p + 16)) = (ushort)NPC.X;
                *((ushort*)(p + 18)) = (ushort)NPC.Y;
                *((ushort*)(p + 20)) = (ushort)(NPC.Type + NPC.Dir);
                *((ushort*)(p + 22)) = (ushort)NPC.Flags;
                *(p + 24) = 17;
            }
            return Packet;
        }
        
        public byte[] SpawnSob(SingleNPC NPC)
        {
            ushort PacketType = 1109;
            byte[] Packet = new byte[28];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)NPC.UID;
                *((uint*)(p + 8)) = (uint)NPC.MaxHP;
                *((uint*)(p + 12)) = (uint)NPC.CurHP;
                *((ushort*)(p + 16)) = (ushort)NPC.X;
                *((ushort*)(p + 18)) = (ushort)NPC.Y;
                *((ushort*)(p + 20)) = (ushort)NPC.Type;
                *(p + 22) = 26;
                *(p + 24) = 21;
            }
            return Packet;
        }
        public byte[] SpawnSobNPCNamed(SingleNPC NPC, string Name)
        {
            ushort PacketType = 1109;
            byte[] Packet = new byte[28+Name.Length];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)NPC.UID;
                *((uint*)(p + 8)) = (uint)NPC.MaxHP;
                *((uint*)(p + 12)) = (uint)NPC.CurHP;
                *((ushort*)(p + 16)) = (ushort)NPC.X;
                *((ushort*)(p + 18)) = (ushort)NPC.Y;
                *((ushort*)(p + 20)) = (ushort)(NPC.Type + +NPC.Dir);
                *((ushort*)(p + 22)) = (ushort)NPC.Flags;
                *(p + 24) = 11;
                *(p + 26) = 1;
                *(p + 27) = (byte)Name.Length;
                for (int i = 0; i < Name.Length; i++)
                {
                    *(p + 28 + i) = Convert.ToByte(Name[i]);
                }
            }
            return Packet;
        }
        public byte[] SpawnCarpet(Character Character, int ID)
        {
            ushort PacketType = 1109;
            byte[] Packet = new byte[28 + Character.Name.Length];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)ID;
                *((ushort*)(p + 16)) = (ushort)(Character.LocX + 1);
                *((ushort*)(p + 18)) = (ushort)Character.LocY;
                *((ushort*)(p + 20)) = (ushort)(406);
                *((ushort*)(p + 22)) = (ushort)14;
                *(p + 24) = 11;
                *(p + 26) = 1;
                *(p + 27) = (byte)Character.Name.Length;
                for (int i = 0; i < Character.Name.Length; i++)
                {
                    *(p + 28 + i) = Convert.ToByte(Character.Name[i]);
                }
            }
            return Packet;
        }

        public byte[] StringGuild(long CharId, byte Type, string name, byte Count)
        {
            ushort PacketType = 1015;
            byte[] Packet = new byte[12 + name.Length];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)CharId;

                *(p + 8) = Type;
                *(p + 9) = (byte)Count;

                for (int i = 0; i < name.Length; i++)
                {
                    *(p + 10 + i) = Convert.ToByte(name[i]);
                }
            }

            return Packet;
        }

        public byte[] String(long CharId, byte Type, string name)
        {
            ushort PacketType = 1015;
            byte[] Packet = new byte[13 + name.Length];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)CharId;

                *(p + 8) = Type;
                *(p + 9) = 1;
                *(p + 10) = (byte)name.Length;

                for (int i = 0; i < name.Length; i++)
                {
                    *(p + 11 + i) = Convert.ToByte(name[i]);
                }
            }

            return Packet;
        }


        public byte[] SpawnEntity(Character Player)
        {
            string[] equip;
            long HeadId = 0;
            long ArmorId = 0;
            long RightHandId = 0;
            long LeftHandId = 0;
            long GarmentId = 0;

            if (Player.Equips[1] != null)
            {
                equip = Player.Equips[1].Split('-');
                HeadId = Convert.ToInt64(equip[0]);
            }

            if (Player.Equips[3] != null)
            {
                equip = Player.Equips[3].Split('-');
                ArmorId = Convert.ToInt64(equip[0]);
            }

            if (Player.Equips[4] != null)
            {
                equip = Player.Equips[4].Split('-');
                RightHandId = Convert.ToInt64(equip[0]);
            }

            if (Player.Equips[5] != null)
            {
                equip = Player.Equips[5].Split('-');
                LeftHandId = Convert.ToInt64(equip[0]);
            }

            if (Player.Equips[9] != null)
            {
                equip = Player.Equips[9].Split('-');
                GarmentId = Convert.ToInt64(equip[0]);
            }

            long ToArmor;

            if (Player.Equips[9] != null)
                ToArmor = GarmentId;
            else
                ToArmor = ArmorId;

            uint Model;
            if (Player.Alive)
                Model = uint.Parse(Convert.ToString(Player.Avatar) + Convert.ToString(Player.Model));
            else
            {
                if (Player.Model == 1003 || Player.Model == 1004)
                    Model = uint.Parse(Convert.ToString(Player.Avatar) + 1098.ToString());
                else
                    Model = uint.Parse(Convert.ToString(Player.Avatar) + 1099.ToString());
            }

            ushort PacketType = 0x3f6;
            byte[] Packet = new byte[85 + Player.Name.Length];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)Player.UID;
                *((uint*)(p + 8)) = (uint)Model;
                *((uint*)(p + 12)) = (uint)Player.GetStat();
                *((ushort*)(p + 20)) = (ushort)Player.GuildID;
                *(p + 23) = Player.GuildPosition;

                if (Player.Alive)
                {
                    *((uint*)(p + 28)) = (uint)HeadId;
                    *((uint*)(p + 32)) = (uint)ToArmor;
                    *((uint*)(p + 36)) = (uint)RightHandId;
                    *((uint*)(p + 40)) = (uint)LeftHandId;
                }
                *((ushort*)(p + 52)) = (ushort)Player.LocX;
                *((ushort*)(p + 54)) = (ushort)Player.LocY;
                *((ushort*)(p + 56)) = (ushort)Player.Hair;

                *(p + 58) = Player.Direction;
                *(p + 59) = Player.Action;
                p[60] = (byte)Player.RBCount;
                p[62] = (byte)Player.Level;
                *(p + 80) = 1;
                *(p + 81) = (byte)Player.Name.Length;
                for (int i = 0; i < Player.Name.Length; i++)
                {
                    *(p + 82 + i) = Convert.ToByte(Player.Name[i]);
                }
            }
            return Packet;
        } 

        public byte[] PlacePacket1(Character Charr)
        {
            ushort PacketType = 0x3f2;
            byte[] Packet = new byte[24];
            uint Timer = (uint)Environment.TickCount;

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)Timer;
                *((ushort*)(p + 8)) = (ushort)Charr.LocMap;
                *((ushort*)(p + 12)) = (ushort)Charr.LocMap;
                *((ushort*)(p + 16)) = (ushort)Charr.LocX;
                *((ushort*)(p + 18)) = (ushort)Charr.LocY;
                *(p + 22) = (byte)(0x4a & 0xff);
            }

            return Packet;
        }

        public byte[] PlacePacket2(Character Charr)
        {
            ushort PacketType = 0x456;
            byte[] Packet = new byte[24];
            uint Timer = (uint)Environment.TickCount;
            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)Timer;
                *((uint*)(p + 8)) = (uint)Charr.UID;
                *(p + 12) = 0xff;
                *(p + 13) = 0xff;
                *(p + 14) = 0xff;
                *(p + 15) = 0xff;
                *((ushort*)(p + 16)) = (ushort)Charr.LocX;
                *((ushort*)(p + 18)) = (ushort)Charr.LocY;
                *(p + 22) = 0x68;
            }

            return Packet;
        }

        public byte[] PlacePacket3(Character Charr)
        {
            ushort PacketType = 0x456;
            byte[] Packet = new byte[16];
            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((ushort*)(p + 4)) = (ushort)Charr.LocMap;
                *((ushort*)(p + 8)) = (ushort)Charr.LocMap;
            }

            return Packet;
        }

        public byte[] LogonPacket()
        {
            ushort PacketType = 0x3f9;
            byte[] Packet = new byte[28];
            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *(p + 4) = 0x57;
                *(p + 5) = 0x1d;
                *(p + 6) = 0x12 & 0xff;
                *(p + 8) = 0x01 & 0xff;
                *(p + 12) = (byte)((0x09) & 0xff);
                *(p + 16) = (byte)((0x64) & 0xff);
            }

            return Packet;
        }

        public byte[] ShowMinimap(bool ff)
        {
            ushort PacketType = 0x3f8;
            byte[] Packet = new byte[20];
            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *(p + 4) = ff ? (byte)0x01 : (byte)0x00;
                *(p + 12) = (byte)((0x14) & 0xff);
            }
            return Packet;
        }

        public byte[] AfterChar()
        {
            ushort PacketType = 0x3f9;
            byte[] Packet = new byte[36];
            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *((uint*)(p + 4)) = (uint)1187159;
                *(p + 8) = 0x01;
                *(p + 12) = 0x1a;
            }
            return Packet;
        }
        public byte[] CharacterInfo(Character Charr)
        {
            byte[] Packet = new byte[70 + Charr.Name.Length + Charr.Spouse.Length];
            long Model = Convert.ToInt64(Convert.ToString(Charr.Avatar) + Convert.ToString(Charr.Model));

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = 1006;
                *((uint*)(p + 4)) = (uint)Charr.UID;
                *((uint*)(p + 8)) = (uint)Model;
                *((ushort*)(p + 12)) = (ushort)Charr.Hair;
                *((uint*)(p + 14)) = (uint)Charr.Silvers;
                *((uint*)(p + 18)) = (uint)Charr.CPs;
                *((uint*)(p + 22)) = (uint)Charr.Exp;
                *((ushort*)(p + 42)) = (ushort)5130;
                *((ushort*)(p + 46)) = (ushort)Charr.Str;
                *((ushort*)(p + 48)) = (ushort)Charr.Agi;
                *((ushort*)(p + 50)) = (ushort)Charr.Vit;
                *((ushort*)(p + 52)) = (ushort)Charr.Spi;
                *((ushort*)(p + 54)) = (ushort)Charr.StatP;
                *((ushort*)(p + 56)) = (ushort)Charr.CurHP;
                *((ushort*)(p + 58)) = (ushort)Charr.MaxMana();
                *((ushort*)(p + 60)) = (ushort)Charr.PKPoints;
                *(p + 62) = Charr.Level;
                *(p + 63) = Charr.Job;
                *(p + 66) = 1;
                *(p + 67) = 2;
                *(p + 68) = (byte)Charr.Name.Length;

                Packet[69 + Charr.Name.Length] = (byte)Charr.Spouse.Length;

                for (sbyte i = 0; i < Charr.Name.Length; i++)
                {
                    *(p + 69 + i) = (byte)Charr.Name[i];
                }
                for (sbyte i = 0; i < Charr.Spouse.Length; i++)
                {
                    *(p + 70 + Charr.Name.Length + i) = (byte)Charr.Spouse[i];
                }

            }
            return Packet;
        }
        public byte[] LanguageResponse(uint MessageId)
        {
            ushort PacketType = 1004;
            byte[] Packet = new byte[55];
            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *(p + 4) = 0xff;
                *(p + 5) = 0xff;
                *(p + 6) = 0xff;
                *(p + 7) = 0x00;
                *(p + 8) = 0x35;
                *(p + 9) = 0x08;
                *(p + 10) = 0x00;
                *(p + 11) = 0x00;

                *((uint*)(p + 12)) = MessageId;

                *(p + 16) = 0x00;
                *(p + 17) = 0x00;
                *(p + 18) = 0x00;
                *(p + 19) = 0x00;
                *(p + 20) = 0x00;
                *(p + 21) = 0x00;
                *(p + 22) = 0x00;
                *(p + 23) = 0x00;

                *(p + 24) = 0x04;
                *(p + 25) = 0x06;
                *(p + 26) = 0x53;
                *(p + 27) = 0x59;
                *(p + 28) = 0x53;
                *(p + 29) = 0x54;
                *(p + 30) = 0x45;
                *(p + 31) = 0x4d;
                *(p + 32) = 0x08;
                *(p + 33) = 0x41;
                *(p + 34) = 0x4c;
                *(p + 35) = 0x4c;
                *(p + 36) = 0x55;
                *(p + 37) = 0x53;
                *(p + 38) = 0x45;
                *(p + 39) = 0x52;
                *(p + 40) = 0x53;
                *(p + 41) = 0x00;
                *(p + 42) = 0x09;
                *(p + 43) = 0x41;
                *(p + 44) = 0x4e;
                *(p + 45) = 0x53;
                *(p + 46) = 0x57;
                *(p + 47) = 0x45;
                *(p + 48) = 0x52;
                *(p + 49) = 0x5f;
                *(p + 50) = 0x4f;
                *(p + 51) = 0x4b;
            }

            return Packet;
        }

        public byte[] CharCreated(int MessageId)
        {
            ushort PacketType = 1004;
            byte[] Packet = new byte[55];
            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *(p + 4) = (0xff);
                *(p + 5) = (0xff);
                *(p + 6) = (0xff);
                *(p + 8) = (0x34);
                *(p + 9) = (0x08);
                *((uint*)(p + 12)) = (uint)MessageId;
                *(p + 24) = (4);
                *(p + 25) = (6);
                *(p + 26) = (83);
                *(p + 27) = (89);
                *(p + 28) = (83);
                *(p + 29) = (84);
                *(p + 30) = (69);
                *(p + 31) = (77);
                *(p + 32) = (8);
                *(p + 33) = (65);
                *(p + 34) = (76);
                *(p + 35) = (76);
                *(p + 36) = (85);
                *(p + 37) = (83);
                *(p + 38) = (69);
                *(p + 39) = (82);
                *(p + 40) = (83);
                *(p + 42) = (9);
                *(p + 43) = (65);
                *(p + 44) = (78);
                *(p + 45) = (83);
                *(p + 46) = (87);
                *(p + 47) = (69);
                *(p + 48) = (82);
                *(p + 49) = (95);
                *(p + 50) = (79);
                *(p + 51) = (75);
            }
            return Packet;
        }

        public byte[] UsedName(int MessageId)
        {
            ushort PacketType = 0x3ec;
            byte[] Packet = new byte[0x44];

            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
            }
            return Packet;
        }

        public byte[] InvalidName(int MessageId)
        {
            ushort PacketType = 0x3ec;
            byte[] Packet = new byte[4];
            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
            }
            return Packet;
        }

        public byte[] NewCharPacket(int MessageId)
        {
            ushort PacketType = 0x3ec;
            byte[] Packet = new byte[54];
            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *(p + 4) = 0xff;
                *(p + 5) = (0xff);
                *(p + 6) = (0xff);
                *(p + 8) = (0x35);
                *(p + 9) = (0x08);
                *(p + 13) = (0x28);
                *(p + 14) = (0x03);                
                *(p + 24) = (0x04);
                *(p + 25) = (0x06);
                *((uint*)(p + 26)) = (uint)MessageId;
                *(p + 30) = (0x45);
                *(p + 31) = (0x4d);
                *(p + 32) = (0x08);
                *(p + 33) = (0x41);
                *(p + 34) = (0x4c);
                *(p + 35) = (0x4c);
                *(p + 36) = (0x55);
                *(p + 37) = (0x53);
                *(p + 38) = (0x45);
                *(p + 39) = (0x52);
                *(p + 40) = (0x53);
                *(p + 42) = (0x08);
                *(p + 43) = (0x4e);
                *(p + 44) = (0x45);
                *(p + 45) = (0x57);
                *(p + 46) = (0x5f);
                *(p + 47) = (0x52);
                *(p + 48) = (0x4f);
                *(p + 49) = (0x4c);
                *(p + 50) = (0x45);
            }

            return Packet;
        }

        public byte[] AuthResponse(string ip, byte[] key1, byte[] key2)
        {
            ushort PacketType = 0x41f;
            byte[] Packet = new byte[32];
            fixed (byte* p = Packet)
            {
                *((ushort*)p) = (ushort)Packet.Length;
                *((ushort*)(p + 2)) = (ushort)PacketType;
                *(p + 4) = key2[3];
                *(p + 5) = key2[2];
                *(p + 6) = key2[1];
                *(p + 7) = key2[0];
                *(p + 8) = key1[3];
                *(p + 9) = key1[2];
                *(p + 10) = key1[1];
                *(p + 11) = key1[0];
                for (int i = 0; i < ip.Length; i++)
                {
                    *(p + 12 + i) = Convert.ToByte(ip[i]);
                }
                *(p + 28) = 0xb8;
                *(p + 29) = 0x16;
            }
            return Packet;
        }

        internal byte[] NPCLink(string p)
        {
            throw new NotImplementedException();
        }
    }
}
