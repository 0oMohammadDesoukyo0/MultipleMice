using gma.System.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Multiple_Mice.Code.Structures
{
    public class Engine : Form
    {
        List<Mouse> Mice = new List<Mouse>();
        public delegate void Updater();
        UserActivityHook actHook;
        Updater UIUpdater;
        public static Engine Instance;
        public Engine(Updater UIUpdateMethod)
        {
            UIUpdater = UIUpdateMethod;
            Instance = this;
            actHook = new UserActivityHook(); // crate an instance with global hooks
                                              // hang on events
            actHook.OnMouseActivity += new MouseEventHandler(MouseMoved);
        }
        public void MouseMoved(object sender, MouseEventArgs e)
        {
            try
            {
                Mice[0].Location = new Point(e.X, e.Y);
                UIUpdater();
            }
            catch (Exception)
            {
                
            }
           
        }

        public void RefreshMice()
        {
            Mice.Clear();
            SelectQuery Sq = new SelectQuery("Win32_PointingDevice");
            ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher(Sq);
            ManagementObjectCollection osDetailsCollection = objOSDetails.Get();
            StringBuilder sb = new StringBuilder();
            foreach (ManagementObject mo in osDetailsCollection)
            {
                Mice.Add(new Mouse((string)mo["PNPDeviceID"], (string)mo["Status"], (string)mo["Caption"]));
                /* ConfigManagerErrorCode: {0}", mo["ConfigManagerErrorCode"].ToString()
                  Convert.ToDateTime(mo["InstallDate"]).ToString()
                 mo["ConfigManagerUserConfig"].ToString()
                 CreationClassName(string)mo["CreationClassName"]
                 (string)mo["Description"]
                 DeviceID(string)mo["DeviceID"]
                 (string)mo["ErrorCleared"]
                 (string)mo["ErrorDescription"]
                 (string)mo["Manufacturer"]
                 ["NumberOfButtons"].ToString()
                 (string)mo["LastErrorCode"]
                 (string)mo["Name"]
                 ["DeviceInterface"].ToString()
                 (ushort)mo["PointingType"]
                 (string)mo["HardwareType"]
                 (string)mo["InfFileName"]
                 (string)mo["InfSection"]
                 (string)mo["SystemCreationClassName"];
                 (string)mo["SystemName"];
                 //mo["PowerManagementSupported"])*/
            }
        }
        public Mouse GetMouse(int Number)
        {
            if (Number > -1 && Number < Mice.Count)
            {

                return Mice[Number];
            }
            return null;
        }
    }
}
