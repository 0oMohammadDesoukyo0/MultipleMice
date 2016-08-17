using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Multiple_Mice.UI
{
    public partial class Cursor : Control
    {

        Form Parent;
        public bool Shown = false;
        public Cursor():base()
        {
            Parent = new Form();
            Parent.FormBorderStyle = FormBorderStyle.None;
            Parent.ControlBox = false;
            Parent.BackColor = Color.Red;
            Parent.TransparencyKey = Color.Red;
            Parent.Size = new Size(50, 50);
            //Location = new Point(1, 30);
            Size = new Size(50, 50);
           // BackColor = Color.Green;
            Parent.ShowInTaskbar = false;
            Parent.ShowIcon = false;
            Parent.FormClosing += Parent_FormClosing;
            Parent.Controls.Add(this);
            //Invalidate();
            Parent.Invalidate();
          //  Parent.Refresh();
            
        }

        private void Parent_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

                Point[] Points = new Point[] {new Point(0,20), new Point(20, 20), new Point(20, 0), new Point(0, 20) };
                Matrix myMatrix = new Matrix();
                myMatrix.Rotate(-45, MatrixOrder.Append);
                e.Graphics.Transform = myMatrix;
                e.Graphics.FillPolygon(new SolidBrush(Color.DarkGreen), Points);
        }
        public void Show()
        {
            if (Parent != null)
            { Parent.Show(); Shown = true; }
        }
        public void Hide()
        {
            if (Parent != null)
            { Parent.Hide(); Shown = false; }
        }
        public void Move(Point Location)
        {
            if (Parent!=null)
            {
                if (!Shown)
                {
                    Parent.Show();
                }
                Parent.TopMost = true;
                Parent.Location = Location;
                //Invalidate();
            }
        }
    }
}
