using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UaclServer;

namespace ServerConsole
{
    public class ServerConsoleServer : InternalServer
    {
        public ServerConsoleServer() : this("localhost", 48030, "ServerConsole")
        {
        }

        public ServerConsoleServer(string ip, int port, string applicationName) : base(ip, port, applicationName)
        {
        }
    }
}
