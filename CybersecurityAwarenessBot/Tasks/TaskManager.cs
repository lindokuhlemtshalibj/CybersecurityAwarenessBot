// TaskManager.cs
// Manages the list of cybersecurity tasks in memory AND in a MySQL database.
// Handles Add, View, Complete, and Delete operations.
// Written by: Lindokuhle Mtshali
// Student Number: ST10233093

using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CybersecurityAwarenessBot.Tasks
{
    public class TaskManager
    {
        // In-memory list - primary data store for the session
        private readonly List<CybersecurityTask> _tasks;
        private int _nextId = 1;

        // MySQL connection string - update with your credentials
        private const string ConnectionString =
            "Server=localhost;Database=cyberguard_db;Uid=root;Pwd=oracle123;";

        private bool _dbAvailable = false;

        public TaskManager()
        {
            _tasks = new List<CybersecurityTask>();
            TryInitialiseDatabase();
        }

        // ── Database Setup ───────────────────────────────────────────────────────

        private void TryInitialiseDatabase()
        {
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    _dbAvailable = true;
                    LoadTasksFromDatabase(conn);
                }
            }
            catch
            {
                // Database unavailable - app runs with in-memory tasks only
                _dbAvailable = false;
            }
        }

        private void LoadTasksFromDatabase(MySqlConnection conn)
        {
            string sql = "SELECT id, title, description, reminder, is_completed, created_at FROM tasks ORDER BY id;";
            using (var cmd = new MySqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var task = new CybersecurityTask
                    {
                        Id = reader.GetInt32("id"),
                        Title = reader.GetString("title"),
                        Description = reader.GetString("description"),
                        Reminder = reader.IsDBNull(reader.GetOrdinal("reminder")) ? "" : reader.GetString("reminder"),
                        IsCompleted = reader.GetBoolean("is_completed"),
                        CreatedAt = reader.GetDateTime("created_at")
                    };
                    _tasks.Add(task);
                    if (task.Id >= _nextId) _nextId = task.Id + 1;
                }
            }
        }

        // ── Public Methods ───────────────────────────────────────────────────────

        /// <summary>Adds a new task. Saves to DB if available.</summary>
        public CybersecurityTask AddTask(string title, string description, string reminder = "")
        {
            var task = new CybersecurityTask(_nextId++, title, description, reminder);
            _tasks.Add(task);

            if (_dbAvailable)
            {
                try
                {
                    using (var conn = new MySqlConnection(ConnectionString))
                    {
                        conn.Open();
                        string sql = "INSERT INTO tasks (id, title, description, reminder, is_completed, created_at) " +
                                     "VALUES (@id, @title, @desc, @reminder, @done, @created);";
                        using (var cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", task.Id);
                            cmd.Parameters.AddWithValue("@title", task.Title);
                            cmd.Parameters.AddWithValue("@desc", task.Description);
                            cmd.Parameters.AddWithValue("@reminder", task.Reminder);
                            cmd.Parameters.AddWithValue("@done", task.IsCompleted);
                            cmd.Parameters.AddWithValue("@created", task.CreatedAt);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch { /* DB write failed silently; in-memory task still exists */ }
            }

            return task;
        }

        /// <summary>Returns all tasks (pending and completed).</summary>
        public List<CybersecurityTask> GetAllTasks() => _tasks;

        /// <summary>Returns only pending (incomplete) tasks.</summary>
        public List<CybersecurityTask> GetPendingTasks()
        {
            return _tasks.FindAll(t => !t.IsCompleted);
        }

        /// <summary>Marks a task as completed by ID. Updates DB if available.</summary>
        public bool MarkCompleted(int id)
        {
            var task = _tasks.Find(t => t.Id == id);
            if (task == null) return false;

            task.IsCompleted = true;

            if (_dbAvailable)
            {
                try
                {
                    using (var conn = new MySqlConnection(ConnectionString))
                    {
                        conn.Open();
                        string sql = "UPDATE tasks SET is_completed = TRUE WHERE id = @id;";
                        using (var cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch { }
            }

            return true;
        }

        /// <summary>Deletes a task by ID. Removes from DB if available.</summary>
        public bool DeleteTask(int id)
        {
            var task = _tasks.Find(t => t.Id == id);
            if (task == null) return false;

            _tasks.Remove(task);

            if (_dbAvailable)
            {
                try
                {
                    using (var conn = new MySqlConnection(ConnectionString))
                    {
                        conn.Open();
                        string sql = "DELETE FROM tasks WHERE id = @id;";
                        using (var cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch { }
            }

            return true;
        }

        /// <summary>Returns a formatted string summary of all tasks for display in chat.</summary>
        public string GetTaskSummary()
        {
            if (_tasks.Count == 0)
                return "You have no tasks yet. Say 'add task' to create one!";

            string result = "Your cybersecurity tasks:\n";
            foreach (var t in _tasks)
            {
                string status = t.IsCompleted ? "[DONE]   " : "[PENDING]";
                string reminder = string.IsNullOrEmpty(t.Reminder) ? "" : $" | Reminder: {t.Reminder}";
                result += $"  #{t.Id} {status} {t.Title}{reminder}\n";
                result += $"         {t.Description}\n";
            }

            string dbStatus = _dbAvailable ? " (synced with database)" : " (in-memory only - database offline)";
            result += $"\n{dbStatus}";
            return result;
        }

        public bool IsDatabaseAvailable => _dbAvailable;
    }
}