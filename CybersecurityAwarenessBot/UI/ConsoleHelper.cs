// ConsoleHelper.cs
// This class handles all the visual styling of the chatbot.
// It keeps our console UI looking clean, dark, and professional.
// All colours, spacing, borders, and text effects live here.
// Written by: [Lindokuhle Mtshali]

using System;
using System.Threading;

namespace CybersecurityAwarenessBot.UI
{
    public static class ConsoleHelper
    {
        // Set dark theme colours
        private static readonly ConsoleColor HeaderColor = ConsoleColor.DarkCyan;
        private static readonly ConsoleColor BotColor = ConsoleColor.Green;
        private static readonly ConsoleColor ErrorColor = ConsoleColor.Red;
        private static readonly ConsoleColor AccentColor = ConsoleColor.DarkYellow;
        private static readonly ConsoleColor DividerColor = ConsoleColor.DarkGray;

        // Shows the ASCII art header when the app starts
        public static void DisplayHeader()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.ForegroundColor = HeaderColor;

            // ASCII art for the Cybersecurity Awareness Bot
            Console.WriteLine(@"
  ██████╗██╗   ██╗██████╗ ███████╗██████╗  ██████╗ ██╗   ██╗ █████╗ ██████╗ ██████╗ 
 ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗██╔════╝ ██║   ██║██╔══██╗██╔══██╗██╔══██╗
 ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝██║  ███╗██║   ██║███████║██████╔╝██║  ██║
 ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗██║   ██║██║   ██║██╔══██║██╔══██╗██║  ██║
 ╚██████╗   ██║   ██████╔╝███████╗██║  ██║╚██████╔╝╚██████╔╝██║  ██║██║  ██║██████╔╝
  ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═════╝ 
            ");

            Console.ForegroundColor = AccentColor;
            Console.WriteLine("             🛡️  Cybersecurity Awareness Assistant for South Africa  🛡️");
            Console.ForegroundColor = DividerColor;
            Console.WriteLine("  ══════════════════════════════════════════════════════════════════════════════");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("             Powered by the Department of Cybersecurity Awareness Initiative");
            Console.WriteLine("  ══════════════════════════════════════════════════════════════════════════════");
            Console.ResetColor();
        }

        // Shows a personalised welcome message after the user enters their name
        public static void ShowWelcomeMessage(string userName)
        {
            Console.ForegroundColor = BotColor;
            Thread.Sleep(400);
            Console.WriteLine($"\n  ┌─────────────────────────────────────────────────────────────┐");
            Console.WriteLine($"  │  Welcome aboard, {userName,-44}│");
            Console.WriteLine($"  │  I'm CyberGuard - your personal cybersecurity guide.        │");
            Console.WriteLine($"  │  Together we'll keep you safe in South Africa's digital      │");
            Console.WriteLine($"  │  landscape. Type 'help' to see what I can assist with.      │");
            Console.WriteLine($"  │  Type 'exit' anytime to end our session.                    │");
            Console.WriteLine($"  └─────────────────────────────────────────────────────────────┘");
            Console.ResetColor();
            ShowDivider();
        }

        // Displays the input prompt with the user's name
        public static void ShowPrompt(string userName)
        {
            Console.ForegroundColor = AccentColor;
            Console.Write($"\n  [{userName}]: ");
            Console.ResetColor();
        }

        // Types text out character by character for a conversational feel
        public static void TypeText(string text, int delayMs = 18)
        {
            Console.ForegroundColor = BotColor;
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delayMs);
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        // Shows a horizontal divider line
        public static void ShowDivider()
        {
            Console.ForegroundColor = DividerColor;
            Console.WriteLine("\n  ──────────────────────────────────────────────────────────────────");
            Console.ResetColor();
        }

        // Shows errors in red so they stand out
        public static void ShowError(string message)
        {
            Console.ForegroundColor = ErrorColor;
            Console.WriteLine($"\n  ⚠  {message}");
            Console.ResetColor();
        }

        // Shows a farewell message when the user exits
        public static void ShowFarewell(string userName)
        {
            Console.ForegroundColor = HeaderColor;
            Console.WriteLine($"\n  ══════════════════════════════════════════════════════════════════");
            TypeText($"\n  Goodbye, {userName}! Stay cyber-safe out there. 🛡️");
            TypeText("  Remember: Think before you click!");
            Console.WriteLine("\n  ══════════════════════════════════════════════════════════════════\n");
            Console.ResetColor();
        }
    }
}