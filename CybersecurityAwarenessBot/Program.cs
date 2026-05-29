// Program.cs
// Entry point for the Cybersecurity Awareness Bot application.
// This file keeps things simple - it just starts the chatbot.
// Written by: [lindokuhle Mtshali ]
// Student Number: [ST10233093]
// Date: May 2026

using System;
using System.Windows.Forms;

namespace CybersecurityAwarenessBot
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Play the voice greeting before the GUI opens (same as Part 1)
            VoiceGreeting.Play();

            // Launch the main GUI window
            Application.Run(new UI.ChatBotForm());
        }
    }
}