using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;

namespace Multiple_Mice.Code.Raw
{
    public class Engine : NativeWindow
    {
        public List<RawMouse> Mice = new List<RawMouse>();
        public List<RawMouse> ActiveMice = new List<RawMouse>();

        public event Action OnRawInput;

        private PreMessageFilter _Filter;
        private readonly Dictionary<IntPtr, MouseEvent> _DeviceList = new Dictionary<IntPtr, MouseEvent>();
        private readonly IntPtr _DevNotifyHandle;

        private static readonly Guid DeviceInterfaceHid = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");
        private static InputData _rawBuffer;

        public Engine(IntPtr parentHandle, bool captureOnlyInForeground)
        {
            
            AssignHandle(parentHandle);
            EnumerateDevices();
            _DevNotifyHandle = RegisterForDeviceNotifications(parentHandle);

            foreach (var device in _DeviceList)
            {
                var rid = new RawInputDevice[1];

                rid[0].UsagePage = HidUsagePage.GENERIC;
                rid[0].Usage = HidUsage.Mouse;
                rid[0].Flags = RawInputDeviceFlags.NONE | RawInputDeviceFlags.DEVNOTIFY | RawInputDeviceFlags.EXINPUTSINK;
                rid[0].Target = parentHandle;

                if (!Win32.RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(rid[0])))
                {
                    throw new ApplicationException("Failed to register raw input device(s).");
                }
                //RawMouse m = new RawMouse(device.Key, this);
                //Mice.Add(m);
                Mice.Add(device.Value.Mouse);
            }
        }
        public void AddMessageFilter()
        {
            if (null != _Filter) return;

            _Filter = new PreMessageFilter();
            Application.AddMessageFilter(_Filter);
        }

        private void RemoveMessageFilter()
        {
            if (null == _Filter) return;

            Application.RemoveMessageFilter(_Filter);
        }

