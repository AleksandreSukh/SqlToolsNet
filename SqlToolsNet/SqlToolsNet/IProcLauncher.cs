namespace SqlToolsNet
{
    public interface IProcLauncher
    {
        void ExecuteProc(ProcContext ctx);
        void ExecuteProcIfNotAlreadyRunningAsync(ProcContext ctx);
    }
}