// ChatBot.cs
// This class is the main controller for the chatbot.
// It manages the conversation loop, greets the user,
// and coordinates between input validation and response handling.
// Written by: [Lindokuhle Mtshali]

using System;
using System.Threading;
using CybersecurityAwarenessBot.UI;
using CybersecurityAwarenessBot.Responses;
using CybersecurityAwarenessBot.Memory;

namespace CybersecurityAwarenessBot
{
    public class ChatBot
    {
        // ── Fields ───────────────────────────────────────────────────────────────
        private readonly UserMemory _memory;
        private readonly ResponseHandler _responseHandler;
        private bool _nameHasBeenSet = false;

        // ── Constructor ──────────────────────────────────────────────────────────
        public ChatBot()
        {
            _memory = new UserMemory();
            _responseHandler = new ResponseHandler(_memory);
        }

        // ── Public Properties ────────────────────────────────────────────────────

        /// <summary>Returns the remembered user name, or string.Empty if not yet set.</summary>
        public string UserName
        {
            get
            {
                string name = _memory.GetName();
                return string.IsNullOrEmpty(name) ? string.Empty : name;
            }
        }

        /// <summary>True once the user has provided a valid name.</summary>
        public bool IsNameSet => _nameHasBeenSet;

        // ── Public Methods ───────────────────────────────────────────────────────

        /// <summary>
        /// Called when the user first provides their name.
        /// Validates it, stores it in memory, and returns a welcome message.
        /// Returns an error message string if the name is invalid.
        /// </summary>
        public string SetUserName(string nameInput)
        {
            if (!InputValidator.IsValidName(nameInput))
            {
                return "⚠ Please enter a valid name (letters only, no numbers or symbols).";
            }

            // Capitalise properly
            string cleaned = nameInput.Trim();
            cleaned = char.ToUpper(cleaned[0]) + cleaned.Substring(1).ToLower();
            _memory.Remember("name", cleaned);
            _nameHasBeenSet = true;

            return $"Welcome aboard, {cleaned}! \n\n" +
                   $"I'm CyberGuard — your personal cybersecurity guide.\n" +
                   $"Together we'll keep you safe in South Africa's digital landscape.\n\n" +
                   $"You can ask me about passwords, phishing, malware, safe browsing, " +
                   $"social engineering, public Wi-Fi, two-factor authentication, and much more.\n\n" +
                   $"Type 'help' to see all available topics. Type 'exit' to end our session.";
        }

        /// <summary>
        /// Processes a user message and returns the bot's response.
        /// Handles exit commands, invalid input, and normal conversation.
        /// </summary>
        public string ProcessMessage(string userInput)
        {
            // Validate input
            if (!InputValidator.IsValidInput(userInput))
            {
                return "⚠ I didn't quite get that. Could you rephrase?";
            }

            // Handle exit
            if (InputValidator.IsExitCommand(userInput))
            {
                string name = _memory.GetName();
                string displayName = string.IsNullOrEmpty(name) ? "there" : name;
                return $"GOODBYE|Goodbye, {displayName}! Stay cyber-safe out there. \nRemember: Think before you click!";
            }

            // Get response from the response handler
            return _responseHandler.GetResponse(userInput);
        }

        /// <summary>Resets the chatbot session (clears memory).</summary>
        public void Reset()
        {
            _memory.Clear();
            _nameHasBeenSet = false;
        }
    }
}