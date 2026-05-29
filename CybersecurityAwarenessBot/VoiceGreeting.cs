// VoiceGreeting.cs
// This class plays the recorded voice greeting when the chatbot starts.
// The WAV file is included in the project resources folder.
// Written by: [Your Name]

using System;
using System.IO;

namespace CybersecurityAwarenessBot
{
    public static class VoiceGreeting
    {
        public static void Play()
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    PlayWavWindows();
                }
                // On non-Windows platforms, silently skip audio
            }
            catch (Exception)
            {
                // If audio fails for any reason, we continue silently
                // The chatbot should never crash just because audio didn't play
            }
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private static void PlayWavWindows()
        {
            // Build the path to the WAV file in our output directory
            string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");

            // Only try to play if the file actually exists
            if (File.Exists(audioPath))
            {
                using (var player = new System.Media.SoundPlayer(audioPath))
                {
                    // PlaySync waits for the audio to finish before continuing
                    player.PlaySync();
                }
            }
            // If file not found, gracefully skip - GUI will still open
        }
    }
}