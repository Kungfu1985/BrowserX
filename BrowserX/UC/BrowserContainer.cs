using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Drawing;
using BrowserX.Properties;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Forms.Integration;
#region "ipc"
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
#endregion


namespace BrowserX.UC
{
    //[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public partial class BrowserContainer : UserControl
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern void OutputDebugStringA(string lpOutputString);

        private MiniBlinkPinvoke.BlinkBrowser browser = null;

        private string inputurl = string.Empty;

        //private Object cboxAddressList;

        private ToolTip toolTip1 = new ToolTip();

        private IpcServerChannel channel = null;


        //构造函数
        public BrowserContainer()
        {
            InitializeComponent();
            SetStyle( ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            DoubleBuffered = true;
            //如果不是设计模式，初始化显示WPF控件
            if (!DesignMode)
            {
                try
                {
                    //清除pnlWebBrowser的所有子控件
                    this.pnlWebBrowser.Controls.Clear();
                    if(cboxAddressList!=null)
                    {
                        cboxAddressList.AddItem("https://www.baidu.com/");
                        cboxAddressList.AddItem("https://cn.bing.com/");
                    }
                    //this.elementHost1.Child = null;
                    //if (this.elementHost1.Child == null)
                    //{
                    //    //OutputDebugStringA("添加WPF中的Combox控件！");
                    //    WebComboBox.WebComboBox _cbb = new WebComboBox.WebComboBox(); // WPF控件
                    //    _cbb.Name = "cboxAddressList";
                    //    _cbb.MaxDropDownHeight = this.Height;
                    //    _cbb.DropDownArrow = MakeImageSource(Resources.arrow);
                    //    _cbb.DropDownClear = MakeImageSource(Resources.clearItem);
                    //    _cbb.Items.Add("https://www.baidu.com/");
                    //    _cbb.Items.Add("https://cn.bing.com/");
                    //    _cbb.IsEditable = true;
                    //    _cbb.FontFamily = new System.Windows.Media.FontFamily("微软雅黑");
                    //    _cbb.FontSize = 15;
                    //    _cbb.Foreground = System.Windows.Media.Brushes.Black;
                    //    _cbb.Text = "";
                    //    //添加事件
                    //    _cbb.KeyUp += _cbb_KeyUp;
                    //    _cbb.SelectionChanged += _cbb_SelectionChanged;
                    //    _cbb.DropDownClosed += _cbb_DropDownClosed;
                    //    this.elementHost1.Child = _cbb;
                    //}
                }
                catch (Exception ex)
                {
                    //Win32API.OutputDebugStringA( string.Format("添加cboxAddressList错误, {0}",ex.Message) ); 
                }
            }

        }

        //析构函数
        ~BrowserContainer()
        {
            try
            {
                if (channel != null)
                {
                    ChannelServices.UnregisterChannel(channel);
                }
            }
            catch(Exception ex)
            {
                //
            }
            
        }


        //组件加时生成下拉列表控件
        private void BrowserContainer_Load(object sender, EventArgs e)
        {
            //
            if (!DesignMode) {
                
                //添加Tooltip
                if (toolTip1 != null)
                {
                    toolTip1.AutoPopDelay = 10000;
                    toolTip1.InitialDelay = 1000;
                    toolTip1.ReshowDelay = 500;
                    toolTip1.ShowAlways = false;

                    toolTip1.SetToolTip(this.cbtnCopyData, "复制标题与网址");
                }
                //尝试建立IPC服务
                try
                {
                    //
                    //Instantiate our server channel.
                    channel = new IpcServerChannel("BrowserContainer");
                    //Register the server channel.
                    ChannelServices.RegisterChannel(channel, false);
                    //Register this service type.
                    RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteObject), "RemoteObject", WellKnownObjectMode.SingleCall);
                    Win32API.OutputDebugStringA(string.Format("BrowserContainer say:创建IPC服务成功！"));
                    //Ending
                }
                catch(Exception ex)
                {
                    Win32API.OutputDebugStringA(string.Format("BrowserContainer say:创建IPC服务失败:{0}", ex.Message));
                }
            }
        }


        private void _cbb_DropDownClosed(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void _cbb_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void _cbb_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //throw new NotImplementedException();
        }


        public static bool CheckIsUrlFormat(string strValue)
        {
            return CheckIsFormat(@"(http://)?([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", strValue);
        }


        public static bool CheckIsFormat(string strRegex, string strValue)
        {
            if (strValue != null && strValue.Trim() != "")
            {
                System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(strRegex);

                if (re.IsMatch(strValue))
                    return true;
                else
                    return false;
            }

            return false;
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


        public ImageSource MakeImageSource(Bitmap img)
        {
            try
            {
                IntPtr hBitmap = img.GetHbitmap();
                if ((hBitmap == IntPtr.Zero))
                    return null;
                ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                try
                {
                    Win32API.DeleteObject(hBitmap);
                }
                catch (Exception ex)
                {
                }
                return wpfBitmap;
            }
            catch (Exception ex)
            {
            }
            return null/* TODO Change to default(_) if this is not a reference type */;
        }


        private MiniBlinkPinvoke.BlinkBrowser FindBrowser()
        {
            try
            {
                if (this.pnlWebBrowser.HasChildren)
                {
                    foreach (Control tmpControl in this.pnlWebBrowser.Controls)
                    {
                        if (tmpControl.GetType().ToString().Contains("BlinkBrowser"))
                        {
                            return (tmpControl as MiniBlinkPinvoke.BlinkBrowser);
                            //OutputDebugStringA(string.Format("pnlWebBrowser的{0}，类型为：{1}", tmpControl.Name, tmpControl.GetType().ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }


        private void cbtnbgoback_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.pnlWebBrowser.HasChildren)
                {
                    foreach (Control tmpControl in this.pnlWebBrowser.Controls)
                    {
                        if (tmpControl.GetType().ToString().Contains("BlinkBrowser"))
                        {
                            if (browserTabHeader1 != null)
                            {
                                //OutputDebugStringA("browserTabHeader1 != null"); 
                                if (browserTabHeader1.thMouseDownPublic != null)
                                {
                                    //OutputDebugStringA("browserTabHeader1.thMouseDown");
                                    browserTabHeader1.thMouseDownPublic.PageState = TabHeader.WebPageState.Loading;
                                }
                            }
                            cbtnbstopload.Visible = true;
                            cbtnbrefresh.Visible = false;

                            (tmpControl as MiniBlinkPinvoke.BlinkBrowser).GoBack();
                            break;
                            //OutputDebugStringA(string.Format("pnlWebBrowser的{0}，类型为：{1}", tmpControl.Name, tmpControl.GetType().ToString()));
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            } 
        }

        private void cbtnbgoforward_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.pnlWebBrowser.HasChildren)
                {
                    foreach (Control tmpControl in this.pnlWebBrowser.Controls)
                    {
                        if (tmpControl.GetType().ToString().Contains("BlinkBrowser"))
                        {
                            if (browserTabHeader1 != null)
                            {
                                //OutputDebugStringA("browserTabHeader1 != null"); 
                                if (browserTabHeader1.thMouseDownPublic != null)
                                {
                                    //OutputDebugStringA("browserTabHeader1.thMouseDown");
                                    browserTabHeader1.thMouseDownPublic.PageState = TabHeader.WebPageState.Loading;
                                }
                            }
                            cbtnbstopload.Visible = true;
                            cbtnbrefresh.Visible = false;

                            (tmpControl as MiniBlinkPinvoke.BlinkBrowser).GoForward();
                            break;
                            //OutputDebugStringA(string.Format("pnlWebBrowser的{0}，类型为：{1}", tmpControl.Name, tmpControl.GetType().ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void cbtnbrefresh_Click(object sender, EventArgs e)
        {
            try
            {
                cbtnbrefresh.Visible = false;
                cbtnbstopload.Visible = true;
                if (this.pnlWebBrowser.HasChildren)
                {
                    foreach (Control tmpControl in this.pnlWebBrowser.Controls)
                    {
                        if (tmpControl.GetType().ToString().Contains("BlinkBrowser"))
                        {
                            if (browserTabHeader1 != null)
                            {
                                //OutputDebugStringA("browserTabHeader1 != null"); 
                                if(browserTabHeader1.thMouseDownPublic != null)
                                {
                                    //OutputDebugStringA("browserTabHeader1.thMouseDown");
                                    browserTabHeader1.thMouseDownPublic.PageState = TabHeader.WebPageState.Loading;
                                }
                            }
                            //OutputDebugStringA(string.Format("BlinkBrowser.handle：{0}", (tmpControl as MiniBlinkPinvoke.BlinkBrowser).handle));
                            (tmpControl as MiniBlinkPinvoke.BlinkBrowser).Reload();

                            break;
                            //OutputDebugStringA(string.Format("pnlWebBrowser的{0}，类型为：{1}", tmpControl.Name, tmpControl.GetType().ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Win32API.OutputDebugStringA(string.Format("刷新时发生错误,Error:{0}", ex.Message));
            }
        }

        private void cbtnbstopload_Click(object sender, EventArgs e)
        {
            try
            {
                cbtnbstopload.Visible = false;
                cbtnbrefresh.Visible = true;
                if (this.pnlWebBrowser.HasChildren)
                {
                    foreach (Control tmpControl in this.pnlWebBrowser.Controls)
                    {
                        if (tmpControl.GetType().ToString().Contains("BlinkBrowser"))
                        {

                            if (browserTabHeader1 != null)
                            {
                                //OutputDebugStringA("browserTabHeader1 != null"); 
                                if (browserTabHeader1.thMouseDownPublic != null)
                                {
                                    //OutputDebugStringA("browserTabHeader1.thMouseDown");
                                    browserTabHeader1.thMouseDownPublic.PageState = TabHeader.WebPageState.Normal;
                                }
                            }

                            (tmpControl as MiniBlinkPinvoke.BlinkBrowser).StopLoading();
                            break;
                            //OutputDebugStringA(string.Format("pnlWebBrowser的{0}，类型为：{1}", tmpControl.Name, tmpControl.GetType().ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void cbtnbhomepage_Click(object sender, EventArgs e)
        {
            try
            {
                if (browserTabHeader1 != null)
                {
                    browserTabHeader1.AddNewTab("新建标签页", browserTabHeader1.Font, PublicModule.HomePage);
                    //OutputDebugStringA("browserTabHeader1 != null");                     
                }

                //暂时先屏蔽下边的代码，点击主页按钮后，用新标签页打开，而不是在当前页打开
                if (this.pnlWebBrowser.HasChildren && 1==0)
                {
                    foreach (Control tmpControl in this.pnlWebBrowser.Controls)
                    {
                        if (tmpControl.GetType().ToString().Contains("BlinkBrowser"))
                        {
                            if (browserTabHeader1 != null)
                            {
                                browserTabHeader1.AddNewTab("新建标签页", Font, PublicModule.HomePage);
                                //OutputDebugStringA("browserTabHeader1 != null"); 
                                if (browserTabHeader1.thMouseDownPublic != null)
                                {
                                    //OutputDebugStringA("browserTabHeader1.thMouseDown");
                                    browserTabHeader1.thMouseDownPublic.PageState = TabHeader.WebPageState.Loading;
                                }
                            }
                            cbtnbstopload.Visible = true;
                            cbtnbrefresh.Visible = false;

                            (tmpControl as MiniBlinkPinvoke.BlinkBrowser).Url = PublicModule.HomePage;
                            break;
                            //OutputDebugStringA(string.Format("pnlWebBrowser的{0}，类型为：{1}", tmpControl.Name, tmpControl.GetType().ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void cbtnCopyData_Click(object sender, EventArgs e)
        {
            try
            {
                //foreach (Control tmpControl in this.pnlAddressBoxParent.Controls)
                //{

                //    //OutputDebugStringA( string.Format("搜索Web地址输入框！Name:{0}" , tmpControl.Name )); 

                //    if (tmpControl.Name.Equals("elementHost1"))
                //    {
                //        cboxAddressList = (tmpControl as ElementHost).Child;
                //        break;
                //    }
                //}
                if (cboxAddressList != null)
                {

                    try
                    {
                        // Dim cliFormat As System.Windows.Forms.TextDataFormat
                        System.Windows.Clipboard.Clear();
                        string tmpCopyValue = string.Empty;
                        string tmpurltitle = cboxAddressList.UrlTitle ;
                        string tmpurl = cboxAddressList.Text;
                        tmpCopyValue = string.Format("{0}{1}{2}", tmpurltitle, Environment.NewLine, tmpurl);
                        System.Windows.Clipboard.SetText(tmpCopyValue, System.Windows.TextDataFormat.Text);
                    }
                    catch (Exception ex)
                    {

                    }
                  
                    //OutputDebugStringA(string.Format("pnlAddressBoxParent中找到了名为{0}的{1}控件。", cboxAddressList.Name, cboxAddressList.GetType().ToString()));
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void cboxAddressList_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                //地址栏按键弹起事件
                if (cboxAddressList != null)
                {
                    if ((cboxAddressList.Text.Length > 0 & e.KeyCode == Keys.Enter))
                    {
                        if ((CheckIsUrlFormat(cboxAddressList.Text)))
                        {
                            browser = FindBrowser();

                            if (browserTabHeader1 != null)
                            {
                                //OutputDebugStringA("browserTabHeader1 != null"); 
                                if (browserTabHeader1.thMouseDownPublic != null)
                                {
                                    //OutputDebugStringA("browserTabHeader1.thMouseDown");
                                    browserTabHeader1.thMouseDownPublic.PageState = TabHeader.WebPageState.Loading;
                                }
                            }
                            cbtnbstopload.Visible = true;
                            cbtnbrefresh.Visible = false;

                            if ((browser != null))
                            {
                                string strNewUrl = string.Empty;
                                string strCurUrl = string.Empty;
                                strCurUrl = browser.Url.ToString();
                                strNewUrl = cboxAddressList.Text.ToString();
                                if ((strCurUrl.Equals(strNewUrl)))
                                {
                                    cboxAddressList.Focus();
                                }
                                else
                                {
                                    browser.Url = strNewUrl;                                    
                                }
                            }
                        }
                        else
                        {
                            // '使用百度API进行搜索 
                            if (cboxAddressList.Text.Length <= 0)
                            {
                                return;
                            }

                            if ((browser != null))
                            {
                                try
                                {
                                    string strNewUrl = string.Empty;
                                    strNewUrl = string.Format("https://www.baidu.com/s?wd={0}", cboxAddressList.Text.ToString());
                                    browser.Url = strNewUrl;
                                }
                                catch (Exception ex)
                                {
                                    //do something
                                }
                            }
                        }
                    }

                }                
            }
            catch (Exception ex)
            {
            }
        }

        private void cboxAddressList_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                //地址栏选择改变事件
                if(cboxAddressList != null)
                {
                    browser = FindBrowser();
                    if (browser != null)
                    {
                        //System.Windows.Controls.ComboBoxItem tmpcboxItem = new System.Windows.Controls.ComboBoxItem();
                        if (cboxAddressList.ClickedItem != null)
                        {
                            try
                            {
                                string tmpurl = string.Empty;
                                tmpurl = cboxAddressList.ClickedItem.Text.ToString();
                                //OutputDebugStringA(string.Format("cboxWebSite.SelectedItem={0}------", tmpurl));

                                if (tmpurl.Length > 0)
                                {
                                    if (!browser.Url.Equals(tmpurl))
                                    {
                                        inputurl = tmpurl;
                                    }
                                    else
                                    {
                                        inputurl = "";
                                    }
                                }
                                else
                                {
                                    inputurl = "";
                                }
                            }
                            catch (Exception ex)
                            {
                                //OutputDebugStringA( string.Format("选择ComboBox的项时发生了错误,{0}",ex.Message));
                            }
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
            }
        }

        private void cboxAddressList_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                //OutputDebugStringA(string.Format("弹出下拉框之前的网址:{0}", inputurl));
                if ((string.IsNullOrEmpty(inputurl) || inputurl.Length <= 0))
                    return;
                // 如果inputurl不是清除命令，则访问此地址
                browser = FindBrowser();
                if (browser != null)
                {
                    if (browserTabHeader1 != null)
                    {
                        //OutputDebugStringA("browserTabHeader1 != null"); 
                        if (browserTabHeader1.thMouseDownPublic != null)
                        {
                            //OutputDebugStringA("browserTabHeader1.thMouseDown");
                            browserTabHeader1.thMouseDownPublic.PageState = TabHeader.WebPageState.Loading;
                        }
                    }
                    cbtnbstopload.Visible = true;
                    cbtnbrefresh.Visible = false;

                    browser.Url = inputurl;
                    inputurl = "";
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