        static IntPtr RegisterForDeviceNotifications(IntPtr parent)
        {
            var usbNotifyHandle = IntPtr.Zero;
            var bdi = new BroadcastDeviceInterface();
            bdi.DbccSize = Marshal.SizeOf(bdi);
            bdi.BroadcastDeviceType = BroadcastDeviceType.DBT_DEVTYP_DEVICEINTERFACE;
            bdi.DbccClassguid = DeviceInterfaceHid;

            var mem = IntPtr.Zero;
            try
            {
                mem = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BroadcastDeviceInterface)));
                Marshal.StructureToPtr(bdi, mem, false);
                usbNotifyHandle = Win32.RegisterDeviceNotification(parent, mem, DeviceNotification.DEVICE_NOTIFY_WINDOW_HANDLE);
                Debug.Print("Registration for device notifications successed. Handle="+usbNotifyHandle+"  Parent:"+parent);

            }
            catch (Exception e)
            {
                Debug.Print("Registration for device notifications Failed. Error: {0}", Marshal.GetLastWin32Error());
                Debug.Print(e.StackTrace);
            }
            finally
            {
                Marshal.FreeHGlobal(mem);
            }

            if (usbNotifyHandle == IntPtr.Zero)
            {
                Debug.Print("Registration for device notifications Failed. Error: {0}", Marshal.GetLastWin32Error());
            }

            return usbNotifyHandle;
        }

        public void EnumerateDevices()
        {
            _DeviceList.Clear();

            var numberOfDevices = 0;
            uint deviceCount = 0;
            var dwSize = (Marshal.SizeOf(typeof(Rawinputdevicelist)));

            if (Win32.GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, (uint)dwSize) == 0)
            {
                var pRawInputDeviceList = Marshal.AllocHGlobal((int)(dwSize * deviceCount));
                Win32.GetRawInputDeviceList(pRawInputDeviceList, ref deviceCount, (uint)dwSize);

                for (var i = 0; i < deviceCount; i++)
                {
                    uint pcbSize = 0;

                    // On Window 8 64bit when compiling against .Net > 3.5 using .ToInt32 you will generate an arithmetic overflow. Leave as it is for 32bit/64bit applications
                    var rid = (Rawinputdevicelist)Marshal.PtrToStructure(new IntPtr((pRawInputDeviceList.ToInt64() + (dwSize * i))), typeof(Rawinputdevicelist));

                    Win32.GetRawInputDeviceInfo(rid.hDevice, RawInputDeviceInfo.RIDI_DEVICENAME, IntPtr.Zero, ref pcbSize);

                    if (pcbSize <= 0) continue;

                    var pData = Marshal.AllocHGlobal((int)pcbSize);
                    Win32.GetRawInputDeviceInfo(rid.hDevice, RawInputDeviceInfo.RIDI_DEVICENAME, pData, ref pcbSize);
                    var deviceName = Marshal.PtrToStringAnsi(pData);

                    if (rid.dwType == DeviceType.RimTypemouse || rid.dwType == DeviceType.RimTypeHid)
                    {
                        var deviceDesc = Win32.GetDeviceDescription(deviceName);

                        var dInfo = new MouseEvent
                        {
                            DeviceName = Marshal.PtrToStringAnsi(pData),
                            DeviceHandle = rid.hDevice,
                            DeviceType = Win32.GetDeviceType(rid.dwType),
                            Name = deviceDesc,
                            Mouse = new RawMouse(rid.hDevice, deviceDesc, this)
                        };

                        if (!_DeviceList.ContainsKey(rid.hDevice))
                        {
                            numberOfDevices++;
                            _DeviceList.Add(rid.hDevice, dInfo);
                        }
                    }

                    Marshal.FreeHGlobal(pData);
                }

                Marshal.FreeHGlobal(pRawInputDeviceList);
                Debug.WriteLine("EnumerateDevices() found {0} Mouse/Mice", _DeviceList.Count);
                return;
            }
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        bool AlreadyExits()
        {
            return false;
        }

        public void ProcessRawInput(IntPtr hdevice)
        {
            //Debug.WriteLine(_rawBuffer.data.keyboard.ToString());
            //Debug.WriteLine(_rawBuffer.data.hid.ToString());
            //Debug.WriteLine(_rawBuffer.header.ToString());

            if (_DeviceList.Count == 0) return;

            var dwSize = 0;
            Win32.GetRawInputData(hdevice, DataCommand.RID_INPUT, IntPtr.Zero, ref dwSize, Marshal.SizeOf(typeof(Rawinputheader)));

            if (dwSize != Win32.GetRawInputData(hdevice, DataCommand.RID_INPUT, out _rawBuffer, ref dwSize, Marshal.SizeOf(typeof(Rawinputheader))))
            {
                Debug.WriteLine("Error getting the rawinput buffer");
                return;
            }

            //int lastX = _rawBuffer.data.mouse.lLastX;
            //int lastY = _rawBuffer.data.mouse.lLastY;
            //int flags = _rawBuffer.data.keyboard.Flags;
            //Debug.WriteLine("X:"+ lastX);
            //Debug.WriteLine("Y:" + lastY);
           /* Debug.WriteLine(_rawBuffer.data.mouse.ToString());
            Debug.WriteLine(_rawBuffer.data.hid.ToString());
            Debug.WriteLine(_rawBuffer.header.ToString());*/
           // if (virtualKey == Win32.KEYBOARD_OVERRUN_MAKE_CODE) return;

            /* var isE0BitSet = ((flags & Win32.RI_KEY_E0) != 0);

             KeyPressEvent keyPressEvent;*/

            if (_DeviceList.ContainsKey(_rawBuffer.header.hDevice))
            {
                _DeviceList[_rawBuffer.header.hDevice].Mouse.MyEventHandler(_DeviceList[_rawBuffer.header.hDevice],
                    _rawBuffer.data.mouse);
                if (OnRawInput != null)
                {
                    OnRawInput();
                }
                //supposed to process the event here
                // keyPressEvent = _deviceList[_rawBuffer.header.hDevice];
                //foreach (var item in Mice)
                //{
                //    if (item.MouseHandle==_rawBuffer.header.hDevice)
                //    {
                //        item.MyEventHandler(_deviceList[_rawBuffer.header.hDevice],_rawBuffer.data.mouse);
                //        break;
                //    }
                //}
                   
         
            }
            else
            {
                Debug.WriteLine("Handle: {0} was not in the device list.", _rawBuffer.header.hDevice);
                return;
            }

          //  var isBreakBitSet = ((flags & Win32.RI_KEY_BREAK) != 0);


        }

        protected override void WndProc(ref Message message)
        {
            switch (message.Msg)
            {
                case Win32.WM_INPUT:
                    {
                        ProcessRawInput(message.LParam);
                    }
                    break;

                case Win32.WM_USB_DEVICECHANGE:
                    {
                        Debug.WriteLine("USB Device Arrival / Removal");
                        EnumerateDevices();
                    }
                    break;
            }
            base.WndProc(ref message);
        }

        ~Engine()
        {
            Win32.UnregisterDeviceNotification(_DevNotifyHandle);
            //RemoveMessageFilter();
        }
    }
}
