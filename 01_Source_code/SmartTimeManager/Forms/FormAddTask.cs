using System;
using System.Drawing;
using System.Windows.Forms;

namespace SmartTimeManager.Forms
{
    public class FormAddTask : Form
    {
        private TextBox txtTaskName;
        private ComboBox cbCategory;
        private ComboBox cbPriority;
        private ComboBox cbStatus;
        private DateTimePicker dtpDueDate;
        private DateTimePicker dtpReminderTime;
        private Button btnSave;
        private Button btnCancel;

        public string TaskName => txtTaskName.Text.Trim();
        public string Category => cbCategory.SelectedItem?.ToString() ?? "Study";
        public string Priority => cbPriority.SelectedItem?.ToString() ?? "Medium";
        public string Status => "Pending";
        public string DueDate => dtpDueDate.Value.ToString("dd/MM/yyyy");
        public string ReminderTime => dtpReminderTime.Value.ToString("HH:mm");

        public FormAddTask()
        {
            DoubleBuffered = true;
            BuildAddTaskUI();
        }

        private void BuildAddTaskUI()
        {
            Text = "Add New Task";
            Size = new Size(460, 610);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(248, 250, 252);
            Font = new Font("Segoe UI", 10);

            Label lblHeader = new Label()
            {
                Text = "Add New Task",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(25, 20),
                AutoSize = true
            };
            Controls.Add(lblHeader);

            Label lblName = MakeLabel("Task Name", 25, 75);
            txtTaskName = new TextBox() { Size = new Size(390, 27), Location = new Point(25, 100), Font = new Font("Segoe UI", 11) };
            Controls.Add(lblName);
            Controls.Add(txtTaskName);

            Label lblCat = MakeLabel("Category", 25, 145);
            cbCategory = new ComboBox() { Size = new Size(390, 28), Location = new Point(25, 170), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10) };
            cbCategory.Items.AddRange(new string[] { "Study", "Project", "Health", "Work", "Entertainment" });
            cbCategory.SelectedIndex = 0;
            Controls.Add(lblCat);
            Controls.Add(cbCategory);

            Label lblPri = MakeLabel("Priority", 25, 215);
            cbPriority = new ComboBox() { Size = new Size(390, 28), Location = new Point(25, 240), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10) };
            cbPriority.Items.AddRange(new string[] { "High", "Medium", "Low" });
            cbPriority.SelectedIndex = 1;
            Controls.Add(lblPri);
            Controls.Add(cbPriority);

            Label lblStatus = MakeLabel("Initial Status", 25, 285);
            cbStatus = new ComboBox() { Size = new Size(390, 28), Location = new Point(25, 310), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10), Enabled = false };
            cbStatus.Items.AddRange(new string[] { "Pending" });
            cbStatus.SelectedIndex = 0;
            Controls.Add(lblStatus);
            Controls.Add(cbStatus);

            GroupBox gbTime = new GroupBox()
            {
                Text = "Date & Reminder",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Size = new Size(390, 130),
                Location = new Point(25, 360)
            };
            Controls.Add(gbTime);

            Label lblDue = new Label() { Text = "Due Date", Font = new Font("Segoe UI", 9), ForeColor = Color.DimGray, Location = new Point(15, 30), AutoSize = true };
            dtpDueDate = new DateTimePicker() { Format = DateTimePickerFormat.Short, Size = new Size(160, 27), Location = new Point(15, 55), Font = new Font("Segoe UI", 10) };
            gbTime.Controls.Add(lblDue);
            gbTime.Controls.Add(dtpDueDate);

            Label lblRemind = new Label() { Text = "Reminder Time", Font = new Font("Segoe UI", 9), ForeColor = Color.DimGray, Location = new Point(210, 30), AutoSize = true };
            dtpReminderTime = new DateTimePicker() { Format = DateTimePickerFormat.Time, ShowUpDown = true, Size = new Size(160, 27), Location = new Point(210, 55), Font = new Font("Segoe UI", 10) };
            gbTime.Controls.Add(lblRemind);
            gbTime.Controls.Add(dtpReminderTime);

            Label hint = new Label()
            {
                Text = "New tasks start as Pending. Use Start Task to move them to In Progress. Overdue is calculated automatically.",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(25, 498),
                Size = new Size(390, 22)
            };
            Controls.Add(hint);

            btnSave = new Button()
            {
                Text = "Save Task",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Size = new Size(110, 35),
                Location = new Point(200, 530),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button()
            {
                Text = "Cancel",
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(226, 232, 240),
                ForeColor = Color.FromArgb(71, 85, 105),
                Size = new Size(95, 35),
                Location = new Point(320, 530),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.Add(btnSave);
            Controls.Add(btnCancel);
        }

        private Label MakeLabel(string text, int x, int y)
        {
            return new Label()
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(x, y),
                AutoSize = true
            };
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTaskName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên công việc!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTaskName.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
