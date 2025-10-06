using System;
using System.IO;
using Newtonsoft.Json;

namespace Kavosh.Hangfire.Oracle.Core.Configuration
{
    public static class HangfireConfigurationLoader
    {
        /// <summary>
        /// Loads Hangfire table mappings from a JSON file.
        /// </summary>
        /// <param name="jsonFilePath">Path to the JSON configuration file</param>
        /// <returns>HangfireTableMappings object</returns>
        public static HangfireTableMappings LoadFromJson(string jsonFilePath)
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

        /// <summary>
        /// Loads Hangfire table mappings from JSON string content.
        /// </summary>
        /// <param name="jsonContent">JSON configuration content</param>
        /// <returns>HangfireTableMappings object</returns>
        public static HangfireTableMappings DeserializeFromJson(string jsonContent)
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                throw new ArgumentException("JSON content cannot be null or empty.", nameof(jsonContent));
            }

            try
            {
                // Using Newtonsoft.Json for .NET Standard 2.0 compatibility
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                var mappings = JsonConvert.DeserializeObject<HangfireTableMappings>(jsonContent, settings);

                if (mappings == null)
                {
                    throw new InvalidOperationException("Failed to deserialize Hangfire table mappings from JSON.");
                }

                // Validate configuration
                ValidateConfiguration(mappings);

                return mappings;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Invalid JSON format for Hangfire table mappings.", ex);
            }
        }

        private static void ValidateConfiguration(HangfireTableMappings mappings)
        {
            if (mappings.Tables == null)
            {
                mappings.Tables = new System.Collections.Generic.Dictionary<string, HangfireTableInfo>();
            }

            if (mappings.DataTypeSettings == null)
            {
                mappings.DataTypeSettings = new OracleDataTypeSettings();
            }

            // Ensure all table names are properly set
            foreach (var kvp in mappings.Tables)
            {
                if (kvp.Value == null)
                {
                    throw new InvalidOperationException($"Table info for '{kvp.Key}' cannot be null.");
                }

                if (string.IsNullOrWhiteSpace(kvp.Value.TableName))
                {
                    throw new InvalidOperationException($"Table name for '{kvp.Key}' cannot be null or empty.");
                }
            }
        }
    }
}