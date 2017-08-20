using System.Collections.Generic;

namespace SqlToolsNet
{
    public class ProcContext
    {
        public ProcContext(string connectionString, string procedureName, bool useTransaction, int timeoutSeconds, List<SqlProcParameter> parameters)
        {
            UseTransaction = useTransaction;
            TimeoutSeconds = timeoutSeconds;
            Parameters = parameters;
            ConnectionString = connectionString;
            ProcedureName = procedureName;
        }
        public string ConnectionString { get; }
        public string ProcedureName { get; }
        public List<SqlProcParameter> Parameters { get; }
        public bool UseTransaction { get; }
        public int TimeoutSeconds { get; }
    }
}