using System;
using System.Collections.Generic;

namespace Hangfire.Oracle.Core.Configuration
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
                { "Job", "HHF_JOB" },
                { "JobParameter", "HHF_JOB_PARAMETER" },
                { "JobQueue", "HHF_JOB_QUEUE" },
                { "JobState", "HHF_JOB_STATE" },
                { "Server", "HHF_SERVER" },
                { "Set", "HHF_SET" },
                { "List", "HHF_LIST" },
                { "Hash", "HHF_HASH" },
                { "Counter", "HHF_COUNTER" },
                { "AggregatedCounter", "HHF_AGGREGATED_COUNTER" },
                { "DistributedLock", "HHF_DISTRIBUTED_LOCK" }
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
    }
}