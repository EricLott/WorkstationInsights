using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion; // AuthorRole is in this namespace
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text.Json;
using System.IO; // Added for file operations
using System.Collections.Generic; // Required for List
using System; // Required for DateTime

namespace WorkstationInsights
{
    // Helper class for JSON serialization
    public class SerializableChatMessage
    {
        public string Role { get; set; } // "User", "Assistant", "Tool", "System"
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public partial class Form1 : Form
    {
        private string currentLogFilePath; // To store the path of the current log file
        private const string LogDirectory = "ChatLogs"; // Directory to store log files
        private const string LogFileExtension = ".json"; // New extension
        private Dictionary<string, string> threadFileMap = new Dictionary<string, string>(); // Maps display name to file path

        public Form1()
        {
            InitializeComponent();
            InitializeChatLogging(); // Initialize logging when the form loads
            LoadChatThreads(); // Load existing chat threads into the ListBox
            this.threadsListBox.SelectedIndexChanged += new System.EventHandler(this.ThreadsListBox_SelectedIndexChanged); // Wire up event handler
        }

        private void InitializeChatLogging()
        {
            // Ensure the log directory exists
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }

            // Start a new log file for the new session
            StartNewLogFile();
        }

        private void StartNewLogFile(bool isNewSession = true)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            currentLogFilePath = Path.Combine(LogDirectory, $"chatlog_{timestamp}{LogFileExtension}");

            // Initialize the JSON file with an empty array
            // This helps in appending new messages as elements of a JSON array
            if (isNewSession || !File.Exists(currentLogFilePath))
            {
                File.WriteAllText(currentLogFilePath, "[]");
            }
        }

