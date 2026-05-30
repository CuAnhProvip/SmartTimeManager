using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using SmartTimeManager.Models;
using SmartTimeManager.Services;
using SmartTimeManager.UI;

namespace SmartTimeManager.Forms
{
    public partial class UcStatistics : UserControl
    {
        public UcStatistics()
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.None;
            DoubleBuffered = true;
            BuildUI();
        }

        private void BuildUI()
        {
            Controls.Clear();
            BackColor = Theme.Background;

            var tasks = SafeTasks();
            int total = tasks.Count;
            int completed = tasks.Count(t => t.IsCompleted || Same(t.Status, "Completed"));
            int inProgress = tasks.Count(IsInProgress);
            int overdue = tasks.Count(IsOverdue);
            int pending = tasks.Count(IsPending);
            int high = tasks.Count(t => Contains(t.Priority, "High"));

            TableLayoutPanel root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.BackColor = Theme.Background;
            root.RowCount = 3;
            root.ColumnCount = 1;
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 164));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(root);

            FlowLayoutPanel header = new FlowLayoutPanel();
            header.Dock = DockStyle.Fill;
            header.FlowDirection = FlowDirection.TopDown;
            header.WrapContents = false;
            header.BackColor = Theme.Background;
            header.Margin = new Padding(0);
            header.Controls.Add(Ui.Label("Statistics", Theme.H1, Theme.Text));
            header.Controls.Add(Ui.Label("Overview of your productivity", Theme.Body, Theme.Muted));
            root.Controls.Add(header, 0, 0);

            TableLayoutPanel cards = new TableLayoutPanel();
            cards.Dock = DockStyle.Fill;
            cards.BackColor = Theme.Background;
            cards.ColumnCount = 3;
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            cards.RowCount = 1;
            cards.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            cards.Controls.Add(CreateTopCard("Task Completion Status", "Overall completion rate", Ui.Percent(completed, total) + "%", Theme.Green), 0, 0);
            cards.Controls.Add(CreateTopCard("Tasks by Category", "Tasks count by category", tasks.Select(t => Ui.Safe(t.Category, "General")).Distinct().Count().ToString(), Theme.Blue), 1, 0);
            cards.Controls.Add(CreateTopCard("High Priority Tasks", "Tasks that need attention", high.ToString(), Theme.Red), 2, 0);
            root.Controls.Add(cards, 0, 1);

            TableLayoutPanel body = new TableLayoutPanel();
            body.Dock = DockStyle.Fill;
            body.BackColor = Theme.Background;
            body.ColumnCount = 3;
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 31));
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 29));
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            body.Controls.Add(BuildCompletionCard(completed, inProgress, pending, overdue, total), 0, 0);
            body.Controls.Add(BuildCategoryCard(tasks), 1, 0);
            body.Controls.Add(BuildHighPriorityCard(tasks), 2, 0);
            root.Controls.Add(body, 0, 2);
        }

        private Control CreateTopCard(string title, string subtitle, string value, Color valueColor)
        {
            RoundedPanel card = new RoundedPanel();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(0, 8, 18, 12);
            card.Padding = new Padding(0);

            Panel body = new Panel();
            body.Dock = DockStyle.Fill;
            body.BackColor = Color.Transparent;
            card.Controls.Add(body);

            body.Controls.Add(MakeLabel(title, Theme.H3, Theme.Text, 34, 26, 240, 24));
            body.Controls.Add(MakeLabel(subtitle, Theme.Small, Theme.Muted, 34, 52, 240, 20));

            Label lblVal = new Label();
            lblVal.Text = value;
            lblVal.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblVal.ForeColor = valueColor;
            lblVal.AutoSize = true;
            lblVal.Location = new Point(34, 84);
            body.Controls.Add(lblVal);

            return card;
        }

        private Control BuildCompletionCard(int completed, int inProgress, int pending, int overdue, int total)
        {
            RoundedPanel card = new RoundedPanel();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(0, 10, 18, 0);
            card.Padding = new Padding(0);

            Panel body = new Panel();
            body.Dock = DockStyle.Fill;
            body.BackColor = Color.Transparent;
            card.Controls.Add(body);

            body.Controls.Add(MakeLabel("Task Completion Status", Theme.H3, Theme.Text, 28, 24, 240, 24));
            body.Controls.Add(MakeLabel("Overall completion rate", Theme.Small, Theme.Muted, 28, 50, 240, 20));

            DonutChart chart = new DonutChart();
            chart.Completed = completed;
            chart.InProgress = inProgress;
            chart.Pending = pending;
            chart.Overdue = overdue;
            chart.Size = new Size(190, 190);
            chart.BackColor = Theme.Card;
            chart.Location = new Point(76, 92);
            body.Controls.Add(chart);

            Label pct = new Label();
            pct.Text = Ui.Percent(completed, total) + "%";
            pct.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            pct.ForeColor = Theme.Text;
            pct.AutoSize = true;
            pct.BackColor = Color.Transparent;
            pct.Location = new Point(146, 164);
            body.Controls.Add(pct);

            Label cap = new Label();
            cap.Text = "Completed";
            cap.Font = Theme.Small;
            cap.ForeColor = Theme.Muted;
            cap.AutoSize = true;
            cap.BackColor = Color.Transparent;
            cap.Location = new Point(128, 196);
            body.Controls.Add(cap);

            int y = 310;
            body.Controls.Add(CreateLegendRow(Theme.Green, "Completed", completed, total, 28, y)); y += 42;
            body.Controls.Add(CreateLegendRow(Theme.Blue, "In Progress", inProgress, total, 28, y)); y += 42;
            body.Controls.Add(CreateLegendRow(Theme.Yellow, "Pending", pending, total, 28, y)); y += 42;
            body.Controls.Add(CreateLegendRow(Theme.Red, "Overdue", overdue, total, 28, y));
            return card;
        }

        private Control CreateLegendRow(Color color, string label, int value, int total, int x, int y)
        {
            Panel row = new Panel();
            row.Size = new Size(270, 28);
            row.Location = new Point(x, y);
            row.BackColor = Color.Transparent;
            Panel dot = new Panel();
            dot.Size = new Size(10, 10);
            dot.BackColor = color;
            dot.Location = new Point(0, 8);
            row.Controls.Add(dot);
            row.Controls.Add(MakeLabel(label, Theme.Body, Theme.Text, 26, 1, 125, 24));
            Label val = MakeLabel(value + " (" + Ui.Percent(value, total) + "%)", Theme.Body, Theme.Text, 165, 1, 90, 24);
            val.TextAlign = ContentAlignment.MiddleRight;
            row.Controls.Add(val);
            return row;
        }

        private Control BuildCategoryCard(List<TaskModel> tasks)
        {
            RoundedPanel card = new RoundedPanel();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(0, 10, 18, 0);
            card.Padding = new Padding(0);
            Panel body = new Panel();
            body.Dock = DockStyle.Fill;
            body.BackColor = Color.Transparent;
            card.Controls.Add(body);

            body.Controls.Add(MakeLabel("Tasks by Category", Theme.H3, Theme.Text, 28, 24, 240, 24));
            body.Controls.Add(MakeLabel("Number of tasks in each category", Theme.Small, Theme.Muted, 28, 50, 250, 20));

            var palette = new[] { Theme.Green, Theme.Blue, Theme.Yellow, Theme.Purple, Theme.Red };
            var groups = tasks.GroupBy(t => Ui.Safe(t.Category, "General")).OrderByDescending(g => g.Count()).Take(5).ToList();
            if (groups.Count == 0)
            {
                body.Controls.Add(MakeLabel("No data yet", Theme.Body, Theme.Muted, 28, 92, 220, 24));
                return card;
            }
            int max = Math.Max(1, groups.Max(g => g.Count()));
            int y = 92;
            for (int i = 0; i < groups.Count; i++)
            {
                body.Controls.Add(CreateCategoryRow(groups[i].Key, groups[i].Count(), max, palette[i % palette.Length], 28, y));
                y += 74;
            }
            return card;
        }

        private Control CreateCategoryRow(string name, int value, int max, Color color, int x, int y)
        {
            Panel row = new Panel();
            row.Size = new Size(300, 58);
            row.Location = new Point(x, y);
            row.BackColor = Color.Transparent;
            row.Controls.Add(MakeLabel(name, Theme.Body, Theme.Text, 0, 0, 190, 24));
            Label num = MakeLabel(value + " task(s)", Theme.Body, Theme.Text, 205, 0, 85, 24);
            num.TextAlign = ContentAlignment.MiddleRight;
            row.Controls.Add(num);
            Panel track = new Panel();
            track.Size = new Size(285, 7);
            track.Location = new Point(0, 34);
            track.BackColor = Color.FromArgb(226, 232, 240);
            row.Controls.Add(track);
            Panel fill = new Panel();
            fill.Height = 7;
            fill.Width = Math.Max(12, (int)Math.Round(285.0 * value / Math.Max(1, max)));
            fill.BackColor = color;
            track.Controls.Add(fill);
            return row;
        }

        private Control BuildHighPriorityCard(List<TaskModel> tasks)
        {
            RoundedPanel card = new RoundedPanel();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(0, 10, 0, 0);
            card.Padding = new Padding(0);
            Panel body = new Panel();
            body.Dock = DockStyle.Fill;
            body.BackColor = Color.Transparent;
            card.Controls.Add(body);

            body.Controls.Add(MakeLabel("High Priority Tasks", Theme.H3, Theme.Text, 28, 24, 240, 24));
            body.Controls.Add(MakeLabel("Tasks that need attention", Theme.Small, Theme.Muted, 28, 50, 240, 20));

            var high = tasks.Where(t => Contains(t.Priority, "High"))
                            .OrderBy(t => ParseDate(t.DueDate) ?? DateTime.MaxValue)
                            .Take(6)
                            .ToList();
            if (high.Count == 0)
            {
                body.Controls.Add(MakeLabel("No high priority tasks", Theme.Body, Theme.Muted, 28, 92, 240, 24));
                return card;
            }

            int y = 92;
            foreach (var task in high)
            {
                body.Controls.Add(CreateHighPriorityRow(task, 28, y));
                y += 70;
            }

            Button viewAll = new Button();
            viewAll.Text = "View all tasks";
            viewAll.FlatStyle = FlatStyle.Flat;
            viewAll.FlatAppearance.BorderColor = Theme.Border;
            viewAll.BackColor = Theme.Card;
            viewAll.ForeColor = Theme.Primary;
            viewAll.Font = Theme.Small;
            viewAll.Size = new Size(120, 32);
            viewAll.Location = new Point(28, y + 4);
            body.Controls.Add(viewAll);
            return card;
        }

        private Control CreateHighPriorityRow(TaskModel task, int x, int y)
        {
            Panel row = new Panel();
            row.Size = new Size(430, 58);
            row.Location = new Point(x, y);
            row.BackColor = Color.Transparent;

            Panel dot = new Panel();
            dot.Size = new Size(8, 8);
            dot.BackColor = Theme.Red;
            dot.Location = new Point(0, 8);
            row.Controls.Add(dot);

            row.Controls.Add(MakeLabel(Ui.Safe(task.TaskName, "Untitled task"), Theme.Body, Theme.Text, 18, 0, 270, 22));
            row.Controls.Add(MakeLabel(Ui.Safe(task.Priority, "High") + " Priority", Theme.Small, Theme.Muted, 18, 28, 220, 20));
            row.Controls.Add(MakeLabel(DueText(ParseDate(task.DueDate)), Theme.Small, Theme.Muted, 300, 0, 90, 20));
            row.Controls.Add(MakeLabel(Ui.Safe(task.ReminderTime, "--:--"), Theme.Small, Theme.Text, 300, 26, 90, 20));
            return row;
        }

        private Label MakeLabel(string text, Font font, Color color, int x, int y, int w, int h)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = font;
            lbl.ForeColor = color;
            lbl.BackColor = Color.Transparent;
            lbl.AutoSize = false;
            lbl.Location = new Point(x, y);
            lbl.Size = new Size(w, h);
            return lbl;
        }

        private bool IsOverdue(TaskModel t)
        {
            if (t == null || t.IsCompleted || Same(t.Status, "Completed")) return false;
            // Nếu người dùng đã bấm Start task thì ưu tiên trạng thái In Progress,
            // không tự ép về Overdue nữa dù thời gian đã qua.
            if (Contains(t.Status, "Progress")) return false;

            DateTime? date = ParseDate(t.DueDate);
            if (!date.HasValue) return false;
            if (date.Value.Date < DateTime.Today) return true;
            if (date.Value.Date == DateTime.Today)
            {
                TimeSpan ts;
                if (TimeSpan.TryParse(t.ReminderTime, out ts))
                    return DateTime.Now.TimeOfDay > ts;
            }
            return false;
        }

        private bool IsInProgress(TaskModel t)
        {
            if (t == null || t.IsCompleted || Same(t.Status, "Completed") || IsOverdue(t)) return false;
            return Contains(t.Status, "Progress");
        }

        private bool IsPending(TaskModel t)
        {
            if (t == null || t.IsCompleted || Same(t.Status, "Completed") || IsOverdue(t) || IsInProgress(t)) return false;
            return true;
        }

        private string DueText(DateTime? date)
        {
            if (!date.HasValue) return "No date";
            if (date.Value.Date == DateTime.Today) return "Today";
            if (date.Value.Date == DateTime.Today.AddDays(1)) return "Tomorrow";
            return date.Value.ToString("MMM dd", CultureInfo.InvariantCulture);
        }

        private DateTime? ParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            string[] formats = { "dd/MM/yyyy", "d/M/yyyy", "MM/dd/yyyy", "M/d/yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "d-M-yyyy" };
            DateTime dt;
            if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) return dt;
            if (DateTime.TryParse(value, out dt)) return dt;
            return null;
        }

        private List<TaskModel> SafeTasks()
        {
            try { return DatabaseService.GetAllTasks(); }
            catch { return new List<TaskModel>(); }
        }

        private bool Same(string a, string b) { return string.Equals(a ?? "", b, StringComparison.OrdinalIgnoreCase); }
        private bool Contains(string a, string b) { return (a ?? "").IndexOf(b, StringComparison.OrdinalIgnoreCase) >= 0; }
    }
}
