using System;

namespace SqlToolsNet
{
    public class ProcRunnerException : Exception
    {
        public ProcRunnerException(string message) : base(message) { }

        public ProcRunnerException(string message, Exception innerException) : base(message, innerException) { }
    }
}