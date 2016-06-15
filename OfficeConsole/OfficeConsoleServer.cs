using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UaclServer;

namespace OfficeConsole
{
    public class OfficeConsoleServer : InternalServer
    {
        public OfficeConsoleServer() : this("localhost", 48040, "OfficeConsole")
        {
        }

        public OfficeConsoleServer(string ip, int port, string applicationName) : base(ip, port, applicationName)
        {
        }
    }
}
