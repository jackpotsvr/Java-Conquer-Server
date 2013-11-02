using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Timers;

namespace COServer_Project
{
    public class World
    {
        public static Hashtable AllChars = new Hashtable();
        public static Hashtable AllMobs = new Hashtable();
        public static Guild PoleHolder;
        public static Hashtable GWScores = new Hashtable();
        public static bool BroadcastSend = false;
        public static bool Broadcast = false;
        public static ArrayList PlayersPraying = new ArrayList(50);
        public static bool GWOn = false;

        public static void LevelUp(Character Player)
        {
            foreach (DictionaryEntry DE in AllChars)
            {
                Character Charr = (Character)DE.Value;

                if (Charr.LocMap == Player.LocMap)
                    if (Charr.MyClient.Online)
                        if (MyMath.CanSee(Player.LocX, Player.LocY, Charr.LocX, Charr.LocY))
                        {
                            Charr.MyClient.SendPacket(General.MyPackets.GeneralData((long)Player.UID, 0, 0, 0, 92));
                        }
            }
        }

        public static void NPCSpawns(SingleNPC NPC)
        {
            foreach (DictionaryEntry DE in AllChars)
            {
                Character Charr = (Character)DE.Value;

                if (NPC.Map == Charr.LocMap)
                    if (Charr.MyClient.Online)
                        if (MyMath.CanSee(NPC.X, NPC.Y, Charr.LocX, Charr.LocY))
                        {
                            if (NPC.Sob == 1)
                                Charr.MyClient.SendPacket(General.MyPackets.SpawnSobNPC(NPC));
                            else if (NPC.Sob == 2)
                                Charr.MyClient.SendPacket(General.MyPackets.SpawnSobNPCNamed(NPC, "Pole"));
                            else
                                Charr.MyClient.SendPacket(General.MyPackets.SpawnSob(NPC));
                        }
            }
        }

        public static void UsingSkill(Character User, short SkillId, byte SkillLvl, uint Target, uint Damage, short X, short Y)
        {
            if (SkillId != 1110 && SkillId != 1100 && SkillId != 1015)
            {
                if (User.LocMap != 1039)
                    User.AddSkillExp(SkillId, Damage);
                else
                    User.AddSkillExp(SkillId, Damage / 10);

                if (SkillId == 1095)
                {
                    if (User.LocMap != 1039)
                        User.AddSkillExp(SkillId, 10);
                    else
                        User.AddSkillExp(SkillId, 1);
                }
            }
            foreach (DictionaryEntry DE in AllChars)
            {
                Character Charr = (Character)DE.Value;
                if (Charr.MyClient.Online)
                    if (User.LocMap == Charr.LocMap)
                        if (MyMath.CanSee(User.LocX, User.LocY, Charr.LocX, Charr.LocY))
                        {
                            if (SkillId == 1002 || SkillId == 1190)
                                Charr.MyClient.SendPacket(General.MyPackets.SkillUse(User, null, null, null, 0, 0, SkillId, SkillLvl, 1, Target, Damage));
                            else
                                Charr.MyClient.SendPacket(General.MyPackets.SkillUse(User, null, null, null, X, Y, SkillId, SkillLvl, 2, Target, Damage));
                        }
            }
        }

