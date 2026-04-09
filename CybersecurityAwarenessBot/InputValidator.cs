// InputValidator.cs
// This class handles all input validation for the chatbot.
// It makes sure we don't crash on bad or empty inputs,
// and provides helpful feedback to the user when something's wrong.
// Written by: [Lindokuhle Mtshali]

using System;
using System.Text.RegularExpressions;

namespace CybersecurityAwarenessBot
{
    public static class InputValidator
    {
        // Words that tell us the user wants to quit
        private static readonly string[] _exitCommands = { "exit", "quit", "bye", "goodbye", "stop" };

        // Checks if the input is not empty or just whitespace
        public static bool IsValidInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Input must be at least 1 real character
            if (input.Trim().Length < 1)
                return false;

            return true;
        }

        // Checks if a name only contains letters and spaces (no numbers or symbols)
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Only allow letters and spaces in names
            return Regex.IsMatch(name.Trim(), @"^[a-zA-Z\s]+$");
        }

        // Checks if the user typed a command to exit the chatbot
        public static bool IsExitCommand(string input)
        {
            string lowerInput = input.ToLower().Trim();
            foreach (string command in _exitCommands)
            {
                if (lowerInput == command)
                    return true;
            }
            return false;
        }
    }
}