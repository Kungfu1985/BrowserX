using BrowserX.Properties;
using BrowserX.UC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrowserX
{
    public partial class FormTestAddressBox : Form
    {

        private Point pMouseMove = new Point(0, 0);
        private Point pMouseDown = new Point(0, 0);
        private bool IsStoped = false;
        private bool bolIsStoped = false;
        //自定义弹出的下拉列表中的内容
        private ToolStripDropDown myToolStripDropDown = new ToolStripDropDown();



        public FormTestAddressBox()
        {
            InitializeComponent();
        }

        public FormTestAddressBox(String[] args)
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show( string.Format("当前标签页标题：{0}, 网址为：{1}", this.addressBox1.UrlTitle, this.addressBox1.Text)  );
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //this.addressBox1.Text="https://bing.com.cn";
            //this.addressBox1.UrlTitle = "必应";          
            try
            {
                //添加到ToolStripDropDown的Items中
                //将ComboBox列表中的项添加到ToolStripDropDown的Items中
                if (myToolStripDropDown != null)
                {
                    myToolStripDropDown.AutoSize = true;
                    myDownloadToolStrip dropDownItem = new myDownloadToolStrip("铁呀呀的牙");
                    dropDownItem.AutoSize = false;
                    dropDownItem.DownloadSpeed = "大小：81.93MB - 8.14MB/s";
                    dropDownItem.ProgressPercentage = 85;
                    dropDownItem.Image = Resources.mnudownload;
                    dropDownItem.ImageAlign = ContentAlignment.MiddleLeft;
                    dropDownItem.TextAlign = ContentAlignment.MiddleLeft;
                    dropDownItem.Size = new Size(this.panel1.Width, 60);
                    //插入项"清空地址栏下列表"之前
                    //myToolStripDropDown.Items.Insert(myToolStripDropDown.Items.Count, dropDownItem);
                    myToolStripDropDown.Items.Add(dropDownItem);
                    myToolStripDropDown.Update();
                    //myToolStripDropDown.Items.Add(dropDownItem);
                }
            }
            catch (Exception ex)
            {
                //Win32API.OutputDebugStringA(string.Format("添加时遇到错误：{0}",ex.Message)); 
            }
        }


        private void button8_Click(object sender, EventArgs e)
        {
            if (myToolStripDropDown != null)
            {
                myToolStripDropDown.TopLevel = false;
                //myToolStripDropDown.Parent = this.panel1;
                myToolStripDropDown.AutoSize = true;
                //myToolStripDropDown.ScrollControlIntoView(this.panel1);
                //myToolStripDropDown.Dock = DockStyle.Fill;
                myToolStripDropDown.AutoClose = false;
                myToolStripDropDown.Height = panel1.Height;
                myToolStripDropDown.MaximumSize = new Size(this.panel1.Width, this.panel1.Height);
                myToolStripDropDown.Show(this.addressBox1, 0, this.addressBox1.Height);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.addressBox1.AddItem("https://im.qq.com"); 
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.addressBox1.RemoveItem("https://im.qq.com");
        }

        private void addressBox1_KeyDown(object sender, KeyEventArgs e)
        {
            //
            label1.Text = e.KeyCode.ToString() ;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    //记录最近一次的选择点，最后恢复时使用
            //    Win32API.OutputDebugStringA(richTextBox1.Rtf);
            //    int SelectionIndex = richTextBox1.SelectionStart;
            //    #region "先将所有的字符显示为DarkGray颜色"
            //    richTextBox1.Font = new Font("微软雅黑", 11);
            //    richTextBox1.SelectionStart = 0;//初始位置
            //    richTextBox1.SelectionLength = richTextBox1.TextLength;//
            //    richTextBox1.SelectionColor = Color.DarkGray;//
            //    #endregion
            //    Regex reg = new Regex(@"(\w*\.(?:com.cn|com|cn\/|edu.cn|org|net))[\^]*");//设定的需要改变颜色的固定字符串
            //    MatchCollection mc = reg.Matches(richTextBox1.Text, 0);//获取匹配到的顶级域名和其它字符串信息
            //    //逐个字符串变更颜色，这里只变更顶级域名的颜色
            //    //richTextBox1.HideSelection = true;
            //    //只处理匹配到的第一个项
            //    foreach (Match item in mc)
            //    {
            //        richTextBox1.SelectionStart = item.Index;
            //        richTextBox1.SelectionLength = item.Value.Length;
            //        richTextBox1.SelectionColor = Color.Black;
            //        break;
            //    }
            //    // richTextBox1.TextLength;//回到了文本末尾
            //    richTextBox1.SelectionStart = SelectionIndex; //回到最近一次的选择点
            //    richTextBox1.SelectionLength = 0;
            //    //richTextBox1.HideSelection = false;
            //    //_Text = richTextBox1.Text;
            //}
            //catch
            //{

            //}
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem lvitem = new ListViewItem();

                myDownloadToolStrip item = new myDownloadToolStrip();
                item.Text = string.Format("PCQQ2020({0}).exe", DateTime.Now.ToString("ss"));
                item.DownloadSpeed = "大小：81.93MB - 8.14MB/s";
                item.ProgressPercentage = 85;
                item.Image = (Image)Properties.Resources.mnudownload;
                item.BackColor = Color.DarkGray;
                item.Width = listBox1.Width;
                item.Height = 60;
                item.PercentageChanged += Item_PercentageChanged; ;
                this.listBox1.Items.Add(item); 
            }
            catch(Exception ex)
            {
                Win32API.OutputDebugStringA(ex.Message); 
            }
        }

        private void Item_PercentageChanged(Rectangle bounds, float Percentage)
        {
            listBox1.Invalidate(bounds); 
            //throw new NotImplementedException();
        }

        private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            //
            e.ItemHeight = 60;
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            //listBox1.Invalidate(e.Bounds);
            try
            {
                if((e.State & DrawItemState.Selected)== DrawItemState.Selected)
                {
                    //e.DrawBackground();
                    e.Graphics.FillRectangle(new SolidBrush(Color.DarkGray), listBox1.GetItemRectangle(e.Index));
                }
                else if ((e.State & DrawItemState.HotLight) == DrawItemState.HotLight)
                {
                    //e.DrawBackground();
                    e.Graphics.FillRectangle(new SolidBrush(Color.Gray), listBox1.GetItemRectangle(e.Index));
                }
                else if ((e.State & DrawItemState.Inactive) == DrawItemState.Inactive)
                {
                    //e.DrawBackground();
                    e.Graphics.FillRectangle(new SolidBrush(Color.White), listBox1.GetItemRectangle(e.Index));
                }

                //e.DrawFocusRectangle();

                if (e.Index >= 0)
                {
                    myDownloadToolStrip item = new myDownloadToolStrip();
                    item = listBox1.Items[e.Index] as myDownloadToolStrip;
                    //item.Invalidate();
                    if (item != null)
                    {
                        e.Graphics.DrawLine(new Pen(Color.DarkGray), new Point(e.Bounds.Left, e.Bounds.Top + e.Bounds.Height - 1), new Point(e.Bounds.Width, e.Bounds.Top + e.Bounds.Height - 1));
                        if (item.Image != null)
                        {
                            e.Graphics.DrawImage(item.Image, e.Bounds.Left + 32 / 2, e.Bounds.Top + 32 / 2, 32, 32);
                        }
                        if (item.Text.Length > 0)
                        {
                            TextRenderer.DrawText(e.Graphics, item.Text, new Font("微软雅黑", 11), new Rectangle(e.Bounds.Left + 64, e.Bounds.Top + 12, e.Bounds.Width - 100, e.Bounds.Height), ForeColor, TextFormatFlags.Left | TextFormatFlags.Top);
                            //e.Graphics.DrawString(item.Text,  Font, ForeColor, 50, 8, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                        }
                        if (item.DownloadSpeed.Length > 0)
                        {
                            TextRenderer.DrawText(e.Graphics, item.DownloadSpeed, new Font("微软雅黑", 9), new Rectangle(e.Bounds.Left, e.Bounds.Top + 12, e.Bounds.Width - 100, e.Bounds.Height), ForeColor, TextFormatFlags.Right | TextFormatFlags.Top);
                            //e.Graphics.DrawString(item.Text,  Font, ForeColor, 50, 8, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                        }
                        //画进度条
                        //e.Graphics.DrawRectangle(new Pen(Color.Gray), e.Bounds.Left + 68, e.Bounds.Top + 32, e.Bounds.Width-168, 8 );
                        Rectangle rectfull = new Rectangle(e.Bounds.Left + 68, e.Bounds.Top + 36, e.Bounds.Width - 168, 4);

                        e.Graphics.FillRectangle(new SolidBrush(Color.Gray), rectfull);

                        //把进度条的宽度分成100份
                        float nowprec = rectfull.Width / 100F;


                        //Win32API.OutputDebugStringA(string.Format("item.ProgressPercentage({0}),nowprec=({1})", item.ProgressPercentage, nowprec.ToString()));
                        if (item.ProgressPercentage <= 30)
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.DarkGray), new RectangleF(rectfull.Left, rectfull.Top, item.ProgressPercentage * nowprec, rectfull.Height));
                        }
                        else if (item.ProgressPercentage <= 50)
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.Red), new RectangleF(rectfull.Left, rectfull.Top, item.ProgressPercentage * nowprec, rectfull.Height));
                        }
                        else if (item.ProgressPercentage <= 70)
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.Blue), new RectangleF(rectfull.Left, rectfull.Top, item.ProgressPercentage * nowprec, rectfull.Height));
                        }
                        else if (item.ProgressPercentage <= 99)
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.Green), new RectangleF(rectfull.Left, rectfull.Top, item.ProgressPercentage * nowprec, rectfull.Height));
                        }
                        else if(item.ProgressPercentage == 100)
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.Green), new RectangleF(rectfull.Left, rectfull.Top, rectfull.Width, rectfull.Height));
                        }
                        string width2percentage = string.Format("width:{0}, {1}%, barwidth:{2}, step:{3}", rectfull.Width, item.ProgressPercentage, item.ProgressPercentage * nowprec, nowprec);

                        //TextRenderer.DrawText(e.Graphics, width2percentage, new Font("微软雅黑", 9), new Rectangle(e.Bounds.Left, e.Bounds.Top + 32, e.Bounds.Width - 100, e.Bounds.Height), ForeColor, TextFormatFlags.Right | TextFormatFlags.Top);

                        //画暂停/恢复按钮
                        Rectangle btnRectControl = new Rectangle(e.Bounds.Width - 60, e.Bounds.Top + 20, 16, 16);
                        if (btnRectControl.Contains(pMouseMove))
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.Red), btnRectControl);
                            if (btnRectControl.Contains(pMouseDown))
                            {
                                pMouseDown = new Point(0, 0);
                                e.Graphics.FillRectangle(new SolidBrush(Color.DarkOrange), btnRectControl);
                                
                                bolIsStoped = !bolIsStoped;
                                IsStoped = bolIsStoped;
                            }
                        }
                        else
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.Blue), btnRectControl);
                            //this.Invalidate();
                        }

                        if (IsStoped)
                        {
                            e.Graphics.DrawImage(Resources.bsuspend, btnRectControl.Left, btnRectControl.Top, 16, 16);
                        }
                        else
                        {
                            e.Graphics.DrawImage(Resources.bresume, btnRectControl.Left, btnRectControl.Top, 16, 16);
                        }
                        //画删除按钮
                        Rectangle btnRectDelete = new Rectangle(e.Bounds.Width - 30, e.Bounds.Top + 20, 16, 16);
                        if (btnRectDelete.Contains(pMouseMove))
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.Red), btnRectDelete);
                            if (btnRectDelete.Contains(pMouseDown))
                            {
                                pMouseDown = new Point(0, 0);
                                e.Graphics.FillRectangle(new SolidBrush(Color.DarkOrange), btnRectDelete);

                                //删除这一项
                                //DeleteItem(true);
                                //this.Dispose();
                                //bolIsStoped = !bolIsStoped;
                            }
                        }
                        else
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.Blue), btnRectDelete);
                        }
                        e.Graphics.DrawImage(Resources.bstop, btnRectDelete.Left, btnRectDelete.Top, 16, 16);

                        //ControlPaint.DrawButton(e.Graphics, new Rectangle(e.Bounds.Width - 80, e.Bounds.Top + 16, 32, 32), ButtonState.Normal);

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if(listBox1.SelectedItem != null)
                {
                    myDownloadToolStrip item = new myDownloadToolStrip();
                    item = listBox1.SelectedItem as myDownloadToolStrip;
                    if (item != null)
                    {
                        MessageBox.Show(item.Text);
                    }                    
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.DoubleBuffered = true;
                if (listBox1.SelectedItem != null)
                {
                    myDownloadToolStrip item = new myDownloadToolStrip();
                    item = listBox1.SelectedItem as myDownloadToolStrip;
                    if (item != null)
                    {
                        item.ProgressPercentage = trackBar1.Value;
                        Win32API.OutputDebugStringA(string.Format("item.bounds({0},{1},{2},{3})", 
                            item.Bounds.X,item.Bounds.Y, item.Bounds.Width,item.Bounds.Height ));
                        //listBox1.Invalidate(item.Bounds);
                        item.Invalidate();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void listBox1_MouseHover(object sender, EventArgs e)
        {
            //
        }

        private void listBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //listBox1.Invalidate();
            return;
            Point ptlbMouseMove = new Point(0, 0);
            ptlbMouseMove = e.Location;
            for(int i = 0; i <= listBox1.Items.Count - 1; i++)
            {
                if(listBox1.GetItemRectangle(i).Contains(ptlbMouseMove))
                {
                    listBox1.Invalidate(listBox1.GetItemRectangle(i)) ;
                    break;
                }
            }
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                //tableLayoutPanel1.MaximumSize = new Size(tableLayoutPanel1.Width, 60 *toolStrip1.Items.Count);
                //tableLayoutPanel1.AutoScroll = true;

                this.toolStrip1.Width = panelOwner1.Width;
                myDownloadToolStrip item = new myDownloadToolStrip();
                item.AutoSize = false;
                item.Text = string.Format("PCQQ2020({0}).exe", DateTime.Now.ToString("ss"));
                item.DownloadSpeed = "大小：81.93MB - 8.14MB/s";
                item.ProgressPercentage = 85;
                item.Image = (Image)Properties.Resources.mnudownload;
                item.BackColor = Color.DarkGray;
                item.NormalColor = Color.White;
                item.Width = panelOwner1.Width;
                item.Height = 60;
                this.toolStrip1.Items.Add(item);
                this.toolStrip1.AutoSize = true;
                //listBox1.Items.Add(toolStrip1); 
            }
            catch (Exception ex)
            {
                Win32API.OutputDebugStringA(ex.Message);
            }

        }


        private void FormTestAddressBox_Shown(object sender, EventArgs e)
        {
            tableLayoutPanel1.MaximumSize = new Size(tableLayoutPanel1.Width, tableLayoutPanel1.Height);
            tableLayoutPanel1.AutoScroll = true;

            tableLayoutPanel1.RowCount = 0;
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.AutoScroll = true;

        }

        private void panelOwner1_Scroll(object sender, ScrollEventArgs e)
        {
            //panelOwner1.AutoScrollPosition = new Point(0, 0);
            Win32API.OutputDebugStringA(string.Format(" scrollX:{0},scrollY:{1} ", panelOwner1.AutoScrollPosition.X, panelOwner1.AutoScrollPosition.Y));

            this.toolStrip1.Location = panelOwner1.AutoScrollPosition;
            this.toolStrip1.Invalidate();
            Win32API.OutputDebugStringA(string.Format(" toolStrip1.Left:{0},toolStrip1.Top:{1} ", toolStrip1.Left, toolStrip1.Top));

            if (e.OldValue < e.NewValue)
            {
                //this.toolStrip1.Top = this.toolStrip1.Top - e.NewValue;
            }
            else
            {
                //this.toolStrip1.Top = this.toolStrip1.Top + e.NewValue;
            }
            
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                this.toolStrip1.Parent = listBox1;
            }
            catch(Exception ex)
            {

            }
        }
    }
}
