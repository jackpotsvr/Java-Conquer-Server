using System;
using System.Collections.Generic;
using System.Text;

namespace COServer_Project
{
    class Portals : object
    {
        #region private members
        private ushort v_PID, v_Smap, v_SX, v_SY, v_Emap, v_EX, v_EY;
        #endregion
        #region public members
        public ushort PID
        {
            get { return v_PID; }
            set { v_PID = value; }
        }
        public ushort Smap
        {
            get { return v_Smap; }
            set { v_Smap = value; }
        }
        public ushort SX
        {
            get { return v_SX; }
            set { v_SX = value; }
        }
        public ushort SY
        {
            get { return v_SY; }
            set { v_SY = value; }
        }
        public ushort Emap
        {
            get { return v_Emap; }
            set { v_Emap = value; }
        }
        public ushort EX
        {
            get { return v_EX; }
            set { v_EX = value; }

        }
        public ushort EY
        {
            get { return v_EY; }
            set { v_EY = value; }
        }
        #endregion
      
    }
}
