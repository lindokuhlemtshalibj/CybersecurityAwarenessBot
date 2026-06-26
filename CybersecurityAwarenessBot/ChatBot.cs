// ChatBot.cs  (UPDATED FOR PART 3)
// Main controller - now integrates Task Manager, Quiz Engine, NLP Handler,
// and Activity Log alongside the existing Part 1 and Part 2 features.
// REPLACE your existing ChatBot.cs with this file.
// Written by: Lindokuhle Mtshali
// Student Number: ST10233093

using System;
using CybersecurityAwarenessBot.Memory;
using CybersecurityAwarenessBot.Responses;
using CybersecurityAwarenessBot.Tasks;
using CybersecurityAwarenessBot.Quiz;
using CybersecurityAwarenessBot.NLP;
using CybersecurityAwarenessBot.Logging;

namespace CybersecurityAwarenessBot
{
    public class ChatBot
    {
        // ── Fields ───────────────────────────────────────────────────────────────
        private readonly UserMemory _memory;
        private readonly ResponseHandler _responseHandler;
        private readonly TaskManager _taskManager;
        private readonly QuizEngine _quizEngine;
        private readonly NlpHandler _nlpHandler;
        private readonly ActivityLog _activityLog;

        private bool _nameHasBeenSet = false;

        // Tracks whether we are currently waiting for reminder input after adding a task
        private bool _waitingForReminder = false;
        private string _pendingTaskTitle = "";
        private string _pendingTaskDescription = "";

        // ── Constructor ──────────────────────────────────────────────────────────
        public ChatBot()
        {
            _memory = new UserMemory();
            _responseHandler = new ResponseHandler(_memory);
            _taskManager = new TaskManager();
            _quizEngine = new QuizEngine();
            _nlpHandler = new NlpHandler();
            _activityLog = new ActivityLog();
        }

        // ── Public Properties ────────────────────────────────────────────────────
        public string UserName
        {
            get
            {
                string name = _memory.GetName();
                return string.IsNullOrEmpty(name) ? string.Empty : name;
            }
        }

        public bool IsNameSet => _nameHasBeenSet;
        public bool IsDbAvailable => _taskManager.IsDatabaseAvailable;

        // ── Public Methods ───────────────────────────────────────────────────────

        public string SetUserName(string nameInput)
        {
            if (!InputValidator.IsValidName(nameInput))
                return "Warning: Please enter a valid name (letters only, no numbers or symbols).";

            string cleaned = nameInput.Trim();
            cleaned = char.ToUpper(cleaned[0]) + cleaned.Substring(1).ToLower();
            _memory.Remember("name", cleaned);
            _nameHasBeenSet = true;

            _activityLog.LogChatAction($"Session started for user: {cleaned}");

            return $"Welcome aboard, {cleaned}!\n\n" +
                   $"I'm CyberGuard - your personal cybersecurity guide.\n" +
                   $"Together we'll keep you safe in South Africa's digital landscape.\n\n" +
                   $"NEW in Part 3 - you can now:\n" +
                   $"  - Add and manage cybersecurity tasks (stored in MySQL)\n" +
                   $"  - Play the cybersecurity quiz\n" +
                   $"  - View your activity log\n\n" +
                   $"Type 'help' to see all topics. Type 'exit' to end our session.\n" +
                   $"Database status: {(IsDbAvailable ? "Connected" : "Offline (running in-memory)")}";
        }

