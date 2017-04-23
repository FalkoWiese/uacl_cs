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
