using System.Data;
using System.Text;

namespace SqlToolsNet
{
    public struct SqlProcParameter
    {
        public readonly string ParameterName;
        public readonly SqlDbType ParameterDbType;
        public readonly object Value;

        public SqlProcParameter(string parameterName, SqlDbType parameterDbType, object value)
        {
            ParameterName = parameterName;
            ParameterDbType = parameterDbType;
            Value = value;
        }
    }
    public interface IProcLauncher
    {
        void ExecuteProc(ProcContext ctx);
        void ExecuteProcIfNotAlreadyRunningAsync(ProcContext ctx);
    }
}
