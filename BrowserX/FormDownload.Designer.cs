namespace BrowserX
{
    partial class FormDownload
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
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.pnlBottomLine = new System.Windows.Forms.Panel();
            this.btnVirtualOne = new CaptionButton();
            this.captionButton1 = new CaptionButton();
            this.btnClearAll = new CaptionButton();
            this.pnlDownloadList = new BrowserX.UC.PanelOwner();
            this.TableLayoutListParent = new System.Windows.Forms.TableLayoutPanel();
            this.ToolStripDownloadList = new System.Windows.Forms.ToolStrip();
            this.pnlBottom.SuspendLayout();
            this.pnlDownloadList.SuspendLayout();
            this.TableLayoutListParent.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.White;
            this.pnlBottom.Controls.Add(this.pnlBottomLine);
            this.pnlBottom.Controls.Add(this.btnVirtualOne);
            this.pnlBottom.Controls.Add(this.captionButton1);
            this.pnlBottom.Controls.Add(this.btnClearAll);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 348);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(654, 53);
            this.pnlBottom.TabIndex = 31;
            // 
            // pnlBottomLine
            // 
            this.pnlBottomLine.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlBottomLine.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBottomLine.Location = new System.Drawing.Point(0, 0);
            this.pnlBottomLine.Name = "pnlBottomLine";
            this.pnlBottomLine.Size = new System.Drawing.Size(654, 1);
            this.pnlBottomLine.TabIndex = 4;
            // 
            // btnVirtualOne
            // 
            this.btnVirtualOne.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVirtualOne.BackColor = System.Drawing.Color.Transparent;
            this.btnVirtualOne.BackImage = null;
            this.btnVirtualOne.BaseColor = System.Drawing.Color.White;
            this.btnVirtualOne.BoardColor = System.Drawing.Color.Gray;
            this.btnVirtualOne.CheckedMaskImage = null;
            this.btnVirtualOne.CheckedMaskImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnVirtualOne.CheckedTextColor = System.Drawing.Color.White;
            this.btnVirtualOne.CornerRadius = 0;
            this.btnVirtualOne.DrawBoard = true;
            this.btnVirtualOne.DrawGlowFlag = true;
            this.btnVirtualOne.DrawHightLigthFlag = false;
            this.btnVirtualOne.DrawIconWidth = 32;
            this.btnVirtualOne.DrawTextBeforeImage = false;
            this.btnVirtualOne.Enabled = false;
            this.btnVirtualOne.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnVirtualOne.ForeColor = System.Drawing.Color.Black;
            this.btnVirtualOne.GlowColor = System.Drawing.Color.Gainsboro;
            this.btnVirtualOne.HighlightColor = System.Drawing.Color.Gray;
            this.btnVirtualOne.Image = global::BrowserX.Properties.Resources.mnudownload;
            this.btnVirtualOne.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVirtualOne.ImageSize = new System.Drawing.Size(24, 24);
            this.btnVirtualOne.Location = new System.Drawing.Point(12, 8);
            this.btnVirtualOne.MouseHoverExSecond = 0;
            this.btnVirtualOne.Name = "btnVirtualOne";
            this.btnVirtualOne.PressedColor = System.Drawing.Color.Silver;
            this.btnVirtualOne.Size = new System.Drawing.Size(133, 33);
            this.btnVirtualOne.StateImage = null;
            this.btnVirtualOne.TabIndex = 3;
            this.btnVirtualOne.Text = "模拟一条下载";
            this.btnVirtualOne.UseVisualStyleBackColor = false;
            this.btnVirtualOne.Visible = false;
            this.btnVirtualOne.Click += new System.EventHandler(this.btnVirtualOne_Click);
            // 
            // captionButton1
            // 
            this.captionButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.captionButton1.BackColor = System.Drawing.Color.Transparent;
            this.captionButton1.BackImage = null;
            this.captionButton1.BaseColor = System.Drawing.Color.White;
            this.captionButton1.BoardColor = System.Drawing.Color.Gray;
            this.captionButton1.CausesValidation = false;
            this.captionButton1.CheckedMaskImage = null;
            this.captionButton1.CheckedMaskImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.captionButton1.CheckedTextColor = System.Drawing.Color.White;
            this.captionButton1.CornerRadius = 0;
            this.captionButton1.DrawBoard = true;
            this.captionButton1.DrawGlowFlag = true;
            this.captionButton1.DrawHightLigthFlag = false;
            this.captionButton1.DrawIconWidth = 32;
            this.captionButton1.DrawTextBeforeImage = false;
            this.captionButton1.Enabled = false;
            this.captionButton1.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.captionButton1.ForeColor = System.Drawing.Color.Black;
            this.captionButton1.GlowColor = System.Drawing.Color.Gainsboro;
            this.captionButton1.HighlightColor = System.Drawing.Color.Gray;
            this.captionButton1.Image = global::BrowserX.Properties.Resources.mnufolders;
            this.captionButton1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.captionButton1.ImageSize = new System.Drawing.Size(24, 24);
            this.captionButton1.Location = new System.Drawing.Point(402, 8);
            this.captionButton1.MouseHoverExSecond = 0;
            this.captionButton1.Name = "captionButton1";
            this.captionButton1.PressedColor = System.Drawing.Color.Silver;
            this.captionButton1.Size = new System.Drawing.Size(117, 33);
            this.captionButton1.StateImage = null;
            this.captionButton1.TabIndex = 2;
            this.captionButton1.Text = "新建下载";
            this.captionButton1.UseVisualStyleBackColor = false;
            this.captionButton1.Click += new System.EventHandler(this.captionButton1_Click);
            // 
            // btnClearAll
            // 
            this.btnClearAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearAll.BackColor = System.Drawing.Color.Transparent;
            this.btnClearAll.BackImage = null;
            this.btnClearAll.BaseColor = System.Drawing.Color.White;
            this.btnClearAll.BoardColor = System.Drawing.Color.Gray;
            this.btnClearAll.CheckedMaskImage = null;
            this.btnClearAll.CheckedMaskImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnClearAll.CheckedTextColor = System.Drawing.Color.White;
            this.btnClearAll.CornerRadius = 0;
            this.btnClearAll.DrawBoard = true;
            this.btnClearAll.DrawGlowFlag = true;
            this.btnClearAll.DrawHightLigthFlag = false;
            this.btnClearAll.DrawIconWidth = 32;
            this.btnClearAll.DrawTextBeforeImage = false;
            this.btnClearAll.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnClearAll.ForeColor = System.Drawing.Color.Black;
            this.btnClearAll.GlowColor = System.Drawing.Color.Gainsboro;
            this.btnClearAll.HighlightColor = System.Drawing.Color.Gray;
            this.btnClearAll.Image = global::BrowserX.Properties.Resources.clearItem;
            this.btnClearAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClearAll.ImageSize = new System.Drawing.Size(16, 16);
            this.btnClearAll.ImageTintUseThemeColor = true;
            this.btnClearAll.Location = new System.Drawing.Point(525, 8);
            this.btnClearAll.MouseHoverExSecond = 0;
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.PressedColor = System.Drawing.Color.Silver;
            this.btnClearAll.Size = new System.Drawing.Size(117, 33);
            this.btnClearAll.StateImage = null;
            this.btnClearAll.TabIndex = 0;
            this.btnClearAll.Text = "清空已下载";
            this.btnClearAll.UseVisualStyleBackColor = false;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // pnlDownloadList
            // 
            this.pnlDownloadList.AutoScroll = true;
            this.pnlDownloadList.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlDownloadList.BackColor = System.Drawing.Color.White;
            this.pnlDownloadList.Controls.Add(this.TableLayoutListParent);
            this.pnlDownloadList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDownloadList.Location = new System.Drawing.Point(0, 0);
            this.pnlDownloadList.Name = "pnlDownloadList";
            this.pnlDownloadList.Size = new System.Drawing.Size(654, 348);
            this.pnlDownloadList.TabIndex = 32;
            this.pnlDownloadList.Scroll += new System.Windows.Forms.ScrollEventHandler(this.pnlDownloadList_Scroll);
            // 
            // TableLayoutListParent
            // 
            this.TableLayoutListParent.AutoSize = true;
            this.TableLayoutListParent.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TableLayoutListParent.ColumnCount = 1;
            this.TableLayoutListParent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutListParent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutListParent.Controls.Add(this.ToolStripDownloadList, 0, 0);
            this.TableLayoutListParent.Location = new System.Drawing.Point(0, 0);
            this.TableLayoutListParent.Name = "TableLayoutListParent";
            this.TableLayoutListParent.RowCount = 1;
            this.TableLayoutListParent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutListParent.Size = new System.Drawing.Size(25, 102);
            this.TableLayoutListParent.TabIndex = 32;
            // 
            // ToolStripDownloadList
            // 
            this.ToolStripDownloadList.AllowItemReorder = true;
            this.ToolStripDownloadList.AllowMerge = false;
            this.ToolStripDownloadList.BackColor = System.Drawing.Color.White;
            this.ToolStripDownloadList.CanOverflow = false;
            this.ToolStripDownloadList.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolStripDownloadList.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.ToolStripDownloadList.Location = new System.Drawing.Point(0, 0);
            this.ToolStripDownloadList.Name = "ToolStripDownloadList";
            this.ToolStripDownloadList.Padding = new System.Windows.Forms.Padding(0);
            this.ToolStripDownloadList.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.ToolStripDownloadList.Size = new System.Drawing.Size(25, 102);
            this.ToolStripDownloadList.Stretch = true;
            this.ToolStripDownloadList.TabIndex = 32;
            this.ToolStripDownloadList.SizeChanged += new System.EventHandler(this.ToolStripDownloadList_SizeChanged);
            this.ToolStripDownloadList.Paint += new System.Windows.Forms.PaintEventHandler(this.ToolStripDownloadList_Paint);
            // 
            // FormDownload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(654, 401);
            this.Controls.Add(this.pnlDownloadList);
            this.Controls.Add(this.pnlBottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormDownload";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "下载 - 云加速由白云提供";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDownload_FormClosing);
            this.Shown += new System.EventHandler(this.FormDownload_Shown);
            this.pnlBottom.ResumeLayout(false);
            this.pnlDownloadList.ResumeLayout(false);
            this.pnlDownloadList.PerformLayout();
            this.TableLayoutListParent.ResumeLayout(false);
            this.TableLayoutListParent.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pnlBottom;
        private CaptionButton btnClearAll;
        private CaptionButton captionButton1;
        private CaptionButton btnVirtualOne;
        private System.Windows.Forms.Panel pnlBottomLine;
        private UC.PanelOwner pnlDownloadList;
        private System.Windows.Forms.TableLayoutPanel TableLayoutListParent;
        private System.Windows.Forms.ToolStrip ToolStripDownloadList;
    }
}