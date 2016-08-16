using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;

namespace Multiple_Mice.Code.Raw
{
    public class RawMouse
    {
        private readonly Dictionary<IntPtr, MouseEvent> _deviceList = new Dictionary<IntPtr, MouseEvent>();
        public delegate void DeviceEventHandler(object sender, RawInputEventArg e);
        public DateTime LastOperationTime;
        public event DeviceEventHandler KeyPressed;
        public Point LastLocation = new Point(-1, -1);
        UI.Cursor Cursor = new UI.Cursor();
        readonly object _padLock = new object();
        public int NumberOfKeyboards { get; private set; }
        public IntPtr MouseHandle;
        public Control MyName, MyXnY, MyStatus;
        DataGridView DGV;int myrow=-1;
        public RawMouse(IntPtr hwnd,IntPtr mouseHandle)
		{
			var rid = new RawInputDevice[1];

			rid[0].UsagePage = HidUsagePage.GENERIC;       
			rid[0].Usage = HidUsage.Mouse;              
            rid[0].Flags =  RawInputDeviceFlags.NONE | RawInputDeviceFlags.DEVNOTIFY|RawInputDeviceFlags.EXINPUTSINK;
			rid[0].Target = hwnd;
            MouseHandle = mouseHandle;

			if(!Win32.RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(rid[0])))
			{
				throw new ApplicationException("Failed to register raw input device(s).");
			}
		}

        internal void MyEventHandler(MouseEvent mouseEvent, Rawmouse mouse)
        {
            LastOperationTime = DateTime.Now;
            if(LastLocation.X==-1)
              AdjustCursor();
            int x = LastLocation.X + mouse.lLastX,
                 y= LastLocation.Y + mouse.lLastY;
            if (!Cursor.Shown)
            {
                Cursor.Show();
            }
            if (x<0)
            {
                x = 0;
            }
            else if (x> SystemParameters.FullPrimaryScreenWidth)
            {
                x =(int) SystemParameters.FullPrimaryScreenWidth;
            }
            if (y < 0)
            {
                y = 0;
            }
            else if (y > SystemParameters.FullPrimaryScreenHeight)
            {
                y = (int)SystemParameters.FullPrimaryScreenHeight;
            }
            LastLocation = new Point(x, y);

            Cursor.Move(LastLocation);
            if (MyName != null)
            {
                MyName.Text = mouseEvent.Name;
            }
            if (MyStatus != null)
            {
                MyStatus.Text = mouseEvent.Source;
            }
            if (MyXnY != null)
            {
                MyXnY.Text = "("+mouse.lLastX+","+mouse.lLastY+")";
            }
            if (myrow==-1)
            {
               
                myrow = DGV.Rows.Add();
            }
            if (DGV!=null && DGV.Rows[myrow]!=null)
            {
                
                DGV[0,myrow].Value= mouseEvent.Name;
                DGV[1, myrow].Value = LastLocation.X;
                DGV[2, myrow].Value = LastLocation.Y;
            }
        }
        public void  SetGridView(DataGridView DGV)
        {
            this.DGV = DGV;
            
        }
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);


            void AdjustCursor()
        {
                GetCursorPos(out LastLocation);
                //LastLocation = System.Windows.Forms.Cursor.Position;
        }

    }
}
