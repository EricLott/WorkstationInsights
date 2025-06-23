using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text.Json;

namespace WorkstationInsights
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private ChatHistory chatHistory = new(); // persist across messages

        private async void SendButton_Click(object sender, EventArgs e)
        {
            var input = inputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            chatHistoryTextBox.AppendText($"> {input}{Environment.NewLine}");

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
                    chatHistoryTextBox.AppendText($"[Tool Response]: {result.Content}{Environment.NewLine}");
                }
                else
                {
                    chatHistoryTextBox.AppendText($"{result.Content}{Environment.NewLine}{Environment.NewLine}");
                    chatHistory.AddAssistantMessage(result.Content ?? "");
                }
            }
            catch (Exception ex)
            {
                chatHistoryTextBox.AppendText($"[Error]: {ex.Message}{Environment.NewLine}");
            }

            inputTextBox.Clear();
        }

        private void NewChatButton_Click(object sender, EventArgs e)
        {
            chatHistoryTextBox.Clear();
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
    }
}
