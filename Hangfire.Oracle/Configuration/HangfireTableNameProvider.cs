using System.Collections.Generic;

namespace Kavosh.Hangfire.Oracle.Core.Configuration
{
    public class HangfireTableNameProvider
    {
        private readonly HangfireConfiguration _config;
        private readonly Dictionary<string, string> _defaultTableNames;

        public HangfireTableNameProvider(HangfireConfiguration config)
        {
            _config = config ?? new HangfireConfiguration();

            // Default Hangfire table names
            _defaultTableNames = new Dictionary<string, string>
            {
                { "Job", "HF_JOB" },
                { "JobParameter", "HF_JOB_PARAMETER" },
                { "JobQueue", "HF_JOB_QUEUE" },
                { "JobState", "HF_JOB_STATE" },
                { "Server", "HF_SERVER" },
                { "Set", "HF_SET" },
                { "List", "HF_LIST" },
                { "Hash", "HF_HASH" },
                { "Counter", "HF_COUNTER" },
                { "AggregatedCounter", "HF_AGGREGATED_COUNTER" },
                { "DistributedLock", "HF_DISTRIBUTED_LOCK" }
            };
        }

        public string GetTableName(string logicalName)
        {
            // Try custom name first
            if (_config.Tables != null && _config.Tables.TryGetValue(logicalName, out var customName))
            {
                return customName;
            }

            // Fallback to default
            return _defaultTableNames.TryGetValue(logicalName, out var defaultName) ? defaultName : logicalName;
        }

        public string GetSchemaName()
        {
            return _config.SchemaName ?? string.Empty;
        }

        public string GetClobType()
        {
            return _config.UseNationalCharacterSet ? "NCLOB" : "CLOB";
        }

        public string GetVarcharType(int size)
        {
            return _config.UseNationalCharacterSet 
                ? $"NVARCHAR2({size})" 
                : $"VARCHAR2({size})";
        }

        public string GetPrimarySequenceName()
        {
            return _config.Sequence?.PrimarySequenceName ?? "HF_SEQUENCE";
        }

        public string GetJobIdSequenceName()
        {
            return _config.Sequence?.JobIdSequenceName ?? "HF_JOB_ID_SEQ";
        }
    }
}