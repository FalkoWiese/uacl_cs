using System;
using UaclServer;
using UaclUtils;

namespace MultiClientConsole
{
    class Program
    {
        static void Main()
        {
            InternalServer server = null;
            try
            {
                server = new MultiClientServer();
                var factory = new UaFactory(server);

                var p = factory.CreateUaObject<MultiClientHost>();
                factory.CreateUaObject<ServerConsoleClient>(p);
                factory.CreateUaObject<ClientConsoleClient>(p);



                if (!server.Start())
                {
                    throw new Exception("Cannot start UA Server without errors!");
                }

                while (true)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, "Error while starting the MultiClientConsole ...");
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