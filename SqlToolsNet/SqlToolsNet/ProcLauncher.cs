using System;

namespace SqlToolsNet
{
    public class ProcLauncher
    {
        private readonly Action<string> _textLogger;
        private readonly Action<Exception> _exceptionLogger;


        readonly object _singleRunLocker = new object();

        public ProcLauncher(Action<string> textLogger, Action<Exception> exceptionLogger)
        {
            _textLogger = textLogger;
            _exceptionLogger = exceptionLogger;
        }
        /// <summary>
        /// Note! Monitor class is used as locking mechanism. So in order to avoid parallel executions you must use same instance of <see cref="ProcLauncher"/> 
        /// </summary>
        /// <param name="ctx"></param>
        public void ExecuteProcIfNotAlreadyRunningAsync(ProcContext ctx)
        {
            new ProcHelper(_textLogger, _exceptionLogger).ExecuteProcIfNotAlreadyRunningAsync(ctx.ConnectionString, ctx.ProcedureName,
                ctx.Parameters, ctx.UseTransaction, ctx.TimeoutSeconds, _singleRunLocker);
        }

        public void ExecuteProc(ProcContext ctx)
        {
            new ProcHelper(_textLogger, _exceptionLogger).ExecuteProc(ctx.ConnectionString, ctx.ProcedureName, ctx.Parameters, ctx.UseTransaction,
                ctx.TimeoutSeconds);
        }
    }
}