namespace BrowserX.UC
{
    partial class BrowserContainer
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if(browser != null)
            {
                browser.Dispose();
                browser = null;
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserContainer));
            this.button1 = new System.Windows.Forms.Button();
            this.imageListWebIcon = new System.Windows.Forms.ImageList(this.components);
            this.pnlWebBrowser = new System.Windows.Forms.Panel();
            this.browserToolbar2 = new BrowserX.UC.BrowserToolbar();
            this.browserToolbar1 = new BrowserX.UC.BrowserToolbar();
            this.pnlAddressBox = new System.Windows.Forms.Panel();
            this.pnlAddressBoxParent = new System.Windows.Forms.Panel();
            this.cboxAddressList = new BrowserX.UC.AddressBox();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.pnlCopyTitleAndUrl = new System.Windows.Forms.Panel();
            this.cbtnCopyData = new CaptionButton();
            this.pnlSeparate2 = new PanelLine();
            this.pnlSeparate1 = new PanelLine();
            this.cbtnbfavorites = new CaptionButton();
            this.cbtnbhomepage = new CaptionButton();
            this.cbtnbundo = new CaptionButton();
            this.cbtnbmainmenu = new CaptionButton();
            this.cbtnbstopload = new CaptionButton();
            this.cbtnbrefresh = new CaptionButton();
            this.cbtnbgoforward = new CaptionButton();
            this.cbtnbgoback = new CaptionButton();
            this.browserTabHeader1 = new BrowserX.UC.BrowserTabHeader();
            this.browserToolbar1.SuspendLayout();
            this.pnlAddressBox.SuspendLayout();
            this.pnlAddressBoxParent.SuspendLayout();
            this.pnlCopyTitleAndUrl.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Right;
            this.button1.Location = new System.Drawing.Point(732, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 379);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // imageListWebIcon
            // 
            this.imageListWebIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListWebIcon.ImageStream")));
            this.imageListWebIcon.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListWebIcon.Images.SetKeyName(0, "icon_normal.png");
            this.imageListWebIcon.Images.SetKeyName(1, "close_on.png");
            // 
            // pnlWebBrowser
            // 
            this.pnlWebBrowser.BackColor = System.Drawing.Color.White;
            this.pnlWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlWebBrowser.Location = new System.Drawing.Point(0, 100);
            this.pnlWebBrowser.Name = "pnlWebBrowser";
            this.pnlWebBrowser.Size = new System.Drawing.Size(819, 377);
            this.pnlWebBrowser.TabIndex = 10;
            // 
            // browserToolbar2
            // 
            this.browserToolbar2.BackColor = System.Drawing.Color.White;
            this.browserToolbar2.Dock = System.Windows.Forms.DockStyle.Top;
            this.browserToolbar2.DrawBottomLine = false;
            this.browserToolbar2.DrawLinearBack = false;
            this.browserToolbar2.Location = new System.Drawing.Point(0, 68);
            this.browserToolbar2.Name = "browserToolbar2";
            this.browserToolbar2.Size = new System.Drawing.Size(819, 32);
            this.browserToolbar2.TabIndex = 9;
            this.browserToolbar2.Visible = false;
            // 
            // browserToolbar1
            // 
            this.browserToolbar1.BackColor = System.Drawing.Color.White;
            this.browserToolbar1.Controls.Add(this.pnlAddressBox);
            this.browserToolbar1.Controls.Add(this.pnlSeparate2);
            this.browserToolbar1.Controls.Add(this.pnlSeparate1);
            this.browserToolbar1.Controls.Add(this.cbtnbfavorites);
            this.browserToolbar1.Controls.Add(this.cbtnbhomepage);
            this.browserToolbar1.Controls.Add(this.cbtnbundo);
            this.browserToolbar1.Controls.Add(this.cbtnbmainmenu);
            this.browserToolbar1.Controls.Add(this.cbtnbstopload);
            this.browserToolbar1.Controls.Add(this.cbtnbrefresh);
            this.browserToolbar1.Controls.Add(this.cbtnbgoforward);
            this.browserToolbar1.Controls.Add(this.cbtnbgoback);
            this.browserToolbar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.browserToolbar1.DrawBottomLine = false;
            this.browserToolbar1.DrawLinearBack = false;
            this.browserToolbar1.Location = new System.Drawing.Point(0, 32);
            this.browserToolbar1.Name = "browserToolbar1";
            this.browserToolbar1.Size = new System.Drawing.Size(819, 36);
            this.browserToolbar1.TabIndex = 7;
            this.browserToolbar1.Text = "CustomComboBox Enabled";
            // 
            // pnlAddressBox
            // 
            this.pnlAddressBox.Controls.Add(this.pnlAddressBoxParent);
            this.pnlAddressBox.Controls.Add(this.pnlCopyTitleAndUrl);
            this.pnlAddressBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAddressBox.Location = new System.Drawing.Point(280, 0);
            this.pnlAddressBox.Name = "pnlAddressBox";
            this.pnlAddressBox.Size = new System.Drawing.Size(439, 36);
            this.pnlAddressBox.TabIndex = 41;
            // 
            // pnlAddressBoxParent
            // 
            this.pnlAddressBoxParent.Controls.Add(this.cboxAddressList);
            this.pnlAddressBoxParent.Controls.Add(this.elementHost1);
            this.pnlAddressBoxParent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAddressBoxParent.Location = new System.Drawing.Point(24, 0);
            this.pnlAddressBoxParent.Name = "pnlAddressBoxParent";
            this.pnlAddressBoxParent.Size = new System.Drawing.Size(415, 36);
            this.pnlAddressBoxParent.TabIndex = 25;
            // 
            // cboxAddressList
            // 
            this.cboxAddressList.BackColor = System.Drawing.SystemColors.Control;
            this.cboxAddressList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboxAddressList.Location = new System.Drawing.Point(0, 0);
            this.cboxAddressList.Name = "cboxAddressList";
            this.cboxAddressList.Size = new System.Drawing.Size(415, 36);
            this.cboxAddressList.TabIndex = 17;
            this.cboxAddressList.SelectionChanged += new BrowserX.UC.AddressBox.SelectionChangeEventHandler(this.cboxAddressList_SelectionChanged);
            this.cboxAddressList.DropDownClosed += new BrowserX.UC.AddressBox.DropDownCloseedEventHandler(this.cboxAddressList_DropDownClosed);
            this.cboxAddressList.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboxAddressList_KeyUp);
            // 
            // elementHost1
            // 
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(415, 36);
            this.elementHost1.TabIndex = 16;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = null;
            // 
            // pnlCopyTitleAndUrl
            // 
            this.pnlCopyTitleAndUrl.Controls.Add(this.cbtnCopyData);
            this.pnlCopyTitleAndUrl.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlCopyTitleAndUrl.Location = new System.Drawing.Point(0, 0);
            this.pnlCopyTitleAndUrl.Name = "pnlCopyTitleAndUrl";
            this.pnlCopyTitleAndUrl.Size = new System.Drawing.Size(24, 36);
            this.pnlCopyTitleAndUrl.TabIndex = 22;
            // 
            // cbtnCopyData
            // 
            this.cbtnCopyData.BackColor = System.Drawing.Color.Transparent;
            this.cbtnCopyData.BackImage = null;
            this.cbtnCopyData.BaseColor = System.Drawing.Color.Transparent;
            this.cbtnCopyData.CheckedMaskImage = null;
            this.cbtnCopyData.CheckedMaskImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbtnCopyData.CheckedTextColor = System.Drawing.Color.White;
            this.cbtnCopyData.CornerRadius = 0;
            this.cbtnCopyData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbtnCopyData.DrawHightLigthFlag = false;
            this.cbtnCopyData.DrawIconWidth = 0;
            this.cbtnCopyData.DrawTextBeforeImage = false;
            this.cbtnCopyData.HighlightColor = System.Drawing.Color.Gray;
            this.cbtnCopyData.Image = null;
            this.cbtnCopyData.ImageSize = new System.Drawing.Size(24, 24);
            this.cbtnCopyData.Location = new System.Drawing.Point(0, 0);
            this.cbtnCopyData.MouseHoverExSecond = 0;
            this.cbtnCopyData.Name = "cbtnCopyData";
            this.cbtnCopyData.Size = new System.Drawing.Size(24, 36);
            this.cbtnCopyData.StateImage = global::BrowserX.Properties.Resources.bcopyitem;
            this.cbtnCopyData.TabIndex = 4;
            this.cbtnCopyData.UseVisualStyleBackColor = false;
            this.cbtnCopyData.Click += new System.EventHandler(this.cbtnCopyData_Click);
            // 
            // pnlSeparate2
            // 
            this.pnlSeparate2.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlSeparate2.LineColor = System.Drawing.Color.Gainsboro;
            this.pnlSeparate2.LineMargin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.pnlSeparate2.Location = new System.Drawing.Point(719, 0);
            this.pnlSeparate2.Name = "pnlSeparate2";
            this.pnlSeparate2.Orientation = PanelLine.EnumOrientation.Horizontal;
            this.pnlSeparate2.Size = new System.Drawing.Size(10, 36);
            this.pnlSeparate2.TabIndex = 40;
            // 
            // pnlSeparate1
            // 
            this.pnlSeparate1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSeparate1.LineColor = System.Drawing.Color.Gainsboro;
            this.pnlSeparate1.LineMargin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.pnlSeparate1.Location = new System.Drawing.Point(270, 0);
            this.pnlSeparate1.Name = "pnlSeparate1";
            this.pnlSeparate1.Orientation = PanelLine.EnumOrientation.Horizontal;
            this.pnlSeparate1.Size = new System.Drawing.Size(10, 36);
            this.pnlSeparate1.TabIndex = 39;
            // 
            // cbtnbfavorites
            // 
            this.cbtnbfavorites.BackColor = System.Drawing.Color.Transparent;
            this.cbtnbfavorites.BackImage = null;
            this.cbtnbfavorites.BaseColor = System.Drawing.Color.Transparent;
            this.cbtnbfavorites.CheckedMaskImage = null;
            this.cbtnbfavorites.CheckedMaskImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbtnbfavorites.CheckedTextColor = System.Drawing.Color.White;
            this.cbtnbfavorites.CornerRadius = 0;
            this.cbtnbfavorites.Dock = System.Windows.Forms.DockStyle.Left;
            this.cbtnbfavorites.DrawGlowFlag = true;
            this.cbtnbfavorites.DrawGlowLeftIndentFlag = true;
            this.cbtnbfavorites.DrawHightLigthFlag = false;
            this.cbtnbfavorites.DrawIconWidth = 0;
            this.cbtnbfavorites.DrawTextBeforeImage = false;
            this.cbtnbfavorites.GlowColor = System.Drawing.Color.Gainsboro;
            this.cbtnbfavorites.HighlightColor = System.Drawing.Color.Gray;
            this.cbtnbfavorites.Image = global::BrowserX.Properties.Resources.bfavorites;
            this.cbtnbfavorites.ImageSize = new System.Drawing.Size(28, 28);
            this.cbtnbfavorites.Location = new System.Drawing.Point(225, 0);
            this.cbtnbfavorites.MouseHoverExSecond = 0;
            this.cbtnbfavorites.Name = "cbtnbfavorites";
            this.cbtnbfavorites.Padding = new System.Windows.Forms.Padding(4);
            this.cbtnbfavorites.PressedColor = System.Drawing.Color.Silver;
            this.cbtnbfavorites.Size = new System.Drawing.Size(45, 36);
            this.cbtnbfavorites.StateImage = null;
            this.cbtnbfavorites.TabIndex = 38;
            this.cbtnbfavorites.UseVisualStyleBackColor = false;
            // 
            // cbtnbhomepage
            // 
            this.cbtnbhomepage.BackColor = System.Drawing.Color.Transparent;
            this.cbtnbhomepage.BackImage = null;
            this.cbtnbhomepage.BaseColor = System.Drawing.Color.Transparent;
            this.cbtnbhomepage.CheckedMaskImage = null;
            this.cbtnbhomepage.CheckedMaskImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbtnbhomepage.CheckedTextColor = System.Drawing.Color.White;
            this.cbtnbhomepage.CornerRadius = 0;
            this.cbtnbhomepage.Dock = System.Windows.Forms.DockStyle.Left;
            this.cbtnbhomepage.DrawGlowFlag = true;
            this.cbtnbhomepage.DrawGlowLeftIndentFlag = true;
            this.cbtnbhomepage.DrawHightLigthFlag = false;
            this.cbtnbhomepage.DrawIconWidth = 0;
            this.cbtnbhomepage.DrawTextBeforeImage = false;
            this.cbtnbhomepage.GlowColor = System.Drawing.Color.Gainsboro;
            this.cbtnbhomepage.HighlightColor = System.Drawing.Color.Gray;
            this.cbtnbhomepage.Image = global::BrowserX.Properties.Resources.bhome;
            this.cbtnbhomepage.ImageSize = new System.Drawing.Size(28, 28);
            this.cbtnbhomepage.Location = new System.Drawing.Point(180, 0);
            this.cbtnbhomepage.MouseHoverExSecond = 0;
            this.cbtnbhomepage.Name = "cbtnbhomepage";
            this.cbtnbhomepage.Padding = new System.Windows.Forms.Padding(2);
            this.cbtnbhomepage.PressedColor = System.Drawing.Color.Silver;
            this.cbtnbhomepage.Size = new System.Drawing.Size(45, 36);
            this.cbtnbhomepage.StateImage = null;
            this.cbtnbhomepage.TabIndex = 37;
            this.cbtnbhomepage.UseVisualStyleBackColor = false;
            this.cbtnbhomepage.Click += new System.EventHandler(this.cbtnbhomepage_Click);
            // 
            // cbtnbundo
            // 
            this.cbtnbundo.BackColor = System.Drawing.Color.Transparent;
            this.cbtnbundo.BackImage = null;
            this.cbtnbundo.BaseColor = System.Drawing.Color.Transparent;
            this.cbtnbundo.CheckedMaskImage = null;
            this.cbtnbundo.CheckedMaskImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbtnbundo.CheckedTextColor = System.Drawing.Color.White;
            this.cbtnbundo.CornerRadius = 0;
            this.cbtnbundo.Dock = System.Windows.Forms.DockStyle.Right;
            this.cbtnbundo.DrawGlowFlag = true;
            this.cbtnbundo.DrawGlowLeftIndentFlag = true;
            this.cbtnbundo.DrawHightLigthFlag = false;
            this.cbtnbundo.DrawIconWidth = 0;
            this.cbtnbundo.DrawTextBeforeImage = false;
            this.cbtnbundo.GlowColor = System.Drawing.Color.Gainsboro;
            this.cbtnbundo.HighlightColor = System.Drawing.Color.Gray;
            this.cbtnbundo.Image = global::BrowserX.Properties.Resources.bundo;
            this.cbtnbundo.ImageSize = new System.Drawing.Size(28, 28);
            this.cbtnbundo.Location = new System.Drawing.Point(729, 0);
            this.cbtnbundo.MouseHoverExSecond = 0;
            this.cbtnbundo.Name = "cbtnbundo";
            this.cbtnbundo.Padding = new System.Windows.Forms.Padding(2);
            this.cbtnbundo.PressedColor = System.Drawing.Color.Silver;
            this.cbtnbundo.Size = new System.Drawing.Size(45, 36);
            this.cbtnbundo.StateImage = null;
            this.cbtnbundo.TabIndex = 36;
            this.cbtnbundo.UseVisualStyleBackColor = false;
            // 
            // cbtnbmainmenu
            // 
            this.cbtnbmainmenu.BackColor = System.Drawing.Color.Transparent;
            this.cbtnbmainmenu.BackImage = null;
            this.cbtnbmainmenu.BaseColor = System.Drawing.Color.Transparent;
            this.cbtnbmainmenu.CheckedMaskImage = null;
            this.cbtnbmainmenu.CheckedMaskImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbtnbmainmenu.CheckedTextColor = System.Drawing.Color.White;
            this.cbtnbmainmenu.CornerRadius = 0;
            this.cbtnbmainmenu.Dock = System.Windows.Forms.DockStyle.Right;
            this.cbtnbmainmenu.DrawGlowFlag = true;
            this.cbtnbmainmenu.DrawGlowLeftIndentFlag = true;
            this.cbtnbmainmenu.DrawHightLigthFlag = false;
            this.cbtnbmainmenu.DrawIconWidth = 0;
            this.cbtnbmainmenu.DrawTextBeforeImage = false;
            this.cbtnbmainmenu.GlowColor = System.Drawing.Color.Gainsboro;
            this.cbtnbmainmenu.HighlightColor = System.Drawing.Color.Gray;
            this.cbtnbmainmenu.Image = global::BrowserX.Properties.Resources.bmenu;
            this.cbtnbmainmenu.ImageSize = new System.Drawing.Size(28, 28);
            this.cbtnbmainmenu.Location = new System.Drawing.Point(774, 0);
            this.cbtnbmainmenu.MouseHoverExSecond = 0;
            this.cbtnbmainmenu.Name = "cbtnbmainmenu";
            this.cbtnbmainmenu.Padding = new System.Windows.Forms.Padding(2);
            this.cbtnbmainmenu.PressedColor = System.Drawing.Color.Silver;
            this.cbtnbmainmenu.Size = new System.Drawing.Size(45, 36);
            this.cbtnbmainmenu.StateImage = null;
            this.cbtnbmainmenu.TabIndex = 35;
            this.cbtnbmainmenu.UseVisualStyleBackColor = false;
            // 
            // cbtnbstopload
            // 
            this.cbtnbstopload.BackColor = System.Drawing.Color.Transparent;
            this.cbtnbstopload.BackImage = null;
            this.cbtnbstopload.BaseColor = System.Drawing.Color.Transparent;
            this.cbtnbstopload.CheckedMaskImage = null;
            this.cbtnbstopload.CheckedMaskImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbtnbstopload.CheckedTextColor = System.Drawing.Color.White;
            this.cbtnbstopload.CornerRadius = 0;
            this.cbtnbstopload.Dock = System.Windows.Forms.DockStyle.Left;
            this.cbtnbstopload.DrawGlowFlag = true;
            this.cbtnbstopload.DrawGlowLeftIndentFlag = true;
            this.cbtnbstopload.DrawHightLigthFlag = false;
            this.cbtnbstopload.DrawIconWidth = 0;
            this.cbtnbstopload.DrawTextBeforeImage = false;
            this.cbtnbstopload.GlowColor = System.Drawing.Color.Gainsboro;
            this.cbtnbstopload.HighlightColor = System.Drawing.Color.Gray;
            this.cbtnbstopload.Image = global::BrowserX.Properties.Resources.bstop;
            this.cbtnbstopload.ImageSize = new System.Drawing.Size(28, 28);
            this.cbtnbstopload.Location = new System.Drawing.Point(135, 0);
            this.cbtnbstopload.MouseHoverExSecond = 0;
            this.cbtnbstopload.Name = "cbtnbstopload";
            this.cbtnbstopload.Padding = new System.Windows.Forms.Padding(2);
            this.cbtnbstopload.PressedColor = System.Drawing.Color.Silver;
            this.cbtnbstopload.Size = new System.Drawing.Size(45, 36);
            this.cbtnbstopload.StateImage = null;
            this.cbtnbstopload.TabIndex = 34;
            this.cbtnbstopload.UseVisualStyleBackColor = false;
            this.cbtnbstopload.Visible = false;
            this.cbtnbstopload.Click += new System.EventHandler(this.cbtnbstopload_Click);
            // 
            // cbtnbrefresh
            // 
            this.cbtnbrefresh.BackColor = System.Drawing.Color.Transparent;
            this.cbtnbrefresh.BackImage = null;
            this.cbtnbrefresh.BaseColor = System.Drawing.Color.Transparent;
            this.cbtnbrefresh.CheckedMaskImage = null;
            this.cbtnbrefresh.CheckedMaskImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbtnbrefresh.CheckedTextColor = System.Drawing.Color.White;
            this.cbtnbrefresh.CornerRadius = 0;
            this.cbtnbrefresh.Dock = System.Windows.Forms.DockStyle.Left;
            this.cbtnbrefresh.DrawGlowFlag = true;
            this.cbtnbrefresh.DrawGlowLeftIndentFlag = true;
            this.cbtnbrefresh.DrawHightLigthFlag = false;
            this.cbtnbrefresh.DrawIconWidth = 0;
            this.cbtnbrefresh.DrawTextBeforeImage = false;
            this.cbtnbrefresh.GlowColor = System.Drawing.Color.Gainsboro;
            this.cbtnbrefresh.HighlightColor = System.Drawing.Color.Gray;
            this.cbtnbrefresh.Image = global::BrowserX.Properties.Resources.brefresh;
            this.cbtnbrefresh.ImageSize = new System.Drawing.Size(28, 28);
            this.cbtnbrefresh.Location = new System.Drawing.Point(90, 0);
            this.cbtnbrefresh.MouseHoverExSecond = 0;
            this.cbtnbrefresh.Name = "cbtnbrefresh";
            this.cbtnbrefresh.Padding = new System.Windows.Forms.Padding(2);
            this.cbtnbrefresh.PressedColor = System.Drawing.Color.Silver;
            this.cbtnbrefresh.Size = new System.Drawing.Size(45, 36);
            this.cbtnbrefresh.StateImage = null;
            this.cbtnbrefresh.TabIndex = 33;
            this.cbtnbrefresh.UseVisualStyleBackColor = false;
            this.cbtnbrefresh.Click += new System.EventHandler(this.cbtnbrefresh_Click);
            // 
            // cbtnbgoforward
            // 
            this.cbtnbgoforward.BackColor = System.Drawing.Color.Transparent;
            this.cbtnbgoforward.BackImage = null;
            this.cbtnbgoforward.BaseColor = System.Drawing.Color.Transparent;
            this.cbtnbgoforward.CheckedMaskImage = null;
            this.cbtnbgoforward.CheckedMaskImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbtnbgoforward.CheckedTextColor = System.Drawing.Color.White;
            this.cbtnbgoforward.CornerRadius = 0;
            this.cbtnbgoforward.Dock = System.Windows.Forms.DockStyle.Left;
            this.cbtnbgoforward.DrawGlowFlag = true;
            this.cbtnbgoforward.DrawGlowLeftIndentFlag = true;
            this.cbtnbgoforward.DrawHightLigthFlag = false;
            this.cbtnbgoforward.DrawIconWidth = 0;
            this.cbtnbgoforward.DrawTextBeforeImage = false;
            this.cbtnbgoforward.GlowColor = System.Drawing.Color.Gainsboro;
            this.cbtnbgoforward.HighlightColor = System.Drawing.Color.Gray;
            this.cbtnbgoforward.Image = global::BrowserX.Properties.Resources.bgoforward;
            this.cbtnbgoforward.ImageSize = new System.Drawing.Size(28, 28);
            this.cbtnbgoforward.Location = new System.Drawing.Point(45, 0);
            this.cbtnbgoforward.MouseHoverExSecond = 0;
            this.cbtnbgoforward.Name = "cbtnbgoforward";
            this.cbtnbgoforward.Padding = new System.Windows.Forms.Padding(2);
            this.cbtnbgoforward.PressedColor = System.Drawing.Color.Silver;
            this.cbtnbgoforward.Size = new System.Drawing.Size(45, 36);
            this.cbtnbgoforward.StateImage = null;
            this.cbtnbgoforward.TabIndex = 32;
            this.cbtnbgoforward.UseVisualStyleBackColor = false;
            this.cbtnbgoforward.Click += new System.EventHandler(this.cbtnbgoforward_Click);
            // 
            // cbtnbgoback
            // 
            this.cbtnbgoback.BackColor = System.Drawing.Color.Transparent;
            this.cbtnbgoback.BackImage = null;
            this.cbtnbgoback.BaseColor = System.Drawing.Color.Transparent;
            this.cbtnbgoback.CheckedMaskImage = null;
            this.cbtnbgoback.CheckedMaskImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbtnbgoback.CheckedTextColor = System.Drawing.Color.White;
            this.cbtnbgoback.CornerRadius = 0;
            this.cbtnbgoback.Dock = System.Windows.Forms.DockStyle.Left;
            this.cbtnbgoback.DrawGlowFlag = true;
            this.cbtnbgoback.DrawGlowLeftIndentFlag = true;
            this.cbtnbgoback.DrawHightLigthFlag = false;
            this.cbtnbgoback.DrawIconWidth = 0;
            this.cbtnbgoback.DrawTextBeforeImage = false;
            this.cbtnbgoback.GlowColor = System.Drawing.Color.Gainsboro;
            this.cbtnbgoback.HighlightColor = System.Drawing.Color.Gray;
            this.cbtnbgoback.Image = global::BrowserX.Properties.Resources.bgoback;
            this.cbtnbgoback.ImageSize = new System.Drawing.Size(28, 28);
            this.cbtnbgoback.Location = new System.Drawing.Point(0, 0);
            this.cbtnbgoback.MouseHoverExSecond = 0;
            this.cbtnbgoback.Name = "cbtnbgoback";
            this.cbtnbgoback.Padding = new System.Windows.Forms.Padding(2);
            this.cbtnbgoback.PressedColor = System.Drawing.Color.Silver;
            this.cbtnbgoback.Size = new System.Drawing.Size(45, 36);
            this.cbtnbgoback.StateImage = null;
            this.cbtnbgoback.TabIndex = 31;
            this.cbtnbgoback.UseVisualStyleBackColor = false;
            this.cbtnbgoback.Click += new System.EventHandler(this.cbtnbgoback_Click);
            // 
            // browserTabHeader1
            // 
            this.browserTabHeader1.BackColor = System.Drawing.Color.Honeydew;
            this.browserTabHeader1.DbClickCloseTab = true;
            this.browserTabHeader1.Dock = System.Windows.Forms.DockStyle.Top;
            this.browserTabHeader1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.browserTabHeader1.FrameNumber = 22;
            this.browserTabHeader1.Image = global::BrowserX.Properties.Resources.loadingmask22;
            this.browserTabHeader1.Location = new System.Drawing.Point(0, 0);
            this.browserTabHeader1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.browserTabHeader1.Name = "browserTabHeader1";
            this.browserTabHeader1.Size = new System.Drawing.Size(819, 32);
            this.browserTabHeader1.TabHoverColor = System.Drawing.Color.WhiteSmoke;
            this.browserTabHeader1.TabIndex = 5;
            this.browserTabHeader1.TabNormalColor = System.Drawing.Color.Lavender;
            // 
            // BrowserContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnlWebBrowser);
            this.Controls.Add(this.browserToolbar2);
            this.Controls.Add(this.browserToolbar1);
            this.Controls.Add(this.browserTabHeader1);
            this.Name = "BrowserContainer";
            this.Size = new System.Drawing.Size(819, 477);
            this.Load += new System.EventHandler(this.BrowserContainer_Load);
            this.browserToolbar1.ResumeLayout(false);
            this.pnlAddressBox.ResumeLayout(false);
            this.pnlAddressBoxParent.ResumeLayout(false);
            this.pnlCopyTitleAndUrl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private BrowserTabHeader browserTabHeader1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ImageList imageListWebIcon;
        private BrowserToolbar browserToolbar1;
        private BrowserToolbar browserToolbar2;
        private System.Windows.Forms.Panel pnlWebBrowser;
        private System.Windows.Forms.Panel pnlAddressBox;
        private System.Windows.Forms.Panel pnlAddressBoxParent;
        private System.Windows.Forms.Panel pnlCopyTitleAndUrl;
        private PanelLine pnlSeparate2;
        private PanelLine pnlSeparate1;
        private CaptionButton cbtnbfavorites;
        private CaptionButton cbtnbhomepage;
        private CaptionButton cbtnbundo;
        private CaptionButton cbtnbmainmenu;
        private CaptionButton cbtnbstopload;
        private CaptionButton cbtnbrefresh;
        private CaptionButton cbtnbgoforward;
        private CaptionButton cbtnbgoback;
        private CaptionButton cbtnCopyData;
        private AddressBox cboxAddressList;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
    }
}
