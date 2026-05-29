// ResponseHandler.cs
// This class manages all the chatbot's responses.
// It matches user questions to the correct cybersecurity topic
// and returns a relevant, helpful answer.
// Written by: [Lindokuhle Mtshali]

using System;
using System.Collections.Generic;
using CybersecurityAwarenessBot.Memory;
using CybersecurityAwarenessBot.Sentiment;

namespace CybersecurityAwarenessBot.Responses
{
    public class ResponseHandler
    {
        // ── Fields ──────────────────────────────────────────────────────────────
        private readonly UserMemory _memory;
        private readonly SentimentDetector _sentimentDetector;
        private readonly Random _random;

        // Tracks the last topic discussed so we can handle follow-ups
        private string _lastTopic = "";

        // Single responses per keyword (Dictionary - Part 1 structure retained)
        private readonly Dictionary<string, string> _responses;

        // Random response pools for topics that support variation (List of strings)
        private readonly Dictionary<string, List<string>> _randomResponses;

        // Follow-up triggers that let the user continue on the same topic
        private readonly List<string> _followUpTriggers;

        // ── Constructor ─────────────────────────────────────────────────────────
        public ResponseHandler(UserMemory memory)
        {
            _memory = memory;
            _sentimentDetector = new SentimentDetector();
            _random = new Random();
            _responses = BuildResponseDictionary();
            _randomResponses = BuildRandomResponseDictionary();
            _followUpTriggers = BuildFollowUpTriggers();
        }

        // ── Public Method ────────────────────────────────────────────────────────

        /// <summary>
        /// Main entry point. Takes the user's raw input and returns
        /// a full, context-aware, sentiment-adjusted response string.
        /// </summary>
        public string GetResponse(string userInput)
        {
            string userName = _memory.GetName() ?? "there";
            string lower = userInput.ToLower().Trim();

            // Detect sentiment and build prefix
            SentimentDetector.SentimentType sentiment = _sentimentDetector.Detect(lower);
            string sentimentPrefix = _sentimentDetector.GetSentimentPrefix(sentiment, userName);

            // --- Step 1: Check for memory-storing inputs ---
            string memoryResponse = TryExtractAndRemember(lower, userName);
            if (memoryResponse != null)
                return sentimentPrefix + memoryResponse;

            // --- Step 2: Handle follow-up requests (e.g. "tell me more", "explain more") ---
            if (IsFollowUp(lower) && !string.IsNullOrEmpty(_lastTopic))
                return sentimentPrefix + GetFollowUpResponse(userName);

            // --- Step 3: Check random response pools first (higher variation value) ---
            foreach (var entry in _randomResponses)
            {
                if (lower.Contains(entry.Key))
                {
                    _lastTopic = entry.Key;
                    // Randomly pick one response from the list
                    string randomPick = entry.Value[_random.Next(entry.Value.Count)];
                    return sentimentPrefix + randomPick;
                }
            }

            // --- Step 4: Check standard keyword dictionary ---
            foreach (var entry in _responses)
            {
                if (lower.Contains(entry.Key))
                {
                    _lastTopic = entry.Key;
                    return sentimentPrefix + entry.Value;
                }
            }

            // --- Step 5: Personalise default response using memory ---
            return sentimentPrefix + GetDefaultResponse(userName);
        }

        // ── Private Helpers ──────────────────────────────────────────────────────

        /// <summary>
        /// Detects if the user is sharing personal info (name, favourite topic)
        /// and stores it in memory. Returns an acknowledgment response or null.
        /// </summary>
        private string TryExtractAndRemember(string lower, string userName)
        {
            // Detect "my name is X"
            if (lower.Contains("my name is "))
            {
                string newName = ExtractFirstWordAfter(lower, "my name is ");
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    // Capitalise first letter
                    newName = char.ToUpper(newName[0]) + newName.Substring(1).ToLower();
                    _memory.Remember("name", newName);
                    return $"Nice to meet you, {newName}! I'll remember that for our conversation. 😊 " +
                           $"How can I help you with cybersecurity today?";
                }
            }

            // Detect "I'm interested in X" / "I like X" / "my favourite topic is X"
            if (lower.Contains("i'm interested in ") || lower.Contains("im interested in ") ||
                lower.Contains("i am interested in ") || lower.Contains("my favourite topic is ") ||
                lower.Contains("i like "))
            {
                string topic = ExtractTopicOfInterest(lower);
                if (topic != null)
                {
                    _memory.Remember("favourite_topic", topic);
                    _lastTopic = topic;
                    return $"Great! I'll remember that you're interested in {topic}. " +
                           $"It's a crucial part of staying safe online.\n\n" +
                           $"Here's something relevant: {GetTopicSnippet(topic, userName)}";
                }
            }

