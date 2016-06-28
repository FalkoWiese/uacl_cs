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
                var bo = new BusinessLogic();
		        bo.CalculateJob("", 2);
		        server = new InternalServer("localhost", 48030, "ServerConsole");
                server.RegisterObject(bo);

                var bo1 = new BoProxy();
                bo1.Items.Add(new BusinessLogic());
                server.RegisterObject(bo1);

                server.Start();

                RemoteObject ro = new RemoteObject("localhost", 48030, "ServerConsole.BusinessLogic");
		        Action<int> changeStateCount = (int c) =>
		        {
		            ro.Write("BoState", $"{c}");
		        };
		        var count = 0;
		        while (true)
		        {
		            changeStateCount(count++);
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
