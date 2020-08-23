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
    public partial class CircleButton : UserControl
    {

        float blank = 2;
        Color colorMouseOn = Color.LightGray;

        public CircleButton()
        {
            InitializeComponent();

            blank = this.Width / 4;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

            base.OnPaint(e);

            int width = this.Width - 1;
            int height = this.Height - 1;

            e.Graphics.FillEllipse(new SolidBrush(colorMouseOn), 0, 0, width, height);
            e.Graphics.DrawLine(new Pen(Color.White, 2), blank, height / 2 + 0.5f, width - blank + 0.5f, height / 2 + 0.5f);
            e.Graphics.DrawLine(new Pen(Color.White, 2), width / 2 + 0.5f, blank, width / 2 + 0.5f, height - blank + 0.5f);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            colorMouseOn = Color.FromArgb(229, 233, 227);
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            colorMouseOn = Color.LightGray;
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            colorMouseOn = Color.Orange;
            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            colorMouseOn = Color.FromArgb(229, 233, 227);
            this.Invalidate();
        }
    }
}
