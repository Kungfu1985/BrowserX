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

namespace BroswerX.UC
{
    public partial class BrowserToolbar : UserControl
    {
        public bool DrawBottomLine { get; set; }
        public bool DrawLinearBack { get; set; }

        public BrowserToolbar()
        {
            InitializeComponent();
            this.DoubleBuffered = true;            

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
                            Color.White, Color.Lavender, LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(linearBrush, r);
                }
            }
        }
    }
}
