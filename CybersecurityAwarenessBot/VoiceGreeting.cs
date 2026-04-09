// VoiceGreeting.cs
// This class plays the recorded voice greeting when the chatbot starts.
// The WAV file is included in the project resources folder.
// Written by: [Your Name]

using System;
using System.IO;
using System.Media;

namespace CybersecurityAwarenessBot
{
    public static class VoiceGreeting
    {
        public static void Play()
        {
            try
            {
                // Build the path to the WAV file in our output directory
                string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");

                // Only try to play if the file actually exists
                if (File.Exists(audioPath))
                {
                    using (SoundPlayer player = new SoundPlayer(audioPath))
                    {
                        // PlaySync waits for the audio to finish before continuing
                        player.PlaySync();
                    }
                }
                else
                {
                    // If the file isn't found, we don't crash - we just skip it
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("  [Audio greeting not found - continuing in text mode]");
                    Console.ResetColor();
                }
            }
            catch (Exception)
            {
                // If audio fails for any reason, we gracefully continue
                // The chatbot should never crash just because audio didn't play
            }
        }
    }
}