            return null;
        }

        /// <summary>Extracts text after common "interest" phrases.</summary>
        private string ExtractTopicOfInterest(string lower)
        {
            string[] markers = {
                "i'm interested in ", "im interested in ",
                "i am interested in ", "my favourite topic is ", "i like "
            };
            foreach (string marker in markers)
            {
                if (lower.Contains(marker))
                {
                    int start = lower.IndexOf(marker) + marker.Length;
                    string topic = lower.Substring(start).Trim().TrimEnd('.', '!', '?');
                    if (topic.Length > 1) return topic;
                }
            }
            return null;
        }

        /// <summary>Extracts the first word after a marker phrase.</summary>
        private string ExtractFirstWordAfter(string lower, string marker)
        {
            int start = lower.IndexOf(marker) + marker.Length;
            return lower.Substring(start).Trim().TrimEnd('.', '!', '?').Split(' ')[0];
        }

        /// <summary>Returns a short snippet about a topic for use in memory acknowledgment.</summary>
        private string GetTopicSnippet(string topic, string userName)
        {
            if (topic.Contains("password"))
                return $"Passwords should be at least 12 characters and unique per account, {userName}.";
            if (topic.Contains("phishing"))
                return $"Always verify the sender's email address before clicking any links, {userName}.";
            if (topic.Contains("privacy"))
                return $"Review your social media privacy settings regularly, {userName}.";
            if (topic.Contains("malware") || topic.Contains("virus"))
                return $"Keep your antivirus updated and never open attachments from unknown senders, {userName}.";
            return $"Staying informed about {topic} is one of the best things you can do for your online safety, {userName}!";
        }

        /// <summary>Returns true if the input is a follow-up request.</summary>
        private bool IsFollowUp(string lower)
        {
            foreach (string trigger in _followUpTriggers)
                if (lower.Contains(trigger)) return true;
            return false;
        }

        /// <summary>Returns extended information on the last discussed topic.</summary>
        private string GetFollowUpResponse(string userName)
        {
            string favTopic = _memory.GetFavouriteTopic();
            string topic = !string.IsNullOrEmpty(favTopic) ? favTopic : _lastTopic;

            string extra = _followUpDetails.ContainsKey(topic)
                ? _followUpDetails[topic]
                : $"Here's a bit more on {topic}: " +
                  "Always stay sceptical online, verify sources, and keep your software updated. " +
                  "These habits protect you against the vast majority of cyber threats.";

            return $"Sure, here's more on {topic}, {userName}:\n\n{extra}";
        }

        /// <summary>Default response when no keyword matched, using memory if available.</summary>
        private string GetDefaultResponse(string userName)
        {
            string favTopic = _memory.GetFavouriteTopic();
            string topicHint = favTopic != null
                ? $" Since you're interested in {favTopic}, you might want to ask me more about that!"
                : "";

            return $"I'm not sure I understand that, {userName}. I'm still learning!\n" +
                   $"You can ask me about:\n" +
                   $"  • Passwords  • Phishing  • Safe Browsing\n" +
                   $"  • Malware  • Social Engineering  • Public Wi-Fi\n" +
                   $"  • Two-Factor Authentication  • SIM Swapping\n" +
                   $"Type 'help' to see all topics, or try rephrasing your question!{topicHint}";
        }

        // ── Data Builders ────────────────────────────────────────────────────────

        /// <summary>Follow-up trigger phrases (Requirement 4 - Conversation Flow).</summary>
        private List<string> BuildFollowUpTriggers()
        {
            return new List<string>
            {
                "tell me more", "give me another tip", "explain more",
                "more info", "more information", "elaborate", "go on",
                "continue", "and then", "what else", "anything else",
                "keep going", "more please", "give me more"
            };
        }

