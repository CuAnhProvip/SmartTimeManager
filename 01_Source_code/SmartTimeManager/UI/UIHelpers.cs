using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace SmartTimeManager.UI
{
    public static class Theme
    {
        public static readonly Color Sidebar = Color.FromArgb(7, 20, 38);
        public static readonly Color SidebarLine = Color.FromArgb(28, 45, 70);
        public static readonly Color Background = Color.FromArgb(248, 250, 252);
        public static readonly Color Card = Color.White;
        public static readonly Color Border = Color.FromArgb(226, 232, 240);
        public static readonly Color Text = Color.FromArgb(15, 23, 42);
        public static readonly Color Muted = Color.FromArgb(100, 116, 139);
        public static readonly Color Primary = Color.FromArgb(37, 99, 235);
        public static readonly Color Primary2 = Color.FromArgb(59, 130, 246);
        public static readonly Color Blue = Color.FromArgb(59, 130, 246);
        public static readonly Color Green = Color.FromArgb(74, 194, 134);
        public static readonly Color Yellow = Color.FromArgb(245, 179, 51);
        public static readonly Color Red = Color.FromArgb(244, 87, 107);
        public static readonly Color Purple = Color.FromArgb(168, 85, 247);
        public static readonly Color SoftBlue = Color.FromArgb(239, 246, 255);
        public static readonly Color SoftGreen = Color.FromArgb(236, 253, 245);
        public static readonly Color SoftYellow = Color.FromArgb(255, 251, 235);
        public static readonly Color SoftRed = Color.FromArgb(254, 242, 242);
        public static readonly Color SoftPurple = Color.FromArgb(245, 243, 255);

        public static readonly Font H1 = new Font("Segoe UI", 22F, FontStyle.Bold);
        public static readonly Font H2 = new Font("Segoe UI", 14F, FontStyle.Bold);
        public static readonly Font H3 = new Font("Segoe UI", 11F, FontStyle.Bold);
        public static readonly Font Body = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static readonly Font Small = new Font("Segoe UI", 8.5F, FontStyle.Regular);
        public static readonly Font Number = new Font("Segoe UI", 24F, FontStyle.Bold);
    }

    public class RoundedPanel : Panel
    {
        public int Radius { get; set; } = 18;
        public Color BorderColor { get; set; } = Theme.Border;
        public int BorderSize { get; set; } = 1;

        public RoundedPanel()
        {
            DoubleBuffered = true;
            BackColor = Theme.Card;
            Padding = new Padding(16);
            Margin = new Padding(8);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            if (Width > 0 && Height > 0)
            {
                using (GraphicsPath path = RoundPath(new Rectangle(0, 0, Width, Height), Radius))
                    Region = new Region(path);
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = RoundPath(rect, Radius))
            using (SolidBrush brush = new SolidBrush(BackColor))
            using (Pen pen = new Pen(BorderColor, BorderSize))
            {
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }
            base.OnPaint(e);
        }

        public static GraphicsPath RoundPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            if (d <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    public class SidebarButton : Button
    {
        public bool Active { get; set; }
        public bool Hovered { get; set; }
        public int Radius { get; set; } = 12;

        public SidebarButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            UseVisualStyleBackColor = false;
            Cursor = Cursors.Hand;
            Height = 50;
            Width = 220;
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            TextAlign = ContentAlignment.MiddleLeft;
            Padding = new Padding(18, 0, 0, 0);
            ForeColor = Color.FromArgb(226, 232, 240);
            BackColor = Theme.Sidebar;
            TabStop = false;

            MouseEnter += (s, e) => { Hovered = true; Invalidate(); };
            MouseLeave += (s, e) => { Hovered = false; Invalidate(); };
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            using (GraphicsPath path = RoundedPanel.RoundPath(new Rectangle(0, 0, Width - 1, Height - 1), Radius))
                Region = new Region(path);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Color bg = Active ? Theme.Primary : (Hovered ? Color.FromArgb(13, 33, 60) : Theme.Sidebar);
            Color fg = Active ? Color.White : Color.FromArgb(226, 232, 240);
            using (GraphicsPath path = RoundedPanel.RoundPath(new Rectangle(0, 0, Width - 1, Height - 1), Radius))
            using (SolidBrush brush = new SolidBrush(bg))
            {
                pevent.Graphics.FillPath(brush, path);
            }
            TextRenderer.DrawText(pevent.Graphics, Text, Font,
                new Rectangle(Padding.Left, 0, Width - Padding.Left, Height), fg,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
    }

    public class PillButton : Button
    {
        public int Radius { get; set; } = 10;
        public PillButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            UseVisualStyleBackColor = false;
            Cursor = Cursors.Hand;
            BackColor = Theme.Primary;
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Height = 38;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            using (GraphicsPath path = RoundedPanel.RoundPath(new Rectangle(0, 0, Width - 1, Height - 1), Radius))
                Region = new Region(path);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = RoundedPanel.RoundPath(new Rectangle(0, 0, Width - 1, Height - 1), Radius))
            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                pevent.Graphics.FillPath(brush, path);
                TextRenderer.DrawText(pevent.Graphics, Text, Font, ClientRectangle, ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }
    }

    public class DonutChart : Control
    {
        public int Completed { get; set; }
        public int InProgress { get; set; }
        public int Pending { get; set; }
        public int Overdue { get; set; }

        public DonutChart()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            BackColor = Theme.Card;
            Size = new Size(230, 230);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            using (SolidBrush brush = new SolidBrush(BackColor))
                pevent.Graphics.FillRectangle(brush, ClientRectangle);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(BackColor);
            int total = Completed + InProgress + Pending + Overdue;
            int size = Math.Min(Width, Height) - 22;
            if (size < 40) return;

            Rectangle outer = new Rectangle((Width - size) / 2, (Height - size) / 2, size, size);
            Rectangle inner = Rectangle.Inflate(outer, -(int)(size * 0.28), -(int)(size * 0.28));
            if (total <= 0)
            {
                using (SolidBrush b = new SolidBrush(Color.FromArgb(230, 235, 242)))
                    e.Graphics.FillEllipse(b, outer);
                using (SolidBrush b = new SolidBrush(BackColor))
                    e.Graphics.FillEllipse(b, inner);
                return;
            }

            float start = -90f;
            DrawPie(e.Graphics, outer, Theme.Green, ref start, Completed, total);
            DrawPie(e.Graphics, outer, Theme.Blue, ref start, InProgress, total);
            DrawPie(e.Graphics, outer, Theme.Yellow, ref start, Pending, total);
            DrawPie(e.Graphics, outer, Theme.Red, ref start, Overdue, total);
            using (SolidBrush b = new SolidBrush(BackColor))
                e.Graphics.FillEllipse(b, inner);
        }

        private void DrawPie(Graphics g, Rectangle rect, Color color, ref float start, int value, int total)
        {
            if (value <= 0) return;
            float sweep = 360f * value / total;
            using (SolidBrush b = new SolidBrush(color))
                g.FillPie(b, rect, start, sweep);
            start += sweep;
        }
    }

    public static class Ui
    {
        private static readonly Dictionary<string, Image> _imageCache = new Dictionary<string, Image>();

        public static Label Label(string text, Font font, Color color)
        {
            return new Label
            {
                Text = text,
                Font = font,
                ForeColor = color,
                BackColor = Color.Transparent,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 6)
            };
        }

        public static Label FixedLabel(string text, Font font, Color color, int width, int height = 24)
        {
            return new Label
            {
                Text = text,
                Font = font,
                ForeColor = color,
                BackColor = Color.Transparent,
                AutoSize = false,
                Width = width,
                Height = height
            };
        }

        public static Panel Dot(Color color)
        {
            Panel dot = new Panel();
            dot.Size = new Size(10, 10);
            dot.BackColor = color;
            dot.Margin = new Padding(0, 8, 10, 0);
            return dot;
        }

        public static string CategoryIcon(string category)
        {
            string c = (category ?? "").Trim().ToLowerInvariant();
            if (c.Contains("project") || c.Contains("dự án") || c.Contains("goal")) return "◎";
            if (c.Contains("study") || c.Contains("school") || c.Contains("learn") || c.Contains("học")) return "📖";
            if (c.Contains("health") || c.Contains("gym") || c.Contains("sport") || c.Contains("sức")) return "✚";
            if (c.Contains("code") || c.Contains("dev") || c.Contains("program")) return "⌘";
            if (c.Contains("work") || c.Contains("job") || c.Contains("office")) return "💼";
            if (c.Contains("entertain") || c.Contains("game") || c.Contains("movie")) return "★";
            if (c.Contains("personal") || c.Contains("home")) return "⌂";
            return "•";
        }

        public static Font CategoryIconFont(string category, float size = 17F)
        {
            string icon = CategoryIcon(category);
            if (icon == "📖" || icon == "💼")
                return new Font("Segoe UI Emoji", size - 1F, FontStyle.Regular);
            return new Font("Segoe UI Symbol", size, FontStyle.Bold);
        }

        public static string CategoryAssetFile(string category)
        {
            string c = (category ?? "").Trim().ToLowerInvariant();
            if (c.Contains("entertain") || c.Contains("game") || c.Contains("movie")) return "icon_entertainment.png";
            if (c.Contains("health") || c.Contains("gym") || c.Contains("sport") || c.Contains("sức")) return "icon_health.png";
            if (c.Contains("project") || c.Contains("dự án") || c.Contains("goal")) return "icon_project.png";
            return null;
        }

        public static Image CategoryImage(string category)
        {
            string file = CategoryAssetFile(category);
            if (string.IsNullOrEmpty(file)) return null;
            return LoadAssetImage(file);
        }

        public static Image AppLogoImage()
        {
            return LoadAssetImage("logo_stopwatch.png");
        }

        public static Image LoadAssetImage(string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName)) return null;
                if (_imageCache.ContainsKey(fileName)) return _imageCache[fileName];
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", fileName);
                if (!File.Exists(path)) return null;
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (var img = Image.FromStream(fs))
                {
                    Image clone = new Bitmap(img);
                    _imageCache[fileName] = clone;
                    return clone;
                }
            }
            catch { return null; }
        }

        public static Color CategoryColor(string category)
        {
            string c = (category ?? "").Trim().ToLowerInvariant();
            if (c.Contains("project") || c.Contains("dự án") || c.Contains("goal")) return Theme.Green;
            if (c.Contains("study") || c.Contains("school") || c.Contains("learn") || c.Contains("học")) return Theme.Purple;
            if (c.Contains("health") || c.Contains("gym") || c.Contains("sport") || c.Contains("sức")) return Theme.Yellow;
            if (c.Contains("code") || c.Contains("dev") || c.Contains("program")) return Theme.Red;
            if (c.Contains("work") || c.Contains("job") || c.Contains("office")) return Theme.Blue;
            if (c.Contains("entertain") || c.Contains("game") || c.Contains("movie")) return Theme.Red;
            if (c.Contains("personal") || c.Contains("home")) return Theme.Primary2;
            return Theme.Muted;
        }

        public static Color CategorySoftColor(string category)
        {
            string c = (category ?? "").Trim().ToLowerInvariant();
            if (c.Contains("project") || c.Contains("dự án") || c.Contains("goal")) return Theme.SoftGreen;
            if (c.Contains("study") || c.Contains("school") || c.Contains("learn") || c.Contains("học")) return Theme.SoftPurple;
            if (c.Contains("health") || c.Contains("gym") || c.Contains("sport") || c.Contains("sức")) return Theme.SoftYellow;
            if (c.Contains("code") || c.Contains("dev") || c.Contains("program")) return Theme.SoftRed;
            if (c.Contains("work") || c.Contains("job") || c.Contains("office")) return Theme.SoftBlue;
            if (c.Contains("entertain") || c.Contains("game") || c.Contains("movie")) return Theme.SoftRed;
            if (c.Contains("personal") || c.Contains("home")) return Theme.SoftBlue;
            return Color.FromArgb(241, 245, 249);
        }

        public static string Safe(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }

        public static int Percent(int value, int total)
        {
            if (total <= 0) return 0;
            return (int)Math.Round(value * 100.0 / total);
        }
    }
}
