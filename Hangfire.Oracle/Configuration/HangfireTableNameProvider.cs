using System.Collections.Generic;

namespace Hangfire.Oracle.Core.Configuration
{
    public class HangfireTableNameProvider
    {
        private readonly HangfireConfiguration _config;
        private readonly Dictionary<string, string> _defaultTableNames;
        private readonly string _instancePrefix;

        public HangfireTableNameProvider(HangfireConfiguration config)
        {
            _config = config ?? new HangfireConfiguration();

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

            _instancePrefix = DetermineInstancePrefix();
        }

        private string DetermineInstancePrefix()
        {
            if (!string.IsNullOrWhiteSpace(_config.InstanceName))
            {
                return _config.InstanceName.ToUpper();
            }
            return "HF";
        }

        public string GetTableName(string logicalName)
        {
            if (_config.Tables != null && _config.Tables.TryGetValue(logicalName, out var customName))
            {
                return customName;
            }
            return _defaultTableNames.TryGetValue(logicalName, out var defaultName) ? defaultName : logicalName;
        }

        public string GetSchemaName()
        {
            return _config.SchemaName ?? string.Empty;
        }

        public string GetPrimarySequenceName()
        {
            return _config.Sequence?.PrimarySequenceName ?? "HF_SEQUENCE";
        }

        public string GetJobIdSequenceName()
        {
            return _config.Sequence?.JobIdSequenceName ?? "HF_JOB_ID_SEQ";
        }

        public string GetInstancePrefix()
        {
            return _instancePrefix;
        }

        public string GetConstraintName(string tableName, string constraintType)
        {
            return $"{_instancePrefix}_{constraintType}_{ShortenTableName(tableName)}";
        }

        public string GetIndexName(string tableName, string indexType = "IDX")
        {
            return $"{_instancePrefix}_{indexType}_{ShortenTableName(tableName)}";
        }

        private string ShortenTableName(string tableName)
        {
            var name = tableName.Replace($"{_instancePrefix}_", "").Replace("HF_", "");

            return name.Length > 20 ? name.Substring(0, 20) : name;
        }
    }
}