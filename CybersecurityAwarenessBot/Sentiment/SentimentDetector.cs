// SentimentDetector.cs
// Detects the user's emotional tone from their input.
// Implements Part 2 Requirement 6: Sentiment Detection.
// Adjusts the bot's response style based on detected mood.
// Written by: Lindokuhle Mtshali

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CybersecurityAwarenessBot.Sentiment
{
    public class SentimentDetector
    {
        // Enum representing the types of sentiment we can detect
        public enum SentimentType
        {
            Neutral,
            Worried,
            Curious,
            Frustrated,
            Happy
        }

        // Keyword lists for each sentiment type
        private readonly List<string> _worriedKeywords = new List<string>
        {
            "worried", "scared", "afraid", "nervous", "anxious", "fear",
            "concern", "concerned", "unsafe", "danger", "dangerous", "risk",
            "terrified", "stressed", "panic", "help me", "i dont know what to do"
        };

        private readonly List<string> _curiousKeywords = new List<string>
        {
            "curious", "wondering", "want to know", "tell me more", "how does",
            "what is", "explain", "interested", "learn", "understand", "why",
            "how", "what", "tell me about", "give me more"
        };

        private readonly List<string> _frustratedKeywords = new List<string>
        {
            "frustrated", "annoyed", "angry", "irritated", "stupid", "useless",
            "doesnt work", "doesn't work", "not working", "wrong", "bad",
            "hate", "terrible", "awful", "ridiculous", "waste of time", "confused"
        };

        private readonly List<string> _happyKeywords = new List<string>
        {
            "great", "thanks", "thank you", "awesome", "cool", "nice",
            "helpful", "good", "love", "excellent", "perfect", "amazing",
            "brilliant", "fantastic", "appreciate", "wonderful"
        };

        /// <summary>
        /// Analyses the given input string and returns the detected sentiment.
        /// </summary>
        public SentimentType Detect(string input)
        {
            string lower = input.ToLower();

            // Check each sentiment category in priority order
            foreach (string keyword in _worriedKeywords)
                if (lower.Contains(keyword)) return SentimentType.Worried;

            foreach (string keyword in _frustratedKeywords)
                if (lower.Contains(keyword)) return SentimentType.Frustrated;

            foreach (string keyword in _happyKeywords)
                if (lower.Contains(keyword)) return SentimentType.Happy;

            foreach (string keyword in _curiousKeywords)
                if (lower.Contains(keyword)) return SentimentType.Curious;

            return SentimentType.Neutral;
        }

        /// <summary>
        /// Returns an empathetic prefix message based on the detected sentiment.
        /// This is prepended to the bot's main response.
        /// </summary>
        public string GetSentimentPrefix(SentimentType sentiment, string userName)
        {
            switch (sentiment)
            {
                case SentimentType.Worried:
                    return $"I can hear that you're concerned, {userName}. That's completely understandable — cybersecurity can feel overwhelming. Let me help put your mind at ease.\n\n";

                case SentimentType.Frustrated:
                    return $"I'm sorry you're feeling frustrated, {userName}. Let me try to explain this more clearly and help you out.\n\n";

                case SentimentType.Happy:
                    return $"Great to hear you're feeling positive, {userName}! 😊 Let's keep that energy going!\n\n";

                case SentimentType.Curious:
                    return $"Love the curiosity, {userName}! That's exactly the right mindset for staying cyber-safe.\n\n";

                default:
                    return "";
            }
        }
    }
}
