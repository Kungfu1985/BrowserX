﻿namespace BrowserX
{
    partial class SkinForm
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
            this.SuspendLayout();
            // 
            // SkinForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "SkinForm";
            this.Text = "SkinForm";
            this.Load += new System.EventHandler(this.SkinForm_Load);
            this.Shown += new System.EventHandler(this.SkinForm_Shown);
            this.SizeChanged += new System.EventHandler(this.SkinForm_SizeChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SkinForm_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SkinForm_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}