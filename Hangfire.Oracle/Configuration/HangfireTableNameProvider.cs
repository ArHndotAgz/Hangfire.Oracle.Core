using System;
using System.Collections.Generic;

namespace Kavosh.Hangfire.Oracle.Core.Configuration
{
    public class HangfireTableNameProvider
    {
        private readonly HangfireTableMappings _mappings;
        private readonly Dictionary<string, string> _defaultTableNames;

        public HangfireTableNameProvider(HangfireTableMappings mappings)
        {
            _mappings = mappings ?? throw new ArgumentNullException(nameof(mappings));

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
            if (_mappings.Tables.TryGetValue(logicalName, out var tableInfo) &&
                !string.IsNullOrEmpty(tableInfo.TableName))
            {
                return tableInfo.TableName;
            }

            return _defaultTableNames.TryGetValue(logicalName, out var defaultName)
                ? defaultName
                : logicalName;
        }

        public string GetFullTableName(string logicalName)
        {
            var tableName = GetTableName(logicalName);
            var schema = GetSchemaForTable(logicalName);

            return string.IsNullOrEmpty(schema)
                ? tableName
                : $"{schema}.{tableName}";
        }

        public string GetSchemaForTable(string logicalName)
        {
            if (_mappings.Tables.TryGetValue(logicalName, out var tableInfo) &&
                !string.IsNullOrEmpty(tableInfo.Schema))
            {
                return tableInfo.Schema;
            }

            return _mappings.DefaultSchema;
        }

        public string GetClobType()
        {
            return _mappings.DataTypeSettings.UseNationalCharacterSet ? "NCLOB" : "CLOB";
        }

        public string GetVarcharType(int size)
        {
            return _mappings.DataTypeSettings.UseNationalCharacterSet
                ? $"NVARCHAR2({size})"
                : $"VARCHAR2({size})";
        }

        public string GetPrimarySequenceName()
        {
            return _mappings.SequenceSettings?.PrimarySequenceName ?? "HF_SEQUENCE";
        }

        public string GetJobIdSequenceName()
        {
            return _mappings.SequenceSettings?.JobIdSequenceName ?? "HF_JOB_ID_SEQ";
        }
    }
}