using System;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Resources;

namespace MiniBlink
{
    // 创建子类Menu,继承于ToolStripMenuItem
    public class MenuItemDraw : System.Windows.Forms.ToolStripMenuItem
    {
        private System.ComponentModel.Container components = null;

        private const int TEXTSTART = 32;
        // Private icon As Image = Nothing
        private string shortcuttext = "";
        public int itemHeight;
        public bool IsMouseHover = false;
        // 菜单项最左侧显示 Check及Icon的区域的颜色
        public Color ibgcolor = Color.FromArgb(255, 255, 255);

        private Color bgcolor = Color.FromArgb(255, 255, 255);
        [Description("自定义菜单项默认显示的背景颜色")]
        [Browsable(true)]
        [Category("BackStyle")]
        public Color MouseLeaveColor
        {
            get
            {
                return bgcolor;
            }

            set
            {
                bgcolor = value;

                this.Invalidate();
            }
        }

        // 鼠标移入菜单项时，背景色的边框
        private Color sbcolor = Color.FromArgb(56, 53, 52);

        [Description("自定义菜单项鼠标悬停时的背景颜色")]
        [Browsable(true)]
        [Category("BackStyle")]
        public Color MouseHoverColor
        {
            get
            {
                return sbcolor;
            }
            set
            {
                sbcolor = value;

                this.Invalidate();
            }
        }

        // 鼠标移入菜单项时，背景色的边框
        private Color sbbcolor = Color.FromArgb(56, 53, 52);

        [Description("自定义菜单项鼠标悬停时的边框颜色")]
        [Browsable(true)]
        [Category("BackStyle")]
        public Color MouseHoverBoardColor
        {
            get
            {
                return sbbcolor;
            }
            set
            {
                sbbcolor = value;

                this.Invalidate();
            }
        }

        private Color sepColor = Color.FromArgb(242, 242, 242);    // 0, 0, 128)
        [Description("自定义菜单项分隔符的显示颜色")]
        [Browsable(true)]
        [Category("BackStyle")]
        public Color SeparatorColor
        {
            get
            {
                return sepColor;
            }

            set
            {
                sepColor = value;

                this.Invalidate();
            }
        }

        // 添加新属性MenuItemIcon,主要是用来设置菜单项左边的图形
        private Image iconArrow = null/* TODO Change to default(_) if this is not a reference type */;
        [Description("自定义菜单项拥有子菜单项时的标识图标，默认[>]图标")]
        [Browsable(true)]
        [Category("BackStyle")]
        public Image MenuSubItemArrow
        {
            get
            {
                return iconArrow;
            }
            set
            {
                iconArrow = value;
                this.Invalidate();
            }
        }


        // 添加新属性MenuItemIcon,主要是用来设置菜单项左边的图形
        private Image iconCheck = null/* TODO Change to default(_) if this is not a reference type */;
        [Description("自定义菜单项被选中时的标识图标，默认[对勾]图标")]
        [Browsable(true)]
        [Category("BackStyle")]
        public Image MenuItemCheckIcon
        {
            get
            {
                return iconCheck;
            }
            set
            {
                iconCheck = value;
                this.Invalidate();
            }
        }


        public MenuItemDraw() : base()
        {
            base.Padding = new Padding(0, 6, 0, 1);
            if ((base.Checked))
            {
            }
            if ((base.DropDown.Items.Count > 0))
            {
            }
        }


