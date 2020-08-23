
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MiniBlink;
using System.Dynamic;

namespace MiniBlinkPinvoke
{
    public class BlinkBrowser : Control, IMessageFilter
    {

        //Timer timer = new Timer();
        #region "变量"
        string CookiePath { get; set; }
        public IntPtr handle = IntPtr.Zero;
        string url = string.Empty;
        public string title = string.Empty;
        public bool isloading = true;

        string overurl = string.Empty;
        string popurl = string.Empty;
        static bool ViewSourceOnlyUrl = true;
        static bool IsEnableContextMenu = true;
        //是否在获得焦点时触发事件
        static bool EnabledFocusEvent = true;

        IntPtr bits = IntPtr.Zero;
        IntPtr snapbits = IntPtr.Zero;
        static IntPtr jsHandle = IntPtr.Zero;
        private IntPtr pbHandle = IntPtr.Zero;
        private IntPtr pcHandle = IntPtr.Zero;

        Size oldSize;
        public object GlobalObjectJs = null;
        public object GlobalObjectJsNet = null;
        #endregion

        #region 事件
        //标题改变事件回调
        public delegate void TitleChange(string title);
        public event TitleChange OnTitleChangeCall;

        public System.Net.WebClient mywebClient;

        //鼠标滑过URL事件回调
        public delegate void MouseOverUrlChanged(string url);
        public event MouseOverUrlChanged OnMouseOverUrlChanged;

        //自身获得焦点时触发事件回调
        public delegate void FocusedChanged(object info );
        public event FocusedChanged OnFocusedChanged;


        //URL改变事件
        UrlChangedCallback urlChangedCallback;
        public delegate void URLChange(string url);
        public event URLChange OnUrlChangeCall;

        UrlChangedCallback2 urlChangedCallback2;
        public delegate void URLChange2(string url);
        public event URLChange2 OnUrlChange2Call;

        public delegate void URLChange3(string url, IntPtr frameid);
        public event URLChange3 OnUrlChange3Call;


        //导航事件
        public delegate void OnUrlNavigation(string url);
        public event OnUrlNavigation OnUrlNavigationCall;

        //DOM加载事件
        public delegate void DocumentReady();
        public event DocumentReady DocumentReadyCallback;

        public delegate void DocumentReady2(IntPtr webView, IntPtr param, IntPtr frameId, bool mainframe);
        public event DocumentReady2 DocumentReady2Callback;


        //链接点击后创建View事件
        public delegate IntPtr OnCreateViewCallback(IntPtr webView, IntPtr param, wkeNavigationType navigationType, string url);
        public event OnCreateViewCallback OnCreateViewEvent;

        //在新标签页中打开链接事件
        public delegate void OnCreateNewTabCallback(IntPtr webView, IntPtr param, string url);
        public event OnCreateNewTabCallback OnCreateNewTabEvent;

        //注册View初始化回调，通知调用方初始化Net函数
        public delegate void OnViewInitCallback(IntPtr webView);
        public event OnViewInitCallback OnViewInitEvent;

        //URL end 回调通知
        public delegate void OnUrlEnd(byte[] bytes, string url, int len, int frameid);
        public event OnUrlEnd OnUrlEndEvent;

        //下载事件回调通知
        public delegate void OnDownload(string url);
        public event OnDownload OnDownloadFile;

        //net提供回调接口，返回数据
        public delegate void OnDownload2(IntPtr webView, string url, string mime, string disposition, string filename, int len);
        public event OnDownload2 OnDownloadFile2;


        //获取图标回调通知，url，图标地址，bytes图标数据，len图标数据长度
        public delegate void OnNetGetFavicon(IntPtr webView, string url, byte[] bytes, int len);
        public event OnNetGetFavicon OnNetGetFaviconEvent;

        //获取网页源码回调
        public delegate void OnNetGetSource(IntPtr webView, string urlsourcecode);
        public event OnNetGetSource OnNetGetSourceEvent;

        public delegate void OnNetGetSourceOnlyUrl(IntPtr webView, string url);
        public event OnNetGetSourceOnlyUrl OnNetGetSourceOnlyUrlEvent;


        //绑定函数列表
        List<object> listObj = new List<object>();
        //回调函数声明
        wkeOnShowDevtoolsCallback _wkeOnShowDevtoolsCallback;
        AlertBoxCallback AlertBoxCallback;
        TitleChangedCallback titleChangeCallback;
        TitleChangedCallback titleChangeCallback2;
        wkeNavigationCallback _wkeNavigationCallback;
        wkeConsoleMessageCallback _wkeConsoleMessageCallback;
        wkePaintUpdatedCallback _wkePaintUpdatedCallback;
        wkeDocumentReadyCallback _wkeDocumentReadyCallback;
        wkeDocumentReady2Callback _wkeDocumentReady2Callback;
        wkeLoadingFinishCallback _wkeLoadingFinishCallback;
        wkeDownloadCallback _wkeDownloadCallback;
        wkeDownload2Callback _wkeDownload2Callback;
        wkeCreateViewCallback _wkeCreateViewCallback;
        wkeLoadUrlBeginCallback _wkeLoadUrlBeginCallback;
        wkeLoadUrlEndCallback _wkeLoadUrlEndCallback;
        wkeOnNetGetFavicon _wkeOnNetGetFavicon;
        wkeGetTempCallbackInfo _wkeGetTempCallbackInfo;
        wkeNetJobDataRecvCallback _wkeNetJobDataRecvCallback;
        wkeNetJobDataFinishCallback _wkeNetJobDataFinishCallback;
        #endregion


