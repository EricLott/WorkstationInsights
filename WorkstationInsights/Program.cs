using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WorkstationInsights
{
    internal static class Program
    {
        public static Kernel KernelInstance { get; private set; }
        public static IChatCompletionService ChatService { get; private set; }
        public static ChatHistory ChatHistory { get; private set; }

        [STAThread]
        static async Task Main()
        {
            // Load config
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var apiKey = config["OpenAI:ApiKey"];
            var modelId = config["OpenAI:Model"] ?? "gpt-3.5-turbo";

            // Build kernel
            var builder = Kernel.CreateBuilder();
            builder.Services.AddLogging();

            builder.AddOpenAIChatCompletion(
                modelId: modelId,
                apiKey: apiKey,
                serviceId: "chat"
            );

            builder.Plugins.AddFromType<SystemDiagnosticsPlugin>();

            KernelInstance = builder.Build();

            // Set chat service reference
            ChatService = KernelInstance.GetRequiredService<IChatCompletionService>();

            // Initialize chat history with guardrail system prompt
            ChatHistory = new ChatHistory();
            ChatHistory.AddSystemMessage(
                "You are a diagnostics and automation assistant for workstation tasks. "
                + "You must never perform or suggest dangerous, destructive, or irreversible operations "
                + "without clearly outlining the risks and obtaining explicit user permission. "
                + "Always prioritize safety, transparency, and user control."
            );

            // Start app
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}