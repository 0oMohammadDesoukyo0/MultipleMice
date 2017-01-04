using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;

namespace Multiple_Mice.Code.Raw
{
    public class RawMouse
    {
        public delegate void DeviceEventHandler(object sender, RawInputEventArg e);

        public DateTime LastOperationTime;
        public event DeviceEventHandler KeyPressed;
        public Point LastLocation = new Point(-1, -1);
        public IntPtr MouseHandle;
        public string Name;
        public bool IsActive;

        private readonly UI.Cursor _Cursor;
        private readonly Engine _Engine;

        public RawMouse(IntPtr mouseHandle, string name, Engine engine)
        {
            MouseHandle = mouseHandle;
            Name = name;
            IsActive = false;
            _Cursor = new UI.Cursor(Color.Blue, new Size(30, 30));
            _Engine = engine;
            AdjustCursor();
        }

        internal void MyEventHandler(MouseEvent mouseEvent, Rawmouse mouse)
        {
            Screen myScreen = Screen.FromControl(_Cursor);
            Rectangle area = myScreen.Bounds;
            LastOperationTime = DateTime.Now;

            if (!_Cursor.Shown)
            {
                _Cursor.Show();
            }

            int x = LastLocation.X + mouse.lLastX;
            int y = LastLocation.Y + mouse.lLastY;

            x = Utility.Clamp(x, 0, area.Width);
            y = Utility.Clamp(y, 0, area.Height);
            LastLocation = new Point(x, y);
            _Cursor.Move(LastLocation);

            if (!IsActive)
            {
                _Engine.ActiveMice.Add(this);
                IsActive = true;
            }
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);


        private void AdjustCursor()
        {
            GetCursorPos(out LastLocation);
        }
    }
}
