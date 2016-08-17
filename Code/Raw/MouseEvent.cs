using System;

namespace Multiple_Mice.Code.Raw
{
    public class MouseEvent
    {
        public string DeviceName;       // i.e. \\?\HID#VID_045E&PID_00DD&MI_00#8&1eb402&0&0000#{884b96c3-56ef-11d1-bc8c-00a0c91405dd}
        public string DeviceType;       // Touchpad or HID
        public IntPtr DeviceHandle;     // Handle to the device that send the input
        public string Name;             // i.e. Microsoft USB Comfort Curve Touchpad 2000 (Mouse and Keyboard Center)
        private string _source;         // Mouse

        public string Source
        {
            get { return _source; }
            set { _source = string.Format("Mouse_{0}", value.PadLeft(2, '0')); }
        }

        public override string ToString()
        {
            return string.Format("Device\n DeviceName: {0}\n DeviceType: {1}\n DeviceHandle: {2}\n Name: {3}\n", DeviceName, DeviceType, DeviceHandle.ToInt64().ToString("X"), Name);
        }
    }
}
                                         

