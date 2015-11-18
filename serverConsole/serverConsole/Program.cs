using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;

namespace serverConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			ApplicationLicenseManager.AddProcessLicenses(System.Reflection.Assembly.GetExecutingAssembly(), "License.txt");
			var application = new ApplicationInstance();
			ServerManager srv = new ServerManager();
			application.Start(srv, Run, srv);
		}

		static void Run(object state)
		{
			System.Console.WriteLine("Application run");
			System.Console.ReadKey();
		}
    }
}
