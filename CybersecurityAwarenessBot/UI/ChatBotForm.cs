// ChatBotForm.cs  (UPDATED FOR PART 3)
// Main GUI window - now includes:
//   - Task panel in the side bar showing task count and quick actions
//   - Quiz mode indicator in the status bar
//   - Activity log accessible via button or by typing 'show activity log'
//   - DB status indicator
// REPLACE your existing UI/ChatBotForm.cs with this file.
// Written by: Lindokuhle Mtshali
// Student Number: ST10233093

using System;
using System.Drawing;
using System.Windows.Forms;
using CybersecurityAwarenessBot.Sentiment;

namespace CybersecurityAwarenessBot.UI
{
    public class ChatBotForm : Form
    {
        private readonly ChatBot _chatBot;
        private readonly SentimentDetector _sentimentDetector;

        // ── UI Controls ──────────────────────────────────────────────────────────
        private RichTextBox chatDisplay;
        private TextBox inputBox;
        private Button sendButton;
        private Button clearButton;
        private Button taskButton;
        private Button quizButton;
        private Button logButton;
        private Label statusLabel;
        private Label memoryLabel;
        private Label dbStatusLabel;
        private Label taskCountLabel;

        // ── Colour Palette (same as Part 2) ─────────────────────────────────────
        private Color bgDark = Color.FromArgb(13, 17, 23);
        private Color bgMid = Color.FromArgb(22, 27, 34);
        private Color bgInput = Color.FromArgb(33, 38, 45);
        private Color accentCyan = Color.FromArgb(0, 210, 211);
        private Color accentGreen = Color.FromArgb(63, 185, 80);
        private Color accentYellow = Color.FromArgb(230, 162, 44);
        private Color accentRed = Color.FromArgb(248, 81, 73);
        private Color accentPurple = Color.FromArgb(139, 92, 246);
        private Color textLight = Color.FromArgb(201, 209, 217);
        private Color textDim = Color.FromArgb(110, 118, 129);
        private Color borderColor = Color.FromArgb(48, 54, 61);

        // ── Constructor ──────────────────────────────────────────────────────────
        public ChatBotForm()
        {
            _chatBot = new ChatBot();
            _sentimentDetector = new SentimentDetector();
            this.Text = "CyberGuard - Cybersecurity Awareness Bot";
            this.Size = new Size(1100, 720);
            this.MinimumSize = new Size(900, 600);
            this.BackColor = bgDark;
            this.Font = new Font("Consolas", 9.5f);
            this.StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
            this.Load += (s, e) => ShowGreeting();
        }

