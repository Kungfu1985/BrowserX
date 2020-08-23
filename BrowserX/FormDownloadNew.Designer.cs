namespace BrowserX
{
    partial class FormDownloadNew
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.chboxAddress = new System.Windows.Forms.RichTextBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.tboxFileSize = new System.Windows.Forms.TextBox();
            this.tboxFileName = new System.Windows.Forms.TextBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel12 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panelLine1 = new PanelLine();
            this.cboxSavePath = new BrowserX.UC.FolderBox();
            this.btnSelectFolder = new CaptionButton();
            this.btnOk = new CaptionButton();
            this.btnCancel = new CaptionButton();
            this.panel1.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel12.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Gainsboro;
            this.panel1.Controls.Add(this.panel7);
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(497, 46);
            this.panel1.TabIndex = 0;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.White;
            this.panel7.Controls.Add(this.panel10);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(89, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(408, 46);
            this.panel7.TabIndex = 1;
            // 
            // panel10
            // 
            this.panel10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel10.Controls.Add(this.chboxAddress);
            this.panel10.Location = new System.Drawing.Point(5, 7);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(377, 30);
            this.panel10.TabIndex = 5;
            // 
            // chboxAddress
            // 
            this.chboxAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chboxAddress.AutoWordSelection = true;
            this.chboxAddress.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chboxAddress.DetectUrls = false;
            this.chboxAddress.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chboxAddress.ForeColor = System.Drawing.Color.Gray;
            this.chboxAddress.Location = new System.Drawing.Point(2, 3);
            this.chboxAddress.Multiline = false;
            this.chboxAddress.Name = "chboxAddress";
            this.chboxAddress.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.chboxAddress.Size = new System.Drawing.Size(369, 19);
            this.chboxAddress.TabIndex = 5;
            this.chboxAddress.Text = "";
            this.chboxAddress.TextChanged += new System.EventHandler(this.chboxAddress_TextChanged);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.Controls.Add(this.label1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(89, 46);
            this.panel4.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(40, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "网址：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox1.Location = new System.Drawing.Point(21, 158);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(75, 21);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "不再询问";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Gray;
            this.panel2.Controls.Add(this.panel8);
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 46);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(497, 46);
            this.panel2.TabIndex = 7;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.White;
            this.panel8.Controls.Add(this.panel11);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(89, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(408, 46);
            this.panel8.TabIndex = 2;
            // 
            // panel11
            // 
            this.panel11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel11.Controls.Add(this.tboxFileSize);
            this.panel11.Controls.Add(this.tboxFileName);
            this.panel11.Location = new System.Drawing.Point(5, 7);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(377, 30);
            this.panel11.TabIndex = 1;
            // 
            // tboxFileSize
            // 
            this.tboxFileSize.BackColor = System.Drawing.Color.White;
            this.tboxFileSize.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tboxFileSize.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tboxFileSize.ForeColor = System.Drawing.Color.Gray;
            this.tboxFileSize.Location = new System.Drawing.Point(267, 5);
            this.tboxFileSize.Name = "tboxFileSize";
            this.tboxFileSize.ReadOnly = true;
            this.tboxFileSize.Size = new System.Drawing.Size(103, 16);
            this.tboxFileSize.TabIndex = 2;
            this.tboxFileSize.Text = "未知";
            this.tboxFileSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tboxFileName
            // 
            this.tboxFileName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tboxFileName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tboxFileName.Location = new System.Drawing.Point(3, 5);
            this.tboxFileName.Name = "tboxFileName";
            this.tboxFileName.Size = new System.Drawing.Size(260, 16);
            this.tboxFileName.TabIndex = 1;
            this.tboxFileName.TextChanged += new System.EventHandler(this.tboxFileName_TextChanged);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.White;
            this.panel5.Controls.Add(this.label2);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(89, 46);
            this.panel5.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(40, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "文件名：";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.panel9);
            this.panel3.Controls.Add(this.panel6);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 92);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(497, 46);
            this.panel3.TabIndex = 8;
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.panel12);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel9.Location = new System.Drawing.Point(89, 0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(408, 46);
            this.panel9.TabIndex = 2;
            // 
            // panel12
            // 
            this.panel12.Controls.Add(this.cboxSavePath);
            this.panel12.Controls.Add(this.btnSelectFolder);
            this.panel12.Location = new System.Drawing.Point(3, 6);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(384, 31);
            this.panel12.TabIndex = 0;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.label3);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(89, 46);
            this.panel6.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(40, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 25);
            this.label3.TabIndex = 1;
            this.label3.Text = "下载到：";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelLine1
            // 
            this.panelLine1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLine1.LineColor = System.Drawing.Color.Gainsboro;
            this.panelLine1.LineMargin = new System.Windows.Forms.Padding(0);
            this.panelLine1.Location = new System.Drawing.Point(0, 138);
            this.panelLine1.Name = "panelLine1";
            this.panelLine1.Orientation = PanelLine.EnumOrientation.Vertical;
            this.panelLine1.Size = new System.Drawing.Size(497, 4);
            this.panelLine1.TabIndex = 9;
            // 
            // cboxSavePath
            // 
            this.cboxSavePath.BackColor = System.Drawing.SystemColors.Control;
            this.cboxSavePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cboxSavePath.ClearTips = "清空下载历史列表";
            this.cboxSavePath.DiskSpace = "剩余：51.9 GB";
            this.cboxSavePath.Items.Add("桌面");
            this.cboxSavePath.Items.Add("E:\\123");
            this.cboxSavePath.Items.Add("F:\\Download");
            this.cboxSavePath.Items.Add("清空地址栏下拉列表");
            this.cboxSavePath.Location = new System.Drawing.Point(2, 1);
            this.cboxSavePath.Name = "cboxSavePath";
            this.cboxSavePath.Size = new System.Drawing.Size(341, 30);
            this.cboxSavePath.TabIndex = 11;
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectFolder.BackColor = System.Drawing.Color.Transparent;
            this.btnSelectFolder.BackImage = null;
            this.btnSelectFolder.BaseColor = System.Drawing.Color.White;
            this.btnSelectFolder.BoardColor = System.Drawing.Color.Gray;
            this.btnSelectFolder.CheckedMaskImage = null;
            this.btnSelectFolder.CheckedMaskImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnSelectFolder.CheckedTextColor = System.Drawing.Color.White;
            this.btnSelectFolder.CornerRadius = 0;
            this.btnSelectFolder.DrawBoard = true;
            this.btnSelectFolder.DrawGlowFlag = true;
            this.btnSelectFolder.DrawHightLigthFlag = false;
            this.btnSelectFolder.DrawIconWidth = 0;
            this.btnSelectFolder.DrawTextBeforeImage = false;
            this.btnSelectFolder.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSelectFolder.ForeColor = System.Drawing.Color.Black;
            this.btnSelectFolder.GlowColor = System.Drawing.Color.Gainsboro;
            this.btnSelectFolder.HighlightColor = System.Drawing.Color.Gray;
            this.btnSelectFolder.Image = global::BrowserX.Properties.Resources.mnufolders;
            this.btnSelectFolder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSelectFolder.ImageSize = new System.Drawing.Size(24, 24);
            this.btnSelectFolder.ImageTintUseThemeColor = true;
            this.btnSelectFolder.Location = new System.Drawing.Point(342, 1);
            this.btnSelectFolder.MouseHoverExSecond = 0;
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.PressedColor = System.Drawing.Color.Silver;
            this.btnSelectFolder.Size = new System.Drawing.Size(37, 30);
            this.btnSelectFolder.StateImage = null;
            this.btnSelectFolder.TabIndex = 5;
            this.btnSelectFolder.UseVisualStyleBackColor = false;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.BackColor = System.Drawing.Color.Transparent;
            this.btnOk.BackImage = null;
            this.btnOk.BaseColor = System.Drawing.Color.White;
            this.btnOk.BoardColor = System.Drawing.Color.Gray;
            this.btnOk.CheckedMaskImage = null;
            this.btnOk.CheckedMaskImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnOk.CheckedTextColor = System.Drawing.Color.White;
            this.btnOk.CornerRadius = 0;
            this.btnOk.DrawBoard = true;
            this.btnOk.DrawGlowFlag = true;
            this.btnOk.DrawHightLigthFlag = false;
            this.btnOk.DrawIconWidth = 0;
            this.btnOk.DrawTextBeforeImage = false;
            this.btnOk.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOk.ForeColor = System.Drawing.Color.Black;
            this.btnOk.GlowColor = System.Drawing.Color.Gainsboro;
            this.btnOk.HighlightColor = System.Drawing.Color.Gray;
            this.btnOk.Image = null;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.ImageSize = new System.Drawing.Size(16, 16);
            this.btnOk.ImageTintUseThemeColor = true;
            this.btnOk.Location = new System.Drawing.Point(279, 153);
            this.btnOk.MouseHoverExSecond = 0;
            this.btnOk.Name = "btnOk";
            this.btnOk.PressedColor = System.Drawing.Color.Silver;
            this.btnOk.Size = new System.Drawing.Size(85, 29);
            this.btnOk.StateImage = null;
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "确定";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BackImage = null;
            this.btnCancel.BaseColor = System.Drawing.Color.White;
            this.btnCancel.BoardColor = System.Drawing.Color.Gray;
            this.btnCancel.CheckedMaskImage = null;
            this.btnCancel.CheckedMaskImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnCancel.CheckedTextColor = System.Drawing.Color.White;
            this.btnCancel.CornerRadius = 0;
            this.btnCancel.DrawBoard = true;
            this.btnCancel.DrawGlowFlag = true;
            this.btnCancel.DrawHightLigthFlag = false;
            this.btnCancel.DrawIconWidth = 0;
            this.btnCancel.DrawTextBeforeImage = false;
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.GlowColor = System.Drawing.Color.Gainsboro;
            this.btnCancel.HighlightColor = System.Drawing.Color.Gray;
            this.btnCancel.Image = null;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.ImageSize = new System.Drawing.Size(16, 16);
            this.btnCancel.ImageTintUseThemeColor = true;
            this.btnCancel.Location = new System.Drawing.Point(387, 153);
            this.btnCancel.MouseHoverExSecond = 0;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.PressedColor = System.Drawing.Color.Silver;
            this.btnCancel.Size = new System.Drawing.Size(85, 29);
            this.btnCancel.StateImage = null;
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormDownloadNew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(497, 194);
            this.Controls.Add(this.panelLine1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDownloadNew";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "新建下载任务";
            this.Load += new System.EventHandler(this.FormDownloadNew_Load);
            this.panel1.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.panel11.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private CaptionButton btnCancel;
        private CaptionButton btnOk;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private PanelLine panelLine1;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.RichTextBox chboxAddress;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.TextBox tboxFileSize;
        private System.Windows.Forms.TextBox tboxFileName;
        private System.Windows.Forms.Panel panel12;
        private CaptionButton btnSelectFolder;
        private UC.FolderBox cboxSavePath;
    }
}