        public static void UsingSkill(Character User, Hashtable MobTargets, Hashtable NPCTargets, Hashtable PlTargets, short AimX, short AimY, short SkillId, byte SkillLvl)
        {
            foreach (DictionaryEntry DE in AllChars)
            {
                Character Charr = (Character)DE.Value;
                if (Charr.MyClient.Online)
                    if (User.LocMap == Charr.LocMap)
                        if (MyMath.CanSee(User.LocX, User.LocY, Charr.LocX, Charr.LocY))
                            Charr.MyClient.SendPacket(General.MyPackets.SkillUse(User, MobTargets, PlTargets, NPCTargets, AimX, AimY, SkillId, SkillLvl, 0, 0, 0));

            }
            if (MobTargets.Count < 1 && NPCTargets.Count < 1 && PlTargets.Count < 1)
                return;
            uint GotXP = 0;
            uint GotSkilExp = 0;

            foreach (DictionaryEntry DE in PlTargets)
            {
                Character Player = (Character)DE.Key;
                uint Damage = (uint)DE.Value;

                if (!Other.CanPK(User.LocMap))
                    if (!User.BlueName)
                        if (!Player.BlueName)
                            if (Player.PKPoints < 100)
                                if (User.LocMap != 1005 && User.LocMap != 6000 && User.LocMap != 1038)
                                {
                                    User.GotBlueName = DateTime.Now;
                                    User.BlueName = true;
                                    User.MyClient.SendPacket(General.MyPackets.Vital(User.UID, 26, User.GetStat()));
                                    UpdateSpawn(User);
                                }

                if (Player.GetHitDie(Damage))
                {
                    if (User.PTarget != null)
                        if (Player == User.PTarget)
                        {
                            User.PTarget = null;
                            User.Attacking = false;
                        }
                    PVP(User, Player, 14, Damage);
                    if (!Other.CanPK(User.LocMap))
                        if (!Player.BlueName)
                            if (Player.PKPoints < 100)
                            {
                                User.GotBlueName = DateTime.Now;
                                User.BlueName = true;
                                User.PKPoints += 10;
                                User.MyClient.SendPacket(General.MyPackets.Vital(User.UID, 6, User.PKPoints));
                                if ((User.PKPoints > 29 && User.PKPoints - 10 < 30) || (User.PKPoints > 99 && User.PKPoints - 10 < 100))
                                {
                                    User.MyClient.SendPacket(General.MyPackets.Vital(User.UID, 26, User.GetStat()));
                                    UpdateSpawn(User);
                                }
                            }
                }

            }

            foreach (DictionaryEntry DE in NPCTargets)
            {
                SingleNPC TGO = (SingleNPC)DE.Key;
                uint Damage = (uint)DE.Value;

                uint TGOHP = TGO.CurHP;
                double ExpQuality = 1;

                if (TGO.Level + 5 < User.Level)
                    ExpQuality = 0.1;

                if (TGO.GetDamageDie(Damage, User))
                {
                    GotXP += (uint)(TGOHP * ExpQuality / 10);
                    if (SkillId != 5030)
                        GotSkilExp += (uint)(TGOHP / 100);
                    else
                        GotSkilExp += (uint)(5 * (SkillLvl + 1)); ;
                }
                else
                {
                    GotXP += (uint)(Damage * ExpQuality / 10);
                    if (SkillId != 5030)
                        GotSkilExp += (uint)(Damage / 100);
                    else
                        GotSkilExp += (uint)(5 * (SkillLvl + 1)); ;
                }
            }

            foreach (DictionaryEntry DE in MobTargets)
            {
                SingleMob Mob = (SingleMob)DE.Key;

                uint Damage = (uint)DE.Value;

                double ExpQuality = 0;

                if (Mob.Level + 4 < User.Level)
                    ExpQuality = 0.1;
                if (Mob.Level + 4 >= User.Level)
                    ExpQuality = 1;
                if (Mob.Level >= User.Level)
                    ExpQuality = 1.1;
                if (Mob.Level - 4 > User.Level)
                    ExpQuality = 1.3;

                uint MobCurHP = Mob.CurHP;
                double EAddExp = 0;
                uint UAddExp = 0;

                if (Mob.GetDamage(Damage))
                {
                    if (Mob.Name == User.QuestMob)
                    {
                        User.QuestKO++;
                        if (User.QuestKO >= 300)
                        {
                            User.MyClient.SendPacket(General.MyPackets.SendMsg(User.MyClient.MessageId, "SYSTEM", User.Name, "You have killed enough monsters for the quest. Go report to the captain.", 2005));
                        }
                    }
                    if (User.Level >= 70)
                    {
                        foreach (Character Member in User.Team)
                        {
                            if (!Member.Alive)
                                continue;

                            double PlvlExp = 1;
                            if (Mob.Level - 20 <= Member.Level)
                                PlvlExp = 0.1;
                            if (Member.Level + 20 < User.Level)
                                if (Member.LocMap == User.LocMap)
                                    if (MyMath.PointDistance(Member.LocX, Member.LocY, User.LocX, User.LocY) < 30)
                                        if (Member.AddExp((ulong)((double)(52 + (Member.Level * 30)) * PlvlExp), true))
                                        {
                                            User.VP += (uint)((Member.Level * 17 / 13 * 12 / 2) + Member.Level * 3);
                                            Member.MyTeamLeader.MyClient.SendPacket(General.MyPackets.SendMsg(Member.MyClient.MessageId, "SYSTEM", Member.Name, User.Name + " has gained " + ((Member.Level * 17 / 13 * 12 / 2) + Member.Level * 3) + " virtue points.", 2003));
                                            foreach (Character Member2 in User.Team)
                                            {
                                                if (Member2 != null)
                                                    Member2.MyClient.SendPacket(General.MyPackets.SendMsg(Member2.MyClient.MessageId, "SYSTEM", Member2.Name, User.Name + " has gained " + ((Member.Level * 17 / 13 * 12 / 2) + Member.Level * 3) + " virtue points.", 2003));
                                            }
                                        }
                        }
                    }
                    if (User.MobTarget == Mob)
                    {
                        User.TargetUID = 0;
                        User.MobTarget = null;
                        User.Attacking = false;
                    }
                    User.XpCircle++;
                    if (User.CycloneOn || User.SMOn)
                        User.ExtraXP += 820;
                    if (User.MobTarget != null)
                        if (Mob == User.MobTarget)
                        {
                            User.MobTarget = null;
                        }

                    EAddExp = MobCurHP * ExpQuality;
                    UAddExp = Convert.ToUInt32(EAddExp);
                    GotXP += UAddExp;
                    if (SkillId != 5030)
                        GotSkilExp += (uint)((double)MobCurHP * ExpQuality);
                    else
                        GotSkilExp += (uint)(5 * (SkillLvl + 1));

                    AttackMob(User, Mob, 14, 0);

                    MobDissappear(Mob);
                    Mob.Death = DateTime.Now;
                }
                else
                {
                    EAddExp = Damage * ExpQuality;
                    UAddExp = Convert.ToUInt32(EAddExp);
                    GotXP += UAddExp;
                    if (SkillId != 5030)
                        GotSkilExp += (uint)((double)Damage * ExpQuality);
                    else
                        GotSkilExp += (uint)(5 * (SkillLvl + 1));
                }
            }
            User.AddExp((ulong)GotXP, true);
            User.AddSkillExp((short)SkillId, GotSkilExp);
        }

