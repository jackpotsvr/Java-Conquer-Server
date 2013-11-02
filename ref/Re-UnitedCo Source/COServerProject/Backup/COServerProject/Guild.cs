using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace COServer_Project
{
    public class Guilds
    {
        public static Hashtable AllGuilds = new Hashtable();

        public static void NewGuild(ushort GuildID, string GuildName, Character Creator)
        {
            bool Can = true;

            foreach (DictionaryEntry DE in AllGuilds)
            {
                Guild G = (Guild)DE.Value;
                if (G.GuildName == GuildName) Can = false;
            }
            if (Can && !AllGuilds.Contains(GuildID))
            {
                Guild NewGuild = new Guild(GuildName, GuildID, Creator.Name + ":" + Creator.UID.ToString() + ":" + Creator.Level.ToString() + ":1000000", new string[6], new string[1000], 1000000, 0, 0, 1, "New guild", "", "");
                AllGuilds.Add(GuildID, NewGuild);
                World.SendMsgToAll(Creator.Name + " has set up " + GuildName + " successfully!", "SYSTEM", 2000);
            }
        }
        public static void AddGuild(string guildname, ushort guildid, string creator, string[] dls, string[] members, uint fund, uint gwwins, byte holdingpole, uint membersc, string bulletin, string allies, string enemies)
        {           

            Guild NewGuild = new Guild(guildname, guildid, creator, dls, members, fund, gwwins, holdingpole, membersc, bulletin, allies, enemies);
            AllGuilds.Add(guildid, NewGuild);

            if (NewGuild.HoldingPole)
                World.PoleHolder = NewGuild;
        }

        public static void SaveAllGuilds()
        {
            foreach (DictionaryEntry DE in AllGuilds)
            {
                Guild TheGuild = (Guild)DE.Value;
                InternalDatabase.SaveGuild(TheGuild);
            }
        }
    }

    public class Guild
    {
        public string GuildName;
        public ushort GuildID;
        public string Creator;
        public int PoleDamaged = 0;

        public Hashtable DLs = new Hashtable();
        public Hashtable Members = new Hashtable();

        public uint Fund;
        public uint GWWins;
        public bool HoldingPole;
        public uint MembersCount;
        public string Bulletin;
        public bool ClaimedPrize = false;

        public ArrayList Allies = new ArrayList();
        public ArrayList Enemies = new ArrayList();        

        public Guild(string guildname, ushort guildid, string creator, string[] dls, string[] members, uint fund, uint gwwins, byte holdingpole, uint membersc, string bulletin, string allies, string enemies)
        {
            GuildName = guildname;
            GuildID = guildid;
            MembersCount = membersc;
            GWWins = gwwins;
            Fund = fund;
            if (holdingpole == 0)
                HoldingPole = false;
            else
                HoldingPole = true;
            Creator = creator;
            Bulletin = bulletin;

            string[] Splitter = allies.Split('~');

            foreach (string ally in Splitter)
            {
                Allies.Add(ally);
            }

            Splitter = allies.Split('~');

            foreach (string enemy in Splitter)
            {
                Enemies.Add(enemy);
            }

            if (dls.Length > 0 && dls[0] != "")
            {
                foreach (string dl in dls)
                {
                    if (dl != null && dl.Length > 0)
                    {
                        Splitter = dl.Split(':');
                        DLs.Add(uint.Parse(Splitter[1]), dl);
                    }
                }
            }

            if (members.Length > 0 && members[0] != "")
            {
                foreach (string mem in members)
                {
                    if (mem != null && mem.Length > 0)
                    {
                        Splitter = mem.Split(':');
                        Members.Add(uint.Parse(Splitter[1]), mem);
                    }
                }
            }
        }

        public void Refresh(Character Who)
        {
            if (Who.GuildPosition == 50)
            {
                if (Members.Contains(Who.UID))
                {
                    Members.Remove(Who.UID);
                    Members.Add(Who.UID, Who.Name + ":" + Who.UID.ToString() + ":" + Who.Level.ToString() + ":" + Who.GuildDonation.ToString());
                }
            }
            else if (Who.GuildPosition == 90)
            {
                if (Members.Contains(Who.UID))
                {
                    DLs.Remove(Who.UID);
                    DLs.Add(Who.UID, Who.Name + ":" + Who.UID.ToString() + ":" + Who.Level.ToString() + ":" + Who.GuildDonation.ToString());
                }
            }
            else if (Who.GuildPosition == 100)
            {
                Creator = Who.Name + ":" + Who.UID.ToString() + ":" + Who.Level.ToString() + ":" + Who.GuildDonation.ToString();
            }
        }

        public void Disband(Character Leader)
        {
            uint CharID = 0;
            foreach (DictionaryEntry DE in DLs)
            {
                string dl = (string)DE.Value;
                string[] Splitter = dl.Split(':');

                CharID = uint.Parse(Splitter[1]);

                if (World.AllChars.Contains(CharID))
                {
                    Character Char = (Character)World.AllChars[CharID];
                    Char.MyClient.SendPacket(General.MyPackets.SendGuild(Char.GuildID, 19));

                    Char.MyGuild = null;
                    Char.GuildDonation = 0;
                    Char.GuildID = 0;
                    Char.GuildPosition = 0;
                    World.UpdateSpawn(Char);
                }
                else
                    ExternalDatabase.NoGuild(CharID);

            }

            foreach (DictionaryEntry DE in Members)
            {
                string nm = (string)DE.Value;
                string[] Splitter = nm.Split(':');

                CharID = uint.Parse(Splitter[1]);

                if (World.AllChars.Contains(CharID))
                {
                    Character Char = (Character)World.AllChars[CharID];
                    Char.MyClient.SendPacket(General.MyPackets.SendGuild(Char.GuildID, 19));

                    Char.MyGuild = null;
                    Char.GuildDonation = 0;
                    Char.GuildID = 0;
                    Char.GuildPosition = 0;
                    World.UpdateSpawn(Char);
                }
                else
                    ExternalDatabase.NoGuild(CharID);
            }

            Members.Clear();
            DLs.Clear();

            Leader.GuildID = 0;
            Leader.GuildPosition = 0;
            Leader.GuildDonation = 0;
            Leader.MyGuild = null;

            Leader.MyClient.SendPacket(General.MyPackets.SendGuild(GuildID, 19));
            Guilds.AllGuilds.Remove(GuildID);
            ExternalDatabase.DisbandGuild(GuildID);

            World.SendMsgToAll(GuildName + " has been disbanded.", "SYSTEM", 2000);
            World.UpdateSpawn(Leader);
        }

        public void PlayerJoins(Character Joiner)
        {
            GuildMessage(Joiner.Name + " has joined our guild.");
            Members.Add(Joiner.UID, Joiner.Name + ":" + Joiner.UID.ToString() + ":" + Joiner.Level.ToString() + ":0");
            MembersCount++;
        }
        public void PlayerQuits(Character Quitter)
        {
            if (Quitter.GuildPosition == 50)
                Members.Remove(Quitter.UID);
            else
                if (Quitter.GuildPosition == 90)
                    DLs.Remove(Quitter.UID);
            GuildMessage(Quitter.Name + " has left our guild.");

            MembersCount--;
        }
        public void KickPlayer(uint ID, string Name, byte Pos)
        {
            if (Pos == 50)
                Members.Remove(ID);
            else
                if (Pos == 90)
                    DLs.Remove(ID);

            if (World.AllChars.Contains(ID))
            {
                Character Char = (Character)World.AllChars[ID];
                Char.MyClient.SendPacket(General.MyPackets.SendGuild(GuildID, 19));
                Char.GuildDonation = 0;
                Char.GuildPosition = 0;
                Char.GuildID = 0;
                Char.MyGuild = null;
                World.UpdateSpawn(Char);
                World.SpawnOthersToMe(Char, false);
            }
            else
            {
                ExternalDatabase.NoGuild(ID);
            }

            GuildMessage(Name + " didn't abide the rules and has been kicked out of the guild.");
            MembersCount--;
        }

        public void GuildMessage(Character Sender, string Message, string ToWho)
        {
            string[] Splitter = Creator.Split(':');
            if (World.AllChars.Contains(uint.Parse(Splitter[1])))
            {
                Character Char = (Character)World.AllChars[uint.Parse(Splitter[1])];
                if (Sender != Char)
                    Char.MyClient.SendPacket(General.MyPackets.SendMsg(Char.MyClient.MessageId, Sender.Name, ToWho, Message, 2004));
            }

            foreach (DictionaryEntry DE in DLs)
            {
                string DL = (string)DE.Value;
                Splitter = DL.Split(':');
                if (World.AllChars.Contains(uint.Parse(Splitter[1])))
                {
                    Character Char = (Character)World.AllChars[uint.Parse(Splitter[1])];
                    if (Sender != Char)
                        Char.MyClient.SendPacket(General.MyPackets.SendMsg(Char.MyClient.MessageId, Sender.Name, ToWho, Message, 2004));
                }
            }

            foreach (DictionaryEntry DE in Members)
            {
                string NM = (string)DE.Value;
                Splitter = NM.Split(':');
                if (World.AllChars.Contains(uint.Parse(Splitter[1])))
                {
                    Character Char = (Character)World.AllChars[uint.Parse(Splitter[1])];
                    if (Sender != Char)
                        Char.MyClient.SendPacket(General.MyPackets.SendMsg(Char.MyClient.MessageId, Sender.Name, ToWho, Message, 2004));
                }
            }
        }
        public void GuildMessage(string Message)
        {
            string[] Splitter = Creator.Split(':');
            if (World.AllChars.Contains(uint.Parse(Splitter[1])))
            {
                Character Char = (Character)World.AllChars[uint.Parse(Splitter[1])];
                Char.MyClient.SendPacket(General.MyPackets.SendMsg(Char.MyClient.MessageId, "Guild", "All", Message, 2004));
            }

            foreach (DictionaryEntry DE in DLs)
            {
                string DL = (string)DE.Value;
                Splitter = DL.Split(':');
                if (World.AllChars.Contains(uint.Parse(Splitter[1])))
                {
                    Character Char = (Character)World.AllChars[uint.Parse(Splitter[1])];
                    Char.MyClient.SendPacket(General.MyPackets.SendMsg(Char.MyClient.MessageId, "Guild", "All", Message, 2004));
                }
            }

            foreach (DictionaryEntry DE in Members)
            {
                string NM = (string)DE.Value;
                Splitter = NM.Split(':');
                if (World.AllChars.Contains(uint.Parse(Splitter[1])))
                {
                    Character Char = (Character)World.AllChars[uint.Parse(Splitter[1])];
                    Char.MyClient.SendPacket(General.MyPackets.SendMsg(Char.MyClient.MessageId, "Guild", "All", Message, 2004));
                }
            }
        }


    }
}
