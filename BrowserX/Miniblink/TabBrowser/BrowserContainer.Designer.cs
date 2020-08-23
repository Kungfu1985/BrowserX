namespace BroswerX.UC
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
            this.browserToolbar2 = new BroswerX.UC.BrowserToolbar();
            this.browserToolbar1 = new BroswerX.UC.BrowserToolbar();
            this.browserTabHeader1 = new BroswerX.UC.BrowserTabHeader();
            this.SuspendLayout();
            // 
            // browserToolbar2
            // 
            this.browserToolbar2.BackColor = System.Drawing.Color.Lavender;
            this.browserToolbar2.Dock = System.Windows.Forms.DockStyle.Top;
            this.browserToolbar2.DrawBottomLine = true;
            this.browserToolbar2.DrawLinearBack = false;
            this.browserToolbar2.Location = new System.Drawing.Point(0, 55);
            this.browserToolbar2.Name = "browserToolbar2";
            this.browserToolbar2.Size = new System.Drawing.Size(819, 30);
            this.browserToolbar2.TabIndex = 7;
            // 
            // browserToolbar1
            // 
            this.browserToolbar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.browserToolbar1.DrawBottomLine = false;
            this.browserToolbar1.DrawLinearBack = true;
            this.browserToolbar1.Location = new System.Drawing.Point(0, 25);
            this.browserToolbar1.Name = "browserToolbar1";
            this.browserToolbar1.Size = new System.Drawing.Size(819, 30);
            this.browserToolbar1.TabIndex = 6;
            // 
            // browserTabHeader1
            // 
            this.browserTabHeader1.BackColor = System.Drawing.Color.White;
            this.browserTabHeader1.Dock = System.Windows.Forms.DockStyle.Top;
            this.browserTabHeader1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.browserTabHeader1.Location = new System.Drawing.Point(0, 0);
            this.browserTabHeader1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.browserTabHeader1.Name = "browserTabHeader1";
            this.browserTabHeader1.Size = new System.Drawing.Size(819, 25);
            this.browserTabHeader1.TabIndex = 5;
            // 
            // BrowserContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.browserToolbar2);
            this.Controls.Add(this.browserToolbar1);
            this.Controls.Add(this.browserTabHeader1);
            this.Name = "BrowserContainer";
            this.Size = new System.Drawing.Size(819, 477);
            this.ResumeLayout(false);

        }

        #endregion

        private BrowserTabHeader browserTabHeader1;
        private BrowserToolbar browserToolbar1;
        private BrowserToolbar browserToolbar2;
    }
}