        // ── Build UI ─────────────────────────────────────────────────────────────
        private void BuildUI()
        {
            // ── Header ───────────────────────────────────────────────────────────
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 85;
            headerPanel.BackColor = bgMid;

            Label titleLabel = new Label();
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Font = new Font("Consolas", 7f);
            titleLabel.ForeColor = accentCyan;
            titleLabel.Text =
                "  ######  ##  ## ######  ######  ######   ######  ##  ## ######  ######  ###### \n" +
                "  ##      ##  ## ##  ##  ##      ##   ##  ##      ##  ## ##  ##  ##  ##  ##  ## \n" +
                "  ##      ##  ## ######  ####    ######   ##  ### ##  ## ######  ##  ##  ##  ## \n" +
                "  ##       ####  ##  ##  ##      ##  ##   ##   ## ##  ## ##  ##  ##  ##  ##  ## \n" +
                "  ######    ##   ######  ######  ##   ##   ######  ####  ######  ######  ###### \n" +
                "                  Cybersecurity Awareness Bot  -  South Africa  [Part 3]         ";
            headerPanel.Controls.Add(titleLabel);

            Panel headerLine = new Panel();
            headerLine.Dock = DockStyle.Top;
            headerLine.Height = 2;
            headerLine.BackColor = accentCyan;

            // ── Side Panel ───────────────────────────────────────────────────────
            Panel sidePanel = new Panel();
            sidePanel.Dock = DockStyle.Right;
            sidePanel.Width = 220;
            sidePanel.BackColor = bgMid;
            sidePanel.Padding = new Padding(8);

            Label sideTitle = new Label();
            sideTitle.Text = "SESSION MEMORY";
            sideTitle.Font = new Font("Consolas", 8.5f, FontStyle.Bold);
            sideTitle.ForeColor = accentCyan;
            sideTitle.Dock = DockStyle.Top;
            sideTitle.Height = 28;
            sideTitle.TextAlign = ContentAlignment.MiddleLeft;

            Panel sideDivider1 = new Panel();
            sideDivider1.Dock = DockStyle.Top;
            sideDivider1.Height = 1;
            sideDivider1.BackColor = borderColor;

            memoryLabel = new Label();
            memoryLabel.Text = "No information stored yet.\n\nTell me your name or\nfavourite topic and\nI will remember it.";
            memoryLabel.Font = new Font("Consolas", 8f);
            memoryLabel.ForeColor = textDim;
            memoryLabel.Dock = DockStyle.Top;
            memoryLabel.Height = 90;
            memoryLabel.TextAlign = ContentAlignment.TopLeft;
            memoryLabel.Padding = new Padding(0, 6, 0, 0);

            Panel sideDivider2 = new Panel();
            sideDivider2.Dock = DockStyle.Top;
            sideDivider2.Height = 1;
            sideDivider2.BackColor = borderColor;

            Label taskSectionLabel = new Label();
            taskSectionLabel.Text = "TASKS";
            taskSectionLabel.Font = new Font("Consolas", 8.5f, FontStyle.Bold);
            taskSectionLabel.ForeColor = accentYellow;
            taskSectionLabel.Dock = DockStyle.Top;
            taskSectionLabel.Height = 24;
            taskSectionLabel.TextAlign = ContentAlignment.MiddleLeft;

            taskCountLabel = new Label();
            taskCountLabel.Text = "No tasks yet.";
            taskCountLabel.Font = new Font("Consolas", 8f);
            taskCountLabel.ForeColor = textDim;
            taskCountLabel.Dock = DockStyle.Top;
            taskCountLabel.Height = 30;
            taskCountLabel.TextAlign = ContentAlignment.TopLeft;

            dbStatusLabel = new Label();
            dbStatusLabel.Text = "DB: Checking...";
            dbStatusLabel.Font = new Font("Consolas", 7.5f);
            dbStatusLabel.ForeColor = textDim;
            dbStatusLabel.Dock = DockStyle.Top;
            dbStatusLabel.Height = 20;

            Label tipsLabel = new Label();
            tipsLabel.Text =
                "QUICK COMMANDS\n" +
                "---------------------\n" +
                "'add task [name]'\n" +
                "'view tasks'\n" +
                "'complete task 1'\n" +
                "'start quiz'\n" +
                "'show activity log'\n" +
                "'tell me more'\n" +
                "'help'\n" +
                "'exit'";
            tipsLabel.Font = new Font("Consolas", 7.8f);
            tipsLabel.ForeColor = textDim;
            tipsLabel.Dock = DockStyle.Bottom;
            tipsLabel.Height = 180;
            tipsLabel.TextAlign = ContentAlignment.TopLeft;

            sidePanel.Controls.Add(tipsLabel);
            sidePanel.Controls.Add(dbStatusLabel);
            sidePanel.Controls.Add(taskCountLabel);
            sidePanel.Controls.Add(taskSectionLabel);
            sidePanel.Controls.Add(sideDivider2);
            sidePanel.Controls.Add(memoryLabel);
            sidePanel.Controls.Add(sideDivider1);
            sidePanel.Controls.Add(sideTitle);

            Panel sideBorder = new Panel();
            sideBorder.Dock = DockStyle.Right;
            sideBorder.Width = 1;
            sideBorder.BackColor = borderColor;

            // ── Quick-Action Button Row ──────────────────────────────────────────
            Panel quickPanel = new Panel();
            quickPanel.Dock = DockStyle.Bottom;
            quickPanel.Height = 38;
            quickPanel.BackColor = bgMid;
            quickPanel.Padding = new Padding(10, 4, 10, 4);

            Label quickLabel = new Label();
            quickLabel.Text = "Quick:";
            quickLabel.Font = new Font("Consolas", 8.5f);
            quickLabel.ForeColor = textDim;
            quickLabel.AutoSize = true;
            quickLabel.Location = new Point(10, 10);

            taskButton = MakeQuickButton("Add Task", accentYellow, bgDark, new Point(75, 4));
            taskButton.Click += (s, e) => InjectCommand("add task ");

            quizButton = MakeQuickButton("Start Quiz", accentPurple, Color.White, new Point(185, 4));
            quizButton.Click += (s, e) => InjectCommand("start quiz");

            logButton = MakeQuickButton("Activity Log", accentGreen, bgDark, new Point(300, 4));
            logButton.Click += (s, e) => InjectCommand("show activity log");

            Button viewTasksBtn = MakeQuickButton("View Tasks", accentCyan, bgDark, new Point(425, 4));
            viewTasksBtn.Click += (s, e) => InjectCommand("view tasks");

            quickPanel.Controls.Add(quickLabel);
            quickPanel.Controls.Add(taskButton);
            quickPanel.Controls.Add(quizButton);
            quickPanel.Controls.Add(logButton);
            quickPanel.Controls.Add(viewTasksBtn);

            // ── Input Panel ──────────────────────────────────────────────────────
            Panel inputPanel = new Panel();
            inputPanel.Dock = DockStyle.Bottom;
            inputPanel.Height = 85;
            inputPanel.BackColor = bgMid;
            inputPanel.Padding = new Padding(10, 8, 10, 8);

            statusLabel = new Label();
            statusLabel.Text = "Ready - Ask me anything about cybersecurity!";
            statusLabel.Font = new Font("Consolas", 8.5f);
            statusLabel.ForeColor = textDim;
            statusLabel.Dock = DockStyle.Bottom;
            statusLabel.Height = 20;
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;

            sendButton = new Button();
            sendButton.Text = "Send";
            sendButton.Dock = DockStyle.Right;
            sendButton.Width = 90;
            sendButton.BackColor = accentCyan;
            sendButton.ForeColor = bgDark;
            sendButton.FlatStyle = FlatStyle.Flat;
            sendButton.Font = new Font("Consolas", 9.5f, FontStyle.Bold);
            sendButton.Cursor = Cursors.Hand;
            sendButton.FlatAppearance.BorderSize = 0;
            sendButton.Click += (s, e) => SendMessage();

            clearButton = new Button();
            clearButton.Text = "Clear";
            clearButton.Dock = DockStyle.Right;
            clearButton.Width = 65;
            clearButton.BackColor = bgInput;
            clearButton.ForeColor = textDim;
            clearButton.FlatStyle = FlatStyle.Flat;
            clearButton.Font = new Font("Consolas", 9f);
            clearButton.Cursor = Cursors.Hand;
            clearButton.FlatAppearance.BorderColor = borderColor;
            clearButton.Click += (s, e) => {
                chatDisplay.Clear();
                AppendBot("Chat cleared. How can I help you?");
            };

            inputBox = new TextBox();
            inputBox.Dock = DockStyle.Fill;
            inputBox.BackColor = bgInput;
            inputBox.ForeColor = textLight;
            inputBox.Font = new Font("Consolas", 10.5f);
            inputBox.BorderStyle = BorderStyle.FixedSingle;
            inputBox.PlaceholderText = "Type your message here and press Enter...";
            inputBox.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    SendMessage();
                }
            };

