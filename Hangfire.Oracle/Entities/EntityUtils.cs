using System.Data;
using Dapper;

namespace Kavosh.Hangfire.Oracle.Core.Entities
{
    public static class EntityUtils
    {
        public static long GetNextId(this IDbConnection connection, string sequenceName = "HF_SEQUENCE")
        {
            return connection.QuerySingle<long>($"SELECT {sequenceName}.NEXTVAL FROM dual");
        }

        public static long GetNextJobId(this IDbConnection connection, string sequenceName = "HF_JOB_ID_SEQ")
        {
            return connection.QuerySingle<long>($"SELECT {sequenceName}.NEXTVAL FROM dual");
        }
    }
}
