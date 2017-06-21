using System;
using System.Threading;
using UaclServer;
using UaclUtils;

namespace OfficeConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            InternalServer server = null;
            try
            {
                server = new OfficeConsoleServer();

                var bo = server.CreateClient<BusinessLogic>();
                var bo1 = server.CreateClient<BusinessLogic1>(bo);
                server.CreateClient<BusinessLogic2>(bo1);
                server.CreateClient<RemoteBusinessLogic>();
/*
                var boParent = server.CreateUaObject<RemoteBusinessLogic>();
                server.CreateUaObject<RemoteBoProxy>(boParent);
*/
                server.Start();

                while (true)
                {
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, "Error while starting the OfficeServerConsole ...");
                Logger.Info("Press <enter> to exit the program.");
                Console.ReadLine();
            }
            finally
            {
                // Stop the server.
                server?.Stop();
            }
        }
    }
}
