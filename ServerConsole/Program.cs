using System;
using UaclClient;
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

                UaFactory factory = new UaFactory(server);
                var bo = factory.CreateUaObject<BusinessLogic>();
                var bo1 = factory.CreateUaObject<BoProxy>();
                var bo2 = factory.CreateUaObject<BusinessLogic>(bo1);

                server.Start();

                bo.CalculateJob("", 2);
                var count = 0;
		        while (true)
		        {
		            bo.ChangeState($"{count++}");
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
