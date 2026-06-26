// QuizEngine.cs
// Cybersecurity mini-game quiz engine.
// Contains 15+ questions covering phishing, passwords, safe browsing,
// social engineering, malware, and 2FA.
// Tracks the user's score and provides feedback per answer.
// Written by: Lindokuhle Mtshali
// Student Number: ST10233093

using System;
using System.Collections.Generic;

namespace CybersecurityAwarenessBot.Quiz
{
    public class QuizEngine
    {
        // ── Quiz Question Model ───────────────────────────────────────────────────
        public class QuizQuestion
        {
            public string Question { get; set; }
            public string[] Options { get; set; }          // A, B, C, D
            public int CorrectIndex { get; set; }           // 0=A, 1=B, 2=C, 3=D
            public string Explanation { get; set; }
            public bool IsMultipleChoice { get; set; }      // false = True/False

            public QuizQuestion(string question, string[] options, int correctIndex, string explanation, bool isMultipleChoice = true)
            {
                Question = question;
                Options = options;
                CorrectIndex = correctIndex;
                Explanation = explanation;
                IsMultipleChoice = isMultipleChoice;
            }
        }

        // ── Fields ───────────────────────────────────────────────────────────────
        private readonly List<QuizQuestion> _allQuestions;
        private List<QuizQuestion> _sessionQuestions;
        private int _currentIndex;
        private int _score;
        private bool _isActive;
        private readonly Random _random;

        // ── Constructor ──────────────────────────────────────────────────────────
        public QuizEngine()
        {
            _random = new Random();
            _allQuestions = BuildQuestions();
            _isActive = false;
        }

        // ── Public Properties ────────────────────────────────────────────────────
        public bool IsActive => _isActive;
        public int Score => _score;
        public int TotalQuestions => _sessionQuestions?.Count ?? 0;
        public int CurrentQuestionNumber => _currentIndex + 1;

        // ── Public Methods ───────────────────────────────────────────────────────

        /// <summary>Starts a new quiz session with 10 randomly selected questions.</summary>
        public string StartQuiz()
        {
            // Shuffle and pick 10 questions
            _sessionQuestions = new List<QuizQuestion>(_allQuestions);
            for (int i = _sessionQuestions.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                var temp = _sessionQuestions[i];
                _sessionQuestions[i] = _sessionQuestions[j];
                _sessionQuestions[j] = temp;
            }
            if (_sessionQuestions.Count > 10)
                _sessionQuestions = _sessionQuestions.GetRange(0, 10);

            _currentIndex = 0;
            _score = 0;
            _isActive = true;

            return "Cybersecurity Quiz started! You will be asked 10 questions.\n" +
                   "Type the letter of your answer (A, B, C, or D) or True/False.\n" +
                   "Type 'quit quiz' at any time to stop.\n\n" +
                   GetCurrentQuestionText();
        }

        /// <summary>Processes the user's answer. Returns feedback + next question or final score.</summary>
        public string SubmitAnswer(string userInput)
        {
            if (!_isActive) return "No quiz is running. Say 'start quiz' to begin!";

            // Allow quitting mid-quiz
            if (userInput.ToLower().Contains("quit quiz") || userInput.ToLower() == "quit")
            {
                _isActive = false;
                return $"Quiz ended early. You scored {_score}/{_currentIndex} on questions answered.";
            }

            var current = _sessionQuestions[_currentIndex];
            int answerIndex = ParseAnswer(userInput.Trim(), current.IsMultipleChoice);

            if (answerIndex == -1)
                return "Please answer with A, B, C, or D (or True/False for True/False questions).";

            string feedback;
            if (answerIndex == current.CorrectIndex)
            {
                _score++;
                feedback = "Correct! Well done.\n";
            }
            else
            {
                string correctLetter = current.IsMultipleChoice
                    ? ((char)('A' + current.CorrectIndex)).ToString()
                    : (current.CorrectIndex == 0 ? "True" : "False");
                feedback = $"Incorrect. The correct answer was {correctLetter}.\n";
            }

            feedback += $"Explanation: {current.Explanation}\n";
            _currentIndex++;

            if (_currentIndex >= _sessionQuestions.Count)
            {
                _isActive = false;
                return feedback + "\n" + GetFinalScore();
            }

            feedback += $"\nScore so far: {_score}/{_currentIndex}\n\n";
            feedback += GetCurrentQuestionText();
            return feedback;
        }

        // ── Private Helpers ──────────────────────────────────────────────────────

        private string GetCurrentQuestionText()
        {
            var q = _sessionQuestions[_currentIndex];
            string text = $"Question {_currentIndex + 1} of {_sessionQuestions.Count}:\n{q.Question}\n\n";

            if (q.IsMultipleChoice)
            {
                for (int i = 0; i < q.Options.Length; i++)
                    text += $"  {(char)('A' + i)}) {q.Options[i]}\n";
            }
            else
            {
                text += "  A) True\n  B) False\n";
            }

            return text;
        }

        private string GetFinalScore()
        {
            string verdict;
            double pct = (double)_score / _sessionQuestions.Count * 100;

            if (pct >= 90)
                verdict = "Outstanding! You are a cybersecurity pro!";
            else if (pct >= 70)
                verdict = "Great work! You have solid cybersecurity knowledge.";
            else if (pct >= 50)
                verdict = "Good effort! Keep learning to stay safe online.";
            else
                verdict = "Keep learning! Cybersecurity knowledge is your best defence.";

            return $"Quiz Complete!\nFinal Score: {_score}/{_sessionQuestions.Count} ({pct:0}%)\n{verdict}";
        }

