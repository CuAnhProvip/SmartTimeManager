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
    public partial class UcDashboard : UserControl
    {
        private Label lblClock;
        private Label lblDate;
        private Label lblTotal;
        private Label lblCompleted;
        private Label lblInProgress;
        private Label lblOverdue;
        private Label lblCompletedNote;
        private Label lblInProgressNote;
        private Label lblOverdueNote;
        private DonutChart donutChart;
        private Label lblLegendCompleted;
        private Label lblLegendInProgress;
        private Label lblLegendPending;
        private Label lblLegendOverdue;
        private FlowLayoutPanel upcomingPanel;
        private FlowLayoutPanel completedPanel;
        private Timer timer;
        private int lastDataRefreshMinute = -1;

        public UcDashboard()
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.None;
            DoubleBuffered = true;
            BuildUI();
            LoadData();
            StartClock();
        }

        private void BuildUI()
        {
            Controls.Clear();

            TableLayoutPanel root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.RowCount = 3;
            root.ColumnCount = 1;
            root.BackColor = Theme.Background;
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 84));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 165));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(root);

            root.Controls.Add(BuildHeader(), 0, 0);
            root.Controls.Add(BuildStats(), 0, 1);
            root.Controls.Add(BuildContent(), 0, 2);
        }

        private Control BuildHeader()
        {
            TableLayoutPanel header = new TableLayoutPanel();
            header.Dock = DockStyle.Fill;
            header.ColumnCount = 3;
            header.RowCount = 1;
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));
            header.BackColor = Theme.Background;

            FlowLayoutPanel titleBox = new FlowLayoutPanel();
            titleBox.Dock = DockStyle.Fill;
            titleBox.FlowDirection = FlowDirection.TopDown;
            titleBox.WrapContents = false;
            titleBox.BackColor = Theme.Background;
            titleBox.Controls.Add(Ui.Label("Dashboard", Theme.H1, Theme.Text));
            titleBox.Controls.Add(Ui.Label("Welcome back!", Theme.Body, Theme.Muted));
            header.Controls.Add(titleBox, 0, 0);

            PillButton addBtn = new PillButton();
            addBtn.Text = "+ Add Task";
            addBtn.Width = 120;
            addBtn.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            addBtn.Margin = new Padding(0, 8, 18, 0);
            addBtn.Click += (s, e) => AddTask();
            header.Controls.Add(addBtn, 1, 0);

            FlowLayoutPanel clockBox = new FlowLayoutPanel();
            clockBox.Dock = DockStyle.Fill;
            clockBox.FlowDirection = FlowDirection.TopDown;
            clockBox.WrapContents = false;
            clockBox.BackColor = Theme.Background;
            clockBox.Padding = new Padding(0, 3, 0, 0);
            lblClock = Ui.Label(DateTime.Now.ToString("HH:mm:ss"), new Font("Segoe UI", 22F, FontStyle.Bold), Theme.Text);
            lblDate = Ui.Label(DateTime.Now.ToString("dddd, MMM dd, yyyy", CultureInfo.InvariantCulture), Theme.Small, Theme.Muted);
            clockBox.Controls.Add(lblClock);
            clockBox.Controls.Add(lblDate);
            header.Controls.Add(clockBox, 2, 0);

            return header;
        }

        private Control BuildStats()
        {
            TableLayoutPanel stats = new TableLayoutPanel();
            stats.Dock = DockStyle.Fill;
            stats.RowCount = 1;
            stats.ColumnCount = 4;
            stats.BackColor = Theme.Background;
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            stats.Controls.Add(CreateStatCard("Total Tasks", "▦", Theme.Purple, out lblTotal, out _), 0, 0);
            stats.Controls.Add(CreateStatCard("Completed", "✓", Theme.Green, out lblCompleted, out lblCompletedNote), 1, 0);
            stats.Controls.Add(CreateStatCard("In Progress", "⌛", Theme.Blue, out lblInProgress, out lblInProgressNote), 2, 0);
            stats.Controls.Add(CreateStatCard("Overdue", "!", Theme.Red, out lblOverdue, out lblOverdueNote), 3, 0);
            return stats;
        }

        private RoundedPanel CreateStatCard(string title, string iconText, Color accent, out Label numberLabel, out Label noteLabel)
        {
            RoundedPanel card = new RoundedPanel();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(0, 8, 20, 12);
            card.Padding = new Padding(20);

            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 2;
            layout.RowCount = 1;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            card.Controls.Add(layout);

            RoundedPanel iconBox = new RoundedPanel();
            iconBox.Size = new Size(70, 70);
            iconBox.BackColor = Color.FromArgb(241, 245, 249);
            iconBox.BorderSize = 0;
            iconBox.Radius = 22;
            iconBox.Padding = new Padding(0);
            iconBox.Margin = new Padding(0, 16, 0, 0);
            Label icon = new Label();
            icon.Text = iconText;
            icon.Dock = DockStyle.Fill;
            icon.TextAlign = ContentAlignment.MiddleCenter;
            icon.Font = new Font("Segoe UI Symbol", 28F, FontStyle.Bold);
            icon.ForeColor = accent;
            iconBox.Controls.Add(icon);
            layout.Controls.Add(iconBox, 0, 0);

            FlowLayoutPanel texts = new FlowLayoutPanel();
            texts.Dock = DockStyle.Fill;
            texts.FlowDirection = FlowDirection.TopDown;
            texts.WrapContents = false;
            texts.Padding = new Padding(0, 16, 0, 0);
            texts.BackColor = Color.Transparent;
            texts.Controls.Add(Ui.Label(title, Theme.H3, Theme.Text));
            numberLabel = Ui.Label("0", Theme.Number, Theme.Text);
            texts.Controls.Add(numberLabel);
            noteLabel = Ui.Label(title == "Total Tasks" ? "All tasks" : "0%", Theme.Small, accent);
            texts.Controls.Add(noteLabel);
            layout.Controls.Add(texts, 1, 0);
            return card;
        }

        private Control BuildContent()
        {
            TableLayoutPanel content = new TableLayoutPanel();
            content.Dock = DockStyle.Fill;
            content.ColumnCount = 2;
            content.RowCount = 1;
            content.BackColor = Theme.Background;
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 47));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 53));
            content.Controls.Add(BuildOverviewCard(), 0, 0);
            content.Controls.Add(BuildRightColumn(), 1, 0);
            return content;
        }

        private Control BuildOverviewCard()
        {
            RoundedPanel card = new RoundedPanel();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(0, 10, 20, 0);
            card.Padding = new Padding(26);

            Label title = Ui.Label("Tasks Overview", Theme.H2, Theme.Text);
            title.Location = new Point(26, 22);
            card.Controls.Add(title);

            donutChart = new DonutChart();
            donutChart.Location = new Point(30, 95);
            card.Controls.Add(donutChart);

            int lx = 300;
            int y = 110;
            AddLegend(card, Theme.Green, "Completed", ref lblLegendCompleted, lx, y); y += 55;
            AddLegend(card, Theme.Blue, "In Progress", ref lblLegendInProgress, lx, y); y += 55;
            AddLegend(card, Theme.Yellow, "Pending", ref lblLegendPending, lx, y); y += 55;
            AddLegend(card, Theme.Red, "Overdue", ref lblLegendOverdue, lx, y);
            return card;
        }

        private void AddLegend(Control parent, Color color, string name, ref Label valueLabel, int x, int y)
        {
            Panel dot = new Panel();
            dot.Size = new Size(12, 12);
            dot.BackColor = color;
            dot.Location = new Point(x, y + 7);
            parent.Controls.Add(dot);
            Label nameLabel = Ui.FixedLabel(name, Theme.Body, Theme.Text, 120);
            nameLabel.Location = new Point(x + 28, y);
            parent.Controls.Add(nameLabel);
            valueLabel = Ui.FixedLabel("0 (0%)", Theme.Body, Theme.Text, 90);
            valueLabel.Location = new Point(x + 160, y);
            parent.Controls.Add(valueLabel);
        }

        private Control BuildRightColumn()
        {
            TableLayoutPanel right = new TableLayoutPanel();
            right.Dock = DockStyle.Fill;
            right.RowCount = 2;
            right.ColumnCount = 1;
            right.BackColor = Theme.Background;
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            right.Controls.Add(BuildUpcomingCard(), 0, 0);
            right.Controls.Add(BuildCompletedCard(), 0, 1);
            return right;
        }

        private Control BuildUpcomingCard()
        {
            RoundedPanel card = new RoundedPanel();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(0, 10, 0, 8);
            card.Padding = new Padding(26);

            Label title = Ui.Label("Upcoming Tasks", Theme.H2, Theme.Text);
            title.Location = new Point(26, 22);
            card.Controls.Add(title);

            Panel line = new Panel();
            line.BackColor = Theme.Border;
            line.Location = new Point(26, 70);
            line.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            line.Width = 600;
            line.Height = 1;
            card.Controls.Add(line);

            upcomingPanel = new FlowLayoutPanel();
            upcomingPanel.Location = new Point(26, 88);
            upcomingPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            upcomingPanel.Size = new Size(610, 360);
            upcomingPanel.FlowDirection = FlowDirection.TopDown;
            upcomingPanel.WrapContents = false;
            upcomingPanel.AutoScroll = true;
            upcomingPanel.BackColor = Color.Transparent;
            card.Controls.Add(upcomingPanel);
            card.Resize += (s, e) =>
            {
                line.Width = Math.Max(100, card.Width - 52);
                upcomingPanel.Width = Math.Max(100, card.Width - 52);
                upcomingPanel.Height = Math.Max(100, card.Height - 110);
            };
            return card;
        }

        private Control BuildCompletedCard()
        {
            RoundedPanel card = new RoundedPanel();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(0, 8, 0, 0);
            card.Padding = new Padding(22);

            Label title = Ui.Label("Completed Tasks", Theme.H2, Theme.Text);
            title.Location = new Point(22, 16);
            card.Controls.Add(title);

            Label hint = Ui.Label("Recently finished tasks", Theme.Small, Theme.Muted);
            hint.Location = new Point(22, 43);
            card.Controls.Add(hint);

            Panel line = new Panel();
            line.BackColor = Theme.Border;
            line.Location = new Point(22, 70);
            line.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            line.Width = 600;
            line.Height = 1;
            card.Controls.Add(line);

            completedPanel = new FlowLayoutPanel();
            completedPanel.Location = new Point(22, 82);
            completedPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            completedPanel.Size = new Size(610, 190);
            completedPanel.FlowDirection = FlowDirection.TopDown;
            completedPanel.WrapContents = false;
            completedPanel.AutoScroll = true;
            completedPanel.BackColor = Color.Transparent;
            card.Controls.Add(completedPanel);

            card.Resize += (s, e) =>
            {
                line.Width = Math.Max(100, card.Width - 44);
                completedPanel.Width = Math.Max(100, card.Width - 44);
                completedPanel.Height = Math.Max(100, card.Height - 96);
            };
            return card;
        }

        private void LoadData()
        {
            List<TaskModel> tasks = SafeTasks();
            int total = tasks.Count;
            int completed = tasks.Count(t => t.IsCompleted || Same(t.Status, "Completed"));
            int overdue = tasks.Count(IsOverdue);
            int inProgress = tasks.Count(IsInProgress);
            int pending = tasks.Count(IsPending);

            lblTotal.Text = total.ToString();
            lblCompleted.Text = completed.ToString();
            lblInProgress.Text = inProgress.ToString();
            lblOverdue.Text = overdue.ToString();
            lblCompletedNote.Text = Ui.Percent(completed, total) + "% completed";
            lblInProgressNote.Text = Ui.Percent(inProgress, total) + "% in progress";
            lblOverdueNote.Text = overdue > 0 ? "Need attention" : "No overdue tasks";

            donutChart.Completed = completed;
            donutChart.InProgress = inProgress;
            donutChart.Pending = pending;
            donutChart.Overdue = overdue;
            donutChart.Invalidate();
            lblLegendCompleted.Text = completed + " (" + Ui.Percent(completed, total) + "%)";
            lblLegendInProgress.Text = inProgress + " (" + Ui.Percent(inProgress, total) + "%)";
            lblLegendPending.Text = pending + " (" + Ui.Percent(pending, total) + "%)";
            lblLegendOverdue.Text = overdue + " (" + Ui.Percent(overdue, total) + "%)";

            LoadUpcoming(tasks);
            LoadCompleted(tasks);
        }

        private void LoadUpcoming(List<TaskModel> tasks)
        {
            upcomingPanel.Controls.Clear();
            var items = tasks.Where(t => !t.IsCompleted && !Same(t.Status, "Completed"))
                             .OrderBy(t => ParseDate(t.DueDate) ?? DateTime.MaxValue)
                             .ThenBy(t => t.ReminderTime)
                             .Take(8)
                             .ToList();
            if (items.Count == 0)
            {
                upcomingPanel.Controls.Add(Ui.Label("No upcoming tasks", Theme.Body, Theme.Muted));
                return;
            }
            foreach (TaskModel t in items)
                upcomingPanel.Controls.Add(CreateTaskRow(t));
        }

        private void LoadCompleted(List<TaskModel> tasks)
        {
            if (completedPanel == null) return;
            completedPanel.Controls.Clear();
            var items = tasks.Where(t => t.IsCompleted || Same(t.Status, "Completed"))
                             .OrderByDescending(t => ParseDate(t.DueDate) ?? DateTime.MinValue)
                             .ThenByDescending(t => t.ReminderTime)
                             .Take(6)
                             .ToList();
            if (items.Count == 0)
            {
                completedPanel.Controls.Add(Ui.Label("No completed tasks yet", Theme.Body, Theme.Muted));
                return;
            }

            foreach (TaskModel t in items)
                completedPanel.Controls.Add(CreateCompletedTaskRow(t));
        }

        private Control CreateCompletedTaskRow(TaskModel task)
        {
            Panel row = new Panel();
            row.Width = Math.Max(520, completedPanel.Width - 25);
            row.Height = 52;
            row.Margin = new Padding(0, 0, 0, 6);
            row.BackColor = Color.Transparent;

            string category = Ui.Safe(task.Category, "General");

            Panel iconBox = new Panel();
            iconBox.Size = new Size(32, 32);
            iconBox.Location = new Point(2, 8);
            iconBox.BackColor = Ui.CategorySoftColor(category);
            row.Controls.Add(iconBox);

            var iconImage = Ui.CategoryImage(category);
            if (iconImage != null)
            {
                PictureBox iconPic = new PictureBox();
                iconPic.Dock = DockStyle.Fill;
                iconPic.SizeMode = PictureBoxSizeMode.Zoom;
                iconPic.Image = iconImage;
                iconBox.Controls.Add(iconPic);
            }
            else
            {
                Label icon = new Label();
                icon.Dock = DockStyle.Fill;
                icon.Text = Ui.CategoryIcon(category);
                icon.Font = Ui.CategoryIconFont(category, 13F);
                icon.ForeColor = Ui.CategoryColor(category);
                icon.TextAlign = ContentAlignment.MiddleCenter;
                iconBox.Controls.Add(icon);
            }

            Label name = Ui.FixedLabel(Ui.Safe(task.TaskName, "Untitled task"), Theme.H3, Theme.Text, 300, 22);
            name.Location = new Point(48, 5);
            row.Controls.Add(name);

            Label sub = Ui.FixedLabel("✓ Completed  •  " + category, Theme.Small, Theme.Green, 320, 20);
            sub.Location = new Point(48, 28);
            row.Controls.Add(sub);

            DateTime? due = ParseDate(task.DueDate);
            Label dueLbl = Ui.FixedLabel(DueCaption(due), Theme.Small, Theme.Muted, 90, 20);
            dueLbl.Location = new Point(row.Width - 110, 6);
            dueLbl.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            row.Controls.Add(dueLbl);

            Label time = Ui.FixedLabel(Ui.Safe(task.ReminderTime, "--:--"), Theme.Small, Theme.Muted, 90, 20);
            time.Location = new Point(row.Width - 110, 28);
            time.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            row.Controls.Add(time);

            Panel divider = new Panel();
            divider.BackColor = Theme.Border;
            divider.Location = new Point(0, 50);
            divider.Width = row.Width;
            divider.Height = 1;
            divider.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            row.Controls.Add(divider);
            return row;
        }

        private Control CreateTaskRow(TaskModel task)
        {
            Panel row = new Panel();
            row.Width = Math.Max(520, upcomingPanel.Width - 25);
            row.Height = 72;
            row.Margin = new Padding(0, 0, 0, 6);
            row.BackColor = Color.Transparent;

            string category = Ui.Safe(task.Category, "General");

            Panel iconBox = new Panel();
            iconBox.Size = new Size(38, 38);
            iconBox.Location = new Point(2, 12);
            iconBox.BackColor = Ui.CategorySoftColor(category);
            row.Controls.Add(iconBox);

            var iconImage = Ui.CategoryImage(category);
            if (iconImage != null)
            {
                PictureBox iconPic = new PictureBox();
                iconPic.Dock = DockStyle.Fill;
                iconPic.SizeMode = PictureBoxSizeMode.Zoom;
                iconPic.Image = iconImage;
                iconBox.Controls.Add(iconPic);
            }
            else
            {
                Label icon = new Label();
                icon.Dock = DockStyle.Fill;
                icon.Text = Ui.CategoryIcon(category);
                icon.Font = Ui.CategoryIconFont(category, 15F);
                icon.ForeColor = Ui.CategoryColor(category);
                icon.TextAlign = ContentAlignment.MiddleCenter;
                iconBox.Controls.Add(icon);
            }

            Label name = Ui.FixedLabel(Ui.Safe(task.TaskName, "Untitled task"), Theme.H3, Theme.Text, 330, 24);
            name.Location = new Point(56, 7);
            row.Controls.Add(name);

            Panel pdot = new Panel();
            pdot.Size = new Size(8, 8);
            pdot.BackColor = task.Priority != null && task.Priority.ToLower().Contains("high") ? Theme.Red : (task.Priority != null && task.Priority.ToLower().Contains("low") ? Theme.Green : Theme.Yellow);
            pdot.Location = new Point(56, 42);
            row.Controls.Add(pdot);

            Label sub = Ui.FixedLabel(Ui.Safe(task.Priority, "Medium") + " Priority  •  " + category, Theme.Body, Theme.Muted, 360, 24);
            sub.Location = new Point(70, 34);
            row.Controls.Add(sub);

            DateTime? due = ParseDate(task.DueDate);
            Label dueLbl = Ui.FixedLabel(DueCaption(due), Theme.H3, Theme.Muted, 100, 24);
            dueLbl.Location = new Point(row.Width - 118, 7);
            dueLbl.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            row.Controls.Add(dueLbl);

            Label time = Ui.FixedLabel(Ui.Safe(task.ReminderTime, "--:--"), Theme.Body, Theme.Muted, 100, 24);
            time.Location = new Point(row.Width - 118, 34);
            time.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            row.Controls.Add(time);

            Panel divider = new Panel();
            divider.BackColor = Theme.Border;
            divider.Location = new Point(0, 70);
            divider.Width = row.Width;
            divider.Height = 1;
            divider.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            row.Controls.Add(divider);
            return row;
        }

        private void AddTask()
        {
            using (FormAddTask f = new FormAddTask())
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    DatabaseService.InsertTask(new TaskModel
                    {
                        TaskName = f.TaskName,
                        Category = f.Category,
                        Priority = f.Priority,
                        DueDate = f.DueDate,
                        ReminderTime = f.ReminderTime,
                        Status = f.Status,
                        IsCompleted = false
                    });
                    LoadData();
                }
            }
        }

        private void StartClock()
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += (s, e) =>
            {
                lblClock.Text = DateTime.Now.ToString("HH:mm:ss");
                lblDate.Text = DateTime.Now.ToString("dddd, MMM dd, yyyy", CultureInfo.InvariantCulture);

                // Tránh dashboard bị chớp: chỉ reload dữ liệu khi đổi phút.
                // Overdue theo ReminderTime chỉ cần cập nhật theo phút, không cần clear/rebuild mỗi giây.
                int currentMinute = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                if (currentMinute != lastDataRefreshMinute)
                {
                    lastDataRefreshMinute = currentMinute;
                    LoadData();
                }
            };
            timer.Start();
        }

        private List<TaskModel> SafeTasks()
        {
            try { return DatabaseService.GetAllTasks(); }
            catch { return new List<TaskModel>(); }
        }

        private bool IsOverdue(TaskModel t)
        {
            if (t == null || t.IsCompleted || Same(t.Status, "Completed")) return false;
            // Nếu người dùng đã bấm Start task thì ưu tiên trạng thái In Progress,
            // không tự ép về Overdue nữa dù thời gian đã qua.
            if (Contains(t.Status, "Progress")) return false;

            DateTime? d = ParseDate(t.DueDate);
            if (!d.HasValue) return false;
            if (d.Value.Date < DateTime.Today) return true;
            if (d.Value.Date == DateTime.Today)
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

        private bool Same(string a, string b) { return string.Equals(a ?? "", b, StringComparison.OrdinalIgnoreCase); }
        private bool Contains(string a, string b) { return (a ?? "").IndexOf(b, StringComparison.OrdinalIgnoreCase) >= 0; }

        private DateTime? ParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            string[] formats = { "dd/MM/yyyy", "d/M/yyyy", "MM/dd/yyyy", "M/d/yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "d-M-yyyy" };
            DateTime dt;
            if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) return dt;
            if (DateTime.TryParse(value, out dt)) return dt;
            return null;
        }

        private string DueCaption(DateTime? due)
        {
            if (!due.HasValue) return "No date";
            if (due.Value.Date == DateTime.Today) return "Today";
            if (due.Value.Date == DateTime.Today.AddDays(1)) return "Tomorrow";
            return due.Value.ToString("MMM dd", CultureInfo.InvariantCulture);
        }

        private Color PriorityColor(string p)
        {
            p = (p ?? "").ToLowerInvariant();
            if (p.Contains("high")) return Theme.Red;
            if (p.Contains("low")) return Theme.Green;
            return Theme.Yellow;
        }
    }
}
