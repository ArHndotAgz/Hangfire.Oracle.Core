namespace Hangfire.Oracle.Core.Configuration
{
    public class SequenceConfiguration
    {
        public string PrimarySequenceName { get; set; } = "HF_SEQUENCE";
        public string JobIdSequenceName { get; set; } = "HF_JOB_ID_SEQ";
    }
}