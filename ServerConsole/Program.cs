﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;
using UaclServer;

namespace serverConsole
{
	class Program
	{
		static void Main(string[] args)
		{
		    InternalServer server = null;
		    try
		    {
		        server = new InternalServer("localhost", 48030);
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