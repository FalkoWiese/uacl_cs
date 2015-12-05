using System;
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
		        server = new InternalServer("localhost", 48030, "ServerConsole");
                server.RegisterObject(new BusinessLogic());
		        server.Start();

		        Logger.Info("Press <enter> to exit the program.");
		        Console.ReadLine();
		    }
		    catch (Exception e)
		    {
                ExceptionHandler.Log(e, "Press <enter> to exit the program.");
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
