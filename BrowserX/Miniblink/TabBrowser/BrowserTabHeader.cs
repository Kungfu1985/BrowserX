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
    public partial class BrowserTabHeader : UserControl
    {
        List<TabHeader> lstTabHeader = new List<TabHeader>();
        int defaultTabWidth = 212;
        int tabWidth = 0;
        string newTabTitle = "新建标签页";


        // 滑动
        TabHeader thMouseDown = null;
        Point pMouseDown = new Point();
        bool slided = false;

        //public Color BottomLineColor = Color.FromArgb(188, 188, 188);

        public BrowserTabHeader()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;

            tabWidth = defaultTabWidth;

            AddNewTab(newTabTitle, this.Font);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            thMouseDown = null;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                pMouseDown = e.Location;

                foreach (TabHeader th in lstTabHeader)
                {
                    if (th.HitTest(pMouseDown))
                    {
                        thMouseDown = th;
                        thMouseDown.Selected = true;
                        break;
                    }
                }

                if (thMouseDown != null)
                {
                    foreach (TabHeader th in lstTabHeader)
                    {
                        if (th != thMouseDown)
                        {
                            th.Selected = false;
                        }
                    }
                    this.Invalidate();
                }
            }

        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left && thMouseDown != null)
            {
                if (!slided)
                {
                    if (Math.Abs(e.X - pMouseDown.X) > 15)
                    {
                        slided = true;
                    }
                }
                else
                {
                    btnAddNew.Visible = false;

                    Point newPos = thMouseDown.Rect.Location;
                    newPos.X += e.Location.X - pMouseDown.X;

                    // 是否在父窗体范围内移动
                    if (newPos.X < 0)
                        newPos.X = 0;
                    if (newPos.X > this.Width - thMouseDown.Rect.Width)
                        newPos.X = this.Width - thMouseDown.Rect.Width;

                   

                    // 判断移动方向，向左或向右
                    if (e.Location.X - pMouseDown.X > 0)
                    {
                        // 判断是否已经是最后一个Tab
                        if (thMouseDown.TabIndex != lstTabHeader.Count - 1)
                        {
                            TabHeader thRight = lstTabHeader[thMouseDown.TabIndex + 1];

                            // 向右移动时，判断是否与后一Tab 交换位置：当前Tab的 Right ,超过后一Tab 位置的一半
                            if (newPos.X + tabWidth > thRight.Rect.X + tabWidth / 2)
                            {
                                thRight.TabIndex --;
                                thMouseDown.TabIndex ++;
                                lstTabHeader.Sort();
                            }
                        }
                    }
                    else
                    {
                        // 判断是否已经是第0个Tab
                        if (thMouseDown.TabIndex != 0)
                        {
                            TabHeader thLeft = lstTabHeader[thMouseDown.TabIndex - 1];

                            // 向右移动时，判断是否与后一Tab 交换位置：当前Tab的 Right ,超过后一Tab 位置的一半
                            if (newPos.X < thLeft.Rect.X + tabWidth / 2)
                            {
                                thLeft.TabIndex ++;
                                thMouseDown.TabIndex --;
                                lstTabHeader.Sort();
                            }
                        }
                    }

                    thMouseDown.Rect.X = newPos.X;
                    pMouseDown = e.Location;
                    this.Invalidate();
                }
            }
            else
            {
                this.Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            thMouseDown = null;
            pMouseDown = new Point();

            if (slided)
            {
                slided = false;

                for (int i = 0; i < lstTabHeader.Count; i++)
                {
                    lstTabHeader[i].TabIndex = i;
                }

                btnAddNew.Visible = true;
                this.Invalidate();
            }
            else
            {
                // 判断是否是Close 区域被点击
                TabHeader thDelete = null;
                foreach (TabHeader th in lstTabHeader)
                {
                    if (th.CloseHitTest(e.Location))
                    {
                        thDelete = th;
                        break;
                    }
                }

                if (thDelete != null)
                {
                    // 移除关闭的Tab, 并重新调整Tab 的Index ,以及Tab 的宽度
                    lstTabHeader.Remove(thDelete);
                    thDelete.Dispose();

                    widthCalculate(false);

                    for (int i = 0; i < lstTabHeader.Count; i++)
                    {
                        lstTabHeader[i].TabIndex = i;
                    }

                    lstTabHeader.Sort();
                    this.Invalidate();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

            e.Graphics.DrawLine(new Pen(TabHeader.BottomLineColor), new Point(0, this.Bottom - 1), new Point(this.Width, this.Bottom - 1));

            // 判断重绘区域大小，解决由最小化还原后，无法绘制Tab的问题
            if (currPaintTh == null || e.ClipRectangle.Size.Width > TabHeader.Left_Offset)
            {
                // 被选中的Tab 需要处于顶层，因此最后绘制
                TabHeader thSelected = null;
                foreach (TabHeader th in lstTabHeader)
                {
                    if (th.Selected)
                        thSelected = th;
                    else
                        th.DrawAll(e.Graphics, th.Rect);
                }
                // 最后绘制
                if (thSelected != null)
                    thSelected.DrawAll(e.Graphics, thSelected.Rect);
            }
            else
            {
                // 绘制完整的TabHeader，如果仅绘制指定区域，可能会出现白色背景
                currPaintTh.DrawAll(e.Graphics, currPaintTh.Rect);
                currPaintTh = null;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            
            widthCalculate(false);

            foreach (TabHeader th in lstTabHeader)
            {
                th.Width = tabWidth;
            }

            base.OnResize(e);
        }

        public void AddNewTab(string title, Font font, string url = "")
        {
            widthCalculate();

            TabHeader newTh = new TabHeader(lstTabHeader.Count, title, font, tabWidth, this, url);
            newTh.Selected = true;

            foreach (TabHeader th in lstTabHeader)
            {
                th.Selected = false;
            }
            
            lstTabHeader.Add(newTh);
            newTh.OnPaintRequest += newTh_OnPaintRequest;

            this.Invalidate();
        }

        void widthCalculate(bool forAdd = true)
        {
            if (forAdd)
            {
                if (this.Width < (lstTabHeader.Count + 1) * (tabWidth - TabHeader.Left_Offset) + 100 || tabWidth < defaultTabWidth)
                {
                    tabWidth = (this.Width - 100) / (lstTabHeader.Count + 1) + TabHeader.Left_Offset;
                }                
            }
            else
            {
                if (this.Width < (lstTabHeader.Count) * (tabWidth - TabHeader.Left_Offset) + 100 || tabWidth < defaultTabWidth)
                {
                    if (lstTabHeader.Count > 0)
                        tabWidth = (this.Width - 100) / (lstTabHeader.Count) + TabHeader.Left_Offset;
                }
            }

            if (tabWidth > defaultTabWidth)
                tabWidth = defaultTabWidth;

            foreach (TabHeader th in lstTabHeader)
            {
                th.Width = tabWidth;
            }

            // 设置btnAddNew 的位置
            if (forAdd)
            {
                btnAddNew.Left = (tabWidth - TabHeader.Left_Offset) * (lstTabHeader.Count + 1) + 18;
            }
            else
                btnAddNew.Left = (tabWidth - TabHeader.Left_Offset) * (lstTabHeader.Count) + 18;

        }

        TabHeader currPaintTh;
        //Rectangle currRect;
        void newTh_OnPaintRequest(TabHeader th, Rectangle rect)
        {
            currPaintTh = th;
            //currRect = rect;
            //this.Invalidate(rect);
            this.Invalidate();            
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            AddNewTab(newTabTitle, this.Font, "bing.com");
        }
    }
}
