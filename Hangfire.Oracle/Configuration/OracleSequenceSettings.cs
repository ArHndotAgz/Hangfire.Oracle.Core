namespace Kavosh.Hangfire.Oracle.Core.Configuration
{
    public class OracleSequenceSettings
    {
        /// <summary>
        /// Primary sequence name used for most Hangfire tables. Default: HF_SEQUENCE
        /// </summary>
        public string PrimarySequenceName { get; set; } = "HF_SEQUENCE";

        /// <summary>
        /// Job ID sequence name. Default: HF_JOB_ID_SEQ
        /// </summary>
        public string JobIdSequenceName { get; set; } = "HF_JOB_ID_SEQ";
    }
}