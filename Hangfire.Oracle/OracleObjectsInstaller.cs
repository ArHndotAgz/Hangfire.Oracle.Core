using System;
using System.Data;
using System.Text;
using Dapper;
using Hangfire.Logging;
using Kavosh.Hangfire.Oracle.Core.Configuration;

namespace Kavosh.Hangfire.Oracle.Core
{
    public static class OracleObjectsInstaller
    {
        private static readonly ILog Log = LogProvider.GetLogger(typeof(OracleStorage));

        public static void Install(IDbConnection connection, HangfireTableNameProvider tableNameProvider)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (tableNameProvider == null) throw new ArgumentNullException(nameof(tableNameProvider));

            var schema = tableNameProvider.GetSchemaForTable("Job");

            if (TablesExists(connection, schema, tableNameProvider))
            {
                Log.Info("DB tables already exist. Exit install");
                return;
            }

            Log.Info("Start installing Hangfire SQL objects...");

            var script = GenerateInstallScript(tableNameProvider);
            var sqlCommands = script.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var sqlCommand in sqlCommands)
            {
                var trimmedCommand = sqlCommand.Trim();
                if (!string.IsNullOrWhiteSpace(trimmedCommand))
                {
                    connection.Execute(trimmedCommand);
                }
            }

            Log.Info("Hangfire SQL objects installed.");
        }

        private static bool TablesExists(IDbConnection connection, string schemaName, HangfireTableNameProvider tableNameProvider)
        {
            string tableExistsQuery;
            var jobTableName = tableNameProvider.GetTableName("Job");

            if (!string.IsNullOrEmpty(schemaName))
            {
                tableExistsQuery = $@"
                            SELECT TABLE_NAME
                            FROM all_tables
                            WHERE OWNER = '{schemaName}' AND TABLE_NAME = '{jobTableName}'";
            }
            else
            {
                tableExistsQuery = $@"
                                        SELECT TABLE_NAME
                                        FROM all_tables
                                        WHERE TABLE_NAME = '{jobTableName}'";
            }

            return connection.ExecuteScalar<string>(tableExistsQuery) != null;
        }

        private static string GenerateInstallScript(HangfireTableNameProvider tableNameProvider)
        {
            var sb = new StringBuilder();
            var varcharType = tableNameProvider.GetVarcharType(255);
            var varcharType100 = tableNameProvider.GetVarcharType(100);
            var varcharType50 = tableNameProvider.GetVarcharType(50);
            var varcharType40 = tableNameProvider.GetVarcharType(40);
            var varcharType36 = tableNameProvider.GetVarcharType(36);
            var varcharType20 = tableNameProvider.GetVarcharType(20);
            var clobType = tableNameProvider.GetClobType();

            // Get sequence names from configuration
            var primarySequence = tableNameProvider.GetPrimarySequenceName();
            var jobIdSequence = tableNameProvider.GetJobIdSequenceName();

            // Get table names
            var jobTable = tableNameProvider.GetTableName("Job");
            var counterTable = tableNameProvider.GetTableName("Counter");
            var aggregatedCounterTable = tableNameProvider.GetTableName("AggregatedCounter");
            var distributedLockTable = tableNameProvider.GetTableName("DistributedLock");
            var hashTable = tableNameProvider.GetTableName("Hash");
            var jobParameterTable = tableNameProvider.GetTableName("JobParameter");
            var jobQueueTable = tableNameProvider.GetTableName("JobQueue");
            var jobStateTable = tableNameProvider.GetTableName("JobState");
            var serverTable = tableNameProvider.GetTableName("Server");
            var setTable = tableNameProvider.GetTableName("Set");
            var listTable = tableNameProvider.GetTableName("List");

            // Create sequences with configured names
            sb.AppendLine($"CREATE SEQUENCE {primarySequence} START WITH 1 MAXVALUE 9999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20 NOORDER;");
            sb.AppendLine();
            sb.AppendLine($"CREATE SEQUENCE {jobIdSequence} START WITH 1 MAXVALUE 9999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20 NOORDER;");
            sb.AppendLine();

            // Job Table
            sb.AppendLine($@"CREATE TABLE {jobTable} (
                                ID                NUMBER(10),
                                STATE_ID          NUMBER(10),
                                STATE_NAME        {varcharType20},
                                INVOCATION_DATA   {clobType},
                                ARGUMENTS         {clobType},
                                CREATED_AT        TIMESTAMP(4),
                                EXPIRE_AT         TIMESTAMP(4)
                                )
                                LOB (INVOCATION_DATA) STORE AS BASICFILE
                                   (ENABLE STORAGE IN ROW
                                    CHUNK 8192
                                    RETENTION
                                    NOCACHE LOGGING)
                                LOB (ARGUMENTS) STORE AS BASICFILE
                                   (ENABLE STORAGE IN ROW
                                    CHUNK 8192
                                    RETENTION
                                    NOCACHE LOGGING)
                                LOGGING
                                NOCOMPRESS
                                NOCACHE
                                NOPARALLEL
                                MONITORING;");
            sb.AppendLine();

            sb.AppendLine($@"ALTER TABLE {jobTable} ADD (
                              PRIMARY KEY (ID)
                              USING INDEX
                              ENABLE VALIDATE);");
                                        sb.AppendLine();

                                        // Counter Table
                                        sb.AppendLine($@"CREATE TABLE {counterTable} (
                                ID          NUMBER(10),
                                KEY         {varcharType},
                                VALUE       NUMBER(10),
                                EXPIRE_AT   TIMESTAMP(4)
                            )
                            LOGGING
                            NOCOMPRESS
                            NOCACHE
                            NOPARALLEL
                            MONITORING;");
            sb.AppendLine();

            sb.AppendLine($@"ALTER TABLE {counterTable} ADD (
                              PRIMARY KEY (ID)
                              USING INDEX
                              ENABLE VALIDATE);");
            sb.AppendLine();

            // Aggregated Counter Table
            sb.AppendLine($@"CREATE TABLE {aggregatedCounterTable} (
                                ID          NUMBER(10),
                                KEY         {varcharType},
                                VALUE       NUMBER(10),
                                EXPIRE_AT   TIMESTAMP(4)
                                )
                                LOGGING
                                NOCOMPRESS
                                NOCACHE
                                NOPARALLEL
                                MONITORING;");
            sb.AppendLine();

            sb.AppendLine($@"ALTER TABLE {aggregatedCounterTable} ADD (
                                  PRIMARY KEY (ID)
                                  USING INDEX
                                  ENABLE VALIDATE,
                                  UNIQUE (KEY)
                                  USING INDEX
                                  ENABLE VALIDATE);");
            sb.AppendLine();

            // Distributed Lock Table
            sb.AppendLine($@"CREATE TABLE {distributedLockTable} (
                                ""RESOURCE"" {varcharType100},
                                CREATED_AT TIMESTAMP(4)
                                )
                                LOGGING
                                NOCOMPRESS
                                NOCACHE
                                NOPARALLEL
                                MONITORING;");
            sb.AppendLine();

            // Hash Table
            sb.AppendLine($@"CREATE TABLE {hashTable} (
                                ID          NUMBER(10),
                                KEY         {varcharType},
                                VALUE       {clobType},
                                EXPIRE_AT   TIMESTAMP(4),
                                FIELD       {varcharType40}
                                )
                                LOB (VALUE) STORE AS BASICFILE
                                   (ENABLE STORAGE IN ROW
                                    CHUNK 8192
                                    RETENTION
                                    NOCACHE LOGGING)
                                LOGGING
                                NOCOMPRESS
                                NOCACHE
                                NOPARALLEL
                                MONITORING;");
            sb.AppendLine();

            sb.AppendLine($@"ALTER TABLE {hashTable} ADD (
                              PRIMARY KEY (ID)
                              USING INDEX
                              ENABLE VALIDATE,
                              UNIQUE (KEY, FIELD)
                              USING INDEX
                              ENABLE VALIDATE);");
            sb.AppendLine();

            // Job Parameter Table
            sb.AppendLine($@"CREATE TABLE {jobParameterTable} (
                                ID       NUMBER(10),
                                NAME     {varcharType40},
                                VALUE    {clobType},
                                JOB_ID   NUMBER(10)
                                )
                                LOB (VALUE) STORE AS BASICFILE
                                   (ENABLE STORAGE IN ROW
                                    CHUNK 8192
                                    RETENTION
                                    NOCACHE LOGGING)
                                LOGGING
                                NOCOMPRESS
                                NOCACHE
                                NOPARALLEL
                                MONITORING;");
            sb.AppendLine();

            sb.AppendLine($@"ALTER TABLE {jobParameterTable} ADD (
                              PRIMARY KEY (ID)
                              USING INDEX
                              ENABLE VALIDATE);");
            sb.AppendLine();

            sb.AppendLine($@"ALTER TABLE {jobParameterTable} ADD (
                              CONSTRAINT FK_JOB_PARAMETER_JOB
                              FOREIGN KEY (JOB_ID)
                              REFERENCES {jobTable} (ID)
                              ON DELETE CASCADE ENABLE VALIDATE);");
            sb.AppendLine();

            // Job Queue Table
            sb.AppendLine($@"CREATE TABLE {jobQueueTable} (
                                ID            NUMBER(10),
                                JOB_ID        NUMBER(10),
                                QUEUE         {varcharType50},
                                FETCHED_AT    TIMESTAMP(4),
                                FETCH_TOKEN   {varcharType36}
                                )
                                LOGGING
                                NOCOMPRESS
                                NOCACHE
                                NOPARALLEL
                                MONITORING;");
            sb.AppendLine();

            sb.AppendLine($@"ALTER TABLE {jobQueueTable} ADD (
                              PRIMARY KEY (ID)
                              USING INDEX
                              ENABLE VALIDATE);");
            sb.AppendLine();

            sb.AppendLine($@"ALTER TABLE {jobQueueTable} ADD (
                          CONSTRAINT FK_JOB_QUEUE_JOB
                          FOREIGN KEY (JOB_ID)
                          REFERENCES {jobTable} (ID)
                          ON DELETE CASCADE ENABLE VALIDATE);");
            sb.AppendLine();

            // Job State Table
            sb.AppendLine($@"CREATE TABLE {jobStateTable} (
                                ID           NUMBER(10),
                                JOB_ID       NUMBER(10),
                                NAME         {varcharType20},
                                REASON       {varcharType100},
                                CREATED_AT   TIMESTAMP(4),
                                DATA         {clobType}
                                )
                                LOB (DATA) STORE AS BASICFILE
                                   (ENABLE STORAGE IN ROW
                                    CHUNK 8192
                                    RETENTION
                                    NOCACHE LOGGING)
                                LOGGING
                                NOCOMPRESS
                                NOCACHE
                                NOPARALLEL
                                MONITORING;");
            sb.AppendLine();

            sb.AppendLine($@"ALTER TABLE {jobStateTable} ADD (
                              PRIMARY KEY (ID)
                              USING INDEX
                              ENABLE VALIDATE);");
            sb.AppendLine();

            sb.AppendLine($@"ALTER TABLE {jobStateTable} ADD (
                                      CONSTRAINT FK_JOB_STATE_JOB
                                      FOREIGN KEY (JOB_ID)
                                      REFERENCES {jobTable} (ID)
                                      ON DELETE CASCADE ENABLE VALIDATE);");
            sb.AppendLine();

            // Server Table
            sb.AppendLine($@"CREATE TABLE {serverTable} (
                                    ID {varcharType100},
                                    DATA {clobType},
                                    LAST_HEART_BEAT TIMESTAMP(4)
                                    )
                                    LOB (DATA) STORE AS BASICFILE
                                       (ENABLE STORAGE IN ROW
                                        CHUNK 8192
                                        RETENTION
                                        NOCACHE LOGGING)
                                    LOGGING
                                    NOCOMPRESS
                                    NOCACHE
                                    NOPARALLEL
                                    MONITORING;");
            sb.AppendLine();

            sb.AppendLine($@"ALTER TABLE {serverTable} ADD (
                              PRIMARY KEY (ID)
                              USING INDEX
                              ENABLE VALIDATE);");
            sb.AppendLine();

            // Set Table
            sb.AppendLine($@"CREATE TABLE {setTable} (
                                ID          NUMBER(10),
                                KEY         {varcharType},
                                VALUE       {varcharType},
                                SCORE       FLOAT(126),
                                EXPIRE_AT   TIMESTAMP(4)
                                )
                                LOGGING
                                NOCOMPRESS
                                NOCACHE
                                NOPARALLEL
                                MONITORING;");
            sb.AppendLine();

            sb.AppendLine($@"ALTER TABLE {setTable} ADD (
                              PRIMARY KEY (ID)
                              USING INDEX
                              ENABLE VALIDATE,
                              UNIQUE (KEY, VALUE)
                              USING INDEX
                              ENABLE VALIDATE);");
            sb.AppendLine();

            // List Table
            sb.AppendLine($@"CREATE TABLE {listTable} (
                                ID          NUMBER(10),
                                KEY         {varcharType},
                                VALUE       {clobType},
                                EXPIRE_AT   TIMESTAMP(4)
                                )
                                LOB (VALUE) STORE AS BASICFILE
                                   (ENABLE STORAGE IN ROW
                                    CHUNK 8192
                                    RETENTION
                                    NOCACHE LOGGING)
                                LOGGING
                                NOCOMPRESS
                                NOCACHE
                                NOPARALLEL
                                MONITORING;");
            sb.AppendLine();

            sb.AppendLine($@"ALTER TABLE {listTable} ADD (
                                  PRIMARY KEY (ID)
                                  USING INDEX
                                  ENABLE VALIDATE);");

            return sb.ToString();
        }
    }
}
