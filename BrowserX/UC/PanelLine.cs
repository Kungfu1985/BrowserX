using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing;

public class PanelLine : System.Windows.Forms.Panel
{
    public enum EnumOrientation
    {
        //水平
        Horizontal,
        //垂址
        Vertical
    }


    //线条的方向
    private EnumOrientation mOrientation = EnumOrientation.Horizontal;
    [Description("线条的方向，枚举值 水平0，垂直1。")]
    [Browsable(true)]
    [DefaultValue(typeof(EnumOrientation), "EnumOrientation.Horizontal")]
    [Category("Custom")]
    public EnumOrientation Orientation
    {
        get
        {
            return mOrientation;
        }
        set
        {
            mOrientation = value;
            this.Invalidate();
        }
    }

    //线条的边距
    private Padding mLineMargin = new Padding(3);
    [Description("线条的边距")]
    [Browsable(true)]
    [DefaultValue(typeof(Padding), "3")]
    [Category("Custom")]
    public Padding LineMargin
    {
        get
        {
            return mLineMargin;
        }
        set
        {
            mLineMargin = value;
            this.Invalidate();
        }
    }

    //线条的颜色。
    private Color mLineColor = Color.FromArgb(118, 118, 118);  
    [Description("线条的颜色。")]
    [Browsable(true)]
    [DefaultValue(typeof(Color), "118, 118, 118")]
    [Category("Custom")]
    public Color LineColor
    {
        get
        {
            return mLineColor;
        }
        set
        {
            mLineColor = value;
            this.Invalidate();
        }
    }

    public PanelLine() : base()
    {
        this.DoubleBuffered = true;

        // 设置控件样式位能够充分地更改控件行为
        this.SetStyle(System.Windows.Forms.ControlStyles.UserPaint, true);
        this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
    }


    protected override void OnPaint(System.Windows.Forms.PaintEventArgs pevent)
    {

        // pevent.ClipRectangle指在其中绘制的矩形,即使用父控件的背景色来画这个矩形按钮
        pevent.Graphics.FillRectangle(new SolidBrush(this.Parent.BackColor), pevent.ClipRectangle);

        // 画线，在控件的中心位置画线
        int W = base.Width /2;
        int H = base.Height /2;

        Rectangle Rec = new Rectangle(base.Margin.Left, base.Margin.Top, W, H);

        pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        //垂直线
        if(Orientation == EnumOrientation.Horizontal) {
            //垂直线
            int x = Bounds.Width / 2;
            pevent.Graphics.DrawLine(new Pen(LineColor), x, LineMargin.Top, x, Bounds.Height - LineMargin.Bottom);
        }
        else {
            //水平线
            int y = Bounds.Height / 2;
            pevent.Graphics.DrawLine(new Pen(LineColor), LineMargin.Left, y, Bounds.Width - LineMargin.Right, y);
        }
        //pevent.Graphics.FillPath((Brush)new SolidBrush(BackColor), Round);
    }
}
