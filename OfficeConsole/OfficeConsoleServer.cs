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
