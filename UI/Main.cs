using Multiple_Mice.Code.Structures;
using System;
using System.Drawing;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace Multiple_Mice.UI
{
    public partial class Main : Form
    {
        Engine MiceEngine;
        public Main()
        {
            InitializeComponent();
            //adjust opacity bar to 100% 
            OpacityBar.Value = 100;
            //addjust window location to be bottom extra right primary screen
            Location = new System.Drawing.Point(
                                (int)System.Windows.SystemParameters.PrimaryScreenWidth - Width,
                                 (int)(System.Windows.SystemParameters.PrimaryScreenHeight - Height-40));
            //25);
            //(int)(System.Windows.SystemParameters.PrimaryScreenHeight - Height) / 2);
            //Create Mice Engine object
            MiceEngine = new Engine(UpdateMiceData);
            //run it
            MiceEngine.RefreshMice();
            //update Mouse data
            UpdateMiceData();
        }
        public void UpdateMiceData()
        {
            Mouse M1 = MiceEngine.GetMouse(0);
            Mouse M2 = MiceEngine.GetMouse(1);
            if (M1 != null)
            {
                Mouse1GB.Text = M1.Caption+":";
                Mouse1SLbl.Text = M1.Status;
                Mouse1SLbl.ForeColor = Color.Black;
                Mouse1LLbl.Text = "(" + M1.Location.X+ ","+ M1.Location.Y + ")";
            }
            else
            {
                Mouse1GB.Text ="Mouse1:";
                Mouse1SLbl.Text = "Disconnected";
                Mouse1SLbl.ForeColor = Color.Red;
            }
            if (M2 != null)
            {
                Mouse2GB.Text = M2.Caption + ":";
                Mouse2SLbl.Text = M2.Status;
                Mouse2SLbl.ForeColor = Color.Black;
            }
            else
            {
                Mouse2GB.Text = "Mouse2:";
                Mouse2SLbl.Text = "Disconnected";
                Mouse2SLbl.ForeColor = Color.Red;
            }
        }

        private void TopMostCB_CheckedChanged(object sender, EventArgs e)
        {
            run();
            TopMost = ((CheckBox)sender).Checked;
        }

        private void OpacityBar_Scroll(object sender, EventArgs e)
        {
            Opacity = ((TrackBar)sender).Value * 0.01;
        }
        void run()
        {
            SelectQuery Sq = new SelectQuery("Win32_PointingDevice");
            ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher(Sq);
            ManagementObjectCollection osDetailsCollection = objOSDetails.Get();
            StringBuilder sb = new StringBuilder();
            foreach (ManagementObject mo in osDetailsCollection)
            {
                sb.AppendLine(string.Format("Caption: {0}", (string)mo["Caption"]));
                sb.AppendLine(string.Format("ConfigManagerErrorCode: {0}", mo["ConfigManagerErrorCode"].ToString()));
                sb.AppendLine(string.Format("InstallDate: {0}", Convert.ToDateTime(mo["InstallDate"]).ToString()));
                sb.AppendLine(string.Format("ConfigManagerUserConfig: {0}", mo["ConfigManagerUserConfig"].ToString()));
                sb.AppendLine(string.Format("CreationClassName : {0}", (string)mo["CreationClassName"]));
                sb.AppendLine(string.Format("Description: {0}", (string)mo["Description"]));
                sb.AppendLine(string.Format("DeviceID : {0}", (string)mo["DeviceID"]));
                sb.AppendLine(string.Format("ErrorCleared: {0}", (string)mo["ErrorCleared"]));
                sb.AppendLine(string.Format("ErrorDescription : {0}", (string)mo["ErrorDescription"]));
                sb.AppendLine(string.Format("Manufacturer : {0}", (string)mo["Manufacturer"]));
                sb.AppendLine(string.Format("NumberOfButtons : {0}", mo["NumberOfButtons"].ToString()));
                sb.AppendLine(string.Format("LastErrorCode : {0}", (string)mo["LastErrorCode"]));
                sb.AppendLine(string.Format("Name : {0}", (string)mo["Name"]));
                sb.AppendLine(string.Format("DeviceInterface : {0}", mo["DeviceInterface"].ToString()));
                sb.AppendLine(string.Format("PointingType : {0}", (ushort)mo["PointingType"]));
                sb.AppendLine(string.Format("HardwareType : {0}", (string)mo["HardwareType"]));
                sb.AppendLine(string.Format("InfFileName : {0}", (string)mo["InfFileName"]));
                sb.AppendLine(string.Format("InfSection : {0}", (string)mo["InfSection"]));
                sb.AppendLine(string.Format("PNPDeviceID: {0}", (string)mo["PNPDeviceID"]));
                sb.AppendLine(string.Format("PowerManagementSupported : {0}", mo["PowerManagementSupported"]).ToString());
                sb.AppendLine(string.Format("Status : {0}", (string)mo["Status"]));
                sb.AppendLine(string.Format("SystemCreationClassName : {0}", (string)mo["SystemCreationClassName"]));
                sb.AppendLine(string.Format("SystemName: {0}", (string)mo["SystemName"]));
            }
            MessageBox.Show(sb.ToString());
        }
    }
}
