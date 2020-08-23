using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BrowserX.UC
{
    public class myToolStripRenderer : ToolStripRenderer
    {
        private static readonly int OffsetMargin = 24;

        private string _menuLogoString = "BrowserX";
        private ToolStripColorTable _colorTable;

        public myToolStripRenderer()
            : base()
        {
        }

        public myToolStripRenderer(
            ToolStripColorTable colorTable) : base()
        {
            _colorTable = colorTable;
        }

        public string MenuLogoString
        {
            get { return _menuLogoString; }
            set { _menuLogoString = value; }
        }

        protected virtual ToolStripColorTable ColorTable
        {
            get
            {
                if (_colorTable == null)
                {
                    _colorTable = new ToolStripColorTable();
                }
                return _colorTable;
            }
        }

        protected override void OnRenderToolStripBackground(
            ToolStripRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;
            Rectangle bounds = e.AffectedBounds;

            if (toolStrip is ToolStripDropDown)
            {
                RegionHelper.CreateRegion(toolStrip, bounds);
                using (SolidBrush brush = new SolidBrush(ColorTable.BackNormal))
                {
                    g.FillRectangle(brush, bounds);
                }
            }
            else if (toolStrip is MenuStrip)
            {
                LinearGradientMode mode =
                    toolStrip.Orientation == Orientation.Horizontal ?
                    LinearGradientMode.Vertical : LinearGradientMode.Horizontal;
                RenderHelper.RenderBackgroundInternal(
                    g,
                    bounds,
                    ColorTable.Base,
                    ColorTable.Border,
                    ColorTable.BackNormal,
                    RoundStyle.None,
                    0,
                    .95f,
                    false,
                    false,
                    mode);
            }
            else
            {
                LinearGradientMode mode =
                    toolStrip.Orientation == Orientation.Horizontal ?
                    LinearGradientMode.Vertical : LinearGradientMode.Horizontal;
                RenderHelper.RenderBackgroundInternal(
                    g,
                    bounds,
                    ColorTable.Base,
                    ColorTable.Border,
                    ColorTable.BackNormal,
                    RoundStyle.None,
                    0,
                    .95f,
                    true,
                    true,
                    mode);
            }
        }

        protected override void OnRenderImageMargin(
            ToolStripRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;
            Rectangle bounds = e.AffectedBounds;
            if (toolStrip is ToolStripDropDown)
            {
                bool bDrawLogo = NeedDrawLogo(toolStrip);
                bDrawLogo = false;
                bool bRightToLeft = toolStrip.RightToLeft == RightToLeft.Yes;

                Rectangle imageBackRect = bounds;
                imageBackRect.Width = OffsetMargin;

                if (bDrawLogo)
                {
                    Rectangle logoRect = bounds;
                    logoRect.Width = OffsetMargin;
                    if (bRightToLeft)
                    {
                        logoRect.X -= 2;
                        imageBackRect.X = logoRect.X - OffsetMargin;
                    }
                    else
                    {
                        logoRect.X += 2;
                        imageBackRect.X = logoRect.Right;
                    }
                    logoRect.Y += 1;
                    logoRect.Height -= 2;

                    using (LinearGradientBrush brush = new LinearGradientBrush(
                        logoRect,
                        ColorTable.BackHover,
                        ColorTable.BackNormal,
                        90f))
                    {
                        Blend blend = new Blend();
                        blend.Positions = new float[] { 0f, .2f, 1f };
                        blend.Factors = new float[] { 0f, 0.1f, .9f };
                        brush.Blend = blend;
                        logoRect.Y += 1;
                        logoRect.Height -= 2;
                        using (GraphicsPath path =
                            GraphicsPathHelper.CreatePath(logoRect, 8, RoundStyle.All, false))
                        {
                            using (SmoothingModeGraphics sg = new SmoothingModeGraphics(g))
                            {
                                g.FillPath(brush, path);
                            }
                        }
                    }

                    StringFormat sf = new StringFormat(StringFormatFlags.NoWrap);
                    Font font = new Font(
                        toolStrip.Font.FontFamily, 11, FontStyle.Bold);
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Trimming = StringTrimming.EllipsisCharacter;

                    g.TranslateTransform(logoRect.X, logoRect.Bottom);
                    g.RotateTransform(270f);

                    if (!string.IsNullOrEmpty(MenuLogoString))
                    {
                        Rectangle newRect = new Rectangle(
                            0, 0, logoRect.Height, logoRect.Width);

                        using (Brush brush = new SolidBrush(ColorTable.Fore))
                        {
                            using (TextRenderingHintGraphics tg =
                                new TextRenderingHintGraphics(g))
                            {
                                g.DrawString(
                                    MenuLogoString,
                                    font,
                                    brush,
                                    newRect,
                                    sf);
                            }
                        }
                    }

                    g.ResetTransform();
                }
                else
                {
                    if (bRightToLeft)
                    {
                        imageBackRect.X -= 3;
                    }
                    else
                    {
                        imageBackRect.X += 3;
                    }
                }

                imageBackRect.Y += 2;
                imageBackRect.Height -= 4;
                using (SolidBrush brush = new SolidBrush(ColorTable.DropDownImageBack))
                {
                    g.FillRectangle(brush, imageBackRect);
                }

                Point ponitStart;
                Point pointEnd;
                if (bRightToLeft)
                {
                    ponitStart = new Point(imageBackRect.X, imageBackRect.Y);
                    pointEnd = new Point(imageBackRect.X, imageBackRect.Bottom);
                }
                else
                {
                    ponitStart = new Point(imageBackRect.Right - 1, imageBackRect.Y);
                    pointEnd = new Point(imageBackRect.Right - 1, imageBackRect.Bottom);
                }

                using (Pen pen = new Pen(ColorTable.DropDownVerticalSeparator))
                {
                    g.DrawLine(pen, ponitStart, pointEnd);
                }
            }
            else
            {
                base.OnRenderImageMargin(e);
            }
        }

        protected override void OnRenderToolStripBorder(
            ToolStripRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;
            Rectangle bounds = e.AffectedBounds;

            if (toolStrip is ToolStripDropDown)
            {
                using (SmoothingModeGraphics sg = new SmoothingModeGraphics(g))
                {
                    using (GraphicsPath path =
                        GraphicsPathHelper.CreatePath(bounds, 8, RoundStyle.All, true))
                    {
                        using (Pen pen = new Pen(ColorTable.DropDownImageSeparator))
                        {
                            path.Widen(pen);
                            g.DrawPath(pen, path);
                        }
                    }
                }

                if (!(toolStrip is ToolStripOverflow))
                {
                    bounds.Inflate(-1, -1);
                    using (GraphicsPath innerPath = GraphicsPathHelper.CreatePath(
                        bounds, 8, RoundStyle.All, true))
                    {
                        using (Pen pen = new Pen(ColorTable.BackNormal))
                        {
                            g.DrawPath(pen, innerPath);
                        }
                    }
                }
            }
            else
            {
                base.OnRenderToolStripBorder(e);
            }
        }

        protected override void OnRenderMenuItemBackground(
           ToolStripItemRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            ToolStripItem item = e.Item;

            if (!item.Enabled)
            {
                return;
            }

            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);

            if (toolStrip is MenuStrip)
            {
                LinearGradientMode mode =
                    toolStrip.Orientation == Orientation.Horizontal ?
                    LinearGradientMode.Vertical : LinearGradientMode.Horizontal;
                if (item.Selected)
                {
                    RenderHelper.RenderBackgroundInternal(
                        g,
                        rect,
                        ColorTable.BackHover,
                        ColorTable.Border,
                        ColorTable.MenuItemBackNormal,
                        RoundStyle.All,
                        false,
                        false,
                        mode);
                }
                else if (item.Pressed)
                {
                    RenderHelper.RenderBackgroundInternal(
                       g,
                       rect,
                       ColorTable.BackPressed,
                       ColorTable.Border,
                       ColorTable.MenuItemBackNormal,
                       RoundStyle.All,
                       false,
                       false,
                       mode);
                }
                else
                {
                    base.OnRenderMenuItemBackground(e);
                }
            }
            else if (toolStrip is ToolStripDropDown)
            {
                bool bDrawLogo = NeedDrawLogo(toolStrip);

                int offsetMargin = bDrawLogo ? OffsetMargin : 0;

                if (item.RightToLeft == RightToLeft.Yes)
                {
                    rect.X += 4;
                }
                else
                {
                    rect.X += offsetMargin + 4;
                }
                rect.Width -= offsetMargin + 8;
                rect.Height--;

                if (item.Selected)
                {
                    RenderHelper.RenderBackgroundInternal(
                       g,
                       rect,
                       ColorTable.BackHover,
                       ColorTable.Border,
                       ColorTable.MenuItemBackNormal,
                       RoundStyle.All,
                       true,
                       true,
                       LinearGradientMode.Vertical);
                }
                else
                {
                    base.OnRenderMenuItemBackground(e);
                }
            }
            else
            {
                base.OnRenderMenuItemBackground(e);
            }
        }

        protected override void OnRenderItemImage(
            ToolStripItemImageRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;

            if (toolStrip is ToolStripDropDown &&
               e.Item is ToolStripMenuItem)
            {
                bool bDrawLogo = NeedDrawLogo(toolStrip);
                int offsetMargin = bDrawLogo ? OffsetMargin : 0;

                ToolStripMenuItem item = (ToolStripMenuItem)e.Item;
                if (item.Checked)
                {
                    return;
                }
                Rectangle rect = e.ImageRectangle;
                if (e.Item.RightToLeft == RightToLeft.Yes)
                {
                    rect.X -= offsetMargin + 2;
                }
                else
                {
                    rect.X += offsetMargin + 2;
                }

                using (InterpolationModeGraphics ig =
                    new InterpolationModeGraphics(g))
                {
                    ToolStripItemImageRenderEventArgs ne =
                        new ToolStripItemImageRenderEventArgs(
                        g, e.Item, e.Image, rect);
                    base.OnRenderItemImage(ne);
                }
            }
            else
            {
                base.OnRenderItemImage(e);
            }
        }

        protected override void OnRenderItemText(
            ToolStripItemTextRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;

            e.TextColor = ColorTable.Fore;

            if (toolStrip is ToolStripDropDown &&
                e.Item is ToolStripMenuItem)
            {
                bool bDrawLogo = NeedDrawLogo(toolStrip);

                int offsetMargin = bDrawLogo ? 18 : 0;

                Rectangle rect = e.TextRectangle;
                if (e.Item.RightToLeft == RightToLeft.Yes)
                {
                    rect.X -= offsetMargin;
                }
                else
                {
                    rect.X += offsetMargin;
                }
                e.TextRectangle = rect;
            }

            base.OnRenderItemText(e);
        }

        protected override void OnRenderItemCheck(
            ToolStripItemImageRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;

            if (toolStrip is ToolStripDropDown &&
               e.Item is ToolStripMenuItem)
            {
                bool bDrawLogo = NeedDrawLogo(toolStrip);
                int offsetMargin = bDrawLogo ? OffsetMargin : 0;
                Rectangle rect = e.ImageRectangle;

                if (e.Item.RightToLeft == RightToLeft.Yes)
                {
                    rect.X -= offsetMargin + 2;
                }
                else
                {
                    rect.X += offsetMargin + 2;
                }

                rect.Width = 13;
                rect.Y += 1;
                rect.Height -= 3;

                using (SmoothingModeGraphics sg = new SmoothingModeGraphics(g))
                {
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.AddRectangle(rect);
                        using (PathGradientBrush brush = new PathGradientBrush(path))
                        {
                            brush.CenterColor = ColorTable.DropDownImageBack;
                            brush.SurroundColors = new Color[] { ColorTable.BackPressed };
                            Blend blend = new Blend();
                            blend.Positions = new float[] { 0f, 0.3f, 1f };
                            blend.Factors = new float[] { 0f, 0.5f, 1f };
                            brush.Blend = blend;
                            g.FillRectangle(brush, rect);
                        }
                    }

                    using (Pen pen = new Pen(ColorTable.BackPressed))
                    {
                        g.DrawRectangle(pen, rect);
                    }

                    ControlPaintEx.DrawCheckedFlag(g, rect, ColorTable.Fore);
                }
            }
            else
            {
                base.OnRenderItemCheck(e);
            }
        }

        protected override void OnRenderArrow(
            ToolStripArrowRenderEventArgs e)
        {
            if (e.Item.Enabled)
            {
                e.ArrowColor = ColorTable.Fore;
            }

            ToolStrip toolStrip = e.Item.Owner;

            if (toolStrip is ToolStripDropDown &&
                e.Item is ToolStripMenuItem)
            {
                bool bDrawLogo = NeedDrawLogo(toolStrip);

                int offsetMargin = bDrawLogo ? 3 : 0;

                Rectangle rect = e.ArrowRectangle;
                if (e.Item.RightToLeft == RightToLeft.Yes)
                {
                    rect.X -= offsetMargin;
                }
                else
                {
                    rect.X += offsetMargin;
                }

                e.ArrowRectangle = rect;
            }

            base.OnRenderArrow(e);
        }

        protected override void OnRenderSeparator(
            ToolStripSeparatorRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            Rectangle rect = e.Item.ContentRectangle;
            Graphics g = e.Graphics;

            Color baseColor = ColorTable.Base;

            if (toolStrip is ToolStripDropDown)
            {
                bool bDrawLogo = NeedDrawLogo(toolStrip);

                int offsetMargin = bDrawLogo ?
                    OffsetMargin * 2 : OffsetMargin;

                if (e.Item.RightToLeft != RightToLeft.Yes)
                {
                    rect.X += offsetMargin + 2;
                }
                rect.Width -= offsetMargin + 4;

                baseColor = ColorTable.DropDownImageSeparator;
            }

            RenderSeparatorLine(
               g,
               rect,
               baseColor,
               ColorTable.BackNormal,
               Color.Snow,
               e.Vertical);
        }

        protected override void OnRenderButtonBackground(
            ToolStripItemRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            ToolStripButton item = e.Item as ToolStripButton;
            Graphics g = e.Graphics;

            if (item != null)
            {
                LinearGradientMode mode =
                    toolStrip.Orientation == Orientation.Horizontal ?
                    LinearGradientMode.Vertical : LinearGradientMode.Horizontal;
                SmoothingModeGraphics sg = new SmoothingModeGraphics(g);
                Rectangle bounds = new Rectangle(Point.Empty, item.Size);

                if (item.BackgroundImage != null)
                {
                    Rectangle clipRect = item.Selected ? item.ContentRectangle : bounds;
                    ControlPaintEx.DrawBackgroundImage(
                        g,
                        item.BackgroundImage,
                        ColorTable.BackNormal,
                        item.BackgroundImageLayout,
                        bounds,
                        clipRect);
                }

                if (item.CheckState == CheckState.Unchecked)
                {
                    if (item.Selected)
                    {
                        Color color = ColorTable.BackHover;
                        if (item.Pressed)
                        {
                            color = ColorTable.BackPressed;
                        }
                        RenderHelper.RenderBackgroundInternal(
                            g,
                            bounds,
                            color,
                            ColorTable.Border,
                            ColorTable.BackNormal,
                            RoundStyle.All,
                            true,
                            true,
                            mode);
                    }
                    else
                    {
                        if (toolStrip is ToolStripOverflow)
                        {
                            using (Brush brush = new SolidBrush(ColorTable.BackHover))
                            {
                                g.FillRectangle(brush, bounds);
                            }
                        }
                    }
                }
                else
                {
                    Color color = ControlPaint.Light(ColorTable.BackHover);
                    if (item.Selected)
                    {
                        color = ColorTable.BackHover;
                    }
                    if (item.Pressed)
                    {
                        color = ColorTable.BackPressed;
                    }
                    RenderHelper.RenderBackgroundInternal(
                        g,
                        bounds,
                        color,
                        ColorTable.Border,
                        ColorTable.BackNormal,
                        RoundStyle.All,
                        true,
                        true,
                        mode);
                }

                sg.Dispose();
            }
        }

        protected override void OnRenderDropDownButtonBackground(
            ToolStripItemRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            ToolStripDropDownItem item = e.Item as ToolStripDropDownItem;

            if (item != null)
            {
                LinearGradientMode mode =
                   toolStrip.Orientation == Orientation.Horizontal ?
                   LinearGradientMode.Vertical : LinearGradientMode.Horizontal;
                Graphics g = e.Graphics;
                SmoothingModeGraphics sg = new SmoothingModeGraphics(g);
                Rectangle bounds = new Rectangle(Point.Empty, item.Size);

                if (item.Pressed && item.HasDropDownItems)
                {
                    RenderHelper.RenderBackgroundInternal(
                      g,
                      bounds,
                      ColorTable.BackPressed,
                      ColorTable.Border,
                      ColorTable.BackNormal,
                      RoundStyle.All,
                      true,
                      true,
                      mode);
                }
                else if (item.Selected)
                {
                    RenderHelper.RenderBackgroundInternal(
                      g,
                      bounds,
                      ColorTable.BackHover,
                      ColorTable.Border,
                      ColorTable.BackNormal,
                      RoundStyle.All,
                      true,
                      true,
                      mode);
                }
                else if (toolStrip is ToolStripOverflow)
                {
                    using (Brush brush = new SolidBrush(ColorTable.BackNormal))
                    {
                        g.FillRectangle(brush, bounds);
                    }
                }
                else
                {
                    base.OnRenderDropDownButtonBackground(e);
                }

                sg.Dispose();
            }
        }

        protected override void OnRenderSplitButtonBackground(
            ToolStripItemRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            ToolStripSplitButton item = e.Item as ToolStripSplitButton;

            if (item != null)
            {
                Graphics g = e.Graphics;
                LinearGradientMode mode =
                    toolStrip.Orientation == Orientation.Horizontal ?
                    LinearGradientMode.Vertical : LinearGradientMode.Horizontal;
                Rectangle bounds = new Rectangle(Point.Empty, item.Size);
                SmoothingModeGraphics sg = new SmoothingModeGraphics(g);

                Color arrowColor = toolStrip.Enabled ?
                    ColorTable.Fore : SystemColors.ControlDark;

                if (item.BackgroundImage != null)
                {
                    Rectangle clipRect = item.Selected ? item.ContentRectangle : bounds;
                    ControlPaintEx.DrawBackgroundImage(
                        g,
                        item.BackgroundImage,
                        ColorTable.BackNormal,
                        item.BackgroundImageLayout,
                        bounds,
                        clipRect);
                }

                if (item.ButtonPressed)
                {
                    Rectangle buttonBounds = item.ButtonBounds;
                    Padding padding = (item.RightToLeft == RightToLeft.Yes) ?
                        new Padding(0, 1, 1, 1) : new Padding(1, 1, 0, 1);
                    buttonBounds = LayoutUtils.DeflateRect(buttonBounds, padding);
                    RenderHelper.RenderBackgroundInternal(
                       g,
                       bounds,
                       ColorTable.BackHover,
                       ColorTable.Border,
                       ColorTable.BackNormal,
                       RoundStyle.All,
                       true,
                       true,
                       mode);

                    buttonBounds.Inflate(-1, -1);
                    g.SetClip(buttonBounds);
                    RenderHelper.RenderBackgroundInternal(
                       g,
                       buttonBounds,
                       ColorTable.BackPressed,
                       ColorTable.Border,
                       ColorTable.BackNormal,
                       RoundStyle.Left,
                       false,
                       true,
                       mode);
                    g.ResetClip();

                    using (Pen pen = new Pen(ColorTable.Border))
                    {
                        g.DrawLine(
                            pen,
                            item.SplitterBounds.Left,
                            item.SplitterBounds.Top,
                            item.SplitterBounds.Left,
                            item.SplitterBounds.Bottom);
                    }
                    base.DrawArrow(
                        new ToolStripArrowRenderEventArgs(
                        g,
                        item,
                        item.DropDownButtonBounds,
                        arrowColor,
                        ArrowDirection.Down));
                    return;
                }

                if (item.Pressed || item.DropDownButtonPressed)
                {
                    RenderHelper.RenderBackgroundInternal(
                      g,
                      bounds,
                      ColorTable.BackPressed,
                      ColorTable.Border,
                      ColorTable.BackNormal,
                      RoundStyle.All,
                      true,
                      true,
                      mode);
                    base.DrawArrow(
                       new ToolStripArrowRenderEventArgs(
                       g,
                       item,
                       item.DropDownButtonBounds,
                       arrowColor,
                       ArrowDirection.Down));
                    return;
                }

                if (item.Selected)
                {
                    RenderHelper.RenderBackgroundInternal(
                      g,
                      bounds,
                      ColorTable.BackHover,
                      ColorTable.Border,
                      ColorTable.BackNormal,
                      RoundStyle.All,
                      true,
                      true,
                      mode);
                    using (Pen pen = new Pen(ColorTable.Border))
                    {
                        g.DrawLine(
                           pen,
                           item.SplitterBounds.Left,
                           item.SplitterBounds.Top,
                           item.SplitterBounds.Left,
                           item.SplitterBounds.Bottom);
                    }
                    base.DrawArrow(
                        new ToolStripArrowRenderEventArgs(
                        g,
                        item,
                        item.DropDownButtonBounds,
                        arrowColor,
                        ArrowDirection.Down));
                    return;
                }

                base.DrawArrow(
                   new ToolStripArrowRenderEventArgs(
                   g,
                   item,
                   item.DropDownButtonBounds,
                   arrowColor,
                   ArrowDirection.Down));
                return;
            }

            base.OnRenderSplitButtonBackground(e);
        }

        protected override void OnRenderOverflowButtonBackground(
            ToolStripItemRenderEventArgs e)
        {
            ToolStripItem item = e.Item;
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;
            bool rightToLeft = item.RightToLeft == RightToLeft.Yes;

            SmoothingModeGraphics sg = new SmoothingModeGraphics(g);

            RenderOverflowBackground(e, rightToLeft);

            bool bHorizontal = toolStrip.Orientation == Orientation.Horizontal;
            Rectangle empty = Rectangle.Empty;

            if (rightToLeft)
            {
                empty = new Rectangle(0, item.Height - 8, 10, 5);
            }
            else
            {
                empty = new Rectangle(item.Width - 12, item.Height - 8, 10, 5);
            }
            ArrowDirection direction = bHorizontal ?
                ArrowDirection.Down : ArrowDirection.Right;
            int x = (rightToLeft && bHorizontal) ? -1 : 1;
            empty.Offset(x, 1);

            Color arrowColor = toolStrip.Enabled ?
                ColorTable.Fore : SystemColors.ControlDark;

            using (Brush brush = new SolidBrush(arrowColor))
            {
                RenderHelper.RenderArrowInternal(g, empty, direction, brush);
            }

            if (bHorizontal)
            {
                using (Pen pen = new Pen(arrowColor))
                {
                    g.DrawLine(
                        pen,
                        empty.Right - 8,
                        empty.Y - 2,
                        empty.Right - 2,
                        empty.Y - 2);
                    g.DrawLine(
                        pen,
                        empty.Right - 8,
                        empty.Y - 1,
                        empty.Right - 2,
                        empty.Y - 1);
                }
            }
            else
            {
                using (Pen pen = new Pen(arrowColor))
                {
                    g.DrawLine(
                        pen,
                        empty.X,
                        empty.Y,
                        empty.X,
                        empty.Bottom - 1);
                    g.DrawLine(
                        pen,
                        empty.X,
                        empty.Y + 1,
                        empty.X,
                        empty.Bottom);
                }
            }
        }

        protected override void OnRenderGrip(
            ToolStripGripRenderEventArgs e)
        {
            if (e.GripStyle == ToolStripGripStyle.Visible)
            {
                Rectangle bounds = e.GripBounds;
                bool vert = e.GripDisplayStyle == ToolStripGripDisplayStyle.Vertical;
                ToolStrip toolStrip = e.ToolStrip;
                Graphics g = e.Graphics;

                if (vert)
                {
                    bounds.X = e.AffectedBounds.X;
                    bounds.Width = e.AffectedBounds.Width;
                    if (toolStrip is MenuStrip)
                    {
                        if (e.AffectedBounds.Height > e.AffectedBounds.Width)
                        {
                            vert = false;
                            bounds.Y = e.AffectedBounds.Y;
                        }
                        else
                        {
                            toolStrip.GripMargin = new Padding(0, 2, 0, 2);
                            bounds.Y = e.AffectedBounds.Y;
                            bounds.Height = e.AffectedBounds.Height;
                        }
                    }
                    else
                    {
                        toolStrip.GripMargin = new Padding(2, 2, 4, 2);
                        bounds.X++;
                        bounds.Width++;
                    }
                }
                else
                {
                    bounds.Y = e.AffectedBounds.Y;
                    bounds.Height = e.AffectedBounds.Height;
                }

                DrawDottedGrip(
                    g,
                    bounds,
                    vert,
                    false,
                    ColorTable.BackNormal,
                    ControlPaint.Dark(ColorTable.Base, 0.3F));
            }
        }

        protected override void OnRenderStatusStripSizingGrip(
            ToolStripRenderEventArgs e)
        {
            DrawSolidStatusGrip(
                e.Graphics,
                e.AffectedBounds,
                ColorTable.BackNormal,
                ControlPaint.Dark(ColorTable.Base, 0.3f));
        }

        internal void RenderSeparatorLine(
            Graphics g,
            Rectangle rect,
            Color baseColor,
            Color backColor,
            Color shadowColor,
            bool vertical)
        {
            if (vertical)
            {
                rect.Y += 2;
                rect.Height -= 4;
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    rect,
                    baseColor,
                    backColor,
                    LinearGradientMode.Vertical))
                {
                    using (Pen pen = new Pen(brush))
                    {
                        g.DrawLine(pen, rect.X, rect.Y, rect.X, rect.Bottom);
                    }
                }
            }
            else
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    rect,
                    baseColor,
                    backColor,
                    180F))
                {
                    Blend blend = new Blend();
                    blend.Positions = new float[] { 0f, .2f, .5f, .8f, 1f };
                    blend.Factors = new float[] { 1f, .3f, 0f, .3f, 1f };
                    brush.Blend = blend;
                    using (Pen pen = new Pen(brush))
                    {
                        g.DrawLine(pen, rect.X, rect.Y, rect.Right, rect.Y);

                        brush.LinearColors = new Color[] {
                        shadowColor, backColor };
                        pen.Brush = brush;
                        g.DrawLine(pen, rect.X, rect.Y + 1, rect.Right, rect.Y + 1);
                    }
                }
            }
        }

        internal void RenderOverflowBackground(
            ToolStripItemRenderEventArgs e,
            bool rightToLeft)
        {
            Color color = Color.Empty;
            Graphics g = e.Graphics;
            ToolStrip toolStrip = e.ToolStrip;
            ToolStripOverflowButton item = e.Item as ToolStripOverflowButton;
            Rectangle bounds = new Rectangle(Point.Empty, item.Size);
            Rectangle withinBounds = bounds;
            bool bParentIsMenuStrip = !(item.GetCurrentParent() is MenuStrip);
            bool bHorizontal = toolStrip.Orientation == Orientation.Horizontal;

            if (bHorizontal)
            {
                bounds.X += (bounds.Width - 12) + 1;
                bounds.Width = 12;
                if (rightToLeft)
                {
                    bounds = LayoutUtils.RTLTranslate(bounds, withinBounds);
                }
            }
            else
            {
                bounds.Y = (bounds.Height - 12) + 1;
                bounds.Height = 12;
            }

            if (item.Pressed)
            {
                color = ColorTable.BackPressed;
            }
            else if (item.Selected)
            {
                color = ColorTable.BackHover;
            }
            else
            {
                color = ColorTable.Base;
            }
            if (bParentIsMenuStrip)
            {
                using (Pen pen = new Pen(ColorTable.Base))
                {
                    Point point = new Point(bounds.Left - 1, bounds.Height - 2);
                    Point point2 = new Point(bounds.Left, bounds.Height - 2);
                    if (rightToLeft)
                    {
                        point.X = bounds.Right + 1;
                        point2.X = bounds.Right;
                    }
                    g.DrawLine(pen, point, point2);
                }
            }

            LinearGradientMode mode = bHorizontal ?
                LinearGradientMode.Vertical : LinearGradientMode.Horizontal;

            RenderHelper.RenderBackgroundInternal(
                g,
                bounds,
                color,
                ColorTable.Border,
                ColorTable.BackNormal,
                RoundStyle.None,
                0,
                .35f,
                false,
                false,
                mode);

            if (bParentIsMenuStrip)
            {
                using (Brush brush = new SolidBrush(ColorTable.Base))
                {
                    if (bHorizontal)
                    {
                        Point point3 = new Point(bounds.X - 2, 0);
                        Point point4 = new Point(bounds.X - 1, 1);
                        if (rightToLeft)
                        {
                            point3.X = bounds.Right + 1;
                            point4.X = bounds.Right;
                        }
                        g.FillRectangle(brush, point3.X, point3.Y, 1, 1);
                        g.FillRectangle(brush, point4.X, point4.Y, 1, 1);
                    }
                    else
                    {
                        g.FillRectangle(brush, bounds.Width - 3, bounds.Top - 1, 1, 1);
                        g.FillRectangle(brush, bounds.Width - 2, bounds.Top - 2, 1, 1);
                    }
                }
                using (Brush brush = new SolidBrush(ColorTable.Base))
                {
                    if (bHorizontal)
                    {
                        Rectangle rect = new Rectangle(bounds.X - 1, 0, 1, 1);
                        if (rightToLeft)
                        {
                            rect.X = bounds.Right;
                        }
                        g.FillRectangle(brush, rect);
                    }
                    else
                    {
                        g.FillRectangle(brush, bounds.X, bounds.Top - 1, 1, 1);
                    }
                }
            }
        }

        private void DrawDottedGrip(
            Graphics g,
            Rectangle bounds,
            bool vertical,
            bool largeDot,
            Color innerColor,
            Color outerColor)
        {
            bounds.Height -= 3;
            Point position = new Point(bounds.X, bounds.Y);
            int sep;
            Rectangle posRect = new Rectangle(0, 0, 2, 2);

            using (SmoothingModeGraphics sg = new SmoothingModeGraphics(g))
            {
                IntPtr hdc;

                if (vertical)
                {
                    sep = bounds.Height;
                    position.Y += 8;
                    for (int i = 0; position.Y > 4; i += 4)
                    {
                        position.Y = sep - (2 + i);
                        if (largeDot)
                        {
                            posRect.Location = position;
                            DrawCircle(
                                g,
                                posRect,
                                outerColor,
                                innerColor);
                        }
                        else
                        {
                            int innerWin32Corlor = ColorTranslator.ToWin32(innerColor);
                            int outerWin32Corlor = ColorTranslator.ToWin32(outerColor);

                            hdc = g.GetHdc();

                            SetPixel(
                                hdc,
                                position.X,
                                position.Y,
                                innerWin32Corlor);
                            SetPixel(
                                hdc,
                                position.X + 1,
                                position.Y,
                                outerWin32Corlor);
                            SetPixel(
                                hdc,
                                position.X,
                                position.Y + 1,
                                outerWin32Corlor);

                            SetPixel(
                                hdc,
                                position.X + 3,
                                position.Y,
                                innerWin32Corlor);
                            SetPixel(
                                hdc,
                                position.X + 4,
                                position.Y,
                                outerWin32Corlor);
                            SetPixel(
                                hdc,
                                position.X + 3,
                                position.Y + 1,
                                outerWin32Corlor);

                            g.ReleaseHdc(hdc);
                        }
                    }
                }
                else
                {
                    bounds.Inflate(-2, 0);
                    sep = bounds.Width;
                    position.X += 2;

                    for (int i = 1; position.X > 0; i += 4)
                    {
                        position.X = sep - (2 + i);
                        if (largeDot)
                        {
                            posRect.Location = position;
                            DrawCircle(g, posRect, outerColor, innerColor);
                        }
                        else
                        {
                            int innerWin32Corlor = ColorTranslator.ToWin32(innerColor);
                            int outerWin32Corlor = ColorTranslator.ToWin32(outerColor);
                            hdc = g.GetHdc();

                            SetPixel(
                                hdc,
                                position.X,
                                position.Y,
                                innerWin32Corlor);
                            SetPixel(
                                hdc,
                                position.X + 1,
                                position.Y,
                                outerWin32Corlor);
                            SetPixel(
                                hdc,
                                position.X,
                                position.Y + 1,
                                outerWin32Corlor);

                            SetPixel(
                                hdc,
                                position.X + 3,
                                position.Y,
                                innerWin32Corlor);
                            SetPixel(
                                hdc,
                                position.X + 4,
                                position.Y,
                                outerWin32Corlor);
                            SetPixel(
                                hdc,
                                position.X + 3,
                                position.Y + 1,
                                outerWin32Corlor);

                            g.ReleaseHdc(hdc);
                        }
                    }
                }
            }
        }

        private void DrawCircle(
            Graphics g,
            Rectangle bounds,
            Color borderColor,
            Color fillColor)
        {
            using (GraphicsPath circlePath = new GraphicsPath())
            {
                circlePath.AddEllipse(bounds);
                circlePath.CloseFigure();

                using (Pen borderPen = new Pen(borderColor))
                {
                    g.DrawPath(borderPen, circlePath);
                }

                using (Brush backBrush = new SolidBrush(fillColor))
                {
                    g.FillPath(backBrush, circlePath);
                }
            }
        }

        private void DrawDottedStatusGrip(
            Graphics g,
            Rectangle bounds,
            Color innerColor,
            Color outerColor)
        {
            Rectangle shape = new Rectangle(0, 0, 2, 2);
            shape.X = bounds.Width - 17;
            shape.Y = bounds.Height - 8;
            using (SmoothingModeGraphics sg = new SmoothingModeGraphics(g))
            {
                DrawCircle(g, shape, outerColor, innerColor);

                shape.X = bounds.Width - 12;
                DrawCircle(g, shape, outerColor, innerColor);

                shape.X = bounds.Width - 7;
                DrawCircle(g, shape, outerColor, innerColor);

                shape.Y = bounds.Height - 13;
                DrawCircle(g, shape, outerColor, innerColor);

                shape.Y = bounds.Height - 18;
                DrawCircle(g, shape, outerColor, innerColor);

                shape.Y = bounds.Height - 13;
                shape.X = bounds.Width - 12;
                DrawCircle(g, shape, outerColor, innerColor);
            }
        }

        private void DrawSolidStatusGrip(
            Graphics g,
            Rectangle bounds,
            Color innerColor,
            Color outerColor
            )
        {
            using (SmoothingModeGraphics sg = new SmoothingModeGraphics(g))
            {
                using (Pen innerPen = new Pen(innerColor),
                    outerPen = new Pen(outerColor))
                {
                    //outer line 
                    g.DrawLine(
                        outerPen,
                        new Point(bounds.Width - 14, bounds.Height - 6),
                        new Point(bounds.Width - 4, bounds.Height - 16));
                    g.DrawLine(
                        innerPen,
                        new Point(bounds.Width - 13, bounds.Height - 6),
                        new Point(bounds.Width - 4, bounds.Height - 15));
                    // line 
                    g.DrawLine(
                        outerPen,
                        new Point(bounds.Width - 12, bounds.Height - 6),
                        new Point(bounds.Width - 4, bounds.Height - 14));
                    g.DrawLine(
                        innerPen,
                        new Point(bounds.Width - 11, bounds.Height - 6),
                        new Point(bounds.Width - 4, bounds.Height - 13));
                    // line 
                    g.DrawLine(
                        outerPen,
                        new Point(bounds.Width - 10, bounds.Height - 6),
                        new Point(bounds.Width - 4, bounds.Height - 12));
                    g.DrawLine(
                        innerPen,
                        new Point(bounds.Width - 9, bounds.Height - 6),
                        new Point(bounds.Width - 4, bounds.Height - 11));
                    // line 
                    g.DrawLine(
                        outerPen,
                        new Point(bounds.Width - 8, bounds.Height - 6),
                        new Point(bounds.Width - 4, bounds.Height - 10));
                    g.DrawLine(
                        innerPen,
                        new Point(bounds.Width - 7, bounds.Height - 6),
                        new Point(bounds.Width - 4, bounds.Height - 9));
                    // inner line 
                    g.DrawLine(
                        outerPen,
                        new Point(bounds.Width - 6, bounds.Height - 6),
                        new Point(bounds.Width - 4, bounds.Height - 8));
                    g.DrawLine(
                        innerPen,
                        new Point(bounds.Width - 5, bounds.Height - 6),
                        new Point(bounds.Width - 4, bounds.Height - 7));
                }
            }
        }

        internal bool NeedDrawLogo(ToolStrip toolStrip)
        {
            ToolStripDropDown dropDown = toolStrip as ToolStripDropDown;
            bool bDrawLogo =
                (dropDown.OwnerItem != null &&
                dropDown.OwnerItem.Owner is MenuStrip) ||
                (toolStrip is ContextMenuStrip);
            return bDrawLogo;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern uint SetPixel(IntPtr hdc, int X, int Y, int crColor);
    }

    public class ToolStripColorTable
    {
        private static readonly Color _base = Color.Transparent;
        private static readonly Color _border = Color.FromArgb(194, 169, 120);
        private static readonly Color _backNormal = Color.FromArgb(250, 250, 250);
        private static readonly Color _backMenuItemNormal = Color.FromArgb(255, 255, 255);
        private static readonly Color _backHover = Color.FromArgb(56, 53, 52);
        private static readonly Color _backPressed = Color.FromArgb(226, 176, 0);
        private static readonly Color _fore = Color.FromArgb(21, 66, 139);
        private static readonly Color _dropDownImageBack = Color.FromArgb(255, 255, 255);
        private static readonly Color _dropDownImageSeparator = Color.FromArgb(197, 197, 197);
        private static readonly Color _DropDownVerticalSeparator = Color.FromArgb(255, 255, 255);

        public ToolStripColorTable() { }

        public virtual Color Base
        {
            get { return _base; }
        }

        public virtual Color Border
        {
            get { return _border; }
        }

        public virtual Color BackNormal
        {
            get { return _backNormal; }
        }

        public virtual Color MenuItemBackNormal
        {
            get { return _backMenuItemNormal; }
        }


        public virtual Color BackHover
        {
            get { return _backHover; }
        }

        public virtual Color BackPressed
        {
            get { return _backPressed; }
        }

        public virtual Color Fore
        {
            get { return _fore; }
        }

        public virtual Color DropDownImageBack
        {
            get { return _dropDownImageBack; }
        }

        public virtual Color DropDownImageSeparator
        {
            get { return _dropDownImageSeparator; }
        }
        public virtual Color DropDownVerticalSeparator
        {
            get { return _DropDownVerticalSeparator; }
        }
    }

    /// <summary> 
    /// 建立圆角路径的样式。 
    /// </summary> 
    public enum RoundStyle
    {
        /// <summary> 
        /// 四个角都不是圆角。 
        /// </summary> 
        None = 0,
        /// <summary> 
        /// 四个角都为圆角。 
        /// </summary> 
        All = 1,
        /// <summary> 
        /// 左边两个角为圆角。 
        /// </summary> 
        Left = 2,
        /// <summary> 
        /// 右边两个角为圆角。 
        /// </summary> 
        Right = 3,
        /// <summary> 
        /// 上边两个角为圆角。 
        /// </summary> 
        Top = 4,
        /// <summary> 
        /// 下边两个角为圆角。 
        /// </summary> 
        Bottom = 5,
        /// <summary> 
        /// 左下角为圆角。 
        /// </summary> 
        BottomLeft = 6,
        /// <summary> 
        /// 右下角为圆角。 
        /// </summary> 
        BottomRight = 7,
    }

    public static class RegionHelper
    {
        public static void CreateRegion(
            Control control,
            Rectangle bounds,
            int radius,
            RoundStyle roundStyle)
        {
            using (GraphicsPath path =
                GraphicsPathHelper.CreatePath(
                bounds, radius, roundStyle, true))
            {
                Region region = new Region(path);
                path.Widen(Pens.White);
                region.Union(path);
                if (control.Region != null)
                {
                    control.Region.Dispose();
                }
                control.Region = region;
            }
        }

        public static void CreateRegion(
            Control control,
            Rectangle bounds)
        {
            CreateRegion(control, bounds, 8, RoundStyle.All);
        }
    }

    internal class RenderHelper
    {
        internal static void RenderBackgroundInternal(
            Graphics g,
            Rectangle rect,
            Color baseColor,
            Color borderColor,
            Color innerBorderColor,
            RoundStyle style,
            bool drawBorder,
            bool drawGlass,
            LinearGradientMode mode)
        {
            RenderBackgroundInternal(
                g,
                rect,
                baseColor,
                borderColor,
                innerBorderColor,
                style,
                8,
                drawBorder,
                drawGlass,
                mode);
        }

        internal static void RenderBackgroundInternal(
           Graphics g,
           Rectangle rect,
           Color baseColor,
           Color borderColor,
           Color innerBorderColor,
           RoundStyle style,
           int roundWidth,
           bool drawBorder,
           bool drawGlass,
           LinearGradientMode mode)
        {
            RenderBackgroundInternal(
                 g,
                 rect,
                 baseColor,
                 borderColor,
                 innerBorderColor,
                 style,
                 8,
                 0.45f,
                 drawBorder,
                 drawGlass,
                 mode);
        }

        internal static void RenderBackgroundInternal(
           Graphics g,
           Rectangle rect,
           Color baseColor,
           Color borderColor,
           Color innerBorderColor,
           RoundStyle style,
           int roundWidth,
           float basePosition,
           bool drawBorder,
           bool drawGlass,
           LinearGradientMode mode)
        {
            if (drawBorder)
            {
                rect.Width--;
                rect.Height--;
            }

            using (LinearGradientBrush brush = new LinearGradientBrush(
                rect, Color.Transparent, Color.Transparent, mode))
            {
                Color[] colors = new Color[4];
                colors[0] = GetColor(baseColor, 0, 35, 24, 9);
                colors[1] = GetColor(baseColor, 0, 13, 8, 3);
                colors[2] = baseColor;
                colors[3] = GetColor(baseColor, 0, 35, 24, 9);

                ColorBlend blend = new ColorBlend();
                blend.Positions = new float[] { 0.0f, basePosition, basePosition + 0.05f, 1.0f };
                blend.Colors = colors;
                brush.InterpolationColors = blend;
                if (style != RoundStyle.None)
                {
                    using (GraphicsPath path =
                        GraphicsPathHelper.CreatePath(rect, roundWidth, style, false))
                    {
                        g.FillPath(brush, path);
                    }

                    if (baseColor.A > 80)
                    {
                        Rectangle rectTop = rect;

                        if (mode == LinearGradientMode.Vertical)
                        {
                            rectTop.Height = (int)(rectTop.Height * basePosition);
                        }
                        else
                        {
                            rectTop.Width = (int)(rect.Width * basePosition);
                        }
                        using (GraphicsPath pathTop = GraphicsPathHelper.CreatePath(
                            rectTop, roundWidth, RoundStyle.Top, false))
                        {
                            using (SolidBrush brushAlpha =
                                new SolidBrush(Color.FromArgb(128, 255, 255, 255)))
                            {
                                g.FillPath(brushAlpha, pathTop);
                            }
                        }
                    }

                    if (drawGlass)
                    {
                        RectangleF glassRect = rect;
                        if (mode == LinearGradientMode.Vertical)
                        {
                            glassRect.Y = rect.Y + rect.Height * basePosition;
                            glassRect.Height = (rect.Height - rect.Height * basePosition) * 2;
                        }
                        else
                        {
                            glassRect.X = rect.X + rect.Width * basePosition;
                            glassRect.Width = (rect.Width - rect.Width * basePosition) * 2;
                        }
                        ControlPaintEx.DrawGlass(g, glassRect, 170, 0);
                    }

                    if (drawBorder)
                    {
                        using (GraphicsPath path =
                            GraphicsPathHelper.CreatePath(rect, roundWidth, style, false))
                        {
                            using (Pen pen = new Pen(borderColor))
                            {
                                g.DrawPath(pen, path);
                            }
                        }

                        rect.Inflate(-1, -1);
                        using (GraphicsPath path =
                            GraphicsPathHelper.CreatePath(rect, roundWidth, style, false))
                        {
                            using (Pen pen = new Pen(innerBorderColor))
                            {
                                g.DrawPath(pen, path);
                            }
                        }
                    }
                }
                else
                {
                    g.FillRectangle(brush, rect);
                    if (baseColor.A > 80)
                    {
                        Rectangle rectTop = rect;
                        if (mode == LinearGradientMode.Vertical)
                        {
                            rectTop.Height = (int)(rectTop.Height * basePosition);
                        }
                        else
                        {
                            rectTop.Width = (int)(rect.Width * basePosition);
                        }
                        using (SolidBrush brushAlpha =
                            new SolidBrush(Color.FromArgb(128, 255, 255, 255)))
                        {
                            g.FillRectangle(brushAlpha, rectTop);
                        }
                    }

                    if (drawGlass)
                    {
                        RectangleF glassRect = rect;
                        if (mode == LinearGradientMode.Vertical)
                        {
                            glassRect.Y = rect.Y + rect.Height * basePosition;
                            glassRect.Height = (rect.Height - rect.Height * basePosition) * 2;
                        }
                        else
                        {
                            glassRect.X = rect.X + rect.Width * basePosition;
                            glassRect.Width = (rect.Width - rect.Width * basePosition) * 2;
                        }
                        ControlPaintEx.DrawGlass(g, glassRect, 200, 0);
                    }

                    if (drawBorder)
                    {
                        using (Pen pen = new Pen(borderColor))
                        {
                            g.DrawRectangle(pen, rect);
                        }

                        rect.Inflate(-1, -1);
                        using (Pen pen = new Pen(innerBorderColor))
                        {
                            g.DrawRectangle(pen, rect);
                        }
                    }
                }
            }
        }

        internal static void RenderArrowInternal(
            Graphics g,
            Rectangle dropDownRect,
            ArrowDirection direction,
            Brush brush)
        {
            Point point = new Point(
                dropDownRect.Left + (dropDownRect.Width / 2),
                dropDownRect.Top + (dropDownRect.Height / 2));
            Point[] points = null;
            switch (direction)
            {
                case ArrowDirection.Left:
                    points = new Point[] {
                        new Point(point.X + 2, point.Y - 3),
                        new Point(point.X + 2, point.Y + 3),
                        new Point(point.X - 1, point.Y) };
                    break;

                case ArrowDirection.Up:
                    points = new Point[] {
                        new Point(point.X - 3, point.Y + 2),
                        new Point(point.X + 3, point.Y + 2),
                        new Point(point.X, point.Y - 2) };
                    break;

                case ArrowDirection.Right:
                    points = new Point[] {
                        new Point(point.X - 2, point.Y - 3),
                        new Point(point.X - 2, point.Y + 3),
                        new Point(point.X + 1, point.Y) };
                    break;

                default:
                    points = new Point[] {
                        new Point(point.X - 3, point.Y - 1),
                        new Point(point.X + 3, point.Y - 1),
                        new Point(point.X, point.Y + 2) };
                    break;
            }
            g.FillPolygon(brush, points);
        }

        internal static Color GetColor(
            Color colorBase, int a, int r, int g, int b)
        {
            int a0 = colorBase.A;
            int r0 = colorBase.R;
            int g0 = colorBase.G;
            int b0 = colorBase.B;

            if (a + a0 > 255) { a = 255; } else { a = Math.Max(0, a + a0); }
            if (r + r0 > 255) { r = 255; } else { r = Math.Max(0, r + r0); }
            if (g + g0 > 255) { g = 255; } else { g = Math.Max(0, g + g0); }
            if (b + b0 > 255) { b = 255; } else { b = Math.Max(0, b + b0); }

            return Color.FromArgb(a, r, g, b);
        }
    }

    public static class GraphicsPathHelper
    {
        /// <summary> 
        /// 建立带有圆角样式的路径。 
        /// </summary> 
        /// <param name="rect">用来建立路径的矩形。</param> 
        /// <param name="_radius">圆角的大小。</param> 
        /// <param name="style">圆角的样式。</param> 
        /// <param name="correction">是否把矩形长宽减 1,以便画出边框。</param> 
        /// <returns>建立的路径。</returns> 
        public static GraphicsPath CreatePath(
            Rectangle rect, int radius, RoundStyle style, bool correction)
        {
            GraphicsPath path = new GraphicsPath();
            int radiusCorrection = correction ? 1 : 0;
            switch (style)
            {
                case RoundStyle.None:
                    path.AddRectangle(rect);
                    break;
                case RoundStyle.All:
                    path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                    path.AddArc(
                        rect.Right - radius - radiusCorrection,
                        rect.Y,
                        radius,
                        radius,
                        270,
                        90);
                    path.AddArc(
                        rect.Right - radius - radiusCorrection,
                        rect.Bottom - radius - radiusCorrection,
                        radius,
                        radius, 0, 90);
                    path.AddArc(
                        rect.X,
                        rect.Bottom - radius - radiusCorrection,
                        radius,
                        radius,
                        90,
                        90);
                    break;
                case RoundStyle.Left:
                    path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                    path.AddLine(
                        rect.Right - radiusCorrection, rect.Y,
                        rect.Right - radiusCorrection, rect.Bottom - radiusCorrection);
                    path.AddArc(
                        rect.X,
                        rect.Bottom - radius - radiusCorrection,
                        radius,
                        radius,
                        90,
                        90);
                    break;
                case RoundStyle.Right:
                    path.AddArc(
                        rect.Right - radius - radiusCorrection,
                        rect.Y,
                        radius,
                        radius,
                        270,
                        90);
                    path.AddArc(
                       rect.Right - radius - radiusCorrection,
                       rect.Bottom - radius - radiusCorrection,
                       radius,
                       radius,
                       0,
                       90);
                    path.AddLine(rect.X, rect.Bottom - radiusCorrection, rect.X, rect.Y);
                    break;
                case RoundStyle.Top:
                    path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                    path.AddArc(
                        rect.Right - radius - radiusCorrection,
                        rect.Y,
                        radius,
                        radius,
                        270,
                        90);
                    path.AddLine(
                        rect.Right - radiusCorrection, rect.Bottom - radiusCorrection,
                        rect.X, rect.Bottom - radiusCorrection);
                    break;
                case RoundStyle.Bottom:
                    path.AddArc(
                        rect.Right - radius - radiusCorrection,
                        rect.Bottom - radius - radiusCorrection,
                        radius,
                        radius,
                        0,
                        90);
                    path.AddArc(
                        rect.X,
                        rect.Bottom - radius - radiusCorrection,
                        radius,
                        radius,
                        90,
                        90);
                    path.AddLine(rect.X, rect.Y, rect.Right - radiusCorrection, rect.Y);
                    break;
                case RoundStyle.BottomLeft:
                    path.AddArc(
                        rect.X,
                        rect.Bottom - radius - radiusCorrection,
                        radius,
                        radius,
                        90,
                        90);
                    path.AddLine(rect.X, rect.Y, rect.Right - radiusCorrection, rect.Y);
                    path.AddLine(
                        rect.Right - radiusCorrection,
                        rect.Y,
                        rect.Right - radiusCorrection,
                        rect.Bottom - radiusCorrection);
                    break;
                case RoundStyle.BottomRight:
                    path.AddArc(
                        rect.Right - radius - radiusCorrection,
                        rect.Bottom - radius - radiusCorrection,
                        radius,
                        radius,
                        0,
                        90);
                    path.AddLine(rect.X, rect.Bottom - radiusCorrection, rect.X, rect.Y);
                    path.AddLine(rect.X, rect.Y, rect.Right - radiusCorrection, rect.Y);
                    break;
            }
            path.CloseFigure();

            return path;
        }
    }

    internal class TextRenderingHintGraphics : IDisposable
    {
        private Graphics _graphics;
        private TextRenderingHint _oldTextRenderingHint;

        public TextRenderingHintGraphics(Graphics graphics)
            : this(graphics, TextRenderingHint.AntiAlias)
        {
        }

        public TextRenderingHintGraphics(
            Graphics graphics,
            TextRenderingHint newTextRenderingHint)
        {
            _graphics = graphics;
            _oldTextRenderingHint = graphics.TextRenderingHint;
            _graphics.TextRenderingHint = newTextRenderingHint;
        }

        #region IDisposable 成员 

        public void Dispose()
        {
            _graphics.TextRenderingHint = _oldTextRenderingHint;
        }

        #endregion
    }

    public class InterpolationModeGraphics : IDisposable
    {
        private InterpolationMode _oldMode;
        private Graphics _graphics;

        public InterpolationModeGraphics(Graphics graphics)
            : this(graphics, InterpolationMode.HighQualityBicubic)
        {
        }

        public InterpolationModeGraphics(
            Graphics graphics, InterpolationMode newMode)
        {
            _graphics = graphics;
            _oldMode = graphics.InterpolationMode;
            graphics.InterpolationMode = newMode;
        }

        #region IDisposable 成员 

        public void Dispose()
        {
            _graphics.InterpolationMode = _oldMode;
        }

        #endregion
    }

    internal class LayoutUtils
    {
        public static Rectangle DeflateRect(Rectangle rect, Padding padding)
        {
            rect.X += padding.Left;
            rect.Y += padding.Top;
            rect.Width -= padding.Horizontal;
            rect.Height -= padding.Vertical;
            return rect;
        }

        public static Rectangle RTLTranslate(Rectangle bounds, Rectangle withinBounds)
        {
            bounds.X = withinBounds.Width - bounds.Right;
            return bounds;
        }

        public static bool IsEmptyRect(Rectangle rect)
        {
            return rect.Width <= 0 || rect.Height <= 0;
        }
    }

    public class SmoothingModeGraphics : IDisposable
    {
        private SmoothingMode _oldMode;
        private Graphics _graphics;

        public SmoothingModeGraphics(Graphics graphics)
            : this(graphics, SmoothingMode.AntiAlias)
        {
        }

        public SmoothingModeGraphics(Graphics graphics, SmoothingMode newMode)
        {
            _graphics = graphics;
            _oldMode = graphics.SmoothingMode;
            graphics.SmoothingMode = newMode;
        }

        #region IDisposable 成员 

        public void Dispose()
        {
            _graphics.SmoothingMode = _oldMode;
        }

        #endregion
    }

    public sealed class ControlPaintEx
    {
        public static void DrawCheckedFlag(Graphics graphics, Rectangle rect, Color color)
        {
            PointF[] points = new PointF[3];
            points[0] = new PointF(
                rect.X + rect.Width / 4.5f,
                rect.Y + rect.Height / 2.5f);
            points[1] = new PointF(
                rect.X + rect.Width / 2.5f,
                rect.Bottom - rect.Height / 3f);
            points[2] = new PointF(
                rect.Right - rect.Width / 4.0f,
                rect.Y + rect.Height / 4.5f);
            using (Pen pen = new Pen(color, 2F))
            {
                graphics.DrawLines(pen, points);
            }
        }

        public static void DrawGlass(
            Graphics g, RectangleF glassRect, int alphaCenter, int alphaSurround)
        {
            DrawGlass(g, glassRect, Color.White, alphaCenter, alphaSurround);
        }

        public static void DrawGlass(
           Graphics g,
            RectangleF glassRect,
            Color glassColor,
            int alphaCenter,
            int alphaSurround)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(glassRect);
                using (PathGradientBrush brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.FromArgb(alphaCenter, glassColor);
                    brush.SurroundColors = new Color[] {
                        Color.FromArgb(alphaSurround, glassColor) };
                    brush.CenterPoint = new PointF(
                        glassRect.X + glassRect.Width / 2,
                        glassRect.Y + glassRect.Height / 2);
                    g.FillPath(brush, path);
                }
            }
        }

        public static void DrawBackgroundImage(
            Graphics g,
            Image backgroundImage,
            Color backColor,
            ImageLayout backgroundImageLayout,
            Rectangle bounds,
            Rectangle clipRect)
        {
            DrawBackgroundImage(
                g,
                backgroundImage,
                backColor,
                backgroundImageLayout,
                bounds,
                clipRect,
                Point.Empty,
                RightToLeft.No);
        }

        public static void DrawBackgroundImage(
            Graphics g,
            Image backgroundImage,
            Color backColor,
            ImageLayout backgroundImageLayout,
            Rectangle bounds,
            Rectangle clipRect,
            Point scrollOffset)
        {
            DrawBackgroundImage(
                g,
                backgroundImage,
                backColor,
                backgroundImageLayout,
                bounds,
                clipRect,
                scrollOffset,
                RightToLeft.No);
        }

        public static void DrawBackgroundImage(
            Graphics g,
            Image backgroundImage,
            Color backColor,
            ImageLayout backgroundImageLayout,
            Rectangle bounds,
            Rectangle clipRect,
            Point scrollOffset,
            RightToLeft rightToLeft)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }
            if (backgroundImageLayout == ImageLayout.Tile)
            {
                using (TextureBrush brush = new TextureBrush(backgroundImage, WrapMode.Tile))
                {
                    if (scrollOffset != Point.Empty)
                    {
                        Matrix transform = brush.Transform;
                        transform.Translate((float)scrollOffset.X, (float)scrollOffset.Y);
                        brush.Transform = transform;
                    }
                    g.FillRectangle(brush, clipRect);
                    return;
                }
            }
            Rectangle rect = CalculateBackgroundImageRectangle(
                bounds,
                backgroundImage,
                backgroundImageLayout);
            if ((rightToLeft == RightToLeft.Yes) &&
                (backgroundImageLayout == ImageLayout.None))
            {
                rect.X += clipRect.Width - rect.Width;
            }
            using (SolidBrush brush2 = new SolidBrush(backColor))
            {
                g.FillRectangle(brush2, clipRect);
            }
            if (!clipRect.Contains(rect))
            {
                if ((backgroundImageLayout == ImageLayout.Stretch) ||
                    (backgroundImageLayout == ImageLayout.Zoom))
                {
                    rect.Intersect(clipRect);
                    g.DrawImage(backgroundImage, rect);
                }
                else if (backgroundImageLayout == ImageLayout.None)
                {
                    rect.Offset(clipRect.Location);
                    Rectangle destRect = rect;
                    destRect.Intersect(clipRect);
                    Rectangle rectangle3 = new Rectangle(Point.Empty, destRect.Size);
                    g.DrawImage(
                        backgroundImage,
                        destRect,
                        rectangle3.X,
                        rectangle3.Y,
                        rectangle3.Width,
                        rectangle3.Height,
                        GraphicsUnit.Pixel);
                }
                else
                {
                    Rectangle rectangle4 = rect;
                    rectangle4.Intersect(clipRect);
                    Rectangle rectangle5 = new Rectangle(
                        new Point(rectangle4.X - rect.X, rectangle4.Y - rect.Y),
                        rectangle4.Size);
                    g.DrawImage(
                        backgroundImage,
                        rectangle4,
                        rectangle5.X,
                        rectangle5.Y,
                        rectangle5.Width,
                        rectangle5.Height,
                        GraphicsUnit.Pixel);
                }
            }
            else
            {
                ImageAttributes imageAttr = new ImageAttributes();
                imageAttr.SetWrapMode(WrapMode.TileFlipXY);
                g.DrawImage(
                    backgroundImage,
                    rect,
                    0,
                    0,
                    backgroundImage.Width,
                    backgroundImage.Height,
                    GraphicsUnit.Pixel,
                    imageAttr);
                imageAttr.Dispose();
            }
        }

        internal static Rectangle CalculateBackgroundImageRectangle(
            Rectangle bounds,
            Image backgroundImage,
            ImageLayout imageLayout)
        {
            Rectangle rectangle = bounds;
            if (backgroundImage != null)
            {
                switch (imageLayout)
                {
                    case ImageLayout.None:
                        rectangle.Size = backgroundImage.Size;
                        return rectangle;

                    case ImageLayout.Tile:
                        return rectangle;

                    case ImageLayout.Center:
                        {
                            rectangle.Size = backgroundImage.Size;
                            Size size = bounds.Size;
                            if (size.Width > rectangle.Width)
                            {
                                rectangle.X = (size.Width - rectangle.Width) / 2;
                            }
                            if (size.Height > rectangle.Height)
                            {
                                rectangle.Y = (size.Height - rectangle.Height) / 2;
                            }
                            return rectangle;
                        }
                    case ImageLayout.Stretch:
                        rectangle.Size = bounds.Size;
                        return rectangle;

                    case ImageLayout.Zoom:
                        {
                            Size size2 = backgroundImage.Size;
                            float num = ((float)bounds.Width) / ((float)size2.Width);
                            float num2 = ((float)bounds.Height) / ((float)size2.Height);
                            if (num >= num2)
                            {
                                rectangle.Height = bounds.Height;
                                rectangle.Width = (int)((size2.Width * num2) + 0.5);
                                if (bounds.X >= 0)
                                {
                                    rectangle.X = (bounds.Width - rectangle.Width) / 2;
                                }
                                return rectangle;
                            }
                            rectangle.Width = bounds.Width;
                            rectangle.Height = (int)((size2.Height * num) + 0.5);
                            if (bounds.Y >= 0)
                            {
                                rectangle.Y = (bounds.Height - rectangle.Height) / 2;
                            }
                            return rectangle;
                        }
                }
            }
            return rectangle;
        }
    }

}