        /// <summary>Extended follow-up detail per topic.</summary>
        private readonly Dictionary<string, string> _followUpDetails = new Dictionary<string, string>
        {
            {
                "password",
                "Advanced password tips:\n" +
                "   Use a passphrase: combine 4 random words like 'coffee-lamp-river-cloud'\n" +
                "   Never reuse passwords across sites - a breach on one exposes all\n" +
                "   Change passwords immediately if you suspect a breach\n" +
                "   Password managers like Bitwarden or KeePass generate and store strong passwords for you"
            },
            {
                "phishing",
                "Advanced phishing awareness:\n" +
                "   Spear phishing targets you personally using your name/employer\n" +
                "   Whaling targets executives and managers specifically\n" +
                "   Clone phishing duplicates a legitimate email but swaps the link\n" +
                "   Always hover over links before clicking to see the real URL\n" +
                "   Report suspicious emails to your IT department or to the SA Cybercrime Hub"
            },
            {
                "malware",
                "More on malware types:\n" +
                "   Trojans disguise themselves as legitimate software\n" +
                "   Keyloggers record every keystroke to steal passwords\n" +
                "   Adware bombards you with ads and may slow your device\n" +
                "   Regularly scan your device with Malwarebytes (free version available)\n" +
                "   Avoid pirated software - it's a primary malware distribution method"
            },
            {
                "wifi",
                "More on Wi-Fi safety:\n" +
                "   Evil Twin attacks: criminals create a hotspot named 'Coffee Shop Wi-Fi' to trick you\n" +
                "   Packet sniffing lets attackers read unencrypted data on the same network\n" +
                "   Always use a VPN - ProtonVPN has a solid free tier\n" +
                "   Disable file sharing and turn on your firewall on public networks"
            },
            {
                "privacy",
                "More on online privacy:\n" +
                "   Use a privacy-focused browser like Brave or Firefox with uBlock Origin\n" +
                "   Disable location permissions for apps that don't need them\n" +
                "   Review app permissions on your phone monthly\n" +
                "   Use DuckDuckGo instead of Google to reduce tracking"
            }
        };

        /// <summary>
        /// Random response pools for topics requiring variation (Requirement 3).
        /// Each key maps to a List of possible responses.
        /// </summary>
        private Dictionary<string, List<string>> BuildRandomResponseDictionary()
        {
            return new Dictionary<string, List<string>>
            {
                {
                    "phishing tip", new List<string>
                    {
                        " Phishing Tip: Be cautious of emails asking for personal information. " +
                        "Scammers often disguise themselves as trusted organisations like your bank or SARS.",

                        " Phishing Tip: Check the sender's actual email address — not just the display name. " +
                        "A scam email might show 'ABSA Bank' but the address is random@fakesite.com.",

                        " Phishing Tip: Hover over links before clicking. If the URL looks suspicious " +
                        "or doesn't match the company's real website, do NOT click it.",

                        " Phishing Tip: Legitimate companies never ask for your password, PIN, or OTP " +
                        "via email or SMS. If they do, it's a scam — report it immediately.",

                        " Phishing Tip: Watch for urgent language like 'Your account will be suspended!' " +
                        "Criminals use urgency to stop you from thinking critically."
                    }
                },
                {
                    "scam", new List<string>
                    {
                        " Scam Alert: If someone contacts you unexpectedly asking for money or gift cards, " +
                        "it is almost certainly a scam. Hang up or delete the message.",

                        " Scam Alert: The 'Nigerian Prince' scam still works because it exploits generosity. " +
                        "Never transfer money to strangers, no matter how convincing their story sounds.",

                        " Scam Alert: Romance scams are rising in South Africa. If someone you met online " +
                        "is asking for money, they are almost certainly not who they say they are.",

                        " Scam Alert: Job scams promise high-paying remote work then ask you to pay upfront " +
                        "for 'training' or 'equipment'. Legitimate employers never charge you to work."
                    }
                },
                {
                    "privacy", new List<string>
                    {
                        " Privacy Tip: Review your Facebook and Instagram privacy settings today. " +
                        "Limit who can see your posts, phone number, and location.",

                        " Privacy Tip: Use a different email address for online shopping and newsletters " +
                        "to protect your main account from spam and data breaches.",

                        " Privacy Tip: Check if your email has been in a data breach at haveibeenpwned.com " +
                        "— it's free and could reveal if your passwords are compromised.",

                        " Privacy Tip: Disable personalised ads in your Google and Facebook account settings. " +
                        "You'll see fewer targeted ads and share less data with advertisers."
                    }
                },
                {
                    "password tip", new List<string>
                    {
                        " Password Tip: Use a passphrase — four random words like 'mountain-river-cloud-door' " +
                        "are easier to remember and harder to crack than 'P@ssw0rd1'.",

                        " Password Tip: Never use your name, ID number, or birthday in a password. " +
                        "Hackers try these first.",

                        " Password Tip: Enable two-factor authentication on top of your password — " +
                        "it's your safety net if your password ever gets stolen.",

                        " Password Tip: A password manager like Bitwarden (free!) remembers all your " +
                        "passwords so you only need to remember one master password."
                    }
                }
            };
        }

