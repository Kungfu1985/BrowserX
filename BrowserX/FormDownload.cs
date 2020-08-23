using BrowserX.UC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrowserX
{
    public partial class FormDownload : Form
    {
        public FormDownload()
        {
            InitializeComponent();
        }


        public FormDownload(String[] args)
        {
            InitializeComponent();
        }


        private void btnVirtualOne_Click(object sender, EventArgs e)
        {
            try
            {
                //tableLayoutPanel1.MaximumSize = new Size(tableLayoutPanel1.Width, 60 *toolStrip1.Items.Count);
                //tableLayoutPanel1.AutoScroll = true;
                //pnlDownloadList.VerticalScroll.Minimum = 60;
                //pnlDownloadList.VerticalScroll.Value = pnlDownloadList.VerticalScroll.Minimum;
                this.ToolStripDownloadList.Width = pnlDownloadList.Width;
                myDownloadToolStrip item = new myDownloadToolStrip();
                item.AutoSize = false;
                item.Text = string.Format("PCQQ2020({0}).exe", DateTime.Now.ToString("ss"));
                item.DownloadSpeed = "大小：81.93MB - 8.14MB/s";
                item.ProgressPercentage = 85;
                item.Image = (Image)Properties.Resources.mnudownload;
                item.BackColor = Color.DarkGray;
                item.ForeColor = Color.FromArgb(0x9b9b9b);
                item.NormalColor = Color.White;
                item.SelectedColor = Color.Orange;
                //item.HoverColor = Color.Green;
                item.SeparateColor = Color.FromArgb(239, 239, 239);
                item.Width = pnlDownloadList.Width;
                item.Height = 60;
                //pnlDownloadList.AutoScrollMinSize = new Size(item.Width, item.Height);
                //this.ToolStripDownloadList.ImageScalingSize = new Size(item.Width, item.Height);
                this.ToolStripDownloadList.Items.Add(item);

                //this.ToolStripDownloadList.Height = this.ToolStripDownloadList.Items.Count * 60;
                this.ToolStripDownloadList.AutoSize = true;
                //
                foreach (myDownloadToolStrip itmeL in ToolStripDownloadList.Items)
                {
                    //Win32API.EnableScrollBar(this.pnloDownloadList.Handle, Win32API.SBFlags.SB_VERT, Win32API.SBArrows.ESB_DISABLE_BOTH); 
                    if (this.ToolStripDownloadList.Items.Count > 5)
                    {
                        itmeL.Width = pnlDownloadList.Width - SystemInformation.VerticalScrollBarWidth;
                        //pnlDownloadList.AutoScrollMinSize = new Size(pnlDownloadList.Width - SystemInformation.VerticalScrollBarWidth, pnlDownloadList.Height);
                    }
                    else
                    {
                        itmeL.Width = pnlDownloadList.Width;
                        //pnloDownloadList.AutoScrollMinSize = new Size(pnloDownloadList.Width, pnloDownloadList.Height);
                    }
                    pnlDownloadList.Invalidate();
                }
                //listBox1.Items.Add(toolStrip1); 
            }
            catch (Exception ex)
            {
                Win32API.OutputDebugStringA(ex.Message);
            }
        }

        /// <summary>
        /// 添加新的下载任务
        /// </summary>
        public void AddDownloadTask()
        {
            try
            {
                if (PublicModule.bolDownFlag)
                {
                    PublicModule.bolDownFlag = false;
                    //添加到下载列表
                    this.ToolStripDownloadList.Width = pnlDownloadList.Width;
                    myDownloadToolStrip item = new myDownloadToolStrip();
                    item.AutoSize = false;
                    item.Image = (Image)Properties.Resources.mnudownload;
                    item.BackColor = Color.DarkGray;
                    item.ForeColor = Color.FromArgb(0x9b9b9b);
                    item.NormalColor = Color.White;
                    item.SelectedColor = Color.Orange;
                    //item.HoverColor = Color.Green;
                    item.SeparateColor = Color.FromArgb(239, 239, 239);
                    item.Width = pnlDownloadList.Width;
                    item.Height = 60;
                    //pnlDownloadList.AutoScrollMinSize = new Size(item.Width, item.Height);
                    //this.ToolStripDownloadList.ImageScalingSize = new Size(item.Width, item.Height);
                    //下载相关的参数,生成新的下载列表项
                    item.DownloadUrl = PublicModule.DownloadUrl;   //设置此项后初始化下载
                    item.DownSavePath = PublicModule.DownSavePath; //下载后保存的路径
                    item.DownSaveSize = PublicModule.DownSaveSize; //下载文件的大小
                    item.DownSaveFile = PublicModule.DownSaveFile; //设置此项后，开始下载
                    item.ProgressPercentage = 0;
                    item.DownloadCompleted = false;
                    //添加到列表中
                    this.ToolStripDownloadList.Items.Add(item);
                    item.StartDownload = true;
                    //释放
                    //下载列表项的窗口的自适应大小
                    //this.ToolStripDownloadList.Height = this.ToolStripDownloadList.Items.Count * 60;
                    this.ToolStripDownloadList.AutoSize = true;
                    //
                    foreach (myDownloadToolStrip itmeL in ToolStripDownloadList.Items)
                    {
                        if (this.ToolStripDownloadList.Items.Count > 5) {
                            itmeL.Width = pnlDownloadList.Width - SystemInformation.VerticalScrollBarWidth;
                        } else {
                            itmeL.Width = pnlDownloadList.Width;
                        }
                        pnlDownloadList.Invalidate();
                    }
                }
            }
            catch (Exception ex)
            {
                //MsgBox("初始化参数出现错误，错误信息：" + ex.Message);
            }
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            try
            {
                //this.pnloDownloadList.AutoScrollMinSize = new Size(this.pnloDownloadList.Width, this.pnloDownloadList.Height);
                //this.captionButton1.Visible = !this.captionButton1.Visible;
                PublicModule.bolAppClosing = true;
            }
            catch(Exception ex)
            {

            }
        }

        private void pnlDownloadList_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                //当拖拽Panel的滚动条时触让下载列表控件重绘
                this.ToolStripDownloadList.Invalidate();
                //pnloDownloadList.VerticalScroll.SmallChange = 600;
                //pnloDownloadList.VerticalScroll.Value = pnloDownloadList.VerticalScroll.Minimum;

                //foreach (myDownloadToolStrip item in tstoDownloadList.Items)
                //{
                //    //Win32API.OutputDebugStringA(string.Format("item({0}).Bounds:(left:{1},top:{2},X:{1},Y:{2})", item.Text, item.Bounds.Left, item.Bounds.Top, item.Bounds.X, item.Bounds.Y));
                //    //Win32API.OutputDebugStringA(string.Format("item({0}).Size:(width:{1},height:{2})", item.Name, item.Width, item.Height));
                //}
                //int ms = e.NewValue - e.OldValue;
                //tabpListParent.Location = new Point(0, tabpListParent.Location.Y + ms);
                //Win32API.OutputDebugStringA(string.Format("tabpListParent.Location(x={0}, Y={1})", tabpListParent.Location.X, tabpListParent.Location.Y));
                //Win32API.OutputDebugStringA(string.Format("tabpListParent.TopLeft(Top={0}, Left={1})", tabpListParent.Top, tabpListParent.Left));

                //Win32API.OutputDebugStringA(string.Format("DisplayRectangle:({0},{1})", tstoDownloadList.DisplayRectangle.Location.X, tstoDownloadList.DisplayRectangle.Location.Y));

            }
            catch (Exception ex)
            {

            }
        }

        private void ToolStripDownloadList_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if ((sender as ToolStrip).RenderMode == ToolStripRenderMode.System)
                {
                    Rectangle rect = new Rectangle(0, 0, this.ToolStripDownloadList.Width, this.ToolStripDownloadList.Height - 2);
                    e.Graphics.SetClip(rect);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void ToolStripDownloadList_SizeChanged(object sender, EventArgs e)
        {
            this.pnlDownloadList.HorizontalScroll.Visible = false;
            this.pnlDownloadList.VerticalScroll.Visible = false;
            foreach (myDownloadToolStrip itmeL in ToolStripDownloadList.Items)
            {
                if (this.ToolStripDownloadList.Items.Count <= 5)
                {
                    ToolStripDownloadList.Width = pnlDownloadList.Width;
                    itmeL.Width = pnlDownloadList.Width;
                    itmeL.Invalidate();
                }
            }
            pnlDownloadList.Invalidate();
            //Win32API.OutputDebugStringA(string.Format("tstoDownloadList.size({0},{1})", tstoDownloadList.Size.Width, tstoDownloadList.Size.Height));
        }

        private void FormDownload_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (PublicModule.bolAppClosing)
                {
                    PublicModule.bolTaskWindowOpened = false;
                    try
                    {
                        // 遍历所有下载任务，如果EventDone任务状态不为已完成，则删除文件
                        foreach (myDownloadToolStrip tmpTask in ToolStripDownloadList.Items)
                        {
                            // 释放所有下载列表的资源
                            tmpTask.Destroyed();
                            // 删除所有未下载完成的临时文件
                            if (!tmpTask.DownloadCompleted)
                            {
                                string strTempFile = string.Empty;
                                strTempFile = string.Format("{0}\\{1}", tmpTask.DownSavePath, tmpTask.DownSaveFile);
                                if (System.IO.File.Exists(strTempFile))
                                {
                                    System.IO.File.Delete(strTempFile);
                                }
                            }
                            System.Windows.Forms.Application.DoEvents();
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                    this.Visible = false;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void FormDownload_Shown(object sender, EventArgs e)
        {
            PublicModule.bolTaskWindowOpened = true;
        }

        private void captionButton1_Click(object sender, EventArgs e)
        {
            //
            try
            {
                this.Visible = false;
                PublicModule.bolAppClosing = true;
                FormDownloadNew fdl = new FormDownloadNew();
                fdl.ShowDialog();
                this.Visible = true;
                fdl.Dispose();
                fdl = null;
            }
            catch(Exception ex)
            {

            }
        }
    }
}
