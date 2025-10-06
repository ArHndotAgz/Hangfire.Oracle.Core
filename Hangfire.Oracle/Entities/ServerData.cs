using System;

namespace Kavosh.Hangfire.Oracle.Core.Entities
{
    internal class ServerData
    {
        public int WorkerCount { get; set; }
        public string[] Queues { get; set; }
        public DateTime? StartedAt { get; set; }
    }
}