        private void LogMessage(AuthorRole role, string messageContent)
        {
            if (string.IsNullOrEmpty(currentLogFilePath))
            {
                // This case might occur if a new chat is started, but no message has been sent yet.
                // We need a log file path to save the message.
                StartNewLogFile(isNewSession: true); // Ensure a log file is ready
            }

            // Ensure the log file exists and is initialized, especially if StartNewLogFile wasn't called before the first message
            if (!File.Exists(currentLogFilePath))
            {
                 File.WriteAllText(currentLogFilePath, "[]"); // Initialize with empty array
            }


            var logEntry = new SerializableChatMessage
            {
                Role = role.ToString(),
                Content = messageContent,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                // Read existing entries
                var existingJson = File.ReadAllText(currentLogFilePath);
                var messages = JsonSerializer.Deserialize<List<SerializableChatMessage>>(existingJson) ?? new List<SerializableChatMessage>();

                bool isFirstUserMessageInFile = role == AuthorRole.User && !messages.Any(m => m.Role == AuthorRole.User.ToString());

                // Add new entry
                messages.Add(logEntry);

                // Write all entries back
                var updatedJson = JsonSerializer.Serialize(messages, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(currentLogFilePath, updatedJson);

                // If this was the first user message, this "solidifies" the new thread, so refresh the list
                if (isFirstUserMessageInFile)
                {
                    // Check if this log file is already in threadFileMap, if not, it's a new thread that needs adding.
                    // This handles the case where "New Chat" was clicked, then the first message is sent.
                    if (!threadFileMap.ContainsValue(currentLogFilePath))
                    {
                         LoadChatThreads(); // Reload all threads to include the new one and select it
                        // Select the newly added thread. This is a bit tricky as LoadChatThreads rebuilds the list.
                        // We need to find it again.
                        var newThreadName = messages.First(m => m.Role == AuthorRole.User.ToString()).Content;
                        newThreadName = newThreadName.Length > 30 ? newThreadName.Substring(0, 30) + "..." : newThreadName;

                        string uniqueNameInList = FindThreadNameInListBox(newThreadName, currentLogFilePath);
                        if(uniqueNameInList != null)
                        {
                           threadsListBox.SelectedItem = uniqueNameInList;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to JSON log file: {ex.Message}");
                // Consider how to handle this error in the UI, e.g., status bar message
            }
        }

        // Helper to find the actual display name in ListBox given a base name and file path
        private string FindThreadNameInListBox(string baseName, string filePath)
        {
            foreach (var item in threadsListBox.Items)
            {
                string itemName = item.ToString();
                if (threadFileMap.TryGetValue(itemName, out string path) && path == filePath)
                {
                    return itemName;
                }
            }
            // Fallback if uniqueness added numbers, check baseName prefix
            // This part could be more robust if needed by checking map directly
            if (threadFileMap.ContainsKey(baseName) && threadFileMap[baseName] == filePath) return baseName;

            // Try with potential suffixes (1), (2) etc.
            for(int i=1; i < 100; i++) // Max 99 duplicates, adjust if necessary
            {
                string potentialName = $"{baseName} ({i})";
                if (threadFileMap.ContainsKey(potentialName) && threadFileMap[potentialName] == filePath) return potentialName;
            }
            return null;
        }

        private ChatHistory chatHistory = new(); // persist across messages

        private async void SendButton_Click(object sender, EventArgs e)
        {
            var input = inputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            chatHistoryTextBox.AppendText($"> {input}{Environment.NewLine}");
            chatHistory.AddUserMessage(input);
            LogMessage(AuthorRole.User, input);

            try
            {
                // var chatService = Program.KernelInstance.GetRequiredService<IChatCompletionService>(); // Already available via Program.ChatService

                var result = await Program.ChatService.GetChatMessageContentAsync(
                    chatHistory,
                    new OpenAIPromptExecutionSettings
                    {
                        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
                    },
                    Program.KernelInstance
                );

                if (result.Role == AuthorRole.Tool)
                {
                    var toolContent = result.Content ?? "No content from tool.";
                    chatHistoryTextBox.AppendText($"[Tool Response]: {toolContent}{Environment.NewLine}");
                    // Note: The Semantic Kernel automatically adds tool messages to chatHistory.
                    // We might want to log it explicitly here or rely on history reconstruction if tools are complex.
                    // For simplicity, logging it directly.
                    LogMessage(AuthorRole.Tool, toolContent);
                }
                else // Assistant message
                {
                    var aiContent = result.Content ?? "";
                    AppendMarkdownMessage("AI", aiContent);
                    chatHistory.AddAssistantMessage(aiContent); // SK adds this, but good to be explicit if we were managing history manually
                    LogMessage(AuthorRole.Assistant, aiContent);
                }
            }
            catch (Exception ex)
            {
                chatHistoryTextBox.AppendText($"[Error]: {ex.Message}{Environment.NewLine}");
                // Logging errors with a "System" role or a dedicated "Error" role might be an option.
                // Using "System" for now as it's a general status/error.
                LogMessage(AuthorRole.System, $"Error: {ex.Message}");
            }

            inputTextBox.Clear();
        }

        private void AppendMarkdownMessage(string sender, string markdown)
        {
            chatHistoryTextBox.SelectionStart = chatHistoryTextBox.TextLength;
            chatHistoryTextBox.SelectionFont = new Font("Segoe UI", 10F, FontStyle.Bold);
            chatHistoryTextBox.AppendText($"{sender}: ");
            chatHistoryTextBox.SelectionFont = new Font("Segoe UI", 10F, FontStyle.Regular);

            var plain = markdown
                .Replace("**", "")   // strip bold markers
                .Replace("__", "")   // strip underline markers
                .Replace("`", "'");  // convert inline code to single quotes

            chatHistoryTextBox.AppendText(plain + Environment.NewLine);
            chatHistoryTextBox.ScrollToCaret();
        }

        private void NewChatButton_Click(object sender, EventArgs e)
        {
            chatHistoryTextBox.Clear();
            chatHistory.Clear();
            StartNewLogFile(isNewSession: true);
            threadsListBox.ClearSelected(); // Deselect any currently selected thread in the list
            inputTextBox.Focus(); // Set focus to the input text box for a new message
            // LogMessage(AuthorRole.System, "New chat session started."); // This will be logged with the first actual message if needed, or we can add it.
            // For now, a new empty log file is created. The session "officially" starts with the first message.
            // If we want to add this to the thread list immediately, we'd need a different approach.
            // The current plan is to add to thread list after first user message.
        }

        private void LoadChatThreads()
        {
            threadsListBox.Items.Clear();
            threadFileMap.Clear();

            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory); // Ensure it exists
                return;
            }

            var logFiles = Directory.GetFiles(LogDirectory, $"*{LogFileExtension}")
                                .OrderByDescending(f => new FileInfo(f).CreationTime) // Show newest first
                                .ToList();

            foreach (var logFile in logFiles)
            {
                try
                {
                    var json = File.ReadAllText(logFile);
                    if (string.IsNullOrWhiteSpace(json) || json == "[]") // Skip empty or uninitialized logs
                    {
                        // Optionally delete empty/invalid log files here
                        // Console.WriteLine($"Skipping empty log file: {logFile}");
                        continue;
                    }

                    var messages = JsonSerializer.Deserialize<List<SerializableChatMessage>>(json);
                    if (messages != null && messages.Count > 0)
                    {
                        var firstUserMessage = messages.FirstOrDefault(m => m.Role == AuthorRole.User.ToString());
                        string threadName = Path.GetFileNameWithoutExtension(logFile); // Fallback name

                        if (firstUserMessage != null && !string.IsNullOrWhiteSpace(firstUserMessage.Content))
                        {
                            threadName = firstUserMessage.Content.Length > 30
                                ? firstUserMessage.Content.Substring(0, 30) + "..."
                                : firstUserMessage.Content;
                        }
                        else // If no user message, use file name or a generic name
                        {
                             // Use file creation time for a more descriptive default name
                            threadName = $"Chat {new FileInfo(logFile).CreationTime:yyyy-MM-dd HH-mm-ss}";
                        }

                        // Ensure thread name is unique in the listbox
                        int counter = 1;
                        string originalThreadName = threadName;
                        while (threadsListBox.Items.Contains(threadName) || threadFileMap.ContainsKey(threadName))
                        {
                            threadName = $"{originalThreadName} ({counter++})";
                        }

                        threadsListBox.Items.Add(threadName);
                        threadFileMap[threadName] = logFile;
                    }
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine($"Error parsing JSON from log file {logFile}: {jsonEx.Message}");
                    // Consider how to handle corrupted log files (e.g., rename, move to an error folder)
                }
                catch (Exception ex) // Catch other potential errors like file access issues
                {
                    Console.WriteLine($"Error processing log file {logFile}: {ex.Message}");
                }
            }
        }

        private void ThreadsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (threadsListBox.SelectedItem == null) return;

            string selectedThreadName = threadsListBox.SelectedItem.ToString();
            if (threadFileMap.TryGetValue(selectedThreadName, out string logFilePath))
            {
                currentLogFilePath = logFilePath; // Set as current log
                chatHistory.Clear();
                chatHistoryTextBox.Clear();

                try
                {
                    var json = File.ReadAllText(logFilePath);
                    var messages = JsonSerializer.Deserialize<List<SerializableChatMessage>>(json);

                    if (messages != null)
                    {
                        foreach (var msg in messages)
                        {
                            var role = msg.Role.ToLower() switch
                            {
                                "user" => Microsoft.SemanticKernel.ChatCompletion.AuthorRole.User,
                                "assistant" => Microsoft.SemanticKernel.ChatCompletion.AuthorRole.Assistant,
                                "system" => Microsoft.SemanticKernel.ChatCompletion.AuthorRole.System,
                                "tool" => Microsoft.SemanticKernel.ChatCompletion.AuthorRole.Tool,
                                _ => Microsoft.SemanticKernel.ChatCompletion.AuthorRole.System
                            };

                            // Fix: Use the correct overload of AddMessage
                            chatHistory.AddMessage(role, msg.Content);

                            // Repopulate UI (simplified, could be more robust)
                            if (role == AuthorRole.User)
                            {
                                chatHistoryTextBox.AppendText($"> {msg.Content}{Environment.NewLine}");
                            }
                            else if (role == AuthorRole.Assistant)
                            {
                                AppendMarkdownMessage("AI", msg.Content);
                            }
                            else if (role == AuthorRole.Tool)
                            {
                                chatHistoryTextBox.AppendText($"[Tool Response]: {msg.Content}{Environment.NewLine}");
                            }
                            else if (role == AuthorRole.System && msg.Content.StartsWith("Error:"))
                            {
                                chatHistoryTextBox.AppendText($"[{msg.Role}]: {msg.Content}{Environment.NewLine}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading chat thread: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void ApiKeyButton_Click(object sender, EventArgs e)
        {
            var input = Microsoft.VisualBasic.Interaction.InputBox("Enter your OpenAI API Key:", "API Key", "");
            if (string.IsNullOrWhiteSpace(input)) return;

            // Validate the API key by calling OpenAI's /v1/models
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", input);

            try
            {
                var response = await http.GetAsync("https://api.openai.com/v1/models");
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Invalid API Key: {response.StatusCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error validating key: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load or create config
            string configPath = "appsettings.json";
            Dictionary<string, object> config;
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                config = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                         ?? new Dictionary<string, object>();
            }
            else
            {
                config = new Dictionary<string, object>();
            }

            // Update OpenAI section
            if (!config.ContainsKey("OpenAI"))
                config["OpenAI"] = new Dictionary<string, object>();

            var openAiSection = (System.Text.Json.JsonElement)config["OpenAI"];
            var openAiDict = openAiSection.ValueKind == System.Text.Json.JsonValueKind.Object
                ? System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(openAiSection.GetRawText())
                : new Dictionary<string, object>();

            openAiDict["ApiKey"] = input;
            config["OpenAI"] = openAiDict;

            // Save updated config
            var updatedJson = System.Text.Json.JsonSerializer.Serialize(config, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(configPath, updatedJson);
            MessageBox.Show("API key validated and saved to appsettings.json.");
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Settings are not implemented yet.");
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sendButton.PerformClick();
                e.SuppressKeyPress = true; // Prevent the Enter key from being processed further (e.g., adding a newline)
            }
        }
    }
}
