// NlpHandler.cs
// Simulates Natural Language Processing using string manipulation and keyword detection.
// Recognises user intent even when phrased in different ways.
// For example: "set a reminder to check my settings" is recognised as a reminder request.
// Written by: Lindokuhle Mtshali
// Student Number: ST10233093

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CybersecurityAwarenessBot.NLP
{
    public class NlpHandler
    {
        // ── Intent Enum ──────────────────────────────────────────────────────────
        public enum Intent
        {
            Unknown,
            AddTask,
            ViewTasks,
            CompleteTask,
            DeleteTask,
            StartQuiz,
            SetReminder,
            ShowActivityLog,
            AskCybersecurity,
            Greeting,
            Exit
        }

        // ── Keyword Maps ─────────────────────────────────────────────────────────
        // Each intent maps to a list of phrase variations the user might type.
        private readonly Dictionary<Intent, List<string>> _intentKeywords;

        public NlpHandler()
        {
            _intentKeywords = BuildIntentKeywords();
        }

        // ── Public Methods ───────────────────────────────────────────────────────

        /// <summary>
        /// Detects the intent of the user's message using keyword matching.
        /// Returns the best-matched Intent enum value.
        /// </summary>
        public Intent DetectIntent(string userInput)
        {
            string lower = userInput.ToLower().Trim();

            foreach (var entry in _intentKeywords)
            {
                foreach (string keyword in entry.Value)
                {
                    if (lower.Contains(keyword))
                        return entry.Key;
                }
            }

            return Intent.Unknown;
        }

        /// <summary>
        /// Extracts the task title from a user message like "add task enable 2fa".
        /// Returns the extracted title or an empty string if nothing found.
        /// </summary>
        public string ExtractTaskTitle(string userInput)
        {
            string lower = userInput.ToLower();

            string[] prefixes = {
                "add task ", "create task ", "new task ", "add a task to ", "add a task ",
                "i need to ", "remind me to ", "create a task for ", "set task "
            };

            foreach (string prefix in prefixes)
            {
                if (lower.Contains(prefix))
                {
                    int start = lower.IndexOf(prefix) + prefix.Length;
                    string extracted = userInput.Substring(start).Trim().TrimEnd('.', '!', '?');
                    if (extracted.Length > 1) return CapitaliseFirst(extracted);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Extracts a reminder phrase from user input like "remind me in 3 days" or "remind me tomorrow".
        /// Returns the reminder string or empty if not found.
        /// </summary>
        public string ExtractReminder(string userInput)
        {
            string lower = userInput.ToLower();

            // Pattern: "remind me in X days/weeks"
            var match = Regex.Match(lower, @"remind me in (\d+\s*(day|week|hour)s?)");
            if (match.Success)
                return CapitaliseFirst(match.Value);

            // Pattern: "remind me tomorrow", "remind me on Monday"
            match = Regex.Match(lower, @"remind me (tomorrow|on \w+day|next \w+)");
            if (match.Success)
                return CapitaliseFirst(match.Value);

            // Pattern: "remind me in X days" anywhere in string
            match = Regex.Match(lower, @"in (\d+) (day|week)s?");
            if (match.Success)
                return $"Remind me {match.Value}";

            if (lower.Contains("tomorrow"))
                return "Remind me tomorrow";

            return string.Empty;
        }

        /// <summary>
        /// Extracts a task ID number from input like "complete task 3" or "delete #2".
        /// Returns -1 if no number found.
        /// </summary>
        public int ExtractTaskId(string userInput)
        {
            var match = Regex.Match(userInput, @"\d+");
            if (match.Success && int.TryParse(match.Value, out int id))
                return id;
            return -1;
        }

        /// <summary>
        /// Returns a friendly message when the bot doesn't understand.
        /// Keeps this to a minimum as per rubric - should fire rarely.
        /// </summary>
        public string GetFallbackResponse(string userName)
        {
            return $"I didn't quite understand that, {userName}. Could you rephrase?\n" +
                   "You can try:\n" +
                   "  'add task Enable 2FA'\n" +
                   "  'view my tasks'\n" +
                   "  'start quiz'\n" +
                   "  'show activity log'\n" +
                   "  or ask me about any cybersecurity topic.";
        }

        // ── Private Helpers ──────────────────────────────────────────────────────

        private string CapitaliseFirst(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return char.ToUpper(text[0]) + text.Substring(1);
        }

        private Dictionary<Intent, List<string>> BuildIntentKeywords()
        {
            return new Dictionary<Intent, List<string>>
            {
                {
                    Intent.AddTask, new List<string>
                    {
                        "add task", "create task", "new task", "add a task",
                        "i need to do", "create a reminder", "set up a task",
                        "add reminder", "set task", "create a task"
                    }
                },
                {
                    Intent.ViewTasks, new List<string>
                    {
                        "view tasks", "show tasks", "list tasks", "my tasks",
                        "what are my tasks", "view my tasks", "show my tasks",
                        "display tasks", "see my tasks", "check my tasks"
                    }
                },
                {
                    Intent.CompleteTask, new List<string>
                    {
                        "complete task", "mark task", "done task", "finish task",
                        "mark as done", "task done", "completed task", "mark complete",
                        "i completed", "i finished task"
                    }
                },
                {
                    Intent.DeleteTask, new List<string>
                    {
                        "delete task", "remove task", "cancel task", "drop task",
                        "get rid of task", "erase task"
                    }
                },
                {
                    Intent.StartQuiz, new List<string>
                    {
                        "start quiz", "begin quiz", "play quiz", "quiz me",
                        "test my knowledge", "take a quiz", "cybersecurity quiz",
                        "start the quiz", "i want to do the quiz", "let's quiz"
                    }
                },
                {
                    Intent.SetReminder, new List<string>
                    {
                        "remind me", "set a reminder", "set reminder",
                        "remind me to", "set me a reminder"
                    }
                },
                {
                    Intent.ShowActivityLog, new List<string>
                    {
                        "show activity log", "show log", "activity log",
                        "what have you done", "what have you done for me",
                        "show recent actions", "view log", "show history",
                        "what did you do", "recent actions", "bot history",
                        "show me the log"
                    }
                },
                {
                    Intent.Greeting, new List<string>
                    {
                        "hello", "hi", "hey", "good morning", "good afternoon",
                        "good evening", "howzit", "sup"
                    }
                },
                {
                    Intent.Exit, new List<string>
                    {
                        "exit", "quit", "bye", "goodbye", "stop", "close"
                    }
                }
            };
        }
    }
}