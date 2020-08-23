using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;


namespace BrowserX
{
    public class Dropshadow : Form
    {
        private const int WM_MOUSEACTIVATE = 0x21;
        private IntPtr MA_NOACTIVATE = new IntPtr(3);


        private Bitmap _shadowBitmap;
        private Color _shadowColor;
        private int _shadowH;
        private byte _shadowOpacity = 255;
        private int _shadowV;

        public Dropshadow(Form f)
        {
            Owner = f;
            ShadowColor = Color.Red;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            Owner.LocationChanged += UpdateLocation;
            Owner.FormClosing += MyFormCloseing;
            Owner.VisibleChanged += MyVisibleChanged;
            Owner.SizeChanged += MySizeChanged;
            Owner.Activated += MyActivated;
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_MOUSEACTIVATE)
                m.Result = MA_NOACTIVATE;
        }

        public Color ShadowColor
        {
            get
            {
                return _shadowColor;
            }
            set
            {
                _shadowColor = value;
                _shadowOpacity = _shadowColor.A;
            }
        }

        public Bitmap ShadowBitmap
        {
            get
            {
                return _shadowBitmap;
            }
            set
            {
                _shadowBitmap = value;
                SetBitmap(_shadowBitmap, ShadowOpacity);
            }
        }

        public byte ShadowOpacity
        {
            get
            {
                return _shadowOpacity;
            }
            set
            {
                _shadowOpacity = value;
                SetBitmap(ShadowBitmap, _shadowOpacity);
            }
        }

        public int ShadowH
        {
            get
            {
                return _shadowH;
            }
            set
            {
                _shadowH = value;
                RefreshShadow(false);
            }
        }

        public int OffsetX
        {
            get
            {
                return ShadowH - (ShadowBlur + ShadowSpread) + 3;
            }
        }

        public int OffsetY
        {
            get
            {
                return ShadowV - (ShadowBlur + ShadowSpread) + 3;
            }
        }

        public new int Width
        {
            get
            {
                return Owner.Width + ShadowBlur * 3 + ShadowSpread; // (ShadowSpread + ShadowBlur) * 3
            }
        }

        public new int Height
        {
            get
            {
                return Owner.Height + ShadowBlur * 3 + ShadowSpread; // (ShadowSpread + ShadowBlur) * 2
            }
        }

        public int ShadowV
        {
            get
            {
                return _shadowV;
            }
            set
            {
                _shadowV = value;
                RefreshShadow(false);
            }
        }

