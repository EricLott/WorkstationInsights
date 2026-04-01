using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.SemanticKernel.ChatCompletion;

namespace WorkstationInsights.Services
{
    public class ChatLogger
    {
        private const string LogDirectory = "ChatLogs";
        private const string LogFileExtension = ".json";

        public static string GetLogPath(string threadName)
        {
            if (!Directory.Exists(LogDirectory)) Directory.CreateDirectory(LogDirectory);
            return Path.Combine(LogDirectory, threadName + LogFileExtension);
        }

        public static void SaveChatLog(string filePath, ChatHistory history)
        {
            var data = new { ModelId = "gpt-4o", Messages = history };
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public static ChatHistory LoadChatFromLog(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            var json = File.ReadAllText(filePath);
            var logData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            
            var history = new ChatHistory();
            if (logData != null && logData.ContainsKey("Messages"))
            {
                var messagesElement = (JsonElement)logData["Messages"];
                foreach (var msg in messagesElement.EnumerateArray())
                {
                    string roleStr = msg.GetProperty("Role").GetProperty("Label").GetString();
                    string content = msg.GetProperty("Content").GetString();
                    var role = new AuthorRole(roleStr);
                    history.AddMessage(role, content);
                }
            }
            return history;
        }

        public static Dictionary<string, string> ListThreads()
        {
            var result = new Dictionary<string, string>();
            if (!Directory.Exists(LogDirectory)) return result;

            foreach (var file in Directory.GetFiles(LogDirectory, "*" + LogFileExtension))
            {
                var displayName = Path.GetFileNameWithoutExtension(file);
                result[displayName] = file;
            }
            return result;
        }
    }
}