            Panel inputRow = new Panel();
            inputRow.Dock = DockStyle.Fill;
            inputRow.Controls.Add(inputBox);
            inputRow.Controls.Add(clearButton);
            inputRow.Controls.Add(sendButton);

            inputPanel.Controls.Add(inputRow);
            inputPanel.Controls.Add(statusLabel);

            // ── Chat Display ─────────────────────────────────────────────────────
            chatDisplay = new RichTextBox();
            chatDisplay.Dock = DockStyle.Fill;
            chatDisplay.BackColor = bgDark;
            chatDisplay.ForeColor = textLight;
            chatDisplay.Font = new Font("Consolas", 10f);
            chatDisplay.ReadOnly = true;
            chatDisplay.BorderStyle = BorderStyle.None;
            chatDisplay.ScrollBars = RichTextBoxScrollBars.Vertical;

            // CRITICAL: Fill first, then Bottom, Right, Top
            this.Controls.Add(chatDisplay);
            this.Controls.Add(sideBorder);
            this.Controls.Add(sidePanel);
            this.Controls.Add(quickPanel);
            this.Controls.Add(inputPanel);
            this.Controls.Add(headerLine);
            this.Controls.Add(headerPanel);
        }

        // ── Event Handlers ────────────────────────────────────────────────────────
        private void ShowGreeting()
        {
            AppendBot(
                "Hello! I am CyberGuard, your Cybersecurity Awareness Assistant.\n\n" +
                "Before we begin, may I ask your name?\n" +
                "Just type your name below and press Enter."
            );
        }

        private void InjectCommand(string command)
        {
            inputBox.Text = command;
            inputBox.Focus();
            inputBox.SelectionStart = inputBox.Text.Length;
        }

        private void SendMessage()
        {
            string userInput = inputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(userInput)) return;

            AppendUser(userInput);
            inputBox.Clear();
            inputBox.Focus();

            UpdateSentiment(userInput);

            if (!_chatBot.IsNameSet)
            {
                string nameResponse = _chatBot.SetUserName(userInput);
                if (nameResponse.StartsWith("Warning"))
                    AppendError(nameResponse);
                else
                {
                    AppendBot(nameResponse);
                    UpdateSidePanel();
                }
                return;
            }

            string response = _chatBot.ProcessMessage(userInput);

            if (response.StartsWith("GOODBYE|"))
            {
                AppendBot(response.Substring("GOODBYE|".Length));
                inputBox.Enabled = false;
                sendButton.Enabled = false;
                statusLabel.Text = "Session ended. Close the window to exit.";
                statusLabel.ForeColor = accentRed;
                return;
            }

            AppendBot(response);
            UpdateSidePanel();
        }

        // ── Display Helpers ───────────────────────────────────────────────────────
        private void AppendUser(string message)
        {
            string name = string.IsNullOrEmpty(_chatBot.UserName) ? "You" : _chatBot.UserName;
            chatDisplay.AppendText("\n");
            AddText("  [" + name + "]: ", accentYellow, true);
            AddText(message + "\n", textLight, false);
        }

        private void AppendBot(string message)
        {
            chatDisplay.AppendText("\n");
            AddText("  [CyberGuard]: ", accentGreen, true);
            AddText(message + "\n", textLight, false);
            AddText("  " + new string('-', 65) + "\n", borderColor, false);
            chatDisplay.SelectionStart = chatDisplay.Text.Length;
            chatDisplay.ScrollToCaret();
        }

        private void AppendError(string message)
        {
            chatDisplay.AppendText("\n");
            AddText("  " + message + "\n", accentRed, false);
            chatDisplay.ScrollToCaret();
        }

        private void AddText(string text, Color colour, bool bold)
        {
            chatDisplay.SelectionStart = chatDisplay.TextLength;
            chatDisplay.SelectionLength = 0;
            chatDisplay.SelectionColor = colour;
            chatDisplay.SelectionFont = new Font(chatDisplay.Font, bold ? FontStyle.Bold : FontStyle.Regular);
            chatDisplay.AppendText(text);
            chatDisplay.SelectionColor = textLight;
        }

        private void UpdateSentiment(string input)
        {
            var sentiment = _sentimentDetector.Detect(input);
            switch (sentiment)
            {
                case SentimentDetector.SentimentType.Worried:
                    statusLabel.Text = "Detected: Worried - Responding with reassurance";
                    statusLabel.ForeColor = accentYellow;
                    break;
                case SentimentDetector.SentimentType.Frustrated:
                    statusLabel.Text = "Detected: Frustrated - Responding with extra clarity";
                    statusLabel.ForeColor = accentRed;
                    break;
                case SentimentDetector.SentimentType.Happy:
                    statusLabel.Text = "Detected: Positive - Keeping up the good vibes!";
                    statusLabel.ForeColor = accentGreen;
                    break;
                case SentimentDetector.SentimentType.Curious:
                    statusLabel.Text = "Detected: Curious - Providing detailed information";
                    statusLabel.ForeColor = accentCyan;
                    break;
                default:
                    statusLabel.Text = "Ready - Ask me anything about cybersecurity!";
                    statusLabel.ForeColor = textDim;
                    break;
            }
        }

        private void UpdateSidePanel()
        {
            // Memory
            string name = _chatBot.UserName;
            memoryLabel.Text = string.IsNullOrEmpty(name)
                ? "No information stored yet.\n\nTell me your name or\nfavourite topic and\nI will remember it."
                : "Name:\n   " + name + "\n\nMore details will\nappear here as\nwe chat.";

            // DB status
            dbStatusLabel.Text = _chatBot.IsDbAvailable ? "DB: Connected" : "DB: Offline (in-memory)";
            dbStatusLabel.ForeColor = _chatBot.IsDbAvailable ? accentGreen : accentYellow;
        }

        private Button MakeQuickButton(string text, Color bg, Color fg, Point location)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Location = location;
            btn.Size = new Size(105, 26);
            btn.BackColor = bg;
            btn.ForeColor = fg;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Consolas", 8.5f);
            btn.Cursor = Cursors.Hand;
            return btn;
        }
    }
}