        public static void SpawnMobForPlayers(SingleMob Mob, bool Check)
        {
            foreach (DictionaryEntry DE in AllChars)
            {
                Character Charr = (Character)DE.Value;

                if (Mob.Map == Charr.LocMap)
                    if (Charr.MyClient.Online)
                        if (!MyMath.CanSee(Mob.PrevX, Mob.PrevY, Charr.LocX, Charr.LocY) || Check == false)
                            if (MyMath.CanSee(Mob.PosX, Mob.PosY, Charr.LocX, Charr.LocY))
                            {
                                Charr.MyClient.SendPacket(General.MyPackets.SpawnMob(Mob));
                            }
            }
        }

        public static void MobMoves(SingleMob Mob, byte Dir)
        {
            try
            {
                foreach (DictionaryEntry DE in AllChars)
                {
                    Character Charr = (Character)DE.Value;

                    if (Mob.Map == Charr.LocMap)
                        if (Charr.MyClient.Online)
                            if (MyMath.CanSeeBig(Mob.PosX, Mob.PosY, Charr.LocX, Charr.LocY))
                            {
                                Charr.MyClient.SendPacket(General.MyPackets.MobMoves((uint)Mob.UID, Dir));
                            }
                }
            }
            catch { }
        }


        public static void ItemDrops(DroppedItem item)
        {
            try
            {
                foreach (DictionaryEntry DE in AllChars)
                {
                    Character Charr = (Character)DE.Value;

                    if (item.Map == Charr.LocMap)
                        if (Charr.MyClient.Online)
                            if (MyMath.CanSee(item.X, item.Y, Charr.LocX, Charr.LocY))
                                Charr.MyClient.SendPacket(General.MyPackets.ItemDrop(item.UID, item.ItemId, item.X, item.Y));
                }
            }
            catch { }
        }

