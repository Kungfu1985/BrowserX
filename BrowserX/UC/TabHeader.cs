using BrowserX.Properties;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
#region "ipc"
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
#endregion

namespace BrowserX.UC
{
    public class TabHeader : IDisposable, IComparable
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void OutputDebugStringA(string lpOutputString);

        public event Action<TabHeader, Rectangle> OnPaintRequest;

        #region 变量
        //标签的标题 
        public string Title = String.Empty;
        //标签的区域
        public Rectangle Rect;
        //标签的字体
        public Font Font;
        public Color ForeColor;
        //当前标签的WebBrowser的Url
        public string Url;
        //当前标签的父窗口
        public Control Parent;
        //当前标签进程的窗口的句柄
        public IntPtr ClientHandle;
        //当前标签进程的IPC客户端名称，为来发消息给客户端
        public string ClientName;
        // 当PageState 为 Loading 时，不会显示该Image
        public Image HeaderIcon = Properties.Resources.icon_normal;       
        //当前标签的加载状态
        private WebPageState webPageState;
        //当前标签的索引
        private int tabIndex;
        public PaintType paintType;
        //标签是否选择
        public bool Selected = true;
        //标签是否悬念
        public bool Hovered = false;

        //构造函数
        public TabHeader()
        {
            //空的构造函数
        }

        // 先设置状态，再修改绘制的TabIcon
        public WebPageState PageState
        {
            get { return webPageState; }
            set
            {
                webPageState = value;
                if (webPageState == WebPageState.Loading) {
                    tmAnimation.Start();
                } else {
                    tmAnimation.Stop();
                    iCurFrame = 0;
                }
            }
        }

        public int Width
        {
            get { return Rect.Width; }
            set {
                Rect = GenRect(this.tabIndex, value);
            }
        }

        public int TabIndex
        {
            get { return tabIndex; }
            set {
                this.tabIndex = value;
                Rect = GenRect(value, Rect.Width);
            }
        }       


        // GIF
        int iCurFrame = 0;
        int iFrameCount = 60;
        System.Windows.Forms.Timer tmAnimation = new System.Windows.Forms.Timer();

        Region region = null;
        public Rectangle rectClose;
        public Rectangle rectIcon;
        public Rectangle rectFont;
        public Rectangle rectFontLinearBrush;
        public Color noSelectedColor = Color.Lavender;
        public Color SelectedColor = Color.White;
        public Color HoverColor = Color.DarkGray;

        public static readonly int Left_Offset = 16;
        public static readonly Color BottomLineColor = Color.FromArgb(188,188,188);


        SolidBrush brushFont = new SolidBrush(Color.DimGray);
        #endregion

        #region "动画变量"
        public Image mImage = null;
        public Image Image
        {
            get { return mImage; }
            set
            {
                mImage = value;
                GetParameter(mImage); 
            }
        }


        public int mFrameNumber = 1; // 动画帧数（默认为1帧）
        public int mFrameRate = 100; // 动画帧数（默认为100毫秒帧）
        /// <summary>
        ///     ''' 图像的切割的方式
        ///     ''' </summary>
        private enum CutStyle
        {
            /// <summary>
            ///         ''' 以X轴切割
            ///         ''' </summary>
            XAxis,
            /// <summary>
            ///         ''' 不切割
            ///         ''' </summary>
            None,
            /// <summary>
            ///         ''' 以Y轴切割
            ///         ''' </summary>
            YAxis
        }
        #endregion

        #region "动画切割变量"
        //动画切割方向，X，Y轴或不切割
        private CutStyle mCutStyle = CutStyle.None;
        //动画切割高/宽度
        private int mCutStepWidth = 0;
        private int mCutStepHeight = 0;
        //动画图片的总宽度
        private Rectangle mCutRect = new Rectangle(0, 0, 0, 0);
        private int mCutMax = 0;
        private int mCurFrame = 0;
        #endregion

        /// <summary>
        /// 创建Tab
        /// </summary>
        public TabHeader(int index, string title, Font font, int tabWidth, Control parent, string url = "")
        {
            
            tmAnimation.Interval = this.mFrameRate;
            tmAnimation.Tick += tmAnimation_Tick;

            // 构建当前Tab Rect
            Rect = GenRect(index, tabWidth);

            this.tabIndex = index;
            Title = title;
            Font = font;
            Url = url;
            Parent = parent;           
            

            if (string.IsNullOrEmpty(url))
            {
                this.PageState = WebPageState.Normal;
            }
            else
            {
                this.PageState = WebPageState.Loading;
            }
        }        


