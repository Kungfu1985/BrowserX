using BrowserX.Properties;
using MiniBlink.WebClientX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrowserX.UC
{
    public class myDownloadToolStrip: ToolStripButton
    {

        //
        System.Windows.Forms.Timer tmPercentage = new System.Windows.Forms.Timer();
        Point pMouseMove = new Point(0, 0);
        Point pMouseDown = new Point(0, 0);
        private bool IsMouseMove = false;
        private bool bolIsStoped = false;
        //下载相关的参数
        // 网络下载组件
        private WebClientX webcDownloading = new WebClientX(); //WebClient
        private long longCurBytes = 0;                         //当前下载字节数
        private long longPrevBytes = 0;                        //前一秒下载的字节数 
        private bool bolDownloadingFlag = false;               //是否开启了下载   
        private bool bolChuckDowningFlag = false;
        private bool bolChuckDownCancelFlag = false;
        private System.Windows.Forms.Timer timDownSpeedTask = new System.Windows.Forms.Timer();
        //显示提示信息
        private ToolTip toolTip1 = new ToolTip();
        //
        private bool bolItemDestroyed = false; 
        //end


        public myDownloadToolStrip(): base()
        {
            //
            InitTimer();
            this.MouseEnter += MyToolStripButton_MouseEnter;
            this.MouseLeave += MyToolStripButton_MouseLeave;
            this.MouseHover += MyToolStripButton_MouseHover;
            this.MouseMove += MyToolStripButton_MouseMove;
        }
        

        public myDownloadToolStrip(string text) : base()
        {
            //
            InitTimer();
            this.Text = text;
            this.MouseEnter += MyToolStripButton_MouseEnter;
            this.MouseLeave += MyToolStripButton_MouseLeave;
            this.MouseHover += MyToolStripButton_MouseHover;
            this.MouseMove += MyToolStripButton_MouseMove;
        }

        ~myDownloadToolStrip()
        {
            //
            //UnInitTimer();
            this.MouseEnter -= MyToolStripButton_MouseEnter;
            this.MouseLeave -= MyToolStripButton_MouseLeave;
            this.MouseHover -= MyToolStripButton_MouseHover;
            this.MouseMove -= MyToolStripButton_MouseMove;
        }

        private void InitTimer()
        {
            try
            {
                //添加Tooltip
                if (toolTip1 != null)
                {
                    toolTip1.AutoPopDelay = 5000;
                    toolTip1.InitialDelay = 1000;
                    toolTip1.ReshowDelay = 5000;
                    toolTip1.ShowAlways = false;
                }
            }
            catch (Exception ex)
            {

            }
            try
            {
                //if (tmPercentage != null)
                //{
                //    tmPercentage.Interval = 100;
                //    tmPercentage.Tick += TmPercentage_Tick;
                //    tmPercentage.Enabled = false;
                //    tmPercentage.Stop();
                //}
            }
            catch(Exception ex)
            {

            }
        }

        private void TmPercentage_Tick(object sender, EventArgs e)
        {
            try
            {
                //if(this.ProgressPercentage >= 100)
                //{
                //    this.ProgressPercentage = 1;
                //}
                //else
                //{
                //    this.ProgressPercentage += 1;
                //}
            }
            catch(Exception ex)
            {

            }
            //throw new NotImplementedException();
        }

        private void UnInitTimer()
        {
            if (tmPercentage != null)
            {
                //tmPercentage.Stop();
                //tmPercentage.Enabled = false;

                //tmPercentage.Tick -= TmPercentage_Tick;
                //tmPercentage.Dispose();
                //tmPercentage = null;
            }
            //Win32API.OutputDebugStringA("停止时钟");
            //System.Windows.Forms.Timer tmWatchNewUrl = new System.Windows.Forms.Timer();
        }

        private void MyToolStripButton_MouseHover(object sender, EventArgs e)
        {
            State = DrawItemState.HotLight;
            this.Invalidate();
            //Win32API.OutputDebugStringA(string.Format("项：{0},鼠标悬停！", Text));
            //throw new NotImplementedException();
        }

        private void MyToolStripButton_MouseLeave(object sender, EventArgs e)
        {
            State = DrawItemState.Default;
            IsMouseMove = false;
            this.Invalidate(); 
            //Win32API.OutputDebugStringA(string.Format("项：{0},鼠标移出！", Text));
            //throw new NotImplementedException();
        }

        private void MyToolStripButton_MouseEnter(object sender, EventArgs e)
        {
            State = DrawItemState.HotLight;
            this.Invalidate();
            //Win32API.OutputDebugStringA(string.Format("项：{0},鼠标移入！", Text)); 
            //throw new NotImplementedException();
        }

        private void MyToolStripButton_MouseMove(object sender, MouseEventArgs e)
        {
            //this.Invalidate();
            IsMouseMove = true;
            this.Invalidate();
            //Win32API.OutputDebugStringA("MyToolStripButton_MouseMove"); 
            //throw new NotImplementedException();
        }

        #region "公开属性"
        // 下拉列表中项选中时的的背景
        private Color _SelectedColor = Color.FromArgb(255, 229, 229, 229);
        [Description("下拉列表中项选中时的的背景。")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "229, 229, 229")]
        [Category("Settings")]
        public Color SelectedColor
        {
            get
            {
                return _SelectedColor;
            }

            set
            {
                _SelectedColor = value;

                this.Invalidate();
            }
        }
        // 下拉列表中项鼠标悬停时的的背景
        private Color _HoverColor = Color.FromArgb(255, 218, 218, 218);
        [Description("下拉列表中项鼠标悬停时的的背景。")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "218, 218, 218")]
        [Category("Settings")]
        public Color HoverColor
        {
            get
            {
                return _HoverColor;
            }

            set
            {
                _HoverColor = value;

                this.Invalidate();
            }
        }


        // 下拉列表中项未选中时的的背景
        private Color _NormalColor = Color.FromArgb(255, 240, 240, 240);
        [Description("下拉列表中项未选中时的的背景。")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "240, 240, 240")]
        [Category("Settings")]
        public Color NormalColor
        {
            get
            {
                return _NormalColor;
            }

            set
            {
                _NormalColor = value;

                this.Invalidate();
            }
        }

        //下载文件名文本颜色
        private Color _TextColor = Color.FromArgb(255, 57, 55, 56);
        [Description("下载文件名文本颜色。")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "57, 55, 56")]
        [Category("Settings")]
        public Color TextColor
        {
            get
            {
                return _TextColor;
            }

            set
            {
                _TextColor = value;

                this.Invalidate();
            }
        }

        //下载文件名文本字体及大小
        private Font _TextFont = new Font("微软雅黑",10) ;
        [Description("下载文件名文本字体及大小。")]
        [Browsable(true)]
        [DefaultValue(typeof(Font), "微软雅黑, 10pt")]
        [Category("Settings")]
        public Font TextFont
        {
            get
            {
                return _TextFont;
            }

            set
            {
                _TextFont = value;

                this.Invalidate();
            }
        }

        //下载速度提示文本的字体及大小
        private Font _SpeedTextFont = new Font("微软雅黑", 9);
        [Description("下载速度提示文本的字体及大小。")]
        [Browsable(true)]
        [DefaultValue(typeof(Font), "微软雅黑, 9pt")]
        [Category("Settings")]
        public Font SpeedTextFont
        {
            get
            {
                return _SpeedTextFont;
            }

            set
            {
                _SpeedTextFont = value;

                this.Invalidate();
            }
        }

        //下载速度文本颜色
        private Color _SpeedTextColor = Color.FromArgb(255, 155, 155, 155);
        [Description("下载速度提示文字的字体颜色。")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "155, 155, 155")]
        [Category("Settings")]
        public Color SpeedTextColor
        {
            get
            {
                return _SpeedTextColor;
            }

            set
            {
                _SpeedTextColor = value;

                this.Invalidate();
            }
        }
        //列表项分隔符颜色
        private Color _SeparateColor = Color.FromArgb(255, 223, 223, 223);
        [Description("列表项分隔符颜色。")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "223, 223, 223")]
        [Category("Settings")]
        public Color SeparateColor
        {
            get
            {
                return _SeparateColor;
            }

            set
            {
                _SeparateColor = value;

                this.Invalidate();
            }
        }


        // 下拉列表是否是被选中的项
        private bool _IsSelected = false;
        private int _SelectedIndex = -1;
        [Description("下拉列表是否是被选中的项。")]
        [Browsable(true)]
        [DefaultValue(typeof(bool), "false")]
        [Category("Settings")]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }

            set
            {
                _IsSelected = value;

                this.Invalidate();
            }
        }

        //当前的状态值
        private string _DownloadSpeed = "";
        [Description("当前项的下载速度。")]
        [Browsable(true)]
        [DefaultValue(typeof(string), "")]
        [Category("Settings")]
        public string DownloadSpeed
        {
            get
            {
                return _DownloadSpeed;
            }

            set
            {
                _DownloadSpeed = value;

                this.Invalidate();
            }
        }
        //当前任务下载百分比

        private int _ProgressPercentage = 0;
        [Description("当前项的已下载百分比。")]
        [Browsable(true)]
        [DefaultValue(typeof(int), "0")]
        [Category("Settings")]
        public int ProgressPercentage
        {
            get
            {
                return _ProgressPercentage;
            }

            set
            {
                _ProgressPercentage = value;

                this.Invalidate();

                if (PercentageChanged != null)
                {
                    PercentageChanged(this.Bounds, value);
                }
            }
        }

        //开始下载
        private bool _StartDownload = false;
        [Description("是否开始下载。")]
        public bool StartDownload
        {
            get
            {
                return _StartDownload;
            }

            set
            {
                _StartDownload = value;

                try
                {
                    if (_StartDownload)
                    {
                        //开始下载
                        //如果WebClient不为空，则使用WebClient的异步方式下载文件到本地
                        if (webcDownloading != null)
                        {
                            if (timDownSpeedTask != null)
                            {
                                //第一次时让文件大小先显示，速度后面再显示
                                this.DownloadSpeed = this.DownSaveSize;
                                //
                                timDownSpeedTask.Enabled = true;
                                timDownSpeedTask.Start();

                                Win32API.OutputDebugStringA("启动了时钟。。。");
                            }
                            //开启异步下载
                            DownloadFileAsync(this.DownloadUrl);
                        }
                    }
                }
                catch(Exception ex)
                {
                    //do something
                }                
                this.Invalidate();
            }
        }


        // 当前项是否下载完成
        private bool _DownloadCompleted = false;
        [Description("当前项是否下载完成。")]
        public bool DownloadCompleted
        {
            get
            {
                return _DownloadCompleted;
            }

            set
            {
                _DownloadCompleted = value;
                if (_DownloadCompleted)
                {
                    try
                    {
                        timDownSpeedTask.Stop();
                        timDownSpeedTask.Enabled = false;
                    }
                    catch (Exception ex)
                    {

                    }
                }                
                this.Invalidate();
            }
        }


        //下载的URL        
        private string _DownloadUrl = "";
        [Description("当前下载项的URL路径。")]
        public string DownloadUrl
        {
            get
            {
                return _DownloadUrl;
            }

            set
            {
                try
                {
                    // '下载组件初始化
                    if (webcDownloading == null)
                    {
                        webcDownloading = new WebClientX();
                    }

                    if (webcDownloading != null)
                    {
                        // '移除下载进度改变事件
                        webcDownloading.DownloadProgressChanged -= DownloadProgressChanged;
                        webcDownloading.DownloadFileCompleted -= DownloadFileCompleted;
                        // '添加下载进度改变事件
                        webcDownloading.DownloadProgressChanged += DownloadProgressChanged;
                        webcDownloading.DownloadFileCompleted += DownloadFileCompleted;
                    }
                    // '如果
                    if (timDownSpeedTask != null)
                    {
                        // 主时钟，倒计时，移除下载事件
                        timDownSpeedTask.Tick -= timDownSpeedTask_Tick;
                        // '添加下载事件
                        timDownSpeedTask.Tick += timDownSpeedTask_Tick;
                        // 主时钟，每隔1秒触发一次
                        timDownSpeedTask.Interval = 1000;
                        timDownSpeedTask.Enabled = true;
                        timDownSpeedTask.Start();
                        Win32API.OutputDebugStringA("创建了时钟。。。");
                    }
                }
                catch (Exception ex)
                {
                }

                _DownloadUrl = value;

                this.Invalidate();
            }
        }

        //下载后保存的文件名
        private string _DownSaveFile = "";
        [Description("下载后保存的文件名，与DownSavePath组合成完整的本地保存路径。")]
        public string DownSaveFile
        {
            get
            {
                return _DownSaveFile;
            }

            set
            {
                if (_DownSaveFile != value)
                {
                    _DownSaveFile = value;
                    //显示下载的文件名
                    Text = value;                    
                }
            }
        }

        //下载后保存的路径
        private string _DownSavePath = "";
        [Description("下载后保存的路径")]
        public string DownSavePath
        {
            get
            {
                return _DownSavePath;
            }

            set
            {
                if (_DownSavePath != value)
                {
                    _DownSavePath = value;
                    //如果文件夹不存在，则创建
                    try
                    {
                        if (!Directory.Exists(value))
                            Directory.CreateDirectory(value);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        //保存的文件大小 23kb,45mb,1.2gb
        private string _DownSaveSize = "";
        public string DownSaveSize
        {
            get
            {
                return _DownSaveSize;
            }

            set
            {
                if (_DownSaveSize != value)
                {
                    _DownSaveSize = value;
                }
            }
        }

        //是否是暂停状态
        private bool _isstoped = false;
        public bool IsStoped
        {
            get
            {
                return _isstoped;
            }
            set
            {
                _isstoped = value;
                if (tmPercentage != null)
                {
                    if (_isstoped)
                    {
                        //tmPercentage.Enabled = false;
                        //tmPercentage.Stop();
                    }
                    else
                    {
                        //tmPercentage.Enabled = true;
                        //tmPercentage.Start();
                    }                   

                }
                //this.Invalidate();
            }
        }
        #endregion

        #region "公开事件"
        //当前项选中时，触发百分比改变事件
        public delegate void PercentageChangeEventHandler(Rectangle bounds, float Percentage);
        public event PercentageChangeEventHandler PercentageChanged;

        #endregion

        #region "自定义函数"
        /// <summary>
        ///     ''' 文本对齐方式
        ///     ''' </summary>
        ///     ''' <param name="textalign"></param>
        ///     ''' <returns></returns>
        private StringFormat SFAlignment(ContentAlignment textalign)
        {
            StringFormat sf = new StringFormat();
            switch ((textalign))
            {
                case ContentAlignment.TopLeft:         // test ok
                    {
                        sf.LineAlignment = StringAlignment.Near;
                        sf.Alignment = StringAlignment.Near;
                        break;
                    }

                case ContentAlignment.TopCenter:        // test ok
                    {
                        sf.LineAlignment = StringAlignment.Near;
                        sf.Alignment = StringAlignment.Center;
                        break;
                    }

                case ContentAlignment.TopRight:         // test ok
                    {
                        sf.LineAlignment = StringAlignment.Near;
                        sf.Alignment = StringAlignment.Far;
                        break;
                    }

                case ContentAlignment.MiddleLeft:
                    {
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Near;
                        break;
                    }

                case ContentAlignment.MiddleCenter:
                    {
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Center;
                        break;
                    }

                case ContentAlignment.MiddleRight:
                    {
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Far;
                        break;
                    }

                case ContentAlignment.BottomLeft:        // test ok
                    {
                        sf.LineAlignment = StringAlignment.Far;
                        sf.Alignment = StringAlignment.Near;
                        break;
                    }

                case ContentAlignment.BottomCenter:      // test ok
                    {
                        sf.LineAlignment = StringAlignment.Far;
                        sf.Alignment = StringAlignment.Center;
                        break;
                    }

                case ContentAlignment.BottomRight:       // test ok
                    {
                        sf.LineAlignment = StringAlignment.Far;
                        sf.Alignment = StringAlignment.Far;
                        break;
                    }
            }
            return sf;
        }


        /// <summary>
        /// 下载进度改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DownloadProgressChanged(object sender, MiniBlink.WebClientXEventArgs.DownloadProgressChangedEventArgs e)
        {
            try
            {
                //显示下载进度
                bolDownloadingFlag = true;
                this.ProgressPercentage = e.ProgressPercentage;

                //当前接受了多少字节数据
                longCurBytes = e.BytesReceived;
            }
            catch (Exception ex)
            {
                //myMsgbox(string.Format("实时下载进度出错，出错信息:{0}", ex.Message), "提示信息", MessageBoxButtons.OK, SystemIcons.Error);
            }
        }


        /// <summary>
        ///     ''' 下载完成事件
        ///     ''' </summary>
        ///     ''' <param name="sender"></param>
        ///     ''' <param name="e"></param>
        public void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    if(e.Error.ToString().Contains("(416)"))
                    {
                        this.DownloadCompleted = true;
                        //检查已下载文件大小与访问的大小是否相等，不相等则删除
                    }
                    else
                    {
                        this.DownloadCompleted = false;
                    }                    
                    //Win32API.OutputDebugStringA( string.Format("下载完成事件触发，但有错误 ：{0}",e.Error) );
                }
                else if (e.Cancelled)
                {
                    this.DownloadCompleted = false;
                    //Win32API.OutputDebugStringA(string.Format("下载完成事件触发，但：Cancelled={0}", e.Cancelled));
                }                    
                else
                {
                    bolDownloadingFlag = false;
                    //this.ProgressPercentage = 100;
                    this.DownloadCompleted = true;
                    //this.Invalidate();
                }
            }
            catch (Exception ex)
            {
                Win32API.OutputDebugStringA(string.Format("下载完成完成时出错，出错信息:{0}", ex.Message));
            }
        }


        /// <summary>
        /// 计算下载速度的时钟
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timDownSpeedTask_Tick(object sender, EventArgs e)
        {
            //倒计时时钟
            timDownSpeedTask.Enabled = false;
            timDownSpeedTask.Stop();
            //Win32API.OutputDebugStringA("计算下载速度的时钟正在运行。。。");
            //显示下载速度
            long longDiffSize = 0;
            double totalSize = 0.0D;
            string strspeed = string.Empty;
            try
            {
                longDiffSize = longCurBytes - longPrevBytes;

                if (longDiffSize >= 1073741824)
                {
                    totalSize = System.Math.Round(longDiffSize / (double)(1024.0F * 1024.0F * 1024.0F), 2);
                    strspeed = string.Format("{0} GB/S", (totalSize).ToString("0.00"));
                }
                else if (longDiffSize >= 1048576)
                {
                    totalSize = System.Math.Round(longDiffSize / (double)(1024.0F * 1024.0F), 2);
                    strspeed = string.Format("{0} MB/S", (totalSize).ToString("0.00"));
                }
                else
                {
                    totalSize = System.Math.Round(longDiffSize / (double)(1024.0F), 2);
                    strspeed = string.Format("{0} KB/S", (totalSize).ToString("0.00"));
                }
                longPrevBytes = longCurBytes;
                //如果没有下载完成才显示下载速度
                if (!this.DownloadCompleted)
                {
                    this.DownloadSpeed = string.Format("{0} - {1}", this.DownSaveSize, strspeed);
                }                
            }
            catch (Exception ex)
            {
                //Win32API.OutputDebugStringA(string.Format("计算下载速度时出错：{0}",ex.Message));
            }
            timDownSpeedTask.Enabled = true;
            timDownSpeedTask.Start();
        }



        /// <summary>
        /// 异步下载文件
        /// </summary>
        /// <param name="serverPath"></param>
        private void DownloadFileAsync(string serverPath)
        {
            if (serverPath.Length <= 0 || string.IsNullOrEmpty(serverPath))
            {
                return;
            }                
            if (webcDownloading == null)
            {
                return;
            }
                
            try
            {
                //将下载地址转换为Uri类
                System.Uri serverUri = new System.Uri(serverPath);
                //下载后保存的文件位置及名称
                string filePath = string.Empty;
                filePath = string.Format("{0}\\{1}", this.DownSavePath, this.DownSaveFile);
                //WebClient异步下载文件
                AsyncOperation asyncOp;
                //暂时没做暂停，继续下载功能，支持断点续传
                webcDownloading.DownloadFileAsync(serverUri, filePath);
            }
            // 'down(serverUri.OriginalString, 0, filePath)

            catch (Exception ex)
            {
                //myMsgbox(string.Format("下载文件时出错，出错信息:{0}", ex.Message), "提示信息", MessageBoxButtons.OK, SystemIcons.Error);
            }
        }


        /// <summary>
        /// 手动销毁当前项
        /// </summary>
        public void Destroyed()
        {
            try
            {
                if (timDownSpeedTask != null)
                {
                    //主时钟，倒计时，移除下载事件
                    timDownSpeedTask.Stop();
                    timDownSpeedTask.Enabled = false;
                    timDownSpeedTask.Tick -= timDownSpeedTask_Tick;
                    timDownSpeedTask = null;
                }

                if (webcDownloading != null)
                {
                    //移除下载进度改变事件
                    webcDownloading.DownloadProgressChanged -= DownloadProgressChanged;
                    webcDownloading.DownloadFileCompleted -= DownloadFileCompleted;
                }

                if (webcDownloading != null)
                {
                    webcDownloading.Dispose();
                    webcDownloading = null;
                }
                bolItemDestroyed = true;
            }
            catch (Exception ex)
            {
            }
        }


        /// <summary>
        /// 暂停，继续下载
        /// </summary>
        /// <param name="bstop"></param>
        public void myTaskReStart(bool bstop)
        {
            try
            {
                if (bstop)
                {
                    if (bolDownloadingFlag)
                    {
                        //webcDownloading.Suspend()
                        if (webcDownloading.IsBusy)
                        {
                            //取消异步下载
                            webcDownloading.CancelAsync();
                            //异步取消，直到IsBusy为False
                            while (webcDownloading.IsBusy)
                            {
                                //do something
                                if (bolItemDestroyed)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    // '如果开启了下载，且下载任务EventDone状态不为已完成，则继续下载，
                    // 'WebClient的异步下载文件已修改为支持断点续传
                    if (bolDownloadingFlag)
                    {
                        //已修改WebClient增加断点续传
                        DownloadFileAsync(this.DownloadUrl);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }



        /// <summary>
        /// 删除下载
        /// </summary>
        /// <param name="inValue"></param>
        public void myTaskDelete(string inValue)
        {
            try
            {
                // 停止时钟
                this.timDownSpeedTask.Enabled = false;
                this.timDownSpeedTask.Stop();
                // 移除时钟触发事件
                timDownSpeedTask.Tick -= timDownSpeedTask_Tick;
                if (webcDownloading.IsBusy)
                {
                    // 取消异步下载
                    webcDownloading.CancelAsync();
                    // '异步取消，直到IsBusy为False
                    while (webcDownloading.IsBusy)
                    {
                        //do something
                        if (bolItemDestroyed)
                        {
                            break;
                        }
                    }
                }
                //删除下载的文件（未完成的）
                if (!this.DownloadCompleted)
                {
                    try
                    {
                        string strFilePath = string.Empty;
                        strFilePath = string.Format("{0}\\{1}", this.DownSavePath, this.DownSaveFile);
                        if (System.IO.File.Exists(strFilePath))
                        {
                            System.IO.File.Delete(strFilePath);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                bolItemDestroyed = true;
                this.Dispose();
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        private bool StateImageStrech = false;
        private DrawItemState State = DrawItemState.Default;


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            this.Select();
            //MessageBox.Show(this.DownloadCompleted.ToString() );
            //State = DrawItemState.Selected;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnInitTimer();
                try
                {
                    Destroyed();
                }
                catch(Exception ex)
                {
                    Win32API.OutputDebugStringA( string.Format("销毁下载项时出现错误：{0}",ex.Message)  );
                }
            }
            base.Dispose(disposing);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            pMouseDown = new Point(0,0);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            pMouseDown = e.Location;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            pMouseMove = e.Location;
            //Win32API.OutputDebugStringA(string.Format("{0} 触发 MouseMove ", this.Text)); 
        }


        /// <summary>
        /// 自绘
        /// </summary>
        /// <param name="pevent"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            try
            {
                //e.Graphics.DrawBackground();
                //e.DrawFocusRectangle();
                bool bolSelected = false;
                //string bounds = string.Format("Bounds:({0},{1},{2},{3}),Item=({4})", Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, Text);
                //Win32API.OutputDebugStringA(bounds);

                //string cliprect = string.Format("ClipRectangle:({0},{1},{2},{3}),Item=({4})", e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height, Text);
                //Win32API.OutputDebugStringA(cliprect);


                Rectangle drawrect = new Rectangle(Bounds.X, 0, Bounds.Width, Bounds.Height);
                //Rectangle drawrect = new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y , e.ClipRectangle.Width, e.ClipRectangle.Height);

                e.Graphics.FillRectangle(new SolidBrush(this.NormalColor), drawrect);

                if ((State & DrawItemState.HotLight) == DrawItemState.HotLight )
                {
                    e.Graphics.FillRectangle(new SolidBrush(this.HoverColor), drawrect);
                }
                else
                {
                    //Win32API.OutputDebugStringA( string.Format("当前项：{0}，是否处于选中状态：{1} ", Text, IsSelected) ); 
                    if ( (State & DrawItemState.Selected) == DrawItemState.Selected)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(this.SelectedColor), drawrect);
                        bolSelected = true;
                    }
                    else
                    {
                        e.Graphics.FillRectangle(new SolidBrush(this.NormalColor), drawrect);
                        bolSelected = false;
                    }
                }
                //画分隔线
                e.Graphics.DrawLine(new Pen(SeparateColor), new Point(drawrect.Left, drawrect.Top + drawrect.Height - 1), new Point(drawrect.Width, drawrect.Top + drawrect.Height - 1));
                //画图标
                if (this.Image != null)
                {
                    e.Graphics.DrawImage(this.Image, drawrect.Left + 32 / 2, drawrect.Top + 32 / 2, 32, 32);
                }
                //画下载文件名
                try
                {                
                    if (this.Text.Length > 0)
                    {
                        if (!this.DownloadCompleted)
                        {
                            //System.Drawing.SizeF TextLengthSize = e.Graphics.MeasureString(this.Text, TextFont);  // 计算字符串所需要的大小
                            //Win32API.OutputDebugStringA( string.Format("下载状态：{0}, 文字：{1}, 文字长度{2}", this.DownloadCompleted, this.Text, this.Text.Length) );
                            if (this.Text.Length >= 50)
                            {
                                string strTempText = string.Format("{0}...", this.Text.Substring(0, 47));
                                TextRenderer.DrawText(e.Graphics, strTempText, TextFont, new Rectangle(drawrect.Left + 64, drawrect.Top + 12, drawrect.Width - 80, drawrect.Height), TextColor, TextFormatFlags.Left | TextFormatFlags.Top);
                            }
                            else
                            {
                                TextRenderer.DrawText(e.Graphics, this.Text, TextFont, new Rectangle(drawrect.Left + 64, drawrect.Top + 12, drawrect.Width - 80, drawrect.Height), TextColor, TextFormatFlags.Left | TextFormatFlags.Top);
                            }
                        }
                        else
                        {
                            //System.Drawing.SizeF TextLengthSize = e.Graphics.MeasureString(this.Text, TextFont);  // 计算字符串所需要的大小
                            if (this.Text.Length >= 60)
                            {
                                string strTempText = string.Format("{0}...", this.Text.Substring(0, 57));
                                TextRenderer.DrawText(e.Graphics, strTempText, TextFont, new Rectangle(drawrect.Left + 64, drawrect.Top + 12, drawrect.Width - 80, drawrect.Height), TextColor, TextFormatFlags.Left | TextFormatFlags.Top);
                            }
                            else
                            {
                                TextRenderer.DrawText(e.Graphics, this.Text, TextFont, new Rectangle(drawrect.Left + 64, drawrect.Top + 12, drawrect.Width - 80, drawrect.Height), TextColor, TextFormatFlags.Left | TextFormatFlags.Top);
                            }
                        }
                    
                        //e.Graphics.DrawString(this.Text,  Font, ForeColor, 50, 8, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                    }
                }
                catch(Exception ex)
                {
                    //Win32API.OutputDebugStringA(string.Format("画文字时出错:{0}",ex.Message));
                }
                //如果没有下载完成则画下载进度
                if (!this.DownloadCompleted)
                {                
                    //画下载速度
                    if (this.DownloadSpeed.Length > 0)
                    {
                        TextRenderer.DrawText(e.Graphics, this.DownloadSpeed, SpeedTextFont, new Rectangle(drawrect.Left, drawrect.Top + 12, drawrect.Width - 80, drawrect.Height), SpeedTextColor, TextFormatFlags.Right | TextFormatFlags.Top);
                        //e.Graphics.DrawString(this.Text,  Font, ForeColor, 50, 8, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                    }
                    //画进度条
                    //e.Graphics.DrawRectangle(new Pen(Color.Gray), this.Bounds.Left + 68, this.Bounds.Top + 32, this.Bounds.Width-168, 8 );
                    Rectangle rectfull = new Rectangle(drawrect.Left + 68, drawrect.Top + 36, drawrect.Width - 148, 4);

                    e.Graphics.FillRectangle(new SolidBrush(Color.DarkGray), rectfull);

                    //把进度条的宽度分成100份
                    float nowprec = rectfull.Width / 100F;

                    //输出调试信息
                    //Win32API.OutputDebugStringA(string.Format("this.ProgressPercentage({0}),nowprec=({1})", this.ProgressPercentage, nowprec.ToString()));
                    if (this.ProgressPercentage <= 99)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.Gray), new RectangleF(rectfull.Left, rectfull.Top, this.ProgressPercentage * nowprec, rectfull.Height));
                    }
                    else if (this.ProgressPercentage == 100)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.Gray), new RectangleF(rectfull.Left, rectfull.Top, rectfull.Width, rectfull.Height));
                    }

                    string width2percentage = string.Format("width:{0}, {1}%, barwidth:{2}, step:{3}", rectfull.Width, this.ProgressPercentage, this.ProgressPercentage * nowprec, nowprec);

                    //TextRenderer.DrawText(e.Graphics, width2percentage, new Font("微软雅黑", 9), new Rectangle(this.Bounds.Left, this.Bounds.Top + 32, this.Bounds.Width - 100, this.Bounds.Height), ForeColor, TextFormatFlags.Right | TextFormatFlags.Top);

                    //画暂停/恢复按钮
                    Rectangle btnRectControl = new Rectangle(drawrect.Width - 60, drawrect.Top + 20, 16, 16);
                    if(btnRectControl.Contains(pMouseMove))
                    {
                        //e.Graphics.FillRectangle( new SolidBrush(HoverColor), btnRectControl);
                        if (IsStoped)
                        {
                            e.Graphics.DrawImage(Resources.bsuspendhover, btnRectControl.Left, btnRectControl.Top, 16, 16);
                        }
                        else
                        {
                            e.Graphics.DrawImage(Resources.bresumehover, btnRectControl.Left, btnRectControl.Top, 16, 16);
                        }
                    
                        if (btnRectControl.Contains(pMouseDown))
                        {
                            pMouseDown = new Point(0, 0);
                            //e.Graphics.FillRectangle(new SolidBrush(Color.DarkOrange), btnRectControl);

                            //循环点击状态，暂停以及继续下载
                            if (bolIsStoped) {
                                bolIsStoped = false;
                            } else {
                                bolIsStoped = true;
                            }
                            IsStoped = bolIsStoped;
                            //bolIsStoped = !bolIsStoped;
                            try
                            {
                                //暂停下载 或 继续下载
                                myTaskReStart(IsStoped);
                            }
                            catch (Exception ex)
                            {

                            }
                        }                    
                    }
                    else
                    {
                        //e.Graphics.FillRectangle(new SolidBrush(NormalColor), btnRectControl);
                        if (IsStoped)
                        {
                            e.Graphics.DrawImage(Resources.bsuspend, btnRectControl.Left, btnRectControl.Top, 16, 16);
                        }
                        else
                        {
                            e.Graphics.DrawImage(Resources.bresume, btnRectControl.Left, btnRectControl.Top, 16, 16);
                        }
                        
                        //this.Invalidate();
                    }
                }
                else
                {
                    //画已完成以及打开文件夹
                    if (this.DownSaveSize.Length > 0)
                    {
                        string strFinished = string.Format("{0} - 已完成", this.DownSaveSize);
                        Font fontFinished = new Font("微软雅黑", 9);
                        TextRenderer.DrawText(e.Graphics, strFinished, fontFinished, new Rectangle(drawrect.Left + 64, drawrect.Top + 32, drawrect.Width - 80, drawrect.Height), SpeedTextColor, TextFormatFlags.Left | TextFormatFlags.Top);
                        //e.Graphics.DrawString(this.Text,  Font, ForeColor, 50, 8, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                    }
                    //画打开文件位置按钮
                    Rectangle btnRectFolder = new Rectangle(drawrect.Width - 120, drawrect.Top + drawrect.Height/4, 60, 24);
                    //画图标
                    //e.Graphics.DrawImage(Resources.mnufolders, btnRectFolder.Left, btnRectFolder.Top, 24, 24);
                    //画文件夹文字
                    //TextRenderer.DrawText(e.Graphics, "文件夹", TextFont, btnRectFolder, TextColor, TextFormatFlags.Left | TextFormatFlags.Top);
                    bool bolFolderMouseMove = false;
                    if (btnRectFolder.Contains(pMouseMove))
                    {
                        //e.Graphics.FillRectangle( new SolidBrush(HoverColor), btnRectFolder); //填充
                        bolFolderMouseMove = true;
                        e.Graphics.DrawImage(Resources.mnufolders, btnRectFolder.Left, btnRectFolder.Top, 24, 24);//画图标
                        TextRenderer.DrawText(e.Graphics, "文件夹", TextFont,  new Rectangle(btnRectFolder.Left + 26, btnRectFolder.Top+2, btnRectFolder.Width, btnRectFolder.Height) , TextColor, TextFormatFlags.Left | TextFormatFlags.Top);
                        if (this.Parent != null)
                        {
                            toolTip1.Show("打开所在文件夹", this.Parent, pMouseMove.X, pMouseMove.Y + 10, 2000);
                            this.Parent.ShowItemToolTips = false;
                        }
                        if (btnRectFolder.Contains(pMouseDown))
                        {
                            pMouseDown = new Point(0, 0);
                            e.Graphics.FillRectangle(new SolidBrush(Color.DarkGray), btnRectFolder);
                            //执行打开文件夹路径命令
                            try
                            {
                                string strcmd = string.Format("/e,{0}", this.DownSavePath.Replace(@"\\", @"\"));
                                System.Diagnostics.Process.Start("Explorer", strcmd);
                            }
                            catch(Exception ex)
                            {

                            }
                            //bolIsStoped = !bolIsStoped;
                        }
                    }
                    else
                    {
                        bolFolderMouseMove = false;
                        //e.Graphics.FillRectangle(new SolidBrush(NormalColor), btnRectFolder);
                        if (this.Parent != null) {
                            toolTip1.Hide(this.Parent);
                            this.Parent.ShowItemToolTips = true;
                        }
                        e.Graphics.DrawImage(Resources.mnufolders, btnRectFolder.Left, btnRectFolder.Top, 24, 24);
                        TextRenderer.DrawText(e.Graphics, "文件夹", TextFont, new Rectangle(btnRectFolder.Left + 26, btnRectFolder.Top+2, btnRectFolder.Width, btnRectFolder.Height), TextColor, TextFormatFlags.Left | TextFormatFlags.Top);
                        //TextRenderer.DrawText(e.Graphics, "文件夹", TextFont, btnRectFolder, TextColor, TextFormatFlags.Left | TextFormatFlags.Top);
                    }
                    try
                    {
                       //
                    }
                    catch(Exception ex)
                    {

                    }
                }

                //画删除按钮
                if (IsMouseMove)
                {
                    Rectangle btnRectDelete = new Rectangle(drawrect.Width - 30, drawrect.Top + 20, 16, 16);

                    if (btnRectDelete.Contains(pMouseMove))
                    {
                        //e.Graphics.FillRectangle(new SolidBrush(HoverColor), btnRectDelete);
                        e.Graphics.DrawImage(Resources.download_deletehover, btnRectDelete.Left, btnRectDelete.Top, 16, 16);
                        if (btnRectDelete.Contains(pMouseDown))
                        {
                            pMouseDown = new Point(0, 0);
                            //e.Graphics.FillRectangle(new SolidBrush(Color.DarkOrange), btnRectDelete);

                            //删除这一项
                            this.myTaskDelete(this.DownSaveFile);
                            //this.Dispose();
                            //bolIsStoped = !bolIsStoped;
                        }
                    }
                    else
                    {
                        e.Graphics.DrawImage(Resources.download_delete, btnRectDelete.Left, btnRectDelete.Top, 16, 16);
                        //e.Graphics.FillRectangle(new SolidBrush(NormalColor), btnRectDelete);
                    }
                    
                }                
                //ControlPaint.DrawButton(e.Graphics, new Rectangle(this.Bounds.Width - 80, this.Bounds.Top + 16, 32, 32), ButtonState.Normal);
            }
            catch (Exception ex)
            {

            }                       
        }
    }
}
