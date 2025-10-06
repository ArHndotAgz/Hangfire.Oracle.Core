namespace Kavosh.Hangfire.Oracle.Core.Configuration
{
    public class OracleSequenceSettings
    {
        public string PrimarySequenceName { get; set; } = "HF_SEQUENCE";

        public string JobIdSequenceName { get; set; } = "HF_JOB_ID_SEQ";
    }
}