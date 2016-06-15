using System;
using System.Collections.Generic;
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

        private ServerConsoleServer(string ip, int port, string applicationName) : base(ip, port, applicationName)
        {
        }
    }
}
