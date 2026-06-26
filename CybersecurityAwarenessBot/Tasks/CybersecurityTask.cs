// CybersecurityTask.cs
// Model class that represents a single cybersecurity task created by the user.
// Each task has a title, description, optional reminder, and a completion status.
// Written by: Lindokuhle Mtshali
// Student Number: ST10233093

using System;

namespace CybersecurityAwarenessBot.Tasks
{
    public class CybersecurityTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Reminder { get; set; }        // e.g. "Remind me in 3 days" or a date string
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public CybersecurityTask()
        {
            Title = string.Empty;
            Description = string.Empty;
            Reminder = string.Empty;
            IsCompleted = false;
            CreatedAt = DateTime.Now;
        }

        public CybersecurityTask(int id, string title, string description, string reminder = "")
        {
            Id = id;
            Title = title;
            Description = description;
            Reminder = reminder;
            IsCompleted = false;
            CreatedAt = DateTime.Now;
        }

        public override string ToString()
        {
            string status = IsCompleted ? "[DONE]" : "[PENDING]";
            string reminderText = string.IsNullOrEmpty(Reminder) ? "No reminder" : Reminder;
            return $"{status} #{Id}: {Title} | Reminder: {reminderText}";
        }
    }
}