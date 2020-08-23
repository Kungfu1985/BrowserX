using BroswerX.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BroswerX.UC
{
    public class TabHeader : IDisposable, IComparable
    {
        public event Action<TabHeader, Rectangle> OnPaintRequest;

        #region 变量

        public string Title = String.Empty;
        public Rectangle Rect;
        
        public Font Font;        
        public string Url;
        public Control Parent;
        public Image HeaderIcon = Properties.Resources.icon_normal;         // 当PageState 为 Loading 时，不会显示该Image
        private WebPageState webPageState;
        private int tabIndex;
        public PaintType paintType;
        public bool Selected = true;

        //public Image HeaderIcon
        //{
        //    get { return headerIcon; }
        //    set {
        //        headerIcon = value;
        //        paintType = PaintType.PaintHeaerIcon;
        //        paintRequest();
        //    }
        //}

        // 先设置状态，再修改绘制的TabIcon
        public WebPageState PageState
        {
            get { return webPageState; }
            set
            {
                webPageState = value;
                if (webPageState == WebPageState.Loading)
                {
                    tmAnimation.Start();
                }
                else
                {
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

        //public bool Selected
        //{
        //    get { return selected; }
        //    set
        //    {
        //        selected = value;
        //        paintType = PaintType.All;
        //        paintRequest();
        //    }
        //}

        // GIF
        int iCurFrame = 0;
        int iFrameCount = 60;
        System.Windows.Forms.Timer tmAnimation = new System.Windows.Forms.Timer();

        Region region = null;
        public Rectangle rectClose;
        public Rectangle rectIcon;
        public Rectangle rectFont;
        public Rectangle rectFontLinearBrush;
        Color noSelectedColor = Color.Lavender;

        public static readonly int Left_Offset = 16;
        public static readonly Color BottomLineColor = Color.FromArgb(188,188,188);


        SolidBrush brushFont = new SolidBrush(Color.DimGray);
        #endregion

        /// <summary>
        /// 创建Tab
        /// </summary>
        public TabHeader(int index, string title, Font font, int tabWidth, Control parent, string url = "")
        {
            tmAnimation.Interval = 20;
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

        void tmAnimation_Tick(object sender, EventArgs e)
        {
            iCurFrame = (iCurFrame) % iFrameCount + 1;
            this.paintType = PaintType.PaintHeaerIcon;
            paintRequest();
        }

        public void DrawAll(Graphics g, Rectangle rect)
        {
            try
            {               
                rectFont = new Rectangle(rect.X + 35, rect.Y + 4, rect.Width - 60, rect.Height - 10);
                rectFontLinearBrush = new Rectangle(rectFont.X + rectFont.Width - 35, rect.Y + 6, 35, rect.Height - 10);
                rectIcon = new Rectangle(rect.X + 16, 4, 16, 16);
                rectClose = new Rectangle(rect.X + rect.Width - 25, 6, 12, 12);


                drawRect(g, rect);
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

            g.FillPath(new SolidBrush(Selected ? Color.White : noSelectedColor), path);
            g.DrawLine(new Pen(Selected ? Color.White : BottomLineColor, 1), rect.X + 2, rect.Bottom - 1, rect.Right - 2, rect.Bottom - 1);
        }

        public void drawString(Graphics g, Rectangle rect, Rectangle rectFontLinearBrush, string title, Font font)
        {
            g.DrawString(title, font, brushFont, rect);

            using (LinearGradientBrush brush = new LinearGradientBrush(rectFontLinearBrush, Color.Transparent, Selected ? Color.White : noSelectedColor, 0, false))
            {
                g.FillRectangle(brush, rectFontLinearBrush);
            }
        }

        public void drawTabIcon(Graphics g, Rectangle rect)
        {
            if (webPageState == WebPageState.Loading)
            {
                if(iCurFrame == 0)
                    g.DrawImage((Image)Resources.ResourceManager.GetObject("Marty_000" + (0).ToString("00")), rect);
                else
                    g.DrawImage((Image)Resources.ResourceManager.GetObject("Marty_000" + (iCurFrame - 1).ToString("00")), rect);
            }
            else
                g.DrawImage(HeaderIcon, rect);
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
            return region.IsVisible(cltPosition);
        }

        public bool CloseHitTest(Point cltPosition)
        {
            return rectClose.Contains(cltPosition);
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
