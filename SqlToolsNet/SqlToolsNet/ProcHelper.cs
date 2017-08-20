using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace SqlToolsNet
{
    public class ProcHelper
    {
        private readonly Action<string> _textLogger;
        private readonly Action<Exception> _exceptionLogger;

        public ProcHelper(Action<string> textLogger, Action<Exception> exceptionLogger)
        {
            _textLogger = textLogger;
            _exceptionLogger = exceptionLogger;
        }

        public void ExecuteProcIfNotAlreadyRunningAsync(string connectionString, string procedureName, IEnumerable<SqlProcParameter> parameters, bool useTransaction, int timeoutSeconds, object singleRunLocker)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((state) =>
            {
                if (Monitor.TryEnter(singleRunLocker, 0))
                    try { ExecuteProc(connectionString, procedureName, parameters, useTransaction, timeoutSeconds); }
                    finally { Monitor.Exit(singleRunLocker); }
                else _textLogger?.Invoke($"{nameof(ProcHelper)} can't launch procedure:{procedureName} because it was already launched and hasn't finished yet");
            }));
        }
        //TODO:
        //NOTE! we catch exception here to avoid unhandled exception in asynchronous scenarios
        public void ExecuteProc(string connectionString, string procedureName, IEnumerable<SqlProcParameter> parameters, bool useTransaction, int timeoutSeconds)
        {
            try { RunProc(connectionString, procedureName, parameters, useTransaction, timeoutSeconds); }
            catch (Exception ex) { _exceptionLogger?.Invoke(new ProcRunnerException($"Error executing SP: {procedureName}" + Environment.NewLine + ex)); }

        }


        private void RunProc(string connectionString, string procedureName, IEnumerable<SqlProcParameter> parameters, bool useTransaction,
            int timeout)
        {

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand cmd;
                if (useTransaction)
                {
                    var tran = sqlConnection.BeginTransaction(procedureName);
                    cmd = new SqlCommand(procedureName, sqlConnection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = timeout,
                        Transaction = tran
                    };
                }
                else
                {
                    cmd = new SqlCommand(procedureName, sqlConnection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = timeout,
                    };
                }
                try
                {
                    if (parameters != null)
                        foreach (var par in parameters)
                        {
                            cmd.Parameters.Add(par.ParameterName, par.ParameterDbType).Value = par.Value;
                        }

                    sqlConnection.InfoMessage += GetSqlInfoMessageEventHandler(s => _textLogger?.Invoke($"Proc:{procedureName}: {s}"));

                    cmd.ExecuteNonQuery();
                    if (useTransaction)
                        cmd.Transaction.Commit();

                    _textLogger?.Invoke($"++++{procedureName} complete++++");
                }
                finally { cmd.Dispose(); }
            }
        }


        private SqlInfoMessageEventHandler GetSqlInfoMessageEventHandler(Action<string> logger)
        {
            if (logger == null) throw new ArgumentException($"argument:{nameof(logger)} shouldn't be null");
            SqlInfoMessageEventHandler handler = (sender, args) =>
            {
                var errors = args
                    .Errors
                    .OfType<SqlError>()
                    .Where(e => e.Message != args.Message)
                    .Select(e => e.Message)
                    .ToArray();
                var nl = Environment.NewLine;
                logger.Invoke(nl + args.Message +
                              (errors.Any() ? nl + "Errors:" + string.Join(nl, errors) : ""));
            };
            return handler;
        }
    }
}