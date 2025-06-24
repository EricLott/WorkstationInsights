using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text.Json;
using System.IO; // Added for file operations

namespace WorkstationInsights
{
    public partial class Form1 : Form
    {
        private string currentLogFilePath; // To store the path of the current log file
        private const string LogDirectory = "ChatLogs"; // Directory to store log files

        public Form1()
        {
            InitializeComponent();
            InitializeChatLogging(); // Initialize logging when the form loads
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

        private void StartNewLogFile()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            currentLogFilePath = Path.Combine(LogDirectory, $"chatlog_{timestamp}.txt");
            // You could write an initial message to the log file if desired, e.g.:
            // File.AppendAllText(currentLogFilePath, $"[Session started at {DateTime.Now}]{Environment.NewLine}");
        }

        private void LogMessage(string sender, string message)
        {
            if (string.IsNullOrEmpty(currentLogFilePath)) return; // Should not happen if initialized correctly

            try
            {
                File.AppendAllText(currentLogFilePath, $"[{DateTime.Now:HH:mm:ss}] {sender}: {message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                // Handle potential I/O errors, e.g., by showing a message to the user or logging to a fallback mechanism
                Console.WriteLine($"Error writing to log file: {ex.Message}"); // Or use a more robust error handling
            }
        }

        private ChatHistory chatHistory = new(); // persist across messages

        private async void SendButton_Click(object sender, EventArgs e)
        {
            var input = inputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            chatHistoryTextBox.AppendText($"> {input}{Environment.NewLine}");
            LogMessage("User", input); // Log user message

            try
            {
                var chatService = Program.KernelInstance.GetRequiredService<IChatCompletionService>();

                chatHistory.AddUserMessage(input);

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
                    LogMessage("Tool", toolContent); // Log tool response
                }
                else
                {
                    var aiContent = result.Content ?? "";
                    AppendMarkdownMessage("AI", aiContent);
                    chatHistory.AddAssistantMessage(aiContent);
                    LogMessage("AI", aiContent); // Log AI message
                }
            }
            catch (Exception ex)
            {
                chatHistoryTextBox.AppendText($"[Error]: {ex.Message}{Environment.NewLine}");
                LogMessage("Error", ex.Message); // Log error
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
            chatHistory.Clear(); // Clear the in-memory chat history as well
            StartNewLogFile(); // Start a new log file for the new session
            // Optionally, log that a new chat has started in the new log file
            LogMessage("System", "New chat session started.");
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