        public static void ItemDissappears(DroppedItem item)
        {
            try
            {
                foreach (DictionaryEntry DE in AllChars)
                {
                    Character Charr = (Character)DE.Value;

                    if (item.Map == Charr.LocMap)
                        if (Charr.MyClient.Online)
                            if (MyMath.CanSee(item.X, item.Y, Charr.LocX, Charr.LocY))
                            {
                                Charr.MyClient.SendPacket(General.MyPackets.ItemDropRemove(item.UID));
                            }
                }
            }
            catch { }
        }

        public static void SurroundDroppedItems(Character Me, bool Check)
        {
            foreach (DictionaryEntry DE in DroppedItems.AllDroppedItems)
            {
                DroppedItem item = (DroppedItem)DE.Value;

                if (item.Map == Me.LocMap)
                    if (MyMath.CanSee(Me.LocX, Me.LocY, item.X, item.Y))
                        if (!MyMath.CanSee(Me.PrevX, Me.PrevY, item.X, item.Y) || Check == false)
                        {
                            Me.MyClient.SendPacket(General.MyPackets.ItemDrop(item.UID, item.ItemId, item.X, item.Y));
                        }
            }
        }

        public static void MobReSpawn(SingleMob Mob)
        {
            lock (AllChars)
            {
                foreach (DictionaryEntry DE in AllChars)
                {
                    Character Charr = (Character)DE.Value;

                    if (Charr.MyClient.Online)
                        if (Mob.Map == Charr.LocMap)
                            if (MyMath.CanSee(Mob.PosX, Mob.PosY, Charr.LocX, Charr.LocY))
                            {
                                Charr.MyClient.SendPacket(General.MyPackets.SpawnMob(Mob));
                                Charr.MyClient.SendPacket(General.MyPackets.String(Mob.UID, 10, "MBStandard"));
                            }
                }
            }
        }
        public static void GuardReSpawn(SingleMob Mob)
        {
            lock (AllChars)
            {
                foreach (DictionaryEntry DE in AllChars)
                {
                    Character Charr = (Character)DE.Value;

                    if (Charr.MyClient.Online)
                        if (Mob.Map == Charr.LocMap)
                            if (MyMath.CanSee(Mob.PosX, Mob.PosY, Charr.LocX, Charr.LocY))
                            {
                                Charr.MyClient.SendPacket(General.MyPackets.SpawnMob(Mob));
                            }
                }
            }
        }
        public static void MobDissappear(SingleMob Mob)
        {
            foreach (DictionaryEntry DE in AllChars)
            {
                Character Charr = (Character)DE.Value;

                if (Mob.Map == Charr.LocMap)
                    if (Charr.MyClient.Online)
                        if (MyMath.CanSeeBig(Mob.PosX, Mob.PosY, Charr.LocX, Charr.LocY))
                        {
                            Charr.MyClient.SendPacket(General.MyPackets.MobFade(Mob.UID));
                        }
            }
        }

        public static void MobAttacksCharSkill(SingleMob Mob, Character Attacked, uint Dmg, ushort SkillID, byte SkillLvl)
        {
            foreach (DictionaryEntry DE in AllChars)
            {
                Character Charr = (Character)DE.Value;

                if (Charr.MyClient.Online)
                    if (MyMath.CanSeeBig(Attacked.LocX, Attacked.LocY, Charr.LocX, Charr.LocY))
                    {
                        Charr.MyClient.SendPacket(General.MyPackets.MobSkillUse(Mob, Attacked, Dmg, SkillID, SkillLvl));
                    }
            }
        }

