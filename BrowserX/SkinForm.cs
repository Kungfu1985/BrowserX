using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrowserX
{
    public partial class SkinForm : Form
    {
        private FormMain Main;
        private bool IsMouseDown = false;
        

        public SkinForm()
        {
            InitializeComponent();
            //设置为无边框
            this.FormBorderStyle = FormBorderStyle.None;            
        }

        public SkinForm(FormMain main)
        {
            InitializeComponent();
            SetStyles();//减少闪烁
            Main = main;//获取控件层对象
            FormBorderStyle = FormBorderStyle.None;//设置无边框的窗口样式
            ShowInTaskbar = true ;//使控件层显示到任务栏
            BackgroundImage = Main.BackgroundImage;//将控件层背景图应用到皮肤层
            BackgroundImageLayout = ImageLayout.Stretch;//自动拉伸背景图以适应窗口

            Size = Main.Size;//统一大小

            //阴影窗口拥有控件层
            Owner = main;
            //Main.Owner = this;//设置控件层的拥有皮肤层
            //main.Visible = false;
            //FormMovableEvent();//激活皮肤层窗体移动
            //
            //DrawShadow();//绘制半透明不规则皮肤            
            //Win32API.OutputDebugStringA("Skin窗口加载结束！");
        }

        private void SetStyles()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.DoubleBuffered = true;
        }

        #region "调整无边框窗口大小使用的变量"
        const int HTLEFT = 10;
        const int HTRIGHT = 11;
        const int HTTOP = 12;
        const int HTTOPLEFT = 13;
        const int HTTOPRIGHT = 14;
        const int HTBOTTOM = 15;
        const int HTBOTTOMLEFT = 0x10;
        const int HTBOTTOMRIGHT = 17;
        #endregion

        #region "重写消息处理过程"
        protected override void WndProc(ref Message m)
        {

            switch (m.Msg)
            {
                case 0x21:
                    base.WndProc(ref m);
                    //m.LParam = IntPtr.Zero;//默认值
                    //m.WParam = new IntPtr(1);
                    //m.Result = new IntPtr(3);//MA_NOACTIVATE不激活窗口，不删除鼠标消息
                    break;
                case 0x0084:
                    //Win32API.OutputDebugStringA(string.Format("SkinForm 捕捉到了 {0} 消息！", m.Msg));
                    
                    base.WndProc(ref m);
                    Point vPoint = new Point((int)m.LParam & 0xFFFF, (int)m.LParam >> 16 & 0xFFFF);
                    vPoint = PointToClient(vPoint);
                    //Win32API.OutputDebugStringA( string.Format("当前鼠标坐标值为：X:{0},Y:{1}", vPoint.X,vPoint.Y ) ); 
                    if (vPoint.X <= 5)
                    {
                        if (vPoint.Y <= 8)
                        {
                            //Win32API.OutputDebugStringA("光标在左上角");
                            m.Result = (IntPtr)HTTOPLEFT;
                        }
                        else if (vPoint.Y >= ClientSize.Height - 8)
                        {
                            //Win32API.OutputDebugStringA("光标在左下角");
                            m.Result = (IntPtr)HTBOTTOMLEFT;
                        }
                        else
                        {
                            //Win32API.OutputDebugStringA("光标在左边");
                            m.Result = (IntPtr)HTLEFT;
                        }
                    }
                    else if (vPoint.X >= ClientSize.Width - 5)
                    {
                        if (vPoint.Y <= 8)
                        {
                            //Win32API.OutputDebugStringA("光标在右上角");
                            m.Result = (IntPtr)HTTOPRIGHT;
                        }
                        else if (vPoint.Y >= ClientSize.Height - 8)
                        {
                            //Win32API.OutputDebugStringA("光标在右下角");
                            m.Result = (IntPtr)HTBOTTOMRIGHT;
                        }
                        else
                        {
                            //Win32API.OutputDebugStringA("光标在右边");
                            m.Result = (IntPtr)HTRIGHT;
                        }

                    }
                    else if (vPoint.Y <= 5)
                    {
                        m.Result = (IntPtr)HTTOP;
                    }
                    else if (vPoint.Y >= ClientSize.Height - 5)
                    {
                        m.Result = (IntPtr)HTBOTTOM;
                    }
                    break;
                case 0x0201://鼠标左键按下的消息 用于实现拖动窗口功能WM_LBUTTONDOWN
                    //IsMouseDown = true;
                    //m.Msg = 0x00A1;//更改消息为非客户区按下鼠标

                    //m.LParam = IntPtr.Zero;//默认值

                    //m.WParam = new IntPtr(2);//鼠标放在标题栏内

                    base.WndProc(ref m);
                    break;
                case 0x0202://鼠标左键弹起的消息 用于实现拖动窗口功能 WM_LBUTTONUP
                    //IsMouseDown = false;
                    //m.Msg = 0x00A1;//更改消息为非客户区按下鼠标

                    //m.LParam = IntPtr.Zero;//默认值

                    //m.WParam = new IntPtr(2);//鼠标放在标题栏内

                    base.WndProc(ref m);
                    break;
                case 0x231: //鼠标左键按下准备尺寸调整时的状态WM_ENTERSIZEMOVE
                    IsMouseDown = true;
                    base.WndProc(ref m);
                    break;
                case 0x232: //鼠标调整尺寸结束鼠标按键弹起时的状态WM_EXITSIZEMOVE
                    IsMouseDown = false;
                    base.WndProc(ref m);
                    break;
                default:
                    //IsMouseDown = false;
                    base.WndProc(ref m);
                    break;
            }

        }

        #endregion

        #region "自定义属性"
        private int _ShadowWidth = 10;
        [Category("Skin")]
        [Description("窗口边框阴影宽度")]
        [DefaultValue(typeof(int), "10")]
        public int ShadowWidth
        {
            get { return _ShadowWidth; }
            set
            {
                if (_ShadowWidth != value) {
                    _ShadowWidth = value;
                }
            }
        }
        #endregion

        #region "画阴影到Bitmap上"
        public static void DrawRoundRectangle(Graphics g, Pen pen, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.DrawPath(pen, path);
                path.Dispose();
            }
        }
        public static void FillRoundRectangle(Graphics g, Brush brush, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.FillPath(brush, path);
                path.Dispose();
            }
        }
        internal static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }

        /// <summary>
        /// 绘制阴影或不规则窗口
        /// </summary>
        internal void DrawShadow()
        {
            Bitmap bitmap = null;
            Graphics g = null;
            try
            {
                bitmap = new Bitmap(Width, Height);
                g = Graphics.FromImage(bitmap);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                Color c = Color.FromArgb(0, 0, 0, 0);
                Pen p = new Pen(c, 3);

                for (int i = 0; i < ShadowWidth; i++)
                {
                    p.Color = Color.FromArgb(( 255 / 10 / (ShadowWidth*2) ) * i, c);
                    DrawRoundRectangle(g, p, new Rectangle(i, i, Width - (2 * i) - 1, Height - (2 * i) - 1), ShadowWidth - i);
                }
                if (Main.Owner!=null)
                {
                    //g.DrawString(Main.Owner.Text, Font, Brushes.DeepPink, new Point((int)((double)(Width / 2.4)), (int)((double)(Height / 2.4))));
                }
                else
                {
                    //g.DrawString("Main没有拥有它的窗口", Font, Brushes.DeepPink, new Point((int)((double)(Width / 2.4)), (int)((double)(Height / 2.4))));
                }

                if (this.Owner != null)
                {
                    //g.DrawString( string.Format("拥有skin的窗口名称为:{0},标题为：{1}", this.Owner.Name, this.Owner.Text) , Font, Brushes.DeepPink, new Point((int)((double)(Width / 2.2)), (int)((double)(Height / 2.2))));
                }

                //g.DrawString("这是分层窗口透明层！", Font, Brushes.DeepPink, new Point(Width / 2, Height / 2));

                SetBits(bitmap);
            }
            catch(Exception ex)
            {
                //Win32API.OutputDebugStringA(string.Format("画阴影时发生了错误:{0}",ex.Message)); 
            }
            finally
            {
                //Win32API.OutputDebugStringA(string.Format("阴影画完，释放对象！"));
                if (g != null) {
                    g.Dispose();
                    g = null;
                }
                if (bitmap!=null) {
                    bitmap.Dispose();
                    bitmap = null;
                }
                //GC.Collect();
            }
        }

        #endregion


        #region "重写窗体的 CreateParams 属性"
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00080000;  //  WS_EX_LAYERED 扩展样式
                //无边框任务栏窗口最小化
                const int WS_MINIMIZEBOX = 0x00020000;  // Winuser.h中定义
                //CreateParams cp = base.CreateParams;
                cp.Style = cp.Style | WS_MINIMIZEBOX;   // 允许最小化操作
                return cp;
            }
        }
        #endregion

        #region API调用
        public void SetBits(Bitmap bitmap)//调用UpdateLayeredWindow（）方法。this.BackgroundImage为你事先准备的带透明图片。
        {
            //if (!haveHandle) return;

            if (!Bitmap.IsCanonicalPixelFormat(bitmap.PixelFormat) || !Bitmap.IsAlphaPixelFormat(bitmap.PixelFormat))
            {
                throw new ApplicationException("图片必须是32位带Alhpa通道的图片。");
            }


            IntPtr oldBits = IntPtr.Zero;
            IntPtr screenDC = Win32API.GetDC(IntPtr.Zero);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr memDc = Win32API.CreateCompatibleDC(screenDC);

            try
            {
                Point topLoc = new Point(Left, Top);
                Size bitMapSize = new Size(bitmap.Width, bitmap.Height);
                Win32API._BLENDFUNCTION blendFunc = new Win32API._BLENDFUNCTION();
                Point srcLoc = new Point(0, 0);

                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                oldBits = Win32API.SelectObject(memDc, hBitmap);

                blendFunc.BlendOp = Win32API.AC_SRC_OVER;
                blendFunc.SourceConstantAlpha = 255;
                blendFunc.AlphaFormat = Win32API.AC_SRC_ALPHA;
                blendFunc.BlendFlags = 0;

                Win32API.UpdateLayeredWindow(Handle, screenDC, ref topLoc, ref bitMapSize, memDc, ref srcLoc, 0, ref blendFunc, Win32API.ULW_ALPHA);
            }
            finally
            {
                Win32API.ReleaseDC(IntPtr.Zero, screenDC);
                if (hBitmap != IntPtr.Zero)
                {
                    Win32API.SelectObject(memDc, oldBits);
                    Win32API.DeleteObject(hBitmap);
                }
                Win32API.DeleteDC(memDc);
            }
        }
        #endregion

        private void SkinForm_Load(object sender, EventArgs e)
        {
            try
            {
                DrawShadow();//绘制半透明不规则皮肤或阴影
                if (Main != null)
                {
                    //统一控件层和皮肤层的尺寸
                    this.Size =  new Size(Main.Size.Width + ShadowWidth * 2-2, Main.Size.Height + ShadowWidth * 2-2) ;
                    //统一控件层和皮肤层的位置
                    Location = new Point(Main.Location.X - ShadowWidth + 1, Main.Location.Y - ShadowWidth + 1);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void SkinForm_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                DrawShadow();//绘制半透明不规则皮肤或阴影
                //Win32API.OutputDebugStringA(string.Format("IsMouseDown:{0}", IsMouseDown)); 
                if(Main != null && IsMouseDown ) {
                    Main.Size = new Size(this.Size.Width - this.ShadowWidth*2 + 2, this.Size.Height - this.ShadowWidth*2 + 2);
                    Main.Location = new Point(this.Location.X + this.ShadowWidth - 1, this.Location.Y + this.ShadowWidth - 1);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void SkinForm_MouseDown(object sender, MouseEventArgs e)
        {
            IsMouseDown = true;
        }

        private void SkinForm_MouseUp(object sender, MouseEventArgs e)
        {
            IsMouseDown = false;
        }

        private void SkinForm_Shown(object sender, EventArgs e)
        {
            this.MinimumSize = new Size(640 + ShadowWidth-1, 480 + ShadowWidth-1);
            this.MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width + ShadowWidth, Screen.PrimaryScreen.WorkingArea.Height+ ShadowWidth);
            if (Main!=null) {
                Main.Visible = true;
                Main.BringToFront();
            }
        }
    }
}
