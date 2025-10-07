using System;
using System.Data;
using System.Globalization;
using Dapper;
using Hangfire.Logging;
using Hangfire.Storage;

namespace Hangfire.Oracle.Core.JobQueue
{
    internal class OracleFetchedJob : IFetchedJob
    {
        private static readonly ILog Logger = LogProvider.GetLogger(typeof(OracleFetchedJob));

        private readonly OracleStorage _storage;
        private readonly IDbConnection _connection;
        private readonly int _id;
        private bool _removedFromQueue;
        private bool _requeued;
        private bool _disposed;

        public OracleFetchedJob(OracleStorage storage, IDbConnection connection, FetchedJob fetchedJob)
        {
            if (fetchedJob == null)
            {
                throw new ArgumentNullException(nameof(fetchedJob));
            }

            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _id = fetchedJob.Id;
            JobId = fetchedJob.JobId.ToString(CultureInfo.InvariantCulture);
            Queue = fetchedJob.Queue;
        }

        private string T(string logicalName) => _storage.TableNameProvider.GetTableName(logicalName);

        public void Dispose()
        {
            if (_disposed) return;

            if (!_removedFromQueue && !_requeued)
            {
                Requeue();
            }

            _storage.ReleaseConnection(_connection);

            _disposed = true;
        }

        public void RemoveFromQueue()
        {
            Logger.TraceFormat("RemoveFromQueue JobId={0}", JobId);

            _connection.Execute(
                $"DELETE FROM {T("JobQueue")} WHERE ID = :ID",
                new
                {
                    ID = _id
                });

            _removedFromQueue = true;
        }

        public void Requeue()
        {
            Logger.TraceFormat("Requeue JobId={0}", JobId);

            _connection.Execute(
                $"UPDATE {T("JobQueue")} SET FETCHED_AT = null WHERE ID = :ID",
                new
                {
                    ID = _id
                });

            _requeued = true;
        }

        public string JobId { get; }

        public string Queue { get; }
    }
}