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
		        server = new ServerConsoleServer();
                server.RegisterObject(new BusinessLogic());
		        server.RegisterObject(new BoProxy());
                server.Start();

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
