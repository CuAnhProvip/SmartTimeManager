using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using SmartTimeManager.Models;

namespace SmartTimeManager.Services
{
    public static class DatabaseService
    {
        private static string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "Tasks.db");
        private static string connectionString = $"Data Source={dbPath};Version=3;";

        static DatabaseService()
        {
            string folder = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                // 1. Giữ nguyên câu lệnh tạo bảng Tasks cũ của bạn...
                string createTableSql = @"
                    CREATE TABLE IF NOT EXISTS Tasks (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        IsCompleted INTEGER DEFAULT 0,
                        TaskName TEXT NOT NULL,
                        Category TEXT,
                        Priority TEXT,
                        DueDate TEXT,
                        ReminderTime TEXT DEFAULT '00:00',
                        Status TEXT DEFAULT 'Pending'
                    );";
                using (var cmd = new SQLiteCommand(createTableSql, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // 2. BỔ SUNG: Câu lệnh tạo bảng Goals mới tinh để lưu trữ mục tiêu dài hạn
                string createGoalsTableSql = @"
                    CREATE TABLE IF NOT EXISTS Goals (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        GoalName TEXT NOT NULL,
                        Progress INTEGER DEFAULT 0,
                        TargetDate TEXT
                    );";
                using (var cmd = new SQLiteCommand(createGoalsTableSql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<TaskModel> GetAllTasks()
        {
            var list = new List<TaskModel>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Tasks ORDER BY Id DESC";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new TaskModel()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            IsCompleted = Convert.ToInt32(reader["IsCompleted"]) == 1,
                            TaskName = reader["TaskName"].ToString(),
                            Category = reader["Category"].ToString(),
                            Priority = reader["Priority"].ToString(),
                            DueDate = reader["DueDate"].ToString(),
                            ReminderTime = reader["ReminderTime"] != DBNull.Value ? reader["ReminderTime"].ToString() : "00:00", // Đọc trường giờ nhắc nhở
                            Status = reader["Status"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public static int InsertTask(TaskModel task)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                // NÂNG CẤP CÂU LỆNH INSERT: Lưu thêm trường ReminderTime
                string sql = @"
                    INSERT INTO Tasks (IsCompleted, TaskName, Category, Priority, DueDate, ReminderTime, Status) 
                    VALUES (@isComp, @name, @cat, @pri, @due, @remind, @status);
                    SELECT last_insert_rowid();";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@isComp", task.IsCompleted ? 1 : 0);
                    cmd.Parameters.AddWithValue("@name", task.TaskName);
                    cmd.Parameters.AddWithValue("@cat", task.Category);
                    cmd.Parameters.AddWithValue("@pri", task.Priority);
                    cmd.Parameters.AddWithValue("@due", task.DueDate);
                    cmd.Parameters.AddWithValue("@remind", task.ReminderTime); // Đưa tham số giờ nhắc nhở vào câu lệnh
                    cmd.Parameters.AddWithValue("@status", task.Status);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static void UpdateTaskStatus(int id, bool isCompleted)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE Tasks SET IsCompleted = @isComp, Status = @status WHERE Id = @id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@isComp", isCompleted ? 1 : 0);
                    cmd.Parameters.AddWithValue("@status", isCompleted ? "Completed" : "Pending");
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateTaskFields(int id, string category, string priority, string status)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE Tasks SET Category = @cat, Priority = @pri, Status = @status, IsCompleted = @isComp WHERE Id = @id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cat", category);
                    cmd.Parameters.AddWithValue("@pri", priority);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@isComp", status == "Completed" ? 1 : 0);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateTaskWorkflowStatus(int id, string status)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE Tasks SET Status = @status, IsCompleted = 0 WHERE Id = @id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteTask(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM Tasks WHERE Id = @id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        // ========================================================
        // 🎯 CÁC HÀM XỬ LÝ DATABASE CHO BẢNG GOALS (MỤC TIÊU)
        // ========================================================

        public static void ResetAllData()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    using (var cmd = new SQLiteCommand(conn))
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = "DELETE FROM Tasks";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "DELETE FROM Goals";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "DELETE FROM sqlite_sequence WHERE name IN ('Tasks', 'Goals')";
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
        }

        public static System.Data.DataTable GetAllGoals()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Goals ORDER BY Id DESC";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var adapter = new SQLiteDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public static int InsertGoal(string name, string targetDate)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Goals (GoalName, Progress, TargetDate) VALUES (@name, 0, @date); SELECT last_insert_rowid();";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@date", targetDate);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static void UpdateGoalProgress(int id, int newProgress)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE Goals SET Progress = @prog WHERE Id = @id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@prog", newProgress);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteGoal(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM Goals WHERE Id = @id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}