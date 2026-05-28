using System;

namespace SmartTimeManager.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public bool IsCompleted { get; set; }
        public string TaskName { get; set; }
        public string Category { get; set; }
        public string Priority { get; set; }
        public string DueDate { get; set; }

        // TRƯỜNG MỚI: Lưu trữ Giờ nhắc nhở dạng chuỗi HH:mm (Ví dụ: "14:25")
        public string ReminderTime { get; set; }
        public string Status { get; set; }
    }
}