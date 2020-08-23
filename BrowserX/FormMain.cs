using BrowserX.Properties;
using BrowserX.UC;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Shell;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Forms.Integration;
#region "ipc"
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Collections;
using System.Runtime.Remoting.Channels.Http;
using NetRemoting;
using System.Collections.Generic;
using BrowserX.IPCChannel;
using ICommand;
using System.Diagnostics;
#endregion


namespace BrowserX
{
    public partial class FormMain : Form
    {
        //public int ShadowWidth = 4;
        private SkinForm Skin;
        public static BrowserTabHeader browserTabHeader1=null;
        //下拉列表中选择的Url
        //private MiniBlinkPinvoke.BlinkBrowser browser = null;
        private string inputurl = string.Empty;
        //private BrowserX.UC.AddressBox cboxAddressList2 = null;
        private ToolTip toolTip1 = new ToolTip();
        //IPC服务端
        HttpChannel _channel;
        private RemotingObject _remotingObject;
        private string channelport = string.Empty;
        //IPC客户端
        private IRemotingObject _remotingObjectc;
        //private IpcServerChannel channel = null;
        //private string channelport = string.Empty;
        //BlinkBrowser
        //浏览器相关的参数
        public MiniBlinkPinvoke.BlinkBrowser webBrowser=null;
        // 当PageState 为 Loading 时，不会显示该Image
        public Image HeaderIcon = Properties.Resources.icon_normal;
        //WIN7任务栏跳转列表
        JumpList jumplist = null;
        //
        private MenuItemDraw tsmiCut = null;
        private MenuItemDraw tsmiCopy = null;
        private MenuItemDraw tsmiSelectPaste = null;
        private MenuItemDraw tsmiDelete = null;
        private MenuItemDraw tsmiSelectAll = null;
        //阴影窗口
        //private Dropshadow shadow = null;
        private bool IsMouseDown = false;
        private bool IsMustClosed = false;
        private bool IsActivated = false;
        private string appCommand = string.Empty;
        //下载窗口
        private FormDownload TaskDownWindow = null;

        System.Windows.Forms.Timer tmWatchNewUrl = new System.Windows.Forms.Timer();

        #region "处理服务器命令的委托"
        private delegate void delegateServerCommandExec(object info, string toName);
        private delegate void delegateClientCommandExec(object info, string toName);

        #endregion


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

        public FormMain()
        {
            InitializeComponent();
            InitAddressBox();
            InitWebBrowserParent();
            this.FormBorderStyle = FormBorderStyle.None;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.DoubleBuffered = true;
            try
            {
                InitIPCChannel();
            }
            catch(Exception ex)
            {

            }
        }

        public FormMain(String[] args)
        {
            InitializeComponent();
            InitAddressBox();
            InitWebBrowserParent();
            this.FormBorderStyle = FormBorderStyle.None;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.DoubleBuffered = true;
            try
            {
                if (args.Length > 0)
                {
                    //只处理第一条启动参数
                    appCommand = args[0];
                    //Win32API.OutputDebugStringA(string.Format("启动时接收到了启动参数，参数为：{0}", appCommand));
                }
                InitIPCChannel();
            }
            catch(Exception ex)
            {

            }
        }

        #region "添加WPF的下拉列表框"
        private void InitAddressBox()
        {
            if (!DesignMode)
            {
                try
                {
                    //清除pnlWebBrowser的所有子控件
                    this.pnlWebBrowser.Controls.Clear();
                    this.cboxAddressList.Visible = false;
                    this.DoubleBuffered = true;
                    //this.browserTabHeader2.Visible = false;
                    //this.browserToolbar1.Visible = false;
                }
                catch (Exception ex)
                {
                    
                }                
            }
        }
        