        public static void MobAttacksChar(SingleMob Mob, Character Attacked, byte AtkType, uint Dmg)
        {
            if (Mob == null)
                return;
            if (Attacked == null)
                return;
            lock (AllChars)
            {
                foreach (DictionaryEntry DE in AllChars)
                {
                    Character Charr = (Character)DE.Value;

                    if (Charr.MyClient.Online)
                        if (MyMath.CanSee(Attacked.LocX, Attacked.LocY, Charr.LocX, Charr.LocY))
                            Charr.MyClient.SendPacket(General.MyPackets.Attack(Mob.UID, Attacked.UID, (short)Attacked.LocX, (short)Attacked.LocY, AtkType, Dmg));
                }
            }
        }

        public static void PlAttacksTG(SingleNPC Npc, Character Me, byte AtkType, uint Dmg)
        {
            if (Npc == null)
                return;

            foreach (DictionaryEntry DE in AllChars)
            {
                Character Charr = (Character)DE.Value;

                if (Charr.MyClient.Online)
                    if (MyMath.CanSee(Me.LocX, Me.LocY, Charr.LocX, Charr.LocY))
                        Charr.MyClient.SendPacket(General.MyPackets.Attack(Me.UID, Npc.UID, Npc.X, Npc.Y, AtkType, Dmg));

            }
        }

        public static void PVP(Character Me, Character Attacked, byte AtkType, uint Dmg)
        {
            if (Attacked == null)
                return;
            foreach (DictionaryEntry DE in AllChars)
            {
                Character Charr = (Character)DE.Value;

                if (MyMath.CanSeeBig(Me.LocX, Me.LocY, Charr.LocX, Charr.LocY))
                    if (Charr.MyClient.Online)
                    {
                        Charr.MyClient.SendPacket(General.MyPackets.Attack(Me.UID, Attacked.UID, (short)Attacked.LocX, (short)Attacked.LocY, AtkType, Dmg));
                    }
            }
        }
        public static void AttackMiss(Character Me, byte AtkType, short X, short Y)
        {
            foreach (DictionaryEntry DE in AllChars)
            {
                Character Charr = (Character)DE.Value;

                if (Charr != null)
                    if (MyMath.CanSee(Me.LocX, Me.LocY, Charr.LocX, Charr.LocY))
                        if (Charr.MyClient.Online)
                        {
                            Charr.MyClient.SendPacket(General.MyPackets.Attack(Me.UID, 0, X, Y, AtkType, 0));
                        }
            }
        }
        public static void AttackMob(Character Me, SingleMob Mob, byte AtkType, uint Dmg)
        {
            foreach (DictionaryEntry DE in AllChars)
            {
                Character Charr = (Character)DE.Value;

                if (Charr != null)
                    if (MyMath.CanSeeBig(Me.LocX, Me.LocY, Charr.LocX, Charr.LocY))
                        if (Charr.MyClient.Online)
                        {
                            Charr.MyClient.SendPacket(General.MyPackets.Attack(Me.UID, Mob.UID, Mob.PosX, Mob.PosY, AtkType, Dmg));
                        }
            }
        }

