using System;
using UaclServer;

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

		        Console.WriteLine("Press <enter> to exit the program.");
		        Console.ReadLine();
		    }
		    catch (Exception e)
		    {
		        Console.WriteLine("ERROR: {0}", e.Message);
		        Console.WriteLine("Press <enter> to exit the program.");
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
