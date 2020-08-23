using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel.Design;

namespace BrowserX.UC
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public partial class BrowserToolbar : Panel
    {
        public bool DrawBottomLine { get; set; }
        public bool DrawLinearBack { get; set; }

        public BrowserToolbar()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.BackColor = Color.White;
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.UpdateStyles();
            this.Paint += BroswerToolbar_Paint;
        }

        void BroswerToolbar_Paint(object sender, PaintEventArgs e)
        {
            if (DrawBottomLine)
            {
                e.Graphics.DrawLine(new Pen(Color.FromArgb(211, 211, 211), 1), new Point(0, this.Height - 1), new Point(this.Width, this.Height - 1)); 
            }

            if (DrawLinearBack)
            {
                Rectangle r = new Rectangle(0, 0, this.Width, this.Height);

                using (LinearGradientBrush linearBrush = new LinearGradientBrush(r,
                            Color.White, Color.White, LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(linearBrush, r);
                }
            }
        }
    }
}
