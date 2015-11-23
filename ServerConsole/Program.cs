using System;
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
            try
            {
                InternalServer server = new InternalServer("localhost", 48043);
                server.Start();

                Console.WriteLine("Press <enter> to exit the program.");
                Console.ReadLine();

                // Stop the server.
                server.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: {0}", e.Message);
                Console.WriteLine("Press <enter> to exit the program.");
                Console.ReadLine();
            }
        }
    }
}
