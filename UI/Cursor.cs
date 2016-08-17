using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace Multiple_Mice.UI
{
    /// <summary>
    /// UI.Cursor class inherits Control class,
    /// it holds a transparent form which contains the cursor control.
    /// </summary>
    public class Cursor : Control
    {
        /// <summary>
        /// Transparent parent form
        /// </summary>
        private Form _Parent;
        /// <summary>
        /// Cursor color, default is green.
        /// </summary>
        private readonly Color _Color;

        /// <summary>
        /// Visability flag.
        /// </summary>
        public bool Shown;

        /// <summary>
        /// Default constructor sets cursor color to green after initiating the
        /// parent form.
        /// </summary>
        public Cursor()
        {
            InitParentForm();
            _Color = Color.Green;
        }

        /// <summary>
        /// Another constructor sets cursor color to a given color 
        /// after initiating the parent form.
        /// </summary>
        public Cursor(Color c)
        {
            InitParentForm();
            _Color = c;
        }

        /// <summary>
        /// Initializing the parent form then adding the cursor control to it.
        /// </summary>
        private void InitParentForm()
        {
            _Parent = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                ControlBox = false,
                BackColor = Color.Red,
                TransparencyKey = Color.Red,
                Size = new Size(20, 20),
                ShowInTaskbar = false,
                MaximumSize = new Size(20,20),
                ShowIcon = false
            };
            Size = new Size(20, 20);
            
            _Parent.FormClosing += Parent_FormClosing;
            _Parent.Controls.Add(this);

            //Invalidate();
            _Parent.Invalidate();
        }

        /// <summary>
        /// Disable parent form closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">FormClosingEventArgs</param>
        private void Parent_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        /// <summary>
        /// Painting a -45 degrees rotated Triangle as the cursor graphics
        /// </summary>
        /// <param name="e">PaintEventArgs</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Point[] points = {new Point(0, 0), new Point(0, 20), new Point(20, 0)};
            e.Graphics.FillPolygon(new SolidBrush(_Color), points);
        }

        /// <summary>
        /// Shows the current cursor by showing the parent form
        /// </summary>
        public new void Show()
        {
            if (_Parent != null)
            {
                _Parent.Show();
                Shown = true;
            }
        }

        /// <summary>
        /// Hides the current cursor by hiding the parent form.
        /// </summary>
        public new void Hide()
        {
            if (_Parent != null)
            {
                _Parent.Hide();
                Shown = false;
            }
        }

        /// <summary>
        /// Moves the current cursor by moving the parent form to "location"
        /// </summary>
        /// <param name="location">Point where the cursor should move to</param>
        public new void Move(Point location)
        {
            if (_Parent != null)
            {
                if (!Shown)
                {
                    _Parent.Show();
                }
                _Parent.TopMost = true;
                _Parent.Location = location;
            }
        }
    }
}
