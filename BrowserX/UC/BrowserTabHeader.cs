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
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using BrowserX.Properties;
using System.ComponentModel.Design;
using System.Windows.Forms.Integration;
using System.Diagnostics;

namespace BrowserX.UC
{
    public partial class BrowserTabHeader : UserControl
    {
        List<TabHeader> lstTabHeader = new List<TabHeader>();
        int defaultTabWidth = 212;
        int tabWidth = 0;
        string newTabTitle = "新建标签页";

        private Point oldLocation = new Point(0, 0);
        private bool bolDbClickedFlag = false;

        private ToolTip toolTip1 = new ToolTip();

        #region "自定义属性"
        private int _ControlBoxWidth = 200;
        [Category("Skin")]
        [Description("窗口右上角控制按钮区域宽度")]
        [Browsable(true)]
        [DefaultValue(typeof(int), "200")]
        public int ControlBoxWidth
        {
            get { return _ControlBoxWidth; }
            set
            {
                if (_ControlBoxWidth != value) {
                    _ControlBoxWidth = value;
                }
            }
        }
        //双击关闭标签
        private bool _DbClickCloseTab = false;
        [Category("Skin")]
        [Description("双击关闭标签")]
        [Browsable(true)]
        [DefaultValue(typeof(bool), "false")]
        public bool DbClickCloseTab
        {
            get { return _DbClickCloseTab; }
            set
            {
                if (_DbClickCloseTab != value)
                {
                    _DbClickCloseTab = value;
                }
            }
        }
        //标签选中时的标签颜色
        private Color _TabNormalColor = Color.White;
        [Category("Skin")]
        [Description("标签未选中时的背景色")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "230,230,240")]
        public Color TabNormalColor
        {
            get { return _TabNormalColor; }
            set
            {
                if (!_TabNormalColor.Equals(value))
                {
                    _TabNormalColor = value;
                }
            }
        }
        //
        private Color _TabSelectedColor = Color.White;
        [Category("Skin")]
        [Description("标签选中时的背景色")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "255,255,255")]
        public Color TabSelectedColor
        {
            get { return _TabSelectedColor; }
            set
            {
                if (!_TabSelectedColor.Equals(value))
                {
                    _TabSelectedColor = value;
                }
            }
        }
        //
        private Color _TabHoverColor = Color.White;
        [Category("Skin")]
        [Description("标签鼠标悬停时的背景色")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "210,220,230")]
        public Color TabHoverColor
        {
            get { return _TabHoverColor; }
            set
            {
                if (!_TabHoverColor.Equals(value))
                {
                    _TabHoverColor = value;
                }
            }
        }
        private Color _TabBottomLineColor = Color.FromArgb(188,188,188) ;
        [Category("Skin")]
        [Description("标签底部线条颜色")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "188,188,188")]
        public Color TabBottomLineColor
        {
            get { return _TabBottomLineColor; }
            set
            {
                if (!_TabBottomLineColor.Equals(value))
                {
                    _TabBottomLineColor = value;
                }
            }
        }
        
        #endregion

        #region " 开放属性"
        // 多帧动画图像文件
        private Image mImage = Resources.loadingmask22;
        [Description("用于显示动画的多帧图像")]
        [DefaultValue(Constants.vbNull)]
        [Browsable(true)]
        [Category("Animator")]
        public Image Image
        {
            get
            {
                return mImage;
            }

            set
            {
                mImage = value;
                //GetParameter(this.Image);

                //this.Invalidate();
            }
        }

        private int mFrameNumber = 1; // 动画帧数（默认为1帧）
        [Description("设置动画图片的总帧数")]
        [DefaultValue(1)]
        [Browsable(true)]
        [Category("Animator")]
        public int FrameNumber
        {
            get
            {
                return mFrameNumber;
            }

            set
            {
                mFrameNumber = value;

                this.Invalidate();
            }
        }


        private int mFrameRate = 100; // 动画帧数（默认为100毫秒帧）
        [Description("动画帧速率，每隔多少毫秒画一帧，默认100ms")]
        [DefaultValue(100)]
        [Browsable(true)]
        [Category("Animator")]
        public int FrameRate
        {
            get
            {
                return mFrameRate;
            }

            set
            {
                mFrameRate = value;
            }
        }

        /// <summary>
        ///     ''' 图像的切割的方式
        ///     ''' </summary>
        public enum CutStyle
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
        // 滑动
        private TabHeader thMouseDown = null;
        private TabHeader thPreviousMouseDown = null;
        public TabHeader thMouseDownPublic = null;
        private TabHeader thMouseMove = null;

        private Panel pnlWebBrowser = null;
        private FormMain tempFormMain = null;
        Point pMouseDown = new Point();
        Point pMouseMove = new Point();
        bool slided = false;
        bool bolMouseDownNothing = false;

        //public Color BottomLineColor = Color.FromArgb(188, 188, 188);

        public BrowserTabHeader()
        {
            InitializeComponent();
            //this.DoubleBuffered = true;            
            this.BackColor = Color.White;
            tabWidth = defaultTabWidth;
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer,true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.UpdateStyles();
            //DoubleBuffered = true;

            //OutputDebugStringA("BrowserTabHeader 构造函数执行...");             
        }

        #region "客户端命令处理"
        /// <summary>
        /// 通过指定的客户端ID来设置标签的客户端窗口的句柄
        /// </summary>
        /// <param name="clientid"></param>
        /// <param name="clienthandle"></param>
        public void SetClientHandle(string clientid, IntPtr clienthandle)
        {
            try
            {
                foreach (TabHeader th in lstTabHeader)
                {
                    if (th.ClientName.Equals(clientid))
                    {
                        th.ClientHandle = clienthandle;
                        //Win32API.OutputDebugStringA(string.Format("标签标题：{0}，客户ID：{1}，句柄：{2}", th.Title, th.ClientName, th.ClientHandle));
                        break;
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 通过指定的客户端ID来设置标签的标题文字
        /// </summary>
        /// <param name="clientid"></param>
        /// <param name="title"></param>
        public void SetClientTitle(string clientid, string title)
        {
            try
            {
                foreach (TabHeader th in lstTabHeader)
                {
                    if (th.ClientName.Equals(clientid))
                    {
                        th.Title  = title;
                        currPaintTh = th;
                        this.Invalidate();
                        //th.paintType = TabHeader.PaintType.All;
                        //Win32API.OutputDebugStringA(string.Format("设置标签标题：{0}，客户ID：{1}", title, th.ClientName));
                        break;
                    }
                }
            }
            catch
            {

            }
        }


        /// <summary>
        /// 通过指定的客户端ID来设置标签的状态
        /// </summary>
        /// <param name="clientid"></param>
        /// <param name="title"></param>
        public void SetClientStatus(string clientid, TabHeader.WebPageState status)
        {
            try
            {
                foreach (TabHeader th in lstTabHeader)
                {
                    if (th.ClientName.Equals(clientid))
                    {
                        th.PageState = status;
                        //th.paintType = TabHeader.PaintType.All;
                        currPaintTh = th;
                        this.Invalidate();
                        //Win32API.OutputDebugStringA(string.Format("设置标签的状态：{0}，客户ID：{1}", status, th.ClientName));
                        break;
                    }
                }
            }
            catch
            {

            }
        }


        /// <summary>
        /// 通过指定的客户端ID来设置标签的图标
        /// </summary>
        /// <param name="clientid"></param>
        /// <param name="title"></param>
        public void SetClientIcon(string clientid, Image headicon)
        {
            try
            {
                foreach (TabHeader th in lstTabHeader)
                {
                    if (th.ClientName.Equals(clientid))
                    {
                        th.HeaderIcon  = headicon;
                        //th.paintType = TabHeader.PaintType.All;
                        currPaintTh = th;
                        this.Invalidate();
                        //Win32API.OutputDebugStringA(string.Format("设置标签的图标：{0}，客户ID：{1}", headicon.ToString(), th.ClientName));
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                //Win32API.OutputDebugStringA(string.Format("设置标签的图标出错 ：{0}！", ex.Message));
            }
        }

        /// <summary>
        /// 通过指定的客户端ID来设置标签的Url
        /// </summary>
        /// <param name="clientid"></param>
        /// <param name="title"></param>
        public void SetClientUrl(string clientid, string url)
        {
            try
            {
                foreach (TabHeader th in lstTabHeader)
                {
                    if (th.ClientName.Equals(clientid))
                    {
                        th.Url = url;
                        currPaintTh = th;
                        this.Invalidate();
                        //Win32API.OutputDebugStringA(string.Format("设置标签的状态：{0}，客户ID：{1}", status, th.ClientName));
                        break;
                    }
                }
            }
            catch
            {

            }
        }

        #endregion

        /// <summary>
        /// 销毁一个标签释放标签中的对象
        /// </summary>
        /// <param name="tab"></param>
        private void DeleteOneTab(TabHeader tab)
        {
            try
            {
                int intTabIndex = -1;
                intTabIndex = tab.TabIndex;
                //如果当前关闭的标签的客户端主窗口句柄不等于IntPtr.Zero，则关闭
                if (tab.ClientHandle != IntPtr.Zero)
                {
                    //发送消息，让子进程退出
                    //Win32API.CloseWindow(tab.ClientHandle);
                    int ret = Win32API.PostMessageA(tab.ClientHandle, Win32API.WM_CLOSE,0,0);
                    //int ret = Win32API.DestroyWindow(tab.ClientHandle);
                    //Win32API.OutputDebugStringA( string.Format("发送API命令Win32API.PostMessageA给{0}，返回值：{1}，LastErrorCode:{2}", tab.ClientHandle, ret, Win32API.GetLastError() ));
                }
                //
                //从标签列表中移除当前选择的标签（标签上的关闭按钮被点击）
                lstTabHeader.Remove(tab);
                tab.Dispose();

                //重新计算剩余标签的宽度
                widthCalculate(false);
                //重置所有标签为未选中状态,并重新设置标签编号
                for (int i = 0; i < lstTabHeader.Count; i++)
                {
                    lstTabHeader[i].TabIndex = i;
                    lstTabHeader[i].Selected = false;
                }
                //如果关闭了标签,则激活该标签右侧的标签，如果为0或<0,不激活
                try
                {
                    //OutputDebugStringA(string.Format("当前删除的标签索引{0}", intTabIndex));
                    //至少有3个标签0，1，2，删除1号，2号成了1号
                    if (lstTabHeader.Count > 1)
                    {
                        //如果当前删除的标签号是最后一个标签，则激活左侧的标签
                        if (lstTabHeader.Count == intTabIndex)
                        {
                            intTabIndex = intTabIndex - 1;
                        }
                        lstTabHeader[intTabIndex].Selected = true;
                        thMouseDownPublic = lstTabHeader[intTabIndex];
                        if(thMouseDownPublic.ClientHandle !=IntPtr.Zero)
                        {
                            //显示到最前面
                            Win32API.MoveWindow(thMouseDownPublic.ClientHandle, 0, 0, pnlWebBrowser.Width, pnlWebBrowser.Height, 1);
                            Win32API.ShowWindow(thMouseDownPublic.ClientHandle, 1);
                        }                        
                    }
                    //至少有2个标签0，1,删除1号，直接激活0号
                    else if (lstTabHeader.Count == 1)
                    {
                        //OutputDebugStringA(string.Format("当前删除的标签索引条件：intTabIndex == 0"));
                        lstTabHeader[0].Selected = true;
                        thMouseDownPublic = lstTabHeader[0];
                        if (thMouseDownPublic.ClientHandle != IntPtr.Zero)
                        {
                            //显示到最前面
                            Win32API.MoveWindow(thMouseDownPublic.ClientHandle, 0, 0, pnlWebBrowser.Width, pnlWebBrowser.Height, 1);
                            Win32API.ShowWindow(thMouseDownPublic.ClientHandle, 1);
                        }                        
                    }
                }
                catch (Exception ex)
                {
                    //OutputDebugStringA(string.Format("切换标签时发生了错误：{0}", ex.Message ));
                }
                
                lstTabHeader.Sort();
                this.Invalidate();
            }
            catch(Exception ex)
            {
                //
            } 
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            thMouseDown = null;
            //Win32API.OutputDebugStringA(string.Format("OnMouseDown 按键为:{0} ,{1}次点击！", e.Button, e.Clicks ));

            if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Clicks == 1)
            {
                pMouseDown = e.Location;

                bolDbClickedFlag = false;

                PublicModule.IsHeadDbClicked = false;

                foreach (TabHeader th in lstTabHeader)
                {
                    if (th.HitTest(pMouseDown))
                    {
                        thMouseDown = th;
                        thMouseDown.Selected = true;
                        thMouseDownPublic = th;
                        break;
                    }
                }


                //切换当前标签页的浏览器控件到前台显示
                try
                {
                    if (thMouseDown != null)
                    {
                        //利用子进程的Handle,让其显示
                        if (thMouseDown.ClientHandle != IntPtr.Zero)
                        {
                            //发送消息，让它显示到前台
                            Win32API.MoveWindow(thMouseDown.ClientHandle, 0, 0, pnlWebBrowser.Width, pnlWebBrowser.Height, 1);
                            Win32API.ShowWindow(thMouseDown.ClientHandle, 1);
                        }
                        //重新设置消息
                        try
                        {
                            //FormMain tmpForm = pnlWebBrowser.Parent as FormMain;
                            if (tempFormMain != null && thPreviousMouseDown != thMouseDown)
                            {
                                string strcommand = string.Format("/cmd:getstatus,{0}", thMouseDown.ClientName);
                                tempFormMain.SendToClient(strcommand, thMouseDown.ClientName);
                                Win32API.OutputDebugStringA(string.Format("发消息 /cmd:getstatus 给{0}", thMouseDown.ClientName));
                            }
                        }
                        catch (Exception ex)
                        {
                            string errmessage = string.Format("发送命令/cmd:getstatus到客户端{0}，发生错误{1}", thMouseDown.ClientName, ex.Message);
                            Win32API.OutputDebugStringA(errmessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //do something
                }

                if (thMouseDown != null)
                {
                    //选中了标签
                    bolMouseDownNothing = false;
                    foreach (TabHeader th in lstTabHeader)
                    {
                        if (th != thMouseDown)
                        {
                            th.Selected = false;
                            if (th.ClientHandle != IntPtr.Zero)
                            {
                                Win32API.ShowWindow(th.ClientHandle, 0);
                            }
                        }
                    }
                    this.Invalidate();
                }
                else
                {
                    //移动窗口
                    bolMouseDownNothing = true;
                }
                //记录当前标签有无变更，如果没变更，不做下一步操作
                thPreviousMouseDown = thMouseDown;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Clicks == 2)
            {
                //双击
                bolDbClickedFlag = true;
                //双击标签，并不是按下非标签区域，置为False，壁纸还原时位置在鼠标点击位置
                bolMouseDownNothing = false;
                PublicModule.IsHeadDbClicked = true;

                //判断当前点击位置是否有标签
                pMouseDown = e.Location;

                //遍历所有标签，进行点击测试
                foreach (TabHeader th in lstTabHeader)
                {
                    if (th.HitTest(pMouseDown))
                    {
                        thMouseDown = th;
                        thMouseDown.Selected = th.Selected;
                        break;
                    }
                }
                //

                if (thMouseDown != null)
                {

                    if (DbClickCloseTab)
                    {
                        //如果当前选择了标签，并且标签数量大于1个（编号从0开始）
                        //OutputDebugStringA(string.Format("当前标签总数：{0}", lstTabHeader.Count)); 
                        if (thMouseDown != null && lstTabHeader.Count > 1)
                        {
                            //
                            DeleteOneTab(thMouseDown);
                        }
                    }
                    return;
                }
                //Win32API.OutputDebugStringA(string.Format("没有选中标签，当前坐标为：X:{0},Y:{1}", pMouseDown.X, pMouseDown.Y));
                //Win32API.OutputDebugStringA("双击标签栏的空白区域！"); 
                if (tempFormMain != null)
                {
                    if (tempFormMain.WindowState == FormWindowState.Normal)
                    {
                        //Win32API.OutputDebugStringA( string.Format("主窗口的left:{0},top:{1}", oldLocation.X, oldLocation.Y) ); 
                        tempFormMain.WindowState = FormWindowState.Maximized;
                        tempFormMain.HideBorder = true;
                        cbtnMaximized.StateImage = Resources.res_mask4;

                    }
                    else if (tempFormMain.WindowState == FormWindowState.Maximized)
                    {
                        tempFormMain.WindowState = FormWindowState.Normal;
                        tempFormMain.HideBorder = false;
                        cbtnMaximized.StateImage = Resources.max_mask4;
                        //Win32API.OutputDebugStringA(string.Format("主窗口的left:{0},top:{1}", tmpForm.Location.X, tmpForm.Location.Y ));
                    }
                    else
                    {
                        tempFormMain.WindowState = FormWindowState.Normal;
                        tempFormMain.HideBorder = false;
                    }
                }
                if (this.Parent != null)
                {
                    //Win32API.OutputDebugStringA(string.Format("当前控件的父控件为:{0},Handle为:{1}", this.Parent, this.Parent.Handle));
                    //Form tmpForm = this.Parent as Form;
                    //if (tmpForm != null)
                    //{
                    //    if (tmpForm.WindowState == FormWindowState.Normal)
                    //    {
                    //        //Win32API.OutputDebugStringA( string.Format("主窗口的left:{0},top:{1}", oldLocation.X, oldLocation.Y) ); 
                    //        tmpForm.WindowState = FormWindowState.Maximized;
                    //        cbtnMaximized.StateImage = Resources.res_mask4;

                    //    }
                    //    else if (tmpForm.WindowState == FormWindowState.Maximized)
                    //    {
                    //        tmpForm.WindowState = FormWindowState.Normal;
                    //        cbtnMaximized.StateImage = Resources.max_mask4;
                    //        //Win32API.OutputDebugStringA(string.Format("主窗口的left:{0},top:{1}", tmpForm.Location.X, tmpForm.Location.Y ));
                    //    }
                    //    else
                    //    {
                    //        tmpForm.WindowState = FormWindowState.Normal;
                    //    }
                    //}
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            //Win32API.OutputDebugStringA("鼠标移出！");
            //Win32API.OutputDebugStringA( string.Format("当前鼠标坐标为X:{0}, y{1}", pMouseMove.X,pMouseMove.Y) );
            try
            {
                //foreach (TabHeader th in lstTabHeader)
                //{
                //    Win32API.OutputDebugStringA(string.Format("当前标签鼠标悬停:{0},客户端名称：{1}", th.Hovered, th.ClientName));
                //    if (!th.Selected)
                //    {
                //        th.Hovered = false;
                //    }
                //}

                //if (thMouseMove != null)
                //{
                //    thMouseMove.Hovered = false;
                //    currPaintTh = thMouseMove;
                //    //this.Invalidate();
                //}
                //使自身重绘，防止鼠标快速移出时没有及时重画状态
                this.Invalidate(); 
            }
            catch (Exception ex)
            {

            }
            
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            if (e.Button == System.Windows.Forms.MouseButtons.Left && thMouseDown != null)
            {
                //Win32API.OutputDebugStringA(string.Format("e.Button == System.Windows.Forms.MouseButtons.Left && thMouseDown != null"));
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
                    if (newPos.X < 0) {
                        newPos.X = 0;
                    }
                        
                    //防止将标签拖动到窗口右上角控制按钮区域
                    if (newPos.X > (this.Width - this.ControlBoxWidth - thMouseDown.Rect.Width)) {
                        newPos.X = this.Width - this.ControlBoxWidth - thMouseDown.Rect.Width;
                    }
                   

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
            if (e.Button == System.Windows.Forms.MouseButtons.Left && bolMouseDownNothing)
            {
                //Win32API.OutputDebugStringA(string.Format("e.Button == System.Windows.Forms.MouseButtons.Left && bolMouseDownNothing"));
                //这里会导致窗口位置移动，BUG
                //Win32API.OutputDebugStringA(string.Format("没有选中标签，当前坐标为：X:{0},Y:{1}", pMouseDown.X, pMouseDown.Y));
                if (this.Parent != null)
                {
                    //Win32API.OutputDebugStringA(string.Format("当前控件的父控件为:{0},Handle为:{1}", this.Parent, this.Parent.Handle));
                    Form tmpForm = this.Parent as Form;
                    if (tmpForm != null)
                    {
                        tmpForm.Location = new Point(tmpForm.Location.X + e.X - pMouseDown.X, tmpForm.Location.Y + e.Y - pMouseDown.Y); ;
                    }
                }
            }
            else
            {
                //判断当前标签是否是鼠标悬停
                pMouseMove = e.Location;
                //thMouseMove = null;
                //Win32API.OutputDebugStringA( string.Format("当前鼠标坐标为X:{0}, y{1}", pMouseMove.X,pMouseMove.Y) );
                try
                {
                    foreach (TabHeader th in lstTabHeader)
                    {
                        //如果当前标签有鼠标悬停
                        if (th.HitTest(pMouseMove))
                        {
                            //如果当前标签是已经选择的标签，跳过
                            if (!th.Selected)
                            {
                                th.Hovered = true;
                                //thMouseMove = th;
                            }
                            else
                            {
                                th.Hovered = false;
                            }
                            //将当前标签的标题作为ToolTip显示
                            if (th.TitleHitTest(pMouseMove))
                            {
                                //判断当前标签的有鼠标悬停
                                if (toolTip1 != null)
                                {
                                    if (!toolTip1.GetToolTip(this).Equals(th.Title))
                                    {
                                        toolTip1.SetToolTip(this, th.Title);
                                    }
                                }
                            }
                            else
                            {
                                if (toolTip1 != null)
                                {
                                    toolTip1.Hide(this);
                                }
                            }
                        }
                        else
                        {
                            th.Hovered = false;
                        }
                        //Win32API.OutputDebugStringA(string.Format("标签是否选择：{0} , 是否鼠标悬停：{1}！", th.Selected, th.Hovered));
                    }
                }
                catch(Exception ex)
                {

                }                
                this.Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            thMouseDown = null;
            pMouseDown = new Point();

            //双击标签，并不是按下非标签区域，置为False，壁纸还原时位置在鼠标点击位置
            //这里不置为false,则网址下拉框弹出后，直接点击标签空白区域，会设置窗口位置
            bolMouseDownNothing = false;

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
                int intTabIndex = -1;
                TabHeader thDelete = null;
                foreach (TabHeader th in lstTabHeader)
                {
                    if (th.CloseHitTest(e.Location))
                    {
                        //存储当前选项卡的位置
                        //tmpCurTabindex = lstTabHeader.IndexOf(th) ;
                        intTabIndex = th.TabIndex;
                        //OutputDebugStringA( string.Format("lstTabHeader.Count:{0}", lstTabHeader.Count) ); 
                        //如果标签数量大于1，才删除，始终保留一个标签，或者当标签小于等于1时，关闭app
                        if(lstTabHeader.Count >1 ) {
                            thDelete = th;
                        } else if(lstTabHeader.Count <=1) {
                            //do something
                        }                        
                        break;
                    }
                }

                if (thDelete != null)
                {
                    try
                    {
                        DeleteOneTab(thDelete);
                    }
                    catch(Exception ex)
                    {

                    }                     
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

            e.Graphics.DrawLine(new Pen(this.TabBottomLineColor), new Point(0, this.Height  - 1), new Point(this.Width, this.Height - 1));

            // 判断重绘区域大小，解决由最小化还原后，无法绘制Tab的问题
            if (currPaintTh == null || e.ClipRectangle.Size.Width > TabHeader.Left_Offset)
            {
                // 被选中的Tab 需要处于顶层，因此最后绘制
                TabHeader thSelected = null;
                foreach (TabHeader th in lstTabHeader)
                {
                    if (th.Selected)
                    {
                        thSelected = th;
                    }                        
                    else
                    {
                        th.DrawAll(e.Graphics, th.Rect);
                    }                        
                }
                // 最后绘制
                if (thSelected != null)
                {
                    thSelected.DrawAll(e.Graphics, thSelected.Rect);
                }                    
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
            btnAddNew.Visible = false;
            //计算所有标签的宽度
            widthCalculate();
            //添加一个新标签
            TabHeader newTh = new TabHeader(lstTabHeader.Count, title, font, tabWidth, this, url);
            newTh.Selected = true;
            //给TabHeader对象赋值,标签左侧加载动画
            newTh.mFrameNumber = this.FrameNumber;
            newTh.mFrameRate = this.FrameRate;
            newTh.ForeColor = this.ForeColor;
            newTh.Image = this.Image;
            newTh.noSelectedColor = this.TabNormalColor;
            newTh.SelectedColor = this.TabSelectedColor;
            newTh.HoverColor = this.TabHoverColor;
            
            //将所有添加新标签之前的所有标签设置为未激活
            foreach (TabHeader th in lstTabHeader)
            {
                th.Selected = false;
            }

            try
            {
                //将当前标签父容器的按钮组件传给当前项的TabHeader
                try
                {
                    //this.cbtnbstopload.Visible = true;
                    //this.cbtnbrefresh.Visible = false;
                    //全局当前选择的标签
                    thMouseDownPublic = newTh;
                }
                catch (Exception ex)
                {
                    //OutputDebugStringA(string.Format("传按钮给WebBrowser时出错,错误描述:{0}",ex.Message )); 
                }
                
            }
            catch (Exception ex)
            {
                //OutputDebugStringA(string.Format("webBrowser访问网页地址时出错:{0}", ex.Message));
            }
            //将新标签，添加到后台标签组中
            lstTabHeader.Add(newTh);
            newTh.OnPaintRequest += newTh_OnPaintRequest;

            this.Invalidate();
        }

        public void AddNewTab(string title, Font font, string clientid, IntPtr clienthandle, string url = "")
        {
            btnAddNew.Visible = false;
            //计算所有标签的宽度
            widthCalculate();
            //添加一个新标签
            TabHeader newTh = new TabHeader(lstTabHeader.Count, title, font, tabWidth, this, url);
            //默认激活新标签
            newTh.Selected = true;
            //给TabHeader对象赋值,标签左侧加载动画
            newTh.mFrameNumber = this.FrameNumber;
            newTh.mFrameRate = this.FrameRate;
            newTh.ForeColor = this.ForeColor;
            newTh.Image = this.Image;
            newTh.noSelectedColor = this.TabNormalColor;
            newTh.SelectedColor = this.TabSelectedColor;
            newTh.HoverColor = this.TabHoverColor;
            //记录客户端ID与句柄
            newTh.ClientName = clientid;
            newTh.ClientHandle = clienthandle;

            //将所有添加新标签之前的所有标签设置为未激活
            foreach (TabHeader th in lstTabHeader)
            {
                th.Selected = false;
            }

            try
            {
                //将当前标签父容器的按钮组件传给当前项的TabHeader
                try
                {
                    //this.cbtnbstopload.Visible = true;
                    //this.cbtnbrefresh.Visible = false;
                    //全局当前选择的标签
                    thMouseDownPublic = newTh;
                }
                catch (Exception ex)
                {
                    //OutputDebugStringA(string.Format("传按钮给WebBrowser时出错,错误描述:{0}",ex.Message )); 
                }

            }
            catch (Exception ex)
            {
                //OutputDebugStringA(string.Format("webBrowser访问网页地址时出错:{0}", ex.Message));
            }
            //将新标签，添加到后台标签组中
            lstTabHeader.Add(newTh);
            newTh.OnPaintRequest += newTh_OnPaintRequest;

            this.Invalidate();
        }

        void widthCalculate(bool forAdd = true)
        {
            //预留Controlbox的宽度出来，默认200
            int tabRightOffset = this.ControlBoxWidth;
            if (forAdd)
            {
                if (this.Width < (lstTabHeader.Count + 1) * (tabWidth - TabHeader.Left_Offset) + tabRightOffset || tabWidth < defaultTabWidth)
                {
                    tabWidth = (this.Width - tabRightOffset) / (lstTabHeader.Count + 1) + TabHeader.Left_Offset;
                }                
            }
            else
            {
                if (this.Width < (lstTabHeader.Count) * (tabWidth - TabHeader.Left_Offset) + tabRightOffset || tabWidth < defaultTabWidth)
                {
                    if (lstTabHeader.Count > 0)
                        tabWidth = (this.Width - tabRightOffset) / (lstTabHeader.Count) + TabHeader.Left_Offset;
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
            {
                btnAddNew.Left = (tabWidth - TabHeader.Left_Offset) * (lstTabHeader.Count) + 18;
            }
            btnAddNew.Top = 10;
            btnAddNew.Visible = true;
            //定位右上角ControlBox的位置
            try
            {
                cbtnClose.Top = 0;
                cbtnClose.Left = this.Width - cbtnClose.Width - 0;
                cbtnMaximized.Top = 0;
                cbtnMaximized.Left = this.Width - cbtnClose.Width * 2 - 2;
                cbtnMinimized.Top = 0;
                cbtnMinimized.Left = this.Width - cbtnClose.Width * 3 - 4;
            }
            catch (Exception ex)
            {

            }
            
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

        private bool IsDesignMode()
        {
            try
            {
                if (this.GetService(typeof(IDesignerHost)) != null)
                {
                    return true;
                }
                else if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                {
                    //design mode
                    return true;
                }
                else
                {
                    return false;
                    //runtime mode
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        //用户自定义组件的Load事件，获取该组件父容器中的控件
        private void BrowserTabHeader_Load(object sender, EventArgs e)
        {
            try
            {
                //OutputDebugStringA("BrowserTabHeader_Load 函数执行...");
                //OutputDebugStringA( this.Parent.ToString() );
                //获取标签的父窗口控件集合中名为pnlWebBrowser的Panel控件
                if(!DesignMode)
                {
                    //查找标签窗口组件的父容器，查找它的子控件pnlWebBrowser                    
                    //Win32API.OutputDebugStringA( string.Format("{0}#___________#{1}", this.Parent.GetType(),  this.Parent.Name)); 
                    if (this.Parent != null && this.Parent.GetType().ToString().Equals("BrowserX.FormMain"))
                    {
                        if ((this.Parent as FormMain).WebBrowserParent != null)
                        {
                            pnlWebBrowser = (this.Parent as FormMain).WebBrowserParent;
                            tempFormMain = (this.Parent as FormMain);
                        }
                        else
                        {
                            pnlWebBrowser = null;
                            tempFormMain = null;
                        }
                    }                    
                }
                if (!DesignMode)
                {
                    //添加Tooltip
                    if (toolTip1 != null)
                    {
                        toolTip1.AutoPopDelay = 10000;
                        toolTip1.InitialDelay = 1000;
                        toolTip1.ReshowDelay = 500;

                        toolTip1.ShowAlways = false;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void BrowserTabHeader_MouseLeave(object sender, EventArgs e)
        {
            //
            try
            {
                foreach (TabHeader th in lstTabHeader)
                {
                    //如果当前标签是已经选择的标签，跳过
                    if (!th.Selected)
                    {
                        th.Hovered = false ;
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            //AddNewTab(newTabTitle, this.Font, PublicModule.HomePage);
            try
            {
                string tempclientid = PublicModule.GetNewClientName();
                using (Process process = new Process())
                {
                    //先添加标签，后面再通过客户发送句柄过来，再设置句柄
                    AddNewTab("新建标签页", Font, tempclientid, IntPtr.Zero, PublicModule.HomePage);
                    //组合启动参数
                    string strargs = string.Format("/Client:{0},{1},{2},{3}", PublicModule.channelserver, tempclientid, this.pnlWebBrowser.Handle, PublicModule.HomePage);
                    //创建新的进程
                    process.StartInfo = new ProcessStartInfo("BrowserX.exe", strargs);
                    process.StartInfo.CreateNoWindow = false;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.RedirectStandardOutput = false;
                    process.Start();
                    //output = process.StandardOutput.ReadToEnd().ToLower();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cbtnClose_Click(object sender, EventArgs e)
        {
            //
            if (this.Parent != null)
            {
                //Win32API.OutputDebugStringA(string.Format("当前控件的父控件为:{0},Handle为:{1}", this.Parent, this.Parent.Handle));
                Form tmpForm = this.Parent as Form;
                if (tmpForm != null)
                {
                    tmpForm.Close();
                }
            }
        }

        private void cbtnMaximized_Click(object sender, EventArgs e)
        {
            //
            if (tempFormMain != null)
            {
                if (tempFormMain.WindowState == FormWindowState.Maximized)
                {
                    tempFormMain.WindowState = FormWindowState.Normal;
                    tempFormMain.HideBorder = false;
                    cbtnMaximized.StateImage = Resources.max_mask4;
                }
                else
                {
                    tempFormMain.WindowState = FormWindowState.Maximized;
                    tempFormMain.HideBorder = true;
                    cbtnMaximized.StateImage = Resources.res_mask4;
                }
            }

            //if (this.Parent != null)
            //{
            //    //Win32API.OutputDebugStringA(string.Format("当前控件的父控件为:{0},Handle为:{1}", this.Parent, this.Parent.Handle));
            //    Form tmpForm = this.Parent as Form;
            //    if (tmpForm != null)
            //    {
            //        if (tmpForm.WindowState == FormWindowState.Maximized)
            //        {
            //            tmpForm.WindowState = FormWindowState.Normal;
            //            this.cbtnMaximized.StateImage = Resources.max_mask4;
            //        }
            //        else
            //        {
            //            tmpForm.WindowState = FormWindowState.Maximized;
            //            this.cbtnMaximized.StateImage = Resources.res_mask4;
            //        }
            //    }
            //}
        }

        private void cbtnMinimized_Click(object sender, EventArgs e)
        {
            //
            if (this.Parent != null)
            {
                //Win32API.OutputDebugStringA(string.Format("当前控件的父控件为:{0},Handle为:{1}", this.Parent, this.Parent.Handle));
                Form tmpForm = this.Parent as Form;
                if (tmpForm != null)
                {
                    tmpForm.WindowState = FormWindowState.Minimized;
                }
            }
        }
        
    }
}
