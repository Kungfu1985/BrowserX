namespace BrowserX.UC
{
    partial class BrowserTabHeader
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.cbtnMinimized = new CaptionButton();
            this.cbtnMaximized = new CaptionButton();
            this.cbtnClose = new CaptionButton();
            this.btnAddNew = new BrowserX.UC.CircleButton();
            this.SuspendLayout();
            // 
            // cbtnMinimized
            // 
            this.cbtnMinimized.BackColor = System.Drawing.Color.Transparent;
            this.cbtnMinimized.BackImage = null;
            this.cbtnMinimized.BaseColor = System.Drawing.Color.Transparent;
            this.cbtnMinimized.CheckedMaskImage = null;
            this.cbtnMinimized.CheckedMaskImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbtnMinimized.CheckedTextColor = System.Drawing.Color.White;
            this.cbtnMinimized.CornerRadius = 0;
            this.cbtnMinimized.DrawHightLigthFlag = false;
            this.cbtnMinimized.DrawIconWidth = 0;
            this.cbtnMinimized.DrawTextBeforeImage = false;
            this.cbtnMinimized.HighlightColor = System.Drawing.Color.Gray;
            this.cbtnMinimized.Image = null;
            this.cbtnMinimized.ImageSize = new System.Drawing.Size(24, 24);
            this.cbtnMinimized.Location = new System.Drawing.Point(148, 2);
            this.cbtnMinimized.MouseHoverExSecond = 0;
            this.cbtnMinimized.Name = "cbtnMinimized";
            this.cbtnMinimized.Size = new System.Drawing.Size(45, 29);
            this.cbtnMinimized.StateImage = global::BrowserX.Properties.Resources.min_mask4;
            this.cbtnMinimized.TabIndex = 3;
            this.cbtnMinimized.UseVisualStyleBackColor = false;
            this.cbtnMinimized.Click += new System.EventHandler(this.cbtnMinimized_Click);
            // 
            // cbtnMaximized
            // 
            this.cbtnMaximized.BackColor = System.Drawing.Color.Transparent;
            this.cbtnMaximized.BackImage = null;
            this.cbtnMaximized.BaseColor = System.Drawing.Color.Transparent;
            this.cbtnMaximized.CheckedMaskImage = null;
            this.cbtnMaximized.CheckedMaskImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbtnMaximized.CheckedTextColor = System.Drawing.Color.White;
            this.cbtnMaximized.CornerRadius = 0;
            this.cbtnMaximized.DrawHightLigthFlag = false;
            this.cbtnMaximized.DrawIconWidth = 0;
            this.cbtnMaximized.DrawTextBeforeImage = false;
            this.cbtnMaximized.HighlightColor = System.Drawing.Color.Gray;
            this.cbtnMaximized.Image = null;
            this.cbtnMaximized.ImageSize = new System.Drawing.Size(24, 24);
            this.cbtnMaximized.Location = new System.Drawing.Point(203, 2);
            this.cbtnMaximized.MouseHoverExSecond = 0;
            this.cbtnMaximized.Name = "cbtnMaximized";
            this.cbtnMaximized.Size = new System.Drawing.Size(45, 29);
            this.cbtnMaximized.StateImage = global::BrowserX.Properties.Resources.max_mask4;
            this.cbtnMaximized.TabIndex = 2;
            this.cbtnMaximized.UseVisualStyleBackColor = false;
            this.cbtnMaximized.Click += new System.EventHandler(this.cbtnMaximized_Click);
            // 
            // cbtnClose
            // 
            this.cbtnClose.BackColor = System.Drawing.Color.Transparent;
            this.cbtnClose.BackImage = null;
            this.cbtnClose.BaseColor = System.Drawing.Color.Transparent;
            this.cbtnClose.CheckedMaskImage = null;
            this.cbtnClose.CheckedMaskImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbtnClose.CheckedTextColor = System.Drawing.Color.White;
            this.cbtnClose.CornerRadius = 0;
            this.cbtnClose.DrawHightLigthFlag = false;
            this.cbtnClose.DrawIconWidth = 0;
            this.cbtnClose.DrawTextBeforeImage = false;
            this.cbtnClose.HighlightColor = System.Drawing.Color.Gray;
            this.cbtnClose.Image = null;
            this.cbtnClose.ImageSize = new System.Drawing.Size(24, 24);
            this.cbtnClose.Location = new System.Drawing.Point(256, 2);
            this.cbtnClose.MouseHoverExSecond = 0;
            this.cbtnClose.Name = "cbtnClose";
            this.cbtnClose.Size = new System.Drawing.Size(45, 29);
            this.cbtnClose.StateImage = global::BrowserX.Properties.Resources.close_mask4;
            this.cbtnClose.TabIndex = 1;
            this.cbtnClose.UseVisualStyleBackColor = false;
            this.cbtnClose.Click += new System.EventHandler(this.cbtnClose_Click);
            // 
            // btnAddNew
            // 
            this.btnAddNew.Location = new System.Drawing.Point(317, 8);
            this.btnAddNew.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(18, 18);
            this.btnAddNew.TabIndex = 0;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // BrowserTabHeader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.cbtnMinimized);
            this.Controls.Add(this.cbtnMaximized);
            this.Controls.Add(this.cbtnClose);
            this.Controls.Add(this.btnAddNew);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "BrowserTabHeader";
            this.Size = new System.Drawing.Size(350, 32);
            this.Load += new System.EventHandler(this.BrowserTabHeader_Load);
            this.MouseLeave += new System.EventHandler(this.BrowserTabHeader_MouseLeave);
            this.ResumeLayout(false);

        }

        #endregion

        private CircleButton btnAddNew;
        private CaptionButton cbtnClose;
        private CaptionButton cbtnMaximized;
        private CaptionButton cbtnMinimized;
    }
}
