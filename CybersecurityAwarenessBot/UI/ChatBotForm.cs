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

        private RichTextBox chatDisplay;
        private TextBox inputBox;
        private Button sendButton;
        private Button clearButton;
        private Label statusLabel;
        private Label memoryLabel;

        private Color bgDark = Color.FromArgb(13, 17, 23);
        private Color bgMid = Color.FromArgb(22, 27, 34);
        private Color bgInput = Color.FromArgb(33, 38, 45);
        private Color accentCyan = Color.FromArgb(0, 210, 211);
        private Color accentGreen = Color.FromArgb(63, 185, 80);
        private Color accentYellow = Color.FromArgb(230, 162, 44);
        private Color accentRed = Color.FromArgb(248, 81, 73);
        private Color textLight = Color.FromArgb(201, 209, 217);
        private Color textDim = Color.FromArgb(110, 118, 129);
        private Color borderColor = Color.FromArgb(48, 54, 61);

        public ChatBotForm()
        {
            _chatBot = new ChatBot();
            _sentimentDetector = new SentimentDetector();
            this.Text = "CyberGuard - Cybersecurity Awareness Bot";
            this.Size = new Size(1050, 700);
            this.MinimumSize = new Size(800, 550);
            this.BackColor = Color.FromArgb(13, 17, 23);
            this.Font = new Font("Consolas", 9.5f);
            this.StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
            this.Load += (s, e) => ShowGreeting();
        }

        private void BuildUI()
        {
            // Header
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 85;
            headerPanel.BackColor = Color.FromArgb(22, 27, 34);

            Label titleLabel = new Label();
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Font = new Font("Consolas", 7f);
            titleLabel.ForeColor = Color.FromArgb(0, 210, 211);
            titleLabel.Text =
                "  ######  ##  ## ######  ######  ######   ######  ##  ## ######  ######  ###### \n" +
                "  ##      ##  ## ##  ##  ##      ##   ##  ##      ##  ## ##  ##  ##  ##  ##  ## \n" +
                "  ##      ##  ## ######  ####    ######   ##  ### ##  ## ######  ##  ##  ##  ## \n" +
                "  ##       ####  ##  ##  ##      ##  ##   ##   ## ##  ## ##  ##  ##  ##  ##  ## \n" +
                "  ######    ##   ######  ######  ##   ##   ######  ####  ######  ######  ###### \n" +
                "                  Cybersecurity Awareness Bot  -  South Africa                  ";
            headerPanel.Controls.Add(titleLabel);

            Panel headerLine = new Panel();
            headerLine.Dock = DockStyle.Top;
            headerLine.Height = 2;
            headerLine.BackColor = Color.FromArgb(0, 210, 211);

            // Side panel
            Panel sidePanel = new Panel();
            sidePanel.Dock = DockStyle.Right;
            sidePanel.Width = 210;
            sidePanel.BackColor = Color.FromArgb(22, 27, 34);
            sidePanel.Padding = new Padding(8);

            Label sideTitle = new Label();
            sideTitle.Text = "SESSION MEMORY";
            sideTitle.Font = new Font("Consolas", 8.5f, FontStyle.Bold);
            sideTitle.ForeColor = Color.FromArgb(0, 210, 211);
            sideTitle.Dock = DockStyle.Top;
            sideTitle.Height = 28;
            sideTitle.TextAlign = ContentAlignment.MiddleLeft;

            Panel sideDivider = new Panel();
            sideDivider.Dock = DockStyle.Top;
            sideDivider.Height = 1;
            sideDivider.BackColor = Color.FromArgb(48, 54, 61);

            memoryLabel = new Label();
            memoryLabel.Text = "No information stored yet.\n\nTell me your name or\nfavourite topic and\nI will remember it.";
            memoryLabel.Font = new Font("Consolas", 8f);
            memoryLabel.ForeColor = Color.FromArgb(110, 118, 129);
            memoryLabel.Dock = DockStyle.Fill;
            memoryLabel.TextAlign = ContentAlignment.TopLeft;
            memoryLabel.Padding = new Padding(0, 6, 0, 0);

            Label tipsLabel = new Label();
            tipsLabel.Text =
                "QUICK TIPS\n" +
                "---------------------\n" +
                "Type 'help' for topics\n" +
                "'Tell me more' to\n" +
                "continue a topic\n" +
                "'Give me a phishing\n" +
                "tip' for random tips\n" +
                "'My name is [name]'\n" +
                "to introduce yourself\n" +
                "'exit' to quit";
            tipsLabel.Font = new Font("Consolas", 7.8f);
            tipsLabel.ForeColor = Color.FromArgb(110, 118, 129);
            tipsLabel.Dock = DockStyle.Bottom;
            tipsLabel.Height = 175;
            tipsLabel.TextAlign = ContentAlignment.TopLeft;

            sidePanel.Controls.Add(memoryLabel);
            sidePanel.Controls.Add(sideDivider);
            sidePanel.Controls.Add(sideTitle);
            sidePanel.Controls.Add(tipsLabel);

            Panel sideBorder = new Panel();
            sideBorder.Dock = DockStyle.Right;
            sideBorder.Width = 1;
            sideBorder.BackColor = Color.FromArgb(48, 54, 61);

            // Bottom input panel
            Panel inputPanel = new Panel();
            inputPanel.Dock = DockStyle.Bottom;
            inputPanel.Height = 85;
            inputPanel.BackColor = Color.FromArgb(22, 27, 34);
            inputPanel.Padding = new Padding(10, 8, 10, 8);

            statusLabel = new Label();
            statusLabel.Text = "Ready - Ask me anything about cybersecurity!";
            statusLabel.Font = new Font("Consolas", 8.5f);
            statusLabel.ForeColor = Color.FromArgb(110, 118, 129);
            statusLabel.Dock = DockStyle.Bottom;
            statusLabel.Height = 20;
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;

            sendButton = new Button();
            sendButton.Text = "Send";
            sendButton.Dock = DockStyle.Right;
            sendButton.Width = 90;
            sendButton.BackColor = Color.FromArgb(0, 210, 211);
            sendButton.ForeColor = Color.FromArgb(13, 17, 23);
            sendButton.FlatStyle = FlatStyle.Flat;
            sendButton.Font = new Font("Consolas", 9.5f, FontStyle.Bold);
            sendButton.Cursor = Cursors.Hand;
            sendButton.FlatAppearance.BorderSize = 0;
            sendButton.Click += (s, e) => SendMessage();

            clearButton = new Button();
            clearButton.Text = "Clear";
            clearButton.Dock = DockStyle.Right;
            clearButton.Width = 65;
            clearButton.BackColor = Color.FromArgb(33, 38, 45);
            clearButton.ForeColor = Color.FromArgb(110, 118, 129);
            clearButton.FlatStyle = FlatStyle.Flat;
            clearButton.Font = new Font("Consolas", 9f);
            clearButton.Cursor = Cursors.Hand;
            clearButton.FlatAppearance.BorderColor = Color.FromArgb(48, 54, 61);
            clearButton.Click += (s, e) => {
                chatDisplay.Clear();
                AppendBot("Chat cleared. How can I help you?");
            };

            inputBox = new TextBox();
            inputBox.Dock = DockStyle.Fill;
            inputBox.BackColor = Color.FromArgb(33, 38, 45);
            inputBox.ForeColor = Color.FromArgb(201, 209, 217);
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

            // Chat display - fill
            chatDisplay = new RichTextBox();
            chatDisplay.Dock = DockStyle.Fill;
            chatDisplay.BackColor = Color.FromArgb(13, 17, 23);
            chatDisplay.ForeColor = Color.FromArgb(201, 209, 217);
            chatDisplay.Font = new Font("Consolas", 10f);
            chatDisplay.ReadOnly = true;
            chatDisplay.BorderStyle = BorderStyle.None;
            chatDisplay.ScrollBars = RichTextBoxScrollBars.Vertical;

            // CRITICAL: Fill control must be added to form first
            // Then Bottom, then Right, then Top
            this.Controls.Add(chatDisplay);
            this.Controls.Add(sideBorder);
            this.Controls.Add(sidePanel);
            this.Controls.Add(inputPanel);
            this.Controls.Add(headerLine);
            this.Controls.Add(headerPanel);
        }

        private void ShowGreeting()
        {
            AppendBot(
                "Hello! I am CyberGuard, your Cybersecurity Awareness Assistant.\n\n" +
                "Before we begin, may I ask your name?\n" +
                "Just type your name below and press Enter."
            );
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
                    UpdateMemory();
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
                statusLabel.ForeColor = Color.FromArgb(248, 81, 73);
                return;
            }

            AppendBot(response);
            UpdateMemory();
        }

        private void AppendUser(string message)
        {
            string name = string.IsNullOrEmpty(_chatBot.UserName) ? "You" : _chatBot.UserName;
            chatDisplay.AppendText("\n");
            AddText("  [" + name + "]: ", Color.FromArgb(230, 162, 44), true);
            AddText(message + "\n", Color.FromArgb(201, 209, 217), false);
        }

        private void AppendBot(string message)
        {
            chatDisplay.AppendText("\n");
            AddText("  [CyberGuard]: ", Color.FromArgb(63, 185, 80), true);
            AddText(message + "\n", Color.FromArgb(201, 209, 217), false);
            AddText("  " + new string('-', 65) + "\n", Color.FromArgb(48, 54, 61), false);
            chatDisplay.SelectionStart = chatDisplay.Text.Length;
            chatDisplay.ScrollToCaret();
        }

        private void AppendError(string message)
        {
            chatDisplay.AppendText("\n");
            AddText("  " + message + "\n", Color.FromArgb(248, 81, 73), false);
            chatDisplay.ScrollToCaret();
        }

        private void AddText(string text, Color colour, bool bold)
        {
            chatDisplay.SelectionStart = chatDisplay.TextLength;
            chatDisplay.SelectionLength = 0;
            chatDisplay.SelectionColor = colour;
            chatDisplay.SelectionFont = new Font(chatDisplay.Font, bold ? FontStyle.Bold : FontStyle.Regular);
            chatDisplay.AppendText(text);
            chatDisplay.SelectionColor = Color.FromArgb(201, 209, 217);
        }

        private void UpdateSentiment(string input)
        {
            var sentiment = _sentimentDetector.Detect(input);
            switch (sentiment)
            {
                case SentimentDetector.SentimentType.Worried:
                    statusLabel.Text = "Detected: Worried - Responding with reassurance";
                    statusLabel.ForeColor = Color.FromArgb(230, 162, 44);
                    break;
                case SentimentDetector.SentimentType.Frustrated:
                    statusLabel.Text = "Detected: Frustrated - Responding with extra clarity";
                    statusLabel.ForeColor = Color.FromArgb(248, 81, 73);
                    break;
                case SentimentDetector.SentimentType.Happy:
                    statusLabel.Text = "Detected: Positive - Keeping up the good vibes!";
                    statusLabel.ForeColor = Color.FromArgb(63, 185, 80);
                    break;
                case SentimentDetector.SentimentType.Curious:
                    statusLabel.Text = "Detected: Curious - Providing detailed information";
                    statusLabel.ForeColor = Color.FromArgb(0, 210, 211);
                    break;
                default:
                    statusLabel.Text = "Ready - Ask me anything about cybersecurity!";
                    statusLabel.ForeColor = Color.FromArgb(110, 118, 129);
                    break;
            }
        }

        private void UpdateMemory()
        {
            string name = _chatBot.UserName;
            memoryLabel.Text = string.IsNullOrEmpty(name)
                ? "No information stored yet.\n\nTell me your name or\nfavourite topic and\nI will remember it."
                : "Name:\n   " + name + "\n\nMore details will\nappear here as\nwe chat.";
        }
    }
}
