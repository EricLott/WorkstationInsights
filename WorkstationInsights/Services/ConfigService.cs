using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace WorkstationInsights.Services
{
    public static class ConfigService
    {
        private const string ConfigPath = "appsettings.json";

        public static Dictionary<string, object> LoadConfig()
        {
            if (!File.Exists(ConfigPath)) return new Dictionary<string, object>();
            var json = File.ReadAllText(ConfigPath);
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
        }

        public static void SaveConfig(Dictionary<string, object> config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
        }

        public static void UpdateOpenAiSetting(string key, string value)
        {
            var config = LoadConfig();
            if (!config.ContainsKey("OpenAI")) config["OpenAI"] = new Dictionary<string, string>();
            
            var openAiSection = JsonSerializer.Deserialize<Dictionary<string, string>>(config["OpenAI"]?.ToString() ?? "{}");
            if (openAiSection != null)
            {
                openAiSection[key] = value;
                config["OpenAI"] = openAiSection;
                SaveConfig(config);
            }
        }

        public static string GetOpenAiSetting(string key)
        {
            var config = LoadConfig();
            if (!config.ContainsKey("OpenAI")) return null;
            var openAiSection = JsonSerializer.Deserialize<Dictionary<string, string>>(config["OpenAI"]?.ToString() ?? "{}");
            return openAiSection?.ContainsKey(key) == true ? openAiSection[key] : null;
        }
    }
}