        /// <summary>
        /// 初始化IPC服务，如果是浏览器主窗口，则注册为服务端，否则注册为客户端
        /// </summary>
        private void InitIPCChannel()
        {
            //判断启动时是否带参数
            try
            {
                //新建客户端,启动时的参数中带了/Client，则是子进程
                //启动参数协议 /Client:IPC服务地址,客户ID（用于跟主进程通信）
                if (appCommand.StartsWith("/Client:"))
                {
                    PublicModule.appmode = "Client";
                    string strIpcServerUrl = string.Empty;
                    //去除协议头的8个字符
                    strIpcServerUrl = appCommand.Substring(8);
                    //分解出IPC服务地址及主进程分配给子进程的客户ID
                    List<string> listipc = new List<string>(strIpcServerUrl.Split(','));
                    //分解出主进程的IPC服务地址
                    PublicModule.channelserver = listipc[0];
                    //分解出主进程传递过来的客户ID
                    PublicModule.channelclientname = listipc[1];
                    //分解出主进程的窗口句柄
                    int tempparent = 0;
                    tempparent = Convert.ToInt32(listipc[2].ToString());
                    PublicModule.parent =  new IntPtr(tempparent);
                    //分解出url
                    PublicModule.channelclienturl = listipc[3];
                    //释放
                    listipc = null;
                    Win32API.OutputDebugStringA( string.Format( "主进程传过来的参数：{0}, {1}, {2}, {3}", PublicModule.channelserver,PublicModule.channelclientname, PublicModule.parent, PublicModule.channelclienturl) ); 
                    
                    try
                    {
                        //设置反序列化级别  
                        BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
                        BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
                        serverProvider.TypeFilterLevel = TypeFilterLevel.Full;//支持所有类型的反序列化，级别很高  
                                                                              //信道端口  
                        IDictionary idic = new Dictionary<string, string>();
                        idic["name"] = "BrowserX.ClientHttp";
                        idic["port"] = "0";
                        HttpChannel channel = new HttpChannel(idic, clientProvider, serverProvider);
                        ChannelServices.RegisterChannel(channel, false);
                        _remotingObjectc = (IRemotingObject)Activator.GetObject(typeof(IRemotingObject), PublicModule.channelserver);
                        //_remotingObject.ServerToClient += (info, toName) => { rtxMessage.AppendText(info + "\r\n"); };
                        SwapObject swap = new SwapObject();
                        _remotingObjectc.ServerToClient += swap.ToClient;
                        //服务端发来的消息
                        swap.SwapServerToClient += (info, toName) =>
                        {
                            if (toName.Equals(PublicModule.channelclientname))
                            {
                                this.BeginInvoke(new delegateServerCommandExec(ServerCommandExec), info, toName);
                            }                                
                        };
                        //登录到主进程的IPC服务
                        _remotingObjectc.ToLogin(PublicModule.channelclientname);
                        //将自己的Handle发给主进程
                        _remotingObjectc.ToServer(string.Format("/cmd:SetHandler,{0},{1}",this.Handle, PublicModule.channelclientname) , PublicModule.channelclientname);
                        Win32API.OutputDebugStringA(string.Format("BrowserX.FormMain client mode say:连接IPC服务成功！Uri:{0}", PublicModule.channelserver));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message); 
                    }
                    //string strUrl = appCommand.Substring(9);
                    //Win32API.OutputDebugStringA(string.Format("启动时访问的Url：{0}", strUrl));
                    //if (browserTabHeader1 != null)
                    //{
                    //    //browserTabHeader1.AddNewTab("新建标签页", Font, strUrl);
                    //    Win32API.OutputDebugStringA(string.Format("找到了browserTabHeader1，名称为：{0}", browserTabHeader1.Name));
                    //}
                }
                else
                {
                    //服务器端，尝试建立IPC服务
                    PublicModule.appmode = "Server";
                    try
                    {
                        //创建TCP/IP IPC通道，设置反序列化级别  
                        BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
                        BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
                        serverProvider.TypeFilterLevel = TypeFilterLevel.Full;//支持所有类型的反序列化，级别很高  
                        IDictionary idic = new Dictionary<string, string>();
                        //获取一个未使用的TCIP/IP端口号，从9527开始编号
                        int portno = PortHelper.FindNextAvailableTCPPort(8022);
                        channelport = string.Format(string.Format("{0}", portno));

                        Win32API.OutputDebugStringA(string.Format("BrowserX.FormMain say:申请IPC端口号：{0}", channelport));
                        //记录IPC服务通道参数
                        idic["name"] = "BrowserX.ServerHttp";
                        idic["port"] = channelport;

                        _channel = new HttpChannel(idic, clientProvider, serverProvider);
                        _remotingObject = new RemotingObject();

                        //注册IPC通信通道
                        ChannelServices.RegisterChannel(_channel, false);
                        //RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemotingObject), "BrowserX.{0}", WellKnownObjectMode.Singleton); //a方案
                        //将给定的 System.MarshalByRefObject 转换为具有指定 URI 的 System.Runtime.Remoting.ObjRef 类的实例。
                        //ObjRef ：存储生成代理以与远程对象通信所需要的所有信息。
                        string remoteobjname = string.Format(string.Format("BrowserX.{0}", this.Handle));
                        ObjRef objRef = RemotingServices.Marshal(_remotingObject, remoteobjname);//b方案
                        //记录IPC通道
                        //使用当前窗体的handle做为端口号
                        //"http://localhost:8022/remoteobj"
                        PublicModule.channelserver = string.Format(string.Format("http://localhost:{0}/{1}", channelport, remoteobjname));

                        //注册消息事件,客户端发消息到服务端
                        _remotingObject.ClientToServer += (info, toName) =>
                        {
                            //rxtInfo.Invoke((MethodInvoker)(() => { rxtInfo.AppendText(info.ToString() + "\r\n"); }));
                            Win32API.OutputDebugStringA(string.Format("BrowserX.FormMain say:接收到{0}的消息:{1}", toName, info));
                            this.BeginInvoke(new delegateClientCommandExec(CommandExec), info, toName);
                            //this.Invoke((MethodInvoker)(() => { CommandExec(info,toName)  ; }));
                            //客户端发消息过来
                            //SendToClient(info, toName);
                        };
                        _remotingObject.Login += (name) =>
                        {
                            //有客户端登录 
                            Win32API.OutputDebugStringA(string.Format("BrowserX.FormMain say:有客户端登录,Name:{0}", name));
                            //rxtInfo.Invoke((MethodInvoker)(() => { rxtInfo.AppendText(name + " 登录" + "\r\n"); }));
                        };
                        _remotingObject.Exit += (name) =>
                        {
                            //有客户端退出
                            //rxtInfo.Invoke((MethodInvoker)(() => { rxtInfo.AppendText(name + " 退出" + "\r\n"); }));
                        };

                        Win32API.OutputDebugStringA(string.Format("BrowserX.FormMain say:创建IPC服务成功！Uri:{0}",PublicModule.channelserver));
                        //Ending
                    }
                    catch (Exception ex)
                    {
                        Win32API.OutputDebugStringA(string.Format("BrowserX.FormMain say:创建IPC服务失败:{0}", ex.Message));
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
        //
        /// <summary>
        /// 发送消息到客户端
        /// </summary>
        /// <param name="info"></param>
        /// <param name="toName"></param>
        public void SendToClient(object info, string toName)
        {
            foreach (var v in _remotingObject.GetServerEventList())
            {
                try
                {
                    ReceiveHandler receive = (ReceiveHandler)v;
                    receive.BeginInvoke(info, toName, null, null);
                }
                catch
                { 
                
                }
            }
            //_remotingObject.ToClient(info, toName);
        }


        /// <summary>
        /// 执行主进程发过来的命令
        /// </summary>
        /// <param name="info"></param>
        /// <param name="toname">发给哪个子进程对应的标签</param>
        private void ServerCommandExec(object info, string toName)
        {
            Win32API.OutputDebugStringA(string.Format("服务端发消息 {0} 给 {1} ", info.ToString(), toName));
            try
            {
                string reccmd = info.ToString();
                string strcommand = string.Empty;
                string strclientid = string.Empty;
                string ptrhandle = string.Empty;
                string strvalue = string.Empty;
                string strurl = string.Empty;
                //处理命令
                if (info.GetType().Equals(typeof(string)))
                {
                    if (reccmd.StartsWith("/cmd:"))
                    {
                        reccmd = reccmd.Substring(5);
                    }
                    if (reccmd.Length > 0)
                    {
                        //分解命令行
                        List<string> listcmd = new List<string>(reccmd.Split(','));
                        strcommand = listcmd[0];
                        try
                        {
                            if (strcommand.Equals("goback"))
                            {
                                if(this.webBrowser != null)
                                {
                                    this.webBrowser.GoBack(); 
                                }                                
                            }
                            else if (strcommand.Equals("goforward"))
                            {
                                if (this.webBrowser != null)
                                {
                                    this.webBrowser.GoForward();
                                }
                            }
                            else if (strcommand.Equals("refresh"))
                            {
                                if (this.webBrowser != null)
                                {
                                    this.webBrowser.Reload();
                                }
                            }
                            else if (strcommand.Equals("stopload"))
                            {
                                if (this.webBrowser != null)
                                {
                                    this.webBrowser.StopLoading();
                                }
                            }
                            else if (strcommand.Equals("loadurl"))
                            {
                                strvalue = listcmd[1];              //网址
                                if (this.webBrowser != null)
                                {
                                    this.webBrowser.Url = strvalue;
                                }                                
                            }
                            else if (strcommand.Equals("getstatus"))
                            {
                                strvalue = listcmd[1];              //客户ID
                                //return;
                                try
                                {
                                    if (_remotingObjectc != null)
                                    {
                                        _remotingObjectc.ToServer(string.Format("/cmd:SetUrlTitle,{0},{1}", webBrowser.title, PublicModule.channelclientname), PublicModule.channelclientname);
                                        _remotingObjectc.ToServer(string.Format("/cmd:SetAddress,{0},{1}", webBrowser.Url, PublicModule.channelclientname), PublicModule.channelclientname);
                                        if (webBrowser.isloading)
                                        {
                                            _remotingObjectc.ToServer(string.Format("/cmd:SetStatus,{0},{1}", TabHeader.WebPageState.Loading, PublicModule.channelclientname), PublicModule.channelclientname);
                                            _remotingObjectc.ToServer(string.Format("/cmd:SetButtonStatus,{0},{1},{2},{3}", "refresh", "Visible", "false", PublicModule.channelclientname), PublicModule.channelclientname);
                                            _remotingObjectc.ToServer(string.Format("/cmd:SetButtonStatus,{0},{1},{2},{3}", "stopload", "Visible", "true", PublicModule.channelclientname), PublicModule.channelclientname);
                                        }
                                        else
                                        {
                                            _remotingObjectc.ToServer(string.Format("/cmd:SetStatus,{0},{1}", TabHeader.WebPageState.Normal, PublicModule.channelclientname), PublicModule.channelclientname);
                                            _remotingObjectc.ToServer(string.Format("/cmd:SetButtonStatus,{0},{1},{2},{3}", "refresh", "Visible", "true", PublicModule.channelclientname), PublicModule.channelclientname);
                                            _remotingObjectc.ToServer(string.Format("/cmd:SetButtonStatus,{0},{1},{2},{3}", "stopload", "Visible", "false", PublicModule.channelclientname), PublicModule.channelclientname);
                                        }
                                        _remotingObjectc.ToServer(string.Format("/cmd:SetButtonStatus,{0},{1},{2},{3}", "goback", "Enabled", webBrowser.CanGoBack, PublicModule.channelclientname), PublicModule.channelclientname);
                                        _remotingObjectc.ToServer(string.Format("/cmd:SetButtonStatus,{0},{1},{2},{3}", "goforward", "Enabled", webBrowser.CanGoForward, PublicModule.channelclientname), PublicModule.channelclientname);
                                        //_remotingObjectc.ToServer(, PublicModule.channelclientname); 
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //Win32API.OutputDebugStringA(string.Format("BrowserX.FormMain 执行命令遇到错误:{0}！", ex.Message));
                        }
                    }
                    //Win32API.OutputDebugStringA(info.GetType().ToString());
                }                
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// 执行子进程发过来的命令
        /// </summary>
        /// <param name="info"></param>
        /// <param name="toname">发给哪个子进程对应的标签</param>
        private void CommandExec(object info, string toname)
        {
            try
            {
                string reccmd = info.ToString();
                string strcommand = string.Empty;
                string strclientid = string.Empty;
                string ptrhandle = string.Empty;
                string strvalue = string.Empty;
                string strurl = string.Empty;
                string strfile = string.Empty;
                string strfilesize = string.Empty;
                //处理命令
                if (info.GetType().Equals(typeof(string)))
                {
                    if (reccmd.StartsWith("/cmd:"))
                    {
                        reccmd = reccmd.Substring(5);
                    }
                    if (reccmd.Length > 0)
                    {
                        //分解命令行
                        List<string> listcmd = new List<string>(reccmd.Split(','));
                        strcommand = listcmd[0];                       
                        try
                        {
                            if (strcommand.Equals("SetHandler"))
                            {
                                ptrhandle = listcmd[1];
                                strclientid = listcmd[2];

                                int temphandle = 0;
                                temphandle = Convert.ToInt32(ptrhandle);
                                IntPtr childhandle = new IntPtr(temphandle);
                                if (childhandle != IntPtr.Zero)
                                {
                                    if (browserTabHeader2 != null)
                                    {
                                        browserTabHeader2.SetClientHandle(strclientid, childhandle);
                                    }
                                }                                
                            }
                            else if(strcommand.Equals("Activated"))
                            {
                                try
                                {
                                    //this.Activate();
                                    //将窗口提升到Z序最前面，窗口在任务栏的图标会有闪烁
                                    if (!IsActivated)
                                    {
                                        this.BringToFront();
                                    }                                    
                                }
                                catch(Exception ex)
                                {

                                }
                            }
                            else if (strcommand.Equals("Download"))
                            {
                                //_remotingObjectc.ToServer(string.Format("/cmd:Download,{0},{1},{2},{3}", url, filename, len, PublicModule.channelclientname), PublicModule.channelclientname);
                                strurl = listcmd[1];
                                strfile= listcmd[2];
                                strfilesize = listcmd[3];
                                try
                                {
                                    PublicModule.strDownloadUrl = strurl;
                                    PublicModule.strDownFileName = strfile;
                                    PublicModule.strDownFileNameSize = Convert.ToInt64(strfilesize);
                                    //弹出下载文件对话框
                                    FormDownloadNew tmpDownWindow = new FormDownloadNew();
                                    DialogResult drDownload = DialogResult.Cancel;
                                    drDownload = tmpDownWindow.ShowDialog();
                                    if (drDownload == DialogResult.OK)
                                    {
                                        // 添加到下载列表
                                        if (PublicModule.bolDownFlag)
                                        {
                                            if (!PublicModule.bolTaskWindowOpened)
                                            {
                                                TaskDownWindow = new FormDownload();
                                            }
                                                
                                            if (TaskDownWindow.WindowState == FormWindowState.Minimized)
                                            {
                                                TaskDownWindow.WindowState = FormWindowState.Normal;
                                            }
                                                
                                            TaskDownWindow.Visible = true;
                                            TaskDownWindow.Show();
                                            TaskDownWindow.AddDownloadTask();
                                        }
                                    }
                                }
                                catch(Exception ex)
                                {

                                }
                            }
                            else if (strcommand.Equals("SetTitle"))
                            {
                                strvalue = listcmd[1];              //标题
                                strclientid = listcmd[2];           //客户端ID

                                try
                                {
                                    if (browserTabHeader2 != null)
                                    {
                                        browserTabHeader2.SetClientTitle(strclientid, strvalue);
                                    }
                                }
                                catch(Exception ex)
                                {
                                    //do something
                                }
                            }
                            else if (strcommand.Equals("SetUrlTitle"))
                            {
                                strvalue = listcmd[1];              //标题
                                strclientid = listcmd[2];           //客户端ID

                                try
                                {
                                    if (browserTabHeader2 != null)
                                    {
                                        browserTabHeader2.SetClientTitle(strclientid, strvalue);
                                    }
                                    if (cboxAddressList != null)
                                    {
                                        cboxAddressList.UrlTitle = strvalue;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //do something
                                }
                            }
                            else if (strcommand.Equals("SetStatus"))
                            {
                                strvalue = listcmd[1];              //状态
                                strclientid = listcmd[2];           //客户端ID

                                try
                                {
                                    if (browserTabHeader2 != null)
                                    {
                                        //MessageBox.Show(strvalue);
                                        if (strvalue.Equals("Normal"))
                                        {
                                            browserTabHeader2.SetClientStatus(strclientid,  TabHeader.WebPageState.Normal);
                                        }
                                        else
                                        {
                                            browserTabHeader2.SetClientStatus(strclientid, TabHeader.WebPageState.Loading);
                                        }                                        
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //do something
                                }
                            }
                            else if (strcommand.Equals("SetAddress"))
                            {
                                strvalue = listcmd[1];              //网址
                                strclientid = listcmd[2];           //客户端ID

                                try
                                {
                                    if (browserTabHeader2 != null)
                                    {
                                        //MessageBox.Show(strvalue);
                                        browserTabHeader2.SetClientUrl(strclientid, strvalue);
                                    }
                                    if (cboxAddressList != null)
                                    {
                                        cboxAddressList.Text = strvalue;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //do something
                                }
                            }
                            else if (strcommand.Equals("SetButtonStatus"))
                            {
                                string strbuttonname = string.Empty;
                                string strbuttonatt = string.Empty;
                                string strbuttonvalue = string.Empty;

                                strbuttonname = listcmd[1];
                                strbuttonatt = listcmd[2];
                                strbuttonvalue = listcmd[3];
                                try
                                {
                                    switch(strbuttonname)
                                    {
                                        case "goback":
                                            if (strbuttonatt.Equals("Visible"))
                                            {
                                                this.cbtnbgoback.Visible = bool.Parse(strbuttonvalue);
                                            }
                                            else
                                            {
                                                this.cbtnbgoback.Enabled = bool.Parse(strbuttonvalue);
                                            }
                                            break;
                                        case "goforward":
                                            if (strbuttonatt.Equals("Visible"))
                                            {
                                                this.cbtnbgoforward.Visible = bool.Parse(strbuttonvalue);
                                            }
                                            else
                                            {
                                                this.cbtnbgoforward.Enabled = bool.Parse(strbuttonvalue);
                                            }
                                            break;
                                        case "refresh":
                                            if (strbuttonatt.Equals("Visible"))
                                            {
                                                this.cbtnbrefresh.Visible = bool.Parse(strbuttonvalue);
                                            } 
                                            else  
                                            {
                                                this.cbtnbrefresh.Enabled = bool.Parse(strbuttonvalue); 
                                            }
                                            break;
                                        case "stopload":
                                            if (strbuttonatt.Equals("Visible"))
                                            {
                                                this.cbtnbstopload.Visible = bool.Parse(strbuttonvalue);
                                            }
                                            else
                                            {
                                                this.cbtnbstopload.Enabled = bool.Parse(strbuttonvalue);
                                            }
                                            break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }

                            }
                            else if (strcommand.Equals("SetIcon"))
                            {
                                strvalue = listcmd[1];              //图标的Base64编码
                                strclientid = listcmd[2];           //客户端ID

                                try
                                {
                                    if (browserTabHeader2 != null)
                                    {
                                        //MessageBox.Show(strvalue);
                                        Image tabicon = null;
                                        tabicon = PublicModule.StringToImage(strvalue); 
                                        if(tabicon != null)
                                        {
                                            browserTabHeader2.SetClientIcon(strclientid, (Image)tabicon.Clone() );
                                            tabicon.Dispose();
                                            tabicon = null;
                                        }
                                        
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //do something
                                }
                            }
                            else if (strcommand.Equals("NewTabByUrl"))
                            {
                                ptrhandle = listcmd[1];
                                strclientid = listcmd[2];
                                strurl = listcmd[3];
                                try
                                {
                                    string tempclientid = PublicModule.GetNewClientName();
                                    using (Process process = new Process())
                                    {
                                        //先添加标签，后面再通过客户发送句柄过来，再设置句柄
                                        if (browserTabHeader2 != null)
                                        {
                                            browserTabHeader2.AddNewTab("新建标签页", Font, tempclientid, IntPtr.Zero, strurl);
                                        }
                                        string strargs = string.Format("/Client:{0},{1},{2},{3}", PublicModule.channelserver, tempclientid, this.pnlWebBrowser.Handle, strurl);
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

                                //int temphandle = 0;
                                //temphandle = Convert.ToInt32(ptrhandle);
                                //IntPtr childhandle = new IntPtr(temphandle);
                                //if (childhandle != IntPtr.Zero)
                                //{
                                //    CreateNewTab(strclientid, childhandle,strurl);
                                //}
                            }
                        }
                        catch (Exception ex)
                        {
                            //Win32API.OutputDebugStringA(string.Format("BrowserX.FormMain 执行命令遇到错误:{0}！", ex.Message));
                        }                        
                    }                    
                    //Win32API.OutputDebugStringA(info.GetType().ToString());
                } 
                else if (info.GetType().Equals(typeof(Bitmap)))
                {
                    //Win32API.OutputDebugStringA("传送过来的是Image对象！"); 
                    //if (browserTabHeader2 != null)
                    //{
                    //    browserTabHeader2.SetClientIcon(toname, info);
                    //}

                    //Win32API.OutputDebugStringA(info.GetType().ToString());
                }
            }
            catch(Exception ex)
            {

            }
            
        }

        //初始化用来显示子进程的区域
        private void InitWebBrowserParent()
        {
            if(this.pnlWebBrowser!=null) 
            {
                this.WebBrowserParent = pnlWebBrowser;
            }
            try
            {
                //添加Tooltip
                if (toolTip1 != null)
                {
                    toolTip1.AutoPopDelay = 5000;
                    toolTip1.InitialDelay = 1000;
                    toolTip1.ReshowDelay = 5000;
                    toolTip1.ShowAlways = false;

                    toolTip1.SetToolTip(this.cbtnCopyData, "复制标题与网址");
                }
            }
            catch(Exception ex)
            {

            }
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


        /// <summary>
        /// 查找当前当前正在浏览的BlinkBrowser对象
        /// </summary>
        /// <returns></returns>
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
        #endregion

        #region "重写窗体的 CreateParams 属性"
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                //cp.ExStyle |= 0x00080000;  //  WS_EX_LAYERED 扩展样式
                const int WS_EX_COMPOSITED = 0x02000000;
                //如果不设置此项，则点击任务栏上的当前应用程序的图标不能在最小
                //与正常窗口之间切换
                const int WS_MINIMIZEBOX = 0x00020000;
                //设置CreateParams的ExStyle为ExStyle 为 0x02000000 （WS_EX_COMPOSITED）
                //会把窗体和它的子窗体都开启双缓冲
                cp.Style = cp.Style | WS_EX_COMPOSITED | WS_MINIMIZEBOX;
                return cp;
            }
        }
        #endregion


        #region "重写消息处理过程"
        protected override void WndProc(ref Message m)
        {

            switch (m.Msg)
            {
                case 0x10:
                    base.WndProc(ref m);
                    //IsMustClosed = true;
                    //Win32API.OutputDebugStringA("接收到了WM_CLOSE消息！"); 
                    break;
                case 0x21:
                    base.WndProc(ref m);
                    //m.LParam = IntPtr.Zero;//默认值
                    //m.WParam = new IntPtr(1);
                    //m.Result = new IntPtr(3);//MA_NOACTIVATE不激活窗口，不删除鼠标消息
                    break;
                case 0x4A: //WM_COPYDATA
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

        #region 属性
        private bool _skinmobile = true;
        [Category("Skin")]
        [Description("窗体是否可以移动")]
        [Browsable(true)]
        [DefaultValue(typeof(bool), "true")]
        public bool SkinMovable
        {
            get { return _skinmobile; }
            set
            {
                if (_skinmobile != value)
                {
                    _skinmobile = value;
                }
            }
        }


        private Panel _WebBrowserParent = null;
        [Category("Skin")]
        [Description("将WebBrowser在指定的控件上的显示")]
        [Browsable(true)]
        [DefaultValue(typeof(Panel), null)]
        public Panel WebBrowserParent
        {
            get { return _WebBrowserParent; }
            set
            {
                if (_WebBrowserParent != value)
                {
                    _WebBrowserParent = value;
                }
            }
        }

        private bool _hideborder = true;
        [Category("Skin")]
        [Description("窗体四边是否显示边框。")]
        [Browsable(true)]
        [DefaultValue(typeof(bool), "true")]
        public bool HideBorder
        {
            get { return _hideborder; }
            set
            {
                _hideborder = value;

                this.pnlTopLine.Visible = !_hideborder;
                this.pnlLeftLine.Visible = !_hideborder;
                this.pnlBottomLine.Visible = !_hideborder;
                this.pnlRightLine.Visible = !_hideborder;                
            }
        }


        private Color _bordercolor = Color.FromArgb(165,166,170) ;
        [Category("Skin")]
        [Description("窗体四边边框的颜色。")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "165,166,170")]
        public Color BorderColor
        {
            get { return _bordercolor; }
            set
            {
                _bordercolor = value;

                this.pnlTopLine.BackColor = _bordercolor;
                this.pnlLeftLine.BackColor = _bordercolor;
                this.pnlBottomLine.BackColor = _bordercolor;
                this.pnlRightLine.BackColor = _bordercolor;
            }
        }


        private int _skinShadowWidth = 4;
        [Category("Skin")]
        [Description("窗体阴影宽度")]
        [Browsable(true)]
        [DefaultValue(typeof(int), "4")]
        public int ShadowWidth
        {
            get { return _skinShadowWidth; }
            set
            {
                if (_skinShadowWidth != value)
                {
                    _skinShadowWidth = value;
                }
            }
        }
        #endregion

        /// <summary>
        /// 创建新的标签页
        /// </summary>
        /// <param name="url"></param>
        public void CreateNewTab(string url)
        {
            if (browserTabHeader2 != null)
            {
                browserTabHeader2.AddNewTab("新建标签页", browserTabHeader2.Font, url);
                //OutputDebugStringA("browserTabHeader1 != null");                     
            }
        }

        /// <summary>
        /// 创建新的标签页时绑定客户端ID及主其主窗口句柄
        /// </summary>
        /// <param name="url"></param>
        public void CreateNewTab(string clientid, IntPtr clienthandle, string url="")
        {
            if (browserTabHeader2 != null)
            {
                browserTabHeader2.AddNewTab("新建标签页", browserTabHeader2.Font, clientid, clienthandle, url);
                //OutputDebugStringA("browserTabHeader1 != null");                     
            }
        }

        //实时检测是否有新建标签的消息
        void tmWatchNewUrl_Tick(object sender, EventArgs e)
        {
            tmWatchNewUrl.Stop();
            tmWatchNewUrl.Enabled = false;

            try
            {
                if (PublicModule.NewTabUrl.Length > 0)
                {
                    CreateNewTab(PublicModule.NewTabUrl);
                    PublicModule.NewTabUrl = string.Empty;
                }
            }
            catch(Exception ex)
            {

            }

            tmWatchNewUrl.Enabled = true;
            tmWatchNewUrl.Start();
        }

        private void FormMain_LocationChanged(object sender, EventArgs e)
        {
            try
            {
                if(Skin != null)
                {
                    if(PublicModule.IsHeadDbClicked) { 
                        //双击标签空白区域，模拟标题栏（最大化/还原状态）
                    }
                    Skin.Location = new Point(this.Location.X - Skin.ShadowWidth+1, this.Location.Y - Skin.ShadowWidth+1) ;
                    //Win32API.OutputDebugStringA( string.Format("Main窗口的坐标,X:{0},Y:{1}", this.Location.X , this.Location.Y)  ); 
                }
            }
            catch(Exception ex)
            {

            } 
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if(this.WindowState == FormWindowState.Minimized)
                {
                    if (Skin != null) {
                        if (Skin.Visible) {
                            Skin.Visible = false;
                        }                        
                    }
                }
                else if (this.WindowState == FormWindowState.Normal )
                {
                    if (Skin != null) {
                        if (!Skin.Visible) {
                            Skin.Visible = true;
                            Skin.DrawShadow();
                            Skin.Update();
                        }                        
                    }
                }
                else
                {
                    if (Skin != null) {
                        if (Skin.Visible) {
                            Skin.Visible = false;
                        }                        
                    }
                }
                //
                if (Skin != null)
                {
                    //Skin.Size =  new Size(this.Size.Width + Skin.ShadowWidth, this.Size.Height + Skin.ShadowWidth);
                    //Win32API.OutputDebugStringA(string.Format("Skin窗口的尺寸,Width:{0},Height:{1}", Skin.Size.Width, Skin.Size.Height));
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void FormMain_MouseDown(object sender, MouseEventArgs e)
        {
            //
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (toolTip1 != null)
                {
                    toolTip1.Dispose();
                    toolTip1 = null;
                }
                try
                {

                    if(_channel != null)
                    {
                        _channel.StopListening(null);
                        ChannelServices.UnregisterChannel(_channel);
                    }
                }
                catch (Exception ex)
                {
                    //
                }

                try
                {
                    if (webBrowser != null)
                    {
                        webBrowser.OnUrlEndEvent -= BlinkBrowser1_OnUrlEndEvent;
                        webBrowser.OnDownloadFile2 -= BlinkBrowser1_OnDownloadFile2;
                        webBrowser.OnCreateViewEvent -= BlinkBrowser1_OnCreateViewEvent;
                        webBrowser.OnUrlChange3Call -= BlinkBrowser1_OnUrlChange3Call;
                        webBrowser.OnCreateNewTabEvent -= BlinkBrowser1_OnCreateNewTabEvent;
                        webBrowser.OnUrlChange2Call -= BlinkBrowser1_OnUrlChange2Call;
                        webBrowser.OnTitleChangeCall -= BlinkBrowser1_OnTitleChangeCall;
                        webBrowser.DocumentReady2Callback -= BlinkBrowser1_DocumentReady2;
                        webBrowser.OnNetGetFaviconEvent -= BlinkBrowser1_OnNetGetFaviconEvent;
                        webBrowser.OnMouseOverUrlChanged -= BlinkBrowser1_OnMouseOverUrl;
                        webBrowser.OnFocusedChanged -= BlinkBrowser1_OnFocusedChanged;
                        //手动销毁
                        webBrowser.Dispose();
                        webBrowser = null;
                    }
                    //注销客户端，客户端的ID主进程分配
                    //_remotingObjectc.ToExit( PublicModule.channelclientname);
                }
                catch
                { 

                }
            }
            catch(Exception ex)
            {

            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //创建子窗口进程并添加到主进程的窗口列表中，在这里无效
            //Win32API.OutputDebugStringA("FormMain_Load say iam coming.");
            try
            {
                if (PublicModule.appmode.Equals("Client"))
                {
                    //所有按键在发送前，主窗口都先接收再转发出去，
                    //这样可以让FormMain_KeyDown触发防止客户用Alt+F4关闭子进程
                    KeyPreview = true;
                    //IntPtr hMenu = Win32API.GetSystemMenu(this.Handle, 0);
                    //Win32API.EnableMenuItem(hMenu, Win32API.SC_CLOSE, Win32API.MF_DISABLED | Win32API.MF_GRAYED);
                    //Win32API.OutputDebugStringA(string.Format("客户端 {0} 屏蔽Alt+F4功能键！", PublicModule.channelclientname));
                }
            }
            catch(Exception ex)
            {

            }
        }

#region "浏览器回调函数"

        //WebBrowser点击其它链接时触发些回调
        private IntPtr BlinkBrowser1_OnCreateViewEvent(IntPtr webView, IntPtr param, MiniBlinkPinvoke.wkeNavigationType navigationType, String url)
        {
            if (this.webBrowser != null)
            {
                this.webBrowser.Url = url;
                try
                {
                    if (_remotingObjectc != null)
                    {
                        _remotingObjectc.ToServer(string.Format("/cmd:SetStatus,{0},{1}", TabHeader.WebPageState.Loading, PublicModule.channelclientname), PublicModule.channelclientname);
                        _remotingObjectc.ToServer(string.Format("/cmd:SetAddress,{0},{1}", url, PublicModule.channelclientname), PublicModule.channelclientname);
                        _remotingObjectc.ToServer(string.Format("/cmd:SetButtonStatus,{0},{1},{2},{3}", "refresh", "Visible", "false", PublicModule.channelclientname), PublicModule.channelclientname);
                        _remotingObjectc.ToServer(string.Format("/cmd:SetButtonStatus,{0},{1},{2},{3}", "stopload", "Visible", "true", PublicModule.channelclientname), PublicModule.channelclientname);

                        //_remotingObjectc.ToServer(, PublicModule.channelclientname); 
                    }
                }
                catch
                {

                }
            }
            return (IntPtr.Zero);
        }

        //DOM加载完毕时触发此回调，可以在这里判断是事是主Frame完成加载，注入JS
        private void BlinkBrowser1_DocumentReady2(IntPtr webView, IntPtr param, IntPtr frameId, bool mainframe)
        {
            if (this.webBrowser == null) {
                return;
            }
            //禁用/启用公共按钮，前进，后退，刷新
            //发送消息到主窗口，
            try
            {
                if (_remotingObjectc != null)
                {
                    _remotingObjectc.ToServer(string.Format("/cmd:SetStatus,{0},{1}",  TabHeader.WebPageState.Normal , PublicModule.channelclientname), PublicModule.channelclientname);
                    //_remotingObjectc.ToServer(, PublicModule.channelclientname); 
                }
            }
            catch
            {

            }

            try
            {
                if (_remotingObjectc != null)
                {
                    _remotingObjectc.ToServer(string.Format("/cmd:SetButtonStatus,{0},{1},{2},{3}", "goback", "Enabled", webBrowser.CanGoBack, PublicModule.channelclientname), PublicModule.channelclientname);
                    _remotingObjectc.ToServer(string.Format("/cmd:SetButtonStatus,{0},{1},{2},{3}", "goforward", "Enabled", webBrowser.CanGoForward, PublicModule.channelclientname), PublicModule.channelclientname);

                    if (mainframe)
                    {
                        _remotingObjectc.ToServer(string.Format("/cmd:SetButtonStatus,{0},{1},{2},{3}", "refresh", "Visible", "true", PublicModule.channelclientname), PublicModule.channelclientname);
                        _remotingObjectc.ToServer(string.Format("/cmd:SetButtonStatus,{0},{1},{2},{3}", "stopload", "Visible", "false", PublicModule.channelclientname), PublicModule.channelclientname);
                    }
                    else
                    {
                        _remotingObjectc.ToServer(string.Format("/cmd:SetButtonStatus,{0},{1},{2},{3}", "refresh", "Visible", "false", PublicModule.channelclientname), PublicModule.channelclientname);
                        _remotingObjectc.ToServer(string.Format("/cmd:SetButtonStatus,{0},{1},{2},{3}", "stopload", "Visible", "true", PublicModule.channelclientname), PublicModule.channelclientname);
                    }
                    //_remotingObjectc.ToServer(, PublicModule.channelclientname); 
                }
            }
            catch
            {

            }
            
        }

        //WebBrowser点击其它链接时触发些回调
        private void BlinkBrowser1_OnUrlChange2Call(string url)
        {
            try
            {
                if (_remotingObjectc != null)
                {
                    _remotingObjectc.ToServer(string.Format("/cmd:SetAddress,{0},{1}", url, PublicModule.channelclientname), PublicModule.channelclientname);
                    //_remotingObjectc.ToServer(, PublicModule.channelclientname); 
                }
            }
            catch
            {

            }

            //Url = url;
            //try
            //{
            //    //OutputDebugStringA( string.Format("{0}触发了OnUrlChange2Call回调！浏览器对象网页:{1}", this.webBrowser.Name, url )  );
            //    if (cboxAddressList != null)
            //    {
            //        (cboxAddressList as WebComboBox.WebComboBox).Text = url;
            //    }
            //}
            //catch (Exception ce)
            //{

            //}
        }


        /// <summary>
        /// Url点击事件触发此回调
        /// </summary>
        /// <param name="url"></param>
        /// <param name="frameid"></param>
        private void BlinkBrowser1_OnUrlChange3Call(string url, IntPtr frameid)
        {
            try
            {
                if (_remotingObjectc != null)
                {
                    if(!url.Contains("about:blank"))
                    {
                        _remotingObjectc.ToServer(string.Format("/cmd:SetAddress,{0},{1}", url, PublicModule.channelclientname), PublicModule.channelclientname);
                    }
                    //_remotingObjectc.ToServer(, PublicModule.channelclientname); 
                }
            }
            catch
            {

            }
            //Url = url;
            //try
            //{
            //    //OutputDebugStringA(string.Format("{0}触发了OnUrlChange3Call回调！浏览器对象网页:{1}，frameid={2}", this.webBrowser.Name, url, frameid ));
            //    if (cboxAddressList != null)
            //    {
            //        if (!url.Contains("about:blank"))
            //        {
            //            (cboxAddressList as WebComboBox.WebComboBox).Text = url;
            //        }
            //    }
            //}
            //catch (Exception ce)
            //{

            //}
        }


        /// <summary>
        /// WebBrowser标题改变时触发些回调，在这里修改标签的标题属性
        /// </summary>
        /// <param name="title"></param>
        private void BlinkBrowser1_OnTitleChangeCall(string title)
        {
            try
            {
                if (_remotingObjectc != null)
                {
                    _remotingObjectc.ToServer(string.Format("/cmd:SetUrlTitle,{0},{1}", title, PublicModule.channelclientname), PublicModule.channelclientname);
                    //_remotingObjectc.ToServer(, PublicModule.channelclientname); 
                }
            }
            catch
            {

            }
            //Title = title;
            //try
            //{
            //    //OutputDebugStringA(string.Format("{0}触发了OnUrlChange3Call回调！浏览器对象网页:{1}，frameid={2}", this.webBrowser.Name, url, frameid ));
            //    if (cboxAddressList != null)
            //    {
            //        (cboxAddressList as WebComboBox.WebComboBox).UrlTitle = Title;
            //    }
            //}
            //catch (Exception ce)
            //{

            //}
        }


        /// <summary>
        /// 浏览器加载结束后调用此回调，可处理网站的图标
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="url"></param>
        /// <param name="bytes"></param>
        /// <param name="len"></param>
        private void BlinkBrowser1_OnNetGetFaviconEvent(IntPtr webView, string url, byte[] bytes, int len)
        {
            // MsgBox(String.Format("OnNetGetFaviconEvent在调用程序中回调成功！ico网址:{0},数据长度:{1}", url, len))
            string imgdata = string.Empty;
            try
            {
                // 将图标数据转换为ICON
                if (len <= 0)
                {
                    HeaderIcon = Resources.icon_normal;                    
                }
                else
                {
                    System.Drawing.Icon tmpico = null;
                    tmpico = IconHelper.FromByte(bytes);
                    if ((tmpico != null))
                    {
                        HeaderIcon = tmpico.ToBitmap();
                    }
                    else
                    {
                        tmpico = IconHelper.FromWebFile(url);
                        HeaderIcon = tmpico.ToBitmap();
                    }
                    tmpico.Dispose();
                    tmpico = null;
                }
            }
            catch (Exception ex)
            {
                // 使用默认的空白图标
                HeaderIcon = Resources.icon_normal;
            }
            try
            {
                if (_remotingObjectc != null)
                {
                    imgdata = PublicModule.ImageToString( HeaderIcon ); 
                    _remotingObjectc.ToServer( string.Format("/cmd:SetIcon,{0},{1}", imgdata, PublicModule.channelclientname), PublicModule.channelclientname);
                }
            }
            catch (Exception)
            {

            }
        }


        /// <summary>
        /// 加载URL结束事件
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="url"></param>
        /// <param name="len"></param>
        /// <param name="frameid"></param>
        private void BlinkBrowser1_OnUrlEndEvent(byte[] bytes, string url, int len, int frameid)
        {
            try
            {
                //do something
            }
            catch (Exception ex)
            {
            }
        }


        /// <summary>
        /// 下载文件回调
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="url"></param>
        /// <param name="mime"></param>
        /// <param name="disposition"></param>
        /// <param name="filename"></param>
        /// <param name="len"></param>
        private void BlinkBrowser1_OnDownloadFile2(IntPtr webView, string url, string mime, string disposition, string filename, int len)
        {
            if (PublicModule.appmode.Equals("Client"))
            {
                try
                {
                    if (_remotingObjectc != null)
                    {
                        _remotingObjectc.ToServer(string.Format("/cmd:Download,{0},{1},{2},{3}", url, filename, len, PublicModule.channelclientname), PublicModule.channelclientname);
                        Win32API.OutputDebugStringA(string.Format("发送命令/cmd:Download 到 {0} 成功！", PublicModule.channelserver));
                    }
                }
                catch (Exception ex)
                {

                }
            }
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 浏览器鼠标悬停在URL之上，必须添加这个回调，否则获取不到Url
        /// </summary>
        /// <param name="url"></param>
        private void BlinkBrowser1_OnMouseOverUrl(string url)
        {
            // 浏览器鼠标滑过URL时调用
            try
            {
                // do something!
                //this.tbOverUrl.Text = url;
            }
            catch (Exception ex)
            {
            }
        }


        private void BlinkBrowser1_OnFocusedChanged(object info)
        {
            //激活主窗口
            if (PublicModule.appmode.Equals("Client"))
            {
                try
                {
                    if (_remotingObjectc != null)
                    {
                        _remotingObjectc.ToServer(string.Format("/cmd:Activated,{0},{1}", this.Handle, PublicModule.channelclientname), PublicModule.channelclientname);
                        Win32API.OutputDebugStringA(string.Format("发送命令/cmd:Activated 到 {0} 成功！", PublicModule.channelserver));
                    }
                }
                catch (Exception ex)
                {

                }
            }
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 浏览器右键链接使用新标签页打开事件
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="param"></param>
        /// <param name="url"></param>
        private void BlinkBrowser1_OnCreateNewTabEvent(IntPtr webView, IntPtr param, string url)
        {
            try
            {
                if (_remotingObjectc != null)
                {
                    //发送消息给服务器，新建一个标签页
                    string strcommand = string.Format("/cmd:NewTabByUrl,{0},{1},{2}", this.Handle, PublicModule.channelclientname, url);
                    //_remotingObjectc.ToServer(url, PublicModule.channelclientname);
                    _remotingObjectc.ToServer(strcommand, PublicModule.channelclientname);

                }
                //Win32API.OutputDebugStringA(string.Format("当前WebBrowser的句柄:{0}, 参数为：{1}, 要访问的网址:{2}", webView, param,url  ));
            }
            catch (Exception ex)
            {
                Win32API.OutputDebugStringA(string.Format("访问IPC服务发生错误:{0}", ex.Message));
            }
        }

#endregion

        private void cbtnbgoback_Click(object sender, EventArgs e)
        {
            try
            {
                this.cbtnbstopload.Visible = true;
                this.cbtnbrefresh.Visible = false;
                if (browserTabHeader2 != null)
                {
                    //OutputDebugStringA("browserTabHeader1 != null"); 
                    if (browserTabHeader2.thMouseDownPublic != null)
                    {
                        //OutputDebugStringA("browserTabHeader1.thMouseDown");
                        browserTabHeader2.thMouseDownPublic.PageState = TabHeader.WebPageState.Loading;
                        if(browserTabHeader2.thMouseDownPublic.ClientName.Length>0)
                        {
                            if (_remotingObject != null)
                            {
                                Win32API.OutputDebugStringA(string.Format("发送命令 /cmd:goback 到客户端：{0}", browserTabHeader2.thMouseDownPublic.ClientName));
                                //_remotingObject.ToClient("/cmd:goback", browserTabHeader2.thMouseDownPublic.ClientName );
                                SendToClient("/cmd:goback", browserTabHeader2.thMouseDownPublic.ClientName); 
                                //(tmpControl as MiniBlinkPinvoke.BlinkBrowser).GoBack();
                            }
                        }
                    }
                }

                
            } 
            catch (Exception ex)
            {

            }
            return;
            try
            {
                if (this.pnlWebBrowser.HasChildren)
                {
                    foreach (Control tmpControl in this.pnlWebBrowser.Controls)
                    {
                        if (tmpControl.GetType().ToString().Contains("BlinkBrowser"))
                        {
                            if (browserTabHeader2 != null)
                            {
                                //OutputDebugStringA("browserTabHeader1 != null"); 
                                if (browserTabHeader2.thMouseDownPublic != null)
                                {
                                    //OutputDebugStringA("browserTabHeader1.thMouseDown");
                                    browserTabHeader2.thMouseDownPublic.PageState = TabHeader.WebPageState.Loading;
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
            catch (Exception ex)
            {

            }
        }

        private void cbtnbgoforward_Click(object sender, EventArgs e)
        {
            try
            {
                this.cbtnbstopload.Visible = true;
                this.cbtnbrefresh.Visible = false;
                if (browserTabHeader2 != null)
                {
                    //OutputDebugStringA("browserTabHeader1 != null"); 
                    if (browserTabHeader2.thMouseDownPublic != null)
                    {
                        //OutputDebugStringA("browserTabHeader1.thMouseDown");
                        browserTabHeader2.thMouseDownPublic.PageState = TabHeader.WebPageState.Loading;
                        if (browserTabHeader2.thMouseDownPublic.ClientName.Length > 0)
                        {
                            if (_remotingObject != null)
                            {
                                Win32API.OutputDebugStringA(string.Format("发送命令 /cmd:goforward 到客户端：{0}", browserTabHeader2.thMouseDownPublic.ClientName));
                                //_remotingObject.ToClient("/cmd:goback", browserTabHeader2.thMouseDownPublic.ClientName );
                                SendToClient("/cmd:goforward", browserTabHeader2.thMouseDownPublic.ClientName);
                                //(tmpControl as MiniBlinkPinvoke.BlinkBrowser).GoBack();
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {

            }
            return;
            try
            {
                if (this.pnlWebBrowser.HasChildren)
                {
                    foreach (Control tmpControl in this.pnlWebBrowser.Controls)
                    {
                        if (tmpControl.GetType().ToString().Contains("BlinkBrowser"))
                        {
                            if (browserTabHeader2 != null)
                            {
                                //OutputDebugStringA("browserTabHeader1 != null"); 
                                if (browserTabHeader2.thMouseDownPublic != null)
                                {
                                    //OutputDebugStringA("browserTabHeader1.thMouseDown");
                                    browserTabHeader2.thMouseDownPublic.PageState = TabHeader.WebPageState.Loading;
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
                this.cbtnbstopload.Visible = true;
                this.cbtnbrefresh.Visible = false;
                if (browserTabHeader2 != null)
                {
                    //OutputDebugStringA("browserTabHeader1 != null"); 
                    if (browserTabHeader2.thMouseDownPublic != null)
                    {
                        //OutputDebugStringA("browserTabHeader1.thMouseDown");
                        browserTabHeader2.thMouseDownPublic.PageState = TabHeader.WebPageState.Loading;
                        if (browserTabHeader2.thMouseDownPublic.ClientName.Length > 0)
                        {
                            if (_remotingObject != null)
                            {
                                Win32API.OutputDebugStringA(string.Format("发送命令 /cmd:refresh 到客户端：{0}", browserTabHeader2.thMouseDownPublic.ClientName));
                                //_remotingObject.ToClient("/cmd:goback", browserTabHeader2.thMouseDownPublic.ClientName );
                                SendToClient("/cmd:refresh", browserTabHeader2.thMouseDownPublic.ClientName);
                                //(tmpControl as MiniBlinkPinvoke.BlinkBrowser).GoBack();
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {

            }
            return;
            try
            {
                cbtnbstopload.Visible = true;
                cbtnbrefresh.Visible = false;
                if (this.pnlWebBrowser.HasChildren)
                {
                    foreach (Control tmpControl in this.pnlWebBrowser.Controls)
                    {
                        if (tmpControl.GetType().ToString().Contains("BlinkBrowser"))
                        {
                            if (browserTabHeader2 != null)
                            {
                                //OutputDebugStringA("browserTabHeader1 != null"); 
                                if (browserTabHeader2.thMouseDownPublic != null)
                                {
                                    //OutputDebugStringA("browserTabHeader1.thMouseDown");
                                    browserTabHeader2.thMouseDownPublic.PageState = TabHeader.WebPageState.Loading;
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
                this.cbtnbrefresh.Visible = true;
                this.cbtnbstopload.Visible = false;
                if (browserTabHeader2 != null)
                {
                    //OutputDebugStringA("browserTabHeader1 != null"); 
                    if (browserTabHeader2.thMouseDownPublic != null)
                    {
                        //OutputDebugStringA("browserTabHeader1.thMouseDown");
                        browserTabHeader2.thMouseDownPublic.PageState = TabHeader.WebPageState.Loading;
                        if (browserTabHeader2.thMouseDownPublic.ClientName.Length > 0)
                        {
                            if (_remotingObject != null)
                            {
                                Win32API.OutputDebugStringA(string.Format("发送命令 /cmd:stopload 到客户端：{0}", browserTabHeader2.thMouseDownPublic.ClientName));
                                //_remotingObject.ToClient("/cmd:goback", browserTabHeader2.thMouseDownPublic.ClientName );
                                SendToClient("/cmd:stopload", browserTabHeader2.thMouseDownPublic.ClientName);
                                //(tmpControl as MiniBlinkPinvoke.BlinkBrowser).GoBack();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return;
            try
            {
                cbtnbrefresh.Visible = true;
                cbtnbstopload.Visible = false;
                if (this.pnlWebBrowser.HasChildren)
                {
                    foreach (Control tmpControl in this.pnlWebBrowser.Controls)
                    {
                        if (tmpControl.GetType().ToString().Contains("BlinkBrowser"))
                        {

                            if (browserTabHeader2 != null)
                            {
                                //OutputDebugStringA("browserTabHeader1 != null"); 
                                if (browserTabHeader2.thMouseDownPublic != null)
                                {
                                    //OutputDebugStringA("browserTabHeader1.thMouseDown");
                                    browserTabHeader2.thMouseDownPublic.PageState = TabHeader.WebPageState.Normal;
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
                string tempclientid = PublicModule.GetNewClientName();
                using (Process process = new Process())
                {
                    //先添加标签，后面再通过客户发送句柄过来，再设置句柄
                    if (browserTabHeader2 != null)
                    {
                        browserTabHeader2.AddNewTab("新建标签页", new Font("微软雅黑",9), tempclientid, IntPtr.Zero, PublicModule.HomePage);
                    }
                    string strargs = string.Format("/Client:{0},{1},{2},{3}", PublicModule.channelserver, tempclientid, this.pnlWebBrowser.Handle, PublicModule.HomePage);
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
            try
            {
                if (browserTabHeader2 != null)
                {
                    //browserTabHeader2.AddNewTab("新建标签页", browserTabHeader2.Font, PublicModule.HomePage);
                    //OutputDebugStringA("browserTabHeader1 != null");                     
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
                if (cboxAddressList != null)
                {
                    try
                    {
                        // Dim cliFormat As System.Windows.Forms.TextDataFormat
                        System.Windows.Clipboard.Clear();
                        string tmpCopyValue = string.Empty;
                        string tmpurltitle = cboxAddressList.UrlTitle;
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
            catch (Exception ex)
            {

            }
        }

        private void pnlWebBrowser_ControlAdded(object sender, ControlEventArgs e)
        {
            //Win32API.OutputDebugStringA("pnlWebBrowser 添加控件事件触发了！"); 
        }

        private void pnlWebBrowser_SizeChanged(object sender, EventArgs e)
        {
            //Win32API.OutputDebugStringA("pnlWebBrowser 尺寸变更事件触发了！");
            if(PublicModule.appmode.Equals("Server"))
            {
                if (browserTabHeader2.thMouseDownPublic!=null)
                {
                    if (browserTabHeader2.thMouseDownPublic.ClientHandle != IntPtr.Zero)
                    {
                        Win32API.MoveWindow(browserTabHeader2.thMouseDownPublic.ClientHandle, 0, 0, pnlWebBrowser.Width, pnlWebBrowser.Height, 1);
                    }
                }                
                //foreach (object tts in pnlWebBrowser.Controls)
                //{
                //    Win32API.OutputDebugStringA(string.Format("pnlWebBrowser 中包含了{0}。", tts.GetType()));
                //}
            }            
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.ShadowWidth = 6;
            this.BorderColor = Color.FromArgb(232, 232, 232);
            this.HideBorder = false;
            if (PublicModule.appmode.Equals("Server"))
            {
                this.MinimumSize = new Size(640 - ShadowWidth + 1, 480 - ShadowWidth + 1);
                this.MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            }            
            //添加阴影层
            try
            {
                if (!DesignMode)
                {
                    //如果是服务端，则显示阴影，否则不显示
                    if (PublicModule.appmode.Equals("Server"))
                    {
                        Skin = new SkinForm(this);//创建皮肤层 
                        Skin.ShowIcon = false;
                        Skin.ShadowWidth = ShadowWidth;
                        Skin.ShowInTaskbar = false;
                        BackgroundImage = null;//去除控件层背景
                        //不能加这句，否则dwm内存一直疯狂上涨
                        //TransparencyKey = BackColor;//使控件层背景透明
                        Skin.Show();//显示皮肤层 
                    }
                }
            }
            catch (Exception ex)
            {

            }
            try
            {
                if (jumplist == null)
                {
                    jumplist = new JumpList();
                }
                if (jumplist != null)
                {
                    //添加跳转列表
                    try
                    {
                        // JumpListLink
                        JumpTask tjumpTask = new JumpTask();
                        tjumpTask.Title = "新建窗口";
                        tjumpTask.Arguments = "/New:HomePage";
                        tjumpTask.Description = "新建普通窗口";
                        //tjumpTask.CustomCategory = "窗口";
                        tjumpTask.IconResourcePath = Application.StartupPath;
                        tjumpTask.ApplicationPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

                        JumpTask tjumpTaskTop = new JumpTask();
                        tjumpTaskTop.Title = "访问主页";
                        tjumpTaskTop.Arguments = "/Browser:im.qq.com";
                        tjumpTaskTop.Description = "访问开发者主页";
                        //tjumpTaskTop.CustomCategory = "功能区";
                        tjumpTaskTop.IconResourcePath = Application.StartupPath;
                        tjumpTaskTop.ApplicationPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;


                        //JumpTask tjumpTask2 = new JumpTask();
                        //tjumpTask2.Title = "新建窗口";
                        //tjumpTask2.Arguments = string.Format("/Client:{0}",PublicModule.channelserver);
                        //tjumpTask2.Description = "创建子进程窗口";
                        ////tjumpTask.CustomCategory = "窗口";
                        //tjumpTask2.IconResourcePath = Application.StartupPath;
                        //tjumpTask2.ApplicationPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;


                        // Dim tjumList As New JumpList
                        jumplist.JumpItems.Add(tjumpTask);
                        jumplist.JumpItems.Add(tjumpTaskTop);
                        jumplist.ShowFrequentCategory = true;
                        jumplist.ShowRecentCategory = true;

                        jumplist.Apply();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }

            try
            {
                //Win32API.OutputDebugStringA("FormMain_Shown say iam coming.");
                //在这里创建一个子进程，并显示到主窗口
                if (PublicModule.appmode.Equals("Server"))
                {
                    try
                    {
                        this.cboxAddressList.Visible = true;
                        this.cboxAddressList.AddItem("https://www.baidu.com/");
                        this.cboxAddressList.AddItem("https://cn.bing.com/");
                    }
                    catch (Exception ex)
                    {

                    }

                    try
                    {
                        this.cbtnbgoback.Enabled = false;
                        this.cbtnbgoforward.Enabled = false;
                        string tempclientid = PublicModule.GetNewClientName();
                        using (Process process = new Process())
                        {
                            //先添加标签，后面再通过客户发送句柄过来，再设置句柄
                            if (browserTabHeader2 != null)
                            {
                                browserTabHeader2.AddNewTab("新建标签页", new Font("微软雅黑", 9), tempclientid, IntPtr.Zero, "");
                            }
                            //
                            string strargs = string.Format("/Client:{0},{1},{2},{3}", PublicModule.channelserver, tempclientid, this.pnlWebBrowser.Handle, "");
                            process.StartInfo = new ProcessStartInfo("BrowserX.exe", strargs);
                            process.StartInfo.CreateNoWindow = false;
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            process.StartInfo.RedirectStandardOutput = false;
                            process.Start();
                            //output = process.StandardOutput.ReadToEnd().ToLower();
                        }
                        try
                        {
                            //this.browserToolbar1.Visible = true;
                            //this.browserTabHeader2.Visible = true;
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        //throw ex;
                    }
                }
                else
                {
                    //创建WebBrowser,并去掉标签栏及工具栏 
                    this.browserTabHeader2.Visible = false;
                    this.browserToolbar1.Visible = false;
                    this.HideBorder = true;
                    this.StartPosition = FormStartPosition.Manual;
                    //this.Dock = DockStyle.Fill;
                    //Win32API.OutputDebugStringA(string.Format("PublicModule.parent:{0}", PublicModule.parent));
                    //设置当前窗口为主进程pnlWebBrowser的子控件
                    Win32API.SetParent(this.Handle, PublicModule.parent);
                    //获取主进程主窗口中pnlWebBrowser控件的尺寸
                    Win32API.RECT webRect = new Win32API.RECT();
                    int ret = Win32API.GetClientRect(PublicModule.parent, ref webRect);
                    //移动窗口位置
                    if (ret != 0)
                    {
                        //bool ret2 = Win32API.SetWindowPos(this.Handle, 0, 0, 0, webRect.Width, webRect.Height, Win32API.SWP_NOZORDER);
                        int ret2 = Win32API.MoveWindow(this.Handle, webRect.Left, webRect.Top, webRect.Width, webRect.Height, 1);
                        //Win32API.OutputDebugStringA(string.Format("Win32API.MoveWindow return value:{0}", ret2));
                        //Win32API.OutputDebugStringA(string.Format("GetLastError return value:{0}", Win32API.GetLastError()));
                    }

                    if (_remotingObjectc != null)
                    {
                        //_remotingObjectc.ToServer( string.Format("/cmd:MoveWindow,{0}", this.Handle ) , PublicModule.channelclientname); 
                    }

                    try
                    {
                        //在这里创建子进程

                        if (webBrowser == null)
                        {
                            webBrowser = new MiniBlinkPinvoke.BlinkBrowser();
                            //OutputDebugStringA(string.Format("创建了BlinkBrowser,句柄：{0}", webBrowser.handle));
                        }

                        if (webBrowser != null)
                        {
                            //OutputDebugStringA(string.Format("创建了BlinkBrowser,句柄：{0}", webBrowser.handle ) );
                            //绑定事件
                            webBrowser.OnUrlEndEvent += BlinkBrowser1_OnUrlEndEvent;
                            webBrowser.OnDownloadFile2 += BlinkBrowser1_OnDownloadFile2;
                            webBrowser.OnCreateViewEvent += BlinkBrowser1_OnCreateViewEvent;
                            webBrowser.OnCreateNewTabEvent += BlinkBrowser1_OnCreateNewTabEvent;
                            webBrowser.OnUrlChange3Call += BlinkBrowser1_OnUrlChange3Call;
                            webBrowser.OnUrlChange2Call += BlinkBrowser1_OnUrlChange2Call;
                            webBrowser.OnTitleChangeCall += BlinkBrowser1_OnTitleChangeCall;
                            webBrowser.DocumentReady2Callback += BlinkBrowser1_DocumentReady2;
                            webBrowser.OnNetGetFaviconEvent += BlinkBrowser1_OnNetGetFaviconEvent;
                            webBrowser.OnMouseOverUrlChanged += BlinkBrowser1_OnMouseOverUrl;
                            webBrowser.OnFocusedChanged += BlinkBrowser1_OnFocusedChanged;
                            //设置回调函数    
                            //加载网页
                            if (PublicModule.channelclienturl.Length <= 0)
                            {
                                webBrowser.Url = PublicModule.HomePage;
                            }
                            else
                            {
                                webBrowser.Url = PublicModule.channelclienturl;
                            }
                            webBrowser.Dock = DockStyle.Fill;
                            //添加事件
                            try
                            {
                                //CreateRemoteObject(webBrowser.Handle);
                                this.pnlWebBrowser.Controls.Clear();
                                this.pnlWebBrowser.Controls.Add(webBrowser);
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }

            try
            {
                if(PublicModule.appmode.Equals("Server"))
                {
                    //给AddressBox添加右键菜单
                    try
                    {
                        //AddressBox右键菜单
                        ContextMenuStrip AddressBoxMenuStrip = new ContextMenuStrip();
                        AddressBoxMenuStrip.AutoSize = false;
                        AddressBoxMenuStrip.Width = 220;
                        AddressBoxMenuStrip.Height = 27 * 8 - 27/2;
                        //添加菜单弹出时事件
                        AddressBoxMenuStrip.Opening += AddressBoxMenuStrip_Opening; ;
                        #region "初始化全局右键菜单"
                        MenuItemDraw tsmiPaste2Go = new MenuItemDraw("粘贴并访问", null, (x, y) =>
                        {
                            //菜单粘贴并访问
                            try
                            {
                                if (cboxAddressList != null)
                                {
                                    string strTempUrl = Clipboard.GetText();
                                    if (strTempUrl.Length <= 0)
                                    {
                                        return;
                                    }
                                    cboxAddressList.Text = strTempUrl;
                                    //触发事件
                                    Keys keydata = new Keys();
                                    keydata = Keys.Enter;
                                    System.Windows.Forms.KeyEventArgs key = new System.Windows.Forms.KeyEventArgs(keydata);
                                    //添加到下拉列表
                                    cboxAddressList.ProcessInput(keydata);
                                    //模拟按下Enter键
                                    //cboxAddressList_KeyUp(null, ke);
                                    //结束 
                                }
                            }
                            catch (Exception ex)
                            {
                                //OutputDebugStringA("MnuPasteAndLoad_Click error:" + ex.Message); 
                            }
                        });
                        tsmiPaste2Go.AutoSize = false;
                        tsmiPaste2Go.Width = 220;
                        tsmiPaste2Go.Height = 27;
                        MenuItemDraw tsmiTitle2Url = new MenuItemDraw("复制标题与网址", null, (x, y) =>
                        {
                            try
                            {
                                // Dim cliFormat As System.Windows.Forms.TextDataFormat
                                Clipboard.Clear();
                                string tmpCopyValue = string.Empty;
                                tmpCopyValue = string.Format("{0}{1}{2}", this.cboxAddressList.UrlTitle, Environment.NewLine, this.cboxAddressList.Text);
                                Clipboard.SetText(tmpCopyValue, TextDataFormat.Text);
                            }
                            catch (Exception ex)
                            {
                            }
                            //BlinkBrowserPInvoke.wkeGoForward(handle);
                        });
                        tsmiTitle2Url.AutoSize = false;
                        tsmiTitle2Url.Width = 220;
                        tsmiTitle2Url.Height = 27;
                        tsmiCut = new MenuItemDraw("剪切(&X)", null, (x, y) =>
                        {
                            cboxAddressList.ProcessCommand("cut");
                            //BlinkBrowserPInvoke.wkeReload(handle);
                        });
                        tsmiCut.AutoSize = false;
                        tsmiCut.Width = 220;
                        tsmiCut.Height = 27;
                        tsmiCopy = new MenuItemDraw("复制(&C)", null, (x, y) =>
                        {
                            cboxAddressList.ProcessCommand("copy");
                            //BlinkBrowserPInvoke.wkeReload(handle);
                        });
                        tsmiCopy.AutoSize = false;
                        tsmiCopy.Width = 220;
                        tsmiCopy.Height = 27;
                        tsmiSelectPaste = new MenuItemDraw("粘贴(&V)", null, (x, y) =>
                        {
                            cboxAddressList.ProcessCommand("paste");
                            //BlinkBrowserPInvoke.wkeSelectAll(handle);
                        });
                        tsmiSelectPaste.AutoSize = false;
                        tsmiSelectPaste.Width = 220;
                        tsmiSelectPaste.Height = 27;
                        tsmiDelete = new MenuItemDraw("删除(&D)", null, (x, y) =>
                        {
                            cboxAddressList.ProcessCommand("delete");
                            //如果查看源码只需要传回URL地址
                        });
                        tsmiDelete.AutoSize = false;
                        tsmiDelete.Width = 220;
                        tsmiDelete.Height = 27;
                        tsmiSelectAll = new MenuItemDraw("全选(&A)", null, (x, y) =>
                        {
                            cboxAddressList.ProcessCommand("selectall");
                            //BlinkBrowserPInvoke.wkeSelectAll(handle);
                        });
                        tsmiSelectAll.AutoSize = false;
                        tsmiSelectAll.Width = 220;
                        tsmiSelectAll.Height = 27;

                        //添加分隔符
                        ToolStripSeparator tssItemFirst = new ToolStripSeparator();
                        tssItemFirst.Height = 5;
                        tssItemFirst.Width = 220;
                        tssItemFirst.AutoSize = false;
                        ToolStripSeparator tssItemFirst1 = new ToolStripSeparator();
                        tssItemFirst1.Height = 5;
                        tssItemFirst1.Width = 220;
                        tssItemFirst1.AutoSize = false;
                        //设置菜单项的参数
                        tsmiCopy.ShortcutKeys = Keys.Control | Keys.C;
                        tsmiCut.ShortcutKeys = Keys.Control | Keys.X;
                        tsmiSelectPaste.ShortcutKeys = Keys.Control | Keys.V;
                        tsmiSelectAll.ShortcutKeys = Keys.Control | Keys.A;
                        //添加到主菜单容器
                        AddressBoxMenuStrip.Items.Add(tsmiPaste2Go);
                        AddressBoxMenuStrip.Items.Add(tsmiTitle2Url);
                        AddressBoxMenuStrip.Items.Add(tssItemFirst1);
                        AddressBoxMenuStrip.Items.Add(tsmiCut);
                        AddressBoxMenuStrip.Items.Add(tsmiCopy);
                        AddressBoxMenuStrip.Items.Add(tsmiSelectPaste);
                        AddressBoxMenuStrip.Items.Add(tsmiDelete);
                        AddressBoxMenuStrip.Items.Add(tssItemFirst);
                        AddressBoxMenuStrip.Items.Add(tsmiSelectAll);
                        AddressBoxMenuStrip.ShowCheckMargin = false;
                        //AddressBoxMenuStrip.Renderer = new myToolStripRenderer();
                        cboxAddressList.Menu = AddressBoxMenuStrip;
                        #endregion "初始化全局右键菜单结束"
                    }
                    catch (Exception ex)
                    {
                        Win32API.OutputDebugStringA(string.Format("初始化菜单出错,{0}",ex.Message) );
                    }
                }
                //tmWatchNewUrl.Interval = 500;
                //tmWatchNewUrl.Tick += tmWatchNewUrl_Tick;
                //tmWatchNewUrl.Stop();
                //tmWatchNewUrl.Enabled = false ;
            }
            catch (Exception ex)
            {

            }
        }

        private void AddressBoxMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if(cboxAddressList != null)
            {
                if (cboxAddressList.SelectedText.Length > 0)
                {
                    tsmiCopy.Enabled = true;
                    tsmiCut.Enabled = true;
                    tsmiDelete.Enabled = true;                    
                    tsmiSelectPaste.Enabled = true;
                    //如果选选择了全部文本，则全选按钮禁用
                    if(cboxAddressList.Text.Length == cboxAddressList.SelectedText.Length)
                    {
                        tsmiSelectAll.Enabled = false;
                    }
                    else
                    {
                        tsmiSelectAll.Enabled = true;
                    }
                }
                else
                {
                    tsmiCopy.Enabled = false;
                    tsmiCut.Enabled = false;
                    tsmiDelete.Enabled = false;
                    tsmiSelectPaste.Enabled = true;
                    //没有选择任何文本，全选按钮启用
                    tsmiSelectAll.Enabled = true;
                }
            }
            //throw new NotImplementedException();
        }

        private void cboxAddressList_SelectionChanged(object sender, EventArgs e)
        {
            //Win32API.OutputDebugStringA( cboxAddressList.Text.ToString() );
            if (cboxAddressList.ClickedItem != null) {
                //Win32API.OutputDebugStringA(cboxAddressList.ClickedItem.Text.ToString());
            }
            //
            try
            {
                //地址栏选择改变事件
                if (browserTabHeader2 != null)
                {
                    if (browserTabHeader2.thMouseDownPublic != null)
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
                                    if (!browserTabHeader2.thMouseDownPublic.Url.Equals(tmpurl))
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

        private void cboxAddressList_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //地址栏按键弹起事件
                if ((cboxAddressList.Text.Length > 0 & e.KeyCode == Keys.Enter))
                {
                    if ((CheckIsUrlFormat(cboxAddressList.Text)))
                    {
                        if (browserTabHeader2 != null)
                        {
                            //OutputDebugStringA("browserTabHeader1 != null"); 
                            cbtnbstopload.Visible = true;
                            cbtnbrefresh.Visible = false;

                            if (browserTabHeader2.thMouseDownPublic != null)
                            {
                                //OutputDebugStringA("browserTabHeader1.thMouseDown");
                                browserTabHeader2.thMouseDownPublic.PageState = TabHeader.WebPageState.Loading;
                                if (browserTabHeader2.thMouseDownPublic.ClientName.Length > 0)
                                {
                                    if (_remotingObject != null)
                                    {
                                        Win32API.OutputDebugStringA(string.Format("发送命令 /cmd:loadurl 到客户端：{0}", browserTabHeader2.thMouseDownPublic.ClientName));
                                        //_remotingObject.ToClient("/cmd:goback", browserTabHeader2.thMouseDownPublic.ClientName );
                                        string strNewUrl = cboxAddressList.Text.ToString();
                                        SendToClient(string.Format("/cmd:loadurl,{0}", strNewUrl), browserTabHeader2.thMouseDownPublic.ClientName);
                                        //(tmpControl as MiniBlinkPinvoke.BlinkBrowser).GoBack();                                        
                                    }
                                }
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
                        if (browserTabHeader2 != null)
                        {
                            //OutputDebugStringA("browserTabHeader1 != null"); 
                            cbtnbstopload.Visible = true;
                            cbtnbrefresh.Visible = false;

                            if (browserTabHeader2.thMouseDownPublic != null)
                            {
                                //OutputDebugStringA("browserTabHeader1.thMouseDown");
                                browserTabHeader2.thMouseDownPublic.PageState = TabHeader.WebPageState.Loading;
                                if (browserTabHeader2.thMouseDownPublic.ClientName.Length > 0)
                                {
                                    if (_remotingObject != null)
                                    {
                                        string strNewUrl = string.Empty;
                                        strNewUrl = string.Format("https://www.baidu.com/s?wd={0}", cboxAddressList.Text.ToString());
                                        Win32API.OutputDebugStringA(string.Format("发送命令 /cmd:loadurl 到客户端：{0}", browserTabHeader2.thMouseDownPublic.ClientName));
                                        //_remotingObject.ToClient("/cmd:goback", browserTabHeader2.thMouseDownPublic.ClientName );
                                        SendToClient(string.Format("/cmd:loadurl,{0}", strNewUrl), browserTabHeader2.thMouseDownPublic.ClientName);
                                        //(tmpControl as MiniBlinkPinvoke.BlinkBrowser).GoBack();
                                    }
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

        private void cboxAddressList_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                //OutputDebugStringA(string.Format("弹出下拉框之前的网址:{0}", inputurl));
                if ((string.IsNullOrEmpty(inputurl) || inputurl.Length <= 0))
                {
                    //Win32API.OutputDebugStringA("(string.IsNullOrEmpty(inputurl) || inputurl.Length <= 0)");  
                    return;
                }

                // 如果inputurl不是清除命令，则访问此地址
                if (browserTabHeader2 != null)
                {
                    //OutputDebugStringA("browserTabHeader1 != null"); 
                    if (browserTabHeader2.thMouseDownPublic != null)
                    {
                        cbtnbstopload.Visible = true;
                        cbtnbrefresh.Visible = false;
                        //OutputDebugStringA("browserTabHeader1.thMouseDown");
                        browserTabHeader2.thMouseDownPublic.PageState = TabHeader.WebPageState.Loading;
                        if (browserTabHeader2.thMouseDownPublic.ClientName.Length > 0)
                        {
                            if (_remotingObject != null)
                            {
                                Win32API.OutputDebugStringA(string.Format("发送命令 /cmd:loadurl 到客户端：{0}", browserTabHeader2.thMouseDownPublic.ClientName));
                                //_remotingObject.ToClient("/cmd:goback", browserTabHeader2.thMouseDownPublic.ClientName );
                                SendToClient(string.Format("/cmd:loadurl,{0}", inputurl), browserTabHeader2.thMouseDownPublic.ClientName);
                                //(tmpControl as MiniBlinkPinvoke.BlinkBrowser).GoBack();

                            }
                        }
                    }
                }
                inputurl = "";
            }
            catch (Exception ex)
            {
            }
        }

        private void cbtnbundo_Click(object sender, EventArgs e)
        {
            try
            {
                FormTestAddressBox testAddressBox = new FormTestAddressBox();
                testAddressBox.ShowDialog();
                testAddressBox.Dispose();
                testAddressBox = null;
            }
            catch(Exception ex)
            {

            }
        }

        private void cboxAddressList_KeyPress(object sender, KeyPressEventArgs e)
        {
            //do something
        }

        private void FormMain_Activated(object sender, EventArgs e)
        {
            //do something
            IsActivated = true;
        }

        private void FormMain_Deactivate(object sender, EventArgs e)
        {
            //
            IsActivated = false;
        }

        /// <summary>
        /// 屏蔽客户端的Alt+F4关闭窗口功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            //
            //Win32API.OutputDebugStringA(string.Format("客户端 {0} 操作键盘！", PublicModule.channelclientname));
            if (PublicModule.appmode.Equals("Client"))
            {
                if (e.KeyCode == Keys.F4 && e.Modifiers == Keys.Alt)
                {
                    KeyPreview = true; //如果不加此行，按Alt+F4两次会关闭程序，屏蔽失效。
                    e.Handled = true;
                    Win32API.OutputDebugStringA(string.Format("客户端 {0} 屏蔽Alt+F4功能键！", PublicModule.channelclientname));
                }
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //do something
                PublicModule.bolAppClosing = true;
                if(TaskDownWindow != null)
                {
                    TaskDownWindow.Close();
                    TaskDownWindow.Dispose();
                    TaskDownWindow = null;
                }
            }
            catch (Exception ex)
            {

            }
        }

    }
}
