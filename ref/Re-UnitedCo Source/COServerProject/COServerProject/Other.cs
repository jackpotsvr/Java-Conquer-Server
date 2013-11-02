using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace COServer_Project
{
    public class Other
    {
        public static uint EquipNextLevel(uint ItemId)
        {
            uint NewItem = ItemId;

            if (Other.ArmorType(ItemId) == false || Other.WeaponType(ItemId) == 117)
            {
                if (Other.ItemType2(ItemId) != 12 && Other.ItemType2(ItemId) != 15 && Other.ItemType2(ItemId) != 16 || Other.ItemInfo(ItemId)[3] == 45 && Other.ItemType2(ItemId) == 12 || Other.ItemInfo(ItemId)[3] >= 112 && Other.ItemType2(ItemId) == 12)
                    NewItem += 10;
                else if (Other.ItemType2(ItemId) == 12 && Other.ItemInfo(ItemId)[3] < 45)
                    NewItem += 20;
                else if (Other.ItemType2(ItemId) == 12 && Other.ItemInfo(ItemId)[3] >= 52 && Other.ItemInfo(ItemId)[3] < 112)
                    NewItem += 30;
                else if (Other.ItemType2(ItemId) == 15 && Other.ItemInfo(ItemId)[3] == 1 || Other.ItemType2(ItemId) == 15 && Other.ItemInfo(ItemId)[3] >= 110)
                    NewItem += 10;
                else if (Other.ItemType2(ItemId) == 15)
                    NewItem += 20;
                else if (Other.ItemType2(ItemId) == 16 && Other.ItemInfo(ItemId)[3] < 124)
                    NewItem += 20;
                else if (Other.ItemType2(ItemId) == 16)
                    NewItem += 10;

                if (Other.WeaponType(NewItem) == 421)
                {
                    NewItem = ItemId;
                    if (Other.ItemInfo(ItemId)[3] == 45 || Other.ItemInfo(ItemId)[3] == 55)
                        NewItem += 20;
                    else
                        NewItem += 10;
                }
            }
            else if (Other.ItemType2(ItemId) != 12 && Other.ItemType2(ItemId) != 15)
            {
                if (Other.ItemInfo(ItemId)[1] == 21)
                    if (Other.ItemType2(ItemId) == 13)
                    {
                        if (Other.ItemInfo(ItemId)[3] < 110)
                            NewItem += 10;
                        else
                            NewItem += 5000;
                    }
                if (Other.ItemInfo(ItemId)[1] == 11)
                    if (Other.ItemType2(ItemId) == 13)
                    {
                        if (Other.ItemInfo(ItemId)[3] < 110)
                            NewItem += 10;
                        else
                            NewItem += 5000;
                    }
                if (Other.ItemInfo(ItemId)[1] == 40)
                    if (Other.ItemType2(ItemId) == 13)
                    {
                        if (Other.ItemInfo(ItemId)[3] < 112)
                            NewItem += 10;
                        else
                            NewItem += 5000;
                    }
                if (Other.ItemInfo(ItemId)[1] == 190)
                    if (Other.ItemType2(ItemId) == 13)
                    {
                        if (Other.ItemInfo(ItemId)[3] < 115)
                            NewItem += 10;
                        else
                            NewItem += 5000;
                    }
                if (Other.ItemInfo(ItemId)[1] == 21)
                    if (Other.ItemType2(ItemId) == 11)
                    {
                        if (Other.ItemInfo(ItemId)[3] < 112)
                            NewItem += 10;
                        else
                            NewItem += 920;
                    }
                if (Other.ItemInfo(ItemId)[1] == 11)
                    if (Other.ItemType2(ItemId) == 11)
                    {
                        if (Other.ItemInfo(ItemId)[3] < 112)
                            NewItem += 10;
                        else
                            NewItem -= 6010;
                    }
                if (Other.ItemInfo(ItemId)[1] == 40)
                    if (Other.ItemType2(ItemId) == 11)
                    {
                        if (Other.ItemInfo(ItemId)[3] < 117)
                            NewItem += 10;
                        else
                            NewItem -= 1060;
                    }
                if (Other.ItemInfo(ItemId)[1] == 190)
                    if (Other.ItemType2(ItemId) == 11)
                    {
                        if (Other.ItemInfo(ItemId)[3] < 112)
                            NewItem += 10;
                        else
                            NewItem -= 2050;
                    }

            }

            if (ItemId == 500301)
                NewItem = 500005;

            if (ItemId == 410301)
                NewItem = 410005;

            return NewItem;
        }
        public static bool NoPK(ushort Map)
        {
            bool Rt = false;

            foreach (ushort map in ExternalDatabase.NoPKMaps)
            {
                if (map == Map)
                {
                    Rt = true;
                    break;
                }
            }
            return Rt;
        }
        public static bool CanPK(ushort Map)
        {
            bool Rt = false;

            foreach (uint map in ExternalDatabase.PKMaps)
            {
                if (map == Map)
                {
                    Rt = true;
                    break;
                }
            }
            return Rt;
        }

        public static uint CalculateDamage(Character Attacker, Character Attacked, byte AttackType, ushort SkillId, byte SkillLvl)
        {
            int Damage = 0;
            ushort[] SkillAttributes = ExternalDatabase.SkillAttributes[SkillId][SkillLvl];
            int ExtraDamage = (int)SkillAttributes[3];

            if (AttackType == 0 || AttackType == 1)//Melee
            {
                Damage = General.Rand.Next((int)Attacker.MinAtk, (int)Attacker.MaxAtk);
                if (AttackType == 0)
                    Damage += ExtraDamage;
                else
                    Damage = (int)(((double)ExtraDamage / 100) * Damage);

                Damage = (int)((double)Damage * Attacker.AddAtkPc);

                if (Attacker.StigBuff)
                    Damage = (int)(Damage*(double)(1 + ((double)(10 + Attacker.StigLevel) / 100)));

                if (Attacker.SMOn)
                    Damage *= 2;

                Damage -= Attacked.Defense;

                if (Damage < 1)
                    Damage = 1;
            }
            else if (AttackType == 2)//Ranged
            {
                double reborn = 1;

                int Atk = General.Rand.Next((int)Attacker.MinAtk, (int)Attacker.MaxAtk);
                Atk += ExtraDamage;

                Atk = (int)((double)Atk * Attacker.AddAtkPc);

                if (Attacker.SMOn)
                    Atk *= 2;
                if (Attacker.StigBuff)
                    Atk = (int)(Atk * (double)(1 + ((double)(10 + Attacker.StigLevel) / 100)));

                if (Attacked.RBCount == 1)
                    reborn = 0.7;
                else if (Attacked.RBCount == 2)
                    reborn = 0.4;

                Damage = (int)(((Atk - Attacked.Defense) * reborn) * (double)(Attacked.Dodge/100));

                if (Damage < 1)
                    Damage = 1;
            }
            else if (AttackType == 3 || AttackType == 4)//Magic
            {
                double reborn = 1;
                if (AttackType == 3)
                    Damage = (int) + ExtraDamage;
                else
                    Damage = (int)((double)ExtraDamage * Attacker.MAtk);
                if(Attacked.RBCount == 1)
                    reborn = 0.7;
                else if (Attacked.RBCount == 2)
                    reborn = 0.4;
                Damage = (int)((Attacker.MAtk - Attacked.MDefense) * reborn);

                //Atk = (float)(((((Atk - Defense) - PlusDef) * Reborn) * Blessed));

                if (Damage < 1)
                    Damage = 1;
            }
            return (uint)Damage;
        }
        public static uint CalculateDamage(Character Attacker, SingleMob Attacked, byte AttackType, ushort SkillId, byte SkillLvl)
        {
            int Damage = 0;
            ushort[] SkillAttributes = ExternalDatabase.SkillAttributes[SkillId][SkillLvl];
            int ExtraDamage = (int)SkillAttributes[3];

            if (AttackType == 0 || AttackType == 1)//Melee
            {
                Damage = General.Rand.Next((int)Attacker.MinAtk, (int)Attacker.MaxAtk);
                if (AttackType == 0)
                    Damage += ExtraDamage;
                else
                    Damage = (int)(((double)ExtraDamage / 100) * Damage);

                Damage = (int)((double)Damage * Attacker.AddAtkPc);

                double LDF = (Attacker.Level - Attacked.Level + 7) / 5;
                LDF = Math.Max(LDF, 1);
                double eDMG = ((Convert.ToUInt32(LDF) - 1) * .8) + 1;

                Damage = (int)(Damage * eDMG);

                if (Attacker.SMOn)
                    Damage *= 10;

                if (Attacker.StigBuff)
                    Damage = (int)(Damage * (double)(1 + ((double)(10 + Attacker.StigLevel) / 100)));
            }
            else if (AttackType == 2)//Ranged
            {
                Damage = General.Rand.Next((int)Attacker.MinAtk, (int)Attacker.MaxAtk);
                Damage = (int)(((double)ExtraDamage / 100) * Damage);
                Damage = (int)((double)Damage * ((double)(100 - Attacked.Dodge) / 100));

                Damage = (int)((double)Damage * Attacker.AddAtkPc);

                double LDF = (Attacker.Level - Attacked.Level + 7) / 5;
                LDF = Math.Max(LDF, 1);
                double eDMG = ((Convert.ToUInt32(LDF) - 1) * .8) + 1;

                Damage = (int)(Damage * eDMG);

                if (Attacker.SMOn)
                    Damage *= 10;

                if (Attacker.StigBuff)
                    Damage = (int)(Damage * (double)(1 + ((double)(10 + Attacker.StigLevel) / 100)));
            }
            else if (AttackType == 3 || AttackType == 4)//Magic
            {
                if (AttackType == 3)
                    Damage = (int)Attacker.MAtk + ExtraDamage;
                else
                    Damage = (int)(((double)ExtraDamage / 100) * Attacker.MAtk);

                Damage = (int)((double)Damage * Attacker.AddAtkPc);

                double LDF = (Attacker.Level - Attacked.Level + 7) / 5;
                LDF = Math.Max(LDF, 1);
                double eDMG = ((Convert.ToUInt32(LDF) - 1) * .8) + 1;

                Damage = (int)(Damage * eDMG);
                Damage = (int)((double)Damage * Attacker.AddMAtkPc);
            }
            if (Attacked.MType == 1)
                Damage /= 100;


            return (uint)Damage;
        }

        /*
Attacking a Non Reborn=1
Attacking a 1st Reborn=.7
Non or 1st Reborn Attacking 2nd Reborn=.5
2nd Reborn Attacking 2nd Reborn=.7

         * */
        public static uint CalculateDamage(Character Attacker, SingleNPC Attacked, byte AttackType, ushort SkillId, byte SkillLvl)
        {
            int Damage = 0;
            ushort[] SkillAttributes = ExternalDatabase.SkillAttributes[SkillId][SkillLvl];
            int ExtraDamage = (int)SkillAttributes[3];

            if (AttackType == 0 || AttackType == 1)//Melee
            {
                Damage = General.Rand.Next((int)Attacker.MinAtk, (int)Attacker.MaxAtk);
                if (AttackType == 0)
                    Damage += ExtraDamage;
                else
                    Damage = (int)(((double)ExtraDamage / 100) * Damage);

                Damage = (int)((double)Damage * Attacker.AddAtkPc);
                if (Attacker.StigBuff)
                    Damage = (int)(Damage * (double)(1 + ((double)(10 + Attacker.StigLevel) / 100)));
            }
            else if (AttackType == 2)//Ranged
            {
                Damage = General.Rand.Next((int)Attacker.MinAtk, (int)Attacker.MaxAtk);
                Damage = (int)((double)Damage * ((double)ExtraDamage / 100));
                Damage = (int)((double)Damage * Attacker.AddAtkPc);
                Damage = (int)((double)Damage * ((double)(100 - Attacked.Dodge) / 100));

                if (Attacker.StigBuff)
                    Damage = (int)(Damage * (double)(1 + ((double)(10 + Attacker.StigLevel) / 100)));
            }
            else if (AttackType == 3 || AttackType == 4)//Magic
            {
                if (AttackType == 3)
                    Damage = (int)Attacker.MAtk + ExtraDamage;
                else
                    Damage = (int)(((double)ExtraDamage / 100) * Attacker.MAtk);
                Damage = (int)((double)Damage * Attacker.AddMAtkPc);
            }
            return (uint)Damage;
        }

        public static bool ChanceSuccess(double pc)
        {
            return ((double)General.Rand.Next(1, 1000000)) / 10000 >= 100 - pc;
        }

        public static uint GenerateGarment()
        {
            uint Item = 0;
            int Count = 0;
            int Do = General.Rand.Next(3, 30);
            foreach (uint[] item in ExternalDatabase.Items)
            {
                if (Other.ItemType2(item[0]) == 18)
                {
                    Count++;
                    Item = item[0];
                    if (Count == Do)
                        break;
                }

            }
            return Item;
        }
        public static uint GenerateCrap()
        {
            uint Item = 723700;

            if (ChanceSuccess(20))
                Item = 723711;
            else if (ChanceSuccess(25))
                Item = 723712;
            else if (ChanceSuccess(30))
                Item = 1088001;
            else if (ChanceSuccess(30))
                Item = 730001;
            else if (ChanceSuccess(40))
            {
                Item = (uint)(700002 + (General.Rand.Next(7) * 10));
            }

            return Item;
        }
        public static uint GenerateSpecial()
        {
            uint Item = 723584;

            if (ChanceSuccess(5))
                Item = 722840;
            else if (ChanceSuccess(5))
                Item = 722841;
            else if (ChanceSuccess(5))
                Item = 722842;
            else if (ChanceSuccess(5))
                Item = 723701;
            else if (ChanceSuccess(5))
                Item = 1200000;
            else if (ChanceSuccess(5))
                Item = 1200001;
            else if (ChanceSuccess(5))
                Item = 1200002;
            else if (ChanceSuccess(5))
                Item = 1088000;
            else if (ChanceSuccess(25))
            {
                Item = (uint)(700003 + (General.Rand.Next(7) * 10));
            }

            return Item;
        }

        public static uint GenerateEtc()
        {
            if (ChanceSuccess(50))
            {
                uint Item = 723713;
                Item += (uint)General.Rand.Next(0, 7);
                if (ChanceSuccess(3))
                    Item = 723721;
                if (ChanceSuccess(2))
                    Item = 723722;
                if (ChanceSuccess(1))
                    Item = 723723;
                return Item;
            }
            else
            {
                uint Item = 730002;
                Item += (uint)General.Rand.Next(0, 3);
                if (ChanceSuccess(3))
                    Item = 730006;
                if (ChanceSuccess(2))
                    Item = 730007;
                if (ChanceSuccess(0.8))
                    Item = 730008;
                if (ChanceSuccess(0.3))
                    Item = 730009;
                return Item;
            }
        }

        public static uint GenerateEquip(byte Level, byte Quality)
        {
            uint returns = 0;
            int tries;
            if (Other.ChanceSuccess(35))
                tries = General.Rand.Next(1, 8);
            else if (Other.ChanceSuccess(35))
                tries = General.Rand.Next(1, 16);
            else
                tries = General.Rand.Next(1, 32);
            int count = 0;
            byte ItemType1 = 0;
            short ItemType0 = 0;
            int nr = General.Rand.Next(1, 7);
            if (nr == 1)
                ItemType1 = 11;
            else if (nr == 2)
                ItemType1 = 12;
            else if (nr == 3)
                ItemType1 = 13;
            else if (nr == 4)
                ItemType1 = 4;
            else if (nr == 5)
                ItemType1 = 5;
            else if (nr == 6)
                ItemType1 = 15;
            else if (nr == 7)
                ItemType1 = 16;

            nr = General.Rand.Next(1, 17);
            if (nr == 1)
                ItemType0 = 410;
            else if (nr == 2)
                ItemType0 = 420;
            else if (nr == 3)
                ItemType0 = 421;
            else if (nr == 4)
                ItemType0 = 430;
            else if (nr == 5)
                ItemType0 = 440;
            else if (nr == 6)
                ItemType0 = 450;
            else if (nr == 7)
                ItemType0 = 460;
            else if (nr == 8)
                ItemType0 = 480;
            else if (nr == 9)
                ItemType0 = 481;
            else if (nr == 10)
                ItemType0 = 490;
            else if (nr == 11)
                ItemType0 = 500;
            else if (nr == 12)
                ItemType0 = 510;
            else if (nr == 13)
                ItemType0 = 530;
            else if (nr == 14)
                ItemType0 = 540;
            else if (nr == 15)
                ItemType0 = 560;
            else if (nr == 16)
                ItemType0 = 561;
            else if (nr == 17)
                ItemType0 = 580;

            foreach (uint[] item in ExternalDatabase.Items)
            {

                if (item[3] - 4 < Level && item[3] + 4 > Level)
                    if (ItemQuality(item[0]) == Quality)
                        if (ItemType(item[0]) == 1 || ItemType(item[0]) == 4 || ItemType(item[0]) == 5 || ItemType(item[5]) == 9)
                            if (ItemType2(item[0]) == ItemType1 || (ItemType1 == 4 && WeaponType(item[0]) == ItemType0) || (ItemType1 == 5 && WeaponType(item[0]) == ItemType0))
                            {
                                count++;
                                returns = item[0];

                                if (count >= tries)
                                    break;
                            }

            }
            return returns;
        }

        public static Character CharNearest(uint X, uint Y, uint Map, bool Blue)
        {
            try
            {
                int ShortestDist = 9999;
                Character NearestChar = null;
                foreach(DictionaryEntry DE in World.AllChars)
                {
                    Character Charr = (Character)DE.Value;
                    if (Map == Charr.LocMap)
                        if (MyMath.PointDistance(X, Y, Charr.LocX, Charr.LocY) < ShortestDist)
                            if (Blue && Charr.BlueName || !Blue)
                            {
                                NearestChar = Charr;
                                ShortestDist = MyMath.PointDistance(X, Y, Charr.LocX, Charr.LocY);
                            }

                }
                return NearestChar;
            }
            catch (Exception Exc) { General.WriteLine(Exc.ToString()); return null; }
        }
        public static Character Charowner(uint uid)
        {
            try
            {

                Character ownerchar = null;
                foreach (DictionaryEntry DE in World.AllChars)
                {
                    Character Charr = (Character)DE.Value;
                    if (uid == Charr.UID)

                        ownerchar = Charr;



                }
                return ownerchar;
            }
            catch (Exception Exc) { General.WriteLine(Exc.ToString()); return null; }
        }
        public static bool CharExist(string Needle, string HayStack)
        {
            int Find = HayStack.IndexOf(Needle);
            int Find2 = HayStack.LastIndexOf(Needle);
            return ((Find >= 0) && (Find2 >= 0));
        }
        public static bool CanEquip(string iteM, Character Charr)
        {
            if (iteM.Length < 5)
                return false;
            string[] Splitter = iteM.Split('-');
            bool Returning = true;

            foreach (uint[] item in ExternalDatabase.Items)
            {
                if (item[0] == uint.Parse(Splitter[0]))
                {
                    byte TJob = 0;

                    if (Charr.Job > 129 && Charr.Job < 136)
                        TJob = (byte)(Charr.Job + 60);
                    else if (Charr.Job > 139 && Charr.Job < 146)
                        TJob = (byte)(Charr.Job + 50);
                    else
                        TJob = Charr.Job;

                    if (Charr.Level >= item[3])
                        if (Charr.Str >= item[5])
                            if (Charr.Agi >= item[6])
                                if (TJob - item[1] >= 0 && TJob - item[1] <= 5 || TJob == 0)
                                    if (Charr.Model != 1002 && Charr.Model != 1003 || item[4] == 0)
                                    {
                                        if (ItemType(item[0]) == 4 || ItemType(item[0]) == 5 && WeaponType(item[0]) != 421)
                                        {
                                            if (WeaponType(item[0]) == 500)
                                                Charr.AtkType = 25;
                                            else
                                                Charr.AtkType = 2;

                                            if (Charr.Profs.Contains(WeaponType(item[0])))
                                                if ((uint)Charr.Profs[WeaponType(item[0])] >= item[2])
                                                    Returning = true;
                                                else
                                                    Returning = false;
                                        }
                                        else
                                        {
                                            Returning = true;
                                        }
                                    }
                                    else
                                        Returning = false;
                    break;
                }
            }

            if (Splitter[0] == "1200006" || Splitter[0] == "722343" || Splitter[0] == "722344" || Splitter[0] == "722345" || Splitter[0] == "722346" || Splitter[0] == "722347" || Splitter[0] == "722348" || Splitter[0] == "722348" || Splitter[0] == "722349" || Splitter[0] == "722350" || Splitter[0] == "722351" || Splitter[0] == "722352")
                Returning = false;
            if (Splitter[0] == "137910" || Splitter[0] == "137810" || Splitter[0] == "137710" || Splitter[0] == "137610" || Splitter[0] == "137510" || Splitter[0] == "137410" || Splitter[0] == "137310" && Charr.MyClient.Status != 8)
                Returning = false;

            return Returning;
        }

        public static bool Upgradable(uint item)
        {
            return (ItemType2(item) == 90 || ItemType2(item) == 11 || ItemType2(item) == 12 || ItemType2(item) == 13 || ItemType2(item) == 15 || ItemType2(item) == 16 || ItemType(item) == 4 || ItemType(item) == 5);
        }

        public static bool ArmorType(uint item)
        {
            return (ItemType2(item) == 11 || ItemType2(item) == 13);
        }



        public static uint[] ItemInfo(uint Item)
        {
            uint[] Returns = null;

            foreach (uint[] item in ExternalDatabase.Items)
            {
                if (item[0] == Item)
                {
                    Returns = item;
                }
            }
            return Returns;
        }

        public static bool EquipMaxedLvl(uint Item)
        {
            bool R = false;

            if (ItemType(Item) == 4 )
                if (ItemInfo(Item)[3] == 130)
                    R = true;

            if (ItemType(Item) == 5)
                if (ItemInfo(Item)[3] == 130)
                    R = true;

            if (ItemType2(Item) == 11)
                if (ItemInfo(Item)[3] == 120)
                    R = true;

            if (ItemType2(Item) == 13)
                if (ItemInfo(Item)[3] == 120)
                    R = true;

            if (WeaponType(Item) == 150)
                if (ItemInfo(Item)[3] == 126)
                    R = true;

            if (WeaponType(Item) == 121)
                if (ItemInfo(Item)[3] == 126)
                    R = true;

            if (WeaponType(Item) == 120)
                if (ItemInfo(Item)[3] == 130)
                    R = true;

            if (WeaponType(Item) == 151)
                if (ItemInfo(Item)[3] == 127)
                    R = true;

            if (WeaponType(Item) == 152)
                if (ItemInfo(Item)[3] == 127)
                    R = true;

            if (WeaponType(Item) == 160)
                if (ItemInfo(Item)[3] == 129)
                    R = true;

            return R;
        }

        public static int ItemQuality(uint item)
        {
            string Item = Convert.ToString(item);
            string qual = Item.Remove(0, Item.Length - 1);

            return int.Parse(qual);
        }

        public static uint ItemQualityChange(uint item, byte To)
        {
            string Item = Convert.ToString(item);
            Item = Item.Remove(Item.Length - 1, 1);
            Item = Item + To.ToString();

            return uint.Parse(Item);
        }

        public static int WeaponType(uint item)
        {
            string Item = Convert.ToString(item);
            string type = Item.Remove(3, Item.Length - 3);

            return int.Parse(type);
        }

        public static int ItemType(uint item)
        {
            string Item = Convert.ToString(item);
            string type = Item.Remove(1, Item.Length - 1);

            return int.Parse(type);
        }

        public static int ItemType2(uint item)
        {
            string Item = Convert.ToString(item);
            string type = Item.Remove(2, Item.Length - 2);

            return int.Parse(type);
        }

        public static string DisplayPacket(byte[] Pack)
        {
            string pack = "";

            foreach (byte b in Pack)
            {
                pack += Convert.ToString(b) + " ";
            }
            pack.Remove(pack.Length - 1, 1);

            return pack;
        }
        public static string DisplayPacketC(byte[] Pack)
        {
            string pack = "";

            foreach (byte b in Pack)
            {
                pack += Convert.ToChar(b).ToString();
            }

            return pack;
        }
        public static bool PlaceFree(short X, short Y, byte MoveDir)
        {
            bool Ret = true;

            switch (MoveDir)
            {
                case 0:
                    {
                        Y += 1;
                        break;
                    }
                case 1:
                    {
                        X -= 1;
                        Y += 1;
                        break;
                    }
                case 2:
                    {
                        X -= 1;
                        break;
                    }
                case 3:
                    {
                        X -= 1;
                        Y -= 1;
                        break;
                    }
                case 4:
                    {
                        Y -= 1;
                        break;
                    }
                case 5:
                    {
                        X += 1;
                        Y -= 1;
                        break;
                    }
                case 6:
                    {
                        X += 1;
                        break;
                    }
                case 7:
                    {
                        Y += 1;
                        X += 1;
                        break;
                    }
            }

            foreach (DictionaryEntry DE in Mobs.AllMobs)
            {
                SingleMob Mob = (SingleMob)DE.Value;
                if (Mob.PosX == X)
                    if (Mob.PosY == Y)
                        Ret = false;
            }
            return Ret;
        }
    }
    public static class MyMath
    {
        public static int rol(int value, int places, int len)
        {
            return (value << places) | (value >> (len - places));
        }

        public static int ror(int value, int places, int len)
        {
            return (value >> places) | (value << len - places);
        }

        public static int PointDistance(double x1, double y1, double x2, double y2)
        {
            return (int)Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
        }

        public static bool CanSee(double x1, double y1, double x2, double y2)
        {
            return (Math.Max(Math.Abs(x1 - x2), Math.Abs(y1 - y2)) <= 15);
        }

        public static bool CanSeeBig(double x1, double y1, double x2, double y2)
        {
            return (Math.Max(Math.Abs(x1 - x2), Math.Abs(y1 - y2)) <= 30);
        }


        public static double PointDirecton(double x1, double y1, double x2, double y2)
        {
            double direction = 0;

            double AddX = x2 - x1;
            double AddY = y2 - y1;
            double r = (double)Math.Atan2(AddY, AddX);

            if (r < 0) r += (double)Math.PI * 2;

            direction = 360 - (r * 180 / (double)Math.PI);
            return direction;
        }
    }
}
