namespace BrowserX.UC
{
    partial class FolderBox
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.pnlTopParent = new System.Windows.Forms.Panel();
            this.chboxAddress = new System.Windows.Forms.RichTextBox();
            this.pnlArrowParent = new System.Windows.Forms.Panel();
            this.pboxArrow = new System.Windows.Forms.PictureBox();
            this.chboxDiskSpace = new System.Windows.Forms.RichTextBox();
            this.pnlTopParent.SuspendLayout();
            this.pnlArrowParent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pboxArrow)).BeginInit();
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("微软雅黑", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.linkLabel1.LinkColor = System.Drawing.Color.Gray;
            this.linkLabel1.Location = new System.Drawing.Point(113, 178);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(124, 20);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "make by kungfu";
            this.linkLabel1.Click += new System.EventHandler(this.linkLabel1_Click);
            // 
            // pnlTopParent
            // 
            this.pnlTopParent.BackColor = System.Drawing.Color.White;
            this.pnlTopParent.Controls.Add(this.chboxDiskSpace);
            this.pnlTopParent.Controls.Add(this.chboxAddress);
            this.pnlTopParent.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopParent.Location = new System.Drawing.Point(0, 0);
            this.pnlTopParent.Name = "pnlTopParent";
            this.pnlTopParent.Size = new System.Drawing.Size(357, 30);
            this.pnlTopParent.TabIndex = 3;
            this.pnlTopParent.SizeChanged += new System.EventHandler(this.pnlTopParent_SizeChanged);
            // 
            // chboxAddress
            // 
            this.chboxAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chboxAddress.AutoWordSelection = true;
            this.chboxAddress.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chboxAddress.DetectUrls = false;
            this.chboxAddress.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chboxAddress.ForeColor = System.Drawing.Color.Black;
            this.chboxAddress.Location = new System.Drawing.Point(3, 4);
            this.chboxAddress.Multiline = false;
            this.chboxAddress.Name = "chboxAddress";
            this.chboxAddress.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.chboxAddress.Size = new System.Drawing.Size(229, 23);
            this.chboxAddress.TabIndex = 3;
            this.chboxAddress.Text = "桌面";
            this.chboxAddress.SelectionChanged += new System.EventHandler(this.chboxAddress_SelectionChanged);
            this.chboxAddress.Click += new System.EventHandler(this.chboxAddress_Click);
            this.chboxAddress.TextChanged += new System.EventHandler(this.chboxAddress_TextChanged);
            this.chboxAddress.Enter += new System.EventHandler(this.chboxAddress_Enter);
            this.chboxAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chboxAddress_KeyDown);
            this.chboxAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.chboxAddress_KeyPress);
            this.chboxAddress.KeyUp += new System.Windows.Forms.KeyEventHandler(this.chboxAddress_KeyUp);
            this.chboxAddress.Leave += new System.EventHandler(this.chboxAddress_Leave);
            this.chboxAddress.MouseEnter += new System.EventHandler(this.chboxAddress_MouseEnter);
            this.chboxAddress.MouseLeave += new System.EventHandler(this.chboxAddress_MouseLeave);
            // 
            // pnlArrowParent
            // 
            this.pnlArrowParent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlArrowParent.BackColor = System.Drawing.Color.White;
            this.pnlArrowParent.Controls.Add(this.pboxArrow);
            this.pnlArrowParent.Location = new System.Drawing.Point(332, 1);
            this.pnlArrowParent.Name = "pnlArrowParent";
            this.pnlArrowParent.Size = new System.Drawing.Size(22, 29);
            this.pnlArrowParent.TabIndex = 4;
            // 
            // pboxArrow
            // 
            this.pboxArrow.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pboxArrow.Image = global::BrowserX.Properties.Resources.bArrow;
            this.pboxArrow.Location = new System.Drawing.Point(3, 6);
            this.pboxArrow.Name = "pboxArrow";
            this.pboxArrow.Size = new System.Drawing.Size(16, 16);
            this.pboxArrow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pboxArrow.TabIndex = 4;
            this.pboxArrow.TabStop = false;
            this.pboxArrow.Tag = "0";
            this.pboxArrow.Click += new System.EventHandler(this.pboxArrow_Click);
            this.pboxArrow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pboxArrow_MouseDown);
            // 
            // chboxDiskSpace
            // 
            this.chboxDiskSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chboxDiskSpace.AutoWordSelection = true;
            this.chboxDiskSpace.BackColor = System.Drawing.Color.White;
            this.chboxDiskSpace.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chboxDiskSpace.DetectUrls = false;
            this.chboxDiskSpace.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chboxDiskSpace.ForeColor = System.Drawing.Color.Gray;
            this.chboxDiskSpace.Location = new System.Drawing.Point(232, 6);
            this.chboxDiskSpace.MaxLength = 68;
            this.chboxDiskSpace.Multiline = false;
            this.chboxDiskSpace.Name = "chboxDiskSpace";
            this.chboxDiskSpace.ReadOnly = true;
            this.chboxDiskSpace.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chboxDiskSpace.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.chboxDiskSpace.Size = new System.Drawing.Size(93, 18);
            this.chboxDiskSpace.TabIndex = 4;
            this.chboxDiskSpace.Text = "剩余：50.78GB";
            this.chboxDiskSpace.Click += new System.EventHandler(this.chboxDiskSpace_Click);
            // 
            // FolderBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.pnlArrowParent);
            this.Controls.Add(this.pnlTopParent);
            this.Controls.Add(this.linkLabel1);
            this.Name = "FolderBox";
            this.Size = new System.Drawing.Size(357, 293);
            this.Load += new System.EventHandler(this.FolderBox_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FolderBox_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FolderBox_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FolderBox_KeyUp);
            this.pnlTopParent.ResumeLayout(false);
            this.pnlArrowParent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pboxArrow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Panel pnlTopParent;
        private System.Windows.Forms.Panel pnlArrowParent;
        private System.Windows.Forms.PictureBox pboxArrow;
        private System.Windows.Forms.RichTextBox chboxAddress;
        private System.Windows.Forms.RichTextBox chboxDiskSpace;
    }
}
