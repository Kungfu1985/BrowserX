using BrowserX.UC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


public sealed class Win32API
{

    //结构体
    #region "结构体"
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        public IntPtr lpData;
    }
    #endregion

    #region "常量表"
    
    //wMsg参数常量值：
    //WM_KEYDOWN 按下一个键
    public static int WM_KEYDOWN = 0x0100;
    public static int GWL_STYLE = -16;

    public static long WS_CATION = 0xC00000;
    public static long WS_CATION_2 = 0xC0000;
    //释放一个键
    public static int WM_KEYUP = 0x0101;
    //按下某键，并已发出WM_KEYDOWN， WM_KEYUP消息
    public static int WM_CHAR = 0x102;
    //当用translatemessage函数翻译WM_KEYUP消息时发送此消息给拥有焦点的窗口
    public static int WM_DEADCHAR = 0x103;
    //当用户按住ALT键同时按下其它键时提交此消息给拥有焦点的窗口
    public static int WM_SYSKEYDOWN = 0x104;
    //当用户释放一个键同时ALT 键还按着时提交此消息给拥有焦点的窗口
    public static int WM_SYSKEYUP = 0x105;
    //当WM_SYSKEYDOWN消息被TRANSLATEMESSAGE函数翻译后提交此消息给拥有焦点的窗口
    public static int WM_SYSCHAR = 0x106;
    //当WM_SYSKEYDOWN消息被TRANSLATEMESSAGE函数翻译后发送此消息给拥有焦点的窗口
    public static int WM_SYSDEADCHAR = 0x107;
    //在一个对话框程序被显示前发送此消息给它，通常用此消息初始化控件和执行其它任务
    public static int WM_INITDIALOG = 0x110;
    //当用户选择一条菜单命令项或当某个控件发送一条消息给它的父窗口，一个快捷键被翻译
    public static int WM_COMMAND = 0x111;
    //当用户选择窗口菜单的一条命令或//当用户选择最大化或最小化时那个窗口会收到此消息
    public static int WM_SYSCOMMAND = 0x112;
    //发生了定时器事件
    public static int WM_TIMER = 0x113;
    //当一个窗口标准水平滚动条产生一个滚动事件时发送此消息给那个窗口，也发送给拥有它的控件
    public static int WM_HSCROLL = 0x114;
    //当一个窗口标准垂直滚动条产生一个滚动事件时发送此消息给那个窗口也，发送给拥有它的控件
    public static int WM_VSCROLL = 0x115;
    //当一个菜单将要被激活时发送此消息，它发生在用户菜单条中的某项或按下某个菜单键，它允许程序在显示前更改菜单
    public static int WM_INITMENU = 0x116;
    //当一个下拉菜单或子菜单将要被激活时发送此消息，它允许程序在它显示前更改菜单，而不要改变全部
    public static int WM_INITMENUPOPUP = 0x117;
    //当用户选择一条菜单项时发送此消息给菜单的所有者（一般是窗口）
    public static int WM_MENUSELECT = 0x11F;
    //当菜单已被激活用户按下了某个键（不同于加速键），发送此消息给菜单的所有者
    public static int WM_MENUCHAR = 0x120;
    //当一个模态对话框或菜单进入空载状态时发送此消息给它的所有者，一个模态对话框或菜单进入空载状态就是在处理完一条或几条先前的消息后没有消息它的列队中等待
    public static int WM_ENTERIDLE = 0x121;
    //在windows绘制消息框前发送此消息给消息框的所有者窗口，通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置消息框的文本和背景颜色
    public static int WM_CTLCOLORMSGBOX = 0x132;
    //当一个编辑型控件将要被绘制时发送此消息给它的父窗口通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置编辑框的文本和背景颜色
    public static int WM_CTLCOLOREDIT = 0x133;

    //当一个列表框控件将要被绘制前发送此消息给它的父窗口通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置列表框的文本和背景颜色
    public static int WM_CTLCOLORLISTBOX = 0x134;
    //当一个按钮控件将要被绘制时发送此消息给它的父窗口通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置按纽的文本和背景颜色
    public static int WM_CTLCOLORBTN = 0x135;
    //当一个对话框控件将要被绘制前发送此消息给它的父窗口通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置对话框的文本背景颜色
    public static int WM_CTLCOLORDLG = 0x136;
    //当一个滚动条控件将要被绘制时发送此消息给它的父窗口通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置滚动条的背景颜色
    public static int WM_CTLCOLORSCROLLBAR = 0x137;
    //当一个静态控件将要被绘制时发送此消息给它的父窗口通过响应这条消息，所有者窗口可以 通过使用给定的相关显示设备的句柄来设置静态控件的文本和背景颜色
    public static int WM_CTLCOLORSTATIC = 0x138;
    //当鼠标轮子转动时发送此消息个当前有焦点的控件
    public static int WM_MOUSEWHEEL = 0x20A;
    //双击鼠标中键
    public static int WM_MBUTTONDBLCLK = 0x209;
    //释放鼠标中键
    public static int WM_MBUTTONUP = 0x208;
    //移动鼠标时发生，同WM_MOUSEFIRST
    public static int WM_MOUSEMOVE = 0x200;
    //按下鼠标左键
    public static int WM_LBUTTONDOWN = 0x201;
    //释放鼠标左键
    public static int WM_LBUTTONUP = 0x202;
    //双击鼠标左键
    public static int WM_LBUTTONDBLCLK = 0x203;
    //按下鼠标右键
    public static int WM_RBUTTONDOWN = 0x204;
    //释放鼠标右键
    public static int WM_RBUTTONUP = 0x205;
    //双击鼠标右键
    public static int WM_RBUTTONDBLCLK = 0x206;
    //按下鼠标中键
    public static int WM_MBUTTONDOWN = 0x207;

    public static int WM_USER = 0x0400;
    public static int MK_LBUTTON = 0x0001;
    public static int MK_RBUTTON = 0x0002;
    public static int MK_SHIFT = 0x0004;
    public static int MK_CONTROL = 0x0008;
    public static int MK_MBUTTON = 0x0010;
    public static int MK_XBUTTON1 = 0x0020;
    public static int MK_XBUTTON2 = 0x0040;
    //创建一个窗口
    public static int WM_CREATE = 0x01;
    //当一个窗口被破坏时发送
    public static int WM_DESTROY = 0x02;
    //移动一个窗口
    public static int WM_MOVE = 0x03;
    //改变一个窗口的大小
    public static int WM_SIZE = 0x05;
    //一个窗口被激活或失去激活状态
    public static int WM_ACTIVATE = 0x06;
    //一个窗口获得焦点
    public static int WM_SETFOCUS = 0x07;
    //一个窗口失去焦点
    public static int WM_KILLFOCUS = 0x08;
    //一个窗口改变成Enable状态
    public static int WM_ENABLE = 0x0A;
    //设置窗口是否能重画
    public static int WM_SETREDRAW = 0x0B;
    //应用程序发送此消息来设置一个窗口的文本
    public static int WM_SETTEXT = 0x0C;
    //应用程序发送此消息来复制对应窗口的文本到缓冲区
    public static int WM_GETTEXT = 0x0D;
    //得到与一个窗口有关的文本的长度（不包含空字符）
    public static int WM_GETTEXTLENGTH = 0x0E;
    //要求一个窗口重画自己
    public static int WM_PAINT = 0x0F;
    //当一个窗口或应用程序要关闭时发送一个信号
    public static int WM_CLOSE = 0x10;
    //当用户选择结束对话框或程序自己调用ExitWindows函数
    public static int WM_QUERYENDSESSION = 0x11;
    //用来结束程序运行
    public static int WM_QUIT = 0x12;
    //当用户窗口恢复以前的大小位置时，把此消息发送给某个图标
    public static int WM_QUERYOPEN = 0x13;
    //当窗口背景必须被擦除时（例在窗口改变大小时）
    public static int WM_ERASEBKGND = 0x14;
    //当系统颜色改变时，发送此消息给所有顶级窗口
    public static int WM_SYSCOLORCHANGE = 0x15;
    //当系统进程发出WM_QUERYENDSESSION消息后，此消息发送给应用程序，通知它对话是否结束
    public static int WM_ENDSESSION = 0x16;
    //当隐藏或显示窗口是发送此消息给这个窗口
    public static int WM_SHOWWINDOW = 0x18;
    //发此消息给应用程序哪个窗口是激活的，哪个是非激活的
    public static int WM_ACTIVATEAPP = 0x1C;
    //当系统的字体资源库变化时发送此消息给所有顶级窗口
    public static int WM_FONTCHANGE = 0x1D;
    //当系统的时间变化时发送此消息给所有顶级窗口
    public static int WM_TIMECHANGE = 0x1E;
    //发送此消息来取消某种正在进行的摸态（操作）
    public static int WM_CANCELMODE = 0x1F;
    //如果鼠标引起光标在某个窗口中移动且鼠标输入没有被捕获时，就发消息给某个窗口
    public static int WM_SETCURSOR = 0x20;
    //当光标在某个非激活的窗口中而用户正按着鼠标的某个键发送此消息给//当前窗口
    public static int WM_MOUSEACTIVATE = 0x21;
    //发送此消息给MDI子窗口//当用户点击此窗口的标题栏，或//当窗口被激活，移动，改变大小
    public static int WM_CHILDACTIVATE = 0x22;
    //此消息由基于计算机的训练程序发送，通过WH_JOURNALPALYBACK的hook程序分离出用户输入消息
    public static int WM_QUEUESYNC = 0x23;
    //此消息发送给窗口当它将要改变大小或位置
    public static int WM_GETMINMAXINFO = 0x24;
    //发送给最小化窗口当它图标将要被重画
    public static int WM_PAINTICON = 0x26;
    //此消息发送给某个最小化窗口，仅//当它在画图标前它的背景必须被重画
    public static int WM_ICONERASEBKGND = 0x27;
    //发送此消息给一个对话框程序去更改焦点位置
    public static int WM_NEXTDLGCTL = 0x28;
    //每当打印管理列队增加或减少一条作业时发出此消息 
    public static int WM_SPOOLERSTATUS = 0x2A;
    //当button，combobox，listbox，menu的可视外观改变时发送
    public static int WM_DRAWITEM = 0x2B;
    //当button, combo box, list box, list view control, or menu item 被创建时
    public static int WM_MEASUREITEM = 0x2C;
    //此消息有一个LBS_WANTKEYBOARDINPUT风格的发出给它的所有者来响应WM_KEYDOWN消息 
    public static int WM_VKEYTOITEM = 0x2E;
    //此消息由一个LBS_WANTKEYBOARDINPUT风格的列表框发送给他的所有者来响应WM_CHAR消息 
    public static int WM_CHARTOITEM = 0x2F;
    //当绘制文本时程序发送此消息得到控件要用的颜色
    public static int WM_SETFONT = 0x30;
    //应用程序发送此消息得到当前控件绘制文本的字体
    public static int WM_GETFONT = 0x31;
    //应用程序发送此消息让一个窗口与一个热键相关连 
    public static int WM_SETHOTKEY = 0x32;
    //应用程序发送此消息来判断热键与某个窗口是否有关联
    public static int WM_GETHOTKEY = 0x33;
    //此消息发送给最小化窗口，当此窗口将要被拖放而它的类中没有定义图标，应用程序能返回一个图标或光标的句柄，当用户拖放图标时系统显示这个图标或光标
    public static int WM_QUERYDRAGICON = 0x37;
    //发送此消息来判定combobox或listbox新增加的项的相对位置
    public static int WM_COMPAREITEM = 0x39;
    //显示内存已经很少了
    public static int WM_COMPACTING = 0x41;
    //发送此消息给那个窗口的大小和位置将要被改变时，来调用setwindowpos函数或其它窗口管理函数
    public static int WM_WINDOWPOSCHANGING = 0x46;
    //发送此消息给那个窗口的大小和位置已经被改变时，来调用setwindowpos函数或其它窗口管理函数
    public static int WM_WINDOWPOSCHANGED = 0x47;
    //当系统将要进入暂停状态时发送此消息
    public static int WM_POWER = 0x48;
    //当一个应用程序传递数据给另一个应用程序时发送此消息
    public static int WM_COPYDATA = 0x4A;
    //当某个用户取消程序日志激活状态，提交此消息给程序
    public static int WM_CANCELJOURNA = 0x4B;
    //当某个控件的某个事件已经发生或这个控件需要得到一些信息时，发送此消息给它的父窗口 
    public static int WM_NOTIFY = 0x4E;
    //当用户选择某种输入语言，或输入语言的热键改变
    public static int WM_INPUTLANGCHANGEREQUEST = 0x50;
    //当平台现场已经被改变后发送此消息给受影响的最顶级窗口
    public static int WM_INPUTLANGCHANGE = 0x51;
    //当程序已经初始化windows帮助例程时发送此消息给应用程序
    public static int WM_TCARD = 0x52;
    //此消息显示用户按下了F1，如果某个菜单是激活的，就发送此消息个此窗口关联的菜单，否则就发送给有焦点的窗口，如果//当前都没有焦点，就把此消息发送给//当前激活的窗口
    public static int WM_HELP = 0x53;
    //当用户已经登入或退出后发送此消息给所有的窗口，//当用户登入或退出时系统更新用户的具体设置信息，在用户更新设置时系统马上发送此消息
    public static int WM_USERCHANGED = 0x54;
    //公用控件，自定义控件和他们的父窗口通过此消息来判断控件是使用ANSI还是UNICODE结构
    public static int WM_NOTIFYFORMAT = 0x55;
    //当用户某个窗口中点击了一下右键就发送此消息给这个窗口
    //public static int WM_CONTEXTMENU = ??;
    //当调用SETWINDOWLONG函数将要改变一个或多个 窗口的风格时发送此消息给那个窗口
    public static int WM_STYLECHANGING = 0x7C;
    //当调用SETWINDOWLONG函数一个或多个 窗口的风格后发送此消息给那个窗口
    public static int WM_STYLECHANGED = 0x7D;
    //当显示器的分辨率改变后发送此消息给所有的窗口
    public static int WM_DISPLAYCHANGE = 0x7E;
    //此消息发送给某个窗口来返回与某个窗口有关连的大图标或小图标的句柄
    public static int WM_GETICON = 0x7F;
    //程序发送此消息让一个新的大图标或小图标与某个窗口关联
    public static int WM_SETICON = 0x80;
    //当某个窗口第一次被创建时，此消息在WM_CREATE消息发送前发送
    public static int WM_NCCREATE = 0x81;
    //此消息通知某个窗口，非客户区正在销毁 
    public static int WM_NCDESTROY = 0x82;
    //当某个窗口的客户区域必须被核算时发送此消息
    public static int WM_NCCALCSIZE = 0x83;
    //移动鼠标，按住或释放鼠标时发生
    public static int WM_NCHITTEST = 0x84;
    //程序发送此消息给某个窗口当它（窗口）的框架必须被绘制时
    public static int WM_NCPAINT = 0x85;
    //此消息发送给某个窗口仅当它的非客户区需要被改变来显示是激活还是非激活状态
    public static int WM_NCACTIVATE = 0x86;
    //发送此消息给某个与对话框程序关联的控件，widdows控制方位键和TAB键使输入进入此控件通过应
    public static int WM_GETDLGCODE = 0x87;
    //当光标在一个窗口的非客户区内移动时发送此消息给这个窗口 非客户区为：窗体的标题栏及窗 的边框体
    public static int WM_NCMOUSEMOVE = 0xA0;
    //当光标在一个窗口的非客户区同时按下鼠标左键时提交此消息
    public static int WM_NCLBUTTONDOWN = 0xA1;
    //当用户释放鼠标左键同时光标某个窗口在非客户区十发送此消息 
    public static int WM_NCLBUTTONUP = 0xA2;
    //当用户双击鼠标左键同时光标某个窗口在非客户区十发送此消息
    public static int WM_NCLBUTTONDBLCLK = 0xA3;
    //当用户按下鼠标右键同时光标又在窗口的非客户区时发送此消息
    public static int WM_NCRBUTTONDOWN = 0xA4;
    //当用户释放鼠标右键同时光标又在窗口的非客户区时发送此消息
    public static int WM_NCRBUTTONUP = 0xA5;
    //当用户双击鼠标右键同时光标某个窗口在非客户区十发送此消息
    public static int WM_NCRBUTTONDBLCLK = 0xA6;
    //当用户按下鼠标中键同时光标又在窗口的非客户区时发送此消息
    public static int WM_NCMBUTTONDOWN = 0xA7;
    //当用户释放鼠标中键同时光标又在窗口的非客户区时发送此消息
    public static int WM_NCMBUTTONUP = 0xA8;
    //当用户双击鼠标中键同时光标又在窗口的非客户区时发送此消息
    public static int WM_NCMBUTTONDBLCLK = 0xA9;

    public const int SC_CLOSE = 0xF060;
    public const int MF_ENABLED = 0x00000000;
    public const int MF_GRAYED = 0x00000001;
    public const int MF_DISABLED = 0x00000002;


    public static class SBArrows
    {
        public const uint ESB_ENABLE_BOTH = 0;
        public const uint ESB_DISABLE_BOTH = 3;
        public const uint ESB_DISABLE_LEFT = 1;
        public const uint ESB_DISABLE_RIGHT = 2;
        public const uint ESB_DISABLE_UP = 1;
        public const uint ESB_DISABLE_DOWN = 2;
        public const uint ESB_DISABLE_LTUP = 1;
        public const uint ESB_DISABLE_RTDN = 2;
    }

    public static class SBFlags
    {
        public const uint SB_HORZ = 0;
        public const uint SB_VERT = 1;
        public const uint SB_CTL = 2;
        public const uint SB_BOTH = 3;
    }

    #endregion

    #region "API"

    [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
    public static extern IntPtr GetSystemMenu(IntPtr hWnd, int bRevert);
    [DllImport("User32.dll")]
    public static extern bool EnableMenuItem(IntPtr hMenu, int uIDEnableItem, int uEnable);


    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, int lParam);

    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);


    [DllImport("user32", EntryPoint = "PostMessage")]
    public static extern int PostMessageA(int hwnd, int wMsg, int wParam, int lParam);

    [DllImport("user32", EntryPoint = "PostMessage")]
    public static extern int PostMessageA(IntPtr hwnd, int wMsg, System.Windows.Forms.Keys wParam, int lParam);


    [DllImport("user32", EntryPoint = "PostMessage")]
    public static extern int PostMessageA(IntPtr hwnd, int wMsg, int wParam, int lParam);

    [DllImport("user32", EntryPoint = "PostMessage")]
    public static extern int PostMessageA(IntPtr hwnd, int wMsg, int wParam, ref COPYDATASTRUCT lParam);

    //此处主要用来让窗口置于最前(SetWindowPos(this.Handle,-1,0,0,0,0,0x4000|0x0001|0x0002);)    
    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

    [DllImport("user32", EntryPoint = "FindWindow")]
    public static extern IntPtr FindWindowA(string lpClassName, string lpWindowName);

    [DllImport("user32")]
    public static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32")]
    public static extern int MoveWindow(IntPtr hwnd, int x, int y, int nWidth, int nHeight, int bRepaint);

    [DllImport("user32")]
    public static extern int GetClientRect(IntPtr hwnd, ref RECT lpRect);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern int GetLastError();

    [DllImport("user32", EntryPoint = "GetWindowLong")]
    public static extern int GetWindowLongA(int hwnd, int nIndex);


    [DllImport("user32", EntryPoint = "SetWindowLong")]
    public static extern int SetWindowLongA(int hwnd, int nIndex, int dwNewLong);

    #endregion

    #region "API函数声明"
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern void OutputDebugStringA(string lpOutputString);

    [DllImport("user32")]
    public static extern int EnableScrollBar(int hwnd, int wSBflags, int wArrows);

    [DllImport("user32")]
    public static extern int EnableScrollBar(IntPtr hwnd, int wSBflags, int wArrows);

    [DllImport("user32.dll")]
    public static extern bool EnableScrollBar(IntPtr hWnd, uint wSBflags, uint wArrows);



    [DllImport("user32")]
    public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

    [DllImport("user32")]
    public static extern int CloseWindow(IntPtr hwnd);

    [DllImport("user32")]
    public static extern int DestroyWindow(IntPtr hwnd);


    [DllImport("user32")]
    public static extern IntPtr GetDC(IntPtr hwnd);

    [System.Runtime.InteropServices.DllImport("user32.dll ")]
    public static extern IntPtr GetWindowDC(IntPtr hWnd);//返回hWnd参数所指定的窗口的设备环境。

    [System.Runtime.InteropServices.DllImport("user32.dll ")]
    public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC); //函数释放设备上下文环境（DC）

    [DllImport("gdi32")]
    public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

    [DllImport("gdi32")]
    public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

    [DllImport("user32.dll")]
    public static extern bool UpdateLayeredWindow(IntPtr hwnd, 
                                                  IntPtr hdcDst, 
                                                  ref Point pptDst, 
                                                  ref Size psize ,
                                                  IntPtr hdcSrc, 
                                                  ref Point pprSrc ,
                                                  Int32 crKey, 
                                                  ref _BLENDFUNCTION pblend, Int32 dwFlags);


[StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
        public int Width { get { return Right - Left; } }
        public int Height { get { return Bottom - Top; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct _BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }


    public const Int32 ULW_COLORKEY = 0x1;
    public const Int32 ULW_ALPHA = 0x2;
    public const Int32 ULW_OPAQUE = 0x4;
    public const byte AC_SRC_OVER = 0x0;
    public const byte AC_SRC_ALPHA = 0x1;

    public const int SWP_NOSIZE = 0x0001;
    public const int SWP_NOZORDER = 0x0004;
    public const int SWP_SHOWWINDOW = 0x0040;
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);


    //public const long WM_SETFONT = 0x30;

    [DllImport("user32.dll", EntryPoint = "SendMessageW")]
    public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", EntryPoint = "SendMessageA")]
    public static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);

    [DllImport("user32.dll")]
    public static extern bool GetComboBoxInfo(IntPtr hWnd, ref COMBOBOXINFO pcbi);

    [DllImport("gdi32")]
    public static extern int DeleteObject(int hObject);

    [DllImport("gdi32")]
    public static extern int DeleteObject(IntPtr hObject);

    [DllImport("gdi32")]
    public static extern int DeleteDC(IntPtr hDC);


    [StructLayout(LayoutKind.Sequential)]
    public struct COMBOBOXINFO
    {
        public int cbSize;
        public RECT rcItem;
        public RECT rcButton;
        public int stateButton;
        public IntPtr hwndCombo;
        public IntPtr hwndEdit;
        public IntPtr hwndList;
    }

    #endregion

}

