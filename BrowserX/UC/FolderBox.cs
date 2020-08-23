using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using BrowserX.Properties;


namespace BrowserX.UC
{
    public partial class FolderBox : UserControl
    {
        #region "enum"
        private enum DropDownStatus
        {
            Opened,
            Closing,
            Normal
        }
        #endregion

        #region "开放属性"
        //下拉列表中的删除按钮的提示文字
        private string _ClearTips = "清空地址栏下拉列表";
        [Category("Settings")]
        [Description("下拉列表中的删除按钮的提示文字")]
        [Browsable(true)]
        [DefaultValue(typeof(string), "")]
        public string ClearTips
        {
            get { return _ClearTips; }
            set
            {
                if (!_ClearTips.Equals(value))
                {
                    _ClearTips = value;
                }
            }
        }
        //磁盘剩余空间
        private string _DiskSpace = "剩余:";
        [Category("Settings")]
        [Description("磁盘剩余空间")]
        [Browsable(false)]
        [DefaultValue(typeof(string), "剩余:")]
        public string DiskSpace
        {
            get { return _DiskSpace; }
            set
            {
                _DiskSpace = value;
                try
                {
                    this.chboxDiskSpace.Text = value;
                }
                catch(Exception ex)
                {

                }                
            }
        }
        //一拉列表中的项
        private List<string> _items = new List<string>();
        //序列化列表
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design","System.Drawing.Design.UITypeEditor, System.Drawing")]
        public List<string> Items
        {
            get
            {
                return _items;
            }
            set
            {
                try
                {
                    _items = value;                    
                }
                catch(Exception ex)
                {
                    Win32API.OutputDebugStringA("设置FolderBox的Items发生了错误，" + ex.Message);
                }
                try
                {
                    //清空下拉列表
                    myToolStripDropDown.Items.Clear();
                    //添加清空按钮
                    AddButtonItem(ClearTips);
                    //添加所有项
                    foreach (string item in _items)
                    {
                        if (!item.Equals(ClearTips)) //"清空地址栏下拉列表"
                        {
                            AddItem(item);
                        }                        
                    }
                }
                catch(Exception ex)
                {

                }
                
            }
        }
        //是否下拉列表打开
        private bool _DropDownOpened = false;
        [Category("Settings")]
        [Description("下拉列表是否打开")]
        [Browsable(false)]
        [DefaultValue(typeof(bool), "false")]
        public bool DropDownOpened
        {
            get { return _DropDownOpened; }
            set
            {
                if (_DropDownOpened != value)
                {
                    _DropDownOpened = value;
                }
            }
        }
        //是否单击选中
        private bool _ClickToSelectAll = false;
        [Category("Settings")]
        [Description("是否单击选中")]
        [Browsable(false)]
        [DefaultValue(typeof(bool), "false")]
        public bool ClickToSelectAll
        {
            get { return _ClickToSelectAll; }
            set
            {
                if (_ClickToSelectAll != value)
                {
                    _ClickToSelectAll = value;
                }
            }
        }
        //存储当前地址的标题
        private string _UrlTitle = string.Empty;
        [Category("Settings")]
        [Description("存储当前页面的标题")]
        [Browsable(true)]
        [DefaultValue(typeof(string), "")]
        public string UrlTitle
        {
            get { return _UrlTitle; }
            set
            {
                if (!_UrlTitle.Equals(value))
                {
                    _UrlTitle = value;
                }
            }
        }
        //存储当前地址的文本值
        private string _Text = string.Empty;
        [Category("Settings")]
        [Description("文本框中显示的内容")]
        [Browsable(true)]
        [DefaultValue(typeof(string), "")]
        public override string Text
        {
            get { return _Text; }
            set
            {
                if (!_Text.Equals(value))
                {
                    _Text = value;
                    this.chboxAddress.Text = _Text;
                }
            }
        }
        // 下拉列表中项选中时的的背景
        private Color _SelectedColor = Color.FromArgb(255, 218, 218, 218);  
        [Description("下拉列表中项选中时的的背景。")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "218, 218, 218")]
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

