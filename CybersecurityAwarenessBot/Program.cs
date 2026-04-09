// Program.cs
// Entry point for the Cybersecurity Awareness Bot application.
// This file keeps things simple - it just starts the chatbot.
// Written by: [lindokuhle Mtshali ]
// Student Number: [ST10233093]
// Date: April 2026

using System;

namespace CybersecurityAwarenessBot
{
    class Program
    {
        static void Main(string[] args)
        {
            // Play the voice greeting when the app launches
            VoiceGreeting.Play();

            // Start the chatbot
            ChatBot bot = new ChatBot();
            bot.Start();
        }
    }
}