public class PublicModule
{
    //是否双击了标签栏
    public static bool IsHeadDbClicked = false;

    /// <summary>
    /// 启动时自动添加的标签页
    /// </summary>
    public static string StartupUrl = string.Empty;

    //通知下载窗口关闭
    public static bool bolAppClosing = false;
    public static bool bolTaskWindowOpened = false;
    //下载参数
    public static string strDownloadUrl = String.Empty;
    public static string strDownFileName = String.Empty;
    public static string strDownloadFile = string.Empty;
    public static long strDownFileNameSize = 0;
    //下载全局变量
    public static bool bolDownFlag = false;
    public static string DownloadUrl = "";
    public static string DownSavePath = "";
    public static string DownSaveSize = "";
    public static string DownSaveFile = "";


    public enum DebugStatus
    {
        none,
        debug
    }

    /// <summary>
    /// 主页地址
    /// </summary>
    public static string HomePage = "fom.qq.com";

    public static DebugStatus _debugstatus = DebugStatus.none;

    public static void myOutputDebugStringA(DebugStatus status, string info1, string info2)
    {
        if(status == DebugStatus.debug)
        {
            Win32API.OutputDebugStringA( string.Format("{0},{1}",info1,info2) ); 
        }
    }

    
    /// <summary>
    /// 将Image转换为Base64字符串
    /// </summary>
    /// <param name="img"></param>
    /// <returns></returns>
    public static String ImageToString(Image img)
    {
        byte[] byteArray = new byte[0];
        using (MemoryStream stream = new MemoryStream())
        {
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Close();
            byteArray = stream.ToArray();
        }
        return Convert.ToBase64String(byteArray);
    }