        // 下拉列表中项未选中时的的背景
        private Color _NormalColor = Color.FromArgb(255, 242, 242, 242);  
        [Description("下拉列表中项未选中时的的背景。")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "242, 242, 242")]
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
        // ComboxBox上显示的文本的对齐方式.
        private ContentAlignment _TextAlign = ContentAlignment.MiddleCenter; 
        [Description("下拉列表的文本的对齐方式.")]
        [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
        [Category("Settings")]
        public ContentAlignment TextAlign
        {
            get
            {
                return _TextAlign;
            }

            set
            {
                _TextAlign = value;

                this.Invalidate();
            }
        }

        // ComboxBox上显示的文本框的右键菜单.
        private ContextMenuStrip _Menu = new ContextMenuStrip();
        [Description("右键快捷菜单.")]
        [DefaultValue(typeof(ContextMenuStrip), null)]
        [Category("Settings")]
        public  ContextMenuStrip Menu
        {
            get
            {
                return _Menu;
            }
            set
            {
                _Menu = value;
                try
                {
                    this.chboxAddress.ContextMenuStrip = _Menu;
                    this.Invalidate();
                }
                catch (Exception ex)
                {

                }
            }
        }
        //当前选择的文本
        private string _SelectedText = string.Empty;
        [Category("Settings")]
        [Description("文本框中选择的文本")]
        [Browsable(false)]
        [DefaultValue(typeof(string), "")]
        public string SelectedText
        {
            get { return _SelectedText; }
            set
            {
                _SelectedText = value;
            }
        }
        //当前选择文本的位置
        private int _SelectionLength = 0;
        [Category("Settings")]
        [Description("文本框中选择的文本长度")]
        [Browsable(false)]
        [DefaultValue(typeof(int), "0")]
        public int SelectionLength
        {
            get { return _SelectionLength; }
            set
            {
                _SelectionLength = value;
            }
        }
        //下拉列表中选项的高度
        private int _ItemHeight = 28;
        [Category("Settings")]
        [Description("下拉列表中选项的高度")]
        [Browsable(false)]
        [DefaultValue(typeof(int), "28")]
        public int ItemHeight
        {
            get { return _ItemHeight; }
            set
            {
                _ItemHeight = value;
            }
        }
        //文本框中选择的文本的起始位置
        private int _SelectionStart = 0;
        [Category("Settings")]
        [Description("文本框中选择的文本的起始位置")]
        [Browsable(false)]
        [DefaultValue(typeof(int), "0")]
        public int SelectionStart
        {
            get { return _SelectionStart; }
            set
            {
                _SelectionStart = value;
            }
        }
        //获取或设置所选内容开始行的缩进距离（以像素为单位）。
        private int _SelectionIndent = 0;
        [Category("Settings")]
        [Description("获取或设置所选内容开始行的缩进距离（以像素为单位）。")]
        [Browsable(false)]
        [DefaultValue(typeof(int), "0")]
        public int SelectionIndent
        {
            get { return _SelectionIndent; }
            set
            {
                _SelectionIndent = value;
            }
        }
        //
        private int _SelectionRightIndent = 0;
        [Category("Settings")]
        [Description("当前选定内容或插入点右侧的缩进空间（以像素为单位）。")]
        [Browsable(false)]
        [DefaultValue(typeof(int), "0")]
        public int SelectionRightIndent
        {
            get { return _SelectionRightIndent; }
            set
            {
                _SelectionRightIndent = value;
            }
        }
        
        #endregion

        #region "开放事件"
        //下拉列表选择某项时触发
        public delegate void SelectionChangeEventHandler(object sender, EventArgs e);
        public event SelectionChangeEventHandler SelectionChanged;
        //下拉列表打开时触发
        public delegate void DropDownOpendEventHandler(object sender, EventArgs e);
        public event DropDownOpendEventHandler DropDownOpening;
        //下拉列表关闭时触发
        public delegate void DropDownCloseedEventHandler(object sender, EventArgs e);
        public event DropDownCloseedEventHandler DropDownClosed;
        #endregion

        #region "临时变量"
        private string inputurl = string.Empty;       //在下拉列表框中选择的项的文本
        private int SelectedIndex = -1;               //在下拉列表框中选择的项的序号（废弃）  
        private myFolderToolStrip PressedItem = null; //在下拉列表框中选择的项
        public ToolStripItem ClickedItem = null;      //当前选择的项 
        private bool bolDropDownMouseDown = false;    //是否在下拉列表框中点击了某项
        private DropDownStatus myDropDownStatus = DropDownStatus.Closing;
        private bool bolUndoFlag = false;             //是否处于撤消状态 
        private bool bolChangeRtfFlag = false;        //是否处于修改RTF状态 
        private string inputpath = string.Empty;      //当前选择的项  

        //自定义弹出的下拉列表中的内容
        private ToolStripDropDown myToolStripDropDown = new ToolStripDropDown();
        #endregion

        #region "自定义函数"
        /// <summary>
        ///     ''' 正则表达式匹配,匹配到则返回True，同时outText为匹配到的数据
        ///     ''' </summary>
        ///     ''' <param name="response"></param>
        ///     ''' <param name="regex"></param>
        ///     ''' <param name="outText"></param>
        ///     ''' <returns>true is matched,false is not matched</returns>
        public bool RegexMatch(string response, string regex, ref string outText)
        {
            bool result = false;
            try
            {
                string txt = response;

                string re = regex;    // 正则表达式

                Regex r = new Regex(re, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Match m = r.Match(txt);
                if ((m.Success))
                {
                    result = true;
                    outText = m.Groups[0].ToString();
                }
                else
                {
                    result = false;
                    outText = "";
                }
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }


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
        #endregion
        //鼠标在输入框中第一次点击
        private bool bolFirstMouseClick =false;

        //构造函数
        public FolderBox()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 添加一项到列表中
        /// </summary>
        /// <param name="text"></param>
        public void AddItem(string text, bool ischecked = false)
        {
            try
            {
                //添加到ToolStripDropDown的Items中
                //将ComboBox列表中的项添加到ToolStripDropDown的Items中
                if (myToolStripDropDown != null)
                {
                    myToolStripDropDown.AutoSize = true;
                    myFolderToolStrip dropDownItem = new myFolderToolStrip(text) ;
                    dropDownItem.AutoSize = false;
                    dropDownItem.Image = Resources.icon_folder;
                    dropDownItem.ImageAlign = ContentAlignment.MiddleLeft;
                    dropDownItem.TextAlign = ContentAlignment.MiddleLeft;
                    dropDownItem.Size = new Size(this.pnlTopParent.Width, this.ItemHeight);
                    //插入项"清空地址栏下列表"之前
                    if(ischecked)
                    {
                        if (PressedItem != null)
                        {
                            PressedItem.IsSelected = false;
                        }                        
                        dropDownItem.IsSelected = true;
                        PressedItem = dropDownItem;
                    }                    
                    myToolStripDropDown.Items.Insert(myToolStripDropDown.Items.Count-1, dropDownItem );
                    myToolStripDropDown.Update();
                    //myToolStripDropDown.Items.Add(dropDownItem);
                }
            }
            catch (Exception ex)
            {
                //Win32API.OutputDebugStringA(string.Format("添加时遇到错误：{0}",ex.Message)); 
            }
        }


        public void AddButtonItem(string btntext)
        {
            try
            {
                //将清空地址栏列表加入ToolStripDropDown的Items中
                myToolStripDropDown.Items.Clear();
                //添加下载项
                myFolderToolStrip dropDownItem = new myFolderToolStrip(btntext);//"清空地址栏下拉列表"
                                                                                  //dropDownItem.Text = tmpitem;
                dropDownItem.AutoSize = false;
                dropDownItem.Image = Resources.clearItem;
                dropDownItem.ImageAlign = ContentAlignment.MiddleRight;
                dropDownItem.TextAlign = ContentAlignment.MiddleRight;
                dropDownItem.Size = new Size(this.pnlTopParent.Width, this.ItemHeight);
                myToolStripDropDown.Items.Add(dropDownItem);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 移除一项
        /// </summary>
        /// <param name="text"></param>
        public void RemoveItem(string text)
        {
            try
            {
                foreach (myFolderToolStrip item in myToolStripDropDown.Items)
                {
                    if (item.Text.Equals(text))
                    {
                        myToolStripDropDown.Items.Remove(item);
                        _items.Remove(item.Text);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }



        /// <summary>
        /// 触发键盘事件
        /// </summary>
        /// <param name="keydata"></param>
        public void ProcessInput(Keys keydata)
        {
            try
            {
                if(keydata == Keys.Enter || keydata == Keys.Return)
                {
                    System.Windows.Forms.KeyEventArgs ke = new System.Windows.Forms.KeyEventArgs(keydata);
                    //UIElement.KeyUpEvent kue = new UIElement.KeyUpEvent; 
                    //Win32API.PostMessageA(chboxAddress.Handle, Win32API.WM_KEYDOWN, keydata, 0);
                    //Win32API.PostMessageA(chboxAddress.Handle, Win32API.WM_KEYUP, keydata, 0);
                    this.OnKeyDown(ke);
                    this.OnKeyUp(ke);
                    //this.RaiseKeyEvent(chboxAddress.Handle, ke);                    
                    // this.OnKeyUp(ke);
                    //this.RaiseKeyEvent(chboxAddress, ke);
                    //chboxAddress_KeyDown(chboxAddress, ke);
                }
            }
            catch (Exception ex)
            {
                //Win32API.OutputDebugStringA("ProcessInput !" + ex.Message ); 
            }
        }

        /// <summary>
        /// 自定义菜单命令，方便添加右键菜单
        /// </summary>
        /// <param name="cmd"></param>
        public void ProcessCommand(string cmd)
        {
            try
            {
                if (cmd.Equals("copy") )
                {
                    try
                    {
                        // Dim cliFormat As System.Windows.Forms.TextDataFormat
                        this.chboxAddress.Copy(); 
                        //Clipboard.Clear();
                        //string tmpCopyValue = this.chboxAddress.SelectedText ;
                        //Clipboard.SetText(tmpCopyValue, TextDataFormat.Text);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else if(cmd.Equals("paste"))
                {
                    try
                    {
                        // Dim cliFormat As System.Windows.Forms.TextDataFormat
                        this.chboxAddress.Paste();
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else if (cmd.Equals("selectall"))
                {
                    try
                    {
                        // Dim cliFormat As System.Windows.Forms.TextDataFormat
                        this.chboxAddress.SelectAll();
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else if (cmd.Equals("cut"))
                {
                    try
                    {
                        // Dim cliFormat As System.Windows.Forms.TextDataFormat
                        this.chboxAddress.Cut();
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else if (cmd.Equals("delete"))
                {
                    try
                    {
                        // Dim cliFormat As System.Windows.Forms.TextDataFormat
                        this.chboxAddress.SelectedText = "";
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                //Win32API.OutputDebugStringA("ProcessInput !" + ex.Message);
            }
        }

        private void pboxArrow_Click(object sender, EventArgs e)
        {
            //do something
            if(myDropDownStatus == DropDownStatus.Normal)
            {
            }
            else if(myDropDownStatus == DropDownStatus.Opened )
            {
                this.pboxArrow.Image = Properties.Resources.bArrow;
                myToolStripDropDown.Close();
            } 
            else if (myDropDownStatus == DropDownStatus.Closing)
            {
                this.pboxArrow.Image = Properties.Resources.bArrowup;
                //弹出ToolStripDropDown，Items是ComboBox的Items中的项
                myToolStripDropDown.Show(this.pnlTopParent, 0, this.pnlTopParent.Height);

            }
            pboxArrow.Enabled = false;
            //myDropDownStatus = DropDownStatus.Normal;
        }

        private void chboxAddress_Click(object sender, EventArgs e)
        {
            if (bolFirstMouseClick)
            {
                if (this.ClickToSelectAll)
                {
                    this.chboxAddress.SelectAll();
                }
                
                bolFirstMouseClick = false;
            } 
        }

        private void chboxAddress_MouseEnter(object sender, EventArgs e)
        {
            //do something
        }

        private void chboxAddress_MouseLeave(object sender, EventArgs e)
        {
            //do something
        }

        private void chboxAddress_Enter(object sender, EventArgs e)
        {
            //do something
            bolFirstMouseClick = true;
        }

        private void chboxAddress_Leave(object sender, EventArgs e)
        {
            //
            bolFirstMouseClick = false;
        }

        private void FolderBox_Load(object sender, EventArgs e)
        {
            //Win32API.OutputDebugStringA("AddressBox_Load"); 
            try
            {
                this.chboxAddress.ClearUndo();
                this.chboxAddress.Font = new Font("微软雅黑",10);
                myToolStripDropDown.AutoSize = true;
                myToolStripDropDown.Opening += MyToolStripDropDown_Opening; 
                myToolStripDropDown.Closing += MyToolStripDropDown_Closing;
                myToolStripDropDown.ItemClicked += MyToolStripDropDown_ItemClicked;
                myToolStripDropDown.ShowItemToolTips = false;
                myToolStripDropDown.Height = 232;
                myToolStripDropDown.DropShadowEnabled = false;
                myToolStripDropDown.Font = new Font("微软雅黑",10);
                //将ComboBox列表中的项添加到ToolStripDropDown的Items中
                myToolStripDropDown.Items.Clear();
                myToolStripDropDown.BackColor = Color.FromArgb(0xf2f2f2) ;
                //初始化ToolStripDropDown.Items
                AddButtonItem(ClearTips);
                //添加删除项              
            }
            catch(Exception ex)
            {

            }
        }

        private void MyToolStripDropDown_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            bolDropDownMouseDown = true;
            ClickedItem = e.ClickedItem;
            //Win32API.OutputDebugStringA("MyToolStripDropDown_ItemClicked"); 
            //throw new NotImplementedException();
        }

        private void MyToolStripDropDown_Opening(object sender, CancelEventArgs e)
        {
            try
            {
                myDropDownStatus = DropDownStatus.Opened;
                DropDownOpened = true;
                if (myToolStripDropDown != null)
                {
                    //Win32API.OutputDebugStringA(this.pnlTopParent.Width.ToString() ); 
                    //myToolStripDropDown.Width = this.pnlTopParent.Width;
                }
                if (DropDownOpening != null)
                {
                    DropDownOpening(this, e);
                }
            }
            catch(Exception ex)
            {

            }
            //throw new NotImplementedException();
        }

        private void MyToolStripDropDown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            try
            {
                myDropDownStatus = DropDownStatus.Closing;
                pboxArrow.Enabled = true;
                DropDownOpened = false;
                this.pboxArrow.Image = Properties.Resources.bArrow;
                //判断当前选择的项
                inputurl = "";
            }
            catch (Exception ex)
            {

            }
            //如果弹出了下拉列表框没有选择一项，则直接返回
            if (!bolDropDownMouseDown)
            {
                //return;
            }
            //选择了项，为下一次判断初始化为false
            bolDropDownMouseDown = false;            
            
            try
            {
                //判断当前选择的项
                foreach (myFolderToolStrip titem in myToolStripDropDown.Items)
                {
                    //当前项，鼠标悬停，并不是真正意义上的点击选中
                    //Win32API.OutputDebugStringA(string.Format("所在项的 Pressed 为：{0}", titem.Pressed));
                    if (titem.Pressed)
                    {
                        inputurl = titem.Text;
                        if (!inputurl.Equals(ClearTips))//"清空地址栏下拉列表"
                        {
                            titem.IsSelected = true;
                            PressedItem = titem;
                            //触发选择了某项事件
                            if (SelectionChanged != null)
                            {
                                SelectionChanged(this, e);
                            }
                        }
                        else
                        {
                            titem.IsSelected = false;
                        }
                    }
                    else
                    {
                        titem.IsSelected = false;
                    }
                    //Win32API.OutputDebugStringA(string.Format("所在项的IsSelected为：{0}", titem.IsSelected));
                }
                //重新遍历一遍myToolStripDropDown.Items,设置当前地址为选定项
                foreach (myFolderToolStrip titem in myToolStripDropDown.Items)
                {
                    if (titem == PressedItem)
                    {
                        titem.IsSelected = true;
                    }
                    else
                    {
                        titem.IsSelected = false;
                    }

                }
                myToolStripDropDown.Update();
                if ((string.IsNullOrEmpty(inputurl) || inputurl.Length <= 0))
                {
                    return;
                }
                else if(inputurl.Equals(ClearTips)) //"清空地址栏下拉列表"
                {
                    //Win32API.OutputDebugStringA(string.Format("在 myToolStripDropDown.Items中包含了:{0}个项目.", myToolStripDropDown.Items.Count ));
                    int iloopcount = myToolStripDropDown.Items.Count - 1;
                    for (int i = iloopcount; i>=0;i--)
                    {
                        myFolderToolStrip item = myToolStripDropDown.Items[i] as myFolderToolStrip;

                        if (!item.Text.Equals(ClearTips))//"清空地址栏下拉列表"
                        {
                            myToolStripDropDown.Items.Remove(item);
                            //列表中删除该项
                            _items.Remove(item.Text);
                        }
                    }
                    myToolStripDropDown.Update();
                }
                else if (inputurl.Equals("桌面"))
                {
                    this.chboxAddress.Text = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    inputurl = "";
                }
                else
                {
                    this.chboxAddress.Text = inputurl;
                    inputurl = "";
                }
                //this.cboxChildbox.DroppedDown = true;
            }
            catch (Exception ex)
            {
                pboxArrow.Enabled = true;
            }
            //触发事件
            try
            {
                if (DropDownClosed != null)
                {
                    DropDownClosed(this, e);
                }
            }
            catch (Exception ex)
            {

            }
            //throw new NotImplementedException();
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            try
            {
                chboxAddress.Text = "QQ:2489046815";
            }
            catch
            {

            }
        }

        //输入框文本改变事件
        private void chboxAddress_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _Text = chboxAddress.Text;
                if (!chboxAddress.Text.Equals("桌面"))
                {
                    this.DiskSpace = string.Format("剩余：{0}", PublicModule.GetHardDiskSpace(chboxAddress.Text));
                }
            }
            catch
            {

            }
        }

        private void chboxAddress_KeyDown(object sender, KeyEventArgs e)
        {
            //this.RaiseKeyEvent(e.KeyCode, e);
            //Win32API.OutputDebugStringA(string.Format( "chboxAddress_KeyDown Control:{0}, KeyCode={1}", e.Control, e.KeyValue));  
            base.OnKeyDown(e);
        }

        private void chboxAddress_KeyUp(object sender, KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        private void chboxAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            base.OnKeyPress(e);       
        }


        private void pboxArrow_MouseDown(object sender, MouseEventArgs e)
        {
            //do something
        }

        private void pnlTopParent_SizeChanged(object sender, EventArgs e)
        {
            //解决AddressBox尺寸改变后，下拉列表框中的项大小不会改变
            try
            {
                //
                if (myToolStripDropDown != null)
                {
                    foreach (myFolderToolStrip item in myToolStripDropDown.Items)
                    {
                        item.Size = new Size(this.pnlTopParent.Width, this.ItemHeight);
                    }
                }                
            }
            catch(Exception ex)
            {

            }
        }

        private void chboxAddress_SelectionChanged(object sender, EventArgs e)
        {
            //
            SelectedText = chboxAddress.SelectedText;
            SelectionLength = chboxAddress.SelectionLength;
            SelectionStart = chboxAddress.SelectionStart;
            SelectionIndent = chboxAddress.SelectionIndent;
            SelectionRightIndent = chboxAddress.SelectionRightIndent;            
            //Win32API.OutputDebugStringA( string.Format("chboxAddress中选择的内容为：{0}", SelectedText) ); 
        }

        private void FolderBox_KeyDown(object sender, KeyEventArgs e)
        {
            //
            //Win32API.OutputDebugStringA("AddressBox_KeyDown");
            bolChangeRtfFlag = false;//在没有任何键入时对rtf进行修改
            if (e.KeyCode == Keys.Return)
            {
                bool AddFlag = true;
                string AddItemText = chboxAddress.Text;
                foreach (myFolderToolStrip item in myToolStripDropDown.Items)
                {
                    if (item.Text.Equals(AddItemText))
                    {
                        AddFlag = false;
                        break;
                    }
                }
                if (AddFlag)
                {
                    this.AddItem(AddItemText, true);
                }
                //清除回车后“咚”的警告音
                e.Handled = true;
            }
            else if (e.Modifiers.CompareTo(Keys.Control) == 0 && e.KeyCode == Keys.C)
            {
                //do something
                chboxAddress.Copy();
            }
            else if (e.Modifiers.CompareTo(Keys.Control) == 0 && e.KeyCode == Keys.X)
            {
                //do something
                chboxAddress.Cut();
            }
            else if (e.Modifiers.CompareTo(Keys.Control) == 0 && e.KeyCode == Keys.V)
            {
                //do something
                chboxAddress.Paste();
            }
            else if (e.Modifiers.CompareTo(Keys.Control) == 0 && e.KeyCode == Keys.A)
            {
                //do something
                chboxAddress.SelectAll();
            }
            else if (e.Modifiers.CompareTo(Keys.Control) == 0 && e.KeyCode == Keys.Z)
            {
                //do something
                if (chboxAddress.CanUndo)
                {
                    chboxAddress.Undo();
                }
            }
        }

        private void FolderBox_KeyUp(object sender, KeyEventArgs e)
        {
            //
            //Win32API.OutputDebugStringA("AddressBox_KeyUp");
        }

        private void FolderBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            //
            //Win32API.OutputDebugStringA("AddressBox_KeyPress");
        }

        private void chboxDiskSpace_Click(object sender, EventArgs e)
        {
           //do something
        }
    }
}