        public MenuItemDraw(String text, Image ico, EventHandler click) : base()
        {
            base.Padding = new Padding(0, 6, 0, 1);
            try
            {
                if (text.Length > 0)
                {
                    base.Text = text;
                }
                if (ico != null)
                {
                    base.Image = ico;
                }
                if (click != null)
                {
                    base.Click += click;
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected override void Dispose(bool disposing)
        {
            if ((disposing))
            {
                if (!(components == null))
                    components.Dispose();
            }
            base.Dispose(disposing);
        }


        protected override void SetBounds(Rectangle rect)
        {
            //if (Text == "-") {
            //    Rectangle bs = new Rectangle(rect.Left, rect.Top, rect.Width, 2);
            //    base.SetBounds(bs);
            //}
            //else {
            //    base.SetBounds(rect);
            //}
            base.SetBounds(rect);
        }


        // 覆盖OnPaint方法
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);

                Graphics g = e.Graphics;
                Rectangle bounds = new Rectangle(0, 0, Width, Height);
                bool selected = IsMouseHover;
                bool hasicon = (!(Image == null));
                bool hassubmenu = (DropDown.Items.Count > 0);
                bool ena = Enabled;

                DrawBackground(g, bounds, hasicon, IsMouseHover, ena);
                if (hasicon)
                    DrawIcon(g, Image, bounds, selected, Enabled, Checked);
                else if (Checked)
                    DrawCheckmark(g, bounds, selected, hasicon);

                if ((hassubmenu))
                {
                    try
                    {
                        //从项目资源文件中读取  Properties.Resources.Key 同样可以读取到项目资源文件中的值
                        Image menuArrow = (Image)Properties.Resources.MenuArrow;

                        if ((iconArrow == null))
                            DrawSubMenuArrow(g, menuArrow, bounds, selected, Enabled, Checked);
                        else
                            DrawSubMenuArrow(g, iconArrow, bounds, selected, Enabled, Checked);
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(String.Format("hassubmenu got error:{0}", ex.Message));
                    }
                }

                if (Text == "-")
                    DrawSeparator(g, bounds);
                else
                {
                    if (ShortcutKeys.Equals(Shortcut.None)) {
                         shortcuttext = "";
                    }
                    else
                    {
                        string text = "";
                        int key = System.Convert.ToInt32(ShortcutKeys);
                        int ch = key & 0xFF;
                        if ((System.Convert.ToInt32(Keys.Control) & key) > 0)
                            text += "Ctrl+";
                        if ((System.Convert.ToInt32(Keys.Shift) & key) > 0)
                            text += "Shift+";
                        if ((System.Convert.ToInt32(Keys.Alt) & key) > 0)
                            text += "Alt+";
                        if ((ch >= System.Convert.ToInt32(Shortcut.F1)) & (ch <= System.Convert.ToInt32(Shortcut.F12)))
                            text += "F" + (ch - 111).ToString();
                        else if (ShortcutKeys.Equals(Shortcut.Del))
                            text += "Del";
                        else
                            text += System.Convert.ToChar(ch);
                        if ((DropDown.Items.Count > 0))
                        {
                        }
                        shortcuttext = text;
                    }
                    DrawMenuText(g, bounds, Text, shortcuttext, IsMouseHover, ena);
                }
            }
            catch
            {
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            IsMouseHover = true;
            EventArgs ex = new EventArgs();
            base.OnOwnerChanged(ex);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            IsMouseHover = false;
            EventArgs ex = new EventArgs();
            base.OnOwnerChanged(ex);
        }


        // 功能名称:       DrawCheckmark(画菜单选项)
        // 参数说明: 
        // bounds          菜单项前面表示选中的小方块
        // selected        boolean型,表示是否选中
        public void DrawCheckmark(Graphics g, Rectangle bounds, bool selected)
        {
            try
            {
                if (((ContextMenuStrip)base.Parent).ShowCheckMargin)
                {
                    ControlPaint.DrawMenuGlyph(g, new Rectangle(bounds.X + 26, bounds.Y + base.Padding.Top, 14, 14), MenuGlyph.Checkmark, Color.Black, Color.White);
                    if (!(base.Image == null))
                    {
                    }
                }
                else
                    ControlPaint.DrawMenuGlyph(g, new Rectangle(bounds.X + 4, bounds.Y + base.Padding.Top, 14, 14), MenuGlyph.Checkmark, Color.Black, Color.White);
            }
            catch (Exception ex)
            {
            }
        }

        // 功能名称:       DrawCheckmark(画菜单选项)
        // 参数说明: 
        // bounds          菜单项前面表示选中的小方块
        // selected        boolean型,表示是否选中
        public void DrawCheckmark(Graphics g, Rectangle bounds, bool selected, bool hasicon)
        {
            try
            {
                // 如果有图标，先画图标
                if (hasicon)
                    g.DrawImage(Image, bounds.Left + 26, bounds.Top + 4, 16, 16);
                // 画选中状态的图标
                //从项目资源文件中读取 Properties.Resources.key 同样可以读取到项目资源文件中的值
                Image menuCheck = (Image)Properties.Resources.MenuCheck;

                if ((this.MenuItemCheckIcon != null))
                    g.DrawImage(iconCheck, bounds.Left + 6, bounds.Top + 4, 16, 16);
                else
                    g.DrawImage(menuCheck, bounds.Left + 6, bounds.Top + 4, 16, 16);
            }
            catch (Exception ex)
            {
            }
        }

        // 功能名称:       DrawIcon(画菜单项图形)
        // 参数说明:
        // icon            菜单项前面的图形
        // enabled         菜单项是否被禁用
        // ischecked       是否被选中
        public void DrawIcon(Graphics g, Image icon, Rectangle bounds, bool selected, bool enabled, bool ischecked)
        {
            try
            {
                // If Not (MyBase.GetMainMenu Is MyBase.Parent) Then
                if (enabled)
                {
                    if (selected)
                        // ControlPaint.DrawImageDisabled(g, icon, bounds.Left + 2, bounds.Top + 3, Color.Black)
                        g.DrawImage(icon, bounds.Left + 6, bounds.Top + 4, 16, 16);
                    else
                        g.DrawImage(icon, bounds.Left + 6, bounds.Top + 4, 16, 16);
                }
                else
                    ControlPaint.DrawImageDisabled(g, icon, bounds.Left + 6, bounds.Top + 4, SystemColors.HighlightText);
            }
            // End If
            catch
            {
            }
        }

        // 功能名称:       DrawSubMenuArrow(画菜单子菜单的三角图形)
        // 参数说明:
        // icon            菜单项前面的图形
        // enabled         菜单项是否被禁用
        // ischecked       是否被选中
        public void DrawSubMenuArrow(Graphics g, Image icon, Rectangle bounds, bool selected, bool enabled, bool ischecked)
        {
            try
            {
                // If Not (MyBase.GetMainMenu Is MyBase.Parent) Then
                if (enabled)
                {
                    if (selected)
                        // ControlPaint.DrawImageDisabled(g, icon, bounds.Left + 2, bounds.Top + 3, Color.Black)
                        g.DrawImage(icon, bounds.Right - icon.Width * 4, bounds.Top + icon.Height - 2, icon.Width, icon.Height);
                    else
                        g.DrawImage(icon, bounds.Right - icon.Width * 4, bounds.Top + icon.Height - 2, icon.Width, icon.Height);
                }
                else
                    ControlPaint.DrawImageDisabled(g, icon, bounds.Right - icon.Width * 4, bounds.Top + icon.Height - 2, SystemColors.HighlightText);
            }
            // End If
            catch
            {
            }
        }


        // 功能名称:       DrawSeparator(画分隔线)
        public void DrawSeparator(Graphics G, Rectangle Bounds)
        {
            int y = Bounds.Y + Bounds.Height / 2;
            // Dim ycolor As Color = Color.FromArgb(242, 242, 242)
            G.DrawLine(new Pen(sepColor), Bounds.X + 2, y, Bounds.X + Bounds.Width - 2, y);
        }

        // 功能名称:       DrawBackground(画菜单背景)
        // 参数说明:
        // State           菜单项状态
        // TopLevel        是否为菜单标题,即顶层菜单
        // HasIcon         菜单项是否有图形
        public void DrawBackground(Graphics G, Rectangle Bounds, bool HasIcon, bool hover, bool enabled)
        {
            // 鼠标移入时的背景
            bool selected = IsMouseHover;
            // 如果当前项鼠标悬停
            if ((hover))
            {
                if (enabled)
                {
                    // draw menuitem,selected  or toplevel,hotlighted
                    G.FillRectangle(new SolidBrush(sbcolor), Bounds);
                    G.DrawRectangle(new Pen(sbbcolor), Bounds.X, Bounds.Y, Bounds.Width - 1, Bounds.Height - 1);
                }
                else
                {
                    G.FillRectangle(new SolidBrush(ibgcolor), Bounds);
                    Bounds.X += SystemInformation.SmallIconSize.Width + 5;
                    Bounds.Width -= SystemInformation.SmallIconSize.Width + 5;
                    G.FillRectangle(new SolidBrush(bgcolor), Bounds);
                }
            }
            else
                try
                {
                    // draw menuitem,unselected
                    G.FillRectangle(new SolidBrush(ibgcolor), Bounds);
                    Bounds.X += SystemInformation.SmallIconSize.Width + 5;
                    Bounds.Width -= SystemInformation.SmallIconSize.Width + 5;
                    G.FillRectangle(new SolidBrush(bgcolor), Bounds);
                }
                catch
                {
                    G.FillRectangle(SystemBrushes.Menu, Bounds);
                }
        }

        // 功能名称:       DrawMenuText(画菜单项文字)
        // 参数说明:
        // text            菜单项文字
        // shortcut        菜单项快捷方式
        public void DrawMenuText(Graphics g, Rectangle bounds, string text, string shortcut, bool hover, bool enabled)
        {
            StringFormat strFormat = new StringFormat();
            if (DrawItemState.NoAccelerator > 0)
            {
                strFormat.HotkeyPrefix = HotkeyPrefix.Hide;
            }
            else
            {
                strFormat.HotkeyPrefix = HotkeyPrefix.Show;
            }

            int textwidth = System.Convert.ToInt32(g.MeasureString(text, SystemInformation.MenuFont).Width);
            int x;
            int y = bounds.Top + (bounds.Height - SystemInformation.MenuFont.Height) / 2;

            x = bounds.Left + TEXTSTART;
            try
            {
                if (((ContextMenuStrip)base.Parent).ShowCheckMargin)
                    x = x + 24;
            }
            catch (Exception ex)
            {
            }
            Brush brush = null/* TODO Change to default(_) if this is not a reference type */;
            if (!enabled)
                brush = new SolidBrush(Color.FromArgb(120, SystemColors.MenuText));
            else if ((hover))
                brush = new SolidBrush(Color.White);
            else
                brush = new SolidBrush(Color.Black);
            // 画菜单项文字
            g.DrawString(text, SystemInformation.MenuFont, brush, x, y, strFormat);
            strFormat.Alignment = StringAlignment.Far;     // 设置对齐方式为右对齐
                                                           // 画菜单项快捷方式
            g.DrawString(shortcut, SystemInformation.MenuFont, brush, bounds.Right - 30, y, strFormat);
            // 释放资源
            strFormat.Dispose();
            brush.Dispose();
        }
    }

}