        /// <summary>
        /// 画标签左侧图标的时钟，加载动画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tmAnimation_Tick(object sender, EventArgs e)
        {
            iCurFrame = (iCurFrame) % iFrameCount + 1;

            if ((mCurFrame + mCutStepWidth >= mCutMax))
                mCurFrame = 0;
            else
                mCurFrame = mCurFrame + mCutStepWidth;
            if ((mCutStyle == CutStyle.XAxis))
            {
                mCutRect.X = mCurFrame;
                mCutRect.Y = 0;
                mCutRect.Width = mCutStepWidth;
                mCutRect.Height = mCutStepHeight;
            }
            else if ((mCutStyle == CutStyle.YAxis))
            {
                mCutRect.X = 0;
                mCutRect.Y = mCurFrame;
                mCutRect.Width = mCutStepWidth;
                mCutRect.Height = mCutStepHeight;
            }

            this.paintType = PaintType.PaintHeaerIcon;
            paintRequest();
        }


        /// <summary>
        /// 计算LoadingImage的参数，以便自绘
        /// </summary>
        private void GetParameter(Image img)
        {
            try
            {
                // 计算FrameNumber,如果宽度大于高度，X轴切割， 高度大于宽度Y轴切割
                // 如果高宽相等，只有一帧，不需要动画
                if ((img.Size.Width > img.Size.Height))
                {
                    mCutStyle = CutStyle.XAxis;
                    mCutStepWidth = img.Size.Width / mFrameNumber;
                    mCutStepHeight = img.Size.Height;
                    mCutMax = img.Size.Width;
                }
                else if ((img.Size.Width == img.Size.Height))
                {
                    mCutStyle = CutStyle.None;
                    mCutMax = 0;
                    mCutStepWidth = img.Size.Width;
                    mCutStepHeight = img.Size.Height;
                }
                else if ((img.Size.Height > img.Size.Width))
                {
                    mCutStyle = CutStyle.YAxis;
                    mCutStepWidth = img.Size.Height / mFrameNumber;
                    mCutStepHeight = img.Size.Width;
                    mCutMax = img.Size.Height;
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void DrawAll(Graphics g, Rectangle rect)
        {
            try
            {
                int yoffset = 7;
                rectFont = new Rectangle(rect.X + 35+2, rect.Y + 2 + yoffset, rect.Width - 60-2, rect.Height - 5);
                rectFontLinearBrush = new Rectangle(rectFont.X + rectFont.Width - 35-2, rect.Y + 6 + yoffset, 35+2, rect.Height - 10);
                rectIcon = new Rectangle(rect.X + 16+2, 4+yoffset , 16, 16);
                rectClose = new Rectangle(rect.X + rect.Width - 25, 6 + yoffset, 12, 12);

                Rectangle rectnew = new Rectangle(rect.X+4, rect.Y + yoffset , rect.Width, rect.Height);

                drawRect(g, rectnew);
                drawString(g, rectFont, rectFontLinearBrush, Title, Font);
                drawTabIcon(g, rectIcon);
                drawClose(g, rectClose, CloseHitTest(Parent.PointToClient( Cursor.Position)));
            }
            catch { }
        }

        public void drawRect(Graphics g, Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();            

            path = new GraphicsPath();
            path.AddBezier(
                new Point(rect.X, rect.Bottom),
                new Point(rect.X + 3, rect.Bottom - 2),
                new Point(rect.X + 3, rect.Bottom - 2),
                new Point(rect.X + 4, rect.Bottom - 4));
            //path.AddLine(rect.X + 4, rect.Bottom - 4, rect.Left + 15 - 4, rect.Y + 4);
            path.AddBezier(
                new Point(rect.Left + 15 - 4, rect.Y + 4),
                new Point(rect.Left + 15 - 3, rect.Y + 2),
                new Point(rect.Left + 15 - 3, rect.Y + 2),
                new Point(rect.Left + 15, rect.Y));
            //path.AddLine(rect.Left + 15, rect.Y, rect.Right - 15, rect.Y);
            path.AddBezier(
                new Point(rect.Right - 15, rect.Y),
                new Point(rect.Right - 15 + 3, rect.Y + 2),
                new Point(rect.Right - 15 + 3, rect.Y + 2),
                new Point(rect.Right - 15 + 4, rect.Y + 4));
            //path.AddLine(rect.Right - 15 + 4, rect.Y + 4, rect.Right - 4, rect.Bottom - 4);
            path.AddBezier(
                new Point(rect.Right - 4, rect.Bottom - 4),
                new Point(rect.Right - 3, rect.Bottom - 3),
                new Point(rect.Right - 3, rect.Bottom - 3),
                new Point(rect.Right, rect.Bottom));

            region = new System.Drawing.Region(path);

            g.DrawPath(new Pen(Color.Black), path);

            g.FillPath(new SolidBrush(Selected ? SelectedColor : (Hovered ? HoverColor : noSelectedColor) ), path);
            g.DrawLine(new Pen(Selected ? Color.White : BottomLineColor, 1), rect.X + 2, rect.Bottom - 1, rect.Right - 2, rect.Bottom - 1);
        }

        public void drawString(Graphics g, Rectangle rect, Rectangle rectFontLinearBrush, string title, Font font)
        {
            //计算标签的文字长度，如果超长，则截断显示
            float fontLength = 0;
            try
            {
                SizeF fFontSize = new SizeF(0, 0);
                fFontSize = g.MeasureString(title, font);
                fontLength = fFontSize.Width;
            }
            catch(Exception ex)
            {
                fontLength = 0;
            }
            //画出文字,超出部分截断
            //OutputDebugStringA(string.Format("当前标签按当前字体占用的宽度为：{0},当前标签宽度为：{1}", fontLength, this.Width  )); 
            if (fontLength > this.Width) {
                //g.DrawString(title.Substring(0,this.Width) , font, brushFont, rect);
            } else {
                //g.DrawString(title, font, brushFont, rect);
                
            }

            TextRenderer.DrawText(g, title, Font, rect, ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            //画渐变色，实现文字超出部分渐隐的效果
            using (LinearGradientBrush brush = new LinearGradientBrush(rectFontLinearBrush, Color.Transparent, Selected ? SelectedColor : ( Hovered ? HoverColor : noSelectedColor), 0, false))
            {
                g.FillRectangle(brush, rectFontLinearBrush);
            }
        }

        public void drawTabIcon(Graphics g, Rectangle rect)
        {
            if (webPageState == WebPageState.Loading)
            {
                
                if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
                {
                    // 画动画
                    Rectangle mDestRect = new Rectangle();
                    if ((mCutStyle != CutStyle.None))
                    {
                        // 根据ImageAling来画图片的位置
                        mDestRect.X = rect.X;
                        mDestRect.Y = rect.Y;
                        mDestRect.Width = mCutStepWidth;
                        mDestRect.Height = mCutStepHeight;
                        g.DrawImage(this.Image, mDestRect, mCutRect, GraphicsUnit.Pixel);
                    }
                    else
                    {
                        mDestRect.X = rect.X;
                        mDestRect.Y = rect.Y;
                        mDestRect.Width = rect.Width;
                        mDestRect.Height = rect.Height;
                        g.DrawImage(this.Image, mDestRect, mCutRect, GraphicsUnit.Pixel);
                    }
                    //if (iCurFrame == 0) {
                    //    g.DrawImage((Image)Resources.ResourceManager.GetObject("Marty_000" + (0).ToString("00")), rect);
                    //} else {
                    //    g.DrawImage((Image)Resources.ResourceManager.GetObject("Marty_000" + (iCurFrame - 1).ToString("00")), rect);
                    //}
                }
            }
            else
            {
                try
                {
                    g.DrawImage(HeaderIcon, rect);
                }
                catch(Exception ex)
                {
                    Win32API.OutputDebugStringA(  string.Format("g.DrawImage(HeaderIcon, rect) error:{0}", ex.Message));
                }
            }
                
        }

        public void drawClose(Graphics g, Rectangle rect, bool mouseOn)
        {
            if (mouseOn)
                g.DrawImage(Properties.Resources.close_on, rect);
            else
                g.DrawImage(Properties.Resources.close_normal, rect);
        }

        public bool HitTest(Point cltPosition)
        {
            if (region != null) {
                return region.IsVisible(cltPosition);
            } else {
                return false;
            }            
        }

        public bool CloseHitTest(Point cltPosition)
        {
            if (rectClose!=null) {
                return rectClose.Contains(cltPosition);
            }
            else {
                return false;
            }            
        }

        public bool TitleHitTest(Point cltPosition)
        {
            if (rectFont != null) {
                return rectFont.Contains(cltPosition);
            } else {
                return false;
            }
        }

        Rectangle GenRect(int index, int tabWidth)
        {
            if (index == 0)
                return new Rectangle(index * tabWidth, 0, tabWidth, 25);
            else
            {
                Rectangle re = new Rectangle(index * tabWidth, 0, tabWidth, 25);
                re.Offset(- index * Left_Offset, 0);
                return re;
            }
        }        

        public void Dispose()
        {
            tmAnimation.Stop();
            tmAnimation.Tick -= tmAnimation_Tick;            
        }

        public int CompareTo(object o)
        {
            TabHeader th = o as TabHeader;
            return this.tabIndex.CompareTo(th.TabIndex);
        }

        void paintRequest()
        {
            if (OnPaintRequest != null)
            {
                switch (paintType)
                {
                    case PaintType.PaintHeaerIcon:
                        OnPaintRequest(this, rectIcon);
                        break;
                }
                
            }
        }

        public enum WebPageState
        {
            Loading, Normal
        }

        public enum PaintType
        {
            PaintTabRect, PaintText, PaintHeaerIcon, PaintClose, All
        }
    }
}
