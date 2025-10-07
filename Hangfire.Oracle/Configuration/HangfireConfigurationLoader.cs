using System;
using System.IO;
using Newtonsoft.Json;

namespace Hangfire.Oracle.Core.Configuration
{
    public static class HangfireConfigurationLoader
    {
        public static HangfireConfiguration LoadFromJson(string jsonFilePath)
        {
            if (string.IsNullOrWhiteSpace(jsonFilePath))
            {
                throw new ArgumentException("JSON file path cannot be null or empty.", nameof(jsonFilePath));
            }

            if (!File.Exists(jsonFilePath))
            {
                throw new FileNotFoundException($"Configuration file not found: {jsonFilePath}");
            }

            var jsonContent = File.ReadAllText(jsonFilePath);
            return DeserializeFromJson(jsonContent);
        }

        public static HangfireConfiguration DeserializeFromJson(string jsonContent)
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                throw new ArgumentException("JSON content cannot be null or empty.", nameof(jsonContent));
            }

            try
            {
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                var config = JsonConvert.DeserializeObject<HangfireConfiguration>(jsonContent, settings);

                if (config == null)
                {
                    throw new InvalidOperationException("Failed to deserialize Hangfire configuration.");
                }

                ValidateAndInitialize(config);

                return config;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Invalid JSON format for Hangfire configuration.", ex);
            }
        }

        private static void ValidateAndInitialize(HangfireConfiguration config)
        {
            config.Tables = config.Tables ?? new System.Collections.Generic.Dictionary<string, string>();
            config.Sequence = config.Sequence ?? new SequenceConfiguration();

            foreach (var table in config.Tables)
            {
                if (string.IsNullOrWhiteSpace(table.Value))
                {
                    throw new InvalidOperationException($"Table name for '{table.Key}' cannot be null or empty.");
                }
            }
        }
    }
}