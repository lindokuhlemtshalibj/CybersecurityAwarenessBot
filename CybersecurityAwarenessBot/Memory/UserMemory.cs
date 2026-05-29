// UserMemory.cs
// Stores and retrieves information the user shares during the conversation.
// Implements Part 2 Requirement 5: Memory and Recall.
// Uses a Dictionary<string, string> as a generic collection to store user data.
// Written by: Lindokuhle Mtshali

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CybersecurityAwarenessBot.Memory
{
    /// <summary>
    /// Manages persistent memory of user-provided details across the conversation.
    /// Allows the bot to recall the user's name, favourite topic, and other context.
    /// </summary>
    public class UserMemory
    {
        // Generic Dictionary collection stores key-value pairs of user information
        // e.g. "name" -> "Lindokuhle", "favourite_topic" -> "phishing"
        private readonly Dictionary<string, string> _memoryStore;

        public UserMemory()
        {
            _memoryStore = new Dictionary<string, string>();
        }

        /// <summary>
        /// Stores a piece of user information under the given key.
        /// If the key already exists, its value is updated.
        /// </summary>
        public void Remember(string key, string value)
        {
            string normalisedKey = key.ToLower().Trim();
            if (_memoryStore.ContainsKey(normalisedKey))
                _memoryStore[normalisedKey] = value;
            else
                _memoryStore.Add(normalisedKey, value);
        }

        /// <summary>
        /// Retrieves a stored value by key. Returns string.Empty if not found.
        /// </summary>
        public string Recall(string key)
        {
            string normalisedKey = key.ToLower().Trim();
            return _memoryStore.ContainsKey(normalisedKey) ? _memoryStore[normalisedKey] : string.Empty;
        }

        /// <summary>
        /// Returns true if a value has been stored for the given key.
        /// </summary>
        public bool Has(string key)
        {
            return _memoryStore.ContainsKey(key.ToLower().Trim());
        }

        /// <summary>Convenience method: returns the remembered user name, or string.Empty.</summary>
        public string GetName() => Recall("name");

        /// <summary>Convenience method: returns the remembered favourite topic, or string.Empty.</summary>
        public string GetFavouriteTopic() => Recall("favourite_topic");

        /// <summary>Clears all stored memory (used on session reset).</summary>
        public void Clear() => _memoryStore.Clear();
    }
}