    /// <summary>
    /// 将Base64编码转换为Bitmap
    /// </summary>
    /// <param name="imageString"></param>
    /// <returns></returns>
    public static Image StringToImage(string imageString)
    {
        if (imageString == null) {
            throw new ArgumentNullException("imageString");
        }
        byte[] array = Convert.FromBase64String(imageString);
        Bitmap image = (Bitmap)Bitmap.FromStream(new MemoryStream(array));
        return image;
    }

    /// <summary>
    /// 创建WPF控件的图片源
    /// </summary>
    /// <param name="img"></param>
    /// <returns></returns>
    public static System.Windows.Media.ImageSource MakeImageSource(Bitmap img)
    {
        try
        {
            IntPtr hBitmap = img.GetHbitmap();
            if ((hBitmap == IntPtr.Zero))
                return null;
            System.Windows.Media.ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
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


    /// <summary>
    /// 计算磁盘剩余空间
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetHardDiskSpace(string path)
    {
        double totalSize = 0.0D;
        string tmpdiskname = string.Empty;
        try
        {
            string tmppath = path.ToUpper();
            if ((tmppath.IndexOf(":") != 1))
                return (string.Format("{0} GB", totalSize.ToString("0.0")));
            else
                tmpdiskname = tmppath.Substring(0, 1);

            tmpdiskname = tmpdiskname + @":\";
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            // 遍历系统中所有的磁盘分区，按名称匹配
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.Name == tmpdiskname)
                    // 换算为GB单位 总字节/1024(KB) 总字节/1024/1024(MB)
                    totalSize = System.Math.Round(drive.TotalFreeSpace / (double)(1024.0F * 1024.0F * 1024.0F), 2);
            }
            return (string.Format("{0} GB", totalSize.ToString("0.0")));
        }
        catch (Exception ex)
        {
        }
        return (string.Format("{0} GB", totalSize.ToString("0.0")));
    }


