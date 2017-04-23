using System;
using System.Threading;
using UaclServer;
using UaclUtils;

namespace ServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            InternalServer server = null;
            try
            {
                ConnectionInfo connection = new ConnectionInfo("localhost", 48030, "ServerConsole");
                server = new InternalServer(connection.Ip, connection.Port, connection.Application);

                server.CreateClient<BusinessLogic>();
                var bo1 = server.CreateClient<BoProxy>();
                server.CreateClient<BusinessLogic>(bo1);

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
