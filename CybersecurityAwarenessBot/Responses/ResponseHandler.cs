// ResponseHandler.cs
// This class manages all the chatbot's responses.
// It matches user questions to the correct cybersecurity topic
// and returns a relevant, helpful answer.
// Written by: [Your Name]

using System;
using System.Collections.Generic;

namespace CybersecurityAwarenessBot.Responses
{
    public class ResponseHandler
    {
        // We store the user's name so responses feel personal
        private readonly string _userName;

        // A dictionary that maps keywords to responses
        private readonly Dictionary<string, string> _responses;

        public ResponseHandler(string userName)
        {
            _userName = userName;
            _responses = BuildResponseDictionary();
        }

        // Tries to find the best response for what the user typed
        public string GetResponse(string userInput)
        {
            string lowerInput = userInput.ToLower();

            // Check each keyword against what the user said
            foreach (var entry in _responses)
            {
                if (lowerInput.Contains(entry.Key))
                    return entry.Value;
            }

            // If nothing matched, return a default response
            return GetDefaultResponse();
        }

        // All our cybersecurity knowledge lives here
        private Dictionary<string, string> BuildResponseDictionary()
        {
            return new Dictionary<string, string>
            {
                // General chatbot questions
                {
                    "how are you",
                    $"I'm running smoothly and ready to help, {_userName}! My circuits are all green. How can I assist you with cybersecurity today?"
                },
                {
                    "what is your purpose",
                    $"Great question, {_userName}! I'm CyberGuard, your Cybersecurity Awareness Assistant. I'm here to help South Africans stay safe online by educating you on threats like phishing, malware, and social engineering."
                },
                {
                    "what can i ask",
                    "You can ask me about:\n  • Password safety\n  • Phishing scams\n  • Safe browsing habits\n  • Malware and viruses\n  • Social engineering\n  • Two-factor authentication\n  • Public Wi-Fi risks\n  Just type your question!"
                },
                {
                    "help",
                    "Sure! Here are some topics I can help with:\n  • Passwords  • Phishing  • Malware\n  • Safe Browsing  • Social Engineering\n  • Two-Factor Authentication  • Public Wi-Fi\n  Try asking about any of these!"
                },

                // Password safety
                {
                    "password",
                    $"Great topic, {_userName}! A strong password should:\n  ✔ Be at least 12 characters long\n  ✔ Mix uppercase, lowercase, numbers, and symbols\n  ✔ Never use personal info like your birthday\n  ✔ Be unique for every account\n  Tip: Use a password manager like Bitwarden (it's free!) to keep track of them all."
                },

                // Phishing
                {
                    "phishing",
                    $"Phishing is one of the most common cyber threats in South Africa right now, {_userName}.\n  🎣 What is it? Criminals send fake emails or messages pretending to be your bank, SARS, or a trusted brand.\n  🚩 Warning signs:\n  • Urgent language like 'Your account will be closed!'\n  • Links that don't match the real website\n  • Spelling mistakes in official-looking emails\n  🛡️ What to do: Never click suspicious links. Go directly to the website by typing the URL yourself."
                },
                {
                    "email scam",
                    "Email scams are very common! Never click links in unexpected emails. Rather go directly to the website by typing the URL. If an email asks for personal info or payment, it's almost certainly a scam."
                },

                // Safe browsing
                {
                    "safe browsing",
                    $"Here are my top safe browsing tips for you, {_userName}:\n  🔒 Always look for HTTPS (the padlock icon) in the address bar\n  🔒 Avoid clicking pop-up ads\n  🔒 Don't download files from websites you don't trust\n  🔒 Keep your browser updated\n  🔒 Use a reputable browser like Chrome or Firefox"
                },
                {
                    "browse",
                    $"Safe browsing is important, {_userName}! Always check for HTTPS before entering personal details on any website. The padlock icon in your browser means the connection is encrypted."
                },

                // Malware
                {
                    "malware",
                    "Malware is malicious software designed to harm your device or steal your data.\n  Types include:\n  🦠 Viruses - spread by infecting files\n  🔐 Ransomware - locks your files and demands payment\n  🕵️ Spyware - secretly monitors your activity\n  🛡️ Protection tips:\n  • Install reputable antivirus software\n  • Keep your OS and apps updated\n  • Don't open attachments from unknown senders"
                },
                {
                    "virus",
                    "Computer viruses are a type of malware that spread by attaching to files. Always have updated antivirus software installed, and never open files from unknown sources. Windows Defender (built into Windows) is a good free option!"
                },
                {
                    "ransomware",
                    $"Ransomware is extremely dangerous, {_userName}! It encrypts all your files and demands money to unlock them.\n  🛡️ How to stay safe:\n  • Back up your files regularly (use an external drive or cloud)\n  • Never open suspicious email attachments\n  • Keep Windows and your software updated\n  • South African businesses have been major targets - stay alert!"
                },

                // Social engineering
                {
                    "social engineering",
                    $"Social engineering is when criminals manipulate people (not computers) to gain access to systems, {_userName}.\n  Common tactics:\n  🎭 Pretexting - pretending to be someone trustworthy\n  📞 Vishing - phone scams pretending to be your bank\n  💬 Smishing - SMS scams with fake links\n  🛡️ Remember: Your bank will NEVER ask for your PIN or password over the phone!"
                },

                // Two-factor authentication
                {
                    "two-factor",
                    "Two-Factor Authentication (2FA) adds an extra layer of security to your accounts.\n  Even if someone steals your password, they still can't log in without the second factor!\n  ✔ Enable 2FA on your email, banking, and social media\n  ✔ Use an authenticator app like Google Authenticator for stronger security\n  ✔ Avoid SMS-based 2FA if possible - SIM swapping is a real threat in South Africa"
                },
                {
                    "2fa",
                    "Two-Factor Authentication (2FA) is one of the best ways to protect your accounts! Enable it on every account that supports it. An authentication app is safer than SMS codes."
                },
                {
                    "authentication",
                    $"Multi-factor authentication is your best friend, {_userName}! It means even if your password is stolen, attackers still can't access your account. Enable it everywhere you can!"
                },

                // Public Wi-Fi
                {
                    "public wifi",
                    $"Be very careful on public Wi-Fi, {_userName}!\n  🚫 Never do online banking or shopping on public Wi-Fi\n  🚫 Criminals can set up fake hotspots to steal your data (a 'man-in-the-middle' attack)\n  🛡️ What to do:\n  ✔ Use a VPN (Virtual Private Network) when on public Wi-Fi\n  ✔ Turn off automatic Wi-Fi connections on your phone\n  ✔ Use your mobile data for sensitive tasks"
                },
                {
                    "wifi",
                    "Public Wi-Fi is a major security risk! Hackers can intercept your data. Use a VPN if you must use public Wi-Fi, and never access banking or sensitive accounts on it."
                },

                // South Africa specific
                {
                    "south africa",
                    "South Africa is one of the most targeted countries for cybercrime in Africa! The Department of Cybersecurity has been working hard to raise awareness. Common local threats include:\n  • SIM swapping attacks\n  • SARS and bank-related phishing emails\n  • Business Email Compromise (BEC) scams\n  Stay informed and stay safe!"
                },
                {
                    "sim swap",
                    $"SIM swapping is a serious threat in South Africa, {_userName}! Criminals convince your mobile network to transfer your number to their SIM, giving them access to your SMS-based 2FA codes.\n  🛡️ Protection:\n  • Use authenticator apps instead of SMS for 2FA\n  • Set a PIN with your mobile provider\n  • Contact your provider immediately if your phone loses signal unexpectedly"
                }
            };
        }

        // What we say when we don't understand the user's input
        private string GetDefaultResponse()
        {
            return $"I'm not sure I understand that, {_userName}. I'm still learning!\n  You can ask me about:\n  • Passwords  • Phishing  • Safe Browsing\n  • Malware  • Social Engineering  • Public Wi-Fi\n  Type 'help' to see all topics, or try rephrasing your question!";
        }
    }
}