    /// <summary>
    /// 获取网络文件大小
    /// </summary>
    /// <param name="url"></param>
    public static string GetWebFileSize(string url)
    {
        string result = string.Empty;
        long ContentL = 0;
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.MaximumAutomaticRedirections = 4;
            request.MaximumResponseHeadersLength = 4;
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            ContentL = response.ContentLength;
            response.Close();
            response.Dispose();
            response = null;
            request = null;
            // response.Server
            result = string.Format("大小: {0}", GetSize(ContentL));
        }
        catch (Exception ex)
        {
            result = string.Format("大小: {0}", "未知");
        }
        return (result);
    }


    /// <summary>
    /// 文件大小换算
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static string GetSize(long L)
    {
        float result;
        string re = "";

        if (L >= 1073741824)
        {
            result = L / 1.07374182E+9F;
            re = "GB";
        }
        else if (L >= 1048576)
        {
            result = L / 1048576.0F;
            re = "MB";
        }
        else
        {
            result = L / 1024.0F;
            re = "KB";
        }

        string File_Size = result.ToString("0.00");
        return (File_Size + re);
    }


    /// <summary>
    /// 子进程发送过来的新建标签URL
    /// </summary>
    public static string NewTabUrl = string.Empty;

    public static string appmode = string.Empty;
    /// <summary>
    /// IPC服务的名称
    /// </summary>
    public static string channelserver = string.Empty;
    //主窗口的句柄
    public static IntPtr parent = IntPtr.Zero;
    /// <summary>
    /// IPC服务的端口名称
    /// </summary>
    public static string channelserverport = string.Empty;
    /// <summary>
    /// IPC客户端的用户名
    /// </summary>
    public static string channelclientname = string.Empty;
    public static int channelclientcount = 1;
    public static string channelclienturl = string.Empty;
    /// <summary>
    /// 生成一个新的客户端名称
    /// </summary>
    /// <returns></returns>
    public static string GetNewClientName()
    {
        string result = string.Format(string.Format("BrowserX{0}", PublicModule.channelclientcount++));
        return (result);
    }
    /// <summary>
    /// IPC客户端的收到的命令
    /// </summary>
    public static string channelclientcmd = string.Empty;


}