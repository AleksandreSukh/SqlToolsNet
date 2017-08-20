using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlToolsNet;

namespace SampleUsage
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=blabla;etc";
            string procedureName = "blublu";
            bool useTransaction = false;
            int timeoutSeconds = 10;

            /*First we need a context which contains all information for launching stored procedure*/
            var context = new ProcContext(connectionString, procedureName, useTransaction, timeoutSeconds,
                new List<SqlProcParameter>()
                {
                    new SqlProcParameter("@param1", SqlDbType.Int, 4),
                    new SqlProcParameter("@anotherParam", SqlDbType.DateTime, DateTime.Now),
                    new SqlProcParameter("@message", SqlDbType.NVarChar, "Hello World!")
                });
            /*Then we need a launcher which will launch this procedure */
            var launcer = new ProcLauncher(log => Console.WriteLine(log), ex => Console.WriteLine(ex));

            /*Then we can just launch stored procedure */
            //NOTE! it will block current thread in order to wait for its result
            launcer.ExecuteProc(context);

            /*Or you can launch it async so you can do multiple procedure/other task without waiting this one */
            launcer.ExecuteProcIfNotAlreadyRunningAsync(context);
            /* 
             * NOTE! if you will launch multiple procedures with the same instance of ProcLauncher
             * it will wait until previous procedure has finished or timed out
             * It is by design because wery often its necessary to avoid parallel executions of stored procedures
             * because of performance, etc.
             * 
             * In case you still need to run same procedure in parrallel just use new instance of ProcLauncher
             */

            /*Or In case you need to run different procedures and/or using different conneciton 
             * strings (like on multiple databases) you should create new instance of ProcLauncher
             * */
            new ProcLauncher(
                log => Console.WriteLine($"With different logger:{log}"),
                ex => Console.WriteLine($"With different logger:{ex}"))
                .ExecuteProcIfNotAlreadyRunningAsync(
                    new ProcContext("AnotherConnection", "AnotherProcedure",
                    useTransaction: true,
                    timeoutSeconds: 15,
                    parameters: null //In case procedure doesn't have any parameters
                        ));


        }
    }
}
