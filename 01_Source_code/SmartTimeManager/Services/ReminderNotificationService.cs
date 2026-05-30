using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Media;
using System.Windows.Forms;
using SmartTimeManager.Models;
using SmartTimeManager.UI;

namespace SmartTimeManager.Services
{
    public class ReminderNotificationService : IDisposable
    {
        private readonly Control _owner;
        private readonly Timer _timer;
        private readonly HashSet<int> _notifiedTaskIds = new HashSet<int>();
        private SoundPlayer _activePlayer;
        private Form _activePopup;

        public ReminderNotificationService(Control owner)
        {
            _owner = owner;
            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += (s, e) => CheckDueNotifications();
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            StopReminderSound();
            try
            {
                if (_activePopup != null && !_activePopup.IsDisposed)
                    _activePopup.Close();
            }
            catch { }
        }

        private void CheckDueNotifications()
        {
            try
            {
                DateTime now = DateTime.Now;
                foreach (TaskModel task in DatabaseService.GetAllTasks())
                {
                    if (task == null || task.IsCompleted || Same(task.Status, "Completed")) continue;
                    if (_notifiedTaskIds.Contains(task.Id)) continue;

                    DateTime? date = ParseDate(task.DueDate);
                    TimeSpan reminder;
                    if (!date.HasValue || !TimeSpan.TryParse(task.ReminderTime, out reminder)) continue;

                    DateTime dueTime = date.Value.Date.Add(reminder);

                    // Chỉ báo trong đúng phút tới hạn. Task đã quá hạn từ trước sẽ không kêu lại.
                    if (dueTime.Year == now.Year && dueTime.Month == now.Month && dueTime.Day == now.Day &&
                        dueTime.Hour == now.Hour && dueTime.Minute == now.Minute)
                    {
                        _notifiedTaskIds.Add(task.Id);
                        ShowReminderPopup(task);
                    }
                }
            }
            catch
            {
                // Không để lỗi notification làm crash app.
            }
        }

        private void ShowReminderPopup(TaskModel task)
        {
            StopReminderSound();
            PlayReminderSoundLoop();

            try
            {
                if (_activePopup != null && !_activePopup.IsDisposed)
                    _activePopup.Close();

                Form popup = new Form();
                popup.Text = "Reminder Notification";
                popup.StartPosition = FormStartPosition.CenterScreen;
                popup.Size = new Size(450, 270);
                popup.FormBorderStyle = FormBorderStyle.FixedDialog;
                popup.MaximizeBox = false;
                popup.MinimizeBox = false;
                popup.TopMost = true;
                popup.BackColor = Theme.Background;
                popup.Font = Theme.Body;

                Panel header = new Panel();
                header.Dock = DockStyle.Top;
                header.Height = 68;
                header.BackColor = Theme.Primary;
                popup.Controls.Add(header);

                Label bell = new Label();
                bell.Text = "🔔";
                bell.Font = new Font("Segoe UI Emoji", 24F, FontStyle.Regular);
                bell.AutoSize = true;
                bell.Location = new Point(22, 14);
                bell.ForeColor = Color.White;
                header.Controls.Add(bell);

                Label title = new Label();
                title.Text = "Reminder is due now";
                title.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
                title.ForeColor = Color.White;
                title.AutoSize = true;
                title.Location = new Point(74, 13);
                header.Controls.Add(title);

                Label subHeader = new Label();
                subHeader.Text = "Bell is ringing — turn it off when you notice it";
                subHeader.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                subHeader.ForeColor = Color.FromArgb(219, 234, 254);
                subHeader.AutoSize = true;
                subHeader.Location = new Point(76, 40);
                header.Controls.Add(subHeader);

                Label taskName = new Label();
                taskName.Text = Ui.Safe(task.TaskName, "Untitled task");
                taskName.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
                taskName.ForeColor = Theme.Text;
                taskName.AutoSize = false;
                taskName.Location = new Point(28, 88);
                taskName.Size = new Size(380, 34);
                popup.Controls.Add(taskName);

                Label detail = new Label();
                detail.Text = "Category: " + Ui.Safe(task.Category, "General") + "   •   Priority: " + Ui.Safe(task.Priority, "Medium");
                detail.Font = Theme.Body;
                detail.ForeColor = Theme.Muted;
                detail.AutoSize = false;
                detail.Location = new Point(30, 126);
                detail.Size = new Size(380, 24);
                popup.Controls.Add(detail);

                Label time = new Label();
                time.Text = "Due time: " + Ui.Safe(task.ReminderTime, "now");
                time.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                time.ForeColor = Theme.Primary;
                time.AutoSize = false;
                time.Location = new Point(30, 154);
                time.Size = new Size(380, 24);
                popup.Controls.Add(time);

                Button completeButton = new Button();
                completeButton.Text = "Mark completed";
                completeButton.Font = Theme.Body;
                completeButton.BackColor = Color.FromArgb(226, 232, 240);
                completeButton.ForeColor = Theme.Text;
                completeButton.FlatStyle = FlatStyle.Flat;
                completeButton.FlatAppearance.BorderSize = 0;
                completeButton.Size = new Size(150, 38);
                completeButton.Location = new Point(76, 196);
                completeButton.Cursor = Cursors.Hand;
                completeButton.Click += (s, e) =>
                {
                    try { DatabaseService.UpdateTaskStatus(task.Id, true); } catch { }
                    popup.Close();
                };
                popup.Controls.Add(completeButton);

                Button stopButton = new Button();
                stopButton.Text = "Turn off notification";
                stopButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                stopButton.BackColor = Theme.Primary;
                stopButton.ForeColor = Color.White;
                stopButton.FlatStyle = FlatStyle.Flat;
                stopButton.FlatAppearance.BorderSize = 0;
                stopButton.Size = new Size(170, 38);
                stopButton.Location = new Point(238, 196);
                stopButton.Cursor = Cursors.Hand;
                stopButton.Click += (s, e) => popup.Close();
                popup.Controls.Add(stopButton);

                popup.FormClosed += (s, e) =>
                {
                    StopReminderSound();
                    if (_activePopup == popup) _activePopup = null;
                };

                _activePopup = popup;
                Form ownerForm = _owner != null ? _owner.FindForm() : null;
                if (ownerForm != null && !ownerForm.IsDisposed) popup.Show(ownerForm);
                else popup.Show();
                popup.Activate();
            }
            catch
            {
                StopReminderSound();
                try { SystemSounds.Exclamation.Play(); } catch { }
            }
        }

        private void PlayReminderSoundLoop()
        {
            try
            {
                string wavPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "alert.wav");
                if (File.Exists(wavPath))
                {
                    _activePlayer = new SoundPlayer(wavPath);
                    _activePlayer.LoadAsync();
                    _activePlayer.PlayLooping();
                }
                else
                {
                    SystemSounds.Exclamation.Play();
                }
            }
            catch
            {
                try { SystemSounds.Exclamation.Play(); } catch { }
            }
        }

        private void StopReminderSound()
        {
            try
            {
                if (_activePlayer != null)
                {
                    _activePlayer.Stop();
                    _activePlayer.Dispose();
                    _activePlayer = null;
                }
            }
            catch { }
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

        private bool Same(string a, string b) { return string.Equals(a ?? "", b, StringComparison.OrdinalIgnoreCase); }

        public void Dispose()
        {
            Stop();
            if (_timer != null) _timer.Dispose();
        }
    }
}
