using System.Collections.Generic;

namespace Kavosh.Hangfire.Oracle.Core.Configuration
{
    public class HangfireTableMappings
    {
        public string DefaultSchema { get; set; } = string.Empty;
        public Dictionary<string, HangfireTableInfo> Tables { get; set; } = new Dictionary<string, HangfireTableInfo>();
        public OracleDataTypeSettings DataTypeSettings { get; set; } = new OracleDataTypeSettings();
        public OracleSequenceSettings SequenceSettings { get; set; } = new OracleSequenceSettings();
    }

    public class HangfireTableInfo
    {
        public string TableName { get; set; } = string.Empty;
        public string Schema { get; set; } = string.Empty;
    }

    public class OracleDataTypeSettings
    {
        /// <summary>
        /// Use National Character Set (NVARCHAR2/NCLOB). Default: false (uses VARCHAR2/CLOB)
        /// Set to true for full Unicode support including Persian characters.
        /// </summary>
        public bool UseNationalCharacterSet { get; set; } = false;
    }
}