        public static void UpdateSpawn(Character Me)
        {
            foreach (DictionaryEntry DE in AllChars)
            {
                Character Charr = (Character)DE.Value;

                if (Me.LocMap == Charr.LocMap)
                    if (MyMath.CanSee(Me.LocX, Me.LocY, Charr.LocX, Charr.LocY))
                    {
                        if (Me.MyGuild != null)
                            Charr.MyClient.SendPacket(General.MyPackets.GuildName(Me.GuildID, Me.MyGuild.GuildName));
                        Charr.MyClient.SendPacket(General.MyPackets.SpawnEntity(Me));
                        if (Me.RBCount > 1)
                            Charr.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "2NDMetempsychosis"));
                        if (Me.MyGuild != null)
                            Charr.MyClient.SendPacket(General.MyPackets.GuildName(Me.GuildID, Me.MyGuild.GuildName));
                        if (Me.Rank == 7)
                            Charr.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter1"));
                        if (Me.Rank == 6)
                            Charr.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter2"));
                        if (Me.Rank == 5)
                            Charr.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter3"));
                        if (Me.Rank == 4)
                            Charr.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter4"));
                        if (Me.Rank == 3)
                            Charr.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter5"));
                        if (Me.Rank == 2)
                        {
                            Charr.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "coronet3"));
                            Charr.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter6"));
                        }
                        if (Me.Rank == 1)
                        {
                            Charr.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "coronet4"));
                            Charr.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter7"));
                        }
                    }
            }
        }
        public static void SurroundMobs(Character Me, bool Check)
        {
            foreach (DictionaryEntry DE in Mobs.AllMobs)
            {
                SingleMob Mob = (SingleMob)DE.Value;

                if (Me.LocMap == Mob.Map)
                    if (Mob.Alive)
                        if (MyMath.CanSee(Me.LocX, Me.LocY, Mob.PosX, Mob.PosY))
                            if (!MyMath.CanSee(Me.PrevX, Me.PrevY, Mob.PosX, Mob.PosY) || Check == false)
                            {
                                Me.MyClient.SendPacket(General.MyPackets.SpawnMob(Mob));
                            }
            }
        }

        public static void SurroundNPCs(Character Me, bool Check)
        {
            foreach (DictionaryEntry DE in NPCs.AllNPCs)
            {
                SingleNPC Npc = (SingleNPC)DE.Value;

                if (Me.LocMap == Npc.Map)
                    if (MyMath.CanSee(Me.LocX, Me.LocY, Npc.X, Npc.Y))
                        if (!MyMath.CanSee(Me.PrevX, Me.PrevY, Npc.X, Npc.Y) || Check == false)
                        {
                            if (Npc.Sob == 0)
                                Me.MyClient.SendPacket(General.MyPackets.SpawnNPC(Npc));
                            else if (Npc.Sob == 1)
                                Me.MyClient.SendPacket(General.MyPackets.SpawnSobNPC(Npc));
                            else if (Npc.Sob == 2)
                            {
                                if (World.PoleHolder != null)
                                    Me.MyClient.SendPacket(General.MyPackets.SpawnSobNPCNamed(Npc, World.PoleHolder.GuildName));
                                else
                                    Me.MyClient.SendPacket(General.MyPackets.SpawnSobNPCNamed(Npc, "Pole"));
                            }
                            else if (Npc.Sob == 3)
                                Me.MyClient.SendPacket(General.MyPackets.SpawnSob(Npc));
                            else if (Npc.Sob == 4)
                                Me.MyClient.SendPacket(General.MyPackets.SpawnShopFlag(Npc));
                        }
            }
        }

        public static void SendMsgToAll(string Message, string From, short MsgType)
        {
            foreach (DictionaryEntry DE in AllChars)
            {
                Character Charr = (Character)DE.Value;
                if (Charr.MyClient.Online)
                    Charr.MyClient.SendPacket(General.MyPackets.SendMsg(Charr.MyClient.MessageId, From, "All", Message, MsgType));
            }
        }


        public static void Chat(Character Char, short ChatType, byte[] Data, string To, string Message)
        {
            if (ChatType == 2010)
            {
                if (Char.CPs >= 5)
                {
                    Char.CPs -= 5;
                    Char.MyClient.SendPacket(General.MyPackets.Vital(Char.UID, 30, Char.CPs));
                    SendMsgToAll(Message, Char.Name, 2010);
                }
            }
            if (ChatType == 2004)
            {
                Char.MyGuild.GuildMessage(Char, Message, To);
            }
            if (ChatType == 2009)
            {
                foreach (DictionaryEntry DE in Char.Friends)
                {
                    uint FriendID = (uint)DE.Key;
                    if (AllChars.Contains(FriendID))
                    {
                        Character Friend = (Character)AllChars[FriendID];
                        if (Friend != null)
                        {
                            Friend.MyClient.SendPacket(Data);
                        }
                    }
                }
            }
            if (ChatType == 2003)
            {
                foreach (Character Member in Char.Team)
                {
                    if (Member != null)
                    {
                        Member.MyClient.SendPacket(Data);
                    }
                }
                if (!Char.TeamLeader && Char.MyTeamLeader != null)
                    Char.MyTeamLeader.MyClient.SendPacket(Data);
            }
            if (ChatType == 2000)
            {
                foreach (DictionaryEntry DE in AllChars)
                {
                    Character Charr = (Character)DE.Value;
                    if (Char.LocMap == Charr.LocMap)
                        if (Char != Charr)
                            if (MyMath.PointDistance(Charr.LocX, Charr.LocY, Char.LocX, Char.LocY) < 16)
                                Charr.MyClient.SendPacket(Data);
                }
            }
            if (ChatType == 2001)
            {
                bool Sent = false;
                foreach (DictionaryEntry DE in AllChars)
                {
                    Character Charr = (Character)DE.Value;
                    if (Charr.Name == To)
                    {
                        Charr.MyClient.SendPacket(Data);
                        Sent = true;
                    }

                }
                if (!Sent)
                    Char.MyClient.SendPacket(General.MyPackets.SendMsg(Char.MyClient.MessageId, "SYSTEM", Char.Name, "The character is offline at the moment.", 2000));
            }

        }

        public static void RemoveEntity(Character Who)
        {
            try
            {
                foreach (DictionaryEntry DE in AllChars)
                {
                    Character Charr = (Character)DE.Value;

                    if (Charr != Who)
                        if (Charr.MyClient.Online)
                            if (MyMath.CanSee(Who.LocX, Who.LocY, Charr.LocX, Charr.LocY))
                                Charr.MyClient.SendPacket(General.MyPackets.GeneralData(Who.UID, 0, 0, 0, 132));

                }
            }
            catch (Exception Exc) { General.WriteLine(Exc.ToString()); }
        }

        public static void RemoveEntity(SingleMob Who)
        {
            try
            {
                foreach (DictionaryEntry DE in AllChars)
                {
                    Character Charr = (Character)DE.Value;
                    if (Charr.MyClient.Online)
                        if (MyMath.CanSeeBig(Who.PosX, Who.PosY, Charr.LocX, Charr.LocY))
                            Charr.MyClient.SendPacket(General.MyPackets.GeneralData(Who.UID, 0, 0, 0, 132));
                }
            }
            catch (Exception Exc) { General.WriteLine(Exc.ToString()); }
        }

        public static void RemoveEntity(SingleNPC Who)
        {
            try
            {
                foreach (DictionaryEntry DE in AllChars)
                {
                    Character Charr = (Character)DE.Value;
                    if (MyMath.CanSee(Who.X, Who.Y, Charr.LocX, Charr.LocY))
                        if (Charr.MyClient.Online)
                            Charr.MyClient.SendPacket(General.MyPackets.GeneralData(Who.UID, 0, 0, 0, 132));


                } // con.SystemChat("You have won a " + ItemName, ChatType.Top, 0x00FF00);
            }
            catch (Exception Exc) { General.WriteLine(Exc.ToString()); }
        }

        public static void PlayersOffLottery()
        {
            try
            {
                foreach (DictionaryEntry DE in AllChars)
                {
                    Character Charr = (Character)DE.Value;
                    if (Charr.LocMap == 700)
                    {
                        Charr.CPs += 27;
                        Charr.MyClient.SendPacket(General.MyPackets.Vital(Charr.UID, 30, Charr.CPs));
                        Charr.Teleport(1036, 200, 200);


                    }
                }
            }
            catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); }
        }

        public static void SaveAllChars()
        {
            try
            {
                foreach (DictionaryEntry DE in AllChars)
                {
                    Character Charr = (Character)DE.Value;
                    InternalDatabase.SaveChar(Charr);
                }
                Guilds.SaveAllGuilds();
            }
            catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); }
        }

        public static void PlayerMoves(Character MovingChar, byte[] Data)
        {
            try
            {
                foreach (DictionaryEntry DE in AllChars)
                {
                    Character Charr = (Character)DE.Value;

                    if (Charr.LocMap == MovingChar.LocMap)
                        if (MyMath.CanSeeBig(Charr.LocX, Charr.LocY, MovingChar.LocX, MovingChar.LocY))
                            Charr.MyClient.SendPacket(Data);
                }
            }
            catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); }
        }

        public static void SpawnMeToOthers(Character Me, bool Check)
        {
            try
            {
                if (AllChars.Contains(Me.UID))
                {
                    foreach (DictionaryEntry DE in AllChars)
                    {
                        Character SpawnTo = (Character)DE.Value;

                        if (Me != SpawnTo)
                            if (SpawnTo.MyClient.Online)
                                if (Me.LocMap == SpawnTo.LocMap)
                                    if (MyMath.CanSee(Me.LocX, Me.LocY, SpawnTo.LocX, SpawnTo.LocY))
                                        if (!MyMath.CanSee(Me.PrevX, Me.PrevY, SpawnTo.LocX, SpawnTo.LocY) || !Check)
                                        {
                                            if (Me.MyGuild != null)
                                                SpawnTo.MyClient.SendPacket(General.MyPackets.GuildName(Me.GuildID, Me.MyGuild.GuildName));

                                            SpawnTo.MyClient.SendPacket(General.MyPackets.SpawnEntity(Me));

                                            if (Me.RBCount > 1)
                                                SpawnTo.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "2NDMetempsychosis"));

                                            if (Me.MyGuild != null)
                                                SpawnTo.MyClient.SendPacket(General.MyPackets.GuildName(Me.GuildID, Me.MyGuild.GuildName));
                                            if (Me.Rank == 7)
                                                SpawnTo.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter1"));
                                            if (Me.Rank == 6)
                                                SpawnTo.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter2"));
                                            if (Me.Rank == 5)
                                                SpawnTo.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter3"));
                                            if (Me.Rank == 4)
                                                SpawnTo.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter4"));
                                            if (Me.Rank == 3)
                                                SpawnTo.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter5"));
                                            if (Me.Rank == 2)
                                            {
                                                SpawnTo.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "coronet3"));
                                                SpawnTo.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter6"));
                                            }
                                            if (Me.Rank == 1)
                                            {
                                                SpawnTo.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "coronet4"));
                                                SpawnTo.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter7"));
                                            }
                                        }

                    }
                }
            }
            catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); }
        }
        public static void SpawnOthersToMe(Character Me, bool Check)
        {
            try
            {
                if (AllChars.Contains(Me.UID))
                {
                    foreach (DictionaryEntry DE in AllChars)
                    {
                        Character SpawnWho = (Character)DE.Value;

                        if (Me != SpawnWho)
                            if (SpawnWho.MyClient.Online)
                                if (Me.LocMap == SpawnWho.LocMap)
                                    if (MyMath.CanSee(Me.LocX, Me.LocY, SpawnWho.LocX, SpawnWho.LocY))
                                        if (!MyMath.CanSee(Me.PrevX, Me.PrevY, SpawnWho.LocX, SpawnWho.LocY) || Check == false)
                                        {
                                            if (SpawnWho.MyGuild != null)
                                                Me.MyClient.SendPacket(General.MyPackets.GuildName(SpawnWho.GuildID, SpawnWho.MyGuild.GuildName));

                                            Me.MyClient.SendPacket(General.MyPackets.SpawnEntity(SpawnWho));

                                            if (SpawnWho.RBCount > 1)
                                                Me.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "2NDMetempsychosis"));

                                            if (SpawnWho.MyGuild != null)
                                                Me.MyClient.SendPacket(General.MyPackets.GuildName(SpawnWho.GuildID, SpawnWho.MyGuild.GuildName));
                                            if (SpawnWho.Rank == 7)
                                                Me.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter1"));
                                            if (SpawnWho.Rank == 6)
                                                Me.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter2"));
                                            if (SpawnWho.Rank == 5)
                                                Me.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter3"));
                                            if (SpawnWho.Rank == 4)
                                                Me.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter4"));
                                            if (SpawnWho.Rank == 3)
                                                Me.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter5"));
                                            if (SpawnWho.Rank == 2)
                                            {
                                                Me.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "coronet3"));
                                                Me.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter6"));
                                            }
                                            if (SpawnWho.Rank == 1)
                                            {
                                                Me.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "coronet4"));
                                                Me.MyClient.SendPacket(General.MyPackets.String(Me.UID, 10, "letter7"));
                                            }
                                        }
                    }
                }
            }
            catch (Exception Exc) { General.WriteLine(Convert.ToString(Exc)); }
        }
    }
}