        public string ProcessMessage(string userInput)
        {
            // Validate input (Part 1 feature retained)
            if (!InputValidator.IsValidInput(userInput))
                return "I didn't quite get that. Could you rephrase?";

            // Handle exit (Part 1 feature retained)
            if (InputValidator.IsExitCommand(userInput))
            {
                string name = _memory.GetName();
                string displayName = string.IsNullOrEmpty(name) ? "there" : name;
                _activityLog.LogChatAction("Session ended by user");
                return $"GOODBYE|Goodbye, {displayName}! Stay cyber-safe out there.\nRemember: Think before you click!";
            }

            string userName = UserName.Length > 0 ? UserName : "there";

            // ── If quiz is active, route answer to quiz engine ────────────────────
            if (_quizEngine.IsActive)
            {
                string quizResult = _quizEngine.SubmitAnswer(userInput);

                // Log quiz completion if it just ended
                if (!_quizEngine.IsActive)
                    _activityLog.LogQuizCompleted(_quizEngine.Score, _quizEngine.TotalQuestions);

                return quizResult;
            }

            // ── If waiting for reminder confirmation ──────────────────────────────
            if (_waitingForReminder)
            {
                return HandleReminderFollowUp(userInput, userName);
            }

            // ── Detect intent with NLP ────────────────────────────────────────────
            NlpHandler.Intent intent = _nlpHandler.DetectIntent(userInput);
            _activityLog.LogNlpAction(intent.ToString(), userInput);

            switch (intent)
            {
                case NlpHandler.Intent.AddTask:
                    return HandleAddTask(userInput, userName);

                case NlpHandler.Intent.ViewTasks:
                    return _taskManager.GetTaskSummary();

                case NlpHandler.Intent.CompleteTask:
                    return HandleCompleteTask(userInput, userName);

                case NlpHandler.Intent.DeleteTask:
                    return HandleDeleteTask(userInput, userName);

                case NlpHandler.Intent.StartQuiz:
                    _activityLog.LogQuizStarted();
                    return _quizEngine.StartQuiz();

                case NlpHandler.Intent.ShowActivityLog:
                    bool showAll = userInput.ToLower().Contains("full");
                    return _activityLog.GetDisplayLog(showAll);

                case NlpHandler.Intent.SetReminder:
                    return HandleStandaloneReminder(userInput, userName);

                case NlpHandler.Intent.Greeting:
                    return $"Hello again, {userName}! How can I help you today?\n" +
                           "Type 'help' for topics, 'add task' to manage tasks, or 'start quiz' for a challenge!";

                default:
                    // Fall through to Part 1+2 response handler (all original features preserved)
                    return _responseHandler.GetResponse(userInput);
            }
        }

        public void Reset()
        {
            _memory.Clear();
            _nameHasBeenSet = false;
            _waitingForReminder = false;
        }

        // ── Private Task Handlers ────────────────────────────────────────────────

        private string HandleAddTask(string userInput, string userName)
        {
            string title = _nlpHandler.ExtractTaskTitle(userInput);
            string reminder = _nlpHandler.ExtractReminder(userInput);

            if (string.IsNullOrEmpty(title))
            {
                // Ask user to specify title
                _waitingForReminder = false;
                return $"What task would you like to add, {userName}?\n" +
                       "Example: 'Add task Enable two-factor authentication'";
            }

            // Build a default description from the title
            string description = GenerateTaskDescription(title);

            if (string.IsNullOrEmpty(reminder))
            {
                // Task added - ask if they want a reminder
                _pendingTaskTitle = title;
                _pendingTaskDescription = description;
                _waitingForReminder = true;

                return $"Task added with the description \"{description}\"\n" +
                       $"Would you like to set a reminder? (e.g. 'Yes, remind me in 3 days' or 'No')";
            }
            else
            {
                // Reminder already in the message - add straight away
                var task = _taskManager.AddTask(title, description, reminder);
                _activityLog.LogTaskAdded(task.Title, reminder);
                _activityLog.LogReminderSet(task.Title, reminder);

                return $"Task added: '{task.Title}'\nDescription: {task.Description}\nReminder: {reminder}\n\n" +
                       $"Say 'view tasks' to see all your tasks.";
            }
        }

        private string HandleReminderFollowUp(string userInput, string userName)
        {
            _waitingForReminder = false;
            string lower = userInput.ToLower().Trim();

            // User said no
            if (lower == "no" || lower == "n" || lower.Contains("no reminder") || lower.Contains("no thanks"))
            {
                var task = _taskManager.AddTask(_pendingTaskTitle, _pendingTaskDescription, "");
                _activityLog.LogTaskAdded(task.Title);
                _pendingTaskTitle = "";
                _pendingTaskDescription = "";
                return $"Got it! Task '{task.Title}' saved without a reminder.\nSay 'view tasks' to see all your tasks.";
            }

            // Extract reminder from user's reply
            string reminder = _nlpHandler.ExtractReminder(userInput);
            if (string.IsNullOrEmpty(reminder))
            {
                // Try reading it directly (e.g. "in 7 days" or "tomorrow")
                reminder = userInput.Trim().TrimEnd('.', '!', '?');
            }

            var savedTask = _taskManager.AddTask(_pendingTaskTitle, _pendingTaskDescription, reminder);
            _activityLog.LogTaskAdded(savedTask.Title, reminder);
            _activityLog.LogReminderSet(savedTask.Title, reminder);

            _pendingTaskTitle = "";
            _pendingTaskDescription = "";

            return $"Got it! I'll remind you {reminder.ToLower().Replace("remind me ", "")}.\n" +
                   $"Task '{savedTask.Title}' saved.";
        }

