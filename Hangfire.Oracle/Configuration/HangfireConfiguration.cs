using System.Collections.Generic;

namespace Kavosh.Hangfire.Oracle.Core.Configuration
{
    public class HangfireConfiguration
    {
        public string SchemaName { get; set; } = string.Empty;
        public bool UseNationalCharacterSet { get; set; } = false;
        public SequenceConfiguration Sequence { get; set; } = new SequenceConfiguration();
        public Dictionary<string, string> Tables { get; set; } = new Dictionary<string, string>();
    }

    public class SequenceConfiguration
    {
        public string PrimarySequenceName { get; set; } = "HF_SEQUENCE";
        public string JobIdSequenceName { get; set; } = "HF_JOB_ID_SEQ";
    }
}