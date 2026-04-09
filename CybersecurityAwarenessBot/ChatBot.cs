// ChatBot.cs
// This class is the main controller for the chatbot.
// It manages the conversation loop, greets the user,
// and coordinates between input validation and response handling.
// Written by: [Lindokuhle Mtshali]

using System;
using System.Threading;
using CybersecurityAwarenessBot.UI;
using CybersecurityAwarenessBot.Responses;

namespace CybersecurityAwarenessBot
{
    public class ChatBot
    {
        // Stores the user's name so we can personalise the conversation
        private string _userName = "";

        // This is the main method that runs the whole chatbot experience
        public void Start()
        {
            // Show the ASCII art header first
            ConsoleHelper.DisplayHeader();

            // Greet the user and ask for their name
            GetUserName();

            // Show a personalised welcome message after getting their name
            ConsoleHelper.ShowWelcomeMessage(_userName);

            // Start the main chat loop
            RunChatLoop();
        }

        // Asks the user for their name and stores it
        private void GetUserName()
        {
            ConsoleHelper.TypeText("\n Welcome! I'm CyberGuard, your Cybersecurity Awareness Assistant.");
            ConsoleHelper.TypeText(" Before we begin, may I ask your name? ");
            Console.Write("\n  >>> ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            _userName = Console.ReadLine()?.Trim() ?? "";
            Console.ResetColor();

            // Keep asking until we get a valid name
            while (!InputValidator.IsValidName(_userName))
            {
                ConsoleHelper.ShowError(" Please enter a valid name (letters only, no numbers or symbols).");
                Console.Write("\n  >>> ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                _userName = Console.ReadLine()?.Trim() ?? "";
                Console.ResetColor();
            }
        }

        // The main conversation loop - keeps running until the user says goodbye
        private void RunChatLoop()
        {
            ResponseHandler responseHandler = new ResponseHandler(_userName);
            bool isRunning = true;

            while (isRunning)
            {
                // Display the input prompt
                ConsoleHelper.ShowPrompt(_userName);
                Console.ForegroundColor = ConsoleColor.Cyan;
                string userInput = Console.ReadLine()?.Trim() ?? "";
                Console.ResetColor();

                // Validate the input before processing it
                if (!InputValidator.IsValidInput(userInput))
                {
                    ConsoleHelper.ShowError(" I didn't quite understand that. Could you rephrase?");
                    continue;
                }

                // Check if the user wants to exit
                if (InputValidator.IsExitCommand(userInput))
                {
                    ConsoleHelper.ShowFarewell(_userName);
                    isRunning = false;
                    continue;
                }

                // Get and display the bot's response
                string response = responseHandler.GetResponse(userInput);
                ConsoleHelper.TypeText($"\n  [CyberGuard]: {response}");
                ConsoleHelper.ShowDivider();
            }
        }
    }
}