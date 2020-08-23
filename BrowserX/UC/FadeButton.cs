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
using System.Data;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Drawing2D;

public class FadeButton : System.Windows.Forms.Button
{
    private bool calledbykey = false;
    private State mButtonState = State.None;
    private Timer mFadeIn = new Timer();
    private Timer mFadeOut = new Timer();
    private int mGlowAlpha = 0;
    private System.ComponentModel.Container components = null;
    /// <summary>
    ///     ''' Initialize the component with it's
    ///     ''' default settings.
    ///     ''' </summary>
    public FadeButton() : base()
    {
        // 该调用是 Windows 窗体设计器所必需的。

        InitializeComponent();

        // 在 InitializeComponent() 调用之后添加任何初始化,true表示将指定的样式应用到控件 

        this.DoubleBuffered = true;

        // 设置控件样式位能够充分地更改控件行为
        this.SetStyle(ControlStyles.UserPaint, true);
        this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        this.SetStyle(ControlStyles.DoubleBuffer, true);
        this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        this.SetStyle(ControlStyles.ResizeRedraw, true);
        this.SetStyle(ControlStyles.Selectable, true);
        this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        this.SetStyle(ControlStyles.UserPaint, true);
        this.UpdateStyles();
        this.BackColor = Color.Transparent;

        mFadeIn.Interval = mFadeInterval;
        mFadeOut.Interval = mFadeInterval;
    }

    protected override void Dispose(bool disposing)
    {
        if ((disposing))
        {
            if (!(components == null))
                components.Dispose();
        }
        base.Dispose(disposing);
    }
    private void InitializeComponent()
    {
        this.Name = "FadeButton";
        this.Size = new System.Drawing.Size(100, 32);

        this.Paint += this.VistaButton_Paint;
        this.KeyUp += this.VistaButton_KeyUp;
        this.KeyDown += this.VistaButton_KeyDown;
        this.MouseEnter += this.VistaButton_MouseEnter;
        this.MouseLeave += this.VistaButton_MouseLeave;
        this.MouseUp += this.VistaButton_MouseUp;
        this.MouseDown += this.VistaButton_MouseDown;
        this.GotFocus += this.VistaButton_MouseEnter;
        this.LostFocus += this.VistaButton_MouseLeave;
        this.mFadeIn.Tick += this.mFadeIn_Tick;
        this.mFadeOut.Tick += this.mFadeOut_Tick;
        this.Resize += this.VistaButton_Resize;
    }
    public enum State
    {
        None,
        Hover,
        Pressed
    }
    /// <summary>
    ///     ''' 按钮的样式
    ///     ''' </summary>
    public enum Style
    {
        /// <summary>
        ///         ''' Draw the button as normal
        ///         ''' </summary>
        Default,
        /// <summary>
        ///         ''' Only draw the background on mouse over.
        ///         ''' </summary>
        Flat
    }
    /// <summary>
    ///     ''' 用于设置按钮的用处
    ///     ''' </summary>
    public enum UseTo
    {
        Min,
        Close
    }

    // [Category("UseTo"),DefaultValue(UseTo.Close),Browsable(true),Description("设置按钮的用处")]
    private UseTo Ut = UseTo.Close; // 默认作为关闭按钮
    [Description("设置按钮的用处")]
    [DefaultValue(UseTo.Close)]
    [Browsable(true)]
    [Category("UseTo")]
    public UseTo UseToWhat
    {
        get
        {
            return Ut;
        }

        set
        {
            Ut = value;

            this.Invalidate();
        }
    }


    private bool mDrawHightLigth = false; // 是否画按钮的高光部分
    [Description("是否画按钮的高光部分")]
    [DefaultValue(UseTo.Close)]
    [Browsable(true)]
    [Category("Appearance")]
    public bool DrawHightLigthFlag
    {
        get
        {
            return mDrawHightLigth;
        }

        set
        {
            mDrawHightLigth = value;

            this.Invalidate();
        }
    }

    // Private mText As String = ""       '// 按钮上显示的文本
    // <Description("按钮上显示的文本."), Category("Text")>
    // Public Property ButtonText() As String

    // Get
    // Return mText
    // End Get

    // Set(ByVal Value As String)
    // mText = Value

    // Me.Invalidate()
    // End Set

    // End Property

    private Color mForeColor = Color.White; // 按钮上文字显示的颜色
    [Description("按钮上显示的文本的颜色.")]
    [Browsable(true)]
    [DefaultValue(typeof(Color), "White")]
    [Category("Appearance")]
    public new Color ForeColor
    {
        get
        {
            return mForeColor;
        }

        set
        {
            mForeColor = value;

            this.Invalidate();
        }
    }

