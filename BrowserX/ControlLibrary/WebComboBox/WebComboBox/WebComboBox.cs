using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WebComboBox.Properties;

namespace WebComboBox
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WebComboBox"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WebComboBox;assembly=WebComboBox"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class WebComboBox : ComboBox
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern void OutputDebugStringA(string lpOutputString);


        static WebComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WebComboBox), new FrameworkPropertyMetadata(typeof(WebComboBox)));
        }

        #region "私有变量"
        private TextBox     dpEditBoxItem = null;
        private DockPanel   dpClearItem = null;
        private TextBlock   tboUrlHighlight = null;
        private MenuItem    mnuPasteAndLoad = null;
        private MenuItem    mnuCopyUrlAndTitle = null;
        #endregion

        /// <summary>
        /// 重写应用模板
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();            
        }


        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);


            if (!DesignerProperties.GetIsInDesignMode(this))
            {            
                // 搜索ComboxBox的模板
                try
                {
                    //MessageBox.Show("this.IsLoaded" + this.IsLoaded);
                    // dpClearAllItem清除下拉列表
                    dpClearItem = Template.FindName("dpClearAllItem", this) as DockPanel;
                    if ((dpClearItem != null))
                    {
                        try
                        {
                            dpClearItem.PreviewMouseUp -= DockPanelClear_PreviewMouseUp;
                            dpClearItem.PreviewMouseUp += DockPanelClear_PreviewMouseUp;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    dpEditBoxItem = Template.FindName("PART_EditableTextBox", this) as TextBox;
                    if ((dpEditBoxItem != null))
                    {
                        try
                        {
                            dpEditBoxItem.TextChanged -= TextBoxComm_TextChanged;
                            dpEditBoxItem.TextChanged += TextBoxComm_TextChanged;
                            dpEditBoxItem.LostFocus -= TextBoxComm_LostFocus;
                            dpEditBoxItem.LostFocus += TextBoxComm_LostFocus;
                            dpEditBoxItem.PreviewTextInput -= TextBoxComm_PreviewTextInput;
                            dpEditBoxItem.PreviewTextInput += TextBoxComm_PreviewTextInput;
                            dpEditBoxItem.Visibility = Visibility.Collapsed;
                            dpEditBoxItem.FontFamily = this.FontFamily;
                            dpEditBoxItem.FontSize = this.FontSize;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    // 绑定新的菜单
                    if (dpEditBoxItem != null)
                    {
                        try
                        {
                            // Me.dpEditBoxItem.ContextMenu = Me.Resources("mnuUrlMenu")                            
                            //dpEditBoxItem.ContextMenu = this.Resources["mnuUrlMenu"] as ContextMenu;
                            if(dpEditBoxItem.ContextMenu != null)
                            {
                                foreach (Object tts in dpEditBoxItem.ContextMenu.Items )
                                {
                                    if (tts != null)
                                    {
                                        if(tts.GetType().Equals( typeof(MenuItem) ))
                                        {
                                            string strMenuItemName = string.Empty;
                                            strMenuItemName = (tts as MenuItem).Name;
                                            if (strMenuItemName.Length >0)
                                            {
                                                if(strMenuItemName.Equals("mnuPasteAndLoad"))
                                                {
                                                    mnuPasteAndLoad = (tts as MenuItem);
                                                    //添加点击事件
                                                    if (mnuPasteAndLoad!=null)
                                                    {
                                                        //
                                                        mnuPasteAndLoad.Click += MnuPasteAndLoad_Click;
                                                    }
                                                }
                                                else if (strMenuItemName.Equals("mnuCopyUrlAndTitle"))
                                                {
                                                    mnuCopyUrlAndTitle = (tts as MenuItem);
                                                    if (mnuCopyUrlAndTitle != null)
                                                    {
                                                        //
                                                        mnuCopyUrlAndTitle.Click += MnuCopyUrlAndTitle_Click;
                                                    }
                                                }
                                            }
                                            //OutputDebugStringA(string.Format("菜单中的对象：{0}",     ));
                                        }
                                        
                                    }
                                }
                            }                            
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(string.Format("设置Web地址栏菜单失败！错误：{0}",ex.Message));
                        }
                    }
                    // 查找tboUrlHighlight,一开始是正常状态
                    tboUrlHighlight = Template.FindName("tboUrlHighlight", this) as TextBlock;
                    if (tboUrlHighlight != null)
                    {
                        try
                        {
                            tboUrlHighlight.PreviewMouseDown -= TextBlockComm_PreviewMouseDown;
                            tboUrlHighlight.PreviewMouseDown += TextBlockComm_PreviewMouseDown;
                            tboUrlHighlight.Visibility = Visibility.Visible;
                            tboUrlHighlight.FontFamily = this.FontFamily;
                            tboUrlHighlight.FontSize = this.FontSize;
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

        private void MnuCopyUrlAndTitle_Click(object sender, RoutedEventArgs e)
        {
            //复制标题及网址菜单点击事件
            // 复制标题与网址
            try
            {
                // Dim cliFormat As System.Windows.Forms.TextDataFormat
                Clipboard.Clear();
                string tmpCopyValue = string.Empty;
                tmpCopyValue = string.Format("{0}{1}{2}", this.UrlTitle, Environment.NewLine, this.Text);
                Clipboard.SetText(tmpCopyValue, TextDataFormat.Text);
            }
            catch (Exception ex)
            {
            }
            //throw new NotImplementedException();
        }

        private void MnuPasteAndLoad_Click(object sender, RoutedEventArgs e)
        {
            //菜单粘贴并访问
            // 粘贴并访问
            try
            {
                if (dpEditBoxItem != null)
                {
                    string strTempUrl = string.Empty;
                    strTempUrl = Clipboard.GetText();

                    if(strTempUrl.Length <=0) {
                        return;
                    }
                    this.Text = Clipboard.GetText();
                    //触发事件
                    KeyEventArgs ke = new KeyEventArgs(Keyboard.PrimaryDevice,Keyboard.PrimaryDevice.ActiveSource,0, Key.Enter)
                    {
                        RoutedEvent = UIElement.KeyUpEvent
                    };

                    InputManager.Current.ProcessInput(ke);

                    this.RaiseEvent(ke);
                }
            }
            catch (Exception ex)
            {
                //OutputDebugStringA("MnuPasteAndLoad_Click error:" + ex.Message); 
            }
            //throw new NotImplementedException();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);            
        }



        private void TextBoxComm_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((dpEditBoxItem != null))
                {
                    dpEditBoxItem.Visibility = Visibility.Collapsed;
                    if (this.tboUrlHighlight!=null)
                    {
                        this.tboUrlHighlight.Visibility = Visibility.Visible;
                    }
                    
                }
            }
            catch (Exception ex)
            {
            }
        }


        private void TextBoxComm_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // MsgBox(sender.ToString)
                // 在这里分析网址协议，域名等信息进行着色
                TextBox tboxBox = null/* TODO Change to default(_) if this is not a reference type */;
                tboxBox = sender as TextBox;
                if ((tboxBox == null))
                    return;
                string tmpText = string.Empty;
                string tmpDnsText = string.Empty;

                // 匹配顶级区域正则表达式，自己搜索修改 (\w*\.(?:com|com|org|net))[\^]*
                string tmpDnsRegex = @"(\w*\.(?:com.cn|com|cn\/|edu.cn|org|net))[\^]*";

                string tmpRegexOutput = string.Empty;
                tmpText = tboxBox.Text.ToString();
                if ((RegexMatch(tmpText, tmpDnsRegex, ref tmpRegexOutput)))
                {
                    int intDnsIndex = 0;
                    if ((tmpRegexOutput.EndsWith("/")))
                        tmpRegexOutput = tmpRegexOutput.Replace("/", "");
                    intDnsIndex = tmpText.IndexOf(tmpRegexOutput);
                    if ((intDnsIndex > 0))
                    {
                        string strHeader = string.Empty;
                        string strEnd = string.Empty;
                        strHeader = tmpText.Substring(0, intDnsIndex);
                        strEnd = tmpText.Substring(intDnsIndex + tmpRegexOutput.Length);
                        // MessageBox.Show(String.Format("匹配到了域名协议头:{0},协议尾：{1}", strHeader, strEnd))
                        // MessageBox.Show(String.Format("网址列表的字体字号为：{0}", Me.tboUrlHighlight.FontSize))
                        Run runHeader = new Run();
                        runHeader.Text = strHeader.ToString();
                        runHeader.FontSize = this.FontSize + 0;
                        runHeader.Foreground = Brushes.DarkGray;
                        Run runDns = new Run();
                        runDns.Text = tmpRegexOutput.ToString();
                        runDns.FontSize = this.FontSize + 0;
                        runDns.Foreground = Brushes.Black;
                        Run runWebPath = new Run();
                        runWebPath.Text = strEnd.ToString();
                        runWebPath.FontSize = this.FontSize + 0;
                        runWebPath.Foreground = Brushes.DarkGray;
                        if (this.tboUrlHighlight != null)
                        {
                            this.tboUrlHighlight.Inlines.Clear();
                            this.tboUrlHighlight.Inlines.Add(runHeader);
                            this.tboUrlHighlight.Inlines.Add(runDns);
                            this.tboUrlHighlight.Inlines.Add(runWebPath);
                        }

                    }
                }
                else
                {
                    Run runWebPath = new Run();
                    runWebPath.Text = tmpText;
                    runWebPath.FontSize = this.FontSize + 0;
                    runWebPath.Foreground = Brushes.DarkGray;
                    if (this.tboUrlHighlight != null)
                    {
                        this.tboUrlHighlight.Inlines.Clear();
                        this.tboUrlHighlight.Inlines.Add(runWebPath);
                    }
                }
                if ((tmpText.ToLower().StartsWith("https://www.")))
                {
                }
                else if ((tmpText.ToLower().StartsWith("http://www.")))
                {
                }
                else if ((tmpText.ToLower().StartsWith("www.")))
                {
                }
            }
            catch (Exception ex)
            {
            }
        }


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



        private void DockPanelClear_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.tboUrlHighlight != null)
                {
                    //MessageBox.Show(string.Format("tboUrlHighlight !=null, visible={0}", this.tboUrlHighlight.Visibility ));                    
                }

                //MessageBox.Show("dpClearItem Clicked");
                if (!this.HasItems) {
                    return;
                }                    
                this.Items.Clear();
            }
            catch (Exception ex)
            {

            }
        }


        private void TextBoxComm_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                if ((dpEditBoxItem != null))
                {
                    if ((dpEditBoxItem.Visibility == Visibility.Collapsed))
                    {
                        dpEditBoxItem.Visibility = Visibility.Visible;
                    }
                    //Url覆盖层                        
                    if (this.tboUrlHighlight!=null)
                    {
                        this.tboUrlHighlight.Visibility = Visibility.Collapsed;
                    }                    
                }
            }
            catch (Exception ex)
            {
            }
        }


        private void TextBlockComm_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // 地址栏遮罩鼠标按键按下动作
            try
            {
                if ((dpEditBoxItem != null))
                {
                    dpEditBoxItem.Visibility = Visibility.Visible;
                    if (this.tboUrlHighlight != null)
                    {
                        this.tboUrlHighlight.Visibility = Visibility.Collapsed;
                    }
                    dpEditBoxItem.Focus();
                }
            }
            catch (Exception ex)
            {
            }
        }


        #region "自定义依赖属性"

        //自定义属性
        public string UrlTitle
        {
            get
            {
                return (string)GetValue(UrlTitleProperty);
            }
            set
            {
                SetValue(UrlTitleProperty, value);
            }
        }
        public static readonly DependencyProperty UrlTitleProperty = DependencyProperty.Register("UrlTitle", typeof(string), typeof(WebComboBox), new PropertyMetadata(""));

        //自定义依赖

        //自定义属性
        public ImageSource DropDownArrow
        {
            get
            {
                return (ImageSource)GetValue(DropDownArrowProperty);
            }
            set
            {
                SetValue(DropDownArrowProperty, value);
            }
        }
        //自定义依赖
        public static readonly DependencyProperty DropDownArrowProperty = DependencyProperty.Register("DropDownArrow", typeof(ImageSource), typeof(WebComboBox), new PropertyMetadata(null));

        //自定义属性
        public ImageSource DropDownClear
        {
            get
            {
                return (ImageSource)GetValue(DropDownClearProperty);
            }
            set
            {
                SetValue(DropDownClearProperty, value);
            }
        }
        //自定义依赖
        public static readonly DependencyProperty DropDownClearProperty = DependencyProperty.Register("DropDownClear", typeof(ImageSource), typeof(WebComboBox), new PropertyMetadata(null));


    }
    #endregion
}
