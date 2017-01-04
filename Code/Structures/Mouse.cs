using System;
using System.Diagnostics;
using System.Drawing;

namespace Multiple_Mice.Code.Structures
{
    public class Mouse:IEquatable<Mouse>
    {

        string _PNPID,status, port,caption;
        Point location;
        public Mouse(string PNPID,string Status, string Caption)
        {
            _PNPID = PNPID;
            status= Status;
            caption = Caption;
        }
        /// <summary>
        /// Plug and play id I supposed this to be a unique ID
        /// </summary>
        public string PNPID
        {
            set { _PNPID = value; }
            get { return _PNPID; }
        }
        public string Caption
        {
            set { caption = value; }
            get { return caption; }
        }
        public string Status
        {
            set { status = value; }
            get { return status; }
        }
        public string Port
        {
            set { port = value; }
            get { return port; }
        }
        public Point Location
        {
            set { location = value; }
            get { return location; }
        }
        public MousePhysicalTypes Type
        {
            set; get;
        }

        public bool Equals(Mouse other)
        {
            return other.PNPID.Equals(PNPID);
        }
    }
    public enum MousePhysicalTypes
    {
        UNKNOWN,
        USB,
        PS2
    }
}
