using System;
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
                server.RegisterObject(new BusinessLogic());
                server.Start();

                Logger.Info("Press <enter> to exit the program.");
                Console.ReadLine();
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