        private string HandleCompleteTask(string userInput, string userName)
        {
            int id = _nlpHandler.ExtractTaskId(userInput);
            if (id == -1)
                return $"Which task would you like to mark as done, {userName}? " +
                       "Please include the task number, e.g. 'complete task 2'.";

            var tasks = _taskManager.GetAllTasks();
            var task = tasks.Find(t => t.Id == id);

            if (task == null)
                return $"I couldn't find task #{id}. Say 'view tasks' to see your current tasks.";

            _taskManager.MarkCompleted(id);
            _activityLog.LogTaskCompleted(id, task.Title);

            return $"Task #{id} '{task.Title}' has been marked as completed!";
        }

        private string HandleDeleteTask(string userInput, string userName)
        {
            int id = _nlpHandler.ExtractTaskId(userInput);
            if (id == -1)
                return $"Which task would you like to delete, {userName}? " +
                       "Please include the task number, e.g. 'delete task 3'.";

            var tasks = _taskManager.GetAllTasks();
            var task = tasks.Find(t => t.Id == id);

            if (task == null)
                return $"I couldn't find task #{id}. Say 'view tasks' to see your current tasks.";

            _taskManager.DeleteTask(id);
            _activityLog.LogTaskDeleted(id, task.Title);

            return $"Task #{id} '{task.Title}' has been deleted.";
        }

        private string HandleStandaloneReminder(string userInput, string userName)
        {
            string reminder = _nlpHandler.ExtractReminder(userInput);
            string taskTitle = _nlpHandler.ExtractTaskTitle(userInput);

            if (!string.IsNullOrEmpty(taskTitle))
            {
                string description = GenerateTaskDescription(taskTitle);
                var task = _taskManager.AddTask(taskTitle, description, reminder);
                _activityLog.LogTaskAdded(task.Title, reminder);
                if (!string.IsNullOrEmpty(reminder))
                    _activityLog.LogReminderSet(task.Title, reminder);

                string reminderText = string.IsNullOrEmpty(reminder) ? "no reminder set" : reminder;
                return $"Task added: '{task.Title}' ({reminderText})";
            }

            if (!string.IsNullOrEmpty(reminder))
                return $"Got it, {userName}! Reminder noted: {reminder}.\n" +
                       "If you'd like to link this to a task, say 'add task [task name]'.";

            return $"What would you like me to remind you about, {userName}?\n" +
                   "Example: 'Remind me to update my password in 3 days'";
        }

        // ── Utility ──────────────────────────────────────────────────────────────

        private string GenerateTaskDescription(string title)
        {
            string lower = title.ToLower();

            if (lower.Contains("2fa") || lower.Contains("two-factor") || lower.Contains("two factor"))
                return "Set up two-factor authentication to add an extra layer of security to your account.";
            if (lower.Contains("password"))
                return "Update and strengthen your password using a unique passphrase of at least 12 characters.";
            if (lower.Contains("privacy") || lower.Contains("settings"))
                return "Review your account privacy settings to ensure your data is protected.";
            if (lower.Contains("antivirus") || lower.Contains("malware"))
                return "Install or update antivirus software to protect against malware and viruses.";
            if (lower.Contains("backup"))
                return "Back up your important files to an external drive or cloud storage.";
            if (lower.Contains("update") || lower.Contains("patch"))
                return "Apply the latest software updates and security patches to your device.";
            if (lower.Contains("vpn"))
                return "Set up a VPN for secure browsing on public Wi-Fi networks.";

            return $"Complete the cybersecurity task: {title}.";
        }
    }
}