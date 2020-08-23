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

public class CaptionButton : System.Windows.Forms.Button
{
    private bool calledbykey = false;
    private State mButtonState = State.None;
    private Timer mFadeIn = new Timer();
    private Timer mFadeOut = new Timer();
    private Timer mDelay = new Timer();
    private int mDelayMs = 1;
    private int mGlowAlpha = 0;
    private Color mThemeColor = Color.White;
    private System.ComponentModel.Container components = null;

    #region "四态按钮的图像区域"
    private int mCutStepWidth = 0;
    private int mCutStepHeight = 0;

    private Rectangle mNormalRect = new Rectangle(0, 0, 45, 29);      // 四态按钮贴图的正常状态区域
    private Rectangle mHoverRect = new Rectangle(45, 0, 45, 29);      // 四态按钮贴图的鼠标停留状态区域
    private Rectangle mDownRect = new Rectangle(90, 0, 45, 29);       // 四态按钮贴图的按下状态区域
    private Rectangle mDisableRect = new Rectangle(135, 0, 45, 29);   // 四态按钮贴图的禁用状态区域
    #endregion

    /// <summary>
    ///     ''' Initialize the component with it's
    ///     ''' default settings.
    ///     ''' </summary>
    public CaptionButton() : base()
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

        // 统计鼠标在组件内静止的时间，默认频率为1秒统计一次
        mDelay.Interval = 1000;
        // 统计鼠标在组件内静止的时间(ms)
        mDelayMs = 0;
        // 淡入/淡出的时间
        mFadeIn.Interval = mFadeInterval;
        mFadeOut.Interval = mFadeInterval;
    }


    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (components != null)
            {
                components.Dispose();
            }
            try
            {
                this.mFadeIn.Stop();
                this.mFadeOut.Stop();
                this.Paint -= this.CaptionButton_Paint;
                this.KeyUp -= this.CaptionButton_KeyUp;
                this.KeyDown -= this.CaptionButton_KeyDown;
                this.MouseEnter -= this.CaptionButton_MouseEnter;
                this.MouseMove -= this.CaptionButton_MouseMove;
                this.MouseLeave -= this.CaptionButton_MouseLeave;
                this.MouseUp -= this.CaptionButton_MouseUp;
                this.MouseDown -= this.CaptionButton_MouseDown;
                this.mFadeIn.Tick -= this.mFadeIn_Tick;
                this.mFadeOut.Tick -= this.mFadeOut_Tick;
                this.mDelay.Tick -= this.mDelay_Tick;
                this.Resize -= this.CaptionButton_Resize;
            }
            catch(Exception ex)
            {

            } 
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.Name = "CaptionButton";
        this.Size = new System.Drawing.Size(100, 32);

        this.Paint += this.CaptionButton_Paint;
        this.KeyUp += this.CaptionButton_KeyUp;
        this.KeyDown += this.CaptionButton_KeyDown;
        this.MouseEnter += this.CaptionButton_MouseEnter;
        this.MouseMove += this.CaptionButton_MouseMove;
        this.MouseLeave += this.CaptionButton_MouseLeave;
        this.MouseUp += this.CaptionButton_MouseUp;
        this.MouseDown += this.CaptionButton_MouseDown;
        // AddHandler Me.GotFocus, AddressOf Me.CaptionButton_MouseEnter
        // AddHandler Me.LostFocus, AddressOf Me.CaptionButton_MouseLeave
        this.mFadeIn.Tick += this.mFadeIn_Tick;
        this.mFadeOut.Tick += this.mFadeOut_Tick;
        this.mDelay.Tick += this.mDelay_Tick;
        this.Resize += this.CaptionButton_Resize;
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
    ///     ''' 按钮选中状态的样式，长方形或圆形,三角形
    ///     ''' </summary>
    public enum CheckStyle
    {
        /// <summary>
        ///         ''' Draw the button checked mark as Rectangle
        ///         ''' </summary>
        Rectangle,
        /// <summary>
        ///         ''' Draw the button checked mark as Round
        ///         ''' </summary>
        Round,
        /// <summary>
        ///         ''' Draw the button checked mark as Triangle
        ///         ''' </summary>
        Triangle,
        /// <summary>
        ///         ''' Draw the button checked mark as Square
        ///         ''' </summary>
        Square,
        /// <summary>
        ///         ''' Draw the button checked mark as Star(Pentagram)
        ///         ''' </summary>
        Star
    }
    /// <summary>
    ///     ''' 用于设置按钮的用处
    ///     ''' </summary>
    public enum UseTo
    {
        Min,
        Close
    }


    [DefaultValue(false)]
    [Browsable(true)]
    [Description("是否启用增强型MouseHover事件(MouseHoverExtend,可自定义MouseHover的触发时间.)")]
    [Category("Appearance")]
    public bool MouseHoverEx { get; set; }


    [DefaultValue(1)]
    [Browsable(true)]
    [Description("MouseHoverExtend事件的触发时间单位:秒(ms),默认1秒(ms).)")]
    [Category("Appearance")]
    public int MouseHoverExSecond { get; set; }


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
            Ut =value;

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
            mDrawHightLigth =value;

            this.Invalidate();
        }
    }

    private bool mDrawGradientColor = false; // 按钮的背景是否使用渐变颜色填充。
    [Description("按钮的背景是否使用渐变颜色填充。")]
    [Browsable(true)]
    [DefaultValue(false)]
    [Category("Gradient")]
    public bool DrawGradientColor
    {
        get
        {
            return mDrawGradientColor;
        }

        set
        {
            mDrawGradientColor =value;

            this.Invalidate();
        }
    }


    private Color mNormalGradientBeginColor = Color.FromArgb(129, 146, 165);  // 按钮正常状态下的背景渐变开始颜色。
    [Description("按钮正常状态下的背景渐变开始颜色。")]
    [Browsable(true)]
    [DefaultValue(typeof(Color), "129, 146, 165")]
    [Category("Gradient")]
    public Color NormalGradientBeginColor
    {
        get
        {
            return mNormalGradientBeginColor;
        }

        set
        {
            mNormalGradientBeginColor =value;

            this.Invalidate();
        }
    }

    private Color mNormalGradientEndColor = Color.FromArgb(129, 146, 165);  // 按钮正常状态下的背景渐变结束颜色。
    [Description("按钮正常状态下的背景渐变结束颜色。")]
    [Browsable(true)]
    [DefaultValue(typeof(Color), "129, 146, 165")]
    [Category("Gradient")]
    public Color NormalGradientEndColor
    {
        get
        {
            return mNormalGradientEndColor;
        }

        set
        {
            mNormalGradientEndColor =value;

            this.Invalidate();
        }
    }


    private Color mHoverGradientBeginColor = Color.FromArgb(129, 146, 165);  // 按钮悬停状态下的背景渐变开始颜色。
    [Description("按钮悬停状态下的背景渐变开始颜色。")]
    [Browsable(true)]
    [DefaultValue(typeof(Color), "129, 146, 165")]
    [Category("Gradient")]
    public Color HoverGradientBeginColor
    {
        get
        {
            return mHoverGradientBeginColor;
        }

        set
        {
            mHoverGradientBeginColor =value;

            this.Invalidate();
        }
    }

    private Color mHoverGradientEndColor = Color.FromArgb(129, 146, 165);  // 按钮悬停状态下的背景渐变结束颜色。
    [Description("按钮悬停状态下的背景渐变结束颜色。")]
    [Browsable(true)]
    [DefaultValue(typeof(Color), "129, 146, 165")]
    [Category("Gradient")]
    public Color HoverGradientEndColor
    {
        get
        {
            return mHoverGradientEndColor;
        }

        set
        {
            mHoverGradientEndColor =value;

            this.Invalidate();
        }
    }


    private Color mPressedGradientBeginColor = Color.FromArgb(129, 146, 165);  // 按钮按下状态下的背景渐变开始颜色。
    [Description("按钮按下状态下的背景渐变开始颜色。")]
    [Browsable(true)]
    [DefaultValue(typeof(Color), "129, 146, 165")]
    [Category("Gradient")]
    public Color PressedGradientBeginColor
    {
        get
        {
            return mPressedGradientBeginColor;
        }

        set
        {
            mPressedGradientBeginColor =value;

            this.Invalidate();
        }
    }

    private Color mPressedGradientEndColor = Color.FromArgb(129, 146, 165);  // 按钮按下状态下的背景渐变结束颜色。
    [Description("按钮按下状态下的背景渐变结束颜色。")]
    [Browsable(true)]
    [DefaultValue(typeof(Color), "129, 146, 165")]
    [Category("Gradient")]
    public Color PressedGradientEndColor
    {
        get
        {
            return mPressedGradientEndColor;
        }

        set
        {
            mPressedGradientEndColor =value;

            this.Invalidate();
        }
    }


    private bool mDrawTextBeforeImage = false; // 按钮上从左至右，画文本与图标。
    [Description("按钮上从左至右，先画文本再画图标。")]
    [DefaultValue(UseTo.Close)]
    [Browsable(true)]
    [Category("Appearance")]
    public bool DrawTextBeforeImage
    {
        get
        {
            return mDrawTextBeforeImage;
        }

        set
        {
            mDrawTextBeforeImage =value;

            this.Invalidate();
        }
    }


    private bool mDrawGlowFlag = false; // 是否画按钮的淡入淡出颜色
    [Description("是否画按钮的淡入淡出颜色")]
    [DefaultValue(false)]
    [Browsable(true)]
    [Category("Appearance")]
    public bool DrawGlowFlag
    {
        get
        {
            return mDrawGlowFlag;
        }

        set
        {
            mDrawGlowFlag =value;

            this.Invalidate();
        }
    }

    private bool mDrawGlowLeftIndentFlag = false; // 是否画按钮的淡入淡出颜色
    [Description("是否画按钮的淡入淡出颜色靠左缩进")]
    [DefaultValue(false)]
    [Browsable(true)]
    [Category("Appearance")]
    public bool DrawGlowLeftIndentFlag
    {
        get
        {
            return mDrawGlowLeftIndentFlag;
        }

        set
        {
            mDrawGlowLeftIndentFlag =value;

            this.Invalidate();
        }
    }

    private bool mChecked = false; // 按钮是否是选中状态
    [Description("按钮是否是选中状态")]
    [DefaultValue(false)]
    [Browsable(true)]
    [Category("Appearance")]
    public bool Checked
    {
        get
        {
            return mChecked;
        }

        set
        {
            mChecked =value;

            this.Invalidate();
        }
    }


    private bool mDrawCheckedMark = false; // 按钮选中状态是否画标记
    [Description("按钮选中状态时是否画标记，在按钮最左侧画选中状态[长方形或其它图形标记]")]
    [DefaultValue(false)]
    [Browsable(true)]
    [Category("Appearance")]
    public bool DrawCheckedMark
    {
        get
        {
            return mDrawCheckedMark;
        }

        set
        {
            mDrawCheckedMark =value;

            this.Invalidate();
        }
    }


    private CheckStyle mCheckedMarkStyle = CheckStyle.Rectangle; // 默认作为关闭按钮
    [Description("按钮选中状态的标记形状，当为[Round]时，可以设置对齐方式.")]
    [DefaultValue(typeof(CheckStyle), "Rectangle")]
    [Browsable(true)]
    [Category("Appearance")]
    public CheckStyle CheckedMarkStyle
    {
        get
        {
            return mCheckedMarkStyle;
        }

        set
        {
            mCheckedMarkStyle =value;

            this.Invalidate();
        }
    }

    private Color mCheckedMarkColor = Color.Blue; // 按钮选中状态的标记颜色
    [Description("按钮选中状态的标记颜色.")]
    [Browsable(true)]
    [DefaultValue(typeof(Color), "Blue")]
    [Category("Appearance")]
    public Color CheckedMarkColor
    {
        get
        {
            return mCheckedMarkColor;
        }

        set
        {
            mCheckedMarkColor =value;

            this.Invalidate();
        }
    }

    private bool mCheckedMarkColorUseThemeColor = false; // 按钮选中状态的标记颜色使用系统主题色.
    [Description("按钮选中状态的标记颜色使用系统主题色.")]
    [Browsable(true)]
    [DefaultValue(false)]
    [Category("Appearance")]
    public  bool CheckedMarkColorUseThemeColor
    {
        get
        {
            return mCheckedMarkColorUseThemeColor;
        }

        set
        {
            mCheckedMarkColorUseThemeColor =value;

            this.Invalidate();
        }
    }


    private Image mStateImage = null;// 按钮选中状态标记图标.
    [Description("按钮正常，悬停，按下，禁用时的四态图标，默认 180x29 像素.")]
    [DefaultValue(Constants.vbNull)]
    [Category("Appearance")]
    public Image StateImage
    {
        get
        {
            return mStateImage;
        }

        set
        {
            mStateImage = value;

            GetParameter(mStateImage);
            this.Invalidate();
        }
    }

    private int mStateImageFrame = 4;// 按钮四态图片中的图标数
    [Description("按钮正常，悬停，按下，禁用时的四态图标中包含的图标数,默认为4.")]
    [DefaultValue(4)]
    [Category("Appearance")]
    public int StateImageFrame
    {
        get
        {
            return mStateImageFrame;
        }

        set
        {
            mStateImageFrame = value;

            this.Invalidate();
        }
    }

    private bool mStateImageStrech = false;// 按钮四态图片中的图标数
    [Description("按钮状态图标是否缩放展示.")]
    [DefaultValue(false)]
    [Category("Appearance")]
    public bool StateImageStrech
    {
        get
        {
            return mStateImageStrech;
        }

        set
        {
            mStateImageStrech = value;

            this.Invalidate();
        }
    }

    private Image mCheckedMaskImage = null/* TODO Change to default(_) if this is not a reference type */; // 按钮选中状态标记图标.
    [Description("按钮标示选中状态的图标，默认 8x8 像素.")]
    [DefaultValue(Constants.vbNull)]
    [Category("Appearance")]
    public  Image CheckedMaskImage
    {
        get
        {
            return mCheckedMaskImage;
        }

        set
        {
            mCheckedMaskImage =value;

            this.Invalidate();
        }
    }


    private ContentAlignment mCheckedMaskImageAlign = ContentAlignment.MiddleCenter; // 按钮选中状态标记图标的对齐方式.
    [Description("按钮选中状态标记图标的对齐方式.")]
    [DefaultValue(typeof(ContentAlignment), "MiddleLeft")]
    [Category("Appearance")]
    public  ContentAlignment CheckedMaskImageAlign
    {
        get
        {
            return mCheckedMaskImageAlign;
        }

        set
        {
            mCheckedMaskImageAlign =value;

            this.Invalidate();
        }
    }

    private Color mCheckedTextColor = Color.White; // 按钮上文字显示的颜色
    [Description("按钮处于选中状态时文本的颜色.")]
    [Browsable(true)]
    [DefaultValue(typeof(Color), "Blue")]
    [Category("Appearance")]
    public  Color CheckedTextColor
    {
        get
        {
            return mCheckedTextColor;
        }

        set
        {
            mCheckedTextColor =value;

            this.Invalidate();
        }
    }

    private bool mCheckedTextColorUseThemeColor = false; // 按钮处于选中状态时文本的颜色使用系统主题色.
    [Description("按钮处于选中状态时文本的颜色使用系统主题色.")]
    [Browsable(true)]
    [DefaultValue(false)]
    [Category("Appearance")]
    public  bool CheckedTextColorUseThemeColor
    {
        get
        {
            return mCheckedTextColorUseThemeColor;
        }

        set
        {
            mCheckedTextColorUseThemeColor =value;

            this.Invalidate();
        }
    }


    private Color mForeColor = Color.White; // 按钮上文字显示的颜色
    [Description("按钮上显示的文本的颜色.")]
    [Browsable(true)]
    [DefaultValue(typeof(Color), "White")]
    [Category("Appearance")]
    public new  Color ForeColor
    {
        get
        {
            return mForeColor;
        }

        set
        {
            mForeColor =value;

            this.Invalidate();
        }
    }

    private Color mPressedColor = Color.White; // 按钮上鼠标按键按下时的背景颜色
    [Description("按钮在鼠标按键按下时的背景颜色.")]
    [Browsable(true)]
    [DefaultValue(typeof(Color), "White")]
    [Category("Appearance")]
    public  Color PressedColor
    {
        get
        {
            return mPressedColor;
        }

        set
        {
            mPressedColor =value;

            this.Invalidate();
        }
    }

    private ContentAlignment mTextAlign = ContentAlignment.MiddleCenter; // 按钮上显示的文本的对齐方式.
    [Description("按钮上显示的文本的对齐方式.")]
    [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
    [Category("Appearance")]
    public  ContentAlignment TextAlign
    {
        get
        {
            return mTextAlign;
        }

        set
        {
            mTextAlign =value;

            this.Invalidate();
        }
    }


    private Image mImage = null/* TODO Change to default(_) if this is not a reference type */; // 按钮上显示的图标.
    [Description("按钮上显示的图标.")]
    [DefaultValue(Constants.vbNull)]
    [Category("Image")]
    public  Image Image
    {
        get
        {
            return mImage;
        }

        set
        {
            mImage =value;

            this.Invalidate();
        }
    }


    private ContentAlignment mImageAlign = ContentAlignment.MiddleCenter; // 按钮上显示的文本的对齐方式.
    [Description("按钮上显示的图标的对齐方式.")]
    [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
    [Category("Image")]
    public  ContentAlignment ImageAlign
    {
        get
        {
            return mImageAlign;
        }

        set
        {
            mImageAlign =value;

            this.Invalidate();
        }
    }

    private bool mImageTintUseThemeColor = false;  // 按钮上显示的图标颜色使用系统主题色进行调色.
    [Description("按钮上显示的图标使用系统主题色进行调色.注意：图标用AI导出PNG，左上角(0,0)坐标请设置为透明色！")]
    [DefaultValue(false)]
    [Category("Image")]
    public  bool ImageTintUseThemeColor
    {
        get
        {
            return mImageTintUseThemeColor;
        }

        set
        {
            mImageTintUseThemeColor =value;

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
            mImageSize =value;

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
            mButtonStyle =value;

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
            mCornerRadius =value;

            this.Invalidate();
        }
    }

    private int mDrawIconWidth = 0; // 将组件分成两块区域，适用于左边画图标，右边画文字
    [Description("将组件分成两块区域，适用于左边画图标(居中)，右边画文字（支持9种对齐方式）。")]
    [DefaultValue(8)]
    [Category("Appearance")]
    public int DrawIconWidth
    {
        get
        {
            return mDrawIconWidth;
        }

        set
        {
            mDrawIconWidth =value;

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
            mFadeInterval =value;
            if ((mFadeInterval > 500))
                mFadeInterval = 20;

            this.Invalidate();
        }
    }


    private Color mHighlightColor = Color.Gray; // 按钮高亮颜色
    [Description("按钮高光部分的颜色，当属性[DrawHightLigth]设置为[true]时生效.")]
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
            mHighlightColor =value;

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
            mBoardColor =value;

            this.Invalidate();
        }
    }


    private int mBoardWidth = 1; // 按钮外框线条的宽度
    [Description("按钮外框线条的宽度")]
    [DefaultValue(1)]
    [Category("Appearance")]
    public int BoardWidth
    {
        get
        {
            return mBoardWidth;
        }

        set
        {
            mBoardWidth =value;
            if ((mBoardWidth > 256))
                mFadeInterval = 1;

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
            mGlowColor =value;

            this.Invalidate();
        }
    }

    private bool mGlowColorUseThemeColor = false; // 按钮鼠标悬停时的背景颜色
    [Description("按钮鼠标悬停时的背景颜色使用系统主题色")]
    [DefaultValue(false)]
    [Category("Appearance")]
    public bool GlowColorUseThemeColor
    {
        get
        {
            return mGlowColorUseThemeColor;
        }

        set
        {
            mGlowColorUseThemeColor =value;

            this.Invalidate();
        }
    }




    private int mGlowAlphaValue = 255; // 按钮鼠标悬停时的背景颜色
    [Description("按钮鼠标悬停时的背景颜色Alpha通道值[0-255]")]
    [DefaultValue(255)]
    [Category("Appearance")]
    public int GlowColorAlpha
    {
        get
        {
            return mGlowAlphaValue;
        }

        set
        {
            mGlowAlphaValue =value;

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
            mBackImage =value;

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
            mBaseColor =value;

            this.Invalidate();
        }
    }


    private bool mDrawBoard = false; // 是否画按钮的外边框
    [Description("是否画按钮的外边框")]
    [DefaultValue(false)]
    [Browsable(true)]
    [Category("Appearance")]
    public bool DrawBoard
    {
        get
        {
            return mDrawBoard;
        }

        set
        {
            mDrawBoard =value;

            this.Invalidate();
        }
    }

    private bool mDrawMouseHoverBoard = false; // 是否在鼠标移入按钮时画外边框
    [Description("是否在鼠标移入按钮时画外边框")]
    [DefaultValue(false)]
    [Browsable(true)]
    [Category("Appearance")]
    public bool DrawMouseHoverBoard
    {
        get
        {
            return mDrawMouseHoverBoard;
        }

        set
        {
            mDrawMouseHoverBoard =value;

            this.Invalidate();
        }
    }




    // Private events As New System.ComponentModel.EventHandlerList

    /// <summary>CaptionButton,自定义鼠标悬停事件</summary>
    public event EventHandler MouseHoverExtend
    {
        add
        {
            Events.AddHandler("MouseHoverExtend", value);
        }
        remove
        {
            Events.RemoveHandler("MouseHoverExtend", value);
        }
    }

    void OnMouseHoverExtend(object sender, EventArgs e)
    {
        try {
            EventHandler eh = Events["MouseHoverExtend"] as EventHandler;

            if (eh != null) {
                eh.Invoke(sender, e);
            }                
        } catch(Exception ex) {
            //do something
        }
        
    }


    /// <summary>
    /// 计算状态Image的状态数[正常，悬停，按下，禁用]，以便自绘
    /// </summary>
    private void GetParameter(Image img)
    {
        try
        {
            // 计算FrameNumber,如果宽度大于高度，X轴切割， 高度大于宽度Y轴切割
            // 如果高宽相等，只有一帧，不需要动画
            if (img.Size.Width > img.Size.Height)
            {

                int intStateWidth = img.Size.Width / StateImageFrame;
                int intStateCount = img.Size.Width / intStateWidth;

                mCutStepWidth = img.Size.Width / StateImageFrame;
                mCutStepHeight = img.Size.Height;


                //Win32API.OutputDebugStringA( string.Format("当前按钮状态图中有{0}态，宽度为：{1}", intStateCount, intStateWidth )); 
                if (intStateCount>0)
                {
                    // 四态按钮贴图的正常状态区域
                    mNormalRect = new Rectangle(0, 0, intStateWidth, img.Size.Height);
                    // 四态按钮贴图的鼠标停留状态区域
                    mHoverRect = new Rectangle(intStateWidth*1, 0, intStateWidth, img.Size.Height);
                    // 四态按钮贴图的按下状态区域
                    mDownRect = new Rectangle(intStateWidth*2, 0, intStateWidth, img.Size.Height);
                    // 四态按钮贴图的禁用状态区域
                    mDisableRect = new Rectangle(intStateWidth*3, 0, intStateWidth, img.Size.Height);
                }

            }
            else if (img.Size.Width == img.Size.Height)
            {
                mNormalRect = new Rectangle(0, 0, img.Size.Width, img.Size.Height );
                mCutStepWidth = img.Size.Width;
                mCutStepHeight = img.Size.Height;

            }
            else if ((img.Size.Height > img.Size.Width))
            {
                int intStateCount = img.Size.Height  / img.Size.Width;
                int intStateWidth = img.Size.Height / intStateCount;

                mCutStepWidth = img.Size.Height / StateImageFrame;
                mCutStepHeight = img.Size.Width;

                if (intStateCount > 0)
                {
                    // 四态按钮贴图的正常状态区域
                    mNormalRect = new Rectangle(0, 0, intStateWidth, img.Size.Height);
                    // 四态按钮贴图的鼠标停留状态区域
                    mHoverRect = new Rectangle(0, intStateWidth * 1,  intStateWidth, img.Size.Height);
                    // 四态按钮贴图的按下状态区域
                    mDownRect = new Rectangle(0, intStateWidth * 2, intStateWidth, img.Size.Height);
                    // 四态按钮贴图的禁用状态区域
                    mDisableRect = new Rectangle(0, intStateWidth * 3, intStateWidth, img.Size.Height);
                }
            }
        }
        catch (Exception ex)
        {
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

    private GraphicsPath RoundRect(int x, int y, int width, int height, int radius)
    {
        GraphicsPath gp = new GraphicsPath();
        gp.AddArc(x, y, radius, radius, 180, 90);
        gp.AddArc(width - radius, y, radius, radius, 270, 90);
        gp.AddArc(width - radius, height - radius, radius, radius, 0, 90);
        gp.AddArc(x, height - radius, radius, radius, 90, 90);
        gp.CloseAllFigures();
        return gp;
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
            case ContentAlignment.TopLeft         // test ok
           :
                {
                    sf.LineAlignment = StringAlignment.Near;
                    sf.Alignment = StringAlignment.Near;
                    break;
                }

            case ContentAlignment.TopCenter        // test ok
     :
                {
                    sf.LineAlignment = StringAlignment.Near;
                    sf.Alignment = StringAlignment.Center;
                    break;
                }

            case ContentAlignment.TopRight         // test ok
     :
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

            case ContentAlignment.BottomLeft        // test ok
     :
                {
                    sf.LineAlignment = StringAlignment.Far;
                    sf.Alignment = StringAlignment.Near;
                    break;
                }

            case ContentAlignment.BottomCenter      // test ok
     :
                {
                    sf.LineAlignment = StringAlignment.Far;
                    sf.Alignment = StringAlignment.Center;
                    break;
                }

            case ContentAlignment.BottomRight       // test ok
     :
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
    private void DrawOuterStroke(Graphics g, int bdwidth = 1)
    {
        if ((this.ButtonStyle == Style.Flat & this.mButtonState == State.None))
            return;
        else
        {
            Rectangle r = this.ClientRectangle;
            r.Width = r.Width - 1;
            r.Height = r.Height - 1;
            GraphicsPath rr = RoundRect(r, CornerRadius, CornerRadius, CornerRadius, CornerRadius);
            Pen p = new Pen(this.mBoardColor, bdwidth);
            g.DrawPath(p, rr);      // 画出外框
        }
    }

    /// <summary>
    ///     ''' 画出按钮的内框线条
    ///     ''' </summary>
    ///     ''' <param name="g">The graphics object used in the paint event.</param>
    private void DrawInnerStroke(Graphics g, int bdwidth = 1)
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
            Pen p = new Pen(this.mBoardColor, bdwidth);
            g.DrawPath(p, rr);      // 画出内框
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
            // 先用父组件的背景色填充，防止黑线框
            g.FillRectangle(new SolidBrush(this.Parent.BackColor), this.ClientRectangle);
            //使用透明度来区分按下或悬停状态
            int alpha = 0;
            if(mButtonState == State.Pressed) {
                alpha = 204;
            }else
            {
                alpha = 127;
            }
            Rectangle r = this.ClientRectangle;
            if (Padding.All > 0) {
                //Win32API.OutputDebugStringA( string.Format("原按钮尺寸X:{0},Y:{1},Width:{2},Height:{3}",r.X,r.Y,r.Width,r.Height) ); 
                r = new Rectangle(this.ClientRectangle.X + Padding.Left/2, 
                                  this.ClientRectangle.Y + Padding.Top/2,
                                  this.ClientRectangle.Width - Padding.Right, this.ClientRectangle.Height - Padding.Bottom );
                //Win32API.OutputDebugStringA(string.Format("改按钮尺寸X:{0},Y:{1},Width:{2},Height:{3}", r.X, r.Y, r.Width, r.Height));
            }
            else
            {
                //Win32API.OutputDebugStringA("Margin.All<=0");
                //r = this.ClientRectangle;
            }
            
            
            GraphicsPath rr = new GraphicsPath();
            if (CornerRadius > 0) {
                r.Width = r.Width - 1;
                r.Height = r.Height - 1;
                rr = RoundRect(r, CornerRadius, CornerRadius, CornerRadius, CornerRadius);
            }
            else
            {
                rr.AddRectangle(r);
            }            

            // Dim rr As GraphicsPath = RoundRect(r.X, r.Y, r.Width, r.Height, CornerRadius)

            SolidBrush sb;
            LinearGradientBrush bBrush;
            if (mButtonState == State.Pressed)
            {
                sb = new SolidBrush(this.mPressedColor);
                bBrush = new LinearGradientBrush(r, this.PressedGradientBeginColor, this.PressedGradientEndColor, 90.0F);
            }
            else
            {
                sb = new SolidBrush(this.mBaseColor);
                bBrush = new LinearGradientBrush(r, this.NormalGradientBeginColor, this.NormalGradientEndColor, 90.0F);
            }

            // 线性渐变填充
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.High;

            if (this.DrawGradientColor) {
                g.FillPath(bBrush, rr);
            } else {
                g.FillPath(sb, rr);
            }               

            // g.FillPath(sb, rr)

            sb.Dispose();
            bBrush.Dispose();
            if (this.BackImage != null)
            {
                g.ResetClip();
                SolidBrush sb1 = new SolidBrush(Color.FromArgb(alpha, this.mBoardColor));
                g.FillPath(sb1, rr);
                sb1.Dispose();
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
            //int alpha = Interaction.IIf(mButtonState == State.Pressed, 60, 150);
            int alpha = 0;
            if (mButtonState == State.Pressed) {
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
        if (this.mButtonState == State.Pressed)
            return;
        else
        {
            GraphicsPath glow = new GraphicsPath();
            Rectangle r = this.ClientRectangle;
            if (Padding.All > 0)
            {
                r = new Rectangle(this.ClientRectangle.X + Padding.Left/2,
                                  this.ClientRectangle.Y + Padding.Top/2,
                                  this.ClientRectangle.Width - Padding.Right, this.ClientRectangle.Height - Padding.Bottom);
            }
            // r.Width -= 3  r.Height -= 3 
            if (this.DrawGlowLeftIndentFlag)
                glow.AddPath(RoundRect(new Rectangle(r.Left + 0, r.Top + 0, r.Width - 0, r.Height - 0), CornerRadius, CornerRadius, CornerRadius, CornerRadius), true);
            else
                glow.AddPath(RoundRect(new Rectangle(r.Left + 1, r.Top + 1, r.Width - 3, r.Height - 3), CornerRadius, CornerRadius, CornerRadius, CornerRadius), true);

            GraphicsPath gpHightLigth = RoundRect(new Rectangle(r.Left + 1, r.Top + 1, r.Width - 3, r.Height / 2 - 2), CornerRadius, CornerRadius, CornerRadius, CornerRadius);
            // Dim c As Color = Color.FromArgb(mGlowAlpha, Me.GlowColor)

            Color c;
            if (mGlowColorUseThemeColor)
            {
                GetColorFormSystemColor();
                c = Color.FromArgb(mGlowAlpha, this.mThemeColor);
            }
            else
                c = Color.FromArgb(mGlowAlpha, this.GlowColor);
            Color c1 = Color.FromArgb(mGlowAlpha / 2 + 50, Color.White);

            SolidBrush sb = new SolidBrush(c);
            LinearGradientBrush bBrush;

            SolidBrush sb1 = new SolidBrush(c1);

            // 线性渐变填充
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (this.DrawGradientColor)
            {
                bBrush = new LinearGradientBrush(r, Color.FromArgb(mGlowAlpha, this.HoverGradientBeginColor), Color.FromArgb(mGlowAlpha, this.HoverGradientEndColor), 90.0F);
                g.FillPath(bBrush, glow);
            }
            else
            {
                bBrush = new LinearGradientBrush(r, this.GlowColor, this.GlowColor, 90.0F);
                g.FillPath(sb, glow);
            }

            // g.FillPath(sb, glow)

            if (mDrawHightLigth) 
            {
                g.FillPath(sb1, gpHightLigth);
            }
                
            g.ResetClip();
            bBrush.Dispose();
            sb.Dispose();
            sb1.Dispose();
            gpHightLigth.Dispose();
            glow.Dispose();
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
        Size s = g.MeasureString(Text, Font).ToSize();
        // '''中间对齐，文字在最中间
        // 'If (Me.TextAlign = ContentAlignment.MiddleCenter) Then
        // '    x = (Me.Width - s.Width) / 2 '文字相对控件x偏移
        // '    y = (Me.Height - s.Height) / 2  '文字相对控件y偏移
        // 'End If
        Rectangle r;
        if ((mDrawIconWidth > 0 && mDrawIconWidth < this.Width))
        {
            if ((this.mDrawTextBeforeImage))
                r = new Rectangle(0, 0, this.Width - mDrawIconWidth, this.Height);
            else
                r = new Rectangle(mDrawIconWidth, 0, this.Width- mDrawIconWidth, this.Height);
        }
        else
        {
            r = new Rectangle(0, 0, this.Width, this.Height);
        }
            

        if (this.Enabled)
        {
            if (this.Checked)
            {
                if ((this.mCheckedTextColorUseThemeColor))
                {
                    GetColorFormSystemColor();
                    g.DrawString(this.Text, this.Font, new SolidBrush(this.mThemeColor), r, sf);
                }
                else
                {
                    g.DrawString(this.Text, this.Font, new SolidBrush(this.CheckedTextColor), r, sf);
                }                    
            }
            else
            {
                g.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), r, sf);
            }
                
        }
        else
        {
            g.DrawString(this.Text, this.Font, Brushes.LightGray, r, sf);
        }
            
    }


    /// <summary>
    ///     ''' 画出按钮上的图标
    ///     ''' </summary>
    ///     ''' <param name="g"></param>
    private void DrawIcon(Graphics g)
    {
        if ((this.Ut == UseTo.Close)) {
        } else if ((this.Ut == UseTo.Min)) {
        } else {
        }
        // 新的画法
        // 计算图标的位置

        int x = 0;
        int y = 0;

        if ((mDrawIconWidth > 0 && mDrawIconWidth < this.ClientSize.Width))
        {
            if ((this.DrawTextBeforeImage))
            {
                x = this.ClientSize.Width - mDrawIconWidth + (mDrawIconWidth - this.ImageSize.Width) / 2;      // 图标相对控件x偏移
                y = (this.ClientSize.Height - this.ImageSize.Height) / 2;    // 图标相对控件y偏移
            }
            else
            {
                x = (mDrawIconWidth - this.ImageSize.Width) / 2;      // 图标相对控件x偏移
                y = (this.ClientSize.Height - this.ImageSize.Height) / 2;    // 图标相对控件y偏移
            }
        }
        else
        {
            x = (this.ClientSize.Width - this.ImageSize.Width) / 2;      // 图标相对控件x偏移
            y = (this.ClientSize.Height - this.ImageSize.Height) / 2;    // 图标相对控件y偏移
        }

        // 画图标
        if (this.Enabled)
        {
            // 在指定位置缩小图片画图
            float wSingle = ImageSize.Width;
            float hSingle = ImageSize.Height;
            if (this.Checked)
            {
                if ((this.ImageTintUseThemeColor))
                {
                    GetColorFormSystemColor();
                    Image remImage;
                    remImage = ImageTintColor(Image, mThemeColor);
                    g.DrawImage(remImage, x, y, wSingle, hSingle);
                    remImage.Dispose();
                    remImage = null;
                }
                else
                {
                    g.DrawImage(Image, x, y, wSingle, hSingle);
                }
                    
            }
            else
            {
                g.DrawImage(Image, x, y, wSingle, hSingle);
            }
                
        }
        else
        {
            float wSingle = ImageSize.Width;
            float hSingle = ImageSize.Height;
            Image imgImageAlpha = null;
            try
            {
                imgImageAlpha = AlphaColor(Image);
                if(imgImageAlpha != null) {
                    g.DrawImage(imgImageAlpha.Clone() as Image, x, y, wSingle, hSingle);
                } else {
                    g.DrawImage(Image, x, y, wSingle, hSingle);
                }
            }
            catch (Exception ex)
            {
                ControlPaint.DrawImageDisabled(g, Image, x, y, SystemColors.HighlightText);
            }
            finally
            {
                imgImageAlpha.Dispose() ;
                imgImageAlpha = null;
            }
            
        }
    }


    /// <summary>
    ///     ''' 画出按钮的四态图标
    ///     ''' </summary>
    ///     ''' <param name="g"></param>
    private void DrawStateImage(Graphics g)
    {
        // 画图标
        Rectangle mDestRect = new Rectangle(0,0,0,0);

        if (this.StateImageStrech)
        {
            mDestRect.X = this.ClientRectangle.X;
            mDestRect.Y = this.ClientRectangle.Y;
            mDestRect.Width = this.ClientRectangle.Width;
            mDestRect.Height = this.ClientRectangle.Height;
        }
        else
        {
            // 根据ImageAling来画图片的位置
            switch (ImageAlign)
            {
                case ContentAlignment.TopLeft:         // test ok
                    {
                        mDestRect.X = this.ClientRectangle.X;
                        mDestRect.Y = this.ClientRectangle.Y;
                        break;
                    }
                case ContentAlignment.TopCenter:        // test ok
                    {
                        mDestRect.X = this.ClientRectangle.Width / 2 - mCutStepWidth / 2;
                        mDestRect.Y = this.ClientRectangle.Y;
                        break;
                    }
                case ContentAlignment.TopRight:         // test ok
                    {
                        mDestRect.X = this.ClientRectangle.Width - mCutStepWidth;
                        mDestRect.Y = this.ClientRectangle.Y;
                        break;
                    }
                case ContentAlignment.MiddleLeft:
                    {
                        mDestRect.X = this.ClientRectangle.X;
                        mDestRect.Y = this.ClientRectangle.Height / 2 - mCutStepHeight / 2;
                        break;
                    }
                case ContentAlignment.MiddleCenter:
                    {
                        mDestRect.X = this.ClientRectangle.Width / 2 - mCutStepWidth / 2;
                        mDestRect.Y = this.ClientRectangle.Height / 2 - mCutStepHeight / 2;
                        break;
                    }
                case ContentAlignment.MiddleRight:
                    {
                        mDestRect.X = this.ClientRectangle.Width - mCutStepWidth;
                        mDestRect.Y = this.ClientRectangle.Height / 2 - mCutStepHeight / 2;
                        break;
                    }
                case ContentAlignment.BottomLeft:        // test ok
                    {
                        mDestRect.X = this.ClientRectangle.X;
                        mDestRect.Y = this.ClientRectangle.Height - mCutStepHeight;
                        break;
                    }
                case ContentAlignment.BottomCenter:      // test ok
                    {
                        mDestRect.X = this.ClientRectangle.Width / 2 - mCutStepWidth / 2;
                        mDestRect.Y = this.ClientRectangle.Height - mCutStepHeight;
                        break;
                    }
                case ContentAlignment.BottomRight:       // test ok
                    {
                        mDestRect.X = this.ClientRectangle.Width - mCutStepWidth;
                        mDestRect.Y = this.ClientRectangle.Height - mCutStepHeight;
                        break;
                    }
            }
            mDestRect.Width = mCutStepWidth;
            mDestRect.Height = mCutStepHeight;
        }

        if (this.Enabled)
        {
            

            if (this.mButtonState == State.Hover)
            {
                g.DrawImage(StateImage, mDestRect, mHoverRect, GraphicsUnit.Pixel);
            }else if (this.mButtonState == State.Pressed)
            {
                g.DrawImage(StateImage, mDestRect, mDownRect, GraphicsUnit.Pixel);
            }
            else
            {
                g.DrawImage(StateImage, mDestRect, mNormalRect, GraphicsUnit.Pixel);
            }
        }
        else
        {
            g.DrawImage(StateImage, mDestRect, mDisableRect, GraphicsUnit.Pixel);
        }
    }

    /// <summary>
    /// 给指定的图片添加Alpha值（透明度）
    /// </summary>
    /// <param name="sources"></param>
    /// <returns></returns>
    public Bitmap AlphaColor(Image sources)
    {
        // 在这里给图片设置透明度
        try
        {
            // 设置透明度
            Bitmap remImage = new Bitmap(sources.Width, sources.Height);
            Bitmap imgDarkColor = null;
            // 一切都在原图上进行
            remImage = sources.Clone() as Bitmap; // 内存中的原图拷贝
            // 在内存中新建一块新画布
            if (!(imgDarkColor == null))
                imgDarkColor = null;
            imgDarkColor = new Bitmap(remImage.Width, remImage.Height);

            // 颜色矩阵来设置透明度
            Graphics gr = Graphics.FromImage(imgDarkColor);

            float[][] matrixArray = new[] { new float[] { 1, 0, 0, 0, 0 }, 
                                            new float[] { 0, 1, 0, 0, 0 }, 
                                            new float[] { 0, 0, 1, 0, 0 }, 
                                            new float[] { 0, 0, 0, 0.30F, 0 }, 
                                            new float[] { 0, 0, 0, 0, 0 } };

            System.Drawing.Imaging.ColorMatrix cMatrix = new System.Drawing.Imaging.ColorMatrix(matrixArray);
            System.Drawing.Imaging.ImageAttributes imgAttr = new System.Drawing.Imaging.ImageAttributes();

            imgAttr.SetColorMatrix(cMatrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);
            // 繪製圖片
            Rectangle recDest = new Rectangle(0, 0, remImage.Width, remImage.Height);

            // 繪製圖片，以半透明方式在画布上画出当前选择的Item的图片
            gr.DrawImage(remImage, recDest, 0, 0, remImage.Width, remImage.Height, GraphicsUnit.Pixel, imgAttr);

            remImage.Dispose();
            remImage = null;

            gr.Dispose();
            gr = null;

            return (imgDarkColor);
        }
        catch (Exception ex)
        {
        }
        return null;
        // 颜色矩阵来反相图片，结束 
    }



    /// <summary>
    ///     ''' 画出按钮的选中标记
    ///     ''' </summary>
    ///     ''' <param name="g"></param>
    private void DrawCheckMark(Graphics g)
    {
        if ((this.CheckedMaskImage != null))
        {
            // 画选中标记图标
            Rectangle mDestRect = new Rectangle();
            int mCutStepWidth = 0;
            int mCutStepHeight = 0;
            mCutStepWidth = this.CheckedMaskImage.Size.Width;
            mCutStepHeight = this.CheckedMaskImage.Size.Height;
            switch ((CheckedMaskImageAlign))
            {
                case ContentAlignment.TopLeft:         // test ok
                    {
                        mDestRect.X = this.ClientRectangle.X;
                        mDestRect.Y = this.ClientRectangle.Y;
                        break;
                    }
                case ContentAlignment.TopCenter:        // test ok
                    {
                        mDestRect.X = this.ClientRectangle.Width / 2 - mCutStepWidth / 2;
                        mDestRect.Y = this.ClientRectangle.Y;
                        break;
                    }
                case ContentAlignment.TopRight:         // test ok
                    {
                        mDestRect.X = this.ClientRectangle.Width - mCutStepWidth;
                        mDestRect.Y = this.ClientRectangle.Y;
                        break;
                    }
                case ContentAlignment.MiddleLeft:
                    {
                        mDestRect.X = this.ClientRectangle.X;
                        mDestRect.Y = this.ClientRectangle.Height / 2 - mCutStepHeight / 2;
                        break;
                    }

                case ContentAlignment.MiddleCenter:
                    {
                        mDestRect.X = this.ClientRectangle.Width / 2 - mCutStepWidth / 2;
                        mDestRect.Y = this.ClientRectangle.Height / 2 - mCutStepHeight / 2;
                        break;
                    }

                case ContentAlignment.MiddleRight:
                    {
                        mDestRect.X = this.ClientRectangle.Width - mCutStepWidth;
                        mDestRect.Y = this.ClientRectangle.Height / 2 - mCutStepHeight / 2;
                        break;
                    }

                case ContentAlignment.BottomLeft:        // test ok
                    {
                        mDestRect.X = this.ClientRectangle.X;
                        mDestRect.Y = this.ClientRectangle.Height - mCutStepHeight;
                        break;
                    }

                case ContentAlignment.BottomCenter:      // test ok
                    {
                        mDestRect.X = this.ClientRectangle.Width / 2 - mCutStepWidth / 2;
                        mDestRect.Y = this.ClientRectangle.Height - mCutStepHeight;
                        break;
                    }
                case ContentAlignment.BottomRight:       // test ok
                    {
                        mDestRect.X = this.ClientRectangle.Width - mCutStepWidth;
                        mDestRect.Y = this.ClientRectangle.Height - mCutStepHeight;
                        break;
                    }
            }
            mDestRect.Width = mCutStepWidth;
            mDestRect.Height = mCutStepHeight;
            g.DrawImage(this.CheckedMaskImage, mDestRect.X, mDestRect.Y, mDestRect.Width, mDestRect.Height);
        }
        else if ((this.CheckedMarkStyle == CheckStyle.Rectangle))
        {
            // 画长方形标记
            try
            {
                // Dim g1 As Graphics = Graphics.FromHwnd(Me.Handle)
                GraphicsPath markpath = new GraphicsPath();
                Rectangle r = new Rectangle();
                r.X = this.ClientRectangle.X - 0;
                r.Y = this.ClientRectangle.Height / 2 - this.ClientRectangle.Height / 4;
                r.Width = 4;
                r.Height = this.ClientRectangle.Height / 2;
                markpath.AddRectangle(r);
                Color c = Color.FromArgb(0);
                if ((this.mCheckedMarkColorUseThemeColor))
                {
                    GetColorFormSystemColor();
                    c = Color.FromArgb(255, this.mThemeColor);
                }
                else
                    c = Color.FromArgb(255, this.CheckedMarkColor);
                SolidBrush sb = new SolidBrush(c);
                // 防止矩形填充时有平滑过度
                g.SmoothingMode = SmoothingMode.Default;
                g.InterpolationMode = InterpolationMode.Default;
                g.FillPath(sb, markpath);
                g.ResetClip();
                markpath.Dispose();
                //r=null;/* TODO Change to default(_) if this is not a reference type */;
                // g1.Dispose()
                // g1 = Nothing
                // 由于要画图标，所以恢复为默认的高精度平滑
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            }
            catch (Exception ex)
            {
            }
        }
        else if ((this.CheckedMarkStyle == CheckStyle.Round))
        {
            // 画圆形标记
            try
            {
                GraphicsPath markpath = new GraphicsPath();
                Rectangle r = new Rectangle();
                Rectangle mDestRect = new Rectangle();
                int mCutStepWidth = 0;  // 8
                int mCutStepHeight = 0; // 8
                mCutStepWidth = 8;
                mCutStepHeight = 8;
                switch ((CheckedMaskImageAlign))
                {
                    case ContentAlignment.TopLeft         // test ok
                   :
                        {
                            r.X = this.ClientRectangle.X;
                            r.Y = this.ClientRectangle.Y;
                            break;
                        }

                    case ContentAlignment.TopCenter        // test ok
             :
                        {
                            r.X = this.ClientRectangle.Width / 2 - mCutStepWidth / 2;
                            r.Y = this.ClientRectangle.Y;
                            break;
                        }

                    case ContentAlignment.TopRight         // test ok
             :
                        {
                            r.X = this.ClientRectangle.Width - mCutStepWidth - 1;
                            r.Y = this.ClientRectangle.Y;
                            break;
                        }

                    case ContentAlignment.MiddleLeft:
                        {
                            r.X = this.ClientRectangle.X;
                            r.Y = this.ClientRectangle.Height / 2 - mCutStepHeight / 2;
                            break;
                        }

                    case ContentAlignment.MiddleCenter:
                        {
                            r.X = this.ClientRectangle.Width / 2 - mCutStepWidth / 2;
                            r.Y = this.ClientRectangle.Height / 2 - mCutStepHeight / 2;
                            break;
                        }

                    case ContentAlignment.MiddleRight:
                        {
                            r.X = this.ClientRectangle.Width - mCutStepWidth - 1;
                            r.Y = this.ClientRectangle.Height / 2 - mCutStepHeight / 2;
                            break;
                        }

                    case ContentAlignment.BottomLeft        // test ok
             :
                        {
                            r.X = this.ClientRectangle.X;
                            r.Y = this.ClientRectangle.Height - mCutStepHeight;
                            break;
                        }

                    case ContentAlignment.BottomCenter      // test ok
             :
                        {
                            r.X = this.ClientRectangle.Width / 2 - mCutStepWidth / 2;
                            r.Y = this.ClientRectangle.Height - mCutStepHeight;
                            break;
                        }

                    case ContentAlignment.BottomRight       // test ok
             :
                        {
                            r.X = this.ClientRectangle.Width - mCutStepWidth - 1;
                            r.Y = this.ClientRectangle.Height - mCutStepHeight;
                            break;
                        }
                }
                // r.X = Me.ClientRectangle.X - 0
                // r.Y = Me.ClientRectangle.Height / 2 - 8 / 2
                r.Width = 8;
                r.Height = 8;
                markpath.AddEllipse(r);
                Color c = Color.FromArgb(0);
                if ((this.mCheckedMarkColorUseThemeColor))
                {
                    GetColorFormSystemColor();
                    c = Color.FromArgb(255, this.mThemeColor);
                }
                else
                    c = Color.FromArgb(255, this.CheckedMarkColor);
                // Dim c As Color = Color.FromArgb(255, Me.CheckedMarkColor)
                SolidBrush sb = new SolidBrush(c);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(sb, markpath);
                g.ResetClip();
                markpath.Dispose();
                //r = null/* TODO Change to default(_) if this is not a reference type */;
            }
            catch (Exception ex)
            {
            }
        }
        else if ((this.CheckedMarkStyle == CheckStyle.Triangle))
        {
            // 三角形
            try
            {
                // 定义单色画刷，标记颜色
                // Dim b As SolidBrush = New SolidBrush(Me.CheckedMarkColor)
                SolidBrush b;
                if ((this.mCheckedMarkColorUseThemeColor))
                {
                    GetColorFormSystemColor();
                    b = new SolidBrush(this.mThemeColor);
                }
                else
                    b = new SolidBrush(this.CheckedMarkColor);
                Point[] p = new Point[3];
                p[0] = new Point(0, this.ClientRectangle.Height / 2 - 8 / 2);
                p[1] = new Point(0, this.ClientRectangle.Height / 2 + 8 / 2);
                p[2] = new Point(8, this.ClientRectangle.Height / 2);
                GraphicsPath path = new GraphicsPath();
                path.AddLine(p[0], p[1]);
                path.AddLine(p[1], p[2]);
                path.AddLine(p[2], p[0]);
                path.CloseFigure();
                g.FillPath(b, path);
                // g.Dispose()
                path.Dispose();
                b.Dispose();
            }
            catch (Exception ex)
            {
            }
        }
        else if ((this.CheckedMarkStyle == CheckStyle.Square))
        {
            // 画正方形
            try
            {
                // 定义单色画刷，标记颜色
                // Dim b As SolidBrush = New SolidBrush(Me.CheckedMarkColor)
                SolidBrush b;
                if ((this.mCheckedMarkColorUseThemeColor))
                {
                    GetColorFormSystemColor();
                    b = new SolidBrush(this.mThemeColor);
                }
                else
                    b = new SolidBrush(this.CheckedMarkColor);
                Point[] p = new Point[4];
                p[0] = new Point(0, this.ClientRectangle.Height / 2 - 8 / 2);
                p[1] = new Point(8, this.ClientRectangle.Height / 2 - 8 / 2);
                p[2] = new Point(0, this.ClientRectangle.Height / 2 + 8 / 2);
                p[3] = new Point(8, this.ClientRectangle.Height / 2 + 8 / 2);
                GraphicsPath path = new GraphicsPath();
                path.AddLine(p[0], p[2]);
                path.AddLine(p[2], p[3]);
                path.AddLine(p[3], p[1]);
                path.AddLine(p[1], p[0]);
                path.CloseFigure();
                // 防止矩形填充时有平滑过度
                g.SmoothingMode = SmoothingMode.Default;
                g.InterpolationMode = InterpolationMode.Default;
                g.FillPath(b, path);
                // 由于要画图标，所以恢复为默认的高精度平滑
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                // g.Dispose()
                path.Dispose();
                b.Dispose();
            }
            catch (Exception ex)
            {
            }
        }
        else if ((this.CheckedMarkStyle == CheckStyle.Star))
            // 画五角星
            // center为五角星的中心点，radius为中心点到顶点的距离，可以成为它的半径，isSolid 指示画空心五角星还是实心五角星。
            DrawStar(g, new System.Drawing.Point(5, this.ClientRectangle.Height / 2), 6, true); // 画实心五角星
        else
        {
        }
    }

    private Image ImageTintColor(Image img, Color acolor)
    {
        Bitmap result = new Bitmap(img.Size.Width, img.Size.Height);
        try
        {
            Graphics g = Graphics.FromImage(result);
            g.Clear(Color.Transparent);
            g.FillRectangle(Brushes.Transparent, new Rectangle(0, 0, img.Size.Width, img.Size.Height));
            g.DrawImage(img, 0, 0, img.Size.Width, img.Size.Height);
            g.Dispose();
            g = null/* TODO Change to default(_) if this is not a reference type */;
            int iWidth = 0;
            int iHeight = 0;
            Color iFirstColor = Color.FromArgb(0);
            // 获取系统第一点的颜色
            iFirstColor = result.GetPixel(0, 0);
            Color iScanColor = Color.FromArgb(0);
            // 循环读取图片的X，Y坐标指向点和颜色值，如果与第一点的不同，则用指定的
            // 颜色替换
            for (iWidth = 0; iWidth <= img.Size.Width - 1; iWidth++)
            {
                for (iHeight = 0; iHeight <= img.Size.Height - 1; iHeight++)
                {
                    iScanColor = result.GetPixel(iWidth, iHeight);
                    if (!(iScanColor.Equals(iFirstColor)))
                        result.SetPixel(iWidth, iHeight, acolor);
                    else
                        result.SetPixel(iWidth, iHeight, Color.Transparent);
                }
            }
            // 直接用Bitmap返回会有背景色，所以将设置颜色后的图像再画一次
            Image MyImage = (Image)img.Clone();
            Graphics g1 = Graphics.FromImage(MyImage);
            g1.Clear(Color.Transparent);
            g1.FillRectangle(Brushes.Transparent, new Rectangle(0, 0, img.Size.Width, img.Size.Height));
            g1.DrawImage(result, 0, 0, img.Size.Width, img.Size.Height);
            g1.Dispose();
            g1 = null;
            // Dim pr As IntPtr = result.GetHbitmap()
            return MyImage;
        }
        catch (Exception ex)
        {
            return null;
        }
        finally
        {
            if ((result != null))
                result.Dispose();
        }
    }


    /// <summary>
    ///     ''' 获取Window颜色是否应用到标题栏上
    ///     ''' </summary>
    ///     ''' <returns>1是已勾选，0未勾选</returns>
    private int GetColorPrevalence()
    {
        try
        {
            int result = 0;
            if ((Environment.OSVersion.Version.Major >= 6))
            {
                Microsoft.Win32.RegistryKey MyReg = Microsoft.Win32.Registry.CurrentUser;
                MyReg = MyReg.OpenSubKey(@"Software\Microsoft\Windows\DWM", true);
                result = (int)MyReg.GetValue("ColorPrevalence");
                MyReg.Close();
            }
            else
                result = -1;
            return result;
        }
        catch (Exception ex)
        {
            return -1;
        }
    }


    /// <summary>
    ///     ''' 获取Window颜色,返回颜色格式：0xAARRGGBB
    ///     ''' </summary>
    ///     ''' <returns>0xAARRGGBB</returns>
    private int GetColorizationColor()
    {
        try
        {
            Int32 result = 0;
            string tmpvalue = "";
            if ((Environment.OSVersion.Version.Major >= 6))
            {
                Microsoft.Win32.RegistryKey MyReg = Microsoft.Win32.Registry.CurrentUser;
                MyReg = MyReg.OpenSubKey(@"Software\Microsoft\Windows\DWM", true);
                result = (Int32)MyReg.GetValue("ColorizationColor");
                MyReg.Close();
            }
            else
            {
                result = 0;
            }                
            return result;
        }
        catch (Exception ex)
        {
            return 0;
        }
    }


    private void GetColorFormSystemColor()
    {
        try
        {
            Color sysColor = Color.White;
            // WIN7或以上系统，获取系统的Window颜色或是否应用到标题栏
            if ((Environment.OSVersion.Version.Major >= 6))
            {
                // 获取系统主题色
                int intCaptionColor = -1;
                intCaptionColor = GetColorizationColor();
                if ((intCaptionColor != 0))
                {
                    Color sysCaptionColor = Color.FromArgb(intCaptionColor);
                    sysColor = Color.FromArgb(sysCaptionColor.R, sysCaptionColor.G, sysCaptionColor.B);
                }
                else
                    sysColor = Color.Black;
            }
            // 窗体颜色保存
            mThemeColor = sysColor;
        }
        catch (Exception ex)
        {
        }
    }

    private void DrawStar(Graphics g, Point center, int radius, bool isSolid)
    {
        Point[] pts = new Point[10];
        pts[0] = new Point(center.X, center.Y - radius);
        pts[1] = RotateTheta(pts[0], center, 36.0);
        double len = radius * Math.Sin((18.0 * Math.PI / 180.0)) / Math.Sin((126.0 * Math.PI / 180.0));
        pts[1].X = System.Convert.ToInt32(center.X + len * (pts[1].X - center.X) / radius);
        pts[1].Y = System.Convert.ToInt32(center.Y + len * (pts[1].Y - center.Y) / radius);
        int i;
        for (i = 1; i <= 4; i++)
        {
            pts[(2 * i)] = RotateTheta(pts[(2 * (i - 1))], center, 72.0);
            pts[(2 * i + 1)] = RotateTheta(pts[(2 * i - 1)], center, 72.0);
        }
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
        if (isSolid == false)
        {
            // Dim mPen As New Pen（New SolidBrush（Me.CheckedMarkColor））
            Pen mPen;
            if ((this.mCheckedMarkColorUseThemeColor))
            {
                GetColorFormSystemColor();
                mPen = new Pen(new SolidBrush(this.mThemeColor));
            }
            else
                mPen = new Pen(new SolidBrush(this.CheckedMarkColor));
            g.DrawPolygon(mPen, pts);   // 画一个空心五角星
        }
        else
        {
            // Dim mBrush As New SolidBrush（Me.CheckedMarkColor）

            SolidBrush mBrush;
            if ((this.mCheckedMarkColorUseThemeColor))
            {
                GetColorFormSystemColor();
                mBrush = new SolidBrush(this.mThemeColor);
            }
            else
                mBrush = new SolidBrush(this.CheckedMarkColor);
            g.FillPolygon(mBrush, pts); // 画一个实心的五角星
        }
    }


    /// <summary>
    ///     ''' 按给定的圆心坐标，旋转指定的点
    ///     ''' </summary>
    ///     ''' <param name="pt"></param>
    ///     ''' <param name="center"></param>
    ///     ''' <param name="theta"></param>
    ///     ''' <returns>Point</returns>
    public Point RotateTheta(Point pt, Point center, double theta)
    {
        int x = System.Convert.ToInt32(center.X + (pt.X - center.X) * Math.Cos((theta * Math.PI / 180)) - (pt.Y - center.Y) * Math.Sin((theta * Math.PI / 180)));
        int y = System.Convert.ToInt32(center.Y + (pt.X - center.X) * Math.Sin((theta * Math.PI / 180)) + (pt.Y - center.Y) * Math.Cos((theta * Math.PI / 180)));
        return new Point(x, y);
    }

    protected void CaptionButton_Paint(object sender, System.Windows.Forms.PaintEventArgs pevent)
    {
        Graphics g = pevent.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        
        //画默认背景
        DrawBackground(g);

        //画高光
        if (mDrawHightLigth) {
            DrawHighlight(g);
        }
        
        // 画鼠标移入时的渐变图形
        if (this.DrawGlowFlag)
        {
            DrawGlow(g);
        }
            
        // 画按钮的外框线条
        if (this.mButtonState == State.Hover & this.DrawMouseHoverBoard)
        {
            DrawOuterStroke(g, this.BoardWidth);
        }            
        else if (this.DrawBoard)
        {
            DrawOuterStroke(g, this.BoardWidth);
        }
            
        // 写文本
        if (this.Text.Length > 0)
        {
            DrawText(g);
        }
            
        // 画四态图标
        if(this.StateImage != null)
        {
            DrawStateImage(g);
        }
        else
        {
            //画图标
            if (this.Image != null)
            {
                DrawIcon(g);
            }               

        }
        // 画选中标记 DrawCheckedMark
        if (this.Checked && this.DrawCheckedMark)
        {
            DrawCheckMark(g);
        }            
    }


    private void CaptionButton_Resize(object sender, EventArgs e)
    {
        Rectangle r = this.ClientRectangle;
        r.X -= 1;
        r.Y -= 1;
        r.Width += 2;
        r.Height += 2;

        GraphicsPath rr = RoundRect(r, CornerRadius, CornerRadius, CornerRadius, CornerRadius);
        this.Region = new Region(rr);
    }


    private void CaptionButton_MouseEnter(object sender, EventArgs e)
    {
        mButtonState = State.Hover;
        mFadeOut.Stop();
        mFadeIn.Start();
        try
        {
            // 开始统计鼠标静止的时间（ms）
            if ((this.MouseHoverEx))
                mDelay.Start();
        }
        catch (Exception ex)
        {
        }
    }

    private void CaptionButton_MouseMove(object sender, MouseEventArgs e)
    {
        try
        {
            // 开始统计鼠标静止的时间（ms）
            if ((mDelay.Enabled && this.MouseHoverEx))
            {
            }
        }
        catch (Exception ex)
        {
        }
    }

    private void CaptionButton_MouseLeave(object sender, EventArgs e)
    {
        mButtonState = State.None;
        if ((this.mButtonStyle == Style.Flat))
            mGlowAlpha = 0;
        mFadeIn.Stop();
        mFadeOut.Start();
        try
        {
            // 停止统计鼠标静止的时间（ms）
            if ((mDelay.Enabled && this.MouseHoverEx))
                mDelay.Stop();
        }
        catch (Exception ex)
        {
        }
    }

    private void CaptionButton_MouseDown(object sender, MouseEventArgs e)
    {
        if ((e.Button == MouseButtons.Left))
        {
            mButtonState = State.Pressed;
            if ((this.mButtonStyle != Style.Flat))
                mGlowAlpha = mGlowAlphaValue;
            mFadeIn.Stop();
            mFadeOut.Stop();
            this.Invalidate();
        }
    }

    private void CaptionButton_MouseUp(object sender, MouseEventArgs e)
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
        if ((mGlowAlpha + 30 >= mGlowAlphaValue))
        {
            mGlowAlpha = mGlowAlphaValue;
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

    private void mDelay_Tick(object sender, EventArgs e)
    {
        if ((mDelayMs < this.MouseHoverExSecond))
            mDelayMs = mDelayMs + 1;
        else if ((mDelayMs >= this.MouseHoverExSecond))
        {
            mDelay.Stop();
            // 触发事件
            try
            {
                mDelayMs = 1;
                EventArgs ev = new EventArgs();
                OnMouseHoverExtend(this, ev);
            }
            catch (Exception ex)
            {
            }
        }
    }

    private void CaptionButton_KeyDown(object sender, KeyEventArgs e)
    {
        if ((e.KeyCode == Keys.Space))
        {
            MouseEventArgs m = new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0);
            CaptionButton_MouseDown(sender, m);
        }
    }

    private void CaptionButton_KeyUp(object sender, KeyEventArgs e)
    {
        if ((e.KeyCode == Keys.Space))
        {
            MouseEventArgs m = new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0);
            calledbykey = true;
            CaptionButton_MouseUp(sender, m);
        }
    }
}
