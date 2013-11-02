using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Timers;

namespace COServer_Project
{
    public class DroppedItems
    {
        public static Hashtable AllDroppedItems = new Hashtable();

        public static DroppedItem DropItem(string item, uint x, uint y, uint map, uint money)
        {
            uint uid = (uint)General.Rand.Next(1, 10000000);
            while (AllDroppedItems.Contains(uid))
                uid = (uint)General.Rand.Next(1, 10000000);
            DroppedItem ITEM = new DroppedItem(uid, item, x, y, map, money);
            AllDroppedItems.Add(uid, ITEM);

            return ITEM;
        }
    }

    public class DroppedItem
    {
        public uint UID;
        public string Item;
        public uint ItemId;
        public uint X;
        public uint Y;
        public uint Map;
        public uint Money;
        public Timer DestroyTimer = new Timer();

        public DroppedItem(uint uid, string item, uint x, uint y, uint map, uint money)
        {
            UID = uid;
            Item = item;
            X = x;
            Y = y;
            Map = map;
            Money = money;
            DestroyTimer.Interval = 90000;
            DestroyTimer.Elapsed += new ElapsedEventHandler(Destroy);
            DestroyTimer.Start();

            string[] Splitter = item.Split('-');
            ItemId = uint.Parse(Splitter[0]);
        }

        public void Destroy(object source, ElapsedEventArgs e)
        {
            DestroyTimer.Stop();
            DestroyTimer.Dispose();
            DestroyTimer = null;
            DroppedItems.AllDroppedItems.Remove(UID);
            World.ItemDissappears(this);
        }
    }
}
