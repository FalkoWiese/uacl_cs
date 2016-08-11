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
                var factory = new UaFactory(server);

                factory.CreateUaObject<BusinessLogic>();
                var boParent = factory.CreateUaObject<RemoteBusinessLogic>();
                factory.CreateUaObject<RemoteBoProxy>(boParent);

                server.Start();

                var rbl = new RemoteBusinessLogic();
                bool isConnected = rbl.Connect();
                Console.Out.WriteLine($"The connection is {isConnected}.");
                var v = rbl.ReadState();
                Console.Out.WriteLine($"Read value is '{v}'.");
                rbl.Subscribe("FireSimpleEvent",
                    (result) => { Logger.Trace($"{result}"); });
                rbl.Invoke("FireSimpleEvent");
                rbl.Disconnect();

                while (true)
                {
                    System.Threading.Thread.Sleep(100);
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
