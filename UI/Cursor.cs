using System.Drawing;
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
        /// Visability flag.
        /// </summary>
        public bool Shown;

        /// <summary>
        /// Transparent parent form
        /// </summary>
        private Form _Parent;
        /// <summary>
        /// Cursor color, default is green.
        /// </summary>
        private readonly Color _Color;

        /// <summary>
        /// Default constructor sets cursor color to green before initiating the
        /// parent form.
        /// </summary>
        public Cursor()
        {
            _Color = Color.Green;
            Size = new Size(20, 20);
            InitParentForm();
        }

        /// <summary>
        /// Constructor sets cursor color to a given color 
        /// before initiating the parent form.
        /// </summary>
        /// <param name="c">Cursor Color</param>
        public Cursor(Color c)
        {
            _Color = c;
            Size = new Size(20, 20);
            InitParentForm();
        }

        /// <summary>
        /// Constructor sets cursor color to a given color 
        /// and cursor size before initiating the parent form.
        /// </summary>
        /// <param name="c">Cursor Color</param>
        /// <param name="size">Cursor Size</param>
        public Cursor(Color c, Size size)
        {
            _Color = c;
            Size = size;
            InitParentForm();
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
                Size = new Size(Size.Width, Size.Height),
                ShowInTaskbar = false,
                MaximumSize = new Size(Size.Width, Size.Height),
                ShowIcon = false
            };
            
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
        /// Painting a triangle as the cursor graphics
        /// </summary>
        /// <param name="e">PaintEventArgs</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Point[] points = {new Point(0, 0), new Point(0, Size.Height), new Point(Size.Width, 0)};
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
