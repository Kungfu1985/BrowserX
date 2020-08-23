using BrowserX.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrowserX.UC
{
    public class myFolderToolStrip : ToolStripButton
    {

        public myFolderToolStrip() : base()
        {
            //
            this.MouseEnter += myFolderToolStrip_MouseEnter;
            this.MouseLeave += myFolderToolStrip_MouseLeave;
            this.MouseHover += myFolderToolStrip_MouseHover;
        }

        public myFolderToolStrip(string text) : base()
        {
            //
            this.Text = text;
            this.MouseEnter += myFolderToolStrip_MouseEnter;
            this.MouseLeave += myFolderToolStrip_MouseLeave;
            this.MouseHover += myFolderToolStrip_MouseHover;
        }

        ~myFolderToolStrip()
        {
            //
            this.MouseEnter -= myFolderToolStrip_MouseEnter;
            this.MouseLeave -= myFolderToolStrip_MouseLeave;
            this.MouseHover -= myFolderToolStrip_MouseHover;
        }

        private void myFolderToolStrip_MouseHover(object sender, EventArgs e)
        {
            State = DrawItemState.HotLight;
            this.Invalidate();
            //Win32API.OutputDebugStringA(string.Format("项：{0},鼠标悬停！", Text));
            //throw new NotImplementedException();
        }

        private void myFolderToolStrip_MouseLeave(object sender, EventArgs e)
        {
            State = DrawItemState.Default;
            this.Invalidate();
            //Win32API.OutputDebugStringA(string.Format("项：{0},鼠标移出！", Text));
            //throw new NotImplementedException();
        }

        private void myFolderToolStrip_MouseEnter(object sender, EventArgs e)
        {
            State = DrawItemState.HotLight;
            this.Invalidate();
            //Win32API.OutputDebugStringA(string.Format("项：{0},鼠标移入！", Text)); 
            //throw new NotImplementedException();
        }

        #region "公开属性"
        // 下拉列表中项选中时的的背景
        private Color _SelectedColor = Color.FromArgb(255, 194, 194, 194);
        [Description("下拉列表中项选中时的的背景。")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "194, 194, 194")]
        [Category("Settings")]
        public Color SelectedColor
        {
            get
            {
                return _SelectedColor;
            }

            set
            {
                _SelectedColor = value;

                this.Invalidate();
            }
        }
        // 下拉列表中项鼠标悬停时的的背景
        private Color _HoverColor = Color.FromArgb(255, 218, 218, 218);
        [Description("下拉列表中项鼠标悬停时的的背景。")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "218, 218, 218")]
        [Category("Settings")]
        public Color HoverColor
        {
            get
            {
                return _HoverColor;
            }

            set
            {
                _HoverColor = value;

                this.Invalidate();
            }
        }


        // 下拉列表中项未选中时的的背景
        private Color _NormalColor = Color.FromArgb(255, 240, 240, 240);
        [Description("下拉列表中项未选中时的的背景。")]
        [Browsable(true)]
        [DefaultValue(typeof(Color), "240, 240, 240")]
        [Category("Settings")]
        public Color NormalColor
        {
            get
            {
                return _NormalColor;
            }

            set
            {
                _NormalColor = value;

                this.Invalidate();
            }
        }


        // 下拉列表是否是被选中的项
        private bool _IsSelected = false;
        private int _SelectedIndex = -1;
        [Description("下拉列表是否是被选中的项。")]
        [Browsable(true)]
        [DefaultValue(typeof(bool), "false")]
        [Category("Settings")]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }

            set
            {
                _IsSelected = value;

                this.Invalidate();
            }
        }

        // 下拉列表项的字号
        private int _FontSize = 10;
        [Description("下拉列表项的字号。")]
        [Browsable(true)]
        [DefaultValue(typeof(int), "10")]
        [Category("Settings")]
        public int FontSize
        {
            get
            {
                return _FontSize;
            }

            set
            {
                _FontSize = value;

                this.Invalidate();
            }
        }


        #endregion

        #region "自定义函数"
        /// <summary>
        ///     ''' 文本对齐方式
        ///     ''' </summary>
        ///     ''' <param name="textalign"></param>
        ///     ''' <returns></returns>
        private StringFormat SFAlignment(ContentAlignment textalign)
        {
            StringFormat sf = new StringFormat();
            switch ((textalign))
            {
                case ContentAlignment.TopLeft:         // test ok
                    {
                        sf.LineAlignment = StringAlignment.Near;
                        sf.Alignment = StringAlignment.Near;
                        break;
                    }

                case ContentAlignment.TopCenter:        // test ok
                    {
                        sf.LineAlignment = StringAlignment.Near;
                        sf.Alignment = StringAlignment.Center;
                        break;
                    }

                case ContentAlignment.TopRight:         // test ok
                    {
                        sf.LineAlignment = StringAlignment.Near;
                        sf.Alignment = StringAlignment.Far;
                        break;
                    }

                case ContentAlignment.MiddleLeft:
                    {
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Near;
                        break;
                    }

                case ContentAlignment.MiddleCenter:
                    {
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Center;
                        break;
                    }

                case ContentAlignment.MiddleRight:
                    {
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Far;
                        break;
                    }

                case ContentAlignment.BottomLeft:        // test ok
                    {
                        sf.LineAlignment = StringAlignment.Far;
                        sf.Alignment = StringAlignment.Near;
                        break;
                    }

                case ContentAlignment.BottomCenter:      // test ok
                    {
                        sf.LineAlignment = StringAlignment.Far;
                        sf.Alignment = StringAlignment.Center;
                        break;
                    }

                case ContentAlignment.BottomRight:       // test ok
                    {
                        sf.LineAlignment = StringAlignment.Far;
                        sf.Alignment = StringAlignment.Far;
                        break;
                    }
            }
            return sf;
        }
        #endregion

        private bool StateImageStrech = false;
        private DrawItemState State = DrawItemState.Default;


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
        }

        /// <summary>
        /// 自绘
        /// </summary>
        /// <param name="pevent"></param>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs pevent)
        {
            Size imageSize = new Size(16, 16);

            Rectangle bounds = new Rectangle();
            bounds = pevent.ClipRectangle;
            try
            {
                StringFormat sf = SFAlignment(this.TextAlign);

                int newLeft, newTop, newWidth, newHeight;
                newLeft = bounds.Left;
                newTop = bounds.Top;

                newLeft = bounds.Left + imageSize.Width * 2;
                newTop = bounds.Top; // + Me.ImageList.ImageSize.Height

                newWidth = bounds.Width - newLeft;
                newHeight = bounds.Height;
                Rectangle r = new Rectangle(newLeft, newTop, newWidth, newHeight);
                Rectangle rText = new Rectangle(newLeft, newTop, newWidth - 8, newHeight);
                Rectangle rItemBackground = new Rectangle(bounds.Left, bounds.Top, newWidth, newHeight);

                //OutputDebugStringA( string.Format("当前绘制项的状态：{0}", e.State )  );
                bool bolSelected = false;

                //Win32API.OutputDebugStringA(string.Format("Image.Size({0}x{1})", Image.Size.Width, Image.Size.Height));

                if (State == DrawItemState.HotLight)
                {
                    pevent.Graphics.FillRectangle(new SolidBrush(this.HoverColor), bounds);
                }
                else
                {
                    //Win32API.OutputDebugStringA( string.Format("当前项：{0}，是否处于选中状态：{1} ", Text, IsSelected) ); 
                    if (IsSelected)
                    {
                        pevent.Graphics.FillRectangle(new SolidBrush(this.SelectedColor), bounds);
                        bolSelected = true;
                    }
                    else
                    {
                        pevent.Graphics.FillRectangle(new SolidBrush(this.NormalColor), bounds);
                        bolSelected = false;
                    }
                }

                //画图标与文字
                //画图标
                if (ImageAlign == ContentAlignment.MiddleRight)
                {
                    System.Drawing.Font font = new Font("微软雅黑", FontSize);
                    System.Drawing.SizeF size = pevent.Graphics.MeasureString(Text, font);  // 计算字符串所需要的大小

                    pevent.Graphics.DrawImage(Image, bounds.Width - size.Width - Image.Width - FontSize, bounds.Top + Image.Size.Height / 2, 16, 16);
                }
                else
                {
                    //Win32API.OutputDebugStringA(string.Format("Bounds(Top:{0}, Left:{1})", bounds.Top, bounds.Left));
                    pevent.Graphics.DrawImage(Image, bounds.Left + Image.Size.Width / 2, bounds.Top + Image.Size.Height / 4, 16, 16);
                }
                //画文字
                if (TextAlign == ContentAlignment.MiddleRight)
                {
                    TextRenderer.DrawText(pevent.Graphics, Text, new Font("微软雅黑", FontSize), rText, ForeColor, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
                }
                else
                {
                    TextRenderer.DrawText(pevent.Graphics, Text, new Font("微软雅黑", FontSize), r, ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                }
            }
            catch (Exception ex)
            {
                Win32API.OutputDebugStringA("myFolderToolStrip 画图标时出现错误" + ex.Message);
            }
        }

        //end
    }
}