        void OnwkeNetGetFavicon(IntPtr webView, IntPtr param, IntPtr url, IntPtr buf)
        {
            try
            {
                //在这里读取网页上的图标,触发回调函数
                try
                {
                    if (OnNetGetFaviconEvent != null)
                    {
                        string strurl = string.Empty;
                        if (buf != IntPtr.Zero)
                        {
                            strurl = BlinkBrowserPInvoke.Utf8IntptrToString(url);
                            //string str = BitConverter.ToString(url);
                            wkeMemBuf bufs = new wkeMemBuf();
                            bufs = (wkeMemBuf)Marshal.PtrToStructure(buf, typeof(wkeMemBuf));
                            if (bufs.length <= 0)
                            {
                                return;
                            }
                            try
                            {
                                byte[] managedArray = new byte[bufs.length];
                                Marshal.Copy(bufs.data, managedArray, 0, bufs.length);
                                OnNetGetFaviconEvent(webView, strurl, managedArray, bufs.length);
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show( string.Format("Marshal.Copy got error:{0},byte len:{1}", ex.Message,len));
                                //throw;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    //throw;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        void OnwkeLoadUrlEndCallback(IntPtr webView, IntPtr param, string url, IntPtr job, IntPtr buf, int len)
        {
            try
            {
                if (OnUrlEndEvent != null)
                {
                    if (len <= 0) { return; }
                    try
                    {
                        byte[] managedArray = new byte[len];
                        int frameid = 0;
                        Marshal.Copy(buf, managedArray, 0, len);
                        //OnUrlEndEvent(managedArray, url, len, frameid);
                        //获取frameid
                        try
                        {
                            wkeTempCallbackInfo temInfo = new wkeTempCallbackInfo();
                            IntPtr tmpBackInfo = BlinkBrowserPInvoke.wkeGetTempCallbackInfo(webView);
                            if (tmpBackInfo != IntPtr.Zero)
                            {
                                temInfo = (wkeTempCallbackInfo)Marshal.PtrToStructure(tmpBackInfo, typeof(wkeTempCallbackInfo));
                                if (temInfo.wkeWebFrameHandle != IntPtr.Zero) {
                                    if (BlinkBrowserPInvoke.wkeIsMainFrame(webView, temInfo.wkeWebFrameHandle)) {
                                        frameid = 1;
                                    }
                                    else {
                                        frameid = 0;
                                    }
                                }
                            }
                            else {
                                frameid = 0;
                            }
                            //Marshal.FreeCoTaskMem(tmpBackInfo);
                            //Marshal.FreeHGlobal(tmpBackInfo);                    
                        }
                        catch (Exception ex)
                        {
                            frameid = 0;
                        }
                        //Url加载结束 回调，可以知道加载了哪些文件
                        //frameid=1主frame的UrlEnd,0是子frame 
                        //MessageBox.Show("call wkeGetTempCallbackInfo Error: " + frameid.ToString() );
                        OnUrlEndEvent(managedArray, url, len, frameid);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("call OnUrlEndEvent error:" + ex.Message); ;
                        //MessageBox.Show( string.Format("Marshal.Copy got error:{0},byte len:{1}", ex.Message,len));
                        //throw;
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("call OnwkeLoadUrlEndCallback Error: " + ex.Message);
                //throw;
            }
            Console.WriteLine("call OnwkeLoadUrlEndCallback url:" + url); ;
        }


        /// <summary>
        /// 这个方法里可HOOK所有资源。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="param"></param>
        /// <param name="url"></param>
        /// <param name="job"></param>
        /// <returns></returns>
        bool OnwkeLoadUrlBeginCallback(IntPtr webView, IntPtr param, string url, IntPtr job)
        {
            //mb://index.html/js/index.js
            if (url.StartsWith("mb://"))
            {
                Regex regex = new Regex(@"mb://", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
                string str = regex.Replace(url, "");// url.TrimStart("mb://".ToArray());//.Substring(startIndex);
                                                    //if (url != Url)//加载本地资源
                                                    //{
                                                    //    str = url.TrimStart(Url.ToArray());
                                                    //}
                str = str.Replace('/', '.');
                MessageBox.Show(string.Format("OnwkeLoadUrlBeginCallback str:{0}", str));
                try
                {
                    System.Reflection.Assembly Assemblys = BlinkBrowserPInvoke.ResourceAssemblys["iWallpaper"];
                    if (Assemblys != null)
                    {
                        using (Stream sm = Assemblys.GetManifestResourceStream("iWallpaper." + str))
                        {
                            if (sm != null)
                            {
                                //MessageBox.Show(string.Format("sm != null"));
                                var bytes = StreamToBytes(sm);
                                if (url.EndsWith(".css"))
                                {
                                    BlinkBrowserPInvoke.wkeNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("text/css"));
                                }
                                else if (url.EndsWith(".png"))
                                {
                                    BlinkBrowserPInvoke.wkeNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("image/png"));
                                }
                                else if (url.EndsWith(".gif"))
                                {
                                    BlinkBrowserPInvoke.wkeNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("image/gif"));
                                }
                                else if (url.EndsWith(".jpg"))
                                {
                                    BlinkBrowserPInvoke.wkeNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("image/jpg"));
                                }
                                else if (url.EndsWith(".js"))
                                {
                                    BlinkBrowserPInvoke.wkeNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("application/javascript"));
                                }
                                else
                                {
                                    BlinkBrowserPInvoke.wkeNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("text/html"));
                                }
                                //wkeNetSetURL(job, url);
                                BlinkBrowserPInvoke.wkeNetSetData(job, bytes, bytes.Length);
                            }
                            else
                            {
                                //MessageBox.Show(string.Format("sm == null"));
                                ResNotFond(url, job);
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show(string.Format("OnwkeLoadUrlBeginCallback Assemblys is null"));
                        ResNotFond(url, job);
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(string.Format("Assemblys got error:{0}", ex.Message));
                    ResNotFond(url, job);
                }
                return true;
            }
            else
            {
                //如果需要 OnwkeLoadUrlEndCallback 回调，需要取消注释下面的 hook
                BlinkBrowserPInvoke.wkeNetHookRequest(job);
            }
            return false;
        }

        byte[] StreamToBytes(Stream stream)

        {

            byte[] bytes = new byte[stream.Length];

            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始 

            stream.Seek(0, SeekOrigin.Begin);

            return bytes;

        }

        private static void ResNotFond(string url, IntPtr job)
        {
            string data = "<html><head><title>404没有找到资源</title></head><body>404没有找到资源</body></html>";
            BlinkBrowserPInvoke.wkeNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("text/html"));
            //wkeNetSetURL(job, url);
            BlinkBrowserPInvoke.wkeNetSetData(job, Encoding.Default.GetBytes(data), Encoding.Default.GetBytes(data).Length);
        }

        IntPtr OnwkeCreateViewCallback(IntPtr webView, IntPtr param, wkeNavigationType navigationType, IntPtr url)
        {
            isloading = true;
            if (OnCreateViewEvent != null)
            {
                return OnCreateViewEvent(webView, param, navigationType, BlinkBrowserPInvoke.Utf8IntptrToString(BlinkBrowserPInvoke.wkeGetString(url)));
            }
            else
            {
                //Console.WriteLine("OnwkeCreateViewCallback url:" + BlinkBrowserPInvoke.Utf8IntptrToString(BlinkBrowserPInvoke.wkeGetString(url)));
                //Console.WriteLine("OnwkeCreateViewCallback navigationType:" + navigationType);
                return webView;
            }
        }

        //下载1回调
        bool OnwkeDownloadCallback(IntPtr webView, IntPtr param, string url)
        {
            //MessageBox.Show("OnwkeDownloadCallback");
            //Console.WriteLine("call OnwkeDownloadFileCallback:" + (url));
            //MessageBox.Show(string.Format("DownloadFile param：{0}，url：{1}", param, url));
            //if (OnDownloadFile != null)
            //{
            //    OnDownloadFile(url);
            //}
            return false;
        }


        //下载2开始接收数据
        void OnwkeNetJobDataRecvCallback(IntPtr param, IntPtr job, IntPtr data, int length)
        {
            //MessageBox.Show( string.Format("OnNetJobDataRecvCallback,接收到数据:{0}", length));
        }


        //下载2下载结束
        void OnwkeNetJobDataFinishCallback(IntPtr param, IntPtr job, wkeLoadingResult result)
        {
            //MessageBox.Show("完成了数据接收");
        }


        //下载2回调
        uint OnwkeDownload2Callback(IntPtr webView, IntPtr param, int expectedContentLength, IntPtr url, IntPtr mime, IntPtr disposition, IntPtr job, IntPtr dataBind)
        {
            //OnwkeDownload2Callback called
            //string strdisposition = BlinkBrowserPInvoke.Utf8IntptrToString(disposition);
            //string strurl = BlinkBrowserPInvoke.Utf8IntptrToString(url);
            //string strmime = BlinkBrowserPInvoke.Utf8IntptrToString(mime);
            //取消下载计划
            //return 0;
            try
            {
                string strdisposition = BlinkBrowserPInvoke.Utf8IntptrToString(disposition);
                string strurl = BlinkBrowserPInvoke.Utf8IntptrToString(url);
                string strmime = BlinkBrowserPInvoke.Utf8IntptrToString(mime);
                //string strresult = String.Format("OnwkeDownload2Callback result,url:{0}, mime:{1}, disposition:{2}，Length:{3}.dataBind:{4}", strurl, strmime, strdisposition, expectedContentLength, dataBind);
                //MessageBox.Show(strresult);

                ////将指针转换为结构，
                ////传过来的dataBind是个空结构体的指针
                //wkeNetJobDataBind tmpDataBind  = new wkeNetJobDataBind();

                ////填充结构体 Marshal.GetFunctionPointerForDelegate( _wkeNetJobDataRecvCallback);
                //tmpDataBind.recvCallback =  _wkeNetJobDataRecvCallback;
                //tmpDataBind.finishCallback =_wkeNetJobDataFinishCallback;

                ////将结构转换为指针
                //IntPtr tmpDataBindPoint = Marshal.AllocHGlobal(Marshal.SizeOf(tmpDataBind));

                ////fDeleteOld：设置为 true 可在执行Marshal.DestroyStructure方法前对 ptr 参数调用此方法。请注意，传递 false 可导致内存泄漏。
                //Marshal.StructureToPtr(tmpDataBind, tmpDataBindPoint, true);

                ////将指针赋值给Node内核
                //dataBind = tmpDataBindPoint;

                //返回0取消下载计划,将下载信息传回调用方
                try
                {
                    if (OnDownloadFile2 != null) {
                        string strfilename = string.Empty;
                        strfilename = GetFileName(strdisposition);
                        OnDownloadFile2(webView, strurl, strmime, strdisposition, strfilename, expectedContentLength);
                    }
                } catch (Exception ex) {

                }
                return 0;
            }
            catch (Exception ex)
            {
                //如果出错，取消下载计划
                return 0;
            }
        }

        string GetFileName(string disp)
        {
            string strresult = string.Empty;
            Regex re = new Regex("(?<=\").*?(?=\")", RegexOptions.IgnoreCase);
            MatchCollection mc = re.Matches(disp);
            if (mc.Count > 0) {
                foreach (Match ma in mc) {
                    strresult = ma.Value;
                    break;
                }
            }
            return strresult;
        }

        void OnwkeLoadingFinishCallback(IntPtr webView, IntPtr param, IntPtr url, wkeLoadingResult result, IntPtr failedReason)
        {
            //Console.WriteLine("call OnwkeLoadingFinishCallback:" + wkeGetString(url).Utf8IntptrToString());
            //Console.WriteLine("call OnwkeLoadingFinishCallback result:" + result);            
            if (result == wkeLoadingResult.WKE_LOADING_FAILED)
            {
                Console.WriteLine("call OnwkeLoadingFinishCallback 加载失败 failedReason:" + BlinkBrowserPInvoke.Utf8IntptrToString(BlinkBrowserPInvoke.wkeGetString(failedReason)));
                HTML = "<h1>" + BlinkBrowserPInvoke.Utf8IntptrToString(BlinkBrowserPInvoke.wkeGetString(failedReason)) + "</h1>";
            }
            else
            {
                try
                {
                    this.url = BlinkBrowserPInvoke.Utf8IntptrToString(BlinkBrowserPInvoke.wkeGetString(url));
                }
                catch (Exception ex)
                {
                    //MessageBox.Show( string.Format ("BlinkBrowserPInvoke.Utf8IntptrToString got error:{0}", ex.Message));
                }
                try
                {
                    //获取网页默认小图标回调
                    //BlinkBrowserPInvoke.wkeNetGetFavicon(webView, _wkeOnNetGetFavicon, IntPtr.Zero);
                    if (this.url.StartsWith("file:"))
                    {
                        if (OnNetGetFaviconEvent != null)
                        {
                            OnNetGetFaviconEvent(webView, string.Empty, null, 0);
                        }
                        return;
                    }
                    else
                    {
                        wkeTempCallbackInfo temInfo = new wkeTempCallbackInfo();
                        IntPtr tmpBackInfo = BlinkBrowserPInvoke.wkeGetTempCallbackInfo(webView);
                        if (tmpBackInfo != IntPtr.Zero)
                        {
                            temInfo = (wkeTempCallbackInfo)Marshal.PtrToStructure(tmpBackInfo, typeof(wkeTempCallbackInfo));
                            if (temInfo.size <= 0)
                            {
                                return;
                            }
                            try
                            {
                                if (BlinkBrowserPInvoke.wkeIsMainFrame(webView, temInfo.wkeWebFrameHandle))
                                {
                                    //MessageBox.Show(string.Format("temInfo.frame:{0}", temInfo.wkeWebFrameHandle));
                                    //触发OnNetGetFaviconEvent回调
                                    BlinkBrowserPInvoke.wkeNetGetFavicon(webView, _wkeOnNetGetFavicon, IntPtr.Zero);
                                }
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show(string.Format("BlinkBrowserPInvoke.wkeIsMainFrame got error:{0},byte len:{1}", ex.Message, temInfo.size));
                                //throw;
                            }
                        }
                        //Marshal.FreeHGlobal(tmpBackInfo);   
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(string.Format("wkeNetGetFavicon got error:{0}", ex.Message));
                }
                //Console.WriteLine("call OnwkeLoadingFinishCallback:成功加载完成。" + wkeGetString(url).Utf8IntptrToString());                
            }
        }


        void OnwkeDocumentReady2Callback(IntPtr webView, IntPtr param, IntPtr frameId)
        {
            bool bolmainframe = false;
            //加载完成,判断是否是主Frame
            try
            {
                if (BlinkBrowserPInvoke.wkeIsMainFrame(webView, frameId))
                {
                    bolmainframe = true;
                }
                else
                {
                    bolmainframe = false;
                }

            }
            catch (Exception ex)
            {
                BlinkBrowserPInvoke.OutputDebugStringA( string.Format("获取是否是主Frame时发生错误,Error:{0}",ex.Message)  );
            } 
            //获取主FrameID
            //if (BlinkBrowserPInvoke.wkeWebFrameGetMainFrame(webView) == frameId) {
            //    bolmainframe = true;
            //}
            if (DocumentReady2Callback != null) {
                isloading = false;
                DocumentReady2Callback(webView, param, frameId, bolmainframe);
            }
        }

        void OnwkeDocumentReadyCallback(IntPtr webView, IntPtr param)
        {
            //Console.WriteLine("call OnwkeDocumentReadyCallback:" + Marshal.PtrToStringUni(param));//.Utf8IntptrToString());
            //DocumentReadyCallback?.Invoke();
            if (DocumentReadyCallback != null) {
                DocumentReadyCallback();
            }
            return;
            //
            try
            {
                StringBuilder srcGetLinkText = new StringBuilder();
                srcGetLinkText.Clear();
                //srcGetLinkText.AppendLine("<script type=\"text/javascript\">");
                srcGetLinkText.AppendLine("function testfun1(url)");
                srcGetLinkText.AppendLine("{");
                srcGetLinkText.AppendLine("console.log(url);");
                srcGetLinkText.AppendLine("var elements = document.getElementsByTagName('a');");
                srcGetLinkText.AppendLine("var result =''");
                srcGetLinkText.AppendLine("for(var j=0;j<elements.length;j++)");
                srcGetLinkText.AppendLine("{");
                srcGetLinkText.AppendLine("if(elements[j].href==url){");
                srcGetLinkText.AppendLine("result = elements[j].text; ");
                srcGetLinkText.AppendLine("break;");
                srcGetLinkText.AppendLine("}");
                srcGetLinkText.AppendLine("}");
                srcGetLinkText.AppendLine("return result;");
                srcGetLinkText.AppendLine("}");
                //srcGetLinkText.AppendLine("</script>");
                //srcGetLinkText.AppendLine("function getLinkText(url){console.log(url);");
                //srcGetLinkText.Append("var elements = document.getElementsByTagName('a');var result ='';");
                //srcGetLinkText.Append("for(var j=0;j<elements.length;j++){");
                //srcGetLinkText.Append("if(elements[j].href==url){result = elements[j].text;break;}}return result;}");
                //插入获取链接的Text属性的JS语句
                String strJsCode = string.Empty;
                //
                //var x = document.createElement("SCRIPT");
                //var t = document.createTextNode("function test1(msg){alert(msg);}");
                //x.appendChild(t);
                //document.head.appendChild(x);
                //
                StringBuilder srcCreateElement = new StringBuilder();
                srcCreateElement.Clear();

                srcCreateElement.AppendLine("var scriptx = document.createElement(\"SCRIPT\")");
                srcCreateElement.AppendLine(string.Format("var textnodey = document.createTextNode(\"{0}\")", srcGetLinkText.ToString()));
                //srcCreateElement.AppendLine("var t = document.createTextNode(\"function getLinkText(url){console.log(url);var elements = document.getElementsByTagName('a');var result ='';for(var j=0;j<elements.length;j++){if(elements[j].href==url){result =elements[j].text; break;}}return result;}\")");
                srcCreateElement.AppendLine("scriptx.appendChild(textnodey);");
                srcCreateElement.AppendLine("document.head.appendChild(scriptx);");

                //strJsCode = string.Format("document.write('{0}')", srcCreateElement.ToString());
                strJsCode = string.Format("{0}", srcCreateElement.ToString());
                MessageBox.Show(strJsCode);

                //执行JS注入JS代码
                this.RunJs(strJsCode);//不成功
                //让页面支持HTML5播放器
                //this.RunJs("document.write('<script src=http://api.html5media.info/1.1.8/html5media.min.js></script>')");//不成功
                //this.RunJs("document.write('<script src=https://myimg-1254438545.cos.ap-beijing.myqcloud.com/one.js></script>')");
                //this.RunJs("hello()");//不成功

                //this.RunJs("alert($('#s_lg_img_new').attr('src'))");//成功
            }
            catch (Exception ex)
            {
                //MessageBox.Show(string.Format("RunJs got error:{0}",ex.Message));
            }
        }

        void OnwkeConsoleMessageCallback(IntPtr webView, IntPtr param, wkeConsoleLevel level, IntPtr message, IntPtr sourceName, int sourceLine, IntPtr stackTrace)
        {
            //Console.WriteLine("Console level" + level);
            Console.WriteLine("Console Msg:" + BlinkBrowserPInvoke.Utf8IntptrToString(BlinkBrowserPInvoke.wkeGetString(message)));
            //Console.WriteLine("Console sourceName:" + wkeGetString(sourceName).Utf8IntptrToString());
            //Console.WriteLine("Console stackTrace:" + wkeGetString(stackTrace).Utf8IntptrToString());
            //Console.WriteLine("Console sourceLine:" + sourceLine);
        }

        //全局窗口右键菜单
        ContextMenuStrip DocumentMenuStrip = new ContextMenuStrip();
        //DocumentMenuStrip.Renderer
        //在光标存手型的全局右键菜单
        ContextMenuStrip OnLinkMenuStrip = new ContextMenuStrip();

        //构造函数
        ~BlinkBrowser()
        {
            if (handle != IntPtr.Zero) {
                BlinkBrowserPInvoke.wkeFinalize();
            }
        }

        //析构函数
        public BlinkBrowser()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            //SetStyle(ControlStyles.OptimizedDoubleBuffer |ControlStyles.DoubleBuffer |ControlStyles.AllPaintingInWmPaint |ControlStyles.ResizeRedraw |
            //    //  ControlStyles.EnableNotifyMessage|
            //    ControlStyles.UserPaint, true);
            UpdateStyles();

            //添加菜单弹出时事件
            DocumentMenuStrip.Opening += DocumentMenuStrip_Opening;
            OnLinkMenuStrip.Opening += OnLinkMenuStrip_Opening;
            //赋值右键菜单
            if (IsEnableContextMenu) {
                ContextMenuStrip = DocumentMenuStrip;
            } else {
                ContextMenuStrip = null;
            }

            #region "初始化全局右键菜单"
            MenuItemDraw tsmiGoBack = new MenuItemDraw("返回(&B)", null, (x, y) =>
            {
                BlinkBrowserPInvoke.wkeGoBack(handle);
            });
            MenuItemDraw tsmiForward = new MenuItemDraw("前进(&O)", null, (x, y) =>
            {
                BlinkBrowserPInvoke.wkeGoForward(handle);
            });
            MenuItemDraw tsmiReload = new MenuItemDraw("刷新(&R)", null, (x, y) =>
            {
                BlinkBrowserPInvoke.wkeReload(handle);
            });
            MenuItemDraw tsmiSelectAll = new MenuItemDraw("全选(&A)", null, (x, y) =>
            {
                BlinkBrowserPInvoke.wkeSelectAll(handle);
            });
            MenuItemDraw tsmiViewSource = new MenuItemDraw("查看源码(&V)", null, (x, y) =>
            {
                //如果查看源码只需要传回URL地址
                if (ViewSourceType == true)
                {
                    try
                    {
                        if (OnNetGetSourceOnlyUrlEvent != null)
                        {
                            OnNetGetSourceOnlyUrlEvent(handle, url);
                            //MessageBox.Show( string.Format("Marshal.Copy got error:{0},byte len:{1}", ex.Message,len));
                            //throw;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    IntPtr vsHandle = IntPtr.Zero;
                    vsHandle = BlinkBrowserPInvoke.wkeGetSource(handle);
                    if (vsHandle != IntPtr.Zero)
                    {
                        string strsource = string.Empty;
                        try
                        {
                            //获取网页源码，调用回调传给Net程序
                            strsource = BlinkBrowserPInvoke.Utf8IntptrToString(vsHandle);
                            if (strsource.Length > 0)
                            {
                                if (OnNetGetSourceEvent != null)
                                {
                                    OnNetGetSourceEvent(handle, strsource);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //
                        }
                    }
                }

            });

            //添加分隔符
            MenuItemDraw tsmSeparator1 = new MenuItemDraw("-", null, null);
            tsmSeparator1.Height = 2;
            tsmSeparator1.Enabled = false;
            ToolStripSeparator tssItemFirst = new ToolStripSeparator();
            tssItemFirst.Height = 1;
            //测试二级菜单
            MenuItemDraw tsmiCodePage = new MenuItemDraw("编码(&E)", null, null);
            MenuItemDraw tsmiCodePageCn = new MenuItemDraw("Unicode (UTF-8)", null, null);
            tsmiCodePageCn.Checked = true;
            tsmiCodePage.DropDownItems.Add(tsmiCodePageCn);
            //设置菜单项的参数
            tsmiSelectAll.ShortcutKeys = Keys.Control | Keys.A;
            DocumentMenuStrip.Items.Add(tsmiGoBack);
            DocumentMenuStrip.Items.Add(tsmiForward);
            DocumentMenuStrip.Items.Add(tsmiReload);
            DocumentMenuStrip.Items.Add(tsmiSelectAll);
            DocumentMenuStrip.Items.Add(tssItemFirst);
            //DocumentMenuStrip.Items.Add(tsmSeparator1);            
            DocumentMenuStrip.Items.Add(tsmiCodePage);
            DocumentMenuStrip.Items.Add(tsmiViewSource);
            DocumentMenuStrip.ShowCheckMargin = false;
            #endregion "初始化全局右键菜单结束"

            #region "初始化光标为手型时的弹出菜单"
            MenuItemDraw tsmiNewTabWindow = new MenuItemDraw("在新标签页中打开链接(&T)", null, (x, y) =>
            {
                if(popurl.Length > 0)
                {
                    if (OnCreateNewTabEvent != null)
                    {
                        OnCreateNewTabEvent(this.handle, IntPtr.Zero, popurl);
                    }
                }
                //BlinkBrowserPInvoke.wkeGoBack(handle);
            });
            MenuItemDraw tsmiNewWindow = new MenuItemDraw("在新窗口中打开链接(&B)", null, (x, y) =>
            {
                if (popurl.Length > 0) {
                    this.CreatePopupWindow(popurl, "");
                }
                //BlinkBrowserPInvoke.wkeGoForward(handle);
            });
            MenuItemDraw tsmiNewInvisibleWindow = new MenuItemDraw("在影身窗口中打开(&G)", null, (x, y) =>
            {
                //BlinkBrowserPInvoke.wkeReload(handle);
            });
            ToolStripSeparator tssLinkItemFirst = new ToolStripSeparator();
            tssLinkItemFirst.Height = 1;
            MenuItemDraw tsmiCopyLinkAddress = new MenuItemDraw("复制链接地址(&E)", null, (x, y) =>
            {
                // 复制标题与网址
                try
                {
                    //BlinkBrowserPInvoke.OutputDebugStringA(string.Format("复制链接地址菜单被点击"));
                    //BlinkBrowserPInvoke.OutputDebugStringA(string.Format("鼠标悬停在网址：{0}上", popurl));
                    // Dim cliFormat As System.Windows.Forms.TextDataFormat
                    if (popurl.Length > 0) {
                        string tmpCopyValue = string.Empty;
                        tmpCopyValue = string.Format("{0}", popurl);

                        //BlinkBrowserPInvoke.OutputDebugStringA(string.Format("当前复制的网址为：{0}", popurl));

                        Clipboard.Clear();
                        Clipboard.SetText(tmpCopyValue, TextDataFormat.Text);
                    }
                } catch (Exception ex) {
                    //BlinkBrowserPInvoke.OutputDebugStringA(string.Format("复制链接地址菜单点时有错，{0}", ex.Message ));
                }
                //BlinkBrowserPInvoke.wkeSelectAll(handle);
            });
            MenuItemDraw tsmiCopyLinkText = new MenuItemDraw("复制链接文字(&X)", null, (x, y) =>
            {
                try
                {
                    // Dim cliFormat As System.Windows.Forms.TextDataFormat
                    if (popurl.Length > 0)
                    {
                        string strLinkText = string.Empty;
                        object objRetValue = null;

                        objRetValue = CallJsFunc("getLinkText", popurl);
                        if (objRetValue != null) {

                            strLinkText = (string)objRetValue.ToString();
                            if (strLinkText.Length > 0) {

                                string tmpCopyValue = string.Empty;
                                tmpCopyValue = string.Format("{0}", strLinkText);

                                Clipboard.Clear();
                                Clipboard.SetText(tmpCopyValue, TextDataFormat.Text);
                            }
                            //MessageBox.Show(strLinkText);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("call js function getLinkText got error:{0}", ex.Message));
                }

                //BlinkBrowserPInvoke.wkeSelectAll(handle);
            });
            ToolStripSeparator tssLinkItemSecond = new ToolStripSeparator();
            tssLinkItemSecond.Height = 1;
            MenuItemDraw tsmiCheckElement = new MenuItemDraw("审查元素(&N)", null, (x, y) =>
            {
                //BlinkBrowserPInvoke.wkeSelectAll(handle);
            });
            #endregion

            OnLinkMenuStrip.Items.Add(tsmiNewTabWindow);
            OnLinkMenuStrip.Items.Add(tsmiNewWindow);
            OnLinkMenuStrip.Items.Add(tsmiNewInvisibleWindow);
            OnLinkMenuStrip.Items.Add(tssLinkItemFirst);
            OnLinkMenuStrip.Items.Add(tsmiCopyLinkAddress);
            OnLinkMenuStrip.Items.Add(tsmiCopyLinkText);
            OnLinkMenuStrip.Items.Add(tssLinkItemSecond);
            OnLinkMenuStrip.Items.Add(tsmiCheckElement);
            //contextMenuStrip.Items.Add(tsmiCopy);
            //contextMenuStrip.Items.Add(tsmiCut);
            //contextMenuStrip.Items.Add(tsmiPaste);
            //contextMenuStrip.Items.Add(tsmiDelete);
            Application.AddMessageFilter(this);
        }


        //重写控件创建时事件
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            //BlinkBrowserPInvoke.OutputDebugStringA("OnCreateControl");
            if (!DesignMode)
            {
                //timer.Tick += Timer_Tick;
                //timer.Start();
                CreateCore();
            }
        }

        /// <summary>
        /// 初始化 MB
        /// </summary>
        public void CreateCore()
        {
            BlinkBrowserPInvoke.wkeInitialize();

            //BlinkBrowserPInvoke.OutputDebugStringA("CreateCore");
            //BlinkBrowserPInvoke.wkeInitializeExWrap(new wkeSettings()
            //{
            //    proxy = new wkeProxy
            //    {
            //        hostname = "127.0.0.1",
            //        port = 8888,
            //        type = wkeProxyType.WKE_PROXY_HTTP,
            //        password = "",
            //        username = ""
            //    },
            //    mask = wkeSettingMask.WKE_SETTING_PROXY
            //});
            //BlinkBrowserPInvoke.wkeInitialize();
            //BlinkBrowserPInvoke.wkeInitializeExWrap(new wkeSettings()
            //{
            //    proxy = new wkeProxy
            //    {
            //        hostname = "127.0.0.1",
            //        port = 8888,
            //        type = wkeProxyType.WKE_PROXY_HTTP,
            //        password = "",
            //        username = ""
            //    },
            //    mask = 1
            //});

            //wkeConfigureWrap
            wkeSettings wkeSets = new wkeSettings();
            //wkeProxy wkyProxys = new wkeProxy();
            //wkyProxys.hostname = "127.0.0.1";
            //wkyProxys.port = 80;
            //wkyProxys.type = wkeProxyType.WKE_PROXY_HTTP;
            //wkyProxys.username = "";
            //wkyProxys.password = "";
            //wkeSets.proxy = null;
            //wkeSets.mask = wkeSets.mask & wkeSettingMask.WKE_ENABLE_DISABLE_H5VIDEO;
            //设置代理以及其它参数
            BlinkBrowserPInvoke.wkeConfigureWrap(wkeSets);

            //创建WebView对象并存储其句柄，用来绘画网页内容，处理消息等
            handle = BlinkBrowserPInvoke.wkeCreateWebView();

            //在这里调用初始化回调，通知调用方绑定NET函数
            try
            {
                if (OnViewInitEvent != null)
                {
                    OnViewInitEvent(handle);
                }
            }
            catch (Exception ex)
            {

            }


            //只有开启才会触发 wkeOnCreateView
            BlinkBrowserPInvoke.wkeSetNavigationToNewWindowEnable(handle, true);

            BlinkBrowserPInvoke.wkeSetHandle(this.handle, this.Handle);
            BlinkBrowserPInvoke.wkeSetHandleOffset(handle, Location.X - 2, 0);

            //BlinkBrowserPInvoke.wkeSetTransparent(handle, true);

            //是否开启插件
            BlinkBrowserPInvoke.wkeSetNpapiPluginsEnabled(handle, true);

            //是否开启内存缓存
            BlinkBrowserPInvoke.wkeSetMemoryCacheEnable(handle, true);

            //开启Cookies
            BlinkBrowserPInvoke.wkeSetCookieEnabled(handle, true);
            CookiePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "cookie");
            if (!Directory.Exists(CookiePath))
            {
                Directory.CreateDirectory(CookiePath);
            }
            BlinkBrowserPInvoke.wkeSetCookieJarPath(handle, CookiePath);

            //调整窗口尺寸以及设置浏览器标识字符
            BlinkBrowserPInvoke.wkeResize(handle, Width, Height);
            BlinkBrowserPInvoke.wkeSetUserAgentW(this.handle, "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.75 Safari/537.36");

            //绑定JSFunction属性的方法到JS前台
            BindJsFunc();
            //Alter方法调用时，调用回调函数通知后台
            AlertBoxCallback = new AlertBoxCallback((a, b, c) =>
            {
                MessageBox.Show(Marshal.PtrToStringUni(BlinkBrowserPInvoke.wkeToStringW(c)), "Alter提示来自于:" + Url);
            });
            BlinkBrowserPInvoke.wkeOnAlertBox(handle, AlertBoxCallback, IntPtr.Zero);

            //设置声音
            //BlinkBrowserPInvoke.wkeSetMediaVolume(handle, 20);

            _wkeNavigationCallback = OnwkeNavigationCallback;
            BlinkBrowserPInvoke.wkeOnNavigation(handle, _wkeNavigationCallback, IntPtr.Zero);

            //BlinkBrowserPInvoke.wkeSetCookieEnabled(handle, false);

            titleChangeCallback = OnTitleChangedCallback;
            BlinkBrowserPInvoke.wkeOnTitleChanged(this.handle, titleChangeCallback, IntPtr.Zero);

            //鼠标滑过URL时触发
            titleChangeCallback2 = OnTitleChangedCallback2;
            BlinkBrowserPInvoke.wkeOnMouseOverUrlChanged(this.handle, titleChangeCallback2, IntPtr.Zero);

            //DOM加载完成时回调
            _wkeDocumentReadyCallback = OnwkeDocumentReadyCallback;
            //BlinkBrowserPInvoke.wkeOnDocumentReady(this.handle, _wkeDocumentReadyCallback, IntPtr.Zero);

            //DOM加载完成时回调2
            _wkeDocumentReady2Callback = OnwkeDocumentReady2Callback;
            BlinkBrowserPInvoke.wkeOnDocumentReady2(this.handle, _wkeDocumentReady2Callback, IntPtr.Zero);

            urlChangedCallback = OnUrlChangedCallback;
            BlinkBrowserPInvoke.wkeOnURLChanged(this.handle, urlChangedCallback, IntPtr.Zero);

            urlChangedCallback2 = OnUrlChangedCallback2;
            BlinkBrowserPInvoke.wkeOnURLChanged2(this.handle, urlChangedCallback2, IntPtr.Zero);

            _wkeConsoleMessageCallback = OnwkeConsoleMessageCallback;
            BlinkBrowserPInvoke.wkeOnConsole(this.handle, _wkeConsoleMessageCallback, IntPtr.Zero);

            _wkePaintUpdatedCallback = OnWkePaintUpdatedCallback;
            BlinkBrowserPInvoke.wkeOnPaintUpdated(this.handle, _wkePaintUpdatedCallback, IntPtr.Zero);

            _wkeLoadingFinishCallback = OnwkeLoadingFinishCallback;
            BlinkBrowserPInvoke.wkeOnLoadingFinish(this.handle, _wkeLoadingFinishCallback, IntPtr.Zero);

            //Download2CallBack调用后，接收数据的回调
            _wkeNetJobDataRecvCallback = OnwkeNetJobDataRecvCallback;
            //Download2CallBack调用后，接收数据结束的回调
            _wkeNetJobDataFinishCallback = OnwkeNetJobDataFinishCallback;

            //使用Post提交的方式下载文件
            _wkeDownload2Callback = OnwkeDownload2Callback;
            BlinkBrowserPInvoke.wkeOnDownload2(this.handle, _wkeDownload2Callback, IntPtr.Zero);

            //会导致 taobao 加载图片异常
            _wkeDownloadCallback = OnwkeDownloadCallback;
            //BlinkBrowserPInvoke.wkeOnDownload(this.handle, _wkeDownloadCallback, IntPtr.Zero);

            _wkeCreateViewCallback = OnwkeCreateViewCallback;
            BlinkBrowserPInvoke.wkeOnCreateView(this.handle, _wkeCreateViewCallback, handle);

            _wkeLoadUrlBeginCallback = OnwkeLoadUrlBeginCallback;
            BlinkBrowserPInvoke.wkeOnLoadUrlBegin(this.handle, _wkeLoadUrlBeginCallback, handle);

            #region JS 动态绑定，并返回值
            wkeJsNativeFunction jsnav = new wkeJsNativeFunction((es, param) =>
            {
                string s = BlinkBrowserPInvoke.Utf8IntptrToString(BlinkBrowserPInvoke.jsToString(es, BlinkBrowserPInvoke.jsArg(es, 0)));
                IntPtr strPtr = Marshal.StringToCoTaskMemUni("这是C#后台返回值:" + s);
                Int64 result = BlinkBrowserPInvoke.jsStringW(es, strPtr);
                Marshal.FreeCoTaskMem(strPtr);
                return result;
            });
            BlinkBrowserPInvoke.wkeJsBindFunction("jsReturnValueTest", jsnav, IntPtr.Zero, 1);
            #endregion

            // get
            wkeJsNativeFunction jsnavGet = new wkeJsNativeFunction((es, param) =>
            {
                Console.WriteLine("call jsBindGetter");
                return BlinkBrowserPInvoke.jsStringW(es, Marshal.StringToCoTaskMemUni("{ \"name\": \"he\" }"));
            });
            BlinkBrowserPInvoke.wkeJsBindGetter("testJson", jsnavGet, IntPtr.Zero);
            listObj.Add(jsnavGet);

            // set
            wkeJsNativeFunction jsnavSet = new wkeJsNativeFunction((es, _param) =>
            {
                Console.WriteLine("call jsBindSetter");

                Int64 testJson = BlinkBrowserPInvoke.jsArg(es, 0);
                IntPtr argStr = BlinkBrowserPInvoke.jsToStringW(es, testJson);
                string argString = Marshal.PtrToStringUni(argStr);
                //MessageBox.Show(argString, "alert setter");

                return BlinkBrowserPInvoke.jsUndefined(es);
            });
            BlinkBrowserPInvoke.wkeJsBindSetter("testJson", jsnavSet, IntPtr.Zero);
            listObj.Add(jsnavSet);

            _wkeLoadUrlEndCallback = OnwkeLoadUrlEndCallback;
            BlinkBrowserPInvoke.wkeOnLoadUrlEnd(this.handle, _wkeLoadUrlEndCallback, handle);

            //获取fav图标
            _wkeOnNetGetFavicon = OnwkeNetGetFavicon;


            //如果在设计器里赋值了，初始化后加载URL
            if (!string.IsNullOrEmpty(url))
            {
                if (!IsDesignMode())
                {
                    BlinkBrowserPInvoke.wkeLoadURLW(handle, url);
                }                    
            }
        }


        private void DocumentMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //菜单弹出后恢复默认光标
            Cursor = Cursors.Default;
            if (this.handle != IntPtr.Zero)
            {
                foreach (object item in DocumentMenuStrip.Items)
                {
                    Type tobjtype = null;
                    tobjtype = item.GetType();
                    if (tobjtype != null)
                    {
                        if (tobjtype.Equals(typeof(MenuItemDraw)))
                        {
                            MenuItemDraw tmpItem = (MenuItemDraw)item;
                            if (tmpItem.Text == "返回(&B)")
                            {
                                tmpItem.Enabled = BlinkBrowserPInvoke.wkeCanGoBack(this.handle);
                            }
                            else if (tmpItem.Text == "前进(&O)")
                            {
                                tmpItem.Enabled = BlinkBrowserPInvoke.wkeCanGoForward(this.handle);
                            }
                        }
                    }
                }
            }
        }


        private void OnLinkMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            popurl = overurl;
            //throw new NotImplementedException();
        }



        void OnTitleChangedCallback(IntPtr webView, IntPtr param, IntPtr title)
        {
            if (OnTitleChangeCall != null)
            {
                string strtitle = BlinkBrowserPInvoke.Utf8IntptrToString(BlinkBrowserPInvoke.wkeGetString(title));
                OnTitleChangeCall(strtitle);
                this.title = strtitle;
            }
        }

        //鼠标滑过url时触发此回调
        void OnTitleChangedCallback2(IntPtr webView, IntPtr param, IntPtr title)
        {
            //设置鼠标形状，如果是链接，设置为[手掌]光标
            SetCursors();
            //如果光标经过的是超链接，则触发回调，返回数据给调用方
            //MessageBox.Show(string.Format("MouseOverUrl return value，param:{0}, title:{1}",param,title));

            try
            {
                string getUrl = string.Empty;

                if (OnMouseOverUrlChanged != null) {

                    getUrl = BlinkBrowserPInvoke.Utf8IntptrToString(BlinkBrowserPInvoke.wkeGetString(title));
                    overurl = getUrl;

                    OnMouseOverUrlChanged(getUrl);
                }
            } catch (Exception ex) {
                //do something
            }
            //Console.WriteLine("OnTitleChangedCallback2 title:" + MiniBlinkPinvoke.BlinkBrowserPInvoke.wkeGetString(title).Utf8IntptrToString());
        }

        bool OnwkeNavigationCallback(IntPtr webView, IntPtr param, wkeNavigationType navigationType, IntPtr url)
        {

            IntPtr urlPtr = BlinkBrowserPInvoke.wkeGetStringW(url);
            Console.WriteLine(navigationType);

            Console.WriteLine("OnwkeNavigationCallback:URL:" + Marshal.PtrToStringUni(urlPtr));

            if (OnUrlNavigationCall != null)
            {
                OnUrlNavigationCall(Marshal.PtrToStringUni(urlPtr));
            }

            return true;
        }

        void OnUrlChangedCallback(IntPtr webView, IntPtr param, IntPtr url)
        {
            if (OnUrlChangeCall != null)
            {
                OnUrlChangeCall(BlinkBrowserPInvoke.Utf8IntptrToString(BlinkBrowserPInvoke.wkeGetString(url)));
            }
            //OnUrlChangeCall?.Invoke(BlinkBrowserPInvoke.Utf8IntptrToString(BlinkBrowserPInvoke.wkeGetString(url)));
            //Console.WriteLine("OnUrlChangedCallback:URL:" +);
        }

        Regex regex = new Regex(@"_miniblink__data_[0-9]{1,}.htm", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
        void OnUrlChangedCallback2(IntPtr webView, IntPtr param, IntPtr frameId, IntPtr url)
        {
            //不管是不是主FrameID，先触发回调
            if(OnUrlChange3Call !=null) {
                string nowURL = BlinkBrowserPInvoke.Utf8IntptrToString(BlinkBrowserPInvoke.wkeGetString(url));
                if (!regex.IsMatch(nowURL)) {
                    OnUrlChange3Call(nowURL, frameId );
                }
            }
            else
            {
                //主窗口才触发事件
                if (frameId.ToInt32() == 1 )
                {
                    string nowURL = BlinkBrowserPInvoke.Utf8IntptrToString(BlinkBrowserPInvoke.wkeGetString(url));
                    if (!regex.IsMatch(nowURL)) {
                        if (OnUrlChange2Call != null) {
                            OnUrlChange2Call(nowURL);
                        }
                    }                    
                }
            }
            
        }

        void OnWkePaintUpdatedCallback(IntPtr webView, IntPtr param, IntPtr hdc, int x, int y, int cx, int cy)
        {
            ////Console.WriteLine(string.Format("call OnWkePaintUpdatedCallback {0},{1},{2},{3},{4},{5}", param, hdc, x, y, cx, cy));
            //if (handle != IntPtr.Zero && BlinkBrowserPInvoke.wkeIsDirty(handle))
            //{
            //Invalidate(new Rectangle(0,0,this.Width,this.Height));
            // Console.WriteLine(DateTime.Now + " 调用重绘");
            Invalidate(new Rectangle(x, y, cx, cy), false);
            #region 从 hdc 中取图像 开启这个可以取消 OnPaint 重写，但感觉页面有卡顿

            //Core.GraphicsWrapper.CopyTo(Graphics.FromHdcInternal(hdc), this.CreateGraphics(), new Rectangle(x, y, cx, cy));
            // ClearMemory();
            #endregion
            //Invalidate();
            //Graphics dc = Graphics.FromHdc(hdc);
            //if (bits == IntPtr.Zero || oldSize != Size)
            //{
            //    if (bits != IntPtr.Zero)
            //    {
            //        Marshal.FreeHGlobal(bits);
            //    }
            //    oldSize = Size;
            //    bits = Marshal.AllocHGlobal(Width * Height * 4);
            //}

            //BlinkBrowserPInvoke.wkePaint(handle, bits, 0);
            //using (Bitmap bmp = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppPArgb, bits))
            //{

            //    dc.DrawImage(bmp, 0, 0);
            //}
            //}
        }


        //在网页渲染结果画到控件上
        protected override void OnPaint(PaintEventArgs e)
        {
            if (handle != IntPtr.Zero)
            {

                if (bits == IntPtr.Zero || oldSize != Size)
                {
                    if (bits != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(bits);
                    }
                    oldSize = Size;
                    bits = Marshal.AllocHGlobal(Width * Height * 4);
                }

                BlinkBrowserPInvoke.wkePaint(handle, bits, 0);
                using (Bitmap bmp = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppPArgb, bits))
                {
                    e.Graphics.DrawImage(bmp, 0, 0);
                    bmp.Dispose();
                }
            }
            else
            {
                base.OnPaint(e);
            }
            if (DesignMode)
            {
                e.Graphics.DrawString("MiniBlinkBrowser", this.Font, Brushes.Red, new Point());
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(0, 0, Width - 1, Height - 1));
            }
        }

        #region 鼠标和按键
        /// <summary>
        /// 设置鼠标划过对象时的光标形状
        /// </summary>
        void SetCursors()
        {

            switch (BlinkBrowserPInvoke.wkeGetCursorInfoType(handle))
            {
                case WkeCursorInfo.WkeCursorInfoPointer:
                    Cursor = Cursors.Default;
                    break;
                case WkeCursorInfo.WkeCursorInfoCross:
                    Cursor = Cursors.Cross;
                    break;
                case WkeCursorInfo.WkeCursorInfoHand:
                    if (IsEnableContextMenu) {
                        if (overurl.ToLower().Contains("javascript:void(0)")) {
                            ContextMenuStrip = null;
                        }
                        else {
                            ContextMenuStrip = OnLinkMenuStrip;
                        }
                    } else {
                        ContextMenuStrip = null;
                    }
                    Cursor = Cursors.Hand;
                    break;
                case WkeCursorInfo.WkeCursorInfoIBeam:
                    Cursor = Cursors.IBeam;
                    break;
                case WkeCursorInfo.WkeCursorInfoWait:
                    Cursor = Cursors.WaitCursor;
                    break;
                case WkeCursorInfo.WkeCursorInfoHelp:
                    Cursor = Cursors.Help;
                    break;
                case WkeCursorInfo.WkeCursorInfoEastResize:
                    Cursor = Cursors.SizeWE;
                    break;
                case WkeCursorInfo.WkeCursorInfoNorthResize:
                    Cursor = Cursors.SizeNS;
                    break;
                case WkeCursorInfo.WkeCursorInfoNorthEastResize:
                    Cursor = Cursors.SizeNESW;
                    break;
                case WkeCursorInfo.WkeCursorInfoNorthWestResize:
                    Cursor = Cursors.SizeNWSE;
                    break;
                case WkeCursorInfo.WkeCursorInfoSouthResize:
                    Cursor = Cursors.SizeWE;
                    break;
                case WkeCursorInfo.WkeCursorInfoSouthEastResize:
                    Cursor = Cursors.SizeNWSE;
                    break;
                case WkeCursorInfo.WkeCursorInfoSouthWestResize:
                    Cursor = Cursors.SizeNESW;
                    break;
                case WkeCursorInfo.WkeCursorInfoWestResize:
                    Cursor = Cursors.SizeWE;
                    break;
                case WkeCursorInfo.WkeCursorInfoNorthSouthResize:
                    Cursor = Cursors.SizeNS;
                    break;
                case WkeCursorInfo.WkeCursorInfoEastWestResize:
                    Cursor = Cursors.SizeWE;
                    break;
                case WkeCursorInfo.WkeCursorInfoNorthEastSouthWestResize:
                    Cursor = Cursors.SizeAll;
                    break;
                case WkeCursorInfo.WkeCursorInfoNorthWestSouthEastResize:
                    Cursor = Cursors.SizeAll;
                    break;
                case WkeCursorInfo.WkeCursorInfoColumnResize:
                    Cursor = Cursors.Default;
                    break;
                case WkeCursorInfo.WkeCursorInfoRowResize:
                    Cursor = Cursors.Default;
                    break;
                default:
                    Cursor = Cursors.Default;
                    break;
            }
            if (Cursor.Equals(Cursors.Default)) {
                if (IsEnableContextMenu) {
                    ContextMenuStrip = DocumentMenuStrip;
                } else {
                    ContextMenuStrip = null;
                }
            }
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (handle != IntPtr.Zero && Width > 0 && Height > 0)
            {
                BlinkBrowserPInvoke.wkeResize(handle, Width, Height);
                Invalidate();
            }
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {

            base.OnMouseWheel(e);
            if (handle != IntPtr.Zero)
            {
                try
                {
                    if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        //do something
                        if (e.Delta > 0) {
                            //放大页面
                            this.ZoomFactor = this.ZoomFactor + 0.1F;
                        } else {
                            //缩小页面
                            this.ZoomFactor = this.ZoomFactor - 0.1F;
                        }

                    } else if ((Control.ModifierKeys & Keys.LShiftKey) == Keys.LShiftKey) {
                        //do something
                        if (e.Delta > 0) {
                            //放大页面
                            this.MediaVolume = this.MediaVolume + 0.1F;
                        } else {
                            //缩小页面
                            this.MediaVolume = this.MediaVolume - 0.1F;
                        }
                    }
                    else {
                        //默认的为直接将滚轮发送到Blink内核，先判断有没有按Ctrl或Shift键
                        uint flags = GetMouseFlags(e);
                        BlinkBrowserPInvoke.wkeFireMouseWheelEvent(handle, e.X, e.Y, e.Delta, flags);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            uint msg = 0;
            if (e.Button == MouseButtons.Left)
            {
                msg = (uint)wkeMouseMessage.WKE_MSG_LBUTTONDOWN;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                msg = (uint)wkeMouseMessage.WKE_MSG_MBUTTONDOWN;
            }
            else if (e.Button == MouseButtons.Right)
            {
                msg = (uint)wkeMouseMessage.WKE_MSG_RBUTTONDOWN;
            }
            else if (e.Button == MouseButtons.XButton1)
            {
                //鼠标侧健前进键
                //2020/6/13 22：31分添加
                if (this.CanGoBack)
                {
                    BlinkBrowserPInvoke.wkeGoBack(handle);
                }
                //msg = (uint)wkeMouseMessage.WKE_MSG_RBUTTONDOWN;
            }
            else if (e.Button == MouseButtons.XButton2)
            {
                //鼠标侧健后退键 //2020/6/13 22：31分添加
                if (this.CanGoForward)
                {
                    BlinkBrowserPInvoke.wkeGoForward(handle);
                }
                //msg = (uint)wkeMouseMessage.WKE_MSG_RBUTTONDOWN;
            }
            uint flags = GetMouseFlags(e);
            if (handle != IntPtr.Zero)
            {
                BlinkBrowserPInvoke.wkeFireMouseEvent(handle, msg, e.X, e.Y, flags);
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            uint msg = 0;
            if (e.Button == MouseButtons.Left)
            {
                msg = (uint)wkeMouseMessage.WKE_MSG_LBUTTONUP;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                msg = (uint)wkeMouseMessage.WKE_MSG_MBUTTONUP;
            }
            else if (e.Button == MouseButtons.Right)
            {
                msg = (uint)wkeMouseMessage.WKE_MSG_RBUTTONUP;
            }
            uint flags = GetMouseFlags(e);
            if (handle != IntPtr.Zero)
            {
                BlinkBrowserPInvoke.wkeFireMouseEvent(handle, msg, e.X, e.Y, flags);
                //if (e.Button == MouseButtons.Right)
                //{
                //    EwePInvoke.wkeFireContextMenuEvent(handle, e.X, e.Y, flags);
                //}
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            //base.OnMouseMove(e);
            //if (handle != IntPtr.Zero)
            //{
            //    //uint msg = (uint)wkeMouseMessage.WKE_MSG_MOUSEMOVE;
            //    uint flags = GetMouseFlags(e);
            //    //EwePInvoke.wkeFireMouseEvent(handle, msg, e.X, e.Y, flags);
            //    EwePInvoke.wkeFireMouseEvent(handle, 0x200, e.X, e.Y, flags);
            //}
            base.OnMouseMove(e);
            if (this.handle != IntPtr.Zero)
            {
                uint flags = GetMouseFlags(e);
                BlinkBrowserPInvoke.wkeFireMouseEvent(this.handle, 0x200, e.X, e.Y, flags);
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (handle != IntPtr.Zero)
            {
                BlinkBrowserPInvoke.wkeFireKeyDownEvent(handle, (uint)e.KeyValue, 0, false);
            }
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (handle != IntPtr.Zero)
            {
                e.Handled = true;
                BlinkBrowserPInvoke.wkeFireKeyPressEvent(handle, (uint)e.KeyChar, 0, false);
            }
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (handle != IntPtr.Zero)
            {
                BlinkBrowserPInvoke.wkeFireKeyUpEvent(handle, (uint)e.KeyValue, 0, false);
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (handle != IntPtr.Zero)
            {
                BlinkBrowserPInvoke.wkeSetFocus(handle);
                try
                {
                    if (!EnabledFocusEvent) {
                        return;
                    }
                    //触发获得焦点回调函数
                    if (OnFocusedChanged != null) {
                        //将MiniBlink WebBrowser的句柄传过去
                        OnFocusedChanged(handle);
                    }
                }
                catch(Exception ex)
                {

                }

            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (handle != IntPtr.Zero)
            {
                BlinkBrowserPInvoke.wkeKillFocus(handle);
            }
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Up:
                case Keys.Left:
                case Keys.Right:
                case Keys.Tab:
                    e.IsInputKey = true;
                    break;
            }
        }
        private static uint GetMouseFlags(MouseEventArgs e)
        {
            uint flags = 0;
            if (e.Button == MouseButtons.Left)
            {
                flags = flags | (uint)wkeMouseFlags.WKE_LBUTTON;
            }
            if (e.Button == MouseButtons.Middle)
            {
                flags = flags | (uint)wkeMouseFlags.WKE_MBUTTON;
            }
            if (e.Button == MouseButtons.Right)
            {
                flags = flags | (uint)wkeMouseFlags.WKE_RBUTTON;
            }
            if (Control.ModifierKeys == Keys.Control)
            {
                flags = flags | (uint)wkeMouseFlags.WKE_CONTROL;
            }
            if (Control.ModifierKeys == Keys.Shift)
            {
                flags = flags | (uint)wkeMouseFlags.WKE_SHIFT;
            }
            return flags;
        }
        #endregion

        /// <summary>
        /// 在一个闭包中运行一段js代码，由于js是弱类型语言，返回值统一用jsValue类型表示（jsValue其实是个封装了v8内部各种类型的类）
        /// ，后面提供了一组用于具体类型转换的接口。
        /// </summary>
        /// <param name="js"></param>
        /// <returns></returns>
        public JsValue InvokeJS(string js)
        {
            IntPtr jsPtr = Marshal.StringToCoTaskMemAnsi(js);
            JsValue result = new JsValue(BlinkBrowserPInvoke.wkeRunJS(handle, jsPtr), BlinkBrowserPInvoke.wkeGlobalExec(handle));
            Marshal.FreeCoTaskMem(jsPtr);
            return result;
        }


        /// <summary>
        /// 执行js 宽字符版wkeRunJS，功能完全相同
        /// </summary>
        /// <param name="js"></param>
        /// <returns></returns>
        public JsValue InvokeJSW(string js)
        {
            return new JsValue(BlinkBrowserPInvoke.wkeRunJSW(handle, js), BlinkBrowserPInvoke.wkeGlobalExec(handle));
        }


        /// <summary>
        /// 执行js
        /// </summary>
        /// <param name="js"></param>
        /// <returns></returns>
        public object RunJs(string script)
        {
            try
            {
                //MessageBox.Show(string.Format("RunJs called, Args:{0}", script));
                var es = BlinkBrowserPInvoke.wkeGlobalExec(handle);
                return BlinkBrowserPInvoke.jsEvalExW(es, script, true).ToJsValue(this, es);

            } catch (Exception ex)
            {
                //MessageBox.Show(string.Format("RunJs got an error:{0}", ex.Message));
                return null;
            }
        }



        /// <summary>
        /// 以创建SCRIPT元素节点的方式注入JS代码
        /// </summary>
        /// <param name="jscode">要注入的JS代码</param>
        /// <param name="jsaddto">要添加到head或者body中</param>
        /// <returns></returns>
        public bool InjectionJsCode(string jscode, string jsaddto = "body")
        {
            try
            {
                string strJsCode = string.Empty;

                StringBuilder srcCreateElement = new StringBuilder();
                srcCreateElement.Clear();

                srcCreateElement.AppendLine("var scriptx = document.createElement(\"SCRIPT\")");
                srcCreateElement.AppendLine(string.Format("var textnodet = document.createTextNode(\"{0}\")", jscode));
                srcCreateElement.AppendLine("scriptx.appendChild(textnodet);");
                srcCreateElement.AppendLine(string.Format("document.{0}.appendChild(scriptx);", jsaddto));

                //strJsCode = string.Format("document.write('{0}')", srcCreateElement.ToString());
                strJsCode = string.Format("{0}", srcCreateElement.ToString());

                //执行JS注入JS代码
                this.RunJs(strJsCode);//不成功
                return (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (false);
        }



        /// <summary>
        /// 以创建SCRIPT元素节点的方式注入JS文件
        /// </summary>
        /// <param name="jsfile"></param>
        /// <param name="jsaddto"></param>
        /// <returns></returns>
        public bool InjectionJsFile(string jsfile, string jsaddto = "body")
        {
            try
            {
                string strJsCode = string.Empty;

                StringBuilder srcCreateElement = new StringBuilder();
                srcCreateElement.Clear();

                srcCreateElement.AppendLine("var scriptf = document.createElement(\"SCRIPT\")");
                srcCreateElement.AppendLine(string.Format("scriptf.setAttribute('src','{0}')", jsfile));
                srcCreateElement.AppendLine(string.Format("document.{0}.appendChild(scriptf);", jsaddto));

                //strJsCode = string.Format("document.write('{0}')", srcCreateElement.ToString());
                strJsCode = string.Format("{0}", srcCreateElement.ToString());

                //MessageBox.Show(strJsCode);

                //执行JS注入JS代码
                this.RunJs(strJsCode);//不成功
                return (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (false);
        }


        /// <summary>
        /// 以创建SCRIPT元素节点的方式注入JS文件异步执行
        /// </summary>
        /// <param name="jsfile">要注入的JS文件</param>
        /// <param name="jsaddto">要添加到body还是head中</param>
        /// <returns>执行成功返回true</returns>
        public bool InjectionJsFileAsync(string jsfile, string jsaddto = "body")
        {
            try
            {
                string strJsCode = string.Empty;

                StringBuilder srcCreateElement = new StringBuilder();
                srcCreateElement.Clear();

                srcCreateElement.AppendLine("var scriptf = document.createElement(\"SCRIPT\")");
                srcCreateElement.AppendLine(string.Format("scriptf.setAttribute('src','{0}')", jsfile));
                srcCreateElement.AppendLine("scriptf.setAttribute('async','async')");
                srcCreateElement.AppendLine(string.Format("document.{0}.appendChild(scriptf);", jsaddto));

                //strJsCode = string.Format("document.write('{0}')", srcCreateElement.ToString());
                strJsCode = string.Format("{0}", srcCreateElement.ToString());

                //执行JS注入JS代码
                this.RunJs(strJsCode);//不成功
                return (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (false);
        }


        /// <summary>
        /// 调用前台JS中的函数
        /// </summary>
        /// <param name="funcName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public object CallJsFunc(string funcName, params object[] param)
        {
            var es = BlinkBrowserPInvoke.wkeGlobalExec(handle);
            var func = BlinkBrowserPInvoke.jsGetGlobal(es, funcName);
            if (func == 0) {
                throw new WKEFunctionNotFondException(funcName);
            }
            var args = param.Select(i => i.ToJsValue(this, es)).ToArray();
            return BlinkBrowserPInvoke.jsCall(es, func, BlinkBrowserPInvoke.jsUndefined(), args, args.Length).ToValue(this, es);
        }

        /// <summary>
        /// 绑定带有 JSFunction 属性的方法到前台JS，实现前后台相互调用。
        /// </summary>
        public void BindJsFunc()
        {
            if (GlobalObjectJs == null)
            {
                GlobalObjectJs = this;
            }
            var att = GlobalObjectJs.GetType().GetMethods();
            //MessageBox.Show("this get methods length=" + att.Length.ToString()  );  
            //jsnaviteList.Clear();
            var result = new ArrayList();
            foreach (var item in att)
            {
                var xx = item.GetCustomAttributes(typeof(JSFunction), true);
                if (xx != null && xx.Length != 0)
                {
                    var jsnav = new wkeJsNativeFunction((es, _param) =>
                    {
                        var xp = item.GetParameters();
                        var argcount = BlinkBrowserPInvoke.jsArgCount(es);
                        long param = 0L;
                        if (xp != null && xp.Length != 0 && argcount != 0)
                        {
                            object[] listParam = new object[BlinkBrowserPInvoke.jsArgCount(es)];
                            for (int i = 0; i < argcount; i++)
                            {
                                Type tType = xp[i].ParameterType;

                                var paramnow = BlinkBrowserPInvoke.jsArg(es, i);
                                param = paramnow;
                                if (tType == typeof(int))
                                {
                                    listParam[i] = Convert.ChangeType(BlinkBrowserPInvoke.jsToInt(es, paramnow), tType);
                                }
                                else
                                if (tType == typeof(double))
                                {
                                    listParam[i] = Convert.ChangeType(BlinkBrowserPInvoke.jsToDouble(es, paramnow), tType);
                                }
                                else
                                if (tType == typeof(float))
                                {
                                    listParam[i] = Convert.ChangeType(BlinkBrowserPInvoke.jsToFloat(es, paramnow), tType);
                                }
                                else
                                if (tType == typeof(bool))
                                {
                                    listParam[i] = Convert.ChangeType(BlinkBrowserPInvoke.jsToBoolean(es, paramnow), tType);
                                }
                                else
                                {
                                    listParam[i] = Convert.ChangeType((BlinkBrowserPInvoke.Utf8IntptrToString(BlinkBrowserPInvoke.jsToString(es, paramnow))), tType);
                                }
                            }
                            try
                            {
                                var res = item.Invoke(GlobalObjectJs, listParam);
                                if (res != null)
                                {
                                    var mStr = Marshal.StringToHGlobalUni(res.ToString());
                                    return BlinkBrowserPInvoke.jsStringW(es, mStr);//返回JS字符串
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                        }
                        else
                        {
                            var res = item.Invoke(GlobalObjectJs, null);
                            if (res != null)
                            {
                                var mStr = Marshal.StringToHGlobalUni(res.ToString());
                                return BlinkBrowserPInvoke.jsStringW(es, mStr);//返回JS字符串
                            }
                        }
                        return param;
                    });
                    BlinkBrowserPInvoke.wkeJsBindFunction(item.Name, jsnav, IntPtr.Zero, (uint)item.GetParameters().Length);
                    listObj.Add(jsnav);
                }
            }
        }

        /// <summary>
        /// 绑定.Net框架应用中组件BlinkBrowser所在调用对象(窗口)中带有 JSFunction 属性的方法到前台JS，
        /// 实现前后台相互调用。
        /// </summary>
        public void BindNetJsFunc(Object win)
        {
            try
            {
                if (GlobalObjectJsNet == null)
                {
                    GlobalObjectJsNet = win;
                }
                //GlobalObjectJsNet = win;
                var att = GlobalObjectJsNet.GetType().GetMethods();
                //jsnaviteList.Clear();
                var result = new ArrayList();
                foreach (var item in att)
                {
                    var xx = item.GetCustomAttributes(typeof(JSFunction), true);
                    //BlinkBrowserPInvoke.OutputDebugStringA( string.Format("{0},length:{1}",xx.ToString(), xx.Length)  );
                    if (xx != null && xx.Length != 0)
                    {
                        var jsnav = new wkeJsNativeFunction((es, _param) =>
                        {
                            var xp = item.GetParameters();
                            var argcount = BlinkBrowserPInvoke.jsArgCount(es);
                            long param = 0L;
                            if (xp != null && xp.Length != 0 && argcount != 0)
                            {
                                object[] listParam = new object[BlinkBrowserPInvoke.jsArgCount(es)];
                                for (int i = 0; i < argcount; i++)
                                {
                                    Type tType = xp[i].ParameterType;

                                    var paramnow = BlinkBrowserPInvoke.jsArg(es, i);
                                    param = paramnow;
                                    if (tType == typeof(int))
                                    {
                                        listParam[i] = Convert.ChangeType(BlinkBrowserPInvoke.jsToInt(es, paramnow), tType);
                                    }
                                    else
                                    if (tType == typeof(double))
                                    {
                                        listParam[i] = Convert.ChangeType(BlinkBrowserPInvoke.jsToDouble(es, paramnow), tType);
                                    }
                                    else
                                    if (tType == typeof(float))
                                    {
                                        listParam[i] = Convert.ChangeType(BlinkBrowserPInvoke.jsToFloat(es, paramnow), tType);
                                    }
                                    else
                                    if (tType == typeof(bool))
                                    {
                                        listParam[i] = Convert.ChangeType(BlinkBrowserPInvoke.jsToBoolean(es, paramnow), tType);
                                    }
                                    else
                                    {
                                        listParam[i] = Convert.ChangeType((BlinkBrowserPInvoke.Utf8IntptrToString(BlinkBrowserPInvoke.jsToString(es, paramnow))), tType);
                                    }
                                }
                                try
                                {
                                    var res = item.Invoke(GlobalObjectJsNet, listParam);
                                    if (res != null)
                                    {
                                        var mStr = Marshal.StringToHGlobalUni(res.ToString());
                                        return BlinkBrowserPInvoke.jsStringW(es, mStr);//返回JS字符串
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                            }
                            else
                            {
                                var res = item.Invoke(GlobalObjectJsNet, null);
                                if (res != null)
                                {
                                    var mStr = Marshal.StringToHGlobalUni(res.ToString());
                                    return BlinkBrowserPInvoke.jsStringW(es, mStr);//返回JS字符串
                                }
                            }
                            return param;
                        });
                        BlinkBrowserPInvoke.wkeJsBindFunction(item.Name, jsnav, IntPtr.Zero, (uint)item.GetParameters().Length);
                        listObj.Add(jsnav);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("BindJsFunc(Object win) got error:{0}", ex.Message));
            }
        }

        #region "属性或方法组"
        /// <summary>
        /// 显示开发人员工具
        /// </summary>
        /// <param name="path"></param>
        public void ShowDevtools(string path)
        {
            BlinkBrowserPInvoke.wkeShowDevtools(this.handle, path, _wkeOnShowDevtoolsCallback, IntPtr.Zero);
        }


        /// <summary>
        /// 显示开发人员工具,无参数
        /// </summary>
        public void ShowDevtools()
        {
            //try
            //{
            //    string strFolder = String.Format("{0}{1}", Application.StartupPath, "\\plugins\\front_end");
            //    //如果目录不存在，则检查是否存在front_end.zip，有就解压
            //    if (Directory.Exists(strFolder) == false)
            //    {
            //        String zipPath = String.Format("{0}{1}", Application.StartupPath, "\\plugins\\front_end.zip");
            //        string zipOutput = string.Format("{0}{1}", Application.StartupPath, "\\plugins");
            //        if (System.IO.File.Exists(zipPath))
            //        {
            //            using (var sm = new FileStream(zipPath, FileMode.Open, FileAccess.Read))
            //            {
            //                using (var zip = ZipFile.Read(sm))
            //                {
            //                    zip.ExtractAll(string.Format("{0}{1}", Application.StartupPath, "\\plugins"));
            //                }
            //            }
            //        }
            //    }
            //    var path = Path.Combine(strFolder, "inspector.html");
            //    if (System.IO.File.Exists(path))
            //    {
            //        BlinkBrowserPInvoke.wkeShowDevtools(this.handle, path, null, IntPtr.Zero);
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
        }

        /// <summary>
        /// 页面是否加载失败
        /// </summary>
        public bool IsLoadingFailed
        {
            get
            {
                if (handle != IntPtr.Zero)
                {
                    return BlinkBrowserPInvoke.wkeIsLoadingFailed(handle);
                }
                return false;
            }
        }
        /// <summary>
        /// 页面是否加载成功
        /// </summary>
        public bool IsLoadingSucceeded
        {
            get
            {
                if (handle != IntPtr.Zero)
                {
                    return BlinkBrowserPInvoke.wkeIsLoadingSucceeded(handle);
                }
                return false;
            }
        }
        public bool IsLoadingCompleted
        {
            get
            {
                if (handle != IntPtr.Zero)
                {
                    return BlinkBrowserPInvoke.wkeIsLoadingCompleted(handle);
                }
                return false;
            }
        }
        //网页呈现时缩放
        public float _ZoomFactor { get; set; }
        [Browsable(false), DefaultValue(1.0)]
        public float ZoomFactor
        {
            get
            {
                if (this.handle == IntPtr.Zero) {
                    return this._ZoomFactor;
                }
                return MiniBlinkPinvoke.BlinkBrowserPInvoke.wkeGetZoomFactor(this.handle);
            }
            set
            {
                if (value >= 0.5 && value <= 4.0)
                {
                    this._ZoomFactor = value;
                    if (this.handle != IntPtr.Zero)
                    {
                        MiniBlinkPinvoke.BlinkBrowserPInvoke.wkeSetZoomFactor(this.handle, value);
                    }
                }

            }
        }
        //public string JsGetValue { get; set; }
        /// <summary>
        /// 获取或设置媒体音量
        /// </summary>
        [Browsable(false), DefaultValue(1.0)]
        public float MediaVolume
        {
            get 
            {
                if (!IsDesignMode())
                {
                    return BlinkBrowserPInvoke.wkeGetMediaVolume(this.handle);
                } 
                else
                {
                    return 0;
                }
            }
            set 
            {   if(value>=0.0 && value<=1.25)  {
                    BlinkBrowserPInvoke.wkeSetMediaVolume(this.handle, value);
                }                
            }
        }
        //内部方法测试开始
        [JSFunction]
        public void Console_WriteLine(string msg)
        {
            MessageBox.Show("Console_WriteLine 方法被调用了：" + msg);
        }
        [JSFunction]
        public void Console_WriteLine2(int msg, string msg2)
        {
            MessageBox.Show("Console_WriteLine w 方法被调用了：" + msg2 + " " + msg);
        }
        [JSFunction]
        public object Func1(int n1, int n2)
        {
            MessageBox.Show(string.Format("BlinkBrowser 内部 Func1 方法被调用了,参数1：{0}，参数2：{1}", n1, n2));
            return "结果是：" + (n1 * n2);
        }
        //内部方法测试结束 

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }

        private void DestroyEvent()
        {
            if (!IsDesignMode())
            {
                try
                {
                    _wkeOnShowDevtoolsCallback=null;
                    AlertBoxCallback = null;
                    titleChangeCallback = null;
                    titleChangeCallback2 = null;
                    _wkeNavigationCallback = null;
                    _wkeConsoleMessageCallback = null;
                    _wkePaintUpdatedCallback = null;
                    _wkeDocumentReadyCallback = null;
                    _wkeDocumentReady2Callback = null;
                    _wkeLoadingFinishCallback = null;
                    _wkeDownloadCallback = null;
                    _wkeDownload2Callback = null;
                    _wkeCreateViewCallback = null;
                    _wkeLoadUrlBeginCallback = null;
                    _wkeLoadUrlEndCallback = null;
                    _wkeOnNetGetFavicon = null;
                    _wkeGetTempCallbackInfo = null;
                    _wkeNetJobDataRecvCallback = null;
                    _wkeNetJobDataFinishCallback = null;
                }
                catch
                {

                }
                try
                {
                    //标题改变事件回调
                    OnTitleChangeCall=null;
                    //鼠标滑过URL事件回调
                    OnMouseOverUrlChanged = null;
                    //URL改变事件
                    urlChangedCallback = null;
                    OnUrlChangeCall = null;
                    urlChangedCallback2 = null;                    
                    OnUrlChange2Call = null;
                    //导航事件
                    OnUrlNavigationCall = null;
                    //DOM加载事件
                    DocumentReadyCallback = null;
                    DocumentReady2Callback = null;
                    //链接点击后创建View事件
                    OnCreateViewEvent = null;
                    //注册View初始化回调，通知调用方初始化Net函数
                    OnViewInitEvent = null;
                    //URL end 回调通知
                    OnUrlEndEvent = null;
                    //下载事件回调通知
                    OnDownloadFile = null;
                    //net提供回调接口，返回数据
                    OnDownloadFile2 = null;
                    //获取图标回调通知，url，图标地址，bytes图标数据，len图标数据长度
                    OnNetGetFaviconEvent = null;
                    //获取网页源码回调
                    OnNetGetSourceEvent = null;
                    OnNetGetSourceOnlyUrlEvent = null;
                }
                catch
                {

                }
            }
        }


        private void DestroyCallback()
        {
            if (!IsDesignMode())
            {
                //BlinkBrowserPInvoke.OutputDebugStringA("DestroyCallback");

                BlinkBrowserPInvoke.wkeOnAlertBox(handle, null, IntPtr.Zero);
                BlinkBrowserPInvoke.wkeOnNavigation(handle, null, IntPtr.Zero);
                BlinkBrowserPInvoke.wkeOnTitleChanged(handle, null, IntPtr.Zero);
                BlinkBrowserPInvoke.wkeOnMouseOverUrlChanged(handle, null, IntPtr.Zero);
                BlinkBrowserPInvoke.wkeOnDocumentReady2(handle, null, IntPtr.Zero);
                BlinkBrowserPInvoke.wkeOnURLChanged(handle, null, IntPtr.Zero);
                BlinkBrowserPInvoke.wkeOnURLChanged2(handle, null, IntPtr.Zero);
                BlinkBrowserPInvoke.wkeOnConsole(handle, null, IntPtr.Zero);
                BlinkBrowserPInvoke.wkeOnPaintUpdated(handle, null, IntPtr.Zero);
                BlinkBrowserPInvoke.wkeOnLoadingFinish(handle, null, IntPtr.Zero);
                BlinkBrowserPInvoke.wkeOnDownload2(handle, null, IntPtr.Zero);
                BlinkBrowserPInvoke.wkeOnCreateView(handle, null, IntPtr.Zero);
                BlinkBrowserPInvoke.wkeOnLoadUrlBegin(handle, null, IntPtr.Zero);
                BlinkBrowserPInvoke.wkeOnLoadUrlEnd(handle, null, IntPtr.Zero);
            }
        }


        protected override void OnHandleDestroyed(EventArgs e)
        {
            //BlinkBrowserPInvoke.OutputDebugStringA("OnHandleDestroyed");
            Enabled = false;

            //Application.AddMessageFilter(this);
            try {
                Application.RemoveMessageFilter(this);
            } catch {
                //do something
            }            

            try
            {
                if (this.bHandle!=IntPtr.Zero )
                {
                    //防止页面刷新时崩溃
                    BlinkBrowserPInvoke.wkeSetHandle(this.bHandle, this.cHandle );
                    BlinkBrowserPInvoke.wkeSetHandleOffset(this.bHandle , 0, 0);
                }
                else
                {
                    //防止页面刷新时崩溃
                    BlinkBrowserPInvoke.wkeSetHandle(IntPtr.Zero, IntPtr.Zero);
                    BlinkBrowserPInvoke.wkeSetHandleOffset(handle, 0, 0);
                    //BlinkBrowserPInvoke.wkeSetTransparent(handle, true);
                    //是否开启插件
                    BlinkBrowserPInvoke.wkeSetNpapiPluginsEnabled(handle, false);
                    //是否开启内存缓存
                    BlinkBrowserPInvoke.wkeSetMemoryCacheEnable(handle, false);
                }
            }
            catch
            {

            }

            //菜单为空
            //if (OnLinkMenuStrip.Items.Count>0)
            //{
            //    OnLinkMenuStrip.Items.Clear();
            //}
            //OnLinkMenuStrip.Opening -= OnLinkMenuStrip_Opening;
            OnLinkMenuStrip = null;
            //if (DocumentMenuStrip.Items.Count > 0)
            //{
            //    DocumentMenuStrip.Items.Clear();
            //}
            //DocumentMenuStrip.Opening -= DocumentMenuStrip_Opening;
            DocumentMenuStrip = null;

            ContextMenu = null;
            //释放回调函数
            try {
                DestroyEvent();
                DestroyCallback();
            } catch {
                //do something
            }
            //释放截图内存
            try
            {
                if (snapbits != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(snapbits);
                }
                if (bits != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(bits);
                }
                if (GlobalObjectJs != null)
                {
                    GlobalObjectJs = null;
                }
                if (GlobalObjectJsNet != null)
                {
                    GlobalObjectJsNet = null;
                }
                if (mywebClient != null)
                {
                    mywebClient.Dispose();
                    mywebClient = null;
                }
            }
            catch(Exception ex)
            {

            }
            
            //释放回调
            //BlinkBrowserPInvoke.wkeInitialize();

            BlinkBrowserPInvoke.wkeDestroyWebView(handle);

            //BlinkBrowserPInvoke.OutputDebugStringA(string.Format("wkeDestroyWebView后，handle的值为“{0}", handle.ToString() ));

            handle = IntPtr.Zero;

            try
            {
                listObj.Clear();
                listObj = null;
            }
            catch (Exception ex)
            {

            }

            try {
                GC.Collect();
                //GC.WaitForPendingFinalizers();
                GC.SuppressFinalize(this);
            } catch {
                //do something
            }

            base.OnHandleDestroyed(e);
        }

        /// <summary>
        /// 获取域名的顶级域名
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public string GetTopDomainName(string domain)
        {
            //http://www.baidu.com/  
            domain = domain.Trim().ToLower();
            if (domain.StartsWith("http://")) domain = domain.Replace("http://", "");
            if (domain.StartsWith("https://")) domain = domain.Replace("https://", "");
            if (domain.StartsWith("www.")) domain = domain.Replace("www.", "");
            //baidu.com/  
            if (domain.IndexOf("/") > 0) {
                domain = domain.Substring(0, domain.IndexOf("/"));
            }
            string[] wildcards;
            wildcards = domain.Split('.');
            //MessageBox.Show(string.Format("dns:{0}, split length:{1}", domain, wildcards.Length));
            //直接扔掉DNS的第一段
            if (wildcards.Length ==4) {
                return string.Format("{0}.{1}.{2}", wildcards[1], wildcards[2], wildcards[3]) ;
            }else if(wildcards.Length == 3) {
                return string.Format("{0}.{1}", wildcards[1], wildcards[2]);
            }else if (wildcards.Length == 2)  {
                return string.Format("{0}.{1}", wildcards[0], wildcards[1]);
            }            
            return "";
        }


        /// <summary>
        /// 获取当前是否处于设计器模式
        /// </summary>
        /// <remarks>
        /// 在程序初始化时获取一次比较准确，若需要时获取可能由于布局嵌套导致获取不正确，如GridControl-GridView组合。
        /// </remarks>
        /// <returns>是否为设计器模式</returns>
        private bool IsDesignMode()
        {
            return (this.DesignMode);
            //return (this.GetService(typeof(System.ComponentModel.Design.IDesignerHost)) != null || LicenseManager.UsageMode == LicenseUsageMode.Designtime);
        }

        //清除所有Cookie
        public void ClearCookie()
        {
            try
            {
                BlinkBrowserPInvoke.wkePerformCookieCommand(handle, wkeCookieCommand.ClearAllCookies);
                BlinkBrowserPInvoke.wkePerformCookieCommand(handle, wkeCookieCommand.ClearSessionCookies);
            }
            catch (Exception ex)
            {

            }
        }

        //创建指定标题及网址的弹出窗口
        public void CreatePopupWindow(string url, string caption)
        {
            if (url.Length <= 0) {
                return;
            }
            if (caption.Length > 255) {
                MessageBox.Show(string.Format("创建弹出窗口失败，错误信息:参数 caption 长度超长！"));
                return;
            }
            try
            {
                int newLeft = 0;
                int newTop = 0;
                try
                {
                    newTop = (Screen.PrimaryScreen.Bounds.Height - 30 - this.Height) / 2;
                    newLeft = (Screen.PrimaryScreen.Bounds.Width - 10 - this.Width) / 2;
                }
                catch(Exception ex)
                {
                    newTop = this.Top;
                    newLeft = this.Left;
                }
                //创建BlinkBrowser弹出窗口
                IntPtr newWindow = BlinkBrowserPInvoke.wkeCreateWebWindow(wkeWindowType.WKE_WINDOW_TYPE_POPUP, IntPtr.Zero, newLeft, newTop, this.Width+10, this.Height+32);
                if(newWindow != IntPtr.Zero) {
                    BlinkBrowserPInvoke.wkeShowWindow(newWindow, true);
                    BlinkBrowserPInvoke.wkeSetWindowTitleW(newWindow, caption.Length <= 0 ?"正常窗口":caption ) ;
                    BlinkBrowserPInvoke.wkeLoadURLW(newWindow, url);
                    //设置窗口图标,从项目资源文件中读取  Properties.Resources.Key 同样可以读取到项目资源文件中的值
                    Icon winicon = MiniBlink.Properties.Resources.winnotrace as Icon;
                    if (winicon != null) {
                        //发送设置图标消息
                        //MessageBox.Show(string.Format("msg:{0},ico:{1}", BlinkBrowserPInvoke.WM_SETICON, BlinkBrowserPInvoke.ICON_SMALL));
                        int ret0 = BlinkBrowserPInvoke.SendMessage(newWindow, BlinkBrowserPInvoke.WM_SETICON, IntPtr.Zero, winicon.Handle);
                        int ret1 = BlinkBrowserPInvoke.SendMessage(newWindow, BlinkBrowserPInvoke.WM_SETICON, new IntPtr(1), winicon.Handle);
                        //int ret2 = BlinkBrowserPInvoke.SendMessage(newWindow.ToInt32(), BlinkBrowserPInvoke.WM_PAINTICON, 0, 0);

                        //MessageBox.Show(string.Format("SendMessage return value:{0}", ret));
                        //MessageBox.Show(string.Format("GetLastWin32Error return value:{0}", Marshal.GetLastWin32Error()));
                    }
                    //
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show( string.Format("创建弹出窗口失败，错误信息:{0}", ex.Message));
            } 
        }


        /// <summary>
        /// 抓取WebView快照保存为Bitmap（截图功能）
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Bitmap SnapshotToBitmap()
        {
            if (this.handle == IntPtr.Zero)
            {
                return null;
            }
            IntPtr prndata = IntPtr.Zero;
            //申请内存
            wkeMemBuf bufs = new wkeMemBuf();
            try
            {
                //打印到Bitmap
                //获取WebView排版后的高/宽度值
                int cw = BlinkBrowserPInvoke.wkeGetContentWidth(this.handle);
                int ch = BlinkBrowserPInvoke.wkeGetContentHeight(this.handle);
                //截图参数设置
                wkeScreenshotSettings tmpset = new wkeScreenshotSettings();
                tmpset.width = cw;
                tmpset.height = ch;

                //打印到Bitmap，返回数据指针
                //隐藏滚动条或其它飘浮控件
                //BlinkBrowserPInvoke.wkeRunJSW(this.handle, @"document.body.style.overflow='hidden'");
                IntPtr mainFrameId = BlinkBrowserPInvoke.wkeWebFrameGetMainFrame(handle);
                prndata = BlinkBrowserPInvoke.wkePrintToBitmap(this.handle, mainFrameId, tmpset);
                if (prndata != IntPtr.Zero)
                {
                    //转换为申请的内存结构
                    bufs = (wkeMemBuf)Marshal.PtrToStructure(prndata, typeof(wkeMemBuf));
                    //如果数据长度小于等于0，则返回null
                    if (bufs.length <= 0)
                    {
                        return null;
                    }
                    try
                    {
                        byte[] bmpArray = new byte[bufs.length];
                        Marshal.Copy(bufs.data, bmpArray, 0, bufs.length);
                        if (bmpArray.Length == bufs.length)
                        {
                            //一定要调用释放内存的API
                            BlinkBrowserPInvoke.wkeFreeMemBuf(prndata);
                        }
                        //将字节数据转换为Bitmap
                        using (MemoryStream bmpms = new MemoryStream(bmpArray))
                        {
                            using (System.Drawing.Bitmap bmtmp = new System.Drawing.Bitmap(bmpms))
                            {
                                bmpArray = new byte[1];
                                bufs = new wkeMemBuf(); 
                                return new System.Drawing.Bitmap(bmtmp);
                            };
                        };
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("将打印数据转换为BMP位图发生错误，错误信息:{0}", ex.Message));
                        return null;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("截图时发生错误，错误信息:{0}", ex.Message));
                return null;
            }
        }



        /// <summary>
        /// 将网页另存为MHTML文件
        /// </summary>
        /// <param name="filename"></param>
        public void SavetoMHTML(string filename)
        {
            try
            {
                IntPtr srcSource = IntPtr.Zero;
                srcSource = BlinkBrowserPInvoke.wkeUtilSerializeToMHTML(handle);

                if(srcSource != IntPtr.Zero)  {

                    string strMhtmlCode = string.Empty;
                    strMhtmlCode = BlinkBrowserPInvoke.Utf8IntptrToString(srcSource);

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Title = "另存为";
                    saveFileDialog.Filter = "mhtml文件|*.mhtml";
                    saveFileDialog.FileName = "";
                    
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)  {

                        if (!string.IsNullOrEmpty(saveFileDialog.FileName))  {

                            MessageBox.Show(strMhtmlCode);
                            File.WriteAllText(saveFileDialog.FileName, strMhtmlCode);
                        }
                    }
                    strMhtmlCode = string.Empty;
                }                
            } catch(Exception ex) {
                //do something
            }
        }


        // 获取网页源码，如果启用了gzip压缩后页面获取会产生乱码，采用此方法可解决gzip压缩而产生的乱码情况
        private string GetHtmlCode(string url)
        {
            string htmlCode = string.Empty ;
            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                if (webRequest == null) {
                    return string.Empty;
                }
                webRequest.Timeout = 30000;
                webRequest.Method = "GET";
                webRequest.UserAgent = "Mozilla/4.0";
                webRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
                System.Net.HttpWebResponse webResponse = (System.Net.HttpWebResponse)webRequest.GetResponse();
                if (webResponse.ContentEncoding.ToLower() == "gzip")//如果使用了GZip则先解压            
                {
                    using (System.IO.Stream streamReceive = webResponse.GetResponseStream())
                    {
                        using (var zipStream = new System.IO.Compression.GZipStream(streamReceive, System.IO.Compression.CompressionMode.Decompress))
                        {
                            using (StreamReader sr = new System.IO.StreamReader(zipStream, Encoding.Default))
                            {
                                htmlCode = sr.ReadToEnd();
                            }
                        }
                    }
                }
                else
                {
                    using (System.IO.Stream streamReceive = webResponse.GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(streamReceive, Encoding.Default))
                        {
                            htmlCode = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }            
 
            return htmlCode;
        }


        public void StopLoading()
        {
            try
            {
                if(handle != IntPtr.Zero)
                {
                    BlinkBrowserPInvoke.wkeStopLoading(handle);
                }                
            }
            catch (Exception)
            {
                //throw;
            }
        }

        public void Reload()
        {
            try
            {
                if (handle != IntPtr.Zero)
                {
                    BlinkBrowserPInvoke.wkeReload(handle);
                }
            }
            catch (Exception ex)
            {
                BlinkBrowserPInvoke.OutputDebugStringA(string.Format("刷新时发生错误,Error:{0}", ex.Message));
                //throw;
            }
        }

        /// <summary>
        /// 网页后退
        /// </summary>
        public void GoBack()
        {
            try
            {
                if (handle != IntPtr.Zero)
                {
                    BlinkBrowserPInvoke.wkeGoBack(handle);
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }

        /// <summary>
        /// 网页前进
        /// </summary>
        public void GoForward()
        {
            try
            {
                if (handle != IntPtr.Zero)
                {
                    BlinkBrowserPInvoke.wkeGoForward(this.handle);
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }

        public void SafeInvoke(Action<object> callback, object state = null)
        {
            if (IsDisposed)
            {
                return;
            }
            if (InvokeRequired)
            {
                Invoke(callback, state);
            }
            else
            {
                callback(state);
            }
        }


        public IntPtr bHandle
        {
            get { return pbHandle; }
            set
            {
                pbHandle = value;                
            }
        }


        public IntPtr cHandle
        {
            get { return pcHandle; }
            set
            {
                pcHandle = value;
            }
        }


        /// <summary>
        /// 设置句柄，获取消息
        /// </summary>
        /// <param name="phandle"></param>
        public void SetHandle(IntPtr phandle)
        {
            try
            {
                if (handle != IntPtr.Zero)
                {
                    BlinkBrowserPInvoke.wkeSetHandle(phandle, this.Handle);
                    BlinkBrowserPInvoke.wkeSetHandleOffset(phandle, Location.X - 2, 0);
                }
            }
            catch (Exception ex)
            {

            } 
        }

        public bool PreFilterMessage(ref Message m)
        {
            IntPtr myPtr = BlinkBrowserPInvoke.GetForegroundWindow();

            int length = BlinkBrowserPInvoke.GetWindowTextLength(myPtr);
            StringBuilder windowName = new StringBuilder(length + 1);
            BlinkBrowserPInvoke.GetWindowText(myPtr, windowName, windowName.Capacity);
            if (windowName.ToString() == "Miniblink Devtools")
            {
                int wp = m.WParam.ToInt32();
                if (wp == 13/*回车*/ || wp == 37/*左 */ || wp == 39/*右*/ || wp == 38/*上*/ || wp == 40/*下*/)
                {
                    BlinkBrowserPInvoke.SendMessage(m.HWnd, m.Msg, m.WParam, m.LParam);
                }
                if (m.Msg == 258)//字符
                {
                    BlinkBrowserPInvoke.SendMessage(m.HWnd, m.Msg, m.WParam, m.LParam);
                }
            }
            return false;
        }

        /// <summary>
        /// 解析 cookies.dat文件得到Cookie,没有判断 path，只有 域的判断
        /// </summary>
        public string GetCookiesFromFile
        {
            get
            {
                StringBuilder sbCookie = new StringBuilder();
                if (File.Exists(CookiePath + "\\cookies.dat"))
                {

                    var uri = new Uri(Url);
                    var host = uri.Host;

                    var allCookies = File.ReadAllLines(CookiePath + "\\cookies.dat");
                    for (int i = 4; i < allCookies.Length; i++)
                    {
                        host = uri.Host;
                        var listCookie = allCookies[i].Split('\t');
                        if (listCookie != null && listCookie.Length != 0 && listCookie.Length == 7)
                        {
                            var _cookie = listCookie[0];

                        Lable:
                            if (_cookie == host)
                            {
                                sbCookie.AppendFormat("{0}={1};", listCookie[5], listCookie[6]);
                            }
                            //httponly
                            var httpOnly = "#HttpOnly_" + host;
                            if (_cookie == httpOnly)
                            {
                                sbCookie.AppendFormat("{0}={1};", listCookie[5], listCookie[6]);
                            }
                            if (host.IndexOf('.') == 0)// . 开头
                            {
                                host = host.Substring(host.IndexOf('.') + 1);//. 开头 去掉 .
                                goto Lable;
                            }
                            else
                            {
                                if (host.TrimStart('.').Split('.').Length > 2)
                                {
                                    host = host.Substring(host.IndexOf('.'));//带 . 
                                    goto Lable;
                                }
                            }
                        }

                    }
                }
                return sbCookie.ToString();
            }
        }

        /// <summary>
        /// 获取或设置网页源码
        /// </summary>
        public string HTML
        {
            get
            {
                if (handle != IntPtr.Zero)
                {
                    return InvokeJSW("return document.documentElement.outerHTML").ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (handle != IntPtr.Zero)
                {
                    BlinkBrowserPInvoke.wkeLoadHTMLW(this.handle, value);
                }
            }
        }


        public string Url
        {
            get { return url; }
            set
            {
                url = value;
                if (handle != IntPtr.Zero)
                {
                    try
                    {
                        BlinkBrowserPInvoke.wkeLoadURLW(handle, url);
                    }
                    catch (Exception)
                    {
                        //throw;
                    }                    
                }
            }
        }

        public bool CanGoBack
        {
            get
            {
                if (handle != IntPtr.Zero)
                {
                    return BlinkBrowserPInvoke.wkeCanGoBack(this.handle);
                }
                else
                {
                    return false;
                }
            }
        }


        public bool CanGoForward
        {
            get
            {
                if (handle != IntPtr.Zero)
                {
                    return BlinkBrowserPInvoke.wkeCanGoForward(this.handle);
                }
                else
                {
                    return false;
                }
            }
        }


        public bool ViewSourceType
        {
            get
            {
                return ViewSourceOnlyUrl;
            }
            set
            {
                ViewSourceOnlyUrl = value;
            }
        }

        public bool EnableContextMenu
        {
            get
            {
                return IsEnableContextMenu;
            }
            set
            {
                IsEnableContextMenu = value;
            }
        }


        public string Cookies
        {
            get
            {
                if (handle!=IntPtr.Zero)
                {
                    return BlinkBrowserPInvoke.Utf8IntptrToString(BlinkBrowserPInvoke.wkeGetCookie(handle));
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        #endregion
    }

}
