using Multiple_Mice.Code.Raw;
using System;
using System.Windows.Forms;

namespace Multiple_Mice.UI
{
    public partial class Main : Form
    {
        public Engine MiceEngine;
        public static Main Instance;
        public Main()
        {

            InitializeComponent();
            MiceEngine = new Engine(Handle, true);
         
            MiceEngine.AddMessageFilter();   // Adding a message filter will cause  to be handled
            Win32.DeviceAudit();            // Writes a file DeviceAudit.txt to the current directory
            MiceEngine.OnRawInput += RefreshDataGridView;

            //adjust opacity bar to 100% 
            OpacityBar.Value = 100;
            //addjust window location to be bottom extra right primary screen
            Location = new System.Drawing.Point(
                                (int)System.Windows.SystemParameters.PrimaryScreenWidth - Width,
                                 (int)(System.Windows.SystemParameters.PrimaryScreenHeight - Height-40));
            
            Instance = this;
        }

        private void RefreshDataGridView()
        {
            if (MiceEngine.ActiveMice.Count == 0)
                return;

            if (DGV.Rows.Count == MiceEngine.ActiveMice.Count - 1)
                DGV.Rows.Add();

            for (int i = 0; i < MiceEngine.ActiveMice.Count; i++)
            {
                DGV[0, i].Value = MiceEngine.ActiveMice[i].Name;
                DGV[1, i].Value = MiceEngine.ActiveMice[i].MyWindowName();
                DGV[2, i].Value = MiceEngine.ActiveMice[i].MyWindowHandle() + "";
                DGV[3, i].Value = MiceEngine.ActiveMice[i].LastLocation.X;
                DGV[4, i].Value = MiceEngine.ActiveMice[i].LastLocation.Y;
            }
        }

        private void TopMostCB_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = ((CheckBox)sender).Checked;
        }

        private void OpacityBar_Scroll(object sender, EventArgs e)
        {
            Opacity = ((TrackBar)sender).Value * 0.01;
        }
    }
}