    private ContentAlignment mTextAlign = ContentAlignment.MiddleCenter; // 按钮上显示的文本的对齐方式.
    [Description("按钮上显示的文本的对齐方式.")]
    [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
    [Category("Appearance")]
    public new ContentAlignment TextAlign
    {
        get
        {
            return mTextAlign;
        }

        set
        {
            mTextAlign = value;

            this.Invalidate();
        }
    }


    private Image mImage = null/* TODO Change to default(_) if this is not a reference type */; // 按钮上显示的图标.
    [Description("按钮上显示的图标.")]
    [DefaultValue(Constants.vbNull)]
    [Category("Image")]
    public new Image Image
    {
        get
        {
            return mImage;
        }

        set
        {
            mImage = value;

            this.Invalidate();
        }
    }

    private ContentAlignment mImageAlign = ContentAlignment.MiddleCenter; // 按钮上显示的文本的对齐方式.
    [Description("按钮上显示的图标的对齐方式.")]
    [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
    [Category("Image")]
    public new ContentAlignment ImageAlign
    {
        get
        {
            return mImageAlign;
        }

        set
        {
            mImageAlign = value;

            this.Invalidate();
        }
    }


    private Size mImageSize = new Size(24, 24); // 按钮上显示的图标的尺寸
    [Description("按钮上显示的图标的尺寸，默认 24x24.")]
    [DefaultValue(typeof(System.Drawing.Size), "24，24")]
    [Category("Image")]
    public System.Drawing.Size ImageSize
    {
        get
        {
            return mImageSize;
        }

        set
        {
            mImageSize = value;

            this.Invalidate();
        }
    }


    private Style mButtonStyle = Style.Default; // 按钮视觉样式
    [Description("按钮视觉样式")]
    [DefaultValue(typeof(Style), "Default")]
    [Category("Appearance")]
    public Style ButtonStyle
    {
        get
        {
            return mButtonStyle;
        }

        set
        {
            mButtonStyle = value;

            this.Invalidate();
        }
    }

    private int mCornerRadius = 3; // 按钮的四个角的弧度
    [Description("按钮四个角的弧度")]
    [DefaultValue(8)]
    [Category("Appearance")]
    public int CornerRadius
    {
        get
        {
            return mCornerRadius;
        }

        set
        {
            mCornerRadius = value;

            this.Invalidate();
        }
    }

    private int mFadeInterval = 20; // 按钮淡入淡出的速度
    [Description("按钮淡入淡出的时间(ms)")]
    [DefaultValue(20)]
    [Category("Appearance")]
    public int FadeInOutInterval
    {
        get
        {
            return mFadeInterval;
        }

        set
        {
            mFadeInterval = value;
            if ((mFadeInterval > 500))
                mFadeInterval = 20;

            this.Invalidate();
        }
    }



    private Color mHighlightColor = Color.Gray; // 按钮高亮颜色
    [Description("按钮高光部分的颜色")]
    [DefaultValue(typeof(Color), "White")]
    [Category("Appearance")]
    public Color HighlightColor
    {
        get
        {
            return mHighlightColor;
        }

        set
        {
            mHighlightColor = value;

            this.Invalidate();
        }
    }


    private Color mBoardColor = Color.Transparent; // 按钮高亮颜色
    [Description("按钮的边框颜色")]
    [DefaultValue(typeof(Color), "Transparent")]
    [Category("Appearance")]
    public Color BoardColor
    {
        get
        {
            return mBoardColor;
        }

        set
        {
            mBoardColor = value;

            this.Invalidate();
        }
    }

    private Color mGlowColor = Color.FromArgb(141, 189, 255); // 按钮鼠标悬停时的背景颜色
    [Description("按钮鼠标悬停时的背景颜色")]
    [DefaultValue(typeof(Color), "141,189,255")]
    [Category("Appearance")]
    public Color GlowColor
    {
        get
        {
            return mGlowColor;
        }

        set
        {
            mGlowColor = value;

            this.Invalidate();
        }
    }

    private Image mBackImage = null/* TODO Change to default(_) if this is not a reference type */; // 按钮鼠标悬停时的背景图片
    [Description("按钮鼠标悬停时的背景图片")]
    [DefaultValue(Constants.vbNull)]
    [Category("Appearance")]
    public Image BackImage
    {
        get
        {
            return mBackImage;
        }

        set
        {
            mBackImage = value;

            this.Invalidate();
        }
    }

    private Color mBaseColor = Color.DarkGray; // 按钮鼠标移出后的背景颜色
    [Description("按钮鼠标移出后的背景颜色")]
    [DefaultValue(typeof(Color), "DarkGray")]
    [Category("Appearance")]
    public Color BaseColor
    {
        get
        {
            return mBaseColor;
        }

        set
        {
            mBaseColor = value;

            this.Invalidate();
        }
    }


    /// <summary>
    ///     ''' 按钮的形状
    ///     ''' </summary>
    ///     ''' <param name="r"></param>
    ///     ''' <param name="r1"></param>
    ///     ''' <param name="r2"></param>
    ///     ''' <param name="r3"></param>
    ///     ''' <param name="r4"></param>
    ///     ''' <returns></returns>
    private GraphicsPath RoundRect(RectangleF r, float r1, float r2, float r3, float r4)
    {
        float x, y, w, h;
        x = r.X;
        y = r.Y;
        w = r.Width;
        h = r.Height;
        GraphicsPath rr = new GraphicsPath();
        rr.AddBezier(x, y + r1, x, y, x + r1, y, x + r1, y);
        rr.AddLine(x + r1, y, x + w - r2, y);
        rr.AddBezier(x + w - r2, y, x + w, y, x + w, y + r2, x + w, y + r2);
        rr.AddLine(x + w, y + r2, x + w, y + h - r3);
        rr.AddBezier(x + w, y + h - r3, x + w, y + h, x + w - r3, y + h, x + w - r3, y + h);
        rr.AddLine(x + w - r3, y + h, x + r4, y + h);
        rr.AddBezier(x + r4, y + h, x, y + h, x, y + h - r4, x, y + h - r4);
        rr.AddLine(x, y + h - r4, x, y + r1);
        return rr;
    }
    /// <summary>
    ///     ''' 对齐方式
    ///     ''' </summary>
    ///     ''' <param name="textalign"></param>
    ///     ''' <returns></returns>
    private StringFormat StringFormatAlignment(ContentAlignment textalign)
    {
        StringFormat sf = new StringFormat();
        switch ((textalign))
        {
            case ContentAlignment.TopLeft:         // test ok
                {
                    sf.LineAlignment = StringAlignment.Near;
                    sf.Alignment = StringAlignment.Near;
                    break;
                }

            case ContentAlignment.TopCenter:        // test ok
                {
                    sf.LineAlignment = StringAlignment.Near;
                    sf.Alignment = StringAlignment.Center;
                    break;
                }

            case ContentAlignment.TopRight:         // test ok
                {
                    sf.LineAlignment = StringAlignment.Near;
                    sf.Alignment = StringAlignment.Far;
                    break;
                }

            case ContentAlignment.MiddleLeft:
                {
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Near;
                    break;
                }

            case ContentAlignment.MiddleCenter:
                {
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;
                    break;
                }

            case ContentAlignment.MiddleRight:
                {
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Far;
                    break;
                }

            case ContentAlignment.BottomLeft:        // test ok
                {
                    sf.LineAlignment = StringAlignment.Far;
                    sf.Alignment = StringAlignment.Near;
                    break;
                }

            case ContentAlignment.BottomCenter:      // test ok
                {
                    sf.LineAlignment = StringAlignment.Far;
                    sf.Alignment = StringAlignment.Center;
                    break;
                }

            case ContentAlignment.BottomRight:       // test ok
                {
                    sf.LineAlignment = StringAlignment.Far;
                    sf.Alignment = StringAlignment.Far;
                    break;
                }
        }

        return sf;
    }
    /// <summary>
    ///     ''' 画出按钮的外框线条
    ///     ''' </summary>
    ///     ''' <param name="g">The graphics object used in the paint event.</param>
    private void DrawOuterStroke(Graphics g)
    {
        if ((this.ButtonStyle == Style.Flat & this.mButtonState == State.None))
            return;
        else
        {
            Rectangle r = this.ClientRectangle;
            r.Width = r.Width - 1;
            r.Height = r.Height - 1;
            GraphicsPath rr = RoundRect(r, CornerRadius, CornerRadius, CornerRadius, CornerRadius);
            Pen p = new Pen(this.mBoardColor);
            g.DrawPath(p, rr);      // 画出外框
        }
    }

    /// <summary>
    ///     ''' 画出按钮的内框线条
    ///     ''' </summary>
    ///     ''' <param name="g">The graphics object used in the paint event.</param>
    private void DrawInnerStroke(Graphics g)
    {
        if ((this.ButtonStyle == Style.Flat & this.mButtonState == State.None))
            return;
        else
        {
            Rectangle r = this.ClientRectangle;
            // r.X++  r.Y++ 
            r.X = r.X + 1;
            r.Y = r.Y + 1;
            r.Width = r.Width - 3;
            r.Height = r.Height - 3;
            GraphicsPath rr = RoundRect(r, CornerRadius, CornerRadius, CornerRadius, CornerRadius);
            Pen p = new Pen(this.mBoardColor);
            g.DrawPath(p, rr);      // 画出外框
        }
    }

    /// <summary>
    ///     ''' 画出按钮的背景
    ///     ''' </summary>
    ///     ''' <param name="g">The graphics object used in the paint event.</param>
    private void DrawBackground(Graphics g)
    {
        if ((this.ButtonStyle == Style.Flat & this.mButtonState == State.None))
            return;
        else
        {

            int alpha = 0;//Interaction.IIf(this.mButtonState == State.Pressed, 204, 127);
            if(this.mButtonState == State.Pressed) {
                alpha = 204;
            } else {
                alpha = 127;
            }
            Rectangle r = this.ClientRectangle;
            r.Width = r.Width - 1;
            r.Height = r.Height - 1;
            GraphicsPath rr = RoundRect(r, CornerRadius, CornerRadius, CornerRadius, CornerRadius);
            SolidBrush sb = new SolidBrush(this.BaseColor);
            g.FillPath(sb, rr);

            if (!(this.BackImage == null))
            {
                g.ResetClip();
                SolidBrush sb1 = new SolidBrush(Color.FromArgb(alpha, this.mBoardColor));
                g.FillPath(sb1, rr);
            }
        }
    }

    /// <summary>
    ///     ''' 画出按钮的上半部分高光颜色
    ///     ''' </summary>
    ///     ''' <param name="g">The graphics object used in the paint event.</param>
    private void DrawHighlight(Graphics g)
    {
        if ((this.ButtonStyle == Style.Flat & this.mButtonState == State.None))
            return;
        else
        {
            int alpha = 0;// Interaction.IIf(mButtonState == State.Pressed, 60, 150);
            if (this.mButtonState == State.Pressed) {
                alpha = 60;
            } else {
                alpha = 150;
            }

            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height / 2);
            GraphicsPath r = RoundRect(rect, CornerRadius, CornerRadius, 0, 0);
            LinearGradientBrush lg = new LinearGradientBrush(r.GetBounds(), Color.FromArgb(alpha, this.HighlightColor), Color.FromArgb(alpha / 3, this.HighlightColor), LinearGradientMode.Vertical);
            g.FillPath(lg, r);
        }
    }

    /// <summary>
    ///     ''' 当鼠标移上去的时候的炫光
    ///     ''' </summary>
    ///     ''' <param name="g">The graphics object used in the paint event.</param>
    private void DrawGlow(Graphics g)
    {
        if ((this.mButtonState == State.Pressed))
            return;
        else
        {
            GraphicsPath glow = new GraphicsPath();
            Rectangle r = this.ClientRectangle;
            // r.Width -= 3  r.Height -= 3 
            glow.AddPath(RoundRect(new Rectangle(r.Left + 1, r.Top + 1, r.Width - 3, r.Height - 3), CornerRadius, CornerRadius, CornerRadius, CornerRadius), true);

            GraphicsPath gp = RoundRect(new Rectangle(r.Left + 1, r.Top + 1, r.Width - 3, r.Height / 2 - 2), CornerRadius, CornerRadius, CornerRadius, CornerRadius);
            Color c = Color.FromArgb(mGlowAlpha, this.GlowColor);
            Color c1 = Color.FromArgb(mGlowAlpha / 2 + 50, Color.White);

            SolidBrush sb = new SolidBrush(c);

            SolidBrush sb1 = new SolidBrush(c1);
            g.FillPath(sb, glow);
            if ((mDrawHightLigth))
                g.FillPath(sb1, gp);
            g.ResetClip();
        }
    }
    /// <summary>
    ///     ''' 显示按钮的文本
    ///     ''' </summary>
    ///     ''' <param name="g">The graphics object used in the paint event.</param>
    private void DrawText(Graphics g)
    {
        // 计算文本的位置
        int x = 0;
        int y = 0;
        StringFormat sf = StringFormatAlignment(this.TextAlign);
        // Dim s As Size = g.MeasureString(Text, Font).ToSize()
        // '''中间对齐，文字在最中间
        // 'If (Me.TextAlign = ContentAlignment.MiddleCenter) Then
        // '    x = (Me.Width - s.Width) / 2 '文字相对控件x偏移
        // '    y = (Me.Height - s.Height) / 2  '文字相对控件y偏移
        // 'End If
        Rectangle r = new Rectangle(0, 0, this.Width, this.Height);

        if ((this.Enabled))
            g.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), r, sf);
        else
            g.DrawString(this.Text, this.Font, Brushes.LightGray, r, sf);
    }
    /// <summary>
    ///     ''' 画出按钮上的图标
    ///     ''' </summary>
    ///     ''' <param name="g"></param>
    private void DrawIcon(Graphics g)
    {
        if ((this.Ut == UseTo.Close)) {
            //do something
        } else if ((this.Ut == UseTo.Min)) {
            //do something
        } else  {
            //do something
        }
        // 新的画法
        // 计算图标的位置

        int x = 0;
        int y = 0;

        x = 12;      // 图标相对控件x偏移
        y = (base.ClientSize.Height - this.Image.Height) / 2;    // 图标相对控件y偏移

        // 画图标
        if ((Enabled))
        {
            // 在指定位置缩小图片画图
            float wSingle = Image.Width;
            float hSingle = Image.Height;
            g.DrawImage(Image, x, y, wSingle, hSingle);
        }
        else
        {

            // 在指定位置缩小图片画图
            float wSingle = Image.Width;
            float hSingle = Image.Height;
            g.DrawImage(Image, x, y, wSingle, hSingle);
        }
    }

    protected void VistaButton_Paint(object sender, System.Windows.Forms.PaintEventArgs pevent)
    {
        Graphics g = pevent.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        DrawBackground(g);
        if ((mDrawHightLigth))
            DrawHighlight(g);
        // 画鼠标移入时的图形
        DrawGlow(g);
        // 画按钮的内框线条
        DrawInnerStroke(g);
        // 写文本
        if ((this.Text.Length > 0))
            DrawText(pevent.Graphics);
        // 画图标
        if (!(this.Image == null))
            DrawIcon(pevent.Graphics);
    }


    private void VistaButton_Resize(object sender, EventArgs e)
    {
        Rectangle r = this.ClientRectangle;
        r.X -= 1;
        r.Y -= 1;
        r.Width += 2;
        r.Height += 2;

        GraphicsPath rr = RoundRect(r, CornerRadius, CornerRadius, CornerRadius, CornerRadius);
        this.Region = new Region(rr);
    }


    private void VistaButton_MouseEnter(object sender, EventArgs e)
    {
        mButtonState = State.Hover;
        mFadeOut.Stop();
        mFadeIn.Start();
    }

    private void VistaButton_MouseLeave(object sender, EventArgs e)
    {
        mButtonState = State.None;
        if ((this.mButtonStyle == Style.Flat))
            mGlowAlpha = 0;
        mFadeIn.Stop();
        mFadeOut.Start();
    }

    private void VistaButton_MouseDown(object sender, MouseEventArgs e)
    {
        if ((e.Button == MouseButtons.Left))
        {
            mButtonState = State.Pressed;
            if ((this.mButtonStyle != Style.Flat))
                mGlowAlpha = 255;
            mFadeIn.Stop();
            mFadeOut.Stop();
            this.Invalidate();
        }
    }

    private void VistaButton_MouseUp(object sender, MouseEventArgs e)
    {
        if ((e.Button == MouseButtons.Left))
        {
            mButtonState = State.Hover;
            mFadeIn.Stop();
            mFadeOut.Stop();
            this.Invalidate();
            if ((calledbykey == true))
            {
                this.OnClick(EventArgs.Empty);
                calledbykey = false;
            }
        }
    }
    private void mFadeIn_Tick(object sender, EventArgs e)
    {
        if ((this.ButtonStyle == Style.Flat))
            mGlowAlpha = 0;
        if ((mGlowAlpha + 30 >= 255))
        {
            mGlowAlpha = 255;
            mFadeIn.Stop();
        }
        else
            mGlowAlpha += 30;
        this.Invalidate();
    }

    private void mFadeOut_Tick(object sender, EventArgs e)
    {
        if ((this.ButtonStyle == Style.Flat))
            mGlowAlpha = 0;
        if ((mGlowAlpha - 30 <= 0))
        {
            mGlowAlpha = 0;
            mFadeOut.Stop();
        }
        else
            mGlowAlpha -= 30;
        this.Invalidate();
    }

    private void VistaButton_KeyDown(object sender, KeyEventArgs e)
    {
        if ((e.KeyCode == Keys.Space))
        {
            MouseEventArgs m = new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0);
            VistaButton_MouseDown(sender, m);
        }
    }

    private void VistaButton_KeyUp(object sender, KeyEventArgs e)
    {
        if ((e.KeyCode == Keys.Space))
        {
            MouseEventArgs m = new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0);
            calledbykey = true;
            VistaButton_MouseUp(sender, m);
        }
    }
}
