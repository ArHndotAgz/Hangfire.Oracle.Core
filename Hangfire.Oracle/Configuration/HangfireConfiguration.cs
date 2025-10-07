using System.Collections.Generic;

namespace Hangfire.Oracle.Core.Configuration
{
    public class HangfireConfiguration
    {
        public string SchemaName { get; set; } = string.Empty;
        public string InstanceName { get; set; } = string.Empty;
        public SequenceConfiguration Sequence { get; set; } = new SequenceConfiguration();
        public Dictionary<string, string> Tables { get; set; } = new Dictionary<string, string>();
    }
}