        /// <summary>
        /// Standard single-response keyword dictionary.
        /// Carried over and expanded from Part 1.
        /// Note: userName is resolved at GetResponse() call time, not here.
        /// </summary>
        private Dictionary<string, string> BuildResponseDictionary()
        {
            // Responses are static strings here; personalisation is
            // added by the sentiment prefix at call time.

            return new Dictionary<string, string>
            {
                // ── Greeting / Meta ──────────────────────────────────────────────
                {
                    "how are you",
                    $"I'm running smoothly and ready to help! My circuits are all green. " +
                    $"How can I assist you with cybersecurity today?"
                },
                {
                    "what is your purpose",
                    "I'm CyberGuard, your Cybersecurity Awareness Assistant! " +
                    "I'm here to help South Africans stay safe online by educating you on threats " +
                    "like phishing, malware, and social engineering."
                },
                {
                    "help",
                    "Here are the topics I can help with:\n" +
                    "   Passwords          Phishing\n" +
                    "   Malware            Safe Browsing\n" +
                    "   Social Engineering   Public Wi-Fi\n" +
                    "   Two-Factor Auth    SIM Swapping\n" +
                    "   Privacy            Scams\n\n" +
                    "You can also say things like:\n" +
                    "  • 'Give me a phishing tip'\n" +
                    "  • 'Tell me about password safety'\n" +
                    "  • 'I'm worried about online scams'\n" +
                    "  • 'Tell me more' (to continue on the last topic)"
                },
 
                // ── Password Safety ──────────────────────────────────────────────
                {
                    "password",
                    "A strong password should:\n" +
                    "   Be at least 12 characters long\n" +
                    "   Mix uppercase, lowercase, numbers, and symbols\n" +
                    "   Never use personal info like your birthday or name\n" +
                    "   Be unique for every single account\n\n" +
                    " Tip: Use a password manager like Bitwarden (free!) to generate and store " +
                    "strong passwords automatically.\n\n" +
                    "Say 'tell me more' for advanced password tips!"
                },
 
                // ── Phishing ─────────────────────────────────────────────────────
                {
                    "phishing",
                    "Phishing is one of the most common cyber threats in South Africa right now.\n" +
                    "   What is it? Criminals send fake emails pretending to be your bank, SARS, or a trusted brand.\n\n" +
                    "   Warning signs:\n" +
                    "  • Urgent language: 'Your account will be closed!'\n" +
                    "  • Links that don't match the real website\n" +
                    "  • Spelling mistakes in official-looking emails\n\n" +
                    "   What to do: Never click suspicious links. Type the URL yourself.\n\n" +
                    "Say 'give me a phishing tip' for random tips, or 'tell me more' for advanced info!"
                },
                {
                    "email scam",
                    "Email scams are very common! Never click links in unexpected emails. " +
                    "Go directly to the website by typing the URL. " +
                    "If an email asks for personal info or payment, it's almost certainly a scam."
                },
 
                // ── Safe Browsing ────────────────────────────────────────────────
                {
                    "safe browsing",
                    "Top safe browsing tips:\n" +
                    "   Always look for HTTPS (the padlock icon) in the address bar\n" +
                    "   Avoid clicking pop-up ads\n" +
                    "   Don't download files from websites you don't trust\n" +
                    "   Keep your browser updated\n" +
                    "   Use a reputable browser like Firefox or Brave for extra privacy"
                },
                {
                    "browse",
                    "Safe browsing is important! Always check for HTTPS before entering personal details. " +
                    "The padlock icon means the connection is encrypted."
                },
 
                // ── Malware ──────────────────────────────────────────────────────
                {
                    "malware",
                    "Malware is malicious software designed to harm your device or steal your data.\n\n" +
                    "  Types include:\n" +
                    "   Viruses — spread by infecting files\n" +
                    "   Ransomware — locks your files and demands payment\n" +
                    "   Spyware — secretly monitors your activity\n" +
                    "   Trojans — disguise themselves as legitimate software\n\n" +
                    "   Protection:\n" +
                    "  • Install reputable antivirus software\n" +
                    "  • Keep your OS and apps updated\n" +
                    "  • Don't open attachments from unknown senders\n\n" +
                    "Say 'tell me more' for deeper malware tips!"
                },
                {
                    "virus",
                    "Computer viruses spread by attaching to files. Always have updated antivirus software " +
                    "installed, and never open files from unknown sources. " +
                    "Windows Defender (built into Windows) is a solid free option!"
                },
                {
                    "ransomware",
                    "Ransomware is extremely dangerous! It encrypts all your files and demands money to unlock them.\n\n" +
                    "   How to stay safe:\n" +
                    "  • Back up your files regularly — use an external drive OR cloud storage\n" +
                    "  • Never open suspicious email attachments\n" +
                    "  • Keep Windows and all software updated\n" +
                    "  • South African businesses have been major targets — stay alert!"
                },
 
                // ── Social Engineering ───────────────────────────────────────────
                {
                    "social engineering",
                    "Social engineering manipulates people (not computers) to gain access to systems.\n\n" +
                    "  Common tactics:\n" +
                    "   Pretexting — pretending to be someone trustworthy\n" +
                    "   Vishing — phone scams pretending to be your bank\n" +
                    "   Smishing — SMS scams with fake links\n\n" +
                    "   Remember: Your bank will NEVER ask for your PIN or password over the phone!"
                },
 
                // ── Two-Factor Authentication ────────────────────────────────────
                {
                    "two-factor",
                    "Two-Factor Authentication (2FA) adds an extra security layer to your accounts.\n" +
                    "Even if someone steals your password, they still can't log in without the second factor!\n\n" +
                    "   Enable 2FA on your email, banking, and social media\n" +
                    "   Use an authenticator app (Google Authenticator, Authy) for stronger security\n" +
                    "   Avoid SMS-based 2FA if possible — SIM swapping is a real threat in South Africa"
                },
                {
                    "2fa",
                    "2FA is one of the best ways to protect your accounts! " +
                    "Enable it on every account that supports it. " +
                    "An authentication app is safer than SMS codes."
                },
                {
                    "authentication",
                    "Multi-factor authentication is your best friend! " +
                    "It means even if your password is stolen, attackers still can't access your account. " +
                    "Enable it everywhere you can!"
                },
 
                // ── Public Wi-Fi ─────────────────────────────────────────────────
                {
                    "public wifi",
                    "Be very careful on public Wi-Fi!\n" +
                    "   Never do online banking or shopping on public Wi-Fi\n" +
                    "   Criminals can set up fake hotspots to steal your data (man-in-the-middle attack)\n\n" +
                    "   What to do:\n" +
                    "   Use a VPN (ProtonVPN has a free tier)\n" +
                    "   Turn off automatic Wi-Fi connections on your phone\n" +
                    "   Use your mobile data for sensitive tasks\n\n" +
                    "Say 'tell me more' for advanced Wi-Fi attack details!"
                },
                {
                    "wifi",
                    "Public Wi-Fi is a major security risk! Hackers can intercept your data. " +
                    "Use a VPN if you must use public Wi-Fi, and never access banking or sensitive accounts on it."
                },
 
                // ── South Africa Specific ────────────────────────────────────────
                {
                    "south africa",
                    "South Africa is one of the most targeted countries for cybercrime in Africa!\n" +
                    "Common local threats include:\n" +
                    "  • SIM swapping attacks\n" +
                    "  • SARS and bank-related phishing emails\n" +
                    "  • Business Email Compromise (BEC) scams\n" +
                    "  • Online shopping fraud\n" +
                    "Stay informed and stay safe! 🇿🇦"
                },
                {
                    "sim swap",
                    "SIM swapping is a serious threat in South Africa! Criminals convince your mobile " +
                    "network to transfer your number to their SIM, giving them access to your SMS-based 2FA codes.\n\n" +
                    "  Protection:\n" +
                    "  • Use authenticator apps instead of SMS for 2FA\n" +
                    "  • Set a port-out PIN with your mobile provider\n" +
                    "  • Contact your provider immediately if your phone loses signal unexpectedly"
                },
 
                // ── Privacy ──────────────────────────────────────────────────────
                {
                    "privacy",
                    "Online privacy matters more than ever!\n" +
                    "  Review your social media privacy settings\n" +
                    "  Limit the personal information you share publicly\n" +
                    "  Use a VPN to mask your IP address\n" +
                    "  Check haveibeenpwned.com to see if your email was in a breach\n\n" +
                    "Say 'give me a privacy tip' or 'tell me more' for more detailed advice!"
                }
            };
        }
    }
}