        public int ShadowBlur { get; set; }
        public int ShadowSpread { get; set; }
        public int ShadowWidth { get; set; }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | 0x80000;
                return cp;
            }
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        // 绘制圆角矩形函数
        private System.Drawing.Drawing2D.GraphicsPath CreateRoundRectPath(Rectangle rect, int radius)
        {
            rect.Offset(-1, -1);
            Rectangle RoundRect = new Rectangle(rect.Location, new Size(radius - 1, radius - 1));

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(RoundRect, 180, 90);     // 左上角
            RoundRect.X = rect.Right - radius;   // 右上角
            path.AddArc(RoundRect, 270, 90);
            RoundRect.Y = rect.Bottom - radius;  // 右下角
            path.AddArc(RoundRect, 0, 90);
            RoundRect.X = rect.Left;             // 左下角
            path.AddArc(RoundRect, 90, 90);
            path.CloseFigure();
            return path;
        }

        public Bitmap DrawShadow(int shadowWidth)
        {
            Bitmap bitmap = null/* TODO Change to default(_) if this is not a reference type */;
            Graphics g = null/* TODO Change to default(_) if this is not a reference type */;
            int iWidth, iHeight;
            iWidth = Owner.Width + shadowWidth * 3 + OffsetX;
            iHeight = Owner.Height + shadowWidth * 3 + OffsetY;

            try
            {
                bitmap = new Bitmap(iWidth, iHeight);
                g = Graphics.FromImage(bitmap);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Color c = Color.FromArgb(0, 0, 0, 0);
                Pen p = new Pen(c, 3);

                for (int i = 0; i <= shadowWidth - 1; i++)
                {
                    p.Color = Color.FromArgb((255 / 24 / shadowWidth) * i, c);
                    Rectangle rect = new Rectangle(i, i, iWidth - (2 * i) - 1, iHeight - (2 * i) - 1);
                    GraphicsPath path = CreateRoundRectPath(rect, shadowWidth);
                    // Dim path As GraphicsPath = New GraphicsPath()
                    // path.AddRectangle(rect)
                    g.DrawPath(p, path);
                    path.Dispose();
                }
                g.Dispose();
                g = null/* TODO Change to default(_) if this is not a reference type */;
                GC.Collect();
                return (bitmap);
            }
            catch (Exception ex)
            {
                return (bitmap);
            }
        }

        public void UpdateLocation(object sender = null, EventArgs eventArgs = null)
        {
            Point pos = Owner.Location;
            pos.Offset(OffsetX, OffsetY);
            Application.DoEvents();
            Location = pos;
        }

        public void MyVisibleChanged(object sender, EventArgs e)
        {
            if (Owner != null)
                Visible = Owner.Visible;
        }

        public void MySizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (Owner != null)
                {
                    if ((Owner.WindowState == FormWindowState.Maximized))
                        this.Visible = false;
                    else if ((Owner.WindowState == FormWindowState.Normal))
                    {
                        this.Visible = true;
                        RefreshShadow(true);
                    }
                    else
                        this.Visible = false;
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void MyFormCloseing(object ByValsender, FormClosingEventArgs e)
        {
            Close();
        }

        private void MyActivated(object sender, EventArgs e)
        {
            try
            {
                Owner.BringToFront();
            }
            catch (Exception ex)
            {
            }
        }

        public void RefreshShadow(bool redraw = true)
        {
            try
            {
                if (redraw)
                    ShadowBitmap = DrawShadow(ShadowBlur);
                // 更新位置，在这里判断是否需要延时显示，开启定时器
                UpdateLocation();
                // 重新定义阴影窗口的尺寸
                var r = new Region(new Rectangle(0, 0, Width + ShadowWidth, Height + ShadowWidth));
                Region = r;
                Owner.Refresh();
            }
            catch (Exception ex)
            {
            }
        }

        public void SetBitmap(Bitmap bitmap, byte opacity = 255)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");
            IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
            IntPtr memDc = Win32.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;

            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                oldBitmap = Win32.SelectObject(memDc, hBitmap);
                var size = new Win32.Size(bitmap.Width, bitmap.Height);
                var pointSource = new Win32.Point(0, 0);
                var topPos = new Win32.Point(Left, Top);
                var blend = new Win32.BLENDFUNCTION();
                blend.BlendOp = Win32.AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = opacity;
                blend.AlphaFormat = Win32.AC_SRC_ALPHA;
                Win32.UpdateLayeredWindow(Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, Win32.ULW_ALPHA);
            }
            finally
            {
                Win32.ReleaseDC(IntPtr.Zero, screenDc);

                if (hBitmap != IntPtr.Zero)
                {
                    Win32.SelectObject(memDc, oldBitmap);
                    Win32.DeleteObject(hBitmap);
                }

                Win32.DeleteDC(memDc);
            }
        }
    }

    public class Win32
    {
        public enum Bool
        {
            False = 0,
            True
        }

        public const Int32 ULW_COLORKEY = 0x1;
        public const Int32 ULW_ALPHA = 0x2;
        public const Int32 ULW_OPAQUE = 0x4;
        public const byte AC_SRC_OVER = 0x0;
        public const byte AC_SRC_ALPHA = 0x1;


        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, int lParam);

        //此处主要用来让窗口置于最前(SetWindowPos(this.Handle,-1,0,0,0,0,0x4000|0x0001|0x0002);)    
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        #region "API函数声明"
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void OutputDebugStringA(string lpOutputString);

        [DllImport("user32")]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [System.Runtime.InteropServices.DllImport("user32.dll ")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);//返回hWnd参数所指定的窗口的设备环境。

        [System.Runtime.InteropServices.DllImport("user32.dll ")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC); //函数释放设备上下文环境（DC）

        [DllImport("gdi32")]
        public static extern int DeleteDC(IntPtr hDC);

        [DllImport("gdi32")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32")]
        public static extern int DeleteObject(int hObject);

        [DllImport("gdi32")]
        public static extern int DeleteObject(IntPtr hObject);

        [DllImport("gdi32")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("user32.dll")]
        public static extern bool UpdateLayeredWindow(IntPtr hwnd,
                                                      IntPtr hdcDst,
                                                      ref Point pptDst,
                                                      ref Size psize,
                                                      IntPtr hdcSrc,
                                                      ref Point pprSrc,
                                                      Int32 crKey,
                                                      ref BLENDFUNCTION pblend, Int32 dwFlags);
        #endregion

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ARGB
        {
            public readonly byte Blue;
            public readonly byte Green;
            public readonly byte Red;
            public readonly byte Alpha;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public Int32 x;
            public Int32 y;

            public Point(Int32 x, Int32 y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Size
        {
            public Int32 cx;
            public Int32 cy;

            public Size(Int32 cx, Int32 cy)
            {
                this.cx = cx;
                this.cy = cy;
            }
        }
    }
}