// ActivityLog.cs
// Records all significant actions the chatbot takes during a session.
// Users can view the log by typing 'show activity log' or 'what have you done for me?'
// Displays the last 5-10 actions with timestamps and descriptions.
// Written by: Lindokuhle Mtshali
// Student Number: ST10233093

using System;
using System.Collections.Generic;
using System.Text;

namespace CybersecurityAwarenessBot.Logging
{
    public class ActivityLog
    {
        // ── Activity Entry Model ─────────────────────────────────────────────────
        public class LogEntry
        {
            public DateTime Timestamp { get; set; }
            public string Category { get; set; }    // e.g. "Task", "Reminder", "Quiz", "NLP"
            public string Description { get; set; }

            public LogEntry(string category, string description)
            {
                Timestamp = DateTime.Now;
                Category = category;
                Description = description;
            }

            public override string ToString()
            {
                return $"[{Timestamp:HH:mm:ss}] {Category}: {Description}";
            }
        }

        // ── Fields ───────────────────────────────────────────────────────────────
        // Using a List<LogEntry> as the generic collection to store the full log
        private readonly List<LogEntry> _fullLog;

        // Maximum entries to display at once (rubric says 5-10)
        private const int DisplayLimit = 10;

        public ActivityLog()
        {
            _fullLog = new List<LogEntry>();
        }

        // ── Public Methods ───────────────────────────────────────────────────────

        /// <summary>Logs a task being added.</summary>
        public void LogTaskAdded(string taskTitle, string reminder = "")
        {
            string desc = $"Task added: '{taskTitle}'";
            if (!string.IsNullOrEmpty(reminder))
                desc += $" [{reminder}]";
            Add("Task", desc);
        }

        /// <summary>Logs a task being marked as completed.</summary>
        public void LogTaskCompleted(int taskId, string taskTitle)
        {
            Add("Task", $"Task #{taskId} marked as completed: '{taskTitle}'");
        }

        /// <summary>Logs a task being deleted.</summary>
        public void LogTaskDeleted(int taskId, string taskTitle)
        {
            Add("Task", $"Task #{taskId} deleted: '{taskTitle}'");
        }

        /// <summary>Logs a reminder being set.</summary>
        public void LogReminderSet(string taskTitle, string reminder)
        {
            Add("Reminder", $"Reminder set for '{taskTitle}': {reminder}");
        }

        /// <summary>Logs the quiz starting.</summary>
        public void LogQuizStarted()
        {
            Add("Quiz", "Cybersecurity quiz started");
        }

        /// <summary>Logs the quiz finishing with a score.</summary>
        public void LogQuizCompleted(int score, int total)
        {
            Add("Quiz", $"Quiz completed - Score: {score}/{total} ({(double)score / total * 100:0}%)");
        }

        /// <summary>Logs an NLP-recognised action.</summary>
        public void LogNlpAction(string recognisedIntent, string originalInput)
        {
            Add("NLP", $"Intent '{recognisedIntent}' recognised from: \"{TruncateInput(originalInput)}\"");
        }

        /// <summary>Logs a general chat action.</summary>
        public void LogChatAction(string description)
        {
            Add("Chat", description);
        }

        /// <summary>
        /// Returns the last 5-10 log entries formatted for display in chat.
        /// </summary>
        public string GetDisplayLog(bool showAll = false)
        {
            if (_fullLog.Count == 0)
                return "No actions have been recorded yet. Start chatting to build up your activity log!";

            int count = showAll ? _fullLog.Count : Math.Min(DisplayLimit, _fullLog.Count);
            int startIndex = _fullLog.Count - count;

            var sb = new StringBuilder();
            sb.AppendLine("Here is a summary of recent actions:");
            sb.AppendLine();

            for (int i = startIndex; i < _fullLog.Count; i++)
            {
                int displayNum = i - startIndex + 1;
                sb.AppendLine($"  {displayNum}. {_fullLog[i]}");
            }

            if (!showAll && _fullLog.Count > DisplayLimit)
            {
                sb.AppendLine();
                sb.AppendLine($"  (Showing last {DisplayLimit} of {_fullLog.Count} total actions)");
                sb.AppendLine("  Say 'show full log' to see everything.");
            }

            return sb.ToString().TrimEnd();
        }

        /// <summary>Returns total number of logged entries.</summary>
        public int TotalEntries => _fullLog.Count;

        // ── Private Helpers ──────────────────────────────────────────────────────

        private void Add(string category, string description)
        {
            _fullLog.Add(new LogEntry(category, description));
        }

        private string TruncateInput(string input)
        {
            if (input.Length <= 40) return input;
            return input.Substring(0, 37) + "...";
        }
    }
}