        private int ParseAnswer(string input, bool isMultipleChoice)
        {
            string lower = input.ToLower().Trim();

            if (isMultipleChoice)
            {
                if (lower == "a") return 0;
                if (lower == "b") return 1;
                if (lower == "c") return 2;
                if (lower == "d") return 3;
            }
            else
            {
                if (lower == "a" || lower == "true") return 0;
                if (lower == "b" || lower == "false") return 1;
            }

            return -1;
        }

        // ── Question Bank (15 questions) ─────────────────────────────────────────
        private List<QuizQuestion> BuildQuestions()
        {
            return new List<QuizQuestion>
            {
                // Phishing
                new QuizQuestion(
                    "What should you do if you receive an email asking for your password?",
                    new[] { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                    2,
                    "Reporting phishing emails helps protect others. Never share your password via email.",
                    true
                ),
                new QuizQuestion(
                    "Which of these is a warning sign of a phishing email?",
                    new[] { "It comes from your manager", "It has urgent language like 'Act now or lose access!'",
                            "It contains your full name", "It is sent during business hours" },
                    1,
                    "Urgency is a classic social engineering tactic to stop you thinking critically.",
                    true
                ),
                new QuizQuestion(
                    "A legitimate bank will never ask for your PIN or password via email or SMS.",
                    new[] { "True", "False" },
                    0,
                    "Banks never ask for sensitive credentials via email or SMS. This is always a scam.",
                    false
                ),

                // Password Safety
                new QuizQuestion(
                    "What is the minimum recommended length for a strong password?",
                    new[] { "6 characters", "8 characters", "12 characters", "20 characters" },
                    2,
                    "Security experts recommend at least 12 characters combining letters, numbers, and symbols.",
                    true
                ),
                new QuizQuestion(
                    "Which of the following is the STRONGEST password?",
                    new[] { "Password123", "P@ssw0rd!", "mountain-river-cloud-door", "MyName1990" },
                    2,
                    "Passphrases with random words are long and easy to remember, making them very secure.",
                    true
                ),
                new QuizQuestion(
                    "Using the same password for multiple websites is safe as long as it is strong.",
                    new[] { "True", "False" },
                    1,
                    "If one website is breached, all your accounts with that password are at risk.",
                    false
                ),

                // Safe Browsing
                new QuizQuestion(
                    "What does HTTPS in a website address mean?",
                    new[] { "The website is government-owned", "The connection is encrypted and more secure",
                            "The website is free", "The website is popular" },
                    1,
                    "HTTPS means data between you and the site is encrypted, making it harder for attackers to intercept.",
                    true
                ),
                new QuizQuestion(
                    "It is safe to download software from any website that appears at the top of a Google search.",
                    new[] { "True", "False" },
                    1,
                    "Cybercriminals buy ads to appear at the top of search results with fake/malicious software.",
                    false
                ),

                // Social Engineering
                new QuizQuestion(
                    "Someone calls you claiming to be from your bank's IT department and asks for your one-time PIN. What do you do?",
                    new[] { "Give them the PIN - they said they are from the bank", "Hang up and call the bank's official number",
                            "Give them only half the PIN", "Ask them to email you first" },
                    1,
                    "Always hang up and call the bank's official number. Banks never call to ask for OTPs.",
                    true
                ),
                new QuizQuestion(
                    "What is 'pretexting' in social engineering?",
                    new[] { "Sending spam emails", "Creating a fake scenario to gain your trust and extract information",
                            "Installing malware on a computer", "Hacking a WiFi network" },
                    1,
                    "Pretexting involves inventing a plausible story (e.g. 'I'm from IT') to manipulate victims.",
                    true
                ),

                // Malware
                new QuizQuestion(
                    "What type of malware encrypts your files and demands payment to restore access?",
                    new[] { "Spyware", "Adware", "Ransomware", "Trojan" },
                    2,
                    "Ransomware is particularly destructive. Regular offline backups are the best defence.",
                    true
                ),
                new QuizQuestion(
                    "Antivirus software alone is enough to fully protect your computer from all cyber threats.",
                    new[] { "True", "False" },
                    1,
                    "Antivirus helps but cannot catch everything. Safe habits, updates, and 2FA are equally important.",
                    false
                ),

                // Two-Factor Authentication
                new QuizQuestion(
                    "What is the purpose of two-factor authentication (2FA)?",
                    new[] { "To make logging in faster", "To add a second layer of security beyond just a password",
                            "To reset your password automatically", "To track your login location" },
                    1,
                    "Even if someone steals your password, 2FA prevents them from accessing your account.",
                    true
                ),
                new QuizQuestion(
                    "Which form of 2FA is considered LEAST secure?",
                    new[] { "An authenticator app (e.g. Google Authenticator)", "A hardware security key",
                            "SMS one-time PIN sent to your phone", "Biometric fingerprint scan" },
                    2,
                    "SMS 2FA is vulnerable to SIM swapping attacks, which are common in South Africa.",
                    true
                ),

                // Public Wi-Fi
                new QuizQuestion(
                    "What is the safest way to use public Wi-Fi?",
                    new[] { "Connect and browse normally - public networks are secure",
                            "Use a VPN to encrypt your traffic",
                            "Only visit websites that start with HTTP",
                            "Share your hotspot with others for free" },
                    1,
                    "A VPN encrypts all your traffic, protecting you from attackers on the same public network.",
                    true
                ),

                // SIM Swapping
                new QuizQuestion(
                    "How can you protect yourself against SIM swapping attacks?",
                    new[] { "Use SMS-based 2FA exclusively", "Set a port-out PIN with your mobile provider and use an authenticator app",
                            "Share your number only with friends", "Change your phone every year" },
                    1,
                    "A port-out PIN prevents criminals from transferring your number. Authenticator apps don't rely on SMS.",
                    true
                )
            };
        }
    }
}